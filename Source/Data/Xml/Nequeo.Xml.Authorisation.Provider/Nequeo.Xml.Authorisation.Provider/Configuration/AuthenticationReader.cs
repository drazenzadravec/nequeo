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
using Nequeo.Data.Enum;
using Nequeo.Data;
using Nequeo.Data.Provider;

namespace Nequeo.Xml.Authorisation.Configuration
{
    /// <summary>
    /// Configuration authentication reader.
    /// </summary>
    public class AuthenticationReader
    {
        /// <summary>
        /// The file authentication xml path.
        /// </summary>
        public static string AuthenticationXmlPath = null;
        private static object _lockObject = new object();

        /// <summary>
        /// Load the authentication context data.
        /// </summary>
        /// <returns>The collection of authentication data.</returns>
        public static Nequeo.Xml.Authorisation.Authentication.Data.context LoadAuthenticationData()
        {
            try
            {
                string xmlValidationMessage = string.Empty;

                // Get the xml file location and
                // the xsd file schema.
                string xml = (String.IsNullOrEmpty(AuthenticationReader.AuthenticationXmlPath) ? Helper.AuthenticationXmlPath : AuthenticationReader.AuthenticationXmlPath);
                string xsd = Nequeo.Xml.Authorisation.Properties.Resources.AuthenticationProvider;

                // Validate the filter xml file.
                if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                    throw new Exception("Xml validation. " + xmlValidationMessage);

                // Deserialise the xml file into
                // the log directory list object
                GeneralSerialisation serial = new GeneralSerialisation();
                Nequeo.Xml.Authorisation.Authentication.Data.context authData =
                    ((Nequeo.Xml.Authorisation.Authentication.Data.context)serial.Deserialise(typeof(Nequeo.Xml.Authorisation.Authentication.Data.context), xml));

                // Return the authentication data.
                return authData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save the authentication context data.
        /// </summary>
        /// <param name="context">The authentication data to save.</param>
        public static void SaveAuthenticationData(Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            lock (_lockObject)
            {
                try
                {
                    string xmlValidationMessage = string.Empty;

                    // Get the xml file location and
                    // the xsd file schema.
                    string xml = (String.IsNullOrEmpty(AuthenticationReader.AuthenticationXmlPath) ? Helper.AuthenticationXmlPath : AuthenticationReader.AuthenticationXmlPath);
                    string xsd = Nequeo.Xml.Authorisation.Properties.Resources.AuthenticationProvider;

                    // Deserialise the xml file into
                    // the log directory list object
                    GeneralSerialisation serial = new GeneralSerialisation();
                    bool authData = serial.Serialise(context, typeof(Nequeo.Xml.Authorisation.Authentication.Data.context), xml);

                    // Validate the filter xml file.
                    if (!Validation.IsXmlValidEx(xsd, xml, out xmlValidationMessage))
                        throw new Exception("Xml validation. " + xmlValidationMessage);
                }
                catch { }
            }
        }

        /// <summary>
        /// Save the authentication context data async.
        /// </summary>
        /// <param name="context">The authentication data to save.</param>
        private static async void SaveAuthenticationDataAsync(Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            var result = Nequeo.Threading.AsyncOperationResult<int>.
                RunTask(() => SaveAuthenticationData(context));

            await result;
        }

        /// <summary>
        /// Does the client exist.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>True if the client exists; else false.</returns>
        public static bool ClientExists(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
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
        /// Get client contacts.
        /// </summary>
        /// <param name="uniqueIdentifier">Client unique identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The collection of client contacts; else null.</returns>
        public static string[] GetClientContacts(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Return the list of contacts.
                    return client.Value.Split(';');
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all client contacts that are online and available.
        /// </summary>
        /// <param name="uniqueIdentifier">Client unique identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The collection of client contacts; else null.</returns>
        public static string[] GetClientContactsOnline(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Return the list of contacts.
                string[] contacts = GetClientContacts(uniqueIdentifier, context);

                // If contacts have been found.
                if (contacts != null && contacts.Length > 0)
                {
                    List<string> onlineContacts = new List<string>();

                    // Set up the number of identifiers to find.
                    object monitor = new object();
                    int numberToFind = contacts.Length;
                    bool[] found = new bool[numberToFind];

                    // For each client.
                    foreach (Authentication.Data.contextClient item in context.clients)
                    {
                        int numberFound = 0;

                        // For each unique identifier.
                        for (int i = 0; i < numberToFind; i++)
                        {
                            // If the unique identifier has been found.
                            if ((item.uniqueIdentifier.ToString().ToLower() == contacts[i].ToLower()) &&
                                (item.onlineStatus.ToLower() == OnlineStatus.Available.ToString().ToLower()) &&
                                (item.isLoggedOn))
                            {
                                // Add the server context item.
                                onlineContacts.Add(item.uniqueIdentifier.ToString());
                                found[i] = true;
                            }

                            // If the current identifier
                            // has been found the stop the
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

                    // Return the collection; else return null.
                    return (onlineContacts.Count > 0 ? onlineContacts.ToArray() : null);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all the client contacts that are online and logged on.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The list of unique client contact identifiers; else null.</returns>
        public static string[] GetClientContactsLoggedOn(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Return the list of contacts.
                string[] contacts = GetClientContacts(uniqueIdentifier, context);

                // If contacts have been found.
                if (contacts != null && contacts.Length > 0)
                {
                    List<string> onlineContacts = new List<string>();

                    // Set up the number of identifiers to find.
                    object monitor = new object();
                    int numberToFind = contacts.Length;
                    bool[] found = new bool[numberToFind];

                    // For each client.
                    foreach (Authentication.Data.contextClient item in context.clients)
                    {
                        int numberFound = 0;

                        // For each unique identifier.
                        for (int i = 0; i < numberToFind; i++)
                        {
                            // If the unique identifier has been found.
                            if ((item.uniqueIdentifier.ToString().ToLower() == contacts[i].ToLower()) &&
                                (item.isLoggedOn))
                            {
                                // Add the server context item.
                                onlineContacts.Add(item.uniqueIdentifier.ToString());
                                found[i] = true;
                            }

                            // If the current identifier
                            // has been found the stop the
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

                    // Return the collection; else return null.
                    return (onlineContacts.Count > 0 ? onlineContacts.ToArray() : null);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Add a client contact.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="uniqueContactIdentifier">The unique client contact identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>True if added contact; else false.</returns>
        public static bool AddClientContact(string uniqueIdentifier, string uniqueContactIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(uniqueContactIdentifier)) throw new ArgumentNullException("uniqueContactIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Get the current contact list and
                    // add the new contact.
                    string contactList = client.Value;
                    string newContactList = contactList + ";" + uniqueContactIdentifier.Trim();

                    // Assign the new contact list to the client.
                    client.Value = newContactList;

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);

                    // Return success.
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
        /// Remove a contact from this client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="uniqueContactIdentifier">The unique client contact identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>True if removed; else false.</returns>
        public static bool RemoveClientContact(string uniqueIdentifier, string uniqueContactIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(uniqueContactIdentifier)) throw new ArgumentNullException("uniqueContactIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Return the list of contacts.
                    object monitor = new object();
                    string[] contacts = client.Value.Split(';');
                    StringBuilder newContactList = new StringBuilder();

                    // Construct the contacts
                    Parallel.For(0, contacts.Length, () => -1, (i, state, local) =>
                    {
                        // If the current contact is not the contact identifier
                        // that is to be removed.
                        if (contacts[i].ToLower() != uniqueContactIdentifier.ToLower())
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
                                // Construct the new contact list.
                                newContactList.Append(contacts[local] + ";");
                        }
                    });

                    // Assign the new contact list to the client.
                    client.Value = newContactList.ToString().TrimEnd(';');

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);

                    // Return success.
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
        /// Find a person that could potentally be a contact.
        /// </summary>
        /// <param name="query">The query data to search for.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The unique client identifiers and client name (1;Name); else null.</returns>
        public static string[] FindPerson(string query, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(query)) throw new ArgumentNullException("query");

            try
            {
                List<string> people = new List<string>();

                // For each client.
                foreach (Authentication.Data.contextClient item in context.clients)
                {
                    // If the query has been found.
                    if ((item.name.ToLower().Contains(query.ToLower())) || (item.emailAddress.ToLower().Contains(query.ToLower())))
                    {
                        // Add the person.
                        people.Add(item.uniqueIdentifier.ToString() + ";" + item.name);
                    }
                }

                // Return the collection; else return null.
                return (people.Count > 0 ? people.ToArray() : null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the client online status.
        /// </summary>
        /// <param name="uniqueIdentifier">Client unique identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The online status; else Invisible.</returns>
        public static OnlineStatus GetClientOnlineStatus(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Select the status.
                    switch (client.onlineStatus.ToLower())
                    {
                        case "available":
                            return OnlineStatus.Available;

                        case "invisible":
                        default:
                            return OnlineStatus.Invisible;
                    }
                }

                // Return invisible.
                return OnlineStatus.Invisible;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Is the client contact online and available.
        /// </summary>
        /// <param name="uniqueIdentifier">Client unique identifier.</param>
        /// <param name="uniqueContactIdentifier"></param>
        /// <param name="context">The authentication data.</param>
        /// <returns>True if the client contact is online; else false.</returns>
        public static bool IsClientContactOnline(string uniqueIdentifier, string uniqueContactIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(uniqueContactIdentifier)) throw new ArgumentNullException("uniqueContactIdentifier");

            try
            {
                // Return the list of contacts.
                string[] contacts = null;
                bool isContact = false;
                bool foundUniqueIdentifier = false;
                bool foundUniqueContactIdentifier = false;
                string onlineStatusUniqueContactIdentifier = string.Empty;

                // For each client.
                foreach (Authentication.Data.contextClient item in context.clients)
                {
                    // Find the unique contact identifier.
                    if (item.uniqueIdentifier.ToString().ToLower() == uniqueContactIdentifier.ToLower())
                    {
                        // Found the contact.
                        foundUniqueContactIdentifier = true;
                        onlineStatusUniqueContactIdentifier = item.onlineStatus;
                    }

                    // Find the unique identifier.
                    if (item.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower())
                    {
                        // Return the list of contacts.
                        contacts = item.Value.Split(';');
                        foundUniqueIdentifier = true;
                    }

                    // If both have been found.
                    if (foundUniqueIdentifier && foundUniqueContactIdentifier)
                    {
                        // Count the number of contacts for the client.
                        int count = contacts.Count(u => u.ToLower().Contains(uniqueContactIdentifier.ToLower()));

                        // If the client has contact end search.
                        if (count > 0)
                            isContact = true;

                        // End the search.
                        break;
                    }
                }

                // If contact found.
                if (foundUniqueIdentifier && foundUniqueContactIdentifier && isContact)
                {
                    // Select the status.
                    switch (onlineStatusUniqueContactIdentifier.ToLower())
                    {
                        case "available":
                            return true;

                        case "invisible":
                        default:
                            return false;
                    }
                }
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Is the client currently suspended, which means that the credentials are valid but the client can not log-in.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>True if suspended; else false.</returns>
        public static bool IsClientSuspended(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    return client.isSuspended;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Is the client currently logged-on.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>True if logged-on; else false.</returns>
        public static bool IsClientLoggedOn(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Get the client reference.
                    return client.isLoggedOn;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client logged-on state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="loggedOn">True if logged-on, false if logged-off.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientLoggedOnState(string uniqueIdentifier, bool loggedOn, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the new logged-on state for the client.
                    client.isLoggedOn = loggedOn;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client suspended state.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="suspended">True if suspended, false if not suspended.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientSuspendedState(string uniqueIdentifier, bool suspended, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the new suspended state for the client.
                    client.isSuspended = suspended;

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the online status of the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="status">The status of the client.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientOnlineStatus(string uniqueIdentifier, OnlineStatus status, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Set the client online status.
                    switch (status)
                    {
                        case OnlineStatus.Available:
                            client.onlineStatus = "Available";
                            break;

                        case OnlineStatus.Invisible:
                        default:
                            // At most there is one connection.
                            if (client.activeConnections < 2)
                                client.onlineStatus = "Invisible";
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Valiadate the user credentials.
        /// </summary>
        /// <param name="username">The client user name.</param>
        /// <param name="password">The client password.</param>
        /// <param name="applicationName">The application name the client belongs to.</param>
        /// <param name="domain">The client domain.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The unique client identifier (could be any uniqie type and indicator); else null.</returns>
        public static string ValidateUser(string username, string password, string applicationName, string domain, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (password == null) throw new ArgumentNullException("password");
            if (applicationName == null) throw new ArgumentNullException("applicationName");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u =>
                        (u.username.ToString().ToLower() == username.ToLower()) &&
                        (u.password == password) &&
                        ((u.applicationName.ToLower() == applicationName.ToLower()) || (u.applicationName.ToLower() == "All".ToLower())));
                }
                catch { }
                
                if (client != null)
                {
                    // Get the client reference.
                    client.isLoggedOn = true;
                    client.activeConnections = client.activeConnections + 1;
                    return client.uniqueIdentifier.ToString();
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="name">The client full name.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientName(string uniqueIdentifier, string name, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the new name for the client.
                    client.name = name;

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client email address.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="emailAddress">The client email address.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientEmailAddress(string uniqueIdentifier, string emailAddress, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the new emailAddress for the client.
                    client.emailAddress = (String.IsNullOrEmpty(emailAddress) ? "" : emailAddress);

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client username.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="username">The client username.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientUsername(string uniqueIdentifier, string username, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (String.IsNullOrEmpty(username)) throw new ArgumentNullException("username");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the new username for the client.
                    client.username = username;

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client password.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="password">The client password.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientPassword(string uniqueIdentifier, string password, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the new password for the client.
                    client.password = (String.IsNullOrEmpty(password) ? "" : password); ;

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client active connections count for the same client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="activeConnections">The client active connections count.</param>
        /// <param name="context">The authentication data.</param>
        public static void SetClientActiveConnections(string uniqueIdentifier, int activeConnections, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");
            if (activeConnections < 0) throw new IndexOutOfRangeException("Active connections must be positive.");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the active connections
                    client.activeConnections = (activeConnections < 1 ? 0 : activeConnections);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the client application name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <param name="applicationName">The client application name (All - indicates all applications).</param>
        public static void SetClientApplicationName(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context, string applicationName = "All")
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Assign the new applicationName for the client.
                    client.applicationName = (String.IsNullOrEmpty(applicationName) ? "All" : applicationName);

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove an existing client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool RemoveClient(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Find the index of the client to remove.
                    context.clients = context.clients.Remove(u => u.Equals(client));
                   
                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
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
        /// Add a new client.
        /// </summary>
        /// <param name="name">The full client name.</param>
        /// <param name="emailAddress">The client email address.</param>
        /// <param name="username">The logon username.</param>
        /// <param name="password">The password username.</param>
        /// <param name="context">The authentication data.</param>
        /// <param name="applicationName">The application name the client will belong to (All - indicates all applications).</param>
        /// <param name="clientContacts">The client contact list, unique identifiers.</param>
        /// <returns>The object containing the new data; else null.</returns>
        public static bool AddClient(string name, string emailAddress, string username,
            string password, Nequeo.Xml.Authorisation.Authentication.Data.context context, string applicationName = "All", string[] clientContacts = null)
        {
            // Validate.
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty(username)) throw new ArgumentNullException("username");

            try
            {
                // Setup a non null application name.
                string applicationNameSearch = (String.IsNullOrEmpty(applicationName) ? "All" : applicationName);

                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u =>
                        (u.name.ToLower() == name.ToLower()) &&
                        (u.username.ToLower() == username.ToLower()) &&
                        ((u.applicationName.ToLower() == applicationNameSearch.ToLower()) || (u.applicationName.ToLower() == "All".ToLower())));
                }
                catch { }
                
                // If the client exits then update.
                if (client != null)
                {
                    // Get the client reference.
                    client.emailAddress = (String.IsNullOrEmpty(emailAddress) ? "" : emailAddress);
                    client.password = (String.IsNullOrEmpty(password) ? "" : password);
                    client.Value = (clientContacts == null || clientContacts.Length < 1) ? "" : String.Join(";", clientContacts);

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                    return true;
                }
                else
                {
                    // Only one thread at a time can add clients
                    // because the unique identifier must be incremented.
                    lock (_lockObject)
                    {
                        // Get the maximum unique identifier;
                        int uniqueIdentifierMax = context.clients.Max(u => u.uniqueIdentifier);
                        int uniqueIdentifierMaxPlusOne = uniqueIdentifierMax + 1;

                        // Load all the clients into a temp list.
                        List<Authentication.Data.contextClient> tempClients = new List<Authentication.Data.contextClient>(context.clients);
                        Authentication.Data.contextClient clientData = new Authentication.Data.contextClient()
                        {
                            uniqueIdentifier = uniqueIdentifierMaxPlusOne,
                            name = name,
                            emailAddress = (String.IsNullOrEmpty(emailAddress) ? "" : emailAddress),
                            username = username,
                            password = (String.IsNullOrEmpty(password) ? "" : password),
                            applicationName = (String.IsNullOrEmpty(applicationName) ? "All" : applicationName),
                            isLoggedOn = false,
                            isSuspended = false,
                            onlineStatus = OnlineStatus.Invisible.ToString(),
                            activeConnections = 0,
                            Value = (clientContacts == null || clientContacts.Length < 1) ? "" : String.Join(";", clientContacts)
                        };

                        // Add the client from the list.
                        tempClients.Add(clientData);

                        // Assign the new client list to the
                        // new context data.
                        context.clients = tempClients.ToArray();
                    }

                    // Save the new data.
                    SaveAuthenticationDataAsync(context);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the client name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The client full name.</returns>
        public static string GetClientName(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Return the list of contacts.
                    return client.name;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the client email address.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The client email address.</returns>
        public static string GetClientEmailAddress(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Return the list of contacts.
                    return client.emailAddress;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the client username.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The client username.</returns>
        public static string GetClientUsername(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Return the list of contacts.
                    return client.username;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the client application name.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The client application name.</returns>
        public static string GetClientApplicationName(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Return the list of contacts.
                    return client.applicationName;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the client active connections count.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The client active connections count.</returns>
        public static int GetClientActiveConnections(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // Return the list of contacts.
                    return client.activeConnections;
                }

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Logoff the client.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique client identifier.</param>
        /// <param name="context">The authentication data.</param>
        /// <returns>The number of active connections; else 0.</returns>
        public static int LogOff(string uniqueIdentifier, Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            // Validate.
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient client = null;

                try
                {
                    client = context.clients.First(u => u.uniqueIdentifier.ToString().ToLower() == uniqueIdentifier.ToLower());
                }
                catch { }

                if (client != null)
                {
                    // At most there is one connection.
                    if (client.activeConnections < 2)
                    {
                        // Logoff the client.
                        client.isLoggedOn = false;
                        client.activeConnections = 0;
                        client.onlineStatus = OnlineStatus.Invisible.ToString();
                    }
                    else
                    {
                        // Decrement the active connection count.
                        client.activeConnections = client.activeConnections - 1;
                    }

                    // Return the number of active client connections.
                    return client.activeConnections;
                }

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all the clients.
        /// </summary>
        /// <param name="context">The authentication data.</param>
        /// <returns>The list of unique client contact identifiers; else null.</returns>
        public static string[] GetClients(Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            try
            {
                // Find all clients with unique identifier.
                Authentication.Data.contextClient[] clients = null;
                List<string> clientCol = new List<string>();

                try
                {
                    // Get all the clients.
                    clients = context.clients;
                }
                catch { }

                if (clients != null)
                {
                    // For each client.
                    foreach (Authentication.Data.contextClient item in clients)
                        clientCol.Add(item.uniqueIdentifier.ToString());

                    // Return the list of clients.
                    return clientCol.Count > 0 ? clientCol.ToArray() : null;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all the clients online.
        /// </summary>
        /// <param name="context">The authentication data.</param>
        /// <returns>The list of unique client contact identifiers; else null.</returns>
        public static string[] GetClientsOnline(Nequeo.Xml.Authorisation.Authentication.Data.context context)
        {
            try
            {
                // Find all clients with unique identifier.
                IEnumerable<Authentication.Data.contextClient> clients = null;
                List<string> clientCol = new List<string>();

                try
                {
                    clients = context.clients.Where(u => 
                        (u.isLoggedOn == true) && 
                        (u.onlineStatus.ToLower() == OnlineStatus.Available.ToString().ToLower()));
                }
                catch { }

                if (clients != null)
                {
                    // For each client.
                    foreach (Authentication.Data.contextClient item in clients)
                        clientCol.Add(item.uniqueIdentifier.ToString());

                    // Return the list of clients.
                    return clientCol.Count > 0 ? clientCol.ToArray() : null;
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
