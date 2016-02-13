namespace Nequeo.VoIP
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
            this.voIPControl1 = new Nequeo.VoIP.Sip.UI.VoIPControl();
            this.SuspendLayout();
            // 
            // voIPControl1
            // 
            this.voIPControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.voIPControl1.AudioRecordingInCall = false;
            this.voIPControl1.AudioRecordingInCallPath = null;
            this.voIPControl1.AudioRecordingOutCall = false;
            this.voIPControl1.AudioRecordingOutCallPath = null;
            this.voIPControl1.ContactsFilePath = null;
            this.voIPControl1.Location = new System.Drawing.Point(12, 12);
            this.voIPControl1.Name = "voIPControl1";
            this.voIPControl1.Size = new System.Drawing.Size(773, 328);
            this.voIPControl1.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 352);
            this.Controls.Add(this.voIPControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Nequeo VoIP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.ResumeLayout(false);

        }


        #endregion

        private Sip.UI.VoIPControl voIPControl1;
    }
}

