namespace Nequeo.Media.Vlc.UI
{
    partial class MediaPlayerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMediaDisplay = new System.Windows.Forms.Panel();
            this.openFileDialogMain = new System.Windows.Forms.OpenFileDialog();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonMute = new System.Windows.Forms.Button();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelOf = new System.Windows.Forms.Label();
            this.labelDuration = new System.Windows.Forms.Label();
            this.folderBrowserDialogMain = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonOpen = new System.Windows.Forms.ComboBox();
            this.trackBarMain = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMain)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMediaDisplay
            // 
            this.panelMediaDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMediaDisplay.Location = new System.Drawing.Point(3, 3);
            this.panelMediaDisplay.Name = "panelMediaDisplay";
            this.panelMediaDisplay.Size = new System.Drawing.Size(767, 58);
            this.panelMediaDisplay.TabIndex = 0;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClose.Enabled = false;
            this.buttonClose.Location = new System.Drawing.Point(99, 123);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Location = new System.Drawing.Point(279, 123);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(75, 23);
            this.buttonPlay.TabIndex = 3;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(360, 123);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPause.Enabled = false;
            this.buttonPause.Location = new System.Drawing.Point(441, 123);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 23);
            this.buttonPause.TabIndex = 5;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonMute
            // 
            this.buttonMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMute.Enabled = false;
            this.buttonMute.Location = new System.Drawing.Point(522, 123);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(75, 23);
            this.buttonMute.TabIndex = 6;
            this.buttonMute.Text = "Mute";
            this.buttonMute.UseVisualStyleBackColor = true;
            this.buttonMute.Click += new System.EventHandler(this.buttonMute_Click);
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(611, 128);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(49, 13);
            this.labelTime.TabIndex = 7;
            this.labelTime.Text = "00:00:00";
            // 
            // labelOf
            // 
            this.labelOf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOf.AutoSize = true;
            this.labelOf.Location = new System.Drawing.Point(676, 128);
            this.labelOf.Name = "labelOf";
            this.labelOf.Size = new System.Drawing.Size(16, 13);
            this.labelOf.TabIndex = 8;
            this.labelOf.Text = "of";
            // 
            // labelDuration
            // 
            this.labelDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDuration.AutoSize = true;
            this.labelDuration.Location = new System.Drawing.Point(710, 128);
            this.labelDuration.Name = "labelDuration";
            this.labelDuration.Size = new System.Drawing.Size(49, 13);
            this.labelDuration.TabIndex = 9;
            this.labelDuration.Text = "00:00:00";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOpen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.buttonOpen.FormattingEnabled = true;
            this.buttonOpen.Items.AddRange(new object[] {
            "Open File",
            "Open Folder",
            "Open Network"});
            this.buttonOpen.Location = new System.Drawing.Point(3, 124);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(90, 21);
            this.buttonOpen.TabIndex = 1;
            this.buttonOpen.SelectedIndexChanged += new System.EventHandler(this.buttonOpen_SelectedIndexChanged);
            // 
            // trackBarMain
            // 
            this.trackBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarMain.Enabled = false;
            this.trackBarMain.Location = new System.Drawing.Point(3, 72);
            this.trackBarMain.Maximum = 100;
            this.trackBarMain.Name = "trackBarMain";
            this.trackBarMain.Size = new System.Drawing.Size(767, 45);
            this.trackBarMain.TabIndex = 10;
            this.trackBarMain.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarMain.Scroll += new System.EventHandler(this.trackBarMain_Scroll);
            this.trackBarMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarMain_MouseDown);
            this.trackBarMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarMain_MouseUp);
            // 
            // MediaPlayerControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBarMain);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.labelDuration);
            this.Controls.Add(this.labelOf);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.buttonMute);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.panelMediaDisplay);
            this.Name = "MediaPlayerControl";
            this.Size = new System.Drawing.Size(773, 151);
            this.Load += new System.EventHandler(this.MediaPlayerControl_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MediaPlayerControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MediaPlayerControl_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMediaDisplay;
        private System.Windows.Forms.OpenFileDialog openFileDialogMain;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonMute;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelOf;
        private System.Windows.Forms.Label labelDuration;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogMain;
        private System.Windows.Forms.ComboBox buttonOpen;
        private System.Windows.Forms.TrackBar trackBarMain;
    }
}
