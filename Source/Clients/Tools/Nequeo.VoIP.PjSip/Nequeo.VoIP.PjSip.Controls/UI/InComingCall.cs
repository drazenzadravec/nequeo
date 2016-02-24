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

using Nequeo.IO.Audio;

namespace Nequeo.VoIP.PjSip.UI
{
    /// <summary>
    /// In coming call.
    /// </summary>
    public partial class InComingCall : Form
    {
        /// <summary>
        /// In coming call.
        /// </summary>
        /// <param name="voipCall">VoIP call.</param>
        /// <param name="inComingCall">In coming call param.</param>
        /// <param name="contactsView">The contacts list view.</param>
        /// <param name="callsView">The calls list view.</param>
        /// <param name="conferenceView">The conference list view.</param>
        /// <param name="contacts">The contact list.</param>
        /// <param name="contactName">The contact name.</param>
        /// <param name="ringFilePath">The filename and path of the ringing audio.</param>
        /// <param name="audioDeviceIndex">The audio device index.</param>
        /// <param name="autoAnswer">Auto answer enabled.</param>
        /// <param name="autoAnswerFilePath">Auto answer file path..</param>
        /// <param name="autoAnswerWait">Auto answer wait time.</param>
        /// <param name="autoAnswerRecordingPath">Auto answer recording file and path.</param>
        /// <param name="messageBankWaitTime">The time to record the message.</param>
        public InComingCall(Nequeo.VoIP.PjSip.VoIPCall voipCall, Nequeo.VoIP.PjSip.Param.OnIncomingCallParam inComingCall,
            ListView contactsView, ListView callsView, ListView conferenceView, Data.contacts contacts, string contactName, string ringFilePath, 
            int audioDeviceIndex = -1, bool autoAnswer = false, string autoAnswerFilePath = null, int autoAnswerWait = 30, 
            string autoAnswerRecordingPath = null, int messageBankWaitTime = 20)
        {
            InitializeComponent();

            _contactName = contactName;
            _ringFilePath = ringFilePath;
            _voipCall = voipCall;
            _inComingCall = inComingCall;
            _callsView = callsView;
            _conferenceView = conferenceView;
            _contactsView = contactsView;
            _contacts = contacts;

            // Auto answer.
            _autoAnswer = autoAnswer;
            _autoAnswerFilePath = autoAnswerFilePath;
            _autoAnswerWait = autoAnswerWait;
            _autoAnswerRecordingPath = autoAnswerRecordingPath;
            _messageBankWaitTime = messageBankWaitTime;

            // If auto answer recording path.
            if (!String.IsNullOrEmpty(autoAnswerRecordingPath))
            {
                // Create the recording file name and path.
                DateTime time = DateTime.Now;
                string audioRecodingExt = System.IO.Path.GetExtension(autoAnswerRecordingPath);
                string audioRecordingDir = System.IO.Path.GetDirectoryName(autoAnswerRecordingPath).TrimEnd(new char[] { '\\' }) + "\\";
                string audioRecordingFile = System.IO.Path.GetFileNameWithoutExtension(autoAnswerRecordingPath) + "_" +
                    contactName.Replace(" ", "").Replace("'", "") + "_" +
                    time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString() + "_" +
                    time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();

                // Create the file name.
                _autoAnswerRecordingPath = audioRecordingDir + audioRecordingFile + audioRecodingExt;
            }

            // If a valid audio device has been set.
            if (audioDeviceIndex >= 0)
            {
                // Get the audio device.
                Nequeo.IO.Audio.Device device = Nequeo.IO.Audio.Devices.GetDevice(audioDeviceIndex);
                _player = new WavePlayer(device);
                _player.PlaybackStopped += _player_PlaybackStopped;
            }
        }

        private bool _hasAction = false;
        private string _contactName = "";
        private Nequeo.VoIP.PjSip.VoIPCall _voipCall = null;
        private Nequeo.VoIP.PjSip.Param.OnIncomingCallParam _inComingCall = null;
        private Nequeo.IO.Audio.WavePlayer _player = null;
        private string _ringFilePath = null;

        private bool _playerStarted = false;
        private bool _isConferenceCall = false;
        private bool _autoAnswerStarted = false;
        private bool _suspended = false;
        private Data.IncomingOutgoingCalls _inOutCalls = null;

        private ListView _callsView = null;
        private ListView _conferenceView = null;
        private ListView _contactsView = null;
        private Data.contacts _contacts = null;

