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
using Nequeo.Web.Mvc.Extended.WebAsset;

namespace Nequeo.Web.Mvc.Extended.UI.ControlBase
{
    /// <summary>
    /// Base script builder.
    /// </summary>
    /// <typeparam name="T">The type of script access.</typeparam>
    /// <typeparam name="TBuilder">The builder base context.</typeparam>
    public class ScriptBuilderBase<T, TBuilder> : IObjectMembers
        where T : class
        where TBuilder : ScriptBuilderBase<T, TBuilder>
    {
        private readonly ScriptBase<T> _component;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="component">The current script component.</param>
        public ScriptBuilderBase(ScriptBase<T> component)
        {
            _component = component;
        }

        /// <summary>
        /// Gets the current component instance.
        /// </summary>
        protected internal ScriptBase<T> Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Exclude the jQuery framework scripts
        /// </summary>
        /// <param name="enable">True to exclude else false</param>
        /// <returns>The current script builder</returns>
        public TBuilder JQuery(bool enable)
        {
            Component.ExcludeJQueryScripts = !enable;
            return this as TBuilder;
        }

        /// <summary>
        /// Call the script action handler.
        /// </summary>
        /// <param name="configurationAction">The configuration action.</param>
        /// <returns>The current script builder</returns>
        public TBuilder Scripts(Action<WebAssetItemGroupBuilder> configurationAction)
        {
            // If the instance object is null.
            if (configurationAction == null)
                throw new System.ArgumentNullException("action");

            // Create the web asset builder and execute the action.
            WebAssetItemGroupBuilder builder = new WebAssetItemGroupBuilder(Component.Scripts);
            configurationAction(builder);

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
