namespace Nequeo.Report.Viewer
{
    partial class ReportViewer
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
            this.reportViewerControl1 = new Nequeo.Report.Viewer.ReportViewerControl();
            this.SuspendLayout();
            // 
            // reportViewerControl1
            // 
            this.reportViewerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reportViewerControl1.BindingSource = null;
            this.reportViewerControl1.Location = new System.Drawing.Point(2, 2);
            this.reportViewerControl1.Name = "reportViewerControl1";
            this.reportViewerControl1.ReportDataSourceName = null;
            this.reportViewerControl1.ReportEmbeddedResource = null;
            this.reportViewerControl1.ReportParameters = null;
            this.reportViewerControl1.ReportPath = null;
            this.reportViewerControl1.Size = new System.Drawing.Size(1066, 769);
            this.reportViewerControl1.TabIndex = 0;
            // 
            // ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 773);
            this.Controls.Add(this.reportViewerControl1);
            this.Name = "ReportViewer";
            this.Text = "ReportViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private ReportViewerControl reportViewerControl1;
    }
}