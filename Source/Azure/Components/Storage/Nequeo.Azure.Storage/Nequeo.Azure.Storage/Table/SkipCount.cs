/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using Microsoft.WindowsAzure.Storage.Table;

namespace Nequeo.Azure.Storage.Table
{
    /// <summary>
    /// The skip count mode.
    /// </summary>
    public sealed class SkipCount<T>
    {
        private string _skipPropertyName = "RowKey";
        private T _skipValue = default(T);

        /// <summary>
        /// Gets or sets the skip property name.
        /// </summary>
        public string SkipPropertyName
        {
            get { return _skipPropertyName; }
            set { _skipPropertyName = value; }
        }

        /// <summary>
        /// Gets or sets the skip value.
        /// </summary>
        public T SkipValue
        {
            get { return _skipValue; }
            set { _skipValue = value; }
        }

        /// <summary>
        /// Get the skip query.
        /// </summary>
        /// <returns>The skip query string.</returns>
        public string GetSkipQuery()
        {
            // Select the skip type.
            switch(typeof(T).Name.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    return TableQuery.GenerateFilterConditionForBool(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual, 
                        (Boolean)Convert.ChangeType(_skipValue, typeof(Boolean)));

                case "system.byte[]":
                case "byte[]":
                    return TableQuery.GenerateFilterConditionForBinary(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual,
                        (Byte[])Convert.ChangeType(_skipValue, typeof(Byte[])));

                case "system.datetimeoffset":
                case "datetimeoffset":
                    return TableQuery.GenerateFilterConditionForDate(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual,
                        (DateTimeOffset)Convert.ChangeType(_skipValue, typeof(DateTimeOffset)));

                case "system.double":
                case "double":
                    return TableQuery.GenerateFilterConditionForDouble(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual,
                        (Double)Convert.ChangeType(_skipValue, typeof(Double)));

                case "system.guid":
                case "guid":
                    return TableQuery.GenerateFilterConditionForGuid(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual,
                        (Guid)Convert.ChangeType(_skipValue, typeof(Guid)));

                case "system.int32":
                case "int32":
                case "int":
                    return TableQuery.GenerateFilterConditionForInt(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual,
                        (Int32)Convert.ChangeType(_skipValue, typeof(Int32)));

                case "system.int64":
                case "int64":
                case "long":
                    return TableQuery.GenerateFilterConditionForLong(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual,
                        (Int64)Convert.ChangeType(_skipValue, typeof(Int64)));

                case "system.string":
                case "string":
                    return TableQuery.GenerateFilterCondition(
                        _skipPropertyName, QueryComparisons.GreaterThanOrEqual,
                        (String)Convert.ChangeType(_skipValue, typeof(String)));

                default:
                    throw new Exception("The skip value type is not supported.");
            }
        }
    }
}
