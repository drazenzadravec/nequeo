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
using System.Data.Objects;
using System.Data.Metadata.Edm;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;

using Nequeo.Data.DataType;
using Nequeo.Data.Control;
using Nequeo.Linq.Extension;
using Nequeo.Extension;

namespace Nequeo.Data.Edm
{
    /// <summary>
    /// The abstract base data access class for all data access objects.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public abstract class SchemaEdmGenericBase<TDataContext, TLinqEntity> : Nequeo.Handler.LogHandler, IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region SchemaEdmGenericBase Abstract Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaItemName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected SchemaEdmGenericBase(string schemaItemName, ConnectionContext.ConnectionType connectionType,
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
        private const string eventNamespace = "Nequeo.Data.Client.Edm";
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
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _dataAccessProvider;
        private DataTypeConversion _dataTypeConversion = null;
        private Boolean _pluralEntity = true;
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
        /// Gets sets the current data context object.
        /// </summary>
        public TDataContext DataContext
        {
            get { return CreateDataContextInstance(); }
            set { _dataContext = value; }
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
        /// Gets sets, the database connection type.
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
        /// Gets, the edm data collection configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.EdmDataCollection EdmDataCollectionReader
        {
            get { return Nequeo.Data.Configuration.EdmDataConfigurationManager.EdmDataCollection(); }
        }

        /// <summary>
        /// Gets, the edm data default configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.EdmDataElementDefault EdmDataElementReader
        {
            get { return Nequeo.Data.Configuration.EdmDataConfigurationManager.EdmDataElement(); }
        }

        /// <summary>
        /// Gets sets, are the entity items plural.
        /// </summary>
        public Boolean PluralEntity
        {
            get { return _pluralEntity; }
            set { _pluralEntity = value; }
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
        /// Get all properties in the current type that are primary keys.
        /// </summary>
        /// <returns>Collection of properties that are primary keys.</returns>
        public List<PropertyInfo> GetAllPrimaryKeys()
        {
            // Create a new property collection.
            List<PropertyInfo> primaryKeys = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(TLinqEntity)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Objects.DataClasses.EdmScalarPropertyAttribute att =
                            (System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)attribute;

                        // If the property is a primary key.
                        if (att.EntityKeyProperty)
                            primaryKeys.Add(member);
                    }
                }
            }

