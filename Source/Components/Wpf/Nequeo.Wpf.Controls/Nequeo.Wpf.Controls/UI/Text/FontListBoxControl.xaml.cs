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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nequeo.Wpf.UI.Text
{
    /// <summary>
    /// Font list box control.
    /// </summary>
    public partial class FontListBoxControl : UserControl
    {
        /// <summary>
        /// Font list box control.
        /// </summary>
        public FontListBoxControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the font family text name.
        /// </summary>
        public string FontFamilyTextName
        {
            get { return txtFontName.Text; }
        }

        /// <summary>
        /// Gets or sets the font family name.
        /// </summary>
        public string FontFamilyName
        {
            get { return (string)GetValue(FontFamilyNameProperty); }
            set { SetValue(FontFamilyNameProperty, value); }
        }

        /// <summary>
        /// Identifies the FontFamilyName dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyNameProperty =
            DependencyProperty.Register(
                "FontFamilyName",
                typeof(string),
                typeof(FontListBoxControl),
                new FrameworkPropertyMetadata("",
                    new PropertyChangedCallback(OnFontFamilyNameChanged)));

        /// <summary>
        /// Identifies the FontFamilyNameChanged routed event.
        /// </summary>
        public static readonly RoutedEvent FontFamilyNameChangedEvent =
            EventManager.RegisterRoutedEvent(
            "FontFamilyNameChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<string>),
            typeof(FontListBoxControl));

        /// <summary>
        /// Occurs when the FontFamilyName property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> FontFamilyNameChanged
        {
            add { AddHandler(FontFamilyNameChangedEvent, value); }
            remove { RemoveHandler(FontFamilyNameChangedEvent, value); }
        }

        /// <summary>
        /// On value change event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">Dependency property event arguments.</param>
        private static void OnFontFamilyNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            FontListBoxControl control = (FontListBoxControl)obj;
            control.UpdateTextBlock();

            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(
                (string)args.OldValue, (string)args.NewValue, FontFamilyNameChangedEvent);
            control.OnFontFamilyNameChanged(e);
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnFontFamilyNameChanged(RoutedPropertyChangedEventArgs<string> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Update the text in the blocktext control.
        /// </summary>
        private void UpdateTextBlock()
        {
            // Do something when it changes.
        }

        /// <summary>
        /// Font selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamilyName = FontCombo.SelectedValue.ToString();
        }
    }
}
