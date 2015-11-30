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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.Principal;

using Nequeo.Net.Common;
using Nequeo.Handler;
using Nequeo.Net.Configuration;
using Nequeo.ComponentModel.Composition;
using Nequeo.Composite.Configuration;

namespace Nequeo.Net.Server
{
    /// <summary>
    /// Http Xml feed server
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public class HttpXmlFeed : ServiceBase, IDisposable
    {
        /// <summary>
        /// Http Xml feed server
        /// </summary>
        /// <param name="uriList">The url prefix listening list</param>
        /// <param name="providerName">The provider name (host="xmlprovider") in the configuration file.</param>
        public HttpXmlFeed(string[] uriList = null, string providerName = "xmlprovider")
        {
            _uriList = uriList;
            _providerName = providerName;
        }

        private AuthenticationSchemes _authenticationSchemes = AuthenticationSchemes.Anonymous;
        private AuthenticationSchemeSelector _authenticationSchemeSelectorDelegate = null;
        private ClientAuthenticationValidator _clientAuthenticationValidator = null;

        private bool _running = true;
        private HttpListener _listener = null;
        private string[] _uriList = null;
        private string _providerName = "xmlprovider";
        private Nequeo.Net.Data.context _contextMimeType = null;
        private Composition _composition = null;

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) prefixes handled by this System.Net.HttpListener object.
        /// </summary>
        public string[] UriList
        {
            get { return _uriList; }
            set { _uriList = value; }
        }

        /// <summary>
        /// Gets sets the provider name, (host="xmlprovider") in the configuration file.
        /// </summary>
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        /// <summary>
        /// Gets sets the client authentication validator. Used to over ride the ClientValidation method
        /// hehavior, when set the ClientValidation method default behavior is over written.
        /// </summary>
        public ClientAuthenticationValidator ClientAuthenticationValidator
        {
            get { return _clientAuthenticationValidator; }
            set { _clientAuthenticationValidator = value; }
        }

        /// <summary>
        /// Gets or sets the authentication schemes
        /// </summary>
        /// <remarks>
        /// The default value is System.Net.AuthenticationSchemes.Anonymous.
        /// </remarks>
        protected AuthenticationSchemes AuthenticationSchemes
        {
            get { return _authenticationSchemes; }
            set { _authenticationSchemes = value; }
        }

        /// <summary>
        /// Gets or sets the scheme used to authenticate clients.
        /// </summary>
        /// <remarks>
        /// The default value is Null.
        /// </remarks>
        protected AuthenticationSchemeSelector AuthenticationSchemeSelector
        {
            get { return _authenticationSchemeSelectorDelegate; }
            set { _authenticationSchemeSelectorDelegate = value; }
        }

