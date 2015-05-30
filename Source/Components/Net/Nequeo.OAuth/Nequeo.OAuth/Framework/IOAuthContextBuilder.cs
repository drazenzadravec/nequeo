/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net;

namespace Nequeo.Net.OAuth.Framework
{
    /// <summary>
    /// OAuth Context Builder interface
    /// </summary>
    public interface IOAuthContextBuilder
    {
        /// <summary>
        /// From Url
        /// </summary>
        /// <param name="httpMethod">The http method</param>
        /// <param name="url">The url</param>
        /// <returns>The OAuth context</returns>
        IOAuthContext FromUrl(string httpMethod, string url);

        /// <summary>
        /// From Uri
        /// </summary>
        /// <param name="httpMethod">The http method</param>
        /// <param name="uri">The url</param>
        /// <returns>The OAuth context</returns>
        IOAuthContext FromUri(string httpMethod, Uri uri);

        /// <summary>
        /// From Http Request
        /// </summary>
        /// <param name="request">The http request</param>
        /// <returns>The OAuth context</returns>
        IOAuthContext FromHttpRequest(HttpRequest request);

        /// <summary>
        /// From Http Request
        /// </summary>
        /// <param name="request">The http request base</param>
        /// <returns>The OAuth context</returns>
        IOAuthContext FromHttpRequest(HttpRequestBase request);

        /// <summary>
        /// FromW eb Request
        /// </summary>
        /// <param name="request">The http web request.</param>
        /// <param name="rawBody">The raw body</param>
        /// <returns>The OAuth context</returns>
        IOAuthContext FromWebRequest(HttpWebRequest request, Stream rawBody);

        /// <summary>
        /// From Web Request
        /// </summary>
        /// <param name="request">The http web request.</param>
        /// <param name="body">The body</param>
        /// <returns>The OAuth context</returns>
        IOAuthContext FromWebRequest(HttpWebRequest request, string body);
    }
}
