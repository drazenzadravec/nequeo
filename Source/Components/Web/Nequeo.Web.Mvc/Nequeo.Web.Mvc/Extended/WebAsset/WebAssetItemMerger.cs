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

using Nequeo.Extension;
using Nequeo.Collections.Extension;
using Nequeo.Web.Mvc.Extended.UI;
using Nequeo.Web.Mvc.Extended.Factory;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Defines the basic building block of web asset merging.
    /// </summary>
    public interface IWebAssetItemMerger
    {
        /// <summary>
        /// Merges the specified assets.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="assetHandlerPath">The asset handler path.</param>
        /// <param name="isSecured">if set to <c>true</c> [is secured].</param>
        /// <param name="canCompress">if set to <c>true</c> [can compress].</param>
        /// <param name="assets">The assets.</param>
        /// <returns></returns>
        IList<string> Merge(string contentType, string assetHandlerPath, bool isSecured, bool canCompress, WebAssetItemCollection assets);
    }

    /// <summary>
    /// The default web asset merger.
    /// </summary>
    public class WebAssetItemMerger : IWebAssetItemMerger
    {
        private readonly IWebAssetRegistry _assetRegistry;
        private readonly IUrlResolver _urlResolver;
        private readonly IUrlEncoder _urlEncoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssetItemMerger"/> class.
        /// </summary>
        /// <param name="assetRegistry">The asset registry.</param>
        /// <param name="urlResolver">The URL resolver.</param>
        /// <param name="urlEncoder">The URL encoder.</param>
        public WebAssetItemMerger(IWebAssetRegistry assetRegistry, IUrlResolver urlResolver, IUrlEncoder urlEncoder)
        {
            // If the instance object is null.
            if (assetRegistry == null) throw new System.ArgumentNullException("assetRegistry");
            if (urlResolver == null) throw new System.ArgumentNullException("urlResolver");
            if (urlEncoder == null) throw new System.ArgumentNullException("urlEncoder");

            _assetRegistry = assetRegistry;
            _urlResolver = urlResolver;
            _urlEncoder = urlEncoder;
        }

        /// <summary>
        /// Merges the specified assets.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="assetHandlerPath">The asset handler path.</param>
        /// <param name="isSecured">if set to <c>true</c> [is secure].</param>
        /// <param name="canCompress">if set to <c>true</c> [can compress].</param>
        /// <param name="assets">The assets.</param>
        /// <returns>The collection of web asset paths.</returns>
        public IList<string> Merge(string contentType, string assetHandlerPath, bool isSecured, bool canCompress, WebAssetItemCollection assets)
        {
            // If the instance object is null.
            if (contentType == null) throw new System.ArgumentNullException("contentType");
            if (assetHandlerPath == null) throw new System.ArgumentNullException("assetHandlerPath");
            if (assets == null) throw new System.ArgumentNullException("assets");

            IList<string> mergedList = new List<string>();
            Func<string, string, string> getRelativePath = (source, version) => _urlResolver.Resolve(_assetRegistry.Locate(source, version));

            Action<WebAssetItemGroup> processGroup = group =>
                    {
                        if (group.Combined)
                        {
                            string id = _assetRegistry.Store(contentType, group);
                            string virtualPath = "{0}?{1}={2}".FormatWith(assetHandlerPath, _urlEncoder.Encode(WebAssetHttpHandler.IdParameterName), _urlEncoder.Encode(id));
                            string relativePath = _urlResolver.Resolve(virtualPath);

                            if (!mergedList.Contains(relativePath, StringComparer.OrdinalIgnoreCase))
                            {
                                mergedList.Add(relativePath);
                            }
                        }
                        else
                        {
                            group.Items.Each(i =>
                            {
                                if (!mergedList.Contains(i.Source, StringComparer.OrdinalIgnoreCase))
                                {
                                    mergedList.Add(getRelativePath(i.Source, group.Version));
                                }
                            });
                        }
                    };

            if (!assets.IsEmpty())
            {
                foreach (IWebAssetItem asset in assets)
                {
                    WebAssetItem item = asset as WebAssetItem;
                    WebAssetItemGroup itemGroup = asset as WebAssetItemGroup;

                    if (item != null)
                        mergedList.Add(getRelativePath(item.Source, null));
                    else if (itemGroup != null)
                    {
                        WebAssetItemGroup frameworkGroup = null;

                        if ((frameworkGroup != null) && !frameworkGroup.Items.IsEmpty())
                            processGroup(frameworkGroup);

                        if (!itemGroup.Items.IsEmpty())
                            processGroup(itemGroup);
                    }
                }
            }

            // Return the list.
            return mergedList.ToList();
        }

        /// <summary>
        /// Remove and get framework group.
        /// </summary>
        /// <param name="itemGroup">The web asset to remove.</param>
        /// <returns>The web asset item group</returns>
        private static WebAssetItemGroup RemoveAndGetFrameworkGroup(WebAssetItemGroup itemGroup)
        {
            WebAssetItemGroup frameworkGroup = 
                new WebAssetItemGroup("framework", false) 
                { 
                    Combined = itemGroup.Combined, 
                    Compress = itemGroup.Compress, 
                    CacheDurationInDays = itemGroup.CacheDurationInDays, 
                    DefaultPath = itemGroup.DefaultPath, 
                    Version = itemGroup.Version
                };

            for (int i = itemGroup.Items.Count - 1; i >= 0; i--)
            {
                WebAssetItem item = itemGroup.Items[i];
                string fileName = Path.GetFileName(item.Source);

                if ((!fileName.Equals(ScriptRegistrar.jQuery, StringComparison.OrdinalIgnoreCase)) && 
                    (ScriptRegistrar.FrameworkScriptFileNames.Contains(fileName, StringComparer.OrdinalIgnoreCase)))
                {
                    frameworkGroup.Items.Add(new WebAssetItem(item.Source));
                    itemGroup.Items.RemoveAt(i);
                }
            }

            frameworkGroup.Items.Reverse();
            return frameworkGroup;
        }

        /// <summary>
        /// Remove and get native files.
        /// </summary>
        /// <param name="itemGroup">The native web asset to remove</param>
        /// <returns>The new list of paths.</returns>
        private static IList<string> RemoveAndGetNativeFiles(WebAssetItemGroup itemGroup)
        {
            List<string> nativeFiles = new List<string>();

            for (int i = itemGroup.Items.Count - 1; i >= 0; i--)
            {
                WebAssetItem item = itemGroup.Items[i];

                if (IsNativeFile(item))
                {
                    nativeFiles.Add(Path.GetFileName(item.Source));
                    itemGroup.Items.RemoveAt(i);
                }
            }

            nativeFiles.Reverse();
            return nativeFiles;
        }

        /// <summary>
        /// Is native file
        /// </summary>
        /// <param name="item">The web asset item to test.</param>
        /// <returns>True if native else false.</returns>
        private static bool IsNativeFile(WebAssetItem item)
        {
            if (item.Source.StartsWith("~/", StringComparison.Ordinal))
            {
                string fileName = Path.GetFileName(item.Source);

                return fileName.Equals(ScriptRegistrar.jQuery, StringComparison.OrdinalIgnoreCase) ||
                       fileName.Equals(ScriptRegistrar.jQueryValidation, StringComparison.OrdinalIgnoreCase) ||
                       fileName.StartsWith("Nequeo.", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}