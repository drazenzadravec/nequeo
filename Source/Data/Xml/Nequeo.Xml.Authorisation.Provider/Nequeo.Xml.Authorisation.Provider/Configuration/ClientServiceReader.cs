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
    /// Configuration client service reader.
    /// </summary>
    public class ClientServiceReader
    {
        /// <summary>
        /// The file client service xml path.
        /// </summary>
        public static string ClientServiceXmlPath = null;
        private static object _lockObject = new object();

        /// <summary>
        /// Load the client service context data.
        /// </summary>
        /// <returns>The collection of communication data.</returns>
        public static Nequeo.Xml.Authorisation.Communication.Data.contextService LoadClientServiceData()
        {
            try
            {
                string xmlValidationMessage = string.Empty;

                // Get the xml file location and
                // the xsd file schema.
                string xml = (String.IsNullOrEmpty(ClientServiceReader.ClientServiceXmlPath) ? Helper.ClientServiceXmlPath : ClientServiceReader.ClientServiceXmlPath);
                string xsd = Nequeo.Xml.Authorisation.Properties.Resources.ClientServiceProvider;

                // Validate the filter xml file.
                if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                    throw new Exception("Xml validation. " + xmlValidationMessage);

                // Deserialise the xml file into
                // the log directory list object
                GeneralSerialisation serial = new GeneralSerialisation();
                Nequeo.Xml.Authorisation.Communication.Data.contextService authData =
                    ((Nequeo.Xml.Authorisation.Communication.Data.contextService)serial.Deserialise(typeof(Nequeo.Xml.Authorisation.Communication.Data.contextService), xml));

                // Return the communication data.
                return authData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save the client service context data.
        /// </summary>
        /// <param name="context">The client service data to save.</param>
        public static void SaveClientServiceData(Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            lock (_lockObject)
            {
                try
                {
                    string xmlValidationMessage = string.Empty;

                    // Get the xml file location and
                    // the xsd file schema.
                    string xml = (String.IsNullOrEmpty(ClientServiceReader.ClientServiceXmlPath) ? Helper.ClientServiceXmlPath : ClientServiceReader.ClientServiceXmlPath);
                    string xsd = Nequeo.Xml.Authorisation.Properties.Resources.ClientServiceProvider;

                    // Deserialise the xml file into
                    // the log directory list object
                    GeneralSerialisation serial = new GeneralSerialisation();
                    bool authData = serial.Serialise(context, typeof(Nequeo.Xml.Authorisation.Communication.Data.contextService), xml);

                    // Validate the filter xml file.
                    if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                        throw new Exception("Xml validation. " + xmlValidationMessage);
                }
                catch { }
            }
        }

        /// <summary>
        /// Save the client service context data.
        /// </summary>
        /// <param name="context">The client service data to save.</param>
        private static async void SaveClientServiceDataAsync(Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            var result = Nequeo.Threading.AsyncOperationResult<int>.
                RunTask(() => SaveClientServiceData(context));

            await result;
        }

        /// <summary>
        /// Add a new client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The host machine the client must connect to.</param>
        /// <param name="context">The communication data.</param>
        /// <param name="active">Is the communication currenlty active.</param>
        /// <param name="communicationToken">The common token used for communication.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool AddClient(string uniqueIdentifier, string serviceName, string hostName,
            Nequeo.Xml.Authorisation.Communication.Data.contextService context, bool active = false, string communicationToken = "")
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                             (u.serviceName.ToLower() == serviceName.ToLower()) &&
                             (u.host.ToLower() == hostName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    client.active = active;
                    client.dateAdded = DateTime.Now;
                    client.Value = (String.IsNullOrEmpty(communicationToken) ? "" : communicationToken);

                    // Save the new data.
                    SaveClientServiceDataAsync(context);
                    return true;
                }
                else
                {
                    // Load all the clients into a temp list.
                    List<Communication.Data.contextServiceClient> tempClients = new List<Communication.Data.contextServiceClient>(context.clients);
                    Communication.Data.contextServiceClient clientData = new Communication.Data.contextServiceClient()
                    {
                        uniqueIdentifier = Int32.Parse(uniqueIdentifier),
                        host = hostName,
                        serviceName = serviceName,
                        active = active,
                        dateAdded = DateTime.Now,
                        Value = (String.IsNullOrEmpty(communicationToken) ? "" : communicationToken)
                    };

                    // Add the client from the list.
                    tempClients.Add(clientData);

                    // Assign the new client list to the
                    // new context data.
                    context.clients = tempClients.ToArray();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Does the client exist.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if the client exists; else false.</returns>
        public static bool ClientExists(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Return true the client exists.
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
        /// Get the communication active state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if active; else false.</returns>
        public static bool GetClientActive(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    return client.active;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the communication token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The communication token; else null.</returns>
        public static string GetClientCommunicationToken(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    return client.Value;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the client host name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The client host name; else null.</returns>
        public static string GetClientHost(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    return client.host;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="activeConnections">The number of active connection using this client.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool RemoveClient(string uniqueIdentifier, string serviceName, int activeConnections,
            Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // If there is one or less active client connections
                    // then remove the client.
                    if (activeConnections < 2)
                    {
                        // Find the index of the client to remove.
                        context.clients = context.clients.Remove(u => u.Equals(client));
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
        /// Set the client communication active state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="active">The communication active state.</param>
        /// <param name="context">The communication data.</param>
        public static void SetClientActive(string uniqueIdentifier, string serviceName, bool active, Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    client.active = active;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the communication token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="communicationToken">The client communication token.</param>
        /// <param name="context">The communication data.</param>
        public static void SetClientCommunicationToken(string uniqueIdentifier, string serviceName, string communicationToken,
            Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    client.Value = (String.IsNullOrEmpty(communicationToken) ? "" : communicationToken);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client host name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="hostName">The unique host name.</param>
        /// <param name="context">The communication data.</param>
        public static void SetClientHost(string uniqueIdentifier, string serviceName, string hostName, 
            Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");
            if (String.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    client.host = hostName;
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
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The host details containing the name (host); else null.</returns>
        public static string GetHost(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Communication.Data.contextService context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all clients with unique identifier.
                Communication.Data.contextServiceClient client = null;

                try
                {
                    // Find the first item.
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                            (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the host.
                    string hostName = client.host;

                    // Return the host information.
                    return hostName;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
