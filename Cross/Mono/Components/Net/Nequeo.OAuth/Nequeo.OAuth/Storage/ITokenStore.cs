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

using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Storage
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
        /// Creates a request token for the consumer.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        IToken CreateRequestToken(IOAuthContext context);

        /// <summary>
        /// Create an access token using xAuth.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        IToken CreateAccessToken(IOAuthContext context);

        /// <summary>
        /// Should consume a use of the request token, throwing a <see cref="OAuthException" /> on failure.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        void ConsumeRequestToken(IOAuthContext requestContext);

        /// <summary>
        /// Should consume a use of an access token, throwing a <see cref="OAuthException" /> on failure.
        /// </summary>
        /// <param name="accessContext">The context.</param>
        void ConsumeAccessToken(IOAuthContext accessContext);

        /// <summary>
        /// Get the access token associated with a request token.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        IToken GetAccessTokenAssociatedWithRequestToken(IOAuthContext requestContext);

        /// <summary>
        /// Returns the status for a request to access a consumers resources.
        /// </summary>
        /// <param name="accessContext">The context.</param>
        /// <returns>The request for access status</returns>
        RequestForAccessStatus GetStatusOfRequestForAccess(IOAuthContext accessContext);

        /// <summary>
        /// Returns the callback url that is stored against this token.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>The callback URL</returns>
        string GetCallbackUrlForToken(IOAuthContext requestContext);

        /// <summary>
        /// Retrieves the verification code for a token
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>verification code</returns>
        string GetVerificationCodeForRequestToken(IOAuthContext requestContext);

        /// <summary>
        /// Gets the token secret for the supplied request token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>token secret</returns>
        string GetRequestTokenSecret(IOAuthContext context);

        /// <summary>
        /// Gets the token secret for the supplied access token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>token secret</returns>
        string GetAccessTokenSecret(IOAuthContext context);

        /// <summary>
        /// Renews the access token.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        IToken RenewAccessToken(IOAuthContext requestContext);

    }
}
