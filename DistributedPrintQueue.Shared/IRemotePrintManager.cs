using System;

namespace DistributedPrintQueue.Shared
{
    /// <summary>
    /// Contract defining remote operations exposed by the centralized print server
    /// through .NET Remoting over TCP.
    /// The client references ONLY this interface and shared DTOs.
    /// </summary>
    public interface IRemotePrintManager
    {
        /// <summary>
        /// Lightweight connection and health check ping.
        /// </summary>
        bool Ping();

        /// <summary>
        /// Registers a connected client name for telemetry tracking.
        /// </summary>
        void RegisterClient(string clientName);

        /// <summary>
        /// Submits a document to the centralized queue for simulated printing.
        /// </summary>
        PrintOperationResult SubmitPrintJob(
            string fileName,
            byte[] fileData,
            int pageCount,
            PrintPriority priority,
            string clientName,
            string printerName);

        /// <summary>
        /// Retrieves snapshot of all jobs in the system.
        /// </summary>
        PrintJob[] GetAllJobs();

        /// <summary>
        /// Retrieves jobs submitted by a specific client.
        /// </summary>
        PrintJob[] GetMyJobs(string clientName);

        /// <summary>
        /// Looks up detailed state of a specific print job.
        /// </summary>
        PrintJob GetJobById(Guid jobId);

        /// <summary>
        /// Requests cancellation of a queued or paused print job.
        /// </summary>
        PrintOperationResult CancelJob(Guid jobId, string clientName);

        /// <summary>
        /// Pauses an actively printing or queued job.
        /// </summary>
        PrintOperationResult PauseJob(Guid jobId, string clientName);

        /// <summary>
        /// Resumes a previously paused job back to active processing.
        /// </summary>
        PrintOperationResult ResumeJob(Guid jobId, string clientName);

        /// <summary>
        /// Resubmits a failed job back into the print queue.
        /// </summary>
        PrintOperationResult RetryJob(Guid jobId, string clientName);

        /// <summary>
        /// Removes a completed, failed, or cancelled job from the queue and storage.
        /// </summary>
        PrintOperationResult DeleteCompletedJob(Guid jobId, string clientName);

        /// <summary>
        /// Fetches aggregate system performance and queue metrics.
        /// </summary>
        ServerStatistics GetServerStatistics();

        /// <summary>
        /// Lists available virtual printers configured on the print server.
        /// </summary>
        string[] GetAvailablePrinters();

        /// <summary>
        /// Retrieves safe recent server audit log entries for client monitoring.
        /// </summary>
        string[] GetRecentLogs(int maxCount);

        /// <summary>
        /// Fetches the current progress percentage (0-100) for a job.
        /// </summary>
        int GetJobProgress(Guid jobId);

        /// <summary>
        /// Returns the count of jobs currently waiting in Queued state.
        /// </summary>
        int GetPendingJobCount();
    }
}
