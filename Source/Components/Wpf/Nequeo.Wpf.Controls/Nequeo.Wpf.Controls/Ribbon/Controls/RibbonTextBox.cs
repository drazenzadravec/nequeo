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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Nequeo.Wpf.Controls.Ribbon.Interfaces;
using System.Diagnostics;
using System.Windows.Input;
using Nequeo.Wpf.Controls.Interfaces;

namespace Nequeo.Wpf.Controls
{
    public class RibbonTextBox : TextBox, IRibbonControl, IRibbonStretch,IKeyTipControl
    {

        static RibbonTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTextBox), new FrameworkPropertyMetadata(typeof(RibbonTextBox)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(RibbonTextBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(RibbonTextBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(RibbonTextBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

        }



        /// <summary>
        /// Gets or sets the image of the textbox.
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonTextBox), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or sets the title of the textbox that appears with Appearance = Medium.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RibbonTextBox), new UIPropertyMetadata(""));




        /// <summary>
        /// Gets or sets the width for the label.
        /// </summary>
        public double LabelWidth
        {
            get { return (double)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
        }

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(double), typeof(RibbonTextBox), new UIPropertyMetadata(double.NaN));



        /// <summary>
        /// Gets or sets the with for the textbox.
        /// This is a dependency property.
        /// </summary>
        public double ContentWidth
        {
            get { return (double)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(double), typeof(RibbonTextBox), new UIPropertyMetadata(double.NaN));


        #region IKeyboardCommand Members

        public void ExecuteKeyTip()
        {
            Focus();
            SelectAll();
        }

        #endregion
    }
}
