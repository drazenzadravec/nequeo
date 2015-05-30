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
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.UI;
using System.Web.Routing;
using System.ComponentModel;
using System.Diagnostics;

using Nequeo.Web.Mvc.Extended.WebAsset;
using Nequeo.Web.Mvc.Extended.Factory;

namespace Nequeo.Web.Mvc.Extended.UI.ControlBase
{
    /// <summary>
    /// Base script container.
    /// </summary>
    /// <typeparam name="T">The type of script access.</typeparam>
    public class ScriptBase<T>
        where T : class
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="viewContext">The current Mvc view context.</param>
        /// <param name="clientSideObjectWriterFactory">The client side object writer factory.</param>
        public ScriptBase(ViewContext viewContext, IClientSideObjectWriterFactory clientSideObjectWriterFactory)
        {
            // Get the current context.
            ViewContext = viewContext;
            ClientSideObjectWriterFactory = clientSideObjectWriterFactory;

            // Create a new instance of the web asset item group manager.
            Scripts = new WebAssetItemGroup("default") { DefaultPath = WebAssetDefaultSettings.ScriptFilesPath };
        }

        /// <summary>
        /// Gets or sets the view context to rendering a view.
        /// </summary>
        public ViewContext ViewContext
        {
            get; private set;
        }

        /// <summary>
        /// Gets the client side object writer factory.
        /// </summary>
        public IClientSideObjectWriterFactory ClientSideObjectWriterFactory
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets exclude the jQuery framework scripts
        /// </summary>
        public bool ExcludeJQueryScripts { get; set; }

        /// <summary>
        /// Gets the default script group.
        /// </summary>
        public WebAssetItemGroup Scripts
        {
            get; private set;
        }

        /// <summary>
        /// Write the html data to the current Mvc view context response output stream.
        /// </summary>
        /// <param name="writer">The html text writer for the component.</param>
        protected virtual void WriteHtml(HtmlTextWriter writer)
        { 
        }

        /// <summary>
        /// Renders the component.
        /// </summary>
        public void Render()
        {
            using (HtmlTextWriter textWriter = new HtmlTextWriter(ViewContext.HttpContext.Response.Output))
                WriteHtml(textWriter);
        }
    }
}