        private System.Threading.Timer _autoAnswerTimer = null;
        private System.Threading.Timer _autoAnswerRecordingTimer = null;
        private bool _autoAnswer = false;
        private string _autoAnswerFilePath = null;
        private int _autoAnswerWait = 30;
        private string _autoAnswerRecordingPath = null;
        private int _messageBankWaitTime = 20;

        private Action _callEnded = null;

        /// <summary>
        /// Gets or sets the incoming outgoing calls reference.
        /// </summary>
        internal Data.IncomingOutgoingCalls IncomingOutgoingCalls
        {
            get { return _inOutCalls; }
            set { _inOutCalls = value; }
        }

        /// <summary>
        /// The call has ended.
        /// </summary>
        private void CallEnded()
        {
            try
            {
                // Close the window.
                Close();
            }
            catch { }
        }

        /// <summary>
        /// On playback stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            try
            {
                // If no exception and the audio has completed.
                if (e.Exception == null && e.AudioComplete)
                {
                    // Loop the sound.
                    _player.Play();
                }
            }
            catch { }
        }

        /// <summary>
        /// Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InComingCall_Load(object sender, EventArgs e)
        {
            _callEnded = () => CallEnded();
            this.Text = "Incoming Call - From : " + _contactName + " - Source : " + (String.IsNullOrEmpty(_inComingCall.SrcAddress) ? "Unknown" : _inComingCall.SrcAddress.Trim());

            UISync.Init(this);
            if (_inComingCall != null)
            {
                // If player.
                if (_player != null)
                {
                    try
                    {
                        // If a file exists.
                        if (!String.IsNullOrEmpty(_ringFilePath))
                        {
                            // Open the file.
                            _player.Open(_ringFilePath);

                            // Start the wave file.
                            _player.Play();
                            _playerStarted = true;
                        }
                    }
                    catch { }
                }

                // If auto answer is on.
                if (_autoAnswer && !String.IsNullOrEmpty(_autoAnswerFilePath))
                {
                    // If there is a wait time.
                    if (_autoAnswerWait > 0)
                    {
                        // Auto answer indication.
                        labelIncomingCallAutoAnswer.Text = "Auto answer in '" + _autoAnswerWait.ToString() + "' seconds.";

                        // Create the auto answer timer.
                        _autoAnswerTimer = new System.Threading.Timer(AutoAnswerTimeout, null,
                            new TimeSpan(0, 0, _autoAnswerWait),
                            new TimeSpan(0, 0, _autoAnswerWait));

                        // Suspended.
                        _suspended = true;
                    }
                }
                else
                {
                    // If message bank is enabled.
                    if (_messageBankWaitTime > 0 && !String.IsNullOrEmpty(_autoAnswerFilePath) && !String.IsNullOrEmpty(_autoAnswerRecordingPath))
                    {
                        buttonSendToMessageBank.Enabled = true;

                        // Suspended.
                        _suspended = true;
                    }
                }

                _inComingCall.Call.OnCallMediaState += Call_OnCallMediaState;
                _inComingCall.Call.OnCallState += Call_OnCallState;
                _inComingCall.Call.OnPlayerEndOfFile += Call_OnPlayerEndOfFile;
                _inComingCall.Call.OnDtmfDigit += Call_OnDtmfDigit;

                // Ask the used to answer incomming call.
                textBoxDetails.Text =
                    "Source : \t" + (String.IsNullOrEmpty(_inComingCall.SrcAddress) ? "Unknown" : _inComingCall.SrcAddress.Trim()) + "\r\n" +
                    (String.IsNullOrEmpty(_inComingCall.From.Trim()) ? "" : "From : \t" + _inComingCall.From.Trim() + "\r\n") +
                    (String.IsNullOrEmpty(_inComingCall.FromContact.Trim()) ? "" : "Contact : \t" + _inComingCall.FromContact.Trim() + "\r\n\r\n") +
                    (String.IsNullOrEmpty(_contactName) ? "" : "Contact : \t" + _contactName + "\r\n\r\n");
            }
        }

        /// <summary>
        /// On DTMF digits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Call_OnDtmfDigit(object sender, Param.OnDtmfDigitParam e)
        {
            UISync.Execute(() =>
            {
                textBoxDetails.Text += (String.IsNullOrEmpty(e.Digit) ? "" : e.Digit);
            });
        }

