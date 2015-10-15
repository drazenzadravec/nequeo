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
using System.Text;

using Nequeo.Net.OAuth2;

namespace Nequeo.Net.OAuth.Client.Version2
{
    /// <summary>
    /// Windows Live OAuth 2.0 client.
    /// </summary>
    public partial class WindowsLive : OAuthBase
    {
        /// <summary>
        /// Windows Live OAuth 2.0 client.
        /// </summary>
        /// <param name="consumerIdentifier">The unique consumer identifier.</param>
        /// <param name="consumerSecret">The unique consumer secret.</param>
        public WindowsLive(string consumerIdentifier, string consumerSecret)
            : this(consumerIdentifier, consumerSecret, 
            new Uri("https://login.live.com/oauth20_authorize.srf"), 
            new Uri("https://login.live.com/oauth20_token.srf"))
        {
        }

        /// <summary>
        /// Windows Live OAuth 2.0 client.
        /// </summary>
        /// <param name="consumerIdentifier">The unique consumer identifier.</param>
        /// <param name="consumerSecret">The unique consumer secret.</param>
        /// <param name="userAuthorizeEndpointUri">The user authorise endpoint uri.</param>
        /// <param name="tokenEndpointUri">The token endpoint uri.</param>
        public WindowsLive(string consumerIdentifier, string consumerSecret, Uri userAuthorizeEndpointUri, Uri tokenEndpointUri)
            : base(consumerIdentifier, consumerSecret, userAuthorizeEndpointUri, tokenEndpointUri)
        {
            OnCreated();
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <returns>The response data.</returns>
        public string GetMeResource(OAuth2.Consumer.Session.IAuthorizationState state)
        {
            byte[] data = base.RequestResource(state, "https://apis.live.net/v5.0/me");
            return System.Text.Encoding.Default.GetString(data);
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="resourceEndPointUrl">The resource endpoint url.</param>
        /// <returns>The response data.</returns>
        public string GetResource(OAuth2.Consumer.Session.IAuthorizationState state, string resourceEndPointUrl)
        {
            byte[] data = base.RequestResource(state, resourceEndPointUrl);
            return System.Text.Encoding.Default.GetString(data);
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <returns>The response windows live graph.</returns>
        public WindowsLiveGraph GetMeResourceGraph(OAuth2.Consumer.Session.IAuthorizationState state)
        {
            return WindowsLiveGraph.Deserialize(GetMeResource(state));
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="resourceEndPointUrl">The resource endpoint url.</param>
        /// <returns>The response windows live graph.</returns>
        public WindowsLiveGraph GetResourceGraph(OAuth2.Consumer.Session.IAuthorizationState state, string resourceEndPointUrl)
        {
            return WindowsLiveGraph.Deserialize(GetResource(state, resourceEndPointUrl));
        }

        /// <summary>
        /// Request the web user authosisation with basic scope.
        /// </summary>
        /// <param name="authWebClient">The auth web client.</param>
        /// <param name="returnTo">The URL the authorization server should redirect the browser (typically on
        /// this site) to when the authorization is completed. If null, the current request's
        /// URL will be used.</param>
        public void RequestWebUserAuthorizationBasicScope(AuthWebClient authWebClient = null, Uri returnTo = null)
        {
            base.RequestWebUserAuthorization(authWebClient, new[] { WindowsLive.Scopes.Basic }, returnTo);
        }

        /// <summary>
        /// Request the user agent user authosisation with basic scope.
        /// </summary>
        /// <param name="authAgentClient">The auth user agent client.</param>
        /// <param name="state">The client state that should be returned with the authorization response.</param>
        /// <param name="returnTo">The URL that the authorization response should be sent to via a user-agent redirect.</param>
        public void RequestAgentUserAuthorizationBasicScope(AuthAgentClient authAgentClient = null, string state = null, Uri returnTo = null)
        {
            base.RequestAgentUserAuthorization(authAgentClient, new[] { WindowsLive.Scopes.Basic }, state, returnTo);
        }

        /// <summary>
        /// Well-known scopes defined by the Windows Live service.
        /// </summary>
        /// <remarks>
        /// This sample includes just a few scopes.  For a complete list of scopes please refer to:
        /// http://msdn.microsoft.com/en-us/library/hh243646.aspx
        /// </remarks>
        public static class Scopes
        {
            /// <summary>
            /// The ability of an app to read and update a user's info at any time. 
            /// Without this scope, an app can access the user's info only while 
            /// the user is signed in to Live Connect and is using your app.
            /// </summary>
            public const string OfflineAccess = "wl.offline_access";

            /// <summary>
            /// Single sign-in behavior. With single sign-in, users who are 
            /// already signed in to Live Connect are also signed in to your website.
            /// </summary>
            public const string SignIn = "wl.signin";

            /// <summary>
            /// Read access to a user's basic profile info. Also enables read 
            /// access to a user's list of contacts.
            /// </summary>
            public const string Basic = "wl.basic";

            /// <summary>
            /// Grants read-only permission to all of a user's OneDrive files, including files shared with the user.
            /// </summary>
            public const string OnedriveReadonly = "onedrive.readonly";

            /// <summary>
            /// Grants read and write permission to all of a user's OneDrive files, including files shared with the user.
            /// </summary>
            public const string OnedriveReadWrite = "onedrive.readwrite";
        }
    }
}
