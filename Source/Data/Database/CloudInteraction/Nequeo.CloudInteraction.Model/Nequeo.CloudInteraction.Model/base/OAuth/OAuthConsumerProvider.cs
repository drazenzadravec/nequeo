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

using Nequeo.Net.OAuth.Storage;
using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.DataAccess.CloudInteraction.OAuth
{
    /// <summary>
    /// OAuth consumer provider.
    /// </summary>
    public class OAuthConsumerProvider : OAuthContextBase, IConsumerStore
	{
        /// <summary>
        /// Get consumer public key
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <returns>The asymmetric algorithm</returns>
        public AsymmetricAlgorithm GetConsumerPublicKey(IConsumer consumer)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer.ConsumerKey);
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
                    throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not contain a certificate",
                        new Exception("The client for consumer key : " + consumer.ConsumerKey + " does not contain a certificate."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + consumer.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Get consumer secret
        /// </summary>
        /// <param name="consumer">The OAuth context.</param>
        /// <returns>The consumer secret.</returns>
        public string GetConsumerSecret(IOAuthContext consumer)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer.ConsumerKey);
            if (client != null)
            {
                // Return the client secret.
                return client.ClientSecret;
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + consumer.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Is consumer
        /// </summary>
        /// <param name="consumer">The counsumer to examine.</param>
        /// <returns>True if consumer ; else false.</returns>
        public bool IsConsumer(IConsumer consumer)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer.ConsumerKey);
            if (client != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Set consumer certificate
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <param name="certificate">The certificate.</param>
        public void SetConsumerCertificate(IConsumer consumer, X509Certificate2 certificate)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer.ConsumerKey);
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
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + consumer.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Set consumer secret
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        public void SetConsumerSecret(IConsumer consumer, string consumerSecret)
        {
            string consumerKey = consumer.ConsumerKey;

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
                        (u.ClientIdentifier == consumerKey)
                );
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + consumer.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Set the verification code for the current user identifier
        /// </summary>
        /// <param name="consumer">The consumer</param>
        /// <param name="userID">The unique user identifier.</param>
        /// <returns>The verification code.</returns>
        public string SetVerificationCode(IOAuthContext consumer, string userID)
        {
            // Get the user for the userID
            var user = GetSpecificUser(Int64.Parse(userID));
            if (user != null)
            {
                // Create a verification code.
                string vCode = GenerateToken();

                // Get the token
                var token = GetSpecificToken(consumer.Token);
                if (token != null)
                {
                    // Get the oAuth consumer.
                    var oAuthConsumer = GetSpecificOAuthConsumers(token.OAuthConsumerID);

                    long userIdentity = user.UserID;
                    long clientID = oAuthConsumer.ClientID;
                    long oAuthConsumerID = token.OAuthConsumerID;

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

                    // Return the verification code.
                    return vCode;
                }
                else
                    throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                        new Exception("No request token has been found, can not create a verification code."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The user for userID : " + userID + " does not exist."));
        }
    }
}
