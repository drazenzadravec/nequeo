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
using System.Text;

using Nequeo.Net.OAuth2;

namespace Nequeo.Net.OAuth.Client.Version2
{
    /// <summary>
    /// Facebook OAuth 2.0 client.
    /// </summary>
    public partial class Facebook : OAuthBase
    {
        /// <summary>
        /// Facebook OAuth 2.0 client.
        /// </summary>
        /// <param name="consumerIdentifier">The unique consumer identifier.</param>
        /// <param name="consumerSecret">The unique consumer secret.</param>
        public Facebook(string consumerIdentifier, string consumerSecret)
            : this(consumerIdentifier, consumerSecret,
            new Uri("https://graph.facebook.com/oauth/authorize"),
            new Uri("https://graph.facebook.com/oauth/access_token"))
        {
        }

        /// <summary>
        /// Facebook OAuth 2.0 client.
        /// </summary>
        /// <param name="consumerIdentifier">The unique consumer identifier.</param>
        /// <param name="consumerSecret">The unique consumer secret.</param>
        /// <param name="userAuthorizeEndpointUri">The user authorise endpoint uri.</param>
        /// <param name="tokenEndpointUri">The token endpoint uri.</param>
        public Facebook(string consumerIdentifier, string consumerSecret, Uri userAuthorizeEndpointUri, Uri tokenEndpointUri)
            : base(consumerIdentifier, consumerSecret, userAuthorizeEndpointUri, tokenEndpointUri)
        {
            OnCreated();
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <returns>The response data.</returns>
        public string GetMeResource(OAuth2.Consumer.Session.IAuthorizationState state)
        {
            byte[] data = base.RequestResource(state, "https://graph.facebook.com/me");
            return System.Text.Encoding.Default.GetString(data);
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="resourceEndPointUrl">The resource endpoint url.</param>
        /// <returns>The response data.</returns>
        public string GetResource(OAuth2.Consumer.Session.IAuthorizationState state, string resourceEndPointUrl)
        {
            byte[] data = base.RequestResource(state, resourceEndPointUrl);
            return System.Text.Encoding.Default.GetString(data);
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <returns>The response facebook graph.</returns>
        public FacebookGraph GetMeResourceGraph(OAuth2.Consumer.Session.IAuthorizationState state)
        {
            return FacebookGraph.Deserialize(GetMeResource(state));
        }

        /// <summary>
        /// Get the response data from the resource.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="resourceEndPointUrl">The resource endpoint url.</param>
        /// <returns>The response facebook graph.</returns>
        public FacebookGraph GetResourceGraph(OAuth2.Consumer.Session.IAuthorizationState state, string resourceEndPointUrl)
        {
            return FacebookGraph.Deserialize(GetResource(state, resourceEndPointUrl));
        }
    }
}
