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
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Virtual path provider.
    /// </summary>
    public interface IVirtualPathProvider
    {
        /// <summary>
        /// Determines is the directory exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path</param>
        /// <returns>True if it exists else false.</returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// Determines is the file exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path</param>
        /// <returns>True if it exists else false.</returns>
        bool FileExists(string virtualPath);

        /// <summary>
        /// Get the directory for the path.
        /// </summary>
        /// <param name="virtualPath">The virtual path</param>
        /// <returns>The full path.</returns>
        string GetDirectory(string virtualPath);

        /// <summary>
        /// Get the file name.
        /// </summary>
        /// <param name="virtualPath">The virtual path</param>
        /// <returns>The file name</returns>
        string GetFile(string virtualPath);

        /// <summary>
        /// Get the file extension.
        /// </summary>
        /// <param name="virtualPath">The virtual path</param>
        /// <returns>The file extension.</returns>
        string GetExtension(string virtualPath);

        /// <summary>
        /// Combine the base path and the relative path.
        /// </summary>
        /// <param name="basePath">The base concrete path.</param>
        /// <param name="relativePath">The relative path</param>
        /// <returns>The combined path.</returns>
        string CombinePaths(string basePath, string relativePath);

        /// <summary>
        /// Read all the text for the path.
        /// </summary>
        /// <param name="virtualPath">The virtual path</param>
        /// <returns>The read text.</returns>
        string ReadAllText(string virtualPath);
    }
}