        /// <summary>
        /// Validate the client. Client is only validated if IsAuthenticated = true; all other is false.
        /// Use the ClientAuthenticationValidator delegate to over ride this behavior. Or override this
        /// method in a derived class to change the behavior
        /// </summary>
        /// <param name="user">Defines the basic functionality of a principal object.</param>
        /// <param name="authenticationSchemes">Specifies protocols for authentication.</param>
        /// <returns>True if the client has been validated; else false.</returns>
        protected virtual Boolean ClientValidation(System.Security.Principal.IPrincipal user, AuthenticationSchemes authenticationSchemes)
        {
            if (_clientAuthenticationValidator != null)
            {
                // Custom client authentication valiador.
                return _clientAuthenticationValidator(user, authenticationSchemes);
            }
            else
            {
                // Does the user priciple exist.
                if (user != null)
                {
                    // Does the user identity exist.
                    if (user.Identity != null)
                    {
                        // If the client was validated.
                        if (!user.Identity.IsAuthenticated)
                            return false;
                        else
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Start the http listener.
        /// </summary>
        public void Start()
        {
            try
            {
                // Create a new http listener
                _listener = new HttpListener();

                // Add URI prefixes to listen for.
                foreach (string uri in _uriList)
                    _listener.Prefixes.Add(uri);

                // Set the Authentication Schemes.
                _listener.AuthenticationSchemes = _authenticationSchemes;
                if (_authenticationSchemeSelectorDelegate != null)
                    _listener.AuthenticationSchemeSelectorDelegate = _authenticationSchemeSelectorDelegate;

                // Get the mime types
                _contextMimeType = ReaderHttp.GetMimeType();

                // Load all the composition assemblies.
                _composition = new Composition();
                _composition.Compose();

                // Start the listener
                _listener.Start();

                // Keep the service in the running start
                // listen for in-comming requests.
                while (_running)
                {
                    // Start a new listening thread for the
                    // current connection request.
                    IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(AsynchronousListenerCallback), _listener);

                    // Wait until the current context is made and processed before continuing.
                    result.AsyncWaitHandle.WaitOne();
                }

                // If the runServer flag gets set to false, 
                // stop the server and close the listener.
                _listener.Close();
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
            finally
            {
                if (_listener != null)
                    _listener.Close();

                try
                {
                    _composition.Compose(false);
                    _composition = null;
                }
                catch (Exception ex)
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(
                        ex.Message,
                        MethodInfo.GetCurrentMethod(),
                        Nequeo.Net.Common.Helper.EventApplicationName);
                }
            }
        }

        /// <summary>
        /// Stop the http listener.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Stop the service.
                _running = false;

                if (_listener != null)
                    _listener.Abort();
            }
            catch (Exception ex)
            {
                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
            finally
            {
                if (_listener != null)
                    _listener.Close();
            }
        }

        /// <summary>
        /// Asynchronous listener callback result.
        /// </summary>
        /// <param name="result">The async result for the current connection.</param>
        private void AsynchronousListenerCallback(IAsyncResult result)
        {
            System.IO.Stream output = null;
            HttpListenerRequest request = null;
            HttpListenerResponse response = null;

            try
            {
                // Get the callback state.
                HttpListener listener = (HttpListener)result.AsyncState;

                // Call EndGetContext to signal the completion of the asynchronous operation.
                HttpListenerContext context = null;

                try
                {
                    // If we have aborted the server while waiting, catch the exception and terminate
                    context = listener.EndGetContext(result);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                // If the context is not null.
                if (context != null)
                {
                    // Is there a collection of imported assemblies.
                    if (_composition.HttpServerContext.Count() < 1)
                        throw new Exception("No http composition service assemblies have been loaded.");

                    bool isClientValid = true;

                    // Attempt to validate the client.
                    if (context.User != null)
                        isClientValid = ClientValidation(context.User, _authenticationSchemes);

                    // Get the request and response context.
                    request = context.Request;
                    response = context.Response;

                    // If the user has not been validated.
                    if (!isClientValid)
                    {
                        // Construct a minimal response string.
                        string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html401();
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                        // Get the response OutputStream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "text/html; charset=utf-8";
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        response.AddHeader("Content-Language", "en-au");
                        response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                        response.AddHeader("WWW-Authenticate", _authenticationSchemes.ToString());

                        // Get the current response output stream
                        // and write the response to the client.
                        output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);

                        // Properly flush and close the output stream
                        output.Flush();
                        output.Close();
                    }
                    else
                    {
                        // Get the local file path for the resource request.
                        string urlFilePath = ReaderHttp.GetBaseDirectoryPath() + HttpUtility.UrlDecode(request.Url.AbsolutePath.TrimStart('/').Replace("/", "\\")).TrimEnd('\\');
                        string authMode = ReaderHttp.GetProviderAuthentication(request.Url, _providerName);
                        bool httpServiceExits = false;

                        // Get the current directory.
                        string directory = System.IO.Path.GetDirectoryName(urlFilePath);

                        // If no extension exists.
                        if (!System.IO.Path.HasExtension(urlFilePath))
                            directory = System.IO.Path.GetDirectoryName(urlFilePath + "\\");

                        // Split the request directories and take the last
                        // directory name as the http service metatadata name
                        // to execute.
                        string[] directories = directory.Split(new char[] { '\\' });

                        // Get a http server context instance and clone the instance.
                        Nequeo.Net.Http.IHttpServerContext[] compositeContextServers = _composition.FindCompositeContext(directories, out httpServiceExits);

                        // If the http service does not exist.
                        if (!httpServiceExits)
                        {
                            // Construct a minimal response string.
                            string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html404();
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                            // Get the response OutputStream and write the response to it.
                            response.ContentLength64 = buffer.Length;
                            response.ContentType = "text/html; charset=utf-8";
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            response.AddHeader("Allow", "POST, PUT, GET, HEAD");
                            response.AddHeader("Content-Language", "en-au");
                            response.AddHeader("Server", "Nequeo/2011.26 (Windows)");
                            response.AddHeader("WWW-Authenticate", (String.IsNullOrEmpty(authMode) ? "none" : authMode.ToLower()));

                            // Get the current response output stream
                            // and write the response to the client.
                            output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);

                            // Properly flush and close the output stream
                            output.Flush();
                            output.Close();
                        }
                        else
                        {
                            // If composite servers instance exists.
                            if (compositeContextServers != null)
                            {
                                // If composite servers have been found.
                                if (compositeContextServers.Count() > 0)
                                {
                                    // For each composite server found.
                                    foreach (Nequeo.Net.Http.IHttpServerContext httpServer in compositeContextServers)
                                    {
                                        Nequeo.Net.Http.IHttpServerContext instance = httpServer;
                                        try
                                        {
                                            // Determine if the current request is a post back.
                                            bool isPostBack = false;
                                            if ((request.HttpMethod.ToLower().Contains("post")) || (request.HttpMethod.ToLower().Contains("put")))
                                                isPostBack = true;

                                            // Execute the http service.
                                            ActiveProcessing process = new ActiveProcessing()
                                            {
                                                MimeType = _contextMimeType,
                                                IsPostBack = isPostBack
                                            };

                                            // Execute the http service.
                                            ActiveHttpContext httpContext = new ActiveHttpContext()
                                            {
                                                Request = context.Request,
                                                Response = context.Response,
                                                User = context.User
                                            };

                                            // Create the marshaled server context.
                                            Net.Http.HttpServerContext httpServerContext = new Http.HttpServerContext()
                                            {
                                                HttpContext = httpContext,
                                                ActiveProcess = process
                                            };

                                            // Process the request.
                                            instance.ProcessHttpRequest(httpServerContext);
                                        }
                                        catch (Exception httpServiceError)
                                        {
                                            // Log the error.
                                            LogHandler.WriteTypeMessage(
                                                httpServiceError.Message,
                                                MethodInfo.GetCurrentMethod(),
                                                Nequeo.Net.Common.Helper.EventApplicationName);
                                        }
                                        finally
                                        {
                                            // Releae the http server reference.
                                            instance = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    throw new Exception("No http context: HttpListenerContext");
            }
            catch (Exception ex)
            {
                try
                {
                    if (response != null)
                    {
                        // Construct a minimal response string.
                        string responseString = Nequeo.Net.Http.Common.HttpPageContent.Html500();
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                        // Get the response OutputStream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.ContentType = "text/html; charset=utf-8";
                        response.AddHeader("Content-Language", "en-au");
                        response.AddHeader("Server", "Nequeo/2011.26 (Windows)");

                        // If the response stream has already been activated.
                        if (output == null)
                        {
                            // Get the current response output stream
                            // and write the response to the client.
                            output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                        }
                        else
                            output.Write(buffer, 0, buffer.Length);

                        // Properly flush and close the output stream
                        output.Flush();
                        output.Close();
                    }
                }
                catch (Exception iex)
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(
                        iex.Message,
                        MethodInfo.GetCurrentMethod(),
                        Nequeo.Net.Common.Helper.EventApplicationName);
                }

                // Log the error.
                LogHandler.WriteTypeMessage(
                    ex.Message,
                    MethodInfo.GetCurrentMethod(),
                    Nequeo.Net.Common.Helper.EventApplicationName);
            }
            finally
            {
                try
                {
                    if (output != null)
                        output.Close();
                }
                catch (Exception ex)
                {
                    // Log the error.
                    LogHandler.WriteTypeMessage(
                        ex.Message,
                        MethodInfo.GetCurrentMethod(),
                        Nequeo.Net.Common.Helper.EventApplicationName);
                }
            }
        }
    }
}
