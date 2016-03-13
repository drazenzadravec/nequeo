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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.voIPControl1 = new Nequeo.VoIP.PjSip.UI.VoIPControl();
            this.tabControlVoIP = new System.Windows.Forms.TabControl();
            this.tabPageVoIP1 = new System.Windows.Forms.TabPage();
            this.tabPageVoIP2 = new System.Windows.Forms.TabPage();
            this.voIPControl2 = new Nequeo.VoIP.Sip.UI.VoIPControl();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.addVoIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.voiceVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.voiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorSep = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlVoIP.SuspendLayout();
            this.tabPageVoIP1.SuspendLayout();
            this.tabPageVoIP2.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.contextMenuStripNotify.SuspendLayout();
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
            this.voIPControl1.Location = new System.Drawing.Point(3, 3);
            this.voIPControl1.Name = "voIPControl1";
            this.voIPControl1.Size = new System.Drawing.Size(387, 375);
            this.voIPControl1.TabIndex = 0;
            // 
            // tabControlVoIP
            // 
            this.tabControlVoIP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlVoIP.Controls.Add(this.tabPageVoIP1);
            this.tabControlVoIP.Controls.Add(this.tabPageVoIP2);
            this.tabControlVoIP.Location = new System.Drawing.Point(12, 27);
            this.tabControlVoIP.Name = "tabControlVoIP";
            this.tabControlVoIP.SelectedIndex = 0;
            this.tabControlVoIP.Size = new System.Drawing.Size(401, 407);
            this.tabControlVoIP.TabIndex = 1;
            this.tabControlVoIP.SelectedIndexChanged += new System.EventHandler(this.tabControlVoIP_SelectedIndexChanged);
            // 
            // tabPageVoIP1
            // 
            this.tabPageVoIP1.Controls.Add(this.voIPControl1);
            this.tabPageVoIP1.Location = new System.Drawing.Point(4, 22);
            this.tabPageVoIP1.Name = "tabPageVoIP1";
            this.tabPageVoIP1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageVoIP1.Size = new System.Drawing.Size(393, 381);
            this.tabPageVoIP1.TabIndex = 0;
            this.tabPageVoIP1.Text = "VoIP Account Voice & Video";
            this.tabPageVoIP1.UseVisualStyleBackColor = true;
            // 
            // tabPageVoIP2
            // 
            this.tabPageVoIP2.Controls.Add(this.voIPControl2);
            this.tabPageVoIP2.Location = new System.Drawing.Point(4, 22);
            this.tabPageVoIP2.Name = "tabPageVoIP2";
            this.tabPageVoIP2.Size = new System.Drawing.Size(393, 381);
            this.tabPageVoIP2.TabIndex = 1;
            this.tabPageVoIP2.Text = "VoIP Account Voice";
            this.tabPageVoIP2.UseVisualStyleBackColor = true;
            // 
            // voIPControl2
            // 
            this.voIPControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.voIPControl2.AudioRecordingInCall = false;
            this.voIPControl2.AudioRecordingInCallPath = null;
            this.voIPControl2.AudioRecordingOutCall = false;
            this.voIPControl2.AudioRecordingOutCallPath = null;
            this.voIPControl2.ContactsFilePath = null;
            this.voIPControl2.Location = new System.Drawing.Point(3, 3);
            this.voIPControl2.Name = "voIPControl2";
            this.voIPControl2.Size = new System.Drawing.Size(387, 375);
            this.voIPControl2.TabIndex = 0;
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(425, 24);
            this.menuStripMain.TabIndex = 2;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemClose,
            this.addVoIPToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuItemClose.Text = "Close";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // addVoIPToolStripMenuItem
            // 
            this.addVoIPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.voiceVideoToolStripMenuItem,
            this.voiceToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.addVoIPToolStripMenuItem.Name = "addVoIPToolStripMenuItem";
            this.addVoIPToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addVoIPToolStripMenuItem.Text = "VoIP";
            this.addVoIPToolStripMenuItem.Click += new System.EventHandler(this.addVoIPToolStripMenuItem_Click);
            // 
            // voiceVideoToolStripMenuItem
            // 
            this.voiceVideoToolStripMenuItem.Name = "voiceVideoToolStripMenuItem";
            this.voiceVideoToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.voiceVideoToolStripMenuItem.Text = "Add Voice and Video";
            this.voiceVideoToolStripMenuItem.Click += new System.EventHandler(this.voiceVideoToolStripMenuItem_Click);
            // 
            // voiceToolStripMenuItem
            // 
            this.voiceToolStripMenuItem.Name = "voiceToolStripMenuItem";
            this.voiceToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.voiceToolStripMenuItem.Text = "Add Voice";
            this.voiceToolStripMenuItem.Visible = false;
            this.voiceToolStripMenuItem.Click += new System.EventHandler(this.voiceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(119, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // notifyIconMain
            // 
            this.notifyIconMain.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIconMain.BalloonTipText = "Nequeo voip application capable of voice and video communication.";
            this.notifyIconMain.BalloonTipTitle = "Nequeo VoIP";
            this.notifyIconMain.ContextMenuStrip = this.contextMenuStripNotify;
            this.notifyIconMain.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconMain.Icon")));
            this.notifyIconMain.Text = "Nequeo VoIP";
            this.notifyIconMain.Visible = true;
            // 
            // contextMenuStripNotify
            // 
            this.contextMenuStripNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemOpen,
            this.toolStripMenuItemAbout,
            this.toolStripSeparatorSep,
            this.toolStripMenuItemExit});
            this.contextMenuStripNotify.Name = "contextMenuStripNotify";
            this.contextMenuStripNotify.Size = new System.Drawing.Size(108, 76);
            // 
            // toolStripMenuItemOpen
            // 
            this.toolStripMenuItemOpen.Name = "toolStripMenuItemOpen";
            this.toolStripMenuItemOpen.Size = new System.Drawing.Size(107, 22);
            this.toolStripMenuItemOpen.Text = "Open";
            this.toolStripMenuItemOpen.Click += new System.EventHandler(this.toolStripMenuItemOpen_Click);
            // 
            // toolStripMenuItemAbout
            // 
            this.toolStripMenuItemAbout.Name = "toolStripMenuItemAbout";
            this.toolStripMenuItemAbout.Size = new System.Drawing.Size(107, 22);
            this.toolStripMenuItemAbout.Text = "About";
            this.toolStripMenuItemAbout.Click += new System.EventHandler(this.toolStripMenuItemAbout_Click);
            // 
            // toolStripSeparatorSep
            // 
            this.toolStripSeparatorSep.Name = "toolStripSeparatorSep";
            this.toolStripSeparatorSep.Size = new System.Drawing.Size(104, 6);
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(107, 22);
            this.toolStripMenuItemExit.Text = "Exit";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.toolStripMenuItemExit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(180, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 446);
            this.Controls.Add(this.tabControlVoIP);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "Main";
            this.Text = "Nequeo VoIP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControlVoIP.ResumeLayout(false);
            this.tabPageVoIP1.ResumeLayout(false);
            this.tabPageVoIP2.ResumeLayout(false);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.contextMenuStripNotify.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private PjSip.UI.VoIPControl voIPControl1;
        private System.Windows.Forms.TabControl tabControlVoIP;
        private System.Windows.Forms.TabPage tabPageVoIP1;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addVoIPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem voiceVideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem voiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageVoIP2;
        private Sip.UI.VoIPControl voIPControl2;
        private System.Windows.Forms.NotifyIcon notifyIconMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotify;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClose;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorSep;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
    }
}

