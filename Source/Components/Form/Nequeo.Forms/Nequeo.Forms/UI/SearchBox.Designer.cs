namespace Nequeo.Forms.UI
{
    partial class SearchBox
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
            this.labelSearchText = new System.Windows.Forms.Label();
            this.textBoxSearchText = new System.Windows.Forms.TextBox();
            this.buttonSearchText = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelSearchText
            // 
            this.labelSearchText.AutoSize = true;
            this.labelSearchText.Location = new System.Drawing.Point(12, 15);
            this.labelSearchText.Name = "labelSearchText";
            this.labelSearchText.Size = new System.Drawing.Size(65, 13);
            this.labelSearchText.TabIndex = 0;
            this.labelSearchText.Text = "Search Text";
            // 
            // textBoxSearchText
            // 
            this.textBoxSearchText.Location = new System.Drawing.Point(83, 12);
            this.textBoxSearchText.Name = "textBoxSearchText";
            this.textBoxSearchText.Size = new System.Drawing.Size(228, 20);
            this.textBoxSearchText.TabIndex = 1;
            this.textBoxSearchText.TextChanged += new System.EventHandler(this.textBoxSearchText_TextChanged);
            // 
            // buttonSearchText
            // 
            this.buttonSearchText.Location = new System.Drawing.Point(317, 10);
            this.buttonSearchText.Name = "buttonSearchText";
            this.buttonSearchText.Size = new System.Drawing.Size(75, 23);
            this.buttonSearchText.TabIndex = 2;
            this.buttonSearchText.Text = "Find Next";
            this.buttonSearchText.UseVisualStyleBackColor = true;
            this.buttonSearchText.Click += new System.EventHandler(this.buttonSearchText_Click);
            // 
            // SearchBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 45);
            this.Controls.Add(this.buttonSearchText);
            this.Controls.Add(this.textBoxSearchText);
            this.Controls.Add(this.labelSearchText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search Box";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSearchText;
        private System.Windows.Forms.TextBox textBoxSearchText;
        private System.Windows.Forms.Button buttonSearchText;
    }
}