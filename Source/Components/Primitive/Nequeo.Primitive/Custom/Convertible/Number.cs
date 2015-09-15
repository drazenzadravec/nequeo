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
    /// Represents a number value.
    /// </summary>
    [Serializable]
    public class Number : IConvertible, IComparable<Number>, IEquatable<Number>
    {
        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(short value)
        {
            _value = value;
            _typeCode = TypeCode.Int16;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(int value)
        {
            _value = value;
            _typeCode = TypeCode.Int32;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(long value)
        {
            _value = value;
            _typeCode = TypeCode.Int64;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(decimal value)
        {
            _value = value;
            _typeCode = TypeCode.Decimal;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(double value)
        {
            _value = value;
            _typeCode = TypeCode.Double;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(float value)
        {
            _value = value;
            _typeCode = TypeCode.Single;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(ushort value)
        {
            _value = value;
            _typeCode = TypeCode.UInt16;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(uint value)
        {
            _value = value;
            _typeCode = TypeCode.UInt32;
        }

        /// <summary>
        /// Initializes a new instance of the Number class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(ulong value)
        {
            _value = value;
            _typeCode = TypeCode.UInt64;
        }

        private TypeCode _typeCode = TypeCode.Empty;
        private object _value;

        /// <summary>
        /// Gets the value of this Number.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the number type.
        /// </summary>
        public Type NumberType
        {
            get
            {
                switch (_typeCode)
                {
                    case TypeCode.Int16:
                        return typeof(short);
                    case TypeCode.Int32:
                        return typeof(int);
                    case TypeCode.Int64:
                        return typeof(long);
                    case TypeCode.Decimal:
                        return typeof(decimal);
                    case TypeCode.Double:
                        return typeof(double);
                    case TypeCode.Single:
                        return typeof(float);
                    case TypeCode.UInt16:
                        return typeof(ushort);
                    case TypeCode.UInt32:
                        return typeof(uint);
                    case TypeCode.UInt64:
                        return typeof(ulong);
                    default:
                        return typeof(double);
                }
            }
        }

        /// <summary>
        /// Converts a short to a Number.
        /// </summary>
        /// <param name="value">A short.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(short value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a short.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A short.</returns>
        public static implicit operator short(Number value)
        {
            return Convert.ToInt16(value.Value);
        }

        /// <summary>
        /// Converts a int to a Number.
        /// </summary>
        /// <param name="value">A int.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(int value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a int.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A int.</returns>
        public static implicit operator int(Number value)
        {
            return Convert.ToInt32(value.Value);
        }

        /// <summary>
        /// Converts a long to a Number.
        /// </summary>
        /// <param name="value">A long.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(long value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a long.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A long.</returns>
        public static implicit operator long(Number value)
        {
            return Convert.ToInt64(value.Value);
        }

        /// <summary>
        /// Converts a decimal to a Number.
        /// </summary>
        /// <param name="value">A decimal.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(decimal value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a decimal.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A decimal.</returns>
        public static implicit operator decimal(Number value)
        {
            return Convert.ToDecimal(value.Value);
        }

        /// <summary>
        /// Converts a double to a Number.
        /// </summary>
        /// <param name="value">A double.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(double value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a double.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A double.</returns>
        public static implicit operator double(Number value)
        {
            return Convert.ToDouble(value.Value);
        }

        /// <summary>
        /// Converts a float to a Number.
        /// </summary>
        /// <param name="value">A float.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(float value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a float.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A float.</returns>
        public static implicit operator float(Number value)
        {
            return Convert.ToSingle(value.Value);
        }

        /// <summary>
        /// Converts a ushort to a Number.
        /// </summary>
        /// <param name="value">A ushort.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(ushort value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a ushort.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A ushort.</returns>
        public static implicit operator ushort(Number value)
        {
            return Convert.ToUInt16(value.Value);
        }

        /// <summary>
        /// Converts a uint to a Number.
        /// </summary>
        /// <param name="value">A uint.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(uint value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a uint.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A uint.</returns>
        public static implicit operator uint(Number value)
        {
            return Convert.ToUInt32(value.Value);
        }

        /// <summary>
        /// Converts a ulong to a Number.
        /// </summary>
        /// <param name="value">A ulong.</param>
        /// <returns>A Number.</returns>
        public static implicit operator Number(ulong value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Converts a Number to a ulong.
        /// </summary>
        /// <param name="value">A Number.</param>
        /// <returns>A ulong.</returns>
        public static implicit operator ulong(Number value)
        {
            return Convert.ToUInt64(value.Value);
        }

        /// <summary>
        /// Compares two Number values.
        /// </summary>
        /// <param name="left">The first Number.</param>
        /// <param name="right">The other Number.</param>
        /// <returns>True if the two Number values are not equal according to !=.</returns>
        public static bool operator !=(Number left, Number right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares two Number values.
        /// </summary>
        /// <param name="left">The first Number.</param>
        /// <param name="right">The other Number.</param>
        /// <returns>True if the two Number values are equal according to ==.</returns>
        public static bool operator ==(Number left, Number right)
        {
            if (object.ReferenceEquals(left, null)) { return object.ReferenceEquals(right, null); }
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether a specified Number value is
        /// less than another specified Number value.
        /// </summary>
        /// <param name="left">The first Number.</param>
        /// <param name="right">The other Number.</param>
        /// <returns>True if left is less than right; otherwise, false.</returns>
        public static bool operator <(Number left, Number right)
        {
            // If left is null.
            if (object.ReferenceEquals(left, null))
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return false;
                else
                    return true;
            }
            else
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return false;
                else
                {
                    // Convert to double.
                    double leftValue = Convert.ToDouble(left.Value);
                    double rightValue = Convert.ToDouble(right.Value);

                    // Return if left is less than right.
                    return leftValue < rightValue;
                }
            }
        }

        /// <summary>
        /// Indicates whether a specified Number value is
        /// greater than another specified Number value.
        /// </summary>
        /// <param name="left">The first Number.</param>
        /// <param name="right">The other Number.</param>
        /// <returns>True if left is greater than right; otherwise, false.</returns>
        public static bool operator >(Number left, Number right)
        {
            // If left is null.
            if (object.ReferenceEquals(left, null))
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return false;
                else
                    return false;
            }
            else
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return true;
                else
                {
                    // Convert to double.
                    double leftValue = Convert.ToDouble(left.Value);
                    double rightValue = Convert.ToDouble(right.Value);

                    // Return if left is less than right.
                    return leftValue > rightValue;
                }
            }
        }

        /// <summary>
        /// Indicates whether a specified Number value is
        /// less than or equal to another specified Number value.
        /// </summary>
        /// <param name="left">The first Number.</param>
        /// <param name="right">The other Number.</param>
        /// <returns>True if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(Number left, Number right)
        {
            // If left is null.
            if (object.ReferenceEquals(left, null))
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return true;
                else
                    return true;
            }
            else
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return false;
                else
                {
                    // Convert to double.
                    double leftValue = Convert.ToDouble(left.Value);
                    double rightValue = Convert.ToDouble(right.Value);

                    // Return if left is less than right.
                    return leftValue <= rightValue;
                }
            }
        }

        /// <summary>
        /// Indicates whether a specified Number value is
        /// greater than or equal to another specified Number value.
        /// </summary>
        /// <param name="left">The first Number.</param>
        /// <param name="right">The other Number.</param>
        /// <returns>True if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(Number left, Number right)
        {
            // If left is null.
            if (object.ReferenceEquals(left, null))
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return true;
                else
                    return false;
            }
            else
            {
                // If right is null.
                if (object.ReferenceEquals(right, null))
                    return true;
                else
                {
                    // Convert to double.
                    double leftValue = Convert.ToDouble(left.Value);
                    double rightValue = Convert.ToDouble(right.Value);

                    // Return if left is less than right.
                    return leftValue >= rightValue;
                }
            }
        }

        /// <summary>
        /// Compares this Number to another Number.
        /// </summary>
        /// <param name="other">The other Number.</param>
        /// <returns>A 32-bit signed integer that indicates whether this 
        /// Number is less than, equal to, or greather than the other.</returns>
        public int CompareTo(Number other)
        {
            if (other == null) { return 1; }
            switch (_typeCode)
            {
                case TypeCode.Int16:
                    return ((short)_value).CompareTo(other.Value);
                case TypeCode.Int32:
                    return ((int)_value).CompareTo(other.Value);
                case TypeCode.Int64:
                    return ((long)_value).CompareTo(other.Value);
                case TypeCode.Decimal:
                    return ((decimal)_value).CompareTo(other.Value);
                case TypeCode.Double:
                    return ((double)_value).CompareTo(other.Value);
                case TypeCode.Single:
                    return ((float)_value).CompareTo(other.Value);
                case TypeCode.UInt16:
                    return ((ushort)_value).CompareTo(other.Value);
                case TypeCode.UInt32:
                    return ((uint)_value).CompareTo(other.Value);
                case TypeCode.UInt64:
                    return ((ulong)_value).CompareTo(other.Value);
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Compares this Number to another Number.
        /// </summary>
        /// <param name="other">The other Number.</param>
        /// <returns>True if the two Number values are equal.</returns>
        public bool Equals(Number other)
        {
            if (object.ReferenceEquals(other, null) || GetType() != other.GetType()) { return false; }
            return _value.Equals(other.Value); 
        }

        /// <summary>
        /// Compares this Number to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other object is a Number and equal to this one.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Number);
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
            return _value.ToString();
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
                return Convert.ToString(_value);
            else
                return Convert.ToString(_value, provider);
        }

        /// <summary>
        /// System.TypeCode for this instance.
        /// </summary>
        /// <returns>The enumerated constant that is the System.TypeCode of 
        /// the class or value type that implements this interface.</returns>
        public TypeCode GetTypeCode()
        {
            return _typeCode;
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
                return Convert.ToDecimal(_value);
            else
                return Convert.ToDecimal(_value, provider);
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
            if (provider == null)
                return Convert.ToDouble(_value);
            else
                return Convert.ToDouble(_value, provider);
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
                return Convert.ToInt16(_value);
            else
                return Convert.ToInt16(_value, provider);
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
                return Convert.ToInt32(_value);
            else
                return Convert.ToInt32(_value, provider);
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
                return Convert.ToInt64(_value);
            else
                return Convert.ToInt64(_value, provider);
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
                return Convert.ToSingle(_value);
            else
                return Convert.ToSingle(_value, provider);
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
                return Convert.ChangeType(_value, conversionType);
            else
                return Convert.ChangeType(_value, conversionType, provider);
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
                return Convert.ToUInt16(_value);
            else
                return Convert.ToUInt16(_value, provider);
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
                return Convert.ToUInt32(_value);
            else
                return Convert.ToUInt32(_value, provider);
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
                return Convert.ToUInt64(_value);
            else
                return Convert.ToUInt64(_value, provider);
        }

        #region Explicit IConvertible Not Used

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

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

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
