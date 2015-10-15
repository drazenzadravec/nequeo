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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Nequeo.Net.OAuth;
using Nequeo.Net.OAuth.Framework;

namespace Nequeo.Net.OAuth.Client.Version1
{
    /// <summary>
    /// Twitter OAuth 1.0 client.
    /// </summary>
    public partial class Twitter : OAuthBase
    {
        /// <summary>
        /// Twitter OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        public Twitter(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri) : 
            this(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, null)
        {
        }

        /// <summary>
        /// Twitter OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        /// <param name="callBackUri">The call back URI</param>
        public Twitter(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri, Uri callBackUri) :
            base(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, callBackUri)
        {
            OnCreated();

            // Twitter can't handle the Expect 100 Continue HTTP header. 
            ServicePointManager.FindServicePoint(new Uri(GetFavoritesEndpoint)).Expect100Continue = false;
        }

        /// <summary>
        /// Twitter OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        public Twitter(string consumerKey, string consumerSecret, SignatureTypes signatureTypes) :
            this(consumerKey, consumerSecret, signatureTypes, null)
        {
        }

        /// <summary>
        /// Twitter OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="callBackUri">The call back URI</param>
        public Twitter(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri callBackUri) :
            this(consumerKey, consumerSecret, signatureTypes,
            new Uri("http://twitter.com/oauth/request_token"),
            new Uri("http://twitter.com/oauth/authorize"),
            new Uri("http://twitter.com/oauth/access_token"), 
            callBackUri)
        {
        }

        /// <summary>
        /// The URI to get a user favorites.
        /// </summary>
        public readonly string GetFavoritesEndpoint = "http://twitter.com/favorites.xml";

        /// <summary>
        /// The URI to get the data on the user home page.
        /// </summary>
        public readonly string GetFriendTimelineStatusEndpoint = "http://twitter.com/statuses/friends_timeline.xml";

        /// <summary>
        /// The URI to get a user update profile background image.
        /// </summary>
        public readonly string UpdateProfileBackgroundImageEndpoint = "http://twitter.com/account/update_profile_background_image.xml";

        /// <summary>
        /// The URI to get a update profile image.
        /// </summary>
        public readonly string UpdateProfileImageEndpoint = "http://twitter.com/account/update_profile_image.xml";

        /// <summary>
        /// The URI to get a verify credentials.
        /// </summary>
        public readonly string VerifyCredentialsEndpoint = "http://api.twitter.com/1/account/verify_credentials.xml";

        /// <summary>
        /// Get friend timeline status
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>The xml document.</returns>
        public XDocument GetFriendTimelineStatus(IToken accessToken)
        {
            string response = base.RequestResource(GetFriendTimelineStatusEndpoint, accessToken);
            return XDocument.Load(new StringReader(response));
        }

        /// <summary>
        /// Get favorites.
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>The xml document.</returns>
        public XDocument GetFavorites(IToken accessToken)
        {
            string response = base.RequestResource(GetFavoritesEndpoint, accessToken);
            return XDocument.Load(new StringReader(response));
        }

        /// <summary>
        /// Get verify credentials.
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>The xml document.</returns>
        public XDocument VerifyCredentials(IToken accessToken)
        {
            string response = base.RequestResource(VerifyCredentialsEndpoint, accessToken);
            return XDocument.Load(new StringReader(response));
        }

        /// <summary>
        /// Get username.
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>Theusername.</returns>
        public string GetUsername(IToken accessToken)
        {
            XDocument xml = VerifyCredentials(accessToken);
            XPathNavigator nav = xml.CreateNavigator();
            return nav.SelectSingleNode("/user/screen_name").Value;
        }
    }
}
