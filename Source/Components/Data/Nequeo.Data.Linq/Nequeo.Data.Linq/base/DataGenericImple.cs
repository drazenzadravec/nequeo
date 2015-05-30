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
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Linq.Expressions;
using System.ComponentModel;

using LinqTypes = Nequeo.Data.DataType.ProviderToDataTypes;

using Nequeo.Data.Linq;
using Nequeo.Data.DataType;

namespace Nequeo.Data
{
    /// <summary>
    /// Abstract base class for all data objects.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    [Serializable]
    [DataContract(Name = "DataBase", IsReference = true)]
    public abstract class DataBase<TDataEntity> : Nequeo.Data.Control.Disposable, IDataBase<TDataEntity>
        where TDataEntity : class, new()
    {
        #region DataBase Abstract Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected DataBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
        {
            _connectionType = connectionType;
            _connectionDataType = connectionDataType;
            _configurationDatabaseConnection = configurationDatabaseConnection;
            _dataAccessProvider = dataAccessProvider;
        }

        #endregion

        #region Private Fields
        private string _configurationDatabaseConnection = string.Empty;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        public virtual ISelectDataGenericBase<TDataEntity> Select
        {
            get
            {
                return new SelectDataGenericBase<TDataEntity>(
                    _configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);
            }
        }

        /// <summary>
        /// Gets, the delete generic members.
        /// </summary>
        public virtual IDeleteDataGenericBase<TDataEntity> Delete
        {
            get
            {
                return new DeleteDataGenericBase<TDataEntity>(
                    _configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);
            }
        }

        /// <summary>
        /// Gets, the insert generic members.
        /// </summary>
        public virtual IInsertDataGenericBase<TDataEntity> Insert
        {
            get
            {
                return new InsertDataGenericBase<TDataEntity>(
                    _configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);
            }
        }

        /// <summary>
        /// Gets, the update generic members.
        /// </summary>
        public virtual IUpdateDataGenericBase<TDataEntity> Update
        {
            get
            {
                return new UpdateDataGenericBase<TDataEntity>(
                    _configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);
            }
        }

        /// <summary>
        /// Gets, the common generic members.
        /// </summary>
        public virtual ICommonDataGenericBase<TDataEntity> Common
        {
            get
            {
                return new CommonDataGenericBase<TDataEntity>(
                    _configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Abstract base class for all data objects.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    [Serializable]
    [DataContract(Name = "DataBaseView", IsReference = true)]
    public abstract class DataBaseView<TDataEntity> : Nequeo.Data.Control.Disposable, IDataBaseView<TDataEntity>
        where TDataEntity : class, new()
    {
        #region DataBase Abstract Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected DataBaseView(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
        {
            _connectionType = connectionType;
            _connectionDataType = connectionDataType;
            _configurationDatabaseConnection = configurationDatabaseConnection;
            _dataAccessProvider = dataAccessProvider;
        }

        #endregion

        #region Private Fields
        private string _configurationDatabaseConnection = string.Empty;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        public virtual ISelectDataGenericBase<TDataEntity> Select
        {
            get
            {
                return new SelectDataGenericBase<TDataEntity>(
                    _configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);
            }
        }

        /// <summary>
        /// Gets, the common generic members.
        /// </summary>
        public virtual ICommonDataGenericBase<TDataEntity> Common
        {
            get
            {
                return new CommonDataGenericBase<TDataEntity>(
                    _configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The generic data base context implementation of
    /// the IQueryProvider interface provider and the
    /// IQueryable interface provider.
    /// </summary>
    public abstract class DataContextBase : IDataContextBase
    {
        #region DataContextBase Class

        /// <summary>
        /// Default constructor. Does not opens a new database connection.
        /// </summary>
        protected DataContextBase()
        {
        }

        /// <summary>
        /// Default constructor. Opens a new database connection.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected DataContextBase(string configurationDatabaseConnection,
            ConnectionContext.ConnectionType connectionType, 
            ConnectionContext.ConnectionDataType connectionDataType,
            IDataAccess dataAccessProvider)
        {
            _configurationDatabaseConnection = configurationDatabaseConnection;
            _connectionDataType = connectionDataType;
            _connectionType = connectionType;
            _dataAccessProvider = dataAccessProvider;
            OpenConnection();
        }

        private DbConnection _connection;
        private Nequeo.Data.Linq.Common.QueryPolicy _policy;
        private string _configurationDatabaseConnection = string.Empty;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        private bool _referenceLazyLoading = false;

        /// <summary>
        /// Gets, the current database connection.
        /// </summary>
        public DbConnection Connection
        {
            get { return this._connection; }
        }

        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        public IDataAccess DataAccessProvider
        {
            get { return _dataAccessProvider; }
            set { _dataAccessProvider = value; }
        }

        /// <summary>
        /// Gets sets, the foregin key reference lazy loading indicator.
        /// </summary>
        public Boolean LazyLoading
        {
            get { return _referenceLazyLoading; }
            set { _referenceLazyLoading = value; }
        }

        /// <summary>
        /// Gets sets, the current configuration database connection.
        /// </summary>
        protected string ConfigurationDatabaseConnection
        {
            get { return this._configurationDatabaseConnection; }
            set { this._configurationDatabaseConnection = value; }
        }

        /// <summary>
        /// Gets sets, the current connection type.
        /// </summary>
        protected ConnectionContext.ConnectionType DbConnectionType
        {
            get { return this._connectionType; }
            set { this._connectionType = value; }
        }

        /// <summary>
        /// Gets sets, the current connection data type.
        /// </summary>
        protected ConnectionContext.ConnectionDataType DbConnectionDataType
        {
            get { return this._connectionDataType; }
            set { this._connectionDataType = value; }
        }

        /// <summary>
        /// Gets the current connection type.
        /// </summary>
        public ConnectionContext.ConnectionType ProviderConnectionType
        {
            get { return this._connectionType; }
        }

        /// <summary>
        /// Gets the current connection data type.
        /// </summary>
        public ConnectionContext.ConnectionDataType ProviderConnectionDataType
        {
            get { return this._connectionDataType; }
        }

        /// <summary>
        /// Gets, the database sql query policy.
        /// </summary>
        protected Nequeo.Data.Linq.Common.QueryPolicy Policy
        {
            get { return this._policy; }
        }

        /// <summary>
        /// Opens a new connection.
        /// </summary>
        protected void OpenConnection()
        {
            string connection = string.Empty;

            try
            {
                // Get the connection string from the configuration key, if
                // the value is not a configuration key then exception will be thrown,
                // use the value as a connection string.
                connection = ConnectionStringsReader[_configurationDatabaseConnection].ConnectionString;
            }
            catch
            { 
                // Use the value as connection string.
                connection = _configurationDatabaseConnection; 
            }

            // Create a connection for the type.
            switch (_connectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    this._connection = _dataAccessProvider.Connection(connection);
                    this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                        new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.TSqlLanguage(_dataAccessProvider.CommandBuilder),
                            ConnectionContext.ConnectionType.SqlConnection, ConnectionContext.ConnectionDataType.SqlDataType));
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    this._connection = _dataAccessProvider.Connection(connection);
                    this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                        new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.PLSqlLanguage(_dataAccessProvider.CommandBuilder),
                            ConnectionContext.ConnectionType.OracleClientConnection, ConnectionContext.ConnectionDataType.OracleDataType));
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    this._connection = _dataAccessProvider.Connection(connection);
                    this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                        new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.PgSqlLanguage(_dataAccessProvider.CommandBuilder),
                            ConnectionContext.ConnectionType.PostgreSqlConnection, ConnectionContext.ConnectionDataType.PostgreSqlDataType));
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    this._connection = _dataAccessProvider.Connection(connection);
                    this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                        new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.MySqlLanguage(_dataAccessProvider.CommandBuilder),
                            ConnectionContext.ConnectionType.MySqlConnection, ConnectionContext.ConnectionDataType.MySqlDataType));
                    break;

                case ConnectionContext.ConnectionType.SqliteConnection:
                    this._connection = _dataAccessProvider.Connection(connection);
                    this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                        new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.SqliteLanguage(_dataAccessProvider.CommandBuilder),
                            ConnectionContext.ConnectionType.SqliteConnection, ConnectionContext.ConnectionDataType.SqliteDataType));
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    this._connection = _dataAccessProvider.Connection(connection);
                    switch (_connectionDataType)
                    {
                        case ConnectionContext.ConnectionDataType.AccessDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.AccessSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.AccessDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.OracleDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OleDbSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.OracleDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OleDbSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.PostgreSqlDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.MySqlDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OleDbSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.MySqlDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OleDbSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.ScxDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.SqlDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OleDbSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.SqlDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.SqliteDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OleDbSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.SqliteDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.None:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OleDbSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OleDbConnection, ConnectionContext.ConnectionDataType.None));
                            break;

                        default:
                            throw new Exception("No connection data type has been specified.");
                    }
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    this._connection = _dataAccessProvider.Connection(connection);
                    switch (_connectionDataType)
                    {
                        case ConnectionContext.ConnectionDataType.AccessDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.AccessDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.OracleDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.OracleDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.PostgreSqlDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.MySqlDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.MySqlDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.ScxDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.SqlDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.SqlDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.SqliteDataType:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.SqliteDataType));
                            break;

                        case ConnectionContext.ConnectionDataType.None:
                            this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                                new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.OdbcSqlLanguage(_dataAccessProvider.CommandBuilder),
                                    ConnectionContext.ConnectionType.OdbcConnection, ConnectionContext.ConnectionDataType.None));
                            break;

                        default:
                            throw new Exception("No connection data type has been specified.");
                    }
                    break;

                default:
                    this._connection = _dataAccessProvider.Connection(connection);
                    this._policy = new Nequeo.Data.Linq.Common.QueryPolicy(
                        new Nequeo.Data.Linq.Common.ImplicitMapping(new Nequeo.Data.Linq.Language.TSqlLanguage(_dataAccessProvider.CommandBuilder),
                            ConnectionContext.ConnectionType.SqlConnection, ConnectionContext.ConnectionDataType.SqlDataType));
                    break;
            }

            // Assign the policy and open a connection.
            this._connection.Open();
        }

        /// <summary>
        /// Gets, the connection strings section reader class.
        /// </summary>
        protected System.Configuration.ConnectionStringSettingsCollection ConnectionStringsReader
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings; }
        }

        /// <summary>
        /// Gets the current data entity queryable object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The queryable data object type.</returns>
        public Query<TDataEntity> GetTable<TDataEntity>()
            where TDataEntity : class, new()
        {
            // Create a new query provider.
            // Return the query object for
            // the database query provider.
            DataQueryProvider provider = new DataQueryProvider(
                this._connection, this._policy, Console.Out, typeof(TDataEntity), this);
            return new Query<TDataEntity>(provider);
        }

        /// <summary>
        /// Gets the common object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The common data entity object.</returns>
        public virtual ICommonDataGenericBase<TDataEntity> Common<TDataEntity>()
             where TDataEntity : class, new()
        {
            return new CommonDataGenericBase<TDataEntity>(
                _configurationDatabaseConnection, this._connectionType, this._connectionDataType, _dataAccessProvider);
        }

        /// <summary>
        /// Gets the select object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The insert data entity object.</returns>
        public virtual ISelectDataGenericBase<TDataEntity> Select<TDataEntity>()
            where TDataEntity : class, new()
        {
            return new SelectDataGenericBase<TDataEntity>(
                _configurationDatabaseConnection, this._connectionType, this._connectionDataType, _dataAccessProvider);
        }

        /// <summary>
        /// Gets the insert object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The insert data entity object.</returns>
        public virtual IInsertDataGenericBase<TDataEntity> Insert<TDataEntity>()
            where TDataEntity : class, new()
        {
            return new InsertDataGenericBase<TDataEntity>(
                _configurationDatabaseConnection, this._connectionType, this._connectionDataType, _dataAccessProvider);
        }

        /// <summary>
        /// Gets the update object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The update data entity object.</returns>
        public virtual IUpdateDataGenericBase<TDataEntity> Update<TDataEntity>()
            where TDataEntity : class, new()
        {
            return new UpdateDataGenericBase<TDataEntity>(
                _configurationDatabaseConnection, this._connectionType, this._connectionDataType, _dataAccessProvider);
        }

        /// <summary>
        /// Gets the delete object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The delete data entity object.</returns>
        public virtual IDeleteDataGenericBase<TDataEntity> Delete<TDataEntity>()
            where TDataEntity : class, new()
        {
            return new DeleteDataGenericBase<TDataEntity>(
                _configurationDatabaseConnection, this._connectionType, this._connectionDataType, _dataAccessProvider);
        }

        /// <summary>
        /// Executes a sql query text directly to the database.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <param name="queryText">The sql query text to execute.</param>
        /// <returns>The collection of data entities.</returns>
        public TDataEntity[] ExecuteQuery<TDataEntity>(string queryText)
             where TDataEntity : class, new()
        {
            DataTable table = null;

            // Create a connection for the type.
            switch (_connectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText, 
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.SqliteConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                default:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText, 
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

            }

            // Has data been found.
            if(table != null)
            {
                // Translate the data table to the entity type.
                Nequeo.Data.Control.AnonymousTypeFunction type = new Nequeo.Data.Control.AnonymousTypeFunction();
                return type.Translator<TDataEntity>(table);
            }
            else
                throw new Exception("No data was found, review the query text.");
        }

        /// <summary>
        /// Executes a sql query text directly to the database.
        /// </summary>
        /// <param name="queryText">The sql query text to execute.</param>
        /// <returns>The collection of data rows.</returns>
        public DataTable ExecuteQuery(string queryText)
        {
            DataTable table = null;

            // Create a connection for the type.
            switch (_connectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                case ConnectionContext.ConnectionType.SqliteConnection:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;

                default:
                    _dataAccessProvider.ExecuteQuery(ref table, queryText,
                        CommandType.Text, this._connection.ConnectionString, true, null);
                    break;
            }

            // Has data been found.
            if (table != null)
                return table;
            else
                throw new Exception("No data was found, review the query text.");
        }

        /// <summary>
        /// Executes a sql query command directly to the database.
        /// </summary>
        /// <param name="queryText">The sql query text to execute.</param>
        /// <returns>The number of rows affected.</returns>
        public Int32 ExecuteCommand(string queryText)
        {
            Int32 ret = 0;
            DbCommand sqlCommand = null;

            // Create a connection for the type.
            switch (_connectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;

                case ConnectionContext.ConnectionType.SqliteConnection:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;

                default:
                    ret = _dataAccessProvider.ExecuteCommand(ref sqlCommand, queryText,
                        CommandType.Text, this._connection.ConnectionString, null);
                    break;
            }

            // Return the number of rows affected.
            return ret;
        }

        #region Disposable class

        #region Private Fields
        private bool disposed = false;
        #endregion

        #region Dispose Object Methods.
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Note disposing has been done.
                disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_connection != null)
                    {
                        try
                        {
                            // Close the connection.
                            _connection.Close();
                        }
                        catch { }
                        _connection.Dispose();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _connection = null;
                _policy = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~DataContextBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        #endregion

        #endregion
    }

    /// <summary>
    /// Data access generic data type transaction function handler.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
    public sealed class DataTransactions<TDataEntity> : IDataTransactions<TDataEntity>
        where TDataEntity : class, new()
    {
        #region DataTransactions Class

        #region Private Fields
        private IInsertDataGenericBase<TDataEntity> _insertInstance;
        private IDeleteDataGenericBase<TDataEntity> _deleteInstance;
        private IUpdateDataGenericBase<TDataEntity> _updateInstance;
        private ISelectDataGenericBase<TDataEntity> _selectInstance;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="insertInstance">The insert instance; can be null if not in use.</param>
        /// <param name="deleteInstance">The delete instance; can be null if not in use.</param>
        /// <param name="updateInstance">The update instance; can be null if not in use.</param>
        /// <param name="selectInstance">The select instance; can be null if not in use.</param>
        public DataTransactions(
            IInsertDataGenericBase<TDataEntity> insertInstance,
            IDeleteDataGenericBase<TDataEntity> deleteInstance,
            IUpdateDataGenericBase<TDataEntity> updateInstance,
            ISelectDataGenericBase<TDataEntity> selectInstance)
        {
            _insertInstance = insertInstance;
            _deleteInstance = deleteInstance;
            _updateInstance = updateInstance;
            _selectInstance = selectInstance;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The connection string or configuration key.</param>
        /// <param name="connectionType">The database connection type.</param>
        /// <param name="connectionDataType">The database connection query type.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public DataTransactions(
            string configurationDatabaseConnection,
            ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType,
            IDataAccess dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(configurationDatabaseConnection)) throw new ArgumentNullException("configurationDatabaseConnection");

            // Create an instance of each type.
            _insertInstance = new Nequeo.Data.InsertDataGenericBase<TDataEntity>
                (configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider);
            _deleteInstance = new Nequeo.Data.DeleteDataGenericBase<TDataEntity>
                (configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider);
            _updateInstance = new Nequeo.Data.UpdateDataGenericBase<TDataEntity>
                (configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider);
            _selectInstance = new Nequeo.Data.SelectDataGenericBase<TDataEntity>
                (configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <param name="action">The insert action to execute.</param>
        public void Insert(Nequeo.Threading.ActionHandler<IInsertDataGenericBase<TDataEntity>> action)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_insertInstance == null) throw new ArgumentNullException("insertInstance");
            
            action(_insertInstance);
        }

        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The insert action to execute.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Insert<TResult>(Nequeo.Threading.FunctionHandler<TResult, IInsertDataGenericBase<TDataEntity>> action)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_insertInstance == null) throw new ArgumentNullException("insertInstance");

            return action(_insertInstance);
        }

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <param name="action">The delete action to execute.</param>
        public void Delete(Nequeo.Threading.ActionHandler<IDeleteDataGenericBase<TDataEntity>> action)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_deleteInstance == null) throw new ArgumentNullException("deleteInstance");

            action(_deleteInstance);
        }

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The delete action to execute.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Delete<TResult>(Nequeo.Threading.FunctionHandler<TResult, IDeleteDataGenericBase<TDataEntity>> action)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_deleteInstance == null) throw new ArgumentNullException("deleteInstance");

            return action(_deleteInstance);
        }

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <param name="action">The update action to execute.</param>
        public void Update(Nequeo.Threading.ActionHandler<IUpdateDataGenericBase<TDataEntity>> action)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_updateInstance == null) throw new ArgumentNullException("updateInstance");

            action(_updateInstance);
        }

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Update<TResult>(Nequeo.Threading.FunctionHandler<TResult, IUpdateDataGenericBase<TDataEntity>> action)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_updateInstance == null) throw new ArgumentNullException("updateInstance");

            return action(_updateInstance);
        }

        /// <summary>
        /// Execute a select action
        /// </summary>
        /// <param name="action">The select action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        public void Select(Nequeo.Threading.ActionHandler<ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_selectInstance == null) throw new ArgumentNullException("selectInstance");

            _selectInstance.LazyLoading = lazyLoading;
            action(_selectInstance);
        }

        /// <summary>
        /// Execute a select action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The select action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Select<TResult>(Nequeo.Threading.FunctionHandler<TResult, ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");
            if (_selectInstance == null) throw new ArgumentNullException("selectInstance");

            _selectInstance.LazyLoading = lazyLoading;
            return action(_selectInstance);
        }
        #endregion

        #region Disposable class

        #region Private Fields
        private bool disposed = false;
        #endregion

        #region Dispose Object Methods.
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this._insertInstance != null)
                        ((IDisposable)_insertInstance).Dispose();

                    if (this._deleteInstance != null)
                        ((IDisposable)_deleteInstance).Dispose();

                    if (this._updateInstance != null)
                        ((IDisposable)_updateInstance).Dispose();

                    if (this._selectInstance != null)
                        ((IDisposable)_selectInstance).Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                this._insertInstance = null;
                this._deleteInstance = null;
                this._updateInstance = null;
                this._selectInstance = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~DataTransactions()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        #endregion

        #endregion
    }

    /// <summary>
    /// Data access generic data type transaction function handler.
    /// </summary>
    public sealed class DataTransactions : IDataTransactions
    {
        #region DataTransactions Class

        #region Private Fields
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private string _configurationDatabaseConnection = string.Empty;
        private IDataAccess _dataAccessProvider;

        private Object _insertInstance = null;
        private Object _deleteInstance = null;
        private Object _updateInstance = null;
        private Object _selectInstance = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="insertInstance">The insert instance; can be null if not in use.</param>
        /// <param name="deleteInstance">The delete instance; can be null if not in use.</param>
        /// <param name="updateInstance">The update instance; can be null if not in use.</param>
        /// <param name="selectInstance">The select instance; can be null if not in use.</param>
        public DataTransactions(
            Object insertInstance,
            Object deleteInstance,
            Object updateInstance,
            Object selectInstance)
        {
            if (insertInstance != null)
            {
                // Get all interfaces of the instance.
                Type[] insertInterfaces = insertInstance.GetType().GetInterfaces();
                IEnumerable<Type> insertInterface = insertInterfaces.Where(u => u.Name == typeof(IInsertDataGenericBase<>).Name);

                // If the insert object does not implement the insert interface.
                if (insertInterface.Count() < 0)
                    throw new Exception("The insert instance does not implement IInsertDataGenericBase'1");
            }

            if (deleteInstance != null)
            {
                // Get all interfaces of the instance.
                Type[] deleteInterfaces = deleteInstance.GetType().GetInterfaces();
                IEnumerable<Type> deleteInterface = deleteInterfaces.Where(u => u.Name == typeof(IDeleteDataGenericBase<>).Name);

                // If the delete object does not implement the delete interface.
                if (deleteInterface.Count() < 0)
                    throw new Exception("The delete instance does not implement IDeleteDataGenericBase'1");
            }

            if (updateInstance != null)
            {
                // Get all interfaces of the instance.
                Type[] updateInterfaces = updateInstance.GetType().GetInterfaces();
                IEnumerable<Type> updateInterface = updateInterfaces.Where(u => u.Name == typeof(IUpdateDataGenericBase<>).Name);

                // If the update object does not implement the update interface.
                if (updateInterface.Count() < 0)
                    throw new Exception("The update instance does not implement IUpdateDataGenericBase'1");
            }

            if (selectInstance != null)
            {
                // Get all interfaces of the instance.
                Type[] selectInterfaces = selectInstance.GetType().GetInterfaces();
                IEnumerable<Type> selectInterface = selectInterfaces.Where(u => u.Name == typeof(ISelectDataGenericBase<>).Name);

                // If the select object does not implement the select interface.
                if (selectInterface.Count() < 0)
                    throw new Exception("The select instance does not implement ISelectDataGenericBase'1");
            }

            _insertInstance = insertInstance;
            _deleteInstance = deleteInstance;
            _updateInstance = updateInstance;
            _selectInstance = selectInstance;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The connection string or configuration key.</param>
        /// <param name="connectionType">The database connection type.</param>
        /// <param name="connectionDataType">The database connection query type.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public DataTransactions(
            string configurationDatabaseConnection,
            ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType,
            IDataAccess dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(configurationDatabaseConnection)) throw new ArgumentNullException("configurationDatabaseConnection");

            _configurationDatabaseConnection = configurationDatabaseConnection;
            _connectionType = connectionType;
            _connectionDataType = connectionDataType;
            _dataAccessProvider = dataAccessProvider;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The insert action to execute.</param>
        public void Insert<TDataEntity>(Nequeo.Threading.ActionHandler<IInsertDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_insertInstance == null)
                // Create an instance of each type.
                _insertInstance = new Nequeo.Data.InsertDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            action((IInsertDataGenericBase<TDataEntity>)_insertInstance);
        }

        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The insert action to execute.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Insert<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, IInsertDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_insertInstance == null)
                // Create an instance of each type.
                _insertInstance = new Nequeo.Data.InsertDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            return action((IInsertDataGenericBase<TDataEntity>)_insertInstance);
        }

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The delete action to execute.</param>
        public void Delete<TDataEntity>(Nequeo.Threading.ActionHandler<IDeleteDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_deleteInstance == null)
                // Create an instance of each type.
                _deleteInstance = new Nequeo.Data.DeleteDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            action((IDeleteDataGenericBase<TDataEntity>)_deleteInstance);
        }

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The delete action to execute.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Delete<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, IDeleteDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_deleteInstance == null)
                // Create an instance of each type.
                _deleteInstance = new Nequeo.Data.DeleteDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            return action((IDeleteDataGenericBase<TDataEntity>)_deleteInstance);
        }

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        public void Update<TDataEntity>(Nequeo.Threading.ActionHandler<IUpdateDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_updateInstance == null)
                // Create an instance of each type.
                _updateInstance = new Nequeo.Data.UpdateDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            action((IUpdateDataGenericBase<TDataEntity>)_updateInstance);
        }

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Update<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, IUpdateDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_updateInstance == null)
                // Create an instance of each type.
                _updateInstance = new Nequeo.Data.UpdateDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            return action((IUpdateDataGenericBase<TDataEntity>)_updateInstance);
        }

        /// <summary>
        /// Execute an select action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        public void Select<TDataEntity>(Nequeo.Threading.ActionHandler<ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_selectInstance == null)
                // Create an instance of each type.
                _selectInstance = new Nequeo.Data.SelectDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            ISelectDataGenericBase<TDataEntity> selectInstance = (ISelectDataGenericBase<TDataEntity>)_selectInstance;
            selectInstance.LazyLoading = lazyLoading;
            action(selectInstance);
        }

        /// <summary>
        /// Execute an select action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        /// <returns>The execute action return type.</returns>
        public TResult Select<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false)
            where TDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (action == null) throw new ArgumentNullException("action");

            if (_selectInstance == null)
                // Create an instance of each type.
                _selectInstance = new Nequeo.Data.SelectDataGenericBase<TDataEntity>
                    (_configurationDatabaseConnection, _connectionType, _connectionDataType, _dataAccessProvider);

            ISelectDataGenericBase<TDataEntity> selectInstance = (ISelectDataGenericBase<TDataEntity>)_selectInstance;
            selectInstance.LazyLoading = lazyLoading;
            return action(selectInstance);
        }
        #endregion

        #region Disposable class

        #region Private Fields
        private bool disposed = false;
        #endregion

        #region Dispose Object Methods.
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this._insertInstance != null)
                        ((IDisposable)_insertInstance).Dispose();

                    if (this._deleteInstance != null)
                        ((IDisposable)_deleteInstance).Dispose();

                    if (this._updateInstance != null)
                        ((IDisposable)_updateInstance).Dispose();

                    if (this._selectInstance != null)
                        ((IDisposable)_selectInstance).Dispose();

                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                this._insertInstance = null;
                this._deleteInstance = null;
                this._updateInstance = null;
                this._selectInstance = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~DataTransactions()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        #endregion

        #endregion
    }

    /// <summary>
    /// Function routine transation handler.
    /// </summary>
    public class FunctionTransaction : Nequeo.Data.Control.FunctionBase
    {
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public FunctionTransaction(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider) :
            base(configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider) { }
    }
}
