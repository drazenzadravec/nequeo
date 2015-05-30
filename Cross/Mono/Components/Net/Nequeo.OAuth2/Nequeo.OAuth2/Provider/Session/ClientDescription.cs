/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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

namespace Nequeo.Net.OAuth2.Provider.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Nequeo.Net.Core.Messaging;
    using Nequeo.Net.Core.Messaging.Reflection;
    using Nequeo.Net.OAuth2.Framework;
    using Nequeo.Net.OAuth2.Framework.Utility;
    using Nequeo.Net.OAuth2.Framework.Messages;
    using Nequeo.Net.OAuth2.Framework.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Consumer.Session.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;
    using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
    using Nequeo.Net.OAuth2.Provider.Session.Messages;

    /// <summary>
    /// A default implementation of the <see cref="IClientDescription"/> interface.
    /// </summary>
    public class ClientDescription : IClientDescription
    {
        /// <summary>
        /// The client's secret, if any.
        /// </summary>
        private readonly string secret;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDescription"/> class.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <param name="defaultCallback">The default callback.</param>
        /// <param name="clientType">Type of the client.</param>
        public ClientDescription(string secret, Uri defaultCallback, ClientType clientType)
        {
            this.secret = secret;
            this.DefaultCallback = defaultCallback;
            this.ClientType = clientType;
        }

        #region IClientDescription Members

        /// <summary>
        /// Gets the callback to use when an individual authorization request
        /// does not include an explicit callback URI.
        /// </summary>
        /// <value>
        /// An absolute URL; or <c>null</c> if none is registered.
        /// </value>
        public Uri DefaultCallback { get; private set; }

        /// <summary>
        /// Gets the type of the client.
        /// </summary>
        public ClientType ClientType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a non-empty secret is registered for this client.
        /// </summary>
        public virtual bool HasNonEmptySecret
        {
            get { return !string.IsNullOrEmpty(this.secret); }
        }

        /// <summary>
        /// Determines whether a callback URI included in a client's authorization request
        /// is among those allowed callbacks for the registered client.
        /// </summary>
        /// <param name="callback">The absolute URI the client has requested the authorization result be received at.  Never null.</param>
        /// <returns>
        ///   <c>true</c> if the callback URL is allowable for this client; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method may be overridden to allow for several callbacks to match.
        /// </remarks>
        public virtual bool IsCallbackAllowed(Uri callback)
        {
            return EqualityComparer<Uri>.Default.Equals(this.DefaultCallback, callback);
        }

        /// <summary>
        /// Checks whether the specified client secret is correct.
        /// </summary>
        /// <param name="secret">The secret obtained from the client.</param>
        /// <returns><c>true</c> if the secret matches the one in the authorization server's record for the client; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// All string equality checks, whether checking secrets or their hashes,
        /// should be done using <see cref="MessagingUtilities.EqualsConstantTime"/> to mitigate timing attacks.
        /// </remarks>
        public virtual bool IsValidClientSecret(string secret)
        {
            return MessagingUtilities.EqualsConstantTime(secret, this.secret);
        }

        #endregion
    }
}
