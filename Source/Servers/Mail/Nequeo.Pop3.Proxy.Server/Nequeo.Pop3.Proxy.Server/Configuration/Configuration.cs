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
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Nequeo.Security;

namespace Nequeo.Net.Configuration
{
    #region Proxy Pop3 Server Hosts Configuration
    /// <summary>
    /// Class that contains the collection of all host
    /// data within the configuration file.
    /// </summary>
    public class ProxyPop3ServerHosts : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyPop3ServerHosts()
        {
        }

        /// <summary>
        /// Gets sets, the host collection type.
        /// </summary>
        [ConfigurationProperty("Host")]
        public ProxyPop3HostCollection HostSection
        {
            get { return (ProxyPop3HostCollection)this["Host"]; }
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
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class ProxyPop3ServerDefaultHost : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyPop3ServerDefaultHost()
        {
        }

        /// <summary>
        /// Constructor with host attribute.
        /// </summary>
        /// <param name="hostNameAttrib">The host name attribute.</param>
        public ProxyPop3ServerDefaultHost(String hostNameAttrib)
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
        public ProxyPop3HostElement HostSection
        {
            get { return (ProxyPop3HostElement)this["Host"]; }
            set { this["Host"] = value; }
        }

        /// <summary>
        /// Gets sets, the host section attributes.
        /// </summary>
        [ConfigurationProperty("ServerCredentials")]
        public ProxyPop3ServerCredentialsElement ServerCredentialsSection
        {
            get { return (ProxyPop3ServerCredentialsElement)this["ServerCredentials"]; }
            set { this["ServerCredentials"] = value; }
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
    public class ProxyPop3ServerCredentialsElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyPop3ServerCredentialsElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="useCertificateStore">The use certificate store attribute.</param>
        public ProxyPop3ServerCredentialsElement(Boolean useCertificateStore)
        {
            UseCertificateStore = useCertificateStore;
        }

        /// <summary>
        /// Gets sets, the use certificate store attribute.
        /// </summary>
        [ConfigurationProperty("useCertificateStore", DefaultValue = "true", IsRequired = true)]
        public Boolean UseCertificateStore
        {
            get { return (Boolean)this["useCertificateStore"]; }
            set { this["useCertificateStore"] = value; }
        }

        /// <summary>
        /// Gets sets, the certificate path attribute.
        /// </summary>
        [ConfigurationProperty("CertificatePath")]
        public ProxyPop3ServerCredentialsPathElement CertificatePath
        {
            get { return (ProxyPop3ServerCredentialsPathElement)this["CertificatePath"]; }
            set { this["CertificatePath"] = value; }
        }


        /// <summary>
        /// Gets sets, the certificate store attribute.
        /// </summary>
        [ConfigurationProperty("CertificateStore")]
        public ProxyPop3ServerCredentialsStoreElement CertificateStore
        {
            get { return (ProxyPop3ServerCredentialsStoreElement)this["CertificateStore"]; }
            set { this["CertificateStore"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class ProxyPop3ServerCredentialsStoreElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyPop3ServerCredentialsStoreElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="findValue">The find value attribute.</param>
        /// <param name="storeLocation">The store location attribute.</param>
        /// <param name="storeName">The store name attribute.</param>
        /// <param name="x509FindType">The x509 find type attribute.</param>
        public ProxyPop3ServerCredentialsStoreElement(String findValue, StoreLocation storeLocation,
            StoreName storeName, X509FindType x509FindType)
        {
            FindValue = findValue;
            StoreLocation = storeLocation;
            StoreName = storeName;
            X509FindType = x509FindType;
        }

        /// <summary>
        /// Gets sets, the find value attribute.
        /// </summary>
        [ConfigurationProperty("findValue", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String FindValue
        {
            get { return (String)this["findValue"]; }
            set { this["findValue"] = value; }
        }

        /// <summary>
        /// Gets sets, the store location attribute.
        /// </summary>
        [ConfigurationProperty("storeLocation", DefaultValue = "LocalMachine", IsRequired = true)]
        public StoreLocation StoreLocation
        {
            get { return (StoreLocation)this["storeLocation"]; }
            set { this["storeLocation"] = value; }
        }

        /// <summary>
        /// Gets sets, the store name attribute.
        /// </summary>
        [ConfigurationProperty("storeName", DefaultValue = "My", IsRequired = true)]
        public StoreName StoreName
        {
            get { return (StoreName)this["storeName"]; }
            set { this["storeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the X509 find type attribute.
        /// </summary>
        [ConfigurationProperty("x509FindType", DefaultValue = "FindBySubjectName", IsRequired = true)]
        public X509FindType X509FindType
        {
            get { return (X509FindType)this["x509FindType"]; }
            set { this["x509FindType"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class ProxyPop3ServerCredentialsPathElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyPop3ServerCredentialsPathElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="path">The path attribute.</param>
        /// <param name="password">The password attribute.</param>
        public ProxyPop3ServerCredentialsPathElement(String path, String password)
        {
            Path = path;
            Password = password;
        }

        /// <summary>
        /// Gets sets, the path attribute.
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String Path
        {
            get { return (String)this["path"]; }
            set { this["path"] = value; }
        }

        /// <summary>
        /// Gets sets, the password attribute.
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Password
        {
            get { return (String)this["password"]; }
            set { this["password"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class ProxyPop3HostElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyPop3HostElement()
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
        public ProxyPop3HostElement(String nameAttribute, String hostAttribute, Int32 portAttribute,
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
        [ConfigurationProperty("port", DefaultValue = "110", IsRequired = true)]
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
        [IntegerValidator(MinValue = -1)]
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
    public class ProxyPop3HostCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyPop3HostCollection()
        {
            // Get the current host element.
            ProxyPop3HostElement host =
                (ProxyPop3HostElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProxyPop3HostElement();
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
            return ((ProxyPop3HostElement)element).NameAttribute;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(ProxyPop3HostElement element)
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
        public ProxyPop3HostElement this[int index]
        {
            get { return (ProxyPop3HostElement)BaseGet(index); }
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
        new public ProxyPop3HostElement this[string Name]
        {
            get { return (ProxyPop3HostElement)BaseGet(Name); }
        }
    }
    #endregion
}
