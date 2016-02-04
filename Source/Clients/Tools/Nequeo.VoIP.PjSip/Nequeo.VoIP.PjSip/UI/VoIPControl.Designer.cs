namespace Nequeo.VoIP.PjSip.UI
{
    partial class VoIPControl
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
            this.groupBoxCall = new System.Windows.Forms.GroupBox();
            this.groupBoxAccount = new System.Windows.Forms.GroupBox();
            this.groupBoxContact = new System.Windows.Forms.GroupBox();
            this.listViewContact = new System.Windows.Forms.ListView();
            this.groupBoxContact.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCall
            // 
            this.groupBoxCall.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCall.Name = "groupBoxCall";
            this.groupBoxCall.Size = new System.Drawing.Size(248, 221);
            this.groupBoxCall.TabIndex = 0;
            this.groupBoxCall.TabStop = false;
            this.groupBoxCall.Text = "Call";
            // 
            // groupBoxAccount
            // 
            this.groupBoxAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxAccount.Location = new System.Drawing.Point(3, 230);
            this.groupBoxAccount.Name = "groupBoxAccount";
            this.groupBoxAccount.Size = new System.Drawing.Size(248, 280);
            this.groupBoxAccount.TabIndex = 1;
            this.groupBoxAccount.TabStop = false;
            this.groupBoxAccount.Text = "Account";
            // 
            // groupBoxContact
            // 
            this.groupBoxContact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxContact.Controls.Add(this.listViewContact);
            this.groupBoxContact.Location = new System.Drawing.Point(257, 3);
            this.groupBoxContact.Name = "groupBoxContact";
            this.groupBoxContact.Size = new System.Drawing.Size(218, 507);
            this.groupBoxContact.TabIndex = 2;
            this.groupBoxContact.TabStop = false;
            this.groupBoxContact.Text = "Contact";
            // 
            // listViewContact
            // 
            this.listViewContact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewContact.Location = new System.Drawing.Point(6, 19);
            this.listViewContact.Name = "listViewContact";
            this.listViewContact.Size = new System.Drawing.Size(206, 482);
            this.listViewContact.TabIndex = 0;
            this.listViewContact.UseCompatibleStateImageBehavior = false;
            // 
            // VoIPControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxContact);
            this.Controls.Add(this.groupBoxAccount);
            this.Controls.Add(this.groupBoxCall);
            this.Name = "VoIPControl";
            this.Size = new System.Drawing.Size(478, 513);
            this.groupBoxContact.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCall;
        private System.Windows.Forms.GroupBox groupBoxAccount;
        private System.Windows.Forms.GroupBox groupBoxContact;
        private System.Windows.Forms.ListView listViewContact;
    }
}
