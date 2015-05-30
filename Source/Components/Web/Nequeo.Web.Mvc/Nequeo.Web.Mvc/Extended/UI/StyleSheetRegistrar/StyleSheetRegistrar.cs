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
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

using Nequeo.Extension;
using Nequeo.Net.Http.Extension;
using Nequeo.Collections.Extension;
using Nequeo.Web.Mvc.Extended.WebAsset;

namespace Nequeo.Web.Mvc.Extended.UI
{
    /// <summary>
    /// Manages ASP.NET MVC views style sheet files.
    /// </summary>
    public class StyleSheetRegistrar
    {
        /// <summary>
        /// Used to ensure that the same instance is used for the same HttpContext.
        /// </summary>
        public static readonly string Key = typeof(StyleSheetRegistrar).AssemblyQualifiedName;

        private string _assetHandlerPath;
        private bool _hasRendered;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleSheetRegistrar"/> class.
        /// </summary>
        /// <param name="styleSheets">The style sheets.</param>
        /// <param name="viewContext">The view context.</param>
        /// <param name="assetItemMerger">The asset merger.</param>
        public StyleSheetRegistrar(WebAssetItemCollection styleSheets, ViewContext viewContext, IWebAssetItemMerger assetItemMerger)
        {
            // If the instance object is null.
            if (styleSheets == null) throw new System.ArgumentNullException("styleSheets");
            if (viewContext == null) throw new System.ArgumentNullException("viewContext");
            if (assetItemMerger == null) throw new System.ArgumentNullException("assetItemMerger");

            if (viewContext.HttpContext.Items[Key] != null)
                throw new InvalidOperationException("Only one style sheet registrar is allowed in a single request");

            viewContext.HttpContext.Items[Key] = this;

            DefaultGroup = new WebAssetItemGroup("default", false) { DefaultPath = WebAssetDefaultSettings.StyleSheetFilesPath };
            StyleSheets = styleSheets;
            ViewContext = viewContext;
            AssetMerger = assetItemMerger;

            AssetHandlerPath = WebAssetHttpHandler.DefaultPath;
        }

        /// <summary>
        /// Gets or sets the asset handler path. Path must be a virtual path. The default value is set to WebAssetHttpHandler.DefaultPath.
        /// </summary>
        /// <value>The asset handler path.</value>
        public string AssetHandlerPath
        {
            get { return _assetHandlerPath; }
            set { _assetHandlerPath = value; }
        }

        /// <summary>
        /// Gets or sets the default group.
        /// </summary>
        /// <value>The default group.</value>
        public WebAssetItemGroup DefaultGroup
        {
            get; private set;
        }

        /// <summary>
        /// Gets the stylesheets that will be rendered in the view.
        /// </summary>
        /// <value>The style sheets.</value>
        public WebAssetItemCollection StyleSheets
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the view context.
        /// </summary>
        /// <value>The view context.</value>
        protected ViewContext ViewContext
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the asset merger.
        /// </summary>
        /// <value>The asset merger.</value>
        protected IWebAssetItemMerger AssetMerger
        {
            get; private set;
        }

        /// <summary>
        /// Writes the stylesheets in the response.
        /// </summary>
        public void Render()
        {
            if (_hasRendered)
                throw new InvalidOperationException("You cannot call render more than once");

            if (ViewContext.HttpContext.Request.Browser.SupportsCss)
                Write(ViewContext.HttpContext.Response.Output);

            _hasRendered = true;
        }

        /// <summary>
        /// Writes all stylesheet source.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void Write(TextWriter writer)
        {
            IList<string> mergedList = new List<string>();

            bool isSecured = ViewContext.HttpContext.Request.IsSecureConnection;
            bool canCompress = ViewContext.HttpContext.Request.CanCompress();

            Action<WebAssetItemCollection> append = assets =>
                    {
                        IList<string> result = AssetMerger.Merge("text/css", AssetHandlerPath, isSecured, canCompress, assets);

                        if (!result.IsNullOrEmpty())
                            mergedList.AddRange(result);
                    };

            if (!DefaultGroup.Items.IsEmpty())
                append(new WebAssetItemCollection(DefaultGroup.DefaultPath) { DefaultGroup });

            if (!StyleSheets.IsEmpty())
                append(StyleSheets);

            if (!mergedList.IsEmpty())
                foreach (string stylesheet in mergedList)
                    writer.WriteLine("<link type=\"text/css\" href=\"{0}\" rel=\"stylesheet\"/>".FormatWith(stylesheet));
        }
    }
}