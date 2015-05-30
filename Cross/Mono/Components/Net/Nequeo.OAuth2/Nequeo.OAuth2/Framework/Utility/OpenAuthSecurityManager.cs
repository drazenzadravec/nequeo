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

namespace Nequeo.Net.OAuth2.Framework.Utility
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    using Nequeo.Net.OAuth2.Consumer;
    using Nequeo.Net.Core.Messaging;

    /// <summary>
    /// Manage authenticating with an external OAuth or OpenID provider
    /// </summary>
    public class OpenAuthSecurityManager
    {

        /// <summary>
        /// The provider query string name.
        /// </summary>
        private const string ProviderQueryStringName = "__provider__";

        /// <summary>
        /// The _authentication provider.
        /// </summary>
        private readonly IAuthenticationClient authenticationProvider;

        /// <summary>
        /// The _data provider.
        /// </summary>
        private readonly IOpenAuthDataProvider dataProvider;

        /// <summary>
        /// The _request context.
        /// </summary>
        private readonly HttpContextBase requestContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAuthSecurityManager"/> class.
        /// </summary>
        /// <param name="requestContext">
        /// The request context. 
        /// </param>
        public OpenAuthSecurityManager(HttpContextBase requestContext)
            : this(requestContext, provider: null, dataProvider: null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAuthSecurityManager"/> class.
        /// </summary>
        /// <param name="requestContext">
        /// The request context. 
        /// </param>
        /// <param name="provider">
        /// The provider. 
        /// </param>
        /// <param name="dataProvider">
        /// The data provider. 
        /// </param>
        public OpenAuthSecurityManager(
            HttpContextBase requestContext, IAuthenticationClient provider, IOpenAuthDataProvider dataProvider)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }

            this.requestContext = requestContext;
            this.dataProvider = dataProvider;
            this.authenticationProvider = provider;
        }

        /// <summary>
        /// Gets a value indicating whether IsAuthenticatedWithOpenAuth.
        /// </summary>
        public bool IsAuthenticatedWithOpenAuth
        {
            get
            {
                return this.requestContext.Request.IsAuthenticated
                       && OpenAuthAuthenticationTicketHelper.IsValidAuthenticationTicket(this.requestContext);
            }
        }

        /// <summary>
        /// Gets the provider that is responding to an authentication request.
        /// </summary>
        /// <param name="context">
        /// The HTTP request context.
        /// </param>
        /// <returns>
        /// The provider name, if one is available.
        /// </returns>
        public static string GetProviderName(HttpContextBase context)
        {
            return context.Request.QueryString[ProviderQueryStringName];
        }

        /// <summary>
        /// Checks if the specified provider user id represents a valid account. If it does, log user in.
        /// </summary>
        /// <param name="providerUserId">
        /// The provider user id. 
        /// </param>
        /// <param name="createPersistentCookie">
        /// if set to <c>true</c> create persistent cookie. 
        /// </param>
        /// <returns>
        /// <c>true</c> if the login is successful. 
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login",
            Justification = "Login is used more consistently in ASP.Net")]
        public bool Login(string providerUserId, bool createPersistentCookie)
        {
            string userName = this.dataProvider.GetUserNameFromOpenAuth(
                this.authenticationProvider.ProviderName, providerUserId);
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            OpenAuthAuthenticationTicketHelper.SetAuthenticationTicket(this.requestContext, userName, createPersistentCookie);
            return true;
        }

        /// <summary>
        /// Requests the specified provider to start the authentication by directing users to an external website
        /// </summary>
        /// <param name="returnUrl">
        /// The return url after user is authenticated. 
        /// </param>
        public void RequestAuthentication(string returnUrl)
        {
            // convert returnUrl to an absolute path
            Uri uri;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                uri = UriHelper.ConvertToAbsoluteUri(returnUrl, this.requestContext);
            }
            else
            {
                uri = this.requestContext.Request.GetPublicFacingUrl();
            }

            // attach the provider parameter so that we know which provider initiated 
            // the login when user is redirected back to this page
            uri = uri.AttachQueryStringParameter(ProviderQueryStringName, this.authenticationProvider.ProviderName);
            this.authenticationProvider.RequestAuthentication(this.requestContext, uri);
        }

        /// <summary>
        /// Checks if user is successfully authenticated when user is redirected back to this user.
        /// </summary>
        /// <returns>The result of the authentication.</returns>
        public AuthenticationResult VerifyAuthentication()
        {
            AuthenticationResult result = this.authenticationProvider.VerifyAuthentication(this.requestContext);
            if (!result.IsSuccessful)
            {
                // if the result is a Failed result, creates a new Failed response which has providerName info.
                result = new AuthenticationResult(
                    isSuccessful: false,
                    provider: this.authenticationProvider.ProviderName,
                    providerUserId: null,
                    userName: null,
                    extraData: null);
            }

            return result;
        }

        /// <summary>
        /// Checks if user is successfully authenticated when user is redirected back to this user.
        /// </summary>
        /// <param name="returnUrl">The return Url which must match exactly the Url passed into RequestAuthentication() earlier.</param>
        /// <returns>
        /// The result of the authentication.
        /// </returns>
        public AuthenticationResult VerifyAuthentication(string returnUrl)
        {

            // Only OAuth2 requires the return url value for the verify authenticaiton step
            OAuthClientBase oauth2Client = this.authenticationProvider as OAuthClientBase;
            if (oauth2Client != null)
            {
                // convert returnUrl to an absolute path
                Uri uri;
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    uri = UriHelper.ConvertToAbsoluteUri(returnUrl, this.requestContext);
                }
                else
                {
                    uri = this.requestContext.Request.GetPublicFacingUrl();
                }

                AuthenticationResult result = oauth2Client.VerifyAuthentication(this.requestContext, uri);
                if (!result.IsSuccessful)
                {
                    // if the result is a Failed result, creates a new Failed response which has providerName info.
                    result = new AuthenticationResult(
                        isSuccessful: false,
                        provider: this.authenticationProvider.ProviderName,
                        providerUserId: null,
                        userName: null,
                        extraData: null);
                }

                return result;
            }
            else
            {
                return this.VerifyAuthentication();
            }
        }
    }
}