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
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

namespace Nequeo.DataAccess.CloudInteraction.OAuth2
{
    /// <summary>
    /// OAuth 2.0 context base handler.
    /// </summary>
    public abstract class OAuthContextBase : OAuthBase
	{
        /// <summary>
        /// Insert a new client authorization.
        /// </summary>
        /// <param name="clientID">The client id.</param>
        /// <param name="nonce">The nonce string itself.</param>
        /// <returns>True if inserted; else false.</returns>
        protected bool InsertClientAuthorization(long clientID, string nonce)
        {
            // Insert the new Nonce.
            return new Nequeo.DataAccess.CloudInteraction.Data.Extension.ClientAuthorization().Insert.InsertItem(
                new Data.ClientAuthorization()
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    ClientID = clientID,
                    NonceCode = nonce
                });
        }

        /// <summary>
        /// Insert the access token.
        /// </summary>
        /// <param name="oAuthConsumerID">The oAuthConsumerID</param>
        /// <param name="token">The current access token.</param>
        /// <param name="callbackUrl">The client global callback.</param>
        /// <param name="version">The consumer version.</param>
        /// <param name="accessTokenLifetime">The access token lifetime.</param>
        /// <returns>True if inserted; else false.</returns>
        protected bool InsertAccessToken(long oAuthConsumerID, AccessToken token, string callbackUrl, string version, double accessTokenLifetime)
        {
            // Insert the new token.
            return new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken().Insert.InsertItem(
                new Data.OAuthToken()
                {
                    Token = GenerateToken(),
                    TokenSecret = GenerateToken(),
                    State = 0,
                    IssueDateUtc = DateTime.UtcNow,
                    ExpiryDateUtc = DateTime.UtcNow.AddMinutes(accessTokenLifetime),
                    OAuthConsumerID = oAuthConsumerID,
                    TokenVerifier = "N/A",
                    TokenCallback = callbackUrl,
                    ConsumerVersion = version,
                    Context = token.ClientIdentifier,
                    TokenType = "AccessToken"
                });
        }

        /// <summary>
        /// Get consumer certificate
        /// </summary>
        /// <param name="consumer">The consumer key</param>
        /// <returns>The certificate.</returns>
        protected X509Certificate2 GetConsumerX509Certificate(string consumer)
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
                    // Get the file store of the certificate to
                    // return the private key for use in signing
                    // the access token.
                    X509Certificate2 x509 = Nequeo.Security.X509Certificate2Store.GetCertificate(cryptoKey.CertificatePath, "drazen");
                    return x509;
                }
                else
                    throw new Exception("The client for consumer key : " + consumer + " does not contain a certificate.");
            }
            else
                throw new Exception("The client for consumer key : " + consumer + " does not exist.");
        }

        /// <summary>
        /// Get the specific client authorization for the current application.
        /// </summary>
        /// <param name="codeKey">The code key.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.ClientAuthorization GetSpecificClientAuthorization(string codeKey)
        {
            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.ClientAuthorization nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.ClientAuthorization();
            Nequeo.DataAccess.CloudInteraction.Data.ClientAuthorization nonce = nonceExt.Select.SelectDataEntity(
                u =>
                    (u.CodeKey == codeKey));

            // Return the nonce.
            return nonce;
        }
	}
}
