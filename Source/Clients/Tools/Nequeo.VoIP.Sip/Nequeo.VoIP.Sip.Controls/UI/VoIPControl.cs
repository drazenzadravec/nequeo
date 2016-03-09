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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Nequeo.Extension;
using Nequeo.Serialisation;
using Nequeo.IO.Audio;

namespace Nequeo.VoIP.Sip.UI
{
    /// <summary>
    /// VoIP control.
    /// </summary>
    public partial class VoIPControl : UserControl
    {
        /// <summary>
        /// VoIP control.
        /// </summary>
        public VoIPControl()
        {
            InitializeComponent();
        }

        private bool _registered = false;
        private bool _disposed = false;
        private string _uri = null;

        private UI.InstantMessage _instantMessage = null;
        private Nequeo.VoIP.Sip.VoIPCall _voipCall = null;
        private Param.CallParam _call = null;
        private Data.contacts _contacts = null;
        private Data.configuration _configuration = null;

        private Nequeo.IO.Audio.Volume _volume = null;
        private Data.Common _common = null;
        private Data.IncomingOutgoingCalls _inOutCalls = null;

        private bool _audioRecordingOutCall = false;
        private bool _audioRecordingInCall = false;
        private string _audioRecordingOutCallPath = null;
        private string _audioRecordingInCallPath = null;
        private string _contactsFilePath = null;
        private bool _hasCredentials = false;
        private bool _created = false;

        private bool _foundContactName = false;
        private string _contactName = "";

        /// <summary>
        /// Gets or sets the contacts file path.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Sets the contacts file path.")]
        [NotifyParentProperty(true)]
        public string ContactsFilePath
        {
            get { return _contactsFilePath; }
            set { _contactsFilePath = value; }
        }

        /// <summary>
        /// Gets or sets the audio recording outgoing call indicator.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Sets the audio recording outgoing call indicator.")]
        [NotifyParentProperty(true)]
        public bool AudioRecordingOutCall
        {
            get { return _audioRecordingOutCall; }
            set { _audioRecordingOutCall = value; }
        }

        /// <summary>
        /// Gets or sets the audio recording incoming call indicator.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Sets the audio recording incoming call indicator.")]
        [NotifyParentProperty(true)]
        public bool AudioRecordingInCall
        {
            get { return _audioRecordingInCall; }
            set { _audioRecordingInCall = value; }
        }

        /// <summary>
        /// Gets or sets the audio recording outgoing call path.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Sets the audio recording outgoing call path.")]
        [NotifyParentProperty(true)]
        public string AudioRecordingOutCallPath
        {
            get { return _audioRecordingOutCallPath; }
            set { _audioRecordingOutCallPath = value; }
        }

        /// <summary>
        /// Gets or sets the audio recording incoming call path.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Sets the audio recording incoming call path.")]
        [NotifyParentProperty(true)]
        public string AudioRecordingInCallPath
        {
            get { return _audioRecordingInCallPath; }
            set { _audioRecordingInCallPath = value; }
        }

        /// <summary>
        /// Dispose of the unmanaged resources.
        /// </summary>
        public void DisposeCall()
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                try
                {
                    // Save the contacts list.
                    if (!String.IsNullOrEmpty(_contactsFilePath))
                    {
                        // Load the contacts.
                        if (_contacts != null && _contacts.contact != null)
                        {
                            // Deserialise the xml file into.
                            GeneralSerialisation serial = new GeneralSerialisation();
                            bool authData = serial.Serialise(_contacts, typeof(Data.contacts), _contactsFilePath);
                        }
                    }
                }
                catch { }

                try
                {
                    // The call.
                    if (_call != null)
                        _call.Dispose();
                }
                catch { }

                try
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    if (_voipCall != null)
                        _voipCall.Dispose();
                }
                catch { }

