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
    /// Web client response.
    /// </summary>
    public class NetResponse : ResponseNetContent
    {
        /// <summary>
        /// Create the web response.
        /// </summary>
        /// <param name="responseInput">The response input stream containing the raw data.</param>
        /// <returns>The web server request.</returns>
        public static NetResponse Create(System.IO.Stream responseInput)
        {
            NetResponse netResponse = new NetResponse();
            netResponse.Input = responseInput;
            return netResponse;
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
        /// Read the response headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="response">The response header.</param>
        public virtual void ReadNetResponseHeaders(List<NameValue> headers, string response)
        {
            // If headers exist.
            if (headers != null)
            {
                // Get the request method, query and version
                ResponseResource resource = Nequeo.Net.Utility.GetResponseResource(response);
                ReadNetResponseHeaders(headers, resource);
            }
        }

        /// <summary>
        /// Read the response headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="response">The response header.</param>
        public virtual void ReadNetResponseHeaders(List<NameValue> headers, ResponseResource response)
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
                    // If the name value pair is a cookie header.
                    if (item.Name.Trim().ToLower().Contains("set-cookie"))
                    {
                        // Add the name value pair.
                        AddCookie(item);
                    }
                    else
                    {
                        // Assign the header collection.
                        headersCol.Add(item.Name, item.Value);
                    }
                }

                // Assign the headers.
                Headers = headersCol;
                StatusCode = response.Code;
                StatusSubcode = response.Subcode;
                ProtocolVersion = response.ProtocolVersion;
                StatusDescription = response.Description;

                // Get the content length
                if (!String.IsNullOrEmpty(headersCol["Content-Length"]))
                {
                    // Get the content length.
                    ContentLength = Int64.Parse(headersCol["Content-Length"].Trim());
                }

                // Get the content type
                if (!String.IsNullOrEmpty(headersCol["Content-Type"]))
                {
                    // Get the content type.
                    ContentType = headersCol["Content-Type"].Trim();
                }

                // Get the server
                if (!String.IsNullOrEmpty(headersCol["Server"]))
                {
                    // Get the server.
                    Server = headersCol["Server"].Trim();
                }

                // Get the Transfer-Encoding chuncked
                if (!String.IsNullOrEmpty(headersCol["Transfer-Encoding"]))
                {
                    // Get keep-alive indicator.
                    if (headersCol["Transfer-Encoding"].Trim().ToLower().Contains("chunked"))
                        SendChunked = true;
                    else
                        SendChunked = false;
                }

                // Get the Allow
                if (!String.IsNullOrEmpty(headersCol["Allow"]))
                {
                    // Get the Allow.
                    Allow = headersCol["Allow"].Trim();
                }

                // Get the Content Encoding
                if (!String.IsNullOrEmpty(headersCol["Content-Encoding"]))
                {
                    // Get the Content Encoding.
                    ContentEncoding = headersCol["Content-Encoding"].Trim();
                }

                // Get the Content Language
                if (!String.IsNullOrEmpty(headersCol["Content-Language"]))
                {
                    // Get the Content Language
                    ContentLanguage = headersCol["Content-Language"].Trim();
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

                // Get the upgrade
                if (!String.IsNullOrEmpty(headersCol["Upgrade"]))
                {
                    // Get the upgrade.
                    Upgrade = headersCol["Upgrade"].Trim();
                }

                // Get the Authorization Type
                if (!String.IsNullOrEmpty(headersCol["WWW-Authenticate"]))
                {
                    // Get the Authorization Type
                    AuthorizationType = headersCol["WWW-Authenticate"].Trim();
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
    /// Web client response content.
    /// </summary>
    public class ResponseNetContent
    {
        private bool _upgradeRequest = false;
        private string _protocolVersion = "HTTP/1.1";
        private string _statusDescription = "OK";
        private int _statusCode = 200;
        private int _statusSubCode = 0;

        /// <summary>
        /// Gets the upgrade request;
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
        /// Gets or sets the response headers.
        /// </summary>
        public virtual NameValueCollection Headers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original response headers.
        /// </summary>
        public virtual List<NameValue> OriginalHeaders
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the request protocol version.
        /// </summary>
        public virtual string ProtocolVersion
        {
            get { return _protocolVersion; }
            set { _protocolVersion = value; }
        }

        /// <summary>
        /// Gets or sets the HTTP status code to be returned to the client.
        /// </summary>
        public virtual int StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        /// <summary>
        /// Gets or sets the HTTP status subcode to be returned to the client.
        /// </summary>
        public virtual int StatusSubcode
        {
            get { return _statusSubCode; }
            set { _statusSubCode = value; }
        }

        /// <summary>
        /// Gets or sets a text description of the HTTP status code returned to the client.
        /// </summary>
        public virtual string StatusDescription
        {
            get { return _statusDescription; }
            set { _statusDescription = value; }
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
        /// Gets or sets the valid actions for a specified resource.
        /// </summary>
        public virtual string Allow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the server requests a persistent connection.
        /// </summary>
        public virtual bool KeepAlive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MIME type of the content returned.
        /// </summary>
        public virtual string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content encoding.
        /// </summary>
        public virtual string ContentEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public virtual string ContentLanguage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name for the server.
        /// </summary>
        public virtual string Server
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
        /// Gets or sets whether the response uses chunked transfer encoding.
        /// </summary>
        public virtual bool SendChunked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cookies sent with the response.
        /// </summary>
        public virtual CookieCollection Cookies
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
        /// Add the cookie header to the collection.
        /// </summary>
        /// <param name="pair">The header name value pair.</param>
        public virtual void AddCookie(NameValue pair)
        {
            // Create a new cookie collection.
            if (Cookies == null)
                Cookies = new CookieCollection();

            // Make sure a value exists.
            if (!String.IsNullOrEmpty(pair.Value))
            {
                // Get the cookie values.
                string[] cookieItems = pair.Value.Split(new string[] { ";" }, StringSplitOptions.None).Trim();

                // Make sure cookies exist.
                if (cookieItems.Length > 0)
                {
                    // Get the name and value of the cookie.
                    string[] cookieNameValue = cookieItems[0].Split(new string[] { "=" }, StringSplitOptions.None).Trim();

                    // Make sure a name exists.
                    if (cookieNameValue.Length > 0)
                    {
                        // Get the cookie name.
                        string cookieName = cookieNameValue[0].Trim();
                        string cookieValue = "";

                        // Get the cookie value if it exists.
                        if (cookieNameValue.Length > 1)
                        {
                            // Get the values
                            string cookieValueItem = "";
                            for (int j = 1; j < cookieNameValue.Length; j++)
                                cookieValueItem += cookieNameValue[j] + "=";

                            // Get the cookie value.
                            cookieValue = cookieValueItem.TrimEnd(new char[] { '=' }).Trim();
                        }

                        try
                        {
                            // Create a new cookie.
                            Cookie cookie = new Cookie(cookieName, cookieValue);

                            // For all other cookie properties
                            for (int i = 1; i < cookieItems.Length; i++)
                            {
                                // Get the cookie name value items.
                                cookieNameValue = cookieItems[i].Split(new string[] { "=" }, StringSplitOptions.None).Trim();

                                // Make sure a property exists.
                                if (cookieNameValue.Length > 0)
                                {
                                    // Get each property.
                                    switch (cookieNameValue[0].Trim().ToLower())
                                    {
                                        case "expires":
                                            // Expires
                                            if (cookieNameValue.Length > 1)
                                            {
                                                try
                                                {
                                                    // Set initial time.
                                                    DateTime expiresDate = DateTime.Parse(cookieNameValue[1].Trim());

                                                    // Set the expiry date.
                                                    cookie.Expires = expiresDate;
                                                }
                                                catch { }
                                            }
                                            break;

                                        case "path":
                                            // Path
                                            if (cookieNameValue.Length > 1)
                                            {
                                                // Set the path.
                                                cookie.Path = cookieNameValue[1].Trim();
                                            }
                                            break;

                                        case "domain":
                                            // Domain
                                            if (cookieNameValue.Length > 1)
                                            {
                                                // Set the domain.
                                                cookie.Domain = cookieNameValue[1].Trim();
                                            }
                                            break;

                                        case "version":
                                            // Version
                                            if (cookieNameValue.Length > 1)
                                            {
                                                try
                                                {
                                                    // Set the version.
                                                    cookie.Version = Int32.Parse(cookieNameValue[1].Trim());
                                                }
                                                catch { }
                                            }
                                            break;

                                        case "port":
                                            // Port
                                            if (cookieNameValue.Length > 1)
                                            {
                                                // Set the port.
                                                cookie.Port = cookieNameValue[1].Trim();
                                            }
                                            break;

                                        case "secure":
                                            // Secure
                                            cookie.Secure = true;
                                            break;

                                        case "httponly":
                                            // HttpOnly
                                            cookie.HttpOnly = true;
                                            break;

                                        case "expired":
                                            // Expired
                                            cookie.Expired = true;
                                            break;

                                        case "discard":
                                            // Discard
                                            cookie.Discard = true;
                                            break;
                                    }
                                }
                            }

                            // Add the cookie to the collection
                            Cookies.Add(cookie);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
