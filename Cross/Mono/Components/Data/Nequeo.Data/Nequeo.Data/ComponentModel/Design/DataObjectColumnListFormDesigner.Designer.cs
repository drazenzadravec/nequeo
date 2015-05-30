namespace Nequeo.ComponentModel.Design
{
    partial class DataObjectColumnListFormDesigner
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
            this.dataObjectColumnListControlDesigner1 = new Nequeo.ComponentModel.Design.DataObjectColumnListControlDesigner();
            this.SuspendLayout();
            // 
            // dataObjectColumnListControlDesigner1
            // 
            this.dataObjectColumnListControlDesigner1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataObjectColumnListControlDesigner1.DataObjectColumnCollection = null;
            this.dataObjectColumnListControlDesigner1.Location = new System.Drawing.Point(12, 12);
            this.dataObjectColumnListControlDesigner1.Name = "dataObjectColumnListControlDesigner1";
            this.dataObjectColumnListControlDesigner1.Size = new System.Drawing.Size(665, 468);
            this.dataObjectColumnListControlDesigner1.TabIndex = 0;
            // 
            // DataObjectColumnListFormDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 492);
            this.Controls.Add(this.dataObjectColumnListControlDesigner1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DataObjectColumnListFormDesigner";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Object Column List Designer";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private DataObjectColumnListControlDesigner dataObjectColumnListControlDesigner1;
    }
}