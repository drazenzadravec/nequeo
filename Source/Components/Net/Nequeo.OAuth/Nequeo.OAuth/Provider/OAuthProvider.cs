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
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;
using Nequeo.Net.OAuth.Provider.Inspectors;

namespace Nequeo.Net.OAuth.Provider
{
    /// <summary>
    /// OAuth provider.
    /// </summary>
    public class OAuthProvider : IOAuthProvider
    {
        /// <summary>
        /// OAuth Provider
        /// </summary>
        /// <param name="tokenStore">The token store container.</param>
        /// <param name="inspectors">The collection of inspectors.</param>
        public OAuthProvider(ITokenStore tokenStore, params IContextInspector[] inspectors)
        {
            RequiresCallbackUrlInRequest = true;

            if (tokenStore == null) throw new ArgumentNullException("tokenStore");
            _tokenStore = tokenStore;

            if (inspectors != null) _inspectors.AddRange(inspectors);
        }

        private readonly List<IContextInspector> _inspectors = new List<IContextInspector>();
        private readonly ITokenStore _tokenStore;

        /// <summary>
        /// Gets sets the requires callback url in request
        /// </summary>
        public bool RequiresCallbackUrlInRequest { get; set; }

        /// <summary>
        /// Grant request token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        public virtual IToken GrantRequestToken(IOAuthContext context)
        {
            AssertContextDoesNotIncludeToken(context);

            InspectRequest(ProviderPhase.GrantRequestToken, context);

            return _tokenStore.CreateRequestToken(context);
        }

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        public virtual IToken ExchangeRequestTokenForAccessToken(IOAuthContext context)
        {
            InspectRequest(ProviderPhase.ExchangeRequestTokenForAccessToken, context);

            _tokenStore.ConsumeRequestToken(context);

            switch (_tokenStore.GetStatusOfRequestForAccess(context))
            {
                case RequestForAccessStatus.Granted:
                    break;
                case RequestForAccessStatus.Unknown:
                    throw Error.ConsumerHasNotBeenGrantedAccessYet(context);
                default:
                    throw Error.ConsumerHasBeenDeniedAccess(context);
            }

            return _tokenStore.GetAccessTokenAssociatedWithRequestToken(context);
        }

        /// <summary>
        /// Access protected resource request
        /// </summary>
        /// <param name="context">OAuth context</param>
        public virtual void AccessProtectedResourceRequest(IOAuthContext context)
        {
            InspectRequest(ProviderPhase.AccessProtectedResourceRequest, context);

            _tokenStore.ConsumeAccessToken(context);
        }

        /// <summary>
        /// Renew access token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        public IToken RenewAccessToken(IOAuthContext context)
        {
            InspectRequest(ProviderPhase.RenewAccessToken, context);

            return _tokenStore.RenewAccessToken(context);
        }

        /// <summary>
        /// Create access token
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <returns>The token.</returns>
        public IToken CreateAccessToken(IOAuthContext context)
        {
            InspectRequest(ProviderPhase.CreateAccessToken, context);

            return _tokenStore.CreateAccessToken(context);
        }

        /// <summary>
        /// Assert context does not include token
        /// </summary>
        /// <param name="context">OAuth context</param>
        void AssertContextDoesNotIncludeToken(IOAuthContext context)
        {
            if (context.Token != null)
            {
                throw Error.RequestForTokenMustNotIncludeTokenInContext(context);
            }
        }

        /// <summary>
        /// Add Inspector
        /// </summary>
        /// <param name="inspector">The inspector to add.</param>
        public void AddInspector(IContextInspector inspector)
        {
            _inspectors.Add(inspector);
        }

        /// <summary>
        /// Inspect Request
        /// </summary>
        /// <param name="phase">Provider phase.</param>
        /// <param name="context">OAuth context</param>
        protected virtual void InspectRequest(ProviderPhase phase, IOAuthContext context)
        {
            AssertContextDoesNotIncludeTokenSecret(context);

            AddStoredTokenSecretToContext(context, phase);

            ApplyInspectors(context, phase);
        }

        /// <summary>
        /// Apply Inspectors
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <param name="phase">Provider phase</param>
        void ApplyInspectors(IOAuthContext context, ProviderPhase phase)
        {
            foreach (IContextInspector inspector in _inspectors)
            {
                inspector.InspectContext(phase, context);
            }
        }

        /// <summary>
        /// Add stored token secret to context
        /// </summary>
        /// <param name="context">OAuth context</param>
        /// <param name="phase">Provider phase</param>
        void AddStoredTokenSecretToContext(IOAuthContext context, ProviderPhase phase)
        {
            if (phase == ProviderPhase.ExchangeRequestTokenForAccessToken)
            {
                string secret = _tokenStore.GetRequestTokenSecret(context);
                context.TokenSecret = secret;
            }

            else if (phase == ProviderPhase.AccessProtectedResourceRequest || phase == ProviderPhase.RenewAccessToken)
            {
                string secret = _tokenStore.GetAccessTokenSecret(context);

                context.TokenSecret = secret;
            }
        }

        /// <summary>
        /// Assert context does not include token secret
        /// </summary>
        /// <param name="context">OAuth context</param>
        static void AssertContextDoesNotIncludeTokenSecret(IOAuthContext context)
        {
            if (!string.IsNullOrEmpty(context.TokenSecret))
            {
                throw new OAuthException(context, OAuthProblemParameters.ParameterRejected, "The oauth_token_secret must not be transmitted to the provider.");
            }
        }
    }
}
