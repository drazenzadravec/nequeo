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

using Nequeo.Web.Mvc.Extended.Factory;
using Nequeo.Web.Mvc.Extended.Factory.Runtime;

namespace Nequeo.Web.Mvc.Extended.WebAsset
{
    /// <summary>
    /// Shared web asset manager.
    /// </summary>
    public static class SharedGroup
    {
        private static readonly IDictionary<string, WebAssetItemGroup> styleSheets = 
            new Dictionary<string, WebAssetItemGroup>(StringComparer.OrdinalIgnoreCase);

        private static readonly IDictionary<string, WebAssetItemGroup> scripts = 
            new Dictionary<string, WebAssetItemGroup>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Executes the provided delegate that is used to configure stylesheets.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        public static void StyleSheets(Action<SharedGroupBuilder> configureAction)
        {
            Configure(WebAssetDefaultSettings.StyleSheetFilesPath, styleSheets, configureAction);
        }

        /// <summary>
        /// Executes the provided delegate that is used to configure scripts.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        public static void Scripts(Action<SharedGroupBuilder> configureAction)
        {
            Configure(WebAssetDefaultSettings.ScriptFilesPath, scripts, configureAction);
        }

        /// <summary>
        /// Find the style sheet group
        /// </summary>
        /// <param name="name">The name of the sheet</param>
        /// <returns>The web asset group</returns>
        internal static WebAssetItemGroup FindStyleSheetGroup(string name)
        {
            return FindInternal(styleSheets, name);
        }

        /// <summary>
        /// Find the script group
        /// </summary>
        /// <param name="name">>The name of the script</param>
        /// <returns>The web asset group</returns>
        internal static WebAssetItemGroup FindScriptGroup(string name)
        {
            return FindInternal(scripts, name);
        }

        /// <summary>
        /// Find all internal shared web assest.
        /// </summary>
        /// <param name="lookup">The collection of web assets</param>
        /// <param name="name">The name</param>
        /// <returns>The web asset group</returns>
        private static WebAssetItemGroup FindInternal(IDictionary<string, WebAssetItemGroup> lookup, string name)
        {
            WebAssetItemGroup group;
            return lookup.TryGetValue(name, out group) ? group : null;
        }

        /// <summary>
        /// Configue
        /// </summary>
        /// <param name="defaultPath">The default path to the web assests.</param>
        /// <param name="target">The collection of web assests</param>
        /// <param name="configureAction">The shared web asset group bulder action</param>
        private static void Configure(string defaultPath, IDictionary<string, WebAssetItemGroup> target, Action<SharedGroupBuilder> configureAction)
        {
            SharedGroupBuilder builder = new SharedGroupBuilder(defaultPath, target);
            configureAction(builder);
        }
    }
}
