namespace DistributedPrintQueue.Client
{
    partial class JobDetailsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblJobId = new System.Windows.Forms.Label();
            this.txtJobId = new System.Windows.Forms.TextBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.lblClient = new System.Windows.Forms.Label();
            this.txtClient = new System.Windows.Forms.TextBox();
            this.lblPriority = new System.Windows.Forms.Label();
            this.txtPriority = new System.Windows.Forms.TextBox();
            this.lblPages = new System.Windows.Forms.Label();
            this.txtPages = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.prgBar = new System.Windows.Forms.ProgressBar();
            this.lblProgressVal = new System.Windows.Forms.Label();
            this.lblSubmitted = new System.Windows.Forms.Label();
            this.txtSubmitted = new System.Windows.Forms.TextBox();
            this.lblPrinter = new System.Windows.Forms.Label();
            this.txtPrinter = new System.Windows.Forms.TextBox();
            this.lblQueuePos = new System.Windows.Forms.Label();
            this.txtQueuePos = new System.Windows.Forms.TextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.txtError = new System.Windows.Forms.TextBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRetry = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.pnlTop.Controls.Add(this.lblHeader);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(564, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(16, 14);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(193, 21);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Print Job Detailed Status";
            // 
            // lblJobId
            // 
            this.lblJobId.AutoSize = true;
            this.lblJobId.Location = new System.Drawing.Point(24, 70);
            this.lblJobId.Name = "lblJobId";
            this.lblJobId.Size = new System.Drawing.Size(43, 15);
            this.lblJobId.TabIndex = 1;
            this.lblJobId.Text = "Job ID:";
            // 
            // txtJobId
            // 
            this.txtJobId.Location = new System.Drawing.Point(120, 67);
            this.txtJobId.Name = "txtJobId";
            this.txtJobId.ReadOnly = true;
            this.txtJobId.Size = new System.Drawing.Size(415, 23);
            this.txtJobId.TabIndex = 2;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(24, 105);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(63, 15);
            this.lblFileName.TabIndex = 3;
            this.lblFileName.Text = "Document:";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(120, 102);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(415, 23);
            this.txtFileName.TabIndex = 4;
            // 
            // lblClient
            // 
            this.lblClient.AutoSize = true;
            this.lblClient.Location = new System.Drawing.Point(24, 140);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(76, 15);
            this.lblClient.TabIndex = 5;
            this.lblClient.Text = "Client Name:";
            // 
            // txtClient
            // 
            this.txtClient.Location = new System.Drawing.Point(120, 137);
            this.txtClient.Name = "txtClient";
            this.txtClient.ReadOnly = true;
            this.txtClient.Size = new System.Drawing.Size(160, 23);
            this.txtClient.TabIndex = 6;
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Location = new System.Drawing.Point(310, 140);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(48, 15);
            this.lblPriority.TabIndex = 7;
            this.lblPriority.Text = "Priority:";
            // 
            // txtPriority
            // 
            this.txtPriority.Location = new System.Drawing.Point(375, 137);
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.ReadOnly = true;
            this.txtPriority.Size = new System.Drawing.Size(160, 23);
            this.txtPriority.TabIndex = 8;
            // 
            // lblPages
            // 
            this.lblPages.AutoSize = true;
            this.lblPages.Location = new System.Drawing.Point(24, 175);
            this.lblPages.Name = "lblPages";
            this.lblPages.Size = new System.Drawing.Size(72, 15);
            this.lblPages.TabIndex = 9;
            this.lblPages.Text = "Page Count:";
            // 
            // txtPages
            // 
            this.txtPages.Location = new System.Drawing.Point(120, 172);
            this.txtPages.Name = "txtPages";
            this.txtPages.ReadOnly = true;
            this.txtPages.Size = new System.Drawing.Size(160, 23);
            this.txtPages.TabIndex = 10;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(310, 175);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 15);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "Status:";
            // 
            // txtStatus
            // 
            this.txtStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtStatus.Location = new System.Drawing.Point(375, 172);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(160, 23);
            this.txtStatus.TabIndex = 12;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(24, 212);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(55, 15);
            this.lblProgress.TabIndex = 13;
            this.lblProgress.Text = "Progress:";
            // 
            // prgBar
            // 
            this.prgBar.Location = new System.Drawing.Point(120, 209);
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(350, 22);
            this.prgBar.TabIndex = 14;
            // 
            // lblProgressVal
            // 
            this.lblProgressVal.AutoSize = true;
            this.lblProgressVal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProgressVal.Location = new System.Drawing.Point(480, 212);
            this.lblProgressVal.Name = "lblProgressVal";
            this.lblProgressVal.Size = new System.Drawing.Size(25, 15);
            this.lblProgressVal.TabIndex = 15;
            this.lblProgressVal.Text = "0%";
            // 
            // lblSubmitted
            // 
            this.lblSubmitted.AutoSize = true;
            this.lblSubmitted.Location = new System.Drawing.Point(24, 250);
            this.lblSubmitted.Name = "lblSubmitted";
            this.lblSubmitted.Size = new System.Drawing.Size(83, 15);
            this.lblSubmitted.TabIndex = 16;
            this.lblSubmitted.Text = "Submitted At:";
            // 
            // txtSubmitted
            // 
            this.txtSubmitted.Location = new System.Drawing.Point(120, 247);
            this.txtSubmitted.Name = "txtSubmitted";
            this.txtSubmitted.ReadOnly = true;
            this.txtSubmitted.Size = new System.Drawing.Size(160, 23);
            this.txtSubmitted.TabIndex = 17;
            // 
            // lblPrinter
            // 
            this.lblPrinter.AutoSize = true;
            this.lblPrinter.Location = new System.Drawing.Point(310, 250);
            this.lblPrinter.Name = "lblPrinter";
            this.lblPrinter.Size = new System.Drawing.Size(45, 15);
            this.lblPrinter.TabIndex = 18;
            this.lblPrinter.Text = "Printer:";
            // 
            // txtPrinter
            // 
            this.txtPrinter.Location = new System.Drawing.Point(375, 247);
            this.txtPrinter.Name = "txtPrinter";
            this.txtPrinter.ReadOnly = true;
            this.txtPrinter.Size = new System.Drawing.Size(160, 23);
            this.txtPrinter.TabIndex = 19;
            // 
            // lblQueuePos
            // 
            this.lblQueuePos.AutoSize = true;
            this.lblQueuePos.Location = new System.Drawing.Point(24, 285);
            this.lblQueuePos.Name = "lblQueuePos";
            this.lblQueuePos.Size = new System.Drawing.Size(90, 15);
            this.lblQueuePos.TabIndex = 20;
            this.lblQueuePos.Text = "Queue Position:";
            // 
            // txtQueuePos
            // 
            this.txtQueuePos.Location = new System.Drawing.Point(120, 282);
            this.txtQueuePos.Name = "txtQueuePos";
            this.txtQueuePos.ReadOnly = true;
            this.txtQueuePos.Size = new System.Drawing.Size(160, 23);
            this.txtQueuePos.TabIndex = 21;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(24, 320);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(86, 15);
            this.lblError.TabIndex = 22;
            this.lblError.Text = "Status Details:";
            // 
            // txtError
            // 
            this.txtError.Location = new System.Drawing.Point(120, 317);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtError.Size = new System.Drawing.Size(415, 50);
            this.txtError.TabIndex = 23;
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(27, 390);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(85, 30);
            this.btnPause.TabIndex = 24;
            this.btnPause.Text = "Pause Job";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(122, 390);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(90, 30);
            this.btnResume.TabIndex = 25;
            this.btnResume.Text = "Resume Job";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(222, 390);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 30);
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "Cancel Job";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.Location = new System.Drawing.Point(317, 390);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(85, 30);
            this.btnRetry.TabIndex = 27;
            this.btnRetry.Text = "Retry Job";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(445, 390);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 30);
            this.btnClose.TabIndex = 28;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // JobDetailsForm
            // 
            this.AcceptButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(564, 440);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRetry);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.txtQueuePos);
            this.Controls.Add(this.lblQueuePos);
            this.Controls.Add(this.txtPrinter);
            this.Controls.Add(this.lblPrinter);
            this.Controls.Add(this.txtSubmitted);
            this.Controls.Add(this.lblSubmitted);
            this.Controls.Add(this.lblProgressVal);
            this.Controls.Add(this.prgBar);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtPages);
            this.Controls.Add(this.lblPages);
            this.Controls.Add(this.txtPriority);
            this.Controls.Add(this.lblPriority);
            this.Controls.Add(this.txtClient);
            this.Controls.Add(this.lblClient);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.txtJobId);
            this.Controls.Add(this.lblJobId);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JobDetailsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Job Details";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblJobId;
        private System.Windows.Forms.TextBox txtJobId;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.TextBox txtClient;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.TextBox txtPriority;
        private System.Windows.Forms.Label lblPages;
        private System.Windows.Forms.TextBox txtPages;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar prgBar;
        private System.Windows.Forms.Label lblProgressVal;
        private System.Windows.Forms.Label lblSubmitted;
        private System.Windows.Forms.TextBox txtSubmitted;
        private System.Windows.Forms.Label lblPrinter;
        private System.Windows.Forms.TextBox txtPrinter;
        private System.Windows.Forms.Label lblQueuePos;
        private System.Windows.Forms.TextBox txtQueuePos;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.Button btnClose;
    }
}
