using System;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.Client
{
    /// <summary>
    /// Encapsulates .NET Remoting TCP communication with the Print Server.
    /// Handles channel registration, timeout configuration, and exception isolation.
    /// The client references ONLY IRemotePrintManager and shared DTOs.
    /// </summary>
    public class ClientService
    {
        private IRemotePrintManager _remoteManager;
        private TcpChannel _channel;
        private static bool _channelRegistered = false;
        private static readonly object _channelLock = new object();

        public string Host { get; private set; }
        public int Port { get; private set; }
        public string ClientName { get; private set; }

        public bool IsConnected
        {
            get { return _remoteManager != null; }
        }

        public void Connect(string host, int port, string clientName)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Server host cannot be empty.");
            }
            if (port <= 0 || port > 65535)
            {
                throw new ArgumentException("Port must be between 1 and 65535.");
            }
            if (string.IsNullOrWhiteSpace(clientName))
            {
                clientName = "Student-PC-" + Environment.MachineName;
            }

            Host = host.Trim();
            Port = port;
            ClientName = clientName.Trim();

            EnsureChannelRegistered();

            string endpointUrl = string.Format("tcp://{0}:{1}/RemotePrintManager", Host, Port);

            try
            {
                // Obtain remote proxy reference from .NET Remoting infrastructure
                _remoteManager = (IRemotePrintManager)Activator.GetObject(typeof(IRemotePrintManager), endpointUrl);

                if (_remoteManager == null)
                {
                    throw new RemotingException("Failed to instantiate remote proxy for endpoint " + endpointUrl);
                }

                // Verify connectivity with ping test
                bool pingOk = _remoteManager.Ping();
                if (!pingOk)
                {
                    _remoteManager = null;
                    throw new RemotingException("Server responded with false to ping test.");
                }

                // Register client identity with server
                _remoteManager.RegisterClient(ClientName);
            }
            catch (SocketException ex)
            {
                _remoteManager = null;
                throw new Exception("Unable to connect to the print server. Ensure the server is running on " + endpointUrl + " and check firewall settings.", ex);
            }
            catch (RemotingException ex)
            {
                _remoteManager = null;
                throw new Exception("Remoting communication failure: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _remoteManager = null;
                throw new Exception("Print server is unavailable: " + ex.Message, ex);
            }
        }

        public void Disconnect()
        {
            _remoteManager = null;
        }

        private void EnsureChannelRegistered()
        {
            lock (_channelLock)
            {
                if (!_channelRegistered)
                {
                    try
                    {
                        IDictionary props = new Hashtable();
                        props["name"] = "tcpClient_" + Guid.NewGuid().ToString("N");
                        props["timeout"] = 3000; // 3000ms connection timeout to prevent hangs
                        props["connectionTimeout"] = 3000;

                        _channel = new TcpChannel(props, null, null);
                        ChannelServices.RegisterChannel(_channel, false);
                        _channelRegistered = true;
                    }
                    catch (RemotingException)
                    {
                        // Channel might already be registered in process
                        _channelRegistered = true;
                    }
                }
            }
        }

        private void CheckConnection()
        {
            if (_remoteManager == null)
            {
                throw new InvalidOperationException("Not connected to print server. Please connect first.");
            }
        }

        public PrintOperationResult SubmitPrintJob(string fileName, byte[] fileData, int pageCount, PrintPriority priority, string printerName)
        {
            CheckConnection();
            try
            {
                return _remoteManager.SubmitPrintJob(fileName, fileData, pageCount, priority, ClientName, printerName);
            }
            catch (SocketException ex)
            {
                return PrintOperationResult.Fail("Server connection lost while submitting job: " + ex.Message);
            }
            catch (RemotingException ex)
            {
                return PrintOperationResult.Fail("Remoting exception during submission: " + ex.Message);
            }
            catch (Exception ex)
            {
                return PrintOperationResult.Fail("Submission failed: " + ex.Message);
            }
        }

        public PrintJob[] GetAllJobs()
        {
            CheckConnection();
            try
            {
                return _remoteManager.GetAllJobs();
            }
            catch (Exception)
            {
                return new PrintJob[0];
            }
        }

        public PrintJob[] GetMyJobs()
        {
            CheckConnection();
            try
            {
                return _remoteManager.GetMyJobs(ClientName);
            }
            catch (Exception)
            {
                return new PrintJob[0];
            }
        }

        public PrintJob GetJobById(Guid jobId)
        {
            CheckConnection();
            try
            {
                return _remoteManager.GetJobById(jobId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PrintOperationResult CancelJob(Guid jobId)
        {
            CheckConnection();
            try
            {
                return _remoteManager.CancelJob(jobId, ClientName);
            }
            catch (Exception ex)
            {
                return PrintOperationResult.Fail("Failed to cancel job: " + ex.Message);
            }
        }

        public PrintOperationResult PauseJob(Guid jobId)
        {
            CheckConnection();
            try
            {
                return _remoteManager.PauseJob(jobId, ClientName);
            }
            catch (Exception ex)
            {
                return PrintOperationResult.Fail("Failed to pause job: " + ex.Message);
            }
        }

        public PrintOperationResult ResumeJob(Guid jobId)
        {
            CheckConnection();
            try
            {
                return _remoteManager.ResumeJob(jobId, ClientName);
            }
            catch (Exception ex)
            {
                return PrintOperationResult.Fail("Failed to resume job: " + ex.Message);
            }
        }

        public PrintOperationResult RetryJob(Guid jobId)
        {
            CheckConnection();
            try
            {
                return _remoteManager.RetryJob(jobId, ClientName);
            }
            catch (Exception ex)
            {
                return PrintOperationResult.Fail("Failed to retry job: " + ex.Message);
            }
        }

        public PrintOperationResult DeleteCompletedJob(Guid jobId)
        {
            CheckConnection();
            try
            {
                return _remoteManager.DeleteCompletedJob(jobId, ClientName);
            }
            catch (Exception ex)
            {
                return PrintOperationResult.Fail("Failed to delete completed job: " + ex.Message);
            }
        }

        public ServerStatistics GetServerStatistics()
        {
            CheckConnection();
            try
            {
                return _remoteManager.GetServerStatistics();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string[] GetAvailablePrinters()
        {
            CheckConnection();
            try
            {
                return _remoteManager.GetAvailablePrinters();
            }
            catch (Exception)
            {
                return new[] { "Default Virtual Printer" };
            }
        }

        public string[] GetRecentLogs(int maxCount)
        {
            CheckConnection();
            try
            {
                return _remoteManager.GetRecentLogs(maxCount);
            }
            catch (Exception)
            {
                return new string[0];
            }
        }
    }
}
