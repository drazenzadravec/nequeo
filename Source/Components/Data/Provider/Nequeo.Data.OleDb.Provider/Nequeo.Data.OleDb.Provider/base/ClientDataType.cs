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

namespace Nequeo.Data.OleDb
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
        /// <param name="oleDbDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static System.Data.OleDb.OleDbType GetOleDbDbType(string oleDbDataType)
        {
            switch (oleDbDataType.ToLower())
            {
                case "bigint":
                    return System.Data.OleDb.OleDbType.BigInt;
                case "binary":
                    return System.Data.OleDb.OleDbType.Binary;
                case "boolean":
                    return System.Data.OleDb.OleDbType.Boolean;
                case "bstr":
                    return System.Data.OleDb.OleDbType.BSTR;
                case "char":
                    return System.Data.OleDb.OleDbType.Char;
                case "currency":
                    return System.Data.OleDb.OleDbType.Currency;
                case "date":
                    return System.Data.OleDb.OleDbType.Date;
                case "dbdate":
                    return System.Data.OleDb.OleDbType.DBDate;
                case "dbtime":
                    return System.Data.OleDb.OleDbType.DBTime;
                case "dbtimestamp":
                    return System.Data.OleDb.OleDbType.DBTimeStamp;
                case "decimal":
                    return System.Data.OleDb.OleDbType.Decimal;
                case "double":
                    return System.Data.OleDb.OleDbType.Double;
                case "empty":
                    return System.Data.OleDb.OleDbType.Empty;
                case "error":
                    return System.Data.OleDb.OleDbType.Error;
                case "filetime":
                    return System.Data.OleDb.OleDbType.Filetime;
                case "guid":
                    return System.Data.OleDb.OleDbType.Guid;
                case "idispatch":
                    return System.Data.OleDb.OleDbType.IDispatch;
                case "integer":
                    return System.Data.OleDb.OleDbType.Integer;
                case "iunknown":
                    return System.Data.OleDb.OleDbType.IUnknown;
                case "longvarbinary":
                    return System.Data.OleDb.OleDbType.LongVarBinary;
                case "longvarchar":
                    return System.Data.OleDb.OleDbType.LongVarChar;
                case "longvarwchar":
                    return System.Data.OleDb.OleDbType.LongVarWChar;
                case "numeric":
                    return System.Data.OleDb.OleDbType.Numeric;
                case "propvariant":
                    return System.Data.OleDb.OleDbType.PropVariant;
                case "single":
                    return System.Data.OleDb.OleDbType.Single;
                case "smallint":
                    return System.Data.OleDb.OleDbType.SmallInt;
                case "tinyint":
                    return System.Data.OleDb.OleDbType.TinyInt;
                case "unsignedbigint":
                    return System.Data.OleDb.OleDbType.UnsignedBigInt;
                case "unsignedint":
                    return System.Data.OleDb.OleDbType.UnsignedInt;
                case "unsignedsmallint":
                    return System.Data.OleDb.OleDbType.UnsignedSmallInt;
                case "unsignedtinyint":
                    return System.Data.OleDb.OleDbType.UnsignedTinyInt;
                case "varbinary":
                    return System.Data.OleDb.OleDbType.VarBinary;
                case "varchar":
                    return System.Data.OleDb.OleDbType.VarChar;
                case "variant":
                    return System.Data.OleDb.OleDbType.Variant;
                case "varnumeric":
                    return System.Data.OleDb.OleDbType.VarNumeric;
                case "varwchar":
                    return System.Data.OleDb.OleDbType.VarWChar;
                case "wchar":
                    return System.Data.OleDb.OleDbType.WChar;
                default:
                    return System.Data.OleDb.OleDbType.Variant;
            }
        }
        #endregion
    }
}
