namespace Nequeo.Forms.UI.Control
{
    partial class Keyboard
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
            this.pictureBoxKeyboard = new System.Windows.Forms.PictureBox();
            this.pictureBoxCapsLockDown = new System.Windows.Forms.PictureBox();
            this.pictureBoxLeftShiftDown = new System.Windows.Forms.PictureBox();
            this.pictureBoxRightShiftDown = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKeyboard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCapsLockDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftShiftDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightShiftDown)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxKeyboard
            // 
            this.pictureBoxKeyboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxKeyboard.Image = global::Nequeo.Forms.Properties.Resources.keyboard_white;
            this.pictureBoxKeyboard.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxKeyboard.Name = "pictureBoxKeyboard";
            this.pictureBoxKeyboard.Size = new System.Drawing.Size(993, 282);
            this.pictureBoxKeyboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxKeyboard.TabIndex = 0;
            this.pictureBoxKeyboard.TabStop = false;
            this.pictureBoxKeyboard.SizeChanged += new System.EventHandler(this.pictureBoxKeyboard_SizeChanged);
            this.pictureBoxKeyboard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxKeyboard_MouseClick);
            // 
            // pictureBoxCapsLockDown
            // 
            this.pictureBoxCapsLockDown.Image = global::Nequeo.Forms.Properties.Resources.caps_down_white;
            this.pictureBoxCapsLockDown.Location = new System.Drawing.Point(3, 113);
            this.pictureBoxCapsLockDown.Name = "pictureBoxCapsLockDown";
            this.pictureBoxCapsLockDown.Size = new System.Drawing.Size(112, 56);
            this.pictureBoxCapsLockDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCapsLockDown.TabIndex = 1;
            this.pictureBoxCapsLockDown.TabStop = false;
            this.pictureBoxCapsLockDown.Visible = false;
            this.pictureBoxCapsLockDown.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxCapsLockState_MouseClick);
            // 
            // pictureBoxLeftShiftDown
            // 
            this.pictureBoxLeftShiftDown.Image = global::Nequeo.Forms.Properties.Resources.shift_down_white;
            this.pictureBoxLeftShiftDown.Location = new System.Drawing.Point(3, 168);
            this.pictureBoxLeftShiftDown.Name = "pictureBoxLeftShiftDown";
            this.pictureBoxLeftShiftDown.Size = new System.Drawing.Size(138, 57);
            this.pictureBoxLeftShiftDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLeftShiftDown.TabIndex = 2;
            this.pictureBoxLeftShiftDown.TabStop = false;
            this.pictureBoxLeftShiftDown.Visible = false;
            this.pictureBoxLeftShiftDown.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxLeftShiftState_MouseClick);
            // 
            // pictureBoxRightShiftDown
            // 
            this.pictureBoxRightShiftDown.Image = global::Nequeo.Forms.Properties.Resources.shift_down_white;
            this.pictureBoxRightShiftDown.Location = new System.Drawing.Point(680, 168);
            this.pictureBoxRightShiftDown.Name = "pictureBoxRightShiftDown";
            this.pictureBoxRightShiftDown.Size = new System.Drawing.Size(136, 57);
            this.pictureBoxRightShiftDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRightShiftDown.TabIndex = 3;
            this.pictureBoxRightShiftDown.TabStop = false;
            this.pictureBoxRightShiftDown.Visible = false;
            this.pictureBoxRightShiftDown.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxRightShiftState_MouseClick);
            // 
            // Keyboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxRightShiftDown);
            this.Controls.Add(this.pictureBoxLeftShiftDown);
            this.Controls.Add(this.pictureBoxCapsLockDown);
            this.Controls.Add(this.pictureBoxKeyboard);
            this.Name = "Keyboard";
            this.Size = new System.Drawing.Size(993, 282);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKeyboard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCapsLockDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftShiftDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightShiftDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxKeyboard;
        private System.Windows.Forms.PictureBox pictureBoxCapsLockDown;
        private System.Windows.Forms.PictureBox pictureBoxLeftShiftDown;
        private System.Windows.Forms.PictureBox pictureBoxRightShiftDown;
    }
}
