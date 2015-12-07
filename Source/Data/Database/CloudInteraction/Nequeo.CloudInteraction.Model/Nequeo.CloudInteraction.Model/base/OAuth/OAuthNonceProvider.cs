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
    /// OAuth nonce provider.
    /// </summary>
    public class OAuthNonceProvider : OAuthContextBase, INonceStore
	{
        /// <summary>
        /// Will record the nonce and check if it's unique.
        /// </summary>
        /// <param name="consumer">The consumer associated with the nonce</param>
        /// <param name="nonce">The nonce string itself</param>
        /// <returns><c>true</c> if the nonce is unique, <c>false</c> if the nonce has been presented before</returns>
        public bool RecordNonceAndCheckIsUnique(IConsumer consumer, string nonce)
        {
            // Get the client for the consumer key
            var client = GetSpecificClient(consumer.ConsumerKey);
            if (client != null)
            {
                // Is the nonce unique.
                bool uniqueNonce = IsNonceUnique(consumer, nonce);
                if (uniqueNonce)
                {
                    // Get the oAuthConsumerID.
                    long? oAuthConsumerID = InsertOAuthConsumer(client.ClientID);
                    if (oAuthConsumerID != null)
                    {
                        // Insert the nonce.
                        if (!InsertNonce(oAuthConsumerID.Value, consumer, nonce))
                            throw new OAuthException(OAuthProblemParameters.UserRefused, "Internal exception", 
                                new Exception("Could not insert nonce; Internal database exception."));
                    }
                    else
                        throw new Exception("Could not insert consumer; The OAuth consumer could not be created.");
                }

                // Return the nonce unique indicator.
                return uniqueNonce;
            }
            else
                throw new OAuthException(OAuthProblemParameters.ConsumerKeyUnknown, "Does not exist", 
                    new Exception("The client for consumer key : " + consumer.ConsumerKey + " does not exist."));
        }
    }
}
