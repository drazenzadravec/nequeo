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

using Nequeo.Net.OAuth2.Storage;
using Nequeo.Net.OAuth2.Framework;
using Nequeo.Net.OAuth2.Framework.Utility;
using Nequeo.Net.OAuth2.Framework.ChannelElements;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization;
using Nequeo.Net.OAuth2.Consumer.Session.Authorization.Messages;

namespace Nequeo.DataAccess.CloudInteraction
{
    /// <summary>
    /// OAuth base handler.
    /// </summary>
    public abstract class OAuthBase
	{
        /// <summary>
        /// Get the specific nonce for the current application.
        /// </summary>
        /// <param name="oAuthConsumerID">The oAuthConsumerID.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.Nonce GetSpecificNonce(long oAuthConsumerID)
        {
            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce();
            Nequeo.DataAccess.CloudInteraction.Data.Nonce nonce = nonceExt.Select.SelectDataEntity(u => (u.OAuthConsumerID == oAuthConsumerID));

            // Return the nonce.
            return nonce;
        }

        /// <summary>
        /// Insert a new OAuth consumer for the client id.
        /// </summary>
        /// <param name="clientID">The client ID associated with the OAuth consumer.</param>
        /// <returns>The OAuthConsumerID of the OAuth consumer inserted.</returns>
        protected long? InsertOAuthConsumer(long clientID)
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

        /// <summary>
        /// Insert a new nonce.
        /// </summary>
        /// <param name="oAuthConsumerID">The OAuthCosumer ID for the current consumer key and client ID.</param>
        /// <param name="context">The consumer key</param>
        /// <param name="nonce">The nonce string itself</param>
        /// <returns>True if inserted; else false.</returns>
        protected bool InsertNonce(long oAuthConsumerID, string context, string nonce)
        {
            // Insert the new Nonce.
            return new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce().Insert.InsertItem(
                new Data.Nonce()
                {
                    Context = context,
                    Code = nonce,
                    Timestamp = DateTime.UtcNow,
                    OAuthConsumerID = oAuthConsumerID
                });
        }

        /// <summary>
        /// Is the Nonce passed unique within the database.
        /// </summary>
        /// <param name="context">The consumer key</param>
        /// <param name="nonce">The nonce string itself</param>
        /// <param name="timestampUtc">The UTC timestamp that together with the nonce string make it unique.</param>
        /// <returns>True if Nonce is unique; else false.</returns>
        protected bool IsNonceUnique(string context, string nonce, DateTime timestampUtc)
        {
            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce();
            Nequeo.DataAccess.CloudInteraction.Data.Nonce nonceData = nonceExt.Select.SelectDataEntity(
                u =>
                    (u.Context == context) &&
                    (u.Code == nonce) &&
                    (u.Timestamp == timestampUtc));

            // If null then Nonce is unique.
            return (nonceData != null ? false : true);
        }

        /// <summary>
        /// Is the Nonce passed unique within the database.
        /// </summary>
        /// <param name="nonce">The nonce string itself</param>
        /// <returns>True if Nonce is unique; else false.</returns>
        protected bool IsNonceUnique(string nonce)
        {
            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce();
            Nequeo.DataAccess.CloudInteraction.Data.Nonce nonceData = nonceExt.Select.SelectDataEntity(
                u =>
                    (u.Code == nonce));

            // If null then Nonce is unique.
            return (nonceData != null ? false : true);
        }

        /// <summary>
        /// Get the specific token for the current application.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The token; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.OAuthToken GetSpecificToken(string token)
        {
            // Get the token data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken tokenExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken();
            Nequeo.DataAccess.CloudInteraction.Data.OAuthToken oAuthToken = tokenExt.Select.SelectDataEntity(
                u =>
                    (u.Token == token));

            // Return the token.
            return oAuthToken;
        }

        /// <summary>
        /// Get the specific refresh token for the current application.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>The token; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.OAuthToken GetSpecificRefreshToken(string refreshToken)
        {
            // Get the token data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken tokenExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken();
            Nequeo.DataAccess.CloudInteraction.Data.OAuthToken oAuthToken = tokenExt.Select.SelectDataEntity(
                u =>
                    (u.TokenSecret == refreshToken));

            // Return the token.
            return oAuthToken;
        }

        /// <summary>
        /// Get the specific OAuthConsumer for the current application.
        /// </summary>
        /// <param name="oAuthConsumerID">The oAuthConsumerID.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.OAuthConsumer GetSpecificOAuthConsumers(long oAuthConsumerID)
        {
            // Get the OAuthConsumer data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthConsumer oAuthConsumerExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthConsumer();
            Nequeo.DataAccess.CloudInteraction.Data.OAuthConsumer oAuthConsumer = oAuthConsumerExt.Select.SelectDataEntity(u => (u.OAuthConsumerID == oAuthConsumerID));

            // Return the OAuthConsumer.
            return oAuthConsumer;
        }

        /// <summary>
        /// Get the specific client for the current application.
        /// </summary>
        /// <param name="consumerKey">The consumerKey.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.Client GetSpecificClient(string consumerKey)
        {
            // Get the client data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Client clientExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Client();
            Nequeo.DataAccess.CloudInteraction.Data.Client nonce = clientExt.Select.SelectDataEntity(u => (u.ClientIdentifier == consumerKey));

            // Return the client.
            return nonce;
        }

