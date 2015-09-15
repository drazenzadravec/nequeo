/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Windows;

namespace Nequeo.Wpf.Controls
{
    public class RibbonApplicationMenuItem:RibbonMenuItem
    {
        static RibbonApplicationMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonApplicationMenuItem), new FrameworkPropertyMetadata(typeof(RibbonApplicationMenuItem)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonMenuItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonApplicationMenuItem();
        }



        /// <summary>
        /// Gets or sets the title for the sub menu popup.
        /// This is a dependency property.
        /// </summary>
        public object SubMenuTitle
        {
            get { return (object)GetValue(SubMenuTitleProperty); }
            set { SetValue(SubMenuTitleProperty, value); }
        }

        public static readonly DependencyProperty SubMenuTitleProperty =
            DependencyProperty.Register("SubMenuTitle", typeof(object), typeof(RibbonApplicationMenuItem), new UIPropertyMetadata(null));



        /// <summary>
        /// Gets or sets the content that appears in the right part of the ApplicationMenu.
        /// </summary>
        public object SubMenuContent
        {
            get { return (object)GetValue(SubMenuContentProperty); }
            set { SetValue(SubMenuContentProperty, value); }
        }

        public static readonly DependencyProperty SubMenuContentProperty =
            DependencyProperty.Register("SubMenuContent", typeof(object), typeof(RibbonApplicationMenuItem), new UIPropertyMetadata(null));



    }
}
