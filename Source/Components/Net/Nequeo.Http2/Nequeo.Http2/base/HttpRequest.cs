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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Authentication;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Extension;
using Nequeo.Model;
using Nequeo.Model.Message;
using Nequeo.Net.Http2.Protocol;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Http server request.
    /// </summary>
    public sealed class HttpRequest : RequestWebContent, IDisposable
    {
        /// <summary>
        /// Http server request.
        /// </summary>
        public HttpRequest()
        {
            // Create the original header list.
            OriginalHeaders = new List<NameValue>();

            // Create the buffers.
            _input = new Nequeo.IO.Stream.StreamBufferBase();
        }

        private Nequeo.IO.Stream.StreamBufferBase _input = null;

        /// <summary>
        /// Gets the request buffer.
        /// </summary>
        internal Nequeo.IO.Stream.StreamBufferBase Input
        {
            get { return _input; }
        }

        /// <summary>
        /// Gets the number of bytes in the input stream.
        /// </summary>
        public long Count
        {
            get { return _input.Length; }
        }

        /// <summary>
        /// Gets or sets a System.Boolean value that indicates whether the request data is compressed.
        /// </summary>
        public bool IsCompressed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a System.Boolean value that indicates whether the request data is the last set of data.
        /// </summary>
        public bool IsEndOfData
        {
            get;
            set;
        }

        /// <summary>
        /// Parse the form data within the stream, including any uploaded file data.
        /// </summary>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely</param>
        /// <returns>The http form model parsed from the stream; else null.</returns>
        public Nequeo.Net.Http.Common.HttpFormModel ParseForm(long timeout = -1)
        {
            System.IO.BinaryReader reader = null;
            try
            {
                // Create the reader.
                reader = new System.IO.BinaryReader(Input, Encoding.Default, true);
                
                // Return the result.
                return Nequeo.Net.Http.Utility.ParseForm(reader, timeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }

        /// <summary>
        /// Set the request headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="request">The request header.</param>
        public void ReadRequestHeaders(List<NameValue> headers, string request)
        {
            // If headers exist.
            if (headers != null)
            {
                // Get the request method, query and version
                RequestResource resource = Nequeo.Net.Http2.Utility.GetRequestResource(request);
                ReadRequestHeaders(headers, resource);
            }
        }

        /// <summary>
        /// Set the request headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="request">The request header.</param>
        public void ReadRequestHeaders(List<NameValue> headers, RequestResource request)
        {
            // If headers exist.
            if (headers != null)
            {
                // Create the new header collection.
                Headers = new NameValueCollection();

                // For each header found.
                foreach (NameValue item in headers)
                {
                    // Assign the header collection.
                    Headers.Add(item.Name, item.Value);
                }

                // If top level header information exists.
                if (request != null)
                {
                    // Assign the headers.
                    Method = (String.IsNullOrEmpty(request.Method) ? Method : request.Method);
                    ProtocolVersion = (String.IsNullOrEmpty(request.ProtocolVersion) ? ProtocolVersion : request.ProtocolVersion);
                    Path = (String.IsNullOrEmpty(request.Path) ? Path : request.Path);
                    Scheme = (String.IsNullOrEmpty(request.Scheme) ? Scheme : request.Scheme);
                    Authority = (String.IsNullOrEmpty(request.Authority) ? Authority : request.Authority);
                }

                try
                {
                    // Get the Host
                    if (!String.IsNullOrEmpty(Headers["host"]))
                    {
                        // Get the query details.
                        Uri uri = new Uri(Scheme + "://" + Headers["host"] + "/" + Path.TrimStart('/'));
                        Url = uri;
                    }
                }
                catch { }

                // Get the Host
                if (!String.IsNullOrEmpty(Headers["host"]))
                {
                    // Get the content length.
                    Host = Headers["host"].Trim();
                }

                // Get the content length
                if (!String.IsNullOrEmpty(Headers["content-length"]))
                {
                    // Get the content length.
                    ContentLength = Int64.Parse(Headers["content-length"].Trim());
                }

                // Get the list of languages
                if (!String.IsNullOrEmpty(Headers["accept-language"]))
                {
                    // Get the languages.
                    AcceptLanguages = Headers["accept-language"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                }

                // Get the list of encoding
                if (!String.IsNullOrEmpty(Headers["accept-encoding"]))
                {
                    // Get the encoding.
                    AcceptEncoding = Headers["accept-encoding"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                }

                // Get the content type
                if (!String.IsNullOrEmpty(Headers["content-type"]))
                {
                    // Get the content type.
                    ContentType = Headers["content-type"].Trim();
                }

                // Get the connection
                if (!String.IsNullOrEmpty(Headers["connection"]))
                {
                    // Get keep-alive indicator.
                    if (Headers["connection"].Trim().ToLower().Contains("keep-alive"))
                        KeepAlive = true;
                    else
                        KeepAlive = false;

                    // Get the upgrade indicator if it exists.
                    string connUpgrade = Headers["connection"];
                    UpgradeRequest = (connUpgrade.Trim().ToLower().Contains("upgrade") ? true : false);
                }

                // Get the accept
                if (!String.IsNullOrEmpty(Headers["accept"]))
                {
                    // Get the accept.
                    AcceptTypes = Headers["accept"].Split(new string[] { "," }, StringSplitOptions.None).Trim();
                }

                // Get the upgrade
                if (!String.IsNullOrEmpty(Headers["upgrade"]))
                {
                    // Get the upgrade.
                    Upgrade = Headers["upgrade"].Trim();
                }

                // Get the authorization
                if (!String.IsNullOrEmpty(Headers["authorization"]))
                {
                    // Get the authorization.
                    string authorization = Headers["authorization"].Trim();
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
                if (!String.IsNullOrEmpty(Headers["user-agent"]))
                {
                    // Get the user agent.
                    UserAgent = Headers["user-agent"].Trim();
                }

                // Get the cookie
                if (!String.IsNullOrEmpty(Headers["cookie"]))
                {
                    // Create a new collection.
                    CookieCollection CookieCollection = new CookieCollection();

                    // Get all cookies.
                    string[] cookies = Headers["cookie"].Split(new string[] { ";" }, StringSplitOptions.None).Trim();
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
                if (!String.IsNullOrEmpty(Headers["referer"]))
                {
                    try
                    {
                        // Get the referer.
                        UrlReferrer = new Uri(Headers["referer"].Trim());
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
                    if (_input != null)
                        _input.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _input = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~HttpRequest()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
