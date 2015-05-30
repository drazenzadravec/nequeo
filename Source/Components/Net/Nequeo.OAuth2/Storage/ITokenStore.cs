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

using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Framework.Messages;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

namespace Nequeo.Net.OAuth2.Storage
{
    /// <summary>
    /// Token store interface.
    /// </summary>
    public interface ITokenStore
    {
        /// <summary>
        /// Gets or sets the access token lifetime (In minutes).
        /// </summary>
        double AccessTokenLifetime { get; set; }

        /// <summary>
        /// Create an access token using xAuth.
        /// </summary>
        /// <param name="accessTokenRequestMessage">A request from a client that should be responded to directly with an access token.</param>
        /// <param name="nonce">The nonce data.</param>
        /// <returns>Describes the parameters to be fed into creating a response to an access token request.</returns>
        AccessTokenResult CreateAccessToken(IAccessTokenRequest accessTokenRequestMessage, string nonce = null);

        /// <summary>
        /// Update the generated access token.
        /// </summary>
        /// <param name="accessToken">The access token generated.</param>
        /// <param name="nonce">The internal nonce.</param>
        /// <param name="refreshToken">The refresh token generated (if any).</param>
        void UpdateAccessToken(string accessToken, string nonce, string refreshToken = null);

        /// <summary>
        /// Store the code key.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="nonce">The nonce value.</param>
        /// <param name="codeKey">The code key to store.</param>
        void StoreCodeKey(string clientIdentifier, string nonce, string codeKey);

        /// <summary>
        /// Get the nonce for the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <returns>The nonce for the refresh.</returns>
        string GetNonce(string refreshToken, string clientIdentifier, string clientSecret);

        /// <summary>
        /// Get the nonce for the access token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The nonce for the access token.</returns>
        string GetNonceByAccessToken(string accessToken);

        /// <summary>
        /// Get the nonce for the code key.
        /// </summary>
        /// <param name="codeKey">The code key.</param>
        /// <returns>The nonce for the code key.</returns>
        string GetNonce(string codeKey);
    }
}
