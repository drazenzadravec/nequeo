namespace Nequeo.MediaPlayer
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.mediaPlayerControl = new Nequeo.Directx.UI.MediaPlayerControl();
            this.SuspendLayout();
            // 
            // mediaPlayerControl
            // 
            this.mediaPlayerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mediaPlayerControl.Location = new System.Drawing.Point(0, 0);
            this.mediaPlayerControl.Name = "mediaPlayerControl";
            this.mediaPlayerControl.Size = new System.Drawing.Size(1103, 631);
            this.mediaPlayerControl.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1103, 631);
            this.Controls.Add(this.mediaPlayerControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Nequeo Media Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Directx.UI.MediaPlayerControl mediaPlayerControl;
    }
}

