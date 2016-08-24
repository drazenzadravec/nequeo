/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 *                  
 *                  
 *                  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Nequeo.Net.FileTransfer.Configuration
{
    #region File Transfer Server Hosts Configuration
    /// <summary>
    /// Class that contains the collection of all host
    /// data within the configuration file.
    /// </summary>
    public class FileTransferServerHosts : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileTransferServerHosts()
        {
        }

        /// <summary>
        /// Gets sets, the host collection type.
        /// </summary>
        [ConfigurationProperty("Host")]
        public FileTransferHostCollection HostSection
        {
            get { return (FileTransferHostCollection)this["Host"]; }
            set { this["Host"] = value; }
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
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class FileTransferServerDefaultHost : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileTransferServerDefaultHost()
        {
        }

        /// <summary>
        /// Constructor with host attribute.
        /// </summary>
        /// <param name="hostNameAttrib">The host name attribute.</param>
        public FileTransferServerDefaultHost(String hostNameAttrib)
        {
            HostNameAttrib = hostNameAttrib;
        }

        /// <summary>
        /// Gets sets, the host name attribute value.
        /// </summary>
        [ConfigurationProperty("hostName", DefaultValue = "localhost", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0, MaxLength = 60)]
        public String HostNameAttrib
        {
            get { return (String)this["hostName"]; }
            set { this["hostName"] = value; }
        }

        /// <summary>
        /// Gets sets, the host section attributes.
        /// </summary>
        [ConfigurationProperty("Host")]
        public FileTransferHostElement HostSection
        {
            get { return (FileTransferHostElement)this["Host"]; }
            set { this["Host"] = value; }
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
    /// Class that contains all the host attributes.
    /// </summary>
    public class FileTransferHostElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileTransferHostElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="nameAttribute">The name attribute.</param>
        /// <param name="hostAttribute">The remote host attribute.</param>
        /// <param name="portAttribute">The remote port attribute.</param>
        /// <param name="maxNumClientsAttribute">The remote max number clients attribute.</param>
        /// <param name="clientTimeOutAttribute">The remote client timeout attribute.</param>
        public FileTransferHostElement(String nameAttribute, String hostAttribute, Int32 portAttribute,
            Int32 maxNumClientsAttribute, Int32 clientTimeOutAttribute)
        {
            NameAttribute = nameAttribute;
            HostAttribute = hostAttribute;
            PortAttribute = portAttribute;
            MaxNumClientsAttribute = maxNumClientsAttribute;
            ClientTimeOutAttribute = clientTimeOutAttribute;
        }

        /// <summary>
        /// Gets sets, the name attribute.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "localhost", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String NameAttribute
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets sets, the remote host attribute.
        /// </summary>
        [ConfigurationProperty("host", DefaultValue = "localhost", IsRequired = true)]
        [StringValidator(MinLength = 1, MaxLength = 60)]
        public String HostAttribute
        {
            get { return (String)this["host"]; }
            set { this["host"] = value; }
        }

        /// <summary>
        /// Gets sets, the remote port attribute.
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = "2766", IsRequired = true)]
        [IntegerValidator(MinValue = 0)]
        public Int32 PortAttribute
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        /// <summary>
        /// Gets sets, the maximum number of clients attribute.
        /// </summary>
        [ConfigurationProperty("maxNumClients", DefaultValue = "30", IsRequired = true)]
        [IntegerValidator(MinValue = 1)]
        public Int32 MaxNumClientsAttribute
        {
            get { return (Int32)this["maxNumClients"]; }
            set { this["maxNumClients"] = value; }
        }

        /// <summary>
        /// Gets sets, the time out for each client attribute.
        /// </summary>
        [ConfigurationProperty("clientTimeOut", DefaultValue = "30", IsRequired = true)]
        [IntegerValidator(MinValue = 1)]
        public Int32 ClientTimeOutAttribute
        {
            get { return (Int32)this["clientTimeOut"]; }
            set { this["clientTimeOut"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host elements
    /// found in the configuration file.
    /// </summary>
    public class FileTransferHostCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileTransferHostCollection()
        {
            // Get the current host element.
            FileTransferHostElement host =
                (FileTransferHostElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new FileTransferHostElement();
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
            return ((FileTransferHostElement)element).NameAttribute;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(FileTransferHostElement element)
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
        public FileTransferHostElement this[int index]
        {
            get { return (FileTransferHostElement)BaseGet(index); }
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
        new public FileTransferHostElement this[string Name]
        {
            get { return (FileTransferHostElement)BaseGet(Name); }
        }
    }
    #endregion
}
