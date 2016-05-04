namespace Nequeo.ComponentModel.Design
{
    partial class CodeGeneration
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
            this.lblCustomTool = new System.Windows.Forms.Label();
            this.txtCustomTool = new System.Windows.Forms.TextBox();
            this.txtXmlConfigurationData = new System.Windows.Forms.TextBox();
            this.txtHelp = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblCustomTool
            // 
            this.lblCustomTool.AutoSize = true;
            this.lblCustomTool.Location = new System.Drawing.Point(12, 8);
            this.lblCustomTool.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCustomTool.Name = "lblCustomTool";
            this.lblCustomTool.Size = new System.Drawing.Size(66, 13);
            this.lblCustomTool.TabIndex = 0;
            this.lblCustomTool.Text = "Custom Tool";
            // 
            // txtCustomTool
            // 
            this.txtCustomTool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomTool.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustomTool.Location = new System.Drawing.Point(84, 5);
            this.txtCustomTool.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtCustomTool.Name = "txtCustomTool";
            this.txtCustomTool.ReadOnly = true;
            this.txtCustomTool.Size = new System.Drawing.Size(606, 20);
            this.txtCustomTool.TabIndex = 1;
            // 
            // txtXmlConfigurationData
            // 
            this.txtXmlConfigurationData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXmlConfigurationData.BackColor = System.Drawing.SystemColors.Window;
            this.txtXmlConfigurationData.Location = new System.Drawing.Point(12, 31);
            this.txtXmlConfigurationData.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtXmlConfigurationData.Multiline = true;
            this.txtXmlConfigurationData.Name = "txtXmlConfigurationData";
            this.txtXmlConfigurationData.ReadOnly = true;
            this.txtXmlConfigurationData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtXmlConfigurationData.Size = new System.Drawing.Size(678, 263);
            this.txtXmlConfigurationData.TabIndex = 2;
            // 
            // txtHelp
            // 
            this.txtHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHelp.BackColor = System.Drawing.SystemColors.Window;
            this.txtHelp.Location = new System.Drawing.Point(12, 301);
            this.txtHelp.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtHelp.Multiline = true;
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.ReadOnly = true;
            this.txtHelp.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHelp.Size = new System.Drawing.Size(678, 116);
            this.txtHelp.TabIndex = 3;
            // 
            // CodeGeneration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 430);
            this.Controls.Add(this.txtHelp);
            this.Controls.Add(this.txtXmlConfigurationData);
            this.Controls.Add(this.txtCustomTool);
            this.Controls.Add(this.lblCustomTool);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "CodeGeneration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Code Generation Configuration";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCustomTool;
        private System.Windows.Forms.TextBox txtCustomTool;
        private System.Windows.Forms.TextBox txtXmlConfigurationData;
        private System.Windows.Forms.TextBox txtHelp;
    }
}