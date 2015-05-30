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

using OracleClient = Oracle.ManagedDataAccess.Client;

namespace Nequeo.Data.Oracle
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
        /// <param name="oracleDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static OracleClient.OracleDbType GetOracleClientDbType(string oracleDataType)
        {
            switch (oracleDataType.ToLower())
            {
                case "bfile":
                    return OracleClient.OracleDbType.BFile;
                case "binarydouble":
                    return OracleClient.OracleDbType.BinaryDouble;
                case "binary double":
                    return OracleClient.OracleDbType.BinaryDouble;
                case "binary_double":
                    return OracleClient.OracleDbType.BinaryDouble;
                case "binaryfloat":
                    return OracleClient.OracleDbType.BinaryFloat;
                case "binary float":
                    return OracleClient.OracleDbType.BinaryFloat;
                case "binary_float":
                    return OracleClient.OracleDbType.BinaryFloat;
                case "blob":
                    return OracleClient.OracleDbType.Blob;
                case "byte":
                    return OracleClient.OracleDbType.Byte;
                case "char":
                    return OracleClient.OracleDbType.Char;
                case "clob":
                    return OracleClient.OracleDbType.Clob;
                case "cursor":
                    return OracleClient.OracleDbType.RefCursor;
                case "ref cursor":
                    return OracleClient.OracleDbType.RefCursor;
                case "refcursor":
                    return OracleClient.OracleDbType.RefCursor;
                case "datetime":
                    return OracleClient.OracleDbType.Date;
                case "Date":
                    return OracleClient.OracleDbType.Date;
                case "decimal":
                    return OracleClient.OracleDbType.Decimal;
                case "double":
                    return OracleClient.OracleDbType.Double;
                case "single":
                    return OracleClient.OracleDbType.Single;
                case "int16":
                    return OracleClient.OracleDbType.Int16;
                case "int32":
                    return OracleClient.OracleDbType.Int32;
                case "int64":
                    return OracleClient.OracleDbType.Int64;
                case "intervaldaytosecond":
                    return OracleClient.OracleDbType.IntervalDS;
                case "intervalds":
                    return OracleClient.OracleDbType.IntervalDS;
                case "intervalyeartomonth":
                    return OracleClient.OracleDbType.IntervalYM;
                case "intervalym":
                    return OracleClient.OracleDbType.IntervalYM;
                case "longvarchar":
                    return OracleClient.OracleDbType.Long;
                case "long raw":
                    return OracleClient.OracleDbType.LongRaw;
                case "long varchar":
                    return OracleClient.OracleDbType.Long;
                case "longraw":
                    return OracleClient.OracleDbType.LongRaw;
                case "nchar":
                    return OracleClient.OracleDbType.NChar;
                case "nclob":
                    return OracleClient.OracleDbType.NClob;
                case "nvarchar2":
                    return OracleClient.OracleDbType.NVarchar2;
                case "raw":
                    return OracleClient.OracleDbType.Raw;
                case "timestamp":
                    return OracleClient.OracleDbType.TimeStamp;
                case "timestamplocal":
                    return OracleClient.OracleDbType.TimeStampLTZ;
                case "timestampltz":
                    return OracleClient.OracleDbType.TimeStampLTZ;
                case "timestampwithtz":
                    return OracleClient.OracleDbType.TimeStampTZ;
                case "timestamptz":
                    return OracleClient.OracleDbType.TimeStampTZ;
                case "timestamp with local time zone":
                    return OracleClient.OracleDbType.TimeStampLTZ;
                case "timestamp with time zone":
                    return OracleClient.OracleDbType.TimeStampTZ;
                case "xmltype":
                    return OracleClient.OracleDbType.XmlType;
                case "varchar2":
                    return OracleClient.OracleDbType.Varchar2;
                default:
                    return OracleClient.OracleDbType.Varchar2;
            }
        }
        #endregion
    }
}