        /// <summary>
        /// Get the specific client for the current application.
        /// </summary>
        /// <param name="clientID">The clientID.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.Client GetSpecificClient(long clientID)
        {
            // Get the client data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Client clientExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Client();
            Nequeo.DataAccess.CloudInteraction.Data.Client nonce = clientExt.Select.SelectDataEntity(u => (u.ClientID == clientID));

            // Return the client.
            return nonce;
        }

        /// <summary>
        /// Get the specific user for the current application.
        /// </summary>
        /// <param name="userID">The userID.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.User GetSpecificUser(long userID)
        {
            // Get the client data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.User userExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();
            Nequeo.DataAccess.CloudInteraction.Data.User user = userExt.Select.SelectDataEntity(u => (u.UserID == userID));

            // Return the user.
            return user;
        }

        /// <summary>
        /// Get the specific user for the current application.
        /// </summary>
        /// <param name="username">The user username.</param>
        /// <param name="password">The user password.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.User GetSpecificUser(string username, string password)
        {
            // Get the client data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.User userExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();
            Nequeo.DataAccess.CloudInteraction.Data.User user = userExt.Select.SelectDataEntity(u => (u.Username == username) && (u.Password == password));

            // Return the user.
            return user;
        }

        /// <summary>
        /// Get the specific client authorization for the current application.
        /// </summary>
        /// <param name="clientID">The client id.</param>
        /// <param name="nonceData">The nonce data.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.ClientAuthorization GetSpecificClientAuthorization(long clientID, string nonceData)
        {
            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.ClientAuthorization nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.ClientAuthorization();
            Nequeo.DataAccess.CloudInteraction.Data.ClientAuthorization nonce = nonceExt.Select.SelectDataEntity(
                u =>
                    (u.ClientID == clientID) &&
                    (u.NonceCode == nonceData));

            // Return the nonce.
            return nonce;
        }

        /// <summary>
        /// Get the specific nonce for the current application.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="nonceData">The nonce data.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.Nonce GetSpecificNonce(string consumerKey, string nonceData)
        {
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
        /// Get the specific token for the current application.
        /// </summary>
        /// <param name="consumerKey">The consumerKey.</param>
        /// <param name="token">The token.</param>
        /// <returns>The token; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.OAuthToken GetSpecificToken(string consumerKey, string token)
        {
            // Get the token data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken tokenExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken();
            Nequeo.DataAccess.CloudInteraction.Data.OAuthToken oAuthToken = tokenExt.Select.SelectDataEntity(
                u =>
                    (u.Context == consumerKey) &&
                    (u.Token == token));

            // Return the token.
            return oAuthToken;
        }

        /// <summary>
        /// Get the specific token for the current application.
        /// </summary>
        /// <param name="consumerKey">The consumerKey.</param>
        /// <param name="token">The token.</param>
        /// <param name="tokenType">The token type.</param>
        /// <returns>The token; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.OAuthToken GetSpecificToken(string consumerKey, string token, string tokenType)
        {
            // Get the token data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken tokenExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken();
            Nequeo.DataAccess.CloudInteraction.Data.OAuthToken oAuthToken = tokenExt.Select.SelectDataEntity(
                u =>
                    (u.Context == consumerKey) &&
                    (u.TokenType == tokenType) &&
                    (u.Token == token));

            // Return the token.
            return oAuthToken;
        }

        /// <summary>
        /// Get the specific token for the current application.
        /// </summary>
        /// <param name="consumerKey">The consumerKey.</param>
        /// <param name="oAuthConsumerID">The oAuthConsumerID.</param>
        /// <param name="tokenType">The token type.</param>
        /// <returns>The token; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.OAuthToken GetSpecificToken(string consumerKey, long oAuthConsumerID, string tokenType)
        {
            // Get the token data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken tokenExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken();
            Nequeo.DataAccess.CloudInteraction.Data.OAuthToken oAuthToken = tokenExt.Select.SelectDataEntity(
                u =>
                    (u.Context == consumerKey) &&
                    (u.TokenType == tokenType) &&
                    (u.OAuthConsumerID == oAuthConsumerID));

            // Return the token.
            return oAuthToken;
        }

        /// <summary>
        /// Get the specific nonce for the current application.
        /// </summary>
        /// <param name="nonceData">The nonce code.</param>
        /// <returns>The user; else null.</returns>
        protected Nequeo.DataAccess.CloudInteraction.Data.Nonce GetSpecificNonce(string nonceData)
        {
            // Get the nonce data.
            Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce nonceExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Nonce();
            Nequeo.DataAccess.CloudInteraction.Data.Nonce nonce = nonceExt.Select.SelectDataEntity(u => (u.Code == nonceData));

            // Return the nonce.
            return nonce;
        }

        /// <summary>
        /// Generate a random token or verification code.
        /// </summary>
        /// <param name="length">The exact length of the token.</param>
        /// <returns>The radom token generated.</returns>
        protected string GenerateToken(int length = 30)
        {
            // Create a new token.
            Nequeo.Invention.TokenGenerator token = new Invention.TokenGenerator();
            return token.Random(length);
        }

        /// <summary>
        /// Generate the Hex value of the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The hex value of the token</returns>
        protected string GenerateHex(string token)
        {
            // Return the Hex value of the token.
            return Nequeo.Conversion.Context.ByteArrayToHexString(System.Text.Encoding.ASCII.GetBytes(token));
        }
	}
}
