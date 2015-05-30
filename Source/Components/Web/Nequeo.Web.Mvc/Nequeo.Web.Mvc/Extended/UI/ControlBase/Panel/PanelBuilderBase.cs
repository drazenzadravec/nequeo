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

namespace Nequeo.Web.Mvc.Extended.UI.ControlBase
{
    /// <summary>
    /// Panel builder base type.
    /// </summary>
    /// <typeparam name="T">The type to eamine.</typeparam>
    /// <typeparam name="TBuilder">The textbox builder base</typeparam>
    public class PanelBuilderBase<T, TBuilder> : ComponentBuilderBase<PanelBase<T>, TBuilder>, IObjectMembers
        where T : class
        where TBuilder : PanelBuilderBase<T, TBuilder>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="component">The current text box component.</param>
        public PanelBuilderBase(PanelBase<T> component)
            : base(component)
        {
        }

        /// <summary>
        /// Get the indicator to use a pre-built control that this treeview extends.
        /// </summary>
        /// <param name="useExtendedControl">True to use control else use built trreview.</param>
        /// <returns>The builder instance.</returns>
        public TBuilder UseExtendedControl(bool useExtendedControl)
        {
            Component.UseExtendedControlOnly = useExtendedControl;
            return this as TBuilder;
        }
    }
}