        /// <summary>
        /// On call state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Call_OnCallState(object sender, Param.CallStateParam e)
        {
            UISync.Execute(() =>
            {
                // If call is disconnected.
                if ((e.State == Nequeo.Net.PjSip.InviteSessionState.PJSIP_INV_STATE_DISCONNECTED) ||
                    (e.State == Nequeo.Net.PjSip.InviteSessionState.PJSIP_INV_STATE_NULL))
                {
                    // Hangup.
                    EnableHangupCall();

                    // Stop the play if not already stopped.
                    if (_playerStarted)
                    {
                        StopPlayer();
                    }

                    try
                    {
                        // Add the call information.
                        Param.CallInfoParam info = e.CallInfo;
                        info.IncomingOutgoing = true;
                        info.ContactName = _contactName;
                        info.FromTo = (!String.IsNullOrEmpty(info.FromTo) ? info.FromTo.Trim(new char[] { '<' }).Trim(new char[] { '>' }) : "");
                        info.Contact = (!String.IsNullOrEmpty(info.Contact) ? info.Contact.Trim(new char[] { '<' }).Trim(new char[] { '>' }) : "");
                        _inOutCalls.Add(info);

                        // Add to the in out view.
                        // Create a new list item.
                        ListViewItem item = new ListViewItem(info.ContactName, 0);
                        item.Name = info.FromTo + "|" + _inComingCall.Call.ID;
                        item.SubItems.Add(info.Date.ToShortDateString() + " " + info.Date.ToShortTimeString());
                        item.SubItems.Add((info.IncomingOutgoing ? "Incoming" : "Outgoing"));
                        item.SubItems.Add(info.FromTo);
                        item.SubItems.Add(info.TotalDuration.ToString());
                        item.SubItems.Add(info.ConnectDuration.ToString());

                        // Add the item.
                        _callsView.Items.Add(item);
                    }
                    catch { }

                    // Close the window.
                    _callEnded?.Invoke();
                }
            });
        }

        /// <summary>
        /// On call media state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Call_OnCallMediaState(object sender, Param.CallMediaStateParam e)
        {
            // Suspend?
            e.Suspend = _suspended;
        }

        /// <summary>
        /// On player end of file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Call_OnPlayerEndOfFile(object sender, EventArgs e)
        {
            // Start recording the message.
            if (_autoAnswerStarted && !String.IsNullOrEmpty(_autoAnswerRecordingPath))
            {
                UISync.Execute(() =>
                {
                    try
                    {
                        // Start the recorder.
                        _inComingCall.Call.StartAutoAnswerRecorder(_autoAnswerRecordingPath);

                        // Create the auto answer recording timer.
                        _autoAnswerRecordingTimer = new System.Threading.Timer(AutoAnswerRecordingTimeout, null,
                            new TimeSpan(0, 0, _messageBankWaitTime),
                            new TimeSpan(0, 0, _messageBankWaitTime));
                    }
                    catch { }
                });
            }
        }

        /// <summary>
        /// Auto answer timeout.
        /// </summary>
        /// <param name="state">The timer state object.</param>
        private void AutoAnswerTimeout(object state)
        {
            UISync.Execute(() =>
            {
                // Stop the timer.
                if (_autoAnswerTimer != null)
                {
                    try
                    {
                        // Stop the auto answer timer.
                        _autoAnswerTimer.Dispose();
                        _autoAnswerTimer = null;
                    }
                    catch { }
                }

                // Enable auto call.
                EnableAutoCallCall();

                // Answer the call.
                AnswerCall();

                // If a auto answer file exists.
                if (!String.IsNullOrEmpty(_autoAnswerFilePath))
                {
                    try
                    {
                        if (_inComingCall != null)
                        {
                            try
                            {
                                // Start playing the sound.
                                _inComingCall.Call.PlaySoundFile(_autoAnswerFilePath);

                                // Auto answer.
                                _autoAnswerStarted = true;
                            }
                            catch { }
                        }
                    }
                    catch (Exception)
                    {
                        _autoAnswerStarted = false;

                        // Stop the auto answer.
                        StopAutoAnswer();
                    }
                }
            });
        }

