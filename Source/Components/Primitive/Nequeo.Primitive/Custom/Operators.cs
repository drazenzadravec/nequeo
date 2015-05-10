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
using System.Text;

namespace Nequeo.Custom
{
    /// <summary>
    /// Database boolean nullable type.
    /// </summary>
    public struct DatabaseBoolean
    {
        /// <summary>
        /// DbNull
        /// </summary>
        public static readonly DatabaseBoolean dbNull = new DatabaseBoolean(0);
        /// <summary>
        /// DbFalse
        /// </summary>
        public static readonly DatabaseBoolean dbFalse = new DatabaseBoolean(-1);
        /// <summary>
        /// DbTrue
        /// </summary>
        public static readonly DatabaseBoolean dbTrue = new DatabaseBoolean(1);

        // Private field that stores -1, 0, 1 for dbFalse, dbNull, dbTrue:
        private int value;

        // Private constructor. The value parameter must be -1, 0, or 1:
        DatabaseBoolean(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Implicit conversion from bool to DBBool. Maps true to 
        /// DBBool.dbTrue and false to DBBool.dbFalse:
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator DatabaseBoolean(bool x)
        {
            return x ? dbTrue : dbFalse;
        }

        /// <summary>
        /// Explicit conversion from DBBool to bool. Throws an 
        /// exception if the given DBBool is dbNull, otherwise returns
        /// true or false:
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static explicit operator bool(DatabaseBoolean x)
        {
            if (x.value == 0) throw new InvalidOperationException();
            return x.value > 0;
        }

        /// <summary>
        /// Equality operator. Returns dbNull if either operand is dbNull, 
        /// otherwise returns dbTrue or dbFalse:
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static DatabaseBoolean operator ==(DatabaseBoolean x, DatabaseBoolean y)
        {
            if (x.value == 0 || y.value == 0) return dbNull;
            return x.value == y.value ? dbTrue : dbFalse;
        }

        /// <summary>
        /// Inequality operator. Returns dbNull if either operand is
        /// dbNull, otherwise returns dbTrue or dbFalse:
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static DatabaseBoolean operator !=(DatabaseBoolean x, DatabaseBoolean y)
        {
            if (x.value == 0 || y.value == 0) return dbNull;
            return x.value != y.value ? dbTrue : dbFalse;
        }

        /// <summary>
        /// Logical negation operator. Returns dbTrue if the operand is 
        /// dbFalse, dbNull if the operand is dbNull, or dbFalse if the
        /// operand is dbTrue:
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static DatabaseBoolean operator !(DatabaseBoolean x)
        {
            return new DatabaseBoolean(-x.value);
        }

        /// <summary>
        /// Logical AND operator. Returns dbFalse if either operand is 
        /// dbFalse, dbNull if either operand is dbNull, otherwise dbTrue:
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static DatabaseBoolean operator &(DatabaseBoolean x, DatabaseBoolean y)
        {
            return new DatabaseBoolean(x.value < y.value ? x.value : y.value);
        }

        /// <summary>
        /// Logical OR operator. Returns dbTrue if either operand is 
        /// dbTrue, dbNull if either operand is dbNull, otherwise dbFalse:
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static DatabaseBoolean operator |(DatabaseBoolean x, DatabaseBoolean y)
        {
            return new DatabaseBoolean(x.value > y.value ? x.value : y.value);
        }

        /// <summary>
        /// Definitely true operator. Returns true if the operand is 
        /// dbTrue, false otherwise:
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool operator true(DatabaseBoolean x)
        {
            return x.value > 0;
        }

        /// <summary>
        /// Definitely false operator. Returns true if the operand is 
        /// dbFalse, false otherwise:
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool operator false(DatabaseBoolean x)
        {
            return x.value < 0;
        }

        /// <summary>
        /// Overload the conversion from DBBool to string:
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static implicit operator string(DatabaseBoolean x)
        {
            return x.value > 0 ? "dbTrue"
                 : x.value < 0 ? "dbFalse"
                 : "dbNull";
        }

        /// <summary>
        /// Override the Object.Equals(object o) method:
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (DatabaseBoolean)o);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Override the Object.GetHashCode() method:
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return value;
        }

        /// <summary>
        /// Override the ToString method to convert DBBool to a string:
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (value)
            {
                case -1:
                    return "DBBool.False";
                case 0:
                    return "DBBool.Null";
                case 1:
                    return "DBBool.True";
                default:
                    return "";
            }
        }
    }
}
