using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.TestHarness
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("=== STARTING END-TO-END VERIFICATION SUITE ===");
            try
            {
                // Register Client TCP Channel
                IDictionary props = new Hashtable();
                props["name"] = "tcpTestClient";
                props["timeout"] = 4000;
                TcpChannel channel = new TcpChannel(props, null, null);
                ChannelServices.RegisterChannel(channel, false);

                string serverUrl = "tcp://localhost:9000/RemotePrintManager";
                Console.WriteLine("Connecting to remote object at " + serverUrl);

                IRemotePrintManager server = (IRemotePrintManager)Activator.GetObject(typeof(IRemotePrintManager), serverUrl);

                // 1. Ping / Health check
                Console.WriteLine("\n[TEST 1] Server Ping / Connectivity Test...");
                bool ping = server.Ping();
                Console.WriteLine("  Ping response: " + ping);
                if (!ping) throw new Exception("Ping failed!");

                // 2. Register two client identities
                Console.WriteLine("\n[TEST 2] Multi-client Registration...");
                server.RegisterClient("Student-PC-01");
                server.RegisterClient("Student-PC-02");
                Console.WriteLine("  Registered Client 1: Student-PC-01 and Client 2: Student-PC-02");

                // 3. Clear any existing completed jobs for a clean test run
                foreach (var j in server.GetAllJobs())
                {
                    if (j.Status == PrintJobStatus.Completed || j.Status == PrintJobStatus.Cancelled || j.Status == PrintJobStatus.Failed)
                    {
                        server.DeleteCompletedJob(j.JobId, "TestHarness");
                    }
                }

                // 4. Submit 3 jobs with Low, Normal, High priorities
                Console.WriteLine("\n[TEST 3] Submitting 3 jobs with Low, Normal, High priorities to test Priority Scheduling...");
                byte[] sampleData1 = System.Text.Encoding.UTF8.GetBytes("Low priority document content");
                byte[] sampleData2 = System.Text.Encoding.UTF8.GetBytes("Normal priority document content");
                byte[] sampleData3 = System.Text.Encoding.UTF8.GetBytes("High priority document content");

                // Submit Low first
                var r1 = server.SubmitPrintJob("low_doc.txt", sampleData1, 2, PrintPriority.Low, "Student-PC-01", "Virtual Office Laserjet (Main Floor)");
                Console.WriteLine("  Submitted Low: " + r1.Message + " (JobId: " + r1.Job.JobId.ToString().Substring(0, 8) + ")");

                // Submit Normal second
                var r2 = server.SubmitPrintJob("normal_doc.txt", sampleData2, 2, PrintPriority.Normal, "Student-PC-02", "Virtual Office Laserjet (Main Floor)");
                Console.WriteLine("  Submitted Normal: " + r2.Message + " (JobId: " + r2.Job.JobId.ToString().Substring(0, 8) + ")");

                // Submit High third
                var r3 = server.SubmitPrintJob("urgent_report.txt", sampleData3, 2, PrintPriority.High, "Student-PC-01", "High-Speed Network Duplex Printer (Room 204)");
                Console.WriteLine("  Submitted High: " + r3.Message + " (JobId: " + r3.Job.JobId.ToString().Substring(0, 8) + ")");

                // Check queue positions
                PrintJob[] jobs = server.GetAllJobs();
                Console.WriteLine("\n  Current Queue Snapshot:");
                foreach (var j in jobs)
                {
                    Console.WriteLine("   - {0} | Priority: {1} | Status: {2} | Pos: {3} | Client: {4}",
                        j.FileName, j.Priority, j.Status, j.PositionInQueue, j.ClientName);
                }

                // Verify scheduling:
                // If one started printing immediately (e.g. low_doc because processor grabbed it before high arrived),
                // the REMAINING queued jobs MUST have High priority ahead of Normal/Low priority!
                var queuedJobs = server.GetAllJobs();
                var remainingQueued = System.Array.FindAll(queuedJobs, j => j.Status == PrintJobStatus.Queued);
                if (remainingQueued.Length >= 2)
                {
                    Console.WriteLine("  Checking Queued ordering: Position 1 Priority = " + remainingQueued[0].Priority + ", Position 2 Priority = " + remainingQueued[1].Priority);
                    if (remainingQueued[0].Priority < remainingQueued[1].Priority)
                    {
                        throw new Exception("Priority scheduling assertion failed: Higher priority job is not ordered ahead of lower priority job!");
                    }
                    Console.WriteLine("  VERIFIED: High priority is properly prioritized in the queue!");
                }

                // 5. Test Pause and Resume
                Console.WriteLine("\n[TEST 4] Testing Pause and Resume...");
                // Find a job that is printing or queued
                PrintJob targetToPause = null;
                for (int t = 0; t < 20; t++)
                {
                    var all = server.GetAllJobs();
                    targetToPause = Array.Find(all, j => j.Status == PrintJobStatus.Printing || j.Status == PrintJobStatus.Queued);
                    if (targetToPause != null) break;
                    Thread.Sleep(200);
                }

                if (targetToPause != null)
                {
                    var pRes = server.PauseJob(targetToPause.JobId, "Student-PC-01");
                    Console.WriteLine("  Pause result for '" + targetToPause.FileName + "': " + pRes.Message + " (Success=" + pRes.Success + ")");
                    var afterPause = server.GetJobById(targetToPause.JobId);
                    Console.WriteLine("  Status after pause: " + afterPause.Status);
                    if (afterPause.Status != PrintJobStatus.Paused)
                    {
                        throw new Exception("Pause verification failed! Expected status Paused, got " + afterPause.Status);
                    }

                    Thread.Sleep(500);

                    var resResume = server.ResumeJob(targetToPause.JobId, "Student-PC-01");
                    Console.WriteLine("  Resume result: " + resResume.Message + " (Success=" + resResume.Success + ")");
                    var afterResume = server.GetJobById(targetToPause.JobId);
                    Console.WriteLine("  Status after resume: " + afterResume.Status);
                    if (afterResume.Status != PrintJobStatus.Printing && afterResume.Status != PrintJobStatus.Queued)
                    {
                        throw new Exception("Resume verification failed! Expected status Printing or Queued, got " + afterResume.Status);
                    }
                    Console.WriteLine("  VERIFIED: Pause and Resume work correctly!");
                }

                // 6. Test Cancel
                Console.WriteLine("\n[TEST 5] Testing Cancel Job...");
                byte[] cancelData = System.Text.Encoding.UTF8.GetBytes("Job to be cancelled");
                var cancelJobSubmit = server.SubmitPrintJob("cancel_me.pdf", cancelData, 5, PrintPriority.Low, "Student-PC-02", "Virtual Office Laserjet (Main Floor)");
                Console.WriteLine("  Submitted job to cancel (JobId: " + cancelJobSubmit.Job.JobId.ToString().Substring(0, 8) + ")");
                var cancelRes = server.CancelJob(cancelJobSubmit.Job.JobId, "Student-PC-02");
                Console.WriteLine("  Cancel result: " + cancelRes.Message + " (Success=" + cancelRes.Success + ")");
                var cancelledJob = server.GetJobById(cancelJobSubmit.Job.JobId);
                Console.WriteLine("  Status after cancel: " + cancelledJob.Status);
                if (cancelledJob.Status != PrintJobStatus.Cancelled)
                {
                    throw new Exception("Cancel verification failed! Expected Cancelled, got " + cancelledJob.Status);
                }
                Console.WriteLine("  VERIFIED: Cancel works correctly!");

                // 7. Test Retry
                Console.WriteLine("\n[TEST 6] Testing Retry Job...");
                var retryRes = server.RetryJob(cancelJobSubmit.Job.JobId, "Student-PC-02");
                Console.WriteLine("  Retry result: " + retryRes.Message + " (Success=" + retryRes.Success + ")");
                var retriedJob = server.GetJobById(cancelJobSubmit.Job.JobId);
                Console.WriteLine("  Status after retry: " + retriedJob.Status);
                if (retriedJob.Status != PrintJobStatus.Queued)
                {
                    throw new Exception("Retry verification failed! Expected Queued, got " + retriedJob.Status);
                }
                Console.WriteLine("  VERIFIED: Retry works correctly!");

                // 8. Test Server Statistics
                Console.WriteLine("\n[TEST 7] Testing Server Telemetry & Statistics...");
                var stats = server.GetServerStatistics();
                Console.WriteLine("  Total Jobs: " + stats.TotalJobs);
                Console.WriteLine("  Waiting (Queued): " + stats.PendingJobs);
                Console.WriteLine("  Printing: " + stats.PrintingJobs);
                Console.WriteLine("  Completed: " + stats.CompletedJobs);
                Console.WriteLine("  Total Pages Printed: " + stats.TotalPagesPrinted);
                Console.WriteLine("  Connected Clients: " + stats.ConnectedClients);
                Console.WriteLine("  Server Uptime: " + stats.ServerUptime);
                if (stats.TotalJobs <= 0)
                {
                    throw new Exception("Statistics assertion failed: TotalJobs must be > 0");
                }
                Console.WriteLine("  VERIFIED: Server statistics work correctly!");

                // 9. Test Storage Persistence & Path Traversal Prevention
                Console.WriteLine("\n[TEST 8] Testing Storage Security & Path Traversal Defense...");
                var badResult = server.SubmitPrintJob("../../dangerous_hack.exe", sampleData1, 1, PrintPriority.High, "Attacker", "Printer");
                if (!badResult.Success)
                {
                    Console.WriteLine("  VERIFIED: Path traversal attempt correctly rejected: " + badResult.Message);
                }
                else
                {
                    throw new Exception("Path traversal test failed: Server accepted dangerous path!");
                }

                // 10. Wait for simulation progress on jobs
                Console.WriteLine("\n[TEST 9] Observing simulated printing execution...");
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(1000);
                    var currentJobs = server.GetAllJobs();
                    var activeJob = Array.Find(currentJobs, j => j.Status == PrintJobStatus.Printing);
                    if (activeJob != null)
                    {
                        Console.WriteLine("  [SIMULATION] Printing: " + activeJob.FileName + " | Progress: " + activeJob.ProgressPercentage + "%");
                    }
                    else
                    {
                        Console.WriteLine("  [SIMULATION] Completed / Queue cycle tick");
                    }
                }

                Console.WriteLine("\n=======================================================");
                Console.WriteLine(" >>> ALL 9 END-TO-END VERIFICATION TESTS PASSED! <<<");
                Console.WriteLine("=======================================================");
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[TEST FAILURE] " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                return 1;
            }
        }
    }
}
