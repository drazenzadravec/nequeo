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

namespace Nequeo.VoIP.PjSip.UI
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
        public Settings(Nequeo.VoIP.PjSip.VoIPCall voipCall)
        {
            InitializeComponent();
            _voipCall = voipCall;
        }

        private Data.Common _common = null;
        private Nequeo.VoIP.PjSip.VoIPCall _voipCall = null;

        private string _audioRecordingOutCallPath = null;
        private string _audioRecordingInCallPath = null;
        private string _contactsFilePath = null;
        private bool _hasCredentials = false;

        /// <summary>
        /// Gets or sets the data common reference.
        /// </summary>
        internal Data.Common DataCommon
        {
            get { return _common; }
            set { _common = value; }
        }

        /// <summary>
        /// Gets or sets has credentials.
        /// </summary>
        public bool HasCredentials
        {
            get { return _hasCredentials; }
        }

        /// <summary>
        /// Gets or sets the contacts file path.
        /// </summary>
        public string ContactsFilePath
        {
            get { return _contactsFilePath; }
            set { _contactsFilePath = value; }
        }

        /// <summary>
        /// Gets or sets the audio recording outgoing call path.
        /// </summary>
        public string AudioRecordingOutCallPath
        {
            get { return _audioRecordingOutCallPath; }
            set { _audioRecordingOutCallPath = value; }
        }

        /// <summary>
        /// Gets or sets the audio recording incoming call path.
        /// </summary>
        public string AudioRecordingInCallPath
        {
            get { return _audioRecordingInCallPath; }
            set { _audioRecordingInCallPath = value; }
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Load(object sender, EventArgs e)
        {
            // Load sounds
            textBoxSoundsRingPath.Text = _common.IncomingCallRingFilePath;
            textBoxSoundsIMPath.Text = _common.InstantMessageFilePath;
            textBoxSoundsAutoAnswer.Text = _common.AutoAnswerFilePath;
            checkBoxVideo.Checked = _common.EnableVideo;

            // Redirect call.
            checkBoxRedirectCallEnabled.Checked = _common.EnableRedirect;
            textBoxRedirectCallText.Text = _common.RedirectCallNumber;
            textBoxRedirectCallTime.Text = _common.RedirectCallAfter.ToString();

            // Auto answer.
            textBoxAutoAnswerWait.Text = _common.AutoAnswerWait.ToString();
            checkBoxAutoAnswer.Checked = _common.AutoAnswer;
            textBoxMessageBankWait.Text = _common.MessageBankWait.ToString();

            // Load settings.
            if (String.IsNullOrEmpty(_voipCall.VoIPManager.AccountConnection.AccountName))
                textBoxCredentialsAccountName.Text = _common.AccountName;
            else
                textBoxCredentialsAccountName.Text = _voipCall.VoIPManager.AccountConnection.AccountName;

            if (String.IsNullOrEmpty(_voipCall.VoIPManager.AccountConnection.SpHost))
                textBoxCredentialsSipHost.Text = _common.SipHost;
            else
                textBoxCredentialsSipHost.Text = _voipCall.VoIPManager.AccountConnection.SpHost;

            // Assign the credentials.
            if (_voipCall.VoIPManager.AccountConnection.AuthCredentials == null)
            {
                textBoxCredentialsUsername.Text = _common.SipUsername;
                textBoxCredentialsPassword.Text = _common.SipPassword;
            }
            else
            {
                Net.PjSip.AuthCredInfo[] authCredentials = _voipCall.VoIPManager.AccountConnection.AuthCredentials.AuthCredentials;
                textBoxCredentialsUsername.Text = authCredentials[0].Username;
                textBoxCredentialsPassword.Text = authCredentials[0].Data;
            }

            checkBoxAudioRecordOutCall.Checked = _common.OutgoingCallAudioRecordingEnabled;
            checkBoxAudioRecordInCall.Checked = _common.IncomingCallAudioRecordingEnabled;

            if (!String.IsNullOrEmpty(_audioRecordingOutCallPath))
                textBoxAudioRecordOutCall.Text = _audioRecordingOutCallPath;

            if (!String.IsNullOrEmpty(_audioRecordingInCallPath))
                textBoxAudioRecordInCall.Text = _audioRecordingInCallPath;

            if (!String.IsNullOrEmpty(_contactsFilePath))
                textBoxContactFilePath.Text = _contactsFilePath;

            // Get the audio devices.
            Nequeo.Net.PjSip.AudioDeviceInfo[] audioDevices = _voipCall.VoIPManager.MediaManager.GetAllAudioDevices();

            // For each audio device
            foreach (Nequeo.Net.PjSip.AudioDeviceInfo audioDevice in audioDevices)
            {
                // Is capture device.
                if(audioDevice.InputCount > 0)
                    comboBoxAudioCaptureDevice.Items.Add(audioDevice.Name + " | " + audioDevice.Driver);

                // Is playback device.
                if (audioDevice.OutputCount > 0)
                    comboBoxAudioPlaybackDevice.Items.Add(audioDevice.Name + " | " + audioDevice.Driver);
            }

            // Set any initial items.
            int captureIndex = _voipCall.VoIPManager.MediaManager.GetCaptureDevice();
            int playbackIndex = _voipCall.VoIPManager.MediaManager.GetPlaybackDevice();

            // Set the selected.
            if (captureIndex >= 0)
            {
                // Get capture device.
                Nequeo.Net.PjSip.AudioDeviceInfo captureDevice = audioDevices[captureIndex];
                int index = comboBoxAudioCaptureDevice.Items.IndexOf(captureDevice.Name + " | " + captureDevice.Driver);
                comboBoxAudioCaptureDevice.SelectedIndex = index;
            }
            else
            {
                // Get from configuration file.
                comboBoxAudioCaptureDevice.SelectedIndex = _common.CaptureAudioDeviceIndex;
            }

            if (playbackIndex >= 0)
            {
                // Get playback device.
                Nequeo.Net.PjSip.AudioDeviceInfo playbackDevice = audioDevices[playbackIndex];
                int index = comboBoxAudioPlaybackDevice.Items.IndexOf(playbackDevice.Name + " | " + playbackDevice.Driver);
                comboBoxAudioPlaybackDevice.SelectedIndex = index;
            }
            else
            {
                // Get from configuration file.
                comboBoxAudioPlaybackDevice.SelectedIndex = _common.PlaybackAudioDeviceIndex;
            }

            // Get the video devices.
            Nequeo.Net.PjSip.VideoDeviceInfo[] videoDevices = _voipCall.VoIPManager.MediaManager.GetAllVideoDevices();

            // For each video device
            foreach (Nequeo.Net.PjSip.VideoDeviceInfo videoDevice in videoDevices)
            {
                // Is capture device.
                if ((videoDevice.Direction == Net.PjSip.MediaDirection.PJMEDIA_DIR_CAPTURE) ||
                    (videoDevice.Direction == Net.PjSip.MediaDirection.PJMEDIA_DIR_ENCODING))
                    comboBoxVideoDeviceCapture.Items.Add(videoDevice.Name + " | " + videoDevice.Driver);

                // Is playback device.
                if ((videoDevice.Direction == Net.PjSip.MediaDirection.PJMEDIA_DIR_PLAYBACK) ||
                    (videoDevice.Direction == Net.PjSip.MediaDirection.PJMEDIA_DIR_DECODING))
                    comboBoxVideoDevicePlayback.Items.Add(videoDevice.Name + " | " + videoDevice.Driver);
            }

            // Set any initial items.
            int captureVideoIndex = _voipCall.VoIPManager.MediaManager.GetVideoCaptureDeviceID();
            int playbackVideoIndex = _voipCall.VoIPManager.MediaManager.GetVideoRenderDeviceID();

            // Set the selected.
            if (captureVideoIndex >= 0)
            {
                // Get capture device.
                Nequeo.Net.PjSip.VideoDeviceInfo captureVideo = videoDevices[captureVideoIndex];
                int index = comboBoxVideoDeviceCapture.Items.IndexOf(captureVideo.Name + " | " + captureVideo.Driver);
                comboBoxVideoDeviceCapture.SelectedIndex = index;
            }
            else
            {
                // Get from configuration file.
                comboBoxVideoDeviceCapture.SelectedIndex = _common.VideoCaptureIndex;
            }

            if (playbackVideoIndex >= 0)
            {
                // Get playback device.
                Nequeo.Net.PjSip.VideoDeviceInfo playbackVideo = videoDevices[playbackVideoIndex];
                int index = comboBoxVideoDevicePlayback.Items.IndexOf(playbackVideo.Name + " | " + playbackVideo.Driver);
                comboBoxVideoDevicePlayback.SelectedIndex = index;
            }
            else
            {
                // Get from configuration file.
                comboBoxVideoDevicePlayback.SelectedIndex = _common.VideoRenderIndex;
            }

            // Assign each property.
            textBoxSipPort.Text = _voipCall.VoIPManager.AccountConnection.SpPort.ToString();
            checkBoxIsDefault.Checked = _voipCall.VoIPManager.AccountConnection.IsDefault;
            textBoxPriority.Text = _voipCall.VoIPManager.AccountConnection.Priority.ToString();
            checkBoxDropCallsOnFail.Checked = _voipCall.VoIPManager.AccountConnection.DropCallsOnFail;
            checkBoxRegisterOnAdd.Checked = _voipCall.VoIPManager.AccountConnection.RegisterOnAdd;
            checkBoxNoIceRtcp.Checked = _voipCall.VoIPManager.AccountConnection.NoIceRtcp;
            checkBoxIceEnabled.Checked = _voipCall.VoIPManager.AccountConnection.IceEnabled;
            checkBoxPublishEnabled.Checked = _voipCall.VoIPManager.AccountConnection.PublishEnabled;
            checkBoxPublishQueue.Checked = _voipCall.VoIPManager.AccountConnection.PublishQueue;
            textBoxPublishShutdownWait.Text = _voipCall.VoIPManager.AccountConnection.PublishShutdownWaitMsec.ToString();
            checkBoxMessageWaitingIndicationEnabled.Checked = _voipCall.VoIPManager.AccountConnection.MessageWaitingIndication;
            textBoxMwiExpiration.Text = _voipCall.VoIPManager.AccountConnection.MWIExpirationSec.ToString();
            textBoxMediaTransportPort.Text = _voipCall.VoIPManager.AccountConnection.MediaTransportPort.ToString();
            textBoxMediaTransportPortRange.Text = _voipCall.VoIPManager.AccountConnection.MediaTransportPortRange.ToString();
            textBoxTimerMinSE.Text = _voipCall.VoIPManager.AccountConnection.TimerMinSESec.ToString();
            textBoxTimerSessExpires.Text = _voipCall.VoIPManager.AccountConnection.TimerSessExpiresSec.ToString();
            textBoxTimeRetryInterval.Text = _voipCall.VoIPManager.AccountConnection.RetryIntervalSec.ToString();
            textBoxTimeFirstRetryInterval.Text = _voipCall.VoIPManager.AccountConnection.FirstRetryIntervalSec.ToString();
            textBoxTimeDelayBeforeRefresh.Text = _voipCall.VoIPManager.AccountConnection.DelayBeforeRefreshSec.ToString();
            textBoxTimeTimeout.Text = _voipCall.VoIPManager.AccountConnection.TimeoutSec.ToString();
            textBoxTimeUnregisterWait.Text = _voipCall.VoIPManager.AccountConnection.UnregWaitSec.ToString();
            textBoxVideoBandwidthRate.Text = _voipCall.VoIPManager.AccountConnection.VideoRateControlBandwidth.ToString();

            // Select use ipv6
            switch (_voipCall.VoIPManager.AccountConnection.IPv6Use)
            {
                case Net.PjSip.IPv6_Use.IPV6_DISABLED:
                    checkBoxUseIPv6.Checked = false;
                    break;
                case Net.PjSip.IPv6_Use.IPV6_ENABLED:
                    checkBoxUseIPv6.Checked = true;
                    break;
            }

            // Use Srtp.
            switch(_voipCall.VoIPManager.AccountConnection.SRTPUse)
            {
                case Net.PjSip.SRTP_Use.SRTP_DISABLED:
                    checkBoxUseSrtp.CheckState = CheckState.Unchecked;
                    break;
                case Net.PjSip.SRTP_Use.SRTP_MANDATORY:
                    checkBoxUseSrtp.CheckState = CheckState.Checked;
                    break;
                case Net.PjSip.SRTP_Use.SRTP_OPTIONAL:
                    checkBoxUseSrtp.CheckState = CheckState.Indeterminate;
                    break;
            }

            // Srtp Secure Signaling
            switch (_voipCall.VoIPManager.AccountConnection.SRTPSecureSignaling)
            {
                case Net.PjSip.SRTP_SecureSignaling.SRTP_SECURESIGNALING_DISABLED:
                    checkBoxSrtpSecureSignaling.CheckState = CheckState.Unchecked;
                    break;
                case Net.PjSip.SRTP_SecureSignaling.SRTP_SECURESIGNALING_REQUIRES:
                    checkBoxSrtpSecureSignaling.CheckState = CheckState.Checked;
                    break;
                case Net.PjSip.SRTP_SecureSignaling.SRTP_SECURESIGNALING_REQUIRES_END_TO_END:
                    checkBoxSrtpSecureSignaling.CheckState = CheckState.Indeterminate;
                    break;
            }

            // List and get the audio device.
            int audioDeviceCount = Nequeo.IO.Audio.Devices.Count;
            if (audioDeviceCount > 0)
            {
                // Add the None device.
                comboBoxSoundsAudioDevice.Items.Add("None");

                // For each device.
                for (int i = 0; i < audioDeviceCount; i++)
                {
                    try
                    {
                        // Add the audio device.
                        Nequeo.IO.Audio.Device audioDevice = Nequeo.IO.Audio.Devices.GetDevice(i);
                        comboBoxSoundsAudioDevice.Items.Add(audioDevice.Details.ProductName);
                    }
                    catch { }
                }

                // Assign the selected device.
                if (_common.AudioDeviceIndex >= 0)
                    comboBoxSoundsAudioDevice.SelectedIndex = _common.AudioDeviceIndex + 1;
                else
                    comboBoxSoundsAudioDevice.SelectedIndex = -1;
            }

            // Load only once.
            if (_common.AudioCodecs == null)
            {
                // List all the audio codec.
                _common.AudioCodecs = _voipCall.VoIPManager.GetAudioCodecInfo();
            }

            // Order the codecs.
            var orederedAudioCodecs = _common.AudioCodecs.OrderByDescending(u => u.Priority);

            // For each codec.
            foreach (Nequeo.Net.PjSip.CodecInfo audioCodec in orederedAudioCodecs)
            {
                bool enabled = true;

                if (audioCodec.Priority == (byte)0)
                    enabled = false;

                // Add to list.
                checkedListBoxAudioCodec.Items.Add(audioCodec.CodecId, enabled);
            }

            // Load only once.
            if (_common.VideoCodecs == null)
            {
                // List all the audio codec.
                _common.VideoCodecs = _voipCall.VoIPManager.GetVideoCodecInfo();
            }

            // Order the codecs.
            var orederedVideoCodecs = _common.VideoCodecs.OrderByDescending(u => u.Priority);

            // For each codec.
            foreach (Nequeo.Net.PjSip.CodecInfo videoCodec in orederedVideoCodecs)
            {
                bool found = true;

                // For each audio codec
                foreach (Nequeo.Net.PjSip.CodecInfo audioCodec in orederedAudioCodecs)
                {
                    // Found match
                    if (videoCodec.CodecId.ToLower() == audioCodec.CodecId.ToLower())
                    {
                        found = false;
                        break;
                    }
                }

                // If video codec only.
                if (found)
                {
                    bool enabled = true;

                    if (videoCodec.Priority == (byte)0)
                        enabled = false;

                    // Add to list.
                    checkedListBoxVideoCodec.Items.Add(videoCodec.CodecId, enabled);
                }
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxRegisterOnAdd_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxRegisterOnAdd.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.RegisterOnAdd = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.RegisterOnAdd = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxNoIceRtcp_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxNoIceRtcp.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.NoIceRtcp = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.NoIceRtcp = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxIceEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxIceEnabled.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.IceEnabled = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.IceEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUseIPv6_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxUseIPv6.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.IPv6Use = Net.PjSip.IPv6_Use.IPV6_ENABLED;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.IPv6Use = Net.PjSip.IPv6_Use.IPV6_DISABLED;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUseSrtp_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxUseSrtp.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.SRTPUse = Net.PjSip.SRTP_Use.SRTP_MANDATORY;
                    break;
                case CheckState.Indeterminate:
                    _voipCall.VoIPManager.AccountConnection.SRTPUse = Net.PjSip.SRTP_Use.SRTP_OPTIONAL;
                    break;
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.SRTPUse = Net.PjSip.SRTP_Use.SRTP_DISABLED;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxSrtpSecureSignaling_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxSrtpSecureSignaling.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.SRTPSecureSignaling = Net.PjSip.SRTP_SecureSignaling.SRTP_SECURESIGNALING_REQUIRES;
                    break;
                case CheckState.Indeterminate:
                    _voipCall.VoIPManager.AccountConnection.SRTPSecureSignaling = Net.PjSip.SRTP_SecureSignaling.SRTP_SECURESIGNALING_REQUIRES_END_TO_END;
                    break;
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.SRTPSecureSignaling = Net.PjSip.SRTP_SecureSignaling.SRTP_SECURESIGNALING_DISABLED;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxPublishEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxPublishEnabled.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.PublishEnabled = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.PublishEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxPublishQueue_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxPublishQueue.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.PublishQueue = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.PublishQueue = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPublishShutdownWait_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxPublishShutdownWait.Text))
            {
                int shutdown = 0;
                bool isNumber = Int32.TryParse(textBoxPublishShutdownWait.Text, out shutdown);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.PublishShutdownWaitMsec = (uint)shutdown;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxMessageWaitingIndicationEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxMessageWaitingIndicationEnabled.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.VoIPManager.AccountConnection.MessageWaitingIndication = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.VoIPManager.AccountConnection.MessageWaitingIndication = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMwiExpiration_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxMwiExpiration.Text))
            {
                int mwiExpiration = 0;
                bool isNumber = Int32.TryParse(textBoxMwiExpiration.Text, out mwiExpiration);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.MWIExpirationSec = (uint)mwiExpiration;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMediaTransportPort_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxMediaTransportPort.Text))
            {
                int mediaTransport = 0;
                bool isNumber = Int32.TryParse(textBoxMediaTransportPort.Text, out mediaTransport);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.MediaTransportPort = (uint)mediaTransport;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMediaTransportPortRange_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxMediaTransportPortRange.Text))
            {
                int mediaTransport = 0;
                bool isNumber = Int32.TryParse(textBoxMediaTransportPortRange.Text, out mediaTransport);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.MediaTransportPortRange = (uint)mediaTransport;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTimeRetryInterval_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxTimeRetryInterval.Text))
            {
                int time = 0;
                bool isNumber = Int32.TryParse(textBoxTimeRetryInterval.Text, out time);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.RetryIntervalSec = (uint)time;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTimeFirstRetryInterval_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxTimeFirstRetryInterval.Text))
            {
                int time = 0;
                bool isNumber = Int32.TryParse(textBoxTimeFirstRetryInterval.Text, out time);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.FirstRetryIntervalSec = (uint)time;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTimeDelayBeforeRefresh_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxTimeDelayBeforeRefresh.Text))
            {
                int time = 0;
                bool isNumber = Int32.TryParse(textBoxTimeDelayBeforeRefresh.Text, out time);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.DelayBeforeRefreshSec = (uint)time;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTimeTimeout_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxTimeTimeout.Text))
            {
                int time = 0;
                bool isNumber = Int32.TryParse(textBoxTimeTimeout.Text, out time);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.TimeoutSec = (uint)time;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTimeUnregisterWait_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxTimeUnregisterWait.Text))
            {
                int time = 0;
                bool isNumber = Int32.TryParse(textBoxTimeUnregisterWait.Text, out time);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.UnregWaitSec = (uint)time;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTimerMinSE_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxTimerMinSE.Text))
            {
                int time = 0;
                bool isNumber = Int32.TryParse(textBoxTimerMinSE.Text, out time);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.TimerMinSESec = (uint)time;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTimerSessExpires_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxTimerSessExpires.Text))
            {
                int time = 0;
                bool isNumber = Int32.TryParse(textBoxTimerSessExpires.Text, out time);
                if (isNumber)
                {
                    // Assign the port.
                    _voipCall.VoIPManager.AccountConnection.TimerSessExpiresSec = (uint)time;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioRecordOutCall_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxAudioRecordOutCall.Text))
                _audioRecordingOutCallPath = textBoxAudioRecordOutCall.Text;
            else
                _audioRecordingOutCallPath = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAudioRecordinCall_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxAudioRecordInCall.Text))
                _audioRecordingInCallPath = textBoxAudioRecordInCall.Text;
            else
                _audioRecordingInCallPath = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAudioRecordOutCall_Click(object sender, EventArgs e)
        {
            // Set the audio filter.
            saveFileDialog.Filter = "Wave File (*.wav)|*.wav";

            // Get the file name selected.
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _audioRecordingOutCallPath = saveFileDialog.FileName;
                textBoxAudioRecordOutCall.Text = _audioRecordingOutCallPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAudioRecordInCall_Click(object sender, EventArgs e)
        {
            // Set the audio filter.
            saveFileDialog.Filter = "Wave File (*.wav)|*.wav";

            // Get the file name selected.
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _audioRecordingInCallPath = saveFileDialog.FileName;
                textBoxAudioRecordInCall.Text = _audioRecordingInCallPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxAudioRecordOutCall_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxAudioRecordOutCall.CheckState)
            {
                case CheckState.Checked:
                    _common.OutgoingCallAudioRecordingEnabled = true;
                    textBoxAudioRecordOutCall.Enabled = true;
                    buttonAudioRecordOutCall.Enabled = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _common.OutgoingCallAudioRecordingEnabled = false;
                    textBoxAudioRecordOutCall.Enabled = false;
                    buttonAudioRecordOutCall.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxAudioRecordInCall_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxAudioRecordInCall.CheckState)
            {
                case CheckState.Checked:
                    _common.IncomingCallAudioRecordingEnabled = true;
                    textBoxAudioRecordInCall.Enabled = true;
                    buttonAudioRecordInCall.Enabled = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _common.IncomingCallAudioRecordingEnabled = false;
                    textBoxAudioRecordInCall.Enabled = false;
                    buttonAudioRecordInCall.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Contact file path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxContactFilePath_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxContactFilePath.Text))
                _contactsFilePath = textBoxContactFilePath.Text;
            else
                _contactsFilePath = null;
        }

        /// <summary>
        /// Contact file path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContactFilePath_Click(object sender, EventArgs e)
        {
            // Set the import filter.
            openFileDialog.Filter = "Text File (*.txt)|*.txt";

            // Get the file name selected.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxContactFilePath.Text = openFileDialog.FileName;
                _contactsFilePath = textBoxContactFilePath.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCredentialsAccountName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCredentialsAccountName.Text == null)
                textBoxCredentialsAccountName.Text = string.Empty;

            if (!String.IsNullOrEmpty(textBoxCredentialsAccountName.Text) &&
                !String.IsNullOrEmpty(textBoxCredentialsSipHost.Text))
            {
                _hasCredentials = true;
            }
            else
                _hasCredentials = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCredentialsSipHost_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCredentialsSipHost.Text == null)
                textBoxCredentialsSipHost.Text = string.Empty;

            if (!String.IsNullOrEmpty(textBoxCredentialsAccountName.Text) &&
                !String.IsNullOrEmpty(textBoxCredentialsSipHost.Text))
            {
                _hasCredentials = true;
            }
            else
                _hasCredentials = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCredentialsUsername_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCredentialsUsername.Text == null)
                textBoxCredentialsUsername.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCredentialsPassword_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCredentialsPassword.Text == null)
                textBoxCredentialsPassword.Text = string.Empty;
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Assign credentials.
            _voipCall.VoIPManager.AccountConnection.AccountName = textBoxCredentialsAccountName.Text;
            _voipCall.VoIPManager.AccountConnection.SpHost = textBoxCredentialsSipHost.Text;

            // Create the credentials.
            Net.PjSip.AuthCredInfo[] AuthCredentials = new Net.PjSip.AuthCredInfo[]
                { new Net.PjSip.AuthCredInfo(textBoxCredentialsUsername.Text, textBoxCredentialsPassword.Text) };
            _voipCall.VoIPManager.AccountConnection.AuthCredentials =
                new Net.PjSip.AuthenticateCredentials() { AuthCredentials = AuthCredentials };
        }

        /// <summary>
        /// Ring.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSoundsRingPath_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSoundsRingPath.Text))
                _common.IncomingCallRingFilePath = textBoxSoundsRingPath.Text;
            else
                _common.IncomingCallRingFilePath = null;
        }

        /// <summary>
        /// In coming call ring.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSoundsRingPath_Click(object sender, EventArgs e)
        {
            // Set the import filter.
            openFileDialog.Filter = "Wave Files (*.wav)|*.wav|MP3 Files (*.mp3)|*.mp3";

            // Get the file name selected.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxSoundsRingPath.Text = openFileDialog.FileName;
                _common.IncomingCallRingFilePath = textBoxSoundsRingPath.Text;
            }
        }

        /// <summary>
        /// IM ring.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSoundsIMPath_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSoundsIMPath.Text))
                _common.InstantMessageFilePath = textBoxSoundsIMPath.Text;
            else
                _common.InstantMessageFilePath = null;
        }

        /// <summary>
        /// Instant message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSoundsIMPath_Click(object sender, EventArgs e)
        {
            // Set the import filter.
            openFileDialog.Filter = "Wave Files (*.wav)|*.wav|MP3 Files (*.mp3)|*.mp3";

            // Get the file name selected.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxSoundsIMPath.Text = openFileDialog.FileName;
                _common.InstantMessageFilePath = textBoxSoundsIMPath.Text;
            }
        }

        /// <summary>
        /// Audio device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxSoundsAudioDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If item is selected.
            if (comboBoxSoundsAudioDevice.SelectedIndex >= 1)
            {
                _common.AudioDeviceIndex = comboBoxSoundsAudioDevice.SelectedIndex - 1;
            }
            else
            {
                _common.AudioDeviceIndex = -1;
            }
        }

        /// <summary>
        /// Auto answer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSoundsAutoAnswer_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSoundsAutoAnswer.Text))
                _common.AutoAnswerFilePath = textBoxSoundsAutoAnswer.Text;
            else
                _common.AutoAnswerFilePath = null;
        }

        /// <summary>
        /// Auto answer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSoundsAutoAnswer_Click(object sender, EventArgs e)
        {
            // Set the import filter.
            openFileDialog.Filter = "Wave Files (*.wav)|*.wav";

            // Get the file name selected.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxSoundsAutoAnswer.Text = openFileDialog.FileName;
                _common.AutoAnswerFilePath = textBoxSoundsAutoAnswer.Text;
            }
        }

        /// <summary>
        /// Auto answer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxAutoAnswer_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxAutoAnswer.CheckState)
            {
                case CheckState.Checked:
                    _common.AutoAnswer = true;
                    textBoxAutoAnswerWait.Enabled = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _common.AutoAnswer = false;
                    textBoxAutoAnswerWait.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Auto answer wait.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAutoAnswerWait_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxAutoAnswerWait.Text))
            {
                int wait = 0;
                bool isNumber = Int32.TryParse(textBoxAutoAnswerWait.Text, out wait);
                if (isNumber)
                {
                    // Assign the port.
                    _common.AutoAnswerWait = wait;
                }
            }
        }

        /// <summary>
        /// Messsage bank.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMessageBankWait_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxMessageBankWait.Text))
            {
                int wait = 0;
                bool isNumber = Int32.TryParse(textBoxMessageBankWait.Text, out wait);
                if (isNumber)
                {
                    // Assign the port.
                    _common.MessageBankWait = wait;
                }
            }
        }

        /// <summary>
        /// Enable video.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxVideo_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxVideo.CheckState)
            {
                case CheckState.Checked:
                    _common.EnableVideo = true;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _common.EnableVideo = false;
                    break;
            }
        }

        /// <summary>
        /// Video capture device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVideoDeviceCapture_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If item is selected.
            if (comboBoxVideoDeviceCapture.SelectedIndex >= 0)
            {
                // Get the device.
                string[] device = ((string)comboBoxVideoDeviceCapture.SelectedItem).Split(new char[] { '|' });
                string name = device[0].Trim();
                string driver = device[1].Trim();

                // Get the capture index.
                int captureIndex = _voipCall.VoIPManager.MediaManager.GetVideoDeviceID(driver, name);

                // Set the capture device.
                _voipCall.VoIPManager.MediaManager.SetVideoCaptureDeviceID(captureIndex);
            }
        }

        /// <summary>
        /// Video playback device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVideoDevicePlayback_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If item is selected.
            if (comboBoxVideoDevicePlayback.SelectedIndex >= 0)
            {
                // Get the device.
                string[] device = ((string)comboBoxVideoDevicePlayback.SelectedItem).Split(new char[] { '|' });
                string name = device[0].Trim();
                string driver = device[1].Trim();

                // Get the render index.
                int renderIndex = _voipCall.VoIPManager.MediaManager.GetVideoDeviceID(driver, name);

                // Set the render device.
                _voipCall.VoIPManager.MediaManager.SetVideoRenderDeviceID(renderIndex);
            }
        }

        /// <summary>
        /// Redirect call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxRedirectCallEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxRedirectCallEnabled.CheckState)
            {
                case CheckState.Checked:
                    _common.EnableRedirect = true;
                    textBoxRedirectCallText.ReadOnly = false;
                    textBoxRedirectCallTime.ReadOnly = false;
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _common.EnableRedirect = false;
                    textBoxRedirectCallText.ReadOnly = true;
                    textBoxRedirectCallTime.ReadOnly = true;
                    break;
            }
        }

        /// <summary>
        /// Redirect call number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxRedirectCallText_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxRedirectCallText.Text))
                _common.RedirectCallNumber = "";
            else
                _common.RedirectCallNumber = textBoxRedirectCallText.Text;
        }

        /// <summary>
        /// Redirect call time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxRedirectCallTime_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxRedirectCallTime.Text))
                _common.RedirectCallAfter = -1;
            else
            {
                int wait = 0;
                bool isNumber = Int32.TryParse(textBoxRedirectCallTime.Text, out wait);
                if (isNumber)
                {
                    // Assign the port.
                    _common.RedirectCallAfter = wait;
                }
            }
        }

        /// <summary>
        /// Video bandwidth rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVideoBandwidthRate_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxVideoBandwidthRate.Text))
            {
                int rate = 0;
                bool isNumber = Int32.TryParse(textBoxVideoBandwidthRate.Text, out rate);
                if (isNumber)
                {
                    // Assign the rate.
                    _voipCall.VoIPManager.AccountConnection.VideoRateControlBandwidth = (uint)rate;
                }
            }
        }

        /// <summary>
        /// Audio codec.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxAudioCodec_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (checkedListBoxAudioCodec.SelectedIndex >= 0)
            {
                // Get the selected codec.
                string audioCodec = (string)checkedListBoxAudioCodec.SelectedItem;
                Nequeo.Net.PjSip.CodecInfo codecInfo = _common.AudioCodecs.First(u => u.CodecId.ToLower() == audioCodec.ToLower());

                // If item is unchecked.
                if (e.NewValue == CheckState.Unchecked)
                {
                    // Set priorty to zero disable.
                    _voipCall.VoIPManager.SetPriorityAudioCodec(audioCodec, 0);
                }
                else
                {
                    // Set priorty to original.
                    _voipCall.VoIPManager.SetPriorityAudioCodec(audioCodec, codecInfo.Priority);
                }
            }
        }

        /// <summary>
        /// Video codec.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxVideoCodec_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (checkedListBoxVideoCodec.SelectedIndex >= 0)
            {
                // Get the selected codec.
                string videoCodec = (string)checkedListBoxVideoCodec.SelectedItem;
                Nequeo.Net.PjSip.CodecInfo codecInfo = _common.VideoCodecs.First(u => u.CodecId.ToLower() == videoCodec.ToLower());

                // If item is unchecked.
                if (e.NewValue == CheckState.Unchecked)
                {
                    // Set priorty to zero disable.
                    _voipCall.VoIPManager.SetPriorityVideoCodec(videoCodec, 0);
                }
                else
                {
                    // Set priorty to original.
                    _voipCall.VoIPManager.SetPriorityVideoCodec(videoCodec, codecInfo.Priority);
                }
            }
        }
    }
}
