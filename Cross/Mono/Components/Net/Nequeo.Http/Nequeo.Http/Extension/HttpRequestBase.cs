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
using System.Web;

namespace Nequeo.Net.Http.Extension
{
    /// <summary>
    /// Contains extension methods of <see cref="HttpRequestBase"/>.
    /// </summary>
    public static class HttpRequestBaseExtensions
    {
        /// <summary>
        /// Get the Application root path.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static string ApplicationRoot(this HttpRequestBase instance)
        {
            string applicationPath = instance.Url.GetLeftPart(UriPartial.Authority) + instance.ApplicationPath;

            // Remove the last /
            if (applicationPath.EndsWith("/", StringComparison.Ordinal))
            {
                applicationPath = applicationPath.Substring(0, applicationPath.Length - 1);
            }

            return applicationPath;
        }

        /// <summary>
        /// Determines whether this instance can compress the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can compress the specified instance; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanCompress(this HttpRequestBase instance)
        {
            string encoding = (instance.Headers["Accept-Encoding"] ?? string.Empty).ToUpperInvariant();
            bool ie6 = (instance.Browser.MajorVersion < 7) && instance.Browser.IsBrowser("IE");

            return !ie6 && (encoding.Contains("GZIP") || encoding.Contains("DEFLATE"));
        }
    }
}