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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Nequeo.Custom
{
    /// <summary>
    /// Defines a simple schema.
    /// </summary>
    public class StringValue
    {
        /// <summary>
        /// Defines a simple xml schema.
        /// </summary>
        public StringValue()
        {
            _value = "";
            SetXml(_value);
        }

        /// <summary>
        /// Defines a simple xml schema.
        /// </summary>
        /// <param name="value">The value.</param>
        public StringValue(string value)
        {
            _value = value;
            SetXml(_value);
        }

        /// <summary>
        /// Defines a simple xml schema.
        /// </summary>
        /// <param name="source">A new string value xml to place on top of the existing one.</param>
        public StringValue(StringValue source)
        {
            _value = source;
            SetXml(_value);
        }

        private string _xml = null;
        private string _value = null;

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Implicitly convert the string value to a StringValue type.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>The StringValue type.</returns>
        public static implicit operator StringValue(string value)
        {
            return new StringValue(value);
        }

        /// <summary>
        /// Implicitly convert the StringValue type to a string.
        /// </summary>
        /// <param name="xml">The String Value type.</param>
        /// <returns>The string equivalent.</returns>
        public static implicit operator string(StringValue xml)
        {
            return xml.Value;
        }

        /// <summary>
        /// Override the ToString method to convert to a string
        /// </summary>
        /// <returns>The string equivalent.</returns>
        public override string ToString()
        {
            return _xml;
        }

        /// <summary>
        /// Set the current value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value.</returns>
        private string SetXml(string value)
        {
            _xml = value;
            return _xml;
        }
    }
}