                try
                {
                    if (_instantMessage != null)
                        _instantMessage.Dispose();
                }
                catch { }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _voipCall = null;
                _instantMessage = null;
                _call = null;
            }
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _common = new Data.Common();
            _inOutCalls = new Data.IncomingOutgoingCalls();
            _volume = new Volume();

            // Create the voip call.
            _voipCall = new VoIPCall();
            _voipCall.OnIncomingCall += Voipcall_OnIncomingCall;
            _voipCall.OnInstantMessage += Voipcall_OnInstantMessage;
            _voipCall.OnRegStarted += Voipcall_OnRegStarted;
            _voipCall.OnRegState += Voipcall_OnRegState;
            _voipCall.VoIPManager.OnContactState += VoIPManager_OnContactState;
        }

        /// <summary>
        /// On contact state changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VoIPManager_OnContactState(object sender, Net.Sip.ContactInfo e)
        {
            UISync.Execute(() =>
            {
                try
                {
                    // Get the contact.
                    ListViewItem item = listViewContact.Items[e.Uri];

                    // If found.
                    if (item != null)
                    {
                        // Set the state.
                        item.SubItems[2].Text = e.PresenceStatus.Status.ToString().ToLower();
                    }
                }
                catch { }
            });
        }

        /// <summary>
        /// Create.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            _created = true;

            // Create.
            _voipCall.Create();

            // Enable.
            buttonCreate.Enabled = false;
            buttonLoadContacts.Enabled = true;
            buttonInstantMessage.Enabled = true;
            buttonRegister.Enabled = true;
            groupBoxCall.Enabled = true;
            groupBoxAccOnlineState.Enabled = true;
            groupBoxAccDetails.Enabled = true;
            groupBoxInOutCalls.Enabled = true;
            groupBoxConference.Enabled = true;

            try
            {
                // Get the volume.
                float[] microphoneVolume = Nequeo.IO.Audio.Volume.GetMicrophoneVolume();
                if (microphoneVolume != null && microphoneVolume.Length > 0)
                {
                    // Get first.
                    trackBarMicrophone.Value = (int)(microphoneVolume[0] * 100.0);
                    labelMicrophoneLevel.Text = trackBarMicrophone.Value.ToString();
                }

                bool[] microphoneMute = Nequeo.IO.Audio.Volume.GetMicrophoneMute();
                if (microphoneMute != null && microphoneMute.Length > 0)
                {
                    // Get first.
                    checkBoxMuteMicrophone.Checked = microphoneMute[0];
                }
            }
            catch { }

            try
            {
                // Get the volume.
                float[] speakerVolume = Nequeo.IO.Audio.Volume.GetSpeakerVolume();
                if (speakerVolume != null && speakerVolume.Length > 0)
                {
                    // Get first.
                    trackBarVolume.Value = (int)(speakerVolume[0] * 100.0);
                    labelVolumeLevel.Text = trackBarVolume.Value.ToString();
                }

                bool[] speakerMute = Nequeo.IO.Audio.Volume.GetSpeakerMute();
                if (speakerMute != null && speakerMute.Length > 0)
                {
                    // Get first.
                    checkBoxMuteVolume.Checked = speakerMute[0];
                }
            }
            catch { }

            try
            {
                _volume.OnMicrophoneNotification += _volume_OnMicrophoneNotification;
                _volume.OnSpeakerNotification += _volume_OnSpeakerNotification;
                _volume.SetMicrophoneNotification();
                _volume.SetSpeakerNotification();
            }
            catch { }
        }

        /// <summary>
        /// Speaker notify.
        /// </summary>
        /// <param name="data"></param>
        private void _volume_OnSpeakerNotification(IO.Audio.Api.AudioVolumeNotificationData data)
        {
            try
            {
                checkBoxMuteVolume.Checked = data.Muted;

                // Get first.
                trackBarVolume.Value = (int)(data.MasterVolume * 100.0);
                labelVolumeLevel.Text = trackBarVolume.Value.ToString();
            }
            catch { }
        }

        /// <summary>
        /// Microphone notify.
        /// </summary>
        /// <param name="data"></param>
        private void _volume_OnMicrophoneNotification(IO.Audio.Api.AudioVolumeNotificationData data)
        {
            try
            {
                checkBoxMuteMicrophone.Checked = data.Muted;

                // Get first.
                trackBarMicrophone.Value = (int)(data.MasterVolume * 100.0);
                labelMicrophoneLevel.Text = trackBarMicrophone.Value.ToString();
            }
            catch { }
        }

        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Register.
                _voipCall.Registration(!_registered);
            }
            catch (Exception)
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Unable to register because of an internal error.",
                    "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Not registered.
                buttonRegister.Text = "Register";
                _registered = false;
            }
        }

        /// <summary>
        /// On registration state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnRegState(object sender, Nequeo.VoIP.Sip.Param.OnRegStateParam e)
        {
            UISync.Execute(() =>
            {
                labelRegistationStatusState.Text = e.Reason + ". " + e.Info;

                // Check the registration state.
                if (e.Code != Net.Sip.StatusCode.SC_OK)
                {
                    // Enable.
                    buttonRegister.Text = "Register";
                    _registered = false;
                }
            });
        }

        /// <summary>
        /// On registration started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnRegStarted(object sender, Nequeo.VoIP.Sip.Param.OnRegStartedParam e)
        {
            if (e.Renew)
            {
                labelRegistationStatus.Text = "Registation Renewed : ";
                buttonRegister.Text = "Un-Register";
                _registered = true;
            }
            else
            {
                // No Registration.
                labelRegistationStatus.Text = "Registation None : ";
                labelRegistationStatusState.Text = "Not Registered";
                buttonRegister.Text = "Register";
                _registered = false;
            }
        }

        /// <summary>
        /// On instant message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnInstantMessage(object sender, Nequeo.VoIP.Sip.Param.OnInstantMessageParam e)
        {
            UISync.Execute(() =>
            {
                // Send the message.
                if (_instantMessage != null)
                    _instantMessage.Message(e);

            });
        }

        /// <summary>
        /// On incoming call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Voipcall_OnIncomingCall(object sender, Nequeo.VoIP.Sip.Param.OnIncomingCallParam e)
        {
            UISync.Execute(() =>
            {
                string contactName = null;
                bool found = false;

                // Find the contact.
                if (_contacts != null)
                {
                    try
                    {
                        // Get the contact number.
                        string[] splitFrom = e.From.Split(new char[] { '@' });
                        string[] splitFromSpace = splitFrom[0].Split(new char[] { ' ' });

                        // For each contact.
                        foreach (Data.contactsContact contact in _contacts.contact)
                        {
                            // Cleanup the sip.
                            string sipAccount = contact.sipAccount.Replace("sip:", "").Replace("sips:", "").
                                Replace("\"", "").Replace("<", "").Replace(">", "");

                            // If the sip matches.
                            if (sipAccount.ToLower().Trim() == e.From.ToLower().Trim())
                            {
                                // Found.
                                contactName = contact.name;
                                break;
                            }

                            // If the sip matches.
                            if (sipAccount.ToLower().Trim() == e.FromContact.ToLower().Trim())
                            {
                                // Found.
                                contactName = contact.name;
                                break;
                            }

                            // For each number.
                            foreach (string number in contact.numbers)
                            {
                                // Get the number.
                                string[] numb = number.Split(new char[] { '|' });

                                // If just a number exists.
                                if (splitFromSpace != null && splitFromSpace.Length > 0)
                                {
                                    // Try next space.
                                    if (splitFromSpace.Length > 3)
                                    {
                                        if (numb[1].ToLower().Trim() == splitFromSpace[3].ToLower().Trim().Replace("\"", ""))
                                        {
                                            // Found.
                                            found = true;
                                            contactName = contact.name;
                                            break;
                                        }
                                    }

                                    // Try next space.
                                    if (splitFromSpace.Length > 2)
                                    {
                                        if (numb[1].ToLower().Trim() == splitFromSpace[2].ToLower().Trim().Replace("\"", ""))
                                        {
                                            // Found.
                                            found = true;
                                            contactName = contact.name;
                                            break;
                                        }
                                    }

                                    // Try next space.
                                    if (splitFromSpace.Length > 1)
                                    {
                                        if (numb[1].ToLower().Trim() == splitFromSpace[1].ToLower().Trim().Replace("\"", ""))
                                        {
                                            // Found.
                                            found = true;
                                            contactName = contact.name;
                                            break;
                                        }
                                    }

                                    // Try to match the number.
                                    if (numb[1].ToLower().Trim() == splitFromSpace[0].ToLower().Trim().Replace("\"", ""))
                                    {
                                        // Found.
                                        found = true;
                                        contactName = contact.name;
                                        break;
                                    }
                                }

                                // If just a number exists.
                                if (numb[1].ToLower().Trim() == splitFrom[0].ToLower().Trim())
                                {
                                    // Found.
                                    found = true;
                                    contactName = contact.name;
                                    break;
                                }
                            }

                            // If found then break;
                            if (found)
                                break;
                        }
                    }
                    catch { }
                }

                // If no contact name.
                if (String.IsNullOrEmpty(contactName))
                {
                    try
                    {
                        // Get the contact number.
                        string[] splitFrom = e.From.Split(new char[] { '@' });
                        contactName = splitFrom[0].Replace("sip:", "").Replace("sips:", "").
                            Replace("\"", "").Replace("<", "").Replace(">", "");
                    }
                    catch (Exception)
                    {
                        // Caller can not be found.
                        contactName = "Unknown";
                    }
                }

                // Open the call.
                string ringFilePath = (_common != null ? _common.IncomingCallRingFilePath : null);
                int audioDeviceIndex = (_common != null ? _common.AudioDeviceIndex : -1);
                bool autoAnswer = (_common != null ? _common.AutoAnswer : false);
                string autoAnswerFile = (_common != null ? _common.AutoAnswerFilePath : null);
                int autoAnswerWait = (_common != null ? _common.AutoAnswerWait : 30);
                int messageBank = (_common != null ? _common.MessageBankWait : 20);
                bool redirectEnable = (_common != null ? _common.EnableRedirect : false);
                string redirectCallNumber = (_common != null ? _common.RedirectCallNumber : null);
                int redirectCallAfter = (_common != null ? _common.RedirectCallAfter : -1);

                // Attach to the disconnect event.
                e.Call.OnCallDisconnected += Call_OnCallDisconnected;

                // Open the window.
                Nequeo.VoIP.Sip.UI.InComingCall incomingCall = new InComingCall(_voipCall, e,
                    listViewContact, listViewInOutCalls, listViewConference, _contacts, imageListSmall, contactName, 
                    ringFilePath, audioDeviceIndex, autoAnswer, autoAnswerFile, autoAnswerWait, 
                    _audioRecordingInCallPath, messageBank, redirectEnable, redirectCallNumber,
                    redirectCallAfter);

                // Show the form.
                incomingCall.IncomingOutgoingCalls = _inOutCalls;
                incomingCall.Show(this);
            });
        }

        /// <summary>
        /// Incoming call disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The call id.</param>
        private void Call_OnCallDisconnected(object sender, Param.CallInfoParam e)
        {
            UISync.Execute(() =>
            {
                try
                {
                    // Add the call information.
                    Param.CallInfoParam info = new Param.CallInfoParam();
                    info.IncomingOutgoing = true;
                    info.ContactName = e.ContactName;
                    info.FromTo = (!String.IsNullOrEmpty(e.FromTo) ? e.FromTo : "");
                    info.Contact = (!String.IsNullOrEmpty(e.Contact) ? e.Contact : "");
                    info.CallID = e.CallID;
                    info.ConnectDuration = e.ConnectDuration;
                    info.Date = e.Date;
                    info.Guid = e.Guid;
                    info.TotalDuration = e.TotalDuration;
                    _inOutCalls.Add(info);

                    int imageIndex = 0;
                    Data.contactsContact contact = null;

                    try
                    {
                        // Get the contact.
                        contact = _contacts.contact.First(u => u.name.ToLower() == info.ContactName.ToLower());
                        if (contact != null)
                        {
                            // Find in the contact list view.
                            ListViewItem listViewItem = listViewContact.Items[contact.sipAccount];
                            imageIndex = listViewItem.ImageIndex;
                        }
                    }
                    catch { imageIndex = 0; }

                    // Add to the in out view.
                    // Create a new list item.
                    ListViewItem item = new ListViewItem(info.ContactName, imageIndex);
                    item.Name = info.FromTo + "|" + info.Guid;
                    item.SubItems.Add(info.Date.ToShortDateString() + " " + info.Date.ToShortTimeString());
                    item.SubItems.Add((info.IncomingOutgoing ? "Incoming" : "Outgoing"));
                    item.SubItems.Add(info.FromTo);
                    item.SubItems.Add(info.TotalDuration.ToString());
                    item.SubItems.Add(info.ConnectDuration.ToString());

                    // Add the item.
                    listViewInOutCalls.Items.Add(item);
                }
                catch { }

                Param.CallParam call = null;
                try
                {
                    // Get the reference of the call
                    call = _voipCall.ConferenceCall.First(u => u.ID == e.Guid);
                }
                catch { call = null; }

                // Found the call.
                if (call != null)
                {
                    try
                    {
                        // Remove from the conference list.
                        _voipCall.RemoveConferenceCallContact(e.Guid);
                    }
                    catch { }

                    try
                    {
                        // Remove from the list view.
                        string confKey = e.CallID + "|" + e.Guid;
                        listViewConference.Items.RemoveByKey(confKey);
                    }
                    catch { }

                    try
                    {
                        // Clean up the current call.
                        call.Dispose();
                        call = null;
                    }
                    catch { }
                }
            });
        }

        /// <summary>
        /// Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSettings_Click(object sender, EventArgs e)
        {
            // Open settings.
            UI.Settings settings = new Settings(_voipCall);
            settings.AudioRecordingInCallPath = _audioRecordingInCallPath;
            settings.AudioRecordingOutCallPath = _audioRecordingOutCallPath;
            settings.ContactsFilePath = _contactsFilePath;
            settings.DataCommon = _common;
            settings.ShowDialog(this);

            if (!String.IsNullOrEmpty(settings.ContactsFilePath))
                _contactsFilePath = settings.ContactsFilePath;
            else
                _contactsFilePath = null;

            // Get recording setting.
            _audioRecordingInCall = _common.IncomingCallAudioRecordingEnabled;
            _audioRecordingOutCall = _common.OutgoingCallAudioRecordingEnabled;

            // Audio incoming call.
            if (_audioRecordingInCall && !String.IsNullOrEmpty(settings.AudioRecordingInCallPath))
            {
                _audioRecordingInCallPath = settings.AudioRecordingInCallPath;

                // Create the call path.
                string audioRecordingPath = null;
                if (!String.IsNullOrEmpty(_audioRecordingInCallPath))
                {
                    // Create the recording file name and path.
                    DateTime time = DateTime.Now;
                    string audioRecodingExt = System.IO.Path.GetExtension(_audioRecordingInCallPath);
                    string audioRecordingDir = System.IO.Path.GetDirectoryName(_audioRecordingInCallPath).TrimEnd(new char[] { '\\' }) + "\\";
                    string audioRecordingFile = System.IO.Path.GetFileNameWithoutExtension(_audioRecordingInCallPath) + "_" +
                        time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString() + "_" +
                        time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();

                    // Create the file name.
                    audioRecordingPath = audioRecordingDir + audioRecordingFile + audioRecodingExt;
                }

                // Assign the file.
                _voipCall.IncomingCallRecordFilename(audioRecordingPath);
            }
            else
            {
                if (!_audioRecordingInCall)
                    _voipCall.IncomingCallRecordFilename(null);
            }

            // Audio outgoing call.
            if (!String.IsNullOrEmpty(settings.AudioRecordingOutCallPath))
                _audioRecordingOutCallPath = settings.AudioRecordingOutCallPath;

            // If not created yet.
            if (!_created)
            {
                // Has credentials.
                _hasCredentials = settings.HasCredentials;
                if (_hasCredentials)
                    buttonCreate.Enabled = true;
                else
                    buttonCreate.Enabled = false;
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VoIPControl_Load(object sender, EventArgs e)
        {
            UISync.Init(this);
            base.Disposed += VoIPControl_Disposed;

            if (!String.IsNullOrEmpty(_contactsFilePath))
                buttonLoadContacts.Enabled = true;
            else
                buttonLoadContacts.Enabled = false;

            // Load the views;
            comboBoxContactView.Items.Add(View.Details.ToString());
            comboBoxContactView.Items.Add(View.LargeIcon.ToString());
            comboBoxContactView.Items.Add(View.List.ToString());
            comboBoxContactView.Items.Add(View.SmallIcon.ToString());
            comboBoxContactView.Items.Add(View.Tile.ToString());
        }

        /// <summary>
        /// Disposed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VoIPControl_Disposed(object sender, EventArgs e)
        {
            // Dispose of the unmanaged resources.
            DisposeCall();
        }

        /// <summary>
        /// Update the time elasped.
        /// </summary>
        private class UISync
        {
            private static ISynchronizeInvoke Sync;

            /// <summary>
            /// Initialisation
            /// </summary>
            /// <param name="sync">The initialisation sync.</param>
            public static void Init(ISynchronizeInvoke sync)
            {
                Sync = sync;
            }

            /// <summary>
            /// Execute the action.
            /// </summary>
            /// <param name="action">The action to perfoem.</param>
            public static void Execute(Action action)
            {
                Sync.BeginInvoke(action, null);
            }
        }

        /// <summary>
        /// Call number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxCallNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If not in a call.
            if (_call == null)
            {
                // If numbers exist.
                if (!String.IsNullOrEmpty(comboBoxCallNumber.Text))
                {
                    // Add the number.
                    AddCallList();
                    SetSipUri();
                    buttonCall.Enabled = true;
                }
                else
                    buttonCall.Enabled = false;
            }
        }

        /// <summary>
        /// Call number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxCallNumber_TextChanged(object sender, EventArgs e)
        {
            // If not in a call.
            if (_call == null)
            {
                // If numbers exist.
                if (!String.IsNullOrEmpty(comboBoxCallNumber.Text))
                {
                    SetSipUri();
                    buttonCall.Enabled = true;
                }
                else
                    buttonCall.Enabled = false;
            }
        }

        /// <summary>
        /// Create the sip uri.
        /// </summary>
        private void SetSipUri()
        {
            string uri = comboBoxCallNumber.Text;
            if (!_foundContactName)
                _contactName = uri;

            // If not sip uri.
            if (!uri.ToLower().Contains("sip"))
            {
                // If not sip uri.
                if (!uri.ToLower().Contains("@"))
                {
                    // Construct the uri
                    _uri = "sip:" + uri +
                        (String.IsNullOrEmpty(_voipCall.VoIPManager.AccountConnection.SpHost) ? "" : "@" + _voipCall.VoIPManager.AccountConnection.SpHost);
                }
                else
                {
                    // Construct the uri
                    _uri = "sip:" + uri;
                }
            }
            else
            {
                // If sip uri.
                if (!uri.ToLower().Contains("@"))
                {
                    // Construct the uri
                    _uri = uri +
                        (String.IsNullOrEmpty(_voipCall.VoIPManager.AccountConnection.SpHost) ? "" : "@" + _voipCall.VoIPManager.AccountConnection.SpHost);
                }
                else
                {
                    // Construct the uri
                    _uri = uri;
                }
            }
        }

        /// <summary>
        /// Call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCall_Click(object sender, EventArgs e)
        {
            // Make the call.
            CallContact();
        }

        /// <summary>
        /// Make the call.
        /// </summary>
        private void CallContact()
        {
            try
            {
                // Create the call path.
                string audioRecordingPath = null;
                if (_audioRecordingOutCall && !String.IsNullOrEmpty(_audioRecordingOutCallPath))
                {
                    DateTime time = DateTime.Now;
                    string audioRecodingExt = System.IO.Path.GetExtension(_audioRecordingOutCallPath);
                    string audioRecordingDir = System.IO.Path.GetDirectoryName(_audioRecordingOutCallPath).TrimEnd(new char[] { '\\' }) + "\\";
                    string audioRecordingFile = System.IO.Path.GetFileNameWithoutExtension(_audioRecordingOutCallPath) + "_" +
                        time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString() + "_" +
                        time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();

                    // Create the file name.
                    audioRecordingPath = audioRecordingDir + audioRecordingFile + audioRecodingExt;
                }

                // Make the call.
                _call = _voipCall.MakeCall(0, _uri, audioRecordingPath);
                _call.OnCallState += _call_OnCallState;
                _call.OnCallMediaState += _call_OnCallMediaState;
                _call.OnCallDisconnected += _call_OnCallDisconnected;

                // If call.
                if (_call != null)
                {
                    AddCallList();
                    buttonCall.Enabled = false;
                    buttonHold.Enabled = true;
                    buttonHangup.Enabled = true;
                    checkBoxSuspend.Enabled = true;
                    groupBoxDigits.Enabled = true;
                    comboBoxCallNumber.Enabled = false;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Unable to make the call because of an internal error." + ex.Message,
                    "Make Call?", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Enable.
                buttonCall.Enabled = true;
                buttonHold.Enabled = false;
                buttonHangup.Enabled = false;
                groupBoxDigits.Enabled = false;
                comboBoxCallNumber.Enabled = true;
                checkBoxSuspend.Enabled = false;
                _call = null;
            }
        }

        /// <summary>
        /// On outgoing call disconnect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _call_OnCallDisconnected(object sender, Param.CallInfoParam e)
        {
            UISync.Execute(() =>
            {
                try
                {
                    // Add the call information.
                    Param.CallInfoParam info = new Param.CallInfoParam();
                    info.IncomingOutgoing = false;
                    info.ContactName = _contactName;
                    info.FromTo = (!String.IsNullOrEmpty(e.FromTo) ? e.FromTo : "");
                    info.Contact = (!String.IsNullOrEmpty(e.Contact) ? e.Contact : "");
                    info.CallID = e.CallID;
                    info.ConnectDuration = e.ConnectDuration;
                    info.Date = e.Date;
                    info.Guid = e.Guid;
                    info.TotalDuration = e.TotalDuration;
                    _inOutCalls.Add(info);

                    int imageIndex = 0;
                    Data.contactsContact contact = null;

                    try
                    {
                        // Get the contact.
                        contact = _contacts.contact.First(u => u.name.ToLower() == info.ContactName.ToLower());
                        if (contact != null)
                        {
                            // Find in the contact list view.
                            ListViewItem listViewItem = listViewContact.Items[contact.sipAccount];
                            imageIndex = listViewItem.ImageIndex;
                        }
                    }
                    catch { imageIndex = 0; }

                    // Add to the in out view.
                    // Create a new list item.
                    ListViewItem item = new ListViewItem(info.ContactName, imageIndex);
                    item.Name = info.FromTo + "|" + info.Guid;
                    item.SubItems.Add(info.Date.ToShortDateString() + " " + info.Date.ToShortTimeString());
                    item.SubItems.Add((info.IncomingOutgoing ? "Incoming" : "Outgoing"));
                    item.SubItems.Add(info.FromTo);
                    item.SubItems.Add(info.TotalDuration.ToString());
                    item.SubItems.Add(info.ConnectDuration.ToString());

                    // Add the item.
                    listViewInOutCalls.Items.Add(item);
                }
                catch { }

                // The call has ended.
                DialogResult result = MessageBox.Show(this, "The call has ended.",
                "Make Call", MessageBoxButtons.OK, MessageBoxIcon.Information);

                EnableHangupCall();

            });
        }

        /// <summary>
        /// On call media state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _call_OnCallMediaState(object sender, Param.CallMediaStateParam e)
        {
            e.Suspend = false;
            if (e.CallOnHold)
                e.Suspend = true;

            UISync.Execute(() =>
            {
                if (e.CallOnHold)
                    buttonHold.Enabled = true;

                // Get the current state of the call.
                if (e.CallOnHold)
                    buttonHold.Text = "Un-Hold";
                else
                    buttonHold.Text = "Hold";
            });
        }

        /// <summary>
        /// On call state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _call_OnCallState(object sender, Param.CallStateParam e)
        {
            UISync.Execute(() =>
            {
                // If calling
                if ((e.State == Nequeo.Net.Sip.InviteSessionState.PJSIP_INV_STATE_CALLING))
                {

                }
            });
        }

        /// <summary>
        /// Hangup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHangup_Click(object sender, EventArgs e)
        {
            HangupCall();
            EnableHangupCall();
        }

        /// <summary>
        /// Hangup call.
        /// </summary>
        private void HangupCall()
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.Hangup();
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Enable hangup call.
        /// </summary>
        private void EnableHangupCall()
        {
            // Enable.
            buttonCall.Enabled = true;
            buttonHold.Enabled = false;
            buttonHangup.Enabled = false;
            groupBoxDigits.Enabled = false;
            comboBoxCallNumber.Enabled = true;
            checkBoxSuspend.Enabled = false;
            _call = null;
        }

        /// <summary>
        /// Digit 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOne_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("1");
                    textBoxDigits.Text += "1";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTwo_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("2");
                    textBoxDigits.Text += "2";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 3.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonThree_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("3");
                    textBoxDigits.Text += "3";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 4.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFour_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("4");
                    textBoxDigits.Text += "4";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 5.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFive_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("5");
                    textBoxDigits.Text += "5";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 6.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSix_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("6");
                    textBoxDigits.Text += "6";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 7.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSeven_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("7");
                    textBoxDigits.Text += "7";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 8.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEight_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("8");
                    textBoxDigits.Text += "8";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 9.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNine_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("9");
                    textBoxDigits.Text += "9";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit *.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStar_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("*");
                    textBoxDigits.Text += "*";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit 0.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonZero_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("0");
                    textBoxDigits.Text += "0";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Digit #.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHash_Click(object sender, EventArgs e)
        {
            try
            {
                // If call.
                if (_call != null)
                {
                    // Hangup.
                    _call.DialDtmf("#");
                    textBoxDigits.Text += "#";
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Key pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxCallNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                AddCallList();
            }
        }

        /// <summary>
        /// Add the call to the list.
        /// </summary>
        private void AddCallList()
        {
            if (!comboBoxCallNumber.Items.Contains(comboBoxCallNumber.Text))
                comboBoxCallNumber.Items.Add(comboBoxCallNumber.Text);
        }

        /// <summary>
        /// Instant message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonInstantMessage_Click(object sender, EventArgs e)
        {
            if (_instantMessage == null)
            {
                string incomingFilePath = (_common != null ? _common.InstantMessageFilePath : null);
                int audioDeviceIndex = (_common != null ? _common.AudioDeviceIndex : -1);
                _instantMessage = new InstantMessage(_voipCall, listViewContact, imageListSmall, imageListLarge, incomingFilePath, audioDeviceIndex);
                _instantMessage.OnInstantMessageClosing += _instantMessage_OnClosing;
                _instantMessage.Show();
                buttonInstantMessage.Enabled = false;
            }
        }

        /// <summary>
        /// On instanr message closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _instantMessage_OnClosing(object sender, EventArgs e)
        {
            buttonInstantMessage.Enabled = true;
            _instantMessage = null;
        }

        /// <summary>
        /// Load contacts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadContacts_Click(object sender, EventArgs e)
        {
            // Only load if a contacts file path exists.
            if (!String.IsNullOrEmpty(_contactsFilePath))
            {
                // Enable contacts.
                groupBoxContact.Enabled = true;

                try
                {
                    // Save the contacts.
                    if (_contacts != null && _contacts.contact != null)
                    {
                        // Deserialise the xml file into.
                        GeneralSerialisation serial = new GeneralSerialisation();
                        bool authData = serial.Serialise(_contacts, typeof(Data.contacts), _contactsFilePath);
                    }
                }
                catch { }

                try
                {
                    // Remove all contacts.
                    if (listViewContact.Items.Count > 0)
                        listViewContact.Items.Clear();

                    // Deserialise the xml file into
                    GeneralSerialisation serial = new GeneralSerialisation();
                    _contacts = ((Data.contacts)serial.Deserialise(typeof(Data.contacts), _contactsFilePath));

                    // Load the contacts.
                    if (_contacts != null && _contacts.contact != null)
                    {
                        // For each contact.
                        foreach (Data.contactsContact contact in _contacts.contact)
                        {
                            int imageIndex = 0;

                            // If a picture exists.
                            if (!String.IsNullOrEmpty(contact.picture))
                            {
                                try
                                {
                                    // Add images large and small.
                                    Image picture = Image.FromFile(contact.picture);
                                    imageListLarge.Images.Add(picture);
                                    imageListSmall.Images.Add(picture);

                                    // Get the index of the image.
                                    imageIndex = imageListLarge.Images.Count - 1;
                                }
                                catch { imageIndex = 0; }

                            }

                            // Create a new list item.
                            ListViewItem item = new ListViewItem(contact.name, imageIndex);
                            item.Name = contact.sipAccount;
                            item.SubItems.Add(item.Name);
                            item.SubItems.Add("Offline");

                            // Select the group
                            switch (contact.group.ToLower().Trim())
                            {
                                case "friend":
                                    item.Group = listViewContact.Groups["listViewGroupFriends"];
                                    break;
                                case "family":
                                    item.Group = listViewContact.Groups["listViewGroupFamily"];
                                    break;
                                case "work":
                                    item.Group = listViewContact.Groups["listViewGroupWork"];
                                    break;
                                case "business":
                                    item.Group = listViewContact.Groups["listViewGroupBusiness"];
                                    break;
                                case "colleague":
                                    item.Group = listViewContact.Groups["listViewGroupColleagues"];
                                    break;
                                case "misc":
                                    item.Group = listViewContact.Groups["listViewGroupMisc"];
                                    break;
                                case "government":
                                    item.Group = listViewContact.Groups["listViewGroupGovernment"];
                                    break;
                                case "private":
                                    item.Group = listViewContact.Groups["listViewGroupPrivate"];
                                    break;
                                case "public":
                                    item.Group = listViewContact.Groups["listViewGroupPublic"];
                                    break;
                                default:
                                    item.Group = listViewContact.Groups["listViewGroupMisc"];
                                    break;
                            }

                            // Add the contacts.
                            listViewContact.Items.Add(item);
                            //listViewContact.Items.Add(contact.sipAccount, contact.name, imageIndex);

                            // Create the contact in the account.
                            Nequeo.Net.Sip.ContactConnection contactConnection = new Net.Sip.ContactConnection(contact.presenceState, contact.sipAccount);
                            _voipCall.VoIPManager.AddContact(contactConnection);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ask the used to answer incomming call.
                    DialogResult result = MessageBox.Show(this, "Unable to load contacts because of an internal error. " + ex.Message,
                        "Load Contacts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Disable.
                buttonContactDelete.Enabled = false;
                buttonContactUpdate.Enabled = false;

                // If not created.
                if (_contacts == null)
                {
                    // Create the contact list.
                    _contacts = new Data.contacts();
                    _contacts.contact = new Data.contactsContact[0];
                }
            }
            else
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Unable to load contacts because no contacts file has been set.",
                    "Load Contacts", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Contacts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewContact_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if items selected.
            if (listViewContact.SelectedItems.Count > 0)
            {
                buttonContactDelete.Enabled = true;
                buttonContactUpdate.Enabled = true;

                // Remove the items in the menu strip.
                contextMenuStripContacts.Items.Clear();

                // For each contact.
                foreach (ListViewItem item in listViewContact.SelectedItems)
                {
                    // Find from contact file.
                    Data.contactsContact contact = null;
                    try
                    {
                        // Find from contact file.
                        contact = _contacts.contact.First(u => u.sipAccount == item.Name);
                    }
                    catch { }

                    // Found the contact.
                    if (contact != null)
                    {
                        // Set the contact name.
                        _contactName = contact.name;
                        _foundContactName = true;

                        // Add the sip account.
                        ToolStripMenuItem menuItem = new ToolStripMenuItem("Call " + contact.name + "'s sip account");
                        menuItem.Tag = contact.sipAccount;
                        menuItem.Click += MenuItem_Click;
                        contextMenuStripContacts.Items.Add(menuItem);

                        // For each numer.
                        foreach (string number in contact.numbers)
                        {
                            string[] numb = number.Split(new char[] { '|' });
                            ToolStripMenuItem menuItemNumber = new ToolStripMenuItem("Call " + contact.name + "'s " + numb[0]);
                            menuItemNumber.Tag = numb[1];
                            menuItemNumber.Click += MenuItem_Click;
                            contextMenuStripContacts.Items.Add(menuItemNumber);
                        }
                    }
                    else
                    {
                        // Not found.
                        _foundContactName = false;
                    }
                }
            }
            else
            {
                buttonContactDelete.Enabled = false;
                buttonContactUpdate.Enabled = false;

                // Remove the items in the menu strip.
                contextMenuStripContacts.Items.Clear();
            }
        }

        /// <summary>
        /// On menu item clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, EventArgs e)
        {
            // Get the menu item number.
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string number = menuItem.Tag.ToString();
            comboBoxCallNumber.Text = number;

            // Make the call.
            CallContact();
        }

        /// <summary>
        /// Add.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContactAdd_Click(object sender, EventArgs e)
        {
            UI.ContactInfo contact = new ContactInfo(_voipCall, null, true, _contacts, listViewContact);
            contact.ShowDialog(this);

            try
            {
                // If a new contact has been created.
                if (contact.NewContact)
                {
                    int imageIndex = 0;

                    // If a picture exists.
                    if (!String.IsNullOrEmpty(contact.ContactPicture))
                    {
                        try
                        {
                            // Add images large and small.
                            Image picture = Image.FromFile(contact.ContactPicture);
                            imageListLarge.Images.Add(picture);
                            imageListSmall.Images.Add(picture);

                            // Get the index of the image.
                            imageIndex = imageListLarge.Images.Count - 1;
                        }
                        catch { imageIndex = 0; }

                    }

                    // Create a new list item.
                    ListViewItem item = new ListViewItem(contact.ContactName, imageIndex);
                    item.Name = contact.SipAccount;
                    item.SubItems.Add(item.Name);
                    item.SubItems.Add("Offline");

                    // Select the group
                    switch (contact.ContactGroup.ToLower().Trim())
                    {
                        case "friend":
                            item.Group = listViewContact.Groups["listViewGroupFriends"];
                            break;
                        case "family":
                            item.Group = listViewContact.Groups["listViewGroupFamily"];
                            break;
                        case "work":
                            item.Group = listViewContact.Groups["listViewGroupWork"];
                            break;
                        case "business":
                            item.Group = listViewContact.Groups["listViewGroupBusiness"];
                            break;
                        case "colleague":
                            item.Group = listViewContact.Groups["listViewGroupColleagues"];
                            break;
                        case "misc":
                            item.Group = listViewContact.Groups["listViewGroupMisc"];
                            break;
                        case "government":
                            item.Group = listViewContact.Groups["listViewGroupGovernment"];
                            break;
                        case "private":
                            item.Group = listViewContact.Groups["listViewGroupPrivate"];
                            break;
                        case "public":
                            item.Group = listViewContact.Groups["listViewGroupPublic"];
                            break;
                        default:
                            item.Group = listViewContact.Groups["listViewGroupMisc"];
                            break;
                    }

                    // Add the contacts.
                    listViewContact.Items.Add(item);
                    //listViewContact.Items.Add(contact.SipAccount, contact.ContactName, imageIndex);

                    // Create the contact in the account.
                    Nequeo.Net.Sip.ContactConnection contactConnection = new Net.Sip.ContactConnection(contact.PresenecState, contact.SipAccount);
                    _voipCall.VoIPManager.AddContact(contactConnection);
                }
            }
            catch (Exception ex)
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Unable to add contact because of an internal error. " + ex.Message,
                    "Add Contact", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContactDelete_Click(object sender, EventArgs e)
        {
            string contactName = "";
            string contactKey = "";

            // Add each contact.
            foreach (ListViewItem item in listViewContact.SelectedItems)
            {
                // Get the name.
                contactName = item.Text;
                contactKey = item.Name;
                break;
            }

            // If a key has been selected.
            if (!String.IsNullOrEmpty(contactKey))
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Are you sure you wish to delete contact " + contactName + ".",
                    "Delete Contact", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If delete.
                if (result == DialogResult.Yes)
                {
                    // Remove the item.
                    listViewContact.Items.RemoveByKey(contactKey);

                    try
                    {
                        // Remove from contact file.
                        Data.contactsContact contact = _contacts.contact.First(u => u.sipAccount == contactKey);
                        _contacts.contact = _contacts.contact.Remove(u => u.Equals(contact));
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContactUpdate_Click(object sender, EventArgs e)
        {
            string contactKey = "";
            string contactName = "";
            ListViewGroup contactGroup = null;

            // Add each contact.
            foreach (ListViewItem item in listViewContact.SelectedItems)
            {
                // Get the name.
                contactKey = item.Name;
                contactName = item.Text;
                contactGroup = item.Group;
                break;
            }

            // If a key has been selected.
            if (!String.IsNullOrEmpty(contactKey))
            {
                UI.ContactInfo contact = new ContactInfo(_voipCall, contactKey, false, _contacts, listViewContact);
                contact.ShowDialog(this);

                // If the contact name has changed.
                if (!String.IsNullOrEmpty(contact.ContactName) && contact.ContactName != contactName)
                {
                    // Get the contact.
                    ListViewItem item = listViewContact.Items[contactKey];

                    // If found.
                    if (item != null)
                    {
                        // Set the state.
                        item.Text = contact.ContactName;
                    }
                }

                // If the contact group has changed.
                if (contactGroup != null && 
                    !String.IsNullOrEmpty(contact.ContactGroup) && 
                    contact.ContactGroup != contactGroup.Header)
                {
                    // Get the contact.
                    ListViewItem item = listViewContact.Items[contactKey];

                    // If found.
                    if (item != null)
                    {
                        // Select the group
                        switch (contact.ContactGroup.ToLower().Trim())
                        {
                            case "friend":
                                item.Group = listViewContact.Groups["listViewGroupFriends"];
                                break;
                            case "family":
                                item.Group = listViewContact.Groups["listViewGroupFamily"];
                                break;
                            case "work":
                                item.Group = listViewContact.Groups["listViewGroupWork"];
                                break;
                            case "business":
                                item.Group = listViewContact.Groups["listViewGroupBusiness"];
                                break;
                            case "colleague":
                                item.Group = listViewContact.Groups["listViewGroupColleagues"];
                                break;
                            case "misc":
                                item.Group = listViewContact.Groups["listViewGroupMisc"];
                                break;
                            case "government":
                                item.Group = listViewContact.Groups["listViewGroupGovernment"];
                                break;
                            case "private":
                                item.Group = listViewContact.Groups["listViewGroupPrivate"];
                                break;
                            case "public":
                                item.Group = listViewContact.Groups["listViewGroupPublic"];
                                break;
                            default:
                                item.Group = listViewContact.Groups["listViewGroupMisc"];
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Account status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAccStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisableSetStatus();
        }

        /// <summary>
        /// Account activity.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAccActivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisableSetStatus();
        }

        /// <summary>
        /// Status text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAccStatusText_TextChanged(object sender, EventArgs e)
        {
            EnableDisableSetStatus();
        }

        /// <summary>
        /// Note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAccNote_TextChanged(object sender, EventArgs e)
        {
            EnableDisableSetStatus();
        }

        /// <summary>
        /// Set online status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAccSetStatus_Click(object sender, EventArgs e)
        {
            try
            {
                // Create the presence status.
                Nequeo.Net.Sip.PresenceStatus status = new Nequeo.Net.Sip.PresenceStatus();

                // Select the status.
                switch (comboBoxAccStatus.SelectedIndex)
                {
                    case 0:
                        status.Status = Nequeo.Net.Sip.ContactStatus.UNKNOWN;
                        break;
                    case 1:
                        status.Status = Nequeo.Net.Sip.ContactStatus.ONLINE;
                        break;
                    case 2:
                        status.Status = Nequeo.Net.Sip.ContactStatus.OFFLINE;
                        break;
                    default:
                        status.Status = Nequeo.Net.Sip.ContactStatus.UNKNOWN;
                        break;
                }

                // Select the activity.
                switch (comboBoxAccActivity.SelectedIndex)
                {
                    case 0:
                        status.Activity = Nequeo.Net.Sip.RpidActivity.UNKNOWN;
                        break;
                    case 1:
                        status.Activity = Nequeo.Net.Sip.RpidActivity.AWAY;
                        break;
                    case 2:
                        status.Activity = Nequeo.Net.Sip.RpidActivity.BUSY;
                        break;
                    default:
                        status.Activity = Nequeo.Net.Sip.RpidActivity.UNKNOWN;
                        break;
                }

                // Set the note.
                status.StatusText = (!String.IsNullOrEmpty(textBoxAccStatusText.Text) ? textBoxAccStatusText.Text : status.Status.ToString());
                status.Note = (!String.IsNullOrEmpty(textBoxAccNote.Text) ? textBoxAccNote.Text : status.Activity.ToString());
                status.RpidId = Guid.NewGuid().ToString();

                // Set the online status.
                _voipCall.VoIPManager.SetOnlineStatus(status);
            }
            catch { }
        }

        /// <summary>
        /// Enable disable set status.
        /// </summary>
        private void EnableDisableSetStatus()
        {
            if (comboBoxAccStatus.SelectedIndex >= 0 &&
                comboBoxAccActivity.SelectedIndex >= 0)
            {
                buttonAccSetStatus.Enabled = true;
            }
            else
            {
                buttonAccSetStatus.Enabled = false;
            }
        }

        /// <summary>
        /// Get account details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAccDetails_Click(object sender, EventArgs e)
        {
            // Get the info.
            Nequeo.Net.Sip.AccountInfo info = _voipCall.VoIPManager.GetAccountInfo();
            checkBoxAccDetails.Checked = info.IsDefault;
            checkBoxAccIsOnline.Checked = info.OnlineStatus;
            textBoxAccOnlineText.Text = info.OnlineStatusText;
            textBoxAccRegExpiresSec.Text = info.RegExpiresSec.ToString();
            checkBoxAccRegIsActive.Checked = info.RegIsActive;
            checkBoxAccRegIsConfigured.Checked = info.RegIsConfigured;
            textBoxAccRegStatus.Text = info.RegStatus.ToString();
            textBoxAccRegStatusText.Text = info.RegStatusText;
            labelAccAccountUri.Text = info.Uri;
            checkBoxAccIsValid.Checked = _voipCall.VoIPManager.IsValid();
        }

        /// <summary>
        /// Contact view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxContactView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxContactView.SelectedIndex >= 0)
            {
                // Select the view.
                switch (comboBoxContactView.SelectedIndex)
                {
                    case 0:
                        listViewContact.View = View.Details;
                        break;
                    case 1:
                        listViewContact.View = View.LargeIcon;
                        break;
                    case 2:
                        listViewContact.View = View.List;
                        break;
                    case 3:
                        listViewContact.View = View.SmallIcon;
                        break;
                    case 4:
                        listViewContact.View = View.Tile;
                        break;
                    default:
                        listViewContact.View = View.Details;
                        break;
                }
            }
        }

        /// <summary>
        /// Mute volume.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxMuteVolume_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxMuteVolume.CheckState)
            {
                case CheckState.Checked:
                    Nequeo.IO.Audio.Volume.MuteVolume(true);
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    Nequeo.IO.Audio.Volume.MuteVolume(false);
                    break;
            }
        }

        /// <summary>
        /// Mute microphone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxMuteMicrophone_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxMuteMicrophone.CheckState)
            {
                case CheckState.Checked:
                    Nequeo.IO.Audio.Volume.MuteMicrophone(true);
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    Nequeo.IO.Audio.Volume.MuteMicrophone(false);
                    break;
            }
        }

        /// <summary>
        /// Load configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConfiguration_Click(object sender, EventArgs e)
        {
            // Set the import filter.
            openFileDialog.Filter = "Xml File (*.xml)|*.xml";

            // Get the file name selected.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bool hasError = false;
                string xml = openFileDialog.FileName;

                try
                {
                    // Deserialise the xml file into
                    GeneralSerialisation serial = new GeneralSerialisation();
                    _configuration = ((Data.configuration)serial.Deserialise(typeof(Data.configuration), xml));

                    // Assign the configuration.
                    _common.AccountName = _configuration.accountName;
                    _common.SipHost = _configuration.sipHost;
                    _common.SipUsername = _configuration.sipUsername;
                    _common.SipPassword = _configuration.sipPassword;
                    _common.IncomingCallRingFilePath = _configuration.soundIncomingCallFilePath;
                    _common.InstantMessageFilePath = _configuration.soundInstantMessageFilePath;
                    _common.AudioDeviceIndex = _configuration.soundAudioDeviceIndex;
                    _common.CaptureAudioDeviceIndex = _configuration.captureAudioDeviceIndex;
                    _common.PlaybackAudioDeviceIndex = _configuration.playbackAudioDeviceIndex;
                    _common.IncomingCallAudioRecordingEnabled = _configuration.incomingCallAudioRecordingEnabled;
                    _common.OutgoingCallAudioRecordingEnabled = _configuration.outgoingCallAudioRecordingEnabled;
                    _common.AutoAnswerFilePath = _configuration.soundAutoAnswerFilePath;
                    _common.AutoAnswer = _configuration.accountAutoAnswerEnabled;
                    _common.AutoAnswerWait = _configuration.accountAutoAnswerWait;
                    _common.MessageBankWait = _configuration.accountMessageBankWait;
                    _common.EnableVideo = _configuration.codecVideoEnabled;
                    _common.VideoCaptureIndex = _configuration.codecVideoCaptureDeviceIndex;
                    _common.VideoRenderIndex = _configuration.codecVideoRenderDeviceIndex;
                    _common.EnableRedirect = _configuration.featureRedirectCallEnabled;
                    _common.RedirectCallNumber = _configuration.featureRedirectCallNumber;
                    _common.RedirectCallAfter = _configuration.featureRedirectCallAfter;

                    _audioRecordingOutCallPath = _configuration.outgoingCallPathAudioRecording;
                    _audioRecordingInCallPath = _configuration.incomingCallPathAudioRecording;
                    _contactsFilePath = _configuration.contactFilePath;

                    // Assign the account.
                    _voipCall.VoIPManager.AccountConnection.DelayBeforeRefreshSec = _configuration.timeDelayBeforeRefresh;
                    _voipCall.VoIPManager.AccountConnection.DropCallsOnFail = _configuration.accountDropCallsOnFail;
                    _voipCall.VoIPManager.AccountConnection.FirstRetryIntervalSec = _configuration.timeFirstRetryInterval;
                    _voipCall.VoIPManager.AccountConnection.IceEnabled = _configuration.accountIceEnabled;
                    _voipCall.VoIPManager.AccountConnection.IPv6Use = (_configuration.accountUseIPv6 ? Net.Sip.IPv6_Use.IPV6_ENABLED : Net.Sip.IPv6_Use.IPV6_DISABLED);
                    _voipCall.VoIPManager.AccountConnection.IsDefault = _configuration.accountIsDefault;
                    _voipCall.VoIPManager.AccountConnection.MediaTransportPort = _configuration.mediaTransportPort;
                    _voipCall.VoIPManager.AccountConnection.MediaTransportPortRange = _configuration.mediaTransportPortRange;
                    _voipCall.VoIPManager.AccountConnection.MessageWaitingIndication = _configuration.mwiEnabled;
                    _voipCall.VoIPManager.AccountConnection.MWIExpirationSec = _configuration.mwiExpiration;
                    _voipCall.VoIPManager.AccountConnection.NoIceRtcp = _configuration.accountNoIceRtcp;
                    _voipCall.VoIPManager.AccountConnection.Priority = _configuration.accountPriority;
                    _voipCall.VoIPManager.AccountConnection.PublishEnabled = _configuration.publishEnabled;
                    _voipCall.VoIPManager.AccountConnection.PublishQueue = _configuration.publishQueue;
                    _voipCall.VoIPManager.AccountConnection.PublishShutdownWaitMsec = _configuration.publishShutdownWait;
                    _voipCall.VoIPManager.AccountConnection.RegisterOnAdd = _configuration.accountRegisterOnAdd;
                    _voipCall.VoIPManager.AccountConnection.RetryIntervalSec = _configuration.timeRetryInterval;
                    _voipCall.VoIPManager.AccountConnection.SpPort = _configuration.accountPort;
                    _voipCall.VoIPManager.AccountConnection.SRTPSecureSignaling = (_configuration.accountSrtpSecureSignaling ? Net.Sip.SRTP_SecureSignaling.SRTP_REQUIRES : Net.Sip.SRTP_SecureSignaling.SRTP_DISABLED);
                    _voipCall.VoIPManager.AccountConnection.SRTPUse = (_configuration.accountUseSrtp ? Net.Sip.SRTP_Use.SRTP_MANDATORY : Net.Sip.SRTP_Use.SRTP_DISABLED);
                    _voipCall.VoIPManager.AccountConnection.TimeoutSec = _configuration.timeTimeout;
                    _voipCall.VoIPManager.AccountConnection.TimerMinSESec = _configuration.timerMinimumSession;
                    _voipCall.VoIPManager.AccountConnection.TimerSessExpiresSec = _configuration.timerSessionExpires;
                    _voipCall.VoIPManager.AccountConnection.UnregWaitSec = _configuration.timeUnregisterWait;
                    _voipCall.VoIPManager.AccountConnection.VideoRateControlBandwidth = _configuration.featureVideoBandwidthRate;
                }
                catch (Exception ex)
                {
                    // Ask the used to answer incomming call.
                    DialogResult result = MessageBox.Show(this, "Unable to load configuration because of an internal error. " + ex.Message,
                        "Load Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    hasError = true;
                }

                // If no error.
                if (!hasError)
                {
                    // Disable Enable.
                    buttonSettings.Enabled = true;
                    buttonConfiguration.Enabled = false;
                }

                // If not created.
                if (_configuration == null)
                {
                    // Create the contact list.
                    _configuration = new Data.configuration();
                }
            }
        }

        /// <summary>
        /// Save calls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonInOutCallsSave_Click(object sender, EventArgs e)
        {
            // If items exist.
            if (listViewInOutCalls.Items.Count > 0)
            {
                // Set the audio filter.
                saveFileDialog.Filter = "Text File (*.txt)|*.txt";

                // Get the file name selected.
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.StreamWriter file = null;
                    try
                    {
                        // Write the file data.
                        file = System.IO.File.CreateText(saveFileDialog.FileName);

                        // For each call.
                        foreach (Param.CallInfoParam call in _inOutCalls)
                        {
                            // Write the call.
                            file.WriteLine(
                                call.ContactName + "," +
                                call.Date.ToShortDateString() + " " + call.Date.ToShortTimeString() + "," +
                                (call.IncomingOutgoing ? "Incoming" : "Outgoing") + "," +
                                call.FromTo + "," +
                                call.TotalDuration.ToString() + "," +
                                call.ConnectDuration.ToString());
                        }
                    }
                    catch { }
                    finally
                    {
                        if (file != null)
                            file.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// In out calls list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewInOutCalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if items selected.
            if (listViewInOutCalls.SelectedItems.Count > 0)
            {
                // Remove the items in the menu strip.
                contextMenuStripCalls.Items.Clear();

                // For each contact.
                foreach (ListViewItem item in listViewInOutCalls.SelectedItems)
                {
                    textBoxInOutCallsDetails.Text = item.Text + "  " + item.SubItems[3].Text;

                    // Find from contact file.
                    Data.contactsContact contact = null;
                    try
                    {
                        // Find from contact file.
                        contact = _contacts.contact.First(u => u.name.ToLower() == item.Text.ToLower());
                    }
                    catch { contact = null; }

                    // Did not find the contact.
                    if (contact == null)
                    {
                        try
                        {
                            // Find from contact file.
                            contact = _contacts.contact.First(u => u.sipAccount == item.Name);
                        }
                        catch { contact = null; }
                    }

                    // Found the contact.
                    if (contact != null)
                    {
                        // Add the sip account.
                        ToolStripMenuItem menuItem = new ToolStripMenuItem("Call " + contact.name + "'s sip account");
                        menuItem.Tag = contact.sipAccount;
                        menuItem.Click += MenuItem_Click;
                        contextMenuStripCalls.Items.Add(menuItem);

                        // For each numer.
                        foreach (string number in contact.numbers)
                        {
                            string[] numb = number.Split(new char[] { '|' });
                            ToolStripMenuItem menuItemNumber = new ToolStripMenuItem("Call " + contact.name + "'s " + numb[0]);
                            menuItemNumber.Tag = numb[1];
                            menuItemNumber.Click += MenuItem_Click;
                            contextMenuStripCalls.Items.Add(menuItemNumber);
                        }
                    }
                    else
                    {
                        // Split the item name.
                        string[] itemName = item.Name.Split(new char[] { '|' });

                        // Add the sip account.
                        ToolStripMenuItem menuItem = new ToolStripMenuItem("Call " + itemName[0] + " sip account");
                        menuItem.Tag = itemName[0];
                        menuItem.Click += MenuItem_Click;
                        contextMenuStripCalls.Items.Add(menuItem);

                        // Split the calling number.
                        string[] splitFrom = itemName[0].Split(new char[] { '@' });
                        string number = splitFrom[0].Replace("sip:", "").Replace("sips:", "");

                        // Add the number.
                        ToolStripMenuItem menuItemNum = new ToolStripMenuItem("Call " + number);
                        menuItemNum.Tag = number;
                        menuItemNum.Click += MenuItem_Click;
                        contextMenuStripCalls.Items.Add(menuItemNum);
                    }

                    // Add the seperator.
                    ToolStripSeparator menuItemSep = new ToolStripSeparator();
                    contextMenuStripCalls.Items.Add(menuItemSep);

                    // Add delete.
                    ToolStripMenuItem menuItemDelete = new ToolStripMenuItem("Delete " + item.Text);
                    menuItemDelete.Tag = item.Text + " " + item.SubItems[2].Text + "|" + item.Name;
                    menuItemDelete.Click += MenuItemDelete_Click;
                    contextMenuStripCalls.Items.Add(menuItemDelete);
                }
            }
            else
            {
                // Remove the items in the menu strip.
                contextMenuStripCalls.Items.Clear();
                textBoxInOutCallsDetails.Text = "";
            }
        }

        /// <summary>
        /// On menu item clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemDelete_Click(object sender, EventArgs e)
        {
            // Get the menu item number.
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string tag = menuItem.Tag.ToString();

            // Split the tag.
            string[] tagName = tag.Split(new char[] { '|' });
            string sipAccount = tagName[1];
            string guid = tagName[2];

            // Delete the call.
            DialogResult result = MessageBox.Show(this, "Are you sure you wish to delete call " + tagName[0] + ".",
                "Delete Call", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If delete.
            if (result == DialogResult.Yes)
            {
                // Remove the item.
                listViewInOutCalls.Items.RemoveByKey(sipAccount + "|" + guid);
            }
        }

        /// <summary>
        /// Conference view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewConference_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if items selected.
            if (listViewConference.SelectedItems.Count > 0)
                contextMenuStripConference.Enabled = true;
            else
                contextMenuStripConference.Enabled = false;

            // Enable or disable conference controls.
            EnableDisableConferenceList();

            // if items selected.
            if (listViewConference.SelectedItems.Count > 0)
            {
                string contactKey = "";

                // Add each contact.
                foreach (ListViewItem item in listViewConference.SelectedItems)
                {
                    // Get the name.
                    contactKey = item.Name;
                    break;
                }

                // If a key has been selected.
                if (!String.IsNullOrEmpty(contactKey))
                {
                    // Video calls.
                    Param.CallParam caller = null;

                    try
                    {
                        // Find the caller.
                        string[] name = contactKey.Split(new char[] { '|' });
                        string callid = name[0];
                        string id = name[1];
                        caller = _voipCall.ConferenceCall.First(u => u.ID == id);
                    }
                    catch { caller = null; }

                    // If found.
                    if (caller != null)
                    {
                        // Start or stop transmitting media.
                        if (caller.IsTransmitting)
                            // Suspend.
                            toolStripMenuItemConferenceSuspend.Checked = false;
                        else
                            // Suspend.
                            toolStripMenuItemConferenceSuspend.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Enable or disable conference controls.
        /// </summary>
        private void EnableDisableConferenceList()
        {
            // If items exist.
            if (listViewConference.Items.Count > 0)
            {
                buttonConferenceHangupAll.Enabled = true;
                checkBoxConferenceSuspendAll.Enabled = true;
            }
            else
            {
                buttonConferenceHangupAll.Enabled = false;
                checkBoxConferenceSuspendAll.Enabled = false;
            }
        }

        /// <summary>
        /// Hangup all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConferenceHangupAll_Click(object sender, EventArgs e)
        {
            if (listViewConference.Items.Count > 0)
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Are you sure you wish to hangup all calls.",
                    "Cancel Conference", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If delete.
                if (result == DialogResult.Yes)
                {
                    // Hangup all calls.
                    Param.CallParam[] conference = _voipCall.ConferenceCall;
                    _voipCall.RemoveAllConferenceCallContacts();

                    // Disconnect.
                    if (conference != null && conference.Length > 0)
                    {
                        // For each caller.
                        foreach (Param.CallParam caller in conference)
                        {
                            try
                            {
                                // Force hangup.
                                caller.Hangup();
                            }
                            catch { }

                            try
                            {
                                // Clean-up.
                                caller.Dispose();
                            }
                            catch { }
                        }
                    }

                    // Remove all callers
                    listViewConference.Items.Clear();
                    checkBoxConferenceSuspendAll.Enabled = false;
                }
            }

            // Enable or disable conference controls.
            EnableDisableConferenceList();
        }

        /// <summary>
        /// Hangup conference caller.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemConferenceHangup_Click(object sender, EventArgs e)
        {
            string contactKey = "";
            string contactName = "";

            // Add each contact.
            foreach (ListViewItem item in listViewConference.SelectedItems)
            {
                // Get the name.
                contactKey = item.Name;
                contactName = item.Text;
                break;
            }

            // If a key has been selected.
            if (!String.IsNullOrEmpty(contactKey))
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Are you sure you wish to hangup caller " + contactName + ".",
                    "Cancel Conference", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If delete.
                if (result == DialogResult.Yes)
                {
                    // Hangup all calls.
                    Param.CallParam caller = null;

                    try
                    {
                        // Find the caller.
                        string[] name = contactKey.Split(new char[] { '|' });
                        string callid = name[0];
                        string id = name[1];
                        caller = _voipCall.ConferenceCall.First(u => u.ID == id);
                    }
                    catch { caller = null; }

                    // If found.
                    if (caller != null)
                    {
                        // Remove caller.
                        _voipCall.RemoveConferenceCallContact(caller.ID);

                        try
                        {
                            // Force hangup.
                            caller.Hangup();
                        }
                        catch { }

                        try
                        {
                            // Clean-up.
                            caller.Dispose();
                        }
                        catch { }
                    }

                    // Remove the item.
                    listViewConference.Items.RemoveByKey(contactKey);
                }
            }

            // Enable or disable conference controls.
            EnableDisableConferenceList();
        }

        /// <summary>
        /// Suspend all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxConferenceSuspendAll_CheckedChanged(object sender, EventArgs e)
        {
            // Select the state.
            switch (checkBoxConferenceSuspendAll.CheckState)
            {
                case CheckState.Checked:
                    _voipCall.SuspendConferenceCall(true);
                    break;
                case CheckState.Indeterminate:
                case CheckState.Unchecked:
                    _voipCall.SuspendConferenceCall(false);
                    break;
            }
        }

        /// <summary>
        /// Suspend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemConferenceSuspend_Click(object sender, EventArgs e)
        {
            string contactKey = "";

            // Add each contact.
            foreach (ListViewItem item in listViewConference.SelectedItems)
            {
                // Get the name.
                contactKey = item.Name;
                break;
            }

            // If a key has been selected.
            if (!String.IsNullOrEmpty(contactKey))
            {
                // Hangup all calls.
                Param.CallParam caller = null;

                try
                {
                    // Find the caller.
                    string[] name = contactKey.Split(new char[] { '|' });
                    string callid = name[0];
                    string id = name[1];
                    caller = _voipCall.ConferenceCall.First(u => u.ID == id);
                }
                catch { caller = null; }

                // If found.
                if (caller != null)
                {
                    // Start or stop transmitting media.
                    if (caller.IsTransmitting)
                    {
                        // Suspend.
                        toolStripMenuItemConferenceSuspend.Checked = true;

                        // Stop transmitting.
                        caller.StopTransmitting();
                    }
                    else
                    {
                        // Suspend.
                        toolStripMenuItemConferenceSuspend.Checked = false;

                        // Start transmitting.
                        caller.StartTransmitting();
                    }
                }
            }
        }

        /// <summary>
        /// Clear digits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDigitsClear_Click(object sender, EventArgs e)
        {
            textBoxDigits.Text = "";
        }

        /// <summary>
        /// Microphone.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarMicrophone_Scroll(object sender, EventArgs e)
        {
            try
            {
                // Set the volume.
                Nequeo.IO.Audio.Volume.SetMicrophoneVolume((float)(trackBarMicrophone.Value / 100.0));
                labelMicrophoneLevel.Text = trackBarMicrophone.Value.ToString();
            }
            catch { }
        }

        /// <summary>
        /// Volume.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            try
            {
                // Set the volume.
                Nequeo.IO.Audio.Volume.SetSpeakerVolume((float)(trackBarVolume.Value / 100.0));
                labelVolumeLevel.Text = trackBarVolume.Value.ToString();
            }
            catch { }
        }

        /// <summary>
        /// Hold call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHold_Click(object sender, EventArgs e)
        {
            // Is the call on hold.
            if (_call.CallOnHold)
            {
                try
                {
                    // Un hold the call.
                    _call.Hold();
                }
                catch { }
            }
            else
            {
                try
                {
                    // Hold the call.
                    _call.Hold();
                }
                catch { }
            }
        }

        /// <summary>
        /// Suspend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxSuspend_CheckedChanged(object sender, EventArgs e)
        {
            // Call exists.
            if (_call != null)
            {
                try
                {
                    // Select the state.
                    switch (checkBoxSuspend.CheckState)
                    {
                        case CheckState.Checked:
                            _call.StopTransmitting();
                            break;
                        case CheckState.Indeterminate:
                        case CheckState.Unchecked:
                            _call.StartTransmitting();
                            break;
                    }
                }
                catch { }
            }
        }
    }
}
