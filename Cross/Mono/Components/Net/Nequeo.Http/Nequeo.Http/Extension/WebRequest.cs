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
using System.Net;
using System.IO;
using System.Threading.Tasks;

using Nequeo.IO.Stream;
using Nequeo.IO.Stream.Extension;

namespace Nequeo.Net.Http.Extension
{
    /// <summary>
    /// Extension methods for working with WebRequest asynchronously.
    /// </summary>
    public static class WebRequestExtensions
    {
        /// <summary>Creates a Task that represents an asynchronous request to GetResponse.</summary>
        /// <param name="webRequest">The WebRequest.</param>
        /// <returns>A Task containing the retrieved WebResponse.</returns>
        public static Task<System.Net.WebResponse> GetResponseAsync(this System.Net.WebRequest webRequest)
        {
            if (webRequest == null) throw new ArgumentNullException("webRequest");
            return Task<System.Net.WebResponse>.Factory.FromAsync(
                webRequest.BeginGetResponse, webRequest.EndGetResponse, webRequest /* object state for debugging */);
        }

        /// <summary>Creates a Task that represents an asynchronous request to GetRequestStream.</summary>
        /// <param name="webRequest">The WebRequest.</param>
        /// <returns>A Task containing the retrieved Stream.</returns>
        public static Task<Stream> GetRequestStreamAsync(this System.Net.WebRequest webRequest)
        {
            if (webRequest == null) throw new ArgumentNullException("webRequest");
            return Task<Stream>.Factory.FromAsync(
                webRequest.BeginGetRequestStream, webRequest.EndGetRequestStream, webRequest /* object state for debugging */);
        }

        /// <summary>Creates a Task that respresents downloading all of the data from a WebRequest.</summary>
        /// <param name="webRequest">The WebRequest.</param>
        /// <returns>A Task containing the downloaded content.</returns>
        public static Task<byte[]> DownloadDataAsync(this System.Net.WebRequest webRequest)
        {
            // Asynchronously get the response.  When that's done, asynchronously read the contents.
            return webRequest.GetResponseAsync().ContinueWith(response =>
            {
                return response.Result.GetResponseStream().ReadAllBytesAsync();
            }).Unwrap();
        }
    }
}