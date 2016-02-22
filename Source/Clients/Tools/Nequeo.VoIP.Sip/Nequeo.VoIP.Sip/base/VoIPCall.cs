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
        public VoIPCall()
        {
            // Create the voip manager.
            _voipManager = new VoIPManager();
        }

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
        private string _recordFilename = null;

        private List<Param.CallParam> _conferenceCall = null;

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
        /// Gets the list of callers in the conference.
        /// </summary>
        public Param.CallParam[] ConferenceCall
        {
            get
            {
                if (_conferenceCall != null)
                    return _conferenceCall.ToArray();
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the VoIP manager.
        /// </summary>
        public VoIPManager VoIPManager
        {
            get { return _voipManager; }
        }

        /// <summary>
        /// Start or stop transmitting media to all conference call contacts.
        /// </summary>
        /// <param name="stop">True to stop transmitting; false to start transmitting.</param>
        public void SuspendConferenceCall(bool stop)
        {
            // If the contact.
            if (_conferenceCall != null)
            {
                // Start the conversation between the caller and all other callers.
                foreach (Param.CallParam call in _conferenceCall)
                {
                    try
                    {
                        // Stop transmitting.
                        if (stop)
                        {
                            // Stop transmitting.
                            call.StopTransmitting();
                        }
                        else
                        {
                            // Start transmitting.
                            call.StartTransmitting();
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Add the conference call contact.
        /// </summary>
        /// <param name="caller">The contact to add.</param>
        public void AddConferenceCallContact(Param.CallParam caller)
        {
            if (_conferenceCall == null)
                _conferenceCall = new List<Param.CallParam>();

            // Add the contact.
            if (_conferenceCall != null)
            {
                // Start the conversation between the caller and all other callers.
                foreach (Param.CallParam call in _conferenceCall)
                {
                    try
                    {
                        // The current callers.
                        caller.StartConversation(call);
                    }
                    catch { }
                }

                // Add the contact.
                _conferenceCall.Add(caller);
            }
        }

        /// <summary>
        /// Remove the conference call contact.
        /// </summary>
        /// <param name="id">The call to remove.</param>
        public void RemoveConferenceCallContact(string id)
        {
            // Remove the contact.
            if (_conferenceCall != null)
            {
                Param.CallParam caller = null;
                try
                {
                    // Find the caller.
                    caller = _conferenceCall.First(u => u.ID == id);
                    bool ret = _conferenceCall.Remove(caller);
                    if (!ret) caller = null;
                }
                catch { }

                // If call has been found.
                if (caller != null)
                {
                    // Start the conversation between the caller and all other callers.
                    foreach (Param.CallParam call in _conferenceCall)
                    {
                        try
                        {
                            // The current callers.
                            caller.StopConversation(call);
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Remove all conference call contacts.
        /// </summary>
        public void RemoveAllConferenceCallContacts()
        {
            // Remove the contact.
            if (_conferenceCall != null)
            {
                // Start the conversation between the caller and all other callers.
                foreach (Param.CallParam caller in _conferenceCall)
                {
                    // Start the conversation between the caller and all other callers.
                    foreach (Param.CallParam call in _conferenceCall)
                    {
                        try
                        {
                            // If not the same call.
                            if (call.CallID != caller.CallID)
                            {
                                // The current callers.
                                caller.StopConversation(call);
                            }
                        }
                        catch { }
                    }
                }

                // Clear all the items.
                _conferenceCall.Clear();
                _conferenceCall = null;
            }
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
        /// Send DTMF digits to remote using RFC 2833 payload formats.
        /// </summary>
        /// <param name="call">The current call.</param>
        /// <param name="digits">DTMF string digits to be sent.</param>
        public void DialDtmf(Param.CallParam call, string digits)
        {
            call.DialDtmf(digits);
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
        }

        /// <summary>
        /// Update registration or perform unregistration. Application normally
        /// only needs to call this function if it wants to manually update the
        /// registration or to unregister from the server.
        /// </summary>
        /// <param name="renew">If False, this will start unregistration process.</param>
        public void Registration(bool renew = true)
        {
            _voipManager.Registration(renew);
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
            FindContact(param);

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
                Param.CallParam callInfo = new Param.CallParam(call, _voipManager.MediaManager, _recordFilename);

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
                param.Info = e.RxData.Info;
                param.SrcAddress = e.RxData.SrcAddress;
                param.WholeMsg = e.RxData.WholeMsg;
                param.Call = callInfo;
                param.Contact = FindContact(param);

                // Call the event handler.
                OnIncomingCall?.Invoke(this, param);
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
        /// Find the contact.
        /// </summary>
        /// <param name="param">The whole message.</param>
        /// <returns>The contact; else null.</returns>
        private Net.Sip.Contact FindContact(Param.OnIncomingCallParam param)
        {
            string from = string.Empty;
            string contact = string.Empty;
            Net.Sip.Contact uriContact = null;

            // Get the whole message.
            string[] headers = (String.IsNullOrEmpty(param.WholeMsg) ? new string[] { "" } : param.WholeMsg.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            foreach (string header in headers)
            {
                // Extract from.
                if (header.ToLower().StartsWith("from"))
                {
                    // Get from.
                    string[] fromHeader = header.Split(new char[] { ':' });
                    string combineFrom = String.Join(":", fromHeader.Skip(1));
                    fromHeader = combineFrom.Split(new char[] { ';' });
                    combineFrom = fromHeader[0];
                    from = combineFrom.Replace("<", "").Replace(">", "").Replace("sip:", "").Replace("sips:", "");
                    param.From = from;
                }

                // Extract contact.
                if (header.ToLower().StartsWith("contact"))
                {
                    // Get contact.
                    string[] contactHeader = header.Split(new char[] { ':' });
                    string combineContact = String.Join(":", contactHeader.Skip(1));
                    contactHeader = combineContact.Split(new char[] { ';' });
                    combineContact = contactHeader[0];
                    contact = combineContact.Replace("<", "").Replace(">", "").Replace("sip:", "").Replace("sips:", "");
                    param.FromContact = contact;
                }
            }

            try
            {
                // Find the contact.
                uriContact = _voipManager.FindContact(from);
            }
            catch { }
            
            // Return the contact.
            return uriContact;
        }

        /// <summary>
        /// Find the contact.
        /// </summary>
        /// <param name="param">The whole message.</param>
        private void FindContact(Param.OnInstantMessageParam param)
        {
            // Get from.
            param.From = param.FromUri.Replace("<", "").Replace(">", ""); ;
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
                    if (_voipManager != null)
                        _voipManager.Dispose();

                    if (_conferenceCall != null)
                        RemoveAllConferenceCallContacts();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _voipManager = null;
                _conferenceCall = null;
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
