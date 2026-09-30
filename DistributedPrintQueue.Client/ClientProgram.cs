using System;
using System.Windows.Forms;

namespace DistributedPrintQueue.Client
{
    static class ClientProgram
    {
        /// <summary>
        /// The main entry point for the Windows Forms client application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
