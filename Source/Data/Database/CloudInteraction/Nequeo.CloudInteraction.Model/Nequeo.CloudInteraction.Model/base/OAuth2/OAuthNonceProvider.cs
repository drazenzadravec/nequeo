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

namespace Nequeo.DataAccess.CloudInteraction.OAuth2
{
    /// <summary>
    /// OAuth nonce provider.
    /// </summary>
    public class OAuthNonceProvider : OAuthContextBase, INonceStore
	{
        /// <summary>
        /// Stores a given nonce and timestamp.
        /// </summary>
        /// <param name="timestampUtc">The UTC timestamp that together with the nonce string make it unique.</param>
        /// <param name="nonce">A series of random characters.</param>
        /// <param name="context">The context; the consumer key.</param>
        /// <returns>
        /// True if the context+nonce+timestamp (combination) was not previously in the database.
        /// False if the nonce was stored previously with the same timestamp and context.
        /// </returns>
        public bool StoreNonce(DateTime timestampUtc, string nonce, string context)
        {
            // If no context has been passed.
            if (String.IsNullOrEmpty(context))
            {
                // Is the nonce unique.
                return IsNonceUnique(nonce);
            }
            else
            {
                // Get the client for the consumer key
                var client = GetSpecificClient(context);
                if (client != null)
                {
                    // Is the nonce unique.
                    bool uniqueNonce = IsNonceUnique(context, nonce, timestampUtc);
                    if (uniqueNonce)
                    {
                        // Get the oAuthConsumerID.
                        long? oAuthConsumerID = InsertOAuthConsumer(client.ClientID);
                        if (oAuthConsumerID != null)
                        {
                            // Insert the nonce.
                            if (!InsertNonce(oAuthConsumerID.Value, context, nonce))
                                throw new Exception("Could not insert nonce; Internal database exception.");

                            // Insert a new client authorization.
                            if (!InsertClientAuthorization(client.ClientID, nonce))
                                throw new Exception("Could not insert client authorisation; Internal database exception.");
                        }
                        else
                            throw new Exception("Could not insert consumer; The OAuth consumer could not be created.");
                    }

                    // Return the nonce unique indicator.
                    return uniqueNonce;
                }
                else
                    throw new Exception("The client for consumer key : " + context + " does not exist.");
            }
        }

        /// <summary>
        /// Genrate a new nonce.
        /// </summary>
        /// <param name="size">The size of the nonce.</param>
        /// <returns>The nonce data.</returns>
        public string GenerateNonce(int size = 30)
        {
            return base.GenerateToken(size);
        }
    }
}
