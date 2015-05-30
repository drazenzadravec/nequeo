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

namespace Nequeo.Data.DataType
{
    /// <summary>
    /// Data type conversion utilities.
    /// </summary>
    public sealed class DataTypeConversion
    {
        #region DataTypeConversion Class

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionDataType">The connection data type</param>
        public DataTypeConversion(ConnectionContext.ConnectionDataType connectionDataType)
        {
            _connectionDataType = connectionDataType;
        }

        #endregion

        #region Private Fields
        private ConnectionContext.ConnectionDataType _connectionDataType =
            ConnectionContext.ConnectionDataType.None;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        public ConnectionContext.ConnectionDataType ConnectionDataType
        {
            get { return _connectionDataType; }
            set { _connectionDataType = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts the system value type to the eqivalent sql string.
        /// </summary>
        /// <param name="systemType">The value type.</param>
        /// <param name="value">The original value.</param>
        /// <returns>The string sql equivalent.</returns>
        public string GetSqlStringValue(Type systemType, object value)
        {
            switch (systemType.Name.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    Boolean itemBoolean = (Boolean)value;
                    switch (_connectionDataType)
                    {
                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            if (itemBoolean)
                                return "True";
                            else
                                return "False";

                        default:
                            if (itemBoolean)
                                return (1).ToString();
                            else
                                return (0).ToString();
                    }

                case "system.byte":
                case "byte":
                    Byte itemByte = (Byte)value;

                    // Foreach byte found convert the byte
                    // to an octet value string item.
                    string octetArray = itemByte.ToString("X2");

                    // Create the octet string of bytes.
                    string octetRowVersion = "0x" + octetArray;
                    return octetRowVersion;

                case "system.char":
                case "char":
                    String itemChar = Convert.ToString((char)value);
                    return "'" + itemChar.Replace("'", "''") + "'";

                case "system.datetime":
                case "datetime":
                    DateTime itemDateTime = (DateTime)value;
                    switch (_connectionDataType)
                    {
                        case ConnectionContext.ConnectionDataType.OracleDataType:
                            return "TO_DATE('" +
                                itemDateTime.Year + "-" +
                                itemDateTime.Month + "-" +
                                itemDateTime.Day + " " +
                                itemDateTime.Hour + ":" +
                                itemDateTime.Minute + ":" +
                                itemDateTime.Second +
                                "', 'yyyy-mm-dd HH24:mi:ss')";

                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            return "{TS '" +
                                itemDateTime.Year + "-" +
                                itemDateTime.Month + "-" +
                                itemDateTime.Day + " " +
                                itemDateTime.Hour + ":" +
                                itemDateTime.Minute + ":" +
                                itemDateTime.Second +
                                "'}";

                        case ConnectionContext.ConnectionDataType.SqlDataType:
                            return "'" +
                                itemDateTime.Year + "-" +
                                itemDateTime.Month + "-" +
                                itemDateTime.Day + " " +
                                itemDateTime.Hour + ":" +
                                itemDateTime.Minute + ":" +
                                itemDateTime.Second +
                                "'";

                        case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                            return "TO_DATE('" +
                                itemDateTime.Year + "-" +
                                itemDateTime.Month + "-" +
                                itemDateTime.Day + " " +
                                itemDateTime.Hour + ":" +
                                itemDateTime.Minute + ":" +
                                itemDateTime.Second +
                                "', 'yyyy-mm-dd HH24:mi:ss')";

                        case ConnectionContext.ConnectionDataType.MySqlDataType:
                            return "TO_DATE('" +
                                itemDateTime.Year + "-" +
                                itemDateTime.Month + "-" +
                                itemDateTime.Day + " " +
                                itemDateTime.Hour + ":" +
                                itemDateTime.Minute + ":" +
                                itemDateTime.Second +
                                "', 'yyyy-mm-dd HH24:mi:ss')";

                        case ConnectionContext.ConnectionDataType.SqliteDataType:
                            return "TO_DATE('" +
                                itemDateTime.Year + "-" +
                                itemDateTime.Month + "-" +
                                itemDateTime.Day + " " +
                                itemDateTime.Hour + ":" +
                                itemDateTime.Minute + ":" +
                                itemDateTime.Second +
                                "', 'yyyy-mm-dd HH24:mi:ss')";

                        default:
                            return "'" +
                                itemDateTime.Year + "-" +
                                itemDateTime.Month + "-" +
                                itemDateTime.Day + " " +
                                itemDateTime.Hour + ":" +
                                itemDateTime.Minute + ":" +
                                itemDateTime.Second +
                                "'";
                    }

                case "system.timespan":
                case "timespan":
                    TimeSpan itemTimeSpan = (TimeSpan)value;
                    return "'" + itemTimeSpan.ToString() + "'";

                case "system.dbnull":
                case "dbnull":
                    return System.DBNull.Value.ToString();

                case "system.decimal":
                case "decimal":
                    Decimal itemDecimal = (Decimal)value;
                    return itemDecimal.ToString();

                case "system.double":
                case "double":
                    Double itemDouble = (Double)value;
                    return itemDouble.ToString();

                case "system.int16":
                case "int16":
                case "short":
                    Int16 itemInt16 = (Int16)value;
                    return itemInt16.ToString();

                case "system.int32":
                case "int32":
                case "int":
                    Int32 itemInt32 = (Int32)value;
                    return itemInt32.ToString();

                case "system.int64":
                case "int64":
                case "long":
                    Int64 itemInt64 = (Int64)value;
                    return itemInt64.ToString();

                case "system.object":
                case "object":
                    Object itemObject = (Object)value;
                    return itemObject.ToString();

                case "system.sbyte":
                case "sbyte":
                    SByte itemSByte = (SByte)value;

                    // Foreach byte found convert the byte
                    // to an octet value string item.
                    string octetArray1 = itemSByte.ToString("X2");

                    // Create the octet string of bytes.
                    string octetRowVersion1 = "0x" + octetArray1;
                    return octetRowVersion1;

                case "system.single":
                case "single":
                case "float":
                    Single itemSingle = (Single)value;
                    return itemSingle.ToString();

                case "system.string":
                case "string":
                    String itemString = (String)value;
                    return "'" + itemString.Replace("'", "''") + "'";

                case "system.uint16":
                case "uint16":
                    UInt16 itemUInt16 = (UInt16)value;
                    return itemUInt16.ToString();

                case "system.uint32":
                case "uint32":
                case "uint":
                    UInt32 itemUInt32 = (UInt32)value;
                    return itemUInt32.ToString();

                case "system.uint64":
                case "uint64":
                case "ulong":
                    UInt64 itemUInt64 = (UInt64)value;
                    return itemUInt64.ToString();

                case "system.byte[]":
                case "byte[]":
                    Byte[] itemByteArray = (Byte[])value;

                    // Foreach byte found convert the byte
                    // to an octet value string item.
                    int i = 0;
                    string[] octetArrayByte = new string[itemByteArray.Count()];
                    foreach (Byte item in itemByteArray)
                        octetArrayByte[i++] = item.ToString("X2");

                    // Create the octet string of bytes.
                    string octetValue = "0x" + String.Join("", octetArrayByte);
                    return octetValue;

                case "system.sbyte[]":
                case "sbyte[]":
                    Byte[] itemSByteArray = (Byte[])value;

                    // Foreach byte found convert the byte
                    // to an octet value string item.
                    int j = 0;
                    string[] octetSArrayByte = new string[itemSByteArray.Count()];
                    foreach (Byte item in itemSByteArray)
                        octetSArrayByte[j++] = item.ToString("X2");

                    // Create the octet string of bytes.
                    string octetSValue = "0x" + String.Join("", octetSArrayByte);
                    return octetSValue;

                case "system.guid":
                case "guid":
                    Guid itemGuid = new Guid(value.ToString());
                    return "'" + itemGuid.ToString().Replace("'", "''") + "'";

                case "system.nullable`1":
                case "nullable`1":
                    Type[] genericArguments = systemType.GetGenericArguments();
                    return GetSqlStringValue(genericArguments[0], value);

                default:
                    Object itemObjectDefault = (Object)value;
                    return itemObjectDefault.ToString();
            }
        }

        /// <summary>
        /// Converts the system value type to the eqivalent type.
        /// </summary>
        /// <param name="systemType">The value type.</param>
        /// <param name="value">The original value.</param>
        /// <returns>The string sql equivalent.</returns>
        public static object GetDataTypeValue(Type systemType, string value)
        {
            switch (systemType.Name.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    return Boolean.Parse(value);

                case "system.byte":
                case "byte":
                    return Byte.Parse(value);

                case "system.char":
                case "char":
                    return Char.Parse(value);

                case "system.datetime":
                case "datetime":
                    return DateTime.Parse(value);

                case "system.timespan":
                case "timespan":
                    return TimeSpan.Parse(value);

                case "system.decimal":
                case "decimal":
                    return Decimal.Parse(value);

                case "system.double":
                case "double":
                    return Double.Parse(value);

                case "system.int16":
                case "int16":
                case "short":
                    return Int16.Parse(value);

                case "system.int32":
                case "int32":
                case "int":
                    return Int32.Parse(value);

                case "system.int64":
                case "int64":
                case "long":
                    return Int64.Parse(value);

                case "system.object":
                case "object":
                    return value;

                case "system.sbyte":
                case "sbyte":
                    return SByte.Parse(value);

                case "system.single":
                case "single":
                case "float":
                    return Single.Parse(value);

                case "system.string":
                case "string":
                    return value;

                case "system.uint16":
                case "uint16":
                    return UInt16.Parse(value);

                case "system.uint32":
                case "uint32":
                case "uint":
                    return UInt32.Parse(value);

                case "system.uint64":
                case "uint64":
                case "ulong":
                    return UInt64.Parse(value);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts the system value type to the eqivalent sql default value.
        /// </summary>
        /// <param name="systemType">The value type.</param>
        /// <returns>The default sql equivalent.</returns>
        public object GetSqlDefaultValue(Type systemType)
        {
            switch (systemType.Name.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    Boolean itemBoolean = false;
                    return itemBoolean;

                case "system.byte":
                case "byte":
                    Byte itemByte = 0;
                    return itemByte;

                case "system.char":
                case "char":
                    String itemChar = "0";
                    return itemChar;

                case "system.datetime":
                case "datetime":
                    DateTime itemDateTime = DateTime.Now;
                    return itemDateTime;

                case "system.timespan":
                case "timespan":
                    TimeSpan itemTimeSpan = new TimeSpan(1);
                    return itemTimeSpan;

                case "system.dbnull":
                case "dbnull":
                    String dbnullString = System.DBNull.Value.ToString();
                    return dbnullString;

                case "system.decimal":
                case "decimal":
                    Decimal itemDecimal = 0.0M;
                    return itemDecimal;

                case "system.double":
                case "double":
                    Double itemDouble = 0.0D;
                    return itemDouble;

                case "system.int16":
                case "int16":
                case "short":
                    Int16 itemInt16 = 0;
                    return itemInt16;

                case "system.int32":
                case "int32":
                case "int":
                    Int32 itemInt32 = 0;
                    return itemInt32;

                case "system.int64":
                case "int64":
                case "long":
                    Int64 itemInt64 = 0;
                    return itemInt64;

                case "system.object":
                case "object":
                    Object itemObject = default(Object);
                    return itemObject;

                case "system.sbyte":
                case "sbyte":
                    SByte itemSByte = 0;
                    return itemSByte;

                case "system.single":
                case "single":
                case "float":
                    Single itemSingle = 0.0F;
                    return itemSingle;

                case "system.string":
                case "string":
                    String itemString = "0";
                    return itemString;

                case "system.uint16":
                case "uint16":
                    UInt16 itemUInt16 = 0;
                    return itemUInt16;

                case "system.uint32":
                case "uint32":
                case "uint":
                    UInt32 itemUInt32 = 0;
                    return itemUInt32;

                case "system.uint64":
                case "uint64":
                case "ulong":
                    UInt64 itemUInt64 = 0;
                    return itemUInt64;

                case "system.byte[]":
                case "byte[]":
                    Byte[] itemByteArray = new Byte[2] { 0, 0 };
                    return itemByteArray;

                case "system.sbyte[]":
                case "sbyte[]":
                    SByte[] itemSByteArray = new SByte[2] { 0, 0 };
                    return itemSByteArray;

                case "system.guid":
                case "guid":
                    String guidString = Guid.NewGuid().ToString();
                    return guidString;

                case "system.nullable`1":
                case "nullable`1":
                    return null;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the sql conversion data type value.
        /// </summary>
        /// <param name="connectionDataType">The current sql data type</param>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted sql value.</returns>
        public static string GetSqlConversionDataType(
            ConnectionContext.ConnectionDataType connectionDataType, string value)
        {
            switch (connectionDataType)
            {
                case ConnectionContext.ConnectionDataType.SqliteDataType:
                case ConnectionContext.ConnectionDataType.AccessDataType:
                case ConnectionContext.ConnectionDataType.SqlDataType:
                    return "[" + value.Replace("[", "[").Replace("]", "]") + "]";
                case ConnectionContext.ConnectionDataType.OracleDataType:
                case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                case ConnectionContext.ConnectionDataType.ScxDataType:
                    return "\"" + value.Replace("[", "\"").Replace("]", "\"") + "\"";
                default:
                    return value.Replace("[", "").Replace("]", "");
            }
        }

        /// <summary>
        /// Get the sql conversion data type value.
        /// </summary>
        /// <param name="connectionDataType">The current sql data type</param>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted sql value.</returns>
        public static string GetSqlConversionDataTypeNoContainer(
            ConnectionContext.ConnectionDataType connectionDataType, string value)
        {
            switch (connectionDataType)
            {
                case ConnectionContext.ConnectionDataType.SqliteDataType:
                case ConnectionContext.ConnectionDataType.AccessDataType:
                case ConnectionContext.ConnectionDataType.SqlDataType:
                    return value.Replace("[", "[").Replace("]", "]");
                case ConnectionContext.ConnectionDataType.OracleDataType:
                case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                case ConnectionContext.ConnectionDataType.ScxDataType:
                    return value.Replace("[", "\"").Replace("]", "\"");
                default:
                    return value.Replace("[", "").Replace("]", "");
            }
        }
        #endregion

        #endregion
    }
}
