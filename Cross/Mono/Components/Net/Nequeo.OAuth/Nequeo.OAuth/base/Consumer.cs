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
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

using Nequeo.Net.OAuth.Consumer;
using Nequeo.Net.OAuth.Consumer.Session;
using Nequeo.Net.OAuth.Framework;

using Nequeo.Cryptography.Parser;
using Nequeo.Cryptography.Signing;
using Nequeo.Cryptography;
using Nequeo.Security;

namespace Nequeo.Net.OAuth
{
    /// <summary>
    /// OAuth consumer
    /// </summary>
    public partial class AuthConsumer
	{
        /// <summary>
        /// Auth consumer interface implementation.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        /// <param name="callBackUri">The call back URI</param>
        /// <param name="certificate">The certificate issued by the provider; only used when SignatureTypes = RSA_SHA1.</param>
        public AuthConsumer(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, 
            Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri, Uri callBackUri = null, X509Certificate2 certificate = null)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _signatureTypes = signatureTypes;
            _requestTokenUri = requestTokenUri;
            _userAuthorizeUri = userAuthorizeUri;
            _accessTokenUri = accessTokenUri;
            _certificate = certificate;
            _callBackUri = callBackUri;

            // Validate all the parameters
            Validate();

            OnCreated();
        }

        /// <summary>
        /// Auth consumer interface implementation.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        /// <param name="certificate">The certificate issued by the provider; only used when SignatureTypes = RSA_SHA1.</param>
        public AuthConsumer(string consumerKey, string consumerSecret, SignatureTypes signatureTypes,
            Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri, X509Certificate2 certificate)
            : this(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, null, certificate)
        {
        }

        /// <summary>
        /// Auth consumer interface implementation.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        /// <param name="callBackUri">The call back URI</param>
        public AuthConsumer(string consumerKey, string consumerSecret, SignatureTypes signatureTypes,
            Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri, Uri callBackUri)
            : this(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, callBackUri, null)
        {
        }

