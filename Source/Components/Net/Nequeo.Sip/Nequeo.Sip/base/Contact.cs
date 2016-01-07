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
    /// Sip account contact.
    /// </summary>
    public class Contact : IDisposable
    {
        /// <summary>
        /// Sip account contact.
        /// </summary>
        /// <param name="account">The Sip account.</param>
        /// <param name="contactConnection">The Sip contact connection configuration.</param>
        public Contact(Account account, ContactConnection contactConnection)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (contactConnection == null) throw new ArgumentNullException(nameof(contactConnection));

            _account = account;
            _contactConnection = contactConnection;
            _pjContact = new ContactCallback(account.PjAccount);

            // Add this contact.
            _contactUri = contactConnection.Uri;
            _account.Contacts.Add(this);
        }

        private string _contactUri = null;
        private Account _account;
        private ContactCallback _pjContact = null;
        private ContactConnection _contactConnection = null;

        private bool _created = false;
        private const string CreateContact = "Please create the contact first.";
        private const string CreateContactConnection = "Please create the contact connection configuration first.";

        /// <summary>
        /// Gets the contact unique uri.
        /// </summary>
        internal string ContactUri
        {
            get { return _contactUri; }
        }

        /// <summary>
        /// Gets the account this contact belongs to.
        /// </summary>
        public Account Account
        {
            get { return _account; }
        }

        /// <summary>
        /// Gets the contact details.
        /// </summary>
        public ContactConnection ContactConnection
        {
            get { return _contactConnection; }
        }

        /// <summary>
        /// Notify application when the contact state has changed.
        /// Application may then query the contact info to get the details.
        /// </summary>
        public event System.EventHandler<ContactInfo> OnContactState;

        /// <summary>
        /// Create the contact.
        /// </summary>
        public void Create()
        {
            // If an contact connection exists.
            if (_contactConnection != null)
            {
                // If not created.
                if (!_created)
                {
                    // Create the callback.
                    _pjContact.OnBuddyState += _pjContact_OnBuddyState;
                    _pjContact.Create(_contactConnection);
                    _created = true;
                }
            }
            else
                throw new Exception(CreateContactConnection);
        }

        /// <summary>
        /// Get detailed contact info.
        /// </summary>
        /// <returns>The contact info.</returns>
        public ContactInfo GetInfo()
        {
            // If contact created.
            if (_created)
            {
                // Get the contact info.
                using (pjsua2.BuddyInfo pjContactInfo = _pjContact.getInfo())
                {
                    ContactInfo contactInfo = new ContactInfo();
                    contactInfo.Info = pjContactInfo.contact;
                    contactInfo.PresMonitorEnabled = pjContactInfo.presMonitorEnabled;
                    contactInfo.SubState = OnMwiInfoParam.GetSubscriptionState(pjContactInfo.subState);
                    contactInfo.SubStateName = pjContactInfo.subStateName;
                    contactInfo.SubTermCode = AccountInfo.GetStatusCodeEx(pjContactInfo.subTermCode);
                    contactInfo.SubTermReason = pjContactInfo.subTermReason;
                    contactInfo.Uri = pjContactInfo.uri;

                    if (pjContactInfo.presStatus != null)
                    {
                        contactInfo.PresenceStatus = new PresenceStatus();
                        contactInfo.PresenceStatus.Activity = PresenceStatus.GetActivityEx(pjContactInfo.presStatus.activity);
                        contactInfo.PresenceStatus.Note = pjContactInfo.presStatus.note;
                        contactInfo.PresenceStatus.RpidId = pjContactInfo.presStatus.rpidId;
                        contactInfo.PresenceStatus.Status = PresenceStatus.GetContactStatusEx(pjContactInfo.presStatus.status);
                        contactInfo.PresenceStatus.StatusText = pjContactInfo.presStatus.statusText;
                    }

                    // Return the contact info.
                    return contactInfo;
                }
            }
            else
                throw new Exception(CreateContact);
        }

        /// <summary>
        /// Is the contact still valid.
        /// </summary>
        /// <returns>True if valid: else false.</returns>
        public bool IsValid()
        {
            // If contact created.
            if (_created)
            {
                // Is contact valid.
                return _pjContact.isValid();
            }
            else
                throw new Exception(CreateContact);
        }

        /// <summary>
        /// Update the presence information for the contact. Although the library
        /// periodically refreshes the presence subscription for all contacts,
        /// some application may want to refresh the contact's presence subscription
        /// immediately, and in this case it can use this function to accomplish
        /// this.
        ///
        /// Note that the contact's presence subscription will only be initiated
        /// if presence monitoring is enabled for the contact. See
        /// subscribePresence() for more info. Also if presence subscription for
        /// the contact is already active, this function will not do anything.
        ///
        /// Once the presence subscription is activated successfully for the contact,
        /// application will be notified about the contact's presence status in the
        /// OnContactState() callback.
        /// </summary>
        public void UpdatePresence()
        {
            // If contact created.
            if (_created)
            {
                // Contact update.
                _pjContact.updatePresence();
            }
            else
                throw new Exception(CreateContact);
        }

        /// <summary>
        /// Enable or disable contact's presence monitoring. Once contact's presence is
        /// subscribed, application will be informed about contact's presence status
        /// changed via OnContactState() callback.
        /// </summary>
        /// <param name="subscribe">Specify true to activate presence subscription.</param>
        public void SubscribePresence(bool subscribe)
        {
            // If contact created.
            if (_created)
            {
                // Contact subscribe.
                _pjContact.subscribePresence(subscribe);
            }
            else
                throw new Exception(CreateContact);
        }

        /// <summary>
        /// Send instant messaging outside dialog, using this contact's specified
        /// account for route set and authentication.
        /// </summary>
        /// <param name="sendInstantMessageParam">Sending instant message parameter.</param>
        public void SendInstantMessage(SendInstantMessage sendInstantMessageParam)
        {
            // If contact created.
            if (_created)
            {
                pjsua2.SendInstantMessageParam prm = new pjsua2.SendInstantMessageParam();
                prm.content = sendInstantMessageParam.Content;
                prm.contentType = sendInstantMessageParam.ContentType;

                if (sendInstantMessageParam.TxOption != null)
                {
                    prm.txOption.contentType = sendInstantMessageParam.TxOption.ContentType;
                    prm.txOption.msgBody = sendInstantMessageParam.TxOption.MsgBody;
                    prm.txOption.targetUri = sendInstantMessageParam.TxOption.TargetUri;

                    if (sendInstantMessageParam.TxOption.Headers != null && sendInstantMessageParam.TxOption.Headers.SipHeaders != null)
                    {
                        prm.txOption.headers = new pjsua2.SipHeaderVector();
                        for (int i = 0; i < sendInstantMessageParam.TxOption.Headers.Count; i++)
                        {
                            pjsua2.SipHeader header = new pjsua2.SipHeader();
                            header.hName = sendInstantMessageParam.TxOption.Headers.SipHeaders[i].Name;
                            header.hValue = sendInstantMessageParam.TxOption.Headers.SipHeaders[i].Value;
                            prm.txOption.headers.Add(header);
                        }
                    }

                    if (sendInstantMessageParam.TxOption.MultipartContentType != null)
                    {
                        prm.txOption.multipartContentType = new pjsua2.SipMediaType();
                        prm.txOption.multipartContentType.subType = sendInstantMessageParam.TxOption.MultipartContentType.SubType;
                        prm.txOption.multipartContentType.type = sendInstantMessageParam.TxOption.MultipartContentType.Type;
                    }

                    if (sendInstantMessageParam.TxOption.MultipartParts != null && sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts != null)
                    {
                        prm.txOption.multipartParts = new pjsua2.SipMultipartPartVector();
                        for (int i = 0; i < sendInstantMessageParam.TxOption.MultipartParts.Count; i++)
                        {
                            pjsua2.SipMultipartPart mulPart = new pjsua2.SipMultipartPart();
                            mulPart.body = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Body;

                            SipMediaType mediaType = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].ContentType;
                            mulPart.contentType = new pjsua2.SipMediaType();
                            mulPart.contentType.subType = mediaType.SubType;
                            mulPart.contentType.type = mediaType.Type;

                            if (sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers != null &&
                                sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders != null)
                            {
                                mulPart.headers = new pjsua2.SipHeaderVector();
                                for (int j = 0; j < sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.Count; j++)
                                {
                                    pjsua2.SipHeader header = new pjsua2.SipHeader();
                                    header.hName = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Name;
                                    header.hValue = sendInstantMessageParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Value;
                                    mulPart.headers.Add(header);
                                }
                            }

                            prm.txOption.multipartParts.Add(mulPart);
                        }
                    }
                }

                // Send message.
                _pjContact.sendInstantMessage(prm);
            }
            else
                throw new Exception(CreateContact);
        }

        /// <summary>
        /// Send typing indication outside dialog.
        /// </summary>
        /// <param name="sendTypingIndicationParam">Sending instant message parameter.</param>
        public void SendTypingIndication(SendTypingIndication sendTypingIndicationParam)
        {
            // If contact created.
            if (_created)
            {
                pjsua2.SendTypingIndicationParam prm = new pjsua2.SendTypingIndicationParam();
                prm.isTyping = sendTypingIndicationParam.IsTyping;
                if(sendTypingIndicationParam.TxOption != null)
                {
                    prm.txOption.contentType = sendTypingIndicationParam.TxOption.ContentType;
                    prm.txOption.msgBody = sendTypingIndicationParam.TxOption.MsgBody;
                    prm.txOption.targetUri = sendTypingIndicationParam.TxOption.TargetUri;

                    if (sendTypingIndicationParam.TxOption.Headers != null && sendTypingIndicationParam.TxOption.Headers.SipHeaders != null)
                    {
                        prm.txOption.headers = new pjsua2.SipHeaderVector();
                        for (int i = 0; i < sendTypingIndicationParam.TxOption.Headers.Count; i++)
                        {
                            pjsua2.SipHeader header = new pjsua2.SipHeader();
                            header.hName = sendTypingIndicationParam.TxOption.Headers.SipHeaders[i].Name;
                            header.hValue = sendTypingIndicationParam.TxOption.Headers.SipHeaders[i].Value;
                            prm.txOption.headers.Add(header);
                        }
                    }

                    if (sendTypingIndicationParam.TxOption.MultipartContentType != null)
                    {
                        prm.txOption.multipartContentType = new pjsua2.SipMediaType();
                        prm.txOption.multipartContentType.subType = sendTypingIndicationParam.TxOption.MultipartContentType.SubType;
                        prm.txOption.multipartContentType.type = sendTypingIndicationParam.TxOption.MultipartContentType.Type;
                    }

                    if (sendTypingIndicationParam.TxOption.MultipartParts != null && sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts != null)
                    {
                        prm.txOption.multipartParts = new pjsua2.SipMultipartPartVector();
                        for (int i = 0; i < sendTypingIndicationParam.TxOption.MultipartParts.Count; i++)
                        {
                            pjsua2.SipMultipartPart mulPart = new pjsua2.SipMultipartPart();
                            mulPart.body = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Body;

                            SipMediaType mediaType = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].ContentType;
                            mulPart.contentType = new pjsua2.SipMediaType();
                            mulPart.contentType.subType = mediaType.SubType;
                            mulPart.contentType.type = mediaType.Type;

                            if (sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers != null &&
                                sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders != null)
                            {
                                mulPart.headers = new pjsua2.SipHeaderVector();
                                for (int j = 0; j < sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.Count; j++)
                                {
                                    pjsua2.SipHeader header = new pjsua2.SipHeader();
                                    header.hName = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Name;
                                    header.hValue = sendTypingIndicationParam.TxOption.MultipartParts.SipMultipartParts[i].Headers.SipHeaders[j].Value;
                                    mulPart.headers.Add(header);
                                }
                            }

                            prm.txOption.multipartParts.Add(mulPart);
                        }
                    }
                }

                // Send typing.
                _pjContact.sendTypingIndication(prm);
            }
            else
                throw new Exception(CreateContact);
        }

        /// <summary>
        /// Notify application when the contact state has changed.
        /// Application may then query the contact info to get the details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pjContact_OnBuddyState(object sender, EventArgs e)
        {
            ContactInfo contactInfo = new ContactInfo();
            contactInfo.PresenceStatus = new PresenceStatus();

            // Get the contact info.
            using (pjsua2.BuddyInfo pjContactInfo = _pjContact.getInfo())
            {
                contactInfo.Info = pjContactInfo.contact;
                contactInfo.PresMonitorEnabled = pjContactInfo.presMonitorEnabled;
                contactInfo.SubState = OnMwiInfoParam.GetSubscriptionState(pjContactInfo.subState);
                contactInfo.SubStateName = pjContactInfo.subStateName;
                contactInfo.SubTermCode = AccountInfo.GetStatusCodeEx(pjContactInfo.subTermCode);
                contactInfo.SubTermReason = pjContactInfo.subTermReason;
                contactInfo.Uri = pjContactInfo.uri;

                if (pjContactInfo.presStatus != null)
                {
                    contactInfo.PresenceStatus.Activity = PresenceStatus.GetActivityEx(pjContactInfo.presStatus.activity);
                    contactInfo.PresenceStatus.Note = pjContactInfo.presStatus.note;
                    contactInfo.PresenceStatus.RpidId = pjContactInfo.presStatus.rpidId;
                    contactInfo.PresenceStatus.Status = PresenceStatus.GetContactStatusEx(pjContactInfo.presStatus.status);
                    contactInfo.PresenceStatus.StatusText = pjContactInfo.presStatus.statusText;
                }
            }

            // Invoke the call back event.
            OnContactState?.Invoke(this, contactInfo);
        }

        /// <summary>
        /// Contact callbacks.
        /// </summary>
        internal class ContactCallback : pjsua2.Buddy
        {
            /// <summary>
            /// Contact callbacks.
            /// </summary>
            /// <param name="pjAccount">Account.</param>
            public ContactCallback(Account.AccountCallback pjAccount)
            {
                _pjAccount = pjAccount;
            }

            private bool _disposed = false;

            private Account.AccountCallback _pjAccount = null;
            private pjsua2.BuddyConfig _pjBuddyConfig = null;

            /// <summary>
            /// Notify application when the buddy state has changed.
            /// Application may then query the buddy info to get the details.
            /// </summary>
            public event System.EventHandler OnBuddyState;

            /// <summary>
            /// Notify application when the buddy state has changed.
            /// Application may then query the buddy info to get the details.
            /// </summary>
            public override void onBuddyState()
            {
                // Invoke the call back event.
                OnBuddyState?.Invoke(this, new EventArgs());
            }

            /// <summary>
            /// Create the contact.
            /// </summary>
            /// <param name="contactConnection">The contact connection configuration.</param>
            public void Create(ContactConnection contactConnection)
            {
                _pjBuddyConfig = new pjsua2.BuddyConfig();
                _pjBuddyConfig.subscribe = contactConnection.Subscribe;
                _pjBuddyConfig.uri = contactConnection.Uri;

                // Create the contact.
                create(_pjAccount, _pjBuddyConfig);
            }

            /// <summary>
            /// Dispose.
            /// </summary>
            public override void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;

                    if (_pjBuddyConfig != null)
                        _pjBuddyConfig.Dispose();

                    base.Dispose();
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
                    if (_pjContact != null)
                        _pjContact.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _pjContact = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Contact()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
