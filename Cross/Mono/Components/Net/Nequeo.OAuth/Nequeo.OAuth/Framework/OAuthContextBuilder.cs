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
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;

using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Framework
{
    /// <summary>
    /// OAuth context builder
    /// </summary>
    public class OAuthContextBuilder : IOAuthContextBuilder
    {
        /// <summary>
        /// From Url
        /// </summary>
        /// <param name="httpMethod">The http method</param>
        /// <param name="url">The url</param>
        /// <returns>The OAuth context</returns>
        public IOAuthContext FromUrl(string httpMethod, string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            Uri uri;

            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                throw new ArgumentException(string.Format("Failed to parse url: {0} into a valid Uri instance", url));
            }

            return FromUri(httpMethod, uri);
        }

        /// <summary>
        /// From Uri
        /// </summary>
        /// <param name="httpMethod">The http method</param>
        /// <param name="uri">The uri</param>
        /// <returns>The OAuth context</returns>
        public IOAuthContext FromUri(string httpMethod, Uri uri)
        {
            uri = CleanUri(uri);

            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            if (uri == null) throw new ArgumentNullException("uri");

            return new OAuthContext
            {
                RawUri = uri,
                RequestMethod = httpMethod
            };
        }

        /// <summary>
        /// From Http Request
        /// </summary>
        /// <param name="request">The http request</param>
        /// <returns>The OAuth context</returns>
        public IOAuthContext FromHttpRequest(HttpRequest request)
        {
            return FromHttpRequest(new HttpRequestWrapper(request));
        }

        /// <summary>
        /// From Http Request
        /// </summary>
        /// <param name="request">The http request base</param>
        /// <returns>The OAuth context</returns>
        public IOAuthContext FromHttpRequest(HttpRequestBase request)
        {
            var context = new OAuthContext
            {
                RawUri = CleanUri(request.Url),
                Cookies = CollectCookies(request),
                Headers = GetCleanedNameValueCollection(request.Headers),
                RequestMethod = request.HttpMethod,
                FormEncodedParameters = GetCleanedNameValueCollection(request.Form),
                QueryParameters = GetCleanedNameValueCollection(request.QueryString),
            };

            if (request.InputStream.Length > 0)
            {
                context.RawContent = new byte[request.InputStream.Length];
                request.InputStream.Read(context.RawContent, 0, context.RawContent.Length);
                request.InputStream.Position = 0;
            }

            ParseAuthorizationHeader(request.Headers, context);

            return context;
        }

        /// <summary>
        /// FromW eb Request
        /// </summary>
        /// <param name="request">The http web request.</param>
        /// <param name="rawBody">The raw body</param>
        /// <returns>The OAuth context</returns>
        public IOAuthContext FromWebRequest(HttpWebRequest request, Stream rawBody)
        {
            using (var reader = new StreamReader(rawBody))
            {
                return FromWebRequest(request, reader.ReadToEnd());
            }
        }

        /// <summary>
        /// From Web Request
        /// </summary>
        /// <param name="request">The http web request.</param>
        /// <param name="body">The body</param>
        /// <returns>The OAuth context</returns>
        public IOAuthContext FromWebRequest(HttpWebRequest request, string body)
        {
            var context = new OAuthContext
            {
                RawUri = CleanUri(request.RequestUri),
                Cookies = CollectCookies(request),
                Headers = request.Headers,
                RequestMethod = request.Method
            };

            string contentType = request.Headers[HttpRequestHeader.ContentType] ?? string.Empty;

            if (contentType.ToLower().Contains("application/x-www-form-urlencoded"))
            {
                context.FormEncodedParameters = HttpUtility.ParseQueryString(body);
            }

            ParseAuthorizationHeader(request.Headers, context);

            return context;
        }

        /// <summary>
        /// Get Cleaned Name Value Collection
        /// </summary>
        /// <param name="requestQueryString">Name value collection</param>
        /// <returns>Name value collection</returns>
        static NameValueCollection GetCleanedNameValueCollection(NameValueCollection requestQueryString)
        {
            var nvc = new NameValueCollection(requestQueryString);

            if (nvc.HasKeys())
            {
                nvc.Remove(null);
            }

            return nvc;
        }

        /// <summary>
        /// Clean Uri
        /// </summary>
        /// <param name="uri">The uri</param>
        /// <returns>The uri</returns>
        static Uri CleanUri(Uri uri)
        {
            // this is a fix for OpenSocial platforms sometimes appending an empty query string parameter
            // to their url.

            string originalUrl = uri.OriginalString;
            return originalUrl.EndsWith("&") ? new Uri(originalUrl.Substring(0, originalUrl.Length - 1)) : uri;
        }

        /// <summary>
        /// Collect Cookies
        /// </summary>
        /// <param name="request">The web request</param>
        /// <returns>Name value collection</returns>
        static NameValueCollection CollectCookies(System.Net.WebRequest request)
        {
            return CollectCookiesFromHeaderString(request.Headers[HttpRequestHeader.Cookie]);
        }

        /// <summary>
        /// Collect Cookies
        /// </summary>
        /// <param name="request">The http request base</param>
        /// <returns>Name value collection</returns>
        static NameValueCollection CollectCookies(HttpRequestBase request)
        {
            return CollectCookiesFromHeaderString(request.Headers["Set-Cookie"]);
        }

        /// <summary>
        /// Collect Cookies From Header String
        /// </summary>
        /// <param name="cookieHeader">The cookie header</param>
        /// <returns>Name value collection</returns>
        static NameValueCollection CollectCookiesFromHeaderString(string cookieHeader)
        {
            var cookieCollection = new NameValueCollection();

            if (!string.IsNullOrEmpty(cookieHeader))
            {
                string[] cookies = cookieHeader.Split(';');
                foreach (string cookie in cookies)
                {
                    //Remove the trailing and Leading white spaces
                    string strCookie = cookie.Trim();

                    var reg = new Regex(@"^(\S*)=(\S*)$");
                    if (reg.IsMatch(strCookie))
                    {
                        Match match = reg.Match(strCookie);
                        if (match.Groups.Count > 2)
                        {
                            cookieCollection.Add(match.Groups[1].Value,
                                                 HttpUtility.UrlDecode(match.Groups[2].Value).Replace(' ', '+'));
                            //HACK: find out why + is coming in as " "
                        }
                    }
                }
            }

            return cookieCollection;
        }

        /// <summary>
        /// Parse Authorization Header
        /// </summary>
        /// <param name="headers">The name value headers</param>
        /// <param name="context">The OAuth context</param>
        static void ParseAuthorizationHeader(NameValueCollection headers, OAuthContext context)
        {
            if (headers.AllKeys.Contains("Authorization"))
            {
                context.AuthorizationHeaderParameters = UriUtility.GetHeaderParameters(headers["Authorization"]).ToNameValueCollection();
                context.UseAuthorizationHeader = true;
            }
        }
    }
}
