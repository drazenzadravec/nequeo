/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nequeo.Wpf.UI.Printing
{
    /// <summary>
    /// Visual container.
    /// </summary>
    internal class VisualContainer : FrameworkElement
    {
        /// <summary>
        /// Visual container.
        /// </summary>
        internal VisualContainer()
            : base()
        {
        }

        private Visual _pageVisual;
        private Size _pageSize;

        /// <summary>
        /// Gets or sets the vector offset.
        /// </summary>
        internal Vector Offset
        {
            get
            {
                return VisualOffset;
            }
            set
            {
                VisualOffset = value;
            }
        }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        internal Transform Transform
        {
            get
            {
                return VisualTransform;
            }
            set
            {
                VisualTransform = value;
            }
        }

        /// <summary>
        /// Gets or sets the page visual.
        /// </summary>
        internal Visual PageVisual
        {
            get
            {
                return _pageVisual;
            }
            set
            {
                this.RemoveVisualChild(_pageVisual);
                this.AddVisualChild(value);
                _pageVisual = value;
            }
        }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        internal Size PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = value;
                Width = _pageSize.Width;
                Height = _pageSize.Height;
            }
        }

        /// <summary>
        /// Gets the visual child on the page.
        /// </summary>
        /// <param name="index">The current page index.</param>
        /// <returns>The child page visual.</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0 || _pageVisual == null)
            {
                throw new ArgumentOutOfRangeException("index", index, "Illegal child index");
            }

            return _pageVisual;
        }

        /// <summary>
        /// Gets the visual children count.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return _pageVisual != null ? 1 : 0; }
        }
    }
}
