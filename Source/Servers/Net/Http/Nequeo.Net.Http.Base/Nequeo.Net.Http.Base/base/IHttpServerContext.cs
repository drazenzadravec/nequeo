/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Principal;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// The http server context interface.
    /// </summary>
    public interface IHttpServerContext : ICloneable
    {
        /// <summary>
        /// Process the http server request.
        /// </summary>
        /// <param name="context">Provides access to the request and response objects.</param>
        void ProcessHttpRequest(IHttpContext context);
    }

    /// <summary>
    /// The http service context.
    /// </summary>
    public sealed class HttpServerContext : MarshalByRefObject, IHttpContext
    {
        /// <summary>
        /// Create a new instance of this type
        /// </summary>
        /// <returns>A new instance of this type.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Gets or sets; provides access to the request and response objects.
        /// </summary>
        public IHttpListenerContext HttpContext { get; set; }

        /// <summary>
        /// Gets or sets the active proccesing handler.
        /// </summary>
        public IActiveProcessing ActiveProcess { get; set; }
    }

    /// <summary>
    /// Http context handler.
    /// </summary>
    public interface IHttpContext : ICloneable
    {
        /// <summary>
        /// Gets or sets; provides access to the request and response objects.
        /// </summary>
        IHttpListenerContext HttpContext { get; set; }

        /// <summary>
        /// Gets or sets the active proccesing handler.
        /// </summary>
        IActiveProcessing ActiveProcess { get; set; }
    }

    /// <summary>
    /// Http listener context handler.
    /// </summary>
    public interface IHttpListenerContext
    {
        /// <summary>
        /// Gets or sets the http request.
        /// </summary>
        System.Net.HttpListenerRequest Request { get; set; }

        /// <summary>
        /// Gets or sets the http response.
        /// </summary>
        System.Net.HttpListenerResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the http user principal.
        /// </summary>
        IPrincipal User { get; set; }
    }

    /// <summary>
    /// Active processing context handler.
    /// </summary>
    public interface IActiveProcessing
    {
        /// <summary>
        /// Gets the post back form data; else empty collection.
        /// </summary>
        NameValueCollection Form { get; set; }

        /// <summary>
        /// Gets the collection of files that are in the postback data; else empty collection.
        /// </summary>
        string[] UploadFiles { get; set; }

        /// <summary>
        /// Start processing post back data from the request.
        /// </summary>
        /// <param name="request">The http request context.</param>
        /// <param name="uploadDirectory">The upload directory path where files are placed; else uploaded files are ingored.</param>
        void ProcessPostBack(System.Net.HttpListenerRequest request, string uploadDirectory = null);

        /// <summary>
        /// Get the response mime content type for the extension.
        /// </summary>
        /// <param name="extension">The extension name.</param>
        /// <returns>The mime content type.</returns>
        string GetMimeContentType(string extension);

        /// <summary>
        /// Gets the list of allowed extensions.
        /// </summary>
        /// <returns>The list of extensions.</returns>
        string[] AllowedExtensions();

        /// <summary>
        /// Is the request a post back.
        /// </summary>
        bool IsPostBack { get; set; }
    }
}
