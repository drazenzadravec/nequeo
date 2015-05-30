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
using System.Linq;
using System.Text;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Reprenets an web asset.
    /// </summary>
    public class WebAsset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAsset"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="version">The version.</param>
        /// <param name="compress">if set to <c>true</c> [compress].</param>
        /// <param name="cacheDurationInDays">The cache duration in days.</param>
        /// <param name="content">The content.</param>
        public WebAsset(string contentType, string version, bool compress, float cacheDurationInDays, string content)
        {
            ContentType = contentType;
            Version = version;
            Compress = compress;
            CacheDurationInDays = cacheDurationInDays;
            Content = content;
        }

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get; private set;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="WebAsset"/> is compressed.
        /// </summary>
        /// <value><c>true</c> if compress; otherwise, <c>false</c>.</value>
        public bool Compress
        {
            get; private set;
        }

        /// <summary>
        /// Gets the cache duration in days.
        /// </summary>
        /// <value>The cache duration in days.</value>
        public float CacheDurationInDays
        {
            get; private set;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get; private set;
        }
    }
}
