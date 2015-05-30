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
using System.IO;

using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Storage.Basic
{
    /// <summary>
    /// Simple in memory token store.
    /// </summary>
    public class SimpleTokenStore : ITokenStore
    {
        readonly ITokenRepository<AccessToken> _accessTokenRepository;
        readonly ITokenRepository<RequestToken> _requestTokenRepository;

        /// <summary>
        /// Simple Token Store
        /// </summary>
        /// <param name="accessTokenRepository">The access token repository oif type access token.</param>
        /// <param name="requestTokenRepository">The request token repository oif type access token.</param>
        public SimpleTokenStore(ITokenRepository<AccessToken> accessTokenRepository, ITokenRepository<RequestToken> requestTokenRepository)
        {
            if (accessTokenRepository == null) throw new ArgumentNullException("accessTokenRepository");
            if (requestTokenRepository == null) throw new ArgumentNullException("requestTokenRepository");
            _accessTokenRepository = accessTokenRepository;
            _requestTokenRepository = requestTokenRepository;
        }

        /// <summary>
        /// Creates a request token for the consumer.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken CreateRequestToken(IOAuthContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var token = new RequestToken
            {
                ConsumerKey = context.ConsumerKey,
                Realm = context.Realm,
                Token = Guid.NewGuid().ToString(),
                TokenSecret = Guid.NewGuid().ToString(),
                CallbackUrl = context.CallbackUrl
            };

            _requestTokenRepository.SaveToken(token);

            return token;
        }

        /// <summary>
        /// Create an access token using xAuth.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken CreateAccessToken(IOAuthContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var accessToken = new AccessToken
            {
                ConsumerKey = context.ConsumerKey,
                ExpiryDate = DateTime.UtcNow.AddDays(20),
                Realm = context.Realm,
                Token = Guid.NewGuid().ToString(),
                TokenSecret = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString(),
            };

            _accessTokenRepository.SaveToken(accessToken);

            return accessToken;
        }

        /// <summary>
        /// Should consume a use of the request token, throwing a <see cref="OAuthException" /> on failure.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        public void ConsumeRequestToken(IOAuthContext requestContext)
        {
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            RequestToken requestToken = GetRequestToken(requestContext);

            UseUpRequestToken(requestContext, requestToken);

            _requestTokenRepository.SaveToken(requestToken);
        }

        /// <summary>
        /// Should consume a use of an access token, throwing a <see cref="OAuthException" /> on failure.
        /// </summary>
        /// <param name="accessContext">The context.</param>
        public void ConsumeAccessToken(IOAuthContext accessContext)
        {
            AccessToken accessToken = GetAccessToken(accessContext);

            if (accessToken.ExpiryDate < Clock.Now)
            {
                throw new OAuthException(accessContext, OAuthProblemParameters.TokenExpired, "Token has expired");
            }
        }

        /// <summary>
        /// Get the access token associated with a request token.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken GetAccessTokenAssociatedWithRequestToken(IOAuthContext requestContext)
        {
            RequestToken requestToken = GetRequestToken(requestContext);
            return requestToken.AccessToken;
        }

        /// <summary>
        /// Returns the status for a request to access a consumers resources.
        /// </summary>
        /// <param name="accessContext">The context.</param>
        /// <returns>The request for access status</returns>
        public RequestForAccessStatus GetStatusOfRequestForAccess(IOAuthContext accessContext)
        {
            RequestToken request = GetRequestToken(accessContext);

            if (request.AccessDenied) return RequestForAccessStatus.Denied;

            if (request.AccessToken == null) return RequestForAccessStatus.Unknown;

            return RequestForAccessStatus.Granted;
        }

        /// <summary>
        /// Returns the callback url that is stored against this token.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>The callback URL</returns>
        public string GetCallbackUrlForToken(IOAuthContext requestContext)
        {
            RequestToken requestToken = GetRequestToken(requestContext);
            return requestToken.CallbackUrl;
        }

        /// <summary>
        /// Retrieves the verification code for a token
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>verification code</returns>
        public string GetVerificationCodeForRequestToken(IOAuthContext requestContext)
        {
            RequestToken requestToken = GetRequestToken(requestContext);

            return requestToken.Verifier;
        }

        /// <summary>
        /// Gets the token secret for the supplied request token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>token secret</returns>
        public string GetRequestTokenSecret(IOAuthContext context)
        {
            RequestToken requestToken = GetRequestToken(context);

            return requestToken.TokenSecret;
        }

        /// <summary>
        /// Gets the token secret for the supplied access token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>token secret</returns>
        public string GetAccessTokenSecret(IOAuthContext context)
        {
            AccessToken token = GetAccessToken(context);

            return token.TokenSecret;
        }

        /// <summary>
        /// Renews the access token.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken RenewAccessToken(IOAuthContext requestContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Request Token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The request token</returns>
        RequestToken GetRequestToken(IOAuthContext context)
        {
            try
            {
                return _requestTokenRepository.GetToken(context.Token);
            }
            catch (Exception exception)
            {
                // TODO: log exception
                throw Error.UnknownToken(context, context.Token, exception);
            }
        }

        /// <summary>
        /// Get Access Token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The access token</returns>
        AccessToken GetAccessToken(IOAuthContext context)
        {
            try
            {
                return _accessTokenRepository.GetToken(context.Token);
            }
            catch (Exception exception)
            {
                // TODO: log exception
                throw Error.UnknownToken(context, context.Token, exception);
            }
        }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The token</returns>
        public IToken GetToken(IOAuthContext context)
        {
            var token = (IToken)null;
            if (!string.IsNullOrEmpty(context.Token))
            {
                try
                {
                    token = _accessTokenRepository.GetToken(context.Token) ??
                            (IToken)_requestTokenRepository.GetToken(context.Token);
                }
                catch (Exception ex)
                {
                    // TODO: log exception
                    throw Error.UnknownToken(context, context.Token, ex);
                }
            }
            return token;
        }

        /// <summary>
        /// Use Up Request Token
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <param name="requestToken">The request token</param>
        static void UseUpRequestToken(IOAuthContext requestContext, RequestToken requestToken)
        {
            if (requestToken.UsedUp)
            {
                throw new OAuthException(requestContext, OAuthProblemParameters.TokenRejected,
                                         "The request token has already be consumed.");
            }

            requestToken.UsedUp = true;
        }

        /// <summary>
        /// Gets or sets the access token lifetime (In minutes).
        /// </summary>
        public double AccessTokenLifetime
        {
            get;
            set;
        }
    }
}
