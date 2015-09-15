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
using System.Windows.Data;
using System.Windows.Controls;

namespace Nequeo.Wpf.Controls
{
    /// <summary>
    /// Breaks a string into two lines by adding a LineBreak at the appropriate position.
    /// </summary>
    [ValueConversion(typeof(string), typeof(TextBlock))]
    public class TwoLineConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a string into a TextBlock with one or two lines.
        /// </summary>
        /// <param name="value">Either a string to convert, or any value to return directly.</param>
        /// <param name="targetType">ignored.</param>
        /// <param name="parameter">ignored.</param>
        /// <param name="culture">ignored.</param>
        /// <returns>A TextBlock class if value is of string, otherwise value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (s == null) return value;
            {
                int l = SplitIn2Lines(s);
                if (l > 0)
                {
                    s = s.Remove(l,1).Insert(l, "\n");
                }
            }
            TextBlock tb = new TextBlock();
            tb.TextWrapping = System.Windows.TextWrapping.Wrap;
            tb.TextAlignment = System.Windows.TextAlignment.Center;
            tb.LineStackingStrategy = System.Windows.LineStackingStrategy.BlockLineHeight;
            tb.LineHeight = 12.0;
            tb.Text = s;
            return tb;
        }

        private static int SplitIn2Lines(string s)
        {
            int n = s.Length;
            int l = n / 2;
            int r = l + 1;
            while (l > 0)
            {
                char c = s[l];
                if (char.IsSeparator(c)) break;
                if (r < n)
                {
                    c = s[r];
                    if (char.IsSeparator(c))
                    {
                        l = r;
                        break;
                    }
                }
                r++;
                l--;
            }
            return l;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
