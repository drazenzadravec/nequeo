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
        private Data.Common _common = null;

        private bool _audioRecordingOutCall = false;
        private bool _audioRecordingInCall = false;
        private string _audioRecordingOutCallPath = null;
        private string _audioRecordingInCallPath = null;
        private string _contactsFilePath = null;
        private bool _hasCredentials = false;
        private bool _created = false;

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

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (_voipCall != null)
                    _voipCall.Dispose();

                if (_instantMessage != null)
                    _instantMessage.Dispose();

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _voipCall = null;
                _instantMessage = null;
            }
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _common = new Data.Common();

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

                        // For each contact.
                        foreach (Data.contactsContact contact in _contacts.contact)
                        {
                            // Cleanup the sip.
                            string sipAccount = contact.sipAccount.Replace("sip:", "").Replace("sips:", "");

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

                            // For each numer.
                            foreach (string number in contact.numbers)
                            {
                                string[] numb = number.Split(new char[] { '|' });
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

                // Open the call.
                string ringFilePath = (_common != null ? _common.IncomingCallRingFilePath : null);
                int audioDeviceIndex = (_common != null ? _common.AudioDeviceIndex : -1);
                Nequeo.VoIP.Sip.UI.InComingCall incomingCall = new InComingCall(e, contactName, ringFilePath, audioDeviceIndex);
                incomingCall.Show(this);
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
            settings.AudioRecordingInCall = _audioRecordingInCall;
            settings.AudioRecordingOutCall = _audioRecordingOutCall;
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
            _audioRecordingInCall = settings.AudioRecordingInCall;
            _audioRecordingOutCall = settings.AudioRecordingOutCall;

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
                _audioRecordingInCallPath = null;
                _voipCall.IncomingCallRecordFilename(_audioRecordingInCallPath);
            }

            // Audio outgoing call.
            if (_audioRecordingOutCall && !String.IsNullOrEmpty(settings.AudioRecordingOutCallPath))
                _audioRecordingOutCallPath = settings.AudioRecordingOutCallPath;
            else
                _audioRecordingOutCallPath = null;

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
                if (!String.IsNullOrEmpty(_audioRecordingOutCallPath))
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

                // If call.
                if (_call != null)
                {
                    AddCallList();
                    buttonCall.Enabled = false;
                    buttonHangup.Enabled = true;
                    groupBoxDigits.Enabled = true;
                    comboBoxCallNumber.Enabled = false;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                // Ask the used to answer incomming call.
                DialogResult result = MessageBox.Show(this, "Unable to make the call because of an internal error.",
                    "Make Call?", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Enable.
                buttonCall.Enabled = true;
                buttonHangup.Enabled = false;
                groupBoxDigits.Enabled = false;
                comboBoxCallNumber.Enabled = true;
                _call = null;
                _uri = null;
            }
        }

        /// <summary>
        /// Hangup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHangup_Click(object sender, EventArgs e)
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

            // Enable.
            buttonCall.Enabled = true;
            buttonHangup.Enabled = false;
            groupBoxDigits.Enabled = false;
            comboBoxCallNumber.Enabled = true;
            _call = null;
            _uri = null;
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
                _instantMessage = new InstantMessage(_voipCall, listViewContact, incomingFilePath, audioDeviceIndex);
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
                            // Create a new list item.
                            ListViewItem item = new ListViewItem(contact.name, 0);
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
                            //listViewContact.Items.Add(contact.sipAccount, contact.name, 0);

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
                    // Remove from contact file.
                    Data.contactsContact contact = _contacts.contact.First(u => u.sipAccount == item.Name);

                    // Found the contact.
                    if (contact != null)
                    {
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
                    // Create a new list item.
                    ListViewItem item = new ListViewItem(contact.ContactName, 0);
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
                    //listViewContact.Items.Add(contact.SipAccount, contact.ContactName, 0);

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
    }
}