        /// <summary>
        /// Auto answer recording timeout.
        /// </summary>
        /// <param name="state">The timer state object.</param>
        private void AutoAnswerRecordingTimeout(object state)
        {
            UISync.Execute(() =>
            {
                // Stop the timer.
                if (_autoAnswerRecordingTimer != null)
                {
                    try
                    {
                        // Stop the auto answer timer.
                        _autoAnswerRecordingTimer.Dispose();
                        _autoAnswerRecordingTimer = null;
                    }
                    catch { }
                }

                try
                {
                    // Stop the recorder.
                    _inComingCall.Call.StopAutoAnswerRecorder();
                }
                catch { }

                try
                {
                    // Close the window
                    Close();
                }
                catch { }
            });
        }

        /// <summary>
        /// Answer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAnswer_Click(object sender, EventArgs e)
        {
            AnswerIncomingCall();
        }

        /// <summary>
        /// Answer the incoming call.
        /// </summary>
        private void AnswerIncomingCall()
        {
            // Suspended.
            _suspended = false;

            EnableAnswerCall();

            // Answer the call.
            AnswerCall();

            // Stop the auto answer.
            StopAutoAnswer();
        }

        /// <summary>
        /// Enable answer call.
        /// </summary>
        private void EnableAnswerCall()
        {
            _hasAction = true;
            buttonAnswer.Enabled = false;
            buttonHangup.Enabled = true;
            buttonSendToMessageBank.Enabled = false;
            buttonAddToConferenceCall.Enabled = false;
            buttonTransfer.Enabled = false;
            groupBoxDigits.Enabled = true;
            checkBoxSuspend.Enabled = true;
        }

        /// <summary>
        /// Stop the player.
        /// </summary>
        private void StopPlayer()
        {
            try
            {
                // Cleanup the player.
                if (_player != null)
                {
                    _player.Stop();
                    _player.Dispose();
                    _playerStarted = false;
                }
            }
            catch { }
        }

        /// <summary>
        /// Answer the call.
        /// </summary>
        private void AnswerCall()
        {
            if (_inComingCall != null)
            {
                try
                {
                    _inComingCall.Call.Answer();
                }
                catch { }
            }

            StopPlayer();
        }

        /// <summary>
        /// Enable hangup call.
        /// </summary>
        private void EnableHangupCall()
        {
            _hasAction = true;
            buttonAnswer.Enabled = false;
            buttonHangup.Enabled = false;
            buttonSendToMessageBank.Enabled = false;
            buttonAddToConferenceCall.Enabled = false;
            groupBoxDigits.Enabled = false;
            buttonTransfer.Enabled = false;
            checkBoxSuspend.Enabled = false;
        }

        /// <summary>
        /// Enable auto call call.
        /// </summary>
        private void EnableAutoCallCall()
        {
            _hasAction = true;
            buttonAnswer.Enabled = false;
            buttonHangup.Enabled = true;
            buttonSendToMessageBank.Enabled = false;
            buttonAddToConferenceCall.Enabled = false;
            buttonTransfer.Enabled = false;
            groupBoxDigits.Enabled = false;
            checkBoxSuspend.Enabled = false;
        }

        /// <summary>
        /// Hangup the call.
        /// </summary>
        private void HangupCall()
        {
            if (_inComingCall != null)
            {
                try
                {
                    _inComingCall.Call.Hangup();
                }
                catch { }
            }

            StopPlayer();
        }

        /// <summary>
        /// Hangup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHangup_Click(object sender, EventArgs e)
        {
            // Suspended.
            _suspended = false;

            EnableHangupCall();

            // Hangup the call.
            HangupCall();

            // Stop the auto answer.
            StopAutoAnswer();
        }

        /// <summary>
        /// Closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InComingCall_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If not in conference call.
            if (!_isConferenceCall)
            {
                if (!_hasAction)
                {
                    if (_inComingCall != null)
                    {
                        try
                        {
                            // Hangup.
                            _inComingCall.Call.Hangup();
                        }
                        catch { }
                    }
                }
            }

            StopPlayer();

            // Stop the auto answer.
            StopAutoAnswer();

