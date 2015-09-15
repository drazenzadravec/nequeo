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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace Nequeo.Wpf.Controls
{
    /// <summary>
    /// Drop down button.
    /// </summary>
    [TemplatePart(Name = "PART_Popup")]
    [ContentProperty("Content")]
    public class DropDownButton : RibbonSplitButton
    {
        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        public DropDownButton()
            : base()
        {
            AddHandler(ClickableTreeViewItem.ClickEvent, new RoutedEventHandler(OnMenuItemClickedEvent));

        }

        protected override UIElement PlacementTarget
        {
            get
            {
                return DropDownButton;
            }
        }


        /// <summary>
        /// Gets or sets the content in the dropdown button area.
        /// </summary>
        public object DropDownButtonContent
        {
            get { return (object)GetValue(DropDownButtonContentProperty); }
            set { SetValue(DropDownButtonContentProperty, value); }
        }

        public static readonly DependencyProperty DropDownButtonContentProperty =
            DependencyProperty.Register("DropDownButtonContent", typeof(object), typeof(DropDownButton), new UIPropertyMetadata(null));

    }
}