        /// <summary>
        /// Auth consumer interface implementation.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        public AuthConsumer(string consumerKey, string consumerSecret, SignatureTypes signatureTypes,
            Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri)
            : this(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, null, null)
        {
        }

        private string _consumerKey = string.Empty;
        private string _consumerSecret = string.Empty;
        private SignatureTypes _signatureTypes = SignatureTypes.PLAIN_TEXT;
        private Uri _requestTokenUri = null;
        private Uri _userAuthorizeUri = null;
        private Uri _accessTokenUri = null;
        private Uri _callBackUri = null;
        private X509Certificate2 _certificate = null;

        private OAuthConsumerContext _oAuthConsumerContext = null;
        private OAuthSession _oAuthSession = null;

        /// <summary>
        /// Gets sets the unique consumer key
        /// </summary>
        public string ConsumerKey
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
        /// Gets sets the signature type
        /// </summary>
        public SignatureTypes SignatureTypes
        {
            get { return _signatureTypes; }
            set { _signatureTypes = value; }
        }

        /// <summary>
        /// Gets sets the certificate issued by the provider; only used when SignatureTypes = RSA_SHA1
        /// </summary>
        public X509Certificate2 Certificate
        {
            get { return _certificate; }
            set { _certificate = value; }
        }

        /// <summary>
        /// Gets sets the request token URI
        /// </summary>
        public Uri RequestTokenUri
        {
            get { return _requestTokenUri; }
            set { _requestTokenUri = value; }
        }

        /// <summary>
        /// Gets sets the user authorize URI
        /// </summary>
        public Uri UserAuthorizeUri
        {
            get { return _userAuthorizeUri; }
            set { _userAuthorizeUri = value; }
        }

        /// <summary>
        /// Gets sets the access token URI
        /// </summary>
        public Uri AccessTokenUri
        {
            get { return _accessTokenUri; }
            set { _accessTokenUri = value; }
        }

        /// <summary>
        /// Gets sets the callback URI
        /// </summary>
        public Uri CallBackUri
        {
            get { return _callBackUri; }
            set { _callBackUri = value; }
        }

        /// <summary>
        /// Make a request to the provider for a request token.
        /// </summary>
        /// <returns>The request token.</returns>
        public IToken GetRequestToken()
        {
            // Create a new session
            CreateSession();

            // Make a request to the provider to get a request token
            return _oAuthSession.GetRequestToken();
        }

        /// <summary>
        /// Make a request to the provider for a access token.
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <returns>The access token</returns>
        public IToken GetAccessToken(IToken requestToken)
        {
            if (requestToken == null) throw new ArgumentNullException("requestToken");

            // Create a new session
            CreateSession();

            // Make a request to the provider to get a access token
            return _oAuthSession.ExchangeRequestTokenForAccessToken(requestToken);
        }

        /// <summary>
        /// Make a request to the provider for a access token.
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="verificationCode">The verification code after succesfull user authorization.</param>
        /// <returns>The access token</returns>
        public IToken GetAccessToken(IToken requestToken, string verificationCode)
        {
            if (requestToken == null) throw new ArgumentNullException("requestToken");
            if (String.IsNullOrEmpty(verificationCode)) throw new ArgumentNullException("verificationCode");

            // Create a new session
            CreateSession();

            // Make a request to the provider to get a access token
            return _oAuthSession.ExchangeRequestTokenForAccessToken(requestToken, verificationCode);
        }

        /// <summary>
        /// Generate a user authorize url for this token (which can use in a redirect from the current site)
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="callBackUri">the callback URI</param>
        /// <returns>The URL string to the user authorisation web service.</returns>
        public string GetUserAuthorizationUrl(IToken requestToken, Uri callBackUri)
        {
            if (requestToken == null) throw new ArgumentNullException("requestToken");
            if (callBackUri == null) throw new ArgumentNullException("callBackUri");

            // Create a new session
            CreateSession();

            // Generate a user authorize url for this token (which you can use in a redirect from the current site)
            return _oAuthSession.GetUserAuthorizationUrlForToken(requestToken, callBackUri.ToString());
        }

        /// <summary>
        /// Get the resource data from the URI.
        /// </summary>
        /// <param name="resourceUri">The resource URI containing the resource data.</param>
        /// <returns>The resource data.</returns>
        public string GetResourceData(Uri resourceUri)
        {
            if (resourceUri == null) throw new ArgumentNullException("resourceUri");

            // Make sure an access token has been generated.
            if (_oAuthSession != null && _oAuthSession.AccessToken != null)
            {
                // Make a request for a protected resource
                return _oAuthSession.Request().Get().ForUrl(resourceUri.ToString()).ToString();
            }
            else
                throw new Exception("An access token has not been generated. Generate an access token before requesting resource data.");
        }

        /// <summary>
        /// Get the resource data from the URI.
        /// </summary>
        /// <param name="resourceUri">The resource URI containing the resource data.</param>
        /// <param name="accessToken">The access token</param>
        /// <returns>The resource data.</returns>
        public string GetResourceData(Uri resourceUri, IToken accessToken)
        {
            if (resourceUri == null) throw new ArgumentNullException("resourceUri");

            // Make sure an access token has been generated.
            if (_oAuthSession != null)
            {
                // Set the access token.
                _oAuthSession.AccessToken = accessToken;

                // Make a request for a protected resource
                return _oAuthSession.Request().Get().ForUrl(resourceUri.ToString()).ToString();
            }
            else
                throw new Exception("An access token has not been generated. Generate an access token before requesting resource data.");
        }

        /// <summary>
        /// Create a stored token.
        /// </summary>
        /// <param name="tokenBase">The token base instance.</param>
        /// <returns>The token from stored values (create a stored request token).</returns>
        public IToken CreateStoredToken(TokenBase tokenBase)
        {
            if (tokenBase == null) throw new ArgumentNullException("tokenBase");
            return tokenBase;
        }

        /// <summary>
        /// Close the current consumer session; allows for a new session to be started with different properties.
        /// </summary>
        public void CloseConsumerSession()
        {
            _oAuthSession = null;
        }

        /// <summary>
        /// Create a new consumer context.
        /// </summary>
        private void CreateConsumerContext()
        {
            // Select the appropriate signature type.
            switch(_signatureTypes)
            {
                case SignatureTypes.PLAIN_TEXT:
                    // Plain text signing.
                    _oAuthConsumerContext = new OAuthConsumerContext()
                    {
                        SignatureMethod = SignatureMethod.PlainText,
                        ConsumerKey = _consumerKey,
                        ConsumerSecret = _consumerSecret
                    };
                    break;

                case SignatureTypes.HMAC_SHA1:
                    // Hmac signing.
                    _oAuthConsumerContext = new OAuthConsumerContext()
                    {
                        SignatureMethod = SignatureMethod.HmacSha1,
                        ConsumerKey = _consumerKey,
                        ConsumerSecret = _consumerSecret
                    };
                    break;

                case SignatureTypes.RSA_SHA1:
                    // Rsa signing.
                    _oAuthConsumerContext = new OAuthConsumerContext()
                    {
                        SignatureMethod = SignatureMethod.RsaSha1,
                        ConsumerKey = _consumerKey,
                        ConsumerSecret = _consumerSecret,
                        Key = _certificate.PrivateKey
                    };
                    break;
            }
        }

        /// <summary>
        /// Create a new session
        /// </summary>
        private void CreateSession()
        {
            if (_oAuthSession == null)
            {
                Validate();
                CreateConsumerContext();

                // Create the new consumer session.
                _oAuthSession = new OAuthSession(_oAuthConsumerContext, _requestTokenUri, _userAuthorizeUri, _accessTokenUri);
            }
        }

        /// <summary>
        /// Validate all the parameters
        /// </summary>
        private void Validate()
        {
            if (String.IsNullOrEmpty(_consumerKey)) throw new ArgumentNullException("consumerKey");
            if (String.IsNullOrEmpty(_consumerSecret)) throw new ArgumentNullException("consumerSecret");
            if (_requestTokenUri == null) throw new ArgumentNullException("requestTokenUri");
            if (_userAuthorizeUri == null) throw new ArgumentNullException("userAuthorizeUri");
            if (_accessTokenUri == null) throw new ArgumentNullException("accessTokenUri");

            // If using RSA-SHA1 signature
            if (_signatureTypes == SignatureTypes.RSA_SHA1)
            {
                // Make sure a certificate is used.
                if (_certificate == null)
                    throw new Exception("If SignatureTypes = RSA_SHA1 then a valid certificate with a private key must be passed.");
            }
        }
	}
}
