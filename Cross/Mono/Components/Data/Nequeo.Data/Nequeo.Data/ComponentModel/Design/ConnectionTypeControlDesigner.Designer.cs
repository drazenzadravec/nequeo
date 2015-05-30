namespace Nequeo.ComponentModel.Design
{
    partial class ConnectionTypeControlDesigner
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
            this.txtDataObjectTypeName = new System.Windows.Forms.TextBox();
            this.txtDatabaseConnection = new System.Windows.Forms.TextBox();
            this.cboConnectionType = new System.Windows.Forms.ComboBox();
            this.cboConnectionDataType = new System.Windows.Forms.ComboBox();
            this.lblDataObjectTypeName = new System.Windows.Forms.Label();
            this.lblDatabaseConnection = new System.Windows.Forms.Label();
            this.lblConnectionType = new System.Windows.Forms.Label();
            this.lblConnectionDataType = new System.Windows.Forms.Label();
            this.lblDataAccessProvider = new System.Windows.Forms.Label();
            this.txtDataAccessProvider = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtDataObjectTypeName
            // 
            this.txtDataObjectTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataObjectTypeName.Location = new System.Drawing.Point(131, 7);
            this.txtDataObjectTypeName.Name = "txtDataObjectTypeName";
            this.txtDataObjectTypeName.Size = new System.Drawing.Size(317, 20);
            this.txtDataObjectTypeName.TabIndex = 0;
            this.txtDataObjectTypeName.TextChanged += new System.EventHandler(this.txtDataObjectTypeName_TextChanged);
            // 
            // txtDatabaseConnection
            // 
            this.txtDatabaseConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabaseConnection.Location = new System.Drawing.Point(131, 33);
            this.txtDatabaseConnection.Name = "txtDatabaseConnection";
            this.txtDatabaseConnection.Size = new System.Drawing.Size(461, 20);
            this.txtDatabaseConnection.TabIndex = 1;
            this.txtDatabaseConnection.TextChanged += new System.EventHandler(this.txtDatabaseConnection_TextChanged);
            // 
            // cboConnectionType
            // 
            this.cboConnectionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConnectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConnectionType.FormattingEnabled = true;
            this.cboConnectionType.Location = new System.Drawing.Point(131, 59);
            this.cboConnectionType.Name = "cboConnectionType";
            this.cboConnectionType.Size = new System.Drawing.Size(225, 21);
            this.cboConnectionType.TabIndex = 2;
            this.cboConnectionType.SelectedIndexChanged += new System.EventHandler(this.cboConnectionType_SelectedIndexChanged);
            // 
            // cboConnectionDataType
            // 
            this.cboConnectionDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConnectionDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConnectionDataType.FormattingEnabled = true;
            this.cboConnectionDataType.Location = new System.Drawing.Point(131, 86);
            this.cboConnectionDataType.Name = "cboConnectionDataType";
            this.cboConnectionDataType.Size = new System.Drawing.Size(225, 21);
            this.cboConnectionDataType.TabIndex = 3;
            this.cboConnectionDataType.SelectedIndexChanged += new System.EventHandler(this.cboConnectionDataType_SelectedIndexChanged);
            // 
            // lblDataObjectTypeName
            // 
            this.lblDataObjectTypeName.AutoSize = true;
            this.lblDataObjectTypeName.Location = new System.Drawing.Point(3, 10);
            this.lblDataObjectTypeName.Name = "lblDataObjectTypeName";
            this.lblDataObjectTypeName.Size = new System.Drawing.Size(122, 13);
            this.lblDataObjectTypeName.TabIndex = 4;
            this.lblDataObjectTypeName.Text = "Data Object Type Name";
            // 
            // lblDatabaseConnection
            // 
            this.lblDatabaseConnection.AutoSize = true;
            this.lblDatabaseConnection.Location = new System.Drawing.Point(3, 36);
            this.lblDatabaseConnection.Name = "lblDatabaseConnection";
            this.lblDatabaseConnection.Size = new System.Drawing.Size(110, 13);
            this.lblDatabaseConnection.TabIndex = 5;
            this.lblDatabaseConnection.Text = "Database Connection";
            // 
            // lblConnectionType
            // 
            this.lblConnectionType.AutoSize = true;
            this.lblConnectionType.Location = new System.Drawing.Point(3, 62);
            this.lblConnectionType.Name = "lblConnectionType";
            this.lblConnectionType.Size = new System.Drawing.Size(88, 13);
            this.lblConnectionType.TabIndex = 6;
            this.lblConnectionType.Text = "Connection Type";
            // 
            // lblConnectionDataType
            // 
            this.lblConnectionDataType.AutoSize = true;
            this.lblConnectionDataType.Location = new System.Drawing.Point(3, 89);
            this.lblConnectionDataType.Name = "lblConnectionDataType";
            this.lblConnectionDataType.Size = new System.Drawing.Size(114, 13);
            this.lblConnectionDataType.TabIndex = 7;
            this.lblConnectionDataType.Text = "Connection Data Type";
            // 
            // lblDataAccessProvider
            // 
            this.lblDataAccessProvider.AutoSize = true;
            this.lblDataAccessProvider.Location = new System.Drawing.Point(3, 117);
            this.lblDataAccessProvider.Name = "lblDataAccessProvider";
            this.lblDataAccessProvider.Size = new System.Drawing.Size(110, 13);
            this.lblDataAccessProvider.TabIndex = 8;
            this.lblDataAccessProvider.Text = "Data Access Provider";
            // 
            // txtDataAccessProvider
            // 
            this.txtDataAccessProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataAccessProvider.Location = new System.Drawing.Point(131, 114);
            this.txtDataAccessProvider.Name = "txtDataAccessProvider";
            this.txtDataAccessProvider.Size = new System.Drawing.Size(461, 20);
            this.txtDataAccessProvider.TabIndex = 9;
            this.txtDataAccessProvider.TextChanged += new System.EventHandler(this.txtDataAccessProvider_TextChanged);
            // 
            // ConnectionTypeControlDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtDataAccessProvider);
            this.Controls.Add(this.lblDataAccessProvider);
            this.Controls.Add(this.lblConnectionDataType);
            this.Controls.Add(this.lblConnectionType);
            this.Controls.Add(this.lblDatabaseConnection);
            this.Controls.Add(this.lblDataObjectTypeName);
            this.Controls.Add(this.cboConnectionDataType);
            this.Controls.Add(this.cboConnectionType);
            this.Controls.Add(this.txtDatabaseConnection);
            this.Controls.Add(this.txtDataObjectTypeName);
            this.Name = "ConnectionTypeControlDesigner";
            this.Size = new System.Drawing.Size(595, 142);
            this.Load += new System.EventHandler(this.ConnectionTypeControlDesigner_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDataObjectTypeName;
        private System.Windows.Forms.TextBox txtDatabaseConnection;
        private System.Windows.Forms.ComboBox cboConnectionType;
        private System.Windows.Forms.ComboBox cboConnectionDataType;
        private System.Windows.Forms.Label lblDataObjectTypeName;
        private System.Windows.Forms.Label lblDatabaseConnection;
        private System.Windows.Forms.Label lblConnectionType;
        private System.Windows.Forms.Label lblConnectionDataType;
        private System.Windows.Forms.Label lblDataAccessProvider;
        private System.Windows.Forms.TextBox txtDataAccessProvider;
    }
}
