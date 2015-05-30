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
using System.Diagnostics;
using System.IO;
using System.Web;

using Nequeo.Net.Http.Extension;
using Nequeo.Web.Mvc.Extended.Factory;
using Nequeo.Web.Mvc.Extended.Factory.Runtime;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// The HttpHandler to compress, cache and combine web assets.
    /// </summary>
    public class WebAssetHttpHandler : HttpHandlerBase
    {
        private readonly IWebAssetRegistry _assetRegistry;
        private readonly IHttpResponseCompressor _httpResponseCompressor;
        private readonly IHttpResponseCacher _httpResponseCacher;

        private static string defaultPath = "~/assetnequeo.axd";
        private static string idParameterName = "id";

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssetHttpHandler"/> class.
        /// </summary>
        /// <param name="assetRegistry">The asset registry.</param>
        /// <param name="httpResponseCompressor">The HTTP response compressor.</param>
        /// <param name="httpResponseCacher">The HTTP response cacher.</param>
        public WebAssetHttpHandler(IWebAssetRegistry assetRegistry, IHttpResponseCompressor httpResponseCompressor, IHttpResponseCacher httpResponseCacher)
        {
            // If the instance object is null.
            if (assetRegistry == null) throw new System.ArgumentNullException("assetRegistry");
            if (httpResponseCompressor == null) throw new System.ArgumentNullException("httpResponseCompressor");
            if (httpResponseCacher == null) throw new System.ArgumentNullException("httpResponseCacher");

            _assetRegistry = assetRegistry;
            _httpResponseCompressor = httpResponseCompressor;
            _httpResponseCacher = httpResponseCacher;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssetHttpHandler"/> class.
        /// </summary>
        public WebAssetHttpHandler()
            : this(ServiceLocator.Current.Resolve<IWebAssetRegistry>(), 
                ServiceLocator.Current.Resolve<IHttpResponseCompressor>(), 
                ServiceLocator.Current.Resolve<IHttpResponseCacher>())
        {
        }

        /// <summary>
        /// Gets or sets the default path of the asset.
        /// </summary>
        /// <value>The default path.</value>
        public static string DefaultPath
        {
            get { return defaultPath; }
            set { defaultPath = value; }
        }

        /// <summary>
        /// Gets or sets the name of the id parameter.
        /// </summary>
        /// <value>The name of the id parameter.</value>
        public static string IdParameterName
        {
            get { return idParameterName; }
            set { idParameterName = value; }
        }

        /// <summary>
        /// Enables a WebAssetHttpHandler object to process of requests.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void ProcessRequest(HttpContextBase context)
        {
            string id = context.Request.QueryString[IdParameterName];

            if (!string.IsNullOrEmpty(id))
            {
                WebAsset asset = _assetRegistry.Retrieve(id);

                if (asset != null)
                {
                    HttpResponseBase response = context.Response;

                    // Set the content type
                    response.ContentType = asset.ContentType;
                    string content = asset.Content;

                    if (!string.IsNullOrEmpty(content))
                    {
                        // Compress
                        if (asset.Compress && !context.IsMono())
                            _httpResponseCompressor.Compress(context);

                        // Write the contenxt to the output stream.
                        using (StreamWriter sw = new StreamWriter((response.OutputStream)))
                            sw.Write(content);

                        // Cache the item.
                        if (!context.IsDebuggingEnabled)
                            _httpResponseCacher.Cache(context, TimeSpan.FromDays(asset.CacheDurationInDays));
                    }
                }
            }
        }
    }
}
