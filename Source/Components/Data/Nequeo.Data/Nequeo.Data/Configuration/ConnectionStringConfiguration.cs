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
using System.Collections;
using System.Configuration;
using System.Xml;

namespace Nequeo.Data.Configuration
{
    #region Connection String Extension Configuration
    /// <summary>
    /// Class that contains the collection of all extension
    /// data within the configuration file.
    /// </summary>
    public class ConnectionStringExtensions : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionStringExtensions()
        {
        }

        /// <summary>
        /// Gets sets, the extension collection type.
        /// </summary>
        [ConfigurationProperty("Extension")]
        public ConnectionStringExtensionCollection HostSection
        {
            get { return (ConnectionStringExtensionCollection)this["Extension"]; }
            set { this["Extension"] = value; }
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader object, 
        /// which reads from the configuration file.</param>
        protected override void DeserializeSection(XmlReader reader)
        {
            base.DeserializeSection(reader);
        }

        /// <summary>
        /// Creates an XML string containing an unmerged view of the 
        /// System.Configuration.ConfigurationSection
        ///  object as a single section to write to a file.
        /// </summary>
        /// <param name="parentElement">The System.Configuration.ConfigurationElement 
        /// instance to use as the parent when performing the un-merge.</param>
        /// <param name="name">The name of the section to create.</param>
        /// <param name="saveMode">The System.Configuration.ConfigurationSaveMode 
        /// instance to use when writing to a string.</param>
        /// <returns>An XML string containing an unmerged view of the 
        /// System.Configuration.ConfigurationSection object.</returns>
        protected override string SerializeSection(
            ConfigurationElement parentElement,
            string name, ConfigurationSaveMode saveMode)
        {
            return base.SerializeSection(parentElement, name, saveMode);
        }
    }

    /// <summary>
    /// Class that contains all the extension attributes.
    /// </summary>
    public class ConnectionStringExtensionElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionStringExtensionElement()
        {
        }

        /// <summary>
        /// Constructor with extension attributes
        /// </summary>
        /// <param name="nameAttribute">The name attribute.</param>
        /// <param name="connectionNameAttribute">The connection name attribute.</param>
        /// <param name="databaseOwnerAttribute">The database owner attribute.</param>
        /// <param name="connectionTimeOutAttribute">The connection timeout attribute.</param>
        /// <param name="connectionType">The connection type attribute.</param>
        /// <param name="connectionDataType">The connection data type attribute.</param>
        /// <param name="typeName">The type name controller attribute.</param>
        /// <param name="tableName">The table name controller attribute.</param>
        /// <param name="indicatorColumnName">The indicator column name controller attribute.</param>
        /// <param name="comparerColumnName">The comparer column name controller attribute.</param>
        /// <param name="comparerValue">The comparer value controller attribute.</param>
        /// <param name="dataObjectTypeName">The data object type name attribute.</param>
        /// <param name="dataObjectPropertyName">The data object key property name attribute.</param>
        /// <param name="serviceMethodName">The service method name attribute.</param>
        /// <param name="serviceMethodRedirectionUrl">The service method redirection url attribute.</param>
        /// <param name="jsonDataTableColumnNames">The json data table column names attribute.</param>
        /// <param name="dataAccessProvider">The data access provider column names attribute.</param>
        public ConnectionStringExtensionElement(String nameAttribute, String connectionNameAttribute,
            String databaseOwnerAttribute, Int32 connectionTimeOutAttribute, String connectionType,
            String connectionDataType, String typeName, String tableName, String indicatorColumnName,
            String comparerColumnName, String comparerValue, String dataObjectTypeName, String dataObjectPropertyName,
            String serviceMethodName, String serviceMethodRedirectionUrl, String jsonDataTableColumnNames, String dataAccessProvider)
        {
            Name = nameAttribute;
            ConnectionName = connectionNameAttribute;
            DatabaseOwner = databaseOwnerAttribute;
            ConnectionTimeOut = connectionTimeOutAttribute;
            ConnectionType = connectionType;
            ConnectionDataType = connectionDataType;
            TypeName = typeName;
            TableName = tableName;
            IndicatorColumnName = indicatorColumnName;
            ComparerColumnName = comparerColumnName;
            ComparerValue = comparerValue;
            DataObjectTypeName = dataObjectTypeName;
            DataObjectPropertyName = dataObjectPropertyName;
            ServiceMethodName = serviceMethodName;
            ServiceMethodRedirectionUrl = serviceMethodRedirectionUrl;
            JsonDataTableColumnNames = jsonDataTableColumnNames;
            DataAccessProvider = dataAccessProvider;
        }

