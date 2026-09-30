using System;
using System.IO;
using System.Windows.Forms;
using DistributedPrintQueue.Shared;

namespace DistributedPrintQueue.Client
{
    public partial class SubmitJobDialog : Form
    {
        public string SelectedFileName { get; private set; }
        public byte[] SelectedFileData { get; private set; }
        public int PageCount { get; private set; }
        public PrintPriority Priority { get; private set; }
        public string TargetPrinter { get; private set; }

        public SubmitJobDialog(string clientName, string[] availablePrinters)
        {
            InitializeComponent();

            txtClientName.Text = clientName;

            // Initialize Priorities
            cboPriority.Items.Add(PrintPriority.Low);
            cboPriority.Items.Add(PrintPriority.Normal);
            cboPriority.Items.Add(PrintPriority.High);
            cboPriority.SelectedItem = PrintPriority.Normal;

            // Initialize Printers
            if (availablePrinters != null && availablePrinters.Length > 0)
            {
                foreach (string printer in availablePrinters)
                {
                    cboPrinter.Items.Add(printer);
                }
                cboPrinter.SelectedIndex = 0;
            }
            else
            {
                cboPrinter.Items.Add("Default Virtual Printer");
                cboPrinter.SelectedIndex = 0;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Document to Print";
                ofd.Filter = "Supported Documents (*.txt;*.pdf;*.docx;*.xlsx;*.jpg;*.png)|*.txt;*.pdf;*.docx;*.xlsx;*.jpg;*.png|All Files (*.*)|*.*";
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(ofd.FileName);
                        if (!fi.Exists)
                        {
                            MessageBox.Show(this, "Selected file does not exist.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (fi.Length > 20 * 1024 * 1024)
                        {
                            MessageBox.Show(this, "File size exceeds the 20MB limit.", "File Too Large", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        SelectedFileData = File.ReadAllBytes(ofd.FileName);
                        SelectedFileName = Path.GetFileName(ofd.FileName);
                        txtFilePath.Text = ofd.FileName;
                        lblFileInfo.Text = string.Format("File: {0} ({1:N0} bytes)", SelectedFileName, fi.Length);

                        // Heuristic default page estimate based on size if text
                        if (SelectedFileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            int estPages = Math.Max(1, (int)(fi.Length / 1500));
                            numPages.Value = Math.Min(100, Math.Max(1, estPages));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Failed to read file: " + ex.Message, "Error Reading File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (SelectedFileData == null || SelectedFileData.Length == 0 || string.IsNullOrWhiteSpace(SelectedFileName))
            {
                MessageBox.Show(this, "Please browse and select a valid document file before submitting.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PageCount = (int)numPages.Value;
            Priority = (PrintPriority)cboPriority.SelectedItem;
            TargetPrinter = cboPrinter.SelectedItem != null ? cboPrinter.SelectedItem.ToString() : "Default Printer";

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
