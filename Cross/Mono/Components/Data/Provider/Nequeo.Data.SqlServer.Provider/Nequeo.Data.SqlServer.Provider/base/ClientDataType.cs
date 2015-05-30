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

namespace Nequeo.Data.SqlServer
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
        /// <param name="sqlDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static System.Data.SqlDbType GetSqlDbType(string sqlDataType)
        {
            switch (sqlDataType.ToLower())
            {
                case "image":
                    return System.Data.SqlDbType.Image;
                case "text":
                    return System.Data.SqlDbType.Text;
                case "tinyint":
                    return System.Data.SqlDbType.TinyInt;
                case "smallint":
                    return System.Data.SqlDbType.SmallInt;
                case "int":
                    return System.Data.SqlDbType.Int;
                case "smalldatetime":
                    return System.Data.SqlDbType.SmallDateTime;
                case "real":
                    return System.Data.SqlDbType.Real;
                case "money":
                    return System.Data.SqlDbType.Money;
                case "datetime":
                    return System.Data.SqlDbType.DateTime;
                case "date":
                    return System.Data.SqlDbType.Date;
                case "float":
                    return System.Data.SqlDbType.Float;
                case "ntext":
                    return System.Data.SqlDbType.NText;
                case "bit":
                    return System.Data.SqlDbType.Bit;
                case "decimal":
                    return System.Data.SqlDbType.Decimal;
                case "numeric":
                    return System.Data.SqlDbType.Decimal;
                case "smallmoney":
                    return System.Data.SqlDbType.SmallMoney;
                case "bigint":
                    return System.Data.SqlDbType.BigInt;
                case "varbinary":
                    return System.Data.SqlDbType.VarBinary;
                case "varchar":
                    return System.Data.SqlDbType.VarChar;
                case "binary":
                    return System.Data.SqlDbType.Binary;
                case "char":
                    return System.Data.SqlDbType.Char;
                case "nvarchar":
                    return System.Data.SqlDbType.NVarChar;
                case "nchar":
                    return System.Data.SqlDbType.NChar;
                case "timestamp":
                    return System.Data.SqlDbType.Timestamp;
                case "uniqueidentifier":
                    return System.Data.SqlDbType.UniqueIdentifier;
                case "sql_variant":
                    return System.Data.SqlDbType.Variant;
                case "xml":
                    return System.Data.SqlDbType.Xml;
                default:
                    return System.Data.SqlDbType.Variant;
            }
        }
        #endregion
    }
}
