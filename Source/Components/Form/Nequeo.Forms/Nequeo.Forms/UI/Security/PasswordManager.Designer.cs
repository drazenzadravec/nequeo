namespace Nequeo.Forms.UI.Security
{
    partial class PasswordManager
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
            this.listBoxPasswordName = new System.Windows.Forms.ListBox();
            this.contextMenuNames = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeAccountNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnRemoveItem = new System.Windows.Forms.Button();
            this.groupBoxItem = new System.Windows.Forms.GroupBox();
            this.btnShowPassword = new System.Windows.Forms.Button();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblWebsite = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtDetails = new System.Windows.Forms.RichTextBox();
            this.txtWebsite = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.lblPasswordFile = new System.Windows.Forms.Label();
            this.txtPasswordFile = new System.Windows.Forms.TextBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.btnAuthenticate = new System.Windows.Forms.Button();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialogMain = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogMain = new System.Windows.Forms.SaveFileDialog();
            this.btnKeyboard = new System.Windows.Forms.Button();
            this.btnTransferPassword = new System.Windows.Forms.Button();
            this.contextMenuNames.SuspendLayout();
            this.groupBoxItem.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxPasswordName
            // 
            this.listBoxPasswordName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxPasswordName.ContextMenuStrip = this.contextMenuNames;
            this.listBoxPasswordName.Enabled = false;
            this.listBoxPasswordName.FormattingEnabled = true;
            this.listBoxPasswordName.HorizontalScrollbar = true;
            this.listBoxPasswordName.IntegralHeight = false;
            this.listBoxPasswordName.Location = new System.Drawing.Point(3, 3);
            this.listBoxPasswordName.Name = "listBoxPasswordName";
            this.listBoxPasswordName.Size = new System.Drawing.Size(237, 523);
            this.listBoxPasswordName.TabIndex = 0;
            this.listBoxPasswordName.SelectedIndexChanged += new System.EventHandler(this.listBoxPasswordName_SelectedIndexChanged);
            // 
            // contextMenuNames
            // 
            this.contextMenuNames.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeAccountNameToolStripMenuItem,
            this.sortByNameToolStripMenuItem});
            this.contextMenuNames.Name = "contextMenuNames";
            this.contextMenuNames.Size = new System.Drawing.Size(199, 48);
            // 
            // changeAccountNameToolStripMenuItem
            // 
            this.changeAccountNameToolStripMenuItem.Name = "changeAccountNameToolStripMenuItem";
            this.changeAccountNameToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.changeAccountNameToolStripMenuItem.Text = "Change Account Name";
            this.changeAccountNameToolStripMenuItem.Click += new System.EventHandler(this.changeAccountNameToolStripMenuItem_Click);
            // 
            // sortByNameToolStripMenuItem
            // 
            this.sortByNameToolStripMenuItem.Name = "sortByNameToolStripMenuItem";
            this.sortByNameToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.sortByNameToolStripMenuItem.Text = "Sort By Name";
            this.sortByNameToolStripMenuItem.Click += new System.EventHandler(this.sortByNameToolStripMenuItem_Click);
            // 
            // btnAddItem
            // 
            this.btnAddItem.Enabled = false;
            this.btnAddItem.Location = new System.Drawing.Point(246, 3);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(75, 23);
            this.btnAddItem.TabIndex = 0;
            this.btnAddItem.Text = "Add";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnRemoveItem
            // 
            this.btnRemoveItem.Enabled = false;
            this.btnRemoveItem.Location = new System.Drawing.Point(246, 32);
            this.btnRemoveItem.Name = "btnRemoveItem";
            this.btnRemoveItem.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveItem.TabIndex = 1;
            this.btnRemoveItem.Text = "Remove";
            this.btnRemoveItem.UseVisualStyleBackColor = true;
            this.btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
            // 
            // groupBoxItem
            // 
            this.groupBoxItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxItem.Controls.Add(this.btnTransferPassword);
            this.groupBoxItem.Controls.Add(this.btnShowPassword);
            this.groupBoxItem.Controls.Add(this.lblDetails);
            this.groupBoxItem.Controls.Add(this.lblWebsite);
            this.groupBoxItem.Controls.Add(this.lblEmail);
            this.groupBoxItem.Controls.Add(this.lblPassword);
            this.groupBoxItem.Controls.Add(this.txtDetails);
            this.groupBoxItem.Controls.Add(this.txtWebsite);
            this.groupBoxItem.Controls.Add(this.txtEmail);
            this.groupBoxItem.Controls.Add(this.txtPassword);
            this.groupBoxItem.Controls.Add(this.txtUsername);
            this.groupBoxItem.Controls.Add(this.lblUsername);
            this.groupBoxItem.Enabled = false;
            this.groupBoxItem.Location = new System.Drawing.Point(246, 61);
            this.groupBoxItem.Name = "groupBoxItem";
            this.groupBoxItem.Size = new System.Drawing.Size(524, 465);
            this.groupBoxItem.TabIndex = 3;
            this.groupBoxItem.TabStop = false;
            // 
            // btnShowPassword
            // 
            this.btnShowPassword.Location = new System.Drawing.Point(303, 49);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.Size = new System.Drawing.Size(57, 23);
            this.btnShowPassword.TabIndex = 10;
            this.btnShowPassword.Text = "Show";
            this.btnShowPassword.UseVisualStyleBackColor = true;
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(11, 132);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(45, 13);
            this.lblDetails.TabIndex = 9;
            this.lblDetails.Text = "Details :";
            // 
            // lblWebsite
            // 
            this.lblWebsite.AutoSize = true;
            this.lblWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblWebsite.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblWebsite.Location = new System.Drawing.Point(11, 106);
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.Size = new System.Drawing.Size(52, 13);
            this.lblWebsite.TabIndex = 8;
            this.lblWebsite.Text = "Website :";
            this.lblWebsite.Click += new System.EventHandler(this.lblWebsite_Click);
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblEmail.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblEmail.Location = new System.Drawing.Point(11, 80);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(38, 13);
            this.lblEmail.TabIndex = 7;
            this.lblEmail.Text = "Email :";
            this.lblEmail.Click += new System.EventHandler(this.lblEmail_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(11, 54);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(59, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password :";
            // 
            // txtDetails
            // 
            this.txtDetails.AcceptsTab = true;
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.EnableAutoDragDrop = true;
            this.txtDetails.Location = new System.Drawing.Point(97, 129);
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtDetails.ShowSelectionMargin = true;
            this.txtDetails.Size = new System.Drawing.Size(421, 330);
            this.txtDetails.TabIndex = 9;
            this.txtDetails.Text = "";
            this.txtDetails.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtDetails_LinkClicked);
            this.txtDetails.TextChanged += new System.EventHandler(this.txtDetails_TextChanged);
            // 
            // txtWebsite
            // 
            this.txtWebsite.Location = new System.Drawing.Point(97, 103);
            this.txtWebsite.Name = "txtWebsite";
            this.txtWebsite.Size = new System.Drawing.Size(421, 20);
            this.txtWebsite.TabIndex = 8;
            this.txtWebsite.TextChanged += new System.EventHandler(this.txtWebsite_TextChanged);
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(97, 77);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(263, 20);
            this.txtEmail.TabIndex = 7;
            this.txtEmail.TextChanged += new System.EventHandler(this.txtEmail_TextChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(97, 51);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            this.txtPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(97, 25);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(263, 20);
            this.txtUsername.TabIndex = 5;
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(11, 28);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(64, 13);
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username : ";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(340, 8);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(23, 13);
            this.lblFile.TabIndex = 4;
            this.lblFile.Text = "File";
            // 
            // lblPasswordFile
            // 
            this.lblPasswordFile.AutoSize = true;
            this.lblPasswordFile.Location = new System.Drawing.Point(340, 37);
            this.lblPasswordFile.Name = "lblPasswordFile";
            this.lblPasswordFile.Size = new System.Drawing.Size(53, 13);
            this.lblPasswordFile.TabIndex = 5;
            this.lblPasswordFile.Text = "Password";
            // 
            // txtPasswordFile
            // 
            this.txtPasswordFile.Location = new System.Drawing.Point(399, 34);
            this.txtPasswordFile.Name = "txtPasswordFile";
            this.txtPasswordFile.PasswordChar = '*';
            this.txtPasswordFile.Size = new System.Drawing.Size(144, 20);
            this.txtPasswordFile.TabIndex = 3;
            this.txtPasswordFile.TextChanged += new System.EventHandler(this.txtPasswordFile_TextChanged);
            this.txtPasswordFile.Enter += new System.EventHandler(this.txtPasswordFile_Enter);
            this.txtPasswordFile.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPasswordFile_KeyPress);
            this.txtPasswordFile.Leave += new System.EventHandler(this.txtPasswordFile_Leave);
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(399, 5);
            this.txtFile.Name = "txtFile";
            this.txtFile.ReadOnly = true;
            this.txtFile.Size = new System.Drawing.Size(335, 20);
            this.txtFile.TabIndex = 7;
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(740, 3);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(29, 23);
            this.btnFile.TabIndex = 2;
            this.btnFile.Text = "....";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // btnAuthenticate
            // 
            this.btnAuthenticate.Enabled = false;
            this.btnAuthenticate.Location = new System.Drawing.Point(694, 32);
            this.btnAuthenticate.Name = "btnAuthenticate";
            this.btnAuthenticate.Size = new System.Drawing.Size(75, 23);
            this.btnAuthenticate.TabIndex = 4;
            this.btnAuthenticate.Text = "Authenticate";
            this.btnAuthenticate.UseVisualStyleBackColor = true;
            this.btnAuthenticate.Click += new System.EventHandler(this.btnAuthenticate_Click);
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
            // btnKeyboard
            // 
            this.btnKeyboard.Location = new System.Drawing.Point(549, 32);
            this.btnKeyboard.Name = "btnKeyboard";
            this.btnKeyboard.Size = new System.Drawing.Size(68, 23);
            this.btnKeyboard.TabIndex = 8;
            this.btnKeyboard.Text = "Keyboard";
            this.btnKeyboard.UseVisualStyleBackColor = true;
            this.btnKeyboard.Click += new System.EventHandler(this.btnKeyboard_Click);
            // 
            // btnTransferPassword
            // 
            this.btnTransferPassword.Location = new System.Drawing.Point(366, 49);
            this.btnTransferPassword.Name = "btnTransferPassword";
            this.btnTransferPassword.Size = new System.Drawing.Size(57, 23);
            this.btnTransferPassword.TabIndex = 11;
            this.btnTransferPassword.Text = "Transfer";
            this.btnTransferPassword.UseVisualStyleBackColor = true;
            this.btnTransferPassword.Click += new System.EventHandler(this.btnTransferPassword_Click);
            // 
            // PasswordManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnKeyboard);
            this.Controls.Add(this.btnAuthenticate);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.txtPasswordFile);
            this.Controls.Add(this.lblPasswordFile);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.groupBoxItem);
            this.Controls.Add(this.btnRemoveItem);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.listBoxPasswordName);
            this.Name = "PasswordManager";
            this.Size = new System.Drawing.Size(773, 529);
            this.Load += new System.EventHandler(this.PasswordManager_Load);
            this.contextMenuNames.ResumeLayout(false);
            this.groupBoxItem.ResumeLayout(false);
            this.groupBoxItem.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxPasswordName;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnRemoveItem;
        private System.Windows.Forms.GroupBox groupBoxItem;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblPasswordFile;
        private System.Windows.Forms.TextBox txtPasswordFile;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.Button btnAuthenticate;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.RichTextBox txtDetails;
        private System.Windows.Forms.TextBox txtWebsite;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Label lblWebsite;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.OpenFileDialog openFileDialogMain;
        private System.Windows.Forms.SaveFileDialog saveFileDialogMain;
        private System.Windows.Forms.Button btnShowPassword;
        private System.Windows.Forms.ContextMenuStrip contextMenuNames;
        private System.Windows.Forms.ToolStripMenuItem changeAccountNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByNameToolStripMenuItem;
        private System.Windows.Forms.Button btnKeyboard;
        private System.Windows.Forms.Button btnTransferPassword;
    }
}
