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
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

using Nequeo.Net.OAuth2;
using Nequeo.Net.OAuth2.Consumer.Session;

namespace Nequeo.Net.OAuth.Client.Version2
{
    /// <summary>
    /// General OAuth 2.0 client.
    /// </summary>
    public partial class OAuthBase : Nequeo.Net.OAuth2.AuthClient
    {
        /// <summary>
        /// General OAuth 2.0 client.
        /// </summary>
        /// <param name="consumerIdentifier">The unique consumer identifier.</param>
        /// <param name="consumerSecret">The unique consumer secret.</param>
        /// <param name="userAuthorizeEndpointUri">The user authorise endpoint uri.</param>
        /// <param name="tokenEndpointUri">The token endpoint uri.</param>
        public OAuthBase(string consumerIdentifier, string consumerSecret, Uri userAuthorizeEndpointUri, Uri tokenEndpointUri)
            : base(consumerIdentifier, consumerSecret, userAuthorizeEndpointUri, tokenEndpointUri)
        {
            OnCreated();
            _authWebClient = base.GetWebClient();
            _authAgentClient = base.GetAgentClient();
        }

        private AuthWebClient _authWebClient = null;
        private AuthAgentClient _authAgentClient = null;

        /// <summary>
        /// Gets the internal OAuth web client.
        /// </summary>
        public AuthWebClient AuthWebClient
        {
            get { return _authWebClient; }
        }

        /// <summary>
        /// Gets the internal OAuth user agent client.
        /// </summary>
        public AuthAgentClient AuthAgentClient
        {
            get { return _authAgentClient; }
        }

        /// <summary>
        /// Processes the authorization response from an authorization server, if available.
        /// </summary>
        /// <param name="authWebClient">The auth web client.</param>
        /// <param name="request">The specific http request.</param>
        /// <returns>The authorization state that contains the details of the authorization.</returns>
        public virtual IAuthorizationState ProcessWebProcessUserAuthorization(AuthWebClient authWebClient = null, HttpRequestBase request = null)
        {
            if (authWebClient != null)
                // Check to see if we're receiving a end user authorization response.
                return authWebClient.ProcessUserAuthorization(request);
            else
                // Check to see if we're receiving a end user authorization response.
                return _authWebClient.ProcessUserAuthorization(request);
        }

        /// <summary>
        /// Prepares a request for user authorization from an authorization server.
        /// </summary>
        /// <param name="authWebClient">The auth web client.</param>
        /// <param name="scopes">The scope of authorized access requested.</param>
        /// <param name="returnTo">The URL the authorization server should redirect the browser (typically on
        /// this site) to when the authorization is completed. If null, the current request's
        /// URL will be used.</param>
        public virtual void RequestWebUserAuthorization(AuthWebClient authWebClient = null, string[] scopes = null, Uri returnTo = null)
        {
            if (authWebClient != null)
                authWebClient.RequestUserAuthorization(scopes, returnTo);
            else
                _authWebClient.RequestUserAuthorization(scopes, returnTo);
        }

        /// <summary>
        /// Refreshes a short-lived access token using a longer-lived refresh token with
        /// a new access token that has the same scope as the refresh token.  The refresh
        /// token itself may also be refreshed.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="authWebClient">The auth web client.</param>
        /// <param name="skipIfUsefulLifeExceeds">If given, the access token will not be refreshed if its remaining lifetime
        /// exceeds this value.</param>
        /// <returns>A value indicating whether the access token was actually renewed; true if
        /// it was renewed, or false if it still had useful life remaining.</returns>
        public virtual bool RefreshWebAuthorization(IAuthorizationState state, AuthWebClient authWebClient = null, TimeSpan? skipIfUsefulLifeExceeds = null)
        {
            if (state == null) throw new ArgumentNullException("state");
            
            if (authWebClient != null)
                return authWebClient.RefreshAuthorization(state, skipIfUsefulLifeExceeds);
            else
                return _authWebClient.RefreshAuthorization(state, skipIfUsefulLifeExceeds);
        }

