using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CueToOgg
{
    public class LoggerForm : Form
    {
        public LoggerForm() { }
            
        public delegate void AlertDelegate(string arg);
        public AlertDelegate Alert;
        public void AlertMethod(string message){}

        public delegate void ExitDelegate();
        public ExitDelegate Exit;
        public void ExitMethod(){}

        public delegate void FatalExitDelegate(string arg);
        public FatalExitDelegate FatalExit;
        public void FatalExitMethod(string reason){}

        public delegate void LogDelegate(string arg);
        public LogDelegate Log;
        public void LogMethod(string message){}

        public delegate void ReportProgressDelegate(int arg);
        public ReportProgressDelegate ReportProgress;
        public void ReportProgressMethod(int message) { }
    }
}
