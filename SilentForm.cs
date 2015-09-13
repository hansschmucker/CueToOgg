using System;
using System.Threading;
using System.Windows.Forms;

namespace CueToOgg
{
    public partial class SilentForm : LoggerForm
    {
        public SilentForm()
        {
            InitializeComponent();
            Alert = new AlertDelegate(AlertMethod);
            Exit = new ExitDelegate(ExitMethod);
            FatalExit = new FatalExitDelegate(FatalExitMethod);
            Log = new LogDelegate(LogMethod);
            ReportProgress = new ReportProgressDelegate(ReportProgressMethod);
        }

        public new void AlertMethod(string message)
        {
        }

        public new void ExitMethod()
        {
            if (converterThread != null && converterThread.ThreadState==ThreadState.Running)
            {
                converterThread.Abort();
            }
            Application.Exit();
        }
        
        public new void FatalExitMethod(string reason)
        {
            LogMethod(reason);
            AlertMethod(reason);
            ExitMethod();
        }

        public new void ReportProgressMethod(int arg)
        {
            this.progressBar1.Value = arg;
        }

        private Thread converterThread=null;
        private void AfterFormLoad(object sender, EventArgs e)
        {
            CenterToScreen();
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
