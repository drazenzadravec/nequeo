namespace Nequeo.Forms.UI.Diagnostics
{
    partial class StopWatch
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
            this.chkSystemHighResTimer = new System.Windows.Forms.CheckBox();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.txtFrequency = new System.Windows.Forms.TextBox();
            this.lblTimeStamp = new System.Windows.Forms.Label();
            this.txtTimeStamp = new System.Windows.Forms.TextBox();
            this.lblElaspedTime = new System.Windows.Forms.Label();
            this.txtElaspedTime = new System.Windows.Forms.TextBox();
            this.lblElaspedMilliseconds = new System.Windows.Forms.Label();
            this.txtElaspedMilliseconds = new System.Windows.Forms.TextBox();
            this.lblElaspedTicks = new System.Windows.Forms.Label();
            this.txtElaspedTicks = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.timerStopWatch = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // chkSystemHighResTimer
            // 
            this.chkSystemHighResTimer.AutoSize = true;
            this.chkSystemHighResTimer.Enabled = false;
            this.chkSystemHighResTimer.Location = new System.Drawing.Point(3, 3);
            this.chkSystemHighResTimer.Name = "chkSystemHighResTimer";
            this.chkSystemHighResTimer.Size = new System.Drawing.Size(183, 17);
            this.chkSystemHighResTimer.TabIndex = 0;
            this.chkSystemHighResTimer.Text = "System has High Resolution timer";
            this.chkSystemHighResTimer.UseVisualStyleBackColor = true;
            // 
            // lblFrequency
            // 
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Location = new System.Drawing.Point(0, 35);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(131, 13);
            this.lblFrequency.TabIndex = 1;
            this.lblFrequency.Text = "Frequency (ticks/second):";
            // 
            // txtFrequency
            // 
            this.txtFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFrequency.Enabled = false;
            this.txtFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFrequency.Location = new System.Drawing.Point(140, 32);
            this.txtFrequency.Name = "txtFrequency";
            this.txtFrequency.Size = new System.Drawing.Size(182, 20);
            this.txtFrequency.TabIndex = 2;
            // 
            // lblTimeStamp
            // 
            this.lblTimeStamp.AutoSize = true;
            this.lblTimeStamp.Location = new System.Drawing.Point(0, 61);
            this.lblTimeStamp.Name = "lblTimeStamp";
            this.lblTimeStamp.Size = new System.Drawing.Size(122, 13);
            this.lblTimeStamp.TabIndex = 3;
            this.lblTimeStamp.Text = "Time Stamp (tick count):";
            // 
            // txtTimeStamp
            // 
            this.txtTimeStamp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTimeStamp.Enabled = false;
            this.txtTimeStamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeStamp.Location = new System.Drawing.Point(140, 58);
            this.txtTimeStamp.Name = "txtTimeStamp";
            this.txtTimeStamp.Size = new System.Drawing.Size(182, 20);
            this.txtTimeStamp.TabIndex = 4;
            // 
            // lblElaspedTime
            // 
            this.lblElaspedTime.AutoSize = true;
            this.lblElaspedTime.Location = new System.Drawing.Point(0, 87);
            this.lblElaspedTime.Name = "lblElaspedTime";
            this.lblElaspedTime.Size = new System.Drawing.Size(99, 13);
            this.lblElaspedTime.TabIndex = 5;
            this.lblElaspedTime.Text = "Elapsed TimeSpan:";
            // 
            // txtElaspedTime
            // 
            this.txtElaspedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtElaspedTime.Enabled = false;
            this.txtElaspedTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtElaspedTime.Location = new System.Drawing.Point(140, 84);
            this.txtElaspedTime.Name = "txtElaspedTime";
            this.txtElaspedTime.Size = new System.Drawing.Size(182, 20);
            this.txtElaspedTime.TabIndex = 6;
            // 
            // lblElaspedMilliseconds
            // 
            this.lblElaspedMilliseconds.AutoSize = true;
            this.lblElaspedMilliseconds.Location = new System.Drawing.Point(0, 113);
            this.lblElaspedMilliseconds.Name = "lblElaspedMilliseconds";
            this.lblElaspedMilliseconds.Size = new System.Drawing.Size(108, 13);
            this.lblElaspedMilliseconds.TabIndex = 7;
            this.lblElaspedMilliseconds.Text = "Elapsed Milliseconds:";
            // 
            // txtElaspedMilliseconds
            // 
            this.txtElaspedMilliseconds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtElaspedMilliseconds.Enabled = false;
            this.txtElaspedMilliseconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtElaspedMilliseconds.Location = new System.Drawing.Point(140, 110);
            this.txtElaspedMilliseconds.Name = "txtElaspedMilliseconds";
            this.txtElaspedMilliseconds.Size = new System.Drawing.Size(182, 20);
            this.txtElaspedMilliseconds.TabIndex = 8;
            // 
            // lblElaspedTicks
            // 
            this.lblElaspedTicks.AutoSize = true;
            this.lblElaspedTicks.Location = new System.Drawing.Point(0, 139);
            this.lblElaspedTicks.Name = "lblElaspedTicks";
            this.lblElaspedTicks.Size = new System.Drawing.Size(77, 13);
            this.lblElaspedTicks.TabIndex = 9;
            this.lblElaspedTicks.Text = "Elapsed Ticks:";
            // 
            // txtElaspedTicks
            // 
            this.txtElaspedTicks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtElaspedTicks.Enabled = false;
            this.txtElaspedTicks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtElaspedTicks.Location = new System.Drawing.Point(140, 136);
            this.txtElaspedTicks.Name = "txtElaspedTicks";
            this.txtElaspedTicks.Size = new System.Drawing.Size(182, 20);
            this.txtElaspedTicks.TabIndex = 10;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(4, 162);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 11;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(85, 162);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 12;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(166, 162);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 13;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.Location = new System.Drawing.Point(247, 162);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 14;
            this.btnNew.Text = "Start New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // timerStopWatch
            // 
            this.timerStopWatch.Tick += new System.EventHandler(this.timerStopWatch_Tick);
            // 
            // StopWatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtElaspedTicks);
            this.Controls.Add(this.lblElaspedTicks);
            this.Controls.Add(this.txtElaspedMilliseconds);
            this.Controls.Add(this.lblElaspedMilliseconds);
            this.Controls.Add(this.txtElaspedTime);
            this.Controls.Add(this.lblElaspedTime);
            this.Controls.Add(this.txtTimeStamp);
            this.Controls.Add(this.lblTimeStamp);
            this.Controls.Add(this.txtFrequency);
            this.Controls.Add(this.lblFrequency);
            this.Controls.Add(this.chkSystemHighResTimer);
            this.Name = "StopWatch";
            this.Size = new System.Drawing.Size(327, 191);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkSystemHighResTimer;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.TextBox txtFrequency;
        private System.Windows.Forms.Label lblTimeStamp;
        private System.Windows.Forms.TextBox txtTimeStamp;
        private System.Windows.Forms.Label lblElaspedTime;
        private System.Windows.Forms.TextBox txtElaspedTime;
        private System.Windows.Forms.Label lblElaspedMilliseconds;
        private System.Windows.Forms.TextBox txtElaspedMilliseconds;
        private System.Windows.Forms.Label lblElaspedTicks;
        private System.Windows.Forms.TextBox txtElaspedTicks;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Timer timerStopWatch;
    }
}
