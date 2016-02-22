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

using Nequeo.Net.PjSip;

namespace Nequeo.VoIP.PjSip
{
    /// <summary>
    /// VoIP manager.
    /// </summary>
    public class VoIPManager : IDisposable
    {
        /// <summary>
        /// VoIP manager.
        /// </summary>
        public VoIPManager()
        {
            _account = new Account(new AccountConnection());

            // Get the media manager.
            _mediaManager = _account.GetMediaManager();
        }

        /// <summary>
        /// VoIP manager.
        /// </summary>
        /// <param name="accountConnection">Account connection configuration.</param>
        public VoIPManager(AccountConnection accountConnection)
        {
            if (accountConnection == null) throw new ArgumentNullException(nameof(accountConnection));

            _account = new Account(accountConnection);

            // Get the media manager.
            _mediaManager = _account.GetMediaManager();
        }

        /// <summary>
        /// VoIP manager.
        /// </summary>
        /// <param name="accountName">The account name or service phone number.</param>
        /// <param name="spHost">The service provider host name or IP address.</param>
        /// <param name="username">The sip username.</param>
        /// <param name="password">The sip password.</param>
        public VoIPManager(string accountName, string spHost, string username, string password)
        {
            _account = new Account(new AccountConnection(accountName, spHost, username, password));

            // Get the media manager.
            _mediaManager = _account.GetMediaManager();
        }

        private bool _created = false;
        private Account _account = null;
        private MediaManager _mediaManager = null;

        /// <summary>
        /// Notify application when the contact state has changed.
        /// Application may then query the contact info to get the details.
        /// </summary>
        public event System.EventHandler<ContactInfo> OnContactState;

        /// <summary>
        /// Notify application on incoming call.
        /// </summary>
        public event System.EventHandler<OnIncomingCallParam> OnIncomingCall;

        /// <summary>
        /// Notification when incoming SUBSCRIBE request is received.
        /// </summary>
        public event System.EventHandler<OnIncomingSubscribeParam> OnIncomingSubscribe;

        /// <summary>
        /// Notify application on incoming instant message or pager (i.e. MESSAGE
        /// request) that was received outside call context.
        /// </summary>
        public event System.EventHandler<OnInstantMessageParam> OnInstantMessage;

        /// <summary>
        /// Notify application about the delivery status of outgoing pager/instant
        /// message(i.e.MESSAGE) request.
        /// </summary>
        public event System.EventHandler<OnInstantMessageStatusParam> OnInstantMessageStatus;

        /// <summary>
        /// Notification about MWI (Message Waiting Indication) status change.
        /// </summary>
        public event System.EventHandler<OnMwiInfoParam> OnMwiInfo;

        /// <summary>
        /// Notify application about typing indication.
        /// </summary>
        public event System.EventHandler<OnTypingIndicationParam> OnTypingIndication;

        /// <summary>
        /// Notify application when registration or unregistration has been initiated.
        /// </summary>
        public event System.EventHandler<OnRegStartedParam> OnRegStarted;

        /// <summary>
        /// Notify application when registration status has changed.
        /// </summary>
        public event System.EventHandler<OnRegStateParam> OnRegState;

        /// <summary>
        /// Gets the account connection configuration.
        /// </summary>
        public AccountConnection AccountConnection
        {
            get { return _account.AccountConnConfig; }
        }

        /// <summary>
        /// Gets the media manager.
        /// </summary>
        public MediaManager MediaManager
        {
            get { return _mediaManager; }
        }

        /// <summary>
        /// Create the account.
        /// </summary>
        public void Create()
        {
            // If not created.
            if (!_created)
            {
                _created = true;

                // Attach to the events.
                _account.OnIncomingCall += _account_OnIncomingCall;
                _account.OnIncomingSubscribe += _account_OnIncomingSubscribe;
                _account.OnInstantMessage += _account_OnInstantMessage;
                _account.OnInstantMessageStatus += _account_OnInstantMessageStatus;
                _account.OnMwiInfo += _account_OnMwiInfo;
                _account.OnRegStarted += _account_OnRegStarted;
                _account.OnRegState += _account_OnRegState;
                _account.OnTypingIndication += _account_OnTypingIndication;

                // Create the account.
                _account.Create();
            }
        }

