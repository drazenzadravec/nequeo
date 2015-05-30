/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Handler;
using Nequeo.Extension;
using Nequeo.Net.Configuration;
using Nequeo.Model;
using Nequeo.Model.Message;

namespace Nequeo.Net
{
    /// <summary>
    /// Web server request.
    /// </summary>
    public class WebRequest : RequestWebContent
    {
        /// <summary>
        /// Create the web request.
        /// </summary>
        /// <param name="requestInput">The request input stream containing the raw data.</param>
        /// <returns>The web server request.</returns>
        public static WebRequest Create(System.IO.Stream requestInput)
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Input = requestInput;
            return webRequest;
        }

        /// <summary>
        /// Gets or sets the input stream.
        /// </summary>
        public System.IO.Stream Input
        {
            get;
            set;
        }

        /// <summary>
        /// Set the request headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="request">The request header.</param>
        public virtual void ReadWebRequestHeaders(List<NameValue> headers, string request)
        {
            // If headers exist.
            if (headers != null)
            {
                // Get the request method, query and version
                RequestResource resource = Nequeo.Net.Utility.GetRequestResource(request);
                ReadWebRequestHeaders(headers, resource);
            }
        }

        /// <summary>
        /// Set the request headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="request">The request header.</param>
        public virtual void ReadWebRequestHeaders(List<NameValue> headers, RequestResource request)
        {
            // Get the original headers.
            OriginalHeaders = headers;
                 
            // If headers exist.
            if (headers != null)
            {
                // Create the new header collection.
                NameValueCollection headersCol = new NameValueCollection();

                // For each header found.
                foreach (NameValue item in headers)
                {
                    // Assign the header collection.
                    headersCol.Add(item.Name, item.Value);
                }

                // Assign the headers.
                Headers = headersCol;
                HeadersFound = true;
                Method = request.Method;
                ProtocolVersion = request.ProtocolVersion;
                Path = request.Path;
                Scheme = request.Scheme;

                try
                {
                    // Get the Host
                    if (!String.IsNullOrEmpty(headersCol["Host"]))
                    {
                        // Get the query details.
                        Uri uri = new Uri("http://" + headersCol["Host"] + "/" + Path.TrimStart('/'));
                        Url = uri;
                    }
                }
                catch { }
                
                // Get the Host
                if (!String.IsNullOrEmpty(headersCol["Host"]))
                {
                    // Get the content length.
                    Host = headersCol["Host"].Trim();
                }

                // Get the content length
                if (!String.IsNullOrEmpty(headersCol["Content-Length"]))
                {
                    // Get the content length.
                    ContentLength = Int64.Parse(headersCol["Content-Length"].Trim());
                }

                // Get the list of languages
                if (!String.IsNullOrEmpty(headersCol["Accept-Language"]))
                {
                    // Get the languages.
                    AcceptLanguages = headersCol["Accept-Language"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                }

                // Get the list of encoding
                if (!String.IsNullOrEmpty(headersCol["Accept-Encoding"]))
                {
                    // Get the encoding.
                    AcceptEncoding = headersCol["Accept-Encoding"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                }

                // Get the content type
                if (!String.IsNullOrEmpty(headersCol["Content-Type"]))
                {
                    // Get the content type.
                    ContentType = headersCol["Content-Type"].Trim();
                }

                // Get the connection
                if (!String.IsNullOrEmpty(headersCol["Connection"]))
                {
                    // Get keep-alive indicator.
                    if (headersCol["Connection"].Trim().ToLower().Contains("keep-alive"))
                        KeepAlive = true;
                    else
                        KeepAlive = false;

                    // Get the upgrade indicator if it exists.
                    string connUpgrade = headersCol["Connection"];
                    UpgradeRequest = (connUpgrade.Trim().ToLower().Contains("upgrade") ? true : false);
                }

                // Get the accept
                if (!String.IsNullOrEmpty(headersCol["Accept"]))
                {
                    // Get the accept.
                    AcceptTypes = headersCol["Accept"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                }

                // Get the upgrade
                if (!String.IsNullOrEmpty(headersCol["Upgrade"]))
                {
                    // Get the upgrade.
                    Upgrade = headersCol["Upgrade"].Trim();
                }

                // Get the authorization
                if (!String.IsNullOrEmpty(headersCol["Authorization"]))
                {
                    // Get the authorization.
                    string authorization = headersCol["Authorization"].Trim();
                    string[] authCredential = authorization.Split(new string[] { " " }, StringSplitOptions.None).Trim();

                    // If authorization data exists.
                    if (authCredential.Length > 0)
                    {
                        // Assign the credential.
                        AuthorizationType = authCredential[0].Trim();

                        // If authorization credential data exists.
                        if (authCredential.Length > 1)
                        {
                            byte[] credentialBytes = Convert.FromBase64String(authCredential[1].Trim());
                            string credentialString = Encoding.Default.GetString(credentialBytes);
                            string[] credentialData = credentialString.Split(new string[] { ":" }, StringSplitOptions.None).Trim();

                            // If credentails exists.
                            if (credentialData.Length > 1)
                            {
                                // Create the credentials
                                NetworkCredential credentials = new NetworkCredential(credentialData[0].Trim(), credentialData[1].Trim());
                                Credentials = credentials;
                            }
                        }
                    }
                }

                // Get the user agent
                if (!String.IsNullOrEmpty(Headers["User-Agent"]))
                {
                    // Get the user agent.
                    UserAgent = Headers["User-Agent"].Trim();
                }

                // Get the cookie
                if (!String.IsNullOrEmpty(Headers["Cookie"]))
                {
                    // Create a new collection.
                    CookieCollection CookieCollection = new CookieCollection();

                    // Get all cookies.
                    string[] cookies = Headers["Cookie"].Split(new string[] { ";" }, StringSplitOptions.None).Trim();
                    for (int i = 0; i < cookies.Length; i++)
                    {
                        // Make sure a cookie exists.
                        if (!String.IsNullOrEmpty(cookies[i].Trim()))
                        {
                            // Split the name and value header pair.
                            string[] nameValue = cookies[i].Split(new string[] { "=" }, StringSplitOptions.None).Trim();

                            // Create a new cookie and add to the collection.
                            Cookie cookie = new Cookie(nameValue[0].Trim(), nameValue[1].Trim());
                            CookieCollection.Add(cookie);
                        }
                    }

                    // Assign the collection.
                    Cookies = CookieCollection;
                }

                // Make sure a URI exists.
                if (Url != null)
                {
                    // Make sure a query exists before processing.
                    if (!String.IsNullOrEmpty(Url.Query))
                    {
                        string[] queryCol = Url.Query.TrimStart(new char[] { '?' }).Split(new string[] { "&" }, StringSplitOptions.None);

                        // Create a header name value collection.
                        NameValueCollection queries = new NameValueCollection();

                        // For each query
                        for (int i = 0; i < queryCol.Length; i++)
                        {
                            // Split the name and value header pair.
                            string[] nameValue = queryCol[i].Split(new string[] { "=" }, StringSplitOptions.None);
                            queries.Add(nameValue[0].Trim(), nameValue[1].Trim());
                        }

                        // Assign the queries.
                        Query = queries;
                    }
                }

                // Get the referer
                if (!String.IsNullOrEmpty(Headers["Referer"]))
                {
                    try
                    {
                        // Get the referer.
                        UrlReferrer = new Uri(Headers["Referer"].Trim());
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream 
        /// and advances the position within the stream by the 
        /// number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, 
        /// the buffer contains the specified byte array with the values between 
        /// offset and (offset + count - 1) replaced by the bytes read from the 
        /// current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which 
        /// to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from 
        /// the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be 
        /// less than the number of bytes requested if that many bytes are not 
        /// currently available, or zero (0) if the end of the stream has been 
        /// reached.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            return Input.Read(buffer, offset, count);
        }
    }

    /// <summary>
    /// Web server request content.
    /// </summary>
    public class RequestWebContent
    {
        private bool _upgradeRequest = false;

        /// <summary>
        /// Gets the upgrade request.
        /// </summary>
        public virtual bool UpgradeRequest
        {
            get { return _upgradeRequest; }
            set { _upgradeRequest = value; }
        }

        /// <summary>
        /// Gets or sets the authentication credentials for authentication.
        /// </summary>
        public virtual NetworkCredential Credentials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public virtual NameValueCollection Headers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original request headers.
        /// </summary>
        public virtual List<NameValue> OriginalHeaders
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request method.
        /// </summary>
        public virtual string Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request protocol version.
        /// </summary>
        public virtual string ProtocolVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path;
        /// </summary>
        public virtual string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scheme;
        /// </summary>
        public virtual string Scheme
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the length of the body data included in the request.
        /// </summary>
        public virtual long ContentLength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a System.Boolean value that indicates whether headers have been found.
        /// </summary>
        public virtual bool HeadersFound
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user languages.
        /// </summary>
        public virtual string[] AcceptLanguages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user encoding.
        /// </summary>
        public virtual string[] AcceptEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MIME type of the body data included in the request.
        /// </summary>
        public virtual string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a System.Boolean value that indicates whether the client requests a persistent connection.
        /// </summary>
        public virtual bool KeepAlive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the protocol version if an upgrade is requested for the connection.
        /// </summary>
        public virtual string Upgrade
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the authorization type for the credentials.
        /// </summary>
        public virtual string AuthorizationType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MIME types accepted by the client.
        /// </summary>
        public virtual string[] AcceptTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the host;
        /// </summary>
        public virtual string Host
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user agent presented by the client.
        /// </summary>
        public virtual string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cookies sent with the request.
        /// </summary>
        public virtual CookieCollection Cookies
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public virtual Uri Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request query.
        /// </summary>
        public virtual NameValueCollection Query
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) of the resource that referred the client to the server.
        /// </summary>
        public virtual Uri UrlReferrer
        {
            get;
            set;
        }
    }
}
