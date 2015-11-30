/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Http static content server.
    /// </summary>
    public class Server : IDisposable
    {
        /// <summary>
        /// Http static content server.
        /// </summary>
        /// <param name="basePath">The base document path.</param>
        public Server(string basePath) 
        {
            _basePath = basePath;

            // Initialise the server.
            Init();
        }

        private Nequeo.Net.Http.HttpServer _httpServer = null;
        private string _basePath = string.Empty;

        /// <summary>
        /// Gets the http server.
        /// </summary>
        public Nequeo.Net.Http.HttpServer HttpServer
        {
            get { return _httpServer; }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                // Start the server.
                if (_httpServer != null)
                    _httpServer.Start();
            }
            catch (Exception)
            {
                if (_httpServer != null)
                    _httpServer.Dispose();

                _httpServer = null;
                throw;
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the server.
                if (_httpServer != null)
                    _httpServer.Stop();
            }
            catch { }
            finally
            {
                if (_httpServer != null)
                    _httpServer.Dispose();

                _httpServer = null;
            }
        }

        /// <summary>
        /// Initialise the server.
        /// </summary>
        private void Init()
        {
            try
            {
                // Create the server endpoint.
                Nequeo.Net.Sockets.MultiEndpointModel[] model = new Nequeo.Net.Sockets.MultiEndpointModel[]
                {
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = Nequeo.Net.Properties.Settings.Default.HttpStaticServerListeningPort,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    },
                    // Secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = Nequeo.Net.Properties.Settings.Default.HttpStaticServerListeningPortSecure,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    }
                };

                // Start the server.
                _httpServer = new Nequeo.Net.Http.HttpServer(model);
                _httpServer.Name = "Application Server";
                _httpServer.OnHttpContext += HttpContext;
                _httpServer.Timeout = 30;
                _httpServer.ReadBufferSize = 32768;
                _httpServer.WriteBufferSize = 32768;

                // Get the certificate reader.
                Nequeo.Security.Configuration.Reader certificateReader = new Nequeo.Security.Configuration.Reader();

                // Inititalise.
                _httpServer.Initialisation();

                try
                {
                    // Look for the certificate information in the configuration file.

                    // Get the certificate if any.
                    X509Certificate2 serverCertificate = certificateReader.GetServerCredentials();

                    // If a certificate exists.
                    if (serverCertificate != null)
                    {
                        // Get the secure servers.
                        _httpServer.Server[2].UseSslConnection = true;
                        _httpServer.Server[2].X509Certificate = serverCertificate;
                        _httpServer.Server[3].UseSslConnection = true;
                        _httpServer.Server[3].X509Certificate = serverCertificate;
                    }
                }
                catch { }
            }
            catch (Exception)
            {
                if (_httpServer != null)
                    _httpServer.Dispose();

                _httpServer = null;
                throw;
            }
        }

        /// <summary>
        /// Http context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="context"></param>
        private void HttpContext(object sender, Nequeo.Net.Http.HttpContext context)
        {
            System.IO.MemoryStream unzip = null;
            System.IO.MemoryStream zip = null;

            Nequeo.Net.Http.HttpRequest request = null;
            Nequeo.Net.Http.HttpResponse response = null;

            byte[] responseData = null;
            byte[] requestBuffer = null;

            try
            {
                // Get the context data.
                request = context.HttpRequest;
                response = context.HttpResponse;

                // if request context exists.
                if (request.ContentLength > 0)
                {
                    // Read the data.
                    requestBuffer = new byte[(int)request.ContentLength];
                    request.Read(requestBuffer, 0, (int)request.ContentLength);
                }

                // Set the response.
                response.Server = _httpServer.Name;
                response.StatusCode = 200;
                response.StatusDescription = "OK";
                response.KeepAlive = false;
                response.ProtocolVersion = request.ProtocolVersion;

                // Get the local path.
                string pathBase = _basePath.TrimEnd(new char[] { '\\' }) + "\\";
                string pathLocal = request.Url.LocalPath.Replace("/", "\\").TrimStart(new char[] { '\\' });
                string path = pathBase + pathLocal;

                // If the path not exists.
                if (!System.IO.File.Exists(path))
                {
                    // default.htm.
                    path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\default.htm";
                    if (!System.IO.File.Exists(path))
                    {
                        // default.html.
                        path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\default.html";
                        if (!System.IO.File.Exists(path))
                        {
                            // index.htm.
                            path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\index.htm";
                            if (!System.IO.File.Exists(path))
                            {
                                // index.html.
                                path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\index.html";
                                if (!System.IO.File.Exists(path))
                                    throw new Exception("Resource can not be found.");
                            }
                        }
                    }
                }

                // Set the content type.
                response.ContentType = Data.MimeType.GetMimeType(System.IO.Path.GetExtension(path));

                // If in cache then get the file data.
                if (Data.Helper.FileCache.ContainsKey(path))
                {
                    // If in the cache then check to see if the file has changed.
                    FileInfo fileInfo = new System.IO.FileInfo(path);

                    // If the file has changed, then reload the file.
                    if ((fileInfo.Length != Data.Helper.FileCache.GetFileSize(path)) || Data.Helper.FileCache.HasBeenModified(path, fileInfo.LastWriteTime))
                    {
                        // Load the static file from base.
                        responseData = System.IO.File.ReadAllBytes(path);

                        // Set the new file data.
                        Data.Helper.FileCache.Set(path, responseData);
                        Data.Helper.FileCache.SetModifiedTime(path, fileInfo.LastWriteTime);
                    }
                    else
                    {
                        // Load from the cache.
                        responseData = Data.Helper.FileCache.Get(path);
                    }
                }
                else
                {
                    // Load the static file from base.
                    responseData = System.IO.File.ReadAllBytes(path);

                    // If cache size has not been reached.
                    if (Data.Helper.FileCache.GetCacheSize() <= Nequeo.Net.Properties.Settings.Default.HttpStaticMaxCacheSize)
                    {
                        // Add the file data to the cache.
                        Data.Helper.FileCache.Add(path, responseData);
                        Data.Helper.FileCache.SetModifiedTime(path, DateTime.Now);
                    }
                }
            }
            catch
            {
                try
                {
                    if (response != null)
                    {
                        response.ContentType = "text/html";
                        response.ContentLength = 0;
                        response.StatusCode = 400;
                        response.StatusDescription = "Bad Request";

                        Nequeo.Net.Http.Common.HttpStatusCode statusCode = Nequeo.Net.Http.Utility.GetStatusCode(400);
                        responseData = Encoding.Default.GetBytes("<html>" + statusCode.HtmlHead + statusCode.HtmlBody + "</html>");
                    }
                }
                catch { }
            }

            try
            {
                // If requesting gzip data.
                if (request.AcceptEncoding != null && request.AcceptEncoding.Contains("gzip"))
                {
                    // Create the compression.
                    unzip = new System.IO.MemoryStream(responseData);
                    zip = new System.IO.MemoryStream();
                    Nequeo.IO.Compression.GZipStream.Compress(unzip, zip);

                    // Send the data.
                    response.ContentLength = zip.Length;
                    response.ContentEncoding = "gzip";
                    response.WriteHttpHeaders();
                    response.Write(zip.ToArray(), 0, (int)zip.Length);
                }
                else
                {
                    response.ContentLength = responseData.Length;
                    response.WriteHttpHeaders();
                    response.Write(responseData, 0, (int)responseData.Length);
                }

                response.Flush();
            }
            catch { }
            finally
            {
                if (unzip != null)
                    unzip.Dispose();

                if (zip != null)
                    zip.Dispose();

                responseData = null;
                requestBuffer = null;
                unzip = null;
                zip = null;
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
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
        /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
        /// equals true, the method has been called directly or indirectly by a user's
        /// code. Managed and unmanaged resources can be disposed.  If disposing equals
        /// false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can
        /// be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
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
                    if (_httpServer != null)
                        _httpServer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _httpServer = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Server()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
