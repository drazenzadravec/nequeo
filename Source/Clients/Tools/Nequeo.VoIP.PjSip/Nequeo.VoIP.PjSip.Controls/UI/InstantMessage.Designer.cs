namespace Nequeo.VoIP.PjSip.UI
{
    partial class InstantMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstantMessage));
            this.listViewMessage = new System.Windows.Forms.ListView();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.richTextBoxMessage = new System.Windows.Forms.RichTextBox();
            this.textBoxSendMesssage = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.labelSendTo = new System.Windows.Forms.Label();
            this.labelSendToValue = new System.Windows.Forms.Label();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewMessage
            // 
            this.listViewMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewMessage.LargeImageList = this.imageListLarge;
            this.listViewMessage.Location = new System.Drawing.Point(12, 12);
            this.listViewMessage.MultiSelect = false;
            this.listViewMessage.Name = "listViewMessage";
            this.listViewMessage.Size = new System.Drawing.Size(188, 375);
            this.listViewMessage.SmallImageList = this.imageListSmall;
            this.listViewMessage.TabIndex = 0;
            this.listViewMessage.UseCompatibleStateImageBehavior = false;
            this.listViewMessage.View = System.Windows.Forms.View.SmallIcon;
            this.listViewMessage.SelectedIndexChanged += new System.EventHandler(this.listViewMessage_SelectedIndexChanged);
            // 
            // imageListLarge
            // 
            this.imageListLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLarge.ImageStream")));
            this.imageListLarge.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListLarge.Images.SetKeyName(0, "Folder_256x256.png");
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "cellphone.png");
            // 
            // richTextBoxMessage
            // 
            this.richTextBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxMessage.Location = new System.Drawing.Point(206, 12);
            this.richTextBoxMessage.Name = "richTextBoxMessage";
            this.richTextBoxMessage.ReadOnly = true;
            this.richTextBoxMessage.Size = new System.Drawing.Size(430, 290);
            this.richTextBoxMessage.TabIndex = 3;
            this.richTextBoxMessage.Text = "";
            this.richTextBoxMessage.TextChanged += new System.EventHandler(this.richTextBoxMessage_TextChanged);
            // 
            // textBoxSendMesssage
            // 
            this.textBoxSendMesssage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSendMesssage.Location = new System.Drawing.Point(206, 331);
            this.textBoxSendMesssage.Multiline = true;
            this.textBoxSendMesssage.Name = "textBoxSendMesssage";
            this.textBoxSendMesssage.Size = new System.Drawing.Size(368, 56);
            this.textBoxSendMesssage.TabIndex = 1;
            this.textBoxSendMesssage.TextChanged += new System.EventHandler(this.textBoxSendMesssage_TextChanged);
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(581, 364);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(55, 23);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // labelSendTo
            // 
            this.labelSendTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSendTo.AutoSize = true;
            this.labelSendTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSendTo.Location = new System.Drawing.Point(206, 312);
            this.labelSendTo.Name = "labelSendTo";
            this.labelSendTo.Size = new System.Drawing.Size(54, 13);
            this.labelSendTo.TabIndex = 4;
            this.labelSendTo.Text = "Send To :";
            // 
            // labelSendToValue
            // 
            this.labelSendToValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSendToValue.AutoSize = true;
            this.labelSendToValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSendToValue.Location = new System.Drawing.Point(266, 312);
            this.labelSendToValue.Name = "labelSendToValue";
            this.labelSendToValue.Size = new System.Drawing.Size(0, 13);
            this.labelSendToValue.TabIndex = 5;
            // 
            // buttonPrint
            // 
            this.buttonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrint.Enabled = false;
            this.buttonPrint.Location = new System.Drawing.Point(581, 335);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(55, 23);
            this.buttonPrint.TabIndex = 6;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // InstantMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 399);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.labelSendToValue);
            this.Controls.Add(this.labelSendTo);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxSendMesssage);
            this.Controls.Add(this.richTextBoxMessage);
            this.Controls.Add(this.listViewMessage);
            this.MaximizeBox = false;
            this.Name = "InstantMessage";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Instant Message";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstantMessage_FormClosing);
            this.Load += new System.EventHandler(this.InstantMessage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewMessage;
        private System.Windows.Forms.RichTextBox richTextBoxMessage;
        private System.Windows.Forms.TextBox textBoxSendMesssage;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label labelSendTo;
        private System.Windows.Forms.Label labelSendToValue;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.Button buttonPrint;
    }
}