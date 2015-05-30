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
    /// The OAuth client for the user-agent flow, providing services for installed apps
    /// and in-browser Javascript widgets.
    /// </summary>
    public class AuthAgentClient : UserAgentClient
	{
        /// <summary>
        /// The OAuth client for the user-agent flow, providing services for installed apps
        /// and in-browser Javascript widgets.
        /// </summary>
        /// <param name="consumerIdentifier">The unique consumer identifier.</param>
        /// <param name="consumerSecret">The unique consumer secret.</param>
        /// <param name="userAuthorizeEndpointUri">The user authorise endpoint uri.</param>
        /// <param name="tokenEndpointUri">The token endpoint uri.</param>
        public AuthAgentClient(string consumerIdentifier, string consumerSecret, Uri userAuthorizeEndpointUri, Uri tokenEndpointUri)
            : base(
                new AuthorizationServerDescription()
                {
                    TokenEndpoint = tokenEndpointUri,
                    AuthorizationEndpoint = userAuthorizeEndpointUri
                },
                consumerIdentifier,
                consumerSecret)
        {
            _consumerKey = consumerIdentifier;
            _consumerSecret = consumerSecret;
            _userAuthorizeUri = userAuthorizeEndpointUri;
            _accessTokenUri = tokenEndpointUri;

            // Validate all the parameters
            Validate();
        }

        private string _consumerKey = string.Empty;
        private string _consumerSecret = string.Empty;
        private Uri _userAuthorizeUri = null;
        private Uri _accessTokenUri = null;

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
