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
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;

using LinqTypes = Nequeo.Data.DataType.ProviderToDataTypes;

using Nequeo.Data.Control;
using Nequeo.Data.DataType;

namespace Nequeo.Data.DataSet
{
    /// <summary>
    /// The abstract base data access class for all data access objects.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public abstract class SchemaDataSetGenericBase<TDataContext, TDataTable> : Nequeo.Handler.LogHandler, IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region SchemaDataSetGenericBase Abstract Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaItemName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected SchemaDataSetGenericBase(string schemaItemName, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(applicationName, eventNamespace)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaItemName)) throw new ArgumentNullException("schemaItemName");

            // Set the current data schema item object name.
            this._connectionType = connectionType;
            this._connectionDataType = connectionDataType;
            _dataAccessProvider = dataAccessProvider;

            this._schemaItemName = schemaItemName;
            _dataTypeConversion = new DataTypeConversion(connectionDataType);
            this.Initialise(string.Empty);
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo.Data.Client";
        private const string eventNamespace = "Nequeo.Data.Client.DataSet";
        #endregion

        #region Private Fields
        private string _fileLogErrorPath = string.Empty;
        private string _fileLogProcessPath = string.Empty;
        private string _schemaItemName = string.Empty;

        private string _specificPath = string.Empty;
        private string _errorProvider = string.Empty;
        private string _informationProvider = string.Empty;
        private Nequeo.Handler.WriteTo _errorWriteTo = Nequeo.Handler.WriteTo.EventLog;
        private Nequeo.Handler.WriteTo _informationWriteTo = Nequeo.Handler.WriteTo.EventLog;

        private TDataContext _dataContext = null;
        private string _configurationDatabaseConnection = string.Empty;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        private DataTypeConversion _dataTypeConversion = null;

        private bool _useBulkInsert = true;
        private bool _useBulkDelete = true;
        private bool _useBulkUpdate = true;
        #endregion

        #region Protected Properties
        /// <summary>
        /// Get set, this property holds the override location, when
        /// writing to an error/process log location.
        /// </summary>
        protected Nequeo.Handler.WriteTo OverrideLocation
        {
            get { return base.OverrideWriteLocation; }
            set { base.OverrideWriteLocation = value; }
        }

        /// <summary>
        /// Get, the last error that has occurred.
        /// </summary>
        protected string GetLastError
        {
            get { return base.GetLastErrorDescription(); }
        }

