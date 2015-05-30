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

using Nequeo.Web.Mvc.Extended.Factory;
using Nequeo.Web.Mvc.Extended.WebAsset;

namespace Nequeo.Web.Mvc.Extended.UI
{
    /// <summary>
    /// The base component type for all components.
    /// </summary>
    public abstract class ComponentBase : IHtmlAttributesContainer, IScriptableComponent
    {
        private string _name;
        private string _scriptFilesLocation;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="viewContext">The current Mvc view context.</param>
        /// <param name="clientSideObjectWriterFactory">The client side object writer factory.</param>
        protected ComponentBase(ViewContext viewContext, IClientSideObjectWriterFactory clientSideObjectWriterFactory)
        {
            // If the instance object is null.
            if (viewContext == null) throw new System.ArgumentNullException("viewContext");
            if (clientSideObjectWriterFactory == null) throw new System.ArgumentNullException("clientSideObjectWriterFactory");

            // Get the current context.
            ViewContext = viewContext;
            ClientSideObjectWriterFactory = clientSideObjectWriterFactory;

            // Get the location of each internally used scripts
            ScriptFilesPath = WebAssetDefaultSettings.ScriptFilesPath;
            ScriptFileNames = new List<string>();

            // Initalise the attribite dictoinary
            HtmlAttributes = new RouteValueDictionary();

            // Get the is ajax request indicator.
            IsSelfInitialized = (ViewContext.HttpContext.Items["$SelfInitialize$"] != null) || ViewContext.HttpContext.Request.IsAjaxRequest();
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
        /// Gets or sets the script files path. Path must be a virtual path.
        /// </summary>
        public string ScriptFilesPath
        {
            get { return _scriptFilesLocation; }
            set { _scriptFilesLocation = value; }
        }

        /// <summary>
        /// Gets or sets the name of the conmponent.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the asset key.
        /// </summary>
        /// <value>The asset key.</value>
        public string AssetKey { get; set; }

        /// <summary>
        /// Gets the ID of the current compoonent.
        /// </summary>
        public string Id
        {
            get
            {
                // Return from htmlattributes if user has specified
                // otherwise build it from name
                return HtmlAttributes.ContainsKey("id") ?
                       HtmlAttributes["id"].ToString() :
                       (!string.IsNullOrEmpty(Name) ? Name.Replace(".", HtmlHelper.IdAttributeDotReplacement) : null);
            }
        }

        /// <summary>
        /// Gets the HTML attributes.
        /// </summary>
        public IDictionary<string, object> HtmlAttributes
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the script file names.
        /// </summary>
        public IList<string> ScriptFileNames
        {
            get; private set;
        }

        /// <summary>
        /// Gets is self initialized
        /// </summary>
        public bool IsSelfInitialized
        {
            get; private set;
        }

        /// <summary>
        /// Write the html data to the current Mvc view context response output stream.
        /// </summary>
        /// <param name="writer">The html text writer for the component.</param>
        protected virtual void WriteHtml(HtmlTextWriter writer)
        {
            // If Is Ajax Request
            if (IsSelfInitialized)
            {
                // Write the the initialization script
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                WriteInitializationScript(writer);
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Writes the initialization script.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void WriteInitializationScript(TextWriter writer)
        {
        }

        /// <summary>
        /// Writes the cleanup script.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void WriteCleanupScript(TextWriter writer)
        {
        }

        /// <summary>
        /// Renders the component.
        /// </summary>
        public void Render()
        {
            EnsureRequired();

            using (HtmlTextWriter textWriter = new HtmlTextWriter(ViewContext.HttpContext.Response.Output))
                WriteHtml(textWriter);
        }

        /// <summary>
        /// Ensures the required settings.
        /// </summary>
        protected virtual void EnsureRequired()
        {
            if (string.IsNullOrEmpty(Name))
                throw new InvalidOperationException("Name cannot be blank.");
        }
    }
}
