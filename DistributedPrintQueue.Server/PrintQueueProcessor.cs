using System;
using System.Threading;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.Server
{
    /// <summary>
    /// Background thread responsible for scheduling and simulating document printing.
    /// Uses a single lock object shared with PrintManager to guarantee thread safety.
    /// Runs as an asynchronous background worker thread (IsBackground = true).
    /// </summary>
    public class PrintQueueProcessor
    {
        private readonly PrintManager _manager;
        private readonly object _queueLock;
        private readonly AutoResetEvent _wakeEvent;
        private Thread _workerThread;
        private volatile bool _isRunning;
        private readonly int _secondsPerPage;

        public PrintQueueProcessor(PrintManager manager, object queueLock, int secondsPerPage = 2)
        {
            _manager = manager;
            _queueLock = queueLock;
            _secondsPerPage = Math.Max(1, secondsPerPage);
            _wakeEvent = new AutoResetEvent(false);
        }

        public void Start()
        {
            _isRunning = true;
            _workerThread = new Thread(ProcessQueueLoop)
            {
                IsBackground = true,
                Name = "PrintQueueProcessorThread"
            };
            _workerThread.Start();
            ServerLogger.System("PrintQueueProcessor background worker thread started.");
        }

        public void Stop()
        {
            _isRunning = false;
            _wakeEvent.Set();
            if (_workerThread != null && _workerThread.IsAlive)
            {
                _workerThread.Join(1500);
            }
        }

        public void NotifyNewJobOrStateChange()
        {
            _wakeEvent.Set();
        }

        private void ProcessQueueLoop()
        {
            while (_isRunning)
            {
                PrintJob targetJob = null;

                lock (_queueLock)
                {
                    // Select next eligible job using priority and FIFO ordering
                    targetJob = _manager.GetNextEligibleJobInternal();
                    if (targetJob != null)
                    {
                        targetJob.Status = PrintJobStatus.Printing;
                        targetJob.StartedAt = DateTime.Now;
                        targetJob.PositionInQueue = 0;
                        _manager.RecalculateQueuePositionsInternal();
                        ServerLogger.Print(string.Format("Started printing Job '{0}' (ID: {1}, {2} pages, Priority: {3}, Client: {4})",
                            targetJob.FileName, targetJob.JobId.ToString().Substring(0, 8), targetJob.PageCount, targetJob.Priority, targetJob.ClientName));
                    }
                }

                if (targetJob != null)
                {
                    ExecutePrintSimulation(targetJob);
                }
                else
                {
                    // Wait for new jobs or state changes (up to 2 seconds timeout)
                    _wakeEvent.WaitOne(2000);
                }
            }
        }

        private void ExecutePrintSimulation(PrintJob job)
        {
            int totalPages = Math.Max(1, job.PageCount);
            int msPerPage = _secondsPerPage * 1000;
            int sliceIntervalMs = 200; // Check state every 200ms

            for (int page = 1; page <= totalPages; page++)
            {
                int elapsedForPage = 0;

                while (elapsedForPage < msPerPage && _isRunning)
                {
                    PrintJobStatus currentStatus;
                    lock (_queueLock)
                    {
                        currentStatus = job.Status;
                    }

                    // Handle Cancellation
                    if (currentStatus == PrintJobStatus.Cancelled)
                    {
                        ServerLogger.Warn(string.Format("Printing cancelled for Job '{0}' at page {1}/{2}.", job.FileName, page, totalPages));
                        return;
                    }

                    // Handle Failure
                    if (currentStatus == PrintJobStatus.Failed)
                    {
                        ServerLogger.Error(string.Format("Printing failed for Job '{0}' at page {1}/{2}.", job.FileName, page, totalPages));
                        return;
                    }

                    // Handle Pause
                    if (currentStatus == PrintJobStatus.Paused)
                    {
                        ServerLogger.Print(string.Format("Job '{0}' paused at page {1}/{2}. Waiting for resume...", job.FileName, page, totalPages));
                        while (_isRunning)
                        {
                            _wakeEvent.WaitOne(500);
                            lock (_queueLock)
                            {
                                currentStatus = job.Status;
                            }
                            if (currentStatus == PrintJobStatus.Cancelled || currentStatus == PrintJobStatus.Failed)
                            {
                                return;
                            }
                            if (currentStatus == PrintJobStatus.Printing)
                            {
                                ServerLogger.Print(string.Format("Job '{0}' resumed. Continuing page {1}/{2}.", job.FileName, page, totalPages));
                                break;
                            }
                        }
                    }

                    Thread.Sleep(sliceIntervalMs);
                    elapsedForPage += sliceIntervalMs;
                }

                // Page completed
                lock (_queueLock)
                {
                    if (job.Status == PrintJobStatus.Cancelled || job.Status == PrintJobStatus.Failed)
                    {
                        return;
                    }

                    job.ProgressPercentage = Math.Min(100, (int)Math.Round((page * 100.0) / totalPages));
                    ServerLogger.Print(string.Format("Job '{0}' [{1}] -> Page {2}/{3} ({4}%)",
                        job.FileName, job.JobId.ToString().Substring(0, 8), page, totalPages, job.ProgressPercentage));
                }
            }

            // Finish Job
            lock (_queueLock)
            {
                if (job.Status == PrintJobStatus.Printing)
                {
                    job.Status = PrintJobStatus.Completed;
                    job.CompletedAt = DateTime.Now;
                    job.ProgressPercentage = 100;
                    _manager.OnJobCompletedInternal(job);
                    ServerLogger.Complete(string.Format("Job '{0}' (ID: {1}) completed printing successfully.",
                        job.FileName, job.JobId.ToString().Substring(0, 8)));
                }
                _manager.RecalculateQueuePositionsInternal();
            }
        }
    }
}
