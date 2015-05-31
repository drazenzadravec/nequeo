namespace Nequeo.Forms.UI.Control
{
    partial class KeyboardForm
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
            this.keyboard = new Nequeo.Forms.UI.Control.Keyboard();
            this.SuspendLayout();
            // 
            // keyboard
            // 
            this.keyboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyboard.KeyboardType = Nequeo.Forms.UI.Control.KeyboardType.Standard;
            this.keyboard.Location = new System.Drawing.Point(0, 0);
            this.keyboard.Name = "keyboard";
            this.keyboard.Size = new System.Drawing.Size(569, 174);
            this.keyboard.TabIndex = 0;
            this.keyboard.UserKeyPressed += new Nequeo.Forms.UI.Control.KeyboardDelegate(this.keyboard_UserKeyPressed);
            // 
            // KeyboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 174);
            this.Controls.Add(this.keyboard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "KeyboardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Keyboard";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private Keyboard keyboard;
    }
}