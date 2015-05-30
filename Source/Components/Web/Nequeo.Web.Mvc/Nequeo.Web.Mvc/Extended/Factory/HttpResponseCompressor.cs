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

using System.IO.Compression;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that compress the response
    /// </summary>
    public class HttpResponseCompressor : IHttpResponseCompressor
    {
        /// <summary>
        /// Compresses the response.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Compress(HttpContextBase context)
        {
            string acceptEncoding;

            if (CanCompress(context, out acceptEncoding))
            {
                HttpResponseBase response = context.Response;

                if (acceptEncoding.Contains("GZIP"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("DEFLATE"))
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }
        }

        /// <summary>
        /// Can the data be compressed.
        /// </summary>
        /// <param name="context">The curent http context.</param>
        /// <param name="encoding">The encoding header details.</param>
        /// <returns>True for the data can be compressed else false.</returns>
        private static bool CanCompress(HttpContextBase context, out string encoding)
        {
            encoding = (context.Request.Headers["Accept-Encoding"] ?? string.Empty).ToUpperInvariant();
            bool ie6 = (context.Request.Browser.MajorVersion < 7) && context.Request.Browser.IsBrowser("IE");

            return !ie6 && (encoding.Contains("GZIP") || encoding.Contains("DEFLATE"));
        }
    }
}
