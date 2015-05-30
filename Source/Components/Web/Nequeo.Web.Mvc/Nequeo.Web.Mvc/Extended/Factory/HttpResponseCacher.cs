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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Class used to cache the http response.
    /// </summary>
    public class HttpResponseCacher : IHttpResponseCacher
    {
        /// <summary>
        /// Set the caching policy.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="duration">The duration.</param>
        public void Cache(HttpContextBase context, TimeSpan duration)
        {
            if ((duration > TimeSpan.Zero) || !context.IsDebuggingEnabled) // Do not cache in debug mode.
            {
                HttpCachePolicyBase cache = context.Response.Cache;

                cache.SetCacheability(HttpCacheability.Public);
                cache.SetOmitVaryStar(true);
                cache.SetExpires(context.Timestamp.Add(duration));
                cache.SetMaxAge(duration);
                cache.SetValidUntilExpires(true);
                cache.SetLastModified(context.Timestamp);
                cache.SetLastModifiedFromFileDependencies();
                cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            }
        }
    }
}
