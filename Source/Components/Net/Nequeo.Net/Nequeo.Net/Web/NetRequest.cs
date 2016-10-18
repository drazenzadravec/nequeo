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
    /// Web client request.
    /// </summary>
    public class NetRequest : RequestNetContent
    {
        /// <summary>
        /// Create the web request.
        /// </summary>
        /// <param name="requestOutput">The request output stream containing the raw data.</param>
        /// <returns>The http server response.</returns>
        public static NetRequest Create(System.IO.Stream requestOutput)
        {
            NetRequest netRequest = new NetRequest();
            netRequest.Output = requestOutput;
            return netRequest;
        }

        /// <summary>
        /// Create the web request.
        /// </summary>
        /// <param name="requestOutput">The request output stream containing the raw data.</param>
        /// <param name="requestUri">The request uri.</param>
        /// <returns>The http server response.</returns>
        public static NetRequest Create(System.IO.Stream requestOutput, Uri requestUri)
        {
            NetRequest netRequest = new NetRequest();
            netRequest.Output = requestOutput;
            netRequest.Host = requestUri.Host + ":" + requestUri.Port.ToString();
            netRequest.Path = requestUri.PathAndQuery;
            return netRequest;
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
        /// Write the request to the stream.
        /// </summary>
        /// <param name="writeEndOfHeaders">Write the end of the header bytes, carrige return line feed.</param>
        /// <param name="writeResponseStatus">Write the response status (HTTP/1.1 200 OK)</param>
        public virtual void WriteNetRequestHeaders(bool writeEndOfHeaders = true, bool writeResponseStatus = true)
        {
            byte[] buffer = null;
            string data = "";

            // If content length has been specified.
            if (ContentLength > 0)
            {
                AddHeader("Content-Length", ContentLength.ToString());
            }

            // If the Host been specified.
            if (!String.IsNullOrEmpty(Host))
            {
                AddHeader("Host", Host);
            }

            // If the content type has been specified.
            if (!String.IsNullOrEmpty(ContentType))
            {
                AddHeader("Content-Type", ContentType);
            }

            // If the content encoding has been specified.
            if (AcceptEncoding != null)
            {
                AddHeader("Accept-Encoding", String.Join(",", AcceptEncoding));
            }

            // If the content lanaguage has been specified.
            if (AcceptLanguages != null)
            {
                AddHeader("Accept-Language", String.Join(",", AcceptLanguages));
            }

            // If the content accept types has been specified.
            if (AcceptTypes != null)
            {
                AddHeader("Accept", String.Join(",", AcceptTypes));
            }

            // If the UrlReferrer has been specified.
            if (UrlReferrer != null)
            {
                AddHeader("Referer", UrlReferrer.OriginalString);
            }

            // If the upgrade has been specified.
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

            // If the credentials has been specified.
            if (Credentials != null)
            {
                string userAndPassword = Credentials.UserName + ":" + Credentials.Password;
                byte[] credentialBytes = Encoding.Default.GetBytes(userAndPassword);
                string credentialString = Convert.ToBase64String(credentialBytes);
                AddHeader("Authorization", AuthorizationType.ToString() + " " + credentialString);
            }

            // If cookies exist.
            if (Cookies != null)
            {
                string cookieNameValueCol = "";

                // For each cookie found.
                foreach (Cookie cookie in Cookies)
                {
                    // Make shore the cookie has been set.
                    if (!String.IsNullOrEmpty(cookie.Name) && !String.IsNullOrEmpty(cookie.Value))
                    {
                        // Set the name value cookie collection.
                        cookieNameValueCol += cookie.Name + "=" + cookie.Value + ";";
                    }
                }

                // Only set the cookies if a name value exists.
                if (!String.IsNullOrEmpty(cookieNameValueCol))
                {
                    // Add the cookie header.
                    AddHeader("Cookie", cookieNameValueCol.TrimEnd(';'));
                }
            }

            // Write response status.
            if (writeResponseStatus)
            {
                // If protocol version http/2 is used.
                if ((ProtocolVersion.ToLower().Equals("http/2")) || (ProtocolVersion.ToLower().Equals("http/2.0")))
                {
                    // Send the http request.
                    data = ":method = " + Method + _deli + ":scheme = " + Scheme + _deli + ":path = " + Path + _deli;
                    buffer = Encoding.Default.GetBytes(data);
                    Write(buffer, 0, buffer.Length);
                }
                else
                {
                    // Send the http request.
                    data = Method + " " + Path + " " + ProtocolVersion + _deli;
                    buffer = Encoding.Default.GetBytes(data);
                    Write(buffer, 0, buffer.Length);
                }
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
    /// Web client request content.
    /// </summary>
    public class RequestNetContent
    {
        private string _protocolVersion = "HTTP/1.1";
        private string _method = "POST";
        private string _path = "/";
        private string _scheme = "https";
        private Nequeo.Security.AuthenticationType _authorizationType = Nequeo.Security.AuthenticationType.Basic;

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
        /// Gets or sets the request method.
        /// </summary>
        public virtual string Method
        {
            get { return _method; }
            set { _method = value; }
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
        /// Gets or sets the path;
        /// </summary>
        public virtual string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// Gets or sets the scheme;
        /// </summary>
        public virtual string Scheme
        {
            get { return _scheme; }
            set { _scheme = value; }
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
        /// Gets or sets a value indicating whether the server requests a persistent connection.
        /// </summary>
        public virtual bool KeepAlive
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
        /// Gets or sets the MIME types accepted by the client.
        /// </summary>
        public string[] AcceptTypes
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
        /// Gets or sets the protocol version if an upgrade is requested for the connection.
        /// </summary>
        public virtual string Upgrade
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
        /// Gets or sets the authorization type for the credentials.
        /// </summary>
        public virtual Nequeo.Security.AuthenticationType AuthorizationType
        {
            get { return _authorizationType; }
            set { _authorizationType = value; }
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) of the resource that referred the client to the server.
        /// </summary>
        public virtual Uri UrlReferrer
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
        /// Adds the specified header and value to the HTTP headers for this request.
        /// </summary>
        /// <param name="name">The name of the HTTP header to set.</param>
        /// <param name="value">The value for the name header.</param>
        public virtual void AddHeader(string name, string value)
        {
            AppendHeader(name, value);
        }

        /// <summary>
        /// Appends a value to the specified HTTP header to be sent with this request.
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