        /// <summary>
        /// Gets, the current DataTypeConversion object.
        /// </summary>
        protected DataTypeConversion DataTypeConversion
        {
            get { return _dataTypeConversion; }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the data access provider
        /// </summary>
        public IDataAccess DataAccessProvider
        {
            get { return _dataAccessProvider; }
            set { _dataAccessProvider = value; }
        }

        /// <summary>
        /// Gets sets, use bulk insert statements to insert multiple records
        /// else insert each record one-by-one.
        /// </summary>
        public bool UseBulkInsert
        {
            get { return _useBulkInsert; }
            set { _useBulkInsert = value; }
        }

        /// <summary>
        /// Gets sets, use bulk delete statements to delete multiple records
        /// else delete each record one-by-one.
        /// </summary>
        public bool UseBulkDelete
        {
            get { return _useBulkDelete; }
            set { _useBulkDelete = value; }
        }

        /// <summary>
        /// Gets sets, use bulk update statements to update multiple records
        /// else update each record one-by-one.
        /// </summary>
        public bool UseBulkUpdate
        {
            get { return _useBulkUpdate; }
            set { _useBulkUpdate = value; }
        }

        /// <summary>
        /// Gets sets, the database configuration key containing
        /// the database connection string.
        /// </summary>
        public String ConfigurationDatabaseConnection
        {
            get { return _configurationDatabaseConnection; }
            set { _configurationDatabaseConnection = value; }
        }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        public ConnectionContext.ConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; }
        }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        public ConnectionContext.ConnectionDataType ConnectionDataType
        {
            get { return _connectionDataType; }
            set
            {
                _connectionDataType = value;
                _dataTypeConversion.ConnectionDataType = value;
            }
        }

        /// <summary>
        /// Gets sets the current data context object.
        /// </summary>
        public TDataContext DataContext
        {
            get { return CreateDataContextInstance(); }
            set { _dataContext = value; }
        }

        /// <summary>
        /// Sets, The specific path of the configuration file, 
        /// used for web applications.
        /// </summary>
        public String ConfigurationPath
        {
            get { return _specificPath; }
            set { Initialise(value); }
        }

        /// <summary>
        /// Gets the currrent error for the operation.
        /// </summary>
        public string Error
        {
            get { return _errorProvider; }
        }

        /// <summary>
        /// Gets the current information for the operation.
        /// </summary>
        public string Information
        {
            get { return _informationProvider; }
        }

        /// <summary>
        /// Gets sets the location where errors are to be written.
        /// </summary>
        public Nequeo.Handler.WriteTo ErrorWriteTo
        {
            get { return _errorWriteTo; }
            set { _errorWriteTo = value; }
        }

        /// <summary>
        /// Gets sets the location where information is to be written.
        /// </summary>
        public Nequeo.Handler.WriteTo InformationWriteTo
        {
            get { return _informationWriteTo; }
            set { _informationWriteTo = value; }
        }

        /// <summary>
        /// Gets, the dataset data collection configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.DataSetDataCollection DataSetDataCollectionReader
        {
            get { return Nequeo.Data.Configuration.DataSetDataConfigurationManager.DataSetDataCollection(); }
        }

        /// <summary>
        /// Gets, the dataset data default configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.DataSetDataElementDefault DataSetDataElementReader
        {
            get { return Nequeo.Data.Configuration.DataSetDataConfigurationManager.DataSetDataElement(); }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// Event handler for providing error information.
        /// </summary>
        public event Nequeo.Threading.EventHandler<InformationProviderArgs> OnErrorProvider;

        /// <summary>
        /// Event handler for providing information.
        /// </summary>
        public event Nequeo.Threading.EventHandler<InformationProviderArgs> OnInformationProvider;
        #endregion

        #region Private Methods
        /// <summary>
        /// Initia;ise the logging locations.
        /// </summary>
        /// <param name="specificPath">The specifiec configuration path.</param>
        private void Initialise(string specificPath)
        {
            // Set the log file path when logging information.
            // Using configuration data to specify the location.
            using (Nequeo.Handler.Global.ProcessLogging processLogging = new Nequeo.Handler.Global.ProcessLogging())
            {
                _fileLogErrorPath = processLogging.ErrorLoggingFilePath(
                    eventNamespace + ".log", specificPath);

                _fileLogProcessPath = processLogging.ProcessLoggingFilePath(
                    eventNamespace + ".log", specificPath);

                // Set the log file path.
                this.LogErrorFilePath = _fileLogErrorPath;
                this.LogProcessFilePath = _fileLogProcessPath;
                _specificPath = specificPath;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Assign DB Null value to any null type.
        /// </summary>
        /// <param name="value">The object value to check.</param>
        /// <returns>Return the DB Null value else return the passed value.</returns>
        protected object AssignDBNullValue(object value)
        {
            try
            {
                if (value == null)
                    return DBNull.Value;
                else
                    return value;
            }
            catch { return value; }
        }
        #endregion

        #region Public Connection Methods
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        public string DefaultConnection(string configurationDatabaseConnection)
        {
            string providerName = null;
            string connection = string.Empty;

            // Get the current database connection string
            // from the configuration file through the
            // specified configuration key.
            using (Nequeo.Handler.Global.DatabaseConnections databaseConnection = new Nequeo.Handler.Global.DatabaseConnections())
                connection = databaseConnection.DatabaseConnection(configurationDatabaseConnection, out providerName);

            // If empty string is returned then
            // value should be the connection string.
            if (String.IsNullOrEmpty(connection))
                return configurationDatabaseConnection;
            else
                return connection;
        }

        /// <summary>
        /// Gets the alternative database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        public string AlternativeConnection(string configurationDatabaseConnection)
        {
            string providerName = null;
            string connection = string.Empty;

            // Get the current database connection string
            // from the configuration file through the
            // specified configuration key.
            using (Nequeo.Handler.Global.DatabaseConnections databaseConnection = new Nequeo.Handler.Global.DatabaseConnections())
                connection = databaseConnection.DatabaseConnection(configurationDatabaseConnection, out providerName);

            // If empty string is returned then
            // value should be the connection string.
            if (String.IsNullOrEmpty(connection))
                return configurationDatabaseConnection;
            else
                return connection;
        }

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        public DbConnection StartDefaultConnection(string databaseConnection)
        {
            DbConnection dbConnection = null;
            switch (_connectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                default:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;
            }
            return dbConnection;
        }

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        public DbConnection StartAlternativeConnection(string databaseConnection)
        {
            DbConnection dbConnection = null;
            switch (_connectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;

                default:
                    dbConnection = _dataAccessProvider.Connection(databaseConnection);
                    break;
            }
            return dbConnection;
        }
        #endregion

        #region Public Data Context Methods
        /// <summary>
        /// Create a new data context instance.
        /// </summary>
        /// <returns>The new instance of the data context.</returns>
        /// <remarks>This data context uses the default connection 
        /// string in the application configuration file.
        /// <para>.</para>
        /// <para>[connectionStrings]</para>
        /// <para>[add name="Nequeo.Data.Client.Access.Properties.Settings.Nequeo.Data.ClientBaseConnectionString"</para>
        /// <para>connectionString="Data Source=DEVELOP\SQL2005DEV;Initial Catalog=Nequeo.Data.ClientBase;Integrated Security=True"</para>
        /// <para>providerName="System.Data.SqlClient" /]</para>
        /// <para>[/connectionStrings]</para></remarks>
        public TDataContext CreateDataContextInstance()
        {
            // If the data context is not null
            // then dispose of the data context.
            if (_dataContext == null)
                // Create a new instance of the data context
                // return the new data context instance.
                _dataContext = new TDataContext();

            // Return the data context
            // instance.
            return _dataContext;
        }

        /// <summary>
        /// Dispose of the current data context instance.
        /// </summary>
        public void DisposeDataContextInstance()
        {
            // If the data context is not null
            // then dispose of the data context.
            if (_dataContext != null)
                _dataContext.Dispose();

            // Assign the null value to the current data context.
            _dataContext = null;
        }
        #endregion

        #region Public Reflection, Attribute Methods
        /// <summary>
        /// Get all fields within the current type.
        /// </summary>
        /// <param name="t">The current type to retreive fields within.</param>
        /// <returns>The collection of all fields within the type.</returns>
        public List<FieldInfo> GetFields(Type t)
        {
            // Create a new instance of the field collection.
            List<FieldInfo> fields = new List<FieldInfo>();

            // Get the base type and the derived type.
            Type type = t;

            // Add the complete field range.
            fields.AddRange(type.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            // Return all the fields within
            // the type.
            return fields;
        }

        /// <summary>
        /// Get all properties within the current type.
        /// </summary>
        /// <param name="t">The current type to retreive properties within.</param>
        /// <returns>The collection of all properties within the type.</returns>
        public List<PropertyInfo> GetProperties(Type t)
        {
            // Create a new instance of the property collection.
            List<PropertyInfo> properties = new List<PropertyInfo>();

            // Get the base type and the derived type.
            Type type = t;

            // Add the complete property range.
            properties.AddRange(type.GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            // Return all the properties within
            // the type.
            return properties;
        }

        /// <summary>
        /// Get all columns in the current type that are primary keys.
        /// </summary>
        /// <returns>Collection of columns that are primary keys.</returns>
        public DataColumn[] GetAllPrimaryKeys()
        {
            DataColumn[] primaryKeys = (new TDataTable()).PrimaryKey;

            // return the collection of properties.
            return primaryKeys;
        }

        /// <summary>
        /// Get all properties in the current type that are primary keys.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>Collection of properties that are primary keys.</returns>
        public DataColumn[] GetAllPrimaryKeys<T>()
            where T : System.Data.DataTable, new()
        {
            DataColumn[] primaryKeys = (new T()).PrimaryKey;

            // return the collection of properties.
            return primaryKeys;
        }

        
        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        public DataColumn GetPrimaryKey()
        {
            DataColumn[] primaryKeys = (new TDataTable()).PrimaryKey;

            // return the collection of properties.
            return primaryKeys.Length > 0 ? primaryKeys[0] : null;
        }

        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        public DataColumn GetPrimaryKey<T>()
            where T : System.Data.DataTable, new()
        {
            DataColumn[] primaryKeys = (new T()).PrimaryKey;

            // return the collection of properties.
            return primaryKeys.Length > 0 ? primaryKeys[0] : null;
        }

        
        /// <summary>
        /// Is the current column value a primary key.
        /// </summary>
        /// <param name="column">The current column to examine.</param>
        /// <returns>True if the column is a primary key.</returns>
        public bool IsPrimaryKey(DataColumn column)
        {
            DataColumn[] primaryKeys = (new TDataTable()).PrimaryKey;
            bool ret = primaryKeys.Contains(column);

            // Return true if the column is a primary key.
            return ret;
        }

        /// <summary>
        /// Is the current column value a primary key.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="column">The current column to examine.</param>
        /// <returns>True if the column is a primary key.</returns>
        public bool IsPrimaryKey<T>(DataColumn column)
            where T : System.Data.DataTable, new()
        {
            DataColumn[] primaryKeys = (new T()).PrimaryKey;
            bool ret = primaryKeys.Contains(column);

            // Return true if the column is a primary key.
            return ret;
        }
        #endregion

        #region Public Virtual Provider Methods
        /// <summary>
        /// Provides error information in a method when called.
        /// </summary>
        /// <param name="exception">An exception that has been constructed.</param>
        /// <param name="method">The method the error occurred in.</param>
        /// <param name="methodDescription">The description of the method.</param>
        public virtual void ErrorProvider(Exception exception, string method, string methodDescription)
        {
            _errorProvider = exception.Message + " " + method + " " + methodDescription;
            Write(typeof(TDataTable).GetType().FullName, method + ". Description : " + methodDescription,
                exception.Message, 401, _errorWriteTo, Nequeo.Handler.LogType.Error);

            if (OnErrorProvider != null)
                OnErrorProvider(this, new InformationProviderArgs(_errorProvider));
            else
                throw new Exception(_errorProvider);
        }

        /// <summary>
        /// Provides information back to the derived class.
        /// </summary>
        /// <param name="method">The method the error occurred in.</param>
        /// <param name="information">The information that is provided.</param>
        public virtual void InformationProvider(string method, string information)
        {
            _informationProvider = method + " " + information;
            Write(typeof(TDataTable).GetType().FullName, method, information, 201,
                 _informationWriteTo, Nequeo.Handler.LogType.Process);

            if (OnInformationProvider != null)
                OnInformationProvider(this, new InformationProviderArgs(_informationProvider));
        }
        #endregion

        #region Internal Type Convertion Methods
        /// <summary>
        /// Convert a null object to null string.
        /// </summary>
        /// <param name="value">The original value to check.</param>
        /// <returns>The value or a null indicator.</returns>
        internal string ConvertNullToString(object value)
        {
            return (value == null) ? "Null" : value.ToString();
        }

        /// <summary>
        /// Convert a null object to null string.
        /// </summary>
        /// <param name="value">The original value to check.</param>
        /// <returns>The value or a null indicator.</returns>
        internal string ConvertNullTypeToString(object value)
        {
            return (value == null) ? "Null" : value.GetType().FullName;
        }

        /// <summary>
        /// Converts a linq binay type to byte array.
        /// </summary>
        /// <param name="value">The original value in the linq entity.</param>
        /// <param name="propertyType">The current property type.</param>
        /// <returns>The new byte array or original value.</returns>
        internal object ConvertBinaryType(object value, Type propertyType)
        {
            // The value is a linq binary type
            // and the property type is byte array.
            if ((value is System.Data.Linq.Binary) &&
                (propertyType == typeof(Byte[])))
            {
                // Convert the current value
                // to a byte array.
                System.Data.Linq.Binary binaryValue = (System.Data.Linq.Binary)value;
                return binaryValue.ToArray();
            }
            else
                return value;
        }
        #endregion

        #region Public Execute Query Methods
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref TDataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
        {
            return ExecuteQuery<TDataTable>(ref dataTable, queryText,
                    commandType, connectionString, values);
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

            try
            {
                switch (this.ConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, values);
                        break;
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
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataTable, queryText, commandType,
                DefaultConnection(ConfigurationDatabaseConnection), getSchemaTable, values);
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

            try
            {
                switch (this.ConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;
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

            try
            {
                switch (this.ConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    default:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;
                }

                // Return true.
                return returnValue;
            }
            catch (Exception ex)
            {
                // Throw a general exception.
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The tables names to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataSet, tables, queryText, commandType,
                DefaultConnection(ConfigurationDatabaseConnection), values);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The datatable schema to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            return ExecuteQuery(ref dataSet, tables, queryText, commandType,
                DefaultConnection(ConfigurationDatabaseConnection), values);
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

            try
            {
                switch (this.ConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;
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

            try
            {
                switch (this.ConnectionType)
                {
                    case ConnectionContext.ConnectionType.SqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, values);
                        break;
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
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public class DataSetDataGenericBase<TDataContext, TDataTable> :
        SchemaDataSetGenericBase<TDataContext, TDataTable>,
        IDataSetDataGenericBase<TDataContext, TDataTable>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region DataSetDataGenericBase Base Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public DataSetDataGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");

            _schemaName = schemaName;
        }
        #endregion

        #region Private Fields
        private string _schemaName = String.Empty;
        #endregion

        #region Public Virtual Convertion Methods

        /// <summary>
        /// Convert all the object data into a data table.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with IQueryable anonymous types.</returns>
        public virtual DataTable ConvertToDataTable(IQueryable query, string tableName)
        {
            Nequeo.Data.Control.AnonymousTypeFunction functions =
                new Nequeo.Data.Control.AnonymousTypeFunction();

            // Return the data table.
            return functions.GetDataTable(query, tableName);
        }

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="data">The object array containing the data.</param>
        /// <returns>The array of the type of object.</returns>
        public virtual T[] ConvertToTypeArray<T>(Object[] data)
            where T : new()
        {
            Nequeo.Data.Control.AnonymousTypeFunction functions =
                new Nequeo.Data.Control.AnonymousTypeFunction();

            // Return the type array.
            return functions.GetTypeDataArray<T>(data);
        }

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of the type of object.</returns>
        public virtual T[] ConvertToTypeArray<T>(IQueryable query)
            where T : new()
        {
            Nequeo.Data.Control.AnonymousTypeFunction functions =
                new Nequeo.Data.Control.AnonymousTypeFunction();

            // Return the type array.
            return functions.GetTypeDataArray<T>(query);
        }

        /// <summary>
        /// Converts a data table to the specified type array.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataTable">The data table to convert.</param>
        /// <returns>The array of the type.</returns>
        public virtual T[] ConvertToTypeArray<T>(DataTable dataTable)
            where T : new()
        {
            Nequeo.Data.Control.AnonymousTypeFunction functions =
                new Nequeo.Data.Control.AnonymousTypeFunction();

            // Return the type array.
            return functions.GetListCollection<T>(dataTable).ToArray();
        }

        /// <summary>
        /// Convert all the object data into a IQueryable generic type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="data">The object array containing the data.</param>
        /// <returns>The IQueryable generic type.</returns>
        public virtual IQueryable<T> ConvertToIQueryable<T>(Object[] data)
            where T : new()
        {
            // Declare the current query type.
            IQueryable<T> query = null;

            // If the data object is enumerable, that is, is the
            // data object an array or a collection object.
            if (data is IEnumerable)
            {
                // If the data collection contains data.
                if (data.Count() > 0)
                {
                    int i = 0;

                    // Create a new instance of the
                    // current linq entity type.
                    T[] dataItems = new T[data.Count()];

                    // Get the data object set enumerator.
                    IEnumerator dataEnum = data.GetEnumerator();

                    // For each data item found.
                    while (dataEnum.MoveNext())
                        // Cast the current object data item
                        // to the current linq entity type.
                        dataItems[i++] = (T)dataEnum.Current;

                    // Cast the current linq entity IEnumerable collection
                    // to the IQueryable collection of linq entities.
                    query = dataItems.AsQueryable<T>();
                }
            }

            // return the IQueryable generic object. 
            return query;
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Gets the current information on the current class.
        /// </summary>
        /// <returns>The string containg the information.</returns>
        public override string ToString()
        {
            return string.Format("Context = {0}, Linq Entity = {1}, Data Entity = {2}",
                DataContext.GetType().FullName, typeof(TDataTable).GetType().FullName, typeof(TDataTable).GetType().FullName);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The select base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public class SelectDataSetGenericBase<TDataContext, TDataTable> :
        SchemaDataSetGenericBase<TDataContext, TDataTable>,
        ISelectDataSetGenericBase<TDataContext, TDataTable>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region Select Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public SelectDataSetGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");

            _schemaName = schemaName;
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="specificPath">The specific path of the config file, 
        /// used for web applications.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public SelectDataSetGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");
            if (String.IsNullOrEmpty(specificPath)) throw new ArgumentNullException("specificPath");

            _schemaName = schemaName;
            base.ConfigurationPath = specificPath;
        }
        #endregion

        #region Private Fields
        private string _schemaName = String.Empty;
        private string _cachedItemName = string.Empty;
        private bool _cacheItems = false;
        private Int32 _cacheTimeout = 120;

        private TDataTable _dataTable = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets an indicator specifying that
        /// the current selected item(s) should be
        /// stored and retreived from the cache.
        /// </summary>
        public bool CacheItems
        {
            get { return _cacheItems; }
            set { _cacheItems = value; }
        }

        /// <summary>
        /// Gets sets the key name of the cached item.
        /// </summary>
        public string CachedItemName
        {
            get { return _cachedItemName; }
            set { _cachedItemName = value; }
        }

        /// <summary>
        /// Gets sets the length of time in seconds the item(s)
        /// are stored in the cache before being removed.
        /// </summary>
        public Int32 CacheTimeout
        {
            get { return _cacheTimeout; }
            set { _cacheTimeout = value; }
        }

        /// <summary>
        /// Gets the data table containing the
        /// collection of table data.
        /// </summary>
        public TDataTable DataTable
        {
            get { return _dataTable; }
        }
        #endregion

        #region Public Caching Control Methods
        /// <summary>
        /// Sets all relevant properties to indicate
        /// that caching is to be used for all selection
        /// operations.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        /// <param name="cacheTimeout">The length of time in second the item(s)
        /// are stored in the cache before being removed.</param>
        /// <returns>The current select object.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public ISelectDataSetGenericBase<TDataContext, TDataTable>
            AddCachingControl(string cachedItemName, Int32 cacheTimeout)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(cachedItemName)) throw new ArgumentNullException("cachedItemName");
            if (cacheTimeout < 0) throw new IndexOutOfRangeException("Cache timeout must be non-negative");

            _cacheItems = true;
            _cacheTimeout = cacheTimeout;
            _cachedItemName = cachedItemName;
            return this;
        }

        /// <summary>
        /// Sets all relevant properties to indicate
        /// that caching is not to be used.
        /// </summary>
        public void RemoveCachingControl()
        {
            _cacheItems = false;
            _cacheTimeout = 120;
            _cachedItemName = string.Empty;
        }

        /// <summary>
        /// Get the specified item from the cache.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        /// <returns>The object that was cached or null if the object does not exist.</returns>
        public Object GetItemFromCache(string cachedItemName)
        {
            return Nequeo.Caching.RuntimeCache.Get(cachedItemName);
        }

        /// <summary>
        /// Remove the specified item from the cache.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        public void RemoveItemFromCache(string cachedItemName)
        {
            Nequeo.Caching.RuntimeCache.Remove(cachedItemName);
        }

        /// <summary>
        /// Adds the object to the cache.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        /// <param name="cacheTimeout">The length of time in second the item(s)
        /// are stored in the cache before being removed.</param>
        /// <param name="value">The object to add to the cache.</param>
        public void AddItemToCache(string cachedItemName, Int32 cacheTimeout, Object value)
        {
            Nequeo.Caching.RuntimeCache.CacheDuration = cacheTimeout;
            Nequeo.Caching.RuntimeCache.Add(_schemaName, cachedItemName, value, (double)cacheTimeout);
        }
        #endregion

        #region Public Virtual Select DataTable Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataTable SelectDataTable(string queryText, CommandType commandType,
            params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            return SelectDataTable<TDataTable>(queryText, commandType, values);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataTable SelectDataTable<TypeDataTable>(string queryText, 
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            TypeDataTable data = null;
            DbCommand sqlCommand = SelectCollection<TypeDataTable>(ref data, queryText,
                commandType, values);

            // Return the data table
            // else return null.
            return (sqlCommand != null) ? data : null;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataTable SelectDataTable(string queryText)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            return SelectDataTable<TDataTable>(queryText);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataTable SelectDataTable<TypeDataTable>(string queryText)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            TypeDataTable data = null;
            DbCommand sqlCommand = SelectCollection<TypeDataTable>(ref data, 
                queryText);

            // Return the data table
            // else return null.
            return (sqlCommand != null) ? data : null;
        }

        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataTable SelectDataTable()
        {
            // Return the data table.
            return SelectDataTable<TDataTable>();
        }

        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataTable SelectDataTable<TypeDataTable>()
            where TypeDataTable : System.Data.DataTable, new()
        {
            TypeDataTable data = null;
            DbCommand sqlCommand = SelectCollection<TypeDataTable>(ref data);

            // Return the data table
            // else return null.
            return (sqlCommand != null) ? data : null;
        }
        #endregion

        #region Public Virtual Select Table Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>True if data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectTable(string queryText, CommandType commandType, 
            params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            DbCommand sqlCommand = SelectCollection<TDataTable>(ref _dataTable, queryText,
                commandType, values);

            // Return true if not null
            // else return false.
            return (sqlCommand != null);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>True if data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectTable(string queryText)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            DbCommand sqlCommand = SelectCollection<TDataTable>(ref _dataTable, queryText);

            // Return true if not null
            // else return false.
            return (sqlCommand != null);
        }

        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <returns>True if data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectTable()
        {
            DbCommand sqlCommand = SelectCollection<TDataTable>(ref _dataTable);

            // Return true if not null
            // else return false.
            return (sqlCommand != null);
        }
        #endregion

        #region Public Virtual Select Collection Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollection(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return SelectEx<TDataTable>(ref dataTable, queryText,
                    commandType, values);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollection<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return SelectEx<TypeDataTable>(ref dataTable, queryText,
                    commandType, values);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollection(ref TDataTable dataTable, string queryText)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return SelectEx<TDataTable>(ref dataTable, queryText,
                CommandType.Text, null);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollection<TypeDataTable>(ref TypeDataTable dataTable, string queryText)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return SelectEx<TypeDataTable>(ref dataTable, queryText,
                CommandType.Text, null);
        }

        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollection(ref TDataTable dataTable)
        {
            string queryText = "SELECT * FROM [" + (new TDataTable()).TableName.Replace(".", "].[") + "]";
            return SelectEx<TDataTable>(ref dataTable, queryText,
                CommandType.Text, null);
        }

        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollection<TypeDataTable>(ref TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new()
        {
            string queryText = "SELECT * FROM [" + (new TypeDataTable()).TableName.Replace(".", "].[") + "]";
            return SelectEx<TypeDataTable>(ref dataTable, queryText,
                CommandType.Text, null);
        }
        #endregion

        #region Public Virtual Select Predicate Methods

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TDataRow">The current data row type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data table type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataTable SelectDataTablePredicate<TDataRow>(
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataRow>> predicate)
            where TDataRow : System.Data.DataRow
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataRow>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionEvaluator expression = new Nequeo.Data.Linq.ExpressionEvaluator();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + (new TDataTable()).TableName.Replace(".", "].[") + "] " +
                "WHERE " + whereQueryText;

            // Execute the query. return the result
            // into the data table.
            DbCommand sqlCommand = SelectEx<TDataTable>(ref _dataTable, queryText, CommandType.Text, null);

            // Return the data table .
            return _dataTable;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <typeparam name="TypeDataRow">The current data row type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data table type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataTable SelectDataTablePredicate<TypeDataTable, TypeDataRow>(
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataRow>> predicate)
            where TypeDataTable : System.Data.DataTable, new()
            where TypeDataRow : System.Data.DataRow
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataRow>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionEvaluator expression = new Nequeo.Data.Linq.ExpressionEvaluator();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + (new TypeDataTable()).TableName.Replace(".", "].[") + "] " +
                "WHERE " + whereQueryText;

            // Execute the query. return the result
            // into the data table.
            TypeDataTable data = null;
            DbCommand sqlCommand = SelectEx<TypeDataTable>(ref data, queryText, CommandType.Text, null);

            // Return the data table
            // else return null.
            return (sqlCommand != null) ? data : null;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataTable SelectDataTablePredicate(
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Return the data table.
            return SelectDataTablePredicate<TDataTable>(predicate, values);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataTable SelectDataTablePredicate<TypeDataTable>(
            string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            TypeDataTable data = null;
            DbCommand sqlCommand = SelectCollectionPredicate<TypeDataTable>(ref data,
                predicate, values);

            // Return the data table
            // else return null.
            return (sqlCommand != null) ? data : null;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollectionPredicate(ref TDataTable dataTable,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TDataTable>(ref dataTable,
                predicate, values);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollectionPredicate<TypeDataTable>(ref TypeDataTable dataTable,
            string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Create a new internal table instance
            // assign the initial predicate string.
            string predicateInternal = predicate;

            // Format the first paramerter found.
            string currentPredicate = predicate.Replace("&&", "AND").Replace("||", "OR").Replace("==", "=").Replace("!=", "<>");

            if (values != null)
            {
                if (values.Count() > 0)
                {
                    // For each parameter value found
                    // interate through the collection
                    // and replace the precidate values.
                    for(int i = 1; i < values.Count(); i++)
                    {
                        predicateInternal = currentPredicate.Replace("@" + i, base.DataTypeConversion.GetSqlStringValue(values[i].GetType(), values[i]));
                        currentPredicate = predicateInternal;
                    }
                }
            }

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + (new TypeDataTable()).TableName.Replace(".", "].[") + "] " +
                "WHERE " + predicateInternal;

            // Execute the query.
            return SelectEx<TypeDataTable>(ref dataTable, queryText,
                CommandType.Text, null);
        }
        #endregion

        #region Public Virtual Select Query Methods
        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return SelectQueryItem<TDataTable>(ref dataTable, queryText, commandType, values);
        }

        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery<TypeDataTable>(ref dataTable, queryText,
                    commandType, base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }

        /// <summary>
        /// Select the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Int32 SelectCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            return ExecuteCommand(ref sqlCommand, commandText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }
        #endregion

        #region Public Asynchronous Select Methods
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TDataRow">The current data row type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTableRowPredicate<TDataRow>(AsyncCallback callback, object state,
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataRow>> predicate)
            where TDataRow : System.Data.DataRow
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTableRowPredicate<TDataContext, TDataTable, TDataRow>(
                this, callback, state, predicate);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TDataRow">The current data row type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public TDataTable EndSelectDataTableRowPredicate<TDataRow>(IAsyncResult ar)
            where TDataRow : System.Data.DataRow
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncSelectDataTableRowPredicate<TDataContext, TDataTable, TDataRow>.End(ar);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <typeparam name="TypeDataRow">The current data row type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTableRowPredicate<TypeDataTable, TypeDataRow>(AsyncCallback callback, object state,
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataRow>> predicate)
            where TypeDataTable : System.Data.DataTable, new()
            where TypeDataRow : System.Data.DataRow
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTableRowPredicate<TDataContext, TDataTable, TypeDataTable, TypeDataRow>(
                this, callback, state, predicate);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <typeparam name="TypeDataRow">The current data row type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public TypeDataTable EndSelectDataTableRowPredicate<TypeDataTable, TypeDataRow>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
            where TypeDataRow : System.Data.DataRow
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncSelectDataTableRowPredicate<TDataContext, TDataTable, TypeDataTable, TypeDataRow>.End(ar);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTablePredicate(AsyncCallback callback, object state,
            string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTablePredicate<TDataContext, TDataTable>(
                this, callback, state, predicate, values);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public TDataTable EndSelectDataTablePredicate(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncSelectDataTablePredicate<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTablePredicate<TypeDataTable>(AsyncCallback callback, object state,
            string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTablePredicate<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, predicate, values);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public TypeDataTable EndSelectDataTablePredicate<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncSelectDataTablePredicate<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }

        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable>(
                this, callback, state, queryText, commandType, values);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state,
            string queryText)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable>(
                this, callback, state, queryText);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable>(
                this, callback, state);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public TDataTable EndSelectDataTable(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable<TypeDataTable>(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, queryText, commandType, values);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable<TypeDataTable>(AsyncCallback callback, object state,
            string queryText)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, queryText);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable<TypeDataTable>(AsyncCallback callback, object state)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public TypeDataTable EndSelectDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncSelectDataTable<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }

        #endregion

        #region Select Private Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        private DbCommand SelectEx<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Initial connection objects.
            DbCommand sqlCommand = null;

            // Cache control.
            Object item = null;
            bool ret = false;

            // If cache control has been specified.
            if (_cacheItems)
            {
                // If a valid cache name has
                // not been specified then throw
                // an exception.
                if (String.IsNullOrEmpty(_cachedItemName))
                    throw new Exception("The cached item name has not been specified.");

                // Get the item from the cache.
                item = GetItemFromCache(_cachedItemName);

                // If the item is null then no item
                // was found in the cache. If the item
                // is not null then the item has been found.
                if (item != null)
                {
                    // If the item in the cache is an
                    // object array then this is the right item.
                    if (item.GetType() == typeof(DataTable))
                    {
                        // If the data table is the
                        // current type.
                        if (item is TypeDataTable)
                        {
                            TypeDataTable table = (TypeDataTable)item;
                            dataTable = table;
                            ret = true;
                        }
                    }
                }
            }

            // If no item in cache. No need
            // to go back to the database.
            if (!ret)
            {
                TypeDataTable refDataTable = null;

                // Execute the command.
                sqlCommand = ExecuteQuery<TypeDataTable>(ref refDataTable, queryText,
                    commandType, base.DefaultConnection(base.ConfigurationDatabaseConnection), values);

                // Assign the data table.
                dataTable = refDataTable;
            }

            // Add the item back to the cache and reset
            // the cache time out.
            if (_cacheItems)
            {
                // Add the object to the cache.
                AddItemToCache(_cachedItemName, _cacheTimeout, dataTable);

                // Re-set the cache item indicator
                // back to false.
                _cacheItems = false;
                _cacheTimeout = 120;
                _cachedItemName = string.Empty;
            }

            // Return the collection of
            // current type linq table data.
            return sqlCommand;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The insert base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public class InsertDataSetGenericBase<TDataContext, TDataTable> :
        SchemaDataSetGenericBase<TDataContext, TDataTable>,
        IInsertDataSetGenericBase<TDataContext, TDataTable>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region Insert Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public InsertDataSetGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");

            _schemaName = schemaName;
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="specificPath">The specific path of the config file, 
        /// used for web applications.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public InsertDataSetGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");
            if (String.IsNullOrEmpty(specificPath)) throw new ArgumentNullException("specificPath");

            _schemaName = schemaName;
            base.ConfigurationPath = specificPath;
        }
        #endregion

        #region Private Fields
        private string _schemaName = String.Empty;
        #endregion

        #region Public Virtual Insert Collection Methods
        /// <summary>
        /// Inserts the specified data table.
        /// </summary>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertDataTable(TDataTable dataTable)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            return InsertDataTable<TDataTable>(dataTable);
        }

        /// <summary>
        /// Inserts the specified data table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertDataTable<TypeDataTable>(TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            int i = 0;
            string[] sql = new string[dataTable.Rows.Count];
            string sqlQuery = string.Empty;

            // For each row found
            // insert the data row.
            foreach (DataRow row in dataTable.Rows)
            {
                string columns = string.Empty;
                string values = string.Empty;

                // For each column in the row.
                foreach (DataColumn column in dataTable.Columns)
                {
                    // Do not insert if the column is
                    // auto generated by the database.
                    if (!column.AutoIncrement)
                    {
                        // Get the current row value value.
                        object value = row[column.ColumnName.ToLower()];

                        // If the row value is not null.
                        // then build the sql column and value.
                        if (value.GetType().ToString() != "System.DBNull")
                        {
                            columns += "[" + column.ColumnName + "],";
                            values += base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    }
                }

                // Build the sql insert statement.
                sql[i++] =
                    "INSERT INTO [" + dataTable.TableName.Replace(".", "].[") + "]" + " " +
                    "(" + columns.TrimEnd(',') + ")" + " " +
                    "VALUES (" + values.TrimEnd(',') + ")";
            }

            // Join the sql statements
            // into one query.
            sqlQuery = string.Join(" ", sql);

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;

            // If using bulk insert operation then
            // execute the combined statement.
            if (base.UseBulkInsert)
            {
                ret = ExecuteCommand(ref sqlCommand, sqlQuery, CommandType.Text,
                    base.DefaultConnection(base.ConfigurationDatabaseConnection));
            }
            else
            {
                // Foreach sql statement created execute one
                // at a time.
                foreach (string sqlStatement in sql)
                    ret = ExecuteCommand(ref sqlCommand, sqlStatement, CommandType.Text,
                        base.DefaultConnection(base.ConfigurationDatabaseConnection));
            }

            // If nothing was inserted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Insertion has failed. No data found."),
                    "InsertDataTable", "Inserts the items to the database. " +
                    "{" + sqlQuery + "}" +
                    "{" + ConvertNullTypeToString(dataTable) + "}");
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Insert Item Methods

        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <param name="dataRow">The data row to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertItem(DataRow dataRow)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            return InsertItem<TDataTable>(dataRow);
        }

        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertItem<TypeDataTable>(DataRow dataRow)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            string columns = string.Empty;
            string values = string.Empty;

            // For each column in the row.
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                // Do not insert if the column is
                // auto generated by the database.
                if (!column.AutoIncrement)
                {
                    // Get the current row value value.
                    object value = dataRow[column.ColumnName.ToLower()];

                    // If the row value is not null.
                    // then build the sql column and value.
                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        columns += "[" + column.ColumnName + "],";
                        values += base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "INSERT INTO [" + dataRow.Table.TableName.Replace(".", "].[") + "]" + " " +
                "(" + columns.TrimEnd(',') + ")" + " " +
                "VALUES (" + values.TrimEnd(',') + ")";

            // Excecute the command.
            DbCommand sqlCommand = null;
            int ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was inserted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Insertion has failed. No data found."),
                    "InsertItem", "Inserts the items to the database. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullTypeToString(dataRow) + "}");
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Insert Identity Methods
        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<object> InsertDataRow(DataRow dataRow, string identitySqlQuery)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            return InsertDataRow<TDataTable>(dataRow, identitySqlQuery);
        }

        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<object> InsertDataRow<TypeDataTable>(DataRow dataRow, string identitySqlQuery)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            string columns = string.Empty;
            string values = string.Empty;
            List<object> identityList = new List<object>();

            // For each column in the row.
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                // Do not insert if the column is
                // auto generated by the database.
                if (!column.AutoIncrement)
                {
                    // Get the current row value value.
                    object value = dataRow[column.ColumnName.ToLower()];

                    // If the row value is not null.
                    // then build the sql column and value.
                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        columns += "[" + column.ColumnName + "],";
                        values += base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "INSERT INTO [" + dataRow.Table.TableName.Replace(".", "].[") + "]" + " " +
                "(" + columns.TrimEnd(',') + ")" + " " +
                "VALUES (" + values.TrimEnd(',') + ")";

            // If the identity query is nor null.
            if (!String.IsNullOrEmpty(identitySqlQuery))
                sql += " " + identitySqlQuery;

            // Excecute the command.
            DataTable table = null;
            ExecuteQuery(ref table, sql, CommandType.Text, base.DefaultConnection(base.ConfigurationDatabaseConnection),
                ((String.IsNullOrEmpty(identitySqlQuery)) ? false : true));

            // Data identity table if not null.
            if (table != null)
            {
                // For each row returned.
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        // Get the new identity created.
                        identityList.Add(row[column.ColumnName]);
                    }

                    // Break the loop.
                    break;
                }
            }

            // Return the new identity.
            return identityList;
        }
        #endregion

        #region Public Virtual Insert Query Methods
        /// <summary>
        /// Insert the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand InsertQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return InsertQueryItem<TDataTable>(ref dataTable, queryText, commandType, values);
        }

        /// <summary>
        /// Insert the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand InsertQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery<TypeDataTable>(ref dataTable, queryText,
                    commandType, base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }

        /// <summary>
        /// Insert the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Int32 InserCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            return ExecuteCommand(ref sqlCommand, commandText, commandType, 
                base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }
        #endregion

        #region Public Asynchronous Insert Methods
        /// <summary>
        /// Begin inserts the specified data row.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataRow(AsyncCallback callback,
            object state, DataRow dataRow, string identitySqlQuery)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncInsertDataRow<TDataContext, TDataTable>(
                this, callback, state, dataRow, identitySqlQuery);
        }

        /// <summary>
        /// End inserts the specified data row.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        public List<Object> EndInsertDataRow(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncInsertDataRow<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataRow<TypeDataTable>(AsyncCallback callback,
            object state, DataRow dataRow, string identitySqlQuery)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncInsertDataRow<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, dataRow, identitySqlQuery);
        }

        /// <summary>
        /// End inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        public List<Object> EndInsertDataRow<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncInsertDataRow<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }

        /// <summary>
        /// Begin inserts the specified data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataTable(AsyncCallback callback,
            object state, TDataTable dataTable)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncInsertDataTable<TDataContext, TDataTable>(
                this, callback, state, dataTable);
        }

        /// <summary>
        /// End inserts the specified data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        public Boolean EndInsertDataTable(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncInsertDataTable<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin inserts the specified data table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataTable<TypeDataTable>(AsyncCallback callback,
            object state, TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncInsertDataTable<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, dataTable);
        }

        /// <summary>
        /// End inserts the specified data table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        public Boolean EndInsertDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncInsertDataTable<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }

        
        #endregion

        #endregion
    }

    /// <summary>
    /// The delete base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public class DeleteDataSetGenericBase<TDataContext, TDataTable> :
        SchemaDataSetGenericBase<TDataContext, TDataTable>,
        IDeleteDataSetGenericBase<TDataContext, TDataTable>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region Delete Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public DeleteDataSetGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");

            _schemaName = schemaName;
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="specificPath">The specific path of the config file, 
        /// used for web applications.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public DeleteDataSetGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");
            if (String.IsNullOrEmpty(specificPath)) throw new ArgumentNullException("specificPath");

            _schemaName = schemaName;
            base.ConfigurationPath = specificPath;
        }
        #endregion

        #region Private Fields
        private string _schemaName = String.Empty;
        private bool _concurrencyError = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, concurrency error indicator.
        /// </summary>
        public bool ConcurrencyError
        {
            get { return _concurrencyError; }
            set { _concurrencyError = value; }
        }
        #endregion

        #region Public Virtual Delete Item Methods
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey(object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return DeleteItemKey<TDataTable>(keyValue, keyName);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey<TypeDataTable>(object keyValue, string keyName)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Build the sql insert statement.
            string sql =
                "DELETE FROM [" + (new TypeDataTable()).TableName.Replace(".", "].[") + "]" + " " +
                "WHERE ([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ")";

            // Excecute the command.
            DbCommand sqlCommand = null;
            int ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was deleted, no
            // rows have been affected.
            if(ret <= 0)
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemKey", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey(object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            return DeleteItemKey<TDataTable>(keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey<TypeDataTable>(object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            // Build the sql insert statement.
            string sql =
                "DELETE FROM [" + (new TypeDataTable()).TableName.Replace(".", "].[") + "]" + " " +
                "WHERE (([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ") AND (" +
                    "[" + rowVersionName + "] = " + base.DataTypeConversion.GetSqlStringValue(rowVersionData.GetType(), rowVersionData) + "))";

            // Excecute the command.
            DbCommand sqlCommand = null;
            int ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was deleted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemKey", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Delete Predicate Methods

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate(string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return DeleteItemPredicate<TDataTable>(predicate);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate<TypeDataTable>(string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Create a new internal table instance
            // assign the initial predicate string.
            string predicateInternal = predicate;

            // Format the first paramerter found.
            string currentPredicate = predicate.Replace("&&", "AND").Replace("||", "OR").Replace("==", "=").Replace("!=", "<>");

            if (values != null)
            {
                if (values.Count() > 0)
                {
                    // For each parameter value found
                    // interate through the collection
                    // and replace the precidate values.
                    for (int i = 0; i < values.Count(); i++)
                    {
                        predicateInternal = currentPredicate.Replace("@" + i, base.DataTypeConversion.GetSqlStringValue(values[i].GetType(), values[i]));
                        currentPredicate = predicateInternal;
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "DELETE FROM [" + (new TypeDataTable()).TableName + "]" + " " +
                "WHERE " + predicateInternal;

            // Excecute the command.
            DbCommand sqlCommand = null;
            int ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was deleted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemPredicate", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(values) + " , " + predicate + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Delete Collection Methods
        /// <summary>
        /// Deletes the data table from the database.
        /// </summary>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteDataTable(TDataTable dataTable)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            return DeleteDataTable<TDataTable>(dataTable);
        }

        /// <summary>
        /// Deletes the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteDataTable<TypeDataTable>(TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            // Gets all the primary keys in the table.
            DataColumn[] primaryKeys = dataTable.PrimaryKey;

            // No primary keys found.
            if (primaryKeys.Count() < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "DeleteDataTable",
                    "Can not delete the data because no primary key has been found.");
                return false;
            }

            int i = 0;
            int j = 0;
            string[] sql = new string[dataTable.Rows.Count];
            string sqlQuery = string.Empty;

            // For each row found
            // insert the data row.
            foreach (DataRow row in dataTable.Rows)
            {
                // Get the current row value.
                string primKeyName = primaryKeys[j++].ColumnName;
                object valueKey = row[primKeyName.ToLower()];

                // If the value is null then exist.
                if (valueKey.GetType().ToString() == "System.DBNull")
                {
                    ErrorProvider(new Exception("Key value can not be null."), "UpdateDataTable",
                        "Can not update the data because the key value is null.");
                    return false;
                }

                string results = string.Empty;
                string keyValue = string.Empty;

                // For each primary key.
                foreach (DataColumn primaryKey in primaryKeys)
                {
                    primKeyName = primaryKey.ColumnName;
                    valueKey = row[primaryKey.ColumnName.ToLower()];
                    keyValue += "[" + primKeyName + "] = " + base.DataTypeConversion.GetSqlStringValue(valueKey.GetType(), valueKey) + " AND ";
                }

                // Build the sql insert statement.
                sql[i++] =
                    "DELETE FROM [" + dataTable.TableName.Replace(".", "].[") + "]" + " " +
                    "WHERE (" + keyValue.Substring(0, keyValue.Length - 5) + ")";
            }

            // Join the sql statements
            // into one query.
            sqlQuery = string.Join(" ", sql);

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            
            // If using bulk delete operation then
            // execute the combined statement.
            if (base.UseBulkDelete)
            {
                ret = ExecuteCommand(ref sqlCommand, sqlQuery, CommandType.Text,
                    base.DefaultConnection(base.ConfigurationDatabaseConnection));
            }
            else
            {
                // Foreach sql statement created execute one
                // at a time.
                foreach (string sqlStatement in sql)
                    ret = ExecuteCommand(ref sqlCommand, sqlStatement, CommandType.Text,
                        base.DefaultConnection(base.ConfigurationDatabaseConnection));
            }

            // If nothing was deleted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteDataTable", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sqlQuery + "}" +
                    "{" + ConvertNullTypeToString(dataTable) + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Delete Query Methods
        /// <summary>
        /// Delete the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand DeleteQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return DeleteQueryItem<TDataTable>(ref dataTable, queryText, commandType, values);
        }

        /// <summary>
        /// Delete the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand DeleteQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery<TypeDataTable>(ref dataTable, queryText,
                    commandType, base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }

        /// <summary>
        /// Delete the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Int32 DeleteCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            return ExecuteCommand(ref sqlCommand, commandText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }
        #endregion

        #region Public Asynchronous Delete Methods
        /// <summary>
        /// Begin deletes the specified item.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteItemPredicate(AsyncCallback callback,
            object state, string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncDeleteItemPredicate<TDataContext, TDataTable>(
                this, callback, state, predicate, values);
        }

        /// <summary>
        /// End deletes the specified item.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was deleted else false.</returns>
        public Boolean EndDeleteItemPredicate(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncDeleteItemPredicate<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteItemPredicate<TypeDataTable>(AsyncCallback callback,
            object state, string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncDeleteItemPredicate<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, predicate, values);
        }

        /// <summary>
        /// End deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was deleted else false.</returns>
        public Boolean EndDeleteItemPredicate<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncDeleteItemPredicate<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }

        /// <summary>
        /// Begin deletes the data table from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteDataTable(AsyncCallback callback,
            object state, TDataTable dataTable)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncDeleteDataTable<TDataContext, TDataTable>(
                this, callback, state, dataTable);
        }

        /// <summary>
        /// End deletes the data table from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        public Boolean EndDeleteDataTable(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncDeleteDataTable<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin deletes the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteDataTable<TypeDataTable>(AsyncCallback callback,
            object state, TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncDeleteDataTable<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, dataTable);
        }

        /// <summary>
        /// End deletes the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        public Boolean EndDeleteDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncDeleteDataTable<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The update base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public class UpdateDataSetGenericBase<TDataContext, TDataTable> :
        SchemaDataSetGenericBase<TDataContext, TDataTable>,
        IUpdateDataSetGenericBase<TDataContext, TDataTable>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region Update Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public UpdateDataSetGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");

            _schemaName = schemaName;
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="specificPath">The specific path of the config file, 
        /// used for web applications.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public UpdateDataSetGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(schemaName, connectionType, connectionDataType, dataAccessProvider)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(schemaName)) throw new ArgumentNullException("schemaName");
            if (String.IsNullOrEmpty(specificPath)) throw new ArgumentNullException("specificPath");

            _schemaName = schemaName;
            base.ConfigurationPath = specificPath;
        }
        #endregion

        #region Private Fields
        private string _schemaName = String.Empty;
        private bool _concurrencyError = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, concurrency error indicator.
        /// </summary>
        public bool ConcurrencyError
        {
            get { return _concurrencyError; }
            set { _concurrencyError = value; }
        }
        #endregion

        #region Public Virtual Update Item Methods
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(DataRow dataRow, object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return UpdateItemKey<TDataTable>(dataRow, keyValue, keyName);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeDataTable>(
            DataRow dataRow, object keyValue, string keyName)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            string results = string.Empty;

            // For each column in the row.
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                // Do not insert if the column is
                // auto generated by the database.
                if (!column.AutoIncrement)
                {
                    // Get the current row value value.
                    object value = dataRow[column.ColumnName.ToLower()];

                    // If the row value is not null.
                    // then build the sql column and value.
                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        results += "[" + column.ColumnName + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "UPDATE [" + dataRow.Table.TableName.Replace(".", "].[") + "]" + " " +
                "SET " + results.TrimEnd(',') + " " +
                "WHERE ([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ")";

            // Excecute the command.
            DbCommand sqlCommand = null;
            int ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was updated, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                    "UpdateItemKey", "Updates the items to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(DataRow dataRow, object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            return UpdateItemKey<TDataTable>(dataRow, keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeDataTable>(DataRow dataRow, object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            string results = string.Empty;

            // For each column in the row.
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                // Do not insert if the column is
                // auto generated by the database.
                if (!column.AutoIncrement)
                {
                    // Get the current row value value.
                    object value = dataRow[column.ColumnName.ToLower()];

                    // If the row value is not null.
                    // then build the sql column and value.
                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        results += "[" + column.ColumnName + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "UPDATE [" + dataRow.Table.TableName.Replace(".", "].[") + "]" + " " +
                "SET " + results.TrimEnd(',') + " " +
                "WHERE (([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ") AND (" +
                    "[" + rowVersionName + "] = " + base.DataTypeConversion.GetSqlStringValue(rowVersionData.GetType(), rowVersionData) + "))";

            // Excecute the command.
            DbCommand sqlCommand = null;
            int ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was updated, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                    "UpdateItemKey", "Updates the items to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Update Predicate Methods
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(
            DataRow dataRow, string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return UpdateItemPredicate<TDataTable>(dataRow, predicate, values);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeDataTable>(
            DataRow dataRow, string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Create a new internal table instance
            // assign the initial predicate string.
            string predicateInternal = predicate;

            // Format the first paramerter found.
            string currentPredicate = predicate.Replace("&&", "AND").Replace("||", "OR").Replace("==", "=").Replace("!=", "<>");
            string results = string.Empty;

            if (values != null)
            {
                if (values.Count() > 0)
                {
                    // For each parameter value found
                    // interate through the collection
                    // and replace the precidate values.
                    for (int i = 0; i < values.Count(); i++)
                    {
                        predicateInternal = currentPredicate.Replace("@" + i, base.DataTypeConversion.GetSqlStringValue(values[i].GetType(), values[i]));
                        currentPredicate = predicateInternal;
                    }
                }
            }

            // For each column in the row.
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                // Do not insert if the column is
                // auto generated by the database.
                if (!column.AutoIncrement)
                {
                    // Get the current row value value.
                    object value = dataRow[column.ColumnName.ToLower()];

                    // If the row value is not null.
                    // then build the sql column and value.
                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        results += "[" + column.ColumnName + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "UPDATE [" + dataRow.Table.TableName.Replace(".", "].[") + "]" + " " +
                "SET " + results.TrimEnd(',') + " " +
                "WHERE " + predicateInternal;

            // Excecute the command.
            DbCommand sqlCommand = null;
            int ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was updated, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                    "UpdateItemPredicate", "Updates the items to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(values) + " , " + predicate + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Update Collection Methods
        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateDataTable(TDataTable dataTable)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            return UpdateDataTable<TDataTable>(dataTable);
        }

        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateDataTable<TypeDataTable>(TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            // Gets all the primary keys in the table.
            DataColumn[] primaryKeys = dataTable.PrimaryKey;

            // No primary keys found.
            if (primaryKeys.Count() < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "UpdateDataTable",
                    "Can not update the data because no primary key has been found.");
                return false;
            }

            int i = 0;
            int j = 0;
            string[] sql = new string[dataTable.Rows.Count];
            string sqlQuery = string.Empty;

            // For each row found
            // insert the data row.
            foreach (DataRow row in dataTable.Rows)
            {
                // Get the current row value.
                string primKeyName = primaryKeys[j++].ColumnName;
                object valueKey = row[primKeyName.ToLower()];

                // If the value is null then exist.
                if (valueKey.GetType().ToString() == "System.DBNull")
                {
                    ErrorProvider(new Exception("Key value can not be null."), "UpdateDataTable",
                        "Can not update the data because the key value is null.");
                    return false;
                }

                string results = string.Empty;
                string keyValue = string.Empty;

                // For each column in the row.
                foreach (DataColumn column in dataTable.Columns)
                {
                    // Do not insert if the column is
                    // auto generated by the database.
                    if (!column.AutoIncrement)
                    {
                        // Get the current row value value.
                        object value = row[column.ColumnName.ToLower()];

                        // If the row value is not null.
                        // then build the sql column and value.
                        if (value.GetType().ToString() != "System.DBNull")
                        {
                            results += "[" + column.ColumnName + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    }
                }

                // For each primary key.
                foreach (DataColumn primaryKey in primaryKeys)
                {
                    primKeyName = primaryKey.ColumnName;
                    valueKey = row[primaryKey.ColumnName.ToLower()];
                    keyValue += "[" + primKeyName + "] = " + base.DataTypeConversion.GetSqlStringValue(valueKey.GetType(), valueKey) + " AND ";
                }

                // Build the sql update statement.
                sql[i++] =
                    "UPDATE [" + dataTable.TableName.Replace(".", "].[") + "]" + " " +
                    "SET " + results.TrimEnd(',') + " " +
                    "WHERE (" + keyValue.Substring(0, keyValue.Length - 5)  + ")";
            }

            // Join the sql statements
            // into one query.
            sqlQuery = string.Join(" ", sql);

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;

            // If using bulk update operation then
            // execute the combined statement.
            if (base.UseBulkUpdate)
            {
                ret = ExecuteCommand(ref sqlCommand, sqlQuery, CommandType.Text,
                    base.DefaultConnection(base.ConfigurationDatabaseConnection));
            }
            else
            {
                // Foreach sql statement created execute one
                // at a time.
                foreach (string sqlStatement in sql)
                    ret = ExecuteCommand(ref sqlCommand, sqlStatement, CommandType.Text,
                        base.DefaultConnection(base.ConfigurationDatabaseConnection));
            }

            // If nothing was deleted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                    "UpdateDataTable", "Updates the items to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sqlQuery + "}" +
                    "{" + ConvertNullTypeToString(dataTable) + "}" +
                    "{" + typeof(TypeDataTable).Name + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Update Query Methods
        /// <summary>
        /// Update the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand UpdateQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return UpdateQueryItem<TDataTable>(ref dataTable, queryText, commandType, values);
        }

        /// <summary>
        /// Update the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand UpdateQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery<TypeDataTable>(ref dataTable, queryText,
                    commandType, base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }

        /// <summary>
        /// Update the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Int32 UpdateCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            return ExecuteCommand(ref sqlCommand, commandText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
        }
        #endregion

        #region Public Asynchronous Update Methods
        /// <summary>
        /// Begin updates the specified item.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateItemPredicate(AsyncCallback callback,
            object state, DataRow dataRow, string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncUpdateItemPredicate<TDataContext, TDataTable>(
                this, callback, state, dataRow, predicate, values);
        }

        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        public Boolean EndUpdateItemPredicate(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncUpdateItemPredicate<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateItemPredicate<TypeDataTable>(AsyncCallback callback,
            object state, DataRow dataRow, string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncUpdateItemPredicate<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, dataRow, predicate, values);
        }

        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        public Boolean EndUpdateItemPredicate<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncUpdateItemPredicate<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }

        /// <summary>
        /// Begin updates the data table from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateDataTable(AsyncCallback callback,
            object state, TDataTable dataTable)
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncUpdateDataTable<TDataContext, TDataTable>(
                this, callback, state, dataTable);
        }

        /// <summary>
        /// End updates the data table from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        public Boolean EndUpdateDataTable(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncUpdateDataTable<TDataContext, TDataTable>.End(ar);
        }

        /// <summary>
        /// Begin updates the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateDataTable<TypeDataTable>(AsyncCallback callback,
            object state, TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Return an AsyncResult.
            return new DataSet.Async.AsyncUpdateDataTable<TDataContext, TDataTable, TypeDataTable>(
                this, callback, state, dataTable);
        }

        /// <summary>
        /// End updates the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        public Boolean EndUpdateDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
        {
            // Use the AsyncResult to complete that async operation.
            return DataSet.Async.AsyncUpdateDataTable<TDataContext, TDataTable, TypeDataTable>.End(ar);
        }
        #endregion

        #endregion
    }
}
