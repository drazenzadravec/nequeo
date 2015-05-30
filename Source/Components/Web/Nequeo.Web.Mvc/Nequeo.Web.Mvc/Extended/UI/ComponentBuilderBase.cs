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

using Nequeo.Collections.Extension;

namespace Nequeo.Web.Mvc.Extended.UI
{
    /// <summary>
    /// The base component builder type for all components.
    /// </summary>
    public abstract class ComponentBuilderBase<TComponent, TBuilder> : IObjectMembers
        where TComponent : ComponentBase
        where TBuilder : ComponentBuilderBase<TComponent, TBuilder>
    {
        private readonly TComponent _component;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="component">The current component.</param>
        protected ComponentBuilderBase(TComponent component)
        {
            _component = component;
        }

        /// <summary>
        /// Gets the current component instance.
        /// </summary>
        protected internal TComponent Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Add the name to the component.
        /// </summary>
        /// <param name="componentName">The name of the component.</param>
        /// <returns>The current component builder.</returns>
        public virtual TBuilder Name(string componentName)
        {
            Component.Name = componentName;
            return this as TBuilder;
        }

        /// <summary>
        /// Sets the web asset key for the component.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The current component builder.</returns>
        public virtual TBuilder AssetKey(string key)
        {
            Component.AssetKey = key;
            return this as TBuilder;
        }

        /// <summary>
        /// Add the script files path.
        /// </summary>
        /// <param name="path">The path to the script files.</param>
        /// <returns>The current component builder.</returns>
        public virtual TBuilder ScriptFilesPath(string path)
        {
            Component.ScriptFilesPath = path;
            return this as TBuilder;
        }

        /// <summary>
        /// Add the collection of script files.
        /// </summary>
        /// <param name="names">The names of all the scripts.</param>
        /// <returns>The current component builder.</returns>
        public virtual TBuilder ScriptFileNames(params string[] names)
        {
            // If the instance object is null.
            if (names == null)
                throw new System.ArgumentNullException("names");

            Component.ScriptFileNames.Clear();
            Component.ScriptFileNames.AddRange(names);

            return this as TBuilder;
        }

        /// <summary>
        /// Add the collection of component attributes.
        /// </summary>
        /// <param name="attributes">The attributes to add.</param>
        /// <returns>The current component builder.</returns>
        public virtual TBuilder HtmlAttributes(object attributes)
        {
            // If the instance object is null.
            if (attributes == null)
                throw new System.ArgumentNullException("attributes");

            Component.HtmlAttributes.Clear();
            Component.HtmlAttributes.Merge(attributes);

            return this as TBuilder;
        }

        /// <summary>
        /// Renders the component.
        /// </summary>
        public virtual void Render()
        {
            Component.Render();
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
