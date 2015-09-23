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
using System.Collections;
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Threading;
using System.Globalization;

namespace Nequeo.Wpf.Converters
{
    /// <summary>
    /// Date time text value converter.
    /// </summary>
    public class DateTime : IValueConverter
    {
        /// <summary>
        /// Convert the date time to the specific format.
        /// </summary>
        /// <param name="value">The date time value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The date time format string.</param>
        /// <param name="culture">The current culture.</param>
        /// <returns>The formatted date time string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retrieve the format string and use it to format the value.
            string formatString = parameter as string;

            // If the format exists.
            if (!string.IsNullOrEmpty(formatString))
            {
                // Return the formated date time value.
                return string.Format(culture, formatString, value);
            }

            // If the format string is null or empty, simply call ToString()
            // on the value.
            return value.ToString();
        }

        /// <summary>
        /// Convert the formatted date time to <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="value">The date time formatted value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The date time format string.</param>
        /// <param name="culture">The current culture.</param>
        /// <returns>The date time type converted value; else null.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retrieve the format string and use it to format the value.
            string formatString = parameter as string;

            // Attempt to convert to date time.
            System.DateTime dateTime;
            bool isDateTime = System.DateTime.TryParse(formatString, out dateTime);

            // If is date.
            if (isDateTime)
            {
                // Return the date time.
                return dateTime;
            }
            return null;
        }
    }
}
