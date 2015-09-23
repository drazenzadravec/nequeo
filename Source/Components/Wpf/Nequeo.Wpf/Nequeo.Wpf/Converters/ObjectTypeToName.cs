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
    /// Object type to name value converter.
    /// </summary>
    public class ObjectTypeToName : IValueConverter
    {
        /// <summary>
        /// Convert the object type to the name of the type as a string.
        /// </summary>
        /// <param name="value">The object type value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The type parameter.</param>
        /// <param name="culture">The current culture.</param>
        /// <returns>The object type name.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value exists.
            if (value != null)
            {
                // Get the type name.
                string valueString = value.ToString();
                if (string.IsNullOrEmpty(valueString) || (valueString == value.GetType().UnderlyingSystemType.ToString()))
                {
                    // Return the name of the type.
                    return value.GetType().Name;
                }
                return value;
            }
            return null;
        }

        /// <summary>
        /// Convert the type name to the type.
        /// </summary>
        /// <param name="value">The type name of the string.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The type parameter.</param>
        /// <param name="culture">The current culture.</param>
        /// <returns>The type from the name; else null.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value exists.
            if (value != null)
            {
                // Get the type name.
                string valueString = value as string;
                return Type.GetType(valueString);
            }
            return null;
        }
    }
}
