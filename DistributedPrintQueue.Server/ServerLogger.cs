using System;
using System.Collections.Generic;
using System.IO;

namespace DistributedPrintQueue.Server
{
    /// <summary>
    /// Thread-safe logger for server events, writing to console, file, and in-memory ring buffer.
    /// </summary>
    public static class ServerLogger
    {
        private static readonly object _logLock = new object();
        private static string _logFilePath = "ServerStorage/server.log";
        private static readonly List<string> _recentLogs = new List<string>();
        private const int MaxLogHistory = 200;

        public static void Initialize(string storagePath)
        {
            lock (_logLock)
            {
                try
                {
                    if (!Directory.Exists(storagePath))
                    {
                        Directory.CreateDirectory(storagePath);
                    }
                    _logFilePath = Path.Combine(storagePath, "server.log");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[LOGGER INIT ERROR] " + ex.Message);
                }
            }
        }

        public static void Log(string tag, string message, ConsoleColor color = ConsoleColor.Gray)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string entry = string.Format("[{0}] [{1}] {2}", timestamp, tag.ToUpper(), message);

            lock (_logLock)
            {
                // Console output
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[" + timestamp + "] ");
                Console.ForegroundColor = color;
                Console.Write("[" + tag.ToUpper() + "] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
                Console.ForegroundColor = oldColor;

                // Memory buffer
                _recentLogs.Add(entry);
                if (_recentLogs.Count > MaxLogHistory)
                {
                    _recentLogs.RemoveAt(0);
                }

                // File append
                try
                {
                    File.AppendAllText(_logFilePath, entry + Environment.NewLine);
                }
                catch
                {
                    // Fail-safe file logging, never crash server on disk glitch
                }
            }
        }

        public static void Info(string message) { Log("INFO", message, ConsoleColor.Cyan); }
        public static void System(string message) { Log("SYSTEM", message, ConsoleColor.Green); }
        public static void Connect(string message) { Log("CONNECT", message, ConsoleColor.Yellow); }
        public static void Queue(string message) { Log("QUEUE", message, ConsoleColor.Magenta); }
        public static void Print(string message) { Log("PRINT", message, ConsoleColor.White); }
        public static void Complete(string message) { Log("COMPLETE", message, ConsoleColor.Green); }
        public static void Warn(string message) { Log("WARN", message, ConsoleColor.Yellow); }
        public static void Error(string message) { Log("ERROR", message, ConsoleColor.Red); }

        public static string[] GetRecentLogs(int maxCount)
        {
            lock (_logLock)
            {
                int count = Math.Min(maxCount, _recentLogs.Count);
                if (count <= 0) return new string[0];
                int start = _recentLogs.Count - count;
                return _recentLogs.GetRange(start, count).ToArray();
            }
        }
    }
}
