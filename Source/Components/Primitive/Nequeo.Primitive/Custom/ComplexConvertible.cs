/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Globalization;

using Nequeo.Custom.Convertible;

namespace Nequeo.Custom
{
    /// <summary>
    /// Complex value.
    /// </summary>
    public class Complex : IConvertible
    {
        /// <summary>
        /// Complex value.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public Complex(double x, double y)
        {
            _x = x;
            _y = y;
        }

        private double _x;
        private double _y;

        /// <summary>
        /// Calculate the complex value.
        /// </summary>
        /// <returns>The calculated value.</returns>
        private double GetDoubleValue()
        {
            return System.Math.Sqrt((_x * _x) + (_y * _y));
        }

        /// <summary>
        /// System.TypeCode for this instance.
        /// </summary>
        /// <returns>The enumerated constant that is the System.TypeCode of 
        /// the class or value type that implements this interface.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Boolean value using
        /// the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            if ((_x != 0.0) || (_y != 0.0))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit unsigned integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        public byte ToByte(IFormatProvider provider)
        {
            if(provider == null)
                return Convert.ToByte(GetDoubleValue());
            else
                return Convert.ToByte(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Unicode character using
        /// the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        public char ToChar(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToChar(GetDoubleValue());
            else
                return Convert.ToChar(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent System.DateTime using
        /// the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>A System.DateTime instance equivalent to the value of this instance.</returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToDateTime(GetDoubleValue());
            else
                return Convert.ToDateTime(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent System.Decimal number
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>A System.Decimal number equivalent to the value of this instance.</returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToDecimal(GetDoubleValue());
            else
                return Convert.ToDecimal(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent double-precision floating-point
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation that 
        /// supplies culture-specific formatting information.</param>
        /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
        public double ToDouble(IFormatProvider provider)
        {
            return GetDoubleValue();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit signed integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        public short ToInt16(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToInt16(GetDoubleValue());
            else
                return Convert.ToInt16(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit signed integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        public int ToInt32(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToInt32(GetDoubleValue());
            else
                return Convert.ToInt32(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit signed integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        public long ToInt64(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToInt64(GetDoubleValue());
            else
                return Convert.ToInt64(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit signed integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToSByte(GetDoubleValue());
            else
                return Convert.ToSByte(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent single-precision floating-point
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
        public float ToSingle(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToSingle(GetDoubleValue());
            else
                return Convert.ToSingle(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent System.String using 
        /// the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>A System.String instance equivalent to the value of this instance.</returns>
        public string ToString(IFormatProvider provider)
        {
            return String.Format("({0}, {1})", _x, _y);
        }

        /// <summary>
        /// Converts the value of this instance to an System.Object of the specified
        /// System.Type that has an equivalent value, using the specified culture-specific
        /// formatting information.
        /// </summary>
        /// <param name="conversionType">The System.Type to which the value of this instance is converted.</param>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An System.Object instance of type conversionType whose value is 
        /// equivalent to the value of this instance.</returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ChangeType(GetDoubleValue(), conversionType);
            else
                return Convert.ChangeType(GetDoubleValue(), conversionType, provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit unsigned integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToUInt16(GetDoubleValue());
            else
                return Convert.ToUInt16(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit unsigned integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToUInt32(GetDoubleValue());
            else
                return Convert.ToUInt32(GetDoubleValue(), provider);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit unsigned integer
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An System.IFormatProvider interface implementation 
        /// that supplies culture-specific formatting information.</param>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        public ulong ToUInt64(IFormatProvider provider)
        {
            if (provider == null)
                return Convert.ToUInt64(GetDoubleValue());
            else
                return Convert.ToUInt64(GetDoubleValue(), provider);
        }
    }
}
