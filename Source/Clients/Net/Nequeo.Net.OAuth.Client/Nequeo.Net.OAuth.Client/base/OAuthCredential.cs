/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Net;
using System.Security;

using Nequeo.Security;

namespace Nequeo.Net.OAuth
{
    /// <summary>
    /// OAuth credentials.
    /// </summary>
    public class OAuthCredential : ICredentials
    {
        /// <summary>
        /// Construct a simple OAuthCredential that doesn't need to be refreshed with an refresh token
        /// </summary>
        public OAuthCredential(string accessToken) : this()
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            AccessToken = accessToken.ToSecureString();
        }

        /// <summary>
        /// Contstruct an OAuthCredenial with a refresh token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="expiration">The time that the access token will expire in.</param>
        /// <param name="expiresIn">The length that the access token will expire in.</param>
        /// <param name="client">OAuthClient to use to refresh the token</param>
        public OAuthCredential(SecureString accessToken, SecureString refreshToken, DateTime expiration, int expiresIn, OAuthClient client)
            : this()
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException("accessToken");
            }
            if (refreshToken == null)
            {
                throw new ArgumentNullException("refreshToken");
            }
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
            AccessTokenExpiration = expiration;
        }

        /// <summary>
        /// Contstruct an OAuthCredenial with a refresh token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="expiresIn">Time in seconds that the access token will expire in.</param>
        /// <param name="client">OAuthClient to use to refresh the token</param>
        public OAuthCredential(string accessToken, string refreshToken, int expiresIn, OAuthClient client)
            : this(accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException("refreshToken");
            }
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
            RefreshToken = refreshToken.ToSecureString();
            ExpiresIn = expiresIn;
            AccessTokenExpiration = DateTime.Now.AddSeconds(expiresIn);
        }

        private OAuthClient _client;

        private OAuthCredential()
        {
            RefreshThreshold = 7.0;
        }

        /// <summary>
        /// The access token used in OAuth 2.0
        /// </summary>
        public SecureString AccessToken { get; set; }

        /// <summary>
        /// Returns when the access token will expire, null if no expiration.
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }

        /// <summary>
        /// Returns the duration of the expiration window, in seconds
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Refresh token used to obtain new access tokens when they expire
        /// </summary>
        public SecureString RefreshToken { get; set; }

        /// <summary>
        /// OAuthCredentials will refresh the token if it expries within this threshold of seconds.
        /// </summary>
        public double RefreshThreshold { get; set; }

        /// <summary>
        /// return OAuth 2.0 Credentials. If the token is expired, it will first attempt to refresh the token.
        /// </summary>
        /// <param name="uri">Ignored.</param>
        /// <param name="authType">Ignored.</param>
        /// <exception cref="WebException">May occour when refreshing the token</exception>
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            if (AccessTokenExpiration != null && RefreshToken != null && AccessTokenExpiration.Subtract(DateTime.Now).TotalSeconds < RefreshThreshold)
            {
                OAuthCredential newCreds = _client.RefreshAccessToken(RefreshToken);
                AccessToken = newCreds.AccessToken;

                // a new refresh token may be issued, per 5.2
                if (newCreds.RefreshToken != null)
                {
                    RefreshToken = newCreds.RefreshToken;
                    AccessTokenExpiration = newCreds.AccessTokenExpiration;
                    ExpiresIn = newCreds.ExpiresIn;
                }
                else
                {
                    // otherwise, calculate the next refresh time
                    AccessTokenExpiration = DateTime.Now.AddSeconds(ExpiresIn);
                }
            }

            return new NetworkCredential("OAuth", AccessToken);
        }
    }
}
