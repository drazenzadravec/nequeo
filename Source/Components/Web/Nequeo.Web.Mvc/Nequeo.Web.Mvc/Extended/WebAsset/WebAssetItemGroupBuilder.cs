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
    /// Web asset item group builder.
    /// </summary>
    public class WebAssetItemGroupBuilder : IObjectMembers
    {
        private readonly WebAssetItemGroup _assetItemGroup;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="assetItemGroup">The current web asset item group to add the item to.</param>
        public WebAssetItemGroupBuilder(WebAssetItemGroup assetItemGroup)
        {
            _assetItemGroup = assetItemGroup;
        }

        /// <summary>
        /// Performs an implicit conversion   
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator WebAssetItemGroup(WebAssetItemGroupBuilder builder)
        {
            return builder.ToGroup();
        }

        /// <summary>
        /// The default script or style paths.
        /// </summary>
        /// <param name="path">The path to append to the value.</param>
        /// <returns>The current web asset item group builder.</returns>
        public virtual WebAssetItemGroupBuilder DefaultPath(string path)
        {
            _assetItemGroup.DefaultPath = path;
            return this;
        }

        /// <summary>
        /// Add a new item to the collection.
        /// </summary>
        /// <param name="value">The value to add to the collection.</param>
        /// <returns>The current web asset item group builder.</returns>
        public virtual WebAssetItemGroupBuilder Add(string value)
        {
            _assetItemGroup.Items.Add(CreateItem(value));
            return this;
        }

        /// <summary>
        /// Create the item in the web asset item collection.
        /// </summary>
        /// <param name="source">The name of the item to add.</param>
        /// <returns>The created web asset item.</returns>
        private WebAssetItem CreateItem(string source)
        {
            // If the instance object is null.
            if (source == null)
                throw new System.ArgumentNullException("source");

            string itemSource = source.StartsWith("~/",
                StringComparison.OrdinalIgnoreCase) ? source : MvcManager.CombinePath(_assetItemGroup.DefaultPath, source);

            // Return the script web item
            return new WebAssetItem(itemSource);
        }

        /// <summary>
        /// Returns the internal group.
        /// </summary>
        /// <returns></returns>
        public WebAssetItemGroup ToGroup()
        {
            return _assetItemGroup;
        }

        /// <summary>
        /// Sets the version.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///           .DefaultGroup(group => group.Version("1.1"))
        /// %&gt;
        /// </code>
        /// </example>
        public virtual WebAssetItemGroupBuilder Version(string value)
        {
            _assetItemGroup.Version = value;
            return this;
        }

        /// <summary>
        /// Sets whether the groups will be served as compressed. By default asset groups are not compressed.
        /// </summary>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///           .DefaultGroup(group => group.Compress(true))
        /// %&gt;
        /// </code>
        /// </example>
        public virtual WebAssetItemGroupBuilder Compress(bool value)
        {
            _assetItemGroup.Compress = value;
            return this;
        }

        /// <summary>
        /// Sets whether the groups items will be served as combined.
        /// </summary>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///           .DefaultGroup(group => group.Combined(true))
        /// %&gt;
        /// </code>
        /// </example>
        public virtual WebAssetItemGroupBuilder Combined(bool value)
        {
            _assetItemGroup.Combined = value;
            return this;
        }
    }
}
