namespace Nequeo.Forms.UI.Net
{
    partial class WebPage
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
            this.statusStripWebBrowser = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.wb = new System.Windows.Forms.WebBrowser();
            this.statusStripWebBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripWebBrowser
            // 
            this.statusStripWebBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusText});
            this.statusStripWebBrowser.Location = new System.Drawing.Point(0, 163);
            this.statusStripWebBrowser.Name = "statusStripWebBrowser";
            this.statusStripWebBrowser.Size = new System.Drawing.Size(349, 22);
            this.statusStripWebBrowser.SizingGrip = false;
            this.statusStripWebBrowser.TabIndex = 1;
            // 
            // toolStripStatusText
            // 
            this.toolStripStatusText.Name = "toolStripStatusText";
            this.toolStripStatusText.Size = new System.Drawing.Size(0, 17);
            // 
            // wb
            // 
            this.wb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wb.Location = new System.Drawing.Point(0, 0);
            this.wb.MinimumSize = new System.Drawing.Size(20, 20);
            this.wb.Name = "wb";
            this.wb.Size = new System.Drawing.Size(349, 163);
            this.wb.TabIndex = 0;
            // 
            // WebPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wb);
            this.Controls.Add(this.statusStripWebBrowser);
            this.Name = "WebPage";
            this.Size = new System.Drawing.Size(349, 185);
            this.Load += new System.EventHandler(this.WebPage_Load);
            this.statusStripWebBrowser.ResumeLayout(false);
            this.statusStripWebBrowser.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripWebBrowser;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusText;
        private System.Windows.Forms.WebBrowser wb;


    }
}
