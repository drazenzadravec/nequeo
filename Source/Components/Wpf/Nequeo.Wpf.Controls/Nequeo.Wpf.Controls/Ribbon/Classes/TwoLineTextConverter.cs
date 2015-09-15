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

namespace Nequeo.Wpf.Controls
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TwoLineTextConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Splits a string into two lines which have almost the same length if possible.
        /// </summary>
        /// <param name="value">The string to split. If this type is not a string, the value is returned directly.</param>
        /// <param name="parameter">Specifies the line number to return. The value must be either (int)1 or (int)2.</param>
        /// <returns>The first or second line of the string, otherwise value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int line;
            try
            {
                line = int.Parse(parameter as string);
            }
            catch
            {
                throw new ArgumentException("parameter must be either 1 or 2.");
            }
            string s = value as string;
            if (s == null) return line == 1 ? value : null;

            int l = SplitIn2Lines(s);
            if (l == 0) return line == 1 ? s : null;

            switch (line)
            {
                case 1: return s.Substring(0, l).Trim();
                case 2: return s.Substring(l + 1).Trim();
                default: throw new ArgumentException("parameter must be either 1 or 2.");
            }
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