        /// <summary>
        /// Gets sets, the name attribute.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "default", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets sets, the type name controller attribute.
        /// </summary>
        [ConfigurationProperty("typeName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String TypeName
        {
            get { return (String)this["typeName"]; }
            set { this["typeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the data object type name attribute.
        /// </summary>
        [ConfigurationProperty("dataObjectTypeName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String DataObjectTypeName
        {
            get { return (String)this["dataObjectTypeName"]; }
            set { this["dataObjectTypeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the data object property name attribute.
        /// </summary>
        [ConfigurationProperty("dataObjectPropertyName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String DataObjectPropertyName
        {
            get { return (String)this["dataObjectPropertyName"]; }
            set { this["dataObjectPropertyName"] = value; }
        }

        /// <summary>
        /// Gets sets, the table name controller attribute.
        /// </summary>
        [ConfigurationProperty("tableName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String TableName
        {
            get { return (String)this["tableName"]; }
            set { this["tableName"] = value; }
        }

        /// <summary>
        /// Gets sets, the indicator column name controller attribute.
        /// </summary>
        [ConfigurationProperty("indicatorColumnName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String IndicatorColumnName
        {
            get { return (String)this["indicatorColumnName"]; }
            set { this["indicatorColumnName"] = value; }
        }

        /// <summary>
        /// Gets sets, the comparer column name controller attribute.
        /// </summary>
        [ConfigurationProperty("comparerColumnName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String ComparerColumnName
        {
            get { return (String)this["comparerColumnName"]; }
            set { this["comparerColumnName"] = value; }
        }

        /// <summary>
        /// Gets sets, the comparer value controller attribute.
        /// </summary>
        [ConfigurationProperty("comparerValue", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String ComparerValue
        {
            get { return (String)this["comparerValue"]; }
            set { this["comparerValue"] = value; }
        }

        /// <summary>
        /// Gets sets, the service method name attribute.
        /// </summary>
        [ConfigurationProperty("serviceMethodName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String ServiceMethodName
        {
            get { return (String)this["serviceMethodName"]; }
            set { this["serviceMethodName"] = value; }
        }

        /// <summary>
        /// Gets sets, the service method redirection url attribute.
        /// </summary>
        [ConfigurationProperty("serviceMethodRedirectionUrl", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String ServiceMethodRedirectionUrl
        {
            get { return (String)this["serviceMethodRedirectionUrl"]; }
            set { this["serviceMethodRedirectionUrl"] = value; }
        }

        /// <summary>
        /// Gets sets, the json data table column names attribute.
        /// </summary>
        [ConfigurationProperty("jsonDataTableColumnNames", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String JsonDataTableColumnNames
        {
            get { return (String)this["jsonDataTableColumnNames"]; }
            set { this["jsonDataTableColumnNames"] = value; }
        }

        /// <summary>
        /// Gets sets, the connection name attribute.
        /// </summary>
        [ConfigurationProperty("connectionName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String ConnectionName
        {
            get { return (String)this["connectionName"]; }
            set { this["connectionName"] = value; }
        }

        /// <summary>
        /// Gets sets, the database owner attribute.
        /// </summary>
        [ConfigurationProperty("databaseOwner", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String DatabaseOwner
        {
            get { return (String)this["databaseOwner"]; }
            set { this["databaseOwner"] = value; }
        }

        /// <summary>
        /// Gets sets, the time out for the connection attribute.
        /// </summary>
        [ConfigurationProperty("connectionTimeOut", DefaultValue = "30", IsRequired = true)]
        [IntegerValidator(MinValue = -1)]
        public Int32 ConnectionTimeOut
        {
            get { return (Int32)this["connectionTimeOut"]; }
            set { this["connectionTimeOut"] = value; }
        }

        /// <summary>
        /// Gets sets, the Connection Type attribute.
        /// </summary>
        [ConfigurationProperty("connectionType", DefaultValue = "SqlConnection", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"", MinLength = 1, MaxLength = 500)]
        public String ConnectionType
        {
            get { return (String)this["connectionType"]; }
            set { this["connectionType"] = value; }
        }

        /// <summary>
        /// Gets sets, the Connection Data Type attribute.
        /// </summary>
        [ConfigurationProperty("connectionDataType", DefaultValue = "SqlDataType", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"", MinLength = 1, MaxLength = 500)]
        public String ConnectionDataType
        {
            get { return (String)this["connectionDataType"]; }
            set { this["connectionDataType"] = value; }
        }

        /// <summary>
        /// Gets sets, the Data Access Provider attribute.
        /// </summary>
        [ConfigurationProperty("dataAccessProvider", DefaultValue = "Nequeo.Data.SqlServer.DataAccess, Nequeo.Data.SqlServer.Provider", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"", MinLength = 1, MaxLength = 5000)]
        public String DataAccessProvider
        {
            get { return (String)this["dataAccessProvider"]; }
            set { this["dataAccessProvider"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the exenstion elements
    /// found in the configuration file.
    /// </summary>
    public class ConnectionStringExtensionCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionStringExtensionCollection()
        {
            // Get the current host element.
            ConnectionStringExtensionElement host =
                (ConnectionStringExtensionElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionStringExtensionElement();
        }

        /// <summary>
        /// Get the current element key.
        /// </summary>
        /// <param name="element">The current element.</param>
        /// <returns>The current host element key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            // The current element key is the name attribute
            // of the host element.
            return ((ConnectionStringExtensionElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(ConnectionStringExtensionElement element)
        {
            // Add the element to the base
            // ConfigurationElementCollection type.
            BaseAdd(element);
        }

        /// <summary>
        /// Indexer that gets the specified host element
        /// for the specified index.
        /// </summary>
        /// <param name="index">The index of the host element.</param>
        /// <returns>The current host element type.</returns>
        public ConnectionStringExtensionElement this[int index]
        {
            get { return (ConnectionStringExtensionElement)BaseGet(index); }
            set
            {
                // Remove the current host element
                // from the collection at this index.
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                // Add a new host element.
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Indexer that gets the specified host element
        /// for the specified key name.
        /// </summary>
        /// <param name="Name">The key name of the element.</param>
        /// <returns>The current host element type.</returns>
        new public ConnectionStringExtensionElement this[string Name]
        {
            get { return (ConnectionStringExtensionElement)BaseGet(Name); }
        }
    }
    #endregion

    /// <summary>
    /// Connection string wxtension section group configuration manager.
    /// </summary>
    public class ConnectionStringExtensionConfigurationManager
    {
        /// <summary>
        /// Gets, the connection string extension collection elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The connection string collection; else null.</returns>
        public static ConnectionStringExtensionCollection ExtensionCollection(string section = "DataConnectionStringExtensionGroup/ConnectionStringExtensions")
        {
            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                ConnectionStringExtensions baseHandler =
                    (ConnectionStringExtensions)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                return baseHandler.HostSection;
            }
            catch { }
            {
                return null;
            }
        }

        /// <summary>
        /// Gets, the connection string extension collection elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The connection string collection; else null.</returns>
        public static ConnectionStringExtensionElement[] ConnectionStringExtensionElements(string section = "DataConnectionStringExtensionGroup/ConnectionStringExtensions")
        {
            try
            {
                //Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                ConnectionStringExtensions baseHandler =
                    (ConnectionStringExtensions)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                // Return the collection.
                ConnectionStringExtensionElement[] items = new ConnectionStringExtensionElement[baseHandler.HostSection.Count];
                baseHandler.HostSection.CopyTo(items, 0);
                return items.Where(q => (q.Name != "default")).ToArray();
            }
            catch { }
            {
                return null;
            }
        }
    }
}
