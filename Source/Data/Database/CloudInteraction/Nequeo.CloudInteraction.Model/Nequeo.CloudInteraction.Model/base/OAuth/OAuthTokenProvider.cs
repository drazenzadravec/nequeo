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
    /// OAuth token provider.
    /// </summary>
    public class OAuthTokenProvider : OAuthContextBase, ITokenStore
	{
        /// <summary>
        /// Gets or sets the access token lifetime (In minutes).
        /// </summary>
        public double AccessTokenLifetime { get; set; }

        /// <summary>
        /// Should consume and use an access token, throwing on failure.
        /// </summary>
        /// <param name="accessContext">The context.</param>
        public void ConsumeAccessToken(IOAuthContext accessContext)
        {
            // Get the access token
            var result = GetSpecificToken(accessContext.ConsumerKey, accessContext.Token);
            if (result != null)
            {
                // If access token has expired.
                if (result.ExpiryDateUtc < DateTime.UtcNow)
                {
                    throw new OAuthException(accessContext, OAuthProblemParameters.TokenExpired, "Access token has expired.");
                }
            }
            else
                throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist", 
                    new Exception("The access token does not exist."));
        }

        /// <summary>
        /// Should consume and use of the request token, throwing on failure.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        public void ConsumeRequestToken(IOAuthContext requestContext)
        {
            // Get the request token
            var result = GetSpecificToken(requestContext.ConsumerKey, requestContext.Token);
            if (result != null)
            {
                // Indicates that the request token has already been consumed.
                if (result.State > 0)
                {
                    throw new OAuthException(requestContext, OAuthProblemParameters.TokenRejected,
                                         "The request token has already been consumed.");
                }
                else
                {
                    string consumerKey = requestContext.ConsumerKey;
                    string token = requestContext.Token;

                    // Update the OAuth token.
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken().
                    Update.UpdateItemPredicate(
                        new Data.OAuthToken()
                        {
                            State = 1
                        }, u =>
                            (u.Context == consumerKey) &&
                            (u.Token == token)
                    );
                }
            }
            else
                throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist", 
                    new Exception("The request token does not exist."));
        }

        /// <summary>
        /// Create an access token using xAuth.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken CreateAccessToken(IOAuthContext context)
        {
            // Get the client for the consumer key
            var token = GetSpecificToken(context.Token);
            if (token != null)
            {
                // Get specific client.
                var client = GetSpecificClient(token.Context);
                context.ConsumerKey = token.Context;
                context.Verifier = token.TokenVerifier;
                context.Version = token.ConsumerVersion;

                // Create the token
                var accessToken = new AccessToken
                {
                    ConsumerKey = token.Context,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(AccessTokenLifetime),
                    Realm = context.Realm,
                    Token = GenerateToken(),
                    TokenSecret = GenerateToken(),
                    UserName = GenerateToken()
                };

                if (!InsertAccessToken(token.OAuthConsumerID, context, accessToken))
                    throw new OAuthException(OAuthProblemParameters.UserRefused, "Internal exception",
                            new Exception("Could not insert token; Internal database exception."));

                // Return the access token.
                return accessToken;
            }
            else
                throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                    new Exception("The token : " + context.Token + " does not exist."));
        }

        /// <summary>
        /// Creates a request token for the consumer.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken CreateRequestToken(IOAuthContext context)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(context.ConsumerKey);
            if (client != null)
            {
                // Create the token
                var requestToken = new RequestToken
                {
                    ConsumerKey = context.ConsumerKey,
                    Realm = context.Realm,
                    Token = GenerateToken(),
                    TokenSecret = GenerateToken(),
                    CallbackUrl = context.CallbackUrl
                };

                // Get the nonce
                var nonce = GetSpecificNonce(context);
                if (nonce != null)
                {
                    // Get the OAuth consumer and insert the access token.
                    var oAuthCosumer = GetSpecificOAuthConsumers(nonce.OAuthConsumerID);
                    if (!InsertRequestToken(oAuthCosumer.OAuthConsumerID, context, requestToken))
                        throw new OAuthException(OAuthProblemParameters.UserRefused, "Internal exception", 
                                new Exception("Could not insert token; Internal database exception."));
                }
                else
                {
                    // Get the oAuthConsumerID.
                    long? oAuthConsumerID = InsertOAuthConsumer(context, client.ClientID);
                    if (oAuthConsumerID != null)
                    {
                        // Insert the nonce.
                        if (!InsertNonce(oAuthConsumerID.Value, context, context.Nonce))
                            throw new OAuthException(OAuthProblemParameters.UserRefused, "Internal exception", 
                                new Exception("Could not insert nonce; Internal database exception."));

                        // Insert the token
                        if (!InsertRequestToken(oAuthConsumerID.Value, context, requestToken))
                            throw new OAuthException(OAuthProblemParameters.UserRefused, "Internal exception", 
                                new Exception("Could not insert token; Internal database exception."));
                    }
                    else
                        throw new OAuthException(OAuthProblemParameters.UserRefused, "Internal exception", 
                                new Exception("Could not insert consumer; The OAuth consumer could not be created."));
                }

                // Return the request token.
                return requestToken;
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + context.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Get the access token associated with a request token.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken GetAccessTokenAssociatedWithRequestToken(IOAuthContext requestContext)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(requestContext.ConsumerKey);
            if (client != null)
            {
                // Get the request token
                var resultRequestToken = GetSpecificToken(requestContext.ConsumerKey, requestContext.Token);
                if (resultRequestToken != null)
                {
                    // Get the access token
                    var resultAccessToken = GetSpecificToken(requestContext.ConsumerKey, resultRequestToken.OAuthConsumerID, "AccessToken");
                    if (resultAccessToken != null)
                    {
                        // Create the token
                        var accessToken = new AccessToken
                        {
                            ConsumerKey = resultAccessToken.Context,
                            ExpiryDate = resultAccessToken.ExpiryDateUtc,
                            Realm = requestContext.Realm,
                            Token = resultAccessToken.Token,
                            TokenSecret = resultAccessToken.TokenSecret
                        };

                        // Return the access token
                        return accessToken;
                    }
                    else
                        throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                            new Exception("The access token does not exist."));
                }
                else
                    throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                        new Exception("The request token does not exist."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + requestContext.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Gets the token secret for the supplied access token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>token secret</returns>
        public string GetAccessTokenSecret(IOAuthContext context)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(context.ConsumerKey);
            if (client != null)
            {
                // Get the access token
                var resultToken = GetSpecificToken(context.ConsumerKey, context.Token);
                if (resultToken != null)
                {
                    // Get the access token secret.
                    return resultToken.TokenSecret;
                }
                else
                    throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                            new Exception("The access token does not exist."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + context.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Returns the callback url that is stored against this token.
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>The callback URL</returns>
        public string GetCallbackUrlForToken(IOAuthContext requestContext)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(requestContext.ConsumerKey);
            if (client != null)
            {
                // Get the request token
                var resultToken = GetSpecificToken(requestContext.ConsumerKey, requestContext.Token);
                if (resultToken != null)
                {
                    // Get the request token callback.
                    return resultToken.TokenCallback;
                }
                else
                    throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                            new Exception("The request token does not exist."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + requestContext.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Gets the token secret for the supplied request token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>token secret</returns>
        public string GetRequestTokenSecret(IOAuthContext context)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(context.ConsumerKey);
            if (client != null)
            {
                // Get the request token
                var resultToken = GetSpecificToken(context.ConsumerKey, context.Token);
                if (resultToken != null)
                {
                    // Get the request token secret.
                    return resultToken.TokenSecret;
                }
                else
                    throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                            new Exception("The request token does not exist."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + context.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Returns the status for a request to access a consumers resources.
        /// </summary>
        /// <param name="accessContext">The context.</param>
        /// <returns>The request for access status</returns>
        public RequestForAccessStatus GetStatusOfRequestForAccess(IOAuthContext accessContext)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(accessContext.ConsumerKey);
            if (client != null)
            {
                // Get the access token
                var resultToken = GetSpecificToken(accessContext.ConsumerKey, accessContext.Token, "RequestToken");
                if (resultToken != null)
                {
                    // Get the specific OAuth consumer.
                    var oAuthConsumer = GetSpecificOAuthConsumers(resultToken.OAuthConsumerID);
                    if (oAuthConsumer != null)
                    {
                        // If the verification code has been set
                        // then grant access.
                        if (!String.IsNullOrEmpty(oAuthConsumer.VerificationCode) && (oAuthConsumer.UserID != null))
                        {
                            // If the verification code stored and passed match then grant access.
                            if (oAuthConsumer.VerificationCode == accessContext.Verifier)
                                return RequestForAccessStatus.Granted;
                            else
                                return RequestForAccessStatus.Denied;
                        }
                        else
                            return RequestForAccessStatus.Denied;
                    }
                    else
                        return RequestForAccessStatus.Unknown;
                }
                else
                    return RequestForAccessStatus.Unknown;
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + accessContext.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Retrieves the verification code for a token
        /// </summary>
        /// <param name="requestContext">The context.</param>
        /// <returns>verification code</returns>
        public string GetVerificationCodeForRequestToken(IOAuthContext requestContext)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(requestContext.ConsumerKey);
            if (client != null)
            {
                // Get the request token
                var resultToken = GetSpecificToken(requestContext.ConsumerKey, requestContext.Token);
                if (resultToken != null)
                {
                    // Get the specific OAuth consumer.
                    var oAuthConsumer = GetSpecificOAuthConsumers(resultToken.OAuthConsumerID);
                    if (oAuthConsumer != null)
                    {
                        // Return the verification code.
                        return oAuthConsumer.VerificationCode;
                    }
                    else
                        throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                                new Exception("The request token does not exist."));
                }
                else
                    throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                            new Exception("The request token does not exist."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + requestContext.ConsumerKey + " does not exist."));
        }

        /// <summary>
        /// Renews the access token.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <returns>Return a new access token with the same oauth_session_handle as the near-expired session token</returns>
        public IToken RenewAccessToken(IOAuthContext requestContext)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(requestContext.ConsumerKey);
            if (client != null)
            {
                // Get the request token
                var resultRequestToken = GetSpecificToken(requestContext.ConsumerKey, requestContext.Token);
                if (resultRequestToken != null)
                {
                    // Get the access token
                    var resultAccessToken = GetSpecificToken(requestContext.ConsumerKey, resultRequestToken.OAuthConsumerID, "AccessToken");
                    if (resultAccessToken != null)
                    {
                        // Create the token
                        var accessToken = new AccessToken
                        {
                            ConsumerKey = resultAccessToken.Context,
                            ExpiryDate = DateTime.UtcNow.AddDays(20),
                            Realm = requestContext.Realm,
                            Token = resultAccessToken.Token,
                            TokenSecret = resultAccessToken.TokenSecret,
                            SessionHandle = requestContext.SessionHandle
                        };

                        string consumerKey = resultAccessToken.Context;
                        string token = resultAccessToken.Token;

                        // Update the OAuth token.
                        new Nequeo.DataAccess.CloudInteraction.Data.Extension.OAuthToken().
                        Update.UpdateItemPredicate(
                            new Data.OAuthToken()
                            {
                                ExpiryDateUtc = accessToken.ExpiryDate
                            }, u =>
                                (u.Context == consumerKey) &&
                                (u.Token == token)
                        );

                        // Return the access token
                        return accessToken;
                    }
                    else
                        throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                            new Exception("The access token does not exist."));
                }
                else
                    throw new OAuthException(OAuthProblemParameters.TokenRejected, "Does not exist",
                            new Exception("The request token does not exist."));
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist",
                    new Exception("The client for consumer key : " + requestContext.ConsumerKey + " does not exist."));
        }
    }
}
