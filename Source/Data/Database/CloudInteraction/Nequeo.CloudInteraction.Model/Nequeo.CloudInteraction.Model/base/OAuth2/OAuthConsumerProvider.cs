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
using Nequeo.Net.OAuth2.Framework.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

namespace Nequeo.DataAccess.CloudInteraction.OAuth2
{
    /// <summary>
    /// OAuth consumer provider.
    /// </summary>
    public class OAuthConsumerProvider : OAuthContextBase, IConsumerStore, IClientDescription
	{
        private string _clientIdentifier = string.Empty;

        /// <summary>
        /// Gets or sets the access token lifetime (In minutes).
        /// </summary>
        public double AccessTokenLifetime { get; set; }

        /// <summary>
        /// Get consumer public key
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <returns>The asymmetric algorithm</returns>
        public AsymmetricAlgorithm GetConsumerPublicKey(string consumer)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer);
            if (client != null)
            {
                long clientID = client.ClientID;

                // Get the crypto key
                var cryptoKey = new Nequeo.DataAccess.CloudInteraction.Data.Extension.SymmetricCryptoKey().Select.SelectDataEntity(u => u.ClientID == clientID);
                if (cryptoKey != null)
                {
                    // Get the x509 certificate from the data.
                    X509Certificate2 x509 = Nequeo.Security.X509Certificate2Store.LoadCertificateFromByteArray(cryptoKey.Secret);
                    return x509.PublicKey.Key;
                }
                else
                    throw new Exception("The client for consumer key : " + consumer + " does not contain a certificate.");
            }
            else
                throw new Exception("The client for consumer key : " + consumer + " does not exist.");
        }

        /// <summary>
        /// Get consumer secret
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <returns>The consumer secret.</returns>
        public string GetConsumerSecret(string consumer)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer);
            if (client != null)
            {
                // Return the client secret.
                return client.ClientSecret;
            }
            else
                throw new Exception("The client for consumer key : " + consumer + " does not exist.");
        }

        /// <summary>
        /// Is consumer
        /// </summary>
        /// <param name="consumer">The consumer key to examine.</param>
        /// <returns>True if consumer ; else false.</returns>
        public bool IsConsumer(string consumer)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer);
            if (client != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get consumer identifier
        /// </summary>
        /// <param name="nonce">The nonce.</param>
        /// <returns>The consumer identifier.</returns>
        public string GetConsumerIdentifier(string nonce)
        {
            var nonceData = GetSpecificNonce(nonce);
            if (nonceData != null)
            {
                // Return the client identifier.
                return nonceData.Context;
            }
            else
                throw new Exception("The nonce does not exist.");
        }

        /// <summary>
        /// Get consumer certificate
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <returns>The certificate.</returns>
        public X509Certificate2 GetConsumerCertificate(string consumer)
        {
            return base.GetConsumerX509Certificate(consumer);
        }

        /// <summary>
        /// Set consumer certificate
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <param name="certificate">The certificate.</param>
        public void SetConsumerCertificate(string consumer, X509Certificate2 certificate)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer);
            if (client != null)
            {
                long clientID1 = client.ClientID;

                // Get the crypto key
                var cryptoKey = new Nequeo.DataAccess.CloudInteraction.Data.Extension.SymmetricCryptoKey().Select.SelectDataEntity(u => u.ClientID == clientID1);
                if (cryptoKey != null)
                {
                    long clientID = client.ClientID;

                    // Update the client crypto key.
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.SymmetricCryptoKey().
                    Update.UpdateItemPredicate(
                        new Data.SymmetricCryptoKey()
                        {
                            Secret = certificate.RawData
                        }, u =>
                            (u.ClientID == clientID)
                    );
                }
                else
                {
                    // Insert the crypto key
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.SymmetricCryptoKey().
                    Insert.InsertItem(
                        new Data.SymmetricCryptoKey()
                        {
                            Secret = certificate.RawData,
                            ClientID = client.ClientID,
                            ExpiresUtc = certificate.NotAfter.ToUniversalTime(),
                            Bucket = certificate.GetSerialNumberString(),
                            Handle = certificate.Subject
                        }
                    );
                }
            }
            else
                throw new Exception("The client for consumer key : " + consumer + " does not exist.");
        }

        /// <summary>
        /// Set consumer secret
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        public void SetConsumerSecret(string consumer, string consumerSecret)
        {
            // If the consumer exists.
            if (IsConsumer(consumer))
            {
                // Update the client secret.
                new Nequeo.DataAccess.CloudInteraction.Data.Extension.Client().
                Update.UpdateItemPredicate(
                    new Data.Client()
                    {
                        ClientSecret = consumerSecret
                    }, u =>
                        (u.ClientIdentifier == consumer)
                );
            }
            else
                throw new Exception("The client for consumer key : " + consumer + " does not exist.");
        }

        /// <summary>
        /// Set the verification code for the current user identifier
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="userID">The unique user identifier.</param>
        /// <param name="scope">The authorisation scope.</param>
        /// <returns>The verification code.</returns>
        public string SetVerificationCode(string consumer, string nonce, string userID, string scope)
        {
            // Get the user for the userID
            var user = GetSpecificUser(Int64.Parse(userID));
            if (user != null)
            {
                // Create a verification code.
                string vCode = GenerateToken();

                // Get the token
                var nonceData = GetSpecificNonce(consumer, nonce);
                if (nonceData != null)
                {
                    // Get the oAuth consumer.
                    var oAuthConsumer = GetSpecificOAuthConsumers(nonceData.OAuthConsumerID);

                    long userIdentity = user.UserID;
                    long clientID = oAuthConsumer.ClientID;
                    long oAuthConsumerID = nonceData.OAuthConsumerID;

                    // Update the OAuthConsumer.
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthConsumer().
                    Update.UpdateItemPredicate(
                        new Data.OAuthConsumer()
                        {
                            VerificationCode = vCode,
                            UserID = userIdentity
                        }, u =>
                            (u.ClientID == clientID) &&
                            (u.OAuthConsumerID == oAuthConsumerID)
                    );

                    // Update the ClientAuthorization.
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.ClientAuthorization().
                    Update.UpdateItemPredicate(
                        new Data.ClientAuthorization()
                        {
                            ExpirationDateUtc = DateTime.UtcNow.Add(TimeSpan.FromMinutes(AccessTokenLifetime)),
                            UserID = userIdentity,
                            Scope = scope
                        }, u =>
                            (u.ClientID == clientID) &&
                            (u.NonceCode == nonce)
                    );

                    // Return the verification code.
                    return vCode;
                }
                else
                    throw new Exception("No request nonce has been found, can not create a verification code.");
            }
            else
                throw new Exception("The user for userID : " + userID + " does not exist.");
        }

        /// <summary>
        /// Gets the client with a given identifier.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <returns>The client registration.  Never null.</returns>
        /// <exception cref="ArgumentException">Thrown when no client with the given identifier is registered with this authorization server.</exception>
        public Net.OAuth2.Consumer.Session.Authorization.IClientDescription GetClient(string clientIdentifier)
        {
            _clientIdentifier = clientIdentifier;
            return this;
        }

        /// <summary>
        /// Gets the type of the client.
        /// </summary>
        public Net.OAuth2.Consumer.Session.Authorization.ClientType ClientType
        {
            get 
            {
                // If the consumer exists.
                if (IsConsumer(_clientIdentifier))
                {
                    // Get the client for the consumer key
                    var client = GetSpecificClient(_clientIdentifier);
                    if (client != null)
                    {
                        // Return the client type.
                        return (Net.OAuth2.Consumer.Session.Authorization.ClientType)client.ClientType;
                    }
                    else
                        throw new Exception("The client for consumer key : " + _clientIdentifier + " does not exist.");
                }
                else
                    throw new Exception("The client for consumer key : " + _clientIdentifier + " does not exist.");
            }
        }

        /// <summary>
        /// Gets the callback to use when an individual authorization request does not include an explicit callback URI.
        /// </summary>
        public Uri DefaultCallback
        {
            get 
            {
                // If the consumer exists.
                if (IsConsumer(_clientIdentifier))
                {
                    // Get the client for the consumer key
                    var client = GetSpecificClient(_clientIdentifier);
                    if (client != null)
                    {
                        // Return the URI of the callback URL else
                        // return a null value.
                        return string.IsNullOrEmpty(client.Callback) ? null : new Uri(client.Callback);
                    }
                    else
                        throw new Exception("The client for consumer key : " + _clientIdentifier + " does not exist.");
                }
                else
                    throw new Exception("The client for consumer key : " + _clientIdentifier + " does not exist.");
            }
        }

        /// <summary>
        /// Gets a value indicating whether a non-empty secret is registered for this client.
        /// </summary>
        public bool HasNonEmptySecret
        {
            get { return true; }
        }

        /// <summary>
        /// Determines whether a callback URI included in a client's authorization request
        /// is among those allowed callbacks for the registered client.
        /// </summary>
        /// <param name="callback">The absolute URI the client has requested the 
        /// authorization result be received at. Never null.</param>
        /// <returns>True if the callback URL is allowable for this client; otherwise, false.</returns>
        public bool IsCallbackAllowed(Uri callback)
        {
            // If the consumer exists.
            if (IsConsumer(_clientIdentifier))
            {
                // Get the client for the consumer key
                var client = GetSpecificClient(_clientIdentifier);
                if (client != null)
                {
                    // If the call back is empty.
                    if (string.IsNullOrEmpty(client.Callback))
                    {
                        // No callback rules have been set up for this client.
                        // Setup callback rules if needed.
                        return true;
                    }

                    // It's enough of a callback URL match if the scheme and host match.
                    // In a production app, it is advisable to require a match on the path as well.
                    Uri acceptableCallbackPattern = new Uri(client.Callback);
                    if (string.Equals(acceptableCallbackPattern.GetLeftPart(UriPartial.Authority), callback.GetLeftPart(UriPartial.Authority), StringComparison.Ordinal))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                throw new Exception("The client for consumer key : " + _clientIdentifier + " does not exist.");
        }

        /// <summary>
        /// Checks whether the specified client secret is correct.
        /// </summary>
        /// <param name="secret">The secret obtained from the client.</param>
        /// <returns>True if the secret matches the one in the authorization 
        /// server's record for the client; false otherwise.</returns>
        public bool IsValidClientSecret(string secret)
        {
            // If the consumer exists.
            if (IsConsumer(_clientIdentifier))
            {
                // Get the client for the consumer key
                var client = GetSpecificClient(_clientIdentifier);
                if (client != null)
                {
                    // If the client secret equals the passed client secret
                    // then indicate a valid client secret.
                    if (secret == client.ClientSecret)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                throw new Exception("The client for consumer key : " + _clientIdentifier + " does not exist.");
        }

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
        public bool IsAuthorizationValid(IAuthorizationDescription authorization)
        {
            HashSet<string> requestedScopes = authorization.Scope;
            string clientIdentifier = authorization.ClientIdentifier;
            DateTime issuedUtc = authorization.UtcIssued;
            string userData = authorization.UserDataAndNonce;

            // Get the client for the consumer key
            var client = GetSpecificClient(clientIdentifier);
            if (client != null)
            {
                string[] userNonce = userData.Split(new char[] { '_' });
                string userID = userNonce[0];
                string nonce = userNonce[1];

                // Get the nonce
                var nonceData = GetSpecificNonce(clientIdentifier, nonce);
                if (nonceData != null)
                {
                    // Get the specific OAuth consumer.
                    var oAuthConsumer = GetSpecificOAuthConsumers(nonceData.OAuthConsumerID);
                    if (oAuthConsumer != null)
                    {
                        // If the verification code has been set
                        // then grant access.
                        if (!String.IsNullOrEmpty(oAuthConsumer.VerificationCode) && (oAuthConsumer.UserID != null))
                        {
                            // Get the specifice user authorization.
                            var clientAuth = GetSpecificClientAuthorization(client.ClientID, nonceData.Code);
                            if (clientAuth != null)
                            {
                                // If the verification code has been set
                                // then grant access.
                                if ((clientAuth.UserID != null) && (clientAuth.ExpirationDateUtc.HasValue))
                                {
                                    // Add one second onto the creation date.
                                    issuedUtc += TimeSpan.FromSeconds(1);
                                    if ((clientAuth.CreatedOnUtc <= issuedUtc) && (clientAuth.ExpirationDateUtc.Value >= DateTime.UtcNow))
                                        return true;
                                    else
                                        return false;
                                }
                                else
                                    return false;
                            }
                            else
                                throw new Exception("No request authorization has been found, can not find a verification code.");
                        }
                        else
                            return false;
                    }
                    else
                        throw new Exception("No request authorization has been found, can not find a verification code.");
                }
                else
                    throw new Exception("No request nonce has been found, can not find a verification code.");
            }
            else
                throw new Exception("The client for consumer key : " + clientIdentifier + " does not exist.");
        }

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
        public bool IsAuthorizationValid(string clientIdentifier, string nonce)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(clientIdentifier);
            if (client != null)
            {
                // Get the nonce
                var nonceData = GetSpecificNonce(clientIdentifier, nonce);
                if (nonceData != null)
                {
                    // Get the specific OAuth consumer.
                    var oAuthConsumer = GetSpecificOAuthConsumers(nonceData.OAuthConsumerID);
                    if (oAuthConsumer != null)
                    {
                        // If the verification code has been set
                        // then grant access.
                        if (!String.IsNullOrEmpty(oAuthConsumer.VerificationCode) && (oAuthConsumer.UserID != null))
                        {
                            // Get the specifice user authorization.
                            var clientAuth = GetSpecificClientAuthorization(client.ClientID, nonceData.Code);
                            if (clientAuth != null)
                            {
                                // If the verification code has been set
                                // then grant access.
                                if ((clientAuth.UserID != null) && (clientAuth.ExpirationDateUtc.HasValue))
                                {
                                    // Add one second onto the creation date.
                                    if (clientAuth.ExpirationDateUtc.Value >= DateTime.UtcNow)
                                        return true;
                                    else
                                        return false;
                                }
                                else
                                    return false;
                            }
                            else
                                throw new Exception("No request authorization has been found, can not find a verification code.");
                        }
                        else
                            return false;
                    }
                    else
                        throw new Exception("No request authorization has been found, can not find a verification code.");
                }
                else
                    throw new Exception("No request nonce has been found, can not find a verification code.");
            }
            else
                throw new Exception("The client for consumer key : " + clientIdentifier + " does not exist.");
        }

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
        public bool IsResourceOwnerCredentialValid(string userName, string password, IAccessTokenRequest accessRequest, out string canonicalUserName)
        {
            canonicalUserName = null;
            
            // If the consumer exists.
            if (IsConsumer(accessRequest.ClientIdentifier))
            {
                // Get the user for the consumer key
                var user = GetSpecificUser(userName, password);
                if (user != null)
                {
                    canonicalUserName = userName;
                    return true;
                }
                else
                    return false;
            }
            else
                throw new Exception("The client for consumer key : " + _clientIdentifier + " does not exist.");
        }
    }
}
