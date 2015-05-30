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
using System.Linq.Expressions;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.Odbc;
using System.ComponentModel;
using System.Threading.Tasks;

using LinqTypes = Nequeo.Data.DataType.ProviderToDataTypes;

using Nequeo.ComponentModel;
using Nequeo.Linq.Extension;
using Nequeo.Data.DataType;

namespace Nequeo.Data
{
    /// <summary>
    /// The schema base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public abstract class SchemaDataGenericBase<TDataEntity> : Nequeo.Handler.LogHandler, IDisposable
        where TDataEntity : class, new()
    {
        #region SchemaLinqGenericBase Abstract Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected SchemaDataGenericBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(applicationName, eventNamespace)
        {
            _connectionType = connectionType;
            _connectionDataType = connectionDataType;
            _dataAccessProvider = dataAccessProvider;

            _functionConnectionType = connectionType;
            _functionConnectionDataType = connectionDataType;
            _functionDataAccessProvider = dataAccessProvider;

            _functionDatabaseConnectionString = configurationDatabaseConnection;
            _configurationDatabaseConnection = configurationDatabaseConnection;
            _dataTypeConversion = new DataTypeConversion(connectionDataType);
            this.Initialise(string.Empty);
        }
        #endregion

        #region Private Constant
        private const string applicationName = "Nequeo.Data";
        private const string eventNamespace = "Nequeo.Data";
        #endregion

        #region Private Fields
        private string _fileLogErrorPath = string.Empty;
        private string _fileLogProcessPath = string.Empty;

        private string _specificPath = string.Empty;
        private string _errorProvider = string.Empty;
        private string _informationProvider = string.Empty;
        private Nequeo.Handler.WriteTo _errorWriteTo = Nequeo.Handler.WriteTo.EventLog;
        private Nequeo.Handler.WriteTo _informationWriteTo = Nequeo.Handler.WriteTo.EventLog;

        private string _configurationDatabaseConnection = string.Empty;
        private ConnectionContext.ConnectionType _connectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _connectionDataType = ConnectionContext.ConnectionDataType.None;
        private DataTypeConversion _dataTypeConversion = null;
        private IDataAccess _dataAccessProvider;

        private bool _useBulkInsert = true;
        private bool _useBulkDelete = true;
        private bool _useBulkUpdate = true;

        private string _functionDatabaseConnectionString = string.Empty;
        private ConnectionContext.ConnectionType _functionConnectionType = ConnectionContext.ConnectionType.None;
        private ConnectionContext.ConnectionDataType _functionConnectionDataType = ConnectionContext.ConnectionDataType.None;
        private IDataAccess _functionDataAccessProvider;
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
        /// Gets sets, the data access provider
        /// </summary>
        public IDataAccess FunctionDataAccessProvider
        {
            get { return _functionDataAccessProvider; }
            set { _functionDataAccessProvider = value; }
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
        /// Gets sets, the database connection string.
        /// </summary>
        public String FunctionDatabaseConnectionString
        {
            get { return DefaultConnection(_functionDatabaseConnectionString); }
            set { _functionDatabaseConnectionString = DefaultConnection(value); }
        }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        public ConnectionContext.ConnectionType FunctionConnectionType
        {
            get { return _functionConnectionType; }
            set { _functionConnectionType = value; }
        }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        public ConnectionContext.ConnectionDataType FunctionConnectionDataType
        {
            get { return _functionConnectionDataType; }
            set { _functionConnectionDataType = value; }
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
        /// Gets, the data collection configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.DataCollection DataCollectionReader
        {
            get { return Nequeo.Data.Configuration.DataConfigurationManager.DataCollection(); }
        }

        /// <summary>
        /// Gets, the data default configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.DataElementDefault DataElementReader
        {
            get { return Nequeo.Data.Configuration.DataConfigurationManager.DataElement(); }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// Event handler for providing error information.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Data.Control.InformationProviderArgs> OnErrorProvider;

        /// <summary>
        /// Event handler for providing information.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Data.Control.InformationProviderArgs> OnInformationProvider;
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

        #region Public Reflection, Attrubute Methods
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
        /// Get all properties in the data entity that are primary keys.
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
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Data.Custom.DataColumnAttribute att =
                            (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                        // If the property is a primary key.
                        if (att.IsPrimaryKey)
                            primaryKeys.Add(member);
                    }
                }
            }

            // return the collection of properties.
            return primaryKeys;
        }

        /// <summary>
        /// Get all row versioning properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        public List<PropertyInfo> GetAllRowVersions<T>()
        {
            // Create a new property collection.
            List<PropertyInfo> rowVersion = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(T)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Data.Custom.DataColumnAttribute att =
                            (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                        // If the property attribute
                        // is a row versioner.
                        if (att.IsRowVersion)
                            rowVersion.Add(member);
                    }
                }
            }

