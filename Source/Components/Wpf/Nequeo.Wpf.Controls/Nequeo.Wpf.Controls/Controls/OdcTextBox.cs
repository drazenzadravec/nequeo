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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Nequeo.Wpf.Controls.Interfaces;

namespace Nequeo.Wpf.Controls
{

    /// <summary>
    /// A TextBox that contains buttons on the right.
    /// </summary>
    [TemplatePart(Name = partTextBox)]
    public class OdcTextBox : TextBox, IKeyTipControl
    {
        private const string partTextBox = "PART_TextBox";

        static OdcTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OdcTextBox), new FrameworkPropertyMetadata(typeof(OdcTextBox)));
            UIElement.FocusableProperty.OverrideMetadata(typeof(OdcTextBox), new FrameworkPropertyMetadata(true));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(OdcTextBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(OdcTextBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(OdcTextBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
        }

        private ObservableCollection<ButtonBase> buttons = new ObservableCollection<ButtonBase>();

        /// <summary>
        /// Gets the collection of buttons to appear on the right of the OdcTextBox.
        /// </summary>
        public ObservableCollection<ButtonBase> Buttons
        {
            get { return buttons; }
        }


        /// <summary>
        /// Gets or sets the text that appears when the textbox is empty.
        /// This is a dependency property.
        /// </summary>
        public string Info
        {
            get { return (string)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register("Info", typeof(string), typeof(OdcTextBox), new UIPropertyMetadata(""));



        #region IKeyTipControl Members

        void IKeyTipControl.ExecuteKeyTip()
        {
            Focus();
            SelectAll();
        }

        #endregion
    }
}
