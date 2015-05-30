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
    /// Style sheet builder.
    /// </summary>
    public class StyleSheetRegistrarBuilder : IObjectMembers
    {
        private readonly StyleSheetRegistrar styleSheetRegistrar;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="styleSheetRegistrar">The style sheet registrar.</param>
        public StyleSheetRegistrarBuilder(StyleSheetRegistrar styleSheetRegistrar)
        {
            // If the instance object is null.
            if (styleSheetRegistrar == null) throw new System.ArgumentNullException("styleSheetRegistrar");

            this.styleSheetRegistrar = styleSheetRegistrar;
        }

        /// <summary>
        /// Performs an implicit conversion.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator StyleSheetRegistrar(StyleSheetRegistrarBuilder builder)
        {
            // If the instance object is null.
            if (builder == null) throw new System.ArgumentNullException("builder");

            return builder.ToRegistrar();
        }

        /// <summary>
        /// Returns the internal style sheet registrar.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StyleSheetRegistrar ToRegistrar()
        {
            return styleSheetRegistrar;
        }

        /// <summary>
        /// Sets the asset handler path. Path must be a virtual path.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().StyleSheetRegistrar()
        ///            .AssetHandlerPath("~/asset.axd")
        /// %&gt;
        /// </code>
        /// </example>
        public virtual StyleSheetRegistrarBuilder AssetHandlerPath(string value)
        {
            styleSheetRegistrar.AssetHandlerPath = value;
            return this;
        }

        /// <summary>
        /// Configures the <see cref="StyleSheetRegistrar.DefaultGroup"/>.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        /// <example>
        /// <code lang="CS">
        /// &lt;%= Html.NequeoUI().StyleSheetRegistrar()
        ///            .DefaultGroup(group => group
        ///                 .Add("style1.css")
        ///                 .Add("style2.css")
        ///                 .Combined(true)
        ///            )
        /// %&gt;
        /// </code>
        /// </example>
        public virtual StyleSheetRegistrarBuilder DefaultGroup(Action<WebAssetItemGroupBuilder> configureAction)
        {
            // If the instance object is null.
            if (configureAction == null) throw new System.ArgumentNullException("configureAction");

            WebAssetItemGroupBuilder builder = new WebAssetItemGroupBuilder(styleSheetRegistrar.DefaultGroup);
            configureAction(builder);

            return this;
        }

        /// <summary>
        /// Executes the provided delegate that is used to register the stylesheet files fluently.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        /// <returns></returns>
        public virtual StyleSheetRegistrarBuilder StyleSheets(Action<WebAssetItemCollectionBuilder> configureAction)
        {
            // If the instance object is null.
            if (configureAction == null) throw new System.ArgumentNullException("configureAction");

            WebAssetItemCollectionBuilder builder = new WebAssetItemCollectionBuilder(WebAssetType.StyleSheet, styleSheetRegistrar.StyleSheets);
            configureAction(builder);

            return this;
        }

        /// <summary>
        /// Renders the component.
        /// </summary>
        public virtual void Render()
        {
            styleSheetRegistrar.Render();
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