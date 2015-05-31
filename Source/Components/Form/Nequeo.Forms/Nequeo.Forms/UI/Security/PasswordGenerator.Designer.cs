namespace Nequeo.Forms.UI.Security
{
    partial class PasswordGenerator
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
            this.lblPasswordGeneratorMin = new System.Windows.Forms.Label();
            this.lblPasswordGeneratorMax = new System.Windows.Forms.Label();
            this.btnPasswordGeneratorGen = new System.Windows.Forms.Button();
            this.txtPasswordGeneratorGen = new System.Windows.Forms.TextBox();
            this.radioButtonLUNS = new System.Windows.Forms.RadioButton();
            this.radioButtonLUN = new System.Windows.Forms.RadioButton();
            this.radioButtonN = new System.Windows.Forms.RadioButton();
            this.radioButtonU = new System.Windows.Forms.RadioButton();
            this.radioButtonL = new System.Windows.Forms.RadioButton();
            this.radioButtonLU = new System.Windows.Forms.RadioButton();
            this.radioButtonLN = new System.Windows.Forms.RadioButton();
            this.radioButtonUN = new System.Windows.Forms.RadioButton();
            this.radioButtonT = new System.Windows.Forms.RadioButton();
            this.radioButtonGUID = new System.Windows.Forms.RadioButton();
            this.txtPasswordGeneratorMax = new Nequeo.Forms.UI.Extender.IntegerTextBox();
            this.txtPasswordGeneratorMin = new Nequeo.Forms.UI.Extender.IntegerTextBox();
            this.passwordStrength = new System.Windows.Forms.Label();
            this.passwordStrengthLevel = new System.Windows.Forms.Label();
            this.lblEntropy = new System.Windows.Forms.Label();
            this.lblEntropyValue = new System.Windows.Forms.Label();
            this.lblPasswordCrackTime = new System.Windows.Forms.Label();
            this.lblPasswordCrackTimeValue = new System.Windows.Forms.Label();
            this.lblPasswordCombinations = new System.Windows.Forms.Label();
            this.txtPasswordCombinations = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPasswordGeneratorMin
            // 
            this.lblPasswordGeneratorMin.AutoSize = true;
            this.lblPasswordGeneratorMin.Location = new System.Drawing.Point(3, 6);
            this.lblPasswordGeneratorMin.Name = "lblPasswordGeneratorMin";
            this.lblPasswordGeneratorMin.Size = new System.Drawing.Size(84, 13);
            this.lblPasswordGeneratorMin.TabIndex = 1;
            this.lblPasswordGeneratorMin.Text = "Minimum Length";
            // 
            // lblPasswordGeneratorMax
            // 
            this.lblPasswordGeneratorMax.AutoSize = true;
            this.lblPasswordGeneratorMax.Location = new System.Drawing.Point(207, 6);
            this.lblPasswordGeneratorMax.Name = "lblPasswordGeneratorMax";
            this.lblPasswordGeneratorMax.Size = new System.Drawing.Size(87, 13);
            this.lblPasswordGeneratorMax.TabIndex = 3;
            this.lblPasswordGeneratorMax.Text = "Maximum Length";
            // 
            // btnPasswordGeneratorGen
            // 
            this.btnPasswordGeneratorGen.Location = new System.Drawing.Point(6, 259);
            this.btnPasswordGeneratorGen.Name = "btnPasswordGeneratorGen";
            this.btnPasswordGeneratorGen.Size = new System.Drawing.Size(75, 23);
            this.btnPasswordGeneratorGen.TabIndex = 5;
            this.btnPasswordGeneratorGen.Text = "Generate";
            this.btnPasswordGeneratorGen.UseVisualStyleBackColor = true;
            this.btnPasswordGeneratorGen.Click += new System.EventHandler(this.btnPasswordGeneratorGen_Click);
            // 
            // txtPasswordGeneratorGen
            // 
            this.txtPasswordGeneratorGen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPasswordGeneratorGen.Location = new System.Drawing.Point(87, 261);
            this.txtPasswordGeneratorGen.Name = "txtPasswordGeneratorGen";
            this.txtPasswordGeneratorGen.Size = new System.Drawing.Size(299, 20);
            this.txtPasswordGeneratorGen.TabIndex = 6;
            // 
            // radioButtonLUNS
            // 
            this.radioButtonLUNS.AutoSize = true;
            this.radioButtonLUNS.Checked = true;
            this.radioButtonLUNS.Location = new System.Drawing.Point(6, 29);
            this.radioButtonLUNS.Name = "radioButtonLUNS";
            this.radioButtonLUNS.Size = new System.Drawing.Size(360, 17);
            this.radioButtonLUNS.TabIndex = 9;
            this.radioButtonLUNS.TabStop = true;
            this.radioButtonLUNS.Text = "Lower Case, Upper Case, Numbers and Special Characters (abAB45_-)";
            this.radioButtonLUNS.UseVisualStyleBackColor = true;
            this.radioButtonLUNS.CheckedChanged += new System.EventHandler(this.radioButtonLUNS_CheckedChanged);
            // 
            // radioButtonLUN
            // 
            this.radioButtonLUN.AutoSize = true;
            this.radioButtonLUN.Location = new System.Drawing.Point(6, 52);
            this.radioButtonLUN.Name = "radioButtonLUN";
            this.radioButtonLUN.Size = new System.Drawing.Size(256, 17);
            this.radioButtonLUN.TabIndex = 10;
            this.radioButtonLUN.TabStop = true;
            this.radioButtonLUN.Text = "Lower Case, Upper Case and Numbers (abAB45)";
            this.radioButtonLUN.UseVisualStyleBackColor = true;
            this.radioButtonLUN.CheckedChanged += new System.EventHandler(this.radioButtonLUN_CheckedChanged);
            // 
            // radioButtonN
            // 
            this.radioButtonN.AutoSize = true;
            this.radioButtonN.Location = new System.Drawing.Point(6, 190);
            this.radioButtonN.Name = "radioButtonN";
            this.radioButtonN.Size = new System.Drawing.Size(88, 17);
            this.radioButtonN.TabIndex = 11;
            this.radioButtonN.TabStop = true;
            this.radioButtonN.Text = "Numbers (45)";
            this.radioButtonN.UseVisualStyleBackColor = true;
            this.radioButtonN.CheckedChanged += new System.EventHandler(this.radioButtonN_CheckedChanged);
            // 
            // radioButtonU
            // 
            this.radioButtonU.AutoSize = true;
            this.radioButtonU.Location = new System.Drawing.Point(6, 167);
            this.radioButtonU.Name = "radioButtonU";
            this.radioButtonU.Size = new System.Drawing.Size(104, 17);
            this.radioButtonU.TabIndex = 12;
            this.radioButtonU.TabStop = true;
            this.radioButtonU.Text = "Upper Case (AB)";
            this.radioButtonU.UseVisualStyleBackColor = true;
            this.radioButtonU.CheckedChanged += new System.EventHandler(this.radioButtonU_CheckedChanged);
            // 
            // radioButtonL
            // 
            this.radioButtonL.AutoSize = true;
            this.radioButtonL.Location = new System.Drawing.Point(6, 144);
            this.radioButtonL.Name = "radioButtonL";
            this.radioButtonL.Size = new System.Drawing.Size(102, 17);
            this.radioButtonL.TabIndex = 13;
            this.radioButtonL.TabStop = true;
            this.radioButtonL.Text = "Lower Case (ab)";
            this.radioButtonL.UseVisualStyleBackColor = true;
            this.radioButtonL.CheckedChanged += new System.EventHandler(this.radioButtonL_CheckedChanged);
            // 
            // radioButtonLU
            // 
            this.radioButtonLU.AutoSize = true;
            this.radioButtonLU.Location = new System.Drawing.Point(6, 75);
            this.radioButtonLU.Name = "radioButtonLU";
            this.radioButtonLU.Size = new System.Drawing.Size(196, 17);
            this.radioButtonLU.TabIndex = 14;
            this.radioButtonLU.TabStop = true;
            this.radioButtonLU.Text = "Lower Case and Upper Case (abAB)";
            this.radioButtonLU.UseVisualStyleBackColor = true;
            this.radioButtonLU.CheckedChanged += new System.EventHandler(this.radioButtonLU_CheckedChanged);
            // 
            // radioButtonLN
            // 
            this.radioButtonLN.AutoSize = true;
            this.radioButtonLN.Location = new System.Drawing.Point(6, 98);
            this.radioButtonLN.Name = "radioButtonLN";
            this.radioButtonLN.Size = new System.Drawing.Size(180, 17);
            this.radioButtonLN.TabIndex = 15;
            this.radioButtonLN.TabStop = true;
            this.radioButtonLN.Text = "Lower Case and Numbers (ab45)";
            this.radioButtonLN.UseVisualStyleBackColor = true;
            this.radioButtonLN.CheckedChanged += new System.EventHandler(this.radioButtonLN_CheckedChanged);
            // 
            // radioButtonUN
            // 
            this.radioButtonUN.AutoSize = true;
            this.radioButtonUN.Location = new System.Drawing.Point(6, 121);
            this.radioButtonUN.Name = "radioButtonUN";
            this.radioButtonUN.Size = new System.Drawing.Size(182, 17);
            this.radioButtonUN.TabIndex = 16;
            this.radioButtonUN.TabStop = true;
            this.radioButtonUN.Text = "Upper Case and Numbers (AB45)";
            this.radioButtonUN.UseVisualStyleBackColor = true;
            this.radioButtonUN.CheckedChanged += new System.EventHandler(this.radioButtonUN_CheckedChanged);
            // 
            // radioButtonT
            // 
            this.radioButtonT.AutoSize = true;
            this.radioButtonT.Location = new System.Drawing.Point(6, 213);
            this.radioButtonT.Name = "radioButtonT";
            this.radioButtonT.Size = new System.Drawing.Size(56, 17);
            this.radioButtonT.TabIndex = 17;
            this.radioButtonT.TabStop = true;
            this.radioButtonT.Text = "Token";
            this.radioButtonT.UseVisualStyleBackColor = true;
            this.radioButtonT.CheckedChanged += new System.EventHandler(this.radioButtonT_CheckedChanged);
            // 
            // radioButtonGUID
            // 
            this.radioButtonGUID.AutoSize = true;
            this.radioButtonGUID.Location = new System.Drawing.Point(6, 236);
            this.radioButtonGUID.Name = "radioButtonGUID";
            this.radioButtonGUID.Size = new System.Drawing.Size(278, 17);
            this.radioButtonGUID.TabIndex = 18;
            this.radioButtonGUID.TabStop = true;
            this.radioButtonGUID.Text = "GUID (3FAB0B79-C97B-49C1-8BAC-16D0B113352E)";
            this.radioButtonGUID.UseVisualStyleBackColor = true;
            this.radioButtonGUID.CheckedChanged += new System.EventHandler(this.radioButtonGUID_CheckedChanged);
            // 
            // txtPasswordGeneratorMax
            // 
            this.txtPasswordGeneratorMax.Location = new System.Drawing.Point(300, 3);
            this.txtPasswordGeneratorMax.Name = "txtPasswordGeneratorMax";
            this.txtPasswordGeneratorMax.Size = new System.Drawing.Size(86, 20);
            this.txtPasswordGeneratorMax.TabIndex = 8;
            this.txtPasswordGeneratorMax.Text = "20";
            this.txtPasswordGeneratorMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPasswordGeneratorMax.TextChanged += new System.EventHandler(this.txtPasswordGeneratorMax_TextChanged);
            // 
            // txtPasswordGeneratorMin
            // 
            this.txtPasswordGeneratorMin.Location = new System.Drawing.Point(93, 3);
            this.txtPasswordGeneratorMin.Name = "txtPasswordGeneratorMin";
            this.txtPasswordGeneratorMin.Size = new System.Drawing.Size(86, 20);
            this.txtPasswordGeneratorMin.TabIndex = 7;
            this.txtPasswordGeneratorMin.Text = "20";
            this.txtPasswordGeneratorMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPasswordGeneratorMin.TextChanged += new System.EventHandler(this.txtPasswordGeneratorMin_TextChanged);
            // 
            // passwordStrength
            // 
            this.passwordStrength.AutoSize = true;
            this.passwordStrength.Location = new System.Drawing.Point(3, 285);
            this.passwordStrength.Name = "passwordStrength";
            this.passwordStrength.Size = new System.Drawing.Size(105, 13);
            this.passwordStrength.TabIndex = 19;
            this.passwordStrength.Text = "Password Strength : ";
            // 
            // passwordStrengthLevel
            // 
            this.passwordStrengthLevel.AutoSize = true;
            this.passwordStrengthLevel.Location = new System.Drawing.Point(114, 285);
            this.passwordStrengthLevel.Name = "passwordStrengthLevel";
            this.passwordStrengthLevel.Size = new System.Drawing.Size(0, 13);
            this.passwordStrengthLevel.TabIndex = 20;
            // 
            // lblEntropy
            // 
            this.lblEntropy.AutoSize = true;
            this.lblEntropy.Location = new System.Drawing.Point(3, 307);
            this.lblEntropy.Name = "lblEntropy";
            this.lblEntropy.Size = new System.Drawing.Size(52, 13);
            this.lblEntropy.TabIndex = 21;
            this.lblEntropy.Text = "Entropy : ";
            // 
            // lblEntropyValue
            // 
            this.lblEntropyValue.AutoSize = true;
            this.lblEntropyValue.Location = new System.Drawing.Point(61, 307);
            this.lblEntropyValue.Name = "lblEntropyValue";
            this.lblEntropyValue.Size = new System.Drawing.Size(0, 13);
            this.lblEntropyValue.TabIndex = 22;
            // 
            // lblPasswordCrackTime
            // 
            this.lblPasswordCrackTime.AutoSize = true;
            this.lblPasswordCrackTime.Location = new System.Drawing.Point(3, 329);
            this.lblPasswordCrackTime.Name = "lblPasswordCrackTime";
            this.lblPasswordCrackTime.Size = new System.Drawing.Size(116, 13);
            this.lblPasswordCrackTime.TabIndex = 23;
            this.lblPasswordCrackTime.Text = "Password Crack Time :";
            // 
            // lblPasswordCrackTimeValue
            // 
            this.lblPasswordCrackTimeValue.AutoSize = true;
            this.lblPasswordCrackTimeValue.Location = new System.Drawing.Point(125, 329);
            this.lblPasswordCrackTimeValue.Name = "lblPasswordCrackTimeValue";
            this.lblPasswordCrackTimeValue.Size = new System.Drawing.Size(0, 13);
            this.lblPasswordCrackTimeValue.TabIndex = 24;
            // 
            // lblPasswordCombinations
            // 
            this.lblPasswordCombinations.AutoSize = true;
            this.lblPasswordCombinations.Location = new System.Drawing.Point(3, 351);
            this.lblPasswordCombinations.Name = "lblPasswordCombinations";
            this.lblPasswordCombinations.Size = new System.Drawing.Size(125, 13);
            this.lblPasswordCombinations.TabIndex = 25;
            this.lblPasswordCombinations.Text = "Password Combinations :";
            // 
            // txtPasswordCombinations
            // 
            this.txtPasswordCombinations.AutoSize = true;
            this.txtPasswordCombinations.Location = new System.Drawing.Point(134, 351);
            this.txtPasswordCombinations.Name = "txtPasswordCombinations";
            this.txtPasswordCombinations.Size = new System.Drawing.Size(0, 13);
            this.txtPasswordCombinations.TabIndex = 26;
            // 
            // PasswordGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtPasswordCombinations);
            this.Controls.Add(this.lblPasswordCombinations);
            this.Controls.Add(this.lblPasswordCrackTimeValue);
            this.Controls.Add(this.lblPasswordCrackTime);
            this.Controls.Add(this.lblEntropyValue);
            this.Controls.Add(this.lblEntropy);
            this.Controls.Add(this.passwordStrengthLevel);
            this.Controls.Add(this.passwordStrength);
            this.Controls.Add(this.radioButtonGUID);
            this.Controls.Add(this.radioButtonT);
            this.Controls.Add(this.radioButtonUN);
            this.Controls.Add(this.radioButtonLN);
            this.Controls.Add(this.radioButtonLU);
            this.Controls.Add(this.radioButtonL);
            this.Controls.Add(this.radioButtonU);
            this.Controls.Add(this.radioButtonN);
            this.Controls.Add(this.radioButtonLUN);
            this.Controls.Add(this.radioButtonLUNS);
            this.Controls.Add(this.txtPasswordGeneratorMax);
            this.Controls.Add(this.txtPasswordGeneratorMin);
            this.Controls.Add(this.txtPasswordGeneratorGen);
            this.Controls.Add(this.btnPasswordGeneratorGen);
            this.Controls.Add(this.lblPasswordGeneratorMax);
            this.Controls.Add(this.lblPasswordGeneratorMin);
            this.Name = "PasswordGenerator";
            this.Size = new System.Drawing.Size(392, 374);
            this.Load += new System.EventHandler(this.PasswordGenerator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPasswordGeneratorMin;
        private System.Windows.Forms.Label lblPasswordGeneratorMax;
        private System.Windows.Forms.Button btnPasswordGeneratorGen;
        private System.Windows.Forms.TextBox txtPasswordGeneratorGen;
        private Extender.IntegerTextBox txtPasswordGeneratorMin;
        private Extender.IntegerTextBox txtPasswordGeneratorMax;
        private System.Windows.Forms.RadioButton radioButtonLUNS;
        private System.Windows.Forms.RadioButton radioButtonLUN;
        private System.Windows.Forms.RadioButton radioButtonN;
        private System.Windows.Forms.RadioButton radioButtonU;
        private System.Windows.Forms.RadioButton radioButtonL;
        private System.Windows.Forms.RadioButton radioButtonLU;
        private System.Windows.Forms.RadioButton radioButtonLN;
        private System.Windows.Forms.RadioButton radioButtonUN;
        private System.Windows.Forms.RadioButton radioButtonT;
        private System.Windows.Forms.RadioButton radioButtonGUID;
        private System.Windows.Forms.Label passwordStrength;
        private System.Windows.Forms.Label passwordStrengthLevel;
        private System.Windows.Forms.Label lblEntropy;
        private System.Windows.Forms.Label lblEntropyValue;
        private System.Windows.Forms.Label lblPasswordCrackTime;
        private System.Windows.Forms.Label lblPasswordCrackTimeValue;
        private System.Windows.Forms.Label lblPasswordCombinations;
        private System.Windows.Forms.Label txtPasswordCombinations;
    }
}
