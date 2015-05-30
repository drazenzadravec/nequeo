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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using Nequeo.Extension;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Represents a list of that can be accessed by index. 
    /// Provides methods to search, sort and manipulate lists.
    /// </summary>
    public class WebAssetItemCollection : Collection<IWebAssetItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssetItemCollection"/> class.
        /// </summary>
        /// <param name="defaultPath">The default path.</param>
        public WebAssetItemCollection(string defaultPath)
        {
            DefaultPath = defaultPath;
        }

        /// <summary>
        /// Gets or sets the default path.
        /// </summary>
        /// <value>The default path.</value>
        public string DefaultPath
        {
            get; private set;
        }

        /// <summary>
        /// Gets the asset groups.
        /// </summary>
        /// <value>The asset groups.</value>
        public virtual IEnumerable<WebAssetItemGroup> AssetGroups
        {
            get { return this.OfType<WebAssetItemGroup>(); }
        }

        /// <summary>
        /// Gets the asset items.
        /// </summary>
        /// <value>The asset items.</value>
        public virtual IEnumerable<WebAssetItem> AssetItems
        {
            get { return this.OfType<WebAssetItem>(); }
        }

        /// <summary>
        /// Finds the group with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual WebAssetItemGroup FindGroupByName(string name)
        {
            return AssetGroups.SingleOrDefault(group => group.Name.IsCaseInsensitiveEqual(name));
        }

        /// <summary>
        /// Finds the item with the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public virtual WebAssetItem FindItemBySource(string source)
        {
            return AssetItems.SingleOrDefault(item => item.Source.IsCaseInsensitiveEqual(source));
        }

        /// <summary>
        /// Adds the specified source as <see cref="WebAssetItem"/>.
        /// </summary>
        /// <param name="itemSource">The item source.</param>
        public virtual void Add(string itemSource)
        {
            Add(CreateItem(itemSource));
        }

        /// <summary>
        /// Adds the specified source as <see cref="WebAssetItem"/> in the specified <see cref="WebAssetItemGroup"/>.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="itemSource">The item source.</param>
        public virtual void Add(string groupName, string itemSource)
        {
            WebAssetItemGroup itemGroup = FindGroupByName(groupName);

            if (itemGroup == null)
            {
                itemGroup = CreateGroup(groupName);
                Add(itemGroup);
            }

            itemGroup.Items.Add(CreateItem(itemSource));
        }

        /// <summary>
        /// Inserts the specified source as <see cref="WebAssetItem"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="itemSource">The item source.</param>
        public virtual void Insert(int index, string itemSource)
        {
            Insert(index, CreateItem(itemSource));
        }

        /// <summary>
        /// Inserts the specified source as <see cref="WebAssetItem"/> at the specified index in the specified <see cref="WebAssetItemGroup"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="itemSource">The item source.</param>
        public virtual void Insert(int index, string groupName, string itemSource)
        {
            WebAssetItemGroup itemGroup = FindGroupByName(groupName);

            if (itemGroup == null)
            {
                itemGroup = CreateGroup(groupName);
                Insert(index, itemGroup);
            }

            itemGroup.Items.Add(CreateItem(itemSource));
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, IWebAssetItem item)
        {
            if (!AlreadyExists(item))
            {
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        protected override void SetItem(int index, IWebAssetItem item)
        {
            if (AlreadyExists(item))
            {
                if (item is WebAssetItem)
                    throw new ArgumentException("Item with specified source already exists");

                if (item is WebAssetItemGroup)
                    throw new ArgumentException("Group with specified name already exists");
            }

            base.SetItem(index, item);
        }

        /// <summary>
        /// Create the web asset group.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <returns>The new web asset group.</returns>
        private WebAssetItemGroup CreateGroup(string name)
        {
            return new WebAssetItemGroup(name) { DefaultPath = DefaultPath };
        }

        /// <summary>
        /// Create the web asset item
        /// </summary>
        /// <param name="source">The name of the source.</param>
        /// <returns>The new web asset item.</returns>
        private WebAssetItem CreateItem(string source)
        {
            string itemSource = source.StartsWith("~/", StringComparison.OrdinalIgnoreCase) ? source : MvcManager.CombinePath(DefaultPath, source);
            return new WebAssetItem(itemSource);
        }

        /// <summary>
        /// Does the web asset already exist.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>True if it exists else false.</returns>
        private bool AlreadyExists(IWebAssetItem item)
        {
            WebAssetItem assetItem = item as WebAssetItem;
            WebAssetItemGroup assetItemGroup = item as WebAssetItemGroup;

            if (assetItem != null)
                return AssetItems.Any(i => i != item && i.Source.IsCaseInsensitiveEqual(assetItem.Source));

            if (assetItemGroup != null)
                return AssetGroups.Any(i => i != item && i.Name.IsCaseInsensitiveEqual(assetItemGroup.Name));

            return false;
        }
    }
}