/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Nequeo.Composite.Configuration
{
    #region Composite Service Hosts Configuration
    /// <summary>
    /// Class that contains the collection of all host
    /// data within the configuration file.
    /// </summary>
    public class CompositeServices : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompositeServices()
        {
        }

        /// <summary>
        /// Gets sets, the service directory collection type.
        /// </summary>
        [ConfigurationProperty("ServiceDirectoryCatalog")]
        public CompositeServiceDirectoryCatalogCollection ServiceDirectoryCatalogSection
        {
            get { return (CompositeServiceDirectoryCatalogCollection)this["ServiceDirectoryCatalog"]; }
            set { this["ServiceDirectoryCatalog"] = value; }
        }

        /// <summary>
        /// Gets sets, the service type collection type.
        /// </summary>
        [ConfigurationProperty("ServiceTypeCatalog")]
        public CompositeServiceTypeCatalogCollection ServiceTypeCatalogSection
        {
            get { return (CompositeServiceTypeCatalogCollection)this["ServiceTypeCatalog"]; }
            set { this["ServiceTypeCatalog"] = value; }
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
        /// object as a single section to write to a file.
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
    /// Class that contains all the host elements
    /// found in the configuration file.
    /// </summary>
    public class CompositeServiceDirectoryCatalogCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompositeServiceDirectoryCatalogCollection()
        {
            // Get the current host element.
            CompositeServiceDirectoryCatalogElement host =
                (CompositeServiceDirectoryCatalogElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CompositeServiceDirectoryCatalogElement();
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
            return ((CompositeServiceDirectoryCatalogElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(CompositeServiceDirectoryCatalogElement element)
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
        public CompositeServiceDirectoryCatalogElement this[int index]
        {
            get { return (CompositeServiceDirectoryCatalogElement)BaseGet(index); }
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
        new public CompositeServiceDirectoryCatalogElement this[string Name]
        {
            get { return (CompositeServiceDirectoryCatalogElement)BaseGet(Name); }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class CompositeServiceDirectoryCatalogElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompositeServiceDirectoryCatalogElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="nameAttribute">The name attribute.</param>
        /// <param name="pathAttribute">The service path attribute.</param>
        /// <param name="searchPatternAttribute">The service search pattern attribute.</param>
        public CompositeServiceDirectoryCatalogElement(String nameAttribute, String pathAttribute, String searchPatternAttribute)
        {
            Name = nameAttribute;
            Path = pathAttribute;
            SearchPattern = searchPatternAttribute;
        }

        /// <summary>
        /// Gets sets, the name attribute.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "default", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets sets, the path attribute.
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "C:\\TempDefault\\", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Path
        {
            get { return (String)this["path"]; }
            set { this["path"] = value; }
        }

        /// <summary>
        /// Gets sets, the search pattern attribute.
        /// </summary>
        [ConfigurationProperty("searchPattern", DefaultValue = "*.dll", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String SearchPattern
        {
            get { return (String)this["searchPattern"]; }
            set { this["searchPattern"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host elements
    /// found in the configuration file.
    /// </summary>
    public class CompositeServiceTypeCatalogCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompositeServiceTypeCatalogCollection()
        {
            // Get the current host element.
            CompositeServiceTypeCatalogElement host =
                (CompositeServiceTypeCatalogElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CompositeServiceTypeCatalogElement();
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
            return ((CompositeServiceTypeCatalogElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(CompositeServiceTypeCatalogElement element)
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
        public CompositeServiceTypeCatalogElement this[int index]
        {
            get { return (CompositeServiceTypeCatalogElement)BaseGet(index); }
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
        new public CompositeServiceTypeCatalogElement this[string Name]
        {
            get { return (CompositeServiceTypeCatalogElement)BaseGet(Name); }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class CompositeServiceTypeCatalogElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompositeServiceTypeCatalogElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="nameAttribute">The name attribute.</param>
        /// <param name="typeAttribute">The service type attribute.</param>
        public CompositeServiceTypeCatalogElement(String nameAttribute, String typeAttribute)
        {
            Name = nameAttribute;
            Type = typeAttribute;
        }

        /// <summary>
        /// Gets sets, the name attribute.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "default", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets sets, the type attribute.
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Type
        {
            get { return (String)this["type"]; }
            set { this["type"] = value; }
        }
    }
    #endregion
}
