using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.Server
{
    /// <summary>
    /// Server host program for the Distributed Print Queue Management System.
    /// Sets up .NET Remoting TCP channel with TypeFilterLevel.Full and publishes
    /// the PrintManager Singleton.
    /// </summary>
    class ServerProgram
    {
        private static PrintManager _printManager;
        private static StorageManager _storageManager;
        private static int _port = 9000;
        private static int _secondsPerPage = 2;
        private static int _maxUploadMb = 20;
        private static string _storagePath = "ServerStorage";

        static void Main(string[] args)
        {
            Console.Title = "Distributed Print Queue Server (.NET Remoting TCP:9000)";

            LoadConfiguration();

            // Initialize Storage and Logger
            _storageManager = new StorageManager(_storagePath, _maxUploadMb);
            ServerLogger.Initialize(_storagePath);

            ServerLogger.System("Initializing Distributed Print Queue Server...");

            // Initialize Singleton PrintManager
            _printManager = new PrintManager(_storageManager, _secondsPerPage);

            // Register Remoting Channel and Service
            bool remotingStarted = StartRemoting();
            if (!remotingStarted)
            {
                ServerLogger.Error("CRITICAL: Failed to bind .NET Remoting channel. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            DisplayBanner();

            RunInteractiveConsole();

            // Shutdown cleanup
            ServerLogger.System("Shutting down print server...");
            _printManager.Stop();
        }

        private static void LoadConfiguration()
        {
            try
            {
                string portStr = ConfigurationManager.AppSettings["ServerPort"];
                int p;
                if (!string.IsNullOrEmpty(portStr) && int.TryParse(portStr, out p))
                {
                    _port = p;
                }

                string secStr = ConfigurationManager.AppSettings["SecondsPerPage"];
                int s;
                if (!string.IsNullOrEmpty(secStr) && int.TryParse(secStr, out s))
                {
                    _secondsPerPage = s;
                }

                string maxMbStr = ConfigurationManager.AppSettings["MaxUploadMb"];
                int m;
                if (!string.IsNullOrEmpty(maxMbStr) && int.TryParse(maxMbStr, out m))
                {
                    _maxUploadMb = m;
                }

                string sp = ConfigurationManager.AppSettings["StoragePath"];
                if (!string.IsNullOrEmpty(sp))
                {
                    _storagePath = sp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: Could not read App.config settings: " + ex.Message);
            }
        }

        private static bool StartRemoting()
        {
            bool configSuccess = false;

            // Attempt 1: Config file registration
            try
            {
                string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (File.Exists(configFile))
                {
                    RemotingConfiguration.Configure(configFile, false);
                    configSuccess = true;
                    ServerLogger.System("Remoting channel configured via App.config.");
                }
            }
            catch (Exception ex)
            {
                ServerLogger.Warn("App.config remoting configuration notice: " + ex.Message + ". Falling back to programmatic channel setup.");
            }

            // Attempt 2: Programmatic fallback to guarantee TypeFilterLevel.Full and singleton binding
            try
            {
                bool channelExists = false;
                foreach (IChannel ch in ChannelServices.RegisteredChannels)
                {
                    if (ch is TcpChannel || ch.ChannelName.StartsWith("tcp", StringComparison.OrdinalIgnoreCase))
                    {
                        channelExists = true;
                        break;
                    }
                }

                if (!channelExists)
                {
                    BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider
                    {
                        TypeFilterLevel = TypeFilterLevel.Full
                    };

                    BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();

                    IDictionary properties = new Hashtable();
                    properties["port"] = _port;
                    properties["name"] = "tcp" + _port;

                    TcpChannel tcpChannel = new TcpChannel(properties, clientProvider, serverProvider);
                    ChannelServices.RegisterChannel(tcpChannel, false);
                    ServerLogger.System(string.Format("Programmatic TCP Channel registered on port {0} (TypeFilterLevel: Full).", _port));
                }

                // Publish our singleton instance under URI 'RemotePrintManager'
                RemotingServices.Marshal(_printManager, "RemotePrintManager", typeof(IRemotePrintManager));
                ServerLogger.System("Singleton PrintManager successfully marshaled to endpoint: tcp://localhost:" + _port + "/RemotePrintManager");
                return true;
            }
            catch (Exception ex)
            {
                ServerLogger.Error("Programmatic Remoting registration error: " + ex.Message);
                return configSuccess;
            }
        }

        private static void DisplayBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("===============================================================================");
            Console.WriteLine("             DISTRIBUTED PRINT QUEUE SERVER (.NET REMOTING)                   ");
            Console.WriteLine("===============================================================================");
            Console.ResetColor();
            Console.WriteLine(string.Format(" Storage Directory : {0}", _storageManager.PrintJobsDirectory));
            Console.WriteLine(string.Format(" Remoting Endpoint : tcp://localhost:{0}/RemotePrintManager", _port));
            Console.WriteLine(string.Format(" Print Rate        : {0} sec/page (Simulated Engine)", _secondsPerPage));
            Console.WriteLine(string.Format(" Status            : RUNNING (Waiting for client connections)"));
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(" Commands: [Q] Queue  [S] Stats  [L] Logs  [B] Broadcast  [C] Clear Completed  [X] Exit");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void RunInteractiveConsole()
        {
            bool exit = false;
            while (!exit)
            {
                try
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        switch (key.Key)
                        {
                            case ConsoleKey.Q:
                                ShowQueue();
                                break;
                            case ConsoleKey.S:
                                ShowStatistics();
                                break;
                            case ConsoleKey.L:
                                ShowLogs();
                                break;
                            case ConsoleKey.B:
                                BroadcastMessage();
                                break;
                            case ConsoleKey.C:
                                ClearCompletedJobs();
                                break;
                            case ConsoleKey.X:
                                exit = true;
                                break;
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Console input is redirected or running in non-interactive environment
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        private static void ShowQueue()
        {
            var jobs = _printManager.GetAllJobs();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- CURRENT PRINT QUEUE SNAPSHOT ({0} jobs) ---", jobs.Length);
            Console.ResetColor();
            if (jobs.Length == 0)
            {
                Console.WriteLine("  (Queue is currently empty)");
            }
            else
            {
                Console.WriteLine("{0,-4} | {1,-8} | {2,-20} | {3,-12} | {4,-6} | {5,-8} | {6,-10} | {7,-4}%",
                    "Pos", "Job ID", "File Name", "Client", "Pages", "Priority", "Status", "Prog");
                Console.WriteLine(new string('-', 85));
                foreach (var j in jobs)
                {
                    Console.WriteLine("{0,-4} | {1,-8} | {2,-20} | {3,-12} | {4,-6} | {5,-8} | {6,-10} | {7,-4}%",
                        j.PositionInQueue,
                        j.JobId.ToString().Substring(0, 8),
                        j.FileName.Length > 20 ? j.FileName.Substring(0, 17) + "..." : j.FileName,
                        j.ClientName,
                        j.PageCount,
                        j.Priority,
                        j.Status,
                        j.ProgressPercentage);
                }
            }
            Console.WriteLine();
        }

        private static void ShowStatistics()
        {
            var stats = _printManager.GetServerStatistics();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--- SERVER TELEMETRY & STATISTICS ---");
            Console.ResetColor();
            Console.WriteLine(string.Format(" Total Jobs Submitted : {0}", stats.TotalJobs));
            Console.WriteLine(string.Format(" Jobs Waiting (Queued): {0}", stats.PendingJobs));
            Console.WriteLine(string.Format(" Currently Printing   : {0}", stats.PrintingJobs));
            Console.WriteLine(string.Format(" Successfully Finished: {0}", stats.CompletedJobs));
            Console.WriteLine(string.Format(" Cancelled Jobs       : {0}", stats.CancelledJobs));
            Console.WriteLine(string.Format(" Failed Jobs          : {0}", stats.FailedJobs));
            Console.WriteLine(string.Format(" Paused Jobs          : {0}", stats.PausedJobs));
            Console.WriteLine(string.Format(" Total Pages Printed  : {0}", stats.TotalPagesPrinted));
            Console.WriteLine(string.Format(" Connected Clients    : {0}", stats.ConnectedClients));
            Console.WriteLine(string.Format(" Server Uptime        : {0:hh\\:mm\\:ss}", stats.ServerUptime));
            Console.WriteLine();
        }

        private static void ShowLogs()
        {
            string[] logs = ServerLogger.GetRecentLogs(15);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("--- RECENT SERVER LOG AUDIT (Last {0} entries) ---", logs.Length);
            Console.ResetColor();
            foreach (var log in logs)
            {
                Console.WriteLine("  " + log);
            }
            Console.WriteLine();
        }

        private static void BroadcastMessage()
        {
            Console.WriteLine();
            Console.Write("Enter broadcast announcement: ");
            string message = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(message))
            {
                ServerLogger.Log("BROADCAST", "[SERVER BROADCAST] " + message, ConsoleColor.Yellow);
            }
        }

        private static void ClearCompletedJobs()
        {
            var jobs = _printManager.GetAllJobs();
            int cleared = 0;
            foreach (var job in jobs)
            {
                if (job.Status == PrintJobStatus.Completed || job.Status == PrintJobStatus.Cancelled)
                {
                    _printManager.DeleteCompletedJob(job.JobId, "ServerAdmin");
                    cleared++;
                }
            }
            ServerLogger.System(string.Format("Admin cleared {0} finished/cancelled jobs from server.", cleared));
        }
    }
}
