using System;

namespace DistributedPrintQueue.Shared
{
    /// <summary>
    /// Represents the outcome of a print management operation across Remoting.
    /// Marked [Serializable] for safe cross-domain marshalling.
    /// </summary>
    [Serializable]
    public class PrintOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PrintJob Job { get; set; }
        public string ErrorDetails { get; set; }

        public static PrintOperationResult Ok(string message, PrintJob job = null)
        {
            return new PrintOperationResult
            {
                Success = true,
                Message = message,
                Job = job
            };
        }

        public static PrintOperationResult Fail(string message, string errorDetails = null)
        {
            return new PrintOperationResult
            {
                Success = false,
                Message = message,
                ErrorDetails = errorDetails
            };
        }
    }
}
