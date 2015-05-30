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
using System.Net;
using System.Configuration;
using System.Xml;
using System.Threading;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Nequeo.Net.Configuration
{
    #region Server Hosts Configuration
    /// <summary>
    /// Class that contains the collection of all host
    /// data within the configuration file.
    /// </summary>
    public class ServerHosts : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerHosts()
        {
        }

        /// <summary>
        /// Gets sets, the host collection type.
        /// </summary>
        [ConfigurationProperty("Host")]
        public ServerHostCollection HostSection
        {
            get { return (ServerHostCollection)this["Host"]; }
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
    /// Class that contains all the host attributes.
    /// </summary>
    public class ServerHostElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerHostElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="name">The name attribute.</param>
        /// <param name="port">The remote port attribute.</param>
        /// <param name="clientTimeOut">The remote client timeout attribute.</param>
        /// <param name="maxNumClients">The remote max number clients attribute.</param>
        /// <param name="individualControl">should this server control maximum number of clients independent of all other servers.</param>
        public ServerHostElement(String name, Int32 port, Int32 clientTimeOut,
            Int32 maxNumClients, Boolean individualControl)
        {
            Name = name;
            Port = port;
            MaxNumClients = maxNumClients;
            ClientTimeOut = clientTimeOut;
            IndividualControl = individualControl;
        }

        /// <summary>
        /// Gets sets, the name attribute.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "localhost", IsRequired = true, IsKey = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets sets, the remote port attribute.
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = "88", IsRequired = true)]
        [IntegerValidator(MinValue = 0)]
        public Int32 Port
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        /// <summary>
        /// Gets sets, the time out for each client attribute.
        /// </summary>
        [ConfigurationProperty("clientTimeOut", DefaultValue = "5", IsRequired = true)]
        [IntegerValidator(MinValue = -1)]
        public Int32 ClientTimeOut
        {
            get { return (Int32)this["clientTimeOut"]; }
            set { this["clientTimeOut"] = value; }
        }

        /// <summary>
        /// Gets sets, the maximum number of clients attribute.
        /// </summary>
        [ConfigurationProperty("maxNumClients", DefaultValue = "2147483647", IsRequired = true)]
        [IntegerValidator(MinValue = -1)]
        public Int32 MaxNumClients
        {
            get { return (Int32)this["maxNumClients"]; }
            set { this["maxNumClients"] = value; }
        }

        /// <summary>
        /// Gets sets, should this server control maximum number of clients
        /// independent of all other servers within the multi-endpoint.
        /// Is this server part of a collection of multi-endpoint servers.
        /// </summary>
        [ConfigurationProperty("individualControl", DefaultValue = "true", IsRequired = true)]
        public Boolean IndividualControl
        {
            get { return (Boolean)this["individualControl"]; }
            set { this["individualControl"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host elements
    /// found in the configuration file.
    /// </summary>
    public class ServerHostCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerHostCollection()
        {
            // Get the current host element.
            ServerHostElement host =
                (ServerHostElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerHostElement();
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
            return ((ServerHostElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(ServerHostElement element)
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
        public ServerHostElement this[int index]
        {
            get { return (ServerHostElement)BaseGet(index); }
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
        new public ServerHostElement this[string Name]
        {
            get { return (ServerHostElement)BaseGet(Name); }
        }
    }
    #endregion

    /// <summary>
    /// Configuration reader
    /// </summary>
    public partial class Reader
    {
        /// <summary>
        /// Get the server host element.
        /// </summary>
        /// <param name="name">The name of the host.</param>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The server host element; else null.</returns>
        public ServerHostElement GetServerHost(string name, string section = "NequeoServerGroup/NequeoServerHosts")
        {
            ServerHostElement serverHostElement = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                ServerHosts defaultHost =
                    (ServerHosts)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultHost == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the host element.
                serverHostElement = defaultHost.HostSection[name];
            }
            catch (Exception)
            {
                throw;
            }

            // Return the host element.
            return serverHostElement;
        }

        /// <summary>
        /// Gets the server host elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The server host element; else null.</returns>
        public ServerHostElement[] ServerHostElements(string section = "NequeoServerGroup/NequeoServerHosts")
        {
            ServerHostElement[] serverHostElements = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                ServerHosts defaultHost =
                    (ServerHosts)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultHost == null)
                    throw new Exception("Configuration section has not been defined.");

                // Return the collection.
                ServerHostElement[] items = new ServerHostElement[defaultHost.HostSection.Count];
                defaultHost.HostSection.CopyTo(items, 0);
                serverHostElements = items.Where(q => (q.Name != "localhost")).ToArray();
            }
            catch (Exception)
            {
                throw;
            }

            // Return the host elements.
            return serverHostElements;
        }
    }
}
