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

namespace Nequeo.Serialisation.Data
{
    /// <summary>
    /// Java object notation data helper
    /// </summary>
    public class Json
    {
        /// <summary>
        /// Gets the JSON data type for the value and type.
        /// </summary>
        /// <param name="systemType">The property type.</param>
        /// <param name="value">The value of the type.</param>
        /// <returns>The JSON text value.</returns>
        public static object GetJavaObjectNotationValue(Type systemType, object value)
        {
            switch (systemType.Name.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                case "system.boolean[]":
                case "boolean[]":
                case "bool[]":
                case "system.boolean[,]":
                case "boolean[,]":
                case "bool[,]":
                    Boolean itemBoolean = (Boolean)value;
                    return itemBoolean.ToString().ToLower();

                case "system.byte":
                case "byte":
                case "system.byte[]":
                case "byte[]":
                case "system.byte[,]":
                case "byte[,]":
                    Byte itemByte = (Byte)value;
                    return itemByte;

                case "system.char":
                case "char":
                case "system.char[]":
                case "char[]":
                case "system.char[,]":
                case "char[,]":
                    String itemChar = Convert.ToString((char)value);
                    return "\"" + itemChar + "\"";

                case "system.datetime":
                case "datetime":
                case "system.datetime[]":
                case "datetime[]":
                case "system.datetime[,]":
                case "datetime[,]":
                    DateTime start = new DateTime(1970, 1, 1);
                    DateTime itemDateTime = ((DateTime)value).ToUniversalTime();
                    return "\"" + "\\/Date(" +
                        Convert.ToInt64(itemDateTime.Subtract(start).TotalMilliseconds).ToString() +
                        Nequeo.Invention.TimeZone.GetUtcOffset((DateTime)value) +
                        ")\\/" + "\"";

                case "system.dbnull":
                case "dbnull":
                case "system.dbnull[]":
                case "dbnull[]":
                case "system.dbnull[,]":
                case "dbnull[,]":
                    String dbnullString = System.DBNull.Value.ToString();
                    return "null";

                case "system.timespan":
                case "timespan":
                case "system.timespan[]":
                case "timespan[]":
                case "system.timespan[,]":
                case "timespan[,]":
                    TimeSpan itemTimeSpan = (TimeSpan)value;
                    return "\"" + Nequeo.Invention.Converter.TimeSpanToDurationFormat(itemTimeSpan) + "\"";

                case "system.decimal":
                case "decimal":
                case "system.decimal[]":
                case "decimal[]":
                case "system.decimal[,]":
                case "decimal[,]":
                    Decimal itemDecimal = (Decimal)value;
                    return itemDecimal;

                case "system.double":
                case "double":
                case "system.double[]":
                case "double[]":
                case "system.double[,]":
                case "double[,]":
                    Double itemDouble = (Double)value;
                    return itemDouble;

                case "system.int16":
                case "int16":
                case "short":
                case "system.int16[]":
                case "int16[]":
                case "short[]":
                case "system.int16[,]":
                case "int16[,]":
                case "short[,]":
                    Int16 itemInt16 = (Int16)value;
                    return itemInt16;

                case "system.int32":
                case "int32":
                case "int":
                case "system.int32[]":
                case "int32[]":
                case "int[]":
                case "system.int32[,]":
                case "int32[,]":
                case "int[,]":
                    Int32 itemInt32 = (Int32)value;
                    return itemInt32;

                case "system.int64":
                case "int64":
                case "long":
                case "system.int64[]":
                case "int64[]":
                case "long[]":
                case "system.int64[,]":
                case "int64[,]":
                case "long[,]":
                    Int64 itemInt64 = (Int64)value;
                    return itemInt64;

                case "system.sbyte":
                case "sbyte":
                case "system.sbyte[]":
                case "sbyte[]":
                case "system.sbyte[,]":
                case "sbyte[,]":
                    SByte itemSByte = (SByte)value;
                    return itemSByte;

                case "system.single":
                case "single":
                case "float":
                case "system.single[]":
                case "single[]":
                case "float[]":
                case "system.single[,]":
                case "single[,]":
                case "float[,]":
                    Single itemSingle = (Single)value;
                    return itemSingle;

                case "system.string":
                case "string":
                case "system.string[]":
                case "string[]":
                case "system.string[,]":
                case "string[,]":
                    String itemString = (String)value;
                    return "\"" + itemString + "\"";

                case "system.uint16":
                case "uint16":
                case "system.uint16[]":
                case "uint16[]":
                case "system.uint16[,]":
                case "uint16[,]":
                    UInt16 itemUInt16 = (UInt16)value;
                    return itemUInt16;

                case "system.uint32":
                case "uint32":
                case "uint":
                case "system.uint32[]":
                case "uint32[]":
                case "uint[]":
                case "system.uint32[,]":
                case "uint32[,]":
                case "uint[,]":
                    UInt32 itemUInt32 = (UInt32)value;
                    return itemUInt32;

                case "system.uint64":
                case "uint64":
                case "ulong":
                case "system.uint64[]":
                case "uint64[]":
                case "ulong[]":
                case "system.uint64[,]":
                case "uint64[,]":
                case "ulong[,]":
                    UInt64 itemUInt64 = (UInt64)value;
                    return itemUInt64;

                case "system.guid":
                case "guid":
                case "system.guid[]":
                case "guid[]":
                case "system.guid[,]":
                case "guid[,]":
                    String guidString = new Guid(value.ToString()).ToString();
                    return "\"" + guidString + "\"";

                case "system.nullable`1":
                case "nullable`1":
                    Type[] genericArguments = systemType.GetGenericArguments();
                    return GetJavaObjectNotationValue(genericArguments[0], value);

                default:
                    return null;
            }
        }
    }
}
