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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Consumer.Session;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;
using Nequeo.Net.OAuth.Provider.Inspectors;

namespace Nequeo.Net.OAuth.Consumer
{
    /// <summary>
    /// OAuth session interface.
    /// </summary>
    public interface IOAuthSession
    {
        /// <summary>
        /// Gets sets the consumer context
        /// </summary>
        IOAuthConsumerContext ConsumerContext { get; set; }

        /// <summary>
        /// Gets sets the request token uri
        /// </summary>
        Uri RequestTokenUri { get; set; }

        /// <summary>
        /// Gets sets the access token uri
        /// </summary>
        Uri AccessTokenUri { get; set; }

        /// <summary>
        /// Gets sets the user authorize uri
        /// </summary>
        Uri UserAuthorizeUri { get; set; }

        /// <summary>
        /// Gets sets the proxy server uri
        /// </summary>
        Uri ProxyServerUri { get; set; }

        /// <summary>
        /// Gets sets the access token
        /// </summary>
        IToken AccessToken { get; set; }

        /// <summary>
        /// Gets sets the response body action
        /// </summary>
        Action<string> ResponseBodyAction { get; set; }

        /// <summary>
        /// Get the consumer request.
        /// </summary>
        /// <returns>The consumer request.</returns>
        IConsumerRequest Request();

        /// <summary>
        /// Get the consumer request
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>The consumer request.</returns>
        IConsumerRequest Request(IToken accessToken);

        /// <summary>
        /// Gets the request token
        /// </summary>
        /// <returns>The token</returns>
        IToken GetRequestToken();

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <returns>The token</returns>
        IToken ExchangeRequestTokenForAccessToken(IToken requestToken);

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="verificationCode">The verification code</param>
        /// <returns>The token</returns>
        IToken ExchangeRequestTokenForAccessToken(IToken requestToken, string verificationCode);

        /// <summary>
        /// Exchange request token for access token
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="method">The method</param>
        /// <param name="verificationCode">The verification code</param>
        /// <returns>The token</returns>
        IToken ExchangeRequestTokenForAccessToken(IToken requestToken, string method, string verificationCode);

        /// <summary>
        /// Get access token using xauth
        /// </summary>
        /// <param name="authMode">The auth mode</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The token</returns>
        IToken GetAccessTokenUsingXAuth(string authMode, string username, string password);

        /// <summary>
        /// Build request token context
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>The consumer request.</returns>
        IConsumerRequest BuildRequestTokenContext(string method);

        /// <summary>
        /// Build exchange request token for access token context
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="method">The method</param>
        /// <param name="verificationCode">The verification code</param>
        /// <returns>The consumer request.</returns>
        IConsumerRequest BuildExchangeRequestTokenForAccessTokenContext(IToken requestToken, string method, string verificationCode);

        /// <summary>
        /// Build access token context
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="xAuthMode">The xAuth mode</param>
        /// <param name="xAuthUsername">The xAuth username</param>
        /// <param name="xAuthPassword">The xAuth password</param>
        /// <returns>The consumer request.</returns>
        IConsumerRequest BuildAccessTokenContext(string method, string xAuthMode, string xAuthUsername, string xAuthPassword);

        /// <summary>
        /// Get user authorization url for token
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="callbackUrl">The callback url</param>
        /// <returns>The url</returns>
        string GetUserAuthorizationUrlForToken(IToken token, string callbackUrl);

        /// <summary>
        /// Get user authorization url for token
        /// </summary>
        /// <param name="token">The token</param>
        /// <returns>The url</returns>
        string GetUserAuthorizationUrlForToken(IToken token);

        /// <summary>
        /// With Form Parameters
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithFormParameters(IDictionary dictionary);

        /// <summary>
        /// With Form Parameters
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithFormParameters(object anonymousClass);

        /// <summary>
        /// With Query Parameters
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithQueryParameters(IDictionary dictionary);

        /// <summary>
        /// With Query Parameters
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithQueryParameters(object anonymousClass);

        /// <summary>
        /// With Cookies
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithCookies(IDictionary dictionary);

        /// <summary>
        /// With Cookies
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithCookies(object anonymousClass);

        /// <summary>
        /// With Headers
        /// </summary>
        /// <param name="dictionary">The dictionay parameters</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithHeaders(IDictionary dictionary);

        /// <summary>
        /// With Headers
        /// </summary>
        /// <param name="anonymousClass">An anonymous class</param>
        /// <returns>The OAuth session</returns>
        IOAuthSession WithHeaders(object anonymousClass);

        /// <summary>
        /// Requires Callback Confirmation
        /// </summary>
        /// <returns>The OAuth session</returns>
        IOAuthSession RequiresCallbackConfirmation();
    }
}
