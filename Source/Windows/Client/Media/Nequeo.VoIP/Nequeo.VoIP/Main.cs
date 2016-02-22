using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.VoIP
{
    /// <summary>
    /// Main.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// Main.
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        private int _voipTabPageCount = 2;

        /// <summary>
        /// Closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // For each tab page created
            // iterate through each tab page.
            foreach (Control control in tabControlVoIP.Controls)
            {
                // If the control type in the
                // tab is a tab page
                if (control is TabPage)
                {
                    // For each control in the
                    // tab page.
                    foreach (Control voipControl in control.Controls)
                    {
                        try
                        {
                            // If the control in the tab page
                            // is a voip control.
                            if (voipControl is Nequeo.VoIP.PjSip.UI.VoIPControl)
                            {
                                Nequeo.VoIP.PjSip.UI.VoIPControl voip =
                                    (Nequeo.VoIP.PjSip.UI.VoIPControl)voipControl;

                                // Dispose
                                voip.DisposeCall();
                            }
                        }
                        catch { }

                        try
                        {
                            // If the control in the tab page
                            // is a voip control.
                            if (voipControl is Nequeo.VoIP.Sip.UI.VoIPControl)
                            {
                                Nequeo.VoIP.Sip.UI.VoIPControl voip =
                                    (Nequeo.VoIP.Sip.UI.VoIPControl)voipControl;

                                // Dispose
                                voip.DisposeCall();
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// On load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            // Initlise the first control.
            voIPControl1.Initialize();
        }

        /// <summary>
        /// Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Add voice and video.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void voiceVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create the control.
            Nequeo.VoIP.PjSip.UI.VoIPControl voip = new PjSip.UI.VoIPControl();

            // Create and format the new tab
            System.Windows.Forms.TabPage tabPageVoipNew = new TabPage();
            tabPageVoipNew.Controls.Add(voip);
            tabPageVoipNew.Location = new System.Drawing.Point(4, 22);
            tabPageVoipNew.Name = "tabPageVoIP" + (_voipTabPageCount++).ToString();
            tabPageVoipNew.Padding = new System.Windows.Forms.Padding(3);
            tabPageVoipNew.Size = new System.Drawing.Size(this.tabControlVoIP.Size.Width - 9, this.tabControlVoIP.Size.Height - 27);
            tabPageVoipNew.Text = "VoIP Account Voice & Video";
            tabPageVoipNew.UseVisualStyleBackColor = true;

            // Format the order control.
            voip.AudioRecordingInCall = false;
            voip.AudioRecordingInCallPath = null;
            voip.AudioRecordingOutCall = false;
            voip.AudioRecordingOutCallPath = null;
            voip.ContactsFilePath = null;
            voip.Dock = System.Windows.Forms.DockStyle.Fill;
            voip.Location = new System.Drawing.Point(3, 3);
            voip.Name = "voIPControl" + (_voipTabPageCount++).ToString();
            voip.Size = new System.Drawing.Size(this.tabControlVoIP.Size.Width - 21, this.tabControlVoIP.Size.Height - 39);

            // Add the new tab object to the tab control.
            this.tabControlVoIP.Controls.Add(tabPageVoipNew);

            // Initlise the first control.
            voip.Initialize();
        }

        /// <summary>
        /// Add voice.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void voiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create the control.
            Nequeo.VoIP.Sip.UI.VoIPControl voip = new Sip.UI.VoIPControl();

            // Create and format the new tab
            System.Windows.Forms.TabPage tabPageVoipNew = new TabPage();
            tabPageVoipNew.Controls.Add(voip);
            tabPageVoipNew.Location = new System.Drawing.Point(4, 22);
            tabPageVoipNew.Name = "tabPageVoIP" + (_voipTabPageCount++).ToString();
            tabPageVoipNew.Padding = new System.Windows.Forms.Padding(3);
            tabPageVoipNew.Size = new System.Drawing.Size(this.tabControlVoIP.Size.Width - 9, this.tabControlVoIP.Size.Height - 27);
            tabPageVoipNew.Text = "VoIP Account Voice";
            tabPageVoipNew.UseVisualStyleBackColor = true;

            // Format the order control.
            voip.AudioRecordingInCall = false;
            voip.AudioRecordingInCallPath = null;
            voip.AudioRecordingOutCall = false;
            voip.AudioRecordingOutCallPath = null;
            voip.ContactsFilePath = null;
            voip.Dock = System.Windows.Forms.DockStyle.Fill;
            voip.Location = new System.Drawing.Point(3, 3);
            voip.Name = "voIPControl" + (_voipTabPageCount++).ToString();
            voip.Size = new System.Drawing.Size(this.tabControlVoIP.Size.Width - 21, this.tabControlVoIP.Size.Height - 39);

            // Add the new tab object to the tab control.
            this.tabControlVoIP.Controls.Add(tabPageVoipNew);

            // Initlise the first control.
            voip.Initialize();
        }

        /// <summary>
        /// Add voip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addVoIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Open window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