        /// <summary>
        /// Update registration or perform unregistration. Application normally
        /// only needs to call this function if it wants to manually update the
        /// registration or to unregister from the server.
        /// </summary>
        /// <param name="renew">If False, this will start unregistration process.</param>
        public void Registration(bool renew = true)
        {
            // If created.
            if (_created)
            {
                _account.Registration(renew);
            }
            else
                throw new Exception("Create the account first.");
        }

        /// <summary>
        /// Send DTMF digits to remote using RFC 2833 payload formats.
        /// </summary>
        /// <param name="call">The current call.</param>
        /// <param name="digits">DTMF string digits to be sent.</param>
        public void DialDtmf(Param.CallParam call, string digits)
        {
            // If created.
            if (_created)
            {
                call.DialDtmf(digits);
            }
            else
                throw new Exception("Create the account first.");
        }

        /// <summary>
        /// Get the account ID or index associated with this account.
        /// </summary>
        /// <returns>The account ID or index.</returns>
        public int GetAccountId()
        {
            return _account.GetAccountId();
        }

        /// <summary>
        /// Is the account still valid.
        /// </summary>
        /// <returns>True if valid: else false.</returns>
        public bool IsValid()
        {
            return _account.IsValid();
        }

        /// <summary>
        /// Get the account info.
        /// </summary>
        /// <returns>The account info.</returns>
        public AccountInfo GetAccountInfo()
        {
            return _account.GetAccountInfo();
        }

        /// <summary>
        /// Set the online status.
        /// </summary>
        /// <param name="presenceStatus">The presence status.</param>
        public void SetOnlineStatus(PresenceState presenceStatus)
        {
            _account.SetOnlineStatus(presenceStatus);
        }

        /// <summary>
        /// Get the media manager.
        /// </summary>
        /// <returns>The media manager.</returns>
        public MediaManager GetMediaManager()
        {
            return _account.GetMediaManager();
        }

        /// <summary>
        /// Get all supported codecs in the system.
        /// </summary>
        /// <returns>The supported codecs in the system.</returns>
        public CodecInfo[] GetCodecInfo()
        {
            return _account.GetAudioCodecInfo();
        }

        /// <summary>
        /// Add audio media device to the application.
        /// </summary>
        /// <param name="audioMedia">The audio media device.</param>
        public void AddAudioCaptureDevice(AudioMedia audioMedia)
        {
            _account.AddAudioCaptureDevice(audioMedia);
        }

        /// <summary>
        /// Add audio media device to the application.
        /// </summary>
        /// <param name="audioMedia">The audio media device.</param>
        public void AddAudioPlaybackDevice(AudioMedia audioMedia)
        {
            _account.AddAudioPlaybackDevice(audioMedia);
        }

        /// <summary>
        /// Get the number of active media ports.
        /// </summary>
        /// <returns>The number of active ports.</returns>
        public uint MediaActivePorts()
        {
            return _account.MediaActivePorts();
        }

        /// <summary>
        /// Add a new contact.
        /// </summary>
        /// <param name="contactConnection">The contact connection configuration.</param>
        public void AddContact(ContactConnection contactConnection)
        {
            if (contactConnection == null) throw new ArgumentNullException(nameof(contactConnection));

            // Create the contact.
            Contact contact = new Contact(_account, contactConnection);
            contact.OnContactState += Contact_OnContactState;
            contact.Create();
        }

        /// <summary>
        /// Get all contacts.
        /// </summary>
        /// <returns>A contact array.</returns>
        public Contact[] GetAllContacts()
        {
            return _account.GetAllContacts();
        }

