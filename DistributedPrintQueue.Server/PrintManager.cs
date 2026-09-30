using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.Server
{
    /// <summary>
    /// Centralized Print Manager implementation running on the Print Server.
    /// Inherits MarshalByRefObject to act as a .NET Remoting remote singleton.
    /// Overrides InitializeLifetimeService() to return null so it never expires.
    /// </summary>
    public class PrintManager : MarshalByRefObject, IRemotePrintManager
    {
        private readonly object _queueLock = new object();
        private readonly List<PrintJob> _jobs = new List<PrintJob>();
        private readonly StorageManager _storageManager;
        private readonly PrintQueueProcessor _processor;
        private readonly DateTime _startTime;
        private readonly HashSet<string> _connectedClients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private int _totalPagesPrinted = 0;

        private static readonly string[] AvailablePrintersList = new[]
        {
            "Virtual Office Laserjet (Main Floor)",
            "High-Speed Network Duplex Printer (Room 204)",
            "Engineering Dept Color Plotter",
            "Departmental PDF Spooler"
        };

        public PrintManager() : this(new StorageManager("ServerStorage", 20), 2)
        {
        }

        public PrintManager(StorageManager storageManager, int secondsPerPage = 2)
        {
            _storageManager = storageManager;
            _startTime = DateTime.Now;
            _processor = new PrintQueueProcessor(this, _queueLock, secondsPerPage);
            _processor.Start();
        }

        /// <summary>
        /// Lifetime service override returning null to guarantee the Remoting singleton
        /// object remains alive indefinitely and does not get garbage collected.
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Stop()
        {
            _processor.Stop();
        }

        public bool Ping()
        {
            return true;
        }

        public void RegisterClient(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName)) return;

            lock (_queueLock)
            {
                if (!_connectedClients.Contains(clientName))
                {
                    _connectedClients.Add(clientName);
                    ServerLogger.Connect(string.Format("Client connected and registered: '{0}' (Total clients: {1})", clientName, _connectedClients.Count));
                }
            }
        }

        public PrintOperationResult SubmitPrintJob(
            string fileName,
            byte[] fileData,
            int pageCount,
            PrintPriority priority,
            string clientName,
            string printerName)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(clientName))
                {
                    clientName = "Anonymous-Client";
                }
                RegisterClient(clientName);

                if (pageCount <= 0 || pageCount > 1000)
                {
                    return PrintOperationResult.Fail("Page count must be between 1 and 1000 pages.");
                }

                if (string.IsNullOrWhiteSpace(printerName))
                {
                    printerName = AvailablePrintersList[0];
                }

                Guid jobId = Guid.NewGuid();

                // Validate and persist file to ServerStorage/PrintJobs
                _storageManager.SaveJobFile(jobId, fileName, fileData);

                PrintJob job = new PrintJob
                {
                    JobId = jobId,
                    FileName = Path.GetFileName(fileName),
                    ClientName = clientName,
                    PageCount = pageCount,
                    Priority = priority,
                    Status = PrintJobStatus.Queued,
                    ProgressPercentage = 0,
                    SubmittedAt = DateTime.Now,
                    PrinterName = printerName,
                    FileSize = fileData.Length
                };

                lock (_queueLock)
                {
                    _jobs.Add(job);
                    RecalculateQueuePositionsInternal();
                }

                ServerLogger.Queue(string.Format("New job '{0}' (ID: {1}) added by '{2}'. Priority: {3}, Pages: {4}, Queue Pos: {5}",
                    job.FileName, job.JobId.ToString().Substring(0, 8), job.ClientName, job.Priority, job.PageCount, job.PositionInQueue));

                _processor.NotifyNewJobOrStateChange();

                return PrintOperationResult.Ok("Print job submitted successfully.", CloneJob(job));
            }
            catch (ArgumentException ex)
            {
                ServerLogger.Warn("Submission validation rejected: " + ex.Message);
                return PrintOperationResult.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                ServerLogger.Error("Unexpected error during job submission: " + ex.Message);
                return PrintOperationResult.Fail("Failed to submit print job: " + ex.Message);
            }
        }

        public PrintJob[] GetAllJobs()
        {
            lock (_queueLock)
            {
                RecalculateQueuePositionsInternal();
                return _jobs
                    .OrderBy(j => GetJobSortOrder(j))
                    .ThenBy(j => j.PositionInQueue)
                    .ThenByDescending(j => j.SubmittedAt)
                    .Select(CloneJob)
                    .ToArray();
            }
        }

        public PrintJob[] GetMyJobs(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
            {
                return GetAllJobs();
            }

            lock (_queueLock)
            {
                RecalculateQueuePositionsInternal();
                return _jobs
                    .Where(j => string.Equals(j.ClientName, clientName, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(j => GetJobSortOrder(j))
                    .ThenBy(j => j.PositionInQueue)
                    .ThenByDescending(j => j.SubmittedAt)
                    .Select(CloneJob)
                    .ToArray();
            }
        }

        private static int GetJobSortOrder(PrintJob job)
        {
            switch (job.Status)
            {
                case PrintJobStatus.Printing: return 1;
                case PrintJobStatus.Queued: return 2;
                case PrintJobStatus.Paused: return 3;
                default: return 4;
            }
        }

        public PrintJob GetJobById(Guid jobId)
        {
            lock (_queueLock)
            {
                PrintJob job = _jobs.FirstOrDefault(j => j.JobId == jobId);
                return job != null ? CloneJob(job) : null;
            }
        }

        public PrintOperationResult CancelJob(Guid jobId, string clientName)
        {
            lock (_queueLock)
            {
                PrintJob job = _jobs.FirstOrDefault(j => j.JobId == jobId);
                if (job == null)
                {
                    return PrintOperationResult.Fail("Job not found.");
                }

                if (job.Status == PrintJobStatus.Completed)
                {
                    return PrintOperationResult.Fail("Job cannot be cancelled because printing is already complete.");
                }

                if (job.Status == PrintJobStatus.Cancelled)
                {
                    return PrintOperationResult.Fail("Job is already cancelled.");
                }

                if (job.Status == PrintJobStatus.Failed)
                {
                    return PrintOperationResult.Fail("Job has already failed.");
                }

                job.Status = PrintJobStatus.Cancelled;
                job.ErrorMessage = string.Format("Cancelled by {0} at {1:HH:mm:ss}", clientName, DateTime.Now);
                job.PositionInQueue = 0;
                RecalculateQueuePositionsInternal();

                ServerLogger.Warn(string.Format("Job '{0}' (ID: {1}) was cancelled by '{2}'.",
                    job.FileName, job.JobId.ToString().Substring(0, 8), clientName));

                _processor.NotifyNewJobOrStateChange();
                return PrintOperationResult.Ok("Print job cancelled successfully.", CloneJob(job));
            }
        }

        public PrintOperationResult PauseJob(Guid jobId, string clientName)
        {
            lock (_queueLock)
            {
                PrintJob job = _jobs.FirstOrDefault(j => j.JobId == jobId);
                if (job == null)
                {
                    return PrintOperationResult.Fail("Job not found.");
                }

                if (job.Status == PrintJobStatus.Completed)
                {
                    return PrintOperationResult.Fail("Cannot pause a completed job.");
                }

                if (job.Status == PrintJobStatus.Cancelled || job.Status == PrintJobStatus.Failed)
                {
                    return PrintOperationResult.Fail("Cannot pause a cancelled or failed job.");
                }

                if (job.Status == PrintJobStatus.Paused)
                {
                    return PrintOperationResult.Fail("Job is already paused.");
                }

                job.Status = PrintJobStatus.Paused;
                ServerLogger.Log("PAUSE", string.Format("Job '{0}' (ID: {1}) paused by '{2}'.",
                    job.FileName, job.JobId.ToString().Substring(0, 8), clientName), ConsoleColor.Yellow);

                _processor.NotifyNewJobOrStateChange();
                return PrintOperationResult.Ok("Print job paused.", CloneJob(job));
            }
        }

        public PrintOperationResult ResumeJob(Guid jobId, string clientName)
        {
            lock (_queueLock)
            {
                PrintJob job = _jobs.FirstOrDefault(j => j.JobId == jobId);
                if (job == null)
                {
                    return PrintOperationResult.Fail("Job not found.");
                }

                if (job.Status != PrintJobStatus.Paused)
                {
                    return PrintOperationResult.Fail("Only paused jobs can be resumed.");
                }

                // If printing had already started, resume to Printing; otherwise return to Queued
                job.Status = (job.StartedAt.HasValue && job.ProgressPercentage > 0)
                    ? PrintJobStatus.Printing
                    : PrintJobStatus.Queued;

                RecalculateQueuePositionsInternal();

                ServerLogger.Log("RESUME", string.Format("Job '{0}' (ID: {1}) resumed by '{2}' (Status: {3}).",
                    job.FileName, job.JobId.ToString().Substring(0, 8), clientName, job.Status), ConsoleColor.Green);

                _processor.NotifyNewJobOrStateChange();
                return PrintOperationResult.Ok("Print job resumed.", CloneJob(job));
            }
        }

        public PrintOperationResult RetryJob(Guid jobId, string clientName)
        {
            lock (_queueLock)
            {
                PrintJob job = _jobs.FirstOrDefault(j => j.JobId == jobId);
                if (job == null)
                {
                    return PrintOperationResult.Fail("Job not found.");
                }

                if (job.Status != PrintJobStatus.Failed && job.Status != PrintJobStatus.Cancelled)
                {
                    return PrintOperationResult.Fail("Only failed or cancelled jobs can be retried.");
                }

                job.Status = PrintJobStatus.Queued;
                job.ProgressPercentage = 0;
                job.StartedAt = null;
                job.CompletedAt = null;
                job.ErrorMessage = null;
                job.SubmittedAt = DateTime.Now; // Re-enter queue at current timestamp

                RecalculateQueuePositionsInternal();

                ServerLogger.Log("RETRY", string.Format("Job '{0}' (ID: {1}) re-queued by '{2}'.",
                    job.FileName, job.JobId.ToString().Substring(0, 8), clientName), ConsoleColor.Cyan);

                _processor.NotifyNewJobOrStateChange();
                return PrintOperationResult.Ok("Print job re-queued for processing.", CloneJob(job));
            }
        }

        public PrintOperationResult DeleteCompletedJob(Guid jobId, string clientName)
        {
            lock (_queueLock)
            {
                PrintJob job = _jobs.FirstOrDefault(j => j.JobId == jobId);
                if (job == null)
                {
                    return PrintOperationResult.Fail("Job not found.");
                }

                if (job.Status == PrintJobStatus.Printing || job.Status == PrintJobStatus.Queued || job.Status == PrintJobStatus.Paused)
                {
                    return PrintOperationResult.Fail("Cannot delete an active or queued job. Cancel or wait for it to complete first.");
                }

                _jobs.Remove(job);
                _storageManager.DeleteJobFile(job.JobId, job.FileName);
                RecalculateQueuePositionsInternal();

                ServerLogger.Log("DELETE", string.Format("Job '{0}' (ID: {1}) deleted by '{2}'.",
                    job.FileName, job.JobId.ToString().Substring(0, 8), clientName), ConsoleColor.DarkYellow);

                return PrintOperationResult.Ok("Job deleted successfully.");
            }
        }

        public ServerStatistics GetServerStatistics()
        {
            lock (_queueLock)
            {
                int total = _jobs.Count;
                int queued = _jobs.Count(j => j.Status == PrintJobStatus.Queued);
                int printing = _jobs.Count(j => j.Status == PrintJobStatus.Printing);
                int completed = _jobs.Count(j => j.Status == PrintJobStatus.Completed);
                int failed = _jobs.Count(j => j.Status == PrintJobStatus.Failed);
                int cancelled = _jobs.Count(j => j.Status == PrintJobStatus.Cancelled);
                int paused = _jobs.Count(j => j.Status == PrintJobStatus.Paused);

                return new ServerStatistics
                {
                    TotalJobs = total,
                    PendingJobs = queued,
                    PrintingJobs = printing,
                    CompletedJobs = completed,
                    FailedJobs = failed,
                    CancelledJobs = cancelled,
                    PausedJobs = paused,
                    TotalPagesPrinted = _totalPagesPrinted,
                    ConnectedClients = _connectedClients.Count,
                    ServerStartTime = _startTime,
                    QueueLength = queued
                };
            }
        }

        public string[] GetAvailablePrinters()
        {
            return (string[])AvailablePrintersList.Clone();
        }

        public string[] GetRecentLogs(int maxCount)
        {
            return ServerLogger.GetRecentLogs(maxCount);
        }

        public int GetJobProgress(Guid jobId)
        {
            lock (_queueLock)
            {
                PrintJob job = _jobs.FirstOrDefault(j => j.JobId == jobId);
                return job != null ? job.ProgressPercentage : 0;
            }
        }

        public int GetPendingJobCount()
        {
            lock (_queueLock)
            {
                return _jobs.Count(j => j.Status == PrintJobStatus.Queued);
            }
        }

        #region Internal Methods (called under _queueLock)

        internal PrintJob GetNextEligibleJobInternal()
        {
            // If any job is actively Printing, do not start another (single print engine simulation)
            if (_jobs.Any(j => j.Status == PrintJobStatus.Printing))
            {
                return null;
            }

            // Order by Priority descending (High -> Normal -> Low), then FIFO SubmittedAt ascending
            return _jobs
                .Where(j => j.Status == PrintJobStatus.Queued)
                .OrderByDescending(j => (int)j.Priority)
                .ThenBy(j => j.SubmittedAt)
                .FirstOrDefault();
        }

        internal void RecalculateQueuePositionsInternal()
        {
            var queuedJobs = _jobs
                .Where(j => j.Status == PrintJobStatus.Queued)
                .OrderByDescending(j => (int)j.Priority)
                .ThenBy(j => j.SubmittedAt)
                .ToList();

            for (int i = 0; i < queuedJobs.Count; i++)
            {
                queuedJobs[i].PositionInQueue = i + 1;
            }

            foreach (var job in _jobs.Where(j => j.Status != PrintJobStatus.Queued))
            {
                job.PositionInQueue = 0;
            }
        }

        internal void OnJobCompletedInternal(PrintJob job)
        {
            _totalPagesPrinted += job.PageCount;
        }

        private static PrintJob CloneJob(PrintJob source)
        {
            if (source == null) return null;
            return new PrintJob
            {
                JobId = source.JobId,
                FileName = source.FileName,
                ClientName = source.ClientName,
                PageCount = source.PageCount,
                Priority = source.Priority,
                Status = source.Status,
                ProgressPercentage = source.ProgressPercentage,
                SubmittedAt = source.SubmittedAt,
                StartedAt = source.StartedAt,
                CompletedAt = source.CompletedAt,
                PrinterName = source.PrinterName,
                ErrorMessage = source.ErrorMessage,
                FileSize = source.FileSize,
                PositionInQueue = source.PositionInQueue
            };
        }

        #endregion
    }
}