        /// <summary>
        /// Request the resource data.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="resourceEndPointUrl">The resource endpoint url.</param>
        /// <returns>The array of bytes from the returned data.</returns>
        public virtual byte[] RequestResource(IAuthorizationState state, string resourceEndPointUrl)
        {
            if (state == null) throw new ArgumentNullException("state");
            if (String.IsNullOrEmpty(resourceEndPointUrl)) throw new ArgumentNullException("resourceEndPointUrl");

            // Create the web request.
            System.Net.WebRequest request = System.Net.WebRequest.Create(resourceEndPointUrl + "?access_token=" + Uri.EscapeDataString(state.AccessToken));

            // Create each stream.
            using (System.Net.WebResponse response = request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (MemoryStream writer = new MemoryStream())
            {
                int readBytes = 0;
                const int bufferLength = 0x2000;
                byte[] buffer = new byte[bufferLength];

                // Read all the data in the source stream and
                // write the data to the destination stream.
                do
                {
                    // Read the data and then write the data.
                    readBytes = responseStream.Read(buffer, 0, bufferLength);
                    writer.Write(buffer, 0, readBytes);
                }
                while (readBytes != 0);

                // Close the stream.
                writer.Close();
                responseStream.Close();
                response.Close();

                // Return the array of byte data.
                return writer.ToArray();
            }
        }

        /// <summary>
        /// Request the resource data from the web service instance. (Places the access token into the request header).
        /// </summary>
        /// <typeparam name="T">The return type of the service method.</typeparam>
        /// <typeparam name="TService">The web service type.</typeparam>
        /// <typeparam name="TServiceChannel">The web service interface channel.</typeparam>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="resourceEndPointUrl">The resource endpoint url.</param>
        /// <param name="serviceInstance">The instance of the web service client.</param>
        /// <param name="predicate">The web service method action to execute.</param>
        /// <returns>The type of data to return from the action execution.</returns>
        public virtual T RequestResourceFromService<T, TService, TServiceChannel>(IAuthorizationState state, string resourceEndPointUrl, TService serviceInstance, Func<TService, T> predicate)
            where TService : ClientBase<TServiceChannel>
            where TServiceChannel : class
        {
            if (state == null) throw new ArgumentNullException("state");
            if (String.IsNullOrEmpty(resourceEndPointUrl)) throw new ArgumentNullException("resourceEndPointUrl");
            if (serviceInstance == null) throw new ArgumentNullException("serviceInstance");
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Create a new http request and add the access token
            // to the headers of the request.
            System.Net.HttpWebRequest httpRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(resourceEndPointUrl);
            ClientBase.AuthorizeRequest(httpRequest, state.AccessToken);

            // Add the headers of the request including the access token to the request details.
            HttpRequestMessageProperty httpDetails = new HttpRequestMessageProperty();
            httpDetails.Headers[HttpRequestHeader.Authorization] = httpRequest.Headers[HttpRequestHeader.Authorization];

            // Create a new service scope context.
            using (OperationContextScope scope = new OperationContextScope(serviceInstance.InnerChannel))
            {
                // Add the headers including the access token
                // to the headers of the outgoing web service message.
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpDetails;

                // Execute the web service method action.
                return predicate(serviceInstance);
            }
        }

        /// <summary>
        /// Scans the incoming request for an authorization response message.
        /// </summary>
        /// <param name="actualRedirectUrl">The actual URL of the incoming HTTP request.</param>
        /// <param name="authAgentClient">The auth user agent client.</param>
        /// <param name="authorizationState">The authorization.</param>
        /// <returns>The granted authorization, or null if the incoming HTTP request did not contain
        /// an authorization server response or authorization was rejected.</returns>
        public virtual IAuthorizationState ProcessAgentProcessUserAuthorization(Uri actualRedirectUrl, AuthAgentClient authAgentClient = null, IAuthorizationState authorizationState = null)
        {
            if (actualRedirectUrl == null) throw new ArgumentNullException("actualRedirectUrl");

            if (authAgentClient != null)
                // Check to see if we're receiving a end user authorization response.
                return authAgentClient.ProcessUserAuthorization(actualRedirectUrl, authorizationState);
            else
                // Check to see if we're receiving a end user authorization response.
                return _authAgentClient.ProcessUserAuthorization(actualRedirectUrl, authorizationState);
        }

        /// <summary>
        /// Generates a URL that the user's browser can be directed to in order to authorize
        /// this client to access protected data at some resource server.
        /// </summary>
        /// <param name="authAgentClient">The auth user agent client.</param>
        /// <param name="scopes">The scope of authorized access requested.</param>
        /// <param name="state">The client state that should be returned with the authorization response.</param>
        /// <param name="returnTo">The URL that the authorization response should be sent to via a user-agent redirect.</param>
        public virtual void RequestAgentUserAuthorization(AuthAgentClient authAgentClient = null, string[] scopes = null, string state = null, Uri returnTo = null)
        {
            if (authAgentClient != null)
                authAgentClient.RequestUserAuthorization(scopes, state, returnTo);
            else
                _authAgentClient.RequestUserAuthorization(scopes, state, returnTo);
        }

        /// <summary>
        /// Refreshes a short-lived access token using a longer-lived refresh token with
        /// a new access token that has the same scope as the refresh token.  The refresh
        /// token itself may also be refreshed.
        /// </summary>
        /// <param name="state">Provides access to a persistent object that tracks the state of an authorization.</param>
        /// <param name="authAgentClient">The auth user agent client.</param>
        /// <param name="skipIfUsefulLifeExceeds">If given, the access token will not be refreshed if its remaining lifetime
        /// exceeds this value.</param>
        /// <returns>A value indicating whether the access token was actually renewed; true if
        /// it was renewed, or false if it still had useful life remaining.</returns>
        public virtual bool RefreshAgentAuthorization(IAuthorizationState state, AuthAgentClient authAgentClient = null, TimeSpan? skipIfUsefulLifeExceeds = null)
        {
            if (state == null) throw new ArgumentNullException("state");

            if (authAgentClient != null)
                return authAgentClient.RefreshAuthorization(state, skipIfUsefulLifeExceeds);
            else
                return _authAgentClient.RefreshAuthorization(state, skipIfUsefulLifeExceeds);
        }
    }
}
