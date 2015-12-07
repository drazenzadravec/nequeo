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
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web.Hosting;
using System.Web;

using Nequeo.Handler;
using Nequeo.Composite.Configuration;

namespace Nequeo.Service.Message
{
    /// <summary>
    /// REST message service provider.
    /// </summary>
    /// <example>
    /// [ServiceContract()]
    /// [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    /// </example>
    public abstract class RestFull
    {
        /// <summary>
        /// REST message service provider.
        /// </summary>
        /// <param name="isWebServer">Is this abstract class being used within an IIS server; 
        /// else this is within a self hosting service application.</param>
        protected RestFull(bool isWebServer = true)
        {
            _isWebServer = isWebServer;
        }

        private bool _isWebServer = true;
        private WebOperationContext _context = null;
        private Uri _requestUri = null;
        private NameValueCollection _queryString = null;
        private NameValueCollection _form = null;
        private NameValueCollection _queryBody = null;
        private long _contentLength = 0;
        private string _contentType = null;
        private string _method = null;
        private byte[] _body = null;

        /// <summary>
        /// Gets the current Web operation context.
        /// </summary>
        protected WebOperationContext Context { get { return _context; } }

        /// <summary>
        /// Gets and sets the matched URI.
        /// </summary>
        protected Uri RequestUri { get { return _requestUri; } }

        /// <summary>
        /// Gets a collection of query string parameters and their values.
        /// </summary>
        protected NameValueCollection QueryString { get { return _queryString; } }

        /// <summary>
        /// Gets a collection of form variables.
        /// </summary>
        protected NameValueCollection Form { get { return _form; } }

        /// <summary>
        /// Gets a collection of query variables within the body.
        /// </summary>
        protected NameValueCollection QueryBody { get { return _queryBody; } }

        /// <summary>
        /// Gets the ContentLength header value of the incoming Web request.
        /// </summary>
        protected long ContentLength { get { return _contentLength; } }

        /// <summary>
        /// Gets the ContentType header value from the incoming Web request.
        /// </summary>
        protected string ContentType { get { return _contentType; } }

        /// <summary>
        /// Gets the HTTP method of the incoming Web request.
        /// </summary>
        protected string Method { get { return _method; } }

        /// <summary>
        /// Gets the message body of the incoming Web request.
        /// </summary>
        protected byte[] Body { get { return _body; } }

        /// <summary>
        /// Gets the message body of the incoming Web request.
        /// </summary>
        protected string BodyString 
        { 
            get 
            {
                if (_body != null)
                    return Encoding.Default.GetString(_body);
                else
                    return "";
            } 
        }

        /// <summary>
        /// Gets sets the current error.
        /// </summary>
        public abstract Exception Exception { get; set; }

        /// <summary>
        /// Compose the MEF instance.
        /// </summary>
        public abstract void Compose();

        /// <summary>
        /// Get the web operation context.
        /// </summary>
        /// <param name="webOperationContext">A helper class that provides easy access 
        /// to contextual properties of Web requests and responses.</param>
        /// <param name="method">The matched URI.</param>
        /// <param name="requestUri">The matched URI.</param>
        /// <param name="queryString">The collection of query string parameters and their values.</param>
        /// <param name="form">The collection of form variables.</param>
        public abstract void WebContext(WebOperationContext webOperationContext, string method, Uri requestUri, NameValueCollection queryString, NameValueCollection form);

        /// <summary>
        /// Method invoke request implementation.
        /// </summary>
        /// <param name="message">The service channel message.</param>
        /// <returns>The result callback.</returns>
        /// <example>
        /// [OperationContract()]
        /// [WebInvoke(Method = "POST")]
        /// </example>
        public abstract System.ServiceModel.Channels.Message WebInvokeRequest(System.ServiceModel.Channels.Message message);

        /// <summary>
        /// Method invoke request get implementation.
        /// </summary>
        /// <param name="message">The service channel message.</param>
        /// <returns>The result callback.</returns>
        /// <example>
        /// [OperationContract()]
        /// [WebInvoke(Method = "GET")]
        /// </example>
        public abstract System.ServiceModel.Channels.Message WebInvokeRequestGet(System.ServiceModel.Channels.Message message);

        /// <summary>
        /// Method invoke implementation.
        /// </summary>
        /// <param name="message">The service channel message.</param>
        /// <example>
        /// [OperationContract()]
        /// [WebInvoke(Method = "POST")]
        /// </example>
        public abstract void WebInvoke(System.ServiceModel.Channels.Message message);

        /// <summary>
        /// Parse the query body.
        /// </summary>
        /// <param name="body">The query body.</param>
        /// <returns>The name value collection.</returns>
        protected virtual void ParseBody(byte[] body)
        {
            if (body != null)
                _queryBody = Nequeo.Web.WebManager.ParseForm(body);
        }

        /// <summary>
        /// Get the query body string.
        /// </summary>
        /// <param name="collection">The name value collection.</param>
        /// <returns>The query string format.</returns>
        protected virtual string GetQueryBody(NameValueCollection collection)
        {
            return Nequeo.Web.WebManager.CreateQueryString(collection);
        }

        /// <summary>
        /// Store the body of the message.
        /// </summary>
        /// <param name="body">The message.</param>
        protected virtual void StoreBody(byte[] body)
        {
            if (body != null)
                _body = body;
        }

        /// <summary>
        /// Process request method.
        /// </summary>
        protected virtual void ProcessRequest()
        {
            try
            {
                // Initialise the composition assembly collection.
                Compose();

                // Get the current web operation context.
                WebOperationContext context = WebOperationContext.Current;
                Uri requestUri = context.IncomingRequest.UriTemplateMatch.RequestUri;
                NameValueCollection queryString = context.IncomingRequest.UriTemplateMatch.QueryParameters;
                NameValueCollection form = null;
                string method = context.IncomingRequest.Method;

                // If this class is within an IIS server then use
                // the current http context.
                // Gets or sets the System.Web.HttpContext object for the current HTTP request.
                if (_isWebServer)
                    form = HttpContext.Current.Request.Form;

                // Assign the context values.
                _context = context;
                _requestUri = requestUri;
                _queryString = queryString;
                _method = method;
                _form = form;

                try
                {
                    _contentType = context.IncomingRequest.ContentType;
                    _contentLength = context.IncomingRequest.ContentLength;
                }
                catch { }
                
                // Send the web context to the derived class.
                WebContext(context, method, requestUri, queryString, form);
            }
            catch (System.Threading.ThreadAbortException)
            { }
            catch (Exception ex)
            {
                // Get the current exception.
                Exception = ex;
            }
        }
    }
}
