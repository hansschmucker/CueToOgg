namespace CueToOgg
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.logArea = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // logArea
            // 
            this.logArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logArea.Location = new System.Drawing.Point(0, 0);
            this.logArea.Multiline = true;
            this.logArea.Name = "logArea";
            this.logArea.ReadOnly = true;
            this.logArea.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logArea.Size = new System.Drawing.Size(437, 274);
            this.logArea.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 274);
            this.Controls.Add(this.logArea);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Tapper[ware] CUEtoOGG";
            this.Shown += new System.EventHandler(this.AfterFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logArea;
    }
}

