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

using PostgreSqlClient = Npgsql;

namespace Nequeo.Data.PostgreSql
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
        #endregion
    }
}
