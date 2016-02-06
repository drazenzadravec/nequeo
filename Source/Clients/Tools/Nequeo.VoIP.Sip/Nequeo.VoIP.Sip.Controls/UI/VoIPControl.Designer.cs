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
            this.groupBoxCall = new System.Windows.Forms.GroupBox();
            this.groupBoxDigits = new System.Windows.Forms.GroupBox();
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
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.labelRegistationStatusState = new System.Windows.Forms.Label();
            this.labelRegistationStatus = new System.Windows.Forms.Label();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.textBoxAccountName = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelHost = new System.Windows.Forms.Label();
            this.labelAccountName = new System.Windows.Forms.Label();
            this.groupBoxContact = new System.Windows.Forms.GroupBox();
            this.listViewContact = new System.Windows.Forms.ListView();
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
            this.groupBoxCall.Size = new System.Drawing.Size(248, 221);
            this.groupBoxCall.TabIndex = 0;
            this.groupBoxCall.TabStop = false;
            this.groupBoxCall.Text = "Call";
            // 
            // groupBoxDigits
            // 
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
            this.groupBoxDigits.Size = new System.Drawing.Size(233, 135);
            this.groupBoxDigits.TabIndex = 16;
            this.groupBoxDigits.TabStop = false;
            // 
            // buttonHash
            // 
            this.buttonHash.Location = new System.Drawing.Point(153, 102);
            this.buttonHash.Name = "buttonHash";
            this.buttonHash.Size = new System.Drawing.Size(61, 23);
            this.buttonHash.TabIndex = 25;
            this.buttonHash.Text = "#";
            this.buttonHash.UseVisualStyleBackColor = true;
            this.buttonHash.Click += new System.EventHandler(this.buttonHash_Click);
            // 
            // buttonStar
            // 
            this.buttonStar.Location = new System.Drawing.Point(19, 102);
            this.buttonStar.Name = "buttonStar";
            this.buttonStar.Size = new System.Drawing.Size(61, 23);
            this.buttonStar.TabIndex = 24;
            this.buttonStar.Text = "*";
            this.buttonStar.UseVisualStyleBackColor = true;
            this.buttonStar.Click += new System.EventHandler(this.buttonStar_Click);
            // 
            // buttonZero
            // 
            this.buttonZero.Location = new System.Drawing.Point(86, 102);
            this.buttonZero.Name = "buttonZero";
            this.buttonZero.Size = new System.Drawing.Size(61, 23);
            this.buttonZero.TabIndex = 23;
            this.buttonZero.Text = "0";
            this.buttonZero.UseVisualStyleBackColor = true;
            this.buttonZero.Click += new System.EventHandler(this.buttonZero_Click);
            // 
            // buttonEight
            // 
            this.buttonEight.Location = new System.Drawing.Point(86, 73);
            this.buttonEight.Name = "buttonEight";
            this.buttonEight.Size = new System.Drawing.Size(61, 23);
            this.buttonEight.TabIndex = 21;
            this.buttonEight.Text = "8 tuv";
            this.buttonEight.UseVisualStyleBackColor = true;
            this.buttonEight.Click += new System.EventHandler(this.buttonEight_Click);
            // 
            // buttonSeven
            // 
            this.buttonSeven.Location = new System.Drawing.Point(19, 73);
            this.buttonSeven.Name = "buttonSeven";
            this.buttonSeven.Size = new System.Drawing.Size(61, 23);
            this.buttonSeven.TabIndex = 20;
            this.buttonSeven.Text = "7 pqrs";
            this.buttonSeven.UseVisualStyleBackColor = true;
            this.buttonSeven.Click += new System.EventHandler(this.buttonSeven_Click);
            // 
            // buttonSix
            // 
            this.buttonSix.Location = new System.Drawing.Point(153, 44);
            this.buttonSix.Name = "buttonSix";
            this.buttonSix.Size = new System.Drawing.Size(61, 23);
            this.buttonSix.TabIndex = 19;
            this.buttonSix.Text = "6 mno";
            this.buttonSix.UseVisualStyleBackColor = true;
            this.buttonSix.Click += new System.EventHandler(this.buttonSix_Click);
            // 
            // buttonFive
            // 
            this.buttonFive.Location = new System.Drawing.Point(86, 44);
            this.buttonFive.Name = "buttonFive";
            this.buttonFive.Size = new System.Drawing.Size(61, 23);
            this.buttonFive.TabIndex = 18;
            this.buttonFive.Text = "5 jkl";
            this.buttonFive.UseVisualStyleBackColor = true;
            this.buttonFive.Click += new System.EventHandler(this.buttonFive_Click);
            // 
            // buttonFour
            // 
            this.buttonFour.Location = new System.Drawing.Point(19, 44);
            this.buttonFour.Name = "buttonFour";
            this.buttonFour.Size = new System.Drawing.Size(61, 23);
            this.buttonFour.TabIndex = 17;
            this.buttonFour.Text = "4 ghi";
            this.buttonFour.UseVisualStyleBackColor = true;
            this.buttonFour.Click += new System.EventHandler(this.buttonFour_Click);
            // 
            // buttonNine
            // 
            this.buttonNine.Location = new System.Drawing.Point(153, 73);
            this.buttonNine.Name = "buttonNine";
            this.buttonNine.Size = new System.Drawing.Size(61, 23);
            this.buttonNine.TabIndex = 22;
            this.buttonNine.Text = "9 wxyz";
            this.buttonNine.UseVisualStyleBackColor = true;
            this.buttonNine.Click += new System.EventHandler(this.buttonNine_Click);
            // 
            // buttonThree
            // 
            this.buttonThree.Location = new System.Drawing.Point(153, 15);
            this.buttonThree.Name = "buttonThree";
            this.buttonThree.Size = new System.Drawing.Size(61, 23);
            this.buttonThree.TabIndex = 16;
            this.buttonThree.Text = "3 def";
            this.buttonThree.UseVisualStyleBackColor = true;
            this.buttonThree.Click += new System.EventHandler(this.buttonThree_Click);
            // 
            // buttonTwo
            // 
            this.buttonTwo.Location = new System.Drawing.Point(86, 15);
            this.buttonTwo.Name = "buttonTwo";
            this.buttonTwo.Size = new System.Drawing.Size(61, 23);
            this.buttonTwo.TabIndex = 15;
            this.buttonTwo.Text = "2 abc";
            this.buttonTwo.UseVisualStyleBackColor = true;
            this.buttonTwo.Click += new System.EventHandler(this.buttonTwo_Click);
            // 
            // buttonOne
            // 
            this.buttonOne.Location = new System.Drawing.Point(19, 15);
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
            // 
            // buttonHangup
            // 
            this.buttonHangup.Enabled = false;
            this.buttonHangup.Location = new System.Drawing.Point(45, 187);
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
            this.buttonCall.Location = new System.Drawing.Point(126, 187);
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
            this.groupBoxAccount.Controls.Add(this.buttonCreate);
            this.groupBoxAccount.Controls.Add(this.buttonSettings);
            this.groupBoxAccount.Controls.Add(this.labelRegistationStatusState);
            this.groupBoxAccount.Controls.Add(this.labelRegistationStatus);
            this.groupBoxAccount.Controls.Add(this.buttonRegister);
            this.groupBoxAccount.Controls.Add(this.textBoxPassword);
            this.groupBoxAccount.Controls.Add(this.textBoxUsername);
            this.groupBoxAccount.Controls.Add(this.textBoxHost);
            this.groupBoxAccount.Controls.Add(this.textBoxAccountName);
            this.groupBoxAccount.Controls.Add(this.labelPassword);
            this.groupBoxAccount.Controls.Add(this.labelUsername);
            this.groupBoxAccount.Controls.Add(this.labelHost);
            this.groupBoxAccount.Controls.Add(this.labelAccountName);
            this.groupBoxAccount.Location = new System.Drawing.Point(3, 230);
            this.groupBoxAccount.Name = "groupBoxAccount";
            this.groupBoxAccount.Size = new System.Drawing.Size(248, 212);
            this.groupBoxAccount.TabIndex = 1;
            this.groupBoxAccount.TabStop = false;
            this.groupBoxAccount.Text = "Account";
            // 
            // buttonCreate
            // 
            this.buttonCreate.Enabled = false;
            this.buttonCreate.Location = new System.Drawing.Point(86, 126);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 5;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(167, 126);
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
            this.labelRegistationStatusState.Location = new System.Drawing.Point(6, 193);
            this.labelRegistationStatusState.Name = "labelRegistationStatusState";
            this.labelRegistationStatusState.Size = new System.Drawing.Size(78, 13);
            this.labelRegistationStatusState.TabIndex = 10;
            this.labelRegistationStatusState.Text = "Not Registered";
            // 
            // labelRegistationStatus
            // 
            this.labelRegistationStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRegistationStatus.AutoSize = true;
            this.labelRegistationStatus.Location = new System.Drawing.Point(6, 171);
            this.labelRegistationStatus.Name = "labelRegistationStatus";
            this.labelRegistationStatus.Size = new System.Drawing.Size(98, 13);
            this.labelRegistationStatus.TabIndex = 9;
            this.labelRegistationStatus.Text = "Registation  None :";
            // 
            // buttonRegister
            // 
            this.buttonRegister.Enabled = false;
            this.buttonRegister.Location = new System.Drawing.Point(5, 126);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonRegister.TabIndex = 6;
            this.buttonRegister.Text = "Register";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(83, 100);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(110, 20);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(83, 74);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(110, 20);
            this.textBoxUsername.TabIndex = 2;
            this.textBoxUsername.TextChanged += new System.EventHandler(this.textBoxUsername_TextChanged);
            // 
            // textBoxHost
            // 
            this.textBoxHost.Location = new System.Drawing.Point(83, 48);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(159, 20);
            this.textBoxHost.TabIndex = 1;
            this.textBoxHost.TextChanged += new System.EventHandler(this.textBoxHost_TextChanged);
            // 
            // textBoxAccountName
            // 
            this.textBoxAccountName.Location = new System.Drawing.Point(83, 22);
            this.textBoxAccountName.Name = "textBoxAccountName";
            this.textBoxAccountName.Size = new System.Drawing.Size(159, 20);
            this.textBoxAccountName.TabIndex = 0;
            this.textBoxAccountName.TextChanged += new System.EventHandler(this.textBoxAccountName_TextChanged);
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(16, 103);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(59, 13);
            this.labelPassword.TabIndex = 3;
            this.labelPassword.Text = "Password :";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(16, 77);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(61, 13);
            this.labelUsername.TabIndex = 2;
            this.labelUsername.Text = "Username :";
            // 
            // labelHost
            // 
            this.labelHost.AutoSize = true;
            this.labelHost.Location = new System.Drawing.Point(16, 51);
            this.labelHost.Name = "labelHost";
            this.labelHost.Size = new System.Drawing.Size(35, 13);
            this.labelHost.TabIndex = 1;
            this.labelHost.Text = "Host :";
            // 
            // labelAccountName
            // 
            this.labelAccountName.AutoSize = true;
            this.labelAccountName.Location = new System.Drawing.Point(16, 25);
            this.labelAccountName.Name = "labelAccountName";
            this.labelAccountName.Size = new System.Drawing.Size(41, 13);
            this.labelAccountName.TabIndex = 0;
            this.labelAccountName.Text = "Name :";
            // 
            // groupBoxContact
            // 
            this.groupBoxContact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxContact.Controls.Add(this.listViewContact);
            this.groupBoxContact.Enabled = false;
            this.groupBoxContact.Location = new System.Drawing.Point(257, 3);
            this.groupBoxContact.Name = "groupBoxContact";
            this.groupBoxContact.Size = new System.Drawing.Size(274, 439);
            this.groupBoxContact.TabIndex = 2;
            this.groupBoxContact.TabStop = false;
            this.groupBoxContact.Text = "Contact";
            // 
            // listViewContact
            // 
            this.listViewContact.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewContact.Location = new System.Drawing.Point(6, 19);
            this.listViewContact.MultiSelect = false;
            this.listViewContact.Name = "listViewContact";
            this.listViewContact.Size = new System.Drawing.Size(262, 414);
            this.listViewContact.TabIndex = 0;
            this.listViewContact.UseCompatibleStateImageBehavior = false;
            // 
            // VoIPControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxContact);
            this.Controls.Add(this.groupBoxAccount);
            this.Controls.Add(this.groupBoxCall);
            this.Name = "VoIPControl";
            this.Size = new System.Drawing.Size(534, 445);
            this.Load += new System.EventHandler(this.VoIPControl_Load);
            this.groupBoxCall.ResumeLayout(false);
            this.groupBoxDigits.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.TextBox textBoxAccountName;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelHost;
        private System.Windows.Forms.Label labelAccountName;
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
    }
}
