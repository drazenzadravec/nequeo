/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.com.au/
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
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Invention;
using Nequeo.Serialisation;
using Nequeo.Xml.Authorisation.Common;
using Nequeo.Extension;

namespace Nequeo.Xml.Authorisation.Configuration
{
    /// <summary>
    /// Configuration communication reader.
    /// </summary>
    public class CommunicationReader
    {
        /// <summary>
        /// The file communication xml path.
        /// </summary>
        public static string CommunicationXmlPath = null;
        private static object _lockObject = new object();

        /// <summary>
        /// Load the communication context data.
        /// </summary>
        /// <returns>The collection of communication data.</returns>
        public static Nequeo.Xml.Authorisation.Communication.Data.context LoadCommunicationData()
        {
            try
            {
                string xmlValidationMessage = string.Empty;

                // Get the xml file location and
                // the xsd file schema.
                string xml = (String.IsNullOrEmpty(CommunicationReader.CommunicationXmlPath) ? Helper.CommunicationXmlPath : CommunicationReader.CommunicationXmlPath);
                string xsd = Nequeo.Xml.Authorisation.Properties.Resources.CommunicationProvider;

                // Validate the filter xml file.
                if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                    throw new Exception("Xml validation. " + xmlValidationMessage);

                // Deserialise the xml file into
                // the log directory list object
                GeneralSerialisation serial = new GeneralSerialisation();
                Nequeo.Xml.Authorisation.Communication.Data.context authData =
                    ((Nequeo.Xml.Authorisation.Communication.Data.context)serial.Deserialise(typeof(Nequeo.Xml.Authorisation.Communication.Data.context), xml));

                // Return the communication data.
                return authData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save the communication context data.
        /// </summary>
        /// <param name="context">The communication data to save.</param>
        public static void SaveCommunicationData(Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            lock (_lockObject)
            {
                try
                {
                    string xmlValidationMessage = string.Empty;

                    // Get the xml file location and
                    // the xsd file schema.
                    string xml = (String.IsNullOrEmpty(CommunicationReader.CommunicationXmlPath) ? Helper.CommunicationXmlPath : CommunicationReader.CommunicationXmlPath);
                    string xsd = Nequeo.Xml.Authorisation.Properties.Resources.CommunicationProvider;

                    // Deserialise the xml file into
                    // the log directory list object
                    GeneralSerialisation serial = new GeneralSerialisation();
                    bool authData = serial.Serialise(context, typeof(Nequeo.Xml.Authorisation.Communication.Data.context), xml);

                    // Validate the filter xml file.
                    if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                        throw new Exception("Xml validation. " + xmlValidationMessage);
                }
                catch { }
            }
        }

        /// <summary>
        /// Save the communication context data.
        /// </summary>
        /// <param name="context">The communication data to save.</param>
        private static async void SaveCommunicationDataAsync(Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            var result = Nequeo.Threading.AsyncOperationResult<int>.
                RunTask(() => SaveCommunicationData(context));

            await result;
        }

        /// <summary>
        /// Add a new port range.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The list of application port names</param>
        /// <param name="portTypeNumber">The list of application port numbers.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool AddPort(string serviceName, 
            string[] portTypeName, int[] portTypeNumber, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all ports with service name and application name.
                Communication.Data.contextPort port = null;

                try
                {
                    // Find the first item.
                    port = context.ports.First(
                        u => (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (port != null)
                {
                    // Get the type list.
                    Communication.Data.contextPortType[] types =
                        port.type.AddIfNotExists<Communication.Data.contextPortType, string, int>
                        (
                            portTypeName,
                            portTypeNumber,
                            (m, u) => m.name.ToLower() == u.ToLower(),
                            (n, p) =>
                            {
                                var portType = new Communication.Data.contextPortType();
                                portType.name = n;
                                portType.number = p;
                                return portType;
                            }
                        );

                    // Assign the port type details.
                    port.type = types;

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                {
                    // Get the type list.
                    Communication.Data.contextPortType[] types = new Communication.Data.contextPortType[portTypeName.Length];

                    // Assign each port detail.
                    for (int i = 0; i < portTypeName.Length; i++)
                    {
                        types[i].name = portTypeName[i];
                        types[i].number = portTypeNumber[i];
                    }

                    // Load all the ports into a temp list.
                    List<Communication.Data.contextPort> tempPorts = new List<Communication.Data.contextPort>(context.ports);
                    Communication.Data.contextPort portData = new Communication.Data.contextPort()
                    {
                        type = types,
                        serviceName = serviceName
                    };

                    // Add the port from the list.
                    tempPorts.Add(portData);

                    // Assign the new port list to the
                    // new context data.
                    context.ports = tempPorts.ToArray();

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Add a new host
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="index">The index number of the host.</param>
        /// <param name="domain">The domain of the host.</param>
        /// <param name="type">The host type.</param>
        /// <param name="context">The communication data.</param>
        /// <param name="activeConnections">The number of active connection on the host.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool AddHost(string hostName, int index, string domain, string type,
            Nequeo.Xml.Authorisation.Communication.Data.context context, int activeConnections = 0)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");
            if (String.IsNullOrEmpty(domain)) throw new ArgumentNullException("domain");
            if (index < 1) throw new IndexOutOfRangeException("A valid index must be supplied.");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    host.index = index;
                    host.domain = domain;
                    host.activeConnections = (activeConnections < 0 ? 0 : activeConnections);
                    host.type = (!String.IsNullOrEmpty(type) ? type : "");

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                {
                    // Load all the hosts into a temp list.
                    List<Communication.Data.contextHost> tempHosts = new List<Communication.Data.contextHost>(context.hosts);
                    Communication.Data.contextHost hostData = new Communication.Data.contextHost()
                    {
                        name = hostName,
                        index = index,
                        domain = domain,
                        activeConnections = (activeConnections < 0 ? 0 : activeConnections),
                        type = (!String.IsNullOrEmpty(type) ? type : ""),
                    };

                    // Add the host from the list.
                    tempHosts.Add(hostData);

                    // Assign the new host list to the
                    // new context data.
                    context.hosts = tempHosts.ToArray();

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Does the host exist.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if the host exists; else false.</returns>
        public static bool HostExists(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Return true the host exists.
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the number of active connections.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The number of active connections; else 0.</returns>
        public static int GetHostActiveConnections(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    return host.activeConnections;
                }

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the host index number.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The index number; else 0.</returns>
        public static int GetHostIndex(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    return host.index;
                }

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the host domain.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The domain; else null.</returns>
        public static string GetHostDomain(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    return host.domain;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove the host.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool RemoveHost(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Find the index of the hosts to remove.
                    context.hosts = context.hosts.Remove(u => u.Equals(host));

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Increments active connections on the host.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        internal static void IncrementHostActiveConnections(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Increment the active connection count.
                    host.activeConnections = host.activeConnections + 1;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Decrements active connections on the host.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        internal static void DecrementHostActiveConnections(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Decrement the active connection count.
                    if (host.activeConnections > 0)
                        host.activeConnections = host.activeConnections - 1;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove the port.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool RemovePort(string serviceName, 
            Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all ports with service name and application name.
                Communication.Data.contextPort port = null;

                try
                {
                    // Find the first item.
                    port = context.ports.First(
                        u => (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (port != null)
                {
                    // Find the index of the ports to remove.
                    context.ports = context.ports.Remove(u => u.Equals(port));

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove all the port ranges.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The application port name.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if the port has been removed; else false.</returns>
        public static bool RemovePort(string serviceName, string portTypeName,
            Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");
            if (String.IsNullOrEmpty(portTypeName)) throw new ArgumentNullException("portTypeName");

            try
            {
                // Find all ports with service name and application name.
                Communication.Data.contextPort port = null;

                try
                {
                    // Find the first item.
                    port = context.ports.First(
                        u => (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (port != null)
                {
                    // Find the index of the port to remove.
                    port.type = port.type.Remove(u => u.name.ToLower().Equals(portTypeName.ToLower()));

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sets the port number.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="portTypeName">The application port name.</param>
        /// <param name="portTypeNumber">The application port number.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if the port has been set; else false.</returns>
        public static bool SetPortNumber(string serviceName, string portTypeName, int portTypeNumber,
            Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");
            if (String.IsNullOrEmpty(portTypeName)) throw new ArgumentNullException("portTypeName");

            try
            {
                // Find all ports with service name and application name.
                Communication.Data.contextPort port = null;

                try
                {
                    // Find the first item.
                    port = context.ports.First(
                        u => (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                // If port exists.
                if (port != null)
                {
                    Communication.Data.contextPortType type = null;

                    try
                    {
                        // Find the index of the port type to set.
                        type = port.type.First(u => u.name.ToLower().Equals(portTypeName.ToLower()));
                    }
                    catch { }

                    // If port type exists.
                    if (type != null)
                    {
                        // Set the port number.
                        type.number = portTypeNumber;

                        // Save the new data.
                        SaveCommunicationDataAsync(context);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the number of active connections.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="activeConnections">The number of active connections on the host.</param>
        /// <param name="context">The communication data.</param>
        public static void SetHostActiveConnections(string hostName, int activeConnections, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    host.activeConnections = (activeConnections < 1 ? 0 : activeConnections);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the host index number.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="index">The host index number.</param>
        /// <param name="context">The communication data.</param>
        public static void SetHostIndex(string hostName, int index, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");
            if (index < 1) throw new IndexOutOfRangeException("A valid index must be supplied.");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    host.index = index;

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the host domain.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="domain">The host domain.</param>
        /// <param name="context">The communication data.</param>
        public static void SetHostDomain(string hostName, string domain, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");
            if (String.IsNullOrEmpty(domain)) throw new ArgumentNullException("domain");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    host.domain = domain;

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the ports.
        /// </summary>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The list of port names and port numbers (name;80); else null.</returns>
        public static string[] GetPorts(string serviceName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            try
            {
                // Find all ports with service name and application name.
                IEnumerable<Communication.Data.contextPort> port = null;
                List<string> ports = new List<string>();

                try
                {
                    // If a service name exists.
                    if (!String.IsNullOrEmpty(serviceName))
                    {
                        // Find the first item.
                        port = context.ports.Where(
                            u => (u.serviceName.ToLower() == serviceName.ToLower()));
                    }
                    else
                    {
                        // Get all.
                        port = context.ports;
                    }
                }
                catch { }

                if (port != null)
                {
                    // Add the port type details.
                    foreach (Communication.Data.contextPort item in port)
                    {
                        foreach (Communication.Data.contextPortType type in item.type)
                            ports.Add(type.name + ";" + type.number.ToString());
                    }
                }

                // Return the result.
                return ((ports != null && ports.Count > 0) ? ports.ToArray() : null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the host type.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The host type; else null.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string GetHostType(string hostName, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    return host.type;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the host type.
        /// </summary>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="type">The host type.</param>
        /// <param name="context">The communication data.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void SetHostType(string hostName, string type, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextHost host = null;

                try
                {
                    host = context.hosts.First(u => u.name.ToLower() == hostName.ToLower());
                }
                catch { }

                if (host != null)
                {
                    // Get the host reference.
                    host.type = (!String.IsNullOrEmpty(type) ? type : "");

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the host details for the identities.
        /// </summary>
        /// <param name="uniqueIdentifiers">The unique client identifiers.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <param name="contextClient">The client service data.</param>
        /// <returns>The host details containing the names (host or "" if not found); else null.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string[] GetHosts(string[] uniqueIdentifiers, string serviceName, 
            Nequeo.Xml.Authorisation.Communication.Data.context context,
            Nequeo.Xml.Authorisation.Communication.Data.contextService contextClient)
        {
            // Validate.
            if (uniqueIdentifiers == null) throw new ArgumentNullException("uniqueIdentifiers");

            try
            {
                List<string> hosts = new List<string>();
                List<string> tempHost = new List<string>();

                // Set up the number of identifiers to find.
                object monitor = new object();
                int numberToFind = uniqueIdentifiers.Length;
                bool[] found = new bool[numberToFind];

                // For each client.
                foreach (Communication.Data.contextServiceClient item in contextClient.clients)
                {
                    int numberFound = 0;

                    // For each unique identifier.
                    for (int i = 0; i < numberToFind; i++)
                    {
                        // If the service name is empty.
                        if (String.IsNullOrEmpty(serviceName))
                        {
                            // If the unique identifier has been found.
                            if ((item.uniqueIdentifier.ToString().ToLower() == uniqueIdentifiers[i].ToLower()))
                            {
                                // Add the server context item.
                                tempHost.Add(item.host);
                                found[i] = true;
                            }
                        }
                        else
                        {
                            // If the unique identifier has been found.
                            if ((item.uniqueIdentifier.ToString().ToLower() == uniqueIdentifiers[i].ToLower()) &&
                                (item.serviceName.ToLower() == serviceName.ToLower()))
                            {
                                // Add the server context item.
                                tempHost.Add(item.host);
                                found[i] = true;
                            }
                        }

                        // If the current identifier
                        // has been found then stop the
                        // search for the current identifier.
                        if (found[i])
                            break;
                    }

                    // Count the number of items found.
                    Parallel.For(0, numberToFind, () => 0, (j, state, local) =>
                    {
                        // If found then increment the count.
                        if (found[j])
                            return local = 1;
                        else
                            return local = 0;

                    }, local =>
                    {
                        // Add one to the count.
                        lock (monitor)
                            numberFound += local;
                    });

                    // If all the machine names have been found
                    // then stop the search.
                    if (numberFound >= numberToFind)
                        break;
                }

                // Construct the hosts
                Parallel.For(0, numberToFind, () => -1, (i, state, local) =>
                {
                    // If found then add the host, else add empty string.
                    if (found[i])
                    {
                        // Found item.
                        return i;
                    }
                    return -1;

                }, local =>
                {
                    // Add one to the count.
                    lock (monitor)
                    {
                        // If a valid index.
                        if (local > -1)
                            hosts.Add(tempHost[local]);
                        else
                            hosts.Add("");
                    }
                });
                
                // If host list exists the host list else null.
                return hosts.Count > 0 ? hosts.ToArray() : null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the host manage URLs for a type.
        /// </summary>
        /// <param name="type">The host type.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The manage URLs; else null.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string[] GetManageURLs(string type, 
            Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(type)) throw new ArgumentNullException("type");

            try
            {
                // Find all host unique identifier.
                Communication.Data.contextManage manage = null;
                List<string> urls = new List<string>();

                try
                {
                    // Get all urls of the type.
                    manage = context.manageURLs.First(u => u.type.ToLower() == type.ToLower());
                }
                catch { }

                // If host exists.
                if (manage != null)
                {
                    // For each host add the url.
                    foreach (Communication.Data.contextManageUrl item in manage.url)
                        urls.Add(item.service);

                    // Return the manage URLs request for the unique identifier.
                    return urls.Count > 0 ? urls.ToArray() : null;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds new manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="urls">The list of service urls.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if the urls have been added; else false.</returns>
        public static bool AddManageURL(string type, string[] urls, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (urls == null) return false;

            try
            {
                // Find all manage with service name and application name.
                Communication.Data.contextManage manage = null;

                try
                {
                    // Get all urls of the type.
                    manage = context.manageURLs.First(u => u.type.ToLower() == type.ToLower());
                }
                catch { }

                // If manage exists.
                if (manage != null)
                {
                    // Get the type list.
                    Communication.Data.contextManageUrl[] manageURLs =
                        manage.url.AddIfNotExists<Communication.Data.contextManageUrl, string>
                        (
                            urls, 
                            (m, u) => m.service.ToLower() == u.ToLower(),
                            a =>
                            {
                                var manageURL = new Communication.Data.contextManageUrl();
                                manageURL.service = a;
                                return manageURL;
                            }
                        );

                    // Assign the manage URL details.
                    manage.url = manageURLs;

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                {
                    // Get the type list.
                    Communication.Data.contextManageUrl[] manageURLs = new Communication.Data.contextManageUrl[urls.Length];

                    // Assign each port detail.
                    for (int i = 0; i < urls.Length; i++)
                    {
                        manageURLs[i].service = urls[i];
                    }

                    // Load all the mangers into a temp list.
                    List<Communication.Data.contextManage> tempManagers = new List<Communication.Data.contextManage>(context.manageURLs);
                    Communication.Data.contextManage manageData = new Communication.Data.contextManage()
                    {
                        url = manageURLs,
                        type = type
                    };

                    // Add the host from the list.
                    tempManagers.Add(manageData);

                    // Assign the new host list to the
                    // new context data.
                    context.manageURLs = tempManagers.ToArray();

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Removes a manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="url">The service url to remove.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if the url have been removed; else false.</returns>
        public static bool RemoveManageURL(string type, string url, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            try
            {
                // Find all manage with service name and application name.
                Communication.Data.contextManage manage = null;

                try
                {
                    // Get all urls of the type.
                    manage = context.manageURLs.First(u => u.type.ToLower() == type.ToLower());
                }
                catch { }

                // If manage exists.
                if (manage != null)
                {
                    // Find the index of the manage to remove.
                    manage.url = manage.url.Remove(u => u.service.ToLower().Equals(url.ToLower()));

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Removes all manage URLs.
        /// </summary>
        /// <param name="type">The manage type.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if the urls have been removed; else false.</returns>
        public static bool RemoveManageURL(string type, Nequeo.Xml.Authorisation.Communication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(type)) throw new ArgumentNullException("type");

            try
            {
                // Find all manage with service name and application name.
                Communication.Data.contextManage manage = null;

                try
                {
                    // Get all urls of the type.
                    manage = context.manageURLs.First(u => u.type.ToLower() == type.ToLower());
                }
                catch { }

                // If manage exists.
                if (manage != null)
                {
                    // Find the index of the manage to remove.
                    context.manageURLs = context.manageURLs.Remove(u => u.Equals(manage));

                    // Save the new data.
                    SaveCommunicationDataAsync(context);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
