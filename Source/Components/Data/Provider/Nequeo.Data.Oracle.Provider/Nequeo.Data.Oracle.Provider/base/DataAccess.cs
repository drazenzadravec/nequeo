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
using System.ComponentModel.Composition;

using OracleClient = Oracle.ManagedDataAccess.Client;

using Nequeo.ComponentModel.Composition;
using Nequeo.Data.DataType;
using Nequeo.Data.Extension;

namespace Nequeo.Data.Oracle
{
    /// <summary>
    /// Custom data access class.
    /// </summary>
    [Export(typeof(IDataAccess))]
    [ContentMetadata(Name = "Oracle_DataAccess_Provider")]
    public sealed class DataAccess : IDataAccess
    {
        #region Oracle Client Query Connections

        private DataTable _dataTableAsyncResult = null;
        /// <summary>
        /// Gets the async result after completion.
        /// </summary>
        public DataTable DataTableAsyncResult
        {
            get { return _dataTableAsyncResult; }
        }

        /// <summary>
        /// The async execute query data loader
        /// </summary>
        public event EventHandler AsyncExecuteQueryComplete;

        /// <summary>
        /// Load data asyncronously
        /// </summary>
        /// <param name="command">The complete command</param>
        public async void ExecuteQuery(DbCommand command)
        {
            System.Threading.Tasks.Task<DataTable> data = command.LoadAsync();
            _dataTableAsyncResult = await data;

            if (AsyncExecuteQueryComplete != null)
                AsyncExecuteQueryComplete(this, new EventArgs());
        }

        private Int32 _commandAsyncResult = -1;
        /// <summary>
        /// Gets the async result after completion.
        /// </summary>
        public Int32 CommandAsyncResult
        {
            get { return _commandAsyncResult; }
        }

        /// <summary>
        /// The async execute non query command.
        /// </summary>
        public event EventHandler AsyncExecuteCommandComplete;

        /// <summary>
        /// Execute non query command asyncronously
        /// </summary>
        /// <param name="command">The complete command</param>
        public async void ExecuteCommand(DbCommand command)
        {
            System.Threading.Tasks.Task<Int32> data = command.NonQueryAsync();
            _commandAsyncResult = await data;

            if (AsyncExecuteCommandComplete != null)
                AsyncExecuteCommandComplete(this, new EventArgs());
        }

        /// <summary>
        /// Gets the provider command builder.
        /// </summary>
        public DbCommandBuilder CommandBuilder
        {
            get { return new OracleClient.OracleCommandBuilder(); }
        }

        /// <summary>
        /// Gets the provider connection string builder.
        /// </summary>
        public DbConnectionStringBuilder ConnectionStringBuilder
        {
            get { return new OracleClient.OracleConnectionStringBuilder(); }
        }

        /// <summary>
        /// Gets the provider data adapter.
        /// </summary>
        public DbDataAdapter DataAdapter
        {
            get { return new OracleClient.OracleDataAdapter(); }
        }

