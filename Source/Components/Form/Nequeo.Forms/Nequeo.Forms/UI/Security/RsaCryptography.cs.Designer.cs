namespace Nequeo.Forms.UI.Security
{
    partial class RsaCryptography
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
            this.btnCryptExecute = new System.Windows.Forms.Button();
            this.btnDecryptLocalFile = new System.Windows.Forms.Button();
            this.btnEncryptLocalFile = new System.Windows.Forms.Button();
            this.openFileDialogMain = new System.Windows.Forms.OpenFileDialog();
            this.txtDecryptedLocalFile = new System.Windows.Forms.TextBox();
            this.txtEncryptLocalFile = new System.Windows.Forms.TextBox();
            this.lblDecryptLocalFile = new System.Windows.Forms.Label();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.saveFileDialogMain = new System.Windows.Forms.SaveFileDialog();
            this.lblEncryptLocalFile = new System.Windows.Forms.Label();
            this.cboOperation = new System.Windows.Forms.ComboBox();
            this.lblOperation = new System.Windows.Forms.Label();
            this.lblCertificatePath = new System.Windows.Forms.Label();
            this.txtCertificatePath = new System.Windows.Forms.TextBox();
            this.lblCertificatePassword = new System.Windows.Forms.Label();
            this.txtCertificatePassword = new System.Windows.Forms.TextBox();
            this.btnCertificatePath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCryptExecute
            // 
            this.btnCryptExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCryptExecute.Location = new System.Drawing.Point(284, 137);
            this.btnCryptExecute.Name = "btnCryptExecute";
            this.btnCryptExecute.Size = new System.Drawing.Size(75, 23);
            this.btnCryptExecute.TabIndex = 21;
            this.btnCryptExecute.Text = "Execute";
            this.btnCryptExecute.UseVisualStyleBackColor = true;
            this.btnCryptExecute.Click += new System.EventHandler(this.btnCryptExecute_Click);
            // 
            // btnDecryptLocalFile
            // 
            this.btnDecryptLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDecryptLocalFile.Location = new System.Drawing.Point(330, 54);
            this.btnDecryptLocalFile.Name = "btnDecryptLocalFile";
            this.btnDecryptLocalFile.Size = new System.Drawing.Size(29, 23);
            this.btnDecryptLocalFile.TabIndex = 20;
            this.btnDecryptLocalFile.Text = "....";
            this.btnDecryptLocalFile.UseVisualStyleBackColor = true;
            this.btnDecryptLocalFile.Click += new System.EventHandler(this.btnDecryptLocalFile_Click);
            // 
            // btnEncryptLocalFile
            // 
            this.btnEncryptLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEncryptLocalFile.Location = new System.Drawing.Point(330, 28);
            this.btnEncryptLocalFile.Name = "btnEncryptLocalFile";
            this.btnEncryptLocalFile.Size = new System.Drawing.Size(29, 23);
            this.btnEncryptLocalFile.TabIndex = 19;
            this.btnEncryptLocalFile.Text = "....";
            this.btnEncryptLocalFile.UseVisualStyleBackColor = true;
            this.btnEncryptLocalFile.Click += new System.EventHandler(this.btnEncryptLocalFile_Click);
            // 
            // openFileDialogMain
            // 
            this.openFileDialogMain.DefaultExt = "*.*";
            this.openFileDialogMain.Filter = "All files (*.*)|*.*";
            this.openFileDialogMain.SupportMultiDottedExtensions = true;
            this.openFileDialogMain.Title = "Open Encrypted/Decrypted File";
            // 
            // txtDecryptedLocalFile
            // 
            this.txtDecryptedLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDecryptedLocalFile.Location = new System.Drawing.Point(112, 56);
            this.txtDecryptedLocalFile.Name = "txtDecryptedLocalFile";
            this.txtDecryptedLocalFile.Size = new System.Drawing.Size(212, 20);
            this.txtDecryptedLocalFile.TabIndex = 17;
            // 
            // txtEncryptLocalFile
            // 
            this.txtEncryptLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEncryptLocalFile.Location = new System.Drawing.Point(112, 30);
            this.txtEncryptLocalFile.Name = "txtEncryptLocalFile";
            this.txtEncryptLocalFile.Size = new System.Drawing.Size(212, 20);
            this.txtEncryptLocalFile.TabIndex = 16;
            // 
            // lblDecryptLocalFile
            // 
            this.lblDecryptLocalFile.AutoSize = true;
            this.lblDecryptLocalFile.Location = new System.Drawing.Point(3, 59);
            this.lblDecryptLocalFile.Name = "lblDecryptLocalFile";
            this.lblDecryptLocalFile.Size = new System.Drawing.Size(75, 13);
            this.lblDecryptLocalFile.TabIndex = 14;
            this.lblDecryptLocalFile.Text = "Decrypted File";
            // 
            // saveFileDialogMain
            // 
            this.saveFileDialogMain.DefaultExt = "*.*";
            this.saveFileDialogMain.Filter = "All files (*.*)|*.*";
            this.saveFileDialogMain.SupportMultiDottedExtensions = true;
            this.saveFileDialogMain.Title = "Save Encrypted/Decrypted File";
            // 
            // lblEncryptLocalFile
            // 
            this.lblEncryptLocalFile.AutoSize = true;
            this.lblEncryptLocalFile.Location = new System.Drawing.Point(3, 33);
            this.lblEncryptLocalFile.Name = "lblEncryptLocalFile";
            this.lblEncryptLocalFile.Size = new System.Drawing.Size(74, 13);
            this.lblEncryptLocalFile.TabIndex = 13;
            this.lblEncryptLocalFile.Text = "Encrypted File";
            // 
            // cboOperation
            // 
            this.cboOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOperation.FormattingEnabled = true;
            this.cboOperation.Location = new System.Drawing.Point(112, 3);
            this.cboOperation.Name = "cboOperation";
            this.cboOperation.Size = new System.Drawing.Size(247, 21);
            this.cboOperation.TabIndex = 12;
            this.cboOperation.SelectedIndexChanged += new System.EventHandler(this.cboOperation_SelectedIndexChanged);
            // 
            // lblOperation
            // 
            this.lblOperation.AutoSize = true;
            this.lblOperation.Location = new System.Drawing.Point(3, 6);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.Size = new System.Drawing.Size(53, 13);
            this.lblOperation.TabIndex = 11;
            this.lblOperation.Text = "Operation";
            // 
            // lblCertificatePath
            // 
            this.lblCertificatePath.AutoSize = true;
            this.lblCertificatePath.Location = new System.Drawing.Point(3, 85);
            this.lblCertificatePath.Name = "lblCertificatePath";
            this.lblCertificatePath.Size = new System.Drawing.Size(79, 13);
            this.lblCertificatePath.TabIndex = 22;
            this.lblCertificatePath.Text = "Certificate Path";
            // 
            // txtCertificatePath
            // 
            this.txtCertificatePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCertificatePath.Location = new System.Drawing.Point(112, 82);
            this.txtCertificatePath.Name = "txtCertificatePath";
            this.txtCertificatePath.ReadOnly = true;
            this.txtCertificatePath.Size = new System.Drawing.Size(212, 20);
            this.txtCertificatePath.TabIndex = 23;
            // 
            // lblCertificatePassword
            // 
            this.lblCertificatePassword.AutoSize = true;
            this.lblCertificatePassword.Location = new System.Drawing.Point(3, 114);
            this.lblCertificatePassword.Name = "lblCertificatePassword";
            this.lblCertificatePassword.Size = new System.Drawing.Size(103, 13);
            this.lblCertificatePassword.TabIndex = 24;
            this.lblCertificatePassword.Text = "Certificate Password";
            // 
            // txtCertificatePassword
            // 
            this.txtCertificatePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCertificatePassword.Location = new System.Drawing.Point(112, 111);
            this.txtCertificatePassword.Name = "txtCertificatePassword";
            this.txtCertificatePassword.PasswordChar = '*';
            this.txtCertificatePassword.Size = new System.Drawing.Size(212, 20);
            this.txtCertificatePassword.TabIndex = 25;
            // 
            // btnCertificatePath
            // 
            this.btnCertificatePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCertificatePath.Location = new System.Drawing.Point(330, 80);
            this.btnCertificatePath.Name = "btnCertificatePath";
            this.btnCertificatePath.Size = new System.Drawing.Size(29, 23);
            this.btnCertificatePath.TabIndex = 26;
            this.btnCertificatePath.Text = "....";
            this.btnCertificatePath.UseVisualStyleBackColor = true;
            this.btnCertificatePath.Click += new System.EventHandler(this.btnCertificatePath_Click);
            // 
            // RsaCryptography
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCertificatePath);
            this.Controls.Add(this.txtCertificatePassword);
            this.Controls.Add(this.lblCertificatePassword);
            this.Controls.Add(this.txtCertificatePath);
            this.Controls.Add(this.lblCertificatePath);
            this.Controls.Add(this.btnCryptExecute);
            this.Controls.Add(this.btnDecryptLocalFile);
            this.Controls.Add(this.btnEncryptLocalFile);
            this.Controls.Add(this.txtDecryptedLocalFile);
            this.Controls.Add(this.txtEncryptLocalFile);
            this.Controls.Add(this.lblDecryptLocalFile);
            this.Controls.Add(this.lblEncryptLocalFile);
            this.Controls.Add(this.cboOperation);
            this.Controls.Add(this.lblOperation);
            this.Name = "RsaCryptography";
            this.Size = new System.Drawing.Size(362, 165);
            this.Load += new System.EventHandler(this.RsaCryptography_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCryptExecute;
        private System.Windows.Forms.Button btnDecryptLocalFile;
        private System.Windows.Forms.Button btnEncryptLocalFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogMain;
        private System.Windows.Forms.TextBox txtDecryptedLocalFile;
        private System.Windows.Forms.TextBox txtEncryptLocalFile;
        private System.Windows.Forms.Label lblDecryptLocalFile;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.SaveFileDialog saveFileDialogMain;
        private System.Windows.Forms.Label lblEncryptLocalFile;
        private System.Windows.Forms.ComboBox cboOperation;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.Label lblCertificatePath;
        private System.Windows.Forms.TextBox txtCertificatePath;
        private System.Windows.Forms.Label lblCertificatePassword;
        private System.Windows.Forms.TextBox txtCertificatePassword;
        private System.Windows.Forms.Button btnCertificatePath;
    }
}
