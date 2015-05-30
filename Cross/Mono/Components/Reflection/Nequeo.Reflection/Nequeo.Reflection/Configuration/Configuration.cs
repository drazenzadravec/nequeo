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
using System.Data;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace Nequeo.Configuration
{
    #region Reflections Provider Configuration
    /// <summary>
    /// Class that contains the collection of all host
    /// data within the configuration file.
    /// </summary>
    public class ReflectionsProvider : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReflectionsProvider()
        {
        }

        /// <summary>
        /// Gets sets, the host collection type.
        /// </summary>
        [ConfigurationProperty("Provider")]
        public ReflectionsProviderCollection HostSection
        {
            get { return (ReflectionsProviderCollection)this["Provider"]; }
            set { this["Provider"] = value; }
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
    /// Class that contains all the host attributes.
    /// </summary>
    public class ReflectionsProviderElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReflectionsProviderElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="name">The name attribute.</param>
        /// <param name="type">The provider type.</param>
        public ReflectionsProviderElement(String name, String type)
        {
            Name = name;
            ProviderType = type;
        }

        /// <summary>
        /// Gets sets, the name attribute.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "None", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets sets, the provider type attribute.
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String ProviderType
        {
            get { return (String)this["type"]; }
            set { this["type"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host elements
    /// found in the configuration file.
    /// </summary>
    public class ReflectionsProviderCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReflectionsProviderCollection()
        {
            // Get the current host element.
            ReflectionsProviderElement host =
                (ReflectionsProviderElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ReflectionsProviderElement();
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
            return ((ReflectionsProviderElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(ReflectionsProviderElement element)
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
        public ReflectionsProviderElement this[int index]
        {
            get { return (ReflectionsProviderElement)BaseGet(index); }
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
        new public ReflectionsProviderElement this[string Name]
        {
            get { return (ReflectionsProviderElement)BaseGet(Name); }
        }
    }
    #endregion

    /// <summary>
    /// Configuration reader
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Get the reflection provider type.
        /// </summary>
        /// <param name="name">The name of the host.</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The reflection provider type; else null.</returns>
        public string GetReflectionProvider(string name, string section = "NequeoReflectionGroup/NequeoReflections")
        {
            string provider = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                ReflectionsProvider defaultHost =
                    (ReflectionsProvider)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultHost == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the provider element.
                ReflectionsProviderElement providerElement = defaultHost.HostSection[name];
                provider = providerElement.ProviderType;
            }
            catch (Exception)
            {
                throw;
            }

            // Return the provider type.
            return provider;
        }
    }
}
