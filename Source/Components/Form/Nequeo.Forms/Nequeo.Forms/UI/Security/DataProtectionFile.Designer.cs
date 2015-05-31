namespace Nequeo.Forms.UI.Security
{
    partial class DataProtectionFile
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataProtectionFile));
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.txtEncryptedFilePath = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxLogin = new System.Windows.Forms.GroupBox();
            this.btnCreateFile = new System.Windows.Forms.Button();
            this.btnEncryptedPath = new System.Windows.Forms.Button();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblEncryptedFilePath = new System.Windows.Forms.Label();
            this.groupBoxData = new System.Windows.Forms.GroupBox();
            this.toolStripRichText = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSelectAll = new System.Windows.Forms.ToolStripButton();
            this.richTextBoxData = new System.Windows.Forms.RichTextBox();
            this.openFileDialogMain = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogMain = new System.Windows.Forms.SaveFileDialog();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.groupBoxLogin.SuspendLayout();
            this.groupBoxData.SuspendLayout();
            this.toolStripRichText.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtEncryptedFilePath
            // 
            this.txtEncryptedFilePath.Location = new System.Drawing.Point(117, 22);
            this.txtEncryptedFilePath.Name = "txtEncryptedFilePath";
            this.txtEncryptedFilePath.ReadOnly = true;
            this.txtEncryptedFilePath.Size = new System.Drawing.Size(392, 20);
            this.txtEncryptedFilePath.TabIndex = 1;
            this.toolTipMain.SetToolTip(this.txtEncryptedFilePath, "Enter your login username.");
            this.txtEncryptedFilePath.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(117, 50);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(176, 20);
            this.txtPassword.TabIndex = 3;
            this.toolTipMain.SetToolTip(this.txtPassword, "Enter your file password.");
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // btnLogin
            // 
            this.btnLogin.Enabled = false;
            this.btnLogin.Location = new System.Drawing.Point(469, 48);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Authenticate";
            this.toolTipMain.SetToolTip(this.btnLogin, "Authenticate user.");
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(388, 48);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.toolTipMain.SetToolTip(this.btnCancel, "Cancel operation.");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBoxLogin
            // 
            this.groupBoxLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLogin.Controls.Add(this.btnCreateFile);
            this.groupBoxLogin.Controls.Add(this.btnEncryptedPath);
            this.groupBoxLogin.Controls.Add(this.btnCancel);
            this.groupBoxLogin.Controls.Add(this.btnLogin);
            this.groupBoxLogin.Controls.Add(this.txtPassword);
            this.groupBoxLogin.Controls.Add(this.lblPassword);
            this.groupBoxLogin.Controls.Add(this.txtEncryptedFilePath);
            this.groupBoxLogin.Controls.Add(this.lblEncryptedFilePath);
            this.groupBoxLogin.Location = new System.Drawing.Point(3, 3);
            this.groupBoxLogin.Name = "groupBoxLogin";
            this.groupBoxLogin.Size = new System.Drawing.Size(555, 80);
            this.groupBoxLogin.TabIndex = 0;
            this.groupBoxLogin.TabStop = false;
            this.groupBoxLogin.Text = "Authenticate";
            // 
            // btnCreateFile
            // 
            this.btnCreateFile.Location = new System.Drawing.Point(307, 48);
            this.btnCreateFile.Name = "btnCreateFile";
            this.btnCreateFile.Size = new System.Drawing.Size(75, 23);
            this.btnCreateFile.TabIndex = 6;
            this.btnCreateFile.Text = "Create File";
            this.btnCreateFile.UseVisualStyleBackColor = true;
            this.btnCreateFile.Click += new System.EventHandler(this.btnCreateFile_Click);
            // 
            // btnEncryptedPath
            // 
            this.btnEncryptedPath.Location = new System.Drawing.Point(515, 20);
            this.btnEncryptedPath.Name = "btnEncryptedPath";
            this.btnEncryptedPath.Size = new System.Drawing.Size(29, 23);
            this.btnEncryptedPath.TabIndex = 2;
            this.btnEncryptedPath.Text = "....";
            this.btnEncryptedPath.UseVisualStyleBackColor = true;
            this.btnEncryptedPath.Click += new System.EventHandler(this.btnEncryptedPath_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(6, 53);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(78, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "File Password :";
            // 
            // lblEncryptedFilePath
            // 
            this.lblEncryptedFilePath.AutoSize = true;
            this.lblEncryptedFilePath.Location = new System.Drawing.Point(6, 25);
            this.lblEncryptedFilePath.Name = "lblEncryptedFilePath";
            this.lblEncryptedFilePath.Size = new System.Drawing.Size(105, 13);
            this.lblEncryptedFilePath.TabIndex = 0;
            this.lblEncryptedFilePath.Text = "Encrypted File Path :";
            // 
            // groupBoxData
            // 
            this.groupBoxData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxData.Controls.Add(this.toolStripRichText);
            this.groupBoxData.Controls.Add(this.richTextBoxData);
            this.groupBoxData.Location = new System.Drawing.Point(3, 89);
            this.groupBoxData.Name = "groupBoxData";
            this.groupBoxData.Size = new System.Drawing.Size(555, 168);
            this.groupBoxData.TabIndex = 6;
            this.groupBoxData.TabStop = false;
            this.groupBoxData.Text = "Data";
            // 
            // toolStripRichText
            // 
            this.toolStripRichText.Enabled = false;
            this.toolStripRichText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRedo,
            this.toolStripButtonUndo,
            this.toolStripSeparator1,
            this.toolStripButtonCopy,
            this.toolStripButtonCut,
            this.toolStripButtonPaste,
            this.toolStripSeparator2,
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomOut,
            this.toolStripSeparator3,
            this.toolStripButtonSelectAll,
            this.toolStripButtonSave});
            this.toolStripRichText.Location = new System.Drawing.Point(3, 16);
            this.toolStripRichText.Name = "toolStripRichText";
            this.toolStripRichText.Size = new System.Drawing.Size(549, 25);
            this.toolStripRichText.TabIndex = 8;
            this.toolStripRichText.Text = "toolStrip1";
            // 
            // toolStripButtonRedo
            // 
            this.toolStripButtonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRedo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRedo.Image")));
            this.toolStripButtonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRedo.Name = "toolStripButtonRedo";
            this.toolStripButtonRedo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRedo.Text = "Redo";
            this.toolStripButtonRedo.Click += new System.EventHandler(this.toolStripButtonRedo_Click);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndo.Image")));
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndo.Text = "Undo";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCopy
            // 
            this.toolStripButtonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCopy.Image")));
            this.toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCopy.Name = "toolStripButtonCopy";
            this.toolStripButtonCopy.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCopy.Text = "Copy";
            this.toolStripButtonCopy.Click += new System.EventHandler(this.toolStripButtonCopy_Click);
            // 
            // toolStripButtonCut
            // 
            this.toolStripButtonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCut.Image")));
            this.toolStripButtonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCut.Name = "toolStripButtonCut";
            this.toolStripButtonCut.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCut.Text = "Cut";
            this.toolStripButtonCut.Click += new System.EventHandler(this.toolStripButtonCut_Click);
            // 
            // toolStripButtonPaste
            // 
            this.toolStripButtonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPaste.Image")));
            this.toolStripButtonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPaste.Name = "toolStripButtonPaste";
            this.toolStripButtonPaste.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPaste.Text = "Paste";
            this.toolStripButtonPaste.Click += new System.EventHandler(this.toolStripButtonPaste_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonZoomIn
            // 
            this.toolStripButtonZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomIn.Image")));
            this.toolStripButtonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
            this.toolStripButtonZoomIn.Size = new System.Drawing.Size(72, 22);
            this.toolStripButtonZoomIn.Text = "Zoom In";
            this.toolStripButtonZoomIn.Click += new System.EventHandler(this.toolStripButtonZoomIn_Click);
            // 
            // toolStripButtonZoomOut
            // 
            this.toolStripButtonZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomOut.Image")));
            this.toolStripButtonZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomOut.Name = "toolStripButtonZoomOut";
            this.toolStripButtonZoomOut.Size = new System.Drawing.Size(82, 22);
            this.toolStripButtonZoomOut.Text = "Zoom Out";
            this.toolStripButtonZoomOut.Click += new System.EventHandler(this.toolStripButtonZoomOut_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSelectAll
            // 
            this.toolStripButtonSelectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSelectAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelectAll.Image")));
            this.toolStripButtonSelectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelectAll.Name = "toolStripButtonSelectAll";
            this.toolStripButtonSelectAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSelectAll.Text = "Select All";
            this.toolStripButtonSelectAll.Click += new System.EventHandler(this.toolStripButtonSelectAll_Click);
            // 
            // richTextBoxData
            // 
            this.richTextBoxData.AcceptsTab = true;
            this.richTextBoxData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxData.EnableAutoDragDrop = true;
            this.richTextBoxData.Enabled = false;
            this.richTextBoxData.Location = new System.Drawing.Point(6, 44);
            this.richTextBoxData.Name = "richTextBoxData";
            this.richTextBoxData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBoxData.ShowSelectionMargin = true;
            this.richTextBoxData.Size = new System.Drawing.Size(543, 118);
            this.richTextBoxData.TabIndex = 7;
            this.richTextBoxData.Text = "";
            this.richTextBoxData.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxData_LinkClicked);
            // 
            // openFileDialogMain
            // 
            this.openFileDialogMain.DefaultExt = "*.txt";
            this.openFileDialogMain.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            this.openFileDialogMain.SupportMultiDottedExtensions = true;
            this.openFileDialogMain.Title = "Open Encrypted File";
            // 
            // saveFileDialogMain
            // 
            this.saveFileDialogMain.DefaultExt = "*.txt";
            this.saveFileDialogMain.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            this.saveFileDialogMain.SupportMultiDottedExtensions = true;
            this.saveFileDialogMain.Title = "Create New File";
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // DataProtectionFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxData);
            this.Controls.Add(this.groupBoxLogin);
            this.Name = "DataProtectionFile";
            this.Size = new System.Drawing.Size(561, 260);
            this.Load += new System.EventHandler(this.DataProtection_Load);
            this.groupBoxLogin.ResumeLayout(false);
            this.groupBoxLogin.PerformLayout();
            this.groupBoxData.ResumeLayout(false);
            this.groupBoxData.PerformLayout();
            this.toolStripRichText.ResumeLayout(false);
            this.toolStripRichText.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.GroupBox groupBoxLogin;
        private System.Windows.Forms.GroupBox groupBoxData;
        private System.Windows.Forms.RichTextBox richTextBoxData;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtEncryptedFilePath;
        private System.Windows.Forms.Label lblEncryptedFilePath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnEncryptedPath;
        private System.Windows.Forms.Button btnCreateFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogMain;
        private System.Windows.Forms.SaveFileDialog saveFileDialogMain;
        private System.Windows.Forms.ToolStrip toolStripRichText;
        private System.Windows.Forms.ToolStripButton toolStripButtonRedo;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
        private System.Windows.Forms.ToolStripButton toolStripButtonCut;
        private System.Windows.Forms.ToolStripButton toolStripButtonPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelectAll;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
    }
}
