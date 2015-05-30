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
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

using Nequeo.Web.Mvc;

namespace Nequeo.Web.Mvc
{
    /// <summary>
    /// Mvc HTMLHelper extension for providing access to.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        private static readonly string KeyExtended = typeof(Extended.ComponentFactory).AssemblyQualifiedName + "_Extended";

        /// <summary>
        /// Gets the Component Factory for the extended UI.
        /// </summary>
        /// <param name="helper">The MVC html helper to extend.</param>
        /// <returns>The compoent factory reference for each UI.</returns>
        public static Extended.ComponentFactory NequeoUI(this HtmlHelper helper)
        {
            // If the instance object is null.
            if (helper == null)
                throw new System.ArgumentNullException("helper");

            ViewContext viewContext = helper.ViewContext;
            HttpContextBase httpContext = viewContext.HttpContext;
            Extended.ComponentFactory factory = httpContext.Items[KeyExtended] as Extended.ComponentFactory;

            if (factory == null)
            {
                // Get the current service locator singleton instance.
                Nequeo.Runtime.IServiceLocator locator = Extended.Factory.Runtime.ServiceLocator.Current;
                Extended.Factory.Runtime.ServiceLocator.Register(locator);

                // Collect information on the current page
                // including all style sheets and scripts
                // that may already be added, do not add these
                // style sheets or scripts if already present.
                Extended.WebAsset.IWebAssetItemMerger assetItemMerger = locator.Resolve<Extended.WebAsset.IWebAssetItemMerger>();
                Extended.UI.ScriptWrapperBase scriptWrapper = locator.Resolve<Extended.UI.ScriptWrapperBase>();
                Extended.Factory.IClientSideObjectWriterFactory clientSideObjectWriterFactory =
                    locator.Resolve<Extended.Factory.IClientSideObjectWriterFactory>();

                Extended.UI.StyleSheetRegistrar styleSheetRegistrar = new Extended.UI.StyleSheetRegistrar(
                    new Extended.WebAsset.WebAssetItemCollection(Extended.WebAsset.WebAssetDefaultSettings.StyleSheetFilesPath), 
                    viewContext, assetItemMerger);

                Extended.UI.ScriptRegistrar scriptRegistrar = new Extended.UI.ScriptRegistrar(
                    new Extended.WebAsset.WebAssetItemCollection(Extended.WebAsset.WebAssetDefaultSettings.ScriptFilesPath),
                    new List<Extended.Factory.IScriptableComponent>(), viewContext, assetItemMerger, scriptWrapper);

                Extended.UI.StyleSheetRegistrarBuilder styleSheetRegistrarBuilder = new Extended.UI.StyleSheetRegistrarBuilder(styleSheetRegistrar);
                Extended.UI.ScriptRegistrarBuilder scriptRegistrarBuilder = new Extended.UI.ScriptRegistrarBuilder(scriptRegistrar);

                // Execute the component factory.
                factory = new Extended.ComponentFactory(helper, clientSideObjectWriterFactory, styleSheetRegistrarBuilder, scriptRegistrarBuilder);
                helper.ViewContext.HttpContext.Items[KeyExtended] = factory;
            }

            // Return the compoent factory instance.
            return factory;
        }
    }
}