            // Return the collection of
            // row versioning properties.
            return rowVersion;
        }

        /// <summary>
        /// Get all reference properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        public List<PropertyInfo> GetAllReference<T>()
        {
            // Create a new property collection.
            List<PropertyInfo> reference = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(T)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                            (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;

                        // If the property attribute
                        // is a reference.
                        if (!att.IsReference)
                            reference.Add(member);
                    }
                }
            }

            // Return the collection of
            // reference properties.
            return reference;
        }

        /// <summary>
        /// Get all foreign key properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        public List<PropertyInfo> GetAllForeignKey<T>()
        {
            // Create a new property collection.
            List<PropertyInfo> foreignKey = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(T)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                            (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;

                        // If the property attribute
                        // is a foreign key it is
                        // not a reference.
                        if (att.IsReference)
                            foreignKey.Add(member);
                    }
                }
            }

            // Return the collection of
            // foreign key properties.
            return foreignKey;
        }

        /// <summary>
        /// Get all foreign key properties for the data entity type.
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns>The collection of properties.</returns>
        public List<PropertyInfo> GetAllForeignKey(object dataEntity)
        {
            // Create a new property collection.
            List<PropertyInfo> foreignKey = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in dataEntity.GetType().GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                            (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;

                        // If the property attribute
                        // is a foreign key it is
                        // not a reference.
                        if (att.IsReference)
                            foreignKey.Add(member);
                    }
                }
            }

            // Return the collection of
            // foreign key properties.
            return foreignKey;
        }

        /// <summary>
        /// Get all nullable properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        public List<PropertyInfo> GetAllNullable<T>()
        {
            // Create a new property collection.
            List<PropertyInfo> nullable = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(T)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        // Cast the current attribute.
                        Nequeo.Data.Custom.DataColumnAttribute att =
                            (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                        // If the property attribute
                        // is nullable.
                        if (att.IsNullable)
                            nullable.Add(member);
                    }
                }
            }

            // Return the collection of
            // nullable properties.
            return nullable;
        }

        /// <summary>
        /// Is the current property value a primary key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a primary key.</returns>
        /// <remarks>Check for a primary key for a data entity column.</remarks>
        public bool IsPrimaryKey(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                    // If the property attribute
                    // is a primary key.
                    if (att.IsPrimaryKey)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value auto generated in the database.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is auto generated.</returns>
        /// <remarks>Check for auto generated for a data entity column.</remarks>
        public bool IsAutoGenerated(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                    // If the property attribute
                    // is auto generated.
                    if (att.IsAutoGenerated)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value a row versioning column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a row versioning column.</returns>
        /// <remarks>Check for a row versioning column for a data entity column.</remarks>
        public bool IsRowVersion(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                    // If the property attribute
                    // is a row versioner.
                    if (att.IsRowVersion)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value a nullable column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a nullable column for a data entity column.</returns>
        public bool IsNullable(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                    // If the property attribute
                    // is nullable.
                    if (att.IsNullable)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value a foreign key column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a foreign key column for a data entity column.</returns>
        public bool IsForeignKey(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                        (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;

                    // If the property attribute
                    // is a foreign key then
                    // it is not a reference.
                    if (att.IsReference)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value a reference column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a reference column for a data entity column.</returns>
        public bool IsReference(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                        (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;

                    // If the property attribute
                    // is a reference.
                    if (!att.IsReference)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Get the DataColumnForeignKeyAttribute data for the current property information.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>The data column foreign key attribute data else null if attribute does not exist.</returns>
        public Nequeo.Data.Custom.DataColumnForeignKeyAttribute GetForeignKeyAttribute(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnForeignKeyAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnForeignKeyAttribute att =
                        (Nequeo.Data.Custom.DataColumnForeignKeyAttribute)attribute;
                    return att;
                }
            }

            // Return null.
            return null;
        }

        /// <summary>
        /// Is the current property value a table column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a nullable column for a data entity column.</returns>
        public bool IsColumnData(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Indicates that the current property
                    // is a table column.
                    return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Get all table column properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        public List<PropertyInfo> GetAllColumnData<T>()
        {
            // Create a new property collection.
            List<PropertyInfo> column = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(T)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        // Add the table column.
                        column.Add(member);
                    }
                }
            }

            // Return the collection of
            // columns properties.
            return column;
        }

        /// <summary>
        /// Get all table column properties for the data entity type.
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns>he collection of properties.</returns>
        public List<PropertyInfo> GetAllColumnData(object dataEntity)
        {
            // Create a new property collection.
            List<PropertyInfo> column = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in dataEntity.GetType().GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        // Add the table column.
                        column.Add(member);
                    }
                }
            }

            // Return the collection of
            // columns properties.
            return column;
        }

        /// <summary>
        /// Gets the current data entity table name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The table name.</returns>
        public string GetTableName<T>()
        {
            // Get the current member.
            MemberInfo member = typeof(T);

            // For each attribute for the member
            foreach (object attribute in member.GetCustomAttributes(true))
            {
                // If the attribute is the nequeo datatable
                // attribute then get the table name.
                if (attribute is Nequeo.Data.Custom.DataTableAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataTableAttribute att =
                        (Nequeo.Data.Custom.DataTableAttribute)attribute;

                    // Return the table name.
                    return att.TableName.TrimStart('_');
                }
            }

            // Return a null.
            return null;
        }

        /// <summary>
        /// Gets the current data entity database name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The table name.</returns>
        public string GetDatabaseName<T>()
        {
            // Get the current member.
            MemberInfo member = typeof(T);

            // For each attribute for the member
            foreach (object attribute in member.GetCustomAttributes(true))
            {
                // If the attribute is the nequeo datatable
                // attribute then get the table name.
                if (attribute is Nequeo.Data.Custom.DatabaseAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DatabaseAttribute att =
                        (Nequeo.Data.Custom.DatabaseAttribute)attribute;

                    // Return the table name.
                    return att.Name.TrimStart('_');
                }
            }

            // Return a null.
            return null;
        }

        /// <summary>
        /// Gets the current data entity column name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The column name.</returns>
        public string GetDbColumnName<T>(PropertyInfo property)
        {
            // For each attribute for the member
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the nequeo datatable
                // attribute then get the table name.
                if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                {
                    // Cast the current attribute.
                    Nequeo.Data.Custom.DataColumnAttribute att =
                        (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                    // Return the column name.
                    return att.ColumnName.TrimStart('_');
                }
            }

            // Return a null.
            return null;
        }

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        public bool HasChanged(TDataEntity original, TDataEntity current)
        {
            return HasChanged<TDataEntity>(original, current);
        }

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        public bool HasChanged<TData>(TData original, TData current)
        {
            return Nequeo.Data.Control.Operation.HasChanged<TData>(original, current);
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
            Write(typeof(TDataEntity).GetType().FullName, method + ". Description : " + methodDescription,
                exception.Message, 401, _errorWriteTo, Nequeo.Handler.LogType.Error);

            if (OnErrorProvider != null)
                OnErrorProvider(this, new Nequeo.Data.Control.InformationProviderArgs(_errorProvider));
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
            Write(typeof(TDataEntity).GetType().FullName, method, information, 201,
                 _informationWriteTo, Nequeo.Handler.LogType.Process);

            if (OnInformationProvider != null)
                OnInformationProvider(this, new Nequeo.Data.Control.InformationProviderArgs(_informationProvider));
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
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        public Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, params DbParameter[] values)
        {
            return ExecuteCommand(ref dbCommand, commandText, commandType,
                DefaultConnection(ConfigurationDatabaseConnection), values);
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

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, _connectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, _connectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, values);
                        break;

                    default:
                        returnValue = _dataAccessProvider.ExecuteCommand(ref dbCommand, commandText, commandType, connectionString, _connectionDataType, values);
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

                    case ConnectionContext.ConnectionType.OleDbConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, _connectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, _connectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OracleClientConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.PostgreSqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.MySqlConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    case ConnectionContext.ConnectionType.SqliteConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, values);
                        break;

                    default:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataTable, queryText, commandType, connectionString, getSchemaTable, _connectionDataType, values);
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
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _connectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _connectionDataType, values);
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
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _connectionDataType, values);
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
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _connectionDataType, values);
                        break;

                    case ConnectionContext.ConnectionType.OdbcConnection:
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _connectionDataType, values);
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
                        dbCommand = _dataAccessProvider.ExecuteQuery(ref dataSet, tables, queryText, commandType, connectionString, _connectionDataType, values);
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

        #region Public Execute Function Methods
        /// <summary>
        /// Execute a function routine.
        /// </summary>
        /// <param name="instance">The current data base instance.</param>
        /// <param name="methodInfo">The method information to execute.</param>
        /// <param name="parameters">The function routine parameters.</param>
        /// <returns>The execution result.</returns>
        public Nequeo.Data.Control.IExecuteFunctionResult ExecuteFunction(
            Nequeo.Data.Control.IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters)
        {
            return new Nequeo.Data.Control.FunctionRountineHandler().ExecuteFunction(instance, methodInfo, parameters);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The select base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public class SelectDataGenericBase<TDataEntity> : SchemaDataGenericBase<TDataEntity>,
        ISelectDataGenericBase<TDataEntity>
        where TDataEntity : class, new()
    {
        #region Select Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public SelectDataGenericBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider)
        {
        }
        #endregion

        #region Private Fields
        private string _cachedItemName = string.Empty;
        private bool _cacheItems = false;
        private Int32 _cacheTimeout = 120;
        private bool _referenceLazyLoading = false;
        private Int32 _isSetReferenceLazyLoading = 0;

        private DataTable _dataTable = null;
        private TDataEntity[] _dataEntities = null;
        private IQueryable<TDataEntity> _query = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the foregin key reference lazy loading indicator.
        /// </summary>
        public Boolean LazyLoading
        {
            get { return _referenceLazyLoading; }
            set
            {
                _referenceLazyLoading = value;
                if (_referenceLazyLoading)
                    _isSetReferenceLazyLoading = 1;
                else
                    _isSetReferenceLazyLoading = 2;
            }
        }

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
        public IQueryable<TDataEntity> IQueryable
        {
            get { return _query; }
        }

        /// <summary>
        /// Gets the data table containing the
        /// collection of table data.
        /// </summary>
        public DataTable DataTable
        {
            get { return _dataTable; }
        }

        /// <summary>
        /// Gets the current data entities type.
        /// </summary>
        public TDataEntity[] DataEntities
        {
            get { return _dataEntities; }
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
        public ISelectDataGenericBase<TDataEntity> AddCachingControl(string cachedItemName, Int32 cacheTimeout)
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
            Nequeo.Caching.RuntimeCache.Add(typeof(TDataEntity).Name, cachedItemName, value, (double)cacheTimeout);
        }
        #endregion

        #region Public Virtual Select Data Methods
        /// <summary>
        /// Gets the data entity item.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The data entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity SelectDataEntity(Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            TDataEntity[] data = null;
            data = SelectDataEntitiesPredicate(predicate);

            // Return the details of the operation.
            return (data.Count() > 0) ? data[0] : null;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] SelectDataEntities(string queryText, CommandType commandType,
            params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            return SelectDataEntities<TDataEntity>(queryText, commandType, values);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] SelectDataEntities<TypeDataEntity>(string queryText, CommandType commandType,
            params DbParameter[] values)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            SelectEx(ref _dataTable, queryText, commandType, true, values);
            return GetListCollection<TypeDataEntity>(_dataTable).ToArray<TypeDataEntity>();
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] SelectDataEntities(string queryText)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            return SelectDataEntities<TDataEntity>(queryText);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] SelectDataEntities<TypeDataEntity>(string queryText)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            SelectEx(ref _dataTable, queryText, CommandType.Text, true, null);
            return GetListCollection<TypeDataEntity>(_dataTable).ToArray<TypeDataEntity>();
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] SelectDataEntities()
        {
            return SelectDataEntities<TDataEntity>();
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] SelectDataEntities<TypeDataEntity>()
            where TypeDataEntity : class, new()
        {
            // Return the data table.
            string queryText = "SELECT * FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]";
            SelectEx(ref _dataTable, queryText, CommandType.Text, true, null);
            return GetListCollection<TypeDataEntity>(_dataTable).ToArray<TypeDataEntity>();
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTable(string queryText, CommandType commandType,
            params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            SelectEx(ref _dataTable, queryText, commandType, true, values);
            return _dataTable;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTable(string queryText)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            // Return the data table.
            SelectEx(ref _dataTable, queryText, CommandType.Text, true, null);
            return _dataTable;
        }

        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTable()
        {
            // Return the data table.
            string queryText = "SELECT * FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]";
            SelectEx(ref _dataTable, queryText, CommandType.Text, true, null);
            return _dataTable;
        }
        #endregion

        #region Public Virtual Select Data Collection Methods
        
        /// <summary>
        /// Select the list collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The data collection items</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<TDataEntity> SelectListCollection(string queryText, CommandType commandType,
            params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            DbCommand sqlCommand = SelectEx(ref _dataTable, queryText, commandType, true, values);

            // Get the data collection.
            List<TDataEntity> dataCollection = GetListCollection<TDataEntity>(_dataTable);

            // Return true if not null
            // else return false.
            return dataCollection;
        }

        /// <summary>
        /// Select the list collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The data collection items</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<TDataEntity> SelectListCollection(string queryText)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            DbCommand sqlCommand = SelectEx(ref _dataTable, queryText, CommandType.Text, true, null);

            // Get the data collection.
            List<TDataEntity> dataCollection = GetListCollection<TDataEntity>(_dataTable);

            // Return true if not null
            // else return false.
            return dataCollection;
        }

        /// <summary>
        /// Select all the list collection of data for the table.
        /// </summary>
        /// <returns>The data collection items</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<TDataEntity> SelectListCollection()
        {
            string queryText = "SELECT * FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]";
            DbCommand sqlCommand = SelectEx(ref _dataTable, queryText, CommandType.Text, true, null);

            // Get the data collection.
            List<TDataEntity> dataCollection = GetListCollection<TDataEntity>(_dataTable);

            // Return true if not null
            // else return false.
            return dataCollection;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TDataEntity> SelectIQueryableItems(
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the array of data entities.
            IQueryable<TDataEntity> dataEntities = null;
            dataEntities = SelectDataEntitiesPredicate(predicate).AsQueryable<TDataEntity>();
            return dataEntities;
        }
        #endregion

        #region Public Virtual Select Predicate Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTablePredicate(Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity> expression = new Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity>();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + whereQueryText;

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);

            return table;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTablePredicate(
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Return the data table.
            DataTable table = null;
            SelectCollectionPredicate(ref table, predicate, values);
            return table;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollectionPredicate(ref DataTable dataTable,
            string predicate, params object[] values)
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

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + predicateInternal;

            // Execute the query. return the result
            // into the data table.
            DbCommand sqlCommand = SelectEx(ref dataTable, queryText, CommandType.Text, true, null);

            // Return the result.
            return sqlCommand;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="lazyLoading">The lazy loading indicator.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] SelectDataEntitiesPredicate(
            string predicate, bool lazyLoading, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Return the data table.
            LazyLoading = lazyLoading;
            TDataEntity[] dataEntities = null;
            SelectCollectionPredicate<TDataEntity>(ref dataEntities, predicate, values);
            return dataEntities;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] SelectDataEntitiesPredicate(
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Return the data table.
            TDataEntity[] dataEntities = null;
            SelectCollectionPredicate<TDataEntity>(ref dataEntities, predicate, values);
            return dataEntities;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] SelectDataEntitiesPredicate<TypeDataEntity>(
            string predicate, params object[] values)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Return the data table.
            TypeDataEntity[] dataEntities = null;
            SelectCollectionPredicate<TypeDataEntity>(ref dataEntities, predicate, values);
            return dataEntities;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataEntities">The data entities to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollectionPredicate(ref TDataEntity[] dataEntities,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TDataEntity>(ref dataEntities,
                predicate, values);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="dataEntities">The data entities to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DbCommand SelectCollectionPredicate<TypeDataEntity>(ref TypeDataEntity[] dataEntities,
            string predicate, params object[] values)
            where TypeDataEntity : class, new()
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
                    for(int i = 0; i < values.Count(); i++)
                    {
                        predicateInternal = currentPredicate.Replace("@" + i, base.DataTypeConversion.GetSqlStringValue(values[i].GetType(), values[i]));
                        currentPredicate = predicateInternal;
                    }
                }
            }

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + predicateInternal;

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);

            // Get the array of data entities.
            dataEntities = GetListCollection<TypeDataEntity>(table).ToArray<TypeDataEntity>();

            // Return the result.
            return sqlCommand;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] SelectDataEntitiesPredicate(
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SelectDataEntitiesPredicate<TDataEntity>(predicate);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] SelectDataEntitiesPredicate<TypeDataEntity>(
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataEntity>> predicate)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataEntity>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionTreeDataGeneric<TypeDataEntity> expression = new Nequeo.Data.Linq.ExpressionTreeDataGeneric<TypeDataEntity>();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + whereQueryText;

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);

            // Get the array of data entities.
            TypeDataEntity[] dataEntities = null;
            dataEntities = GetListCollection<TypeDataEntity>(table).ToArray<TypeDataEntity>();
            return dataEntities;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] SelectDataEntitiesExpression(Expression expression)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (expression == null) throw new ArgumentNullException("expression");

            return SelectDataEntitiesExpression<TDataEntity>(expression);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="expression">The query expression.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] SelectDataEntitiesExpression<TypeDataEntity>(Expression expression)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (expression == null) throw new ArgumentNullException("expression");

            Nequeo.Data.Linq.Common.QueryLanguage language = null;
            ConnectionContext.ConnectionType mappingType = ConnectionContext.ConnectionType.None;
            ConnectionContext.ConnectionDataType mappingDataType = ConnectionContext.ConnectionDataType.None;

            switch (this.ConnectionType)
            {
                case ConnectionContext.ConnectionType.SqlConnection:
                    language = new Nequeo.Data.Linq.Language.TSqlLanguage(DataAccessProvider.CommandBuilder);
                    mappingType = ConnectionContext.ConnectionType.SqlConnection;
                    mappingDataType = ConnectionContext.ConnectionDataType.SqlDataType;
                    break;

                case ConnectionContext.ConnectionType.OracleClientConnection:
                    language = new Nequeo.Data.Linq.Language.PLSqlLanguage(DataAccessProvider.CommandBuilder);
                    mappingType = ConnectionContext.ConnectionType.OracleClientConnection;
                    mappingDataType = ConnectionContext.ConnectionDataType.OracleDataType;
                    break;

                case ConnectionContext.ConnectionType.PostgreSqlConnection:
                    language = new Nequeo.Data.Linq.Language.PgSqlLanguage(DataAccessProvider.CommandBuilder);
                    mappingType = ConnectionContext.ConnectionType.PostgreSqlConnection;
                    mappingDataType = ConnectionContext.ConnectionDataType.PostgreSqlDataType;
                    break;

                case ConnectionContext.ConnectionType.MySqlConnection:
                    language = new Nequeo.Data.Linq.Language.MySqlLanguage(DataAccessProvider.CommandBuilder);
                    mappingType = ConnectionContext.ConnectionType.MySqlConnection;
                    mappingDataType = ConnectionContext.ConnectionDataType.MySqlDataType;
                    break;

                case ConnectionContext.ConnectionType.OleDbConnection:
                    language = new Nequeo.Data.Linq.Language.OleDbSqlLanguage(DataAccessProvider.CommandBuilder);
                    mappingType = ConnectionContext.ConnectionType.OleDbConnection;
                    switch (this.ConnectionDataType)
                    {
                        case ConnectionContext.ConnectionDataType.AccessDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.AccessDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.OracleDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.OracleDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.ScxDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.PostgreSqlDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.MySqlDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.MySqlDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.SqlDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.SqlDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.None:
                            mappingDataType = ConnectionContext.ConnectionDataType.None;
                            break;
                    }
                    break;

                case ConnectionContext.ConnectionType.OdbcConnection:
                    language = new Nequeo.Data.Linq.Language.OdbcSqlLanguage(DataAccessProvider.CommandBuilder);
                    mappingType = ConnectionContext.ConnectionType.OdbcConnection;
                    switch (this.ConnectionDataType)
                    {
                        case ConnectionContext.ConnectionDataType.AccessDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.AccessDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.OracleDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.OracleDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.ScxDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.ScxDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.PostgreSqlDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.MySqlDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.MySqlDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.SqlDataType:
                            mappingDataType = ConnectionContext.ConnectionDataType.SqlDataType;
                            break;
                        case ConnectionContext.ConnectionDataType.None:
                            mappingDataType = ConnectionContext.ConnectionDataType.None;
                            break;
                    }
                    break;
            }

            Nequeo.Data.Linq.Provider.QueryTranslater translator =
                new Nequeo.Data.Linq.Provider.QueryTranslater(
                    new Nequeo.Data.Linq.Common.QueryPolicy(
                        new Nequeo.Data.Linq.Common.ImplicitMapping(language, mappingType, mappingDataType)));

            // Create the sql string to pass
            // the the execute method.
            string queryText = translator.GetQueryText(expression);

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);

            // Get the array of data entities.
            TypeDataEntity[] dataEntities = null;
            dataEntities = GetListCollection<TypeDataEntity>(table).ToArray<TypeDataEntity>();
            return dataEntities;
        }
        #endregion

        #region Public Virtual Select Query Methods
        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public virtual DbCommand SelectQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery(ref dataTable, queryText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), getSchemaTable, values);
        }

        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The collection of tables to include in the dataset.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public virtual DbCommand SelectQueryItem(ref System.Data.DataSet dataSet, System.Data.DataTable[] tables,
            string queryText, CommandType commandType, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery(ref dataSet, tables, queryText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), values);
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

        /// <summary>
        /// Gets the queryable provider.
        /// </summary>
        /// <returns>The object queryable provider.</returns>
        public virtual Nequeo.Linq.QueryableProvider<TDataEntity> QueryableProvider()
        {
            string tableName = GetTableName<TDataEntity>();
            Nequeo.Data.Linq.SqlStatementConstructor<TDataEntity> sql = Nequeo.Data.Linq.SqlStatementConstructor<TDataEntity>.
                CreateInstance
                (
                    tableName,
                    ConnectionType,
                    DataTypeConversion,
                    DefaultConnection(ConfigurationDatabaseConnection),
                    DataAccessProvider
                );
            Nequeo.Linq.QueryableProvider<TDataEntity> queryableProvider =
                new Nequeo.Linq.QueryableProvider<TDataEntity>(sql);
            return queryableProvider;
        }

        /// <summary>
        /// Gets the queryable provider.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <returns>The object queryable provider.</returns>
        public virtual Nequeo.Linq.QueryableProvider<T> QueryableProvider<T>()
            where T : class, new()
        {
            string tableName = GetTableName<T>();
            Nequeo.Data.Linq.SqlStatementConstructor<T> sql = Nequeo.Data.Linq.SqlStatementConstructor<T>.
                CreateInstance
                (
                    tableName,
                    ConnectionType,
                    DataTypeConversion,
                    DefaultConnection(ConfigurationDatabaseConnection),
                    DataAccessProvider
                );
            Nequeo.Linq.QueryableProvider<T> queryableProvider =
                new Nequeo.Linq.QueryableProvider<T>(sql);
            return queryableProvider;
        }

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>The collection of data entity types.</returns>
        public virtual TDataEntity[] SelectData(int skip, string orderBy)
        {
            return this.QueryableProvider().OrderBy(orderBy).Skip(skip).ToArray();
        }

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <returns>The collection of data entity types.</returns>
        public virtual TDataEntity[] SelectData(int skip, int take, string orderBy)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (orderBy == null) throw new ArgumentNullException("orderBy");

            return QueryableProvider().OrderBy(orderBy).Skip(skip).Take(take).ToArray();
        }

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="predicate">The where clause.</param>
        /// <returns>The collection of data entity types.</returns>
        public virtual TDataEntity[] SelectData(int skip, string orderBy, string predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (orderBy == null) throw new ArgumentNullException("orderBy");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return QueryableProvider().OrderBy(orderBy).Where(predicate).Skip(skip).ToArray();
        }

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="predicate">The where clause.</param>
        /// <returns>The collection of data entity types.</returns>
        public virtual TDataEntity[] SelectData(int skip, int take, string orderBy, string predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (orderBy == null) throw new ArgumentNullException("orderBy");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return QueryableProvider().Where(predicate).OrderBy(orderBy).Skip(skip).Take(take).ToArray();
        }

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="predicate">The where clause.</param>
        /// <returns>The collection of data entity types.</returns>
        public virtual TDataEntity[] SelectData(int skip, string orderBy, Expression<Func<TDataEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (orderBy == null) throw new ArgumentNullException("orderBy");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return QueryableProvider().Where(predicate).OrderBy(orderBy).Skip(skip).ToArray();
        }

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="predicate">The where clause.</param>
        /// <returns>The collection of data entity types.</returns>
        public virtual TDataEntity[] SelectData(int skip, int take, string orderBy, Expression<Func<TDataEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (orderBy == null) throw new ArgumentNullException("orderBy");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return QueryableProvider().Where(predicate).OrderBy(orderBy).Skip(skip).Take(take).ToArray();
        }

        /// <summary>
        /// Get the total number of records.
        /// </summary>
        /// <returns>The total number of records.</returns>
        public virtual long GetRecordCount()
        {
            // Return the data table.
            string queryText = "SELECT Count(*) FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]";

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);
            return Convert.ToInt64(table.Rows[0][0]);
        }

        /// <summary>
        /// Get the total number of records.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The total number of records.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual long GetRecordCount(string predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT Count(*) FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + predicate.Replace("&&", "AND").Replace("||", "OR").Replace("==", "=").Replace("!=", "<>");

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);
            return Convert.ToInt64(table.Rows[0][0]);
        }

        /// <summary>
        /// Get the total number of records.
        /// </summary>
        /// <param name="predicate">The expression containing the predicate.</param>
        /// <returns>The total number of records.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual long GetRecordCount(Expression<Func<TDataEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Func<TDataEntity, bool>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity> expression = new Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity>();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT Count(*) FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + whereQueryText;

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);
            return Convert.ToInt64(table.Rows[0][0]);
        }
        #endregion

        #region Public Asynchronous Select Methods
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntitiesPredicate(AsyncCallback callback, object state,
            string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntitiesPredicate<TDataEntity>(
                this, callback, state, predicate, values);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntitiesPredicate(AsyncCallback callback, object state,
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntitiesPredicate<TDataEntity>(
                this, callback, state, predicate);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public virtual TDataEntity[] EndSelectDataEntitiesPredicate(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncSelectDataEntitiesPredicate<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntitiesPredicate<TypeDataEntity>(AsyncCallback callback, object state,
            string predicate, params object[] values)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntitiesPredicate<TDataEntity, TypeDataEntity>(
                this, callback, state, predicate, values);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public virtual TypeDataEntity[] EndSelectDataEntitiesPredicate<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncSelectDataEntitiesPredicate<TDataEntity, TypeDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataTablePredicate(AsyncCallback callback, object state,
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataTablePredicate<TDataEntity>(
                this, callback, state, predicate);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public virtual DataTable EndSelectDataTablePredicate(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncSelectDataTablePredicate<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntities<TypeDataEntity>(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntities<TDataEntity, TypeDataEntity>(
                this, callback, state, queryText, commandType, values);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntities<TypeDataEntity>(AsyncCallback callback, object state,
            string queryText)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntities<TDataEntity, TypeDataEntity>(
                this, callback, state, queryText);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntities<TypeDataEntity>(AsyncCallback callback, object state)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntities<TDataEntity, TypeDataEntity>(
                this, callback, state);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public virtual TypeDataEntity[] EndSelectDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncSelectDataEntities<TDataEntity, TypeDataEntity>.End(ar);
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
        public  IAsyncResult BeginSelectDataEntities(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values)
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntities<TDataEntity>(
                this, callback, state, queryText, commandType, values);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntities(AsyncCallback callback, object state,
            string queryText)
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntities<TDataEntity>(
                this, callback, state, queryText);
        }

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginSelectDataEntities(AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new Async.AsyncSelectDataEntities<TDataEntity>(
                this, callback, state);
        }

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        public virtual TDataEntity[] EndSelectDataEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncSelectDataEntities<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="degreeOfParallelism">Degree of parallelism is the maximum number 
        /// of concurrently executing tasks that will be used to process the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        public virtual ParallelQuery<TDataEntity> SelectDataEntitiesParallel(int degreeOfParallelism = 1)
        {
            if (degreeOfParallelism < 1) throw new IndexOutOfRangeException("Argument 'degreeOfParallelism' must be greater than zero.");
            return SelectDataEntitiesParallel<TDataEntity>(degreeOfParallelism);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to return.</typeparam>
        /// <param name="degreeOfParallelism">Degree of parallelism is the maximum number 
        /// of concurrently executing tasks that will be used to process the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        public virtual ParallelQuery<TypeDataEntity> SelectDataEntitiesParallel<TypeDataEntity>(int degreeOfParallelism = 1)
            where TypeDataEntity : class, new()
        {
            if (degreeOfParallelism < 1) throw new IndexOutOfRangeException("Argument 'degreeOfParallelism' must be greater than zero.");

            // Get the data.
            TypeDataEntity[] data = SelectDataEntities<TypeDataEntity>();
            ParallelQuery<TypeDataEntity> parallelQuery = null;

            // Load the parallel query data.
            parallelQuery = from b in data.AsParallel().WithDegreeOfParallelism(degreeOfParallelism)
                            select b;
            
            // Return the collection, allowing the user
            // to query the data in parallel.
            return parallelQuery;
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="degreeOfParallelism">Degree of parallelism is the maximum number 
        /// of concurrently executing tasks that will be used to process the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        public virtual ParallelQuery<TDataEntity> SelectDataEntitiesPredicateParallel(
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate, int degreeOfParallelism = 1)
        {
            if (degreeOfParallelism < 1) throw new IndexOutOfRangeException("Argument 'degreeOfParallelism' must be greater than zero.");
            return SelectDataEntitiesPredicateParallel<TDataEntity>(predicate, degreeOfParallelism);
        }

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to return.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="degreeOfParallelism">Degree of parallelism is the maximum number 
        /// of concurrently executing tasks that will be used to process the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        public virtual ParallelQuery<TypeDataEntity> SelectDataEntitiesPredicateParallel<TypeDataEntity>(
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataEntity>> predicate, int degreeOfParallelism = 1)
            where TypeDataEntity : class, new()
        {
            if (degreeOfParallelism < 1) throw new IndexOutOfRangeException("Argument 'degreeOfParallelism' must be greater than zero.");

            // Get the data.
            TypeDataEntity[] data = SelectDataEntitiesPredicate<TypeDataEntity>(predicate);
            ParallelQuery<TypeDataEntity> parallelQuery = null;

            // Load the parallel query data.
            parallelQuery = from b in data.AsParallel().WithDegreeOfParallelism(degreeOfParallelism)
                            select b;
            
            // Return the collection, allowing the user
            // to query the data in parallel.
            return parallelQuery;
        }
        #endregion

        #region Public Virtual Translator Methods
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] Translator()
        {
            return Translator<TDataEntity>();
        }

        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] Translator<TypeDataEntity>()
            where TypeDataEntity : class, new()
        {
            string queryText = "SELECT * FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]";

            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TypeDataEntity[], DataTable> function =
                (DataTable data) => this.Mapper<TypeDataEntity>(data);

            // Project the data table.
            return Projector<TypeDataEntity>(table, function);
        }

        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <param name="table">The table to translate.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] Translator(DataTable table)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (table == null) throw new ArgumentNullException("table");

            return Translator<TDataEntity>(table);
        }

        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="table">The table to translate.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] Translator<TypeDataEntity>(DataTable table)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (table == null) throw new ArgumentNullException("table");

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TypeDataEntity[], DataTable> function =
                (DataTable data) => this.Mapper<TypeDataEntity>(data);

            // Project the data table.
            return Projector<TypeDataEntity>(table, function);
        }

        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TDataEntity[] Translator(Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return Translator<TDataEntity>(predicate);
        }

        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeDataEntity[] Translator<TypeDataEntity>(Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataEntity>> predicate)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataEntity>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionTreeDataGeneric<TypeDataEntity> expression = new Nequeo.Data.Linq.ExpressionTreeDataGeneric<TypeDataEntity>();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Create the sql string to pass
            // the the execute method.
            string queryText = "SELECT * FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + whereQueryText;

            // Execute the query. return the result
            // into the data table.
            DataTable table = null;
            DbCommand sqlCommand = SelectEx(ref table, queryText, CommandType.Text, true, null);

            // Create the delegate function.
            Nequeo.Threading.FunctionHandler<TypeDataEntity[], DataTable> function =
                (DataTable data) => this.Mapper<TypeDataEntity>(data);

            // Project the data table.
            return Projector<TypeDataEntity>(table, function);
        }
        #endregion

        #region Select Private Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        private DbCommand SelectEx(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values)
        {
            // Initial connection objects.
            DbCommand dbCommand = null;
          
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
                        if (item is DataTable)
                        {
                            DataTable table = (DataTable)item;
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
                DataTable refDataTable = null;

                // Execute the command.
                dbCommand = ExecuteQuery(ref refDataTable, queryText,
                    commandType, base.DefaultConnection(base.ConfigurationDatabaseConnection), getSchemaTable, values);

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
            return dbCommand;
        }

        /// <summary>
        /// Gets the data collection from the data table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The type of object to return.</typeparam>
        /// <param name="dataTable">The datatable containing the data.</param>
        private List<TypeDataEntity> GetListCollection<TypeDataEntity>(DataTable dataTable)
            where TypeDataEntity : class, new()
        {
            // Create a new instance for the generic data type.
            List<TypeDataEntity> dataObjectCollection = new List<TypeDataEntity>();

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> propertiesRef = GetAllForeignKey<TypeDataEntity>();
            List<PropertyInfo> propertiesData = GetAllColumnData<TypeDataEntity>();

            // For each row within the data collection.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data business 
                // object for each row.
                TypeDataEntity data = new TypeDataEntity();

                // If properties exist within the data type.
                if (propertiesData.Count > 0)
                {
                    // For each column within the data collection.
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = propertiesData.First(p => p.Name.ToLower() == column.ColumnName.ToLower().TrimStart('_'));
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // If the current property within the property collection
                            // is the current column within the data collection.
                            if (GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                    column.ColumnName.ToLower().TrimStart('_'))
                            {
                                // If the data within the current row and column
                                // is 'NULL' then do not store the data.
                                if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                    // Assign the current data for the current row
                                    // into the current data business object.
                                    propertyInfo.SetValue(data, row[column.ColumnName.ToLower()], null);
                            }
                        }
                    }
                }

                // If properties exist within the data type.
                if (propertiesRef.Count > 0)
                {
                    // Should the reference data be loading
                    // if the lasy loading attribute is set to true.
                    if (ApplyLazyLoadingData<TypeDataEntity>())
                        TranslateLazyLoading(data);
                }

                // Add the current data row to the
                // business object collection.
                dataObjectCollection.Add(data);
            }

            // Return the collection.
            return dataObjectCollection;
        }

        /// <summary>
        /// Executes to delegate function for data table projection.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="table">The table to project.</param>
        /// <param name="function">The delegate function to execute.</param>
        /// <returns>The array of data entities.</returns>
        private TypeDataEntity[] Projector<TypeDataEntity>(DataTable table, Nequeo.Threading.FunctionHandler<TypeDataEntity[], DataTable> function)
            where TypeDataEntity : class, new()
        {
            if (table == null)
                throw new ArgumentNullException("table");

            // Call the function delegate with table parameter
            // to execute the method.
            return function(table);
        }

        /// <summary>
        /// Maps a data table to the corresponding data entities.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataTable">The table to map.</param>
        /// <returns>The array of data entities.</returns>
        private TypeDataEntity[] Mapper<TypeDataEntity>(DataTable dataTable)
            where TypeDataEntity : class, new()
        {
            // Create a new instance for the generic data type.
            TypeDataEntity[] dataObjectCollection = new TypeDataEntity[dataTable.Rows.Count];

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> propertiesRef = GetAllForeignKey<TypeDataEntity>();
            List<PropertyInfo> propertiesData = GetAllColumnData<TypeDataEntity>();
            int i = 0;

            // For each row within the data collection.
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new data business 
                // object for each row.
                TypeDataEntity data = new TypeDataEntity();

                // If properties exist within the data type.
                if (propertiesData.Count > 0)
                {
                    // For each column within the data collection.
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = propertiesData.First(p => p.Name.ToLower() == column.ColumnName.ToLower().TrimStart('_'));
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // If the current property within the property collection
                            // is the current column within the data collection.
                            if (GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                    column.ColumnName.ToLower().TrimStart('_'))
                            {
                                // If the data within the current row and column
                                // is 'NULL' then do not store the data.
                                if (row[column.ColumnName.ToLower()].GetType().ToString() != "System.DBNull")
                                    // Assign the current data for the current row
                                    // into the current data business object.
                                    propertyInfo.SetValue(data, row[column.ColumnName.ToLower()], null);
                            }
                        }
                    }
                }

                // If properties exist within the data type.
                if (propertiesRef.Count > 0)
                {
                    // Should the reference data be loading
                    // if the lasy loading attribute is set to true.
                    if (ApplyLazyLoadingData<TypeDataEntity>())
                        TranslateLazyLoading(data);
                }

                // Add the current data row to the
                // business object collection.
                dataObjectCollection[i++] = data;
            }

            // Return the collection.
            return dataObjectCollection;
        }

        /// <summary>
        /// Should lazy loading be applied to any foregin key reference columns.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <returns>True if the lazy loading attribute has been set else false.</returns>
        private bool ApplyLazyLoadingData<TypeDataEntity>()
            where TypeDataEntity : class, new()
        {
            if (_isSetReferenceLazyLoading > 0)
                return LazyLoading;
            else
            {
                if (base.DataCollectionReader != null)
                {
                    // Get the collection of data configuration info.
                    // Assign to the array of data elements.
                    Nequeo.Data.Configuration.DataElement[] data = new Nequeo.Data.Configuration.DataElement[base.DataCollectionReader.Count];
                    base.DataCollectionReader.CopyTo(data, 0);

                    if (data.Count() > 1)
                    {
                        // Get only the collection of config data that
                        // belongs to this database and this table.
                        IEnumerable<Nequeo.Data.Configuration.DataElement> items = data.Where(a => (a.TableName == GetTableName<TypeDataEntity>()) &&
                            (a.DatabaseName == GetDatabaseName<TypeDataEntity>()));

                        // Get the referenced lazy loading indicator.
                        if (items.Count() > 0)
                            return items.First().ReferenceLazyLoading;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else return false;
            }
        }

        /// <summary>
        /// Translate the lazy loading reference data.
        /// </summary>
        /// <param name="dataObject">The current object to add lazy loading to.</param>
        private void TranslateLazyLoading(object dataObject)
        {
            List<PropertyInfo> propertiesRef = GetAllForeignKey(dataObject);
            List<PropertyInfo> propertiesData = GetAllColumnData(dataObject);

            foreach (PropertyInfo property in propertiesRef)
            {
                // Get the current foreign key attribute for the property
                Nequeo.Data.Custom.DataColumnForeignKeyAttribute data = GetForeignKeyAttribute(property);

                PropertyInfo propertyInfo = null;
                try
                {
                    propertyInfo = propertiesData.First(p => p.Name.ToLower() == data.ColumnName.ToLower());
                }
                catch { }
                if (propertyInfo != null)
                {
                    // Get the current referenced coulm in the current data object.
                    object value = propertyInfo.GetValue(dataObject, null);

                    // Set the reference type lazy loading data.
                    if (value != null)
                    {
                        // Set the current reference object back to
                        // the parent type
                        property.SetValue(dataObject, SetForeginKeyReferenceData(property, value, data), null);

                        // Get the new refrence type object instance.
                        object newRefObject = property.GetValue(dataObject, null);

                        // Has the current reference column type contain
                        // reference columns, if true then apply recursion
                        List<PropertyInfo> propertiesRefValue = GetAllForeignKey(newRefObject);
                        if (propertiesRefValue.Count > 0)
                            TranslateLazyLoading(newRefObject);
                    }
                }
            }
        }

        /// <summary>
        /// Get the current data column foreign key reference attribute data for the proerty.
        /// </summary>
        /// <param name="property">The current property information</param>
        /// <returns>The data column foreign key reference attribute data.</returns>
        private Nequeo.Data.Custom.DataColumnForeignKeyAttribute GetForeginKeyReferenceData(PropertyInfo property)
        {
            // Get the current foreign key attribute for the property
            Nequeo.Data.Custom.DataColumnForeignKeyAttribute data = GetForeignKeyAttribute(property);
            return data;
        }

        /// <summary>
        /// Get the translated foreign key reference data.
        /// </summary>
        /// <param name="property">The current property information</param>
        /// <param name="foreignKeyValue">The foreign key value for the referenced data entity.</param>
        /// <param name="data">The data column foreign key reference attribute data.</param>
        /// <returns>The translated data entity object.</returns>
        private object SetForeginKeyReferenceData(PropertyInfo property, object foreignKeyValue, Nequeo.Data.Custom.DataColumnForeignKeyAttribute data)
        {
            // Get the get method of the property.
            MethodInfo method = property.GetGetMethod();

            string tableName = DataTypeConversion.GetSqlConversionDataType(ConnectionDataType, data.Name.Replace(".", "].["));
            string columnName = DataTypeConversion.GetSqlConversionDataType(ConnectionDataType, data.ReferenceColumnName);

            // Execute the queryable provider and return the constructed
            // sql statement and return the data.
            string statement = base.DataTypeConversion.GetSqlStringValue(LinqTypes.GetDataType(data.ColumnType, ConnectionDataType), foreignKeyValue);
            DataTable table = SelectDataTable("SELECT * FROM " + tableName + " WHERE " + columnName + " = " + statement);

            // Get the anonymous type translator from datarow
            // to the foreign key reference property return type.
            Nequeo.Data.Control.AnonymousTypeFunction typeFunction = new Nequeo.Data.Control.AnonymousTypeFunction();
            return ((table.Rows.Count > 0) ? typeFunction.TypeTranslator(table.Rows[0], method.ReturnType) : null);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The delete base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public class DeleteDataGenericBase<TDataEntity> : SchemaDataGenericBase<TDataEntity>,
        IDeleteDataGenericBase<TDataEntity>
        where TDataEntity : class, new()
    {
        #region Delete Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public DeleteDataGenericBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider)
        {
        }
        #endregion

        #region Private Fields
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

            return DeleteItemKey<TDataEntity>(keyValue, keyName);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey<TypeDataEntity>(object keyValue, string keyName)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Build the sql insert statement.
            string sql =
                "DELETE FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE ([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ")";

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
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
                    "{" + GetTableName<TypeDataEntity>() + "}");

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

            return DeleteItemKey<TDataEntity>(keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey<TypeDataEntity>(object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            // Build the sql insert statement.
            string sql =
                "DELETE FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE (([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ") AND (" +
                    "[" + rowVersionName + "] = " + base.DataTypeConversion.GetSqlStringValue(rowVersionData.GetType(), rowVersionData) + "))";

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
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
                    "{" + GetTableName<TypeDataEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItem(TDataEntity dataEntity, bool useRowVersion)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return DeleteDataEntities<TDataEntity>(new TDataEntity[] { dataEntity }, useRowVersion);
        }

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItem<TypeDataEntity>(TypeDataEntity dataEntity, bool useRowVersion)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return DeleteDataEntities<TypeDataEntity>(new TypeDataEntity[] { dataEntity }, useRowVersion);
        }

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItem(TDataEntity dataEntity)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return DeleteDataEntities<TDataEntity>(new TDataEntity[] { dataEntity }, false);
        }

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItem<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return DeleteDataEntities<TypeDataEntity>(new TypeDataEntity[] { dataEntity }, false);
        }
        #endregion

        #region Public Virtual Delete Predicate Methods
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate(Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity> expression = new Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity>();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Build the sql insert statement.
            string sql =
                "DELETE FROM [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + whereQueryText;

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));
                    
            // If nothing was deleted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemPredicate", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(whereQueryText) + " , " + predicate + "}" +
                    "{" + GetTableName<TDataEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

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

            return DeleteItemPredicate<TDataEntity>(predicate);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate<TypeDataEntity>(string predicate, params object[] values)
            where TypeDataEntity : class, new()
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
                "DELETE FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "WHERE " + predicateInternal;

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
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
                    "{" + GetTableName<TypeDataEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Delete Collection Methods
        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteDataEntities(TDataEntity[] dataEntities)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            return DeleteDataEntities<TDataEntity>(dataEntities, false);
        }

        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            return DeleteDataEntities<TypeDataEntity>(dataEntities, false);
        }

        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteDataEntities(TDataEntity[] dataEntities, bool useRowVersion)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            return DeleteDataEntities<TDataEntity>(dataEntities, useRowVersion);
        }

        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities, bool useRowVersion)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            // Get all primary keys.
            List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeDataEntity>();
            List<PropertyInfo> rowVersions = null;

            // Should row version data be
            // used to update data.
            if (useRowVersion)
            {
                // Get all row version properties.
                rowVersions = GetAllRowVersions<TypeDataEntity>();

                // No row version found.
                if (rowVersions.Count < 1)
                {
                    ErrorProvider(new Exception("No row version found."), "DeleteDataEntities",
                        "Can not delete the data because no row version has been found.");
                    return false;
                }
            }

            // No primary keys found.
            if (primaryKeys.Count < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "DeleteDataEntities",
                    "Can not delete the data because no primary key has been found.");
                return false;
            }

            int i = 0;
            string[] sql = new string[dataEntities.Count()];
            string sqlQuery = string.Empty;

            // Get the collection of all properties
            // in the current type.
            List<PropertyInfo> dataProperties = GetProperties(typeof(TypeDataEntity));

            // For each item in the collection
            // update the data item.
            foreach (TypeDataEntity dataEntity in dataEntities)
            {
                int j = 0;
                int k = 0;
                string[] keys = new string[primaryKeys.Count];
                string[] rows = null;

                // Should row version data be
                // used to update data.
                if (useRowVersion)
                    rows = new string[rowVersions.Count];

                // For each field within the entity.
                foreach (PropertyInfo primaryKey in primaryKeys)
                {
                    // Find in the property collection the current property that matches
                    // the current column. Use the Predicate delegate object to
                    // initiate a search for the specified match.
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = dataProperties.First(p => p.Name.ToLower() == primaryKey.Name.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (GetDbColumnName<TypeDataEntity>(primaryKey).ToLower().TrimStart('_') ==
                            GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_'))
                        {
                            // Get the current value
                            // within the current property.
                            object value = propertyInfo.GetValue(dataEntity, null);

                            // Get the name of the column
                            string name = GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_');
                            keys[j++] = "[" + name + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value);
                        }
                    }
                }

                if (rowVersions != null)
                {
                    // For each field within the entity.
                    foreach (PropertyInfo rowVersion in rowVersions)
                    {
                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = dataProperties.First(p => p.Name.ToLower() == rowVersion.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            if (GetDbColumnName<TypeDataEntity>(rowVersion).ToLower().TrimStart('_') ==
                                        GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_'))
                            {
                                // Get the current value
                                // within the current property.
                                object value = propertyInfo.GetValue(dataEntity, null);

                                // Get the name of the column
                                string name = GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_');
                                rows[k++] = "[" + name + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value);
                            }
                        }
                    }
                }

                string keyItems = string.Join(" AND ", keys);
                string rowItems = string.Empty;

                // Should row version data be
                // used to update data.
                if (useRowVersion)
                {
                    rowItems = " AND " + string.Join(" AND ", rows);
                }

                // Create the current row query item.
                string queryText = "(" + keyItems + rowItems + ")";

                // Should row version data be
                // used to update data.
                if (useRowVersion)
                {
                    // Build the sql delete statement.
                    sql[i++] =
                        "DELETE FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                        "WHERE " + queryText;
                }
                else
                {
                    // Build the sql delete statement.
                    sql[i++] =
                        "DELETE FROM [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                        "WHERE " + queryText;
                }
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
                foreach(string sqlStatement in sql)
                    ret = ExecuteCommand(ref sqlCommand, sqlStatement, CommandType.Text,
                        base.DefaultConnection(base.ConfigurationDatabaseConnection));
            }

            // If nothing was deleted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteDataEntities", "Deletes the items from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sqlQuery + "}" +
                    "{" + ConvertNullTypeToString(dataEntities) + "}" +
                    "{" + GetTableName<TypeDataEntity>() + "}");

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
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public virtual DbCommand DeleteQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery(ref dataTable, queryText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), getSchemaTable, values);
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
            return new Async.AsyncDeleteItemPredicate<TDataEntity>(
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
            return Async.AsyncDeleteItemPredicate<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteItemPredicate<TypeDataEntity>(AsyncCallback callback,
            object state, string predicate, params object[] values)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncDeleteItemPredicate<TDataEntity, TypeDataEntity>(
                this, callback, state, predicate, values);
        }

        /// <summary>
        /// End deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was deleted else false.</returns>
        public Boolean EndDeleteItemPredicate<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncDeleteItemPredicate<TDataEntity, TypeDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin deletes the data entities from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteDataEntities(AsyncCallback callback,
            object state, TDataEntity[] dataEntities, bool useRowVersion)
        {
            // Return an AsyncResult.
            return new Async.AsyncDeleteDataEntities<TDataEntity>(
                this, callback, state, dataEntities, useRowVersion);
        }

        /// <summary>
        /// End deletes the data entities from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        public Boolean EndDeleteDataEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncDeleteDataEntities<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteDataEntities<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity[] dataEntities, bool useRowVersion)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncDeleteDataEntities<TDataEntity, TypeDataEntity>(
                this, callback, state, dataEntities, useRowVersion);
        }

        /// <summary>
        /// End deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        public Boolean EndDeleteDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncDeleteDataEntities<TDataEntity, TypeDataEntity>.End(ar);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The insert base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public class InsertDataGenericBase<TDataEntity> : SchemaDataGenericBase<TDataEntity>,
        IInsertDataGenericBase<TDataEntity>
        where TDataEntity : class, new()
    {
        #region Insert Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public InsertDataGenericBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider)
        {
        }
        #endregion

        #region Public Virtual Insert Collection Methods
        /// <summary>
        /// Inserts the specified data entities.
        /// </summary>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertDataEntities(TDataEntity[] dataEntities)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            return InsertDataEntities<TDataEntity>(dataEntities);
        }

        /// <summary>
        /// Inserts the specified data entities.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            int i = 0;
            string[] sql = new string[dataEntities.Count()];
            string sqlQuery = string.Empty;

            // Get all the column data entity properties.
            List<PropertyInfo> properties = GetAllColumnData<TypeDataEntity>();

            // For each data entity insert the entity.
            foreach (TypeDataEntity entity in dataEntities)
            {
                string columns = string.Empty;
                string values = string.Empty;
                List<string> changedProperties = GetChangedPropertyNames<TypeDataEntity>(entity);

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in properties)
                {
                    // If the property is not database
                    // auto generated then insert.
                    if (!IsAutoGenerated(propertyItem))
                    {
                        //if (changedProperties.Contains(propertyItem.Name))
                        //{
                            // Get the current value from
                            // linq entity property.
                            object value = propertyItem.GetValue(entity, null);

                            // If the property value is not null.
                            // then build the sql column and value.
                            if (value != null)
                            {
                                columns += "[" + GetDbColumnName<TypeDataEntity>(propertyItem) + "],";
                                values += base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                            }
                        //}
                    }
                }

                // Build the sql insert statement.
                sql[i++] =
                    "INSERT INTO [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
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
                    "InsertDataEntities", "Inserts the items to the database. " +
                    "{" + sqlQuery + "}" +
                    "{" + ConvertNullTypeToString(dataEntities) + "}");
            }

            // Return true if nor errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Insert Item Methods
        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertItem(TDataEntity dataEntity)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return InsertItem<TDataEntity>(dataEntity);
        }

        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertItem<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            string columns = string.Empty;
            string values = string.Empty;

            // Get all the column data entity properties.
            List<PropertyInfo> properties = GetAllColumnData<TypeDataEntity>();
            List<string> changedProperties = GetChangedPropertyNames<TypeDataEntity>(dataEntity);

            // For each property found add the
            // property information.
            foreach (PropertyInfo propertyItem in properties)
            {
                // If the property is not database
                // auto generated then insert.
                if (!IsAutoGenerated(propertyItem))
                {
                    //if (changedProperties.Contains(propertyItem.Name))
                    //{
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(dataEntity, null);

                        // If the property value is not null.
                        // then build the sql column and value.
                        if (value != null)
                        {
                            columns += "[" + GetDbColumnName<TypeDataEntity>(propertyItem) + "],";
                            values += base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    //}
                }
            }

            // Build the sql insert statement.
            string sql =
                "INSERT INTO [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "(" + columns.TrimEnd(',') + ")" + " " +
                "VALUES (" + values.TrimEnd(',') + ")";

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was inserted, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Insertion has failed. No data found."),
                    "InsertItem", "Inserts the items to the database. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullTypeToString(dataEntity) + "}");
            }

            // Return true if no errors else false.
            return (ret > 0);
        }
        #endregion

        #region Public Virtual Insert Identity Methods
        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<object> InsertDataEntity(TDataEntity dataEntity)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return InsertDataEntity<TDataEntity>(dataEntity);
        }

        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<object> InsertDataEntity<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            // Add the auto insert identity sql.
            switch (this.ConnectionDataType)
            {
                case ConnectionContext.ConnectionDataType.MySqlDataType:
                    return InsertDataEntity<TypeDataEntity>(dataEntity, "; SELECT LAST_INSERT_ID()");

                case ConnectionContext.ConnectionDataType.SqlDataType:
                    return InsertDataEntity<TypeDataEntity>(dataEntity, "; SELECT SCOPE_IDENTITY()");

                case ConnectionContext.ConnectionDataType.AccessDataType:
                    return InsertDataEntity<TypeDataEntity>(dataEntity, "; SELECT @@IDENTITY");

                case ConnectionContext.ConnectionDataType.SqliteDataType:
                    return InsertDataEntity<TypeDataEntity>(dataEntity, "; SELECT last_insert_rowid()");

                default:
                    return InsertDataEntity<TypeDataEntity>(dataEntity, "");
            }
        }

        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<object> InsertDataEntity(TDataEntity dataEntity, string identitySqlQuery)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return InsertDataEntity<TDataEntity>(dataEntity, identitySqlQuery);
        }

        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual List<object> InsertDataEntity<TypeDataEntity>(TypeDataEntity dataEntity, string identitySqlQuery)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            string columns = string.Empty;
            string values = string.Empty;

            // Get all the column data entity properties.
            List<PropertyInfo> properties = GetAllColumnData<TypeDataEntity>();
            List<string> changedProperties = GetChangedPropertyNames<TypeDataEntity>(dataEntity);
            List<object> identityList = new List<object>();

            // For each property found add the
            // property information.
            foreach (PropertyInfo propertyItem in properties)
            {
                // If the property is not database
                // auto generated then insert.
                if (!IsAutoGenerated(propertyItem))
                {
                    //if (changedProperties.Contains(propertyItem.Name))
                    //{
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(dataEntity, null);

                        // If the property value is not null.
                        // then build the sql column and value.
                        if (value != null)
                        {
                            columns += "[" + GetDbColumnName<TypeDataEntity>(propertyItem) + "],";
                            values += base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    //}
                }
            }

            // Build the sql insert statement.
            string sql =
                "INSERT INTO [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "(" + columns.TrimEnd(',') + ")" + " " +
                "VALUES (" + values.TrimEnd(',') + ")";

            // If the identity query is not null.
            if(!String.IsNullOrEmpty(identitySqlQuery))
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
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public virtual DbCommand InsertQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery(ref dataTable, queryText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), getSchemaTable, values);
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
        public virtual Int32 InsertCommandItem(ref DbCommand sqlCommand, string commandText,
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
        /// Begin inserts the specified data entity.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataEntity(AsyncCallback callback,
            object state, TDataEntity dataEntity, string identitySqlQuery)
        {
            // Return an AsyncResult.
            return new Async.AsyncInsertDataEntity<TDataEntity>(
                this, callback, state, dataEntity, identitySqlQuery);
        }

        /// <summary>
        /// End inserts the specified data entity.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        public List<Object> EndInsertDataEntity(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncInsertDataEntity<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataEntity<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity dataEntity, string identitySqlQuery)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncInsertDataEntity<TDataEntity, TypeDataEntity>(
                this, callback, state, dataEntity, identitySqlQuery);
        }

        /// <summary>
        /// End inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        public List<Object> EndInsertDataEntity<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncInsertDataEntity<TDataEntity, TypeDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin inserts the specified data entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataEntities(AsyncCallback callback,
            object state, TDataEntity[] dataEntities)
        {
            // Return an AsyncResult.
            return new Async.AsyncInsertDataEntities<TDataEntity>(
                this, callback, state, dataEntities);
        }

        /// <summary>
        /// End inserts the specified data entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        public Boolean EndInsertDataEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncInsertDataEntities<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin inserts the specified data entities.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataEntities<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncInsertDataEntities<TDataEntity, TypeDataEntity>(
                this, callback, state, dataEntities);
        }

        /// <summary>
        /// End inserts the specified data entities.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        public Boolean EndInsertDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncInsertDataEntities<TDataEntity, TypeDataEntity>.End(ar);
        }
        #endregion

        #region Private Insert Methods
        /// <summary>
        /// Creates a new data table that controls the identity
        /// value with inerting new values.
        /// </summary>
        /// <returns>The new data table with identity.</returns>
        private DataTable IdentityDataTable()
        {
            // Create a new data table
            // with identity column.
            DataTable table = new DataTable("IDENTITYDataTable");
            table.Columns.Add(new DataColumn("IDENTITY", typeof(System.Object)));
            return table;
        }

        /// <summary>
        /// Gets all the properties that have been changed.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity containing the data.</param>
        /// <returns>The list of proprty names that have been changed.</returns>
        private List<string> GetChangedPropertyNames<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new()
        {
            List<string> propertyNames = null;
            FieldInfo propertyNameField = typeof(TypeDataEntity).
                GetField("_changedPropertyNames", BindingFlags.NonPublic | BindingFlags.Instance);

            propertyNames = (List<string>)propertyNameField.GetValue(dataEntity);
            return propertyNames ?? new List<string>();
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The update base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public class UpdateDataGenericBase<TDataEntity> : SchemaDataGenericBase<TDataEntity>,
        IUpdateDataGenericBase<TDataEntity>
        where TDataEntity : class, new()
    {
        #region Update Data Generic Base

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public UpdateDataGenericBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider)
        {
        }
        #endregion

        #region Private Fields
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
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(TDataEntity dataEntity, object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return UpdateItemKey<TDataEntity>(dataEntity, keyValue, keyName);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeDataEntity>(TypeDataEntity dataEntity, object keyValue, string keyName)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            string results = string.Empty;

            // Get all the column data entity properties.
            List<PropertyInfo> properties = GetAllColumnData<TypeDataEntity>();
            List<string> changedProperties = GetChangedPropertyNames<TypeDataEntity>(dataEntity);

            // For each property found add the
            // property information.
            foreach (PropertyInfo propertyItem in properties)
            {
                // If the property is not database
                // auto generated then insert.
                if (!IsAutoGenerated(propertyItem))
                {
                    if (changedProperties.Contains(propertyItem.Name))
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(dataEntity, null);

                        // If the property value is not null.
                        // then build the sql column and value.
                        if (value != null)
                        {
                            results += "[" + GetDbColumnName<TypeDataEntity>(propertyItem) + "] = " +
                                base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "UPDATE [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "SET " + results.TrimEnd(',') + " " +
                "WHERE ([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ")";

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
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
                    "{" + GetTableName<TypeDataEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(TDataEntity dataEntity,
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            return UpdateItemKey<TDataEntity>(dataEntity, keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeDataEntity>(TypeDataEntity dataEntity,
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            string results = string.Empty;

            // Get all the column data entity properties.
            List<PropertyInfo> properties = GetAllColumnData<TypeDataEntity>();
            List<string> changedProperties = GetChangedPropertyNames<TypeDataEntity>(dataEntity);

            // For each property found add the
            // property information.
            foreach (PropertyInfo propertyItem in properties)
            {
                // If the property is not database
                // auto generated then insert.
                if (!IsAutoGenerated(propertyItem))
                {
                    if (changedProperties.Contains(propertyItem.Name))
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(dataEntity, null);

                        // If the property value is not null.
                        // then build the sql column and value.
                        if (value != null)
                        {
                            results += "[" + GetDbColumnName<TypeDataEntity>(propertyItem) + "] = " +
                                base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "UPDATE [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "SET " + results.TrimEnd(',') + " " +
                "WHERE (([" + keyName + "] = " + base.DataTypeConversion.GetSqlStringValue(keyValue.GetType(), keyValue) + ") AND (" +
                    "[" + rowVersionName + "] = " + base.DataTypeConversion.GetSqlStringValue(rowVersionData.GetType(), rowVersionData) + "))";

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
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
                    "{" + GetTableName<TypeDataEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItem(TDataEntity dataEntity, bool useRowVersion)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return UpdateDataEntities<TDataEntity>(new TDataEntity[] { dataEntity }, useRowVersion);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItem<TypeDataEntity>(TypeDataEntity dataEntity, bool useRowVersion)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return UpdateDataEntities<TypeDataEntity>(new TypeDataEntity[] { dataEntity }, useRowVersion);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItem(TDataEntity dataEntity)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return UpdateDataEntities<TDataEntity>(new TDataEntity[] { dataEntity }, false);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItem<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntity == null) throw new ArgumentNullException("dataEntity");

            return UpdateDataEntities<TypeDataEntity>(new TypeDataEntity[] { dataEntity }, false);
        }
        #endregion

        #region Public Virtual Update Predicate Methods
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(
            TDataEntity dataEntity, Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the current expression.
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataEntity>> expressionTree = predicate;

            // Create a new express tree class and
            // return the sql query expression from the
            // expression tree.
            Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity> expression = new Nequeo.Data.Linq.ExpressionTreeDataGeneric<TDataEntity>();
            string whereQueryText = expression.CreateSqlWhereQuery(expressionTree, base.DataTypeConversion);

            // Get all the column data entity properties.
            List<PropertyInfo> properties = GetAllColumnData<TDataEntity>();
            List<string> changedProperties = GetChangedPropertyNames<TDataEntity>(dataEntity);
            string results = string.Empty;

            // For each property found add the
            // property information.
            foreach (PropertyInfo propertyItem in properties)
            {
                // If the property is not database
                // auto generated then insert.
                if (!IsAutoGenerated(propertyItem))
                {
                    if (changedProperties.Contains(propertyItem.Name))
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(dataEntity, null);

                        // If the property value is not null.
                        // then build the sql column and value.
                        if (value != null)
                        {
                            results += "[" + GetDbColumnName<TDataEntity>(propertyItem) + "] = " +
                                base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "UPDATE [" + GetTableName<TDataEntity>().Replace(".", "].[") + "]" + " " +
                "SET " + results.TrimEnd(',') + " " +
                "WHERE " + whereQueryText;

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
                base.DefaultConnection(base.ConfigurationDatabaseConnection));

            // If nothing was updated, no
            // rows have been affected.
            if (ret <= 0)
            {
                ErrorProvider(new Exception("Update has failed. No data found."),
                    "UpdateItemPredicate", "Updates the items to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sql + "}" +
                    "{" + ConvertNullToString(whereQueryText) + " , " + predicate + "}" +
                    "{" + GetTableName<TDataEntity>() + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
            }

            // Return true if no errors else false.
            return (ret > 0);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(
            TDataEntity dataEntity, string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return UpdateItemPredicate<TDataEntity>(dataEntity, predicate, values);
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeDataEntity>(
            TypeDataEntity dataEntity, string predicate, params object[] values)
            where TypeDataEntity : class, new()
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

            // Get all the column data entity properties.
            List<PropertyInfo> properties = GetAllColumnData<TypeDataEntity>();
            List<string> changedProperties = GetChangedPropertyNames<TypeDataEntity>(dataEntity);

            // For each property found add the
            // property information.
            foreach (PropertyInfo propertyItem in properties)
            {
                // If the property is not database
                // auto generated then insert.
                if (!IsAutoGenerated(propertyItem))
                {
                    if (changedProperties.Contains(propertyItem.Name))
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(dataEntity, null);

                        // If the property value is not null.
                        // then build the sql column and value.
                        if (value != null)
                        {
                            results += "[" + GetDbColumnName<TypeDataEntity>(propertyItem) + "] = " +
                                base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                        }
                    }
                }
            }

            // Build the sql insert statement.
            string sql =
                "UPDATE [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                "SET " + results.TrimEnd(',') + " " +
                "WHERE " + predicateInternal;

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;
            ret = ExecuteCommand(ref sqlCommand, sql, CommandType.Text,
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
                    "{" + GetTableName<TypeDataEntity>() + "}");

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
        /// <param name="dataEntities">The data entities to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateDataEntities(TDataEntity[] dataEntities)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            return UpdateDataEntities<TDataEntity>(dataEntities, false);
        }

        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            return UpdateDataEntities<TypeDataEntity>(dataEntities, false);
        }

        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateDataEntities(TDataEntity[] dataEntities, bool useRowVersion)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            return UpdateDataEntities<TDataEntity>(dataEntities, useRowVersion);
        }

        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities, bool useRowVersion)
            where TypeDataEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataEntities == null) throw new ArgumentNullException("dataEntities");

            // Get all primary keys.
            List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeDataEntity>();
            List<PropertyInfo> properties = GetAllColumnData<TypeDataEntity>();
            List<PropertyInfo> rowVersions = null;

            // Should row version data be
            // used to update data.
            if (useRowVersion)
            {
                // Get all row version properties.
                rowVersions = GetAllRowVersions<TypeDataEntity>();

                // No row version found.
                if (rowVersions.Count < 1)
                {
                    ErrorProvider(new Exception("No row version found."), "UpdateDataEntities",
                        "Can not update the data because no row version has been found.");
                    return false;
                }
            }

            // No primary keys found.
            if (primaryKeys.Count < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "UpdateDataEntities",
                    "Can not update the data because no primary key has been found.");
                return false;
            }

            int i = 0;
            string[] sql = new string[dataEntities.Count()];
            string sqlQuery = string.Empty;

            // Get the collection of all properties
            // in the current type.
            List<PropertyInfo> dataProperties = GetProperties(typeof(TypeDataEntity));

            // For each item in the collection
            // update the data item.
            foreach (TypeDataEntity dataEntity in dataEntities)
            {
                int j = 0;
                int k = 0;
                string[] keys = new string[primaryKeys.Count];
                string[] rows = null;
                List<string> changedProperties = GetChangedPropertyNames<TypeDataEntity>(dataEntity);

                // Should row version data be
                // used to update data.
                if (useRowVersion)
                    rows = new string[rowVersions.Count];

                // For each field within the entity.
                foreach (PropertyInfo primaryKey in primaryKeys)
                {
                    // Find in the property collection the current property that matches
                    // the current column. Use the Predicate delegate object to
                    // initiate a search for the specified match.
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = dataProperties.First(p => p.Name.ToLower() == primaryKey.Name.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (GetDbColumnName<TypeDataEntity>(primaryKey).ToLower().TrimStart('_') ==
                                    GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_'))
                        {
                            // Get the current value
                            // within the current property.
                            object value = propertyInfo.GetValue(dataEntity, null);

                            // Get the name of the column
                            string name = GetDbColumnName<TypeDataEntity>(propertyInfo).TrimStart('_');
                            keys[j++] = "[" + name + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value);
                        }
                    }
                }

                if (rowVersions != null)
                {
                    // For each field within the entity.
                    foreach (PropertyInfo rowVersion in rowVersions)
                    {
                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = dataProperties.First(p => p.Name.ToLower() == rowVersion.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            if (GetDbColumnName<TypeDataEntity>(rowVersion).ToLower().TrimStart('_') ==
                                        GetDbColumnName<TypeDataEntity>(propertyInfo).ToLower().TrimStart('_'))
                            {
                                // Get the current value
                                // within the current property.
                                object value = propertyInfo.GetValue(dataEntity, null);

                                // Get the name of the column
                                string name = GetDbColumnName<TypeDataEntity>(propertyInfo).TrimStart('_');
                                rows[k++] = "[" + name + "] = " + base.DataTypeConversion.GetSqlStringValue(value.GetType(), value);
                            }
                        }
                    }
                }

                string keyItems = string.Join(" AND ", keys);
                string rowItems = string.Empty;

                // Should row version data be
                // used to update data.
                if (useRowVersion)
                {
                    rowItems = " AND " + string.Join(" AND ", rows);
                }

                // Create the current row query item.
                string queryText = "(" + keyItems + rowItems + ")";

                string results = string.Empty;

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in properties)
                {
                    // If the property is not database
                    // auto generated then insert.
                    if (!IsAutoGenerated(propertyItem))
                    {
                        if (changedProperties.Contains(propertyItem.Name))
                        {
                            // Get the current value from
                            // linq entity property.
                            object value = propertyItem.GetValue(dataEntity, null);

                            // If the property value is not null.
                            // then build the sql column and value.
                            if (value != null)
                            {
                                results += "[" + GetDbColumnName<TypeDataEntity>(propertyItem) + "] = " +
                                    base.DataTypeConversion.GetSqlStringValue(value.GetType(), value) + ",";
                            }
                        }
                    }
                }

                // Should row version data be
                // used to update data.
                if (useRowVersion)
                {
                    // Build the sql update statement.
                    sql[i++] =
                        "UPDATE [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                        "SET " + results.TrimEnd(',') + " " +
                        "WHERE " + queryText;
                }
                else
                {
                    // Build the sql update statement.
                    sql[i++] =
                        "UPDATE [" + GetTableName<TypeDataEntity>().Replace(".", "].[") + "]" + " " +
                        "SET " + results.TrimEnd(',') + " " +
                        "WHERE " + queryText;
                }
            }

            // Join the sql statements
            // into one query.
            sqlQuery = string.Join(" ", sql);

            // Excecute the command.
            int ret = 0;
            DbCommand sqlCommand = null;

            // If using bulk insert operation then
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
                    "UpdateDataEntities", "Updates the items to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + sqlQuery + "}" +
                    "{" + ConvertNullTypeToString(dataEntities) + "}" +
                    "{" + GetTableName<TypeDataEntity>() + "}");

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
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        public virtual DbCommand UpdateQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(queryText)) throw new ArgumentNullException("queryText");

            return ExecuteQuery(ref dataTable, queryText, commandType,
                base.DefaultConnection(base.ConfigurationDatabaseConnection), getSchemaTable, values);
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
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateItemPredicate(AsyncCallback callback,
            object state, TDataEntity dataEntity, string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new Async.AsyncUpdateItemPredicate<TDataEntity>(
                this, callback, state, dataEntity, predicate, values);
        }

        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        public Boolean EndUpdateItemPredicate(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncUpdateItemPredicate<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateItemPredicate<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity dataEntity, string predicate, params object[] values)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncUpdateItemPredicate<TDataEntity, TypeDataEntity>(
                this, callback, state, dataEntity, predicate, values);
        }

        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        public Boolean EndUpdateItemPredicate<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncUpdateItemPredicate<TDataEntity, TypeDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin updates the data entities from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateDataEntities(AsyncCallback callback,
            object state, TDataEntity[] dataEntities, bool useRowVersion)
        {
            // Return an AsyncResult.
            return new Async.AsyncUpdateDataEntities<TDataEntity>(
                this, callback, state, dataEntities, useRowVersion);
        }

        /// <summary>
        /// End updates the data entities from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        public Boolean EndUpdateDataEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncUpdateDataEntities<TDataEntity>.End(ar);
        }

        /// <summary>
        /// Begin updates the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateDataEntities<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity[] dataEntities, bool useRowVersion)
            where TypeDataEntity : class, new()
        {
            // Return an AsyncResult.
            return new Async.AsyncUpdateDataEntities<TDataEntity, TypeDataEntity>(
                this, callback, state, dataEntities, useRowVersion);
        }

        /// <summary>
        /// End updates the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        public Boolean EndUpdateDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return Async.AsyncUpdateDataEntities<TDataEntity, TypeDataEntity>.End(ar);
        }
        #endregion

        #region Private Update Methods
        /// <summary>
        /// Gets all the properties that have been changed.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity containing the data.</param>
        /// <returns>The list of proprty names that have been changed.</returns>
        private List<string> GetChangedPropertyNames<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new()
        {
            List<string> propertyNames = null;
            FieldInfo propertyNameField = typeof(TypeDataEntity).
                GetField("_changedPropertyNames", BindingFlags.NonPublic | BindingFlags.Instance);

            propertyNames = (List<string>)propertyNameField.GetValue(dataEntity);
            return propertyNames ?? new List<string>();
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The common base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public class CommonDataGenericBase<TDataEntity> : SchemaDataGenericBase<TDataEntity>,
        ICommonDataGenericBase<TDataEntity>
        where TDataEntity : class, new()
    {
        #region CommonDataGenericBase Base Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key database connection section.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public CommonDataGenericBase(string configurationDatabaseConnection, ConnectionContext.ConnectionType connectionType,
            ConnectionContext.ConnectionDataType connectionDataType, IDataAccess dataAccessProvider)
            : base(configurationDatabaseConnection, connectionType, connectionDataType, dataAccessProvider)
        {
        }
        #endregion

        #region Public Properites
        /// <summary>
        /// Gets, the data entity type descriptor.
        /// </summary>
        public Nequeo.Data.Control.DataEntityTypeDescriptor<TDataEntity> DataEntityTypeDescriptor
        {
            get { return new Nequeo.Data.Control.DataEntityTypeDescriptor<TDataEntity>(); }
        }
        #endregion

        #region Public Virtual Convertion Methods
        /// <summary>
        /// Convert all the IQueryable data into a array of
        /// anonymous types.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of anonymous type data.</returns>
        public virtual Nequeo.Data.Control.AnonymousType[]
            ConvertToAnonymousType(IQueryable query)
        {
            Nequeo.Data.Control.AnonymousTypeFunction functions =
                new Nequeo.Data.Control.AnonymousTypeFunction();

            // Return the anonymous type array.
            return functions.GetAnonymousTypeData(query);
        }

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
            return string.Format("Data Entity = {0}",
                typeof(TDataEntity).GetType().FullName);
        }
        #endregion

        #endregion
    }
}
