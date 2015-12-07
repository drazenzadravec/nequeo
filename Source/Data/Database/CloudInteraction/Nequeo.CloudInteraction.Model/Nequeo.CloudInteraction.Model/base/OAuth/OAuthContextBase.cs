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
using Nequeo.Net.OAuth.Storage.Basic;

namespace Nequeo.DataAccess.CloudInteraction.OAuth
{
    /// <summary>
    /// OAuth context base handler.
    /// </summary>
    public abstract class OAuthContextBase : OAuthBase
	{
        /// <summary>
        /// Is the Nonce passed unique within the database.
        /// </summary>
        /// <param name="consumer">The consumer associated with the nonce</param>
        /// <param name="nonce">The nonce string itself</param>
        /// <returns>True if Nonce is unique; else false.</returns>
        protected bool IsNonceUnique(IConsumer consumer, string nonce)
        {
            string consumerKey = consumer.ConsumerKey;

            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce();
            Nequeo.DataAccess.CloudInteraction.Data.Nonce nonceData = nonceExt.Select.SelectDataEntity(
                u =>
                    (u.Context == consumerKey) &&
                    (u.Code == nonce));

            // If null then Nonce is unique.
            return (nonceData != null ? false : true);
        }

        /// <summary>
        /// Insert a new nonce.
        /// </summary>
        /// <param name="oAuthConsumerID">The OAuthCosumer ID for the current consumer key and client ID.</param>
        /// <param name="consumer">The consumer associated with the nonce</param>
        /// <param name="nonce">The nonce string itself</param>
        /// <returns>True if inserted; else false.</returns>
        protected bool InsertNonce(long oAuthConsumerID, IConsumer consumer, string nonce)
        {
            // Insert the new Nonce.
            return new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce().Insert.InsertItem(
                new Data.Nonce()
                {
                    Context = consumer.ConsumerKey,
                    Code = nonce,
                    Timestamp = DateTime.UtcNow,
                    OAuthConsumerID = oAuthConsumerID
                });
        }

        

        /// <summary>
        /// Get the specific nonce for the current application.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.Nonce GetSpecificNonce(IOAuthContext context)
        {
            string consumerKey = context.ConsumerKey;
            string nonceData = context.Nonce;

            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce();
            Nequeo.DataAccess.CloudInteraction.Data.Nonce nonce = nonceExt.Select.SelectDataEntity(
                u =>
                    (u.Context == consumerKey) &&
                    (u.Code == nonceData));

            // Return the nonce.
            return nonce;
        }

        /// <summary>
        /// Insert the access token.
        /// </summary>
        /// <param name="oAuthConsumerID">The oAuthConsumerID</param>
        /// <param name="context">The current context.</param>
        /// <param name="token">The current access token.</param>
        /// <returns>True if inserted; else false.</returns>
        protected bool InsertAccessToken(long oAuthConsumerID, IOAuthContext context, AccessToken token)
        {
            // Insert the new token.
            return new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken().Insert.InsertItem(
                new Data.OAuthToken()
                {
                    Token = token.Token,
                    TokenSecret = token.TokenSecret,
                    State = 0,
                    IssueDateUtc = DateTime.UtcNow,
                    ExpiryDateUtc = token.ExpiryDate,
                    OAuthConsumerID = oAuthConsumerID,
                    TokenVerifier = context.Verifier,
                    TokenCallback = context.CallbackUrl,
                    ConsumerVersion = context.Version,
                    Context = token.ConsumerKey,
                    TokenType = "AccessToken"
                });
        }

        /// <summary>
        /// Insert the request token.
        /// </summary>
        /// <param name="oAuthConsumerID">The oAuthConsumerID</param>
        /// <param name="context">The current context.</param>
        /// <param name="token">The current request token.</param>
        /// <returns>True if inserted; else false.</returns>
        protected bool InsertRequestToken(long oAuthConsumerID, IOAuthContext context, RequestToken token)
        {
            // Insert the new token.
            return new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken().Insert.InsertItem(
                new Data.OAuthToken()
                {
                    Token = token.Token,
                    TokenSecret = token.TokenSecret,
                    State = 0,
                    IssueDateUtc = DateTime.UtcNow,
                    ExpiryDateUtc = DateTime.UtcNow,
                    OAuthConsumerID = oAuthConsumerID,
                    TokenVerifier = context.Verifier,
                    TokenCallback = token.CallbackUrl,
                    ConsumerVersion = context.Version,
                    Context = token.ConsumerKey,
                    TokenType = "RequestToken"
                });
        }

        /// <summary>
        /// Insert a new OAuth consumer for the client id.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clientID">The client ID associated with the OAuth consumer.</param>
        /// <returns>The OAuthConsumerID of the OAuth consumer inserted.</returns>
        protected long? InsertOAuthConsumer(IOAuthContext context, long clientID)
        {
            // Insert the new OAuthConsumer.
            List<object> oAuthConsumerIDs = new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthConsumer().Insert.InsertDataEntity(
                Nequeo.DataAccess.CloudInteraction.Data.OAuthConsumer.CreateOAuthConsumer(0, clientID));

            // Get the identities of the newly inserted item.
            if (oAuthConsumerIDs != null && oAuthConsumerIDs.Count > 0)
            {
                // Return the OAuthConsumerID value.
                return Int64.Parse(oAuthConsumerIDs[0].ToString());
            }

            // Return null nothing was inserted.
            return null;
        }
	}
}
