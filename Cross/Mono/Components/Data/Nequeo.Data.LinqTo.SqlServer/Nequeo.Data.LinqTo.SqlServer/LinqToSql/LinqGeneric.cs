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

using Nequeo.Data.DataType;
using Nequeo.Data.Control;
using Nequeo.Linq.Extension;
using Nequeo.Extension;

namespace Nequeo.Data.LinqToSql
{
    /// <summary>
    /// The abstract base data access class for all data access objects.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public abstract class SchemaLinqToSqlGenericBase<TDataContext, TLinqEntity> : Nequeo.Handler.LogHandler, IDisposable
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region SchemaLinqGenericBase Abstract Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaItemName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        protected SchemaLinqToSqlGenericBase(string schemaItemName, ConnectionContext.ConnectionType connectionType,
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

            _dataTypeConversion = new DataTypeConversion(connectionDataType);
            this._schemaItemName = schemaItemName;
            this.Initialise(string.Empty);
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo.Data.Client";
        private const string eventNamespace = "Nequeo.Data.Client.LinqToSql";
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
        /// Gets, the LinqToSql data collection configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.LinqToSqlDataCollection LinqToSqlDataCollectionReader
        {
            get { return Nequeo.Data.Configuration.LinqToSqlDataConfigurationManager.LinqToSqlDataCollection(); }
        }

        /// <summary>
        /// Gets, the LinqToSql data default configuration section reader class.
        /// </summary>
        public Nequeo.Data.Configuration.LinqToSqlDataElementDefault LinqToSqlDataElementReader
        {
            get { return Nequeo.Data.Configuration.LinqToSqlDataConfigurationManager.LinqToSqlDataElement(); }
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
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

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
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

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
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                        // If the property is a primary key.
                        if (att.IsPrimaryKey)
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
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                        // If the property is a primary key.
                        if (att.IsPrimaryKey)
                            return member;
                    }
                }
            }

            // Return null none found.
            return null;
        }

        /// <summary>
        /// Get the row version property for the current type.
        /// </summary>
        /// <returns>The property that is the row version.</returns>
        /// <remarks>This method should only be used if one row version exists.</remarks>
        public PropertyInfo GetRowVersion()
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
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                        // If the property attribute
                        // is a row versioner.
                        if (att.IsVersion)
                            return member;
                    }
                }
            }

            // Return null none found.
            return null;
        }

        /// <summary>
        /// Get the row version property for the current type.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>The property that is the row version.</returns>
        /// <remarks>This method should only be used if one row version exists.</remarks>
        public PropertyInfo GetRowVersion<T>()
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
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                        // If the property attribute
                        // is a row versioner.
                        if (att.IsVersion)
                            return member;
                    }
                }
            }

            // Return null none found.
            return null;
        }

        /// <summary>
        /// Get all row versioning properties for the current type.
        /// </summary>
        /// <returns>The collection of properties.</returns>
        public List<PropertyInfo> GetAllRowVersions()
        {
            // Create a new property collection.
            List<PropertyInfo> rowVersion = new List<PropertyInfo>();

            // For each property member in the current type.
            foreach (PropertyInfo member in (typeof(TLinqEntity)).GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                        // If the property attribute
                        // is a row versioner.
                        if (att.IsVersion)
                            rowVersion.Add(member);
                    }
                }
            }

            // Return the collection of
            // row versioning properties.
            return rowVersion;
        }

        /// <summary>
        /// Get all row versioning properties for the current type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties</returns>
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
                    if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                    {
                        // Cast the current attribute.
                        System.Data.Linq.Mapping.ColumnAttribute att =
                            (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                        // If the property attribute
                        // is a row versioner.
                        if (att.IsVersion)
                            rowVersion.Add(member);
                    }
                }
            }

            // Return the collection of
            // row versioning properties.
            return rowVersion;
        }

        /// <summary>
        /// Is the current property value a row versioning property.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a row versioner.</returns>
        public bool IsRowVersion(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Linq.Mapping.ColumnAttribute att =
                        (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                    // If the property attribute
                    // is a row versioner.
                    if (att.IsVersion)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value auto generated by the database.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is database auto generated.</returns>
        public bool IsAutoGenerated(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Linq.Mapping.ColumnAttribute att =
                        (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                    // If the property attribute
                    // is database auto generated.
                    if (att.IsDbGenerated)
                        return true;
                }
            }

            // Return false.
            return false;
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
                if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Linq.Mapping.ColumnAttribute att =
                        (System.Data.Linq.Mapping.ColumnAttribute)attribute;

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
                if (attribute is System.Data.Linq.Mapping.AssociationAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Linq.Mapping.AssociationAttribute att =
                        (System.Data.Linq.Mapping.AssociationAttribute)attribute;

                    // If the property attribute
                    // is a foreign key.
                    if (att.IsForeignKey)
                        return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Is the current property value an aAssociation key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a foreign key.</returns>
        public bool IsAssociationKey(PropertyInfo property)
        {
            // For each attribute on each property
            // in the type.
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                // If the attribute is the
                // linq column attribute.
                if (attribute is System.Data.Linq.Mapping.AssociationAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Linq.Mapping.AssociationAttribute att =
                        (System.Data.Linq.Mapping.AssociationAttribute)attribute;

                    // If the property attribute
                    // is a association key.
                    return true;
                }
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Gets the current linq entity column name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The column name.</returns>
        public string GetDbColumnName<T>(PropertyInfo property)
        {
            // For each attribute for the member
            foreach (object attribute in property.GetCustomAttributes(true))
            {
                if (attribute is System.Data.Linq.Mapping.ColumnAttribute)
                {
                    // Cast the current attribute.
                    System.Data.Linq.Mapping.ColumnAttribute att =
                        (System.Data.Linq.Mapping.ColumnAttribute)attribute;

                    // Return the column name.
                    return att.Storage.TrimStart('_');
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
    public class LinqToSqlDataGenericBase<TDataContext, TLinqEntity> :
        SchemaLinqToSqlGenericBase<TDataContext, TLinqEntity>,
        ILinqToSqlDataGenericBase<TDataContext, TLinqEntity>
            where TDataContext : System.Data.Linq.DataContext, new()
            where TLinqEntity : class, new()
    {
        #region LinqDataGenericBase Base Class

        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="schemaName">The current data schema item object name.</param>
        /// <param name="connectionType">The connection type to use.</param>
        /// <param name="connectionDataType">The connection data type to use.</param>
        /// <param name="dataAccessProvider">The data access provider.</param>
        public LinqToSqlDataGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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

        #region Public Query Execution Methods
        /// <summary>
        /// Execute an SQL query directly to the database.
        /// </summary>
        /// <param name="sqlQuery">The sql command to execute to the database.</param>
        /// <param name="values">The parameter values for the command, can be null.</param>
        /// <returns>The enumerable collection type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IEnumerable<TLinqEntity> ExecuteQuery(string sqlQuery, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (sqlQuery == null) throw new ArgumentNullException("sqlQuery");

            // Execute the query.
            IEnumerable<TLinqEntity> query =
                DataContext.ExecuteQuery<TLinqEntity>(sqlQuery, values);

            // Return the enumerable result
            // of the specified type.
            return query;
        }

        /// <summary>
        /// Execute an SQL query directly to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="sqlQuery">The sql command to execute to the database.</param>
        /// <param name="values">The parameter values for the command, can be null.</param>
        /// <returns>The enumerable collection type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IEnumerable<TypeLinqEntity> ExecuteQuery<TypeLinqEntity>(string sqlQuery, params object[] values)
            where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (sqlQuery == null) throw new ArgumentNullException("sqlQuery");

            // Execute the query.
            IEnumerable<TypeLinqEntity> query =
                DataContext.ExecuteQuery<TypeLinqEntity>(sqlQuery, values);
            
            // Return the enumerable result
            // of the specified type.
            return query;
        }

        /// <summary>
        /// Execute an SQL command directly to the database.
        /// </summary>
        /// <param name="sqlCommand">The sql command to execute to the database.</param>
        /// <param name="values">The parameter values for the command, can be null.</param>
        /// <returns>A value indicating the result of the command.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Int32 ExecuteCommand(string sqlCommand, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (sqlCommand == null) throw new ArgumentNullException("sqlCommand");

            // Execute the command and return
            // the result from the command.
            return DataContext.ExecuteCommand(sqlCommand, values);
        }
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
    public class SelectLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        SchemaLinqToSqlGenericBase<TDataContext, TLinqEntity>, 
        ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity>
            where TDataContext : System.Data.Linq.DataContext, new()
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
        public SelectLinqToSqlGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public SelectLinqToSqlGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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
        
        private DataTable _dataTable = null;
        private TLinqEntity _linqEntity = null;
        private IQueryable<TLinqEntity> _query = null;
        private List<TLinqEntity> _linqCollection = null;

        private Nequeo.Data.LinqToSql.SelectDataType _selectType = Nequeo.Data.LinqToSql.SelectDataType.None;
        private Nequeo.Data.LinqToSql.ChangeDataType _changeType = Nequeo.Data.LinqToSql.ChangeDataType.None;
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
        /// Gets sets the selection type.
        /// </summary>
        public Nequeo.Data.LinqToSql.SelectDataType SelectType
        {
            get { return _selectType; }
            set { _selectType = value; }
        }

        /// <summary>
        /// Gets sets the change type.
        /// </summary>
        public Nequeo.Data.LinqToSql.ChangeDataType ChangeType
        {
            get { return _changeType; }
            set { _changeType = value; }
        }

        /// <summary>
        /// Gets the IQueryable generic provider
        /// for the current linq entity.
        /// </summary>
        public IQueryable<TLinqEntity> IQueryable
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
        /// Gets the current linq entity type.
        /// </summary>
        public TLinqEntity LinqEntity
        {
            get { return _linqEntity; }
        }

        /// <summary>
        /// Gets the linq entity type collection.
        /// </summary>
        public List<TLinqEntity> LinqCollection
        {
            get { return _linqCollection; }
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
        public ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity>
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

        #region Public Virtual Select Methods
        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity[] SelectLinqEntities()
        {
            List<TLinqEntity> data = null;
            bool ret = SelectCollectionKey(ref data, null, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));

            // Return the details of the operation.
            return ret ? data.ToTSourceArray() : null;
        }

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity[] SelectLinqEntities(object keyValue)
        {
            List<TLinqEntity> data = null;
            bool ret = SelectCollectionKey(ref data, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));

            // Return the details of the operation.
            return ret ? data.ToTSourceArray() : null;
        }

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>()
            where TypeLinqEntity : class, new()
        {
            List<TypeLinqEntity> data = null;
            bool ret = SelectCollectionKey<TypeLinqEntity>(
                ref data, null, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TypeLinqEntity>()));

            // Return the details of the operation.
            return ret ? data.ToTSourceArray() : null;
        }

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>(object keyValue)
            where TypeLinqEntity : class, new()
        {
            List<TypeLinqEntity> data = null;
            bool ret = SelectCollectionKey<TypeLinqEntity>(
                ref data, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TypeLinqEntity>()));

            // Return the details of the operation.
            return ret ? data.ToTSourceArray() : null;
        }

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity[] SelectLinqEntities(string predicate, params object[] values)
        {
            List<TLinqEntity> data = null;
            bool ret = SelectCollectionPredicate(ref data, predicate, values);

            // Return the details of the operation.
            return ret ? data.ToTSourceArray() : null;
        }

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        ///<param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>(string predicate, params object[] values)
            where TypeLinqEntity : class, new()
        {
            List<TypeLinqEntity> data = null;
            bool ret = SelectCollectionPredicate<TypeLinqEntity>(
                ref data, predicate, values);

            // Return the details of the operation.
            return ret ? data.ToTSourceArray() : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TLinqEntity> SelectIQueryableItems()
        {
            IQueryable<TLinqEntity> data = null;
            bool ret = SelectCollectionKey(ref data, null, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TLinqEntity> SelectIQueryableItems(object keyValue)
        {
            IQueryable<TLinqEntity> data = null;
            bool ret = SelectCollectionKey(ref data, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TLinqEntity> SelectIQueryableItems(string predicate, params object[] values)
        {
            IQueryable<TLinqEntity> data = null;
            bool ret = SelectCollectionPredicate(ref data, predicate, values);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTable()
        {
            DataTable data = null;
            bool ret = SelectCollectionKey(ref data, null, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTable(object keyValue)
        {
            DataTable data = null;
            bool ret = SelectCollectionKey(ref data, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTable(string predicate, params object[] values)
        {
            DataTable data = null;
            bool ret = SelectCollectionPredicate(ref data, predicate, values);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity SelectLinqEntity(object keyValue)
        {
            TLinqEntity data = null;
            bool ret = SelectItemKey(ref data, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity SelectLinqEntity<TypeLinqEntity>(object keyValue)
            where TypeLinqEntity : class, new()
        {
            TypeLinqEntity data = null;
            bool ret = SelectItemKey<TypeLinqEntity>(
                ref data, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TypeLinqEntity>()));

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity SelectLinqEntity(string predicate, params object[] values)
        {
            TLinqEntity data = null;
            bool ret = SelectItemPredicate(ref data, predicate, values);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity SelectLinqEntity<TypeLinqEntity>(string predicate, params object[] values)
            where TypeLinqEntity : class, new()
        {
            TypeLinqEntity data = null;
            bool ret = SelectItemPredicate<TypeLinqEntity>(
                ref data, predicate, values);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity[] SelectLinqEntities(Expression<Func<TLinqEntity, bool>> predicate)
        {
            return SelectLinqEntities<TLinqEntity>(predicate);
        }

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>(Expression<Func<TypeLinqEntity, bool>> predicate)
            where TypeLinqEntity : class, new()
        {
            List<TypeLinqEntity> data = null;
            bool ret = SelectCollectionPredicate<TypeLinqEntity>(
                ref data, predicate);

            // Return the details of the operation.
            return ret ? data.ToTSourceArray() : null;
        }

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual IQueryable<TLinqEntity> SelectIQueryableItems(Expression<Func<TLinqEntity, bool>> predicate)
        {
            IQueryable<TLinqEntity> data = null;
            bool ret = SelectCollectionPredicate(ref data, predicate);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual DataTable SelectDataTable(Expression<Func<TLinqEntity, bool>> predicate)
        {
            DataTable data = null;
            bool ret = SelectCollectionPredicate(ref data, predicate);

            // Return the details of the operation.
            return ret ? data : null;
        }

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity SelectLinqEntity(Expression<Func<TLinqEntity, bool>> predicate)
        {
            return SelectLinqEntity<TLinqEntity>(predicate);
        }

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity SelectLinqEntity<TypeLinqEntity>(Expression<Func<TypeLinqEntity, bool>> predicate)
            where TypeLinqEntity : class, new()
        {
            TypeLinqEntity data = null;
            bool ret = SelectItemPredicate<TypeLinqEntity>(
                ref data, predicate);

            // Return the details of the operation.
            return ret ? data : null;
        }
        #endregion

        #region Public Virtual Select Data Type Methods
        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <remarks>Uses the current primary key to get single item data.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectDataTypeKey(object keyValue)
        {
            bool ret = false;

            // The current selected data type.
            switch (_selectType)
            {
                case Nequeo.Data.LinqToSql.SelectDataType.LinqEntity:
                    // Get the linq enitiy.
                    ret = SelectItemKey(ref _linqEntity, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.LinqCollection:
                    // Get the linq enitiy collection.
                    ret = SelectCollectionKey(ref _linqCollection, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.DataTable:
                    // Get the data table collection.
                    ret = SelectCollectionKey(ref _dataTable, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.IQueryable:
                    // Get the IQueryable data collection.
                    ret = SelectCollectionKey(ref _query, keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));
                    break;

                default:
                    // No selection was made.
                    throw new Exception("A valid selection data type must be specified.");
            }

            return ret;
        }

        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectDataTypeKey(object keyValue, string keyName)
        {
            bool ret = false;

            // The current selected data type.
            switch (_selectType)
            {
                case Nequeo.Data.LinqToSql.SelectDataType.LinqEntity:
                    // Get the linq enitiy.
                    ret = SelectItemKey(ref _linqEntity, keyValue, keyName);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.LinqCollection:
                    // Get the linq enitiy collection.
                    ret = SelectCollectionKey(ref _linqCollection, keyValue, keyName);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.DataTable:
                    // Get the data table collection.
                    ret = SelectCollectionKey(ref _dataTable, keyValue, keyName);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.IQueryable:
                    // Get the IQueryable data collection.
                    ret = SelectCollectionKey(ref _query, keyValue, keyName);
                    break;

                default:
                    // No selection was made.
                    throw new Exception("A valid selection data type must be specified.");
            }

            return ret;
        }

        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectDataTypePredicate(string predicate, params object[] values)
        {
            bool ret = false;

            // The current selected data type.
            switch (_selectType)
            {
                case Nequeo.Data.LinqToSql.SelectDataType.LinqEntity:
                    // Get the linq enitiy.
                    ret = SelectItemPredicate(ref _linqEntity, predicate, values);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.LinqCollection:
                    // Get the linq enitiy collection.
                    ret = SelectCollectionPredicate(ref _linqCollection, predicate, values);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.DataTable:
                    // Get the data table collection.
                    ret = SelectCollectionPredicate(ref _dataTable, predicate, values);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.IQueryable:
                    // Get the IQueryable data collection.
                    ret = SelectCollectionPredicate(ref _query, predicate, values);
                    break;

                default:
                    // No selection was made.
                    throw new Exception("A valid selection data type must be specified.");
            }

            return ret;
        }

        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectDataTypePredicate(Expression<Func<TLinqEntity, bool>> predicate)
        {
            bool ret = false;

            // The current selected data type.
            switch (_selectType)
            {
                case Nequeo.Data.LinqToSql.SelectDataType.LinqEntity:
                    // Get the linq enitiy.
                    ret = SelectItemPredicate(ref _linqEntity, predicate);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.LinqCollection:
                    // Get the linq enitiy collection.
                    ret = SelectCollectionPredicate(ref _linqCollection, predicate);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.DataTable:
                    // Get the data table collection.
                    ret = SelectCollectionPredicate(ref _dataTable, predicate);
                    break;

                case Nequeo.Data.LinqToSql.SelectDataType.IQueryable:
                    // Get the IQueryable data collection.
                    ret = SelectCollectionPredicate(ref _query, predicate);
                    break;

                default:
                    // No selection was made.
                    throw new Exception("A valid selection data type must be specified.");
            }

            return ret;
        }
        #endregion

        #region Public Virtual Select Item Methods
        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectItemKey(ref TLinqEntity linqEntity,
            object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return SelectItemKey<TLinqEntity>(ref linqEntity, keyValue, keyName);
        }

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectItemKey<TypeLinqEntity>(
            ref TypeLinqEntity linqEntity, object keyValue, string keyName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Create a new instance of the 
            // reference type.
            linqEntity = new TypeLinqEntity();

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                keyValue, keyName, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectItemKey", "No data found. " +
                        "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                        "{" + linqEntity.GetType().FullName + "}");
                    return false;
                }

                // Get the array of linq enities
                // and return only the first
                // linq entity.
                TypeLinqEntity[] linqEntityItems = query.ToTSourceArray();
                TypeLinqEntity linqEntityItem = linqEntityItems[0];
                linqEntity = linqEntityItem;

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectItemKey", "Gets the first linq entity. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + linqEntity.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectItemPredicate(ref TLinqEntity linqEntity,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return SelectItemPredicate<TLinqEntity>(ref linqEntity, predicate, values);
        }

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectItemPredicate<TypeLinqEntity>(
            ref TypeLinqEntity linqEntity, string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Create a new instance of the 
            // reference type.
            linqEntity = new TypeLinqEntity();

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                null, null, predicate, values);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectItemPredicate", "No data found. " +
                        "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                        "{" + linqEntity.GetType().FullName + "}");
                    return false;
                }

                // Get the array of linq enities
                // and return only the first
                // linq entity.
                TypeLinqEntity[] linqEntityItems = query.ToTSourceArray();
                TypeLinqEntity linqEntityItem = linqEntityItems[0];
                linqEntity = linqEntityItem;

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectItemPredicate", "Gets the first linq entity. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + linqEntity.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectItemPredicate(
            ref TLinqEntity linqEntity, Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SelectItemPredicate<TLinqEntity>(ref linqEntity, predicate);
        }

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectItemPredicate<TypeLinqEntity>(
            ref TypeLinqEntity linqEntity, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Create a new instance of the 
            // reference type.
            linqEntity = new TypeLinqEntity();

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(predicate,
                null, null, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectItemPredicate", "No data found. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + linqEntity.GetType().FullName + "}");
                    return false;
                }

                // Get the array of linq enities
                // and return only the first
                // linq entity.
                TypeLinqEntity[] linqEntityItems = query.ToTSourceArray();
                TypeLinqEntity linqEntityItem = linqEntityItems[0];
                linqEntity = linqEntityItem;

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectItemPredicate", "Gets the first linq entity. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + linqEntity.GetType().FullName + "}");
                return false;
            }
        }
        #endregion

        #region Public Virtual Select Collection Methods
        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionKey(ref List<TLinqEntity> linqCollection,
            object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return SelectCollectionKey<TLinqEntity>(ref linqCollection, keyValue, keyName);
        }

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionKey<TypeLinqEntity>(
            ref List<TypeLinqEntity> linqCollection, object keyValue, string keyName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Create a new instance of the 
            // reference type.
            linqCollection = new List<TypeLinqEntity>();

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                keyValue, keyName, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionKey", "No data found. " +
                        "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                        "{" + linqCollection.GetType().FullName + "}");
                    return false;
                }

                // Assign the collection type to
                // the list collection linq entities.
                linqCollection = query.ToListTSourceArray();

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollectionKey", "Gets a generic linq collection of linq entities. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + linqCollection.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate(
            ref List<TLinqEntity> linqCollection,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TLinqEntity>(ref linqCollection, predicate, values);
        }

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate<TypeLinqEntity>(
            ref List<TypeLinqEntity> linqCollection, string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Create a new instance of the 
            // reference type.
            linqCollection = new List<TypeLinqEntity>();

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                null, null, predicate, values);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionPredicate", "No data found. " +
                        "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                        "{" + linqCollection.GetType().FullName + "}");
                    return false;
                }

                // Assign the collection type to
                // the list collection linq entities.
                linqCollection = query.ToListTSourceArray();

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollectionPredicate", "Gets a generic linq collection of linq entities. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + linqCollection.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionKey(ref DataTable dataTable,
            object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return SelectCollectionKey<TLinqEntity>(ref dataTable, keyValue, keyName);
        }

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionKey<TypeLinqEntity>(
            ref DataTable dataTable, object keyValue, string keyName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Create a new instance of the 
            // reference type.
            dataTable = new DataTable(_schemaName);

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                keyValue, keyName, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionKey", "No data found. " +
                        "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                        "{" + dataTable.GetType().FullName + "}");
                    return false;
                }

                // Get the collection of linq entity properties.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in linqProperties)
                {
                    // If the current property is not
                    // a foreign key value then create
                    // the new column in the table.
                    if (!IsForeignKey(propertyItem))
                    {
                        // Create a new column and assign
                        // each of the properties on the column.
                        DataColumn column = new DataColumn();
                        column.DataType = propertyItem.PropertyType;
                        column.ColumnName = GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_');
                        dataTable.Columns.Add(column);
                    }
                }
                
                int i = 0;

                // Get all the primary keys that are
                // associated with the current type.
                List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeLinqEntity>();
                DataColumn[] keys = new DataColumn[primaryKeys.Count];

                // For each property in the current
                // linq type.
                foreach (PropertyInfo propertyItem in primaryKeys)
                {
                    // Get the current column and add
                    // it to the primary key column collection.
                    DataColumn column = dataTable.Columns[GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_')];
                    keys[i++] = column;
                }

                // Assign all primary keys to the table.
                dataTable.PrimaryKey = keys;
                

                // For each linq entity item 
                // found add to the data collection.
                foreach (TypeLinqEntity linqEntity in query)
                {
                    // Create a new data row.
                    DataRow row = null;
                    row = dataTable.NewRow();

                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo propertyItem in linqProperties)
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(linqEntity, null);
                        
                        // If the current property is not
                        // a foreign key value then assign
                        // the current row column with the
                        // value from the current linq type.
                        if (!IsForeignKey(propertyItem))
                            row[GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_')] = value;
                    }

                    // Add the current row to the table.
                    dataTable.Rows.Add(row);
                }

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollectionKey", "Gets a collection of rows inserted into a data table. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + dataTable.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate(ref DataTable dataTable,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TLinqEntity>(ref dataTable, predicate, values);
        }

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate<TypeLinqEntity>(
            ref DataTable dataTable, string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Create a new instance of the 
            // reference type.
            dataTable = new DataTable(_schemaName);

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                null, null, predicate, values);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionPredicate", "No data found. " +
                        "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                        "{" + dataTable.GetType().FullName + "}");
                    return false;
                }

                // Get the collection of linq entity properties.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in linqProperties)
                {
                    // If the current property is not
                    // a foreign key value then create
                    // the new column in the table.
                    if (!IsForeignKey(propertyItem))
                    {
                        // Create a new column and assign
                        // each of the properties on the column.
                        DataColumn column = new DataColumn();
                        column.DataType = propertyItem.PropertyType;
                        column.ColumnName = GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_');
                        dataTable.Columns.Add(column);
                    }
                }

                int i = 0;

                // Get all the primary keys that are
                // associated with the current type.
                List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeLinqEntity>();
                DataColumn[] keys = new DataColumn[primaryKeys.Count];

                // For each property in the current
                // linq type.
                foreach (PropertyInfo propertyItem in primaryKeys)
                {
                    // Get the current column and add
                    // it to the primary key column collection.
                    DataColumn column = dataTable.Columns[GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_')];
                    keys[i++] = column;
                }

                // Assign all primary keys to the table.
                dataTable.PrimaryKey = keys;

                // For each linq entity item 
                // found add to the data collection.
                foreach (TypeLinqEntity linqEntity in query)
                {
                    // Create a new data row.
                    DataRow row = null;
                    row = dataTable.NewRow();

                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo propertyItem in linqProperties)
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(linqEntity, null);

                        // If the current property is not
                        // a foreign key value then assign
                        // the current row column with the
                        // value from the current linq type.
                        if (!IsForeignKey(propertyItem))
                            row[GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_')] = value;
                    }

                    // Add the current row to the table.
                    dataTable.Rows.Add(row);
                }

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollectionPredicate", "Gets a collection of rows inserted into a data table. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + dataTable.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionKey(ref IQueryable<TLinqEntity> queryResult,
            object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return SelectCollectionKey<TLinqEntity>(ref queryResult, keyValue, keyName);
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionKey<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult, object keyValue, string keyName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                keyValue, keyName, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionKey", "No data found. " +
                        "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
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
                    "SelectCollectionKey", "Gets a collection of IQueryable provider linq entities. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate(
            ref IQueryable<TLinqEntity> queryResult,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TLinqEntity>(ref queryResult, predicate, values);
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult,
            string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(null,
                null, null, predicate, values);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionPredicate", "No data found. " +
                        "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
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
                    "SelectCollectionPredicate", "Gets a collection of IQueryable provider linq entities. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate(
            ref List<TLinqEntity> linqCollection, Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TLinqEntity>(ref linqCollection, predicate);
        }

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate<TypeLinqEntity>(
            ref List<TypeLinqEntity> linqCollection, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Create a new instance of the 
            // reference type.
            linqCollection = new List<TypeLinqEntity>();

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(predicate,
                null, null, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionPredicate", "No data found. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + linqCollection.GetType().FullName + "}");
                    return false;
                }

                // Assign the collection type to
                // the list collection linq entities.
                linqCollection = query.ToListTSourceArray();

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollectionPredicate", "Gets a generic linq collection of linq entities. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + linqCollection.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate(
            ref DataTable dataTable, Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TLinqEntity>(ref dataTable, predicate);
        }

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate<TypeLinqEntity>(
            ref DataTable dataTable, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Create a new instance of the 
            // reference type.
            dataTable = new DataTable(_schemaName);

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(predicate,
                null, null, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionPredicate", "No data found. " +
                        "{" + ConvertNullToString(predicate) + "}" +
                        "{" + dataTable.GetType().FullName + "}");
                    return false;
                }

                // Get the collection of linq entity properties.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each property found add the
                // property information.
                foreach (PropertyInfo propertyItem in linqProperties)
                {
                    // If the current property is not
                    // a foreign key value then create
                    // the new column in the table.
                    if (!IsForeignKey(propertyItem))
                    {
                        // Create a new column and assign
                        // each of the properties on the column.
                        DataColumn column = new DataColumn();
                        column.DataType = propertyItem.PropertyType;
                        column.ColumnName = GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_');
                        dataTable.Columns.Add(column);
                    }
                }

                int i = 0;

                // Get all the primary keys that are
                // associated with the current type.
                List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeLinqEntity>();
                DataColumn[] keys = new DataColumn[primaryKeys.Count];

                // For each property in the current
                // linq type.
                foreach (PropertyInfo propertyItem in primaryKeys)
                {
                    // Get the current column and add
                    // it to the primary key column collection.
                    DataColumn column = dataTable.Columns[GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_')];
                    keys[i++] = column;
                }

                // Assign all primary keys to the table.
                dataTable.PrimaryKey = keys;

                // For each linq entity item 
                // found add to the data collection.
                foreach (TypeLinqEntity linqEntity in query)
                {
                    // Create a new data row.
                    DataRow row = null;
                    row = dataTable.NewRow();

                    // For each property found add the
                    // property information.
                    foreach (PropertyInfo propertyItem in linqProperties)
                    {
                        // Get the current value from
                        // linq entity property.
                        object value = propertyItem.GetValue(linqEntity, null);

                        // If the current property is not
                        // a foreign key value then assign
                        // the current row column with the
                        // value from the current linq type.
                        if (!IsForeignKey(propertyItem))
                            row[GetDbColumnName<TypeLinqEntity>(propertyItem).TrimStart('_')] = value;
                    }

                    // Add the current row to the table.
                    dataTable.Rows.Add(row);
                }

                return true;
            }
            else
            {
                ErrorProvider(new Exception("Selection has failed"),
                    "SelectCollectionPredicate", "Gets a collection of rows inserted into a data table. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + dataTable.GetType().FullName + "}");
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate(
            ref IQueryable<TLinqEntity> queryResult,
            Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SelectCollectionPredicate<TLinqEntity>(ref queryResult, predicate);
        }

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SelectCollectionPredicate<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult,
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the data from the database.
            IQueryable<TypeLinqEntity> query = SelectItemEx<TypeLinqEntity>(predicate,
                null, null, null, null);

            // If data was returned.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    InformationProvider("SelectCollectionPredicate", "No data found. " +
                        "{" + ConvertNullToString(predicate) + "}" +
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
                    "SelectCollectionPredicate", "Gets a collection of IQueryable provider linq entities. " +
                    "{" + ConvertNullToString(predicate) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");
                return false;
            }
        }
        #endregion

        #region Private Select Item(s) Methods
        /// <summary>
        /// Select the data from the database for the current
        /// linq table type.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="expressionPredicate">The expression predicate string to search.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq table data.</returns>
        private IQueryable<TypeLinqEntity> SelectItemEx<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> expressionPredicate,
            object keyValue, string keyName, string predicate, params object[] values)
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
                    query = DataContext.GetTable<TypeLinqEntity>().Where(expressionPredicate);
                }
                else if (keyValue == null && String.IsNullOrEmpty(keyName))
                {
                    // Execute the Link query on 
                    // the current linq table.
                    // This query uses the predicate
                    // and values.
                    query = DataContext.GetTable<TypeLinqEntity>().Where(predicate, values);
                }
                else if (keyValue == null)
                {
                    // Execute the Link query on 
                    // the current linq table.
                    // this query returns all
                    // current linq table data.
                    query = from u in DataContext.GetTable<TypeLinqEntity>()
                            select u;
                }
                else
                {
                    // Execute the Link query on 
                    // the current linq table.
                    query = DataContext.GetTable<TypeLinqEntity>().Where(keyName + " == @0", keyValue);
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
        /// Begin get all linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectLinqToSqlEntities(AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity>(this, callback, state);
        }

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectLinqToSqlEntities(AsyncCallback callback,
            object state, object keyValue)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity>(this, callback, state, keyValue);
        }

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectLinqToSqlEntities(AsyncCallback callback,
            object state, string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity>(this, callback, state, predicate, values);
        }

        /// <summary>
        /// End get all linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        public TLinqEntity[] EndSelectLinqToSqlEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback, object state)
            where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>(this, callback, state);
        }

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, object keyValue)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>(this, callback, state, keyValue);
        }

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>(this, callback, state, predicate, values);
        }

        /// <summary>
        /// End get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        public TypeLinqEntity[] EndSelectLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin get the data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectDataTable<TDataContext, TLinqEntity>(this, callback, state);
        }

        /// <summary>
        /// Begin get the data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable(AsyncCallback callback,
            object state, object keyValue)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectDataTable<TDataContext, TLinqEntity>(this, callback, state, keyValue);
        }

        /// <summary>
        /// Begin get the data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectDataTable(AsyncCallback callback,
            object state, string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectDataTable<TDataContext, TLinqEntity>(this, callback, state, predicate, values);
        }

        /// <summary>
        /// End get the data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        public DataTable EndSelectDataTable(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncSelectDataTable<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectIQueryableItems<TDataContext, TLinqEntity>(this, callback, state);
        }

        /// <summary>
        /// Begin get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback,
            object state, object keyValue)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectIQueryableItems<TDataContext, TLinqEntity>(this, callback, state, keyValue);
        }

        /// <summary>
        /// Begin get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback,
            object state, string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncSelectIQueryableItems<TDataContext, TLinqEntity>(this, callback, state, predicate, values);
        }

        /// <summary>
        /// End get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        public IQueryable<TLinqEntity> EndSelectIQueryableItems(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncSelectIQueryableItems<TDataContext, TLinqEntity>.End(ar);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The delete base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public class DeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        SchemaLinqToSqlGenericBase<TDataContext, TLinqEntity>,
        IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity>
            where TDataContext : System.Data.Linq.DataContext, new()
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
        public DeleteLinqToSqlGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public DeleteLinqToSqlGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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

        #region Public Virtual Delete Collection Methods
        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionKey(object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            return DeleteCollectionKey<TLinqEntity>(keyValue, keyName);
        }

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionKey<TypeLinqEntity>(
            object keyValue, string keyName)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Get all the items.
            IQueryable<TypeLinqEntity> query =
                DataContext.GetTable<TypeLinqEntity>().Where(keyName + " == @0", keyValue);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    ErrorProvider(new Exception("Deletion has failed. No data found."),
                        "DeleteCollectionKey", "Deletes the collection of linq entities found. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                    return false;
                }

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
                        DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(entity);
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteCollectionKey", "Deletes the collection of linq entities found. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionKey(object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            return DeleteCollectionKey<TLinqEntity>(keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionKey<TypeLinqEntity>(
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            // Get all the items.
            IQueryable<TypeLinqEntity> query =
                DataContext.GetTable<TypeLinqEntity>().Where(keyName + " == @0 AND " + rowVersionName + " == @1", keyValue, rowVersionData);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    ErrorProvider(new Exception("Deletion has failed. No data found."),
                        "DeleteCollectionKey", "Deletes the collection of linq entities found. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                    return false;
                }

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
                        DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(entity);
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteCollectionKey", "Deletes the collection of linq entities found. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #region Public Virtual Delete Item Methods
        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
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

            return DeleteItemKey<TLinqEntity>(keyValue, keyName);
        }

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey<TypeLinqEntity>(
            object keyValue, string keyName)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.GetTable<TypeLinqEntity>().First(keyName + " == @0", keyValue);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Delete the currrent item  in the entity
                // form the database.
                DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(query);

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemKey", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
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

            return DeleteItemKey<TLinqEntity>(keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey<TypeLinqEntity>(
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.GetTable<TypeLinqEntity>().First(keyName + " == @0 AND " + rowVersionName + " == @1", keyValue, rowVersionData);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Delete the currrent item  in the entity
                // form the database.
                DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(query);

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemKey", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #region Public Virtual Delete Predicate Methods
        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionPredicate(
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return DeleteCollectionPredicate<TLinqEntity>(predicate, values);
        }

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionPredicate<TypeLinqEntity>(
            string predicate, params object[] values)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Get all the items.
            IQueryable<TypeLinqEntity> query =
                DataContext.GetTable<TypeLinqEntity>().Where(predicate, values);

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
                        "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                        "{" + ConvertNullTypeToString(query) + "}");

                    // Set the concurrency error.
                    ConcurrencyError = true;
                    return false;
                }

                // New linq enity type;
                TypeLinqEntity entity = null;

                // For each item found.
                foreach (var result in query)
                {
                    // Assign the current linq entity.
                    entity = result;

                    if (entity != null)
                        // Delete the currrent item in the entity
                        // form the database.
                        DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(entity);
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteCollectionPredicate", "Deletes the collection of linq entities found. " +
                    "A concurrency error may have occurred. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate(
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            return DeleteItemPredicate<TLinqEntity>(predicate, values);
        }

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate<TypeLinqEntity>(
            string predicate, params object[] values)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.GetTable<TypeLinqEntity>().First(predicate, values);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Delete the currrent item  in the entity
                // form the database.
                DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(query);

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteItemPredicate", "Deletes the linq entity item from the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
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
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollectionPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get all the items.
            IQueryable<TypeLinqEntity> query =
                DataContext.GetTable<TypeLinqEntity>().Where(predicate);

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

                // New linq enity type;
                TypeLinqEntity entity = null;

                // For each item found.
                foreach (var result in query)
                {
                    // Assign the current linq entity.
                    entity = result;

                    if (entity != null)
                        // Delete the currrent item in the entity
                        // form the database.
                        DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(entity);
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
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
        }

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
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
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.GetTable<TypeLinqEntity>().First(predicate);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Delete the currrent item  in the entity
                // form the database.
                DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(query);

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
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

        #region Public Virtual Delete Item With Key Methods
        /// <summary>
        /// Deletes the linq entity from the database.
        /// </summary>
        /// <param name="linqEntityItem">The linq entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItem(TLinqEntity linqEntityItem, bool useRowVersion)
        {
            return DeleteCollection<TLinqEntity>(new TLinqEntity[] { linqEntityItem }, useRowVersion);
        }

        /// <summary>
        /// Deletes the linq entity from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItem<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, bool useRowVersion)
                where TypeLinqEntity : class, new()
        {
            return DeleteCollection<TypeLinqEntity>
                (new TypeLinqEntity[] { linqEntityItem }, useRowVersion);
        }
        #endregion

        #region Public Virtual Delete Collection With Key Methods
        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollection(TLinqEntity[] linqEntityItems, bool useRowVersion)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            return DeleteCollection<TLinqEntity>(linqEntityItems, useRowVersion);
        }

        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteCollection<TypeLinqEntity>(
            TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            // Get all primary keys.
            List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeLinqEntity>();
            List<PropertyInfo> rowVersions = null;

            // Should row version data be
            // used to update data.
            if (useRowVersion)
            {
                // Get all row version properties.
                rowVersions = GetAllRowVersions<TypeLinqEntity>();

                // No row version found.
                if (rowVersions.Count < 1)
                {
                    ErrorProvider(new Exception("No row version found."), "DeleteCollection",
                        "Can not delete the data because no row version has been found.");
                    return false;
                }
            }

            // No primary keys found.
            if (primaryKeys.Count < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "DeleteCollection",
                    "Can not delete the data because no primary key has been found.");
                return false;
            }

            // Return indicator.
            int count = -1;
            bool ret = false;
            List<object> values = new List<object>();
            List<string> queryItems = new List<string>();

            // Get the collection of all properties
            // in the current type.
            List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));
            
            // For each item in the collection
            // update the data item.
            foreach (TypeLinqEntity linqEntity in linqEntityItems)
            {
                int i = 0;
                int j = 0;
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
                        propertyInfo = linqProperties.First(p => p.Name.ToLower() == primaryKey.Name.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (GetDbColumnName<TypeLinqEntity>(primaryKey).ToLower().TrimStart('_') ==
                                    GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_'))
                        {
                            // Get the current value
                            // within the current property.
                            object value = propertyInfo.GetValue(linqEntity, null);
                            values.Add(value);

                            string name = GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_');
                            keys[i++] = name + " == @" + (count++).ToString();
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
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == rowVersion.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            if (GetDbColumnName<TypeLinqEntity>(rowVersion).ToLower().TrimStart('_') ==
                                        GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_'))
                            {
                                // Get the current value
                                // within the current property.
                                object value = propertyInfo.GetValue(linqEntity, null);
                                values.Add(value);

                                string name = GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_');
                                rows[j++] = name + " == @" + (count++).ToString();
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
                string item = "(" + keyItems + rowItems + ")";
                queryItems.Add(item);
            }

            // Create the query text and execute
            // the collection method.
            string queryText = string.Join(" OR ", queryItems.ToArray());
            ret = DeleteCollectionEx<TypeLinqEntity>(queryText, values.ToArray());

            // Return the collection of linq entities
            // including the new values.
            return ret;
        }
        #endregion

        #region Public Virtual Delete Methods
        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey(object keyValue)
        {
            return DeleteItemKey<TLinqEntity>(keyValue, GetDbColumnName<TLinqEntity>(GetPrimaryKey<TLinqEntity>()));
        }

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The value to search on</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool DeleteItemKey<TypeLinqEntity>(object keyValue)
            where TypeLinqEntity : class
        {
            return DeleteItemKey<TypeLinqEntity>(keyValue, GetDbColumnName<TypeLinqEntity>(GetPrimaryKey<TypeLinqEntity>()));
        }
        #endregion

        #region Public Asynchronous Delete Methods
        /// <summary>
        /// Begin delete linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteLinqToSqlEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems, bool useRowVersion)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncDeleteLinqToSqlEntities<TDataContext, TLinqEntity>
                (this, callback, state, linqEntityItems, useRowVersion);
        }

        /// <summary>
        /// End delete linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        public Boolean EndDeleteLinqToSqlEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncDeleteLinqToSqlEntities<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin delete linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncDeleteLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, linqEntityItems, useRowVersion);
        }

        /// <summary>
        /// End delete linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        public Boolean EndDeleteLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncDeleteLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin delete collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteCollection(AsyncCallback callback,
            object state, string predicate, params object[] values)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncDeleteCollection<TDataContext, TLinqEntity>
                (this, callback, state, predicate, values);
        }

        /// <summary>
        /// End delete collection.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        public Boolean EndDeleteCollection(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncDeleteCollection<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin delete collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginDeleteCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncDeleteCollection<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, predicate, values);
        }

        /// <summary>
        /// End delete collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        public Boolean EndDeleteCollection<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncDeleteCollection<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }
        #endregion

        #region Private Delete Collection Methods
        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryText">The query string to search on.</param>
        /// <param name="values">The query parameter values.</param>
        /// <returns>True if the deletion succeeded else false.</returns>
        private bool DeleteCollectionEx<TypeLinqEntity>(string queryText, params object[] values)
            where TypeLinqEntity : class, new()
        {
            // Get all the items.
            IQueryable<TypeLinqEntity> query =
                DataContext.GetTable<TypeLinqEntity>().Where(queryText, values);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Is there data.
                if (query.Count() < 1)
                {
                    ErrorProvider(new Exception("Deletion has failed. No data found."),
                        "DeleteCollectionKey", "Deletes the collection of linq entities found. " +
                        "A concurrency error may have occurred. " +
                        "{" + ConvertNullToString(queryText));

                    // Set the concurrency error.
                    ConcurrencyError = true;
                    return false;
                }

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
                        DataContext.GetTable<TypeLinqEntity>().DeleteOnSubmit(entity);
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Deletion has failed. No data found."),
                    "DeleteCollectionKey", "Deletes the collection of linq entities found. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(queryText));

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// The insert base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public class InsertLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        SchemaLinqToSqlGenericBase<TDataContext, TLinqEntity>,
        IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Linq.DataContext, new()
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
        public InsertLinqToSqlGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public InsertLinqToSqlGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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
                    ErrorProvider(new Exception("Insertion of linq entity failed."),
                        "InsertCollection", exp.Message + " " + exp.Source + " " +
                        "{" + ConvertNullTypeToString(linqEntityItems) + "}");

                    return null;
                }
            }

            // Insert the linq enity values.
            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);

            // Return the collection of linq entities
            // including the new values.
            return linqEntityArray;
        }

        /// <summary>
        /// Insert the collection of data rows.
        /// </summary>
        /// <param name="dataTable">The data table with data rows.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity[] InsertCollection(DataTable dataTable)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            return InsertCollection<TLinqEntity>(dataTable);
        }

        /// <summary>
        /// Insert the collection of data rows.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The data table with data rows.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity[] InsertCollection<TypeLinqEntity>(
            DataTable dataTable)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            // Create a new collection instance
            // of the linq entity.
            TypeLinqEntity[] linqEntityArray = new TypeLinqEntity[dataTable.Rows.Count];

            // Initial count;
            int i = 0;

            // For each item in the collection
            // insert the new data row.
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    linqEntityArray[i++] = InsertCollectionEx<TypeLinqEntity>(dataRow);
                }
                catch (Exception exp)
                {
                    // A System.Data.Linq.ChangeConflictException
                    // exception may hav occurred.
                    ErrorProvider(new Exception("Insertion of data table failed."),
                        "InsertCollection", exp.Message + " " + exp.Source + " " +
                        "{" + ConvertNullTypeToString(dataTable) + "}");

                    return null;
                }
            }

            // Insert the linq enity values.
            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);

            // Return the collection of linq entities
            // including the new values.
            return linqEntityArray;
        }
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
            DataContext.GetTable<TypeLinqEntity>().InsertOnSubmit(linqEntityItem);
            DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);

            // Assign the current linq entity with
            // the new primary key value.
            return linqEntityItem;
        }

        /// <summary>
        /// Insert the Column Enity to the database.
        /// </summary>
        /// <param name="dataRow">The data row containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TLinqEntity InsertItem(DataRow dataRow)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            return InsertItem<TLinqEntity>(dataRow);
        }

        /// <summary>
        /// Insert the Column Enity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual TypeLinqEntity InsertItem<TypeLinqEntity>(DataRow dataRow)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            // Create a new instance of the
            // linq entity type.
            TypeLinqEntity linqEntityItem = new TypeLinqEntity();

            // Get the collection of all properties
            // in the current type.
            List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

            // For each field within the entity.
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                // Get the current entity property value.
                object value = dataRow[column.ColumnName.ToLower()];

                if (value.GetType().ToString() != "System.DBNull")
                {
                    // If the value is a Byte[].
                    if (value.GetType().ToString() != "System.DBNull" && value is Byte[])
                    {
                        // Get the ling binary convertion
                        // for the current byte array.
                        // assign the current value
                        // with the new type.
                        System.Data.Linq.Binary binaryValue = new System.Data.Linq.Binary((Byte[])value);
                        value = binaryValue;
                    }

                    // Find in the property collection the current property that matches
                    // the current column. Use the Predicate delegate object to
                    // initiate a search for the specified match.
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = linqProperties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (!IsAutoGenerated(propertyInfo))
                        {
                            if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                            {
                                // If the current property within the property collection
                                // is the current propert name within the data collection.
                                if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                    column.ColumnName.ToLower().TrimStart('_'))
                                {
                                    // If the current property is not database
                                    // auto generated, no need to update auto
                                    // generated properties. Set the property
                                    // with the value.
                                    propertyInfo.SetValue(linqEntityItem, value, null);
                                }
                            }
                        }
                    }
                }
            }

            // Insert the linq enity values.
            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.GetTable<TypeLinqEntity>().InsertOnSubmit(linqEntityItem);
            DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);

            // Assign the current linq entity with
            // the new primary key value.
            return linqEntityItem;
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
        /// Inserts the data table rows.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertDataTable(DataTable dataTable)
        {
            TLinqEntity[] linqEntityItems = InsertCollection(dataTable);
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

        /// <summary>
        /// Inserts the data row.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool InsertDataRow(DataRow dataRow)
        {
            TLinqEntity linqEntityItem = InsertItem(dataRow);
            return (linqEntityItem != null);
        }
        #endregion

        #region Public Asynchronous Insert Methods

        /// <summary>
        /// Begin insert generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The linq entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertLinqToSqlEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntities)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncInsertLinqToSqlEntities<TDataContext, TLinqEntity>
                (this, callback, state, linqEntities);
        }

        /// <summary>
        /// End insert generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        public Boolean EndInsertLinqToSqlEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncInsertLinqToSqlEntities<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin insert data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertDataTable(AsyncCallback callback,
            object state, DataTable dataTable)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncInsertDataTable<TDataContext, TLinqEntity>
                (this, callback, state, dataTable);
        }

        /// <summary>
        /// End insert data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        public Boolean EndInsertDataTable(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncInsertDataTable<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin insert generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The linq entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertTypeLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntities)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncInsertLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, linqEntities);
        }

        /// <summary>
        /// End insert generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        public Boolean EndInsertTypeLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncInsertLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin insert collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The linq entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertCollection(AsyncCallback callback,
            object state, TLinqEntity[] linqEntities)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncInsertCollection<TDataContext, TLinqEntity>
                (this, callback, state, linqEntities);
        }

        /// <summary>
        /// Begin insert collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertCollection(AsyncCallback callback,
            object state, DataTable dataTable)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncInsertCollection<TDataContext, TLinqEntity>
                (this, callback, state, dataTable);
        }

        /// <summary>
        /// End insert collection.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>Collection of type data.</returns>
        public TLinqEntity[] EndInsertCollection(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncInsertCollection<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin insert collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The data table.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginInsertTypeCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntities)
            where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncInsertCollection<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, linqEntities);
        }

        /// <summary>
        /// End insert collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>Collection of type data.</returns>
        public TypeLinqEntity[] EndInsertTypeCollection<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncInsertCollection<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }
        #endregion

        #region Private Insert Item Methods
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
            DataContext.GetTable<TypeLinqEntity>().InsertOnSubmit(linqEntityItem);

            // Assign the current linq entity with
            // the new primary key value.
            return linqEntityItem;
        }

        /// <summary>
        /// Insert the Column Enity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private TypeLinqEntity InsertCollectionEx<TypeLinqEntity>(DataRow dataRow)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            // Create a new instance of the
            // linq entity type.
            TypeLinqEntity linqEntityItem = new TypeLinqEntity();

            // Get the collection of all properties
            // in the current type.
            List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

            // For each field within the entity.
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                // Get the current entity property value.
                object value = dataRow[column.ColumnName.ToLower()];

                if (value.GetType().ToString() != "System.DBNull")
                {
                    // If the value is a Byte[].
                    if (value.GetType().ToString() != "System.DBNull" && value is Byte[])
                    {
                        // Get the ling binary convertion
                        // for the current byte array.
                        // assign the current value
                        // with the new type.
                        System.Data.Linq.Binary binaryValue = new System.Data.Linq.Binary((Byte[])value);
                        value = binaryValue;
                    }

                    // Find in the property collection the current property that matches
                    // the current column. Use the Predicate delegate object to
                    // initiate a search for the specified match.
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = linqProperties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (!IsAutoGenerated(propertyInfo))
                        {
                            if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                            {
                                // If the current property within the property collection
                                // is the current propert name within the data collection.
                                if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                    column.ColumnName.ToLower().TrimStart('_'))
                                {
                                    // If the current property is not database
                                    // auto generated, no need to update auto
                                    // generated properties. Set the property
                                    // with the value.
                                    propertyInfo.SetValue(linqEntityItem, value, null);
                                }
                            }
                        }
                    }
                }
            }

            // Insert the linq enity values.
            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.GetTable<TypeLinqEntity>().InsertOnSubmit(linqEntityItem);

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
    public class UpdateLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        SchemaLinqToSqlGenericBase<TDataContext, TLinqEntity>,
        IUpdateLinqToSqlGenericBase<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Linq.DataContext, new()
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
        public UpdateLinqToSqlGenericBase(string schemaName, ConnectionContext.ConnectionType connectionType,
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
        public UpdateLinqToSqlGenericBase(string schemaName, string specificPath, ConnectionContext.ConnectionType connectionType,
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
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(TLinqEntity linqEntityItem,
            object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            return UpdateItemKey<TLinqEntity>(linqEntityItem, keyValue, keyName);
        }

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeLinqEntity>(TypeLinqEntity linqEntityItem, 
            object keyValue, string keyName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            // Get the first item found.
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(keyName + " == @0", keyValue);

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
                PropertyInfo[] linqEntityItemFields = linqEntityItem.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // For each field within the entity.
                foreach (PropertyInfo infoItem in linqEntityItemFields)
                {
                    // Get the current entity property value.
                    object value = infoItem.GetValue(linqEntityItem, null);

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

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current property name within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(infoItem).ToLower().TrimStart('_') ==
                                            GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                    "UpdateItem", "Updates the Linq Entity to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(TLinqEntity linqEntityItem,
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            return UpdateItemKey<TLinqEntity>(linqEntityItem, 
                keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem,
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.GetTable<TypeLinqEntity>().First(keyName + " == @0 AND " + rowVersionName + " == @1", keyValue, rowVersionData);

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
                PropertyInfo[] linqEntityItemFields = linqEntityItem.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // For each field within the entity.
                foreach (PropertyInfo infoItem in linqEntityItemFields)
                {
                    // Get the current entity property value.
                    object value = infoItem.GetValue(linqEntityItem, null);

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

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current property name within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(infoItem).ToLower().TrimStart('_') ==
                                            GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                    "UpdateItemKey", "Updates the Linq Entity to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(DataRow dataRow, object keyValue, string keyName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            return UpdateItemKey<TLinqEntity>(dataRow, keyValue, keyName);
        }

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeLinqEntity>(
            DataRow dataRow, object keyValue, string keyName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            // Get the first item found.
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(keyName + " == @0", keyValue);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each column within the data collection.
                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    // Get the current column data.
                    object value = dataRow[column.ColumnName.ToLower()];

                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        // If the current type is Byte[]
                        if (value.GetType().ToString() != "System.DBNull" && value is Byte[])
                        {
                            // Get the ling binary convertion
                            // for the current byte array.
                            // assign the current value
                            // with the new type.
                            System.Data.Linq.Binary binaryValue = new System.Data.Linq.Binary((Byte[])value);
                            value = binaryValue;
                        }

                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // Get the original value
                            // within the current property.
                            object originalValue = propertyInfo.GetValue(query, null);

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current column within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                            column.ColumnName.ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                    "UpdateItemKey", "Updates the DataRow to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey(DataRow dataRow,
            object keyValue, string keyName, object rowVersionData,
            string rowVersionName)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            return UpdateItemKey<TLinqEntity>(dataRow, keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemKey<TypeLinqEntity>(
            DataRow dataRow, object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (keyValue == null) throw new ArgumentNullException("keyValue");
            if (String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("keyName");
            if (rowVersionData == null) throw new ArgumentNullException("rowVersionData");
            if (String.IsNullOrEmpty(rowVersionName)) throw new ArgumentNullException("rowVersionName");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            // Get the first item found.
            TypeLinqEntity query =
                DataContext.GetTable<TypeLinqEntity>().First(keyName + " == @0 AND " + rowVersionName + " == @1", keyValue, rowVersionData);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each column within the data collection.
                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    // Get the current column data.
                    object value = dataRow[column.ColumnName.ToLower()];

                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        // If the current type is Byte[]
                        if (value.GetType().ToString() != "System.DBNull" && value is Byte[])
                        {
                            // Get the ling binary convertion
                            // for the current byte array.
                            // assign the current value
                            // with the new type.
                            System.Data.Linq.Binary binaryValue = new System.Data.Linq.Binary((Byte[])value);
                            value = binaryValue;
                        }

                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // Get the original value
                            // within the current property.
                            object originalValue = propertyInfo.GetValue(query, null);

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the column is not a null value.
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current column within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                            column.ColumnName.ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                    "UpdateItemKey", "Updates the DataRow to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + ConvertNullToString(keyValue) + " , " + keyName + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #region Public Virtual Update Predicate Methods
        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(
            DataRow dataRow, Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            return UpdateItemPredicate<TLinqEntity>(dataRow, predicate);
        }

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeLinqEntity>(
            DataRow dataRow, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            // Get the first item found.
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(predicate);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each column within the data collection.
                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    // Get the current column data.
                    object value = dataRow[column.ColumnName.ToLower()];

                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        // If the current type is Byte[]
                        if (value.GetType().ToString() != "System.DBNull" && value is Byte[])
                        {
                            // Get the ling binary convertion
                            // for the current byte array.
                            // assign the current value
                            // with the new type.
                            System.Data.Linq.Binary binaryValue = new System.Data.Linq.Binary((Byte[])value);
                            value = binaryValue;
                        }

                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // Get the original value
                            // within the current property.
                            object originalValue = propertyInfo.GetValue(query, null);

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current column within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                            column.ColumnName.ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                     "UpdateItemPredicate", "Updates the Data Entity to the database. " +
                     "A concurrency error may have occurred. " +
                     "{" + ConvertNullToString(predicate) + "}" +
                     "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(
            TLinqEntity linqEntityItem, Expression<Func<TLinqEntity, bool>> predicate)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            return UpdateItemPredicate<TLinqEntity>(linqEntityItem, predicate);
        }

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            // Get the first item found.
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(predicate);

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
                PropertyInfo[] linqEntityItemFields = linqEntityItem.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // For each field within the entity.
                foreach (PropertyInfo infoItem in linqEntityItemFields)
                {
                    // Get the current entity property value.
                    object value = infoItem.GetValue(linqEntityItem, null);

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

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current property name within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                            GetDbColumnName<TypeLinqEntity>(infoItem).ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                     "UpdateItemPredicate", "Updates the Data Entity to the database. " +
                     "A concurrency error may have occurred. " +
                     "{" + ConvertNullToString(predicate) + "}" +
                     "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(TLinqEntity linqEntityItem,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            return UpdateItemPredicate<TLinqEntity>(linqEntityItem, predicate, values);
        }

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");
            if (linqEntityItem == null) throw new ArgumentNullException("linqEntityItem");

            // Get the first item found.
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(predicate, values);

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
                PropertyInfo[] linqEntityItemFields = linqEntityItem.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // For each field within the entity.
                foreach (PropertyInfo infoItem in linqEntityItemFields)
                {
                    // Get the current entity property value.
                    object value = infoItem.GetValue(linqEntityItem, null);

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

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current property name within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                            GetDbColumnName<TypeLinqEntity>(infoItem).ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                    "UpdateItemPredicate", "Updates the Linq Entity to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate(DataRow dataRow,
            string predicate, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            return UpdateItemPredicate<TLinqEntity>(dataRow, predicate, values);
        }

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItemPredicate<TypeLinqEntity>(
            DataRow dataRow, string predicate, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (String.IsNullOrEmpty(predicate)) throw new ArgumentNullException("predicate");
            if (dataRow == null) throw new ArgumentNullException("dataRow");

            // Get the first item found.
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(predicate, values);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each column within the data collection.
                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    // Get the current column data.
                    object value = dataRow[column.ColumnName.ToLower()];

                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        // If the current type is Byte[]
                        if (value.GetType().ToString() != "System.DBNull" && value is Byte[])
                        {
                            // Get the ling binary convertion
                            // for the current byte array.
                            // assign the current value
                            // with the new type.
                            System.Data.Linq.Binary binaryValue = new System.Data.Linq.Binary((Byte[])value);
                            value = binaryValue;
                        }

                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            // Get the original value
                            // within the current property.
                            object originalValue = propertyInfo.GetValue(query, null);

                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // If the original and value are different
                                    // set the current property with the new value,
                                    // no need to set the property if the same.
                                    if (originalValue != value)
                                    {
                                        // If the current property within the property collection
                                        // is the current column within the data collection.
                                        if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                            column.ColumnName.ToLower().TrimStart('_'))
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
                    }
                }

                // Submit the changes that have been made
                // if any conflict then throw exception.
                DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            else
            {
                ErrorProvider(new Exception("Updating has failed. No data found."),
                    "UpdateItemPredicate", "Updates the DataRow to the database. " +
                    "A concurrency error may have occurred. " +
                    "{" + predicate.ToString() + " , " + ConvertNullToString(values) + "}" +
                    "{" + ConvertNullTypeToString(query) + "}");

                // Set the concurrency error.
                ConcurrencyError = true;
                return false;
            }
        }
        #endregion

        #region Public Virtual Update Item With Key Methods

        /// <summary>
        /// Updates the linq entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The linq entity to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItem(TLinqEntity linqEntityItem, bool useRowVersion)
        {
            return UpdateCollection(new TLinqEntity[] { linqEntityItem }, useRowVersion);
        }

        /// <summary>
        /// Updates the linq entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateItem<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, bool useRowVersion)
                where TypeLinqEntity : class, new()
        {
            return UpdateCollection<TypeLinqEntity>
                (new TypeLinqEntity[] { linqEntityItem }, useRowVersion);
        }
        #endregion

        #region Public Virtual Update Collection Methods

        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateCollection(TLinqEntity[] linqEntityItems, bool useRowVersion)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            return UpdateCollection<TLinqEntity>(linqEntityItems, useRowVersion);
        }

        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateCollection<TypeLinqEntity>(
            TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (linqEntityItems == null) throw new ArgumentNullException("linqEntityItems");

            // Get all primary keys.
            List<PropertyInfo> primaryKeys = GetAllPrimaryKeys<TypeLinqEntity>();
            List<PropertyInfo> rowVersions = null;

            // Should row version data be
            // used to update data.
            if (useRowVersion)
            {
                // Get all row version properties.
                rowVersions = GetAllRowVersions<TypeLinqEntity>();

                // No row version found.
                if (rowVersions.Count < 1)
                {
                    ErrorProvider(new Exception("No row version found."), "UpdateCollection",
                        "Can not update the data because no row version has been found.");
                    return false;
                }
            }

            // No primary keys found.
            if (primaryKeys.Count < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "UpdateCollection",
                    "Can not update the data because no primary key has been found.");
                return false;
            }

            // Return indicator.
            bool ret = false;

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
                int j = 0;
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
                        propertyInfo = linqProperties.First(p => p.Name.ToLower() == primaryKey.Name.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (GetDbColumnName<TypeLinqEntity>(primaryKey).ToLower().TrimStart('_') ==
                                    GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_'))
                        {
                            // Get the current value
                            // within the current property.
                            object value = propertyInfo.GetValue(linqEntity, null);
                            values.Add(value);

                            string name = GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_');
                            keys[i++] = name + " == @" + (count++).ToString();
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
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == rowVersion.Name.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            if (GetDbColumnName<TypeLinqEntity>(rowVersion).ToLower().TrimStart('_') ==
                                        GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_'))
                            {
                                // Get the current value
                                // within the current property.
                                object value = propertyInfo.GetValue(linqEntity, null);
                                values.Add(value);

                                string name = GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_');
                                rows[j++] = name + " == @" + (count++).ToString();
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

                // Create the query text and execute
                // the collection method.
                ret = UpdateCollectionEx<TypeLinqEntity>(linqEntity, queryText, values.ToArray());
                if (!ret)
                    return false;
            }

            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);

            // Return the collection of linq entities
            // including the new values.
            return ret;
        }

        /// <summary>
        /// Updates the data table to the database.
        /// </summary>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateCollection(DataTable dataTable)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            return UpdateCollection<TLinqEntity>(dataTable);
        }

        /// <summary>
        /// Updates the data table to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool UpdateCollection<TypeLinqEntity>(DataTable dataTable)
                where TypeLinqEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (dataTable == null) throw new ArgumentNullException("dataTable");

            // Get all primary keys for
            // the current table.
            DataColumn[] primaryKeys = dataTable.PrimaryKey;

            // No primary keys found.
            if (primaryKeys.Count() < 1)
            {
                ErrorProvider(new Exception("No primary key found."), "UpdateCollection",
                    "Can not update the data because no primary key has been found.");
                return false;
            }

            // Return indicator.
            bool ret = false;

            // Get the collection of all properties
            // in the current type.
            List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

            // For each item in the collection
            // insert the new data row.
            foreach (DataRow dataRow in dataTable.Rows)
            {
                int count = -1;
                List<object> values = new List<object>();

                int i = 0;
                string[] keys = new string[primaryKeys.Count()];

                // For each field within the entity.
                foreach (DataColumn primaryKey in primaryKeys)
                {
                    // Find in the property collection the current property that matches
                    // the current column. Use the Predicate delegate object to
                    // initiate a search for the specified match.
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = linqProperties.First(p => p.Name.ToLower() == primaryKey.ColumnName.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                primaryKey.ColumnName.ToLower().TrimStart('_'))
                        {
                            // Get the current column data.
                            object value = dataRow[primaryKey.ColumnName.ToLower()];
                            values.Add(value);

                            string name = GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_');
                            keys[i++] = name + " == @" + (count++).ToString();
                        }
                    }
                }

                string keyItems = string.Join(" AND ", keys);

                // Create the current row query item.
                string queryText = "(" + keyItems + ")";

                // Create the query text and execute
                // the collection method.
                ret = UpdateCollectionEx<TypeLinqEntity>(dataRow, queryText, values.ToArray());
                if (!ret)
                    return false;
            }

            // Submit the changes that have been made
            // if any conflict then throw exception.
            DataContext.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);

            // Return the collection of linq entities
            // including the new values.
            return ret;
        }
        #endregion

        #region Public Asynchronous Update Methods

        /// <summary>
        /// Begin update generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateLinqToSqlEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems, bool useRowVersion)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncUpdateLinqToSqlEntities<TDataContext, TLinqEntity>
                (this, callback, state, linqEntityItems, useRowVersion);
        }

        /// <summary>
        /// End update generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated items else false.</returns>
        public Boolean EndUpdateLinqToSqlEntities(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncUpdateLinqToSqlEntities<TDataContext, TLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin update generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateTypeLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new()
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncUpdateLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>
                (this, callback, state, linqEntityItems, useRowVersion);
        }

        /// <summary>
        /// End update generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated items else false.</returns>
        public Boolean EndUpdateTypeLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new()
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncUpdateLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity>.End(ar);
        }

        /// <summary>
        /// Begin update data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>The asynchronous result.</returns>
        public IAsyncResult BeginUpdateDataTable(AsyncCallback callback,
            object state, DataTable dataTable)
        {
            // Return an AsyncResult.
            return new LinqToSql.Async.AsyncUpdateDataTable<TDataContext, TLinqEntity>
                (this, callback, state, dataTable);
        }

        /// <summary>
        /// End update data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated items else false.</returns>
        public Boolean EndUpdateDataTable(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return LinqToSql.Async.AsyncUpdateDataTable<TDataContext, TLinqEntity>.End(ar);
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
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(queryText, values);

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
                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // Get the original value
                                    // within the current property.
                                    object originalValue = propertyInfo.GetValue(query, null);

                                    // If the current property within the property collection
                                    // is the current propert name within the data collection.
                                    if (GetDbColumnName<TypeLinqEntity>(infoItem).ToLower().TrimStart('_') ==
                                        GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_'))
                                    {
                                        if (originalValue != value)
                                            propertyInfo.SetValue(query, value, null);
                                    }
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
        /// Update the collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="dataRow">The row to update.</param>
        /// <param name="queryText">The predicate query text.</param>
        /// <param name="values">The query parameters.</param>
        /// <returns>True if succesful else false.</returns>
        private bool UpdateCollectionEx<TypeLinqEntity>(
            DataRow dataRow, string queryText, params object[] values)
                where TypeLinqEntity : class, new()
        {
            // Get the first item found.
            TypeLinqEntity query = DataContext.GetTable<TypeLinqEntity>().First(queryText, values);

            // If the query returned some data
            // then continue.
            if (query != null)
            {
                // Get the collection of all properties
                // in the current type.
                List<PropertyInfo> linqProperties = GetProperties(typeof(TypeLinqEntity));

                // For each column within the data collection.
                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    // Get the current column data.
                    object value = dataRow[column.ColumnName.ToLower()];
                    if (value.GetType().ToString() != "System.DBNull")
                    {
                        // If the current type is Byte[]
                        if (value.GetType().ToString() != "System.DBNull" && value is Byte[])
                        {
                            // Get the ling binary convertion
                            // for the current byte array.
                            // assign the current value
                            // with the new type.
                            System.Data.Linq.Binary binaryValue = new System.Data.Linq.Binary((Byte[])value);
                            value = binaryValue;
                        }

                        // Find in the property collection the current property that matches
                        // the current column. Use the Predicate delegate object to
                        // initiate a search for the specified match.
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = linqProperties.First(p => p.Name.ToLower() == column.ColumnName.ToLower());
                        }
                        catch { }
                        if (propertyInfo != null)
                        {
                            if (!IsAutoGenerated(propertyInfo))
                            {
                                if (!IsForeignKey(propertyInfo) && !IsAssociationKey(propertyInfo))
                                {
                                    // Get the original value
                                    // within the current property.
                                    object originalValue = propertyInfo.GetValue(query, null);

                                    // If the current property within the property collection
                                    // is the current propert name within the data collection.
                                    if (GetDbColumnName<TypeLinqEntity>(propertyInfo).ToLower().TrimStart('_') ==
                                        column.ColumnName.ToLower().TrimStart('_'))
                                    {
                                        if (originalValue != value)
                                            propertyInfo.SetValue(query, value, null);
                                    }
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
        #endregion

        #endregion
    }
}
