namespace Nequeo.MediaPlayer.Vlc
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
            this.mediaPlayerControlMain = new Nequeo.Media.Vlc.UI.MediaPlayerControl();
            this.SuspendLayout();
            // 
            // mediaPlayerControlMain
            // 
            this.mediaPlayerControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mediaPlayerControlMain.Location = new System.Drawing.Point(0, 0);
            this.mediaPlayerControlMain.Name = "mediaPlayerControlMain";
            this.mediaPlayerControlMain.Size = new System.Drawing.Size(1141, 666);
            this.mediaPlayerControlMain.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1141, 666);
            this.Controls.Add(this.mediaPlayerControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Nequeo VLC Media Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private Media.Vlc.UI.MediaPlayerControl mediaPlayerControlMain;
    }
}

