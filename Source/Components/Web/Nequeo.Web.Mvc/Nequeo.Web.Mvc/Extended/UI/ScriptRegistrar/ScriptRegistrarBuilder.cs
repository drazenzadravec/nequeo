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
using System.ComponentModel;

using Nequeo.Web.Mvc.Extended.WebAsset;

namespace Nequeo.Web.Mvc.Extended.UI
{
    /// <summary>
    /// Defines the fluent interface for configuring the component.
    /// </summary>
    public class ScriptRegistrarBuilder : IObjectMembers
    {
        private readonly ScriptRegistrar _scriptRegistrar;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="scriptRegistrar">The script registrar.</param>
        public ScriptRegistrarBuilder(ScriptRegistrar scriptRegistrar)
        {
            // If the instance object is null.
            if (scriptRegistrar == null) throw new System.ArgumentNullException("scriptRegistrar");

            _scriptRegistrar = scriptRegistrar;
        }

        /// <summary>
        /// Performs an implicit conversion.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ScriptRegistrar(ScriptRegistrarBuilder builder)
        {
            // If the instance object is null.
            if (builder == null) throw new System.ArgumentNullException("builder");

            return builder.ToRegistrar();
        }

        /// <summary>
        /// Returns the internal script registrar.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScriptRegistrar ToRegistrar()
        {
            return _scriptRegistrar;
        }

        /// <summary>
        /// Sets the asset handler path. Path must be a virtual path.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///            .AssetHandlerPath("~/asset.axd")
        /// %&gt;
        /// </code>
        /// </example>
        public virtual ScriptRegistrarBuilder AssetHandlerPath(string value)
        {
            _scriptRegistrar.AssetHandlerPath = value;

            return this;
        }

        /// <summary>
        /// Configures the <see cref="ScriptRegistrar.DefaultGroup"/>.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///            .DefaultGroup(group => group
        ///                 .Add("script1.js")
        ///                 .Add("script2.js")
        ///                 .Combined(true)
        ///            )
        /// %&gt;
        /// </code>
        /// </example>
        public virtual ScriptRegistrarBuilder DefaultGroup(Action<WebAssetItemGroupBuilder> configureAction)
        {
            // If the instance object is null.
            if (configureAction == null) throw new System.ArgumentNullException("configureAction");

            WebAssetItemGroupBuilder builder = new WebAssetItemGroupBuilder(_scriptRegistrar.DefaultGroup);
            configureAction(builder);

            return this;
        }

        /// <summary>
        /// Enables globalization support.
        /// </summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///            .Globalization(true)
        /// %&gt;
        /// </code>
        /// </example>
        public virtual ScriptRegistrarBuilder Globalization(bool enable)
        {
            _scriptRegistrar.EnableGlobalization = enable;
            return this;
        }

        /// <summary>
        /// Includes the jQuery script files. By default jQuery JavaScript is included. 
        /// </summary>
        /// <remarks>
        /// Nequeo Extensions for ASP.NET MVC require jQuery so make sure you manually include the JavaScrip file
        /// if you disable the automatic including.
        /// </remarks>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().ScriptRegistrar()
        ///            .jQuery(false)
        /// %&gt;
        /// </code>
        /// </example>
        public virtual ScriptRegistrarBuilder jQuery(bool enable)
        {
            _scriptRegistrar.ExcludeFrameworkScripts = !enable;
            return this;
        }

        /// <summary>
        /// Executes the provided delegate that is used to register the script files fluently in different groups.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        /// <returns></returns>
        public virtual ScriptRegistrarBuilder Scripts(Action<WebAssetItemCollectionBuilder> configureAction)
        {
            // If the instance object is null.
            if (configureAction == null) throw new System.ArgumentNullException("configureAction");

            WebAssetItemCollectionBuilder builder = new WebAssetItemCollectionBuilder(WebAssetType.JavaScript, _scriptRegistrar.Scripts);
            configureAction(builder);

            return this;
        }

        /// <summary>
        /// Defines the inline handler executed when the DOM document is ready (using the $(document).ready jQuery event)
        /// </summary>
        /// <param name="onDocumentReadyAction">The action defining the inline handler</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;% Html.NequeoUI().ScriptRegistrar()
        ///           .OnDocumentReady(() =>
        ///           {
        ///             %&gt;
        ///             function() {
        ///                 alert("Document is ready");
        ///             }
        ///             &lt;%
        ///           })
        ///           .Render();
        /// %&gt;
        /// </code>
        /// </example>
        public virtual ScriptRegistrarBuilder OnDocumentReady(Action onDocumentReadyAction)
        {
            // If the instance object is null.
            if (onDocumentReadyAction == null) throw new System.ArgumentNullException("onDocumentReadyAction");

            _scriptRegistrar.OnDocumentReadyActions.Add(onDocumentReadyAction);
            return this;
        }

        /// <summary>
        /// Appends the specified statement in $(document).ready jQuery event. This method should be
        /// used in <code>Html.RenderAction()</code>.
        /// </summary>
        /// <param name="statements">The statements.</param>
        /// <returns></returns>
        public virtual ScriptRegistrarBuilder OnDocumentReady(string statements)
        {
            // If the instance object is null.
            if (statements == null) throw new System.ArgumentNullException("statements");

            _scriptRegistrar.OnDocumentReadyStatements.Add(statements);
            return this;
        }

        /// <summary>
        /// Defines the inline handler executed when the DOM window object is unloaded.
        /// </summary>
        /// <param name="onWindowUnloadAction">The action defining the inline handler</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;% Html.NequeoUI().ScriptRegistrar()
        ///           .OnWindowUnload(() =>
        ///           {
        ///             %&gt;
        ///             function() {
        ///                 // event handler code
        ///             }
        ///             &lt;%
        ///           })
        ///           .Render();
        /// %&gt;
        /// </code>
        /// </example>
        public virtual ScriptRegistrarBuilder OnWindowUnload(Action onWindowUnloadAction)
        {
            // If the instance object is null.
            if (onWindowUnloadAction == null) throw new System.ArgumentNullException("onWindowUnloadAction");

            _scriptRegistrar.OnWindowUnloadActions.Add(onWindowUnloadAction);
            return this;
        }

        /// <summary>
        /// Appends the specified statement window unload event. This method should be
        /// used in <code>Html.RenderAction()</code>.
        /// </summary>
        /// <param name="statements">The statements.</param>
        /// <returns></returns>
        public virtual ScriptRegistrarBuilder OnWindowUnload(string statements)
        {
            // If the instance object is null.
            if (statements == null) throw new System.ArgumentNullException("statements");

            _scriptRegistrar.OnWindowUnloadStatements.Add(statements);
            return this;
        }

        /// <summary>
        /// Renders the component.
        /// </summary>
        public virtual void Render()
        {
            _scriptRegistrar.Render();
        }

        /// <summary>
        /// Called when the render of all component attributes has
        /// been built. Calls the component render method, which
        /// writes directly to the output stram.
        /// </summary>
        /// <returns>Null because the response output stram is used instead.</returns>
        public override string ToString()
        {
            Render();
            return null;
        }
    }
}