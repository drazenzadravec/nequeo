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
            this.comboBoxAudioPlaybackDevice = new System.Windows.Forms.ComboBox();
            this.comboBoxAudioCaptureDevice = new System.Windows.Forms.ComboBox();
            this.labelAudioPlaybackDevice = new System.Windows.Forms.Label();
            this.labelAudioCaptureDevice = new System.Windows.Forms.Label();
            this.groupBoxPublish = new System.Windows.Forms.GroupBox();
            this.groupBoxAccount = new System.Windows.Forms.GroupBox();
            this.labelSipPort = new System.Windows.Forms.Label();
            this.textBoxSipPort = new System.Windows.Forms.TextBox();
            this.checkBoxIsDefault = new System.Windows.Forms.CheckBox();
            this.labelPriority = new System.Windows.Forms.Label();
            this.textBoxPriority = new System.Windows.Forms.TextBox();
            this.checkBoxDropCallsOnFail = new System.Windows.Forms.CheckBox();
            this.groupBoxAudioDevice.SuspendLayout();
            this.groupBoxAccount.SuspendLayout();
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
            // comboBoxAudioPlaybackDevice
            // 
            this.comboBoxAudioPlaybackDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAudioPlaybackDevice.FormattingEnabled = true;
            this.comboBoxAudioPlaybackDevice.Location = new System.Drawing.Point(78, 46);
            this.comboBoxAudioPlaybackDevice.Name = "comboBoxAudioPlaybackDevice";
            this.comboBoxAudioPlaybackDevice.Size = new System.Drawing.Size(391, 21);
            this.comboBoxAudioPlaybackDevice.TabIndex = 3;
            this.comboBoxAudioPlaybackDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxAudioPlaybackDevice_SelectedIndexChanged);
            // 
            // comboBoxAudioCaptureDevice
            // 
            this.comboBoxAudioCaptureDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAudioCaptureDevice.FormattingEnabled = true;
            this.comboBoxAudioCaptureDevice.Location = new System.Drawing.Point(78, 19);
            this.comboBoxAudioCaptureDevice.Name = "comboBoxAudioCaptureDevice";
            this.comboBoxAudioCaptureDevice.Size = new System.Drawing.Size(391, 21);
            this.comboBoxAudioCaptureDevice.TabIndex = 2;
            this.comboBoxAudioCaptureDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxAudioCaptureDevice_SelectedIndexChanged);
            // 
            // labelAudioPlaybackDevice
            // 
            this.labelAudioPlaybackDevice.AutoSize = true;
            this.labelAudioPlaybackDevice.Location = new System.Drawing.Point(15, 49);
            this.labelAudioPlaybackDevice.Name = "labelAudioPlaybackDevice";
            this.labelAudioPlaybackDevice.Size = new System.Drawing.Size(57, 13);
            this.labelAudioPlaybackDevice.TabIndex = 1;
            this.labelAudioPlaybackDevice.Text = "Playback :";
            // 
            // labelAudioCaptureDevice
            // 
            this.labelAudioCaptureDevice.AutoSize = true;
            this.labelAudioCaptureDevice.Location = new System.Drawing.Point(15, 22);
            this.labelAudioCaptureDevice.Name = "labelAudioCaptureDevice";
            this.labelAudioCaptureDevice.Size = new System.Drawing.Size(50, 13);
            this.labelAudioCaptureDevice.TabIndex = 0;
            this.labelAudioCaptureDevice.Text = "Capture :";
            // 
            // groupBoxPublish
            // 
            this.groupBoxPublish.Location = new System.Drawing.Point(287, 102);
            this.groupBoxPublish.Name = "groupBoxPublish";
            this.groupBoxPublish.Size = new System.Drawing.Size(200, 165);
            this.groupBoxPublish.TabIndex = 1;
            this.groupBoxPublish.TabStop = false;
            this.groupBoxPublish.Text = "Publish";
            // 
            // groupBoxAccount
            // 
            this.groupBoxAccount.Controls.Add(this.checkBoxDropCallsOnFail);
            this.groupBoxAccount.Controls.Add(this.textBoxPriority);
            this.groupBoxAccount.Controls.Add(this.labelPriority);
            this.groupBoxAccount.Controls.Add(this.checkBoxIsDefault);
            this.groupBoxAccount.Controls.Add(this.textBoxSipPort);
            this.groupBoxAccount.Controls.Add(this.labelSipPort);
            this.groupBoxAccount.Location = new System.Drawing.Point(12, 102);
            this.groupBoxAccount.Name = "groupBoxAccount";
            this.groupBoxAccount.Size = new System.Drawing.Size(269, 165);
            this.groupBoxAccount.TabIndex = 2;
            this.groupBoxAccount.TabStop = false;
            this.groupBoxAccount.Text = "Account";
            // 
            // labelSipPort
            // 
            this.labelSipPort.AutoSize = true;
            this.labelSipPort.Location = new System.Drawing.Point(15, 22);
            this.labelSipPort.Name = "labelSipPort";
            this.labelSipPort.Size = new System.Drawing.Size(32, 13);
            this.labelSipPort.TabIndex = 0;
            this.labelSipPort.Text = "Port :";
            // 
            // textBoxSipPort
            // 
            this.textBoxSipPort.Location = new System.Drawing.Point(65, 19);
            this.textBoxSipPort.Name = "textBoxSipPort";
            this.textBoxSipPort.Size = new System.Drawing.Size(59, 20);
            this.textBoxSipPort.TabIndex = 1;
            this.textBoxSipPort.TextChanged += new System.EventHandler(this.textBoxSipPort_TextChanged);
            // 
            // checkBoxIsDefault
            // 
            this.checkBoxIsDefault.AutoSize = true;
            this.checkBoxIsDefault.Location = new System.Drawing.Point(65, 45);
            this.checkBoxIsDefault.Name = "checkBoxIsDefault";
            this.checkBoxIsDefault.Size = new System.Drawing.Size(71, 17);
            this.checkBoxIsDefault.TabIndex = 3;
            this.checkBoxIsDefault.Text = "Is Default";
            this.checkBoxIsDefault.UseVisualStyleBackColor = true;
            this.checkBoxIsDefault.CheckedChanged += new System.EventHandler(this.checkBoxIsDefault_CheckedChanged);
            // 
            // labelPriority
            // 
            this.labelPriority.AutoSize = true;
            this.labelPriority.Location = new System.Drawing.Point(15, 71);
            this.labelPriority.Name = "labelPriority";
            this.labelPriority.Size = new System.Drawing.Size(44, 13);
            this.labelPriority.TabIndex = 4;
            this.labelPriority.Text = "Priority :";
            // 
            // textBoxPriority
            // 
            this.textBoxPriority.Location = new System.Drawing.Point(65, 68);
            this.textBoxPriority.Name = "textBoxPriority";
            this.textBoxPriority.Size = new System.Drawing.Size(59, 20);
            this.textBoxPriority.TabIndex = 5;
            this.textBoxPriority.TextChanged += new System.EventHandler(this.textBoxPriority_TextChanged);
            // 
            // checkBoxDropCallsOnFail
            // 
            this.checkBoxDropCallsOnFail.AutoSize = true;
            this.checkBoxDropCallsOnFail.Location = new System.Drawing.Point(153, 21);
            this.checkBoxDropCallsOnFail.Name = "checkBoxDropCallsOnFail";
            this.checkBoxDropCallsOnFail.Size = new System.Drawing.Size(110, 17);
            this.checkBoxDropCallsOnFail.TabIndex = 6;
            this.checkBoxDropCallsOnFail.Text = "Drop Calls On Fail";
            this.checkBoxDropCallsOnFail.UseVisualStyleBackColor = true;
            this.checkBoxDropCallsOnFail.CheckedChanged += new System.EventHandler(this.checkBoxDropCallsOnFail_CheckedChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 354);
            this.Controls.Add(this.groupBoxAccount);
            this.Controls.Add(this.groupBoxPublish);
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
            this.groupBoxAccount.ResumeLayout(false);
            this.groupBoxAccount.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAudioDevice;
        private System.Windows.Forms.ComboBox comboBoxAudioPlaybackDevice;
        private System.Windows.Forms.ComboBox comboBoxAudioCaptureDevice;
        private System.Windows.Forms.Label labelAudioPlaybackDevice;
        private System.Windows.Forms.Label labelAudioCaptureDevice;
        private System.Windows.Forms.GroupBox groupBoxPublish;
        private System.Windows.Forms.GroupBox groupBoxAccount;
        private System.Windows.Forms.CheckBox checkBoxIsDefault;
        private System.Windows.Forms.TextBox textBoxSipPort;
        private System.Windows.Forms.Label labelSipPort;
        private System.Windows.Forms.TextBox textBoxPriority;
        private System.Windows.Forms.Label labelPriority;
        private System.Windows.Forms.CheckBox checkBoxDropCallsOnFail;
    }
}