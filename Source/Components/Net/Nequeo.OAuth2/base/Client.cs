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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Nequeo.Net.Core.Messaging;
using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Provider.Session;
using Nequeo.Net.OAuth2.Provider;
using Nequeo.Net.OAuth2.Consumer.Session;
using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.ChannelElements;

using Nequeo.Cryptography.Parser;
using Nequeo.Cryptography.Signing;
using Nequeo.Cryptography;
using Nequeo.Security;

namespace Nequeo.Net.OAuth2
{
    /// <summary>
    /// OAuth client
    /// </summary>
    public partial class AuthClient
	{
        /// <summary>
        /// OAuth client.
        /// </summary>
        /// <param name="consumerIdentifier">The unique consumer identifier.</param>
        /// <param name="consumerSecret">The unique consumer secret.</param>
        /// <param name="userAuthorizeEndpointUri">The user authorise endpoint uri.</param>
        /// <param name="tokenEndpointUri">The token endpoint uri.</param>
        public AuthClient(string consumerIdentifier, string consumerSecret, Uri userAuthorizeEndpointUri, Uri tokenEndpointUri)
        {
            _consumerKey = consumerIdentifier;
            _consumerSecret = consumerSecret;
            _userAuthorizeUri = userAuthorizeEndpointUri;
            _accessTokenUri = tokenEndpointUri;

            // Validate all the parameters
            Validate();

            OnCreated();
        }

        private string _consumerKey = string.Empty;
        private string _consumerSecret = string.Empty;
        private Uri _userAuthorizeUri = null;
        private Uri _accessTokenUri = null;

        private AuthWebClient _authWebClient = null;
        private AuthAgentClient _authAgentClient = null;

        /// <summary>
        /// Gets sets the unique consumer identifier
        /// </summary>
        public string ConsumerIdentifier
        {
            get { return _consumerKey; }
            set { _consumerKey = value; }
        }

        /// <summary>
        /// Gets sets the consumer secret used for signing
        /// </summary>
        public string ConsumerSecret
        {
            get { return _consumerSecret; }
            set { _consumerSecret = value; }
        }

        /// <summary>
        /// Gets sets the user authorize endpoint URI
        /// </summary>
        public Uri UserAuthorizeEndpointUri
        {
            get { return _userAuthorizeUri; }
            set { _userAuthorizeUri = value; }
        }

        /// <summary>
        /// Gets sets the token endpoint URI
        /// </summary>
        public Uri TokenEndpointUri
        {
            get { return _accessTokenUri; }
            set { _accessTokenUri = value; }
        }

        /// <summary>
        /// Get the web client.
        /// </summary>
        /// <returns>An OAuth 2.0 consumer designed for web applications.</returns>
        public AuthWebClient GetWebClient()
        {
            _authWebClient = new AuthWebClient(_consumerKey, _consumerSecret, _userAuthorizeUri, _accessTokenUri);
            return _authWebClient;
        }

        /// <summary>
        /// Get the user agent client.
        /// </summary>
        /// <returns>The OAuth client for the user-agent flow, providing services for installed apps
        /// and in-browser Javascript widgets.</returns>
        public AuthAgentClient GetAgentClient()
        {
            _authAgentClient = new AuthAgentClient(_consumerKey, _consumerSecret, _userAuthorizeUri, _accessTokenUri);
            return _authAgentClient;
        }

        /// <summary>
        /// Validate all the parameters
        /// </summary>
        private void Validate()
        {
            if (String.IsNullOrEmpty(_consumerKey)) throw new ArgumentNullException("consumerIdentifier");
            if (String.IsNullOrEmpty(_consumerSecret)) throw new ArgumentNullException("consumerSecret");
            if (_userAuthorizeUri == null) throw new ArgumentNullException("userAuthorizeEndpointUri");
            if (_accessTokenUri == null) throw new ArgumentNullException("tokenEndpointUri");
        }
	}
}