        /// <summary>
        /// Find the contact.
        /// </summary>
        /// <param name="uri">The contact unique uri.</param>
        /// <returns>The contact.</returns>
        public Contact FindContact(string uri)
        {
            return _account.FindContact(uri);
        }

        /// <summary>
        /// Remove the contact from the list.
        /// </summary>
        /// <param name="contact">The contact to remove.</param>
        public void RemoveContact(Contact contact)
        {
            _account.RemoveContact(contact);
        }

        /// <summary>
        /// Create a new call.
        /// </summary>
        /// <param name="callId">An index call id (0 - 3).</param>
        /// <returns>A new call instance.</returns>
        public Call CreateCall(int callId)
        {
            // If created.
            if (_created)
            {
                // Create a new call.
                Call call = new Call(_account, callId);
                return call;
            }
            else
                throw new Exception("Create the account first.");
        }

        /// <summary>
        /// Set the audio capture device.
        /// </summary>
        /// <param name="audioCaptureDeviceID">The audio capture device id.</param>
        public void SetAudioCaptureDevice(int audioCaptureDeviceID)
        {
            _mediaManager.SetCaptureDevice(audioCaptureDeviceID);
            _account.AddAudioCaptureDevice(_mediaManager.GetCaptureDeviceMedia());
        }

        /// <summary>
        /// Set the audio playback device.
        /// </summary>
        /// <param name="audioPlaybackDeviceID">The audio playback device id.</param>
        public void SetAudioPlaybackDevice(int audioPlaybackDeviceID)
        {
            _mediaManager.SetPlaybackDevice(audioPlaybackDeviceID);
            _account.AddAudioPlaybackDevice(_mediaManager.GetPlaybackDeviceMedia());
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
            // If created.
            if (_created)
            {
                // Create a new call.
                Call call = new Call(_account, callId);

                // Create the call settings.
                CallSetting setting = new CallSetting(true);
                CallOpParam parm = new CallOpParam(true);
                setting.AudioCount = 1;
                parm.Setting = setting;

                // Make the call.
                call.MakeCall(uri, parm);

                // return the call information.
                Param.CallParam callInfo = new Param.CallParam(call, _mediaManager, recordFilename);
                return callInfo;
            }
            else
                throw new Exception("Create the account first.");
        }

        /// <summary>
        /// Notify application when the contact state has changed.
        /// Application may then query the contact info to get the details.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void Contact_OnContactState(object sender, ContactInfo e)
        {
            OnContactState?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application about typing indication.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnTypingIndication(object sender, OnTypingIndicationParam e)
        {
            OnTypingIndication?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application when registration status has changed.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnRegState(object sender, OnRegStateParam e)
        {
            OnRegState?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application when registration or unregistration has been initiated.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnRegStarted(object sender, OnRegStartedParam e)
        {
            OnRegStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Notification about MWI (Message Waiting Indication) status change.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnMwiInfo(object sender, OnMwiInfoParam e)
        {
            OnMwiInfo?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application about the delivery status of outgoing pager/instant
        /// message(i.e.MESSAGE) request.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnInstantMessageStatus(object sender, OnInstantMessageStatusParam e)
        {
            OnInstantMessageStatus?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application on incoming instant message or pager (i.e. MESSAGE
        /// request) that was received outside call context.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnInstantMessage(object sender, OnInstantMessageParam e)
        {
            OnInstantMessage?.Invoke(this, e);
        }

        /// <summary>
        /// Notification when incoming SUBSCRIBE request is received.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnIncomingSubscribe(object sender, OnIncomingSubscribeParam e)
        {
            OnIncomingSubscribe?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application on incoming call.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _account_OnIncomingCall(object sender, OnIncomingCallParam e)
        {
            OnIncomingCall?.Invoke(this, e);
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
                    if (_mediaManager != null)
                        _mediaManager.Dispose();

                    if (_account != null)
                        _account.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _mediaManager = null;
                _account = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~VoIPManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
