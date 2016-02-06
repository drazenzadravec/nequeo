/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.VoIP.Sip.UI
{
    /// <summary>
    /// Settings.
    /// </summary>
    public partial class Settings : Form
    {
        /// <summary>
        /// Settings.
        /// </summary>
        /// <param name="voipCall">VoIP call.</param>
        public Settings(Nequeo.VoIP.Sip.VoIPCall voipCall)
        {
            InitializeComponent();
            _voipCall = voipCall;
        }

        private Nequeo.VoIP.Sip.VoIPCall _voipCall = null;

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Load(object sender, EventArgs e)
        {
            // Get the audio devices.
            Nequeo.Net.Sip.AudioDeviceInfo[] audioDevices = _voipCall.VoIPManager.MediaManager.GetAllAudioDevices();

            // For each audio device
            foreach (Nequeo.Net.Sip.AudioDeviceInfo audioDevice in audioDevices)
            {
                comboBoxAudioCaptureDevice.Items.Add(audioDevice.Name + " | " + audioDevice.Driver);
                comboBoxAudioPlaybackDevice.Items.Add(audioDevice.Name + " | " + audioDevice.Driver);
            }

            // Set any initial items.
            int captureIndex = _voipCall.VoIPManager.MediaManager.GetCaptureDevice();
            int playbackIndex = _voipCall.VoIPManager.MediaManager.GetPlaybackDevice();

            // Set the selected.
            if (captureIndex >= 0)
                comboBoxAudioCaptureDevice.SelectedIndex = captureIndex;

            if (playbackIndex >= 0)
                comboBoxAudioPlaybackDevice.SelectedIndex = playbackIndex;

            textBoxSipPort.Text = _voipCall.VoIPManager.AccountConnection.SpPort.ToString();
            checkBoxIsDefault.Checked = _voipCall.VoIPManager.AccountConnection.IsDefault;
            textBoxPriority.Text = _voipCall.VoIPManager.AccountConnection.Priority.ToString();
            checkBoxDropCallsOnFail.Checked = _voipCall.VoIPManager.AccountConnection.DropCallsOnFail;
        }

        /// <summary>
        /// Capture changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAudioCaptureDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If item is selected.
            if (comboBoxAudioCaptureDevice.SelectedIndex >= 0)
            {
                // Get the device.
                string[] device = ((string)comboBoxAudioCaptureDevice.SelectedItem).Split(new char[] { '|' });
                string name = device[0].Trim();
                string driver = device[1].Trim();

                // Get the capture index.
                int captureIndex = _voipCall.VoIPManager.MediaManager.GetAudioDeviceID(driver, name);

                // Set the capture device.
                _voipCall.SetAudioCaptureDevice(captureIndex);
            }
        }

        /// <summary>
        /// Playback changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAudioPlaybackDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If item is selected.
            if (comboBoxAudioPlaybackDevice.SelectedIndex >= 0)
            {
                // Get the device.
                string[] device = ((string)comboBoxAudioPlaybackDevice.SelectedItem).Split(new char[] { '|' });
                string name = device[0].Trim();
                string driver = device[1].Trim();

                // Get the playback index.
                int playbackIndex = _voipCall.VoIPManager.MediaManager.GetAudioDeviceID(driver, name);

                // Set the playback device.
                _voipCall.SetAudioPlaybackDevice(playbackIndex);
            }
        }

        /// <summary>
        /// Sip port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSipPort_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSipPort.Text))
            {
                int sipPort = 0;
                bool isNumber = Int32.TryParse(textBoxSipPort.Text, out sipPort);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.SpPort = sipPort;
                }
            }
        }

        /// <summary>
        /// Is default.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxIsDefault_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxIsDefault.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.IsDefault = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.IsDefault = false;
                    break;
            }
        }

        /// <summary>
        /// Priority.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPriority_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxPriority.Text))
            {
                int priority = 0;
                bool isNumber = Int32.TryParse(textBoxPriority.Text, out priority);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.Priority = priority;
                }
            }
        }

        /// <summary>
        /// Drop Calls On Fail/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxDropCallsOnFail_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxDropCallsOnFail.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.DropCallsOnFail = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.DropCallsOnFail = false;
                    break;
            }
        }
    }
}
