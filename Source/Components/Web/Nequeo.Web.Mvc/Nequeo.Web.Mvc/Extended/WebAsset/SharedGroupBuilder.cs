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
    /// Builder class for fluently configuring the shared group.
    /// </summary>
    public class SharedGroupBuilder : IObjectMembers
    {
        private readonly string _defaultPath;
        private readonly IDictionary<string, WebAssetItemGroup> _assets;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="defaultPath">The default path.</param>
        /// <param name="assets">The assets.</param>
        public SharedGroupBuilder(string defaultPath, IDictionary<string, WebAssetItemGroup> assets)
        {
            _defaultPath = defaultPath;
            _assets = assets;
        }

        /// <summary>
        /// Adds the group.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configureAction">The configure action.</param>
        /// <returns>The shared asset builder.</returns>
        public virtual SharedGroupBuilder AddGroup(string name, Action<WebAssetItemGroupBuilder> configureAction)
        {
            WebAssetItemGroup group;

            if (_assets.TryGetValue(name, out group))
                throw new ArgumentException("Group with specified name already exists please specify a different name", "name");

            group = new WebAssetItemGroup(name, true) { DefaultPath = _defaultPath };
            _assets.Add(name, group);

            WebAssetItemGroupBuilder builder = new WebAssetItemGroupBuilder(group);
            configureAction(builder);
            return this;
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configureAction">The configure action.</param>
        /// <returns>The shared asset builder.</returns>
        public virtual SharedGroupBuilder GetGroup(string name, Action<WebAssetItemGroupBuilder> configureAction)
        {
            WebAssetItemGroup group;

            if (!_assets.TryGetValue(name, out group))
                throw new ArgumentException("Group with specified name does not exist please make sure you have specified a correct name", "name");

            WebAssetItemGroupBuilder builder = new WebAssetItemGroupBuilder(group);
            configureAction(builder);
            return this;
        }
    }
}
