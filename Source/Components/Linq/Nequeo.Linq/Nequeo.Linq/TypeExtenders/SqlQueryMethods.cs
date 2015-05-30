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

namespace Nequeo.Data.TypeExtenders
{
    /// <summary>
    /// Sql query expression tree methods.
    /// </summary>
    public sealed partial class SqlQueryMethods
    {
        #region Sql Query Methods
        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(object expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(object expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(object expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(object expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64 expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64? expression, string pattern1, string pattern2)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(object expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(object expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64 expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64? expression, string pattern1, string pattern2, string pattern3)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(object expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(object expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64 expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64? expression, string pattern1, string pattern2, string pattern3, string pattern4)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(object expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Boolean? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(DateTime? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int16? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int32? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(Int64? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt16? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt32? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool In(UInt64? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(object expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Boolean? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(DateTime? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int16? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int32? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(Int64? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64 expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt16? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt32? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Not In' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern1">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern2">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern3">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern4">The search pattern. Must be a full sql pattern.</param>
        /// <param name="pattern5">The search pattern. Must be a full sql pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool NotIn(UInt64? expression, string pattern1, string pattern2, string pattern3, string pattern4, string pattern5)
        {
            return (!String.IsNullOrEmpty(pattern1));
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(object expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(Boolean expression)
        {
            return (expression);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(Boolean? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(DateTime expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(DateTime? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(Int16? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(Int32? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(Int64? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(UInt16? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(UInt32? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNull(UInt64? expression)
        {
            return (expression == null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(object expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(Boolean? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(DateTime expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(DateTime? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(Int16? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(Int32? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(Int64? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(UInt16? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(UInt32? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Is Not Null' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool IsNotNull(UInt64? expression)
        {
            return (expression != null);
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(object expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Boolean expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Boolean? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(DateTime expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(DateTime? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Int16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Int32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Int64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Int16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Int32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(Int64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(UInt16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(UInt32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(UInt64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(UInt16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(UInt32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Contains' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if the expression is not null.</returns>
        public static bool Contains(UInt64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(object expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Boolean expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Boolean? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(DateTime expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(DateTime? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Int64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Int32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Int16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(UInt64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(UInt32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(UInt16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Int64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Int32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(Int16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(UInt64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(UInt32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool Like(UInt16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(object expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Boolean expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Boolean? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(DateTime expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(DateTime? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Int16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Int32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Int64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Int16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Int32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(Int64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(UInt16 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(UInt32 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(UInt64 expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(UInt16? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(UInt32? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLike(UInt64? expression, string pattern)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(object expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Boolean expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Boolean? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(DateTime expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(DateTime? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Int16 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Int32 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Int64 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Int16? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Int32? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(Int64? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(UInt16 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(UInt32 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(UInt64 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(UInt16? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(UInt32? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool LikeEscape(UInt64? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(object expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Boolean expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Boolean? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(DateTime expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(DateTime? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Int16 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Int32 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Int64 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(UInt16 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(UInt32 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(UInt64 expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Int16? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Int32? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(Int64? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(UInt16? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(UInt32? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Not Like with Escape clause' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <param name="escapeValue">The escape value used in the like search.</param>
        /// <returns>True if pattern is correct else false.</returns>
        public static bool NotLikeEscape(UInt64? expression, string pattern, string escapeValue)
        {
            return (!String.IsNullOrEmpty(pattern));
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(string expression, string from, string to)
        {
            return (from.Equals(to));
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(DateTime expression, DateTime from, DateTime to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(Int16 expression, Int16 from, Int16 to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(Int32 expression, Int32 from, Int32 to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(Int64 expression, Int64 from, Int64 to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(DateTime? expression, DateTime? from, DateTime? to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(Int16? expression, Int16? from, Int16? to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(Int32? expression, Int32? from, Int32? to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool Between(Int64? expression, Int64? from, Int64? to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(string expression, string from, string to)
        {
            return (from.Equals(to));
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(DateTime expression, DateTime from, DateTime to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(Int16 expression, Int16 from, Int16 to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(Int32 expression, Int32 from, Int32 to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(Int64 expression, Int64 from, Int64 to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(DateTime? expression, DateTime? from, DateTime? to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(Int16? expression, Int16? from, Int16? to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(Int32? expression, Int32? from, Int32? to)
        {
            return (from <= to);
        }

        /// <summary>
        /// Sql 'Not Between' query method.
        /// </summary>
        /// <param name="expression">The expression tree member.</param>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The ending value.</param>
        /// <returns>True if 'from is less than or equal to 'to'.</returns>
        public static bool NotBetween(Int64? expression, Int64? from, Int64? to)
        {
            return (from <= to);
        }
        #endregion
    }
}
