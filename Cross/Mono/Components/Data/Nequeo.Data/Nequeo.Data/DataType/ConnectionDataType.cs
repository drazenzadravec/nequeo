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
    /// The connection data type context.
    /// </summary>
    public class ConnectionContext
    {
        /// <summary>
        /// Connection type enum.
        /// </summary>
        public enum ConnectionType
        {
            /// <summary>
            /// No connection type.
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
            /// The PostgreSql client connection
            /// </summary>
            PostgreSqlConnection = 5,
            /// <summary>
            /// The MySql client connection
            /// </summary>
            MySqlConnection = 6,
            /// <summary>
            /// The SQLite client connection
            /// </summary>
            SqliteConnection = 7,
        }

        /// <summary>
        /// Connection data type enum.
        /// </summary>
        public enum ConnectionDataType
        {
            /// <summary>
            /// A proprietary sql data type
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
            /// <summary>
            /// The SQLite server data type
            /// </summary>
            SqliteDataType = 7,
        }

        /// <summary>
        /// Converts to the respective conntion type.
        /// </summary>
        public class ConnectionTypeConverter
        {
            /// <summary>
            /// Gets the connection type.
            /// </summary>
            /// <param name="connectionType">The connection type index.</param>
            /// <returns>The connection type.</returns>
            public static ConnectionType GetConnectionType(string connectionType)
            {
                // Get the connection type.
                switch (connectionType.ToLower())
                {
                    case "sqlconnection":
                        return ConnectionType.SqlConnection;
                    case "oledbconnection":
                        return ConnectionType.OleDbConnection;
                    case "odbcconnection":
                        return ConnectionType.OdbcConnection;
                    case "oracleclientconnection":
                        return ConnectionType.OracleClientConnection;
                    case "postgresqlconnection":
                        return ConnectionType.PostgreSqlConnection;
                    case "mysqlconnection":
                        return ConnectionType.MySqlConnection;
                    case "sqliteconnection":
                        return ConnectionType.SqliteConnection;
                    default:
                        return ConnectionType.None;
                }
            }

            /// <summary>
            /// Gets the connection data type.
            /// </summary>
            /// <param name="connectionDataType">The connection data type index.</param>
            /// <returns>The connection data type.</returns>
            public static ConnectionDataType GetConnectionDataType(string connectionDataType)
            {
                // Get the connection data type.
                switch (connectionDataType.ToLower())
                {
                    case "sqldatatype":
                        return ConnectionDataType.SqlDataType;
                    case "oracledatatype":
                        return ConnectionDataType.OracleDataType;
                    case "accessdatatype":
                        return ConnectionDataType.AccessDataType;
                    case "scxdatatype":
                        return ConnectionDataType.ScxDataType;
                    case "postgresqldatatype":
                        return ConnectionDataType.PostgreSqlDataType;
                    case "mysqldatatype":
                        return ConnectionDataType.MySqlDataType;
                    case "sqlitedatatype":
                        return ConnectionDataType.SqliteDataType;
                    default:
                        return ConnectionDataType.None;
                }
            }
        }
    }
}
