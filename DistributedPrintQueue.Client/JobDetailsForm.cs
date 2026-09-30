using System;
using System.Drawing;
using System.Windows.Forms;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.Client
{
    public partial class JobDetailsForm : Form
    {
        private readonly ClientService _service;
        private Guid _jobId;

        public JobDetailsForm(ClientService service, Guid jobId)
        {
            InitializeComponent();
            _service = service;
            _jobId = jobId;
            RefreshJobData();
        }

        private void RefreshJobData()
        {
            try
            {
                PrintJob job = _service.GetJobById(_jobId);
                if (job == null)
                {
                    MessageBox.Show(this, "The requested job was not found on the server.", "Job Missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                    return;
                }

                txtJobId.Text = job.JobId.ToString();
                txtFileName.Text = job.FileName;
                txtClient.Text = job.ClientName;
                txtPriority.Text = job.Priority.ToString();
                txtPages.Text = job.PageCount.ToString();
                txtStatus.Text = job.Status.ToString();
                prgBar.Value = Math.Max(0, Math.Min(100, job.ProgressPercentage));
                lblProgressVal.Text = job.ProgressPercentage + "%";
                txtSubmitted.Text = job.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss");
                txtPrinter.Text = job.PrinterName;
                txtQueuePos.Text = job.PositionInQueue > 0 ? job.PositionInQueue.ToString() : "(Not in queue)";
                txtError.Text = !string.IsNullOrEmpty(job.ErrorMessage) ? job.ErrorMessage : "No errors reported.";

                // Status color
                switch (job.Status)
                {
                    case PrintJobStatus.Printing:
                        txtStatus.ForeColor = Color.DarkGreen;
                        break;
                    case PrintJobStatus.Paused:
                        txtStatus.ForeColor = Color.DarkOrange;
                        break;
                    case PrintJobStatus.Completed:
                        txtStatus.ForeColor = Color.Blue;
                        break;
                    case PrintJobStatus.Cancelled:
                        txtStatus.ForeColor = Color.Red;
                        break;
                    case PrintJobStatus.Failed:
                        txtStatus.ForeColor = Color.DarkRed;
                        break;
                    default:
                        txtStatus.ForeColor = Color.Black;
                        break;
                }

                // Enable/disable buttons based on state
                btnPause.Enabled = (job.Status == PrintJobStatus.Queued || job.Status == PrintJobStatus.Printing);
                btnResume.Enabled = (job.Status == PrintJobStatus.Paused);
                btnCancel.Enabled = (job.Status == PrintJobStatus.Queued || job.Status == PrintJobStatus.Printing || job.Status == PrintJobStatus.Paused);
                btnRetry.Enabled = (job.Status == PrintJobStatus.Failed || job.Status == PrintJobStatus.Cancelled);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error fetching job details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            var res = _service.PauseJob(_jobId);
            MessageBox.Show(this, res.Message, "Pause Job", MessageBoxButtons.OK, res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshJobData();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            var res = _service.ResumeJob(_jobId);
            MessageBox.Show(this, res.Message, "Resume Job", MessageBoxButtons.OK, res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshJobData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to cancel this print job?", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var res = _service.CancelJob(_jobId);
                MessageBox.Show(this, res.Message, "Cancel Job", MessageBoxButtons.OK, res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                RefreshJobData();
            }
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            var res = _service.RetryJob(_jobId);
            MessageBox.Show(this, res.Message, "Retry Job", MessageBoxButtons.OK, res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshJobData();
        }
    }
}
