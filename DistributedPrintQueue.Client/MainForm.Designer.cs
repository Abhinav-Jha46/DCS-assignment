namespace DistributedPrintQueue.Client
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.lblStatusDot = new System.Windows.Forms.Label();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.lblClientName = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.cboFilter = new System.Windows.Forms.ComboBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.btnDetails = new System.Windows.Forms.Button();
            this.btnDeleteCompleted = new System.Windows.Forms.Button();
            this.btnRetry = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.dgvQueue = new System.Windows.Forms.DataGridView();
            this.colPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colJobId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPages = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubmittedAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrinter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlDashboard = new System.Windows.Forms.Panel();
            this.grpStats = new System.Windows.Forms.GroupBox();
            this.lblStatsUptime = new System.Windows.Forms.Label();
            this.lblStatsClients = new System.Windows.Forms.Label();
            this.lblStatsPages = new System.Windows.Forms.Label();
            this.lblStatsCancelled = new System.Windows.Forms.Label();
            this.lblStatsCompleted = new System.Windows.Forms.Label();
            this.lblStatsPrinting = new System.Windows.Forms.Label();
            this.lblStatsPending = new System.Windows.Forms.Label();
            this.lblStatsTotal = new System.Windows.Forms.Label();
            this.grpActiveJob = new System.Windows.Forms.GroupBox();
            this.prgActiveJob = new System.Windows.Forms.ProgressBar();
            this.lblActiveJobProgress = new System.Windows.Forms.Label();
            this.lblActiveJobName = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statLabelQueueCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.statLabelLastUpdated = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.pnlHeader.SuspendLayout();
            this.grpConnection.SuspendLayout();
            this.pnlToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueue)).BeginInit();
            this.pnlDashboard.SuspendLayout();
            this.grpStats.SuspendLayout();
            this.grpActiveJob.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.pnlHeader.Controls.Add(this.lblSubTitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(984, 56);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.lblSubTitle.Location = new System.Drawing.Point(18, 33);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(425, 13);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "Distributed Computing Systems Laboratory Project | .NET Remoting TCP Client";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(414, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "DISTRIBUTED PRINT QUEUE MANAGER";
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.lblStatusDot);
            this.grpConnection.Controls.Add(this.lblConnectionStatus);
            this.grpConnection.Controls.Add(this.btnDisconnect);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.txtClientName);
            this.grpConnection.Controls.Add(this.lblClientName);
            this.grpConnection.Controls.Add(this.txtPort);
            this.grpConnection.Controls.Add(this.lblPort);
            this.grpConnection.Controls.Add(this.txtHost);
            this.grpConnection.Controls.Add(this.lblHost);
            this.grpConnection.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpConnection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpConnection.Location = new System.Drawing.Point(0, 56);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(984, 68);
            this.grpConnection.TabIndex = 1;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Server Connection";
            // 
            // lblStatusDot
            // 
            this.lblStatusDot.AutoSize = true;
            this.lblStatusDot.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblStatusDot.ForeColor = System.Drawing.Color.Red;
            this.lblStatusDot.Location = new System.Drawing.Point(735, 27);
            this.lblStatusDot.Name = "lblStatusDot";
            this.lblStatusDot.Size = new System.Drawing.Size(22, 21);
            this.lblStatusDot.TabIndex = 9;
            this.lblStatusDot.Text = "●";
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblConnectionStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblConnectionStatus.Location = new System.Drawing.Point(760, 30);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(83, 15);
            this.lblConnectionStatus.TabIndex = 8;
            this.lblConnectionStatus.Text = "Disconnected";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDisconnect.Location = new System.Drawing.Point(645, 26);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(80, 26);
            this.btnDisconnect.TabIndex = 7;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConnect.Location = new System.Drawing.Point(555, 26);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(80, 26);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtClientName
            // 
            this.txtClientName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtClientName.Location = new System.Drawing.Point(400, 28);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(140, 23);
            this.txtClientName.TabIndex = 5;
            this.txtClientName.Text = "Student-PC-01";
            // 
            // lblClientName
            // 
            this.lblClientName.AutoSize = true;
            this.lblClientName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblClientName.Location = new System.Drawing.Point(320, 31);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(76, 15);
            this.lblClientName.TabIndex = 4;
            this.lblClientName.Text = "Client Name:";
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPort.Location = new System.Drawing.Point(245, 28);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(65, 23);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "9000";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPort.Location = new System.Drawing.Point(205, 31);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 15);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            // 
            // txtHost
            // 
            this.txtHost.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHost.Location = new System.Drawing.Point(85, 28);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(110, 23);
            this.txtHost.TabIndex = 1;
            this.txtHost.Text = "localhost";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHost.Location = new System.Drawing.Point(12, 31);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(69, 15);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Server Host:";
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlToolbar.Controls.Add(this.cboFilter);
            this.pnlToolbar.Controls.Add(this.lblFilter);
            this.pnlToolbar.Controls.Add(this.btnDetails);
            this.pnlToolbar.Controls.Add(this.btnDeleteCompleted);
            this.pnlToolbar.Controls.Add(this.btnRetry);
            this.pnlToolbar.Controls.Add(this.btnResume);
            this.pnlToolbar.Controls.Add(this.btnPause);
            this.pnlToolbar.Controls.Add(this.btnCancel);
            this.pnlToolbar.Controls.Add(this.btnRefresh);
            this.pnlToolbar.Controls.Add(this.btnSubmit);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Location = new System.Drawing.Point(0, 124);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(984, 46);
            this.pnlToolbar.TabIndex = 2;
            // 
            // cboFilter
            // 
            this.cboFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboFilter.FormattingEnabled = true;
            this.cboFilter.Items.AddRange(new object[] {
            "All Jobs",
            "My Jobs Only"});
            this.cboFilter.Location = new System.Drawing.Point(860, 11);
            this.cboFilter.Name = "cboFilter";
            this.cboFilter.Size = new System.Drawing.Size(112, 23);
            this.cboFilter.TabIndex = 9;
            this.cboFilter.SelectedIndexChanged += new System.EventHandler(this.cboFilter_SelectedIndexChanged);
            // 
            // lblFilter
            // 
            this.lblFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilter.AutoSize = true;
            this.lblFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFilter.Location = new System.Drawing.Point(818, 14);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(36, 15);
            this.lblFilter.TabIndex = 8;
            this.lblFilter.Text = "View:";
            // 
            // btnDetails
            // 
            this.btnDetails.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDetails.Location = new System.Drawing.Point(670, 8);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(75, 30);
            this.btnDetails.TabIndex = 7;
            this.btnDetails.Text = "Job Details";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnDeleteCompleted
            // 
            this.btnDeleteCompleted.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeleteCompleted.Location = new System.Drawing.Point(545, 8);
            this.btnDeleteCompleted.Name = "btnDeleteCompleted";
            this.btnDeleteCompleted.Size = new System.Drawing.Size(115, 30);
            this.btnDeleteCompleted.TabIndex = 6;
            this.btnDeleteCompleted.Text = "Delete Finished";
            this.btnDeleteCompleted.UseVisualStyleBackColor = true;
            this.btnDeleteCompleted.Click += new System.EventHandler(this.btnDeleteCompleted_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRetry.Location = new System.Drawing.Point(475, 8);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(60, 30);
            this.btnRetry.TabIndex = 5;
            this.btnRetry.Text = "Retry";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnResume
            // 
            this.btnResume.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnResume.Location = new System.Drawing.Point(405, 8);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(65, 30);
            this.btnResume.TabIndex = 4;
            this.btnResume.Text = "Resume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnPause.Location = new System.Drawing.Point(340, 8);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(60, 30);
            this.btnPause.TabIndex = 3;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(245, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel Job";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRefresh.Location = new System.Drawing.Point(165, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(70, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(12, 8);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(140, 30);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "+ Submit Print Job";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // dgvQueue
            // 
            this.dgvQueue.AllowUserToAddRows = false;
            this.dgvQueue.AllowUserToDeleteRows = false;
            this.dgvQueue.AllowUserToResizeRows = false;
            this.dgvQueue.BackgroundColor = System.Drawing.Color.White;
            this.dgvQueue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvQueue.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvQueue.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvQueue.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvQueue.ColumnHeadersHeight = 30;
            this.dgvQueue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvQueue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPosition,
            this.colJobId,
            this.colFileName,
            this.colClientName,
            this.colPages,
            this.colPriority,
            this.colStatus,
            this.colProgress,
            this.colSubmittedAt,
            this.colPrinter});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(234)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvQueue.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvQueue.EnableHeadersVisualStyles = false;
            this.dgvQueue.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvQueue.Location = new System.Drawing.Point(0, 170);
            this.dgvQueue.MultiSelect = false;
            this.dgvQueue.Name = "dgvQueue";
            this.dgvQueue.ReadOnly = true;
            this.dgvQueue.RowHeadersVisible = false;
            this.dgvQueue.RowTemplate.Height = 26;
            this.dgvQueue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQueue.Size = new System.Drawing.Size(984, 280);
            this.dgvQueue.TabIndex = 3;
            this.dgvQueue.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvQueue_CellDoubleClick);
            // 
            // colPosition
            // 
            this.colPosition.HeaderText = "Pos";
            this.colPosition.Name = "colPosition";
            this.colPosition.ReadOnly = true;
            this.colPosition.Width = 45;
            // 
            // colJobId
            // 
            this.colJobId.HeaderText = "Job ID";
            this.colJobId.Name = "colJobId";
            this.colJobId.ReadOnly = true;
            this.colJobId.Width = 80;
            // 
            // colFileName
            // 
            this.colFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFileName.HeaderText = "Document File";
            this.colFileName.Name = "colFileName";
            this.colFileName.ReadOnly = true;
            // 
            // colClientName
            // 
            this.colClientName.HeaderText = "Client";
            this.colClientName.Name = "colClientName";
            this.colClientName.ReadOnly = true;
            this.colClientName.Width = 110;
            // 
            // colPages
            // 
            this.colPages.HeaderText = "Pages";
            this.colPages.Name = "colPages";
            this.colPages.ReadOnly = true;
            this.colPages.Width = 55;
            // 
            // colPriority
            // 
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            this.colPriority.Width = 75;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 90;
            // 
            // colProgress
            // 
            this.colProgress.HeaderText = "Progress";
            this.colProgress.Name = "colProgress";
            this.colProgress.ReadOnly = true;
            this.colProgress.Width = 70;
            // 
            // colSubmittedAt
            // 
            this.colSubmittedAt.HeaderText = "Submitted";
            this.colSubmittedAt.Name = "colSubmittedAt";
            this.colSubmittedAt.ReadOnly = true;
            this.colSubmittedAt.Width = 125;
            // 
            // colPrinter
            // 
            this.colPrinter.HeaderText = "Target Printer";
            this.colPrinter.Name = "colPrinter";
            this.colPrinter.ReadOnly = true;
            this.colPrinter.Width = 140;
            // 
            // pnlDashboard
            // 
            this.pnlDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlDashboard.Controls.Add(this.grpStats);
            this.pnlDashboard.Controls.Add(this.grpActiveJob);
            this.pnlDashboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDashboard.Location = new System.Drawing.Point(0, 450);
            this.pnlDashboard.Name = "pnlDashboard";
            this.pnlDashboard.Size = new System.Drawing.Size(984, 110);
            this.pnlDashboard.TabIndex = 4;
            // 
            // grpStats
            // 
            this.grpStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStats.Controls.Add(this.lblStatsUptime);
            this.grpStats.Controls.Add(this.lblStatsClients);
            this.grpStats.Controls.Add(this.lblStatsPages);
            this.grpStats.Controls.Add(this.lblStatsCancelled);
            this.grpStats.Controls.Add(this.lblStatsCompleted);
            this.grpStats.Controls.Add(this.lblStatsPrinting);
            this.grpStats.Controls.Add(this.lblStatsPending);
            this.grpStats.Controls.Add(this.lblStatsTotal);
            this.grpStats.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStats.Location = new System.Drawing.Point(400, 6);
            this.grpStats.Name = "grpStats";
            this.grpStats.Size = new System.Drawing.Size(572, 95);
            this.grpStats.TabIndex = 1;
            this.grpStats.TabStop = false;
            this.grpStats.Text = "Server Telemetry & Queue Statistics";
            // 
            // lblStatsUptime
            // 
            this.lblStatsUptime.AutoSize = true;
            this.lblStatsUptime.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsUptime.Location = new System.Drawing.Point(400, 55);
            this.lblStatsUptime.Name = "lblStatsUptime";
            this.lblStatsUptime.Size = new System.Drawing.Size(91, 13);
            this.lblStatsUptime.TabIndex = 7;
            this.lblStatsUptime.Text = "Uptime: 00:00:00";
            // 
            // lblStatsClients
            // 
            this.lblStatsClients.AutoSize = true;
            this.lblStatsClients.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsClients.Location = new System.Drawing.Point(400, 25);
            this.lblStatsClients.Name = "lblStatsClients";
            this.lblStatsClients.Size = new System.Drawing.Size(107, 13);
            this.lblStatsClients.TabIndex = 6;
            this.lblStatsClients.Text = "Connected Clients: 0";
            // 
            // lblStatsPages
            // 
            this.lblStatsPages.AutoSize = true;
            this.lblStatsPages.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsPages.Location = new System.Drawing.Point(230, 55);
            this.lblStatsPages.Name = "lblStatsPages";
            this.lblStatsPages.Size = new System.Drawing.Size(117, 13);
            this.lblStatsPages.TabIndex = 5;
            this.lblStatsPages.Text = "Total Pages Printed: 0";
            // 
            // lblStatsCancelled
            // 
            this.lblStatsCancelled.AutoSize = true;
            this.lblStatsCancelled.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsCancelled.ForeColor = System.Drawing.Color.DarkRed;
            this.lblStatsCancelled.Location = new System.Drawing.Point(230, 25);
            this.lblStatsCancelled.Name = "lblStatsCancelled";
            this.lblStatsCancelled.Size = new System.Drawing.Size(126, 13);
            this.lblStatsCancelled.TabIndex = 4;
            this.lblStatsCancelled.Text = "Cancelled / Failed: 0 / 0";
            // 
            // lblStatsCompleted
            // 
            this.lblStatsCompleted.AutoSize = true;
            this.lblStatsCompleted.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsCompleted.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblStatsCompleted.Location = new System.Drawing.Point(100, 55);
            this.lblStatsCompleted.Name = "lblStatsCompleted";
            this.lblStatsCompleted.Size = new System.Drawing.Size(76, 13);
            this.lblStatsCompleted.TabIndex = 3;
            this.lblStatsCompleted.Text = "Completed: 0";
            // 
            // lblStatsPrinting
            // 
            this.lblStatsPrinting.AutoSize = true;
            this.lblStatsPrinting.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsPrinting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblStatsPrinting.Location = new System.Drawing.Point(100, 25);
            this.lblStatsPrinting.Name = "lblStatsPrinting";
            this.lblStatsPrinting.Size = new System.Drawing.Size(61, 13);
            this.lblStatsPrinting.TabIndex = 2;
            this.lblStatsPrinting.Text = "Printing: 0";
            // 
            // lblStatsPending
            // 
            this.lblStatsPending.AutoSize = true;
            this.lblStatsPending.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsPending.Location = new System.Drawing.Point(15, 55);
            this.lblStatsPending.Name = "lblStatsPending";
            this.lblStatsPending.Size = new System.Drawing.Size(63, 13);
            this.lblStatsPending.TabIndex = 1;
            this.lblStatsPending.Text = "Queued: 0";
            // 
            // lblStatsTotal
            // 
            this.lblStatsTotal.AutoSize = true;
            this.lblStatsTotal.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblStatsTotal.Location = new System.Drawing.Point(15, 25);
            this.lblStatsTotal.Name = "lblStatsTotal";
            this.lblStatsTotal.Size = new System.Drawing.Size(70, 13);
            this.lblStatsTotal.TabIndex = 0;
            this.lblStatsTotal.Text = "Total Jobs: 0";
            // 
            // grpActiveJob
            // 
            this.grpActiveJob.Controls.Add(this.prgActiveJob);
            this.grpActiveJob.Controls.Add(this.lblActiveJobProgress);
            this.grpActiveJob.Controls.Add(this.lblActiveJobName);
            this.grpActiveJob.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpActiveJob.Location = new System.Drawing.Point(12, 6);
            this.grpActiveJob.Name = "grpActiveJob";
            this.grpActiveJob.Size = new System.Drawing.Size(375, 95);
            this.grpActiveJob.TabIndex = 0;
            this.grpActiveJob.TabStop = false;
            this.grpActiveJob.Text = "Current Active Print Job";
            // 
            // prgActiveJob
            // 
            this.prgActiveJob.Location = new System.Drawing.Point(15, 55);
            this.prgActiveJob.Name = "prgActiveJob";
            this.prgActiveJob.Size = new System.Drawing.Size(280, 20);
            this.prgActiveJob.TabIndex = 2;
            // 
            // lblActiveJobProgress
            // 
            this.lblActiveJobProgress.AutoSize = true;
            this.lblActiveJobProgress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblActiveJobProgress.Location = new System.Drawing.Point(305, 58);
            this.lblActiveJobProgress.Name = "lblActiveJobProgress";
            this.lblActiveJobProgress.Size = new System.Drawing.Size(25, 15);
            this.lblActiveJobProgress.TabIndex = 1;
            this.lblActiveJobProgress.Text = "0%";
            // 
            // lblActiveJobName
            // 
            this.lblActiveJobName.AutoSize = true;
            this.lblActiveJobName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblActiveJobName.Location = new System.Drawing.Point(15, 25);
            this.lblActiveJobName.Name = "lblActiveJobName";
            this.lblActiveJobName.Size = new System.Drawing.Size(126, 15);
            this.lblActiveJobName.TabIndex = 0;
            this.lblActiveJobName.Text = "No job currently active";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statLabelStatus,
            this.statLabelQueueCount,
            this.statLabelLastUpdated});
            this.statusStrip.Location = new System.Drawing.Point(0, 560);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(984, 22);
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statLabelStatus
            // 
            this.statLabelStatus.Name = "statLabelStatus";
            this.statLabelStatus.Size = new System.Drawing.Size(134, 17);
            this.statLabelStatus.Text = "Status: Not Connected";
            // 
            // statLabelQueueCount
            // 
            this.statLabelQueueCount.Name = "statLabelQueueCount";
            this.statLabelQueueCount.Size = new System.Drawing.Size(699, 17);
            this.statLabelQueueCount.Spring = true;
            this.statLabelQueueCount.Text = "Queue: 0 items";
            // 
            // statLabelLastUpdated
            // 
            this.statLabelLastUpdated.Name = "statLabelLastUpdated";
            this.statLabelLastUpdated.Size = new System.Drawing.Size(104, 17);
            this.statLabelLastUpdated.Text = "Last Updated: --";
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 1000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(984, 582);
            this.Controls.Add(this.dgvQueue);
            this.Controls.Add(this.pnlDashboard);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.grpConnection);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(850, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Distributed Print Queue Management System (.NET Remoting)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.pnlToolbar.ResumeLayout(false);
            this.pnlToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueue)).EndInit();
            this.pnlDashboard.ResumeLayout(false);
            this.grpStats.ResumeLayout(false);
            this.grpStats.PerformLayout();
            this.grpActiveJob.ResumeLayout(false);
            this.grpActiveJob.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblClientName;
        private System.Windows.Forms.TextBox txtClientName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Label lblStatusDot;
        private System.Windows.Forms.Panel pnlToolbar;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.Button btnDeleteCompleted;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.ComboBox cboFilter;
        private System.Windows.Forms.DataGridView dgvQueue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colJobId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPages;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubmittedAt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrinter;
        private System.Windows.Forms.Panel pnlDashboard;
        private System.Windows.Forms.GroupBox grpActiveJob;
        private System.Windows.Forms.Label lblActiveJobName;
        private System.Windows.Forms.Label lblActiveJobProgress;
        private System.Windows.Forms.ProgressBar prgActiveJob;
        private System.Windows.Forms.GroupBox grpStats;
        private System.Windows.Forms.Label lblStatsTotal;
        private System.Windows.Forms.Label lblStatsPending;
        private System.Windows.Forms.Label lblStatsPrinting;
        private System.Windows.Forms.Label lblStatsCompleted;
        private System.Windows.Forms.Label lblStatsCancelled;
        private System.Windows.Forms.Label lblStatsPages;
        private System.Windows.Forms.Label lblStatsClients;
        private System.Windows.Forms.Label lblStatsUptime;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statLabelStatus;
        private System.Windows.Forms.ToolStripStatusLabel statLabelQueueCount;
        private System.Windows.Forms.ToolStripStatusLabel statLabelLastUpdated;
        private System.Windows.Forms.Timer timerRefresh;
    }
}
