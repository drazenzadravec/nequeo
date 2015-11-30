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

using Nequeo.Net.Data;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Http dynamic server side processing content server single.
    /// </summary>
    public class ServerSingleDynamic : IDisposable
    {
        /// <summary>
        /// Http static content server single.
        /// </summary>
        /// <param name="basePath">The base document path.</param>
        /// <param name="virtualDir">The virtual path to the application directory; for example, "/app".</param>
        /// <param name="configurationFile">The application configuration file (e.g. "[application].config").</param>
        public ServerSingleDynamic(string basePath, string virtualDir = "/", string configurationFile = null) 
        {
            _basePath = basePath;
            _virtualDir = virtualDir;
            _configurationFile = configurationFile;

            // Initialise the server.
            Init();
        }

        private Nequeo.Net.Http.HttpServerSingle _server = null;
        private AppHostDomain _appHostDomain = null;

        private string _configurationFile = null;
        private string _basePath = string.Empty;
        private string _virtualDir = "/";

        /// <summary>
        /// Gets the http server.
        /// </summary>
        public Nequeo.Net.Http.HttpServerSingle HttpServer
        {
            get { return _server; }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                // Start the server.
                if (_server != null)
                    _server.Start();
            }
            catch (Exception)
            {
                if (_server != null)
                    _server.Dispose();

                if (_appHostDomain != null)
                    _appHostDomain.Dispose();

                _server = null;
                _appHostDomain = null;
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
                if (_server != null)
                    _server.Stop();
            }
            catch { }
            finally
            {
                if (_server != null)
                    _server.Dispose();

                if (_appHostDomain != null)
                    _appHostDomain.Dispose();

                _server = null;
                _appHostDomain = null;
            }
        }

        /// <summary>
        /// Initialise the server.
        /// </summary>
        private void Init()
        {
            try
            {
                // Create application host domain.
                _appHostDomain = new AppHostDomain(_basePath, _virtualDir, _configurationFile);

                string socketProviderHostPrefix = "HttpDynamicServerSingle_";
                string hostProviderFullName = socketProviderHostPrefix + "SocketProviderV6";
                string hostProviderFullNameSecure = socketProviderHostPrefix + "SocketProviderV6Ssl";

                // Get the certificate reader.
                Nequeo.Security.Configuration.Reader certificateReader = new Nequeo.Security.Configuration.Reader();
                Nequeo.Net.Configuration.Reader hostReader = new Nequeo.Net.Configuration.Reader();

                // Create the server endpoint.
                Nequeo.Net.Sockets.MultiEndpointModel[] model = new Nequeo.Net.Sockets.MultiEndpointModel[]
                {
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = hostReader.GetServerHost(hostProviderFullName).Port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    },
                    // Secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = hostReader.GetServerHost(hostProviderFullNameSecure).Port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    }
                };

                // Start the server.
                _server = new Nequeo.Net.Http.HttpServerSingle(model, hostReader.GetServerHost(hostProviderFullName).MaxNumClients);
                _server.Name = "Application Server";
                _server.OnHttpContext += HttpContext;
                _server.OnClientConnected = (context) => ClientConnected(context);
                _server.Timeout = hostReader.GetServerHost(hostProviderFullName).ClientTimeOut;
                _server.ReadBufferSize = 32768;
                _server.WriteBufferSize = 32768;

                // Inititalise.
                _server.Initialisation();

                try
                {
                    // Look for the certificate information in the configuration file.

                    // Get the certificate if any.
                    X509Certificate2 serverCertificate = certificateReader.GetServerCredentials();

                    // If a certificate exists.
                    if (serverCertificate != null)
                    {
                        // Get the secure servers.
                        _server.Server[2].UseSslConnection = true;
                        _server.Server[2].X509Certificate = serverCertificate;
                        _server.Server[3].UseSslConnection = true;
                        _server.Server[3].X509Certificate = serverCertificate;
                    }
                }
                catch { }

                // Create the application host.
                _appHostDomain.CreateApplicationHost();
            }
            catch (Exception)
            {
                if (_server != null)
                    _server.Dispose();

                if (_appHostDomain != null)
                    _appHostDomain.Dispose();

                _server = null;
                _appHostDomain = null;
                throw;
            }
        }

        /// <summary>
        /// On client connected.
        /// </summary>
        /// <param name="context">The client context.</param>
        private void ClientConnected(Nequeo.Net.Provider.ISingleContextMux context)
        {
            // If this is a secure connection.
            if (context.UseSslConnection && !context.IsSslAuthenticated)
            {
                try
                {
                    // Start the ssl negotiations.
                    context.BeginSslNegotiation();
                }
                catch { }
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

            System.IO.StreamWriter streamWriter = null;
            System.IO.MemoryStream dynamicStream = null;

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
                response.Server = _server.Name;
                response.StatusCode = 200;
                response.StatusDescription = "OK";
                response.KeepAlive = false;
                response.ProtocolVersion = request.ProtocolVersion;

                // Get the local path.
                string pathBase = _basePath.TrimEnd(new char[] { '\\' }) + "\\";
                string pathLocal = request.Url.LocalPath.Replace("/", "\\").TrimStart(new char[] { '\\' });
                string path = pathBase + pathLocal;
                bool foundDefault = false;

                // If the path not exists.
                if (!System.IO.File.Exists(path))
                {
                    // default.aspx.
                    path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\default.aspx";
                    if (!System.IO.File.Exists(path))
                    {
                        // default.asp.
                        path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\default.asp";
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
                                    // index.aspx.
                                    path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\index.aspx";
                                    if (!System.IO.File.Exists(path))
                                    {
                                        // index.asp.
                                        path = pathBase + pathLocal.TrimEnd(new char[] { '\\' }) + "\\index.asp";
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
                                                else
                                                    foundDefault = true;
                                            }
                                            else
                                                foundDefault = true;
                                        }
                                        else
                                            foundDefault = true;
                                    }
                                    else
                                        foundDefault = true;
                                }
                                else
                                    foundDefault = true;
                            }
                            else
                                foundDefault = true;
                        }
                        else
                            foundDefault = true;
                    }
                    else
                        foundDefault = true;
                }

                // Set the content type.
                response.ContentType = Data.MimeType.GetMimeType(System.IO.Path.GetExtension(path));

                // Is the page type an application type.
                bool isApplicationType = Data.MimeType.IsApplicationType(System.IO.Path.GetExtension(path));

                // If in cache then get the file data.
                if (Data.Helper.FileCache.ContainsKey(path))
                {
                    // Is the page type an application type.
                    if (isApplicationType)
                    {
                        // Run the application host.
                        dynamicStream = new MemoryStream();
                        streamWriter = new StreamWriter(dynamicStream);

                        // Execute the host.
                        _appHostDomain.ProcessRequest(
                            (!foundDefault ? request.Url.LocalPath.Replace("/", "\\").TrimStart(new char[] { '\\' }) : 
                            request.Url.LocalPath.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + Path.GetFileName(path)), 
                            request, streamWriter);

                        // Close the stream.
                        streamWriter.Flush();
                        streamWriter.Close();

                        // Load the request data.
                        responseData = dynamicStream.ToArray();
                    }
                    else
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
                }
                else
                {
                    // Is the page type an application type.
                    if (isApplicationType)
                    {
                        // Run the application host.
                        dynamicStream = new MemoryStream();
                        streamWriter = new StreamWriter(dynamicStream);

                        // Execute the host.
                        _appHostDomain.ProcessRequest(
                            (!foundDefault ? request.Url.LocalPath.Replace("/", "\\").TrimStart(new char[] { '\\' }) :
                            request.Url.LocalPath.Replace("/", "\\").TrimStart(new char[] { '\\' }).TrimEnd(new char[] { '\\' }) + Path.GetFileName(path)),
                            request, streamWriter);

                        // Close the stream.
                        streamWriter.Flush();
                        streamWriter.Close();

                        // Load the request data.
                        responseData = dynamicStream.ToArray();
                    }
                    else
                    {
                        // Load the static file from base.
                        responseData = System.IO.File.ReadAllBytes(path);
                    }

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

                if (streamWriter != null)
                    streamWriter.Dispose();

                if (dynamicStream != null)
                    dynamicStream.Dispose();

                responseData = null;
                requestBuffer = null;
                unzip = null;
                zip = null;
                dynamicStream = null;
                streamWriter = null;
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
                    if (_server != null)
                        _server.Dispose();

                    if (_appHostDomain != null)
                        _appHostDomain.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _server = null;
                _appHostDomain = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ServerSingleDynamic()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
