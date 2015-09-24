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
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections;
using System.Globalization;

namespace Nequeo.Wpf.UI.Custom
{
    /// <summary>
    /// Define a item for chosing text color.
    /// </summary>
    public class ColorComboBoxItem : System.Windows.Controls.ListBoxItem, IFontChooserComboBoxItem
    {
        private SolidColorBrush _brush;
        private string _colorName;

        /// <summary>
        /// Define a item for chosing text color.
        /// </summary>
        /// <param name="colourName">The colour name.</param>
        /// <param name="brush">The brush colour.</param>
        public ColorComboBoxItem(string colourName, SolidColorBrush brush)
        {
            _brush = brush;

            StackPanel panel = new StackPanel();
            panel.Height = double.NaN; // _height;
            panel.Orientation = Orientation.Horizontal;

            Rectangle colorSwatch = new Rectangle();
            colorSwatch.Height = double.NaN; // _height;
            colorSwatch.Width = 25;
            colorSwatch.Fill = brush;
            colorSwatch.StrokeThickness = 1;
            colorSwatch.Stroke = Brushes.DarkGray;
            colorSwatch.Margin = new Thickness(1, 1, 1, 1);

            TextBlock colorName = new TextBlock();
            colorName.Text = "  " + colourName.Trim();
            colorName.Height = double.NaN;       // auto height
            colorName.Width = double.NaN;       // auto width
            colorName.Margin = new Thickness(1, 1, 1, 1);

            panel.Children.Add(colorSwatch);
            panel.Children.Add(colorName);

            Content = panel;
            _colorName = colourName;
        }

        /// <summary>
        /// return the name of a color
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _colorName;
        }

        /// <summary>
        /// Return the color as a solid colorBrush
        /// </summary>
        public SolidColorBrush Brush
        {
            get
            {
                return _brush;
            }
        }

        /// <summary>
        /// Compare with string.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>The compare value.</returns>
        public int CompareWithString(string value)
        {
            return String.Compare(_colorName, value, true, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// RestrictToListedValues.
        /// </summary>
        /// <returns>True.</returns>
        public bool RestrictToListedValues()
        {
            return true;     // Only available fonts may be selected
        }
    }

    /// <summary>
    /// IFontChooserComboBoxItem provides methods to control the relationship between the
    /// textbox and listbox components of a font chooser combobox.
    /// </summary>
    public interface IFontChooserComboBoxItem
    {
        int CompareWithString(string value);
    }
}