            // return the collection of properties.
            return primaryKeys;
        }

        /// <summary>
        /// Get all properties in the current type that are primary keys.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>Collection of properties that are primary keys.</returns>
        public List<PropertyInfo> GetAllPrimaryKeys<T>()
        {
            // Create a new property collection.
            List<PropertyInfo> primaryKeys = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(T)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Objects.DataClasses.EdmScalarPropertyAttribute att =
                            (System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)attribute;

                        // If the property is a primary key.
                        if (att.EntityKeyProperty)
                            primaryKeys.Add(member);
                    }
                }
            }

            // return the collection of properties.
            return primaryKeys;
        }

        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        public PropertyInfo GetPrimaryKey()
        {
            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(TLinqEntity)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Objects.DataClasses.EdmScalarPropertyAttribute att =
                            (System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)attribute;

                        // If the property is a primary key.
                        if (att.EntityKeyProperty)
                            return member;
                    }
                }
            }

            // Return null none found.
            return null;
        }

        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        public PropertyInfo GetPrimaryKey<T>()
        {
            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(T)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Objects.DataClasses.EdmScalarPropertyAttribute att =
                            (System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)attribute;

                        // If the property is a primary key.
                        if (att.EntityKeyProperty)
                            return member;
                    }
                }
            }

            // Return null none found.
            return null;
        }

        /// <summary>
        /// Is the current property value a primary key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a primary key.</returns>
        public bool IsPrimaryKey(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Objects.DataClasses.EdmScalarPropertyAttribute att =
                        (System.Data.Objects.DataClasses.EdmScalarPropertyAttribute)attribute;

                    // If the property attribute
                    // is a primary key.
                    if (att.EntityKeyProperty)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value a association key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a foreign key.</returns>
        public bool IsAssociationKey(PropertyInfo property)
        {
            if(property.PropertyType == typeof(System.Data.Objects.DataClasses.EntityReference))
                return true;

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value a foreign key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a foreign key.</returns>
        public bool IsForeignKey(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute att =
                        (System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute)attribute;

                    // If the property attribute
                    // is a foreign key.
                    return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Gets the current linq entity table name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The ttable name.</returns>
        public string GetTableName<T>()
        {
            // Get the current member.
            MemberInfo member = typeof(T);

            // For each attribute for the member
            foreach (object attribute in member.GetCustomAttributes(true))
            {
                // If the attribute is the Nequeo datatable
                // attribute then get the table name.
                if (attribute is System.Data.Objects.DataClasses.EdmEntityTypeAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Objects.DataClasses.EdmEntityTypeAttribute att =
                        (System.Data.Objects.DataClasses.EdmEntityTypeAttribute)attribute;

                    if (_pluralEntity)
                        // Return the table name.
                        return att.Name.TrimStart('_').Plural();
                    else
                        // Return the table name.
                        return att.Name.TrimStart('_');
                }
            }

            // Return a null.
            return null;
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
            Write(typeof(TLinqEntity).GetType().FullName, method + ". Description : " + methodDescription,
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
            Write(typeof(TLinqEntity).GetType().FullName, method, information, 201,
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
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public class EdmDataGenericBase<TDataContext, TLinqEntity> :
        SchemaEdmGenericBase<TDataContext, TLinqEntity>,
        IEdmDataGenericBase<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region EdmDataGenericBase Base Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public EdmDataGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
                DataContext.GetType().FullName, typeof(TLinqEntity).GetType().FullName, typeof(TLinqEntity).GetType().FullName);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The select base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public class SelectEdmGenericBase<TDataContext, TLinqEntity> :
        SchemaEdmGenericBase<TDataContext, TLinqEntity>,
        ISelectEdmGenericBase<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
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
        public SelectEdmGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public SelectEdmGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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
        
        private IQueryable<TLinqEntity> _query = null;
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
        /// Gets the IQueryable generic provider
        /// for the current linq entity.
        /// </summary>
        public IQueryable<TLinqEntity> IQueryable
        {
            get { return _query; }
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
        public ISelectEdmGenericBase<TDataContext, TLinqEntity>
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

        #region Public Virtual Select Items Methods
        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TLinqEntity> SelectIQueryableItems(string queryString,
            params System.Data.Objects.ObjectParameter[] values)
        {
            IQueryable<TLinqEntity> data = null;
            bool ret = SelectCollection<TLinqEntity>(ref data, queryString, values);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TypeLinqEntity> SelectIQueryableItems<TypeLinqEntity>(
            string queryString, params System.Data.Objects.ObjectParameter[] values)
                where TypeLinqEntity : class, new()
        {
            IQueryable<TypeLinqEntity> data = null;
            bool ret = SelectCollection<TypeLinqEntity>(ref data, queryString, values);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TLinqEntity> SelectIQueryableItems()
        {
            IQueryable<TLinqEntity> data = null;
            bool ret = SelectCollection<TLinqEntity>(ref data);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TypeLinqEntity> SelectIQueryableItems<TypeLinqEntity>()
                where TypeLinqEntity : class, new()
        {
            IQueryable<TypeLinqEntity> data = null;
            bool ret = SelectCollection<TypeLinqEntity>(ref data);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TLinqEntity> SelectIQueryableItems(
            Expression<Func<TLinqEntity, bool>> predicate)
        {
            IQueryable<TLinqEntity> data = null;
            bool ret = SelectCollection<TLinqEntity>(ref data, predicate);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TypeLinqEntity> SelectIQueryableItems<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            IQueryable<TypeLinqEntity> data = null;
            bool ret = SelectCollection<TypeLinqEntity>(ref data, predicate);

            // Return the details of the operation.
            return ret ? data : null;
        }
        #endregion

        #region Public Virtual Select Collection Methods
        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollection(
            ref IQueryable<TLinqEntity> queryResult, string queryString,
            params System.Data.Objects.ObjectParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryString)) throw new ArgumentNullException("queryString");

            return SelectCollection<TLinqEntity>(ref queryResult, queryString, values);
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollection<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult, string queryString, 
            params System.Data.Objects.ObjectParameter[] values)
                where TypeLinqEntity : class, new()

        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryString)) throw new ArgumentNullException("queryString");

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null, queryString, values);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollection", "No data found. " +
                        "{" + ConvertNullToString(values) + " , " + queryString + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");
                    return false;
                }

                // Assign the referenced object.
                queryResult = query;

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollection", "Gets a collection of IQueryable provider linq entities. " +
                    "{" + ConvertNullToString(values) + " , " + queryString + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollection(ref IQueryable<TLinqEntity> queryResult)
        {
            return SelectCollection<TLinqEntity>(ref queryResult);
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollection<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult)
                where TypeLinqEntity : class, new()
        {
            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollection", "No data found. " +
                        "{" + GetTableName<TypeLinqEntity>() + "}");
                    return false;
                }

                // Assign the referenced object.
                queryResult = query;

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollection", "Gets a collection of IQueryable provider linq entities. " +
                    "{" + GetTableName<TypeLinqEntity>() + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollection(ref IQueryable<TLinqEntity> queryResult, 
            Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SelectCollection<TLinqEntity>(ref queryResult, predicate);
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollection<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(predicate, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollection", "No data found. " +
                        "{" + ConvertNullTypeToString(query) + "}" +
                        "{" + ConvertNullTypeToString(predicate) + "}");
                    return false;
                }

                // Assign the referenced object.
                queryResult = query;

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollection", "Gets a collection of IQueryable provider linq entities. " +
                    "{" + ConvertNullTypeToString(query) + "}" +
                    "{" + ConvertNullTypeToString(predicate) + "}");
                return false;
            }
        }
        #endregion

        #region Private Select Item(s) Methods
        /// <summary>
        /// Select the data from the database for the current
        /// linq entity type.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="expressionPredicate">The expression predicate string to search.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq type data.</returns>
        private IQueryable<TypeLinqEntity> SelectItemEx<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> expressionPredicate,
            string queryString, params System.Data.Objects.ObjectParameter[] values)
                where TypeLinqEntity : class, new()
        {
            // Declare the current query type.
            IQueryable<TypeLinqEntity> query = null;

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
                    if (item.GetType() == typeof(Object[]))
                    {
                        // If the data object is enumerable, that is, is the
                        // data object an array or a collection object.
                        if (item is IEnumerable)
                        {
                            // Convert the current object to
                            // an object array.
                            Object[] dataColl = (Object[])Convert.ChangeType(item, typeof(Object[]));

                            // If the data collection contains data.
                            if (dataColl.Count() > 0)
                            {
                                // If the data collection items are of
                                // the current linq entity type.
                                if (dataColl[0] is TypeLinqEntity)
                                {
                                    int i = 0;

                                    // Create a new instance of the
                                    // current linq entity type.
                                    TypeLinqEntity[] data = new TypeLinqEntity[dataColl.Count()];

                                    // Get the data object set enumerator.
                                    IEnumerator dataEnum = dataColl.GetEnumerator();

                                    // For each data item found.
                                    while (dataEnum.MoveNext())
                                        // Cast the current object data item
                                        // to the current linq entity type.
                                        data[i++] = (TypeLinqEntity)dataEnum.Current;

                                    // Cast the current linq entity IEnumerable collection
                                    // to the IQueryable collection of linq entities.
                                    IQueryable<TypeLinqEntity> cacheQuery = data.AsQueryable<TypeLinqEntity>();

                                    // Assign the current IQueryable colelction
                                    // with the IQueryable collection from the cache.
                                    query = cacheQuery;
                                    ret = true;
                                }
                            }
                        }
                    }
                }
            }

            // If no item in cache. No need
            // to go back to the database.
            if (!ret)
            {
                if (expressionPredicate != null)
                {
                    // Execute the Link query on 
                    // the current linq table.
                    // This query uses the predicate
                    // and values.
                    query = DataContext.Set<TypeLinqEntity>().Where(expressionPredicate);
                }
                else
                {
                    // Execute the Link query on 
                    // the current linq table.
                    // this query returns all
                    // current linq table data.
                    query = DataContext.Set<TypeLinqEntity>();
                }
            }

            // Add the item back to the cache and reset
            // the cache time out.
            if (_cacheItems)
            {
                // Add the object to the cache.
                AddItemToCache(_cachedItemName, _cacheTimeout, query.ToObjectArray());

                // Re-set the cache item indicator
                // back to false.
                _cacheItems = false;
                _cacheTimeout = 120;
                _cachedItemName = string.Empty;
            }

            // Return the collection of
            // current type linq table data.
            return query;
        }
        #endregion

        #region Public Asynchronous Select Methods
        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems<TypeLinqEntity>(AsyncCallback callback, object state,
            string queryString, params System.Data.Objects.ObjectParameter[] values)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity, TypeLinqEntity>(this, callback, state, queryString, values);
        }

        /// <summary>
        /// End gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        public IQueryable<TypeLinqEntity> EndSelectIQueryableItems<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state,
            string queryString, params System.Data.Objects.ObjectParameter[] values)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity>(this, callback, state, queryString, values);
        }

        /// <summary>
        /// End gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        public IQueryable<TLinqEntity> EndSelectIQueryableItems(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems<TypeLinqEntity>(AsyncCallback callback, object state)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity, TypeLinqEntity>(this, callback, state);
        }

        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity>(this, callback, state);
        }

        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems<TypeLinqEntity>(AsyncCallback callback, object state,
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity, TypeLinqEntity>(this, callback, state, predicate);
        }

        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state,
            Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncSelectIQueryableEdmItems<TDataContext, TLinqEntity>(this, callback, state, predicate);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The delete base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public class DeleteEdmGenericBase<TDataContext, TLinqEntity> :
        SchemaEdmGenericBase<TDataContext, TLinqEntity>,
        IDeleteEdmGenericBase<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
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
        public DeleteEdmGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public DeleteEdmGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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
        /// Deletes the current entity item.
        /// </summary>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteLinqEntity(TDataContext dataContext, TLinqEntity linqEntity)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntity == null) throw new ArgumentNullException("linqEntity");

            return DeleteLinqEntity<TDataContext, TLinqEntity>(dataContext, linqEntity);
        }

        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <typeparam name="TypeDataContext">The data context to examine.</typeparam>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteLinqEntity<TypeDataContext, TypeLinqEntity>(
            TypeDataContext dataContext, TypeLinqEntity linqEntity)
            where TypeDataContext : System.Data.Entity.DbContext, new()
            where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntity == null) throw new ArgumentNullException("linqEntity");

            int ret = 0;

            try
            {
                dataContext.Set<TypeLinqEntity>().Remove(linqEntity);
                ret = dataContext.SaveChanges();
            }
            catch (OptimisticConcurrencyException cexp)
            {
                ErrorProvider(cexp,
                    "DeleteLinqEntity", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }
            catch (Exception exp)
            {
                ErrorProvider(exp,
                    "DeleteLinqEntity", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate(
            string predicate, params ObjectParameter[] parameters)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return DeleteItemPredicate<TLinqEntity>(predicate, parameters);
        }

        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate<TypeLinqEntity>(
            string predicate, params ObjectParameter[] parameters)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.
                Set<TypeLinqEntity>().
                Where(predicate, parameters).
                First();

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                int ret = 0;

                try
                {
                    DataContext.Set<TypeLinqEntity>().Remove(query);
                    ret = DataContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException cexp)
                {
                    ErrorProvider(cexp,
                    "DeleteItemPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }
                catch (Exception exp)
                {
                    ErrorProvider(exp,
                    "DeleteItemPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }

                // Return true if no errors else false.
                return (ret > 0);
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate(
            Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return DeleteItemPredicate<TLinqEntity>(predicate);
        }

        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.
                Set<TypeLinqEntity>().
                Where(predicate).
                First();

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                int ret = 0;

                try
                {
                    DataContext.Set<TypeLinqEntity>().Remove(query);
                    ret = DataContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException cexp)
                {
                    ErrorProvider(cexp,
                    "DeleteItemPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }
                catch (Exception exp)
                {
                    ErrorProvider(exp,
                    "DeleteItemPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }

                // Return true if no errors else false.
                return (ret > 0);
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #region Public Virtual Delete Collection Methods
        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntities">The linq entities to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollection(
            TDataContext dataContext, TLinqEntity[] linqEntities)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntities == null) throw new ArgumentNullException("linqEntities");

            return DeleteCollection<TDataContext, TLinqEntity>(dataContext, linqEntities);
        }

        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeDataContext">The data context to examine.</typeparam>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntities">The linq entities to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollection<TypeDataContext, TypeLinqEntity>(
            TypeDataContext dataContext, TypeLinqEntity[] linqEntities)
            where TypeDataContext : System.Data.Entity.DbContext, new()
            where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntities == null) throw new ArgumentNullException("linqEntities");

            int ret = 0;

            // New linq enity type;
            TypeLinqEntity entity = null;

            // For each item found.
            foreach (TypeLinqEntity result in linqEntities)
            {
                // Assign the current linq entity.
                entity = result;

                if (entity != null)
                    // Delete the current item in the entity
                    // form the database.
                    dataContext.Set<TypeLinqEntity>().Remove(entity);
            }

            try
            {
                ret = dataContext.SaveChanges();
            }
            catch (OptimisticConcurrencyException cexp)
            {
                ErrorProvider(cexp,
                    "DeleteLinqEntity", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntities) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }
            catch (Exception exp)
            {
                ErrorProvider(exp,
                    "DeleteLinqEntity", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntities) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionPredicate(
            string predicate, params ObjectParameter[] parameters)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return DeleteCollectionPredicate<TLinqEntity>(predicate, parameters);
        }

        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionPredicate<TypeLinqEntity>(
            string predicate, params ObjectParameter[] parameters)
            where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Get the first item found.
            IQueryable<TypeLinqEntity> query =
                DataContext.
                Set<TypeLinqEntity>().
                Where(predicate, parameters);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    ErrorProvider(new Exception("Deletion has failed. No data found."),
                        "DeleteCollectionPredicate", "Deletes the collection of linq entities found. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                    return false;
                }

                int ret = 0;

                // New linq enity type;
                TypeLinqEntity entity = null;

                // For each item found.
                foreach (var result in query)
                {
                    // Assign the current linq entity.
                    entity = result;

                    if (entity != null)
                        // Delete the current item in the entity
                        // form the database.
                        DataContext.Set<TypeLinqEntity>().Remove(entity);
                }

                try
                {
                    // Submit the changes that have been made
                    // if any conflict then throw exception.
                    ret = DataContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException cexp)
                {
                    ErrorProvider(cexp,
                        "DeleteCollectionPredicate", "Deletes the items from the database. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }
                catch (Exception exp)
                {
                    ErrorProvider(exp,
                        "DeleteCollectionPredicate", "Deletes the items from the database. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }

                // Return true if no errors else false.
                return (ret > 0);
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteCollectionPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionPredicate(
            Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return DeleteCollectionPredicate<TLinqEntity>(predicate);
        }

        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the first item found.
            IQueryable<TypeLinqEntity> query =
                DataContext.
                Set<TypeLinqEntity>().
                Where(predicate);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    ErrorProvider(new Exception("Deletion has failed. No data found."),
                        "DeleteCollectionPredicate", "Deletes the collection of linq entities found. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                    return false;
                }

                int ret = 0;

                // New linq enity type;
                TypeLinqEntity entity = null;

                // For each item found.
                foreach (var result in query)
                {
                    // Assign the current linq entity.
                    entity = result;

                    if (entity != null)
                        // Delete the current item in the entity
                        // form the database.
                        DataContext.Set<TypeLinqEntity>().Remove(entity);
                }

                try
                {
                    // Submit the changes that have been made
                    // if any conflict then throw exception.
                    ret = DataContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException cexp)
                {
                    ErrorProvider(cexp,
                        "DeleteCollectionPredicate", "Deletes the items from the database. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }
                catch (Exception exp)
                {
                    ErrorProvider(exp,
                        "DeleteCollectionPredicate", "Deletes the items from the database. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }

                // Return true if no errors else false.
                return (ret > 0);
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteCollectionPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #region Public Asynchronous Delete Methods
        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteCollectionPredicate(AsyncCallback callback,
            object state, string predicate, params ObjectParameter[] parameters)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity>
                (this, callback, state, predicate, parameters);
        }

        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteCollectionPredicate(AsyncCallback callback,
            object state, Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity>
                (this, callback, state, predicate);
        }

        /// <summary>
        /// End deletes the collection of entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        public Boolean EndDeleteCollectionPredicate(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteCollectionPredicate<TypeLinqEntity>(AsyncCallback callback,
            object state, string predicate, params ObjectParameter[] parameters)
            where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, predicate, parameters);
        }

        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteCollectionPredicate<TypeLinqEntity>(AsyncCallback callback,
            object state, Expression<Func<TypeLinqEntity, bool>> predicate)
            where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, predicate);
        }

        /// <summary>
        /// End deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        public Boolean EndDeleteCollectionPredicate<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The insert base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public class InsertEdmGenericBase<TDataContext, TLinqEntity> :
        SchemaEdmGenericBase<TDataContext, TLinqEntity>,
        IInsertEdmGenericBase<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
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
        public InsertEdmGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public InsertEdmGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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

        #region Public Virtual Insert Item Methods
        /// <summary>
        /// Insert the Linq Enity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The linq entity containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity InsertItem(TLinqEntity linqEntityItem)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            return InsertItem<TLinqEntity>(linqEntityItem);
        }

        /// <summary>
        /// Insert the Linq Enity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity InsertItem<TypeLinqEntity>(TypeLinqEntity linqEntityItem)
            where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            // Insert the linq enity values.
            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.Set<TypeLinqEntity>().Add(linqEntityItem);
            int ret = DataContext.SaveChanges();

            // Assign the current linq entity with
            // the new primary key value.
            return (ret > 0) ? linqEntityItem : null;
        }
        #endregion

        #region Public Virtual Insert Collection Methods
        /// <summary>
        /// Insert the collection of linq entities.
        /// </summary>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity[] InsertCollection(TLinqEntity[] linqEntityItems)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            return InsertCollection<TLinqEntity>(linqEntityItems);
        }

        /// <summary>
        /// Insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity[] InsertCollection<TypeLinqEntity>(
            TypeLinqEntity[] linqEntityItems)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            // Create a new collection instance
            // of the linq entity.
            TypeLinqEntity[] linqEntityArray = new TypeLinqEntity[linqEntityItems.Count()];

            // Initial count;
            int i = 0;

            // For each item in the collection
            // insert the new linq item.
            foreach (TypeLinqEntity linqEntity in linqEntityItems)
            {
                try
                {
                    linqEntityArray[i++] = InsertCollectionEx<TypeLinqEntity>(linqEntity);
                }
                catch (Exception exp)
                {
                    // A System.Data.Linq.ChangeConflictException
                    // exception may have occurred.
                    ErrorProvider(new Exception("Insertion of data entity failed."),
                        "InsertCollection", exp.Message + " " + exp.Source + " " +
                        "{" + ConvertNullTypeToString(linqEntityItems) + "}");

                    return null;
                }
            }

            // Insert the linq enity values.
            // Submit the changes that have been made
            // if any conflict then throw exception.
            int ret = DataContext.SaveChanges();

            // Return the collection of linq entities
            // including the new values.
            return (ret > 0) ? linqEntityArray : null;
        }
        #endregion

        #region Public Virtual Insert Methods
        /// <summary>
        /// Inserts the array of linq entities.
        /// </summary>
        /// <param name="linqEntities">The array of linq entities.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertLinqEntities(TLinqEntity[] linqEntities)
        {
            TLinqEntity[] linqEntityItems = InsertCollection(linqEntities);
            return (linqEntityItems != null);
        }

        /// <summary>
        /// Inserts the array of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntities">The array of linq entities.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertLinqEntities<TypeLinqEntity>(TypeLinqEntity[] linqEntities)
            where TypeLinqEntity : class, new()
        {
            TypeLinqEntity[] linqEntityItems = InsertCollection<TypeLinqEntity>(linqEntities);
            return (linqEntityItems != null);
        }

        /// <summary>
        /// Inserts the linq entity.
        /// </summary>
        /// <param name="linqEntity">The linq entity.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertLinqEntity(TLinqEntity linqEntity)
        {
            TLinqEntity linqEntityItem = InsertItem(linqEntity);
            return (linqEntityItem != null);
        }

        /// <summary>
        /// Inserts the linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The linq entity.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertLinqEntity<TypeLinqEntity>(TypeLinqEntity linqEntity)
            where TypeLinqEntity : class, new()
        {
            TypeLinqEntity linqEntityItem = InsertItem<TypeLinqEntity>(linqEntity);
            return (linqEntityItem != null);
        }
        #endregion

        #region Public Asynchronous Insert Methods
        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertCollection(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncInsertCollection<TDataContext, TLinqEntity>
                (this, callback, state, linqEntityItems);
        }

        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities with new values.</returns>
        public TLinqEntity[] EndInsertCollection(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncInsertCollection<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems)
            where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncInsertCollection<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, linqEntityItems);
        }

        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities with new values.</returns>
        public TypeLinqEntity[] EndInsertCollection<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncInsertCollection<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertLinqEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncInsertLinqEntities<TDataContext, TLinqEntity>
                (this, callback, state, linqEntityItems);
        }

        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        public Boolean EndInsertLinqEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncInsertLinqEntities<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertLinqEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems)
            where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncInsertLinqEntities<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, linqEntityItems);
        }

        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        public Boolean EndInsertLinqEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncInsertLinqEntities<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }
        #endregion

        #region Private Insert Item Methods
        /// <summary>
        /// Insert the Linq Enity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private TypeLinqEntity InsertCollectionEx<TypeLinqEntity>(TypeLinqEntity linqEntityItem)
                where TypeLinqEntity : class, new()
        {
            // Insert the linq enity values.
            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.Set<TypeLinqEntity>().Add(linqEntityItem);

            // Assign the current linq entity with
            // the new primary key value.
            return linqEntityItem;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The update base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public class UpdateEdmGenericBase<TDataContext, TLinqEntity> :
        SchemaEdmGenericBase<TDataContext, TLinqEntity>,
        IUpdateEdmGenericBase<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
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
        public UpdateEdmGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public UpdateEdmGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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
        /// Updates the current entity item.
        /// </summary>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateLinqEntity(TDataContext dataContext, TLinqEntity linqEntity)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntity == null) throw new ArgumentNullException("linqEntity");

            return UpdateLinqEntity<TDataContext, TLinqEntity>(dataContext, linqEntity);
        }

        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <typeparam name="TypeDataContext">The data context to examine.</typeparam>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateLinqEntity<TypeDataContext, TypeLinqEntity>(
            TypeDataContext dataContext, TypeLinqEntity linqEntity)
            where TypeDataContext : System.Data.Entity.DbContext, new()
            where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntity == null) throw new ArgumentNullException("linqEntity");

            int ret = 0;

            try
            {
                ret = dataContext.SaveChanges();
            }
            catch (OptimisticConcurrencyException cexp)
            {
                ErrorProvider(cexp,
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }
            catch (Exception exp)
            {
                ErrorProvider(exp,
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(TLinqEntity linqEntity, 
            string predicate, params ObjectParameter[] parameters)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return UpdateItemPredicate<TLinqEntity>(linqEntity, predicate, parameters);
        }

        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeLinqEntity>(TypeLinqEntity linqEntity,
            string predicate, params ObjectParameter[] parameters)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.
                Set<TypeLinqEntity>().
                Where(predicate, parameters).
                First();

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // Get the collection of linq entity
                // fields that are within the linq
                // entity item.
                PropertyInfo[] linqEntityItemFields = linqEntity.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // For each field within the entity.
                foreach (PropertyInfo infoItem in linqEntityItemFields)
                {
                    // Get the current entity property value.
                    object value = infoItem.GetValue(linqEntity, null);

                    if (value != null)
                    {
                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == infoItem.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // Get the original value
                            // within the current property.
                            object originalValue = propertyInfo.GetValue(query, null);

                            // If the original and value are different
                            // set the current property with the new value,
                            // no need to set the property if the same.
                            if (originalValue != value)
                            {
                                // If the current property within the property collection
                                // is the current property name within the data collection.
                                if (propertyInfo.Name.ToLower().TrimStart('_') ==
                                    infoItem.Name.ToLower().TrimStart('_'))
                                {
                                    // If the current property is not database
                                    // auto generated, no need to update auto
                                    // generated properties.
                                    propertyInfo.SetValue(query, value, null);
                                }
                            }
                        }
                    }
                }

                int ret = 0;

                try
                {
                    ret = DataContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException cexp)
                {
                    ErrorProvider(cexp,
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }
                catch (Exception exp)
                {
                    ErrorProvider(exp,
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }

                // Return true if no errors else false.
                return (ret > 0);
            }
            else
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(TLinqEntity linqEntity,
            Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return UpdateItemPredicate<TLinqEntity>(linqEntity, predicate);
        }

        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeLinqEntity>(TypeLinqEntity linqEntity,
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.
                Set<TypeLinqEntity>().
                Where(predicate).
                First();

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // Get the collection of linq entity
                // fields that are within the linq
                // entity item.
                PropertyInfo[] linqEntityItemFields = linqEntity.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // For each field within the entity.
                foreach (PropertyInfo infoItem in linqEntityItemFields)
                {
                    // Get the current entity property value.
                    object value = infoItem.GetValue(linqEntity, null);

                    if (value != null)
                    {
                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == infoItem.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // Get the original value
                            // within the current property.
                            object originalValue = propertyInfo.GetValue(query, null);

                            // If the original and value are different
                            // set the current property with the new value,
                            // no need to set the property if the same.
                            if (originalValue != value)
                            {
                                // If the current property within the property collection
                                // is the current property name within the data collection.
                                if (propertyInfo.Name.ToLower().TrimStart('_') ==
                                    infoItem.Name.ToLower().TrimStart('_'))
                                {
                                    // If the current property is not database
                                    // auto generated, no need to update auto
                                    // generated properties.
                                    propertyInfo.SetValue(query, value, null);
                                }
                            }
                        }
                    }
                }

                int ret = 0;

                try
                {
                    ret = DataContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException cexp)
                {
                    ErrorProvider(cexp,
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }
                catch (Exception exp)
                {
                    ErrorProvider(exp,
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                }

                // Return true if no errors else false.
                return (ret > 0);
            }
            else
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                    "UpdateLinqEntity", "Updates the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullTypeToString(linqEntity) + "}" +
                    "{" + GetTableName<TypeLinqEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #region Public Virtual Update Collection Methods
        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateCollection(TLinqEntity[] linqEntityItems)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            return UpdateCollection<TLinqEntity>(linqEntityItems);
        }

        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateCollection<TypeLinqEntity>(TypeLinqEntity[] linqEntityItems)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            // Get all primary keys.
            List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeLinqEntity>();

            // No primary keys found.
            if (primaryKeys.Count < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "UpdateCollection",
                    "Can not update the data because no primary key has been found.");
                return false;
            }

            // Return indicator.
            int ret = 0;
            bool retCol = false;

            // Get the collection of all properties
            // in the current type.
            List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

            // For each item in the collection
            // update the data item.
            foreach (TypeLinqEntity linqEntity in linqEntityItems)
            {
                int count = -1;
                List<object> values = new List<object>();

                int i = 0;
                string[] keys = new string[primaryKeys.Count];

                // For each field within the entity.
                foreach (PropertyInfo primaryKey in primaryKeys)
                {
                    // Find in the property collection the current property that matches
                    // the current column. Use the Predicate delegate object to
                    // initiate a search for the specified match.
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = linqProperties.First(p => p.Name.ToLower() == primaryKey.Name.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (primaryKey.Name.ToLower().TrimStart('_') ==
                                    propertyInfo.Name.ToLower().TrimStart('_'))
                        {
                            // Get the current value
                            // within the current property.
                            object value = propertyInfo.GetValue(linqEntity, null);
                            values.Add(value);

                            //string name = GetDbColumnName<TypeLinqEntity>(property).ToLower().TrimStart('_');
                            string name = propertyInfo.Name.ToLower().TrimStart('_');
                            keys[i++] = name + " == @" + (count++).ToString();
                        }
                    }
                }

                string keyItems = string.Join(" AND ", keys);

                // Create the current row query item.
                string queryText = "(" + keyItems + ")";

                // Create the query text and execute
                // the collection method.
                retCol = UpdateCollectionEx<TypeLinqEntity>(linqEntity, queryText, values.ToArray());
                if (!retCol)
                    return false;
            }

            // Submit the changes that have been made
            // if any conflict then throw exception.
            ret = DataContext.SaveChanges();

            // Return the collection of linq entities
            // including the new values.
            return (ret > 0);
        }
        #endregion

        #region Public Asynchronous Update Methods
        /// <summary>
        /// Begin updates the linq entities to the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateCollection(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems)
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncUpdateCollection<TDataContext, TLinqEntity>
                (this, callback, state, linqEntityItems);
        }

        /// <summary>
        /// End updates the linq entities to the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if update was successful else false.</returns>
        public bool EndUpdateCollection(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncUpdateCollection<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems)
            where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new Edm.Async.AsyncUpdateCollection<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, linqEntityItems);
        }

        /// <summary>
        /// End updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if update was successful else false.</returns>
        public bool EndUpdateCollection<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Edm.Async.AsyncUpdateCollection<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }
        #endregion

        #region Private Update Methods
        /// <summary>
        /// Update the collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="linqEntity">The linq entity to update.</param>
        /// <param name="queryText">The predicate query text.</param>
        /// <param name="values">The query parameters.</param>
        /// <returns>True if succesful else false.</returns>
        private bool UpdateCollectionEx<TypeLinqEntity>(
            TypeLinqEntity linqEntity, string queryText, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Get the first item found.
            TypeLinqEntity query = DataContext.
                Set<TypeLinqEntity>().
                First(queryText, values);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // Get the collection of linq entity
                // fields that are within the linq
                // entity item.
                PropertyInfo[] linqEntityItemFields = linqEntity.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // For each field within the entity.
                foreach (PropertyInfo infoItem in linqEntityItemFields)
                {
                    // Get the current entity property value.
                    object value = infoItem.GetValue(linqEntity, null);
                    if (value != null)
                    {
                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == infoItem.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                            {
                                // Get the original value
                                // within the current property.
                                object originalValue = propertyInfo.GetValue(query, null);

                                // If the current property within the property collection
                                // is the current propert name within the data collection.
                                if (infoItem.Name.ToLower().TrimStart('_') ==
                                    propertyInfo.Name.ToLower().TrimStart('_'))
                                {
                                    if (originalValue != value)
                                        propertyInfo.SetValue(query, value, null);
                                }
                            }
                        }
                    }
                }

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                     "UpdateCollection", "Updates the collection of linq entities found. " +
                     "A concurrency error may have occurred. " +
                     "{" + ConvertNullToString(queryText));

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Get the primary key name from the context meta data work space.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="workspace">The current context meta data work space.</param>
        /// <param name="linqEntityName">The current entity item name.</param>
        /// <returns>The primary key.</returns>
        private string GetPrimaryKeyColumnName<TypeLinqEntity>(MetadataWorkspace workspace, string linqEntityName)
            where TypeLinqEntity : class, new()
        {
            // Get the current entity type from the work space
            // in the conceptual work space.
            EntityType type = workspace.GetItem<EntityType>(linqEntityName, DataSpace.CSpace);

            // Get all the key members of the entity.
            ReadOnlyMetadataCollection<EdmMember> properties = type.KeyMembers;

            // Iterate through the properties.
            foreach (EdmMember result in properties)
            {
                string ff = result.Name;
            }
            return null;
        }

        /// <summary>
        /// Gets items from the specified data model space.
        /// </summary>
        /// <param name="workspace">The current context meta data work space.</param>
        /// <param name="model">The data space model.</param>
        private void GetItemsFromModel(MetadataWorkspace workspace, DataSpace model)
        {
            // For the conceptual model (DataSpace.CSpace):
            // This collection contains all types defined in .csdl file and 
            // also 
            // the canonical functions and primitive types defined 
            // in the Entity Data Model (EDM).
            // For the storage model (DataSpace.SSpace):
            // This collection contains all types defined in .ssdl file 
            // and also
            // all SQL Server primitive types and functions.

            // An EdmType class is the base class for the classes that 
            // represent types in the Entity Data Model (EDM).
            ReadOnlyCollection<EntityType> types =
                workspace.GetItems<EntityType>(model);

        }
        #endregion

        #endregion
    }
}
