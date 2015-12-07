/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          MembershipProvider.cs
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.ComponentModel.Composition;
using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;

using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Storage.Basic;
using Nequeo.Net.OAuth2.Provider.Session;
using Nequeo.Net.OAuth2.Provider;
using Nequeo.Net.OAuth2.Provider.Session.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

namespace Nequeo.DataAccess.CloudInteraction.OAuth2
{
    /// <summary>
    /// OAuth token provider.
    /// </summary>
    public class OAuthTokenProvider : OAuthContextBase, ITokenStore
	{
        /// <summary>
        /// Gets or sets the access token lifetime (In minutes).
        /// </summary>
        public double AccessTokenLifetime { get; set; }

        /// <summary>
        /// Create an access token using xAuth.
        /// </summary>
        /// <param name="accessTokenRequestMessage">A request from a client that should be responded to directly with an access token.</param>
        /// <param name="nonce">The nonce data.</param>
        /// <returns>Describes the parameters to be fed into creating a response to an access token request.</returns>
        public AccessTokenResult CreateAccessToken(IAccessTokenRequest accessTokenRequestMessage, string nonce)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(accessTokenRequestMessage.ClientIdentifier);
            if (client != null)
            {
                // Make sure then client has been authenticated.
                if (accessTokenRequestMessage.ClientAuthenticated)
                {
                    long clientID = client.ClientID;

                    // Get the file store of the certificate to
                    // return the private key for use in signing
                    // the access token.
                    var cryptoKey = new Nequeo.DataAccess.CloudInteraction.Data.Extension.SymmetricCryptoKey().Select.SelectDataEntity(u => u.ClientID == clientID);
                    X509Certificate2 certificate = base.GetConsumerX509Certificate(client.ClientIdentifier);
                   
                    var accessToken = new AuthorizationServerAccessToken();
                    long oAuthConsumerID = 0;

                    // Get the specific nonce
                    var nonceData = GetSpecificNonce(nonce);
                    if (nonceData != null)
                    {
                        oAuthConsumerID = nonceData.OAuthConsumerID;
                        accessToken.Nonce = System.Text.Encoding.Default.GetBytes(nonce);
                        accessToken.ClientIdentifier = accessTokenRequestMessage.ClientIdentifier;
                    }
                    else
                        throw new Exception("Could not insert token; Internal database exception.");

                    // Insert the access token.
                    if (!InsertAccessToken(oAuthConsumerID, accessToken, client.Callback, accessTokenRequestMessage.Version.ToString(), AccessTokenLifetime))
                        throw new Exception("Could not insert token; Internal database exception.");

                    // If the certificate exists.
                    if (certificate != null)
                    {
                        // This can be useful to mitigate the security risks
                        // of access tokens that are used over standard HTTP.
                        // But this is just the lifetime of the access token. 
                        // The client can still renew it using their refresh 
                        // token until the authorization itself expires.
                        accessToken.Lifetime = TimeSpan.FromMinutes(AccessTokenLifetime);
                        accessToken.ClientIdentifier = accessTokenRequestMessage.ClientIdentifier;
                        
                        // For this sample, we assume just one resource server.
                        // If this authorization server needs to mint access tokens for more than one resource server,
                        // we'd look at the request message passed to us and decide which public key to return.
                        accessToken.ResourceServerEncryptionKey = (RSACryptoServiceProvider)certificate.PublicKey.Key;
                        accessToken.AccessTokenSigningKey = (RSACryptoServiceProvider)certificate.PrivateKey;
                    }

                    // Create the access token result.
                    var result = new AccessTokenResult(accessToken);

                    // Return the new access token data.
                    return result;
                }
                else
                    throw new Exception("The client has not been authenticated.");
            }
            else
                throw new Exception("The client for consumer key : " + accessTokenRequestMessage.ClientIdentifier + " does not exist.");
        }

