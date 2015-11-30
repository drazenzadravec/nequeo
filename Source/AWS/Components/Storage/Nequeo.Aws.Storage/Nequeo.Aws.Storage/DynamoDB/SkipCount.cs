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
using System.Threading;

using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace Nequeo.Aws.Storage.DynamoDB
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
        public ScanCondition GetSkipQuery()
        {
            // Select the skip type.
            switch (typeof(T).Name.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (Boolean)Convert.ChangeType(_skipValue, typeof(Boolean)));

                case "system.byte[]":
                case "byte[]":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (Byte[])Convert.ChangeType(_skipValue, typeof(Byte[])));

                case "system.datetimeoffset":
                case "datetimeoffset":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (DateTimeOffset)Convert.ChangeType(_skipValue, typeof(DateTimeOffset)));

                case "system.datetime":
                case "datetime":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (DateTime)Convert.ChangeType(_skipValue, typeof(DateTime)));

                case "system.double":
                case "double":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (Double)Convert.ChangeType(_skipValue, typeof(Double)));

                case "system.guid":
                case "guid":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (Guid)Convert.ChangeType(_skipValue, typeof(Guid)));

                case "system.int32":
                case "int32":
                case "int":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (Int32)Convert.ChangeType(_skipValue, typeof(Int32)));

                case "system.int64":
                case "int64":
                case "long":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (Int64)Convert.ChangeType(_skipValue, typeof(Int64)));

                case "system.string":
                case "string":
                    return new ScanCondition(
                        _skipPropertyName, ScanOperator.GreaterThanOrEqual,
                        (String)Convert.ChangeType(_skipValue, typeof(String)));

                default:
                    throw new Exception("The skip value type is not supported.");
            }
        }
    }
}
