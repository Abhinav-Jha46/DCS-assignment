using System;

namespace DistributedPrintQueue.Shared
{
    /// <summary>
    /// Lifecycle statuses for print jobs in the queue management system.
    /// Exactly follows Section 6 specification: Queued, Printing, Paused, Completed, Cancelled, Failed.
    /// </summary>
    public enum PrintJobStatus
    {
        Queued,
        Printing,
        Paused,
        Completed,
        Cancelled,
        Failed
    }
}
