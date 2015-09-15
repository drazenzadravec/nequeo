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
using System.ComponentModel;

namespace Nequeo.Wpf.Controls.Classes
{
    public class RibbonReductionCollectionConverter:TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            str = str.Trim();
            if (str.Length == 0) return null;

            string[] sizes = str.Split(',');
            RibbonSizeCollection collection = new RibbonSizeCollection();
            foreach (string size in sizes)
            {
                RibbonSize rs;
                switch (size.ToUpper())
                {
                    case "L": rs = RibbonSize.Large; break;
                    case "M": rs = RibbonSize.Medium; break;
                    case "S": rs = RibbonSize.Small; break;
                    case "0": rs = RibbonSize.Minimized; break;


                    default:
                        rs = (RibbonSize)Enum.Parse(typeof(RibbonSize), size, true);
                        break;
                }
                collection.Add(rs);
            }
            return collection;
        }
    }
}
