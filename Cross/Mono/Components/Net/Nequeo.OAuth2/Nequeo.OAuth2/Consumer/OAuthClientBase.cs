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

namespace Nequeo.Net.OAuth2.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    using Nequeo.Net.OAuth2.Framework.Utility;

    /// <summary>
    /// Represents the base class for OAuth 2.0 clients
    /// </summary>
    public abstract class OAuthClientBase : IAuthenticationClient
    {
        /// <summary>
        /// The provider name.
        /// </summary>
        private readonly string providerName;

        /// <summary>
        /// The return url.
        /// </summary>
        private Uri returnUrl;

        /// <summary>
        /// Initializes a new instance of the class with the specified provider name.
        /// </summary>
        /// <param name="providerName">
        /// Name of the provider. 
        /// </param>
        protected OAuthClientBase(string providerName)
        {
            this.providerName = providerName;
        }

        /// <summary>
        /// Gets the name of the provider which provides authentication service.
        /// </summary>
        public string ProviderName
        {
            get
            {
                return this.providerName;
            }
        }

        /// <summary>
        /// Attempts to authenticate users by forwarding them to an external website, and upon succcess or failure, redirect users back to the specified url.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="returnUrl">
        /// The return url after users have completed authenticating against external website. 
        /// </param>
        public virtual void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            this.returnUrl = returnUrl;

            string redirectUrl = this.GetServiceLoginUrl(returnUrl).AbsoluteUri;
            context.Response.Redirect(redirectUrl, endResponse: true);
        }

        /// <summary>
        /// Check if authentication succeeded after user is redirected back from the service provider.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AuthenticationResult"/> containing authentication result. 
        /// </returns>
        public AuthenticationResult VerifyAuthentication(HttpContextBase context)
        {

            return VerifyAuthentication(context, this.returnUrl);
        }

        /// <summary>
        /// Check if authentication succeeded after user is redirected back from the service provider.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="returnPageUrl">The return URL which should match the value passed to RequestAuthentication() method.</param>
        /// <returns>
        /// An instance of <see cref="AuthenticationResult"/> containing authentication result.
        /// </returns>
        public virtual AuthenticationResult VerifyAuthentication(HttpContextBase context, Uri returnPageUrl)
        {

            string code = context.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                return AuthenticationResult.Failed;
            }

            string accessToken = this.QueryAccessToken(returnPageUrl, code);
            if (accessToken == null)
            {
                return AuthenticationResult.Failed;
            }

            IDictionary<string, string> userData = this.GetUserData(accessToken);
            if (userData == null)
            {
                return AuthenticationResult.Failed;
            }

            string id = userData["id"];
            string name;

            // Some oAuth providers do not return value for the 'username' attribute. 
            // In that case, try the 'name' attribute. If it's still unavailable, fall back to 'id'
            if (!userData.TryGetValue("username", out name) && !userData.TryGetValue("name", out name))
            {
                name = id;
            }

            // add the access token to the user data dictionary just in case page developers want to use it
            userData["accesstoken"] = accessToken;

            return new AuthenticationResult(
                isSuccessful: true, provider: this.ProviderName, providerUserId: id, userName: name, extraData: userData);
        }

        /// <summary>
        /// Gets the full url pointing to the login page for this client. The url should include the specified return url so that when the login completes, user is redirected back to that url.
        /// </summary>
        /// <param name="returnUrl">
        /// The return URL. 
        /// </param>
        /// <returns>
        /// An absolute URL. 
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login",
            Justification = "Login is used more consistently in ASP.Net")]
        protected abstract Uri GetServiceLoginUrl(Uri returnUrl);

        /// <summary>
        /// Given the access token, gets the logged-in user's data. The returned dictionary must include two keys 'id', and 'username'.
        /// </summary>
        /// <param name="accessToken">
        /// The access token of the current user. 
        /// </param>
        /// <returns>
        /// A dictionary contains key-value pairs of user data 
        /// </returns>
        protected abstract IDictionary<string, string> GetUserData(string accessToken);

        /// <summary>
        /// Queries the access token from the specified authorization code.
        /// </summary>
        /// <param name="returnUrl">
        /// The return URL. 
        /// </param>
        /// <param name="authorizationCode">
        /// The authorization code. 
        /// </param>
        /// <returns>
        /// The access token 
        /// </returns>
        protected abstract string QueryAccessToken(Uri returnUrl, string authorizationCode);

    }
}
