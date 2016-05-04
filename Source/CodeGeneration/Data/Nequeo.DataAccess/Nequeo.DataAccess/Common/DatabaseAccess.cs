using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;

using PostgreSqlClient = Npgsql;
using OracleClient = Oracle.ManagedDataAccess.Client;
using MySqlClient = MySql.Data.MySqlClient;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// Database access class.
    /// </summary>
    public class DatabaseAccess
    {
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
        public SqlCommand ExecuteQuery<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params SqlParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            dataTable = new TypeDataTable();

            // Initial connection objects.
            SqlCommand sqlCommand = null;
            SqlConnection connection = null;
            IDataReader dataReader = null;
            SqlTransaction sqlTransaction = null;

            try
            {
                // Create a new connection.
                using (connection = new SqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new SqlCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (SqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction("DataExecuteCommand");
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

                if (connection != null)
                    connection.Close();
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        public Int32 ExecuteCommand(ref SqlCommand sqlCommand, string commandText,
            CommandType commandType, string connectionString, params SqlParameter[] values)
        {
            // Initial connection objects.
            sqlCommand = null;
            Int32 returnValue = -1;
            SqlConnection connection = null;
            SqlTransaction sqlTransaction = null;

            try
            {
                // Create a new connection.
                using (connection = new SqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new SqlCommand(commandText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (SqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction("DataExecuteCommand");
                    sqlCommand.Transaction = sqlTransaction;

                    // Execute the command.
                    returnValue = sqlCommand.ExecuteNonQuery();

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

                    // Close the database connection.
                    connection.Close();
                }

                // Return true.
                return returnValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public SqlCommand ExecuteQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params SqlParameter[] values)
        {
            // Initial connection objects.
            SqlCommand sqlCommand = null;
            SqlConnection connection = null;
            SqlTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new SqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new SqlCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (SqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction("DataExecuteCommand");
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

                if (connection != null)
                    connection.Close();
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public SqlCommand ExecuteQuerySchema(ref DataSet dataSet, string queryText,
            CommandType commandType, string connectionString, params SqlParameter[] values)
        {
            // Initial connection objects.
            SqlCommand sqlCommand = null;
            SqlConnection connection = null;
            SqlTransaction sqlTransaction = null;
            IDataReader dataReader = null;
            string[] tables = new string[] { 
                "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                "Eleven", "Twelve", "Thirteen", "FourTeen", "Fifteen", "SixTeen", "SevenTeen", 
                "Eighteen", "Nineteen", "Twenty"};

            try
            {
                // Create a new connection.
                using (connection = new SqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new SqlCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (SqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction("DataExecuteCommand");
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataSet = new DataSet();
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public OleDbCommand ExecuteOleDbQuerySchema(ref DataSet dataSet, string queryText,
            CommandType commandType, string connectionString, params OleDbParameter[] values)
        {
            // Initial connection objects.
            OleDbCommand sqlCommand = null;
            OleDbConnection connection = null;
            OleDbTransaction sqlTransaction = null;
            IDataReader dataReader = null;
            string[] tables = new string[] { 
                "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                "Eleven", "Twelve", "Thirteen", "FourTeen", "Fifteen", "SixTeen", "SevenTeen", 
                "Eighteen", "Nineteen", "Twenty"};

            try
            {
                // Create a new connection.
                using (connection = new OleDbConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new OleDbCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OleDbParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataSet = new DataSet();
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public OracleClient.OracleCommand ExecuteOracleClientQuerySchema(ref DataSet dataSet, string queryText,
            CommandType commandType, string connectionString, params OracleClient.OracleParameter[] values)
        {
            // Initial connection objects.
            OracleClient.OracleCommand sqlCommand = null;
            OracleClient.OracleConnection connection = null;
            OracleClient.OracleTransaction sqlTransaction = null;
            IDataReader dataReader = null;
            string[] tables = new string[] { 
                "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                "Eleven", "Twelve", "Thirteen", "FourTeen", "Fifteen", "SixTeen", "SevenTeen", 
                "Eighteen", "Nineteen", "Twenty"};

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new OracleClient.OracleCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    //sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataSet = new DataSet();
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public MySqlClient.MySqlCommand ExecuteMySqlClientQuerySchema(ref DataSet dataSet, string queryText,
            CommandType commandType, string connectionString, params MySqlClient.MySqlParameter[] values)
        {
            // Initial connection objects.
            MySqlClient.MySqlCommand sqlCommand = null;
            MySqlClient.MySqlConnection connection = null;
            MySqlClient.MySqlTransaction sqlTransaction = null;
            IDataReader dataReader = null;
            string[] tables = new string[] { 
                "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                "Eleven", "Twelve", "Thirteen", "FourTeen", "Fifteen", "SixTeen", "SevenTeen", 
                "Eighteen", "Nineteen", "Twenty"};

            try
            {
                // Create a new connection.
                using (connection = new MySqlClient.MySqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new MySqlClient.MySqlCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (MySqlClient.MySqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    //sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataSet = new DataSet();
                        dataSet.EnforceConstraints = false;
                        dataSet.Load(dataReader, LoadOption.OverwriteChanges, tables);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public SqlCommand ExecuteFunctionTableQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params SqlParameter[] values)
        {
            // Initial connection objects.
            SqlCommand sqlCommand = null;
            SqlConnection connection = null;
            SqlTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new SqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new SqlCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (SqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction("DataExecuteCommand");
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public OracleClient.OracleCommand ExecuteOracleClientQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params OracleClient.OracleParameter[] values)
        {
            // Initial connection objects.
            OracleClient.OracleCommand sqlCommand = null;
            OracleClient.OracleConnection connection = null;
            OracleClient.OracleTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new OracleClient.OracleCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    //sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public Npgsql.NpgsqlCommand ExecutePostgreSqlFunctionQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params Npgsql.NpgsqlParameter[] values)
        {
            // Initial connection objects.
            Npgsql.NpgsqlCommand sqlCommand = null;
            Npgsql.NpgsqlConnection connection = null;
            Npgsql.NpgsqlTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new Npgsql.NpgsqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new Npgsql.NpgsqlCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (Npgsql.NpgsqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public MySqlClient.MySqlCommand ExecuteMySqlFunctionQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params MySqlClient.MySqlParameter[] values)
        {
            // Initial connection objects.
            MySqlClient.MySqlCommand sqlCommand = null;
            MySqlClient.MySqlConnection connection = null;
            MySqlClient.MySqlTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new MySqlClient.MySqlConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new MySqlClient.MySqlCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (MySqlClient.MySqlParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public OracleClient.OracleCommand ExecuteOracleClientFunctionQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params OracleClient.OracleParameter[] values)
        {
            // Initial connection objects.
            OracleClient.OracleCommand sqlCommand = null;
            OracleClient.OracleConnection connection = null;
            OracleClient.OracleTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new OracleClient.OracleConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new OracleClient.OracleCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OracleClient.OracleParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    //sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public OleDbCommand ExecuteOleDbQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params OleDbParameter[] values)
        {
            // Initial connection objects.
            OleDbCommand sqlCommand = null;
            OleDbConnection connection = null;
            OleDbTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new OleDbConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new OleDbCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OleDbParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public OdbcCommand ExecuteOdbcQuerySchema(ref DataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params OdbcParameter[] values)
        {
            // Initial connection objects.
            OdbcCommand sqlCommand = null;
            OdbcConnection connection = null;
            OdbcTransaction sqlTransaction = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (connection = new OdbcConnection(connectionString))
                {
                    // Create the command and assign any parameters.
                    sqlCommand = new OdbcCommand(queryText, connection);
                    sqlCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OdbcParameter sqlParameter in values)
                            sqlCommand.Parameters.Add(sqlParameter);

                    // Open the connection.
                    connection.Open();

                    // Start a new transaction.
                    sqlTransaction = connection.BeginTransaction();
                    sqlCommand.Transaction = sqlTransaction;

                    // Load the data into the table.
                    using (dataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Load the table after the schema is
                        // returned.
                        dataTable = dataReader.GetSchemaTable();
                        dataTable = new DataTable("TableName");
                        DataSet localDataSet = new DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(dataTable);
                        dataTable.Load(dataReader);
                        dataReader.Close();
                    }

                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();

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
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                try
                {
                    // Attempt to roll back the transaction.
                    if (sqlTransaction != null)
                        sqlTransaction.Rollback();
                }
                catch { }

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
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public SqlCommand GetSqlData(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params SqlParameter[] values)
        {
            // Initial connection objects.
            SqlCommand dbCommand = null;
            SqlConnection sqlConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    // Open the connection.
                    sqlConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new SqlCommand(queryText, sqlConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (SqlParameter sqlParameter in values)
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
                            dataTable = new DataTable("TableName");
                            DataSet localDataSet = new DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    sqlConnection.Close();
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

                if (sqlConnection != null)
                    sqlConnection.Close();
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
        public OracleClient.OracleCommand GetOracleClientData(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params OracleClient.OracleParameter[] values)
        {
            // Initial connection objects.
            OracleClient.OracleCommand dbCommand = null;
            OracleClient.OracleConnection oracleConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (oracleConnection = new OracleClient.OracleConnection(connectionString))
                {
                    // Open the connection.
                    oracleConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new OracleClient.OracleCommand(queryText, oracleConnection);
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
                            dataTable = new DataTable("TableName");
                            DataSet localDataSet = new DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    oracleConnection.Close();
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

                if (oracleConnection != null)
                    oracleConnection.Close();
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
        public PostgreSqlClient.NpgsqlCommand GetPostgreSqlClientData(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params PostgreSqlClient.NpgsqlParameter[] values)
        {
            // Initial connection objects.
            PostgreSqlClient.NpgsqlCommand dbCommand = null;
            PostgreSqlClient.NpgsqlConnection pgConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (pgConnection = new PostgreSqlClient.NpgsqlConnection(connectionString))
                {
                    // Open the connection.
                    pgConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new PostgreSqlClient.NpgsqlCommand(queryText, pgConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (PostgreSqlClient.NpgsqlParameter sqlParameter in values)
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
                            dataTable = new DataTable("TableName");
                            DataSet localDataSet = new DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    pgConnection.Close();
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

                if (pgConnection != null)
                    pgConnection.Close();
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
        public MySqlClient.MySqlCommand GetMySqlClientData(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params MySqlClient.MySqlParameter[] values)
        {
            // Initial connection objects.
            MySqlClient.MySqlCommand dbCommand = null;
            MySqlClient.MySqlConnection pgConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (pgConnection = new MySqlClient.MySqlConnection(connectionString))
                {
                    // Open the connection.
                    pgConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new MySqlClient.MySqlCommand(queryText, pgConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (MySqlClient.MySqlParameter sqlParameter in values)
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
                            dataTable = new DataTable("TableName");
                            DataSet localDataSet = new DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    pgConnection.Close();
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

                if (pgConnection != null)
                    pgConnection.Close();
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
        public OleDbCommand GetOleDbData(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params OleDbParameter[] values)
        {
            // Initial connection objects.
            OleDbCommand dbCommand = null;
            OleDbConnection oledbConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (oledbConnection = new OleDbConnection(connectionString))
                {
                    // Open the connection.
                    oledbConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new OleDbCommand(queryText, oledbConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OleDbParameter sqlParameter in values)
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
                            dataTable = new DataTable("TableName");
                            DataSet localDataSet = new DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    oledbConnection.Close();
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

                if (oledbConnection != null)
                    oledbConnection.Close();
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
        public OdbcCommand GetOdbcData(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params OdbcParameter[] values)
        {
            // Initial connection objects.
            OdbcCommand dbCommand = null;
            OdbcConnection oledbConnection = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (oledbConnection = new OdbcConnection(connectionString))
                {
                    // Open the connection.
                    oledbConnection.Open();

                    // Create the command and assign any parameters.
                    dbCommand = new OdbcCommand(queryText, oledbConnection);
                    dbCommand.CommandType = commandType;

                    if (values != null)
                        foreach (OdbcParameter sqlParameter in values)
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
                            dataTable = new DataTable("TableName");
                            DataSet localDataSet = new DataSet();
                            localDataSet.EnforceConstraints = false;
                            localDataSet.Tables.Add(dataTable);
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            // Load the data into a table schema.
                            dataTable.Load(dataReader);
                        }

                        dataReader.Close();
                    }

                    // Close the database connection.
                    oledbConnection.Close();
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

                if (oledbConnection != null)
                    oledbConnection.Close();
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="schemaGuid">The schema to include.</param>
        /// <param name="restrictions">The restictions to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public bool GetOleDbData(ref DataTable dataTable,
            string connectionString, Guid schemaGuid, Object[] restrictions)
        {
            // Initial connection objects.
            OleDbConnection oledbConnection = null;

            try
            {
                // Create a new connection.
                using (oledbConnection = new OleDbConnection(connectionString))
                {
                    // Open the connection.
                    oledbConnection.Open();

                    dataTable = new DataTable("TableName");
                    dataTable = oledbConnection.GetOleDbSchemaTable(schemaGuid, restrictions);

                    // Close the database connection.
                    oledbConnection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (oledbConnection != null)
                    oledbConnection.Close();
            }
        }

        /// <summary>
        /// The database table columns.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="owner">The schema (owner) of the object.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of table column results.</returns>
        public List<TableColumnsResult> GetDatabaseTableColumns(string connectionString, string dataBase, string tableName,
            string owner, Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetDatabaseTableColumnsSQL(dataBase, tableName, owner);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TableColumnsResult));
                    return ((List<TableColumnsResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetDatabaseTableColumnsOracle(dataBaseConnect, dataBaseOwner, tableName);
                    OracleClient.OracleCommand retClientOracle = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsClientOracle = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TableColumnsResult));
                    return ((List<TableColumnsResult>)(resultsClientOracle));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetDatabaseTableColumnsPostgreSql(dataBaseConnect, dataBaseOwner, tableName);
                    PostgreSqlClient.NpgsqlCommand retClientPg = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsClientPg = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TableColumnsResult));
                    return ((List<TableColumnsResult>)(resultsClientPg));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetDatabaseTableColumnsMySql(dataBaseConnect, dataBaseOwner, tableName);
                    MySqlClient.MySqlCommand retClientMy = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsClientMy = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TableColumnsResult));
                    return ((List<TableColumnsResult>)(resultsClientMy));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Columns,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), tableName });
                        DataTable dataTableTranslated = typeConversion.TranslateTableColumns(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(TableColumnsResult));
                        return ((List<TableColumnsResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The database procedure columns.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="owner">The schema (owner) of the object.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of table column results.</returns>
        public List<ProcedureColumnsResult> GetDatabaseProcedureColumns(string connectionString, string dataBase, string tableName,
            string owner, Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetDatabaseProcedureColumnsSQL(dataBase, tableName, owner);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ProcedureColumnsResult));
                    return ((List<ProcedureColumnsResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetDatabaseProcedureColumnsOracle(dataBaseConnect, dataBaseOwner, tableName);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ProcedureColumnsResult));
                    return ((List<ProcedureColumnsResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    return null;

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetDatabaseProcedureColumnsMySql(dataBaseConnect, dataBaseOwner, tableName);
                    MySqlClient.MySqlCommand retMyClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMyClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ProcedureColumnsResult));
                    return ((List<ProcedureColumnsResult>)(resultsMyClient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Procedure_Parameters,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), tableName });
                        DataTable dataTableTranslated = typeConversion.TranslateProcedureColumns(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(ProcedureColumnsResult));
                        return ((List<ProcedureColumnsResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The database function columns.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="owner">The schema (owner) of the object.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of table column results.</returns>
        public List<FunctionColumnsResult> GetDatabaseFunctionColumns(string connectionString, string dataBase, string tableName,
            string owner, Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, 
            string dataBaseOwner, string overloadName, int functionType)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetDatabaseFunctionColumnsSQL(dataBase, tableName, owner);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));
                    return ((List<FunctionColumnsResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetDatabaseFunctionColumnsOracle(dataBaseConnect, dataBaseOwner, tableName);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));
                    return ((List<FunctionColumnsResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgQuery = string.Empty;
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = null;
                    object resultsPostgreSql = null;

                    string pgQueryReturn = string.Empty;
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClientReturn = null;
                    object resultsPostgreSqlReturn = null;

                    switch (functionType)
                    {
                        case 0:
                            // Table
                            pgQuery = GetDatabaseFunctionTableColumnsPostgreSql(dataBaseConnect, dataBaseOwner, tableName);
                            retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgQuery, CommandType.Text, connectionString, true, null);
                            resultsPostgreSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));
                            if (String.IsNullOrEmpty(overloadName))
                                return ((List<FunctionColumnsResult>)(resultsPostgreSql));
                            else
                                return ((List<FunctionColumnsResult>)(resultsPostgreSql)).
                                    Where(u => u.OverloadName == overloadName).ToList<FunctionColumnsResult>();
                        case 1:
                            // Scalar
                            pgQuery = GetDatabaseFunctionScalarColumnsPostgreSql(dataBaseConnect, dataBaseOwner, tableName);
                            retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgQuery, CommandType.Text, connectionString, true, null);
                            resultsPostgreSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));

                            pgQueryReturn = GetDatabaseFunctionScalarReturnColumnsPostgreSql(dataBaseConnect, dataBaseOwner, tableName);
                            retPostgreSqlClientReturn = GetPostgreSqlClientData(ref dataTable, pgQueryReturn, CommandType.Text, connectionString, true, null);
                            resultsPostgreSqlReturn = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));

                            IEnumerable<FunctionColumnsResult> combine =
                                ((List<FunctionColumnsResult>)(resultsPostgreSql)).Concat(((List<FunctionColumnsResult>)(resultsPostgreSqlReturn)));
                            if (String.IsNullOrEmpty(overloadName))
                                return combine.ToList<FunctionColumnsResult>();
                            else
                                return combine.Where(u => u.OverloadName == overloadName).ToList<FunctionColumnsResult>();
                    }
                    return null;

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myQuery = string.Empty;
                    MySqlClient.MySqlCommand retMySqlClient = null;
                    object resultsMySql = null;

                    string myQueryReturn = string.Empty;
                    MySqlClient.MySqlCommand retMySqlClientReturn = null;
                    object resultsMySqlReturn = null;

                    switch (functionType)
                    {
                        case 0:
                            // Table
                            myQuery = GetDatabaseFunctionTableColumnsMySql(dataBaseConnect, dataBaseOwner, tableName);
                            retMySqlClient = GetMySqlClientData(ref dataTable, myQuery, CommandType.Text, connectionString, true, null);
                            resultsMySql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));
                            if (String.IsNullOrEmpty(overloadName))
                                return ((List<FunctionColumnsResult>)(resultsMySql));
                            else
                                return ((List<FunctionColumnsResult>)(resultsMySql)).
                                    Where(u => u.OverloadName == overloadName).ToList<FunctionColumnsResult>();
                        case 1:
                            // Scalar
                            myQuery = GetDatabaseFunctionScalarColumnsMySql(dataBaseConnect, dataBaseOwner, tableName);
                            retMySqlClient = GetMySqlClientData(ref dataTable, myQuery, CommandType.Text, connectionString, true, null);
                            resultsMySql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));

                            myQueryReturn = GetDatabaseFunctionScalarReturnColumnsMySql(dataBaseConnect, dataBaseOwner, tableName);
                            retMySqlClientReturn = GetMySqlClientData(ref dataTable, myQueryReturn, CommandType.Text, connectionString, true, null);
                            resultsMySqlReturn = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionColumnsResult));

                            IEnumerable<FunctionColumnsResult> combine =
                                ((List<FunctionColumnsResult>)(resultsMySql)).Concat(((List<FunctionColumnsResult>)(resultsMySqlReturn)));
                            if (String.IsNullOrEmpty(overloadName))
                                return combine.ToList<FunctionColumnsResult>();
                            else
                                return combine.Where(u => u.OverloadName == overloadName).ToList<FunctionColumnsResult>();
                    }
                    return null;
                    
                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Procedure_Parameters,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), tableName });
                        DataTable dataTableTranslated = typeConversion.TranslateFunctionColumns(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(FunctionColumnsResult));
                        return ((List<FunctionColumnsResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The collection of tables.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of table results.</returns>
        public List<TablesResult> GetTables(string connectionString, string dataBase,
            Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetTablesSQL(dataBase);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetTablesOracle(dataBaseConnect, dataBaseOwner);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetTablesPostgreSql(dataBaseConnect, dataBaseOwner);
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsPostgreSqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsPostgreSqlClient));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetTablesMySql(dataBaseConnect, dataBaseOwner);
                    MySqlClient.MySqlCommand retMySqlClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsMySqlClient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Tables,
                            new Object[] { null, (String.IsNullOrEmpty(dataBase) ? null : dataBase), (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), "TABLE" });
                        DataTable dataTableTranslated = typeConversion.TranslateTable(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(TablesResult));
                        return ((List<TablesResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The collection of procedures.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of table results.</returns>
        public List<ProceduresResult> GetProcedures(string connectionString, string dataBase,
            Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetProcedureSQL(dataBase);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ProceduresResult));
                    return ((List<ProceduresResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Procedures,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), null });
                        DataTable dataTableTranslated = typeConversion.TranslateProcedures(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(ProceduresResult));
                        return ((List<ProceduresResult>)(resultsOleDb));
                    }
                    catch { return null; }

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetProcedureOracle(dataBaseConnect, dataBaseOwner);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ProceduresResult));
                    List<ProceduresResult> proceduresClient = ((List<ProceduresResult>)(resultsOracleClient));

                    foreach (ProceduresResult procedure in proceduresClient)
                    {
                        string oracleXmlObject = this.GetXmlProcedureOracle(dataBaseConnect, dataBaseOwner, procedure.ProcedureName);
                        OracleClient.OracleCommand retXmlObjectOracle = GetOracleClientData(ref dataTable, oracleXmlObject, CommandType.Text, connectionString, true, null);
                        object resultsXmlObjectOracle = typeConversion.ListGenericTypeTranslator(dataTable, typeof(XmlObjectResult));
                        List<XmlObjectResult> functionsXmlObjects = ((List<XmlObjectResult>)(resultsXmlObjectOracle));

                        try
                        {
                            // Creates an instance of the XmlSerialiser class;
                            // specifies the type of object to be deserialised.
                            XmlSerializer deserialiser = new XmlSerializer(typeof(XmlObjectContainer));

                            // Create a new string writer, this
                            // is where the generated code will be
                            // written to.
                            TextReader stringReader = new StringReader(functionsXmlObjects[0].ObjectXml);

                            // Uses the Deserialise method to restore the object's state 
                            // with data from the XML document.
                            XmlObjectContainer data = (XmlObjectContainer)deserialiser.Deserialize(stringReader);

                            foreach (SOURCE_LINES_ITEM SourceLinesItem in data.RowEntity.ProcedureEntity.SourceLines.SourceLinesItem)
                            {
                                if (SourceLinesItem.Source.ToUpper().Contains("PROCEDURE " + procedure.ProcedureName.ToUpper()))
                                {
                                    try
                                    {
                                        int start = SourceLinesItem.Source.ToUpper().IndexOf(procedure.ProcedureName.ToUpper());
                                        int end = procedure.ProcedureName.ToUpper().Length;
                                        procedure.FunctionRealName = SourceLinesItem.Source.Substring(start, end);
                                    }
                                    catch { }
                                    break;
                                }
                            }
                        }
                        catch { }
                    }

                    oracleClientQuery = GetPackagesOracle(dataBaseConnect, dataBaseOwner);
                    retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(PackagesResult));
                    List<PackagesResult> packagesClient = ((List<PackagesResult>)(resultsOracleClient));

                    foreach (PackagesResult package in packagesClient)
                    {
                        oracleClientQuery = GetPackageProcedureOracle(dataBaseConnect, dataBaseOwner, package.PackageName);
                        retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                        resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ProceduresResult));
                        List<ProceduresResult> packageProcedures = ((List<ProceduresResult>)(resultsOracleClient));

                        string oracleXmlObject = GetXmlPackageSpecificationOracle(dataBaseConnect, dataBaseOwner, package.PackageName);
                        OracleClient.OracleCommand retXmlObjectOracle = GetOracleClientData(ref dataTable, oracleXmlObject, CommandType.Text, connectionString, true, null);
                        object resultsXmlObjectOracle = typeConversion.ListGenericTypeTranslator(dataTable, typeof(XmlObjectResult));
                        List<XmlObjectResult> proceduresXmlObjects = ((List<XmlObjectResult>)(resultsXmlObjectOracle));

                        try
                        {
                            // Creates an instance of the XmlSerialiser class;
                            // specifies the type of object to be deserialised.
                            XmlSerializer deserialiser = new XmlSerializer(typeof(XmlObjectContainer));

                            // Create a new string writer, this
                            // is where the generated code will be
                            // written to.
                            TextReader stringReader = new StringReader(proceduresXmlObjects[0].ObjectXml);

                            // Uses the Deserialise method to restore the object's state 
                            // with data from the XML document.
                            XmlObjectContainer data = (XmlObjectContainer)deserialiser.Deserialize(stringReader);

                            foreach (ProceduresResult procedure in packageProcedures)
                            {
                                foreach (SOURCE_LINES_ITEM SourceLinesItem in data.RowEntity.PackageEntity.SourceLines.SourceLinesItem)
                                {
                                    if (SourceLinesItem.Source.ToUpper().Contains("PROCEDURE " + procedure.ProcedureName.ToUpper()))
                                    {
                                        ProceduresResult newProcedure = new ProceduresResult();
                                        newProcedure.PackageName = procedure.PackageName;
                                        newProcedure.ProcedureName = procedure.ProcedureName;
                                        newProcedure.ProcedureOwner = procedure.ProcedureOwner;

                                        try
                                        {
                                            int start = SourceLinesItem.Source.ToUpper().IndexOf(procedure.ProcedureName.ToUpper());
                                            int end = procedure.ProcedureName.ToUpper().Length;
                                            newProcedure.FunctionRealName = SourceLinesItem.Source.Substring(start, end);
                                        }
                                        catch { }

                                        proceduresClient.Add(newProcedure);
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    return (proceduresClient);

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    return null;

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myQuery = GetProcedureMySql(dataBaseConnect, dataBaseOwner);
                    MySqlClient.MySqlCommand retMySql = GetMySqlClientData(ref dataTable, myQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ProceduresResult));
                    return ((List<ProceduresResult>)(resultsMySql));

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The collection of functions.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of table results.</returns>
        public List<FunctionResult> GetFunctions(string connectionString, string dataBase,
            Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect,
            string dataBaseOwner, int functionType)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetFunctionSQL(dataBase);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionResult));
                    return ((List<FunctionResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Procedures,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), null });
                        DataTable dataTableTranslated = typeConversion.TranslateFunctions(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(FunctionResult));
                        return ((List<FunctionResult>)(resultsOleDb));
                    }
                    catch { return null; }

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetFunctionOracle(dataBaseConnect, dataBaseOwner);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionResult));
                    List<FunctionResult> functionsClient = ((List<FunctionResult>)(resultsOracleClient));

                    foreach (FunctionResult function in functionsClient)
                    {
                        string oracleXmlObject = this.GetXmlFunctionOracle(dataBaseConnect, dataBaseOwner, function.FunctionName);
                        OracleClient.OracleCommand retXmlObjectOracle = GetOracleClientData(ref dataTable, oracleXmlObject, CommandType.Text, connectionString, true, null);
                        object resultsXmlObjectOracle = typeConversion.ListGenericTypeTranslator(dataTable, typeof(XmlObjectResult));
                        List<XmlObjectResult> functionsXmlObjects = ((List<XmlObjectResult>)(resultsXmlObjectOracle));

                        try
                        {
                            // Creates an instance of the XmlSerialiser class;
                            // specifies the type of object to be deserialised.
                            XmlSerializer deserialiser = new XmlSerializer(typeof(XmlObjectContainer));

                            // Create a new string writer, this
                            // is where the generated code will be
                            // written to.
                            TextReader stringReader = new StringReader(functionsXmlObjects[0].ObjectXml);

                            // Uses the Deserialise method to restore the object's state 
                            // with data from the XML document.
                            XmlObjectContainer data = (XmlObjectContainer)deserialiser.Deserialize(stringReader);

                            foreach (SOURCE_LINES_ITEM SourceLinesItem in data.RowEntity.FunctionEntity.SourceLines.SourceLinesItem)
                            {
                                if (SourceLinesItem.Source.ToUpper().Contains("FUNCTION " + function.FunctionName.ToUpper()))
                                {
                                    try
                                    {
                                        int start = SourceLinesItem.Source.ToUpper().IndexOf(function.FunctionName.ToUpper());
                                        int end = function.FunctionName.ToUpper().Length;
                                        function.FunctionRealName = SourceLinesItem.Source.Substring(start, end);
                                    }
                                    catch { }
                                    break;
                                }
                            }
                        }
                        catch { }
                    }

                    oracleClientQuery = GetPackagesOracle(dataBaseConnect, dataBaseOwner);
                    retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(PackagesResult));
                    List<PackagesResult> packagesClient = ((List<PackagesResult>)(resultsOracleClient));

                    foreach (PackagesResult package in packagesClient)
                    {
                        oracleClientQuery = GetPackageFunctionOracle(dataBaseConnect, dataBaseOwner, package.PackageName);
                        retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                        resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionResult));
                        List<FunctionResult> packageFunctions = ((List<FunctionResult>)(resultsOracleClient));

                        string oracleXmlObject = GetXmlPackageSpecificationOracle(dataBaseConnect, dataBaseOwner, package.PackageName);
                        OracleClient.OracleCommand retXmlObjectOracle = GetOracleClientData(ref dataTable, oracleXmlObject, CommandType.Text, connectionString, true, null);
                        object resultsXmlObjectOracle = typeConversion.ListGenericTypeTranslator(dataTable, typeof(XmlObjectResult));
                        List<XmlObjectResult> functionsXmlObjects = ((List<XmlObjectResult>)(resultsXmlObjectOracle));

                        try
                        {
                            // Creates an instance of the XmlSerialiser class;
                            // specifies the type of object to be deserialised.
                            XmlSerializer deserialiser = new XmlSerializer(typeof(XmlObjectContainer));

                            // Create a new string writer, this
                            // is where the generated code will be
                            // written to.
                            TextReader stringReader = new StringReader(functionsXmlObjects[0].ObjectXml);

                            // Uses the Deserialise method to restore the object's state 
                            // with data from the XML document.
                            XmlObjectContainer data = (XmlObjectContainer)deserialiser.Deserialize(stringReader);

                            foreach (FunctionResult function in packageFunctions)
                            {
                                foreach (SOURCE_LINES_ITEM SourceLinesItem in data.RowEntity.PackageEntity.SourceLines.SourceLinesItem)
                                {
                                    if (SourceLinesItem.Source.ToUpper().Contains("FUNCTION " + function.FunctionName.ToUpper()))
                                    {
                                        FunctionResult newFunction = new FunctionResult();
                                        newFunction.PackageName = function.PackageName;
                                        newFunction.FunctionName = function.FunctionName;
                                        newFunction.FunctionOwner = function.FunctionOwner;

                                        try
                                        {
                                            int start = SourceLinesItem.Source.ToUpper().IndexOf(function.FunctionName.ToUpper());
                                            int end = function.FunctionName.ToUpper().Length;
                                            newFunction.FunctionRealName = SourceLinesItem.Source.Substring(start, end);
                                        }
                                        catch { }

                                        functionsClient.Add(newFunction);
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    return (functionsClient);

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgQuery = string.Empty;
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = null;
                    object resultsPostgreSql = null;

                    switch (functionType)
                    {
                        case 0:
                            // Table
                            pgQuery = GetFunctionTablePostgreSql(dataBaseConnect, dataBaseOwner);
                            retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgQuery, CommandType.Text, connectionString, true, null);
                            resultsPostgreSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionResult));
                            return ((List<FunctionResult>)(resultsPostgreSql));
                        case 1:
                            // Scalar
                            pgQuery = GetFunctionScalarPostgreSql(dataBaseConnect, dataBaseOwner);
                            retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgQuery, CommandType.Text, connectionString, true, null);
                            resultsPostgreSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionResult));
                            return ((List<FunctionResult>)(resultsPostgreSql));
                    }
                    return null;

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myQuery = string.Empty;
                    MySqlClient.MySqlCommand retMySqlClient = null;
                    object resultsMySql = null;

                    switch (functionType)
                    {
                        case 0:
                            // Table
                            myQuery = GetFunctionTableMySql(dataBaseConnect, dataBaseOwner);
                            retMySqlClient = GetMySqlClientData(ref dataTable, myQuery, CommandType.Text, connectionString, true, null);
                            resultsMySql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionResult));
                            return ((List<FunctionResult>)(resultsMySql));
                        case 1:
                            // Scalar
                            myQuery = GetFunctionScalarMySql(dataBaseConnect, dataBaseOwner);
                            retMySqlClient = GetMySqlClientData(ref dataTable, myQuery, CommandType.Text, connectionString, true, null);
                            resultsMySql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(FunctionResult));
                            return ((List<FunctionResult>)(resultsMySql));
                    }
                    return null;

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The collection of views.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of table results.</returns>
        public List<TablesResult> GetViews(string connectionString, string dataBase,
            Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetViewsSQL(dataBase);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetViewsOracle(dataBaseConnect, dataBaseOwner);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetViewsPostgreSql(dataBaseConnect, dataBaseOwner);
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsPostgreSqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsPostgreSqlClient));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetViewsMySql(dataBaseConnect, dataBaseOwner);
                    MySqlClient.MySqlCommand retMySqlClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(TablesResult));
                    return ((List<TablesResult>)(resultsMySqlClient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Views,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), "TABLE_NAME" });
                        DataTable dataTableTranslated = typeConversion.TranslateTable(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(TablesResult));
                        return ((List<TablesResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The database foreign keys.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of foreign key results.</returns>
        public List<ForeignKeyTableResult> GetDatabaseForeignKey(string connectionString, string dataBase, string tableName,
            Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            DataTable dataTableEx = null;

            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetDatabaseForeignKeySQL(dataBase, tableName);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetDatabaseForeignKeyOracle(dataBaseConnect, dataBaseOwner, tableName);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleCLient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsOracleCLient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetDatabaseForeignKeyPostgreSql(dataBaseConnect, dataBaseOwner, tableName);
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsPostgreSqlCLient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsPostgreSqlCLient));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetDatabaseForeignKeyMySql(dataBaseConnect, dataBaseOwner, tableName);
                    MySqlClient.MySqlCommand retMySqlClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySqlCLient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsMySqlCLient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), null });
                        GetOleDbData(ref dataTableEx, connectionString, System.Data.OleDb.OleDbSchemaGuid.Columns,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), tableName });

                        DataTable dataTableTranslated = typeConversion.TranslateTableForeignKey(tableName, dataBaseOwner, dataTable, dataTableEx);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(ForeignKeyTableResult));
                        return ((List<ForeignKeyTableResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The database reference keys.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of foreign key results.</returns>
        public List<ForeignKeyTableResult> GetDatabaseReferenceKey(string connectionString, string dataBase, string tableName,
            Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            DataTable dataTableEx = null;

            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetDatabaseReferenceKeysSQL(dataBase, tableName);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetDatabaseReferenceKeysOracle(dataBaseConnect, dataBaseOwner, tableName);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetDatabaseReferenceKeysPostgreSql(dataBaseConnect, dataBaseOwner, tableName);
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsPostgreSqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsPostgreSqlClient));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetDatabaseReferenceKeysMySql(dataBaseConnect, dataBaseOwner, tableName);
                    MySqlClient.MySqlCommand retMySqlClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ForeignKeyTableResult));
                    return ((List<ForeignKeyTableResult>)(resultsMySqlClient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), null });
                        GetOleDbData(ref dataTableEx, connectionString, System.Data.OleDb.OleDbSchemaGuid.Columns,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), tableName });

                        DataTable dataTableTranslated = typeConversion.TranslateTableReferenceKey(tableName, dataBaseOwner, dataTable, dataTableEx);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(ForeignKeyTableResult));
                        return ((List<ForeignKeyTableResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The database primary keys.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of foreign key results.</returns>
        public List<PrimaryKeyColumnsResult> GetDatabasePrimaryKeys(string connectionString, string dataBase, string tableName,
            Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetDatabasePrimaryKeysSQL(dataBase, tableName);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(PrimaryKeyColumnsResult));
                    return ((List<PrimaryKeyColumnsResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetDatabasePrimaryKeysOracle(dataBaseConnect, dataBaseOwner, tableName);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(PrimaryKeyColumnsResult));
                    return ((List<PrimaryKeyColumnsResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetDatabasePrimaryKeysPostgreSql(dataBaseConnect, dataBaseOwner, tableName);
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsPostgreSqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(PrimaryKeyColumnsResult));
                    return ((List<PrimaryKeyColumnsResult>)(resultsPostgreSqlClient));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetDatabasePrimaryKeysMySql(dataBaseConnect, dataBaseOwner, tableName);
                    MySqlClient.MySqlCommand retMySqlClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(PrimaryKeyColumnsResult));
                    return ((List<PrimaryKeyColumnsResult>)(resultsMySqlClient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        GetOleDbData(ref dataTable, connectionString, System.Data.OleDb.OleDbSchemaGuid.Primary_Keys,
                            new Object[] { null, (String.IsNullOrEmpty(dataBaseOwner) ? null : dataBaseOwner), tableName });
                        DataTable dataTableTranslated = typeConversion.TranslateTablePrimaryKey(dataTable);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTableTranslated, typeof(PrimaryKeyColumnsResult));
                        return ((List<PrimaryKeyColumnsResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The column values from the table.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of column values and indicators.</returns>
        public List<ColumnValuesResult> GetColumnValue(string connectionString, string dataBase, string tablename, string valueColumnName,
            string indicatorColumnName, Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetColumnValueSQL(dataBase, tablename, valueColumnName, indicatorColumnName);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetColumnValueOracle(dataBaseConnect, dataBaseOwner, tablename, valueColumnName, indicatorColumnName);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetColumnValuePostgreSql(dataBaseConnect, dataBaseOwner, tablename, valueColumnName, indicatorColumnName);
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsPostgreSqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsPostgreSqlClient));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetColumnValueMySql(dataBaseConnect, dataBaseOwner, tablename, valueColumnName, indicatorColumnName);
                    MySqlClient.MySqlCommand retMySqlClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsMySqlClient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        string oledbQuery = GetColumnValueGeneral(dataBase, tablename, valueColumnName, indicatorColumnName);
                        OleDbCommand retOleDb = GetOleDbData(ref dataTable, oledbQuery, CommandType.Text, connectionString, true, null);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                        return ((List<ColumnValuesResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The column values from the table.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <param name="dataFilter">The sql filter query to use./param>
        /// <param name="connectionType">The connection type provider.</param>
        /// <returns>The list of column values and indicators.</returns>
        public List<ColumnValuesResult> GetColumnValue(string connectionString, string dataBase, string tablename, string valueColumnName, string indicatorColumnName,
            string dataFilter, Common.ConnectionProvider.ConnectionType connectionType, string dataBaseConnect, string dataBaseOwner)
        {
            DataTable dataTable = null;
            // Create a new instance of the type
            // conversion object.
            Common.AnonymousTypeFunction typeConversion =
                new Common.AnonymousTypeFunction();

            switch (connectionType)
            {
                case ConnectionProvider.ConnectionType.SqlConnection:
                    string sqlQuery = GetColumnValueSQL(dataBase, tablename, valueColumnName, indicatorColumnName, dataFilter);
                    SqlCommand retSql = GetSqlData(ref dataTable, sqlQuery, CommandType.Text, connectionString, true, null);
                    object resultsSql = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsSql));

                case ConnectionProvider.ConnectionType.OracleClientConnection:
                    string oracleClientQuery = GetColumnValueOracle(dataBaseConnect, dataBaseOwner, tablename, valueColumnName, indicatorColumnName, dataFilter);
                    OracleClient.OracleCommand retOracleClient = GetOracleClientData(ref dataTable, oracleClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsOracleClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsOracleClient));

                case ConnectionProvider.ConnectionType.PostgreSqlConnection:
                    string pgClientQuery = GetColumnValuePostgreSql(dataBaseConnect, dataBaseOwner, tablename, valueColumnName, indicatorColumnName, dataFilter);
                    PostgreSqlClient.NpgsqlCommand retPostgreSqlClient = GetPostgreSqlClientData(ref dataTable, pgClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsPostgreSqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsPostgreSqlClient));

                case ConnectionProvider.ConnectionType.MySqlConnection:
                    string myClientQuery = GetColumnValueMySql(dataBaseConnect, dataBaseOwner, tablename, valueColumnName, indicatorColumnName, dataFilter);
                    MySqlClient.MySqlCommand retMySqlClient = GetMySqlClientData(ref dataTable, myClientQuery, CommandType.Text, connectionString, true, null);
                    object resultsMySqlClient = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                    return ((List<ColumnValuesResult>)(resultsMySqlClient));

                case ConnectionProvider.ConnectionType.OleDbConnection:
                    try
                    {
                        string oledbQuery = GetColumnValueGeneral(dataBase, tablename, valueColumnName, indicatorColumnName, dataFilter);
                        OleDbCommand retOleDb = GetOleDbData(ref dataTable, oledbQuery, CommandType.Text, connectionString, true, null);
                        object resultsOleDb = typeConversion.ListGenericTypeTranslator(dataTable, typeof(ColumnValuesResult));
                        return ((List<ColumnValuesResult>)(resultsOleDb));
                    }
                    catch { return null; }

                default:
                    throw new Exception("No connection type provider specified.");
            }
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <returns>The sql query</returns>
        public string GetColumnValueSQL(string dataBase, string tablename,
            string valueColumnName, string indicatorColumnName)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT CAST(" + valueColumnName + " AS varchar(max)) AS [ColumnValue]," + " " +
                    indicatorColumnName + " AS [ColumnIndicator]" + " " +
                "FROM [" + tablename + "]";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <returns>The sql query</returns>
        public string GetColumnValueOracle(string dataBaseConnect, string dataBaseOwner, string tablename,
            string valueColumnName, string indicatorColumnName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT CAST(" + valueColumnName.ToUpper() + " AS varchar2) AS \"ColumnValue\"," + " " +
                    indicatorColumnName.ToUpper() + " AS \"ColumnIndicator\"" + " " +
                "FROM \"" + dataBaseOwner.ToUpper() + "\".\"" + tablename.ToUpper() + "\"";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <returns>The sql query</returns>
        public string GetColumnValuePostgreSql(string dataBaseConnect, string dataBaseOwner, string tablename,
            string valueColumnName, string indicatorColumnName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT CAST(" + valueColumnName.ToUpper() + " AS varchar) AS \"ColumnValue\"," + " " +
                    indicatorColumnName.ToUpper() + " AS \"ColumnIndicator\"" + " " +
                "FROM \"" + dataBaseOwner.ToUpper() + "\".\"" + tablename.ToUpper() + "\"";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <returns>The sql query</returns>
        public string GetColumnValueMySql(string dataBaseConnect, string dataBaseOwner, string tablename,
            string valueColumnName, string indicatorColumnName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT CAST(" + valueColumnName.ToUpper() + " AS varchar) AS \"ColumnValue\"," + " " +
                    indicatorColumnName.ToUpper() + " AS \"ColumnIndicator\"" + " " +
                "FROM \"" + dataBaseOwner.ToUpper() + "\".\"" + tablename.ToUpper() + "\"";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <returns>The sql query</returns>
        public string GetColumnValueGeneral(string dataBase, string tablename,
            string valueColumnName, string indicatorColumnName)
        {
            string sql = "";
            sql +=
                "SELECT " + valueColumnName + " AS ColumnValue," + " " +
                    indicatorColumnName + " AS ColumnIndicator" + " " +
                "FROM " + tablename + "";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <param name="dataFilter">The sql filter query to use./param>
        /// <returns>The sql query.</returns>
        public string GetColumnValueGeneral(string dataBase, string tablename,
            string valueColumnName, string indicatorColumnName, string dataFilter)
        {
            string sql = "";
            sql +=
                "SELECT " + valueColumnName + " AS ColumnValue," + " " +
                    indicatorColumnName + " AS ColumnIndicator" + " " +
                "FROM " + tablename + " " +
                "WHERE (" + dataFilter + ")";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <param name="dataFilter">The sql filter query to use./param>
        /// <returns>The sql query.</returns>
        public string GetColumnValueSQL(string dataBase, string tablename,
            string valueColumnName, string indicatorColumnName, string dataFilter)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT CAST(" + valueColumnName + " AS varchar(max)) AS [ColumnValue]," + " " +
                    indicatorColumnName + " AS [ColumnIndicator]" + " " +
                "FROM [" + tablename + "] " +
                "WHERE (" + dataFilter + ")";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <param name="dataFilter">The sql filter query to use./param>
        /// <returns>The sql query.</returns>
        public string GetColumnValueOracle(string dataBaseConnect, string dataBaseOwner, string tablename,
            string valueColumnName, string indicatorColumnName, string dataFilter)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT CAST(" + valueColumnName.ToUpper() + " AS varchar2) AS \"ColumnValue\"," + " " +
                    indicatorColumnName.ToUpper() + " AS \"ColumnIndicator\"" + " " +
                "FROM \"" + dataBaseOwner.ToUpper() + "\".\"" + tablename.ToUpper() + "\"," + " " +
                "WHERE (" + dataFilter + ")";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <param name="dataFilter">The sql filter query to use./param>
        /// <returns>The sql query.</returns>
        public string GetColumnValuePostgreSql(string dataBaseConnect, string dataBaseOwner, string tablename,
            string valueColumnName, string indicatorColumnName, string dataFilter)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT CAST(" + valueColumnName.ToUpper() + " AS varchar) AS \"ColumnValue\"," + " " +
                    indicatorColumnName.ToUpper() + " AS \"ColumnIndicator\"" + " " +
                "FROM \"" + dataBaseOwner.ToUpper() + "\".\"" + tablename.ToUpper() + "\"," + " " +
                "WHERE (" + dataFilter + ")";

            return sql;
        }

        /// <summary>
        /// The sql query used to used execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="tablename">The table name.</param>
        /// <param name="valueColumnName">The column name of the value.</param>
        /// <param name="indicatorColumnName">The column name of the indicator.</param>
        /// <param name="dataFilter">The sql filter query to use./param>
        /// <returns>The sql query.</returns>
        public string GetColumnValueMySql(string dataBaseConnect, string dataBaseOwner, string tablename,
            string valueColumnName, string indicatorColumnName, string dataFilter)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT CAST(" + valueColumnName.ToUpper() + " AS varchar) AS \"ColumnValue\"," + " " +
                    indicatorColumnName.ToUpper() + " AS \"ColumnIndicator\"" + " " +
                "FROM \"" + dataBaseOwner.ToUpper() + "\".\"" + tablename.ToUpper() + "\"," + " " +
                "WHERE (" + dataFilter + ")";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetTablesSQL(string dataBase)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT" + " " +
                    "information_schema.tables.Table_Schema AS [TableOwner]," + " " +
                    "information_schema.tables.Table_Name AS [TableName]" + " " +
                "FROM  information_schema.tables" + " " +
                "WHERE" + " " +
                    "information_schema.tables.Table_Type = 'Base Table'" + " " +
                "ORDER BY" + " " +
                    "information_schema.tables.Table_Schema ASC";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <returns>The sql query.</returns>
        public string GetTablesOracle(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "OWNER AS \"TableOwner\"," + " " +
                    "TABLE_NAME AS \"TableName\"" + " " +
                "FROM ALL_TABLES" + " " +
                "WHERE" + " " +
                    "OWNER = '" + dataBaseOwner.ToUpper() + "'" + " " +
                "ORDER BY" + " " +
                    "OWNER ASC";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <returns>The sql query.</returns>
        public string GetTablesPostgreSql(string dataBase, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "" : "";
            sql +=
                "SELECT" + " " +
	                "information_schema.tables.table_schema AS \"TableOwner\"," + " " +
	                "information_schema.tables.table_name AS \"TableName\"" + " " +
                "FROM information_schema.tables" + " " +
                "WHERE information_schema.tables.table_type = 'BASE TABLE' And" + " " +
	                "information_schema.tables.table_schema = '" + dataBaseOwner + "'" + " " +
                "ORDER BY" + " " +
	                "information_schema.tables.table_schema ASC";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <returns>The sql query.</returns>
        public string GetTablesMySql(string dataBase, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "information_schema.tables.table_schema AS \"TableOwner\"," + " " +
                    "information_schema.tables.table_name AS \"TableName\"" + " " +
                "FROM information_schema.tables" + " " +
                "WHERE information_schema.tables.table_type = 'BASE TABLE' And" + " " +
                    "information_schema.tables.table_schema = '" + dataBaseOwner + "'" + " " +
                "ORDER BY" + " " +
                    "information_schema.tables.table_schema ASC";

            return sql;
        }

        /// <summary>
        /// The view query to execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetViewsSQL(string dataBase)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT" + " " +
                    "information_schema.views.Table_Schema AS [TableOwner]," + " " +
                    "information_schema.views.TABLE_NAME AS [TableName]" + " " +
                "FROM information_schema.views" + " " +
                "ORDER BY" + " " +
                    "information_schema.views.Table_Schema ASC";

            return sql;
        }

        /// <summary>
        /// The view query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <returns>The sql query.</returns>
        public string GetViewsOracle(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "OWNER AS \"TableOwner\"," + " " +
                    "VIEW_NAME AS \"TableName\"" + " " +
                "FROM ALL_VIEWS" + " " +
                "WHERE" + " " +
                    "OWNER = '" + dataBaseOwner.ToUpper() + "'" + " " +
                "ORDER BY" + " " +
                    "OWNER ASC";

            return sql;
        }

        /// <summary>
        /// The view query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <returns>The sql query.</returns>
        public string GetViewsPostgreSql(string dataBase, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "" : "";
            sql +=
                "SELECT" + " " +
	                "information_schema.views.table_schema AS \"TableOwner\"," + " " +
	                "information_schema.views.table_name AS \"TableName\"" + " " +
                "FROM information_schema.views" + " " +
                "WHERE information_schema.views.table_schema = '" + dataBaseOwner + "'" + " " +
                "ORDER BY" + " " +
	                "information_schema.views.table_schema ASC";

            return sql;
        }

        /// <summary>
        /// The view query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <returns>The sql query.</returns>
        public string GetViewsMySql(string dataBase, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "information_schema.views.table_schema AS \"TableOwner\"," + " " +
                    "information_schema.views.table_name AS \"TableName\"" + " " +
                "FROM information_schema.views" + " " +
                "WHERE information_schema.views.table_schema = '" + dataBaseOwner + "'" + " " +
                "ORDER BY" + " " +
                    "information_schema.views.table_schema ASC";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetFunctionSQL(string dataBase)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT" + " " +
                    "information_schema.ROUTINES.ROUTINE_NAME  AS [OverloadName]," + " " +
				    "information_schema.ROUTINES.ROUTINE_NAME  AS [FunctionName]," + " " +
				    "information_schema.ROUTINES.ROUTINE_SCHEMA  AS [FunctionOwner]" + " " + 
			    "FROM  information_schema.ROUTINES" + " " +
			    "WHERE" + " " +
				    "information_schema.ROUTINES.ROUTINE_TYPE = 'FUNCTION'" + " " +
			    "ORDER BY" + " " +
				    "information_schema.ROUTINES.ROUTINE_NAME ASC";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetFunctionOracle(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "OBJECT_NAME AS \"OverloadName\"," + " " +
                    "OWNER AS \"FunctionOwner\"," + " " +
                    "OBJECT_NAME AS \"FunctionName\"" + " " +
                "FROM ALL_OBJECTS" + " " +
                "WHERE OWNER = '" + dataBaseOwner.ToUpper() + "' AND OBJECT_TYPE = 'FUNCTION'";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetFunctionTablePostgreSql(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "r.routine_schema As \"FunctionOwner\"," + " " +
                    "r.routine_name As \"FunctionName\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type = 'USER-DEFINED'";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetFunctionScalarPostgreSql(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "r.routine_schema As \"FunctionOwner\"," + " " +
                    "r.routine_name As \"FunctionName\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type <> 'USER-DEFINED'";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetFunctionTableMySql(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "r.routine_schema As \"FunctionOwner\"," + " " +
                    "r.routine_name As \"FunctionName\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type = 'USER-DEFINED'";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetFunctionScalarMySql(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "r.routine_schema As \"FunctionOwner\"," + " " +
                    "r.routine_name As \"FunctionName\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type <> 'USER-DEFINED'";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetProcedureSQL(string dataBase)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT" + " " +
				    "information_schema.ROUTINES.ROUTINE_NAME  AS [ProcedureName]," + " " +
				    "information_schema.ROUTINES.ROUTINE_SCHEMA  AS [ProcedureOwner]" + " " + 
			    "FROM  information_schema.ROUTINES" + " " +
			    "WHERE" + " " +
                    "information_schema.ROUTINES.ROUTINE_TYPE = 'PROCEDURE'" + " " +
			    "ORDER BY" + " " +
				    "information_schema.ROUTINES.ROUTINE_NAME ASC";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetProcedureOracle(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "OWNER AS \"ProcedureOwner\"," + " " +
                    "OBJECT_NAME AS \"ProcedureName\"" + " " +
                "FROM ALL_OBJECTS" + " " +
                "WHERE OWNER = '" + dataBaseOwner.ToUpper() + "' AND OBJECT_TYPE = 'PROCEDURE'";

            return sql;
        }

        /// <summary>
        /// The table query to execute.
        /// </summary>
        /// <param name="dataBaseConnect">The database name.</param>
        /// <returns>The sql query.</returns>
        public string GetProcedureMySql(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "r.routine_schema As \"FunctionOwner\"," + " " +
                    "r.routine_name As \"FunctionName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_type = 'PROCEDURE' And" + " " +
                    "r.data_type = 'USER-DEFINED'";

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseConnect"></param>
        /// <param name="dataBaseOwner"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public string GetXmlPackageSpecificationOracle(string dataBaseConnect, string dataBaseOwner, string packageName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "dbms_metadata.get_xml('PACKAGE_SPEC', '" + packageName.ToUpper() + "', '" + dataBaseOwner.ToUpper() + "') AS \"ObjectXml\"" + " " +
                "FROM DUAL";

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseConnect"></param>
        /// <param name="dataBaseOwner"></param>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public string GetXmlProcedureOracle(string dataBaseConnect, string dataBaseOwner, string procedureName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "dbms_metadata.get_xml('PROCEDURE', '" + procedureName.ToUpper() + "', '" + dataBaseOwner.ToUpper() + "') AS \"ObjectXml\"" + " " +
                "FROM DUAL";

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseConnect"></param>
        /// <param name="dataBaseOwner"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public string GetXmlFunctionOracle(string dataBaseConnect, string dataBaseOwner, string functionName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "dbms_metadata.get_xml('FUNCTION', '" + functionName.ToUpper() + "', '" + dataBaseOwner.ToUpper() + "') AS \"ObjectXml\"" + " " +
                "FROM DUAL";

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseConnect"></param>
        /// <param name="dataBaseOwner"></param>
        /// <returns></returns>
        public string GetPackagesOracle(string dataBaseConnect, string dataBaseOwner)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "OWNER AS \"PackageOwner\"," + " " +
                    "OBJECT_NAME AS \"PackageName\"" + " " +
                "FROM ALL_OBJECTS" + " " +
                "WHERE OWNER = '" + dataBaseOwner.ToUpper() + "' AND OBJECT_TYPE = 'PACKAGE'";

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseConnect"></param>
        /// <param name="dataBaseOwner"></param>
        /// <returns></returns>
        public string GetPackageProcedureOracle(string dataBaseConnect, string dataBaseOwner, string packageName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "OWNER AS \"ProcedureOwner\"," + " " +
                    "OBJECT_NAME AS \"ProcedureName\"," + " " +
                    "DECODE(PACKAGE_NAME, NULL, '', PACKAGE_NAME) AS \"PackageName\"" + " " +
                "FROM ALL_ARGUMENTS" + " " +
                "WHERE OWNER = '" + dataBaseOwner.ToUpper() + "' AND PACKAGE_NAME = '" + packageName.ToUpper() + "'";

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseConnect"></param>
        /// <param name="dataBaseOwner"></param>
        /// <returns></returns>
        public string GetPackageFunctionOracle(string dataBaseConnect, string dataBaseOwner, string packageName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "OWNER AS \"FunctionOwner\"," + " " +
                    "OBJECT_NAME AS \"FunctionName\"," + " " +
                    "DECODE(PACKAGE_NAME, NULL, '', PACKAGE_NAME) AS \"PackageName\"" + " " +
                "FROM ALL_ARGUMENTS" + " " +
                "WHERE OWNER = '" + dataBaseOwner.ToUpper() + "' AND PACKAGE_NAME = '" + packageName.ToUpper() + "'";

            return sql;
        }

        /// <summary>
        /// The database table foreign keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseForeignKeySQL(string dataBase, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
			    "SELECT" + " " +
				    "CAST(sysobjects.Name as varchar(max)) as [TableName]," + " " +
				    "CAST(syscolumns.Name as varchar(max)) as [ColumnName]," + " " +
				    "CAST(syscolumns.isNullable as bit) as [IsNullable]," + " " +
				    "CAST(syscolumns.Length as bigint) as [Length]," + " " +
                    "CAST(syscolumns.prec as bigint) as [Precision]," + " " +
				    "CAST(systypes.Name as varchar(max)) as [ColumnType]," + " " +
                    "CAST(OBJECT_NAME(sysforeignkeys.rkeyid) as varchar(max)) as [ForeignKeyTable]," + " " +
                    "CAST(COL_NAME(sysforeignkeys.rkeyid, sysforeignkeys.rkey) as varchar(max)) as [ForeignKeyColumnName]," + " " +
                    "CAST(SCHEMA_NAME(sysobjects.uid) as varchar(max)) AS [ForeignKeyOwner]" + " " +
			    "FROM" + " " +
                    "syscolumns, sysobjects, systypes, sysforeignkeys" + " " +
			    "WHERE syscolumns.ID = sysobjects.ID" + " " +
				    "and syscolumns.xusertype = systypes.xusertype" + " " +
				    "and syscolumns.id = sysforeignkeys.fkeyid" + " " +
				    "and syscolumns.colid = sysforeignkeys.fkey" + " " +
				    "and sysobjects.status <> 1" + " " +
				    "and sysobjects.type = 'U'" + " " +
				    "and systypes.name <> 'XML'" + " " +
                    "and dbo.sysobjects.name = '" + tableName + "'";

            return sql;
        }

        /// <summary>
        /// The database table foreign keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseForeignKeyOracle(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "A.TABLE_NAME AS \"TableName\"," + " " +
                    "B.COLUMN_NAME AS \"ColumnName\"," + " " +
                    "DECODE((SELECT TAB.NULLABLE FROM ALL_TAB_COLUMNS TAB WHERE B.TABLE_NAME = TAB.TABLE_NAME AND B.COLUMN_NAME = TAB.COLUMN_NAME), 'N', 0, 'Y', 1, 0) AS \"IsNullable\"," + " " +
                    "(SELECT TAB.DATA_LENGTH FROM ALL_TAB_COLUMNS TAB WHERE B.TABLE_NAME = TAB.TABLE_NAME AND B.COLUMN_NAME = TAB.COLUMN_NAME) AS \"Length\"," + " " +
                    "(SELECT TAB.DATA_PRECISION FROM ALL_TAB_COLUMNS TAB WHERE B.TABLE_NAME = TAB.TABLE_NAME AND B.COLUMN_NAME = TAB.COLUMN_NAME) AS \"Precision\"," + " " +
                    "(SELECT TAB.DATA_TYPE FROM ALL_TAB_COLUMNS TAB WHERE B.TABLE_NAME = TAB.TABLE_NAME AND B.COLUMN_NAME = TAB.COLUMN_NAME) AS \"ColumnType\"," + " " +
                    "C.TABLE_NAME AS \"ForeignKeyTable\"," + " " +
                    "D.COLUMN_NAME AS \"ForeignKeyColumnName\"," + " " +
                    "C.OWNER AS \"ForeignKeyOwner\"" + " " +
                "FROM" + " " +
                    "ALL_CONSTRAINTS A," + " " +
                    "ALL_CONS_COLUMNS B," + " " +
                    "ALL_CONSTRAINTS C," + " " +
                    "ALL_CONS_COLUMNS D" + " " +
                "WHERE" + " " +
                    "A.OWNER = '" + dataBaseOwner.ToUpper() + "' AND" + " " +
                    "A.TABLE_NAME = '" + tableName.ToUpper() + "' AND " + " " +
                    "A.CONSTRAINT_NAME = B.CONSTRAINT_NAME AND" + " " +
                    "A.R_CONSTRAINT_NAME IS NOT NULL AND" + " " +
                    "A.R_CONSTRAINT_NAME = C.CONSTRAINT_NAME AND" + " " +
                    "C.CONSTRAINT_NAME = D.CONSTRAINT_NAME" + " " +
                "ORDER BY" + " " +
                    "A.TABLE_NAME";

            return sql;
        }

        /// <summary>
        /// The database table foreign keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseForeignKeyPostgreSql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " + 
	                "C.table_name As \"TableName\"," + " " +
	                "C.column_name As \"ColumnName\"," + " " +
	                "A.table_name As \"ForeignKeyTable\"," + " " +
	                "A.column_name As \"ForeignKeyColumnName\"," + " " +
	                "A.table_schema As \"ForeignKeyOwner\"," + " " +
	                "CAST(T.is_nullable As Boolean) As \"IsNullable\"," + " " +
	                "CAST(T.character_maximum_length As BigInt) As \"Length\"," + " " +
	                "CAST((SELECT tp.typlen FROM pg_type tp WHERE tp.typname = T.udt_name) as BigInt) As \"Precision\"," + " " +
	                "T.udt_name As \"ColumnType\"" + " " +
                "FROM" + " " +
			        "information_schema.columns T," + " " +
			        "information_schema.table_constraints Z," + " " +
			        "information_schema.key_column_usage A," + " " +
			        "information_schema.referential_constraints B INNER JOIN" + " " +
				        "information_schema.key_column_usage C ON" + " " + 
					        "(b.constraint_name = c.constraint_name)" + " " +
                "WHERE" + " " +
			        "Z.table_name = '" + tableName + "' AND" + " " +
	                "Z.table_schema = '" + dataBaseOwner + "' AND" + " " +
	                "Z.CONSTRAINT_TYPE = 'FOREIGN KEY' AND" + " " +
			        "B.unique_constraint_name = A.constraint_name AND" + " " +
			        "Z.constraint_name = C.constraint_name AND" + " " +
			        "Z.table_name = C.table_name AND" + " " +
			        "C.table_name = T.table_name AND" + " " +
			        "C.column_name = T.column_name";

            return sql;
        }

        /// <summary>
        /// The database table foreign keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseForeignKeyMySql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "C.table_name As \"TableName\"," + " " +
                    "C.column_name As \"ColumnName\"," + " " +
                    "A.table_name As \"ForeignKeyTable\"," + " " +
                    "A.column_name As \"ForeignKeyColumnName\"," + " " +
                    "A.table_schema As \"ForeignKeyOwner\"," + " " +
                    "CASE" + " " +
                        "WHEN T.is_nullable = 'NO' THEN 0" + " " +
                        "WHEN T.is_nullable = 'YES' THEN 1" + " " +
                        "ELSE 0 END As \"IsNullable\"," + " " +
                    "T.character_maximum_length As \"Length\"," + " " +
                    "T.numeric_precision As \"Precision\"," + " " +
                    "T.data_type As \"ColumnType\"" + " " +
                "FROM" + " " +
                    "information_schema.columns T," + " " +
                    "information_schema.table_constraints Z," + " " +
                    "information_schema.key_column_usage A," + " " +
                    "information_schema.referential_constraints B INNER JOIN" + " " +
                        "information_schema.key_column_usage C ON" + " " +
                            "(b.constraint_name = c.constraint_name)" + " " +
                "WHERE" + " " +
                    "Z.table_name = '" + tableName + "' AND" + " " +
                    "Z.table_schema = '" + dataBaseOwner + "' AND" + " " +
                    "Z.CONSTRAINT_TYPE = 'FOREIGN KEY' AND" + " " +
                    "B.unique_constraint_name = A.constraint_name AND" + " " +
                    "Z.constraint_name = C.constraint_name AND" + " " +
                    "Z.table_name = C.table_name AND" + " " +
                    "C.table_name = T.table_name AND" + " " +
                    "C.column_name = T.column_name";

            return sql;
        }

        /// <summary>
        /// The database table reference keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseReferenceKeysSQL(string dataBase, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT" + " " +
                    "CAST(sysobjects.Name as varchar(max)) as [TableName]," + " " +
                    "CAST(syscolumns.Name as varchar(max)) as [ColumnName]," + " " +
                    "CAST(syscolumns.isNullable as bit) as [IsNullable]," + " " +
                    "CAST(syscolumns.Length as bigint) as [Length]," + " " +
                    "CAST(syscolumns.prec as bigint) as [Precision]," + " " +
                    "CAST(systypes.Name as varchar(max)) as [ColumnType]," + " " +
                    "CAST(OBJECT_NAME(sysforeignkeys.rkeyid) as varchar(max)) as [ForeignKeyTable]," + " " +
                    "CAST(COL_NAME(sysforeignkeys.rkeyid, sysforeignkeys.rkey) as varchar(max)) as [ForeignKeyColumnName]," + " " +
                    "CAST(SCHEMA_NAME(sysobjects.uid) as varchar(max)) AS [ForeignKeyOwner]" + " " +
                "FROM" + " " +
                    "syscolumns, sysobjects, systypes, sysforeignkeys" + " " +
                "WHERE syscolumns.ID = sysobjects.ID" + " " +
                    "and syscolumns.xusertype = systypes.xusertype" + " " +
                    "and syscolumns.id = sysforeignkeys.fkeyid" + " " +
                    "and syscolumns.colid = sysforeignkeys.fkey" + " " +
                    "and sysobjects.status <> 1" + " " +
                    "and sysobjects.type = 'U'" + " " +
                    "and systypes.name <> 'XML'" + " " +
                    "and OBJECT_NAME(sysforeignkeys.rkeyid) ='" + tableName + "'";

            return sql;
        }

        /// <summary>
        /// The database table reference keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseReferenceKeysOracle(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "CON.TABLE_NAME AS \"TableName\"," + " " +
                    "COL.COLUMN_NAME AS \"ColumnName\"," + " " +
                    "DECODE((SELECT TAB.NULLABLE FROM ALL_TAB_COLUMNS TAB WHERE COL.TABLE_NAME = TAB.TABLE_NAME AND COL.COLUMN_NAME = TAB.COLUMN_NAME), 'N', 0, 'Y', 1, 0) AS \"IsNullable\"," + " " +
                    "(SELECT TAB.DATA_LENGTH FROM ALL_TAB_COLUMNS TAB WHERE COL.TABLE_NAME = TAB.TABLE_NAME AND COL.COLUMN_NAME = TAB.COLUMN_NAME) AS \"Length\"," + " " +
                    "(SELECT TAB.DATA_PRECISION FROM ALL_TAB_COLUMNS TAB WHERE COL.TABLE_NAME = TAB.TABLE_NAME AND COL.COLUMN_NAME = TAB.COLUMN_NAME) AS \"Precision\"," + " " +
                    "(SELECT TAB.DATA_TYPE FROM ALL_TAB_COLUMNS TAB WHERE COL.TABLE_NAME = TAB.TABLE_NAME AND COL.COLUMN_NAME = TAB.COLUMN_NAME) AS \"ColumnType\"," + " " +
                    "COL.TABLE_NAME AS \"ForeignKeyTable\"," + " " +
                    "COL.COLUMN_NAME AS \"ForeignKeyColumnName\"," + " " +
                    "CON.OWNER AS \"ForeignKeyOwner\"" + " " +
                "FROM" + " " +
                    "ALL_CONSTRAINTS CON," + " " +
                    "ALL_CONS_COLUMNS COL" + " " +
                "WHERE" + " " +
                    "CON.OWNER = '" + dataBaseOwner.ToUpper() + "' AND" + " " +
                    "COL.TABLE_NAME = '" + tableName.ToUpper() + "' AND" + " " +
                    "CON.CONSTRAINT_TYPE = 'R' AND" + " " +
                    "CON.R_CONSTRAINT_NAME = COL.CONSTRAINT_NAME" + " " +
                "ORDER BY" + " " +
                    "CON.TABLE_NAME";

            return sql;
        }

        /// <summary>
        /// The database table reference keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseReferenceKeysPostgreSql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
	                "Z.table_name As \"TableName\"," + " " +
	                "A.column_name As \"ColumnName\"," + " " +
	                "A.table_name As \"ForeignKeyTable\"," + " " +
	                "A.column_name As \"ForeignKeyColumnName\"," + " " +
	                "A.table_schema As \"ForeignKeyOwner\"," + " " +
	                "CAST(T.is_nullable As Boolean) As \"IsNullable\"," + " " +
	                "CAST(T.character_maximum_length As BigInt) As \"Length\"," + " " +
	                "CAST((SELECT tp.typlen FROM pg_type tp WHERE tp.typname = T.udt_name) as BigInt) As \"Precision\"," + " " +
	                "T.udt_name As \"ColumnType\"" + " " +
                "FROM" + " " +
			        "information_schema.columns T," + " " +
			        "information_schema.table_constraints Z," + " " +
			        "information_schema.key_column_usage A" + " " +
                "WHERE" + " " +
			        "A.table_name = '" + tableName + "' AND" + " " +
	                "A.table_schema = '" + dataBaseOwner + "' AND" + " " +
	                "Z.CONSTRAINT_TYPE = 'FOREIGN KEY' AND" + " " +
			        "A.table_name = T.table_name AND" + " " +
			        "Z.table_name <> A.table_name";

            return sql;
        }

        /// <summary>
        /// The database table reference keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseReferenceKeysMySql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "Z.table_name As \"TableName\"," + " " +
                    "A.column_name As \"ColumnName\"," + " " +
                    "A.table_name As \"ForeignKeyTable\"," + " " +
                    "A.column_name As \"ForeignKeyColumnName\"," + " " +
                    "A.table_schema As \"ForeignKeyOwner\"," + " " +
                    "CASE" + " " +
                        "WHEN T.is_nullable = 'NO' THEN 0" + " " +
                        "WHEN T.is_nullable = 'YES' THEN 1" + " " +
                        "ELSE 0 END As \"IsNullable\"," + " " +
                    "T.character_maximum_length As \"Length\"," + " " +
                    "T.numeric_precision As \"Precision\"," + " " +
                    "T.data_type As \"ColumnType\"" + " " +
                "FROM" + " " +
                    "information_schema.columns T," + " " +
                    "information_schema.table_constraints Z," + " " +
                    "information_schema.key_column_usage A" + " " +
                "WHERE" + " " +
                    "A.table_name = '" + tableName + "' AND" + " " +
                    "A.table_schema = '" + dataBaseOwner + "' AND" + " " +
                    "Z.CONSTRAINT_TYPE = 'FOREIGN KEY' AND" + " " +
                    "A.table_name = T.table_name AND" + " " +
                    "Z.table_name <> A.table_name";

            return sql;
        }

        /// <summary>
        /// The database table primary keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabasePrimaryKeysSQL(string dataBase, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT c.COLUMN_NAME AS [PrimaryKeyName]" + " " +
                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk," + " " +
                    "INFORMATION_SCHEMA.KEY_COLUMN_USAGE c" + " " +
                "WHERE pk.TABLE_NAME = '" + tableName + "'" + " " +
                    "and pk.CONSTRAINT_TYPE = 'PRIMARY KEY'" + " " +
                    "and c.TABLE_NAME = pk.TABLE_NAME" + " " +
                    "and c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME";

            return sql;
        }

        /// <summary>
        /// The database table primary keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabasePrimaryKeysOracle(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "cols.column_name AS \"PrimaryKeyName\"" + " " +
                "FROM" + " " +
                    "ALL_CONS_COLUMNS cols, ALL_CONSTRAINTS cons" + " " +
                "WHERE" + " " +
                    "cons.OWNER = '" + dataBaseOwner.ToUpper() + "' AND" + " " +
                    "cols.TABLE_NAME = '" + tableName.ToUpper() + "' AND" + " " +
                    "cons.constraint_type = 'P' AND" + " " +
                    "cons.constraint_name = cols.constraint_name" + " " +
                "ORDER BY cols.table_name, cols.position";

            return sql;
        }

        /// <summary>
        /// The database table primary keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabasePrimaryKeysPostgreSql(string dataBase, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "" : "";
            sql +=
                "SELECT c.column_name as \"PrimaryKeyName\"" + " " +
                "FROM information_schema.table_constraints pk," + " " +
	                "information_schema.key_column_usage c" + " " +
                "WHERE pk.table_name = '" + tableName + "'" + " " +
	                "And pk.table_schema = '" + dataBaseOwner + "'" + " " +
	                "And pk.CONSTRAINT_TYPE = 'PRIMARY KEY'" + " " +
	                "And c.TABLE_NAME = pk.TABLE_NAME" + " " +
	                "And c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME";

            return sql;
        }

        /// <summary>
        /// The database table primary keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="dataBaseOwner">The database owner.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabasePrimaryKeysMySql(string dataBase, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "" : "";
            sql +=
                "SELECT c.column_name as \"PrimaryKeyName\"" + " " +
                "FROM information_schema.table_constraints pk," + " " +
                    "information_schema.key_column_usage c" + " " +
                "WHERE pk.table_name = '" + tableName + "'" + " " +
                    "And pk.table_schema = '" + dataBaseOwner + "'" + " " +
                    "And pk.CONSTRAINT_TYPE = 'PRIMARY KEY'" + " " +
                    "And c.TABLE_NAME = pk.TABLE_NAME" + " " +
                    "And c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME";

            return sql;
        }

        /// <summary>
        /// The database table primary keys.
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <returns>The sql query.</returns>
        public string GetDatabasePrimaryKeysClustrerSQL(string dataBase, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT dbo.syscolumns.name AS PrimaryKeyName" + " " +
                "FROM dbo.syscolumns" + " " +
                "WHERE dbo.syscolumns.id IN" + " " +
                        "(SELECT dbo.sysobjects.id" + " " +
                        "FROM dbo.sysobjects" + " " +
                        "WHERE dbo.sysobjects.name = '" + tableName + "')" + " " +
                    "AND dbo.syscolumns.colid IN" + " " +
                        "(SELECT dbo.sysindexkeys.colid" + " " +
                        "FROM dbo.sysindexkeys" + " " +
                            "JOIN dbo.sysobjects ON dbo.sysindexkeys.id = dbo.sysobjects.id" + " " +
                        "WHERE dbo.sysindexkeys.indid = 1" + " " +
                            "AND dbo.sysobjects.name = '" + tableName + "')";

            return sql;
        }

        /// <summary>
        /// The database table columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseTableColumnsSQL(string dataBase, string tableName, string owner)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
			    "SELECT DISTINCT" + " " +
				    "dbo.syscolumns.name AS ColumnName," + " " +
				    "(SELECT TOP 1 dbo.systypes.name" + " " +
				    "FROM dbo.systypes" + " " +
				    "WHERE dbo.systypes.xtype = dbo.syscolumns.xtype) AS ColumnType," + " " +
				    "dbo.syscolumns.isnullable AS ColumnNullable," + " " +
				    "dbo.syscolumns.colorder AS ColumnOrder," + " " +
				    "dbo.syscolumns.iscomputed AS IsComputed," + " " +
				    "dbo.syscolumns.length AS Length," + " " +
                    "dbo.syscolumns.prec AS Precision," + " " +
                    "dbo.syscolumns.colstat AS PrimaryKeySeed INTO #t_obj" + " " +
			    "FROM" + " " +
				    "dbo.syscolumns INNER JOIN" + " " +
					    "dbo.sysobjects ON dbo.syscolumns.id = dbo.sysobjects.id INNER JOIN" + " " +
						    "dbo.systypes ON dbo.syscolumns.xtype = dbo.systypes.xtype" + " " +
			    "WHERE" + " " +
                    "(dbo.sysobjects.name = '" + tableName + "')" + " " +
			    "ORDER BY" + " " +
				    "dbo.syscolumns.colorder ASC" + " " +
			    "SELECT" + " " +
				    "CAST(ColumnName AS varchar(max)) AS [ColumnName]," + " " +
				    "CAST(ColumnType AS varchar(max)) AS [ColumnType]," + " " +
				    "CAST(ColumnNullable AS Bit) AS [ColumnNullable]," + " " +
				    "CAST(ColumnOrder AS Int) AS [ColumnOrder]," + " " +
				    "CAST(IsComputed AS Bit) AS [IsComputed]," + " " +
				    "CAST(Length AS BigInt) AS [Length]," + " " +
                    "CAST(Precision AS BigInt) AS [Precision]," + " " +
				    "CAST(PrimaryKeySeed AS Bit) AS [PrimaryKeySeed]" + " " +
			    "FROM #t_obj" + " " +
			    "DROP TABLE #t_obj";

            return sql;
        }

        /// <summary>
        /// The database table columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseTableColumnsOracle(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "TAB.COLUMN_NAME AS \"ColumnName\"," + " " +
                    "TAB.DATA_TYPE AS \"ColumnType\"," + " " +
                    "DECODE(TAB.NULLABLE, 'N', 0, 'Y', 1, 0) AS \"ColumnNullable\"," + " " +
                    "TAB.COLUMN_ID AS \"ColumnOrder\"," + " " +
                    "0 AS \"IsComputed\"," + " " +
                    "TAB.DATA_LENGTH AS \"Length\"," + " " +
                    "TAB.DATA_PRECISION AS \"Precision\"," + " " +
                    "0 AS \"PrimaryKeySeed\"" + " " +
                "FROM" + " " +
                    "ALL_TAB_COLUMNS TAB" + " " +
                "WHERE" + " " +
                    "TAB.OWNER = '" + dataBaseOwner.ToUpper() + "' AND" + " " +
                    "TAB.TABLE_NAME = '" + tableName.ToUpper() + "'" + " " +
                "ORDER BY TAB.COLUMN_ID";

            return sql;
        }

        /// <summary>
        /// The database table columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseTableColumnsPostgreSql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
	                "column_name as \"ColumnName\"," + " " +
	                "udt_name as \"ColumnType\"," + " " +
	                "CAST(is_nullable as boolean) as \"ColumnNullable\"," + " " +
	                "CAST(ordinal_position as integer) as \"ColumnOrder\"," + " " +
	                "CAST(is_identity as boolean) as \"IsComputed\"," + " " +
	                "CAST(character_maximum_length as bigint) as \"Length\"," + " " +
	                "CAST((SELECT t.typlen FROM pg_type t WHERE t.typname = c.udt_name) as bigint) as \"Precision\"," + " " +
	                "CAST(0 as boolean) as \"PrimaryKeySeed\"" + " " +
                "FROM" + " " +
	                "information_schema.columns c" + " " +
                "WHERE" + " " +
                    "table_name = '" + tableName + "'" + " " +
	                "And table_schema = '" + dataBaseOwner + "'" + " " +
                "ORDER BY ordinal_position";

            return sql;
        }

        /// <summary>
        /// The database table columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseTableColumnsMySql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT" + " " +
                    "column_name as \"ColumnName\"," + " " +
                    "data_type as \"ColumnType\"," + " " +
                    "CASE" + " " +
                        "WHEN is_nullable = 'NO' THEN 0" + " " +
                        "WHEN is_nullable = 'YES' THEN 1" + " " +
                        "ELSE 0 END As \"ColumnNullable\"," + " " +
                    "ordinal_position as \"ColumnOrder\"," + " " +
                    "0 as \"IsComputed\"," + " " +
                    "character_maximum_length as \"Length\"," + " " +
                    "numeric_precision as \"Precision\"," + " " +
                    "0 as \"PrimaryKeySeed\"" + " " +
                "FROM" + " " +
                    "information_schema.columns c" + " " +
                "WHERE" + " " +
                    "table_name = '" + tableName + "'" + " " +
                    "And table_schema = '" + dataBaseOwner + "'" + " " +
                "ORDER BY ordinal_position";

            return sql;
        }

        /// <summary>
        /// The database procedure columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseProcedureColumnsSQL(string dataBase, string tableName, string owner)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "dbo.syscolumns.name AS ColumnName," + " " +
                    "(SELECT TOP 1 dbo.systypes.name" + " " +
                    "FROM dbo.systypes" + " " +
                    "WHERE dbo.systypes.xtype = dbo.syscolumns.xtype) AS ColumnType," + " " +
                    "dbo.syscolumns.isnullable AS ColumnNullable," + " " +
                    "dbo.syscolumns.colorder AS ColumnOrder," + " " +
                    "dbo.syscolumns.length AS Length," + " " +
                    "dbo.syscolumns.prec AS Precision," + " " +
                    "dbo.syscolumns.isoutparam AS IsOutParameter INTO #t_obj" + " " +
                "FROM" + " " +
                    "dbo.syscolumns INNER JOIN" + " " +
                        "dbo.sysobjects ON dbo.syscolumns.id = dbo.sysobjects.id INNER JOIN" + " " +
                            "dbo.systypes ON dbo.syscolumns.xtype = dbo.systypes.xtype" + " " +
                "WHERE" + " " +
                    "(dbo.sysobjects.name = '" + tableName + "')" + " " +
                "ORDER BY" + " " +
                    "dbo.syscolumns.colorder ASC" + " " +
                "SELECT" + " " +
                    "CAST(ColumnName AS varchar(max)) AS [ColumnName]," + " " +
                    "CAST(ColumnType AS varchar(max)) AS [ColumnType]," + " " +
                    "CAST(ColumnNullable AS Bit) AS [ColumnNullable]," + " " +
                    "CAST(ColumnOrder AS Int) AS [ColumnOrder]," + " " +
                    "CAST(Length AS BigInt) AS [Length]," + " " +
                    "CAST(Precision AS BigInt) AS [Precision]," + " " +
                    "CAST(IsOutParameter AS Bit) AS [IsOutParameter]" + " " +
                "FROM #t_obj" + " " +
                "DROP TABLE #t_obj";

            return sql;
        }

        /// <summary>
        /// The database procedure columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseProcedureColumnsOracle(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql +=
                "SELECT" + " " +
                    "ARGUMENT_NAME AS \"ColumnName\"," + " " +
                    "DATA_TYPE AS \"ColumnType\"," + " " +
                    "1 AS \"ColumnNullable\"," + " " +
                    "POSITION AS \"ColumnOrder\"," + " " +
                    "DATA_LENGTH AS \"Length\"," + " " +
                    "DATA_PRECISION AS \"Precision\"," + " " +
                    "DECODE(IN_OUT, 'IN', 0, 'OUT', 1, 0) AS \"IsOutParameter\"" + " " +
                "FROM ALL_ARGUMENTS" + " " +
                "WHERE OWNER = '" + dataBaseOwner.ToUpper() + "' AND OBJECT_NAME = '" + tableName.ToUpper() + "'" + " " +
                "ORDER BY POSITION";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionColumnsSQL(string dataBase, string tableName, string owner)
        {
            string sql = (!String.IsNullOrEmpty(dataBase)) ? "USE [" + dataBase + "]" + " " : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "dbo.syscolumns.name AS ColumnName," + " " +
                    "(SELECT TOP 1 dbo.systypes.name" + " " +
                    "FROM dbo.systypes" + " " +
                    "WHERE dbo.systypes.xtype = dbo.syscolumns.xtype) AS ColumnType," + " " +
                    "dbo.syscolumns.isnullable AS ColumnNullable," + " " +
                    "dbo.syscolumns.colorder AS ColumnOrder," + " " +
                    "dbo.syscolumns.length AS Length," + " " +
                    "dbo.syscolumns.prec AS Precision," + " " +
                    "dbo.syscolumns.name AS OverloadName," + " " +
                    "dbo.syscolumns.isoutparam AS IsOutParameter INTO #t_obj" + " " +
                "FROM" + " " +
                    "dbo.syscolumns INNER JOIN" + " " +
                        "dbo.sysobjects ON dbo.syscolumns.id = dbo.sysobjects.id INNER JOIN" + " " +
                            "dbo.systypes ON dbo.syscolumns.xtype = dbo.systypes.xtype" + " " +
                "WHERE" + " " +
                    "(dbo.sysobjects.name = '" + tableName + "')" + " " +
                "ORDER BY" + " " +
                    "dbo.syscolumns.colorder ASC" + " " +
                "SELECT" + " " +
                    "CAST(ColumnName AS varchar(max)) AS [ColumnName]," + " " +
                    "CAST(ColumnType AS varchar(max)) AS [ColumnType]," + " " +
                    "CAST(ColumnNullable AS Bit) AS [ColumnNullable]," + " " +
                    "CAST(ColumnOrder AS Int) AS [ColumnOrder]," + " " +
                    "CAST(Length AS BigInt) AS [Length]," + " " +
                    "CAST(Precision AS BigInt) AS [Precision]," + " " +
                    "CAST(IsOutParameter AS Bit) AS [IsOutParameter]" + " " +
                "FROM #t_obj" + " " +
                "DROP TABLE #t_obj";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionColumnsOracle(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "CONNECT " + dataBaseConnect + " SET Role" + " " : "";
            sql += 
                "SELECT" + " " +
                    "ARGUMENT_NAME AS \"ColumnName\"," + " " +
                    "DATA_TYPE AS \"ColumnType\"," + " " +
                    "1 AS \"ColumnNullable\"," + " " +
                    "POSITION AS \"ColumnOrder\"," + " " +
                    "DATA_LENGTH AS \"Length\"," + " " +
                    "DATA_PRECISION AS \"Precision\"," + " " +
                    "DECODE(IN_OUT, 'IN', 0, 'OUT', 1, 0) AS \"IsOutParameter\"," + " " +
                    "ARGUMENT_NAME AS \"OverloadName\"" + " " +
                "FROM ALL_ARGUMENTS" + " " +
                "WHERE OWNER = '" + dataBaseOwner.ToUpper() + "' AND OBJECT_NAME = '" + tableName.ToUpper() + "'" + " " +
                "ORDER BY POSITION";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionTableColumnsPostgreSql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "p.parameter_name As \"ColumnName\"," + " " +
                    "p.udt_name As \"ColumnType\"," + " " +
                    "CAST(1 As Boolean) As \"ColumnNullable\"," + " " +
                    "p.ordinal_position As \"ColumnOrder\"," + " " +
                    "CAST(0 As Bigint) As \"Length\"," + " " +
                    "CAST((SELECT t.typlen FROM pg_type t WHERE t.typname = c.udt_name) as bigint) As \"Precision\"," + " " +
                    "CAST(CASE" + " " +
                        "WHEN p.parameter_mode = 'IN' THEN 0" + " " +
                        "WHEN p.parameter_mode = 'OUT' THEN 1" + " " +
                        "ELSE 0 END As Boolean) As \"IsOutParameter\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r," + " " +
                    "information_schema.parameters p," + " " +
                    "information_schema.columns c" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_name = '" + tableName + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type = 'USER-DEFINED' And" + " " +
                    "r.specific_name = p.specific_name And" + " " +
                    "c.udt_name = p.udt_name" + " " +
                "ORDER BY p.ordinal_position ASC";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionTableColumnsMySql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "p.parameter_name As \"ColumnName\"," + " " +
                    "p.data_type As \"ColumnType\"," + " " +
                    "1 As \"ColumnNullable\"," + " " +
                    "p.ordinal_position As \"ColumnOrder\"," + " " +
                    "0 As \"Length\"," + " " +
                    "0 As \"Precision\"," + " " +
                    "CASE" + " " +
                        "WHEN p.parameter_mode = 'IN' THEN 0" + " " +
                        "WHEN p.parameter_mode = 'OUT' THEN 1" + " " +
                        "ELSE 0 END As \"IsOutParameter\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r," + " " +
                    "information_schema.parameters p," + " " +
                    "information_schema.columns c" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_name = '" + tableName + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type = 'USER-DEFINED' And" + " " +
                    "r.specific_name = p.specific_name And" + " " +
                    "c.udt_name = p.udt_name" + " " +
                "ORDER BY p.ordinal_position ASC";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseProcedureColumnsMySql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "p.parameter_name As \"ColumnName\"," + " " +
                    "p.data_type As \"ColumnType\"," + " " +
                    "1 As \"ColumnNullable\"," + " " +
                    "p.ordinal_position As \"ColumnOrder\"," + " " +
                    "0 As \"Length\"," + " " +
                    "0 As \"Precision\"," + " " +
                    "CASE" + " " +
                        "WHEN p.parameter_mode = 'IN' THEN 0" + " " +
                        "WHEN p.parameter_mode = 'OUT' THEN 1" + " " +
                        "ELSE 0 END As \"IsOutParameter\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r," + " " +
                    "information_schema.parameters p," + " " +
                    "information_schema.columns c" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_name = '" + tableName + "' And" + " " +
                    "r.routine_type = 'PROCEDURE' And" + " " +
                    "r.data_type = 'USER-DEFINED' And" + " " +
                    "r.specific_name = p.specific_name And" + " " +
                    "c.udt_name = p.udt_name" + " " +
                "ORDER BY p.ordinal_position ASC";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionScalarColumnsPostgreSql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
	                "p.parameter_name As \"ColumnName\"," + " " +
	                "p.udt_name As \"ColumnType\"," + " " +
	                "CAST(1 As Boolean) As \"ColumnNullable\"," + " " +
	                "p.ordinal_position As \"ColumnOrder\"," + " " +
	                "CAST(0 As Bigint) As \"Length\"," + " " +
	                "CAST((SELECT t.typlen FROM pg_type t WHERE t.typname = c.udt_name) as bigint) As \"Precision\"," + " " +
	                "CAST(CASE" + " " + 
		                "WHEN p.parameter_mode = 'IN' THEN 0" + " " +
		                "WHEN p.parameter_mode = 'OUT' THEN 1" + " " +
		                "ELSE 0 END As Boolean) As \"IsOutParameter\"," + " " +
	                "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " + 
	                "information_schema.routines r," + " " +
	                "information_schema.parameters p," + " " +
	                "information_schema.columns c" + " " +
                "WHERE" + " " + 
	                "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
	                "r.routine_name = '" + tableName + "' And" + " " +
	                "r.routine_type = 'FUNCTION' And" + " " +
	                "r.data_type <> 'USER-DEFINED' And" + " " +
	                "r.specific_name = p.specific_name And" + " " +
	                "c.udt_name = p.udt_name" + " " +
                "ORDER BY p.ordinal_position ASC";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionScalarColumnsMySql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "p.parameter_name As \"ColumnName\"," + " " +
                    "p.data_type As \"ColumnType\"," + " " +
                    "1 As Boolean) As \"ColumnNullable\"," + " " +
                    "p.ordinal_position As \"ColumnOrder\"," + " " +
                    "0 As \"Length\"," + " " +
                    "0 As \"Precision\"," + " " +
                    "CASE" + " " +
                        "WHEN p.parameter_mode = 'IN' THEN 0" + " " +
                        "WHEN p.parameter_mode = 'OUT' THEN 1" + " " +
                        "ELSE 0 END As \"IsOutParameter\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r," + " " +
                    "information_schema.parameters p," + " " +
                    "information_schema.columns c" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_name = '" + tableName + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type <> 'USER-DEFINED' And" + " " +
                    "r.specific_name = p.specific_name And" + " " +
                    "c.udt_name = p.udt_name" + " " +
                "ORDER BY p.ordinal_position ASC";

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionScalarReturnColumnsPostgreSql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
	                "'ReturnValue' As \"ColumnName\"," + " " +
	                "r.type_udt_name As \"ColumnType\"," + " " +
	                "CAST(1 As Boolean) As \"ColumnNullable\"," + " " +
	                "CAST(1 as Integer) As \"ColumnOrder\"," + " " +
	                "CAST(0 As Bigint) As \"Length\"," + " " +
	                "CAST((SELECT t.typlen FROM pg_type t WHERE t.typname = r.type_udt_name) as bigint) As \"Precision\"," + " " +
	                "CAST(1 As Boolean) As \"IsOutParameter\"," + " " +
	                "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " + 
	                "information_schema.routines r" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
	                "r.routine_name = '" + tableName + "' And" + " " +
	                "r.routine_type = 'FUNCTION' And" + " " +
	                "r.data_type <> 'USER-DEFINED'"; 

            return sql;
        }

        /// <summary>
        /// The database function columns
        /// </summary>
        /// <param name="dataBase">The database name.</param>
        /// <param name="tableName">The database table.</param>
        /// <param name="owner">The schema (owner) the object</param>
        /// <returns>The sql query.</returns>
        public string GetDatabaseFunctionScalarReturnColumnsMySql(string dataBaseConnect, string dataBaseOwner, string tableName)
        {
            string sql = (!String.IsNullOrEmpty(dataBaseConnect)) ? "" : "";
            sql +=
                "SELECT DISTINCT" + " " +
                    "'ReturnValue' As \"ColumnName\"," + " " +
                    "r.data_type_name As \"ColumnType\"," + " " +
                    "1 As \"ColumnNullable\"," + " " +
                    "1 As \"ColumnOrder\"," + " " +
                    "0 As \"Length\"," + " " +
                    "0 As \"Precision\"," + " " +
                    "1 As \"IsOutParameter\"," + " " +
                    "r.specific_name As \"OverloadName\"" + " " +
                "FROM" + " " +
                    "information_schema.routines r" + " " +
                "WHERE" + " " +
                    "r.specific_schema = '" + dataBaseOwner + "' And" + " " +
                    "r.routine_name = '" + tableName + "' And" + " " +
                    "r.routine_type = 'FUNCTION' And" + " " +
                    "r.data_type <> 'USER-DEFINED'";

            return sql;
        }
    }
}
