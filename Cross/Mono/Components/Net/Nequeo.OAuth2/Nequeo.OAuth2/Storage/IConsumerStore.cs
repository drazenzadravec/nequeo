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

using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Framework.Messages;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Framework.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

namespace Nequeo.Net.OAuth2.Storage
{
    /// <summary>
    /// Consumer store interface.
    /// </summary>
    public interface IConsumerStore
    {
        /// <summary>
        /// Gets or sets the access token lifetime (In minutes).
        /// </summary>
        double AccessTokenLifetime { get; set; }

        /// <summary>
        /// Is consumer
        /// </summary>
        /// <param name="consumer">The consumer key to examine.</param>
        /// <returns>True if consumer ; else false.</returns>
        bool IsConsumer(string consumer);

        /// <summary>
        /// Set consumer secret
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        void SetConsumerSecret(string consumer, string consumerSecret);

        /// <summary>
        /// Get consumer secret
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <returns>The consumer secret.</returns>
        string GetConsumerSecret(string consumer);

        /// <summary>
        /// Get consumer identifier
        /// </summary>
        /// <param name="nonce">The nonce.</param>
        /// <returns>The consumer identifier.</returns>
        string GetConsumerIdentifier(string nonce);

        /// <summary>
        /// Set consumer certificate
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <param name="certificate">The certificate.</param>
        void SetConsumerCertificate(string consumer, X509Certificate2 certificate);

        /// <summary>
        /// Get consumer certificate
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <returns>The certificate.</returns>
        X509Certificate2 GetConsumerCertificate(string consumer);

        /// <summary>
        /// Get consumer public key
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <returns>The asymmetric algorithm</returns>
        AsymmetricAlgorithm GetConsumerPublicKey(string consumer);

        /// <summary>
        /// Set the verification code for the current user identifier
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="userID">The unique user identifier.</param>
        /// <param name="scope">The authorisation scope.</param>
        /// <returns>The verification code.</returns>
        string SetVerificationCode(string consumer, string nonce, string userID, string scope);

        /// <summary>
        /// Gets the client with a given identifier.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <returns>The client registration.  Never null.</returns>
        /// <exception cref="ArgumentException">Thrown when no client with the given identifier is registered with this authorization server.</exception>
        IClientDescription GetClient(string clientIdentifier);

        /// <summary>
        /// Determines whether a described authorization is (still) valid.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <returns>
        /// 	<c>true</c> if the original authorization is still valid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>When establishing that an authorization is still valid,
        /// it's very important to only match on recorded authorizations that
        /// meet these criteria:</para>
        ///  1) The client identifier matches.
        ///  2) The user account matches.
        ///  3) The scope on the recorded authorization must include all scopes in the given authorization.
        ///  4) The date the recorded authorization was issued must be <em>no later</em> that the date the given authorization was issued.
        /// <para>One possible scenario is where the user authorized a client, later revoked authorization,
        /// and even later reinstated authorization.  This subsequent recorded authorization 
        /// would not satisfy requirement #4 in the above list.  This is important because the revocation
        /// the user went through should invalidate all previously issued tokens as a matter of
        /// security in the event the user was revoking access in order to sever authorization on a stolen
        /// account or piece of hardware in which the tokens were stored. </para>
        /// </remarks>
        bool IsAuthorizationValid(IAuthorizationDescription authorization);

        /// <summary>
        /// Determines whether a described authorization is (still) valid.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="nonce">The nonce data.</param>
        /// <returns>
        /// 	<c>true</c> if the original authorization is still valid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>When establishing that an authorization is still valid,
        /// it's very important to only match on recorded authorizations that
        /// meet these criteria:</para>
        ///  1) The client identifier matches.
        ///  2) The user account matches.
        ///  3) The scope on the recorded authorization must include all scopes in the given authorization.
        ///  4) The date the recorded authorization was issued must be <em>no later</em> that the date the given authorization was issued.
        /// <para>One possible scenario is where the user authorized a client, later revoked authorization,
        /// and even later reinstated authorization.  This subsequent recorded authorization 
        /// would not satisfy requirement #4 in the above list.  This is important because the revocation
        /// the user went through should invalidate all previously issued tokens as a matter of
        /// security in the event the user was revoking access in order to sever authorization on a stolen
        /// account or piece of hardware in which the tokens were stored. </para>
        /// </remarks>
        bool IsAuthorizationValid(string clientIdentifier, string nonce);

        /// <summary>
        /// Determines whether a given set of resource owner credentials is valid based on the authorization server's user database.
        /// </summary>
        /// <param name="userName">Username on the account.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="accessRequest">
        /// The access request the credentials came with.
        /// This may be useful if the authorization server wishes to apply some policy based on the client that is making the request.
        /// </param>
        /// <param name="canonicalUserName">
        /// Receives the canonical username (normalized for the resource server) of the user, for valid credentials;
        /// Or <c>null</c> if the return value is false.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the given credentials are valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsResourceOwnerCredentialValid(string userName, string password, IAccessTokenRequest accessRequest, out string canonicalUserName);

    }
}
