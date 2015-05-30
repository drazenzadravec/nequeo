namespace Nequeo.ComponentModel.Design
{
    partial class ConnectionTypeFormDesigner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionTypeFormDesigner));
            this.connectionTypeControlDesigner1 = new Nequeo.ComponentModel.Design.ConnectionTypeControlDesigner();
            this.SuspendLayout();
            // 
            // connectionTypeControlDesigner1
            // 
            this.connectionTypeControlDesigner1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionTypeControlDesigner1.Location = new System.Drawing.Point(12, 12);
            this.connectionTypeControlDesigner1.Name = "connectionTypeControlDesigner1";
            this.connectionTypeControlDesigner1.Size = new System.Drawing.Size(610, 134);
            this.connectionTypeControlDesigner1.TabIndex = 0;
            // 
            // ConnectionTypeFormDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 155);
            this.Controls.Add(this.connectionTypeControlDesigner1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConnectionTypeFormDesigner";
            this.Text = "Connection Type Designer";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private ConnectionTypeControlDesigner connectionTypeControlDesigner1;
    }
}