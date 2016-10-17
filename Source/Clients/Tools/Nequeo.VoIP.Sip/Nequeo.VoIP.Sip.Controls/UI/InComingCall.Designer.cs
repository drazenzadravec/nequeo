namespace Nequeo.VoIP.Sip.UI
{
    partial class InComingCall
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
            this.textBoxDetails = new System.Windows.Forms.TextBox();
            this.buttonAnswer = new System.Windows.Forms.Button();
            this.buttonHangup = new System.Windows.Forms.Button();
            this.groupBoxDigits = new System.Windows.Forms.GroupBox();
            this.buttonDigitsClear = new System.Windows.Forms.Button();
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
            this.buttonSendToMessageBank = new System.Windows.Forms.Button();
            this.buttonAddToConferenceCall = new System.Windows.Forms.Button();
            this.checkBoxSuspend = new System.Windows.Forms.CheckBox();
            this.buttonTransfer = new System.Windows.Forms.Button();
            this.buttonHold = new System.Windows.Forms.Button();
            this.statusStripCall = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelAuto = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelCallerImage = new System.Windows.Forms.Panel();
            this.groupBoxDigits.SuspendLayout();
            this.statusStripCall.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDetails
            // 
            this.textBoxDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxDetails.Location = new System.Drawing.Point(12, 12);
            this.textBoxDetails.Multiline = true;
            this.textBoxDetails.Name = "textBoxDetails";
            this.textBoxDetails.Size = new System.Drawing.Size(257, 126);
            this.textBoxDetails.TabIndex = 0;
            // 
            // buttonAnswer
            // 
            this.buttonAnswer.Location = new System.Drawing.Point(437, 176);
            this.buttonAnswer.Name = "buttonAnswer";
            this.buttonAnswer.Size = new System.Drawing.Size(75, 23);
            this.buttonAnswer.TabIndex = 1;
            this.buttonAnswer.Text = "Answer";
            this.buttonAnswer.UseVisualStyleBackColor = true;
            this.buttonAnswer.Click += new System.EventHandler(this.buttonAnswer_Click);
            // 
            // buttonHangup
            // 
            this.buttonHangup.Location = new System.Drawing.Point(356, 176);
            this.buttonHangup.Name = "buttonHangup";
            this.buttonHangup.Size = new System.Drawing.Size(75, 23);
            this.buttonHangup.TabIndex = 2;
            this.buttonHangup.Text = "Hangup";
            this.buttonHangup.UseVisualStyleBackColor = true;
            this.buttonHangup.Click += new System.EventHandler(this.buttonHangup_Click);
            // 
            // groupBoxDigits
            // 
            this.groupBoxDigits.Controls.Add(this.buttonDigitsClear);
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
            this.groupBoxDigits.Location = new System.Drawing.Point(279, 6);
            this.groupBoxDigits.Name = "groupBoxDigits";
            this.groupBoxDigits.Size = new System.Drawing.Size(233, 164);
            this.groupBoxDigits.TabIndex = 17;
            this.groupBoxDigits.TabStop = false;
            // 
            // buttonDigitsClear
            // 
            this.buttonDigitsClear.Location = new System.Drawing.Point(153, 12);
            this.buttonDigitsClear.Name = "buttonDigitsClear";
            this.buttonDigitsClear.Size = new System.Drawing.Size(61, 23);
            this.buttonDigitsClear.TabIndex = 8;
            this.buttonDigitsClear.Text = "Clear";
            this.buttonDigitsClear.UseVisualStyleBackColor = true;
            this.buttonDigitsClear.Click += new System.EventHandler(this.buttonDigitsClear_Click);
            // 
            // textBoxDigits
            // 
            this.textBoxDigits.Location = new System.Drawing.Point(19, 14);
            this.textBoxDigits.Name = "textBoxDigits";
            this.textBoxDigits.ReadOnly = true;
            this.textBoxDigits.Size = new System.Drawing.Size(128, 20);
            this.textBoxDigits.TabIndex = 2;
            // 
            // buttonHash
            // 
            this.buttonHash.Location = new System.Drawing.Point(153, 127);
            this.buttonHash.Name = "buttonHash";
            this.buttonHash.Size = new System.Drawing.Size(61, 23);
            this.buttonHash.TabIndex = 20;
            this.buttonHash.Text = "#";
            this.buttonHash.UseVisualStyleBackColor = true;
            this.buttonHash.Click += new System.EventHandler(this.buttonHash_Click);
            // 
            // buttonStar
            // 
            this.buttonStar.Location = new System.Drawing.Point(19, 127);
            this.buttonStar.Name = "buttonStar";
            this.buttonStar.Size = new System.Drawing.Size(61, 23);
            this.buttonStar.TabIndex = 18;
            this.buttonStar.Text = "*";
            this.buttonStar.UseVisualStyleBackColor = true;
            this.buttonStar.Click += new System.EventHandler(this.buttonStar_Click);
            // 
            // buttonZero
            // 
            this.buttonZero.Location = new System.Drawing.Point(86, 127);
            this.buttonZero.Name = "buttonZero";
            this.buttonZero.Size = new System.Drawing.Size(61, 23);
            this.buttonZero.TabIndex = 19;
            this.buttonZero.Text = "0";
            this.buttonZero.UseVisualStyleBackColor = true;
            this.buttonZero.Click += new System.EventHandler(this.buttonZero_Click);
            // 
            // buttonEight
            // 
            this.buttonEight.Location = new System.Drawing.Point(86, 98);
            this.buttonEight.Name = "buttonEight";
            this.buttonEight.Size = new System.Drawing.Size(61, 23);
            this.buttonEight.TabIndex = 16;
            this.buttonEight.Text = "8 tuv";
            this.buttonEight.UseVisualStyleBackColor = true;
            this.buttonEight.Click += new System.EventHandler(this.buttonEight_Click);
            // 
            // buttonSeven
            // 
            this.buttonSeven.Location = new System.Drawing.Point(19, 98);
            this.buttonSeven.Name = "buttonSeven";
            this.buttonSeven.Size = new System.Drawing.Size(61, 23);
            this.buttonSeven.TabIndex = 15;
            this.buttonSeven.Text = "7 pqrs";
            this.buttonSeven.UseVisualStyleBackColor = true;
            this.buttonSeven.Click += new System.EventHandler(this.buttonSeven_Click);
            // 
            // buttonSix
            // 
            this.buttonSix.Location = new System.Drawing.Point(153, 69);
            this.buttonSix.Name = "buttonSix";
            this.buttonSix.Size = new System.Drawing.Size(61, 23);
            this.buttonSix.TabIndex = 14;
            this.buttonSix.Text = "6 mno";
            this.buttonSix.UseVisualStyleBackColor = true;
            this.buttonSix.Click += new System.EventHandler(this.buttonSix_Click);
            // 
            // buttonFive
            // 
            this.buttonFive.Location = new System.Drawing.Point(86, 69);
            this.buttonFive.Name = "buttonFive";
            this.buttonFive.Size = new System.Drawing.Size(61, 23);
            this.buttonFive.TabIndex = 13;
            this.buttonFive.Text = "5 jkl";
            this.buttonFive.UseVisualStyleBackColor = true;
            this.buttonFive.Click += new System.EventHandler(this.buttonFive_Click);
            // 
            // buttonFour
            // 
            this.buttonFour.Location = new System.Drawing.Point(19, 69);
            this.buttonFour.Name = "buttonFour";
            this.buttonFour.Size = new System.Drawing.Size(61, 23);
            this.buttonFour.TabIndex = 12;
            this.buttonFour.Text = "4 ghi";
            this.buttonFour.UseVisualStyleBackColor = true;
            this.buttonFour.Click += new System.EventHandler(this.buttonFour_Click);
            // 
            // buttonNine
            // 
            this.buttonNine.Location = new System.Drawing.Point(153, 98);
            this.buttonNine.Name = "buttonNine";
            this.buttonNine.Size = new System.Drawing.Size(61, 23);
            this.buttonNine.TabIndex = 17;
            this.buttonNine.Text = "9 wxyz";
            this.buttonNine.UseVisualStyleBackColor = true;
            this.buttonNine.Click += new System.EventHandler(this.buttonNine_Click);
            // 
            // buttonThree
            // 
            this.buttonThree.Location = new System.Drawing.Point(153, 40);
            this.buttonThree.Name = "buttonThree";
            this.buttonThree.Size = new System.Drawing.Size(61, 23);
            this.buttonThree.TabIndex = 11;
            this.buttonThree.Text = "3 def";
            this.buttonThree.UseVisualStyleBackColor = true;
            this.buttonThree.Click += new System.EventHandler(this.buttonThree_Click);
            // 
            // buttonTwo
            // 
            this.buttonTwo.Location = new System.Drawing.Point(86, 40);
            this.buttonTwo.Name = "buttonTwo";
            this.buttonTwo.Size = new System.Drawing.Size(61, 23);
            this.buttonTwo.TabIndex = 10;
            this.buttonTwo.Text = "2 abc";
            this.buttonTwo.UseVisualStyleBackColor = true;
            this.buttonTwo.Click += new System.EventHandler(this.buttonTwo_Click);
            // 
            // buttonOne
            // 
            this.buttonOne.Location = new System.Drawing.Point(19, 40);
            this.buttonOne.Name = "buttonOne";
            this.buttonOne.Size = new System.Drawing.Size(61, 23);
            this.buttonOne.TabIndex = 9;
            this.buttonOne.Text = "1";
            this.buttonOne.UseVisualStyleBackColor = true;
            this.buttonOne.Click += new System.EventHandler(this.buttonOne_Click);
            // 
            // buttonSendToMessageBank
            // 
            this.buttonSendToMessageBank.Location = new System.Drawing.Point(356, 205);
            this.buttonSendToMessageBank.Name = "buttonSendToMessageBank";
            this.buttonSendToMessageBank.Size = new System.Drawing.Size(156, 23);
            this.buttonSendToMessageBank.TabIndex = 7;
            this.buttonSendToMessageBank.Text = "Send To Messasge Bank";
            this.buttonSendToMessageBank.UseVisualStyleBackColor = true;
            this.buttonSendToMessageBank.Click += new System.EventHandler(this.buttonSendToMessageBank_Click);
            // 
            // buttonAddToConferenceCall
            // 
            this.buttonAddToConferenceCall.Location = new System.Drawing.Point(194, 205);
            this.buttonAddToConferenceCall.Name = "buttonAddToConferenceCall";
            this.buttonAddToConferenceCall.Size = new System.Drawing.Size(156, 23);
            this.buttonAddToConferenceCall.TabIndex = 6;
            this.buttonAddToConferenceCall.Text = "Add To Conference Call";
            this.buttonAddToConferenceCall.UseVisualStyleBackColor = true;
            this.buttonAddToConferenceCall.Click += new System.EventHandler(this.buttonAddToConferenceCall_Click);
            // 
            // checkBoxSuspend
            // 
            this.checkBoxSuspend.AutoSize = true;
            this.checkBoxSuspend.Location = new System.Drawing.Point(194, 153);
            this.checkBoxSuspend.Name = "checkBoxSuspend";
            this.checkBoxSuspend.Size = new System.Drawing.Size(68, 17);
            this.checkBoxSuspend.TabIndex = 5;
            this.checkBoxSuspend.Text = "Suspend";
            this.checkBoxSuspend.UseVisualStyleBackColor = true;
            this.checkBoxSuspend.CheckedChanged += new System.EventHandler(this.checkBoxSuspend_CheckedChanged);
            // 
            // buttonTransfer
            // 
            this.buttonTransfer.Location = new System.Drawing.Point(275, 176);
            this.buttonTransfer.Name = "buttonTransfer";
            this.buttonTransfer.Size = new System.Drawing.Size(75, 23);
            this.buttonTransfer.TabIndex = 3;
            this.buttonTransfer.Text = "Transfer";
            this.buttonTransfer.UseVisualStyleBackColor = true;
            this.buttonTransfer.Click += new System.EventHandler(this.buttonTransfer_Click);
            // 
            // buttonHold
            // 
            this.buttonHold.Enabled = false;
            this.buttonHold.Location = new System.Drawing.Point(194, 176);
            this.buttonHold.Name = "buttonHold";
            this.buttonHold.Size = new System.Drawing.Size(75, 23);
            this.buttonHold.TabIndex = 4;
            this.buttonHold.Text = "Hold";
            this.buttonHold.UseVisualStyleBackColor = true;
            this.buttonHold.Click += new System.EventHandler(this.buttonHold_Click);
            // 
            // statusStripCall
            // 
            this.statusStripCall.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelAuto});
            this.statusStripCall.Location = new System.Drawing.Point(0, 238);
            this.statusStripCall.Name = "statusStripCall";
            this.statusStripCall.Size = new System.Drawing.Size(526, 22);
            this.statusStripCall.TabIndex = 24;
            // 
            // toolStripStatusLabelAuto
            // 
            this.toolStripStatusLabelAuto.Name = "toolStripStatusLabelAuto";
            this.toolStripStatusLabelAuto.Size = new System.Drawing.Size(0, 17);
            // 
            // panelCallerImage
            // 
            this.panelCallerImage.BackColor = System.Drawing.SystemColors.Window;
            this.panelCallerImage.Location = new System.Drawing.Point(12, 144);
            this.panelCallerImage.Name = "panelCallerImage";
            this.panelCallerImage.Size = new System.Drawing.Size(95, 84);
            this.panelCallerImage.TabIndex = 21;
            // 
            // InComingCall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 260);
            this.Controls.Add(this.panelCallerImage);
            this.Controls.Add(this.statusStripCall);
            this.Controls.Add(this.buttonHold);
            this.Controls.Add(this.buttonTransfer);
            this.Controls.Add(this.checkBoxSuspend);
            this.Controls.Add(this.buttonAddToConferenceCall);
            this.Controls.Add(this.buttonSendToMessageBank);
            this.Controls.Add(this.groupBoxDigits);
            this.Controls.Add(this.buttonHangup);
            this.Controls.Add(this.buttonAnswer);
            this.Controls.Add(this.textBoxDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InComingCall";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Incoming Call";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InComingCall_FormClosing);
            this.Load += new System.EventHandler(this.InComingCall_Load);
            this.groupBoxDigits.ResumeLayout(false);
            this.groupBoxDigits.PerformLayout();
            this.statusStripCall.ResumeLayout(false);
            this.statusStripCall.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDetails;
        private System.Windows.Forms.Button buttonAnswer;
        private System.Windows.Forms.Button buttonHangup;
        private System.Windows.Forms.GroupBox groupBoxDigits;
        private System.Windows.Forms.Button buttonHash;
        private System.Windows.Forms.Button buttonStar;
        private System.Windows.Forms.Button buttonZero;
        private System.Windows.Forms.Button buttonEight;
        private System.Windows.Forms.Button buttonSeven;
        private System.Windows.Forms.Button buttonSix;
        private System.Windows.Forms.Button buttonFive;
        private System.Windows.Forms.Button buttonFour;
        private System.Windows.Forms.Button buttonNine;
        private System.Windows.Forms.Button buttonThree;
        private System.Windows.Forms.Button buttonTwo;
        private System.Windows.Forms.Button buttonOne;
        private System.Windows.Forms.TextBox textBoxDigits;
        private System.Windows.Forms.Button buttonSendToMessageBank;
        private System.Windows.Forms.Button buttonAddToConferenceCall;
        private System.Windows.Forms.CheckBox checkBoxSuspend;
        private System.Windows.Forms.Button buttonDigitsClear;
        private System.Windows.Forms.Button buttonTransfer;
        private System.Windows.Forms.Button buttonHold;
        private System.Windows.Forms.StatusStrip statusStripCall;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelAuto;
        private System.Windows.Forms.Panel panelCallerImage;
    }
}