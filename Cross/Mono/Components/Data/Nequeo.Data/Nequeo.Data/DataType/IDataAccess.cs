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
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Nequeo.Data.DataType
{
    /// <summary>
    /// Custom data access provider.
    /// </summary>
    public interface IDataAccessBase
    {
        /// <summary>
        /// Gets the async result after completion.
        /// </summary>
        DataTable DataTableAsyncResult { get; }

        /// <summary>
        /// The async execute query data loader
        /// </summary>
        event EventHandler AsyncExecuteQueryComplete;

        /// <summary>
        /// Load data asyncronously
        /// </summary>
        /// <param name="command">The complete command</param>
        void ExecuteQuery(DbCommand command);

        /// <summary>
        /// Gets the async result after completion.
        /// </summary>
        Int32 CommandAsyncResult { get; }

        /// <summary>
        /// The async execute non query command.
        /// </summary>
        event EventHandler AsyncExecuteCommandComplete;

        /// <summary>
        /// Execute non query command asyncronously
        /// </summary>
        /// <param name="command">The complete command</param>
        void ExecuteCommand(DbCommand command);
    }

    /// <summary>
    /// Custom data access provider.
    /// </summary>
    public interface IDataAccess : IDataAccessBase
    {
        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection Connection(string databaseConnection);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data type to examine.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteClientQuery<TypeDataTable>(ref TypeDataTable dataTable,
            string queryText, CommandType commandType, string connectionString, params DbParameter[] values)
            where TypeDataTable : DataTable, new();

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particular table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteClientQuery(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params DbParameter[] values);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="pgCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        Int32 ExecuteClientCommand(ref DbCommand pgCommand, string commandText,
            CommandType commandType, string connectionString, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="TypeDataSet">The data type to examine.</typeparam>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The data tables names to return.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteClientQuery<TypeDataSet>(ref TypeDataSet dataSet, string[] tables,
            string queryText, CommandType commandType, string connectionString, params DbParameter[] values)
            where TypeDataSet : System.Data.DataSet, new();

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The data tables names to return.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteClientQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values);

        /// <summary>
        /// Gets the list of postgresql parameters.
        /// </summary>
        /// <param name="functionParameters">The function parameters created.</param>
        /// <param name="functionValues">The function values created.</param>
        /// <param name="methodInfo">The current method information.</param>
        /// <param name="parameters">The parameter collection.</param>
        /// <returns>The list of parameters.</returns>
        DbParameter[] GetParameters(ref string functionParameters, ref string functionValues,
            MethodInfo methodInfo, params Object[] parameters);

        /// <summary>
        /// Gets the list of odbc parameters.
        /// </summary>
        /// <param name="functionParameters">The function parameters created.</param>
        /// <param name="functionValues">The function values created.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="methodInfo">The current method information.</param>
        /// <param name="parameters">The parameter collection.</param>
        /// <returns>The list of parameters.</returns>
        DbParameter[] GetParameters(ref string functionParameters, ref string functionValues,
            ConnectionContext.ConnectionDataType connectionDataType, MethodInfo methodInfo, params Object[] parameters);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params DbParameter[] values);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="dbCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, string connectionString, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The tables names to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The datatable schema to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values);
       
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particular table.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, ConnectionContext.ConnectionDataType connectionDataType,
            params DbParameter[] values);
        
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="dbCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, string connectionString, ConnectionContext.ConnectionDataType connectionDataType,
            params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The tables names to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, string connectionString, ConnectionContext.ConnectionDataType connectionDataType,
            params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The datatable schema to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, string connectionString, ConnectionContext.ConnectionDataType connectionDataType,
            params DbParameter[] values);

        /// <summary>
        /// Gets the provider command builder.
        /// </summary>
        DbCommandBuilder CommandBuilder { get; }

        /// <summary>
        /// Gets the provider connection string builder.
        /// </summary>
        DbConnectionStringBuilder ConnectionStringBuilder { get; }

        /// <summary>
        /// Gets the provider data adapter.
        /// </summary>
        DbDataAdapter DataAdapter { get; }

        /// <summary>
        /// Gets the provider parameter.
        /// </summary>
        DbParameter Parameter { get; }

    }
}
