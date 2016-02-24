namespace Nequeo.VoIP.Sip.UI
{
    partial class TransferList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransferList));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.listViewNumber = new System.Windows.Forms.ListView();
            this.listViewTransfer = new System.Windows.Forms.ListView();
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(249, 218);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(330, 218);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "Tranfer";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // listViewNumber
            // 
            this.listViewNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewNumber.Location = new System.Drawing.Point(206, 12);
            this.listViewNumber.MultiSelect = false;
            this.listViewNumber.Name = "listViewNumber";
            this.listViewNumber.Size = new System.Drawing.Size(199, 200);
            this.listViewNumber.StateImageList = this.imageListSmall;
            this.listViewNumber.TabIndex = 5;
            this.listViewNumber.UseCompatibleStateImageBehavior = false;
            this.listViewNumber.View = System.Windows.Forms.View.SmallIcon;
            this.listViewNumber.SelectedIndexChanged += new System.EventHandler(this.listViewNumber_SelectedIndexChanged);
            // 
            // listViewTransfer
            // 
            this.listViewTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewTransfer.Location = new System.Drawing.Point(12, 12);
            this.listViewTransfer.MultiSelect = false;
            this.listViewTransfer.Name = "listViewTransfer";
            this.listViewTransfer.Size = new System.Drawing.Size(188, 229);
            this.listViewTransfer.StateImageList = this.imageListSmall;
            this.listViewTransfer.TabIndex = 4;
            this.listViewTransfer.UseCompatibleStateImageBehavior = false;
            this.listViewTransfer.View = System.Windows.Forms.View.SmallIcon;
            this.listViewTransfer.SelectedIndexChanged += new System.EventHandler(this.listViewTransfer_SelectedIndexChanged);
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "cellphone.png");
            // 
            // TransferList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 253);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listViewNumber);
            this.Controls.Add(this.listViewTransfer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TransferList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Transfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransferList_FormClosing);
            this.Load += new System.EventHandler(this.TransferList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ListView listViewNumber;
        private System.Windows.Forms.ListView listViewTransfer;
        private System.Windows.Forms.ImageList imageListSmall;
    }
}