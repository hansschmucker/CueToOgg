using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

namespace CueToOgg
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void AfterFormLoad(object sender, EventArgs e)
        {
            var converter = new CueDirectoryConverter(logArea);
            converter.StartProcessing();
        }
    }
}
