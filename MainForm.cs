using System;
using System.Threading;
using System.Windows.Forms;

namespace CueToOgg
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Alert = new AlertDelegate(AlertMethod);
            Exit = new ExitDelegate(ExitMethod);
            FatalExit = new FatalExitDelegate(FatalExitMethod);
            Log = new LogDelegate(LogMethod);
        }

        public delegate void AlertDelegate(string arg);
        public AlertDelegate Alert;
        public void AlertMethod(string message)
        {
            MessageBox.Show(message);
        }

        public delegate void ExitDelegate();
        public ExitDelegate Exit;
        public void ExitMethod()
        {
            if (converterThread != null && converterThread.ThreadState==ThreadState.Running)
            {
                converterThread.Abort();
            }
            Application.Exit();
        }

        public delegate void FatalExitDelegate(string arg);
        public FatalExitDelegate FatalExit;
        public void FatalExitMethod(string reason)
        {
            LogMethod(reason);
            AlertMethod(reason);
            ExitMethod();
        }

        public delegate void LogDelegate(string arg);
        public LogDelegate Log;
        public void LogMethod(string message)
        {
            logArea.AppendText(message);
        }


        private Thread converterThread=null;
        private void AfterFormLoad(object sender, EventArgs e)
        {
            var thread = new Thread(new ThreadStart(StartProcessing));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }


        private void StartProcessing()
        {
            var c = new CueDirectoryConverter(this);
            c.StartProcessing();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (converterThread != null && converterThread.ThreadState == ThreadState.Running)
            {
                converterThread.Abort();
            }
        }
    }
}
