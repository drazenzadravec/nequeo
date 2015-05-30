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

using Nequeo.Extension;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Web asset item group action collector
    /// </summary>
    public class WebAssetItemGroup : IWebAssetItem
    {
        private string _defaultPath;
        private string _version;
        private float _cacheDurationInDays;

        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="name">The name of the web asset item group.</param>
        public WebAssetItemGroup(string name)
        {
            Name = name;
            IsShared = false;
            Version = WebAssetDefaultSettings.Version;
            Compress = WebAssetDefaultSettings.Compress;
            CacheDurationInDays = WebAssetDefaultSettings.CacheDurationInDays;
            Combined = WebAssetDefaultSettings.Combined;
            Items = new InternalAssetItemCollection();
        }

        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="name">The name of the web asset item group.</param>
        /// <param name="isShared">Is the web asset shared.</param>
        public WebAssetItemGroup(string name, bool isShared)
        {
            Name = name;
            IsShared = isShared;
            Version = WebAssetDefaultSettings.Version;
            Compress = WebAssetDefaultSettings.Compress;
            CacheDurationInDays = WebAssetDefaultSettings.CacheDurationInDays;
            Combined = WebAssetDefaultSettings.Combined;
            Items = new InternalAssetItemCollection();
        }

        /// <summary>
        /// Gets the name that identifies the current web asset item group.
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is shared.
        /// </summary>
        /// <value>True if this instance is shared; otherwise, false</value>
        public bool IsShared
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is compress.
        /// </summary>
        /// <value>True if compress; otherwise, false.</value>
        public bool Compress { get; set; }

        /// <summary>
        /// Gets or sets the cache duration in days.
        /// </summary>
        /// <value>The cache duration in days.</value>
        public float CacheDurationInDays
        {
            get { return _cacheDurationInDays; }
            set { _cacheDurationInDays = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is combined.
        /// </summary>
        /// <value>True if combined; otherwise, false.</value>
        public bool Combined { get; set; }

        /// <summary>
        /// Gets the default script or style folder path.
        /// </summary>
        public string DefaultPath
        {
            get { return _defaultPath; }
            set { _defaultPath = value; }
        }

        /// <summary>
        /// Gets the collection of items
        /// </summary>
        public IList<WebAssetItem> Items
        {
            get; private set;
        }

        /// <summary>
        /// Internal asset item collection.
        /// </summary>
        private sealed class InternalAssetItemCollection : System.Collections.ObjectModel.Collection<WebAssetItem>
        {
            /// <summary>
            /// Insert the current item at the specified index.
            /// </summary>
            /// <param name="index">The index to insert.</param>
            /// <param name="item">The item to insert.</param>
            protected override void InsertItem(int index, WebAssetItem item)
            {
                // If the instance object is null.
                if (item == null)
                    throw new System.ArgumentNullException("item");

                if (!AlreadyExists(item))
                    base.InsertItem(index, item);
            }

            /// <summary>
            /// Set the current item.
            /// </summary>
            /// <param name="index">The index to set at.</param>
            /// <param name="item">The item to set.</param>
            protected override void SetItem(int index, WebAssetItem item)
            {
                if (AlreadyExists(item))
                    throw new ArgumentException("Item with specified source already exists.", "item");

                base.SetItem(index, item);
            }

            /// <summary>
            /// Does the item alrady exist.
            /// </summary>
            /// <param name="item">The item to test.</param>
            /// <returns>True if it exists else false.</returns>
            private bool AlreadyExists(WebAssetItem item)
            {
                return this.Any(i => i != item && i.Source.IsCaseInsensitiveEqual(item.Source));
            }
        }
    }
}
