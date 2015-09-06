using System;
using System.Windows.Forms;

namespace CueToOgg
{
    public partial class MainForm : Form , LogApplication
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void Alert(string message)
        {
            MessageBox.Show(message);
        }

        public void Exit()
        {
            Application.Exit();
        }

        public void FatalExit(string reason)
        {
            Log(reason);
            Alert(reason);
            Exit();
        }

        public void Log(string message)
        {
            logArea.AppendText(message);
            if (MainForm.ActiveForm != null)
                MainForm.ActiveForm.Invalidate();
        }

        private void AfterFormLoad(object sender, EventArgs e)
        {
            var converter = new CueDirectoryConverter(this);

            try {
                converter.StartProcessing();
            }catch(Exception ex)
            {
                FatalExit(ex.Message);
            }
        }
    }
}
