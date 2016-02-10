namespace Nequeo.VoIP.Sip.UI
{
    partial class VoIPControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoIPControl));
            this.groupBoxCall = new System.Windows.Forms.GroupBox();
            this.groupBoxDigits = new System.Windows.Forms.GroupBox();
            this.textBoxDigits = new System.Windows.Forms.TextBox();
            this.buttonHash = new System.Windows.Forms.Button();
            this.buttonStar = new System.Windows.Forms.Button();
            this.buttonZero = new System.Windows.Forms.Button();
            this.buttonEight = new System.Windows.Forms.Button();
            this.buttonSeven = new System.Windows.Forms.Button();
            this.buttonSix = new System.Windows.Forms.Button();
            this.buttonFive = new System.Windows.Forms.Button();
            this.buttonFour = new System.Windows.Forms.Button();
            this.buttonNine = new System.Windows.Forms.Button();
            this.buttonThree = new System.Windows.Forms.Button();
            this.buttonTwo = new System.Windows.Forms.Button();
            this.buttonOne = new System.Windows.Forms.Button();
            this.comboBoxCallNumber = new System.Windows.Forms.ComboBox();
            this.buttonHangup = new System.Windows.Forms.Button();
            this.buttonCall = new System.Windows.Forms.Button();
            this.groupBoxAccount = new System.Windows.Forms.GroupBox();
            this.buttonLoadContacts = new System.Windows.Forms.Button();
            this.buttonInstantMessage = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.labelRegistationStatusState = new System.Windows.Forms.Label();
            this.labelRegistationStatus = new System.Windows.Forms.Label();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.groupBoxContact = new System.Windows.Forms.GroupBox();
            this.buttonContactUpdate = new System.Windows.Forms.Button();
            this.buttonContactDelete = new System.Windows.Forms.Button();
            this.buttonContactAdd = new System.Windows.Forms.Button();
            this.listViewContact = new System.Windows.Forms.ListView();
            this.contextMenuStripContacts = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.groupBoxCall.SuspendLayout();
            this.groupBoxDigits.SuspendLayout();
            this.groupBoxAccount.SuspendLayout();
            this.groupBoxContact.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCall
            // 
            this.groupBoxCall.Controls.Add(this.groupBoxDigits);
            this.groupBoxCall.Controls.Add(this.comboBoxCallNumber);
            this.groupBoxCall.Controls.Add(this.buttonHangup);
            this.groupBoxCall.Controls.Add(this.buttonCall);
            this.groupBoxCall.Enabled = false;
            this.groupBoxCall.Location = new System.Drawing.Point(3, 3);
            this.groupBoxCall.Name = "groupBoxCall";
            this.groupBoxCall.Size = new System.Drawing.Size(248, 257);
            this.groupBoxCall.TabIndex = 0;
            this.groupBoxCall.TabStop = false;
            this.groupBoxCall.Text = "Call";
            // 
            // groupBoxDigits
            // 
            this.groupBoxDigits.Controls.Add(this.textBoxDigits);
            this.groupBoxDigits.Controls.Add(this.buttonHash);
            this.groupBoxDigits.Controls.Add(this.buttonStar);
            this.groupBoxDigits.Controls.Add(this.buttonZero);
            this.groupBoxDigits.Controls.Add(this.buttonEight);
            this.groupBoxDigits.Controls.Add(this.buttonSeven);
            this.groupBoxDigits.Controls.Add(this.buttonSix);
            this.groupBoxDigits.Controls.Add(this.buttonFive);
            this.groupBoxDigits.Controls.Add(this.buttonFour);
            this.groupBoxDigits.Controls.Add(this.buttonNine);
            this.groupBoxDigits.Controls.Add(this.buttonThree);
            this.groupBoxDigits.Controls.Add(this.buttonTwo);
            this.groupBoxDigits.Controls.Add(this.buttonOne);
            this.groupBoxDigits.Enabled = false;
            this.groupBoxDigits.Location = new System.Drawing.Point(9, 46);
            this.groupBoxDigits.Name = "groupBoxDigits";
            this.groupBoxDigits.Size = new System.Drawing.Size(233, 168);
            this.groupBoxDigits.TabIndex = 16;
            this.groupBoxDigits.TabStop = false;
            // 
            // textBoxDigits
            // 
            this.textBoxDigits.Location = new System.Drawing.Point(19, 19);
            this.textBoxDigits.Name = "textBoxDigits";
            this.textBoxDigits.ReadOnly = true;
            this.textBoxDigits.Size = new System.Drawing.Size(195, 20);
            this.textBoxDigits.TabIndex = 26;
            // 
            // buttonHash
            // 
            this.buttonHash.Location = new System.Drawing.Point(153, 136);
            this.buttonHash.Name = "buttonHash";
            this.buttonHash.Size = new System.Drawing.Size(61, 23);
            this.buttonHash.TabIndex = 25;
            this.buttonHash.Text = "#";
            this.buttonHash.UseVisualStyleBackColor = true;
            this.buttonHash.Click += new System.EventHandler(this.buttonHash_Click);
            // 
            // buttonStar
            // 
            this.buttonStar.Location = new System.Drawing.Point(19, 136);
            this.buttonStar.Name = "buttonStar";
            this.buttonStar.Size = new System.Drawing.Size(61, 23);
            this.buttonStar.TabIndex = 24;
            this.buttonStar.Text = "*";
            this.buttonStar.UseVisualStyleBackColor = true;
            this.buttonStar.Click += new System.EventHandler(this.buttonStar_Click);
            // 
            // buttonZero
            // 
            this.buttonZero.Location = new System.Drawing.Point(86, 136);
            this.buttonZero.Name = "buttonZero";
            this.buttonZero.Size = new System.Drawing.Size(61, 23);
            this.buttonZero.TabIndex = 23;
            this.buttonZero.Text = "0";
            this.buttonZero.UseVisualStyleBackColor = true;
            this.buttonZero.Click += new System.EventHandler(this.buttonZero_Click);
            // 
            // buttonEight
            // 
            this.buttonEight.Location = new System.Drawing.Point(86, 107);
            this.buttonEight.Name = "buttonEight";
            this.buttonEight.Size = new System.Drawing.Size(61, 23);
            this.buttonEight.TabIndex = 21;
            this.buttonEight.Text = "8 tuv";
            this.buttonEight.UseVisualStyleBackColor = true;
            this.buttonEight.Click += new System.EventHandler(this.buttonEight_Click);
            // 
            // buttonSeven
            // 
            this.buttonSeven.Location = new System.Drawing.Point(19, 107);
            this.buttonSeven.Name = "buttonSeven";
            this.buttonSeven.Size = new System.Drawing.Size(61, 23);
            this.buttonSeven.TabIndex = 20;
            this.buttonSeven.Text = "7 pqrs";
            this.buttonSeven.UseVisualStyleBackColor = true;
            this.buttonSeven.Click += new System.EventHandler(this.buttonSeven_Click);
            // 
            // buttonSix
            // 
            this.buttonSix.Location = new System.Drawing.Point(153, 78);
            this.buttonSix.Name = "buttonSix";
            this.buttonSix.Size = new System.Drawing.Size(61, 23);
            this.buttonSix.TabIndex = 19;
            this.buttonSix.Text = "6 mno";
            this.buttonSix.UseVisualStyleBackColor = true;
            this.buttonSix.Click += new System.EventHandler(this.buttonSix_Click);
            // 
            // buttonFive
            // 
            this.buttonFive.Location = new System.Drawing.Point(86, 78);
            this.buttonFive.Name = "buttonFive";
            this.buttonFive.Size = new System.Drawing.Size(61, 23);
            this.buttonFive.TabIndex = 18;
            this.buttonFive.Text = "5 jkl";
            this.buttonFive.UseVisualStyleBackColor = true;
            this.buttonFive.Click += new System.EventHandler(this.buttonFive_Click);
            // 
            // buttonFour
            // 
            this.buttonFour.Location = new System.Drawing.Point(19, 78);
            this.buttonFour.Name = "buttonFour";
            this.buttonFour.Size = new System.Drawing.Size(61, 23);
            this.buttonFour.TabIndex = 17;
            this.buttonFour.Text = "4 ghi";
            this.buttonFour.UseVisualStyleBackColor = true;
            this.buttonFour.Click += new System.EventHandler(this.buttonFour_Click);
            // 
            // buttonNine
            // 
            this.buttonNine.Location = new System.Drawing.Point(153, 107);
            this.buttonNine.Name = "buttonNine";
            this.buttonNine.Size = new System.Drawing.Size(61, 23);
            this.buttonNine.TabIndex = 22;
            this.buttonNine.Text = "9 wxyz";
            this.buttonNine.UseVisualStyleBackColor = true;
            this.buttonNine.Click += new System.EventHandler(this.buttonNine_Click);
            // 
            // buttonThree
            // 
            this.buttonThree.Location = new System.Drawing.Point(153, 49);
            this.buttonThree.Name = "buttonThree";
            this.buttonThree.Size = new System.Drawing.Size(61, 23);
            this.buttonThree.TabIndex = 16;
            this.buttonThree.Text = "3 def";
            this.buttonThree.UseVisualStyleBackColor = true;
            this.buttonThree.Click += new System.EventHandler(this.buttonThree_Click);
            // 
            // buttonTwo
            // 
            this.buttonTwo.Location = new System.Drawing.Point(86, 49);
            this.buttonTwo.Name = "buttonTwo";
            this.buttonTwo.Size = new System.Drawing.Size(61, 23);
            this.buttonTwo.TabIndex = 15;
            this.buttonTwo.Text = "2 abc";
            this.buttonTwo.UseVisualStyleBackColor = true;
            this.buttonTwo.Click += new System.EventHandler(this.buttonTwo_Click);
            // 
            // buttonOne
            // 
            this.buttonOne.Location = new System.Drawing.Point(19, 49);
            this.buttonOne.Name = "buttonOne";
            this.buttonOne.Size = new System.Drawing.Size(61, 23);
            this.buttonOne.TabIndex = 14;
            this.buttonOne.Text = "1";
            this.buttonOne.UseVisualStyleBackColor = true;
            this.buttonOne.Click += new System.EventHandler(this.buttonOne_Click);
            // 
            // comboBoxCallNumber
            // 
            this.comboBoxCallNumber.FormattingEnabled = true;
            this.comboBoxCallNumber.Location = new System.Drawing.Point(9, 19);
            this.comboBoxCallNumber.Name = "comboBoxCallNumber";
            this.comboBoxCallNumber.Size = new System.Drawing.Size(233, 21);
            this.comboBoxCallNumber.TabIndex = 0;
            this.comboBoxCallNumber.SelectedIndexChanged += new System.EventHandler(this.comboBoxCallNumber_SelectedIndexChanged);
            this.comboBoxCallNumber.TextChanged += new System.EventHandler(this.comboBoxCallNumber_TextChanged);
            this.comboBoxCallNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxCallNumber_KeyPress);
            // 
            // buttonHangup
            // 
            this.buttonHangup.Enabled = false;
            this.buttonHangup.Location = new System.Drawing.Point(45, 221);
            this.buttonHangup.Name = "buttonHangup";
            this.buttonHangup.Size = new System.Drawing.Size(75, 23);
            this.buttonHangup.TabIndex = 15;
            this.buttonHangup.Text = "Hangup";
            this.buttonHangup.UseVisualStyleBackColor = true;
            this.buttonHangup.Click += new System.EventHandler(this.buttonHangup_Click);
            // 
            // buttonCall
            // 
            this.buttonCall.Enabled = false;
            this.buttonCall.Location = new System.Drawing.Point(126, 221);
            this.buttonCall.Name = "buttonCall";
            this.buttonCall.Size = new System.Drawing.Size(75, 23);
            this.buttonCall.TabIndex = 14;
            this.buttonCall.Text = "Call";
            this.buttonCall.UseVisualStyleBackColor = true;
            this.buttonCall.Click += new System.EventHandler(this.buttonCall_Click);
            // 
            // groupBoxAccount
            // 
            this.groupBoxAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxAccount.Controls.Add(this.buttonLoadContacts);
            this.groupBoxAccount.Controls.Add(this.buttonInstantMessage);
            this.groupBoxAccount.Controls.Add(this.buttonCreate);
            this.groupBoxAccount.Controls.Add(this.buttonSettings);
            this.groupBoxAccount.Controls.Add(this.labelRegistationStatusState);
            this.groupBoxAccount.Controls.Add(this.labelRegistationStatus);
            this.groupBoxAccount.Controls.Add(this.buttonRegister);
            this.groupBoxAccount.Location = new System.Drawing.Point(3, 266);
            this.groupBoxAccount.Name = "groupBoxAccount";
            this.groupBoxAccount.Size = new System.Drawing.Size(248, 121);
            this.groupBoxAccount.TabIndex = 1;
            this.groupBoxAccount.TabStop = false;
            this.groupBoxAccount.Text = "Account";
            // 
            // buttonLoadContacts
            // 
            this.buttonLoadContacts.Enabled = false;
            this.buttonLoadContacts.Location = new System.Drawing.Point(38, 48);
            this.buttonLoadContacts.Name = "buttonLoadContacts";
            this.buttonLoadContacts.Size = new System.Drawing.Size(99, 23);
            this.buttonLoadContacts.TabIndex = 12;
            this.buttonLoadContacts.Text = "Load Contacts";
            this.buttonLoadContacts.UseVisualStyleBackColor = true;
            this.buttonLoadContacts.Click += new System.EventHandler(this.buttonLoadContacts_Click);
            // 
            // buttonInstantMessage
            // 
            this.buttonInstantMessage.Enabled = false;
            this.buttonInstantMessage.Location = new System.Drawing.Point(143, 48);
            this.buttonInstantMessage.Name = "buttonInstantMessage";
            this.buttonInstantMessage.Size = new System.Drawing.Size(99, 23);
            this.buttonInstantMessage.TabIndex = 11;
            this.buttonInstantMessage.Text = "Instant Message";
            this.buttonInstantMessage.UseVisualStyleBackColor = true;
            this.buttonInstantMessage.Click += new System.EventHandler(this.buttonInstantMessage_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Enabled = false;
            this.buttonCreate.Location = new System.Drawing.Point(87, 19);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 5;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(168, 19);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 23);
            this.buttonSettings.TabIndex = 4;
            this.buttonSettings.Text = "Settings";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // labelRegistationStatusState
            // 
            this.labelRegistationStatusState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRegistationStatusState.AutoSize = true;
            this.labelRegistationStatusState.Location = new System.Drawing.Point(6, 102);
            this.labelRegistationStatusState.Name = "labelRegistationStatusState";
            this.labelRegistationStatusState.Size = new System.Drawing.Size(78, 13);
            this.labelRegistationStatusState.TabIndex = 10;
            this.labelRegistationStatusState.Text = "Not Registered";
            // 
            // labelRegistationStatus
            // 
            this.labelRegistationStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRegistationStatus.AutoSize = true;
            this.labelRegistationStatus.Location = new System.Drawing.Point(6, 80);
            this.labelRegistationStatus.Name = "labelRegistationStatus";
            this.labelRegistationStatus.Size = new System.Drawing.Size(98, 13);
            this.labelRegistationStatus.TabIndex = 9;
            this.labelRegistationStatus.Text = "Registation  None :";
            // 
            // buttonRegister
            // 
            this.buttonRegister.Enabled = false;
            this.buttonRegister.Location = new System.Drawing.Point(6, 19);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonRegister.TabIndex = 6;
            this.buttonRegister.Text = "Register";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // groupBoxContact
            // 
            this.groupBoxContact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxContact.Controls.Add(this.buttonContactUpdate);
            this.groupBoxContact.Controls.Add(this.buttonContactDelete);
            this.groupBoxContact.Controls.Add(this.buttonContactAdd);
            this.groupBoxContact.Controls.Add(this.listViewContact);
            this.groupBoxContact.Enabled = false;
            this.groupBoxContact.Location = new System.Drawing.Point(257, 3);
            this.groupBoxContact.Name = "groupBoxContact";
            this.groupBoxContact.Size = new System.Drawing.Size(249, 384);
            this.groupBoxContact.TabIndex = 2;
            this.groupBoxContact.TabStop = false;
            this.groupBoxContact.Text = "Contact";
            // 
            // buttonContactUpdate
            // 
            this.buttonContactUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonContactUpdate.Enabled = false;
            this.buttonContactUpdate.Location = new System.Drawing.Point(87, 355);
            this.buttonContactUpdate.Name = "buttonContactUpdate";
            this.buttonContactUpdate.Size = new System.Drawing.Size(75, 23);
            this.buttonContactUpdate.TabIndex = 5;
            this.buttonContactUpdate.Text = "Update";
            this.buttonContactUpdate.UseVisualStyleBackColor = true;
            this.buttonContactUpdate.Click += new System.EventHandler(this.buttonContactUpdate_Click);
            // 
            // buttonContactDelete
            // 
            this.buttonContactDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonContactDelete.Enabled = false;
            this.buttonContactDelete.Location = new System.Drawing.Point(168, 355);
            this.buttonContactDelete.Name = "buttonContactDelete";
            this.buttonContactDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonContactDelete.TabIndex = 4;
            this.buttonContactDelete.Text = "Delete";
            this.buttonContactDelete.UseVisualStyleBackColor = true;
            this.buttonContactDelete.Click += new System.EventHandler(this.buttonContactDelete_Click);
            // 
            // buttonContactAdd
            // 
            this.buttonContactAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonContactAdd.Location = new System.Drawing.Point(6, 355);
            this.buttonContactAdd.Name = "buttonContactAdd";
            this.buttonContactAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonContactAdd.TabIndex = 3;
            this.buttonContactAdd.Text = "Add";
            this.buttonContactAdd.UseVisualStyleBackColor = true;
            this.buttonContactAdd.Click += new System.EventHandler(this.buttonContactAdd_Click);
            // 
            // listViewContact
            // 
            this.listViewContact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewContact.ContextMenuStrip = this.contextMenuStripContacts;
            this.listViewContact.LargeImageList = this.imageListLarge;
            this.listViewContact.Location = new System.Drawing.Point(6, 19);
            this.listViewContact.MultiSelect = false;
            this.listViewContact.Name = "listViewContact";
            this.listViewContact.Size = new System.Drawing.Size(237, 330);
            this.listViewContact.SmallImageList = this.imageListSmall;
            this.listViewContact.TabIndex = 0;
            this.listViewContact.UseCompatibleStateImageBehavior = false;
            this.listViewContact.View = System.Windows.Forms.View.SmallIcon;
            this.listViewContact.SelectedIndexChanged += new System.EventHandler(this.listViewContact_SelectedIndexChanged);
            // 
            // contextMenuStripContacts
            // 
            this.contextMenuStripContacts.Name = "contextMenuStripContacts";
            this.contextMenuStripContacts.Size = new System.Drawing.Size(61, 4);
            // 
            // imageListLarge
            // 
            this.imageListLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListLarge.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListLarge.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "cellphone.png");
            // 
            // VoIPControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxContact);
            this.Controls.Add(this.groupBoxAccount);
            this.Controls.Add(this.groupBoxCall);
            this.Name = "VoIPControl";
            this.Size = new System.Drawing.Size(509, 390);
            this.Load += new System.EventHandler(this.VoIPControl_Load);
            this.groupBoxCall.ResumeLayout(false);
            this.groupBoxDigits.ResumeLayout(false);
            this.groupBoxDigits.PerformLayout();
            this.groupBoxAccount.ResumeLayout(false);
            this.groupBoxAccount.PerformLayout();
            this.groupBoxContact.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCall;
        private System.Windows.Forms.GroupBox groupBoxAccount;
        private System.Windows.Forms.GroupBox groupBoxContact;
        private System.Windows.Forms.ListView listViewContact;
        private System.Windows.Forms.Label labelRegistationStatusState;
        private System.Windows.Forms.Label labelRegistationStatus;
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonHangup;
        private System.Windows.Forms.Button buttonCall;
        private System.Windows.Forms.ComboBox comboBoxCallNumber;
        private System.Windows.Forms.GroupBox groupBoxDigits;
        private System.Windows.Forms.Button buttonOne;
        private System.Windows.Forms.Button buttonTwo;
        private System.Windows.Forms.Button buttonThree;
        private System.Windows.Forms.Button buttonNine;
        private System.Windows.Forms.Button buttonFour;
        private System.Windows.Forms.Button buttonFive;
        private System.Windows.Forms.Button buttonSix;
        private System.Windows.Forms.Button buttonSeven;
        private System.Windows.Forms.Button buttonEight;
        private System.Windows.Forms.Button buttonZero;
        private System.Windows.Forms.Button buttonStar;
        private System.Windows.Forms.Button buttonHash;
        private System.Windows.Forms.TextBox textBoxDigits;
        private System.Windows.Forms.Button buttonInstantMessage;
        private System.Windows.Forms.Button buttonLoadContacts;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.ImageList imageListSmall;
        private System.Windows.Forms.Button buttonContactDelete;
        private System.Windows.Forms.Button buttonContactAdd;
        private System.Windows.Forms.Button buttonContactUpdate;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripContacts;
    }
}
