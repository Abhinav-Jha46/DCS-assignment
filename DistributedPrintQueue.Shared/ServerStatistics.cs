using System;

namespace DistributedPrintQueue.Shared
{
    /// <summary>
    /// Dashboard telemetry metrics gathered from the centralized print server.
    /// Marked [Serializable] for .NET Remoting cross-domain transport.
    /// </summary>
    [Serializable]
    public class ServerStatistics
    {
        public int TotalJobs { get; set; }
        public int PendingJobs { get; set; } // Queued jobs waiting in queue
        public int PrintingJobs { get; set; }
        public int CompletedJobs { get; set; }
        public int FailedJobs { get; set; }
        public int CancelledJobs { get; set; }
        public int PausedJobs { get; set; }
        public int TotalPagesPrinted { get; set; }
        public int ConnectedClients { get; set; }
        public DateTime ServerStartTime { get; set; }
        public int QueueLength { get; set; }

        public TimeSpan ServerUptime
        {
            get { return DateTime.Now - ServerStartTime; }
        }
    }
}
