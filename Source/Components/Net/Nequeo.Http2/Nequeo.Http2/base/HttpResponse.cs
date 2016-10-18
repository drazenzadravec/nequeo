/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

using Nequeo.Collections;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Http server response.
    /// </summary>
    public sealed class HttpResponse : ResponseWebContent, IDisposable
    {
        /// <summary>
        /// Http server request.
        /// </summary>
        public HttpResponse() 
        {
            // Create the buffers.
            _output = new Nequeo.IO.Stream.StreamBufferBase();
            ProtocolVersion = "HTTP/2";
        }

        private string _deli = "\r\n";
        private Action<int> _writeComplete = null;
        private Nequeo.IO.Stream.StreamBufferBase _output = null;

        /// <summary>
        /// Gets the response buffer.
        /// </summary>
        internal Nequeo.IO.Stream.StreamBufferBase Output
        {
            get { return _output; }
        }

        /// <summary>
        /// Gets or sets the action when the write is complete.
        /// </summary>
        internal Action<int> WriteComplete
        {
            get { return _writeComplete; }
            set { _writeComplete = value; }
        }

        /// <summary>
        /// Gets the number of bytes in the output stream.
        /// </summary>
        public long Count
        {
            get { return _output.Length; }
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances 
        /// the current position within this stream by the number of bytes 
        /// written.
        /// </summary>
        /// <param name="buffer">An array of bytes.</param>
        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
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

            // Trigger the write complete action.
            if (_writeComplete != null)
                _writeComplete(count);
        }

        /// <summary>
        /// Write the response status and compressed header.
        /// </summary>
        /// <param name="writeEndOfHeaders">Write the end of the header bytes, carrige return line feed.</param>
        /// <param name="writeResponseStatus">Write the response status (:status = 200).</param>
        /// <param name="compressed">Compress the headers.</param>
        /// <param name="headerFrame">Only header frame types are supported.</param>
        public void WriteResponseHeaders(bool writeEndOfHeaders = true, bool writeResponseStatus = true, 
            bool compressed = true, Protocol.OpCodeFrame headerFrame = Protocol.OpCodeFrame.Headers)
        {
            byte[] buffer = null;
            string data = "";
            HeadersList headers = new HeadersList();

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

            // Write response status.
            if (writeResponseStatus)
            {
                // Compress the headers
                if (compressed)
                {
                    // Set the response status.
                    headers.Add(new KeyValuePair<string, string>(":status", StatusCode.ToString() + (StatusSubcode > 0 ? "." + StatusSubcode.ToString() : "")));
                }
                else
                {
                    // Send the http response status.
                    data = ":status = " + StatusCode.ToString() + (StatusSubcode > 0 ? "." + StatusSubcode.ToString() : "") + _deli;
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
                    // Compress the headers
                    if (compressed)
                    {
                        // Add each header.
                        headers.Add(new KeyValuePair<string, string>(header.ToLower(), Headers[header]));
                    }
                    else
                    {
                        // Add each header.
                        data = header.ToLower() + " = " + Headers[header] + _deli;
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
                        // Compress the headers
                        if (compressed)
                        {
                            // Get the cookie details.
                            headers.Add(new KeyValuePair<string, string>("set-cookie",
                                cookie.Name + "=" + cookie.Value +
                               (cookie.Expires != null ? "; Expires=" + cookie.Expires.ToUniversalTime().ToLongDateString() + " " + cookie.Expires.ToUniversalTime().ToLongTimeString() + " GMT" : "") +
                               (!String.IsNullOrEmpty(cookie.Path) ? "; Path=" + cookie.Path : "") +
                               (!String.IsNullOrEmpty(cookie.Domain) ? "; Domain=" + cookie.Domain : "") +
                               (cookie.Version > 0 ? "; Version=" + cookie.Version : "") +
                               (cookie.Secure ? "; Secure" : "") +
                               (cookie.HttpOnly ? "; HttpOnly" : "")
                            ));
                        }
                        else
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
            }

            // Compress the headers
            if (compressed)
            {
                // Compress the headers.
                buffer = Utility.CompressHeaders(headers);
                Write(buffer, 0, buffer.Length);
            }
            else
            {
                // Write the end of the headers.
                if (writeEndOfHeaders)
                {
                    // Send the header end space.
                    data = _deli;
                    buffer = Encoding.Default.GetBytes(data);
                    Write(buffer, 0, buffer.Length);
                }
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_output != null)
                        _output.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _output = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~HttpResponse()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
