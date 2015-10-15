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

using Nequeo.Net.OAuth;
using Nequeo.Net.OAuth.Framework;

namespace Nequeo.Net.OAuth.Client.Version1
{
    /// <summary>
    /// General OAuth 1.0 client.
    /// </summary>
    public partial class OAuthBase : Nequeo.Net.OAuth.AuthConsumer
    {
        /// <summary>
        /// General OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        public OAuthBase(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri) : 
            this(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, null)
        {
        }

        /// <summary>
        /// General OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        /// <param name="callBackUri">The call back URI</param>
        public OAuthBase(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri, Uri callBackUri) :
            base(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, callBackUri)
        {
            OnCreated();
        }

        /// <summary>
        /// Make a request to the provider for a access token.
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <returns>The access token</returns>
        public virtual IToken RequestAccessToken(IToken requestToken)
        {
            if (requestToken == null) throw new ArgumentNullException("requestToken");

            return base.GetAccessToken(requestToken);
        }

        /// <summary>
        /// Make a request to the provider for a access token.
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="verificationCode">The verification code after succesfull user authorization.</param>
        /// <returns>The access token</returns>
        public virtual IToken RequestAccessToken(IToken requestToken, string verificationCode)
        {
            if (requestToken == null) throw new ArgumentNullException("requestToken");
            if (String.IsNullOrEmpty(verificationCode)) throw new ArgumentNullException("verificationCode");

            return base.GetAccessToken(requestToken, verificationCode);
        }

        /// <summary>
        /// Make a request to the provider for a request token.
        /// </summary>
        /// <returns>The request token.</returns>
        public virtual IToken ProcessRequestToken()
        {
            return base.GetRequestToken();
        }

        /// <summary>
        /// Generate a user authorize url for this token (which can use in a redirect from the current site)
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <param name="callBackUri">the callback URI</param>
        /// <returns>The URL string to the user authorisation web service.</returns>
        public virtual string RequestUserAuthorizationUrl(IToken requestToken, Uri callBackUri)
        {
            if (requestToken == null) throw new ArgumentNullException("requestToken");
            if (callBackUri == null) throw new ArgumentNullException("callBackUri");

            base.CallBackUri = callBackUri;
            return base.GetUserAuthorizationUrl(requestToken, base.CallBackUri);
        }

        /// <summary>
        /// Generate a user authorize url for this token (which can use in a redirect from the current site)
        /// </summary>
        /// <param name="requestToken">The request token</param>
        /// <returns>The URL string to the user authorisation web service.</returns>
        public virtual string RequestUserAuthorizationUrl(IToken requestToken)
        {
            if (requestToken == null) throw new ArgumentNullException("requestToken");

            if (base.CallBackUri != null)
                return base.GetUserAuthorizationUrl(requestToken, base.CallBackUri);
            else
                throw new Exception("A callback Uri has not been sepcified.");
        }

        /// <summary>
        /// Get the resource data from the Url.
        /// </summary>
        /// <param name="resourceEndPointUrl">The resource Url containing the resource data.</param>
        /// <returns>The resource data.</returns>
        public virtual string RequestResource(string resourceEndPointUrl)
        {
            if (String.IsNullOrEmpty(resourceEndPointUrl)) throw new ArgumentNullException("resourceEndPointUrl");

            return base.GetResourceData(new Uri(resourceEndPointUrl));
        }

        /// <summary>
        /// Get the resource data from the Url.
        /// </summary>
        /// <param name="resourceEndPointUrl">The resource Url containing the resource data.</param>
        /// <param name="accessToken">The access token</param>
        /// <returns>The resource data.</returns>
        public virtual string RequestResource(string resourceEndPointUrl, IToken accessToken)
        {
            if (String.IsNullOrEmpty(resourceEndPointUrl)) throw new ArgumentNullException("resourceEndPointUrl");
            if (accessToken == null) throw new ArgumentNullException("accessToken");

            return base.GetResourceData(new Uri(resourceEndPointUrl), accessToken);
        }
    }
}
