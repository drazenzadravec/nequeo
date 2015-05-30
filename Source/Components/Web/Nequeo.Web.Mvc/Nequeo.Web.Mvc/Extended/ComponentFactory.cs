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
using System.ComponentModel;
using System.Diagnostics;

using Nequeo.Web.Mvc.Extended;
using Nequeo.Web.Mvc.Extended.Factory;
using Nequeo.Web.Mvc.Extended.UI;

namespace Nequeo.Web.Mvc.Extended
{
    /// <summary>
    /// The global component factory builder.
    /// </summary>
    public class ComponentFactory
    {
        private readonly StyleSheetRegistrarBuilder _styleSheetRegistrarBuilder;
        private readonly ScriptRegistrarBuilder _scriptRegistrarBuilder;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="htmlHelper">The current Mvc HtmlHelper context.</param>
        /// <param name="clientSideObjectWriterFactory"></param>
        /// <param name="styleSheetRegistrar"></param>
        /// <param name="scriptRegistrar"></param>
        public ComponentFactory(
            HtmlHelper htmlHelper, 
            IClientSideObjectWriterFactory clientSideObjectWriterFactory,
            StyleSheetRegistrarBuilder styleSheetRegistrar,
            ScriptRegistrarBuilder scriptRegistrar)
        {
            // If the instance object is null.
            if (htmlHelper == null) throw new System.ArgumentNullException("htmlHelper");
            if (clientSideObjectWriterFactory == null) throw new System.ArgumentNullException("clientSideObjectWriterFactory");
            if (styleSheetRegistrar == null) throw new System.ArgumentNullException("styleSheetRegistrar");
            if (scriptRegistrar == null) throw new System.ArgumentNullException("scriptRegistrar");

            HtmlHelper = htmlHelper;
            ClientSideObjectWriterFactory = clientSideObjectWriterFactory;

            _styleSheetRegistrarBuilder = styleSheetRegistrar;
            _scriptRegistrarBuilder = scriptRegistrar;
        }

        /// <summary>
        /// Gets the curent HtmlHelper context.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HtmlHelper HtmlHelper { get; private set; }

        /// <summary>
        /// Gets the current client side object writer factory.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IClientSideObjectWriterFactory ClientSideObjectWriterFactory { get; private set; }

        /// <summary>
        /// Gets the current Mvc view context.
        /// </summary>
        private ViewContext ViewContext
        {
            get { return HtmlHelper.ViewContext; }
        }

        /// <summary>
        /// Creates a <see cref="StyleSheetRegistrar"/>
        /// </summary>
        public StyleSheetRegistrarBuilder StyleSheetRegistrar()
        {
            return _styleSheetRegistrarBuilder;
        }

        /// <summary>
        /// Creates a <see cref="ScriptRegistrar"/>
        /// </summary>
        public ScriptRegistrarBuilder ScriptRegistrar()
        {
            return _scriptRegistrarBuilder;
        }

        /// <summary>
        /// Create a new stylesheet component with the current view context.
        /// </summary>
        /// <returns>The style sheet builder.</returns>
        /// <example>
        /// <code lang="CS">
        ///  &lt;%= Html.NequeoUI().StyleSheet()
        ///             .StyleSheets(group => group
        ///                        .Add("Site.css")
        ///                        .Add("nequeo.common.css")
        ///                        .Add("nequeo.vista.css")
        ///                        .Compressed(true)
        ///             )
        /// %&gt;
        /// </code>
        /// </example>
        public virtual UI.StyleSheetBuilder StyleSheet()
        {
            return new UI.StyleSheetBuilder(new UI.StyleSheet(ViewContext, ClientSideObjectWriterFactory));
        }

        /// <summary>
        /// Create a new javascript component with the current view context.
        /// </summary>
        /// <returns>The java script builder.</returns>
        /// <example>
        /// <code lang="CS">
        ///  &lt;%= Html.NequeoUI().JavaScript()
        ///             .Scripts(group => group
        ///                        .Add("Site.css")
        ///                        .Add("nequeo.common.js")
        ///                        .Add("nequeo.vista.js")
        ///                        .Compressed(true)
        ///             )
        /// %&gt;
        /// </code>
        /// </example>
        public virtual UI.JavaScriptBuilder JavaScript()
        {
            return new UI.JavaScriptBuilder(new UI.JavaScript(ViewContext, ClientSideObjectWriterFactory));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual UI.TreeViewBuilder TreeView()
        {
            return new UI.TreeViewBuilder(Create(() => new UI.TreeView(ViewContext, ClientSideObjectWriterFactory)));
        }

        /// <summary>
        /// Create a new component factory.
        /// </summary>
        /// <typeparam name="TComponent">The current component type.</typeparam>
        /// <param name="factory">The execution function expression.</param>
        /// <returns>The passed component base type.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TComponent Create<TComponent>(Func<TComponent> factory) where TComponent : ComponentBase
        {
            // Execute the action and return the component base.
            TComponent component = factory();

            // Register the component scripts with the
            // script rigistrar, adds the scripts to the collection.
            _scriptRegistrarBuilder.ToRegistrar().Register(component);
            return component;
        }
    }
}
