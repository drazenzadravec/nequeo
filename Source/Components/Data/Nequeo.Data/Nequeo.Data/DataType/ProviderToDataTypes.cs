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
    /// Linq data type and database conversion.
    /// </summary>
    public sealed class ProviderToDataTypes
    {
        #region Provider To Data Types

        private static string[] _keyWordList = new string[] { "Event", "Object",
            "Delegate", "Class", "Interface", "Enum", "Struct", "Type", "Long",
            "Byte", "Bool", "String", "Decimal", "Double", "Int", "Float", 
            "Short", "Char", "Uint", "Ulong", "Ushort"};

        /// <summary>
        /// Gets, the key word list.
        /// </summary>
        public static string[] KeyWordList
        {
            get
            {
                return _keyWordList;
            }
        }

        /// <summary>
        /// Directly applied nullable types.
        /// </summary>
        /// <param name="sqlDataType">The database type.</param>
        /// <param name="type">The database connection type.</param>
        /// <returns>True if the type is nullable else false.</returns>
        public static bool GetApplyNullableType(string sqlDataType,
            ConnectionContext.ConnectionDataType type)
        {
            return !GetNullableType(sqlDataType, type);
        }

        /// <summary>
        /// Gets an indicator that represents a linq type to be nullable.
        /// </summary>
        /// <param name="sqlDataType">The database type.</param>
        /// <param name="type">The database connection type.</param>
        /// <returns>True if the type is nullable else false.</returns>
        public static bool GetNullableType(string sqlDataType,
            ConnectionContext.ConnectionDataType type)
        {
            switch (sqlDataType.ToLower())
            {
                case "image":
                    return false;
                case "text":
                    return false;
                case "tinyint":
                    return true;
                case "smallint":
                    return true;
                case "int":
                    return true;
                case "smalldatetime":
                    return true;
                case "real":
                    return true;
                case "money":
                    return true;
                case "datetime":
                    return true;
                case "date":
                    return true;
                case "float":
                    return true;
                case "binarydouble":
                    return true;
                case "binaryfloat":
                    return true;
                case "binary double":
                    return true;
                case "binary float":
                    return true;
                case "binary_double":
                    return true;
                case "binary_float":
                    return true;
                case "ntext":
                    return false;
                case "bit":
                    return true;
                case "decimal":
                    return true;
                case "dec":
                    return true;
                case "smallmoney":
                    return true;
                case "bigint":
                    return true;
                case "varbinary":
                    return false;
                case "varchar":
                    return false;
                case "varchar1":
                    return false;
                case "varchar2":
                    return false;
                case "binary":
                    return false;
                case "char":
                    return false;
                case "nvarchar":
                    return false;
                case "nvarchar1":
                    return false;
                case "nvarchar2":
                    return false;
                case "nchar":
                    return false;
                case "uniqueidentifier":
                    return true;
                case "rowversion":
                    return false;
                case "xml":
                    return false;
                case "numeric":
                    return true;
                case "sql_variant":
                    return false;
                case "bfile":
                    return false;
                case "blob":
                    return false;
                case "byte":
                    return true;
                case "clob":
                    return false;
                case "cursor":
                    return false;
                case "ref cursor":
                    return false;
                case "refcursor":
                    return false;
                case "double":
                    return true;
                case "single":
                    return true;
                case "int16":
                    return true;
                case "int32":
                    return true;
                case "int64":
                    return true;
                case "interval day to second":
                    return false;
                case "interval year to month":
                    return true;
                case "intervalds":
                    return false;
                case "intervalym":
                    return true;
                case "long raw":
                    return false;
                case "longraw":
                    return false;
                case "long":
                    return false;
                case "long varchar":
                    return false;
                case "longvarchar":
                    return false;
                case "nclob":
                    return false;
                case "number":
                    return true;
                case "integer":
                    return true;
                case "raw":
                    return false;
                case "rowid":
                    return false;
                case "urowid":
                    return false;
                case "sbyte":
                    return true;
                case "timestamp with local time zone":
                    return true;
                case "timestamp with time zone":
                    return true;
                case "timestampltz":
                    return true;
                case "timestamptz":
                    return true;
                case "uint16":
                    return true;
                case "uint32":
                    return true;
                case "boolean":
                    return true;
                case "bstr":
                    return false;
                case "currency":
                    return true;
                case "dbdate":
                    return true;
                case "dbtime":
                    return true;
                case "dbtimestamp":
                    return true;
                case "filetime":
                    return true;
                case "guid":
                    return true;
                case "idispatch":
                    return false;
                case "iunknown":
                    return false;
                case "longvarbinary":
                    return false;
                case "longvarwchar":
                    return false;
                case "propvariant":
                    return false;
                case "unsignedbigint":
                    return true;
                case "unsignedint":
                    return true;
                case "unsignedsmallint":
                    return true;
                case "unsignedtinyint":
                    return true;
                case "variant":
                    return false;
                case "varnumeric":
                    return true;
                case "varwchar":
                    return false;
                case "wchar":
                    return false;
                case "xmltype":
                    return false;
                case "box":
                    return false;
                case "circle":
                    return false;
                case "line":
                    return false;
                case "lseg":
                    return false;
                case "oidvector":
                    return false;
                case "path":
                    return false;
                case "point":
                    return false;
                case "polygon":
                    return false;
                case "bytea":
                    return false;
                case "interval":
                    return true;
                case "array":
                    return false;
                case "inet":
                    return false;
                case "name":
                    return false;
                case "uuid":
                    return true;
                case "timetz":
                    return true;
                case "int8":
                    return true;
                case "float8":
                    return true;
                case "int4":
                    return true;
                case "float4":
                    return true;
                case "int2":
                    return true;
                case "timestamp":
                    switch (type)
                    {
                        case ConnectionContext.ConnectionDataType.SqlDataType:
                        case ConnectionContext.ConnectionDataType.AccessDataType:
                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            return false;
                        default:
                            return true;
                    }
                case "enum":
                    return false;
                case "geometry":
                    return false;
                case "int24":
                    return true;
                case "longblob":
                    return false;
                case "longtext":
                    return false;
                case "mediumblob":
                    return false;
                case "mediumtext":
                    return false;
                case "newdecimal":
                    return true;
                case "set":
                    return false;
                case "tinyblob":
                    return false;
                case "tinytext":
                    return false;
                case "ubyte":
                    return true;
                case "uint24":
                    return true;
                case "uint64":
                    return true;
                case "varstring":
                    return false;
                case "year":
                    return true;
                case "null":
                    return false;
                case "uninitialized":
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts the database data type to the linq data type.
        /// </summary>
        /// <param name="sqlDataType">The database type.</param>
        /// <param name="type">The database connection type.</param>
        /// <returns>The linq data type.</returns>
        public static Type GetDataType(string sqlDataType,
            ConnectionContext.ConnectionDataType type)
        {
            switch (sqlDataType.ToLower())
            {
                case "image":
                    return typeof(System.Byte[]);
                case "text":
                    return typeof(System.String);
                case "tinyint":
                    return typeof(System.Byte);
                case "smallint":
                    return typeof(System.Int16);
                case "int":
                    return typeof(System.Int32);
                case "smalldatetime":
                    return typeof(System.DateTime);
                case "real":
                    return typeof(System.Double);
                case "binarydouble":
                    return typeof(System.Byte[]);
                case "binary double":
                    return typeof(System.Byte[]);
                case "binary_double":
                    return typeof(System.Byte[]);
                case "binaryfloat":
                    return typeof(System.Byte[]);
                case "binary float":
                    return typeof(System.Byte[]);
                case "binary_float":
                    return typeof(System.Byte[]);
                case "money":
                    return typeof(System.Decimal);
                case "datetime":
                    return typeof(System.DateTime);
                case "date":
                    return typeof(System.DateTime);
                case "float":
                    return typeof(System.Double);
                case "single":
                    return typeof(System.Single);
                case "ntext":
                    return typeof(System.String);
                case "bit":
                    return typeof(System.Boolean);
                case "decimal":
                    return typeof(System.Decimal);
                case "dec":
                    return typeof(System.Decimal);
                case "smallmoney":
                    return typeof(System.Decimal);
                case "bigint":
                    return typeof(System.Int64);
                case "varbinary":
                    return typeof(System.Byte[]);
                case "varchar":
                    return typeof(System.String);
                case "varchar1":
                    return typeof(System.String);
                case "varchar2":
                    return typeof(System.String);
                case "binary":
                    return typeof(System.Byte[]);
                case "char":
                    return typeof(System.String);
                case "nvarchar":
                    return typeof(System.String);
                case "nvarchar1":
                    return typeof(System.String);
                case "nvarchar2":
                    return typeof(System.String);
                case "nchar":
                    return typeof(System.String);
                case "time":
                    return typeof(System.DateTime);
                case "uniqueidentifier":
                    return typeof(System.Guid);
                case "rowversion":
                    return typeof(System.Byte[]);
                case "xml":
                    return typeof(System.String);
                case "numeric":
                    return typeof(System.Decimal);
                case "sql_variant":
                    return typeof(System.Object);
                case "bfile":
                    return typeof(System.Byte[]);
                case "blob":
                    return typeof(System.Byte[]);
                case "byte":
                    return typeof(System.Byte);
                case "clob":
                    return typeof(System.String);
                case "cursor":
                    return typeof(System.Object);
                case "ref cursor":
                    return typeof(System.Object);
                case "refcursor":
                    return typeof(System.Object);
                case "double":
                    return typeof(System.Double);
                case "int16":
                    return typeof(System.Int16);
                case "int32":
                    return typeof(System.Int32);
                case "int64":
                    return typeof(System.Int64);
                case "interval day to second":
                    return typeof(System.Object);
                case "interval year to month":
                    return typeof(System.Int32);
                case "intervalds":
                    return typeof(System.Object);
                case "intervalym":
                    return typeof(System.Int32);
                case "long raw":
                    return typeof(System.Byte[]);
                case "longraw":
                    return typeof(System.Byte[]);
                case "long":
                    return typeof(System.String);
                case "long varchar":
                    return typeof(System.String);
                case "longvarchar":
                    return typeof(System.String);
                case "nclob":
                    return typeof(System.String);
                case "number":
                    return typeof(System.Decimal);
                case "integer":
                    return typeof(System.Int32);
                case "raw":
                    return typeof(System.Byte[]);
                case "rowid":
                    return typeof(System.String);
                case "urowid":
                    return typeof(System.String);
                case "sbyte":
                    return typeof(System.SByte);
                case "timestamp with local time zone":
                    return typeof(System.DateTime);
                case "timestamp with time zone":
                    return typeof(System.DateTime);
                case "timestampltz":
                    return typeof(System.DateTime);
                case "timestamptz":
                    return typeof(System.DateTime);
                case "uint16":
                    return typeof(System.UInt16);
                case "uint32":
                    return typeof(System.UInt32);
                case "boolean":
                    return typeof(System.Boolean);
                case "bstr":
                    return typeof(System.String);
                case "currency":
                    return typeof(System.Decimal);
                case "dbdate":
                    return typeof(System.DateTime);
                case "dbtime":
                    return typeof(System.TimeSpan);
                case "dbtimestamp":
                    return typeof(System.DateTime);
                case "filetime":
                    return typeof(System.DateTime);
                case "guid":
                    return typeof(System.Guid);
                case "idispatch":
                    return typeof(System.Object);
                case "iunknown":
                    return typeof(System.Object);
                case "longvarbinary":
                    return typeof(System.Byte[]);
                case "longvarwchar":
                    return typeof(System.String);
                case "propvariant":
                    return typeof(System.Object);
                case "unsignedbigint":
                    return typeof(System.UInt64);
                case "unsignedint":
                    return typeof(System.UInt32);
                case "unsignedsmallint":
                    return typeof(System.UInt16);
                case "unsignedtinyint":
                    return typeof(System.Byte);
                case "variant":
                    return typeof(System.Object);
                case "varnumeric":
                    return typeof(System.Decimal);
                case "varwchar":
                    return typeof(System.String);
                case "wchar":
                    return typeof(System.String);
                case "xmltype":
                    return typeof(System.String);
                case "box":
                    return typeof(System.Object);
                case "circle":
                    return typeof(System.Object);
                case "line":
                    return typeof(System.Object);
                case "lseg":
                    return typeof(System.Object);
                case "oidvector":
                    return typeof(System.Object);
                case "path":
                    return typeof(System.Object);
                case "point":
                    return typeof(System.Object);
                case "polygon":
                    return typeof(System.Object);
                case "bytea":
                    return typeof(System.Byte[]);
                case "interval":
                    return typeof(System.TimeSpan);
                case "array":
                    return typeof(System.Array);
                case "inet":
                    return typeof(System.String);
                case "name":
                    return typeof(System.String);
                case "uuid":
                    return typeof(System.Guid);
                case "timetz":
                    return typeof(System.DateTime);
                case "int8":
                    return typeof(System.Int64);
                case "float8":
                    return typeof(System.Double);
                case "int4":
                    return typeof(System.Int32);
                case "float4":
                    return typeof(System.Single);
                case "int2":
                    return typeof(System.Int16);
                case "timestamp":
                    switch (type)
                    {
                        case ConnectionContext.ConnectionDataType.SqlDataType:
                        case ConnectionContext.ConnectionDataType.AccessDataType:
                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            return typeof(System.Byte[]);
                        default:
                            return typeof(System.DateTime);

                    }
                case "enum":
                    return typeof(System.Object);
                case "geometry":
                    return typeof(System.Object);
                case "int24":
                    return typeof(System.Int32);
                case "longblob":
                    return typeof(System.Byte[]);
                case "longtext":
                    return typeof(System.String);
                case "mediumblob":
                    return typeof(System.Byte[]);
                case "mediumtext":
                    return typeof(System.String);
                case "newdecimal":
                    return typeof(System.Decimal);
                case "set":
                    return typeof(System.String);
                case "tinyblob":
                    return typeof(System.Byte[]);
                case "tinytext":
                    return typeof(System.String);
                case "ubyte":
                    return typeof(System.SByte);
                case "uint24":
                    return typeof(System.UInt32);
                case "uint64":
                    return typeof(System.UInt64);
                case "varstring":
                    return typeof(System.String);
                case "year":
                    return typeof(System.Int16);
                case "null":
                    return typeof(System.Object);
                case "uninitialized":
                    return typeof(System.Object);
                default:
                    return typeof(System.Object);
            }
        }

        /// <summary>
        /// Get the data type of a .NET Framework data provider.
        /// </summary>
        /// <param name="sqlDataType">The sql data type.</param>
        /// <param name="type">The database connection type.</param>
        /// <returns>The data type of a field, a property, or a Parameter object of
        /// a .NET Framework data provider.</returns>
        public static System.Data.DbType GetDbType(string sqlDataType,
            ConnectionContext.ConnectionDataType type)
        {
            switch (sqlDataType.ToLower())
            {
                case "image":
                    return System.Data.DbType.Binary;
                case "text":
                    return System.Data.DbType.String;
                case "tinyint":
                    return System.Data.DbType.Byte;
                case "smallint":
                    return System.Data.DbType.Int16;
                case "int":
                    return System.Data.DbType.Int32;
                case "smalldatetime":
                    return System.Data.DbType.DateTime;
                case "real":
                    return System.Data.DbType.Double;
                case "binarydouble":
                    return System.Data.DbType.Binary;
                case "binary double":
                    return System.Data.DbType.Binary;
                case "binary_double":
                    return System.Data.DbType.Binary;
                case "binaryfloat":
                    return System.Data.DbType.Binary;
                case "binary float":
                    return System.Data.DbType.Binary;
                case "binary_float":
                    return System.Data.DbType.Binary;
                case "money":
                    return System.Data.DbType.Currency;
                case "datetime":
                    return System.Data.DbType.DateTime;
                case "date":
                    return System.Data.DbType.Date;
                case "float":
                    return System.Data.DbType.Single;
                case "single":
                    return System.Data.DbType.Single;
                case "ntext":
                    return System.Data.DbType.String;
                case "bit":
                    return System.Data.DbType.Boolean;
                case "decimal":
                    return System.Data.DbType.Decimal;
                case "dec":
                    return System.Data.DbType.Decimal;
                case "smallmoney":
                    return System.Data.DbType.Currency;
                case "bigint":
                    return System.Data.DbType.Int64;
                case "varbinary":
                    return System.Data.DbType.Binary;
                case "varchar":
                    return System.Data.DbType.String;
                case "varchar1":
                    return System.Data.DbType.String;
                case "varchar2":
                    return System.Data.DbType.String;
                case "binary":
                    return System.Data.DbType.Binary;
                case "char":
                    return System.Data.DbType.String;
                case "nvarchar":
                    return System.Data.DbType.String;
                case "nvarchar1":
                    return System.Data.DbType.String;
                case "nvarchar2":
                    return System.Data.DbType.String;
                case "nchar":
                    return System.Data.DbType.String;
                case "time":
                    return System.Data.DbType.Time;
                case "uniqueidentifier":
                    return System.Data.DbType.Guid;
                case "rowversion":
                    return System.Data.DbType.Binary;
                case "xml":
                    return System.Data.DbType.Xml;
                case "numeric":
                    return System.Data.DbType.Decimal;
                case "sql_variant":
                    return System.Data.DbType.Object;
                case "bfile":
                    return System.Data.DbType.Binary;
                case "blob":
                    return System.Data.DbType.Binary;
                case "byte":
                    return System.Data.DbType.Byte;
                case "clob":
                    return System.Data.DbType.String;
                case "cursor":
                    return System.Data.DbType.Object;
                case "ref cursor":
                    return System.Data.DbType.Object;
                case "refcursor":
                    return System.Data.DbType.Object;
                case "double":
                    return System.Data.DbType.Double;
                case "int16":
                    return System.Data.DbType.Int16;
                case "int32":
                    return System.Data.DbType.Int32;
                case "int64":
                    return System.Data.DbType.Int64;
                case "interval day to second":
                    return System.Data.DbType.Object;
                case "interval year to month":
                    return System.Data.DbType.Int32;
                case "intervalds":
                    return System.Data.DbType.Object;
                case "intervalym":
                    return System.Data.DbType.Int32;
                case "long raw":
                    return System.Data.DbType.Binary;
                case "longraw":
                    return System.Data.DbType.Binary;
                case "long":
                    return System.Data.DbType.String;
                case "long varchar":
                    return System.Data.DbType.String;
                case "longvarchar":
                    return System.Data.DbType.String;
                case "nclob":
                    return System.Data.DbType.String;
                case "number":
                    return System.Data.DbType.Decimal;
                case "integer":
                    return System.Data.DbType.Int32;
                case "raw":
                    return System.Data.DbType.Binary;
                case "rowid":
                    return System.Data.DbType.String;
                case "urowid":
                    return System.Data.DbType.String;
                case "sbyte":
                    return System.Data.DbType.SByte;
                case "timestamp with local time zone":
                    return System.Data.DbType.DateTime;
                case "timestamp with time zone":
                    return System.Data.DbType.DateTime;
                case "timestampltz":
                    return System.Data.DbType.DateTime;
                case "timestamptz":
                    return System.Data.DbType.DateTime;
                case "uint16":
                    return System.Data.DbType.UInt16;
                case "uint32":
                    return System.Data.DbType.UInt32;
                case "boolean":
                    return System.Data.DbType.Boolean;
                case "bstr":
                    return System.Data.DbType.String;
                case "currency":
                    return System.Data.DbType.Currency;
                case "dbdate":
                    return System.Data.DbType.Date;
                case "dbtime":
                    return System.Data.DbType.Time;
                case "dbtimestamp":
                    return System.Data.DbType.DateTime;
                case "filetime":
                    return System.Data.DbType.DateTime;
                case "guid":
                    return System.Data.DbType.Guid;
                case "idispatch":
                    return System.Data.DbType.Object;
                case "iunknown":
                    return System.Data.DbType.Object;
                case "longvarbinary":
                    return System.Data.DbType.Binary;
                case "longvarwchar":
                    return System.Data.DbType.String;
                case "propvariant":
                    return System.Data.DbType.Object;
                case "unsignedbigint":
                    return System.Data.DbType.UInt64;
                case "unsignedint":
                    return System.Data.DbType.UInt32;
                case "unsignedsmallint":
                    return System.Data.DbType.UInt16;
                case "unsignedtinyint":
                    return System.Data.DbType.Byte;
                case "variant":
                    return System.Data.DbType.Object;
                case "varnumeric":
                    return System.Data.DbType.Decimal;
                case "varwchar":
                    return System.Data.DbType.String;
                case "wchar":
                    return System.Data.DbType.String;
                case "xmltype":
                    return System.Data.DbType.Xml;
                case "box":
                    return System.Data.DbType.Object;
                case "circle":
                    return System.Data.DbType.Object;
                case "line":
                    return System.Data.DbType.Object;
                case "lseg":
                    return System.Data.DbType.Object;
                case "oidvector":
                    return System.Data.DbType.Object;
                case "path":
                    return System.Data.DbType.Object;
                case "point":
                    return System.Data.DbType.Object;
                case "polygon":
                    return System.Data.DbType.Object;
                case "bytea":
                    return System.Data.DbType.Binary;
                case "interval":
                    return System.Data.DbType.DateTime;
                case "array":
                    return System.Data.DbType.Binary;
                case "inet":
                    return System.Data.DbType.String;
                case "name":
                    return System.Data.DbType.String;
                case "uuid":
                    return System.Data.DbType.Guid;
                case "timetz":
                    return System.Data.DbType.DateTime;
                case "int8":
                    return System.Data.DbType.Int64;
                case "float8":
                    return System.Data.DbType.Double;
                case "int4":
                    return System.Data.DbType.Int32;
                case "float4":
                    return System.Data.DbType.Single;
                case "int2":
                    return System.Data.DbType.Int16;
                case "timestamp":
                    switch (type)
                    {
                        case ConnectionContext.ConnectionDataType.SqlDataType:
                        case ConnectionContext.ConnectionDataType.AccessDataType:
                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            return System.Data.DbType.Binary;
                        default:
                            return System.Data.DbType.DateTime;

                    }
                case "enum":
                    return System.Data.DbType.Object;
                case "geometry":
                    return System.Data.DbType.Object;
                case "int24":
                    return System.Data.DbType.Int32;
                case "longblob":
                    return System.Data.DbType.Binary;
                case "longtext":
                    return System.Data.DbType.String;
                case "mediumblob":
                    return System.Data.DbType.Binary;
                case "mediumtext":
                    return System.Data.DbType.String;
                case "newdecimal":
                    return System.Data.DbType.Decimal;
                case "set":
                    return System.Data.DbType.String;
                case "tinyblob":
                    return System.Data.DbType.Binary;
                case "tinytext":
                    return System.Data.DbType.String;
                case "ubyte":
                    return System.Data.DbType.SByte;
                case "uint24":
                    return System.Data.DbType.UInt32;
                case "uint64":
                    return System.Data.DbType.UInt64;
                case "varstring":
                    return System.Data.DbType.String;
                case "year":
                    return System.Data.DbType.UInt16;
                case "null":
                    return System.Data.DbType.Object;
                case "uninitialized":
                    return System.Data.DbType.Object;
                default:
                    return System.Data.DbType.Object;
            }
        }

        /// <summary>
        /// Get the SQL date part from the time span.
        /// </summary>
        /// <param name="timeSpan">The time span to get the date part from.</param>
        /// <param name="timeSpanValue">The time span equivalent value.</param>
        /// <returns>The SQL date part for the time span.</returns>
        public static SqlDatePartType GetSqlDatePartFromTimeSpan(TimeSpan timeSpan, out long timeSpanValue)
        {
            if (timeSpan.Days >= 365)
            {
                timeSpanValue = Convert.ToInt64(((double)timeSpan.Days) / ((double)365));
                return SqlDatePartType.Year;
            }
            else if (timeSpan.Days > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.Days);
                return SqlDatePartType.Day;
            }
            else if (timeSpan.Hours > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.Hours);
                return SqlDatePartType.Hour;
            }
            else if (timeSpan.Minutes > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.Minutes);
                return SqlDatePartType.Minute;
            }
            else if (timeSpan.Seconds > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.Seconds);
                return SqlDatePartType.Second;
            }
            else if (timeSpan.Milliseconds > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.Milliseconds);
                return SqlDatePartType.Millisecond;
            }
            else
            {
                timeSpanValue = timeSpan.Ticks;
                return SqlDatePartType.Nanosecond;
            }
        }

        /// <summary>
        /// Get the SQL date part from the time span.
        /// </summary>
        /// <param name="timeSpan">The time span to get the date part from.</param>
        /// <param name="timeSpanValue">The time span equivalent value.</param>
        /// <returns>The SQL date part for the time span.</returns>
        public static SqlDatePartType GetSqlDatePartFromTimeSpanTotal(TimeSpan timeSpan, out long timeSpanValue)
        {
            if (timeSpan.Days >= 365)
            {
                timeSpanValue = Convert.ToInt64(((double)timeSpan.TotalDays) / ((double)365));
                return SqlDatePartType.Year;
            }
            else if (timeSpan.Days > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.TotalDays);
                return SqlDatePartType.Day;
            }
            else if (timeSpan.Hours > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.TotalHours);
                return SqlDatePartType.Hour;
            }
            else if (timeSpan.Minutes > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.TotalMinutes);
                return SqlDatePartType.Minute;
            }
            else if (timeSpan.Seconds > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.TotalSeconds);
                return SqlDatePartType.Second;
            }
            else if (timeSpan.Milliseconds > 0)
            {
                timeSpanValue = Convert.ToInt64(timeSpan.TotalMilliseconds);
                return SqlDatePartType.Millisecond;
            }
            else
            {
                timeSpanValue = timeSpan.Ticks;
                return SqlDatePartType.Nanosecond;
            }
        }
        #endregion
    }

    #region SqlDatePartType

    /// <summary>
    /// The SQL date part type.
    /// </summary>
    public enum SqlDatePartType : int
    {
        /// <summary>
        /// year
        /// </summary>
        Year = 0,
        /// <summary>
        /// quarter
        /// </summary>
        Quarter = 1,
        /// <summary>
        /// month
        /// </summary>
        Month = 2,
        /// <summary>
        /// dayofyear
        /// </summary>
        DayOfYear = 3,
        /// <summary>
        /// day
        /// </summary>
        Day = 4,
        /// <summary>
        /// week
        /// </summary>
        Week = 5,
        /// <summary>
        /// hour
        /// </summary>
        Hour = 6,
        /// <summary>
        /// minute
        /// </summary>
        Minute = 7,
        /// <summary>
        /// second
        /// </summary>
        Second = 8,
        /// <summary>
        /// millisecond
        /// </summary>
        Millisecond = 9,
        /// <summary>
        /// microsecond
        /// </summary>
        Microsecond = 10,
        /// <summary>
        /// nanosecond
        /// </summary>
        Nanosecond = 11,
    }
    #endregion
    
}
