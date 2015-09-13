using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CueToOgg
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var args = Environment.GetCommandLineArgs();

            for(var i = 0; i < args.Length; i++)
            {
                if (args[i].Length > 3 && args[i].Substring(0,2)=="--" && i<args.Length-1)
                {
                    if (!cmdArgs.ContainsKey(args[i]))
                        cmdArgs.Add(args[i], args[i + 1]);
                    else
                        cmdArgs[args[i]] += ";" + args[i + 1];
                }
            }
            if (cmdArgs.ContainsKey("--silent") && cmdArgs["--silent"]=="enable")
            {
                Application.Run(new SilentForm());
            }
            else
            {

                Application.Run(new MainForm());
            }
        }

        public static Dictionary<string, string> cmdArgs = new Dictionary<string, string>();
    }
}
