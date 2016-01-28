/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// Sip account.
    /// </summary>
    public class Account : IDisposable
    {
        /// <summary>
        /// Sip account.
        /// </summary>
        public Account()
        {
            _pjAccount = new AccountCallback();
            _contacts = new List<Contact>();
        }

        /// <summary>
        /// Sip account.
        /// </summary>
        /// <param name="accountConnection">Account connection configuration.</param>
        public Account(AccountConnection accountConnection)
        {
            if (accountConnection == null) throw new ArgumentNullException(nameof(accountConnection));

            _accountConnection = accountConnection;
            _pjAccount = new AccountCallback();
            _contacts = new List<Contact>();
        }

        private AccountCallback _pjAccount = null;
        private AccountConnection _accountConnection = null;
        private List<Contact> _contacts = null;

        private bool _created = false;
        private const string CreateAccount = "Please create the account first.";
        private const string CreateAccountConnection = "Please create the account connection configuration first.";

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
        /// Gets or sets the account connection configuration.
        /// </summary>
        public AccountConnection AccountConnection
        {
            get { return _accountConnection; }
            set { _accountConnection = value; }
        }

        /// <summary>
        /// Gets or sets the pj account.
        /// </summary>
        internal AccountCallback PjAccount
        {
            get { return _pjAccount; }
            set { _pjAccount = value; }
        }

        /// <summary>
        /// Gets the list of contacts.
        /// </summary>
        internal List<Contact> Contacts
        {
            get { return _contacts; }
        }

        /// <summary>
        /// Get all contacts.
        /// </summary>
        /// <returns>A contact array.</returns>
        public Contact[] GetAllContacts()
        {
            // If account created.
            if (_created)
            {
                // Get all buddies.
                List<Contact> contacts = new List<Contact>();
                pjsua2.BuddyVector buddies = _pjAccount.enumBuddies();

                // For each code found.
                for (int i = 0; i < buddies.Count; i++)
                {
                    pjsua2.BuddyInfo buddyInfo = buddies[i].getInfo();

                    // Find the matching buddy.
                    for (int j = 0; j < _contacts.Count; j++)
                    {
                        if (_contacts[j].ContactUri == buddyInfo.uri)
                        {
                            // Add the contact.
                            contacts.Add(_contacts[j]);
                            break;
                        }
                    }
                }

                // Return the contact list.
                return contacts.ToArray();
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Remove the contact from the list.
        /// </summary>
        /// <param name="contact">The contact to remove.</param>
        public void RemoveContact(Contact contact)
        {
            // If account created.
            if (_created)
            {
                // Get all buddies.
                pjsua2.BuddyVector buddies = _pjAccount.enumBuddies();

                // For each code found.
                for (int i = 0; i < buddies.Count; i++)
                {
                    pjsua2.BuddyInfo buddyInfo = buddies[i].getInfo();

                    // Find the buddy.
                    if (contact.ContactUri == buddyInfo.uri)
                    {
                        try
                        {
                            // Remove the buddy.
                            _pjAccount.removeBuddy(buddies[i]);
                        }
                        catch { }
                    }

                    // Find the matching buddy.
                    for (int j = 0; j < _contacts.Count; j++)
                    {
                        // Find the buddy.
                        if (_contacts[j].ContactUri == buddyInfo.uri)
                        {
                            try
                            {
                                // Remove the contact.
                                _contacts[i].Dispose();
                                _contacts.Remove(contact);
                            }
                            catch { }
                            break;
                        }
                    }
                }
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Find the contact.
        /// </summary>
        /// <param name="uri">The contact unique uri.</param>
        /// <returns>The contact.</returns>
        Contact FindContact(string uri)
        {
            // If account created.
            if (_created)
            {
                Contact contact = null;

                // Find the buddy.
                pjsua2.Buddy buddy = _pjAccount.findBuddy(uri);

                // Find the matching buddy.
                for (int j = 0; j < _contacts.Count; j++)
                {
                    // Find the buddy.
                    if (_contacts[j].ContactUri == uri)
                    {
                        try
                        {
                            // Get the contact.
                            contact = _contacts[j];
                        }
                        catch { }
                        break;
                    }
                }

                // Return the contact.
                return contact;
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Create the account.
        /// </summary>
        public void Create()
        {
            // If an account connection exists.
            if (_accountConnection != null)
            {
                // If not created.
                if (!_created)
                {
                    // Create the callback
                    _pjAccount.OnIncomingCall += _pjAccount_OnIncomingCall;
                    _pjAccount.OnIncomingSubscribe += _pjAccount_OnIncomingSubscribe;
                    _pjAccount.OnInstantMessage += _pjAccount_OnInstantMessage;
                    _pjAccount.OnInstantMessageStatus += _pjAccount_OnInstantMessageStatus;
                    _pjAccount.OnMwiInfo += _pjAccount_OnMwiInfo;
                    _pjAccount.OnTypingIndication += _pjAccount_OnTypingIndication;
                    _pjAccount.OnRegStarted += _pjAccount_OnRegStarted;
                    _pjAccount.OnRegState += _pjAccount_OnRegState;

                    // Account has been created.
                    _pjAccount.Create(_accountConnection);
                    _created = true;
                }
            }
            else
                throw new Exception(CreateAccountConnection);
        }

        /// <summary>
        /// Update registration or perform unregistration. Application normally
        /// only needs to call this function if it wants to manually update the
        /// registration or to unregister from the server.
        /// </summary>
        /// <param name="renew">If False, this will start unregistration process.</param>
        public void Registration(bool renew = true)
        {
            // If account created.
            if (_created)
            {
                // Register or unregister the account.
                _pjAccount.setRegistration(renew);
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Get the account ID or index associated with this account.
        /// </summary>
        /// <returns>The account ID or index.</returns>
        public int GetAccountId()
        {
            // If account created.
            if (_created)
            {
                // Get account ID.
                return _pjAccount.getId();
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Is the account still valid.
        /// </summary>
        /// <returns>True if valid: else false.</returns>
        public bool IsValid()
        {
            // If account created.
            if (_created)
            {
                // Is account valid.
                return _pjAccount.isValid();
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Get the account info.
        /// </summary>
        /// <returns>The account info.</returns>
        public AccountInfo GetAccountInfo()
        {
            // If account created.
            if (_created)
            {
                // Get the account info.
                using (pjsua2.AccountInfo pjAccountInfo = _pjAccount.getInfo())
                {
                    AccountInfo accountInfo = new AccountInfo();
                    accountInfo.Id = pjAccountInfo.id;
                    accountInfo.IsDefault = pjAccountInfo.isDefault;
                    accountInfo.OnlineStatus = pjAccountInfo.onlineStatus;
                    accountInfo.OnlineStatusText = pjAccountInfo.onlineStatusText;
                    accountInfo.RegExpiresSec = pjAccountInfo.regExpiresSec;
                    accountInfo.RegIsActive = pjAccountInfo.regIsActive;
                    accountInfo.RegIsConfigured = pjAccountInfo.regIsConfigured;
                    accountInfo.RegLastErr = pjAccountInfo.regLastErr;
                    accountInfo.RegStatus = AccountInfo.GetStatusCodeEx(pjAccountInfo.regStatus);
                    accountInfo.RegStatusText = pjAccountInfo.regStatusText;
                    accountInfo.Uri = pjAccountInfo.uri;

                    // Return the account info.
                    return accountInfo;
                }
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Set the online status.
        /// </summary>
        /// <param name="presenceStatus">The presence status.</param>
        public void SetOnlineStatus(PresenceStatus presenceStatus)
        {
            // If account created.
            if (_created)
            {
                // Set the online state.
                using (pjsua2.PresenceStatus pjPresenceStatus = new pjsua2.PresenceStatus())
                {
                    // Set the presence.
                    pjPresenceStatus.activity = PresenceStatus.GetActivity(presenceStatus.Activity);
                    pjPresenceStatus.status = PresenceStatus.GetContactStatus(presenceStatus.Status);
                    pjPresenceStatus.note = presenceStatus.Note;
                    pjPresenceStatus.rpidId = presenceStatus.RpidId;
                    pjPresenceStatus.statusText = presenceStatus.StatusText;

                    // Set the online status.
                    _pjAccount.setOnlineStatus(pjPresenceStatus);
                }
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Get the media manager.
        /// </summary>
        /// <returns>The media manager.</returns>
        public MediaManager GetMediaManager()
        {
            // If account created.
            if (_created)
            {
                // Get the audio device manager.
                pjsua2.AudDevManager pjAudDevManager = _pjAccount.GetAudDevManager();
                MediaManager mediaManager = new MediaManager(pjAudDevManager);
                return mediaManager;
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Get all supported codecs in the system.
        /// </summary>
        /// <returns>The supported codecs in the system.</returns>
        public CodecInfo[] GetCodecInfo()
        {
            // If account created.
            if (_created)
            {
                List<CodecInfo> codecList = new List<CodecInfo>();
                pjsua2.CodecInfoVector codecs = _pjAccount.GetCodecInfo();

                // For each code found.
                for (int i = 0; i < codecs.Count; i++)
                {
                    CodecInfo codec = new CodecInfo();
                    codec.CodecId = codecs[i].codecId;
                    codec.Description = codecs[i].desc;
                    codec.Priority = codecs[i].priority;
                    codecList.Add(codec);
                }

                // Return the code list.
                return codecList.ToArray();
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Add audio media device to the application.
        /// </summary>
        /// <param name="audioMedia">The audio media device.</param>
        public void AddAudioCaptureDevice(AudioMedia audioMedia)
        {
            // If account created.
            if (_created)
            {
                _pjAccount.AddAudioMedia(audioMedia.PjAudioMedia);
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Add audio media device to the application.
        /// </summary>
        /// <param name="audioMedia">The audio media device.</param>
        public void AddAudioPlaybackDevice(AudioMedia audioMedia)
        {
            // If account created.
            if (_created)
            {
                _pjAccount.AddAudioMedia(audioMedia.PjAudioMedia);
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Get the number of active media ports.
        /// </summary>
        /// <returns>The number of active ports.</returns>
        public uint MediaActivePorts()
        {
            // If account created.
            if (_created)
            {
                return _pjAccount.MediaActivePorts();
            }
            else
                throw new Exception(CreateAccount);
        }

        /// <summary>
        /// Notify application on incoming call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnIncomingCall(object sender, OnIncomingCallParam e)
        {
            OnIncomingCall?.Invoke(this, e);
        }

        /// <summary>
        /// Notification when incoming SUBSCRIBE request is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnIncomingSubscribe(object sender, OnIncomingSubscribeParam e)
        {
            OnIncomingSubscribe?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application on incoming instant message or pager (i.e. MESSAGE
        /// request) that was received outside call context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnInstantMessage(object sender, OnInstantMessageParam e)
        {
            OnInstantMessage?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application about the delivery status of outgoing pager/instant
        /// message(i.e.MESSAGE) request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnInstantMessageStatus(object sender, OnInstantMessageStatusParam e)
        {
            OnInstantMessageStatus?.Invoke(this, e);
        }

        /// <summary>
        /// Notification about MWI (Message Waiting Indication) status change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnMwiInfo(object sender, OnMwiInfoParam e)
        {
            OnMwiInfo?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application about typing indication.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnTypingIndication(object sender, OnTypingIndicationParam e)
        {
            OnTypingIndication?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application when registration or unregistration has been initiated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnRegStarted(object sender, OnRegStartedParam e)
        {
            OnRegStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Notify application when registration status has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjAccount_OnRegState(object sender, OnRegStateParam e)
        {
            OnRegState?.Invoke(this, e);
        }

        /// <summary>
        /// Account callbacks.
        /// </summary>
        internal class AccountCallback : pjsua2.Account
        {
            /// <summary>
            /// Account callbacks.
            /// </summary>
            public AccountCallback()
            {
                // Create an endpoint.
                _pjEndpoint = new pjsua2.Endpoint();
                _pjEpConfig = new pjsua2.EpConfig();

                //_pjEpConfig.logConfig.filename = @"C:\Temp\PJSIP\log.txt";

                // Start the application.
                StartUp();
            }

            private bool _disposed = false;

            private pjsua2.Endpoint _pjEndpoint = null;
            private pjsua2.EpConfig _pjEpConfig = null;

            private pjsua2.AccountConfig _pjAccountConfig = null;
            private pjsua2.AccountRegConfig _pjAccountRegConfig = null;
            private pjsua2.AccountSipConfig _pjAccountSipConfig = null;
            private pjsua2.AccountCallConfig _pjAccountCallConfig = null;
            private pjsua2.AccountMediaConfig _pjAccountMediaConfig = null;
            private pjsua2.TransportConfig _pjTransportConfig = null;
            private pjsua2.AccountMwiConfig _pjAccountMwiConfig = null;
            private pjsua2.AccountPresConfig _pjAccountPresConfig = null;
            private pjsua2.AccountNatConfig _pjAccountNatConfig = null;
            private pjsua2.AccountVideoConfig _pjAccountVideoConfig = null;

            private pjsua2.TransportConfig _transportConfig_UDP = null;
            private pjsua2.TransportConfig _transportConfig_UDP6 = null;
            private pjsua2.TransportConfig _transportConfig_TCP = null;
            private pjsua2.TransportConfig _transportConfig_TCP6 = null;
            private pjsua2.TransportConfig _transportConfig_TLS = null;
            private pjsua2.TransportConfig _transportConfig_TLS6 = null;

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
            /// Start the application.
            /// </summary>
            public void StartUp()
            {
                // Create endpoint data.
                _pjEndpoint.libCreate();
                _pjEndpoint.libInit(_pjEpConfig);
            }

            /// <summary>
            /// Create the account.
            /// </summary>
            /// <param name="accountConnection">The account connection configuration.</param>
            public void Create(AccountConnection accountConnection)
            {
                _transportConfig_UDP = new pjsua2.TransportConfig();
                _transportConfig_UDP6 = new pjsua2.TransportConfig();
                _transportConfig_TCP = new pjsua2.TransportConfig();
                _transportConfig_TCP6 = new pjsua2.TransportConfig();
                _transportConfig_TLS = new pjsua2.TransportConfig();
                _transportConfig_TLS6 = new pjsua2.TransportConfig();

                // Assign the transport.
                _transportConfig_TLS.tlsConfig.method = pjsua2.pjsip_ssl_method.PJSIP_TLSV1_METHOD;
                _transportConfig_TLS.tlsConfig.verifyServer = false;
                _transportConfig_TLS.tlsConfig.verifyClient = false;

                _transportConfig_TLS6.tlsConfig.method = pjsua2.pjsip_ssl_method.PJSIP_TLSV1_METHOD;
                _transportConfig_TLS6.tlsConfig.verifyServer = false;
                _transportConfig_TLS6.tlsConfig.verifyClient = false;

                // Create the client transport.
                _pjEndpoint.transportCreate(pjsua2.pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, _transportConfig_UDP);
                _pjEndpoint.transportCreate(pjsua2.pjsip_transport_type_e.PJSIP_TRANSPORT_UDP6, _transportConfig_UDP6);
                _pjEndpoint.transportCreate(pjsua2.pjsip_transport_type_e.PJSIP_TRANSPORT_TCP, _transportConfig_TCP);
                _pjEndpoint.transportCreate(pjsua2.pjsip_transport_type_e.PJSIP_TRANSPORT_TCP6, _transportConfig_TCP6);
                _pjEndpoint.transportCreate(pjsua2.pjsip_transport_type_e.PJSIP_TRANSPORT_TLS, _transportConfig_TLS);
                _pjEndpoint.transportCreate(pjsua2.pjsip_transport_type_e.PJSIP_TRANSPORT_TLS6, _transportConfig_TLS6);

                // Start.
                _pjEndpoint.libStart();

                // Create account config.
                _pjAccountConfig = new pjsua2.AccountConfig();
                _pjAccountRegConfig = new pjsua2.AccountRegConfig();
                _pjAccountSipConfig = new pjsua2.AccountSipConfig();
                _pjAccountCallConfig = new pjsua2.AccountCallConfig();
                _pjAccountMediaConfig = new pjsua2.AccountMediaConfig();
                _pjAccountMwiConfig = new pjsua2.AccountMwiConfig();
                _pjAccountPresConfig = new pjsua2.AccountPresConfig();
                _pjTransportConfig = new pjsua2.TransportConfig();
                _pjAccountNatConfig = new pjsua2.AccountNatConfig();
                _pjAccountVideoConfig = new pjsua2.AccountVideoConfig();

                // Set the account options.
                _pjAccountConfig.idUri = "sip:" + accountConnection.AccountName + "@" + accountConnection.SpHost + ":" + accountConnection.SpPort.ToString();
                _pjAccountConfig.priority = accountConnection.Priority;

                // Set the registration options.
                _pjAccountRegConfig.registrarUri = "sip:" + accountConnection.SpHost + ":" + accountConnection.SpPort.ToString();
                _pjAccountRegConfig.dropCallsOnFail = accountConnection.DropCallsOnFail;
                _pjAccountRegConfig.registerOnAdd = accountConnection.RegisterOnAdd;
                _pjAccountRegConfig.retryIntervalSec = accountConnection.RetryIntervalSec;
                _pjAccountRegConfig.timeoutSec = accountConnection.TimeoutSec;
                _pjAccountRegConfig.firstRetryIntervalSec = accountConnection.FirstRetryIntervalSec;
                _pjAccountRegConfig.unregWaitSec = accountConnection.UnregWaitSec;
                _pjAccountRegConfig.delayBeforeRefreshSec = accountConnection.DelayBeforeRefreshSec;

                // Set the media options.
                _pjTransportConfig.port = accountConnection.MediaTransportPort; // RTP
                _pjTransportConfig.portRange = accountConnection.MediaTransportPortRange; // RTP
                _pjAccountMediaConfig.ipv6Use = AccountInfo.GetIPv6UseEx(accountConnection.IPv6Use);
                _pjAccountMediaConfig.srtpUse = AccountInfo.GetSrtpUseEx(accountConnection.SRTPUse);
                _pjAccountMediaConfig.srtpSecureSignaling = AccountInfo.GetSRTPSecureSignalingEx(accountConnection.SRTPSecureSignaling);
                _pjAccountMediaConfig.transportConfig = _pjTransportConfig;

                // Set the sip options.
                _pjAccountSipConfig.authCreds = (accountConnection.AuthenticateCredentials != null ? accountConnection.AuthenticateCredentials.GetAuthCreds() : null);

                // Set the call options.
                _pjAccountCallConfig.holdType = pjsua2.pjsua_call_hold_type.PJSUA_CALL_HOLD_TYPE_RFC3264;
                _pjAccountCallConfig.prackUse = pjsua2.pjsua_100rel_use.PJSUA_100REL_NOT_USED;
                _pjAccountCallConfig.timerUse = pjsua2.pjsua_sip_timer_use.PJSUA_SIP_TIMER_OPTIONAL;
                _pjAccountCallConfig.timerMinSESec = accountConnection.TimerMinSESec;
                _pjAccountCallConfig.timerSessExpiresSec = accountConnection.TimerSessExpiresSec;

                // Set the message waiting indicatoin options.
                _pjAccountMwiConfig.enabled = accountConnection.MessageWaitingIndication;
                _pjAccountMwiConfig.expirationSec = accountConnection.MWIExpirationSec;

                // Set the presence options.
                _pjAccountPresConfig.publishEnabled = accountConnection.PublishEnabled;
                _pjAccountPresConfig.publishQueue = accountConnection.PublishQueue;
                _pjAccountPresConfig.publishShutdownWaitMsec = accountConnection.PublishShutdownWaitMsec;

                // Set the nat options.
                _pjAccountNatConfig.iceNoRtcp = accountConnection.NoIceRtcp;
                _pjAccountNatConfig.iceEnabled = accountConnection.IceEnabled;

                // Set the video options.
                _pjAccountVideoConfig.defaultCaptureDevice = (int)pjsua2.pjmedia_vid_dev_std_index.PJMEDIA_VID_DEFAULT_CAPTURE_DEV;
                _pjAccountVideoConfig.defaultRenderDevice = (int)pjsua2.pjmedia_vid_dev_std_index.PJMEDIA_VID_DEFAULT_RENDER_DEV;
                _pjAccountVideoConfig.rateControlBandwidth = accountConnection.VideoRateControlBandwidth;
                _pjAccountVideoConfig.autoTransmitOutgoing = accountConnection.VideoAutoTransmit;
                _pjAccountVideoConfig.autoShowIncoming = accountConnection.VideoAutoShow;

                // Assign the account config.
                _pjAccountConfig.regConfig = _pjAccountRegConfig;
                _pjAccountConfig.sipConfig = _pjAccountSipConfig;
                _pjAccountConfig.callConfig = _pjAccountCallConfig;
                _pjAccountConfig.mediaConfig = _pjAccountMediaConfig;
                _pjAccountConfig.mwiConfig = _pjAccountMwiConfig;
                _pjAccountConfig.presConfig = _pjAccountPresConfig;
                _pjAccountConfig.natConfig = _pjAccountNatConfig;
                _pjAccountConfig.videoConfig = _pjAccountVideoConfig;

                // Create the account.
                create(_pjAccountConfig, accountConnection.IsDefault);
            }

            /// <summary>
            /// Get the number of active media ports.
            /// </summary>
            /// <returns>The number of active ports.</returns>
            public uint MediaActivePorts()
            {
                return _pjEndpoint.mediaActivePorts();
            }

            /// <summary>
            /// Get all supported codecs in the system.
            /// </summary>
            /// <returns>The supported codecs in the system.</returns>
            public pjsua2.CodecInfoVector GetCodecInfo()
            {
                return _pjEndpoint.codecEnum();
            }

            /// <summary>
            /// Get the audio deveice manager.
            /// </summary>
            /// <returns>The audio device manager.</returns>
            public pjsua2.AudDevManager GetAudDevManager()
            {
                return _pjEndpoint.audDevManager();
            }

            /// <summary>
            /// Add audio media device to the application.
            /// </summary>
            /// <param name="audioMedia">The audio media device.</param>
            public void AddAudioMedia(pjsua2.AudioMedia audioMedia)
            {
                _pjEndpoint.mediaAdd(audioMedia);
            }

            /// <summary>
            /// Notify application on incoming call.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onIncomingCall(pjsua2.OnIncomingCallParam prm)
            {
                // Create the callback.
                OnIncomingCallParam onIncomingCallParam = new OnIncomingCallParam();
                onIncomingCallParam.RxData = new SipRxData();

                if (prm != null)
                {
                    onIncomingCallParam.CallId = prm.callId;

                    if (prm.rdata != null)
                    {
                        onIncomingCallParam.RxData.Info = prm.rdata.info;
                        onIncomingCallParam.RxData.SrcAddress = prm.rdata.srcAddress;
                        onIncomingCallParam.RxData.WholeMsg = prm.rdata.wholeMsg;
                    }
                }

                // Invoke the call back event.
                OnIncomingCall?.Invoke(this, onIncomingCallParam);
            }

            /// <summary>
            /// Notification when incoming SUBSCRIBE request is received. Application
            /// may use this callback to authorize the incoming subscribe request
            /// (e.g.ask user permission if the request should be granted).
            ///
            /// If this callback is not implemented, all incoming presence subscription
            /// requests will be accepted.
            ///
            /// If this callback is implemented, application has several choices on
            /// what to do with the incoming request:
            ///	- it may reject the request immediately by specifying non-200 class
            ///    final response in the IncomingSubscribeParam.code parameter.
            ///  - it may immediately accept the request by specifying 200 as the
            /// IncomingSubscribeParam.code parameter. This is the default value if
            ///	  application doesn't set any value to the IncomingSubscribeParam.code
            ///	  parameter.In this case, the library will automatically send NOTIFY
            ///	  request upon returning from this callback.
            ///  - it may delay the processing of the request, for example to request
            /// user permission whether to accept or reject the request. In this
            ///    case, the application MUST set the IncomingSubscribeParam.code
            /// argument to 202, then IMMEDIATELY calls presNotify() with
            /// state PJSIP_EVSUB_STATE_PENDING and later calls presNotify()
            ///    again to accept or reject the subscription request.
            ///
            /// Any IncomingSubscribeParam.code other than 200 and 202 will be treated
            /// as 200.
            ///
            /// Application MUST return from this callback immediately (e.g.it must
            /// not block in this callback while waiting for user confirmation).
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onIncomingSubscribe(pjsua2.OnIncomingSubscribeParam prm)
            {
                // Create the callback.
                OnIncomingSubscribeParam onIncomingSubscribeParam = new OnIncomingSubscribeParam();
                onIncomingSubscribeParam.RxData = new SipRxData();
                onIncomingSubscribeParam.TxOption = new SipTxOption();
                onIncomingSubscribeParam.TxOption.Headers = new SipHeaderVector();
                onIncomingSubscribeParam.TxOption.MultipartContentType = new SipMediaType();
                onIncomingSubscribeParam.TxOption.MultipartParts = new SipMultipartPartVector();

                if (prm != null)
                {
                    onIncomingSubscribeParam.Code = AccountInfo.GetStatusCodeEx(prm.code);
                    onIncomingSubscribeParam.FromUri = prm.fromUri;
                    onIncomingSubscribeParam.Reason = prm.reason;

                    if (prm.rdata != null)
                    {
                        onIncomingSubscribeParam.RxData.Info = prm.rdata.info;
                        onIncomingSubscribeParam.RxData.SrcAddress = prm.rdata.srcAddress;
                        onIncomingSubscribeParam.RxData.WholeMsg = prm.rdata.wholeMsg;
                    }

                    if (prm.txOption != null)
                    {
                        onIncomingSubscribeParam.TxOption.ContentType = prm.txOption.contentType;
                        onIncomingSubscribeParam.TxOption.MsgBody = prm.txOption.msgBody;
                        onIncomingSubscribeParam.TxOption.TargetUri = prm.txOption.targetUri;

                        if (prm.txOption.headers != null)
                        {
                            onIncomingSubscribeParam.TxOption.Headers.Count = prm.txOption.headers.Count;
                            onIncomingSubscribeParam.TxOption.Headers.SipHeaders = new SipHeader[prm.txOption.headers.Count];
                            for (int i = 0; i < prm.txOption.headers.Count; i++)
                            {
                                onIncomingSubscribeParam.TxOption.Headers.SipHeaders[i].Name = prm.txOption.headers[i].hName;
                                onIncomingSubscribeParam.TxOption.Headers.SipHeaders[i].Value = prm.txOption.headers[i].hValue;
                            }
                        }

                        if (prm.txOption.multipartContentType != null)
                        {
                            onIncomingSubscribeParam.TxOption.MultipartContentType.SubType = prm.txOption.multipartContentType.subType;
                            onIncomingSubscribeParam.TxOption.MultipartContentType.Type = prm.txOption.multipartContentType.type;
                        }

                        if (prm.txOption.multipartParts != null)
                        {
                            onIncomingSubscribeParam.TxOption.MultipartParts.Count = prm.txOption.multipartParts.Count;
                            onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts = new SipMultipartPart[prm.txOption.multipartParts.Count];
                            for (int i = 0; i < prm.txOption.multipartParts.Count; i++)
                            {
                                onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].Body = prm.txOption.multipartParts[i].body;
                                onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].ContentType = new SipMediaType();
                                onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].Headers = new SipHeaderVector();

                                if (prm.txOption.multipartParts[i].contentType != null)
                                {
                                    onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].ContentType.SubType = prm.txOption.multipartParts[i].contentType.subType;
                                    onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].ContentType.Type = prm.txOption.multipartParts[i].contentType.type;
                                }

                                if (prm.txOption.multipartParts[i].headers != null)
                                {
                                    onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.Count = prm.txOption.multipartParts[i].headers.Count;
                                    onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders = new SipHeader[prm.txOption.multipartParts[i].headers.Count];
                                    for(int j = 0; j< prm.txOption.multipartParts[i].headers.Count; j++)
                                    {
                                        onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Name = prm.txOption.multipartParts[i].headers[j].hName;
                                        onIncomingSubscribeParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Value = prm.txOption.multipartParts[i].headers[j].hValue;
                                    }
                                }
                            }
                        }
                    }
                }

                // Invoke the call back event.
                OnIncomingSubscribe?.Invoke(this, onIncomingSubscribeParam);
            }

            /// <summary>
            /// Notify application on incoming instant message or pager (i.e. MESSAGE
            /// request) that was received outside call context.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onInstantMessage(pjsua2.OnInstantMessageParam prm)
            {
                // Create the callback.
                OnInstantMessageParam onInstantMessageParam = new OnInstantMessageParam();
                onInstantMessageParam.RxData = new SipRxData();

                if (prm != null)
                {
                    onInstantMessageParam.ContactUri = prm.contactUri;
                    onInstantMessageParam.ContentType = prm.contentType;
                    onInstantMessageParam.FromUri = prm.fromUri;
                    onInstantMessageParam.MsgBody = prm.msgBody;
                    onInstantMessageParam.ToUri = prm.toUri;

                    if (prm.rdata != null)
                    {
                        onInstantMessageParam.RxData.Info = prm.rdata.info;
                        onInstantMessageParam.RxData.SrcAddress = prm.rdata.srcAddress;
                        onInstantMessageParam.RxData.WholeMsg = prm.rdata.wholeMsg;
                    }
                }

                // Invoke the call back event.
                OnInstantMessage?.Invoke(this, onInstantMessageParam);
            }

            /// <summary>
            /// Notify application about the delivery status of outgoing pager/instant
            /// message(i.e.MESSAGE) request.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onInstantMessageStatus(pjsua2.OnInstantMessageStatusParam prm)
            {
                // Create the callback.
                OnInstantMessageStatusParam onInstantMessageStatusParam = new OnInstantMessageStatusParam();
                onInstantMessageStatusParam.RxData = new SipRxData();

                if (prm != null)
                {
                    onInstantMessageStatusParam.Code = AccountInfo.GetStatusCodeEx(prm.code);
                    onInstantMessageStatusParam.MsgBody = prm.msgBody;
                    onInstantMessageStatusParam.Reason = prm.reason;
                    onInstantMessageStatusParam.ToUri = prm.toUri;

                    if (prm.rdata != null)
                    {
                        onInstantMessageStatusParam.RxData.Info = prm.rdata.info;
                        onInstantMessageStatusParam.RxData.SrcAddress = prm.rdata.srcAddress;
                        onInstantMessageStatusParam.RxData.WholeMsg = prm.rdata.wholeMsg;
                    }
                }

                // Invoke the call back event.
                OnInstantMessageStatus?.Invoke(this, onInstantMessageStatusParam);
            }

            /// <summary>
            /// Notification about MWI (Message Waiting Indication) status change.
            /// This callback can be called upon the status change of the
            /// SUBSCRIBE request(for example, 202/Accepted to SUBSCRIBE is received)
            /// or when a NOTIFY reqeust is received.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onMwiInfo(pjsua2.OnMwiInfoParam prm)
            {
                // Create the callback.
                OnMwiInfoParam onMwiInfoParam = new OnMwiInfoParam();
                onMwiInfoParam.RxData = new SipRxData();

                if (prm != null)
                {
                    onMwiInfoParam.State = OnMwiInfoParam.GetSubscriptionState(prm.state);

                    if (prm.rdata != null)
                    {
                        onMwiInfoParam.RxData.Info = prm.rdata.info;
                        onMwiInfoParam.RxData.SrcAddress = prm.rdata.srcAddress;
                        onMwiInfoParam.RxData.WholeMsg = prm.rdata.wholeMsg;
                    }
                }

                // Invoke the call back event.
                OnMwiInfo?.Invoke(this, onMwiInfoParam);
            }

            /// <summary>
            /// Notify application about typing indication.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onTypingIndication(pjsua2.OnTypingIndicationParam prm)
            {
                // Create the callback.
                OnTypingIndicationParam onTypingIndicationParam = new OnTypingIndicationParam();
                onTypingIndicationParam.RxData = new SipRxData();

                if (prm != null)
                {
                    onTypingIndicationParam.ContactUri = prm.contactUri;
                    onTypingIndicationParam.FromUri = prm.fromUri;
                    onTypingIndicationParam.IsTyping = prm.isTyping;
                    onTypingIndicationParam.ToUri = prm.toUri;

                    if (prm.rdata != null)
                    {
                        onTypingIndicationParam.RxData.Info = prm.rdata.info;
                        onTypingIndicationParam.RxData.SrcAddress = prm.rdata.srcAddress;
                        onTypingIndicationParam.RxData.WholeMsg = prm.rdata.wholeMsg;
                    }
                }

                // Invoke the call back event.
                OnTypingIndication?.Invoke(this, onTypingIndicationParam);
            }

            /// <summary>
            /// Notify application when registration or unregistration has been
            /// initiated. Note that this only notifies the initial registration
            /// and unregistration. Once registration session is active, subsequent
            /// refresh will not cause this callback to be called.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onRegStarted(pjsua2.OnRegStartedParam prm)
            {
                OnRegStartedParam onRegStartedParam = new OnRegStartedParam();

                if (prm != null)
                {
                    onRegStartedParam.Renew = prm.renew;
                }

                // Invoke the call back event.
                OnRegStarted?.Invoke(this, onRegStartedParam);
            }

            /// <summary>
            /// Notify application when registration status has changed.
            /// Application may then query the account info to get the
            /// registration details.
            /// </summary>
            /// <param name="prm">Callback parameter.</param>
            public override void onRegState(pjsua2.OnRegStateParam prm)
            {
                OnRegStateParam onRegStateParam = new OnRegStateParam();
                onRegStateParam.RxData = new SipRxData();

                if (prm != null)
                {
                    onRegStateParam.Code = AccountInfo.GetStatusCodeEx(prm.code);
                    onRegStateParam.Expiration = prm.expiration;
                    onRegStateParam.Reason = prm.reason;
                    onRegStateParam.Status = prm.status;

                    if (prm.rdata != null)
                    {
                        onRegStateParam.RxData.Info = prm.rdata.info;
                        onRegStateParam.RxData.SrcAddress = prm.rdata.srcAddress;
                        onRegStateParam.RxData.WholeMsg = prm.rdata.wholeMsg;
                    }
                }

                // Invoke the call back event.
                OnRegState?.Invoke(this, onRegStateParam);
            }

            /// <summary>
            /// Dispose.
            /// </summary>
            public override void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;

                    if (_pjAccountRegConfig != null)
                        _pjAccountRegConfig.Dispose();

                    if (_pjAccountSipConfig != null)
                        _pjAccountSipConfig.Dispose();

                    if (_pjAccountCallConfig != null)
                        _pjAccountCallConfig.Dispose();

                    if (_pjTransportConfig != null)
                        _pjTransportConfig.Dispose();

                    if (_pjAccountMediaConfig != null)
                        _pjAccountMediaConfig.Dispose();

                    if (_pjAccountMwiConfig != null)
                        _pjAccountMwiConfig.Dispose();

                    if (_pjAccountPresConfig != null)
                        _pjAccountPresConfig.Dispose();

                    if (_pjAccountNatConfig != null)
                        _pjAccountNatConfig.Dispose();

                    if (_pjAccountVideoConfig != null)
                        _pjAccountVideoConfig.Dispose();

                    if (_pjAccountConfig != null)
                        _pjAccountConfig.Dispose();

                    if (_transportConfig_UDP != null)
                        _transportConfig_UDP.Dispose();

                    if (_transportConfig_UDP6 != null)
                        _transportConfig_UDP6.Dispose();

                    if (_transportConfig_TCP != null)
                        _transportConfig_TCP.Dispose();

                    if (_transportConfig_TCP6 != null)
                        _transportConfig_TCP6.Dispose();

                    if (_transportConfig_TLS != null)
                        _transportConfig_TLS.Dispose();

                    if (_transportConfig_TLS6 != null)
                        _transportConfig_TLS6.Dispose();

                    try
                    {
                        if (_pjEpConfig != null)
                            _pjEpConfig.Dispose();
                    }
                    catch { }

                    if (_pjEndpoint != null)
                    {
                        try
                        {
                            _pjEndpoint.libStopWorkerThreads();
                        }
                        catch { }

                        try
                        {
                            _pjEndpoint.libDestroy();
                            _pjEndpoint.Dispose();
                        }
                        catch { }
                    }

                    base.Dispose();
                }

                _pjAccountConfig = null;
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

                if (_contacts != null)
                {
                    // Dispose of the contact.
                    for (int i = 0; i < _contacts.Count; i++)
                    {
                        try
                        {
                            // Dispose of the contact.
                            _contacts[i].Dispose();
                        }
                        catch { }
                    }

                    _contacts.Clear();
                    _contacts = null;
                }

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_pjAccount != null)
                        _pjAccount.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _pjAccount = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Account()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
