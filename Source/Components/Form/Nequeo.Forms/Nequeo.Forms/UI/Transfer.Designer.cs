namespace Nequeo.Forms.UI
{
    partial class Transfer
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
            this.richTextBoxTransfer = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBoxTransfer
            // 
            this.richTextBoxTransfer.AutoWordSelection = true;
            this.richTextBoxTransfer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxTransfer.EnableAutoDragDrop = true;
            this.richTextBoxTransfer.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxTransfer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.richTextBoxTransfer.Name = "richTextBoxTransfer";
            this.richTextBoxTransfer.Size = new System.Drawing.Size(564, 98);
            this.richTextBoxTransfer.TabIndex = 0;
            this.richTextBoxTransfer.Text = "";
            // 
            // Transfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 98);
            this.Controls.Add(this.richTextBoxTransfer);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Transfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Transfer";
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.RichTextBox richTextBoxTransfer;

    }
}