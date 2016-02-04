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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Net.Sip;

namespace Nequeo.VoIP.Sip
{
    /// <summary>
    /// VoIP call.
    /// </summary>
    public class VoIPCall : IDisposable
    {
        /// <summary>
        /// VoIP call.
        /// </summary>
        /// <param name="accountConnection">Account connection configuration.</param>
        public VoIPCall(AccountConnection accountConnection)
        {
            if (accountConnection == null) throw new ArgumentNullException(nameof(accountConnection));

            // Create the voip manager.
            _voipManager = new VoIPManager(accountConnection);
        }

        /// <summary>
        /// VoIP call.
        /// </summary>
        /// <param name="accountName">The account name or service phone number.</param>
        /// <param name="spHost">The service provider host name or IP address.</param>
        /// <param name="username">The sip username.</param>
        /// <param name="password">The sip password.</param>
        public VoIPCall(string accountName, string spHost, string username, string password)
        {
            _voipManager = new VoIPManager(accountName, spHost, username, password);
        }

        private VoIPManager _voipManager = null;
        private AudioMediaRecorder _recorder = null;
        private string _recordFilename = null;

        /// <summary>
        /// Notify application on incoming call.
        /// </summary>
        public event System.EventHandler<Param.OnIncomingCallParam> OnIncomingCall;

        /// <summary>
        /// Notify application on incoming instant message or pager (i.e. MESSAGE
        /// request) that was received outside call context.
        /// </summary>
        public event System.EventHandler<Param.OnInstantMessageParam> OnInstantMessage;

        /// <summary>
        /// Notify application when registration or unregistration has been initiated.
        /// </summary>
        public event System.EventHandler<Param.OnRegStartedParam> OnRegStarted;

        /// <summary>
        /// Notify application when registration status has changed.
        /// </summary>
        public event System.EventHandler<Param.OnRegStateParam> OnRegState;

        /// <summary>
        /// Gets the VoIP manager.
        /// </summary>
        public VoIPManager VoIPManager
        {
            get { return _voipManager; }
        }

        /// <summary>
        /// Make outgoing call to the specified URI.
        /// </summary>
        /// <param name="callId">An index call id (0 - 3).</param>
        /// <param name="uri">URI to be put in the To header (normally is the same as the target URI).</param>
        /// <param name="recordFilename">The path and filename where the conversation is to be recorded to. Currently ".wav" is supported on all platforms.</param>
        /// <returns>The call information.</returns>
        public Param.CallParam MakeCall(int callId, string uri, string recordFilename = null)
        {
            return _voipManager.MakeCall(callId, uri, recordFilename);
        }

        /// <summary>
        /// Set the audio capture device.
        /// </summary>
        /// <param name="audioCaptureDeviceID">The audio capture device id.</param>
        public void SetAudioCaptureDevice(int audioCaptureDeviceID)
        {
            _voipManager.SetAudioCaptureDevice(audioCaptureDeviceID);
        }

        /// <summary>
        /// Set the audio playback device.
        /// </summary>
        /// <param name="audioPlaybackDeviceID">The audio playback device id.</param>
        public void SetAudioPlaybackDevice(int audioPlaybackDeviceID)
        {
            _voipManager.SetAudioPlaybackDevice(audioPlaybackDeviceID);
        }

        /// <summary>
        /// Set the recording path filename of the file where incoming calls should be recorded to. Currently ".wav" is supported on all platforms.
        /// </summary>
        /// <param name="recordFilename">The path and filename where the conversation is to be recorded to. Currently ".wav" is supported on all platforms.</param>
        /// <remarks>If not in use then set to null or empty string.</remarks>
        public void IncomingCallRecordFilename(string recordFilename)
        {
            _recordFilename = recordFilename;
        }

        /// <summary>
        /// Create the voip call.
        /// </summary>
        public void Create()
        {
            _voipManager.OnIncomingCall += _voipManager_OnIncomingCall;
            _voipManager.OnInstantMessage += _voipManager_OnInstantMessage;
            _voipManager.OnRegStarted += _voipManager_OnRegStarted;
            _voipManager.OnRegState += _voipManager_OnRegState;
            _voipManager.Create();
            _voipManager.Registration(true);
        }

        /// <summary>
        /// Notify application on incoming instant message or pager (i.e. MESSAGE
        /// request) that was received outside call context.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _voipManager_OnInstantMessage(object sender, OnInstantMessageParam e)
        {
            // Send a notification to the call.
            Param.OnInstantMessageParam param = new Param.OnInstantMessageParam();
            param.Info = e.RxData.Info;
            param.SrcAddress = e.RxData.SrcAddress;
            param.WholeMsg = e.RxData.WholeMsg;
            param.ContactUri = e.ContactUri;
            param.ContentType = e.ContentType;
            param.FromUri = e.FromUri;
            param.MsgBody = e.MsgBody;
            param.ToUri = e.ToUri;

            // Call the event handler.
            OnInstantMessage?.Invoke(this, param);
        }

