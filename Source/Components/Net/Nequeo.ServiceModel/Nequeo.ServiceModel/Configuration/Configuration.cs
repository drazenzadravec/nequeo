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
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

using Nequeo.Security;

namespace Nequeo.Net.ServiceModel.Configuration
{
    #region Wcf Service Host Configuration
    /// <summary>
    /// Class that contains the collection of all extension
    /// data within the configuration file.
    /// </summary>
    public class ServiceHostExtensions : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceHostExtensions()
        {
        }

        /// <summary>
        /// Gets sets, the extension collection type.
        /// </summary>
        [ConfigurationProperty("Extension")]
        public ServiceHostCollection HostSection
        {
            get { return (ServiceHostCollection)this["Extension"]; }
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
    public class ServiceHostExtensionElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceHostExtensionElement()
        {
        }

        /// <summary>
        /// Constructor with extension attributes
        /// </summary>
        /// <param name="name">The name attribute.</param>
        /// <param name="serviceTypeName">The service type name attribute.</param>
        /// <param name="baseAddressIndex">The base address index attribute.</param>
        public ServiceHostExtensionElement(String name, String serviceTypeName, Int32 baseAddressIndex)
        {
            Name = name;
            ServiceTypeName = serviceTypeName;
            BaseAddressIndex = baseAddressIndex;
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
        /// Gets sets, the service type name attribute.
        /// </summary>
        [ConfigurationProperty("serviceTypeName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String ServiceTypeName
        {
            get { return (String)this["serviceTypeName"]; }
            set { this["serviceTypeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the base address index attribute.
        /// </summary>
        [ConfigurationProperty("baseAddressIndex", DefaultValue = "0", IsRequired = true)]
        [IntegerValidator(MinValue = 0)]
        public Int32 BaseAddressIndex
        {
            get { return (Int32)this["baseAddressIndex"]; }
            set { this["baseAddressIndex"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the exenstion elements
    /// found in the configuration file.
    /// </summary>
    public class ServiceHostCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceHostCollection()
        {
            // Get the current host element.
            ServiceHostExtensionElement host =
                (ServiceHostExtensionElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceHostExtensionElement();
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
            return ((ServiceHostExtensionElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(ServiceHostExtensionElement element)
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
        public ServiceHostExtensionElement this[int index]
        {
            get { return (ServiceHostExtensionElement)BaseGet(index); }
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
        new public ServiceHostExtensionElement this[string Name]
        {
            get { return (ServiceHostExtensionElement)BaseGet(Name); }
        }
    }
    #endregion

    /// <summary>
    /// Service host configuration manager
    /// </summary>
    public class ServiceHostConfigurationManager
    {
        /// <summary>
        /// Gets, the Service Host extension collection elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The service host collection; else null.</returns>
        public static ServiceHostCollection ServiceHostCollection(string section = "ServiceHostExtensionGroup/ServiceHostExtensions")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                ServiceHostExtensions baseHandler =
                    (ServiceHostExtensions)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                return baseHandler.HostSection;
            }
            catch { }
            {
                return null;
            }
        }

        /// <summary>
        /// Gets sets, the Service Host state elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The service host collection; else null.</returns>
        public static ServiceHostExtensionElement[] ServiceHostExtensionElements(string section = "ServiceHostExtensionGroup/ServiceHostExtensions")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                ServiceHostExtensions baseHandler =
                    (ServiceHostExtensions)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the collection.
                ServiceHostExtensionElement[] items = new ServiceHostExtensionElement[baseHandler.HostSection.Count];
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
