namespace Nequeo.VoIP.Sip.UI
{
    partial class Settings
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
            this.groupBoxAudioDevice = new System.Windows.Forms.GroupBox();
            this.labelAudioCaptureDevice = new System.Windows.Forms.Label();
            this.labelAudioPlaybackDevice = new System.Windows.Forms.Label();
            this.comboBoxAudioCaptureDevice = new System.Windows.Forms.ComboBox();
            this.comboBoxAudioPlaybackDevice = new System.Windows.Forms.ComboBox();
            this.groupBoxAudioDevice.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxAudioDevice
            // 
            this.groupBoxAudioDevice.Controls.Add(this.comboBoxAudioPlaybackDevice);
            this.groupBoxAudioDevice.Controls.Add(this.comboBoxAudioCaptureDevice);
            this.groupBoxAudioDevice.Controls.Add(this.labelAudioPlaybackDevice);
            this.groupBoxAudioDevice.Controls.Add(this.labelAudioCaptureDevice);
            this.groupBoxAudioDevice.Location = new System.Drawing.Point(12, 12);
            this.groupBoxAudioDevice.Name = "groupBoxAudioDevice";
            this.groupBoxAudioDevice.Size = new System.Drawing.Size(475, 84);
            this.groupBoxAudioDevice.TabIndex = 0;
            this.groupBoxAudioDevice.TabStop = false;
            this.groupBoxAudioDevice.Text = "Audio Device";
            // 
            // labelAudioCaptureDevice
            // 
            this.labelAudioCaptureDevice.AutoSize = true;
            this.labelAudioCaptureDevice.Location = new System.Drawing.Point(24, 22);
            this.labelAudioCaptureDevice.Name = "labelAudioCaptureDevice";
            this.labelAudioCaptureDevice.Size = new System.Drawing.Size(50, 13);
            this.labelAudioCaptureDevice.TabIndex = 0;
            this.labelAudioCaptureDevice.Text = "Capture :";
            // 
            // labelAudioPlaybackDevice
            // 
            this.labelAudioPlaybackDevice.AutoSize = true;
            this.labelAudioPlaybackDevice.Location = new System.Drawing.Point(24, 49);
            this.labelAudioPlaybackDevice.Name = "labelAudioPlaybackDevice";
            this.labelAudioPlaybackDevice.Size = new System.Drawing.Size(51, 13);
            this.labelAudioPlaybackDevice.TabIndex = 1;
            this.labelAudioPlaybackDevice.Text = "Playback";
            // 
            // comboBoxAudioCaptureDevice
            // 
            this.comboBoxAudioCaptureDevice.FormattingEnabled = true;
            this.comboBoxAudioCaptureDevice.Location = new System.Drawing.Point(80, 19);
            this.comboBoxAudioCaptureDevice.Name = "comboBoxAudioCaptureDevice";
            this.comboBoxAudioCaptureDevice.Size = new System.Drawing.Size(389, 21);
            this.comboBoxAudioCaptureDevice.TabIndex = 2;
            // 
            // comboBoxAudioPlaybackDevice
            // 
            this.comboBoxAudioPlaybackDevice.FormattingEnabled = true;
            this.comboBoxAudioPlaybackDevice.Location = new System.Drawing.Point(80, 46);
            this.comboBoxAudioPlaybackDevice.Name = "comboBoxAudioPlaybackDevice";
            this.comboBoxAudioPlaybackDevice.Size = new System.Drawing.Size(389, 21);
            this.comboBoxAudioPlaybackDevice.TabIndex = 3;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 354);
            this.Controls.Add(this.groupBoxAudioDevice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBoxAudioDevice.ResumeLayout(false);
            this.groupBoxAudioDevice.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAudioDevice;
        private System.Windows.Forms.ComboBox comboBoxAudioPlaybackDevice;
        private System.Windows.Forms.ComboBox comboBoxAudioCaptureDevice;
        private System.Windows.Forms.Label labelAudioPlaybackDevice;
        private System.Windows.Forms.Label labelAudioCaptureDevice;
    }
}