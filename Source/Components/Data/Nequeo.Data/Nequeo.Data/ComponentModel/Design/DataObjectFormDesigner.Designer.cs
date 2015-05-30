namespace Nequeo.ComponentModel.Design
{
    partial class DataObjectFormDesigner
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
            this.dataObjectControlDesigner1 = new Nequeo.ComponentModel.Design.DataObjectControlDesigner();
            this.SuspendLayout();
            // 
            // dataObjectControlDesigner1
            // 
            this.dataObjectControlDesigner1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataObjectControlDesigner1.DataObject = null;
            this.dataObjectControlDesigner1.Location = new System.Drawing.Point(12, 12);
            this.dataObjectControlDesigner1.Name = "dataObjectControlDesigner1";
            this.dataObjectControlDesigner1.Size = new System.Drawing.Size(405, 405);
            this.dataObjectControlDesigner1.TabIndex = 0;
            // 
            // DataObjectFormDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 427);
            this.Controls.Add(this.dataObjectControlDesigner1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DataObjectFormDesigner";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Object Designer";
            this.ResumeLayout(false);

        }

        #endregion

        private DataObjectControlDesigner dataObjectControlDesigner1;
    }
}