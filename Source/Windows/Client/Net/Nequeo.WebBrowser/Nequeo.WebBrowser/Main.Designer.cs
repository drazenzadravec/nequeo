namespace Nequeo.WebBrowser
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageWB = new System.Windows.Forms.TabPage();
            this.webBrowser0 = new Nequeo.Forms.UI.Net.WebBrowser();
            this.contextMenuTab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuNewTab = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlMain.SuspendLayout();
            this.tabPageWB.SuspendLayout();
            this.contextMenuTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.ContextMenuStrip = this.contextMenuTab;
            this.tabControlMain.Controls.Add(this.tabPageWB);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.ItemSize = new System.Drawing.Size(170, 18);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.ShowToolTips = true;
            this.tabControlMain.Size = new System.Drawing.Size(1262, 794);
            this.tabControlMain.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageWB
            // 
            this.tabPageWB.Controls.Add(this.webBrowser0);
            this.tabPageWB.Location = new System.Drawing.Point(4, 22);
            this.tabPageWB.Name = "tabPageWB";
            this.tabPageWB.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWB.Size = new System.Drawing.Size(1254, 768);
            this.tabPageWB.TabIndex = 0;
            this.tabPageWB.Text = "Nequeo Web Browser";
            this.tabPageWB.UseVisualStyleBackColor = true;
            // 
            // webBrowser0
            // 
            this.webBrowser0.AddressChanged = null;
            this.webBrowser0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser0.DocumentComplete = null;
            this.webBrowser0.HomeUrl = null;
            this.webBrowser0.Location = new System.Drawing.Point(3, 3);
            this.webBrowser0.MenuStripVisible = false;
            this.webBrowser0.Name = "webBrowser0";
            this.webBrowser0.NewWindow = null;
            this.webBrowser0.Size = new System.Drawing.Size(1248, 762);
            this.webBrowser0.TabIndex = 0;
            this.webBrowser0.Url = null;
            // 
            // contextMenuTab
            // 
            this.contextMenuTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuNewTab});
            this.contextMenuTab.Name = "contextMenuTab";
            this.contextMenuTab.Size = new System.Drawing.Size(122, 26);
            // 
            // toolStripMenuNewTab
            // 
            this.toolStripMenuNewTab.Name = "toolStripMenuNewTab";
            this.toolStripMenuNewTab.Size = new System.Drawing.Size(121, 22);
            this.toolStripMenuNewTab.Text = "New Tab";
            this.toolStripMenuNewTab.Click += new System.EventHandler(this.toolStripMenuNewTab_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 794);
            this.Controls.Add(this.tabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Nequeo Web Browser";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageWB.ResumeLayout(false);
            this.contextMenuTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageWB;
        private Forms.UI.Net.WebBrowser webBrowser0;
        private System.Windows.Forms.ContextMenuStrip contextMenuTab;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuNewTab;






    }
}