using System;

namespace DistributedPrintQueue.Shared
{
    /// <summary>
    /// Priority level for print queue ordering.
    /// Higher numeric value indicates higher processing priority.
    /// </summary>
    public enum PrintPriority
    {
        Low = 1,
        Normal = 2,
        High = 3
    }
}
