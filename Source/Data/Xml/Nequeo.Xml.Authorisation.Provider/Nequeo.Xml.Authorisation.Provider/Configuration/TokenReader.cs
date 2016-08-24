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
    /// Configuration token reader.
    /// </summary>
    public class TokenReader
    {
        /// <summary>
        /// The file communication xml path.
        /// </summary>
        public static string TokenXmlPath = null;
        private static object _lockObject = new object();

        /// <summary>
        /// Load the token context data.
        /// </summary>
        /// <returns>The collection of token data.</returns>
        public static Nequeo.Xml.Authorisation.Token.Data.context LoadTokenData()
        {
            try
            {
                string xmlValidationMessage = string.Empty;

                // Get the xml file location and
                // the xsd file schema.
                string xml = (String.IsNullOrEmpty(TokenReader.TokenXmlPath) ? Helper.TokenXmlPath : TokenReader.TokenXmlPath);
                string xsd = Nequeo.Xml.Authorisation.Properties.Resources.TokenProvider;

                // Validate the filter xml file.
                if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                    throw new Exception("Xml validation. " + xmlValidationMessage);

                // Deserialise the xml file into
                // the log directory list object
                GeneralSerialisation serial = new GeneralSerialisation();
                Nequeo.Xml.Authorisation.Token.Data.context authData =
                    ((Nequeo.Xml.Authorisation.Token.Data.context)serial.Deserialise(typeof(Nequeo.Xml.Authorisation.Token.Data.context), xml));

                // Return the communication data.
                return authData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save the token context data.
        /// </summary>
        /// <param name="context">The token data to save.</param>
        public static void SaveTokenData(Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            lock (_lockObject)
            {
                try
                {
                    string xmlValidationMessage = string.Empty;

                    // Get the xml file location and
                    // the xsd file schema.
                    string xml = (String.IsNullOrEmpty(TokenReader.TokenXmlPath) ? Helper.TokenXmlPath : TokenReader.TokenXmlPath);
                    string xsd = Nequeo.Xml.Authorisation.Properties.Resources.TokenProvider;

                    // Deserialise the xml file into
                    // the log directory list object
                    GeneralSerialisation serial = new GeneralSerialisation();
                    bool authData = serial.Serialise(context, typeof(Nequeo.Xml.Authorisation.Token.Data.context), xml);

                    // Validate the filter xml file.
                    if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                        throw new Exception("Xml validation. " + xmlValidationMessage);
                }
                catch { }
            }
        }

        /// <summary>
        /// Save the token context data.
        /// </summary>
        /// <param name="context">The token data to save.</param>
        private static async void SaveTokenDataAsync(Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            var result = Nequeo.Threading.AsyncOperationResult<int>.
                RunTask(() => SaveTokenData(context));

            await result;
        }

        /// <summary>
        /// Create a new token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The token; else null.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string Create(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all host unique identifier.
                Token.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                             (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    client.dateAdded = DateTime.Now;
                    client.token = Guid.NewGuid().ToString();

                    // Save the new data.
                    SaveTokenDataAsync(context);
                    return client.token;
                }
                else
                {
                    // Load all the clients into a temp list.
                    List<Token.Data.contextClient> tempClients = new List<Token.Data.contextClient>(context.clients);
                    Token.Data.contextClient clientData = new Token.Data.contextClient()
                    {
                        uniqueIdentifier = Int32.Parse(uniqueIdentifier),
                        serviceName = serviceName,
                        dateAdded = DateTime.Now,
                        token = Guid.NewGuid().ToString(),
                        permission = Nequeo.Security.PermissionType.None.ToString()
                    };

                    // Add the client from the list.
                    tempClients.Add(clientData);

                    // Assign the new client list to the
                    // new context data.
                    context.clients = tempClients.ToArray();
                    return clientData.token;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if deleted; else false.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool Delete(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all host unique identifier.
                Token.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                             (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Find the index of the client to remove.
                    context.clients = context.clients.Remove(u => u.Equals(client));
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
        /// Get the current token.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The token; else null.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string Get(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all host unique identifier.
                Token.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                             (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    return client.token;
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
        /// Update the token
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="token">The token that will replace the cuurent token.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if updated; else false.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool Update(string uniqueIdentifier, string serviceName, string token, Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");
            if (String.IsNullOrEmpty(token)) throw new ArgumentNullException("token");

            try
            {
                // Find all host unique identifier.
                Token.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                             (u.serviceName.ToLower() == serviceName.ToLower()) &&
                             (u.token.ToLower() == token.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    client.dateAdded = DateTime.Now;
                    client.token = Guid.NewGuid().ToString();

                    // Save the new data.
                    SaveTokenDataAsync(context);
                    return true;
                }
                else
                {
                    // Load all the clients into a temp list.
                    List<Token.Data.contextClient> tempClients = new List<Token.Data.contextClient>(context.clients);
                    Token.Data.contextClient clientData = new Token.Data.contextClient()
                    {
                        uniqueIdentifier = Int32.Parse(uniqueIdentifier),
                        serviceName = serviceName,
                        dateAdded = DateTime.Now,
                        token = Guid.NewGuid().ToString(),
                        permission = Nequeo.Security.PermissionType.None.ToString()
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
        /// Is the token valid
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="token">The token to validate.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>True if valid; else false.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsValid(string uniqueIdentifier, string serviceName, string token, Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");
            if (String.IsNullOrEmpty(token)) throw new ArgumentNullException("token");

            try
            {
                // Find all host unique identifier.
                Token.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                             (u.serviceName.ToLower() == serviceName.ToLower()) &&
                             (u.token.ToLower() == token.ToLower()));
                }
                catch { }

                if (client != null)
                {
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
        /// Get the client permission.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="serviceName">The service name the client is connected to.</param>
        /// <param name="context">The communication data.</param>
        /// <returns>The permission.</returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static Nequeo.Security.IPermission GetPermission(string uniqueIdentifier, string serviceName, Nequeo.Xml.Authorisation.Token.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

            try
            {
                // Find all host unique identifier.
                Token.Data.contextClient client = null;
               
                try
                {
                    client = context.clients.First(
                        u => (u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower()) &&
                             (u.serviceName.ToLower() == serviceName.ToLower()));
                }
                catch { }

                if (client != null)
                {
                    return new Nequeo.Security.PermissionSource(
                        (Nequeo.Security.PermissionType)Enum.Parse(typeof(Nequeo.Security.PermissionType), client.permission));
                }
                else
                    return new Nequeo.Security.PermissionSource(Security.PermissionType.None);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
