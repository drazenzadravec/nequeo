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

namespace Nequeo.Data.Odbc
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
        /// <param name="odbcDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static System.Data.Odbc.OdbcType GetOdbcDbType(string odbcDataType)
        {
            switch (odbcDataType.ToLower())
            {
                case "bigint":
                    return System.Data.Odbc.OdbcType.BigInt;
                case "binary":
                    return System.Data.Odbc.OdbcType.Binary;
                case "bit":
                    return System.Data.Odbc.OdbcType.Bit;
                case "char":
                    return System.Data.Odbc.OdbcType.Char;
                case "date":
                    return System.Data.Odbc.OdbcType.Date;
                case "datetime":
                    return System.Data.Odbc.OdbcType.DateTime;
                case "decimal":
                    return System.Data.Odbc.OdbcType.Decimal;
                case "double":
                    return System.Data.Odbc.OdbcType.Double;
                case "image":
                    return System.Data.Odbc.OdbcType.Image;
                case "int":
                    return System.Data.Odbc.OdbcType.Int;
                case "nchar":
                    return System.Data.Odbc.OdbcType.NChar;
                case "ntext":
                    return System.Data.Odbc.OdbcType.NText;
                case "numeric":
                    return System.Data.Odbc.OdbcType.Numeric;
                case "nvarchar":
                    return System.Data.Odbc.OdbcType.NVarChar;
                case "real":
                    return System.Data.Odbc.OdbcType.Real;
                case "smalldatetime":
                    return System.Data.Odbc.OdbcType.SmallDateTime;
                case "smallint":
                    return System.Data.Odbc.OdbcType.SmallInt;
                case "text":
                    return System.Data.Odbc.OdbcType.Text;
                case "time":
                    return System.Data.Odbc.OdbcType.Time;
                case "timestamp":
                    return System.Data.Odbc.OdbcType.Timestamp;
                case "tinyint":
                    return System.Data.Odbc.OdbcType.TinyInt;
                case "uniqueidentifier":
                    return System.Data.Odbc.OdbcType.UniqueIdentifier;
                case "varbinary":
                    return System.Data.Odbc.OdbcType.VarBinary;
                case "varchar":
                    return System.Data.Odbc.OdbcType.VarChar;
                default:
                    return System.Data.Odbc.OdbcType.Text;
            }
        }
        #endregion
    }
}