            // If not in conference call.
            if (!_isConferenceCall)
            {
                if (_inComingCall != null && _inComingCall.Call != null)
                {
                    try
                    {
                        // Dispose of the call.
                        _inComingCall.Call.Dispose();
                        _inComingCall.Call = null;
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Stop the auto answer.
        /// </summary>
        private void StopAutoAnswer()
        {
            _autoAnswerStarted = false;

            try
            {
                // Dispose of the timer.
                if (_autoAnswerTimer != null)
                    _autoAnswerTimer.Dispose();
            }
            catch { }

            try
            {
                // Dispose of the timer.
                if (_autoAnswerRecordingTimer != null)
                    _autoAnswerRecordingTimer.Dispose();
            }
            catch { }

            // If auto answer is on.
            if (_autoAnswerStarted)
            {
                if (_inComingCall != null)
                {
                    try
                    {
                        // Stop playing the sound.
                        _inComingCall.Call.StopSoundFile();
                    }
                    catch { }
                }
            }

            // Stop the answer process.
            _autoAnswerTimer = null;
            _autoAnswerRecordingTimer = null;
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("1");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("2");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("3");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("4");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("5");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("6");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("7");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("8");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("9");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("*");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("0");
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
                if (_inComingCall != null)
                {
                    // Hangup.
                    _inComingCall.Call.DialDtmf("#");
                    textBoxDigits.Text += "#";
                }
            }
            catch (Exception) { }
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
        /// Send to message bank.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSendToMessageBank_Click(object sender, EventArgs e)
        {
            buttonSendToMessageBank.Enabled = false;

            // Create the auto answer timer.
            _autoAnswerTimer = new System.Threading.Timer(AutoAnswerTimeout, null,
                new TimeSpan(0, 0, 2),
                new TimeSpan(0, 0, 2));
        }

        /// <summary>
        /// Add to conference call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddToConferenceCall_Click(object sender, EventArgs e)
        {
            _isConferenceCall = true;

            // Answer the call.
            AnswerIncomingCall();

            // Add to the conference list.
            _voipCall.AddConferenceCallContact(_inComingCall.Call);

            // Add the conference call.
            ListViewItem item = new ListViewItem(_contactName, 0);
            item.Name = _inComingCall.Call.CallID + "|" + _inComingCall.Call.ID;

            // Add the item.
            _conferenceView.Items.Add(item);

            // Close the window.
            Close();
        }

        /// <summary>
        /// Suspend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxSuspend_CheckedChanged(object sender, EventArgs e)
        {
            // Call exists.
            if (_inComingCall != null && _inComingCall.Call != null)
            {
                try
                {
                    // Select the state.
                    switch (checkBoxSuspend.CheckState)
                    {
                        case CheckState.Checked:
                            _inComingCall.Call.StopTransmitting();
                            break;
                        case CheckState.Indeterminate:
                        case CheckState.Unchecked:
                            _inComingCall.Call.StartTransmitting();
                            break;
                    }
                }
                catch (Exception)
                {

                    throw;
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
        /// Transfer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTransfer_Click(object sender, EventArgs e)
        {
            // Call exists.
            if (_inComingCall != null && _inComingCall.Call != null)
            {
                // Open the transfer window.
                UI.TransferList transfer = new TransferList(_contacts, _contactsView);
                transfer.ShowDialog(this);

                // Has a transfer number been selected.
                if (transfer.ContactSelected)
                {
                    // Get the number.
                    string number = transfer.ContactNumber;
                    if (!String.IsNullOrEmpty(number))
                    {
                        try
                        {
                            // Get the transfer number.
                            string destination = SetSipUri(number);

                            // Transfer.
                            _inComingCall.Call.Transfer(destination);
                            _isConferenceCall = true;

                            // Stop the player.
                            StopPlayer();
                        }
                        catch { }

                        // Close
                        Close();
                    }
                }
            }
        }

        /// <summary>
        /// Create the sip uri.
        /// </summary>
        private string SetSipUri(string number)
        {
            string destination = null;

            // If not sip uri.
            if (!number.ToLower().Contains("sip"))
            {
                // If not sip uri.
                if (!number.ToLower().Contains("@"))
                {
                    // Construct the uri
                    destination = "sip:" + number +
                        (String.IsNullOrEmpty(_voipCall.VoIPManager.AccountConnection.SpHost) ? "" : "@" + _voipCall.VoIPManager.AccountConnection.SpHost);
                }
                else
                {
                    // Construct the uri
                    destination = "sip:" + number;
                }
            }
            else
            {
                // If sip uri.
                if (!number.ToLower().Contains("@"))
                {
                    // Construct the uri
                    destination = number +
                        (String.IsNullOrEmpty(_voipCall.VoIPManager.AccountConnection.SpHost) ? "" : "@" + _voipCall.VoIPManager.AccountConnection.SpHost);
                }
                else
                {
                    // Construct the uri
                    destination = number;
                }
            }

            // Return the uri.
            return destination;
        }
    }
}
