using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OracleClient = Oracle.ManagedDataAccess.Client;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionProvider
    {
        /// <summary>
        /// Connection type enum.
        /// </summary>
        public enum ConnectionType
        {
            /// <summary>
            /// No connection
            /// </summary>
            None = 0,
            /// <summary>
            /// The sql connection
            /// </summary>
            SqlConnection = 1,
            /// <summary>
            /// The oledb connection
            /// </summary>
            OleDbConnection = 2,
            /// <summary>
            /// The odbc connection
            /// </summary>
            OdbcConnection = 3,
            /// <summary>
            /// The Oracle client connection
            /// </summary>
            OracleClientConnection = 4,
            /// <summary>
            /// The PostgreSql connection
            /// </summary>
            PostgreSqlConnection = 5,
            /// <summary>
            /// The MySql connection
            /// </summary>
            MySqlConnection = 6,
        }

        /// <summary>
        /// Connection data type enum.
        /// </summary>
        public enum ConnectionDataType
        {
            /// <summary>
            /// The no data type
            /// </summary>
            None = 0,
            /// <summary>
            /// The microsoft sql server data type
            /// </summary>
            SqlDataType = 1,
            /// <summary>
            /// The oracle server data type
            /// </summary>
            OracleDataType = 2,
            /// <summary>
            /// The microsoft access server data type
            /// </summary>
            AccessDataType = 3,
            /// <summary>
            /// The SCX server data type
            /// </summary>
            ScxDataType = 4,
            /// <summary>
            /// The PostgreSql server data type
            /// </summary>
            PostgreSqlDataType = 5,
            /// <summary>
            /// The MySql server data type
            /// </summary>
            MySqlDataType = 6,
        }

        /// <summary>
        /// Gets the connection type string value.
        /// </summary>
        /// <param name="connectionType">The connection type index.</param>
        /// <returns>The connection type value.</returns>
        public static string GetConnectionTypeValue(int connectionType)
        {
            // Get the connection type.
            switch (connectionType)
            {
                case 1:
                    return ConnectionType.SqlConnection.ToString();
                case 2:
                    return ConnectionType.OleDbConnection.ToString();
                case 3:
                    return ConnectionType.OdbcConnection.ToString();
                case 4:
                    return ConnectionType.OracleClientConnection.ToString();
                case 5:
                    return ConnectionType.PostgreSqlConnection.ToString();
                case 6:
                    return ConnectionType.MySqlConnection.ToString();
                default:
                    return ConnectionType.None.ToString();
            }
        }

        /// <summary>
        /// Gets the connection data type string value.
        /// </summary>
        /// <param name="connectionDataType">The connection data type index.</param>
        /// <returns>The connection data type value.</returns>
        public static string GetConnectionDataTypeValue(int connectionDataType)
        {
            // Get the connection data type.
            switch (connectionDataType)
            {
                case 1:
                    return ConnectionDataType.SqlDataType.ToString();
                case 2:
                    return ConnectionDataType.OracleDataType.ToString();
                case 3:
                    return ConnectionDataType.AccessDataType.ToString();
                case 4:
                    return ConnectionDataType.ScxDataType.ToString();
                case 5:
                    return ConnectionDataType.PostgreSqlDataType.ToString();
                case 6:
                    return ConnectionDataType.MySqlDataType.ToString();
                default:
                    return ConnectionDataType.None.ToString();
            }
        }

        /// <summary>
        /// Gets the connection type.
        /// </summary>
        /// <param name="connectionType">The connection type index.</param>
        /// <returns>The connection type.</returns>
        public static ConnectionType GetConnectionType(int connectionType)
        {
            // Get the connection type.
            switch (connectionType)
            {
                case 1:
                    return ConnectionType.SqlConnection;
                case 2:
                    return ConnectionType.OleDbConnection;
                case 3:
                    return ConnectionType.OdbcConnection;
                case 4:
                    return ConnectionType.OracleClientConnection;
                case 5:
                    return ConnectionType.PostgreSqlConnection;
                case 6:
                    return ConnectionType.MySqlConnection;
                default:
                    return ConnectionType.None;
            }
        }

        /// <summary>
        /// Gets the connection data type.
        /// </summary>
        /// <param name="connectionDataType">The connection data type index.</param>
        /// <returns>The connection data type.</returns>
        public static ConnectionDataType GetConnectionDataType(int connectionDataType)
        {
            // Get the connection data type.
            switch (connectionDataType)
            {
                case 1:
                    return ConnectionDataType.SqlDataType;
                case 2:
                    return ConnectionDataType.OracleDataType;
                case 3:
                    return ConnectionDataType.AccessDataType;
                case 4:
                    return ConnectionDataType.ScxDataType;
                case 5:
                    return ConnectionDataType.PostgreSqlDataType;
                case 6:
                    return ConnectionDataType.MySqlDataType;
                default:
                    return ConnectionDataType.None;
            }
        }
    }

    /// <summary>
    /// Linq data type and database conversion.
    /// </summary>
    public class LinqToDataTypes
    {
        /// <summary>
        /// Validated to the current column name parameter.
        /// </summary>
        /// <param name="value">The column to validated.</param>
        /// <returns>True if valid else false.</returns>
        public static bool ValidateColumnName(string value, ConnectionProvider.ConnectionType connectionType)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    if (!value.StartsWith("@"))
                        return false;
                    break;

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    return true;

                default:
                    return true;
            }

            return true;
        }

        /// <summary>
        /// Gets the default length.
        /// </summary>
        /// <param name="connectionType">The connection type.</param>
        /// <returns>The default length value.</returns>
        public static int DefaultLengthValue(ConnectionProvider.ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    return -1;

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    return 0;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts the word from singular to plural.
        /// </summary>
        /// <param name="name">The word to convert.</param>
        /// <returns>The converted word as plural.</returns>
        public static string Plural(string name)
        {
            if (name.EndsWith("x", StringComparison.InvariantCultureIgnoreCase)
                || name.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)
                || name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
            {
                return name + "es";
            }
            else if (name.EndsWith("y", StringComparison.InvariantCultureIgnoreCase))
            {
                return name.Substring(0, name.Length - 1) + "ies";
            }
            else if (!name.EndsWith("s"))
            {
                return name + "s";
            }
            return name;
        }

        /// <summary>
        /// Validates only double types
        /// </summary>
        /// <param name="number">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool NumberValidator(string number)
        {
            // Validates numbers 3446.77888 or .865886 or 20
            // \. means match only one " . " but " . " means match anything once at that point.
            if (!Regex.IsMatch(number.Trim(), @"^([0-9]+\.[0-9]+)$") &&
                !Regex.IsMatch(number.Trim(), @"^(\.[0-9]+)$") &&
                !Regex.IsMatch(number.ToString().Trim(), @"^([0-9]+)$"))
                return false;
            else
                return true;
        }

        private static string[] _keyWordList = new string[] { "Event", "Object",
            "Delegate", "Class", "Interface", "Enum", "Struct", "TypeOf", "Long",
            "Byte", "Bool", "String", "Decimal", "Double", "Int", "Float", 
            "Short", "Char", "Uint", "Ulong", "Value"};

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
        /// <returns>True if the type is nullable else false.</returns>
        public static bool GetLinqApplyNullableType(string sqlDataType, ConnectionProvider.ConnectionDataType type)
        {
            return !GetLinqNullableType(sqlDataType, type);
        }

        /// <summary>
        /// Gets an indicator that represents a linq type to be nullable.
        /// </summary>
        /// <param name="sqlDataType">The database type.</param>
        /// <returns>True if the type is nullable else false.</returns>
        public static bool GetLinqNullableType(string sqlDataType, ConnectionProvider.ConnectionDataType type)
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
                        case ConnectionProvider.ConnectionDataType.SqlDataType:
                        case ConnectionProvider.ConnectionDataType.AccessDataType:
                        case ConnectionProvider.ConnectionDataType.ScxDataType:
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
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the system data type.
        /// </summary>
        /// <param name="systemType">The system string data type.</param>
        /// <returns>>The system data type.</returns>
        public static Type GetSystemType(string systemType)
        {
            switch (systemType.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    return typeof(System.Boolean);
                case "system.byte":
                case "byte":
                    return typeof(System.Byte);
                case "system.char":
                case "char":
                    return typeof(System.Char);
                case "system.datetime":
                case "datetime":
                    return typeof(System.DateTime);
                case "system.dbnull":
                case "dbnull":
                    return typeof(System.DBNull);
                case "system.decimal":
                case "decimal":
                    return typeof(System.Decimal);
                case "system.double":
                case "double":
                    return typeof(System.Double);
                case "system.int16":
                case "int16":
                case "short":
                    return typeof(System.Int16);
                case "system.int32":
                case "int32":
                case "int":
                    return typeof(System.Int32);
                case "system.int64":
                case "int64":
                case "long":
                    return typeof(System.Int64);
                case "system.object":
                case "object":
                    return typeof(System.Object);
                case "system.sbyte":
                case "sbyte":
                    return typeof(System.SByte);
                case "system.single":
                case "single":
                case "float":
                    return typeof(System.Single);
                case "system.string":
                case "string":
                    return typeof(System.String);
                case "system.uint16":
                case "uint16":
                    return typeof(System.UInt16);
                case "system.uint32":
                case "uint32":
                case "uint":
                    return typeof(System.UInt32);
                case "system.uint64":
                case "uint64":
                case "ulong":
                    return typeof(System.UInt64);
                case "system.guid":
                case "guid":
                    return typeof(System.Guid);
                case "system.timespan":
                case "timespan":
                    return typeof(System.TimeSpan);
                case "system.array":
                case "array":
                    return typeof(System.Array);
                case "system.boolean[]":
                case "boolean[]":
                case "bool[]":
                    return typeof(System.Boolean[]);
                case "system.byte[]":
                case "byte[]":
                    return typeof(System.Byte[]);
                case "system.char[]":
                case "char[]":
                    return typeof(System.Char[]);
                case "system.datetime[]":
                case "datetime[]":
                    return typeof(System.DateTime[]);
                case "system.dbnull[]":
                case "dbnull[]":
                    return typeof(System.DBNull[]);
                case "system.decimal[]":
                case "decimal[]":
                    return typeof(System.Decimal[]);
                case "system.double[]":
                case "double[]":
                    return typeof(System.Double[]);
                case "system.int16[]":
                case "int16[]":
                case "short[]":
                    return typeof(System.Int16[]);
                case "system.int32[]":
                case "int32[]":
                case "int[]":
                    return typeof(System.Int32[]);
                case "system.int64[]":
                case "int64[]":
                case "long[]":
                    return typeof(System.Int64[]);
                case "system.object[]":
                case "object[]":
                    return typeof(System.Object[]);
                case "system.sbyte[]":
                case "sbyte[]":
                    return typeof(System.SByte[]);
                case "system.single[]":
                case "single[]":
                case "float[]":
                    return typeof(System.Single[]);
                case "system.string[]":
                case "string[]":
                    return typeof(System.String[]);
                case "system.uint16[]":
                case "uint16[]":
                    return typeof(System.UInt16[]);
                case "system.uint32[]":
                case "uint32[]":
                case "uint[]":
                    return typeof(System.UInt32[]);
                case "system.uint64[]":
                case "uint64[]":
                case "ulong[]":
                    return typeof(System.UInt64[]);
                case "system.guid[]":
                case "guid[]":
                    return typeof(System.Guid[]);
                case "system.timespan[]":
                case "timespan[]":
                    return typeof(System.TimeSpan[]);
                default:
                    return typeof(System.Object);
            }
        }

        /// <summary>
        /// Converts the database data type to the linq data type.
        /// </summary>
        /// <param name="sqlDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static Type GetLinqDataType(string sqlDataType, ConnectionProvider.ConnectionDataType type)
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
                        case ConnectionProvider.ConnectionDataType.SqlDataType:
                        case ConnectionProvider.ConnectionDataType.AccessDataType:
                        case ConnectionProvider.ConnectionDataType.ScxDataType:
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
                default:
                    return typeof(System.Object);
            }
        }

        /// <summary>
        /// Gets the OleDb data type.
        /// </summary>
        /// <param name="dataType">The current oledb data type value</param>
        /// <returns>The data type.</returns>
        public static string GetOleDbType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "130":
                case "129":
                    return "bstr";
                case "131":
                    return "decimal";
                case "139":
                    return "numeric";
                case "7":
                case "135":
                    return "date";
                case "5":
                case "4":
                    return "double";
                case "20":
                    return "bigint";
                case "3":
                case "2":
                    return "integer";
                case "17":
                    return "tinyint";
                case "6":
                    return "currency";
                case "11":
                    return "boolean";
                case "12":
                    return "variant";
                case "13":
                    return "longvarbinary";
                case "128":
                    return "binary";
                case "72":
                    return "guid";
                default:
                    return string.Empty;
            }
        }

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

        /// <summary>
        /// Converts the database data type to the linq data type.
        /// </summary>
        /// <param name="sqlDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static OracleClient.OracleDbType GetOracleClientDbType(string sqlDataType)
        {
            switch (sqlDataType.ToLower())
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

        /// <summary>
        /// Converts the database data type to the linq data type.
        /// </summary>
        /// <param name="pgDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static NpgsqlTypes.NpgsqlDbType GetPostgreSqlDbType(string pgDataType)
        {
            switch (pgDataType.ToLower())
            {
                case "array":
                    return NpgsqlTypes.NpgsqlDbType.Array;
                case "bigint":
                case "int8":
                    return NpgsqlTypes.NpgsqlDbType.Bigint;
                case "bit":
                    return NpgsqlTypes.NpgsqlDbType.Bit;
                case "boolean":
                    return NpgsqlTypes.NpgsqlDbType.Boolean;
                case "box":
                    return NpgsqlTypes.NpgsqlDbType.Box;
                case "bytea":
                    return NpgsqlTypes.NpgsqlDbType.Bytea;
                case "char":
                    return NpgsqlTypes.NpgsqlDbType.Char;
                case "circle":
                    return NpgsqlTypes.NpgsqlDbType.Circle;
                case "date":
                    return NpgsqlTypes.NpgsqlDbType.Date;
                case "double":
                case "float8":
                    return NpgsqlTypes.NpgsqlDbType.Double;
                case "inet":
                    return NpgsqlTypes.NpgsqlDbType.Inet;
                case "integer":
                case "int4":
                    return NpgsqlTypes.NpgsqlDbType.Integer;
                case "interval":
                    return NpgsqlTypes.NpgsqlDbType.Interval;
                case "line":
                    return NpgsqlTypes.NpgsqlDbType.Line;
                case "lseg":
                    return NpgsqlTypes.NpgsqlDbType.LSeg;
                case "money":
                    return NpgsqlTypes.NpgsqlDbType.Money;
                case "name":
                    return NpgsqlTypes.NpgsqlDbType.Name;
                case "numeric":
                    return NpgsqlTypes.NpgsqlDbType.Numeric;
                case "oidvector":
                    return NpgsqlTypes.NpgsqlDbType.Oidvector;
                case "path":
                    return NpgsqlTypes.NpgsqlDbType.Path;
                case "point":
                    return NpgsqlTypes.NpgsqlDbType.Point;
                case "polygon":
                    return NpgsqlTypes.NpgsqlDbType.Polygon;
                case "real":
                case "float4":
                    return NpgsqlTypes.NpgsqlDbType.Real;
                case "refcursor":
                    return NpgsqlTypes.NpgsqlDbType.Refcursor;
                case "smallint":
                case "int2":
                    return NpgsqlTypes.NpgsqlDbType.Smallint;
                case "text":
                    return NpgsqlTypes.NpgsqlDbType.Text;
                case "time":
                    return NpgsqlTypes.NpgsqlDbType.Time;
                case "timestamp":
                    return NpgsqlTypes.NpgsqlDbType.Timestamp;
                case "timestamptz":
                    return NpgsqlTypes.NpgsqlDbType.TimestampTZ;
                case "timetz":
                    return NpgsqlTypes.NpgsqlDbType.TimeTZ;
                case "uuid":
                    return NpgsqlTypes.NpgsqlDbType.Uuid;
                case "varchar":
                    return NpgsqlTypes.NpgsqlDbType.Varchar;
                case "xml":
                    return NpgsqlTypes.NpgsqlDbType.Xml;
                default:
                    return NpgsqlTypes.NpgsqlDbType.Varchar;
            }
        }

        /// <summary>
        /// Converts the database data type to the linq data type.
        /// </summary>
        /// <param name="myDataType">The database type.</param>
        /// <returns>The linq data type.</returns>
        public static MySql.Data.MySqlClient.MySqlDbType GetMySqlDbType(string myDataType)
        {
            switch (myDataType.ToLower())
            {
                case "binary":
                    return MySql.Data.MySqlClient.MySqlDbType.Binary;
                case "bit":
                    return MySql.Data.MySqlClient.MySqlDbType.Bit;
                case "blob":
                    return MySql.Data.MySqlClient.MySqlDbType.Blob;
                case "byte":
                    return MySql.Data.MySqlClient.MySqlDbType.Byte;
                case "date":
                    return MySql.Data.MySqlClient.MySqlDbType.Date;
                case "datetime":
                    return MySql.Data.MySqlClient.MySqlDbType.DateTime;
                case "decimal":
                    return MySql.Data.MySqlClient.MySqlDbType.Decimal;
                case "double":
                    return MySql.Data.MySqlClient.MySqlDbType.Double;
                case "enum":
                    return MySql.Data.MySqlClient.MySqlDbType.Enum;
                case "float":
                    return MySql.Data.MySqlClient.MySqlDbType.Float;
                case "geometry":
                    return MySql.Data.MySqlClient.MySqlDbType.Geometry;
                case "guid":
                    return MySql.Data.MySqlClient.MySqlDbType.Guid;
                case "int16":
                    return MySql.Data.MySqlClient.MySqlDbType.Int16;
                case "int24":
                    return MySql.Data.MySqlClient.MySqlDbType.Int24;
                case "int32":
                    return MySql.Data.MySqlClient.MySqlDbType.Int32;
                case "int64":
                    return MySql.Data.MySqlClient.MySqlDbType.Int64;
                case "longblob":
                    return MySql.Data.MySqlClient.MySqlDbType.LongBlob;
                case "longtext":
                    return MySql.Data.MySqlClient.MySqlDbType.LongText;
                case "mediumblob":
                    return MySql.Data.MySqlClient.MySqlDbType.MediumBlob;
                case "mediumtext":
                    return MySql.Data.MySqlClient.MySqlDbType.MediumText;
                case "newdecimal":
                    return MySql.Data.MySqlClient.MySqlDbType.NewDecimal;
                case "set":
                    return MySql.Data.MySqlClient.MySqlDbType.Set;
                case "text":
                    return MySql.Data.MySqlClient.MySqlDbType.Text;
                case "time":
                    return MySql.Data.MySqlClient.MySqlDbType.Time;
                case "timestamp":
                    return MySql.Data.MySqlClient.MySqlDbType.Timestamp;
                case "tinyblob":
                    return MySql.Data.MySqlClient.MySqlDbType.TinyBlob;
                case "tinytext":
                    return MySql.Data.MySqlClient.MySqlDbType.TinyText;
                case "ubyte":
                    return MySql.Data.MySqlClient.MySqlDbType.UByte;
                case "uint16":
                    return MySql.Data.MySqlClient.MySqlDbType.UInt16;
                case "uint24":
                    return MySql.Data.MySqlClient.MySqlDbType.UInt24;
                case "uint32":
                    return MySql.Data.MySqlClient.MySqlDbType.UInt32;
                case "uint64":
                    return MySql.Data.MySqlClient.MySqlDbType.UInt64;
                case "varbinary":
                    return MySql.Data.MySqlClient.MySqlDbType.VarBinary;
                case "varchar":
                    return MySql.Data.MySqlClient.MySqlDbType.VarChar;
                case "varstring":
                    return MySql.Data.MySqlClient.MySqlDbType.VarString;
                case "year":
                    return MySql.Data.MySqlClient.MySqlDbType.Year;
                default:
                    return MySql.Data.MySqlClient.MySqlDbType.VarChar;
            }
        }

        /// <summary>
        /// Converts the system value type to the eqivalent sql string.
        /// </summary>
        /// <param name="systemType">The value type.</param>
        /// <param name="value">The original value.</param>
        /// <returns>The string sql equivalent.</returns>
        public static object GetSqlStringValue(string systemType)
        {
            switch (systemType.ToLower())
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
                    Char itemChar = '0';
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
                    DBNull dbnullString = System.DBNull.Value;
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
                    Object itemObject = "0";
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
                    Guid guidString = Guid.NewGuid();
                    return guidString;

                default:
                    Object itemObjectDefault = "0";
                    return itemObjectDefault;
            }
        }
    }
}