        /// <summary>
        /// Gets the provider parameter.
        /// </summary>
        public DbParameter Parameter
        {
            get { return new OracleClient.OracleParameter(); }
        }

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        public DbConnection Connection(string databaseConnection)
        {
            return new OracleClient.OracleConnection(databaseConnection);
        }

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
        public DbCommand ExecuteClientQuery<TypeDataTable>(ref TypeDataTable dataTable,
            string queryText, CommandType commandType, string connectionString, params DbParameter[] values)
            where TypeDataTable : DataTable, new()
        {
            // Initial connection objects.
            OracleClient.OracleCommand sqlCommand = null;
            OracleClient.OracleConnection connection = null;
            IDataReader dataReader = null;
            dataTable = new TypeDataTable();

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    // Create the command and assign any parameters.
                    sqlCommand = new OracleClient.OracleCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader())
                    {
                        System.Data.DataSet localDataSet = new System.Data.DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Close the database connection.
                    connection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return sqlCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (connection != null)
                    connection.Close();
            }
        }

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
        public DbCommand ExecuteClientQuery(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params DbParameter[] values)
        {
            // Initial connection objects.
            OracleClient.OracleCommand sqlCommand = null;
            OracleClient.OracleConnection connection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    // Create the command and assign any parameters.
                    sqlCommand = new OracleClient.OracleCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader())
                    {
                        // Get the schema from the data because the
                        // table has not predefined schema
                        if (getSchemaTable)
                        {
                            // Load the table after the schema is
                            // returned.
                            dataTable = dataReader.GetSchemaTable();
                            dataTable = new DataTable("TableData");
                            System.Data.DataSet localDataSet = new System.Data.DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            System.Data.DataSet localDataSet = new System.Data.DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    connection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return sqlCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (connection != null)
                    connection.Close();
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="oracleCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        public Int32 ExecuteClientCommand(ref DbCommand oracleCommand, string commandText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            oracleCommand = null;
            Int32 returnValue = -1;
            OracleClient.OracleConnection connection = null;
            OracleClient.OracleTransaction transaction = null;

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    transaction = connection.BeginTransaction();

                    // Create the command and assign any parameters.
                    oracleCommand = new OracleClient.OracleCommand(commandText, connection);
                    oracleCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            oracleCommand.Parameters.Add(sqlParameter);

                    // Execute the command.
                    returnValue = oracleCommand.ExecuteNonQuery();

                    // Commit the transaction.
                    transaction.Commit();

                    // Close the database connection.
                    connection.Close();
                }

                // Return true.
                return returnValue;
            }
            catch (Exception ex)
            {
                try
                {
                    // Attempt to roll back the transaction.
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp.InnerException);
                }

                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

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
        public DbCommand ExecuteClientQuery<TypeDataSet>(ref TypeDataSet dataSet, string[] tables,
            string queryText, CommandType commandType, string connectionString, params DbParameter[] values)
            where TypeDataSet : System.Data.DataSet, new()
        {
            // Initial connection objects.
            OracleClient.OracleCommand sqlCommand = null;
            OracleClient.OracleConnection connection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    // Create the command and assign any parameters.
                    sqlCommand = new OracleClient.OracleCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader())
                    {
                        dataSet = new TypeDataSet();
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Close the database connection.
                    connection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return sqlCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (connection != null)
                    connection.Close();
            }
        }

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
        public DbCommand ExecuteClientQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            OracleClient.OracleCommand sqlCommand = null;
            OracleClient.OracleConnection connection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    // Create the command and assign any parameters.
                    sqlCommand = new OracleClient.OracleCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader())
                    {
                        dataSet = new System.Data.DataSet();
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Close the database connection.
                    connection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return sqlCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (connection != null)
                    connection.Close();
            }
        }

        /// <summary>
        /// Gets the list of oracle parameters.
        /// </summary>
        /// <param name="functionParameters">The function parameters created.</param>
        /// <param name="functionValues">The function values created.</param>
        /// <param name="methodInfo">The current method information.</param>
        /// <param name="parameters">The parameter collection.</param>
        /// <returns>The list of parameters.</returns>
        public DbParameter[] GetParameters(ref string functionParameters, ref string functionValues,
            MethodInfo methodInfo, params Object[] parameters)
        {
            int i = -1;
            long length = -1;
            string dbType = null;
            bool isNullable = true;
            string parameterName = null;
            System.Data.ParameterDirection parameterDirection = ParameterDirection.Input;

            // Create a new instance of the oracle parameter collection.
            string functionParameterNames = string.Empty;
            string functionValueNames = string.Empty;
            List<OracleClient.OracleParameter> oracleParameters = new List<OracleClient.OracleParameter>();
            DataTypeConversion dataTypeConversion = new DataTypeConversion(ConnectionContext.ConnectionDataType.OracleDataType);

            // For each parameter within the method.
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                // For each attribute for the parameter.
                foreach (object attribute in parameter.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // function parameter column attribute.
                    if (attribute is Nequeo.Data.Custom.FunctionParameterAttribute)
                    {
                        // Increment the parameter count.
                        i++;

                        // Cast the current attribute.
                        Nequeo.Data.Custom.FunctionParameterAttribute att =
                            (Nequeo.Data.Custom.FunctionParameterAttribute)attribute;

                        dbType = att.DbType;
                        length = att.Length;
                        parameterName = att.Name;
                        isNullable = att.IsNullable;
                        parameterDirection = att.ParameterDirection;

                        // Add each parameter to the collection.
                        oracleParameters.Add(new OracleClient.OracleParameter(
                            parameterName, Nequeo.Data.Oracle.ClientDataType.GetOracleClientDbType(dbType), Convert.ToInt32(length),
                            parameterDirection, isNullable, ((Byte)(0)), ((Byte)(0)), "", System.Data.DataRowVersion.Current, parameters[i]));

                        // If the parameter is an input type
                        // then add the parameter to the list.
                        if (parameterDirection == ParameterDirection.Input || parameterDirection == ParameterDirection.InputOutput)
                        {
                            functionParameterNames += parameterName + ", ";
                            functionValueNames += dataTypeConversion.GetSqlStringValue(parameters[i].GetType(), parameters[i]) + ", ";
                        }
                    }
                }
            }

            // Get the parameters for the function
            functionParameters = functionParameterNames.TrimEnd(' ', ',');
            functionValues = functionValueNames.TrimEnd(' ', ',');
            // Return the oracle parameters.
            return oracleParameters.ToArray();
        }

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
        public DbCommand ExecuteQuery<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            dataTable = new TypeDataTable();

            // Initial connection objects.
            DbCommand dbCommand = null;
            OracleClient.OracleConnection orlConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (orlConnection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    orlConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new OracleClient.OracleCommand(DataTypeConversion.GetSqlConversionDataTypeNoContainer(
                        ConnectionContext.ConnectionDataType.OracleDataType, queryText), orlConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            dbCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = dbCommand.ExecuteReader())
                    {
                        System.Data.DataSet localDataSet = new System.Data.DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Close the database connection.
                    orlConnection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return dbCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (orlConnection != null)
                    orlConnection.Close();
            }
        }

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
        public DbCommand ExecuteQuery(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params DbParameter[] values)
        {
            // Initial connection objects.
            DbCommand dbCommand = null;
            OracleClient.OracleConnection orlConnection = null;
            IDataReader dataReader = null;


            try
            {
                // Create a new connection.
                using (orlConnection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    orlConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new OracleClient.OracleCommand(DataTypeConversion.GetSqlConversionDataTypeNoContainer(
                        ConnectionContext.ConnectionDataType.OracleDataType, queryText), orlConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            dbCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = dbCommand.ExecuteReader())
                    {
                        // Get the schema from the data because the
                        // table has not predefined schema
                        if (getSchemaTable)
                        {
                            // Load the table after the schema is
                            // returned.
                            dataTable = dataReader.GetSchemaTable();
                            dataTable = new DataTable();
                            System.Data.DataSet localDataSet = new System.Data.DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            // Load the data into a table schema.
                            System.Data.DataSet localDataSet = new System.Data.DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    orlConnection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return dbCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (orlConnection != null)
                    orlConnection.Close();
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="dbCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        public Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            dbCommand = null;
            Int32 returnValue = -1;
            
            OracleClient.OracleConnection orlConnection = null;
            OracleClient.OracleTransaction orlTransaction = null;

            try
            {
                // Create a new connection.
                using (orlConnection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    orlConnection.Open();

                    // Start a new transaction.
                    orlTransaction = orlConnection.BeginTransaction();

                    // Create the command and assign any parameters.
                    dbCommand = new OracleClient.OracleCommand(DataTypeConversion.GetSqlConversionDataTypeNoContainer(
                        ConnectionContext.ConnectionDataType.OracleDataType, commandText), orlConnection);
                    dbCommand.CommandType = commandType;
                    dbCommand.Transaction = orlTransaction;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            dbCommand.Parameters.Add(sqlParameter);

                    // Execute the command.
                    returnValue = dbCommand.ExecuteNonQuery();

                    // Commit the transaction.
                    orlTransaction.Commit();

                    // Close the database connection.
                    orlConnection.Close();
                }

                // Return true.
                return returnValue;
            }
            catch (Exception ex)
            {
                try
                {
                    // Attempt to roll back the transaction.
                    if (orlTransaction != null)
                        orlTransaction.Rollback();
                }
                catch { }

                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (orlConnection != null)
                    orlConnection.Close();
            }
        }

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
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            DbCommand dbCommand = null;
            OracleClient.OracleConnection orlConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (orlConnection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    orlConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new OracleClient.OracleCommand(DataTypeConversion.GetSqlConversionDataTypeNoContainer(
                        ConnectionContext.ConnectionDataType.OracleDataType, queryText), orlConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            dbCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = dbCommand.ExecuteReader())
                    {
                        dataSet = new System.Data.DataSet();
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Close the database connection.
                    orlConnection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return dbCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (orlConnection != null)
                    orlConnection.Close();
            }
        }

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
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            // Initial connection objects.
            DbCommand dbCommand = null;
            OracleClient.OracleConnection orlConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (orlConnection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    orlConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new OracleClient.OracleCommand(DataTypeConversion.GetSqlConversionDataTypeNoContainer(
                        ConnectionContext.ConnectionDataType.OracleDataType, queryText), orlConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            dbCommand.Parameters.Add(sqlParameter);

                    // Load the data into the table.
                    using (dataReader = dbCommand.ExecuteReader())
                    {
                        dataSet = new System.Data.DataSet();
                        dataSet.Tables.AddRange(tables);
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Close the database connection.
                    orlConnection.Close();
                }

                // Return the sql command, including
                // any parameters that have been
                // marked as output direction.
                return dbCommand;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (orlConnection != null)
                    orlConnection.Close();
            }
        }

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
        public int ExecuteCommand(ref DbCommand dbCommand, string commandText, CommandType commandType,
            string connectionString, ConnectionContext.ConnectionDataType connectionDataType, params DbParameter[] values)
        {
            return ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
        }

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
        public DbCommand ExecuteQuery(ref DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, string connectionString, ConnectionContext.ConnectionDataType connectionDataType, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
        }

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
        public DbCommand ExecuteQuery(ref DataSet dataSet, string[] tables, string queryText, CommandType commandType,
            string connectionString, ConnectionContext.ConnectionDataType connectionDataType, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref DataTable dataTable, string queryText, CommandType commandType, string connectionString,
            bool getSchemaTable, ConnectionContext.ConnectionDataType connectionDataType, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
        }

        /// <summary>
        /// Gets the list of postgresql parameters.
        /// </summary>
        /// <param name="functionParameters">The function parameters created.</param>
        /// <param name="functionValues">The function values created.</param>
        /// <param name="connectionDataType">The connection data type.</param>
        /// <param name="methodInfo">The current method information.</param>
        /// <param name="parameters">The parameter collection.</param>
        /// <returns>The list of parameters.</returns>
        public DbParameter[] GetParameters(ref string functionParameters, ref string functionValues,
            ConnectionContext.ConnectionDataType connectionDataType, MethodInfo methodInfo, params object[] parameters)
        {
            return GetParameters(ref functionParameters, ref functionValues, methodInfo, parameters);
        }
        #endregion
    }
}