        /// <summary>
        /// Notify application on incoming call.
        /// </summary>
        ///<param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _voipManager_OnIncomingCall(object sender, OnIncomingCallParam e)
        {
            // Is valid call.
            if (e.CallId >= 0)
            {
                // Create a new call.
                Call call = _voipManager.CreateCall(e.CallId);
                call.OnCallState += Call_OnCallState;
                call.OnCallMediaState += Call_OnCallMediaState;

                // Create the call settings.
                CallSetting setting = new CallSetting(true);
                CallOpParam parm = new CallOpParam(true);
                setting.AudioCount = 1;
                parm.Setting = setting;

                // Continue ringing.
                parm.Code = StatusCode.SC_RINGING;
                call.Answer(parm);

                // Send a notification to the call.
                Param.OnIncomingCallParam param = new Param.OnIncomingCallParam();
                param.CallID = e.CallId;
                param.AnswerCall = false;
                param.Info = e.RxData.Info;
                param.SrcAddress = e.RxData.SrcAddress;
                param.WholeMsg = e.RxData.WholeMsg;
                param.Call = new Param.CallParam(call);

                // Call the event handler.
                OnIncomingCall?.Invoke(this, param);

                // Answer call
                if (param.AnswerCall)
                {
                    parm.Code = StatusCode.SC_OK;
                    call.Answer(parm);
                }
                else
                {
                    // Hangup.
                    parm.Code = StatusCode.SC_BUSY_HERE;
                    call.Hangup(parm);
                }
            }
        }

        /// <summary>
        /// Notify application when registration status has changed.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _voipManager_OnRegState(object sender, OnRegStateParam e)
        {
            // Send a notification to the call.
            Param.OnRegStateParam param = new Param.OnRegStateParam();
            param.Info = e.RxData.Info;
            param.SrcAddress = e.RxData.SrcAddress;
            param.WholeMsg = e.RxData.WholeMsg;
            param.Code = e.Code;
            param.Expiration = e.Expiration;
            param.Reason = e.Reason;
            param.Status = e.Status;

            // Call the event handler.
            OnRegState?.Invoke(this, param);
        }

        /// <summary>
        /// Notify application when registration or unregistration has been initiated.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _voipManager_OnRegStarted(object sender, OnRegStartedParam e)
        {
            // Send a notification to the call.
            Param.OnRegStartedParam param = new Param.OnRegStartedParam();
            param.Renew = e.Renew;

            // Call the event handler.
            OnRegStarted?.Invoke(this, param);
        }

        /// <summary>
        /// Notify application when media state in the call has changed.
        /// Normal application would need to implement this callback, e.g.
        /// to connect the call's media to sound device. When ICE is used,
        /// this callback will also be called to report ICE negotiation failure.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void Call_OnCallMediaState(object sender, OnCallMediaStateParam e)
        {
            Nequeo.Net.Sip.CallInfo ci = e.Info;
            if (ci != null)
            {
                // For each media.
                for (int i = 0; i < ci.Media.Length; i++)
                {
                    bool recoderSet = false;

                    // If objects exist.
                    if (ci.Media != null && ci.Media[i] != null && e.CurrentCall != null)
                    {
                        // If audio type.
                        if ((ci.Media[i].Type == Nequeo.Net.Sip.MediaType.PJMEDIA_TYPE_AUDIO) &&
                            (e.CurrentCall.GetMedia((uint)i) != null))
                        {
                            // Get the audio media.
                            AudioMedia audioMedia = (AudioMedia)e.CurrentCall.GetMedia((uint)i);

                            // Connect the call audio media to sound device.
                            audioMedia.StartTransmit(_voipManager.MediaManager.GetPlaybackDeviceMedia());
                            _voipManager.MediaManager.GetCaptureDeviceMedia().StartTransmit(audioMedia);

                            // If recording.
                            if (!recoderSet && !String.IsNullOrEmpty(_recordFilename))
                            {
                                // Get the capture audio device.
                                AudioMedia audioMediaRecord = _voipManager.MediaManager.GetCaptureDeviceMedia();

                                try
                                {
                                    // Create the recorder.
                                    _recorder = new AudioMediaRecorder();
                                    _recorder.CreateRecorder(_recordFilename, 0, 0, 0);
                                    _recorder.StartRecordingConversation(audioMediaRecord, new AudioMedia[] { audioMedia });
                                }
                                catch { }

                                // Set one recorder.
                                recoderSet = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Notify application when call state has changed.
        /// Application may then query the call info to get the
        /// detail call states by calling getInfo() function.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void Call_OnCallState(object sender, OnCallStateParam e)
        {
            Nequeo.Net.Sip.CallInfo ci = e.Info;
            if (ci != null)
            {
                // If call is disconnected.
                if ((ci.State == InviteSessionState.PJSIP_INV_STATE_DISCONNECTED) ||
                    (ci.State == InviteSessionState.PJSIP_INV_STATE_NULL))
                {
                    // If current call.
                    if (e.CurrentCall != null)
                    {
                        // Cleanup the call.
                        e.CurrentCall.Dispose();
                        e.CurrentCall = null;
                    }

                    // If recoder.
                    if (_recorder != null)
                    {
                        try
                        {
                            // Stop the recorder.
                            AudioMedia audioMedia = _voipManager.MediaManager.GetCaptureDeviceMedia();
                            _recorder.Stop(audioMedia);
                        }
                        catch { }

                        // Cleanup the recoder.
                        _recorder.Dispose();
                        _recorder = null;
                    }
                }
            }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_recorder != null)
                        _recorder.Dispose();

                    if (_voipManager != null)
                        _voipManager.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _recorder = null;
                _voipManager = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~VoIPCall()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
