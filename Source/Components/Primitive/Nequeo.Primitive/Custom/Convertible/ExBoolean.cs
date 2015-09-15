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
using System.Threading.Tasks;
using System.Xml;

namespace Nequeo.Custom.Convertible
{
    /// <summary>
    /// Represents a boolean value.
    /// </summary>
    [Serializable]
    public class ExBoolean : IConvertible, IComparable<ExBoolean>, IEquatable<ExBoolean>
    {
        /// <summary>
        /// Initializes a new instance of the ExBoolean class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ExBoolean(bool value)
        {
            _value = value;
        }

        private bool _value;

        /// <summary>
        /// False string.
        /// </summary>
        public static readonly string FalseString = "false";

        /// <summary>
        /// True string.
        /// </summary>
        public static readonly string TrueString = "true";

        /// <summary>
        /// Gets the value of this ExBoolean.
        /// </summary>
        public bool Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Converts a bool to a ExBoolean.
        /// </summary>
        /// <param name="value">A bool.</param>
        /// <returns>A ExBoolean.</returns>
        public static implicit operator ExBoolean(bool value)
        {
            return new ExBoolean(value);
        }

        /// <summary>
        /// Converts a ExBoolean to a bool.
        /// </summary>
        /// <param name="value">A ExBoolean.</param>
        /// <returns>A bool.</returns>
        public static implicit operator bool(ExBoolean value)
        {
            return value.Value;
        }

        /// <summary>
        /// Compares two ExBoolean values.
        /// </summary>
        /// <param name="left">The first ExBoolean.</param>
        /// <param name="right">The other ExBoolean.</param>
        /// <returns>True if the two BsonBoolean values are not equal according to !=.</returns>
        public static bool operator !=(ExBoolean left, ExBoolean right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares two ExBoolean values.
        /// </summary>
        /// <param name="left">The first ExBoolean.</param>
        /// <param name="right">The other ExBoolean.</param>
        /// <returns>True if the two BsonBoolean values are equal according to ==.</returns>
        public static bool operator ==(ExBoolean left, ExBoolean right)
        {
            if (object.ReferenceEquals(left, null)) { return object.ReferenceEquals(right, null); }
            return left.Equals(right);
        }

        /// <summary>
        /// Compares this ExBoolean to another ExBoolean.
        /// </summary>
        /// <param name="other">The other ExBoolean.</param>
        /// <returns>A 32-bit signed integer that indicates whether this 
        /// ExBoolean is less than, equal to, or greather than the other.</returns>
        public int CompareTo(ExBoolean other)
        {
            if (other == null) { return 1; }
            return (_value ? 1 : 0).CompareTo(other._value ? 1 : 0);
        }

        /// <summary>
        /// Compares this ExBoolean to another BsonBoolean.
        /// </summary>
        /// <param name="other">The other ExBoolean.</param>
        /// <returns>True if the two ExBoolean values are equal.</returns>
        public bool Equals(ExBoolean other)
        {
            if (object.ReferenceEquals(other, null) || GetType() != other.GetType()) { return false; }
            return _value == other._value;
        }

        /// <summary>
        /// Compares this ExBoolean to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other object is a ExBoolean and equal to this one.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ExBoolean); // works even if obj is null or of a different type
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>A string representation of the value.</returns>
        public override string ToString()
        {
            return XmlConvert.ToString(_value);
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
            if (provider == null)
                return Convert.ToString(_value ? ExBoolean.TrueString : ExBoolean.FalseString);
            else
                return Convert.ToString(_value ? ExBoolean.TrueString : ExBoolean.FalseString, provider);
        }

        /// <summary>
        /// System.TypeCode for this instance.
        /// </summary>
        /// <returns>The enumerated constant that is the System.TypeCode of 
        /// the class or value type that implements this interface.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Boolean;
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
            if (provider == null)
                return Convert.ToBoolean(_value);
            else
                return Convert.ToBoolean(_value, provider);
        }

        #region Explicit IConvertible Not Used

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