        /// <summary>
        /// Update the generated access token.
        /// </summary>
        /// <param name="accessToken">The access token generated.</param>
        /// <param name="nonce">The internal nonce.</param>
        /// <param name="refreshToken">The refresh token generated (if any).</param>
        public void UpdateAccessToken(string accessToken, string nonce, string refreshToken = null)
        {
            // Get the specific nonce
            var nonceData = GetSpecificNonce(nonce);
            if (nonceData != null)
            {
                // Get the oAuth consumer.
                var oAuthConsumer = GetSpecificOAuthConsumers(nonceData.OAuthConsumerID);
                long oAuthConsumerID = oAuthConsumer.OAuthConsumerID;

                if (String.IsNullOrEmpty(refreshToken))
                {
                    // Update the ClientAuthorization code key.
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken().
                    Update.UpdateItemPredicate(
                        new Data.OAuthToken()
                        {
                            Token = accessToken
                        }, u =>
                            (u.OAuthConsumerID == oAuthConsumerID)
                    );
                }
                else
                {
                    // Update the ClientAuthorization code key.
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken().
                    Update.UpdateItemPredicate(
                        new Data.OAuthToken()
                        {
                            Token = accessToken,
                            TokenSecret = refreshToken
                        }, u =>
                            (u.OAuthConsumerID == oAuthConsumerID)
                    );
                }
            }
            else
                throw new Exception("Could not insert token; Internal database exception.");
        }

        /// <summary>
        /// Store the code key.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="nonce">The nonce value.</param>
        /// <param name="codeKey">The code key to store.</param>
        public void StoreCodeKey(string clientIdentifier, string nonce, string codeKey)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(clientIdentifier);
            if (client != null)
            {
                // Update the ClientAuthorization code key.
                new Nequeo.DataAccess.CloudInteraction.Data.Extension.ClientAuthorization().
                Update.UpdateItemPredicate(
                    new Data.ClientAuthorization()
                    {
                        CodeKey = codeKey
                    }, u =>
                        (u.NonceCode == nonce)
                );
            }
            else
                throw new Exception("The client for consumer key : " + clientIdentifier + " does not exist.");
        }

        /// <summary>
        /// Get the nonce for the code key.
        /// </summary>
        /// <param name="codeKey">The code key.</param>
        /// <returns>The nonce for the code key.</returns>
        public string GetNonce(string codeKey)
        {
            var clientAuth = GetSpecificClientAuthorization(codeKey);
            if (clientAuth != null)
            {
                // Return the nonce data.
                return clientAuth.NonceCode;
            }
            else
                throw new Exception("The client code key does not exist.");
        }

        /// <summary>
        /// Get the nonce for the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <returns>The nonce for the refresh.</returns>
        public string GetNonce(string refreshToken, string clientIdentifier, string clientSecret)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(clientIdentifier);
            if (client != null)
            {
                // Make sure the client secret match.
                if (client.ClientSecret != clientSecret)
                    throw new Exception("The client for client secret : " + clientSecret + " does not exist.");
                else
                {
                    // Get the token.
                    var token = GetSpecificRefreshToken(refreshToken);
                    if (client != null)
                    {
                        // Get the nonce object.
                        var nonce = GetSpecificNonce(token.OAuthConsumerID);
                        return nonce.Code;
                    }
                    else
                        throw new Exception("The refresh token does not exist.");
                }
            }
            else
                throw new Exception("The client for client key : " + clientIdentifier + " does not exist.");
        }

        /// <summary>
        /// Get the nonce for the access token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The nonce for the access token.</returns>
        public string GetNonceByAccessToken(string accessToken)
        {
            // Get the specific token
            var token = GetSpecificToken(accessToken);
            if (token != null)
            {
                // Get the nonce object.
                var nonce = GetSpecificNonce(token.OAuthConsumerID);
                return nonce.Code;
            }
            else
                throw new Exception("The toekn does not exist.");
        }
	}
}
