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
using System.Security.Principal;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Net;
using System.IO;

using Nequeo.Net.Http.Common;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Http page base.
    /// </summary>
    public abstract class HttpPageBase
    {
        private string _uploadDirectory = null;

        /// <summary>
        /// The page is loading.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public abstract void OnLoad(HttpPageContext pageContext);

        /// <summary>
        /// The pre-process event.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public abstract void OnPreProcess(HttpPageContext pageContext);

        /// <summary>
        /// The page is initialised.
        /// </summary>
        /// <param name="pageContext">The page content.</param>
        public abstract void OnInit(HttpPageContext pageContext);

        /// <summary>
        /// Gets the request context.
        /// </summary>
        public HttpListenerRequest Request { get; internal set; }

        /// <summary>
        /// Gets the response context
        /// </summary>
        public HttpListenerResponse Response { get; internal set; }

        /// <summary>
        /// Gets the is post back indicator.
        /// </summary>
        public Boolean IsPostBack { get; internal set; }

        /// <summary>
        /// Gets the post back form data; else empty collection.
        /// </summary>
        public NameValueCollection Form { get; internal set; }

        /// <summary>
        /// Gets the collection of files that are in the postback data; else empty collection.
        /// </summary>
        public string[] UploadFiles { get; internal set; }

        /// <summary>
        /// Gets the local file name and full path of the resource.
        /// </summary>
        public string UrlFilePath { get; internal set; }

        /// <summary>
        /// Gets the active processing instance.
        /// </summary>
        public IActiveProcessing ActiveProcessing { get; internal set; }

        /// <summary>
        /// Gets or sets the upload directory path where files are placed; else uploaded files are ingored.
        /// </summary>
        public string UploadDirectory 
        {
            get { return _uploadDirectory; }
            set { _uploadDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the current user principal.
        /// </summary>
        public IPrincipal User { get; internal set; }

        /// <summary>
        /// Gets or set the alternative content to send. Default is null (sends the original page).
        /// </summary>
        public byte[] AlternativeContent { get; set; }

        /// <summary>
        /// Gets the internal exception.
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Create the page requested and send the context information to the page.
        /// </summary>
        /// <param name="baseNameSpace">The base namespace of the code behind resource.</param>
        /// <param name="assemblyName">The assembly name,from where the namespace belongs.</param>
        /// <param name="urlFilePath">The path and file name of the resource.</param>
        /// <param name="context">Provides access to the request and response objects.</param>
        /// <returns>True if the page code behind is found; else false;</returns>
        public static bool PageInstance(string baseNameSpace, string assemblyName, string urlFilePath, IHttpContext context)
        {
            // Page base.
            HttpPageBase page = null;
            bool foundPage = true;

            try
            {
                // Get the name of the file without extension.
                string fileNameWithoutEx = System.IO.Path.GetFileNameWithoutExtension(urlFilePath);
                
                try
                {
                    // Attempt to create the instnce of the page.
                    Type pageType = Type.GetType(baseNameSpace.TrimEnd('.') + "." + fileNameWithoutEx + ", " + assemblyName, true, true);
                    page = (HttpPageBase)Activator.CreateInstance(pageType);
                }
                catch (Exception)
                {
                    // If error
                    foundPage = false;
                    page = null;
                }

                // The page hierachy initialiser.
                HttpPageBase.PageInitialiser(page, urlFilePath, context);
                return foundPage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The page hierachy initialiser.
        /// </summary>
        /// <param name="page">The current page instance.</param>
        /// <param name="urlFilePath">The local file name and full path of the resource.</param>
        /// <param name="context">Provides access to the request and response objects.</param>
        public static void PageInitialiser(HttpPageBase page, string urlFilePath, IHttpContext context)
        {
            // Make sure that the page exists.
            if (page != null)
            {
                System.IO.Stream outputResponse = null;
                System.IO.Stream inputRequest = null;

                try
                {
                    // Create a new page context.
                    HttpPageContext pageContext = new HttpPageContext();
                    pageContext.IsValid = true;
                    pageContext.OverrideRequestResponse = false;
                    pageContext.ProcessOnPostBack = true;

                    // Assign the initial values.
                    page.Request = context.HttpContext.Request;
                    page.Response = context.HttpContext.Response;
                    page.IsPostBack = context.ActiveProcess.IsPostBack;
                    page.UrlFilePath = urlFilePath;
                    page.User = context.HttpContext.User;

                    // Execute the page hierachy.
                    page.OnInit(pageContext);

                    // If true then continue
                    if (pageContext.IsValid)
                    {
                        // Assign the current active processing.
                        page.ActiveProcessing = context.ActiveProcess;

                        // Execute the pre-processing event.
                        page.OnPreProcess(pageContext);
                        if (pageContext.ProcessOnPostBack)
                        {
                            // If post back.
                            if (context.ActiveProcess.IsPostBack)
                            {
                                // Get all the post back data.
                                context.ActiveProcess.ProcessPostBack(page.Request, page.UploadDirectory);
                                page.Form = context.ActiveProcess.Form;
                                page.UploadFiles = context.ActiveProcess.UploadFiles;
                            }
                        }

                        // Execute the page load event.
                        page.OnLoad(pageContext);
                        if (!pageContext.OverrideRequestResponse)
                        {
                            // If alternative content is to be sent.
                            if (page.AlternativeContent != null)
                            {
                                // Get the request, response stream.
                                outputResponse = context.HttpContext.Response.OutputStream;

                                // Transfer the data to the client.
                                inputRequest = HttpResponseContent.CreateResponseStream(page.AlternativeContent);
                                HttpResponseContent.TransferResponse(inputRequest, outputResponse);
                            }
                            else
                            {
                                // Get the request, response stream.
                                outputResponse = context.HttpContext.Response.OutputStream;

                                // Get the response html. Create the response. Transfer the data.
                                byte[] responseResourceHtmlData = HttpResponseContent.ResourceFound(context.HttpContext.Response, context.ActiveProcess,
                                    urlFilePath, System.IO.Path.GetExtension(urlFilePath).TrimStart('.'));

                                // Transfer the data to the client.
                                inputRequest = HttpResponseContent.CreateResponseStream(responseResourceHtmlData);
                                HttpResponseContent.TransferResponse(inputRequest, outputResponse);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    try
                    {
                        if (inputRequest != null)
                            inputRequest.Close();
                    }
                    catch (Exception ex) { page.Exception = ex; }

                    try
                    {
                        if (outputResponse != null)
                            outputResponse.Close();
                    }
                    catch (Exception ex) { page.Exception = ex; }
                }
            }
        }
    }
}
