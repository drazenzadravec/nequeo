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

namespace Nequeo.Net
{
    /// <summary>
    /// Web server response.
    /// </summary>
    public class WebResponse : ResponseWebContent
    {
        /// <summary>
        /// Create the web response.
        /// </summary>
        /// <param name="responseOutput">The response output stream containing the raw data.</param>
        /// <returns>The http server response.</returns>
        public static WebResponse Create(System.IO.Stream responseOutput)
        {
            WebResponse webResponse = new WebResponse();
            webResponse.Output = responseOutput;
            return webResponse;
        }

        private string _deli = "\r\n";

        /// <summary>
        /// Gets or sets the output stream.
        /// </summary>
        public System.IO.Stream Output
        {
            get;
            set;
        }

        /// <summary>
        /// Write the response status to the stream.
        /// </summary>
        /// <param name="writeEndOfHeaders">Write the end of the header bytes, carrige return line feed.</param>
        public virtual void WriteWebResponseHeaders(bool writeEndOfHeaders = true)
        {
            byte[] buffer = null;
            string data = "";

            // If chunked is used.
            if (SendChunked)
            {
                AddHeader("Transfer-Encoding", "Chunked");
            }

            // If the server has been specified.
            if (!String.IsNullOrEmpty(Server))
            {
                AddHeader("Server", Server);
            }

            // If content length has been specified.
            if (ContentLength > 0)
            {
                AddHeader("Content-Length", ContentLength.ToString());
            }

            // If the allow has been specified.
            if (!String.IsNullOrEmpty(Allow))
            {
                AddHeader("Allow", Allow);
            }

            // If the content type has been specified.
            if (!String.IsNullOrEmpty(ContentType))
            {
                AddHeader("Content-Type", ContentType);
            }

            // If the Upgrade has been specified.
            if (!String.IsNullOrEmpty(Upgrade))
            {
                // If an upgrade is required
                // then set the connection to upgrade
                // and set the upgrade to the protocol (e.g. WebSocket, HTTP/2.0 .. etc).
                AddHeader("Connection", "Upgrade");
                AddHeader("Upgrade", Upgrade);
            }
            else
            {
                // If the connection is open.
                if (KeepAlive)
                {
                    AddHeader("Connection", "Keep-Alive");
                }
            }

            // If the content encoding has been specified.
            if (!String.IsNullOrEmpty(ContentEncoding))
            {
                AddHeader("Content-Encoding", ContentEncoding);
            }

            // If the content lanaguage has been specified.
            if (!String.IsNullOrEmpty(ContentLanguage))
            {
                AddHeader("Content-Language", ContentLanguage);
            }

            // If authenticate type is other than none.
            if (AuthorizationType != Nequeo.Security.AuthenticationType.None)
            {
                AddHeader("WWW-Authenticate", AuthorizationType.ToString());
            }

            // If protocol version http/2 is used.
            if ((ProtocolVersion.ToLower().Equals("http/2")) || (ProtocolVersion.ToLower().Equals("http/2.0")))
            {
                // Send the http response status.
                data = ":status = " + StatusCode.ToString() + (StatusSubcode > 0 ? "." + StatusSubcode.ToString() : "") + _deli;
                buffer = Encoding.Default.GetBytes(data);
                Write(buffer, 0, buffer.Length);
            }
            else
            {
                // Send the http response status.
                data = ProtocolVersion + " " + StatusCode.ToString() + (StatusSubcode > 0 ? "." + StatusSubcode.ToString() : "") + " " + StatusDescription + _deli;
                buffer = Encoding.Default.GetBytes(data);
                Write(buffer, 0, buffer.Length);
            }

            // If headers exists.
            if (Headers != null)
            {
                // For each header found.
                foreach (string header in Headers.AllKeys)
                {
                    // If protocol version http/2 is used.
                    if ((ProtocolVersion.ToLower().Equals("http/2")) || (ProtocolVersion.ToLower().Equals("http/2.0")))
                    {
                        // Add each header.
                        data = header.ToLower() + " = " + Headers[header] + _deli;
                        buffer = Encoding.Default.GetBytes(data);
                        Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        // Add each header.
                        data = header + ": " + Headers[header] + _deli;
                        buffer = Encoding.Default.GetBytes(data);
                        Write(buffer, 0, buffer.Length);
                    }
                }
            }

            // If cookies exists.
            if (Cookies != null)
            {
                // For each cookie found.
                foreach (Cookie cookie in Cookies)
                {
                    // Make shore the cookie has been set.
                    if (!String.IsNullOrEmpty(cookie.Name) && !String.IsNullOrEmpty(cookie.Value))
                    {
                        // Get the cookie details.
                        data = "Set-Cookie" + ": " + cookie.Name + "=" + cookie.Value +
                               (cookie.Expires != null ? "; Expires=" + cookie.Expires.ToUniversalTime().ToLongDateString() + " " + cookie.Expires.ToUniversalTime().ToLongTimeString() + " GMT" : "") +
                               (!String.IsNullOrEmpty(cookie.Path) ? "; Path=" + cookie.Path : "") +
                               (!String.IsNullOrEmpty(cookie.Domain) ? "; Domain=" + cookie.Domain : "") +
                               (cookie.Version > 0 ? "; Version=" + cookie.Version : "") +
                               (cookie.Secure ? "; Secure" : "") +
                               (cookie.HttpOnly ? "; HttpOnly" : "") +
                               _deli;

                        // Write to the stream.
                        buffer = Encoding.Default.GetBytes(data);
                        Write(buffer, 0, buffer.Length);
                    }
                }
            }

            // Write the end of the headers.
            if (writeEndOfHeaders)
            {
                // Send the header end space.
                data = _deli;
                buffer = Encoding.Default.GetBytes(data);
                Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances 
        /// the current position within this stream by the number of bytes 
        /// written.
        /// </summary>
        /// <param name="buffer">An array of bytes.</param>
        public void Write(byte[] buffer)
        {
            Output.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances 
        /// the current position within this stream by the number of bytes 
        /// written.
        /// </summary>
        /// <param name="buffer">A string of data.</param>
        public void Write(string buffer)
        {
            byte[] data = Encoding.Default.GetBytes(buffer);
            Write(data);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances 
        /// the current position within this stream by the number of bytes 
        /// written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies 
        /// count bytes from buffer to the currentstream.</param>
        /// <param name="offset">The zero-based byte offset in buffer 
        /// at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            Output.Write(buffer, offset, count);
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered 
        /// data to be written to the underlying device.
        /// </summary>
        public void Flush()
        {
            Output.Flush();
        }

        /// <summary>
        /// Closes the current connection and releases the resources.
        /// </summary>
        public void Close()
        {
            System.IO.Stream output = Output;
            if (output != null)
            {
                // Close the stream and connection.
                output.Close();
            }
        }
    }

    /// <summary>
    /// Web server response content.
    /// </summary>
    public class ResponseWebContent
    {
        private string _protocolVersion = "HTTP/1.1";
        private string _statusDescription = "OK";
        private int _statusCode = 200;
        private int _statusSubCode = 0;
        private Nequeo.Security.AuthenticationType _authorizationType = Nequeo.Security.AuthenticationType.None;

        /// <summary>
        /// Gets or sets the authentication credentials for authentication.
        /// </summary>
        public virtual NetworkCredential Credentials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets the response headers.
        /// </summary>
        public virtual NameValueCollection Headers
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
        /// Gets or sets whether the response uses chunked transfer encoding.
        /// </summary>
        public virtual bool SendChunked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the authorization type for the credentials.
        /// </summary>
        public virtual Nequeo.Security.AuthenticationType AuthorizationType
        {
            get { return _authorizationType; }
            set { _authorizationType = value; }
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
        /// Adds the specified header and value to the HTTP headers for this response.
        /// </summary>
        /// <param name="name">The name of the HTTP header to set.</param>
        /// <param name="value">The value for the name header.</param>
        public virtual void AddHeader(string name, string value)
        {
            AppendHeader(name, value);
        }

        /// <summary>
        /// Appends a value to the specified HTTP header to be sent with this response.
        /// </summary>
        /// <param name="name">The name of the HTTP header to append value to.</param>
        /// <param name="value">The value to append to the name header.</param>
        public virtual void AppendHeader(string name, string value)
        {
            // If headers does not exist create the header collection.
            if (Headers == null)
                Headers = new NameValueCollection();

            // If headers exists.
            if (Headers != null)
            {
                // If the header exists.
                if (Headers[name] != null)
                {
                    // Update the header.
                    Headers[name] = value;
                }
                else
                {
                    // Add the header.
                    Headers.Add(name, value);
                }
            }
        }

        /// <summary>
        /// Adds the specified System.Net.Cookie to the collection of cookies for this response.
        /// </summary>
        /// <param name="cookie">The System.Net.Cookie to add to the collection to be sent with this response.</param>
        public virtual void AppendCookie(Cookie cookie)
        {
            // If cookies does not exist create the cookie collection.
            if (Cookies == null)
                Cookies = new CookieCollection();

            // If cookies exists.
            if (Cookies != null)
            {
                // Add the cookie.
                Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Adds or updates a System.Net.Cookie in the collection of cookies sent with this response.
        /// </summary>
        /// <param name="cookie">A System.Net.Cookie for this response.</param>
        public virtual void SetCookie(Cookie cookie)
        {
            // If cookies does not exist create the cookie collection.
            if (Cookies == null)
                Cookies = new CookieCollection();

            // If cookies exists.
            if (Cookies != null)
            {
                // If the cookie exists.
                if (Cookies[cookie.Name] != null)
                {
                    // Update the cookie.
                    Cookie cookieCurrent = Cookies[cookie.Name];
                    cookieCurrent.Comment = cookie.Comment;
                    cookieCurrent.CommentUri = cookie.CommentUri;
                    cookieCurrent.Discard = cookie.Discard;
                    cookieCurrent.Domain = cookie.Domain;
                    cookieCurrent.Expired = cookie.Expired;
                    cookieCurrent.Expires = cookie.Expires;
                    cookieCurrent.HttpOnly = cookie.HttpOnly;
                    cookieCurrent.Path = cookie.Path;
                    cookieCurrent.Port = cookie.Port;
                    cookieCurrent.Secure = cookie.Secure;
                    cookieCurrent.Value = cookie.Value;
                    cookieCurrent.Version = cookie.Version;
                }
                else
                {
                    // Add the cookie.
                    Cookies.Add(cookie);
                }
            }
        }
    }
}
