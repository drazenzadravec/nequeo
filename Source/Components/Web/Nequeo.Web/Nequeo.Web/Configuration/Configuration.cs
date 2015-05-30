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

namespace Nequeo.Web.Configuration
{
    #region HttpHandler Data Configuration
    /// <summary>
    /// Class that contains the collection of all extension
    /// data within the configuration file.
    /// </summary>
    public class HttpHandlerDataExtensions : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpHandlerDataExtensions()
        {
        }

        /// <summary>
        /// Gets sets, the extension collection type.
        /// </summary>
        [ConfigurationProperty("Extension")]
        public HttpHandlerDataCollection HostSection
        {
            get { return (HttpHandlerDataCollection)this["Extension"]; }
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
    public class HttpHandlerDataExtensionElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpHandlerDataExtensionElement()
        {
        }

        /// <summary>
        /// Constructor with extension attributes
        /// </summary>
        /// <param name="name">The name attribute.</param>
        /// <param name="connectionName">The connection name attribute.</param>
        /// <param name="connectionType">The connection type attribute.</param>
        /// <param name="connectionDataType">The connection data type attribute.</param>
        /// <param name="httpHandlerTypeName">The http handler type name attribute.</param>
        /// <param name="dataObjectTypeName">The data object type name attribute.</param>
        /// <param name="dataObjectPropertyName">The data object key property name attribute.</param>
        /// <param name="urlQueryTextName">The the url query text key name for the porperty name match attribute.</param>
        /// <param name="referenceLazyLoading">The lazy loading indicator attribute.</param>
        /// <param name="childPageGroupExecution">The child page to execute attribute.</param>
        /// <param name="dataAccessProvider">The data access provider attribute.</param>
        public HttpHandlerDataExtensionElement(String name, String connectionName, String connectionType, 
            String connectionDataType, String httpHandlerTypeName, String dataObjectTypeName, 
            String dataObjectPropertyName, String urlQueryTextName, Boolean referenceLazyLoading,
            String childPageGroupExecution, String dataAccessProvider)
        {
            Name = name;
            ConnectionName = connectionName;
            ConnectionType = connectionType;
            ConnectionDataType = connectionDataType;
            HttpHandlerTypeName = httpHandlerTypeName;
            DataObjectTypeName = dataObjectTypeName;
            DataObjectPropertyName = dataObjectPropertyName;
            UrlQueryTextName = urlQueryTextName;
            ReferenceLazyLoading = referenceLazyLoading;
            ChildPageGroupExecution = childPageGroupExecution;
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
        /// Gets sets, the http handler type name attribute.
        /// </summary>
        [ConfigurationProperty("httpHandlerTypeName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String HttpHandlerTypeName
        {
            get { return (String)this["httpHandlerTypeName"]; }
            set { this["httpHandlerTypeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the data object type name attribute.
        /// </summary>
        [ConfigurationProperty("dataObjectTypeName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String DataObjectTypeName
        {
            get { return (String)this["dataObjectTypeName"]; }
            set { this["dataObjectTypeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the data object property name attribute.
        /// </summary>
        [ConfigurationProperty("dataObjectPropertyName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String DataObjectPropertyName
        {
            get { return (String)this["dataObjectPropertyName"]; }
            set { this["dataObjectPropertyName"] = value; }
        }

        /// <summary>
        /// Gets sets, the url query text name attribute.
        /// </summary>
        [ConfigurationProperty("urlQueryTextName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String UrlQueryTextName
        {
            get { return (String)this["urlQueryTextName"]; }
            set { this["urlQueryTextName"] = value; }
        }

        /// <summary>
        /// Gets sets, the child page group execution attribute.
        /// </summary>
        [ConfigurationProperty("childPageGroupExecution", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String ChildPageGroupExecution
        {
            get { return (String)this["childPageGroupExecution"]; }
            set { this["childPageGroupExecution"] = value; }
        }

        /// <summary>
        /// Gets sets, the reference lazy loading attribute.
        /// </summary>
        [ConfigurationProperty("referenceLazyLoading", DefaultValue = "false", IsRequired = true)]
        public Boolean ReferenceLazyLoading
        {
            get { return (Boolean)this["referenceLazyLoading"]; }
            set { this["referenceLazyLoading"] = value; }
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
        /// Gets sets, the Connection Type attribute.
        /// </summary>
        [ConfigurationProperty("connectionType", DefaultValue = "SqlConnection", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"", MinLength = 0, MaxLength = 500)]
        public String ConnectionType
        {
            get { return (String)this["connectionType"]; }
            set { this["connectionType"] = value; }
        }

        /// <summary>
        /// Gets sets, the Connection Data Type attribute.
        /// </summary>
        [ConfigurationProperty("connectionDataType", DefaultValue = "SqlDataType", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"", MinLength = 0, MaxLength = 500)]
        public String ConnectionDataType
        {
            get { return (String)this["connectionDataType"]; }
            set { this["connectionDataType"] = value; }
        }

        /// <summary>
        /// Gets sets, the Data Access Provider attribute.
        /// </summary>
        [ConfigurationProperty("dataAccessProvider", DefaultValue = "Nequeo.Data.SqlServer.DataAccess, Nequeo.Data.SqlServer.Provider", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"", MinLength = 0, MaxLength = 5000)]
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
    public class HttpHandlerDataCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpHandlerDataCollection()
        {
            // Get the current host element.
            HttpHandlerDataExtensionElement host =
                (HttpHandlerDataExtensionElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new HttpHandlerDataExtensionElement();
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
            return ((HttpHandlerDataExtensionElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(HttpHandlerDataExtensionElement element)
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
        public HttpHandlerDataExtensionElement this[int index]
        {
            get { return (HttpHandlerDataExtensionElement)BaseGet(index); }
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
        new public HttpHandlerDataExtensionElement this[string Name]
        {
            get { return (HttpHandlerDataExtensionElement)BaseGet(Name); }
        }
    }
    #endregion

    #region HttpHandler File Upload Configuration
    /// <summary>
    /// Class that contains the collection of all extension
    /// data within the configuration file.
    /// </summary>
    public class HttpHandlerUploadExtensions : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpHandlerUploadExtensions()
        {
        }

        /// <summary>
        /// Gets sets, the extension collection type.
        /// </summary>
        [ConfigurationProperty("Extension")]
        public HttpHandlerUploadCollection HostSection
        {
            get { return (HttpHandlerUploadCollection)this["Extension"]; }
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
    public class HttpHandlerUploadExtensionElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpHandlerUploadExtensionElement()
        {
        }

        /// <summary>
        /// Constructor with extension attributes
        /// </summary>
        /// <param name="name">The name attribute.</param>
        /// <param name="httpHandlerTypeName">The http handler type name attribute.</param>
        /// <param name="uploadPath">The upload path attribute.</param>
        public HttpHandlerUploadExtensionElement(String name, String httpHandlerTypeName, String uploadPath)
        {
            Name = name;
            HttpHandlerTypeName = httpHandlerTypeName;
            UploadPath = uploadPath;
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
        /// Gets sets, the http handler type name attribute.
        /// </summary>
        [ConfigurationProperty("httpHandlerTypeName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String HttpHandlerTypeName
        {
            get { return (String)this["httpHandlerTypeName"]; }
            set { this["httpHandlerTypeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the upload path attribute.
        /// </summary>
        [ConfigurationProperty("uploadPath", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String UploadPath
        {
            get { return (String)this["uploadPath"]; }
            set { this["uploadPath"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the exenstion elements
    /// found in the configuration file.
    /// </summary>
    public class HttpHandlerUploadCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpHandlerUploadCollection()
        {
            // Get the current host element.
            HttpHandlerUploadExtensionElement host =
                (HttpHandlerUploadExtensionElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new HttpHandlerUploadExtensionElement();
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
            return ((HttpHandlerUploadExtensionElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(HttpHandlerUploadExtensionElement element)
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
        public HttpHandlerUploadExtensionElement this[int index]
        {
            get { return (HttpHandlerUploadExtensionElement)BaseGet(index); }
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
        new public HttpHandlerUploadExtensionElement this[string Name]
        {
            get { return (HttpHandlerUploadExtensionElement)BaseGet(Name); }
        }
    }
    #endregion

    /// <summary>
    /// Http generic data handler extension section group configuration manager.
    /// </summary>
    public class HttpHandlerDataConfigurationManager
    {
        /// <summary>
        /// Gets, the http handler extension collection elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The http handler collection; else null.</returns>
        public static HttpHandlerDataCollection HttpHandlerCollection(string section = "HttpHandlerExtensionGroup/HttpHandlerDataExtensions")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                HttpHandlerDataExtensions baseHandler =
                    (HttpHandlerDataExtensions)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                return baseHandler.HostSection;
            }
            catch { }
            {
                return null;
            }
        }

        /// <summary>
        /// Gets sets, the http handler state elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The http handler collection; else null.</returns>
        public static HttpHandlerDataExtensionElement[] HttpHandlerExtensionElements(string section = "HttpHandlerExtensionGroup/HttpHandlerDataExtensions")
        {
            // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
            System.Configuration.ConfigurationManager.RefreshSection(section);

            // Get the default element
            // configuration information
            // from the configuration manager.
            HttpHandlerDataExtensions baseHandler =
                (HttpHandlerDataExtensions)System.Configuration.ConfigurationManager.GetSection(section);

            // Return the collection.
            HttpHandlerDataExtensionElement[] items = new HttpHandlerDataExtensionElement[baseHandler.HostSection.Count];
            baseHandler.HostSection.CopyTo(items, 0);
            return items.Where(q => (q.Name != "default")).ToArray();
        }
    }

    /// <summary>
    /// Http file upload handler extension section group configuration manager.
    /// </summary>
    public class HttpHandlerUploadConfigurationManager
    {
        /// <summary>
        /// Gets, the http handler extension collection elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The http handler upload collection; else null.</returns>
        public static HttpHandlerUploadCollection HttpHandlerCollection(string section = "HttpHandlerExtensionGroup/HttpHandlerUploadExtensions")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                HttpHandlerUploadExtensions baseHandler =
                    (HttpHandlerUploadExtensions)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                return baseHandler.HostSection;
            }
            catch { }
            {
                return null;
            }
        }

        /// <summary>
        /// Gets sets, the http handler state elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The http handler upload collection; else null.</returns>
        public static HttpHandlerUploadExtensionElement[] HttpHandlerExtensionElements(string section = "HttpHandlerExtensionGroup/HttpHandlerUploadExtensions")
        {
            // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
            System.Configuration.ConfigurationManager.RefreshSection(section);

            // Get the default element
            // configuration information
            // from the configuration manager.
            HttpHandlerUploadExtensions baseHandler =
                (HttpHandlerUploadExtensions)System.Configuration.ConfigurationManager.GetSection(section);

            // Return the collection.
            HttpHandlerUploadExtensionElement[] items = new HttpHandlerUploadExtensionElement[baseHandler.HostSection.Count];
            baseHandler.HostSection.CopyTo(items, 0);
            return items.Where(q => (q.Name != "default")).ToArray();
        }
    }
}
