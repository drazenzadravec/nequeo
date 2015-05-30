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
using System.Collections;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Webdav
{
    /// <summary>
    /// Encapsulates webdav functions.
    /// </summary>
    public partial class Client
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Client()
        {
            OnCreated();
        }
        #endregion

        #region Private Fields
        private HttpStatusCode _httpStatusCode = HttpStatusCode.OK;
        private string _httpStatusDescription = string.Empty;
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the secure certificate.
        /// </summary>
        public Nequeo.Security.X509Certificate2Info Certificate
        {
            get { return _sslCertificate; }
        }
        
        /// <summary>
        /// Get, the most recent http status code from the server.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _httpStatusCode; }
        }

        /// <summary>
        /// Get, the most recent http status description from the server.
        /// </summary>
        public string StatusDescription
        {
            get { return _httpStatusDescription; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Download data from the host.
        /// </summary>
        /// <param name="adapter">The webdav connection adapter.</param>
        /// <param name="path">The path hierachy resources to get.</param>
        /// <param name="localDestination">The local destination stream to write to.</param>
        public void Get(WebdavConnectionAdapter adapter, string path, System.IO.Stream localDestination)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (localDestination == null) throw new ArgumentNullException("localDestination");

            HttpWebRequest request = null;
            WebResponse response = null;
            Stream responseStream = null;

            try
            {
                // Create the Uri
                Uri uri = CreateUri(adapter, Uri.EscapeDataString(path.TrimStart('/').TrimEnd('/')));

                // Create the request object.
                request = (HttpWebRequest)HttpWebRequest.Create(uri);

                // If a proxy server has been specified.
                if (adapter.Proxy != null)
                    request.Proxy = adapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (adapter.ClientCertificate != null)
                    request.ClientCertificates.Add(adapter.ClientCertificate);

                // If the web dav connection is not
                // anonymous then create a new
                // network credential.
                if (!adapter.IsAnonymousUser)
                    request.Credentials = new NetworkCredential(
                        adapter.UserName, adapter.Password, adapter.Domain);

                request.KeepAlive = false;
                request.Timeout = adapter.TimeOut;
                request.Method = WebRequestMethods.Http.Get.ToString();

                // Override the validation of
                // the server certificate.
                if (adapter.UseSSLConnection && adapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (adapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);

                // Assign the response object from
                // the request server.
                response = (HttpWebResponse)request.GetResponse();

                // Get all the data from the server.
                using (responseStream = response.GetResponseStream())
                {
                    // Write the data to the local stream.
                    Nequeo.IO.Stream.Operation.CopyStream(responseStream, localDestination, response.ContentLength, adapter.ResponseTimeout);

                    // Close stream reader.
                    responseStream.Close();
                }

                // Close the response stream.
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (responseStream != null)
                    responseStream.Dispose();

                if (response != null)
                {
                    _httpStatusCode = ((HttpWebResponse)response).StatusCode;
                    _httpStatusDescription = ((HttpWebResponse)response).StatusDescription;
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Upload data to the host.
        /// </summary>
        /// <param name="adapter">The webdav connection adapter.</param>
        /// <param name="path">The path hierachy resources to put.</param>
        /// <param name="localSource">The local source stream to read from.</param>
        /// <param name="contentLength">The content length of the local source stream.</param>
        public void Put(WebdavConnectionAdapter adapter, string path, System.IO.Stream localSource, long contentLength)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (localSource == null) throw new ArgumentNullException("localSource");

            HttpWebRequest request = null;
            WebResponse response = null;
            Stream requestStream = null;

            try
            {
                // Create the Uri
                Uri uri = CreateUri(adapter, Uri.EscapeDataString(path.TrimStart('/').TrimEnd('/')));

                // Create the request object.
                request = (HttpWebRequest)HttpWebRequest.Create(uri);

                // If a proxy server has been specified.
                if (adapter.Proxy != null)
                    request.Proxy = adapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (adapter.ClientCertificate != null)
                    request.ClientCertificates.Add(adapter.ClientCertificate);

                // If the web dav connection is not
                // anonymous then create a new
                // network credential.
                if (!adapter.IsAnonymousUser)
                    request.Credentials = new NetworkCredential(
                        adapter.UserName, adapter.Password, adapter.Domain);

                request.KeepAlive = false;
                request.Timeout = adapter.TimeOut;
                request.Method = WebRequestMethods.Http.Put.ToString();

                // Override the validation of
                // the server certificate.
                if (adapter.UseSSLConnection && adapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (adapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);

                // Write the request data to the request stream.
                using (requestStream = request.GetRequestStream())
                {
                    // Read the data from the local stream.
                    Nequeo.IO.Stream.Operation.CopyStream(localSource, requestStream, contentLength, adapter.RequestTimeout);
                    requestStream.Close();
                }

                // Assign the response object from
                // the request server.
                response = (HttpWebResponse)request.GetResponse();

                // Close the response stream.
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (requestStream != null)
                    requestStream.Dispose();

                if (response != null)
                {
                    _httpStatusCode = ((HttpWebResponse)response).StatusCode;
                    _httpStatusDescription = ((HttpWebResponse)response).StatusDescription;
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Create a directory.
        /// </summary>
        /// <param name="adapter">The webdav connection adapter.</param>
        /// <param name="directoryPath">The directory path hierachy to search in.</param>
        public void CreateDirectory(WebdavConnectionAdapter adapter, string directoryPath)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (String.IsNullOrEmpty(directoryPath)) throw new ArgumentNullException("directoryPath");

            HttpWebRequest request = null;
            WebResponse response = null;

            try
            {
                // Create the Uri
                Uri uri = CreateUri(adapter, Uri.EscapeDataString(directoryPath.TrimStart('/').TrimEnd('/')));

                // Create the request object.
                request = (HttpWebRequest)HttpWebRequest.Create(uri);

                // If a proxy server has been specified.
                if (adapter.Proxy != null)
                    request.Proxy = adapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (adapter.ClientCertificate != null)
                    request.ClientCertificates.Add(adapter.ClientCertificate);

                // If the web dav connection is not
                // anonymous then create a new
                // network credential.
                if (!adapter.IsAnonymousUser)
                    request.Credentials = new NetworkCredential(
                        adapter.UserName, adapter.Password, adapter.Domain);

                request.KeepAlive = false;
                request.Timeout = adapter.TimeOut;
                request.Method = WebRequestMethods.Http.MkCol.ToString();

                // Override the validation of
                // the server certificate.
                if (adapter.UseSSLConnection && adapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (adapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);

                // Assign the response object from
                // the request server.
                response = (HttpWebResponse)request.GetResponse();

                // Close the response stream.
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (response != null)
                {
                    _httpStatusCode = ((HttpWebResponse)response).StatusCode;
                    _httpStatusDescription = ((HttpWebResponse)response).StatusDescription;
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Delete the resource.
        /// </summary>
        /// <param name="adapter">The webdav connection adapter.</param>
        /// <param name="path">The path hierachy resources to delete.</param>
        public void Delete(WebdavConnectionAdapter adapter, string path)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            HttpWebRequest request = null;
            WebResponse response = null;

            try
            {
                // Create the Uri
                Uri uri = CreateUri(adapter, Uri.EscapeDataString(path.TrimStart('/').TrimEnd('/')));

                // Create the request object.
                request = (HttpWebRequest)HttpWebRequest.Create(uri);

                // If a proxy server has been specified.
                if (adapter.Proxy != null)
                    request.Proxy = adapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (adapter.ClientCertificate != null)
                    request.ClientCertificates.Add(adapter.ClientCertificate);

                // If the web dav connection is not
                // anonymous then create a new
                // network credential.
                if (!adapter.IsAnonymousUser)
                    request.Credentials = new NetworkCredential(
                        adapter.UserName, adapter.Password, adapter.Domain);

                request.KeepAlive = false;
                request.Timeout = adapter.TimeOut;
                request.Method = "DELETE";

                // Override the validation of
                // the server certificate.
                if (adapter.UseSSLConnection && adapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (adapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);

                // Assign the response object from
                // the request server.
                response = (HttpWebResponse)request.GetResponse();

                // Close the response stream.
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (response != null)
                {
                    _httpStatusCode = ((HttpWebResponse)response).StatusCode;
                    _httpStatusDescription = ((HttpWebResponse)response).StatusDescription;
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Get the directory list.
        /// </summary>
        /// <param name="adapter">The webdav connection adapter.</param>
        /// <param name="directoryPath">The directory path hierachy to search in.</param>
        /// <param name="directoryDepth">Recursion depth (directory depth). Set default depth to 1. This would prevent recursion.</param>
        /// <returns>The list of directories and files is any; else empty.</returns>
        public List<string> DirectoryList(WebdavConnectionAdapter adapter, string directoryPath = "/", int? directoryDepth = 1)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (String.IsNullOrEmpty(directoryPath)) throw new ArgumentNullException("directoryPath");

            HttpWebRequest request = null;
            WebResponse response = null;

            // The read and write stream objects
            // for the transfer of data.
            Stream requestStream = null;
            Stream responseStream = null;
            List<string> list = new List<string>();

            try
            {
                // Create the request text.
                StringBuilder propfind = new StringBuilder();
                propfind.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                propfind.Append("<propfind xmlns=\"DAV:\">");
                propfind.Append("  <propname/>");
                propfind.Append("</propfind>");

                // Create the request bytes.
                byte[] bytesReq = Encoding.UTF8.GetBytes(propfind.ToString());
                Uri uri = CreateUri(adapter, Uri.EscapeDataString(directoryPath.TrimStart('/').TrimEnd('/')));

                // Create the request object.
                request = (HttpWebRequest)HttpWebRequest.Create(uri);

                // If a proxy server has been specified.
                if (adapter.Proxy != null)
                    request.Proxy = adapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (adapter.ClientCertificate != null)
                    request.ClientCertificates.Add(adapter.ClientCertificate);

                // If the web dav connection is not
                // anonymous then create a new
                // network credential.
                if (!adapter.IsAnonymousUser)
                    request.Credentials = new NetworkCredential(
                        adapter.UserName, adapter.Password, adapter.Domain);

                request.KeepAlive = false;
                request.Timeout = adapter.TimeOut;
                request.Method = "PROPFIND";
                request.ContentLength = bytesReq.Length;
                request.ContentType = "text/xml";

                // Create a list of headers to pass.
                IDictionary<string, string> headers = new Dictionary<string, string>();
                if (directoryDepth != null)
                    headers.Add("Depth", directoryDepth.ToString());

                // Need to send along headers?
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                        request.Headers.Set(key, headers[key]);
                }

                // Override the validation of
                // the server certificate.
                if (adapter.UseSSLConnection && adapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (adapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);

                // Write the request data to the request stream.
                using (requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytesReq, 0, bytesReq.Length);
                    requestStream.Flush();
                    requestStream.Close();
                }

                // Assign the response object from
                // the request server.
                response = (HttpWebResponse)request.GetResponse();

                // Create a new xml document.
                XmlDocument document = new XmlDocument();

                // Get all the data from the server.
                using (responseStream = response.GetResponseStream())
                {
                    // Load the xml response.
                    document.Load(responseStream);

                    // Close stream reader.
                    responseStream.Close();
                }

                // Get the DAV namesapce.
                XmlNamespaceManager xmlNsManager = new XmlNamespaceManager(document.NameTable);
                xmlNsManager.AddNamespace("d", "DAV:");

                // For each directory node.
                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    // Get each directory and file.
                    XmlNode xmlNode = node.SelectSingleNode("d:href", xmlNsManager);
                    string path = Uri.UnescapeDataString(xmlNode.InnerXml).Trim('/');
                    string dir = path.Replace(Uri.UnescapeDataString(uri.AbsoluteUri), "").Trim('/');

                    // If a directory or file exits.
                    if(!String.IsNullOrEmpty(dir))
                        list.Add(dir);
                }

                // Close the response stream.
                response.Close();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (requestStream != null)
                    requestStream.Dispose();

                if (responseStream != null)
                    responseStream.Dispose();

                if (response != null)
                {
                    _httpStatusCode = ((HttpWebResponse)response).StatusCode;
                    _httpStatusDescription = ((HttpWebResponse)response).StatusDescription;
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Execute a command on the web dav server.
        /// </summary>
        /// <param name="adapter">The webdav connection adapter.</param>
        /// <param name="method">The method to execute.</param>
        /// <param name="requestData">The data to send as a request to the server.</param>
        /// <param name="responseData">The data returned from the server.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="contentLengthRequest">The request data content length.</param>
        /// <param name="contentLengthResponse">The response data content length.</param>
        /// <param name="path">The path hierachy resources to execute.</param>
        /// <param name="headers">The list of headers to include in the request.</param>
        public void Execute(WebdavConnectionAdapter adapter, string method, System.IO.Stream requestData = null, System.IO.Stream responseData = null,
            string contentType = "text/xml", long contentLengthRequest = 0, long contentLengthResponse = 0, string path = null, IDictionary<string, string> headers = null)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (String.IsNullOrEmpty(method)) throw new ArgumentNullException("method");

            HttpWebRequest request = null;
            WebResponse response = null;

            // The read and write stream objects
            // for the transfer of data.
            Stream requestStream = null;
            Stream responseStream = null;

            try
            {
                // Create the Uri
                Uri uri = CreateUri(adapter,
                    (!String.IsNullOrEmpty(path) ?
                    Uri.EscapeDataString(path.TrimStart('/').TrimEnd('/')) : ""));

                // Create the request object.
                request = (HttpWebRequest)HttpWebRequest.Create(uri);

                // If a proxy server has been specified.
                if (adapter.Proxy != null)
                    request.Proxy = adapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (adapter.ClientCertificate != null)
                    request.ClientCertificates.Add(adapter.ClientCertificate);

                // If the web dav connection is not
                // anonymous then create a new
                // network credential.
                if (!adapter.IsAnonymousUser)
                    request.Credentials = new NetworkCredential(
                        adapter.UserName, adapter.Password, adapter.Domain);

                request.KeepAlive = false;
                request.Timeout = adapter.TimeOut;
                request.Method = method;
                request.ContentLength = contentLengthRequest;
                request.ContentType = contentType;

                // Need to send along headers?
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                        request.Headers.Set(key, headers[key]);
                }

                // Override the validation of
                // the server certificate.
                if (adapter.UseSSLConnection && adapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (adapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);

                // If request data stream exists.
                if (requestData != null)
                {
                    // Write the request data to the request stream.
                    using (requestStream = request.GetRequestStream())
                    {
                        // Read the data from the local stream.
                        Nequeo.IO.Stream.Operation.CopyStream(requestData, requestStream, contentLengthRequest, adapter.RequestTimeout);
                        requestStream.Close();
                    }
                }

                // Assign the response object from
                // the request server.
                response = (HttpWebResponse)request.GetResponse();

                // If response data stream exists.
                if (responseData != null)
                {
                    // Get all the data from the server.
                    using (responseStream = response.GetResponseStream())
                    {
                        // Write the data to the local stream.
                        Nequeo.IO.Stream.Operation.CopyStream(responseStream, responseData, contentLengthResponse, adapter.ResponseTimeout);

                        // Close stream reader.
                        responseStream.Close();
                    }
                }

                // Close the response stream.
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (requestStream != null)
                    requestStream.Dispose();

                if (responseStream != null)
                    responseStream.Dispose();
                
                if (response != null)
                {
                    _httpStatusCode = ((HttpWebResponse)response).StatusCode;
                    _httpStatusDescription = ((HttpWebResponse)response).StatusDescription;
                    response.Dispose();
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Create the URI from the adapter.
        /// </summary>
        /// <param name="adapter">The current adapter.</param>
        /// <param name="resource">The trailing resource path.</param>
        /// <returns>The URI to connect to.</returns>
        private Uri CreateUri(WebdavConnectionAdapter adapter, string resource)
        {
            // If the standard ports are not used.
            if (adapter.Port != 80 && adapter.Port != 443)
            {
                // Append the port number.
                return new Uri(adapter.WebdavServer.TrimEnd('/') + ":" + adapter.Port.ToString() + "/" + resource.TrimStart('/'));
            }
            else
            {
                // Create the standard URI.
                return new Uri(adapter.WebdavServer.TrimEnd('/') + "/" + resource.TrimStart('/'));
            }
        }

        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Create a new instance of the x509 certificate 
            // information class.
            _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                certificate as X509Certificate2, chain, sslPolicyErrors);

            // Get the current error level.
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                return false;
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                return false;
            else
            {
                //Remote Certificate Name Mismatch
                System.Security.Policy.Zone z =
                    System.Security.Policy.Zone.CreateFromUrl(((HttpWebRequest)sender).RequestUri.ToString());

                // Get the security zone for
                // the current request URI.
                if (z.SecurityZone == System.Security.SecurityZone.Intranet ||
                    z.SecurityZone == System.Security.SecurityZone.MyComputer ||
                    z.SecurityZone == System.Security.SecurityZone.NoZone ||
                    z.SecurityZone == System.Security.SecurityZone.Trusted)
                    return true;

                // Return false otherwise.
                return false;
            }
        }

        /// <summary>
        /// Certificate override validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidationOverride(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Create a new instance of the x509 certificate 
            // information class.
            _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                certificate as X509Certificate2, chain, sslPolicyErrors);

            return true;
        }
        #endregion
    }
}
