using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.Client
{
    public partial class MainForm : Form
    {
        private readonly ClientService _service;
        private bool _isUpdating = false;

        public MainForm()
        {
            InitializeComponent();
            _service = new ClientService();
            cboFilter.SelectedIndex = 0; // All Jobs

            // Parse initial command line arguments if provided
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
            {
                txtClientName.Text = args[1].Trim();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            int port;
            if (!int.TryParse(txtPort.Text.Trim(), out port) || port <= 0 || port > 65535)
            {
                MessageBox.Show(this, "Please enter a valid TCP port number (1-65535).", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string clientName = txtClientName.Text.Trim();
            if (string.IsNullOrWhiteSpace(clientName))
            {
                MessageBox.Show(this, "Please enter a client identification name.", "Missing Client Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                _service.Connect(host, port, clientName);

                SetConnectionUiState(true);
                RefreshData();
                timerRefresh.Start();
                statLabelStatus.Text = string.Format("Status: Connected to tcp://{0}:{1}/RemotePrintManager", host, port);
            }
            catch (Exception ex)
            {
                SetConnectionUiState(false);
                MessageBox.Show(this, ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            timerRefresh.Stop();
            _service.Disconnect();
            SetConnectionUiState(false);
            dgvQueue.Rows.Clear();
            ResetDashboard();
            statLabelStatus.Text = "Status: Disconnected";
            statLabelQueueCount.Text = "Queue: 0 items";
        }

        private void SetConnectionUiState(bool connected)
        {
            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;
            txtHost.ReadOnly = connected;
            txtPort.ReadOnly = connected;
            txtClientName.ReadOnly = connected;

            btnSubmit.Enabled = connected;
            btnRefresh.Enabled = connected;
            btnCancel.Enabled = connected;
            btnPause.Enabled = connected;
            btnResume.Enabled = connected;
            btnRetry.Enabled = connected;
            btnDeleteCompleted.Enabled = connected;
            btnDetails.Enabled = connected;
            cboFilter.Enabled = connected;

            if (connected)
            {
                lblStatusDot.ForeColor = Color.LimeGreen;
                lblConnectionStatus.Text = string.Format("Connected ({0})", _service.ClientName);
                lblConnectionStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatusDot.ForeColor = Color.Red;
                lblConnectionStatus.Text = "Disconnected";
                lblConnectionStatus.ForeColor = Color.DimGray;
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (!_service.IsConnected || _isUpdating) return;
            RefreshData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            if (!_service.IsConnected) return;

            try
            {
                _isUpdating = true;

                // 1. Fetch print jobs
                PrintJob[] jobs;
                if (cboFilter.SelectedIndex == 1) // My Jobs
                {
                    jobs = _service.GetMyJobs();
                }
                else
                {
                    jobs = _service.GetAllJobs();
                }

                UpdateQueueGrid(jobs);

                // 2. Fetch server statistics
                ServerStatistics stats = _service.GetServerStatistics();
                if (stats != null)
                {
                    UpdateStatistics(stats);
                }

                statLabelLastUpdated.Text = "Last Updated: " + DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                // Remoting connection severed
                statLabelStatus.Text = "Status: Connection interrupted - " + ex.Message;
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateQueueGrid(PrintJob[] jobs)
        {
            Guid? selectedJobId = GetSelectedJobId();

            dgvQueue.SuspendLayout();

            // Match row count
            while (dgvQueue.Rows.Count > jobs.Length)
            {
                dgvQueue.Rows.RemoveAt(dgvQueue.Rows.Count - 1);
            }
            while (dgvQueue.Rows.Count < jobs.Length)
            {
                dgvQueue.Rows.Add();
            }

            int selectedIndex = -1;

            for (int i = 0; i < jobs.Length; i++)
            {
                PrintJob j = jobs[i];
                DataGridViewRow row = dgvQueue.Rows[i];
                row.Tag = j.JobId;

                row.Cells[0].Value = j.PositionInQueue > 0 ? j.PositionInQueue.ToString() : "-";
                row.Cells[1].Value = j.JobId.ToString().Substring(0, 8);
                row.Cells[2].Value = j.FileName;
                row.Cells[3].Value = j.ClientName;
                row.Cells[4].Value = j.PageCount;
                row.Cells[5].Value = j.Priority.ToString();
                row.Cells[6].Value = j.Status.ToString();
                row.Cells[7].Value = j.ProgressPercentage + "%";
                row.Cells[8].Value = j.SubmittedAt.ToString("HH:mm:ss");
                row.Cells[9].Value = j.PrinterName;

                // Color code rows by status
                switch (j.Status)
                {
                    case PrintJobStatus.Printing:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(235, 248, 255);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(0, 102, 204);
                        break;
                    case PrintJobStatus.Paused:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 253, 230);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(180, 100, 0);
                        break;
                    case PrintJobStatus.Completed:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(0, 128, 0);
                        break;
                    case PrintJobStatus.Cancelled:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(178, 34, 34);
                        break;
                    case PrintJobStatus.Failed:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(139, 0, 0);
                        break;
                    default:
                        row.DefaultCellStyle.BackColor = Color.White;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                        break;
                }

                if (selectedJobId.HasValue && j.JobId == selectedJobId.Value)
                {
                    selectedIndex = i;
                }
            }

            if (selectedIndex >= 0 && selectedIndex < dgvQueue.Rows.Count)
            {
                dgvQueue.Rows[selectedIndex].Selected = true;
            }

            dgvQueue.ResumeLayout();

            statLabelQueueCount.Text = string.Format("Total Jobs in View: {0} ({1} Queued, {2} Printing)",
                jobs.Length,
                jobs.Count(j => j.Status == PrintJobStatus.Queued),
                jobs.Count(j => j.Status == PrintJobStatus.Printing));
        }

        private void UpdateStatistics(ServerStatistics stats)
        {
            lblStatsTotal.Text = "Total Jobs: " + stats.TotalJobs;
            lblStatsPending.Text = "Queued: " + stats.PendingJobs;
            lblStatsPrinting.Text = "Printing: " + stats.PrintingJobs;
            lblStatsCompleted.Text = "Completed: " + stats.CompletedJobs;
            lblStatsCancelled.Text = string.Format("Cancelled / Failed: {0} / {1}", stats.CancelledJobs, stats.FailedJobs);
            lblStatsPages.Text = "Total Pages Printed: " + stats.TotalPagesPrinted;
            lblStatsClients.Text = "Connected Clients: " + stats.ConnectedClients;
            lblStatsUptime.Text = string.Format("Uptime: {0:hh\\:mm\\:ss}", stats.ServerUptime);

            // Active job preview
            var allJobs = _service.GetAllJobs();
            var active = allJobs.FirstOrDefault(j => j.Status == PrintJobStatus.Printing);
            if (active != null)
            {
                lblActiveJobName.Text = string.Format("{0} ({1} pgs) - Client: {2}", active.FileName, active.PageCount, active.ClientName);
                prgActiveJob.Value = Math.Max(0, Math.Min(100, active.ProgressPercentage));
                lblActiveJobProgress.Text = active.ProgressPercentage + "%";
            }
            else
            {
                var nextQueued = allJobs.FirstOrDefault(j => j.Status == PrintJobStatus.Queued);
                if (nextQueued != null)
                {
                    lblActiveJobName.Text = string.Format("Next in Queue: {0} (Priority: {1})", nextQueued.FileName, nextQueued.Priority);
                }
                else
                {
                    lblActiveJobName.Text = "Printer is Idle (No active jobs)";
                }
                prgActiveJob.Value = 0;
                lblActiveJobProgress.Text = "0%";
            }
        }

        private void ResetDashboard()
        {
            lblStatsTotal.Text = "Total Jobs: 0";
            lblStatsPending.Text = "Queued: 0";
            lblStatsPrinting.Text = "Printing: 0";
            lblStatsCompleted.Text = "Completed: 0";
            lblStatsCancelled.Text = "Cancelled / Failed: 0 / 0";
            lblStatsPages.Text = "Total Pages Printed: 0";
            lblStatsClients.Text = "Connected Clients: 0";
            lblStatsUptime.Text = "Uptime: 00:00:00";
            lblActiveJobName.Text = "No job currently active";
            prgActiveJob.Value = 0;
            lblActiveJobProgress.Text = "0%";
        }

        private Guid? GetSelectedJobId()
        {
            if (dgvQueue.SelectedRows.Count > 0 && dgvQueue.SelectedRows[0].Tag != null)
            {
                return (Guid)dgvQueue.SelectedRows[0].Tag;
            }
            return null;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string[] printers = _service.GetAvailablePrinters();
            using (SubmitJobDialog dlg = new SubmitJobDialog(_service.ClientName, printers))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        var result = _service.SubmitPrintJob(
                            dlg.SelectedFileName,
                            dlg.SelectedFileData,
                            dlg.PageCount,
                            dlg.Priority,
                            dlg.TargetPrinter);

                        if (result.Success)
                        {
                            MessageBox.Show(this, "Print job submitted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshData();
                        }
                        else
                        {
                            MessageBox.Show(this, result.Message, "Submission Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Submission error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Guid? jobId = GetSelectedJobId();
            if (!jobId.HasValue)
            {
                MessageBox.Show(this, "Please select a job from the queue to pause.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = _service.PauseJob(jobId.Value);
            MessageBox.Show(this, result.Message, "Pause Job", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshData();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            Guid? jobId = GetSelectedJobId();
            if (!jobId.HasValue)
            {
                MessageBox.Show(this, "Please select a paused job to resume.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = _service.ResumeJob(jobId.Value);
            MessageBox.Show(this, result.Message, "Resume Job", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Guid? jobId = GetSelectedJobId();
            if (!jobId.HasValue)
            {
                MessageBox.Show(this, "Please select a job to cancel.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, "Are you sure you want to cancel the selected print job?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = _service.CancelJob(jobId.Value);
                MessageBox.Show(this, result.Message, "Cancel Job", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                RefreshData();
            }
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            Guid? jobId = GetSelectedJobId();
            if (!jobId.HasValue)
            {
                MessageBox.Show(this, "Please select a failed or cancelled job to retry.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = _service.RetryJob(jobId.Value);
            MessageBox.Show(this, result.Message, "Retry Job", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshData();
        }

        private void btnDeleteCompleted_Click(object sender, EventArgs e)
        {
            Guid? jobId = GetSelectedJobId();
            if (!jobId.HasValue)
            {
                MessageBox.Show(this, "Please select a completed or cancelled job to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = _service.DeleteCompletedJob(jobId.Value);
            MessageBox.Show(this, result.Message, "Delete Job", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshData();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            OpenSelectedJobDetails();
        }

        private void dgvQueue_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                OpenSelectedJobDetails();
            }
        }

        private void OpenSelectedJobDetails()
        {
            Guid? jobId = GetSelectedJobId();
            if (!jobId.HasValue)
            {
                MessageBox.Show(this, "Please select a job to inspect details.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (JobDetailsForm form = new JobDetailsForm(_service, jobId.Value))
            {
                form.ShowDialog(this);
            }
            RefreshData();
        }

        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerRefresh.Stop();
            if (_service.IsConnected)
            {
                _service.Disconnect();
            }
        }
    }
}
