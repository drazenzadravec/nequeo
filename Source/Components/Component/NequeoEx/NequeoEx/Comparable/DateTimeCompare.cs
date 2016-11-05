/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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

namespace Nequeo.Comparable
{
    /// <summary>
    /// Date time custom comparer.
    /// </summary>
    public class DateTimeCompare : IComparable<DateTimeCompare>, IEquatable<DateTimeCompare>
    {
        /// <summary>
        /// Date time custom comparer.
        /// </summary>
        /// <param name="dateTime">The date time structure.</param>
        public DateTimeCompare(System.DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        private System.DateTime _dateTime;

        /// <summary>
        /// Gets the value of this date time.
        /// </summary>
        public System.DateTime Value
        {
            get { return _dateTime; }
        }

        /// <summary>
        /// Converts a data time to a DateTimeCompare.
        /// </summary>
        /// <param name="value">A short.</param>
        /// <returns>A DateTimeCompare.</returns>
        public static implicit operator DateTimeCompare(System.DateTime value)
        {
            return new DateTimeCompare(value);
        }

        /// <summary>
        /// Converts a DateTimeCompare to a short.
        /// </summary>
        /// <param name="value">A DateTimeCompare.</param>
        /// <returns>A short.</returns>
        public static implicit operator System.DateTime(DateTimeCompare value)
        {
            return Convert.ToDateTime(value.Value);
        }

        /// <summary>
        /// Compares two DateTimeCompare values.
        /// </summary>
        /// <param name="left">The first DateTimeCompare.</param>
        /// <param name="right">The other DateTimeCompare.</param>
        /// <returns>True if the two DateTimeCompare values are not equal according to !=.</returns>
        public static bool operator !=(DateTimeCompare left, DateTimeCompare right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares two DateTimeCompare values.
        /// </summary>
        /// <param name="left">The first DateTimeCompare.</param>
        /// <param name="right">The other DateTimeCompare.</param>
        /// <returns>True if the two DateTimeCompare values are equal according to ==.</returns>
        public static bool operator ==(DateTimeCompare left, DateTimeCompare right)
        {
            if (object.ReferenceEquals(left, null)) { return object.ReferenceEquals(right, null); }
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether a specified DateTimeCompare value is
        /// less than another specified DateTimeCompare value.
        /// </summary>
        /// <param name="left">The first DateTimeCompare.</param>
        /// <param name="right">The other DateTimeCompare.</param>
        /// <returns>True if left is less than right; otherwise, false.</returns>
        public static bool operator <(DateTimeCompare left, DateTimeCompare right)
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
                    long leftValue = left.Value.Ticks;
                    long rightValue = right.Value.Ticks;

                    // Return if left is less than right.
                    return leftValue < rightValue;
                }
            }
        }

        /// <summary>
        /// Indicates whether a specified DateTimeCompare value is
        /// greater than another specified DateTimeCompare value.
        /// </summary>
        /// <param name="left">The first DateTimeCompare.</param>
        /// <param name="right">The other DateTimeCompare.</param>
        /// <returns>True if left is greater than right; otherwise, false.</returns>
        public static bool operator >(DateTimeCompare left, DateTimeCompare right)
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
                    long leftValue = left.Value.Ticks;
                    long rightValue = right.Value.Ticks;

                    // Return if left is less than right.
                    return leftValue > rightValue;
                }
            }
        }

        /// <summary>
        /// Indicates whether a specified DateTimeCompare value is
        /// less than or equal to another specified DateTimeCompare value.
        /// </summary>
        /// <param name="left">The first DateTimeCompare.</param>
        /// <param name="right">The other DateTimeCompare.</param>
        /// <returns>True if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(DateTimeCompare left, DateTimeCompare right)
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
                    long leftValue = left.Value.Ticks;
                    long rightValue = right.Value.Ticks;

                    // Return if left is less than right.
                    return leftValue <= rightValue;
                }
            }
        }

        /// <summary>
        /// Indicates whether a specified DateTimeCompare value is
        /// greater than or equal to another specified DateTimeCompare value.
        /// </summary>
        /// <param name="left">The first DateTimeCompare.</param>
        /// <param name="right">The other DateTimeCompare.</param>
        /// <returns>True if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(DateTimeCompare left, DateTimeCompare right)
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
                    long leftValue = left.Value.Ticks;
                    long rightValue = right.Value.Ticks;

                    // Return if left is less than right.
                    return leftValue >= rightValue;
                }
            }
        }

        /// <summary>
        /// Compares this DateTimeCompare to another DateTimeCompare.
        /// </summary>
        /// <param name="other">The other DateTimeCompare.</param>
        /// <returns>A 32-bit signed integer that indicates whether this 
        /// DateTimeCompare is less than, equal to, or greather than the other.</returns>
        public int CompareTo(DateTimeCompare other)
        {
            return ((System.DateTime)_dateTime).CompareTo(other.Value);
        }

        /// <summary>
        /// Compares this DateTimeCompare to another DateTimeCompare.
        /// </summary>
        /// <param name="other">The other DateTimeCompare.</param>
        /// <returns>True if the two DateTimeCompare values are equal.</returns>
        public bool Equals(DateTimeCompare other)
        {
            if (object.ReferenceEquals(other, null) || GetType() != other.GetType()) { return false; }
            return _dateTime.Equals(other.Value);
        }

        /// <summary>
        /// Compares this Number to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other object is a Number and equal to this one.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as DateTimeCompare);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _dateTime.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>A string representation of the value.</returns>
        public override string ToString()
        {
            return _dateTime.ToString();
        }
    }
}
