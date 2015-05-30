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
using System.IO;
using System.Linq;
using System.Text;

using Nequeo.Extension;
using Nequeo.Web.Mvc.Extended.Factory;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Basic building block to locate the correct virtual path.
    /// </summary>
    public interface IWebAssetLocator
    {
        /// <summary>
        /// Returns the correct virtual path based upon the debug mode and version.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        string Locate(string virtualPath, string version);
    }

    /// <summary>
    /// Default web asset locator.
    /// </summary>
    public class WebAssetLocator : CacheBase<string, string>, IWebAssetLocator
    {
        private readonly bool _isInDebugMode;
        private readonly IVirtualPathProvider _virtualPathProvider;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="isInDebugMode"></param>
        /// <param name="virtualPathProvider"></param>
        public WebAssetLocator(bool isInDebugMode, IVirtualPathProvider virtualPathProvider)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            // If the instance object is null.
            if (virtualPathProvider == null)
                throw new System.ArgumentNullException("virtualPathProvider");

            _isInDebugMode = isInDebugMode;
            _virtualPathProvider = virtualPathProvider;
        }

        /// <summary>
        /// Returns the correct virtual path based upon the debug mode and version.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="version">The version.</param>
        /// <returns>The virtual path</returns>
        public string Locate(string virtualPath, string version)
        {
            // If the instance object is null.
            if (virtualPath == null)
                throw new System.ArgumentNullException("virtualPath");

            return _isInDebugMode ? InternalLocate(virtualPath, version) : GetOrCreate("{0}:{1}".FormatWith(virtualPath, 
                version), () => InternalLocate(virtualPath, version));
        }

        /// <summary>
        /// Locate the jQuery style sheets and scripts.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="version">The version.</param>
        /// <returns>The virtual path</returns>
        private string InternalLocate(string virtualPath, string version)
        {
            string result = virtualPath;

            string extension = _virtualPathProvider.GetExtension(virtualPath);

            if (extension.IsCaseInsensitiveEqual(".js"))
            {
                result = _isInDebugMode ? ProbePath(virtualPath, 
                    version, new[] { ".debug.js", ".js", ".min.js" }) : ProbePath(virtualPath, version, new[] { ".min.js", ".js", ".debug.js" });
            }
            else if (extension.IsCaseInsensitiveEqual(".css"))
            {
                result = _isInDebugMode ? ProbePath(virtualPath, 
                    version, new[] { ".css", ".min.css" }) : ProbePath(virtualPath, version, new[] { ".min.css", ".css" });
            }

            return result;
        }

        /// <summary>
        /// Get the path to the script.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="version">The version.</param>
        /// <param name="extensions">The collection of extensions.</param>
        /// <returns>The path.</returns>
        private string ProbePath(string virtualPath, string version, IEnumerable<string> extensions)
        {
            string result = null;

            Func<string, string> fixPath = path =>
            {
                string directory = _virtualPathProvider.GetDirectory(path);
                string fileName = _virtualPathProvider.GetFile(path);

                if (!directory.EndsWith(version + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    string newDirectory = _virtualPathProvider.CombinePaths(directory, version);
                    string newPath = newDirectory + Path.AltDirectorySeparatorChar + fileName;

                    if (_virtualPathProvider.FileExists(newPath))
                    {
                        return newPath;
                    }
                }

                return path;
            };

            foreach (string extension in extensions)
            {
                string changedPath = Path.ChangeExtension(virtualPath, extension);
                string newVirtualPath = string.IsNullOrEmpty(version) ? changedPath : fixPath(changedPath);

                if (_virtualPathProvider.FileExists(newVirtualPath))
                {
                    result = newVirtualPath;
                    break;
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                result = virtualPath;

                if (!_virtualPathProvider.FileExists(result))
                    throw new FileNotFoundException("The specified file does not exist : '" + result + "'");
            }

            // Return the virtual file.
            return result;
        }
    }
}
