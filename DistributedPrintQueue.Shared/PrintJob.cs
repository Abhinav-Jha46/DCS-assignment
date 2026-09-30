using System;

namespace DistributedPrintQueue.Shared
{
    /// <summary>
    /// Represents a print job DTO that crosses the .NET Remoting boundary.
    /// Marked [Serializable] so it can be marshaled by value to clients.
    /// Contains no MarshalByRefObject references.
    /// </summary>
    [Serializable]
    public class PrintJob
    {
        public Guid JobId { get; set; }
        public string FileName { get; set; }
        public string ClientName { get; set; }
        public int PageCount { get; set; }
        public PrintPriority Priority { get; set; }
        public PrintJobStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string PrinterName { get; set; }
        public string ErrorMessage { get; set; }
        public long FileSize { get; set; }
        public int PositionInQueue { get; set; }

        public PrintJob()
        {
            JobId = Guid.NewGuid();
            Status = PrintJobStatus.Queued;
            ProgressPercentage = 0;
            SubmittedAt = DateTime.Now;
            Priority = PrintPriority.Normal;
            PrinterName = "Default Virtual Printer";
        }
    }
}
