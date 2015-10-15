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
    /// Google OAuth 1.0 client.
    /// </summary>
    public partial class Google : OAuthBase
    {/// <summary>
        /// Google OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        public Google(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri) : 
            this(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, null)
        {
        }

        /// <summary>
        /// Google OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="requestTokenUri">The request token URI</param>
        /// <param name="userAuthorizeUri">The user authorize URI</param>
        /// <param name="accessTokenUri">The access token URI</param>
        /// <param name="callBackUri">The call back URI</param>
        public Google(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri, Uri callBackUri) :
            base(consumerKey, consumerSecret, signatureTypes, requestTokenUri, userAuthorizeUri, accessTokenUri, callBackUri)
        {
            OnCreated();
        }

        /// <summary>
        /// Google OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        public Google(string consumerKey, string consumerSecret, SignatureTypes signatureTypes) :
            this(consumerKey, consumerSecret, signatureTypes, null)
        {
        }

        /// <summary>
        /// Google OAuth 1.0 client.
        /// </summary>
        /// <param name="consumerKey">The unique consumer key.</param>
        /// <param name="consumerSecret">The consumer secret used for signing</param>
        /// <param name="signatureTypes">The signature type.</param>
        /// <param name="callBackUri">The call back URI</param>
        public Google(string consumerKey, string consumerSecret, SignatureTypes signatureTypes, Uri callBackUri) :
            this(consumerKey, consumerSecret, signatureTypes,
            new Uri("https://www.google.com/accounts/OAuthGetRequestToken"),
            new Uri("https://www.google.com/accounts/OAuthAuthorizeToken"),
            new Uri("https://www.google.com/accounts/OAuthGetAccessToken"), 
            callBackUri)
        {
        }

        /// <summary>
        /// The URI to get contacts once authorization is granted.
        /// </summary>
        public readonly string GetContactsEndpoint = "http://www.google.com/m8/feeds/contacts/default/full/";

        /// <summary>
        /// A mapping between Google's applications and their URI scope values.
        /// </summary>
        public static readonly Dictionary<GoogleApplications, string> DataScopeUris = new Dictionary<GoogleApplications, string> {
			{ GoogleApplications.Analytics, "https://www.google.com/analytics/feeds/" },
			{ GoogleApplications.GoogleBase, "http://www.google.com/base/feeds/" },
			{ GoogleApplications.Blogger, "http://www.blogger.com/feeds" },
			{ GoogleApplications.BookSearch, "http://www.google.com/books/feeds/" },
			{ GoogleApplications.Calendar, "http://www.google.com/calendar/feeds/" },
			{ GoogleApplications.Contacts, "http://www.google.com/m8/feeds/" },
			{ GoogleApplications.DocumentsList, "http://docs.google.com/feeds/" },
			{ GoogleApplications.Finance, "http://finance.google.com/finance/feeds/" },
			{ GoogleApplications.Gmail, "https://mail.google.com/mail/feed/atom" },
			{ GoogleApplications.Health, "https://www.google.com/h9/feeds/" },
			{ GoogleApplications.Maps, "http://maps.google.com/maps/feeds/" },
			{ GoogleApplications.OpenSocial, "http://sandbox.gmodules.com/api/" },
			{ GoogleApplications.PicasaWeb, "http://picasaweb.google.com/data/" },
			{ GoogleApplications.Spreadsheets, "http://spreadsheets.google.com/feeds/" },
			{ GoogleApplications.WebmasterTools, "http://www.google.com/webmasters/tools/feeds/" },
			{ GoogleApplications.YouTube, "http://gdata.youtube.com" },
		};
    }
}
