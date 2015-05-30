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

using MySqlClient = MySql.Data.MySqlClient;

namespace Nequeo.Data.MySql
{
    /// <summary>
    /// Client data type and database conversion.
    /// </summary>
    public sealed class ClientDataType
    {
        #region Client To Data Types
        /// <summary>
        /// Converts the database data type to the linq data type.
        /// </summary>
        /// <param name="myDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static MySqlClient.MySqlDbType GetMySqlDbType(string myDataType)
        {
            switch (myDataType.ToLower())
            {
                case "binary":
                    return MySqlClient.MySqlDbType.Binary;
                case "bit":
                    return MySqlClient.MySqlDbType.Bit;
                case "blob":
                    return MySqlClient.MySqlDbType.Blob;
                case "byte":
                    return MySqlClient.MySqlDbType.Byte;
                case "date":
                    return MySqlClient.MySqlDbType.Date;
                case "datetime":
                    return MySqlClient.MySqlDbType.DateTime;
                case "decimal":
                    return MySqlClient.MySqlDbType.Decimal;
                case "double":
                    return MySqlClient.MySqlDbType.Double;
                case "enum":
                    return MySqlClient.MySqlDbType.Enum;
                case "float":
                    return MySqlClient.MySqlDbType.Float;
                case "geometry":
                    return MySqlClient.MySqlDbType.Geometry;
                case "guid":
                    return MySqlClient.MySqlDbType.Guid;
                case "int16":
                    return MySqlClient.MySqlDbType.Int16;
                case "int24":
                    return MySqlClient.MySqlDbType.Int24;
                case "int32":
                    return MySqlClient.MySqlDbType.Int32;
                case "int64":
                    return MySqlClient.MySqlDbType.Int64;
                case "longblob":
                    return MySqlClient.MySqlDbType.LongBlob;
                case "longtext":
                    return MySqlClient.MySqlDbType.LongText;
                case "mediumblob":
                    return MySqlClient.MySqlDbType.MediumBlob;
                case "mediumtext":
                    return MySqlClient.MySqlDbType.MediumText;
                case "newdecimal":
                    return MySqlClient.MySqlDbType.NewDecimal;
                case "set":
                    return MySqlClient.MySqlDbType.Set;
                case "text":
                    return MySqlClient.MySqlDbType.Text;
                case "time":
                    return MySqlClient.MySqlDbType.Time;
                case "timestamp":
                    return MySqlClient.MySqlDbType.Timestamp;
                case "tinyblob":
                    return MySqlClient.MySqlDbType.TinyBlob;
                case "tinytext":
                    return MySqlClient.MySqlDbType.TinyText;
                case "ubyte":
                    return MySqlClient.MySqlDbType.UByte;
                case "uint16":
                    return MySqlClient.MySqlDbType.UInt16;
                case "uint24":
                    return MySqlClient.MySqlDbType.UInt24;
                case "uint32":
                    return MySqlClient.MySqlDbType.UInt32;
                case "uint64":
                    return MySqlClient.MySqlDbType.UInt64;
                case "varbinary":
                    return MySqlClient.MySqlDbType.VarBinary;
                case "varchar":
                    return MySqlClient.MySqlDbType.VarChar;
                case "varstring":
                    return MySqlClient.MySqlDbType.VarString;
                case "year":
                    return MySqlClient.MySqlDbType.Year;
                default:
                    return MySqlClient.MySqlDbType.VarChar;
            }
        }
        #endregion
    }
}
