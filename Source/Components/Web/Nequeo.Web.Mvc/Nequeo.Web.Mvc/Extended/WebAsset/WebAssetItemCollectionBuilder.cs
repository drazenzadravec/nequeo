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
using System.ComponentModel;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Defines the fluent interface for configuring web assets.
    /// </summary>
    public class WebAssetItemCollectionBuilder : IObjectMembers
    {
        private readonly WebAssetType _assetType;
        private readonly WebAssetItemCollection _assets;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="assets">The assets.</param>
        public WebAssetItemCollectionBuilder(WebAssetType assetType, WebAssetItemCollection assets)
        {
            if (assetType == WebAssetType.None)
                throw new ArgumentException("None is only used for internal purpose", "assets");

            _assetType = assetType;
            _assets = assets;
        }

        /// <summary>
        /// Performs an implicit conversion.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator WebAssetItemCollection(WebAssetItemCollectionBuilder builder)
        {
            return builder.ToCollection();
        }

        /// <summary>
        /// Returns the internal collection.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WebAssetItemCollection ToCollection()
        {
            return _assets;
        }

        /// <summary>
        /// Adds a new web asset
        /// </summary>
        /// <param name="source">The source.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///            .Scripts(scripts => scripts.Add("script1.js"))
        /// %&gt;
        /// </code>
        /// </example>
        public virtual WebAssetItemCollectionBuilder Add(string source)
        {
            _assets.Add(source);
            return this;
        }

        /// <summary>
        /// Adds a new web asset group.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configureAction">The configure action.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///            .Scripts(scripts => scripts.AddGroup("Group1", group => 
        ///                 {
        ///                     group.Add("script1.js");
        ///                 }
        ///            ))
        /// %&gt;
        /// </code>
        /// </example>
        public virtual WebAssetItemCollectionBuilder AddGroup(string name, Action<WebAssetItemGroupBuilder> configureAction)
        {
            WebAssetItemGroup itemGroup = _assets.FindGroupByName(name);

            if (itemGroup != null)
                throw new ArgumentException("Group with specified name already exists please specify a different name", "name");

            itemGroup = new WebAssetItemGroup(name) { DefaultPath = _assets.DefaultPath };
            _assets.Add(itemGroup);

            WebAssetItemGroupBuilder builder = new WebAssetItemGroupBuilder(itemGroup);
            configureAction(builder);

            return this;
        }

        /// <summary>
        /// Adds the specified shared group.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///            .Scripts(scripts => scripts.AddShareGroup("SharedGroup1"))
        /// %&gt;
        /// </code>
        /// </example>
        public virtual WebAssetItemCollectionBuilder AddSharedGroup(string name)
        {
            WebAssetItemGroup group = (_assetType == WebAssetType.StyleSheet) ?
                                      SharedGroup.FindStyleSheetGroup(name) :
                                      SharedGroup.FindScriptGroup(name);

            if (group == null)
                throw new ArgumentException("GroupWithSpecifiedNameDoesNotExistInAssetTypeOfSharedWebAssets", "name");

            if (_assets.FindGroupByName(name) == null)
            {
                // People might have the same group reference in multiple place.
                // So we will skip it once it is added.

                // throw new ArgumentException(TextResource.LocalGroupWithSpecifiedNameAlreadyExists.FormatWith(name));

                // Add a copy of the shared asset
                WebAssetItemGroup localGroup = new WebAssetItemGroup(group.Name, true)
                                                   {
                                                       DefaultPath = group.DefaultPath,
                                                       Version = group.Version,
                                                       Compress = group.Compress,
                                                       CacheDurationInDays = group.CacheDurationInDays,
                                                       Combined = group.Combined
                                                   };

                foreach (WebAssetItem item in group.Items)
                    localGroup.Items.Add(new WebAssetItem(item.Source));

                _assets.Add(localGroup);
            }

            return this;
        }

        /// <summary>
        /// Executes the provided delegate that is used to configure the group fluently.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configureAction">The configure action.</param>
        public virtual WebAssetItemCollectionBuilder GetGroup(string name, Action<WebAssetItemGroupBuilder> configureAction)
        {
            WebAssetItemGroup itemGroup = _assets.FindGroupByName(name);

            if (itemGroup == null)
                throw new ArgumentException("Group with specified name does not exist please make sure you have specified a correct name", "name");

            if (itemGroup.IsShared)
                throw new InvalidOperationException("You cannot configure a shared web asset group");

            WebAssetItemGroupBuilder builder = new WebAssetItemGroupBuilder(itemGroup);
            configureAction(builder);
            return this;
        }
    }
}