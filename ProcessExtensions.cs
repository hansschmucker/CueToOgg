using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueToOgg
{
    public static class ProcessExtensions
    {

        private static LoggerForm app = null;
        public static void StartSilent(this Process p, LoggerForm aApp)
        {
            app = aApp;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_OutputDataReceived;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = true;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
        }
        

        private static void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (app != null && app.Created && e != null && e.Data != null && e.Data != "")
            {
                var str = e.Data.Split('\n');

                for(var i=0;i<str.Length;i++)
                    app.Invoke(app.Log, new object[] { str[i] + "\r\n" });
            }
        }
    }
}
