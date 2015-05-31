namespace Nequeo.Forms.UI.Security
{
    partial class TripleDesCryptography
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
            this.lblOperation = new System.Windows.Forms.Label();
            this.cboOperation = new System.Windows.Forms.ComboBox();
            this.openFileDialogMain = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogMain = new System.Windows.Forms.SaveFileDialog();
            this.lblEncryptLocalFile = new System.Windows.Forms.Label();
            this.txtEncryptLocalFile = new System.Windows.Forms.TextBox();
            this.lblDecryptLocalFile = new System.Windows.Forms.Label();
            this.txtDecryptedLocalFile = new System.Windows.Forms.TextBox();
            this.lblCryptPassword = new System.Windows.Forms.Label();
            this.txtCryptPassword = new System.Windows.Forms.TextBox();
            this.btnEncryptLocalFile = new System.Windows.Forms.Button();
            this.btnDecryptLocalFile = new System.Windows.Forms.Button();
            this.btnCryptExecute = new System.Windows.Forms.Button();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblOperation
            // 
            this.lblOperation.AutoSize = true;
            this.lblOperation.Location = new System.Drawing.Point(3, 3);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.Size = new System.Drawing.Size(53, 13);
            this.lblOperation.TabIndex = 0;
            this.lblOperation.Text = "Operation";
            // 
            // cboOperation
            // 
            this.cboOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOperation.FormattingEnabled = true;
            this.cboOperation.Location = new System.Drawing.Point(92, 0);
            this.cboOperation.Name = "cboOperation";
            this.cboOperation.Size = new System.Drawing.Size(220, 21);
            this.cboOperation.TabIndex = 1;
            this.cboOperation.SelectedIndexChanged += new System.EventHandler(this.cboOperation_SelectedIndexChanged);
            // 
            // openFileDialogMain
            // 
            this.openFileDialogMain.DefaultExt = "*.*";
            this.openFileDialogMain.Filter = "All files (*.*)|*.*";
            this.openFileDialogMain.SupportMultiDottedExtensions = true;
            this.openFileDialogMain.Title = "Open Encrypted/Decrypted File";
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
            this.lblEncryptLocalFile.Location = new System.Drawing.Point(3, 30);
            this.lblEncryptLocalFile.Name = "lblEncryptLocalFile";
            this.lblEncryptLocalFile.Size = new System.Drawing.Size(74, 13);
            this.lblEncryptLocalFile.TabIndex = 2;
            this.lblEncryptLocalFile.Text = "Encrypted File";
            // 
            // txtEncryptLocalFile
            // 
            this.txtEncryptLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEncryptLocalFile.Location = new System.Drawing.Point(92, 27);
            this.txtEncryptLocalFile.Name = "txtEncryptLocalFile";
            this.txtEncryptLocalFile.Size = new System.Drawing.Size(185, 20);
            this.txtEncryptLocalFile.TabIndex = 3;
            // 
            // lblDecryptLocalFile
            // 
            this.lblDecryptLocalFile.AutoSize = true;
            this.lblDecryptLocalFile.Location = new System.Drawing.Point(3, 56);
            this.lblDecryptLocalFile.Name = "lblDecryptLocalFile";
            this.lblDecryptLocalFile.Size = new System.Drawing.Size(75, 13);
            this.lblDecryptLocalFile.TabIndex = 4;
            this.lblDecryptLocalFile.Text = "Decrypted File";
            // 
            // txtDecryptedLocalFile
            // 
            this.txtDecryptedLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDecryptedLocalFile.Location = new System.Drawing.Point(92, 53);
            this.txtDecryptedLocalFile.Name = "txtDecryptedLocalFile";
            this.txtDecryptedLocalFile.Size = new System.Drawing.Size(185, 20);
            this.txtDecryptedLocalFile.TabIndex = 5;
            // 
            // lblCryptPassword
            // 
            this.lblCryptPassword.AutoSize = true;
            this.lblCryptPassword.Location = new System.Drawing.Point(3, 82);
            this.lblCryptPassword.Name = "lblCryptPassword";
            this.lblCryptPassword.Size = new System.Drawing.Size(53, 13);
            this.lblCryptPassword.TabIndex = 6;
            this.lblCryptPassword.Text = "Password";
            // 
            // txtCryptPassword
            // 
            this.txtCryptPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCryptPassword.Location = new System.Drawing.Point(92, 79);
            this.txtCryptPassword.Name = "txtCryptPassword";
            this.txtCryptPassword.PasswordChar = '*';
            this.txtCryptPassword.Size = new System.Drawing.Size(185, 20);
            this.txtCryptPassword.TabIndex = 7;
            // 
            // btnEncryptLocalFile
            // 
            this.btnEncryptLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEncryptLocalFile.Location = new System.Drawing.Point(283, 25);
            this.btnEncryptLocalFile.Name = "btnEncryptLocalFile";
            this.btnEncryptLocalFile.Size = new System.Drawing.Size(29, 23);
            this.btnEncryptLocalFile.TabIndex = 8;
            this.btnEncryptLocalFile.Text = "....";
            this.btnEncryptLocalFile.UseVisualStyleBackColor = true;
            this.btnEncryptLocalFile.Click += new System.EventHandler(this.btnEncryptLocalFile_Click);
            // 
            // btnDecryptLocalFile
            // 
            this.btnDecryptLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDecryptLocalFile.Location = new System.Drawing.Point(283, 51);
            this.btnDecryptLocalFile.Name = "btnDecryptLocalFile";
            this.btnDecryptLocalFile.Size = new System.Drawing.Size(29, 23);
            this.btnDecryptLocalFile.TabIndex = 9;
            this.btnDecryptLocalFile.Text = "....";
            this.btnDecryptLocalFile.UseVisualStyleBackColor = true;
            this.btnDecryptLocalFile.Click += new System.EventHandler(this.btnDecryptLocalFile_Click);
            // 
            // btnCryptExecute
            // 
            this.btnCryptExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCryptExecute.Location = new System.Drawing.Point(237, 105);
            this.btnCryptExecute.Name = "btnCryptExecute";
            this.btnCryptExecute.Size = new System.Drawing.Size(75, 23);
            this.btnCryptExecute.TabIndex = 10;
            this.btnCryptExecute.Text = "Execute";
            this.btnCryptExecute.UseVisualStyleBackColor = true;
            this.btnCryptExecute.Click += new System.EventHandler(this.btnCryptExecute_Click);
            // 
            // TripleDesCryptography
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCryptExecute);
            this.Controls.Add(this.btnDecryptLocalFile);
            this.Controls.Add(this.btnEncryptLocalFile);
            this.Controls.Add(this.txtCryptPassword);
            this.Controls.Add(this.lblCryptPassword);
            this.Controls.Add(this.txtDecryptedLocalFile);
            this.Controls.Add(this.lblDecryptLocalFile);
            this.Controls.Add(this.txtEncryptLocalFile);
            this.Controls.Add(this.lblEncryptLocalFile);
            this.Controls.Add(this.cboOperation);
            this.Controls.Add(this.lblOperation);
            this.Name = "TripleDesCryptography";
            this.Size = new System.Drawing.Size(315, 133);
            this.Load += new System.EventHandler(this.TripleDesCryptography_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.ComboBox cboOperation;
        private System.Windows.Forms.OpenFileDialog openFileDialogMain;
        private System.Windows.Forms.SaveFileDialog saveFileDialogMain;
        private System.Windows.Forms.Label lblEncryptLocalFile;
        private System.Windows.Forms.TextBox txtEncryptLocalFile;
        private System.Windows.Forms.Label lblDecryptLocalFile;
        private System.Windows.Forms.TextBox txtDecryptedLocalFile;
        private System.Windows.Forms.Label lblCryptPassword;
        private System.Windows.Forms.TextBox txtCryptPassword;
        private System.Windows.Forms.Button btnEncryptLocalFile;
        private System.Windows.Forms.Button btnDecryptLocalFile;
        private System.Windows.Forms.Button btnCryptExecute;
        private System.Windows.Forms.ToolTip toolTipMain;
    }
}
