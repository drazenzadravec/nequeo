namespace Nequeo.VoIP.Sip.UI
{
    partial class ContactInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactInfo));
            this.groupBoxContact = new System.Windows.Forms.GroupBox();
            this.comboBoxGroup = new System.Windows.Forms.ComboBox();
            this.labelGroup = new System.Windows.Forms.Label();
            this.checkBoxPresenceState = new System.Windows.Forms.CheckBox();
            this.textBoxSipAccount = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelSipAccount = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.listViewNumbers = new System.Windows.Forms.ListView();
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxContact.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxContact
            // 
            this.groupBoxContact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxContact.Controls.Add(this.comboBoxGroup);
            this.groupBoxContact.Controls.Add(this.labelGroup);
            this.groupBoxContact.Controls.Add(this.checkBoxPresenceState);
            this.groupBoxContact.Controls.Add(this.textBoxSipAccount);
            this.groupBoxContact.Controls.Add(this.textBoxName);
            this.groupBoxContact.Controls.Add(this.labelSipAccount);
            this.groupBoxContact.Controls.Add(this.labelName);
            this.groupBoxContact.Controls.Add(this.buttonDelete);
            this.groupBoxContact.Controls.Add(this.buttonAdd);
            this.groupBoxContact.Controls.Add(this.listViewNumbers);
            this.groupBoxContact.Location = new System.Drawing.Point(12, 12);
            this.groupBoxContact.Name = "groupBoxContact";
            this.groupBoxContact.Size = new System.Drawing.Size(386, 265);
            this.groupBoxContact.TabIndex = 0;
            this.groupBoxContact.TabStop = false;
            this.groupBoxContact.Enter += new System.EventHandler(this.groupBoxContact_Enter);
            // 
            // comboBoxGroup
            // 
            this.comboBoxGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGroup.FormattingEnabled = true;
            this.comboBoxGroup.Location = new System.Drawing.Point(83, 86);
            this.comboBoxGroup.Name = "comboBoxGroup";
            this.comboBoxGroup.Size = new System.Drawing.Size(194, 21);
            this.comboBoxGroup.TabIndex = 3;
            this.comboBoxGroup.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // labelGroup
            // 
            this.labelGroup.AutoSize = true;
            this.labelGroup.Location = new System.Drawing.Point(6, 89);
            this.labelGroup.Name = "labelGroup";
            this.labelGroup.Size = new System.Drawing.Size(42, 13);
            this.labelGroup.TabIndex = 8;
            this.labelGroup.Text = "Group :";
            // 
            // checkBoxPresenceState
            // 
            this.checkBoxPresenceState.AutoSize = true;
            this.checkBoxPresenceState.Location = new System.Drawing.Point(9, 65);
            this.checkBoxPresenceState.Name = "checkBoxPresenceState";
            this.checkBoxPresenceState.Size = new System.Drawing.Size(99, 17);
            this.checkBoxPresenceState.TabIndex = 2;
            this.checkBoxPresenceState.Text = "Presence State";
            this.checkBoxPresenceState.UseVisualStyleBackColor = true;
            // 
            // textBoxSipAccount
            // 
            this.textBoxSipAccount.Location = new System.Drawing.Point(83, 39);
            this.textBoxSipAccount.Name = "textBoxSipAccount";
            this.textBoxSipAccount.Size = new System.Drawing.Size(194, 20);
            this.textBoxSipAccount.TabIndex = 1;
            this.textBoxSipAccount.TextChanged += new System.EventHandler(this.textBoxSipAccount_TextChanged);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(83, 13);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(194, 20);
            this.textBoxName.TabIndex = 0;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // labelSipAccount
            // 
            this.labelSipAccount.AutoSize = true;
            this.labelSipAccount.Location = new System.Drawing.Point(6, 42);
            this.labelSipAccount.Name = "labelSipAccount";
            this.labelSipAccount.Size = new System.Drawing.Size(71, 13);
            this.labelSipAccount.TabIndex = 4;
            this.labelSipAccount.Text = "Sip Account :";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(6, 16);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(41, 13);
            this.labelName.TabIndex = 3;
            this.labelName.Text = "Name :";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new System.Drawing.Point(102, 120);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(90, 23);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "Delete Number";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Enabled = false;
            this.buttonAdd.Location = new System.Drawing.Point(6, 120);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(90, 23);
            this.buttonAdd.TabIndex = 4;
            this.buttonAdd.Text = "Add Number";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // listViewNumbers
            // 
            this.listViewNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewNumbers.Enabled = false;
            this.listViewNumbers.Location = new System.Drawing.Point(6, 149);
            this.listViewNumbers.MultiSelect = false;
            this.listViewNumbers.Name = "listViewNumbers";
            this.listViewNumbers.Size = new System.Drawing.Size(374, 110);
            this.listViewNumbers.SmallImageList = this.imageListSmall;
            this.listViewNumbers.TabIndex = 0;
            this.listViewNumbers.UseCompatibleStateImageBehavior = false;
            this.listViewNumbers.View = System.Windows.Forms.View.SmallIcon;
            this.listViewNumbers.SelectedIndexChanged += new System.EventHandler(this.listViewNumbers_SelectedIndexChanged);
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "cellphone.png");
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(323, 283);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(242, 283);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // ContactInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 318);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBoxContact);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContactInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Contact";
            this.Load += new System.EventHandler(this.ContactInfo_Load);
            this.groupBoxContact.ResumeLayout(false);
            this.groupBoxContact.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxContact;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView listViewNumbers;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.CheckBox checkBoxPresenceState;
        private System.Windows.Forms.TextBox textBoxSipAccount;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelSipAccount;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.ComboBox comboBoxGroup;
        private System.Windows.Forms.Label labelGroup;
    }
}