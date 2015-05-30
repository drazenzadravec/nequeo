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

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Nequeo.Net.Http.Extension;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Class used to resolve relative path for virtual path.
    /// </summary>
    public class UrlResolver : IUrlResolver
    {
        /// <summary>
        /// Returns the relative path for the specified virtual path.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The resolved path.</returns>
        public string Resolve(string url)
        {
            HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
            RequestContext requestContext = httpContext.RequestContext();
            UrlHelper helper = new UrlHelper(requestContext);

            string query;

            url = StripQuery(url, out query);

            return helper.Content(url) + query;
        }

        /// <summary>
        /// Strip the query
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="query">The query</param>
        /// <returns></returns>
        private static string StripQuery(string path, out string query)
        {
            int queryIndex = path.IndexOf('?');

            if (queryIndex >= 0)
            {
                query = path.Substring(queryIndex);

                return path.Substring(0, queryIndex);
            }

            query = null;
            return path;
        }
    }
}