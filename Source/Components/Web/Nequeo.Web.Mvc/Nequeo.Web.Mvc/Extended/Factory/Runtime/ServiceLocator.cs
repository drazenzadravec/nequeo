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
using System.Web;
using System.Collections.Generic;
using System.Web.Routing;

using Nequeo.Web.Mvc.Extended.WebAsset;
using Nequeo.Web.Mvc.Extended.UI;

namespace Nequeo.Web.Mvc.Extended.Factory.Runtime
{
    /// <summary>
    /// Service locator for the current Mvc context view.
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// Get of create the current singleton for the Mvc view and the current http context
        /// </summary>
        public static Nequeo.Runtime.IServiceLocator Current
        {
            get { return Nequeo.Runtime.ServiceLocator.Current; }
        }

        /// <summary>
        /// Register the initial services.
        /// </summary>
        /// <param name="serviceLocator">The current singleton service locator.</param>
        internal static void Register(Nequeo.Runtime.IServiceLocator serviceLocator)
        {
            serviceLocator.Register<RouteCollection>(locator => RouteTable.Routes);
            serviceLocator.Register<IVirtualPathProvider>(locator => new VirtualPathProviderWrapper());
            serviceLocator.Register<IPathResolver>(locator => new PathResolver());
            serviceLocator.Register<ICacheManager>(locator => new CacheManagerWrapper());
            serviceLocator.Register<IUrlResolver>(locator => new UrlResolver());
            serviceLocator.Register<IUrlEncoder>(locator => new UrlEncoder());
            serviceLocator.Register<IConfigurationManager>(locator => new ConfigurationManagerWrapper());
            serviceLocator.Register<IHttpResponseCacher>(locator => new HttpResponseCacher());
            serviceLocator.Register<IHttpResponseCompressor>(locator => new HttpResponseCompressor());
            serviceLocator.Register<IWebAssetLocator>(locator => new WebAssetLocator(false, locator.Resolve<IVirtualPathProvider>()));
            serviceLocator.Register<IWebAssetRegistry>(locator => new WebAssetRegistry(false, locator.Resolve<ICacheManager>(), locator.Resolve<IWebAssetLocator>(), locator.Resolve<IUrlResolver>(), locator.Resolve<IPathResolver>(), locator.Resolve<IVirtualPathProvider>()));
            serviceLocator.Register<IWebAssetItemMerger>(locator => new WebAssetItemMerger(locator.Resolve<IWebAssetRegistry>(), locator.Resolve<IUrlResolver>(), locator.Resolve<IUrlEncoder>()));
            serviceLocator.Register<IClientSideObjectWriterFactory>(locator => new ClientSideObjectWriterFactory());
            serviceLocator.Register<ScriptWrapperBase>(locator => new ScriptWrapper());
        }
    }
}