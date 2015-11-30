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
using System.Threading.Tasks;

using Nequeo.Extension;
using Nequeo.Data.Enum;
using Nequeo.Data.Provider;

namespace Nequeo.Server
{
    /// <summary>
    /// Interaction sorted binary search manager. Stores all interaction clients and
    /// managers the searching of context items within the current machine.
    /// </summary>
    /// <remarks>This object is thread-safe.</remarks>
    public class InteractionManager : IDisposable
    {
        /// <summary>
        /// Interaction sorted binary search manager. Stores all interaction clients and
        /// managers the searching of context items within the current machine.
        /// </summary>
        public InteractionManager()
        {
            _clientList = new SortedDictionary<string, Nequeo.Client.Manage.IInteractionClient>();
            _activeList = new SortedDictionary<string, ActiveConnectionModel>();
        }

        private object _lockClientObject = new object();
        private object _lockActiveObject = new object();

        SortedDictionary<string, Nequeo.Client.Manage.IInteractionClient> _clientList = null;
        SortedDictionary<string, ActiveConnectionModel> _activeList = null;

        /// <summary>
        /// Add the client item.
        /// </summary>
        /// <param name="machineName">The name of the machine the client is connected to.</param>
        /// <param name="client">The client connected to the machine.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void AddClient(string machineName, Nequeo.Client.Manage.IInteractionClient client)
        {
            if (String.IsNullOrEmpty(machineName)) throw new ArgumentNullException("machineName");
            if (client == null) throw new ArgumentNullException("client");

            lock (_lockClientObject)
            {
                // Add the item.
                _clientList.Add(machineName, client);
            }
        }

        /// <summary>
        /// Remove the client item.
        /// </summary>
        /// <param name="machineName">The name of the machine the client is connected to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void RemoveClient(string machineName)
        {
            if (String.IsNullOrEmpty(machineName)) throw new ArgumentNullException("machineName");

            lock (_lockClientObject)
            {
                // Find the client.
                Nequeo.Client.Manage.IInteractionClient client = FindClient(machineName);
                if (client != null)
                {
                    // Close the connection.
                    client.Close();
                    client.Dispose();
                }

                // Remove the item.
                _clientList.Remove(machineName);
            }
        }

        /// <summary>
        /// Add the active model.
        /// </summary>
        /// <param name="machineName">The name of the machine the client is connected to.</param>
        /// <param name="model">The active connection model.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void AddActive(string machineName, ActiveConnectionModel model)
        {
            if (String.IsNullOrEmpty(machineName)) throw new ArgumentNullException("machineName");
            if (model == null) throw new ArgumentNullException("model");

            lock (_lockActiveObject)
            {
                // Add the item.
                _activeList.Add(machineName, model);
            }
        }

        /// <summary>
        /// Remove the active model.
        /// </summary>
        /// <param name="machineName">The name of the machine the client is connected to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void RemoveActive(string machineName)
        {
            if (String.IsNullOrEmpty(machineName)) throw new ArgumentNullException("machineName");

            lock (_lockActiveObject)
            {
                // Remove the item.
                _activeList.Remove(machineName);
            }
        }

        /// <summary>
        /// Find the active model.
        /// </summary>
        /// <param name="machineName">The name of the machine the client is connected to.</param>
        /// <returns>The active model; else null.</returns>
        public ActiveConnectionModel FindActive(string machineName)
        {
            if (String.IsNullOrEmpty(machineName)) throw new ArgumentNullException("machineName");

            // Attempt to find the client for the machine name.
            ActiveConnectionModel model = null;
            bool ret = _activeList.TryGetValue(machineName, out model);

            if (ret)
                return model;
            else
                return null;
        }

        /// <summary>
        /// Find the client.
        /// </summary>
        /// <param name="machineName">The name of the machine the client is connected to.</param>
        /// <returns>The client; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Nequeo.Client.Manage.IInteractionClient FindClient(string machineName)
        {
            if (String.IsNullOrEmpty(machineName)) throw new ArgumentNullException("machineName");

            // Attempt to find the client for the machine name.
            Nequeo.Client.Manage.IInteractionClient client = null;
            bool ret = _clientList.TryGetValue(machineName, out client);

            if (ret)
                return client;
            else
                return null;
        }

        /// <summary>
        /// Find all the clients for the host list.
        /// </summary>
        /// <param name="hosts">The list of hosts.</param>
        /// <param name="keyPrefix">The additional key prefix.</param>
        /// <returns>The collection of clients; else null.</returns>
        public Nequeo.Client.Manage.IInteractionClient[] FindClients(string[] hosts, string keyPrefix = "")
        {
            // Make sure references exists.
            if (hosts != null && hosts.Length > 0)
            {
                List<Nequeo.Client.Manage.IInteractionClient> clients = new List<Nequeo.Client.Manage.IInteractionClient>();
                IEnumerable<string> distinctHosts = hosts.Distinct(new Nequeo.Invention.ToLowerComparer());

                // Set up the number of identifiers to find.
                object monitor = new object();
                int numberToFind = distinctHosts.Count();
                bool[] found = new bool[numberToFind];

                // For each client in the collection.
                foreach (KeyValuePair<string, Nequeo.Client.Manage.IInteractionClient> item in _clientList)
                {
                    int i = 0;
                    int numberFound = 0;

                    // For each machine name.
                    foreach (string host in distinctHosts)
                    {
                        string key = item.Key;
                        if (!String.IsNullOrEmpty(keyPrefix))
                            key = item.Key.Replace(keyPrefix, "").Trim();

                        // If the machine name has been found.
                        if (key.ToLower() == host.ToLower())
                        {
                            // Add the client item.
                            clients.Add(item.Value);
                            found[i] = true;
                        }

                        // If the current identifier
                        // has been found then stop the
                        // search for the current identifier.
                        if (found[i])
                            break;

                        i++;
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

                // Return the client list else null.
                return (clients.Count > 0 ? clients.ToArray() : null);
            }
            else
                return null;
        }

        /// <summary>
        /// Get the list of port combinations.
        /// </summary>
        /// <param name="ports">The list of port names and port numbers (name;80)</param>
        /// <returns>The list of combinations; else null.</returns>
        public SendToPortModel[] GetPorts(string[] ports)
        {
            // Make sure references exists.
            if (ports != null && ports.Length > 0)
            {
                List<SendToPortModel> models = new List<SendToPortModel>();

                // For each port type
                foreach (string item in ports)
                {
                    // Extract the port data.
                    string[] split = item.Split(';');
                    SendToPortModel model = new SendToPortModel()
                    {
                        Name = split[0].Trim(),
                        Number = Int32.Parse(split[1].Trim())
                    };

                    // Add the model.
                    models.Add(model);
                }

                // Return the model list else null.
                return (models.Count > 0 ? models.ToArray() : null);
            }
            else
                return null;
        }

        /// <summary>
        /// Get the list of host and receivers combinations.
        /// </summary>
        /// <param name="hosts">The list of hosts that each receiver is connected to (this array size must be the same as receivers array size).</param>
        /// <param name="receivers">The list of unique identities to send the data to (this array size must be the same as receiverHosts array size).</param>
        /// <returns>The list of combinations; else null.</returns>
        public SendToHostModel[] GetHostReceivers(string[] hosts, string[] receivers)
        {
            // Make sure references exists.
            if (hosts != null && receivers != null)
            {
                // Make sure data exists.
                if (hosts.Length > 0 && receivers.Length > 0)
                {
                    object monitor = new object();
                    List<SendToHostModel> models = new List<SendToHostModel>();
                    IEnumerable<string> distinctHosts = hosts.Distinct(new Nequeo.Invention.ToLowerComparer());

                    // For each distinct host.
                    foreach (string host in distinctHosts)
                    {
                        List<string> hostReceivers = new List<string>();

                        // Find all distinct host combinations.
                        Parallel.For(0, hosts.Length, () => -1, (i, state, local) =>
                        {
                            // If host are equal.
                            if (host.ToLower() == hosts[i].ToLower())
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
                                if(local > -1)
                                    // Add the receiver associated with the host.
                                    hostReceivers.Add(receivers[local]);
                            }
                        });

                        // Add the unique host and receivers to the model..
                        models.Add(new SendToHostModel()
                        {
                            Host = host,
                            Receivers = hostReceivers.ToArray()
                        });
                    }

                    // Return the model list else null.
                    return (models.Count > 0 ? models.ToArray() : null);
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the list of hosts that have no instance created.
        /// </summary>
        /// <param name="clients">The current clients</param>
        /// <param name="sentToHosts">The host to receiver collection.</param>
        /// <returns>The list of hosts with no instance; else null all instances exist.</returns>
        public string[] GetHostsWithNoInstance(Nequeo.Client.Manage.IInteractionClient[] clients, SendToHostModel[] sentToHosts)
        {
            object monitor = new object();
            List<string> hostsWithNoConnection = new List<string>();

            // For each host to receivers item.
            // Attempt to find all clients for
            // each host.
            Parallel.For(0, sentToHosts.Length, () => -1, (i, state, local) =>
            {
                bool found = false;

                // For each client in the collection.
                foreach (Nequeo.Client.Manage.IInteractionClient client in clients)
                {
                    // If the host names match then
                    // the client exists.
                    if (sentToHosts[i].Host.ToLower() == client.HostNameOrAddress.ToLower())
                    {
                        found = true;
                        break;
                    }
                }

                // If the host has no mtach then
                // add the host so a connection can be made.
                if (!found)
                    return i;

                return -1;

            }, local =>
            {
                // Add one to the count.
                lock (monitor)
                {
                    // If a valid index.
                    if (local > -1)
                        // Add the receiver associated with the host.
                        hostsWithNoConnection.Add(sentToHosts[local].Host);
                }
            });

            // The client send list.
            return hostsWithNoConnection.Count > 0 ? hostsWithNoConnection.ToArray() : null;
        }

        /// <summary>
        /// Get the differences between the receivers to comparer.
        /// </summary>
        /// <param name="receivers">The new receivers to examine.</param>
        /// <param name="receiversCompare">The store receivers to compare with.</param>
        /// <returns>The list of receivers; else null.</returns>
        public string[] GetDifferences(string[] receivers, string[] receiversCompare)
        {
            if (receivers == null || receiversCompare == null)
                return null;

            List<string> differenceList = new List<string>();

            // For each receiver
            foreach (string item in receivers)
            {
                int i = 0;
                bool found = false;
                
                // For each receiver
                foreach (string itemCompare in receiversCompare)
                {
                    // An item has been found.
                    if (item.ToLower().Equals(itemCompare.ToLower()))
                    {
                        found = true;
                    }

                    // If the current identifier
                    // has been found then stop the
                    // search for the current identifier.
                    if (found)
                        break;

                    i++;
                }

                // If not found then add to the list.
                if (!found)
                    differenceList.Add(item);
            }

            // Get the diffences.
            return differenceList.Count > 0 ? differenceList.ToArray() : null;
        }

        /// <summary>
        /// Is there a differences between the receivers to comparer.
        /// </summary>
        /// <param name="receivers">The new receivers to examine.</param>
        /// <param name="receiversCompare">The store receivers to compare with.</param>
        /// <returns>True if different; else false.</returns>
        public bool IsDifferent(string[] receivers, string[] receiversCompare)
        {
            bool diff = false;

            if (receivers == null || receiversCompare == null)
                return diff;

            // For each receiver
            foreach (string item in receivers)
            {
                int i = 0;
                bool found = false;

                // For each receiver
                foreach (string itemCompare in receiversCompare)
                {
                    // An item has been found.
                    if (item.ToLower().Equals(itemCompare.ToLower()))
                    {
                        found = true;
                    }

                    // If the current identifier
                    // has been found then stop the
                    // search for the current identifier.
                    if (found)
                        break;

                    i++;
                }

                // If not found the break.
                if (!found)
                {
                    diff = true;
                    break;
                }
            }

            // Return the result.
            return diff;
        }

        /// <summary>
        /// Combine all active receivers.
        /// </summary>
        /// <param name="model">The active connection model.</param>
        /// <returns>The collection of receivers.</returns>
        public string[] CombineReceivers(ActiveConnectionModel model)
        {
            if (model == null)
                return null;

            List<string> differenceList = new List<string>();

            // For each host model add all the receivers.
            foreach (SendToHostModel item in model.Hosts)
                differenceList.AddRange(item.Receivers);

            // Get the combined list.
            return differenceList.Count > 0 ? differenceList.ToArray() : null;
        }

        /// <summary>
        /// Combine all active receiver hosts.
        /// </summary>
        /// <param name="model">The active connection model.</param>
        /// <returns>The collection of receiver hosts.</returns>
        public string[] CombineReceiverHosts(ActiveConnectionModel model)
        {
            if (model == null)
                return null;

            List<string> differenceList = new List<string>();

            // For each host model add all the receivers.
            foreach (SendToHostModel item in model.Hosts)
                differenceList.Add(item.Host);

            // Get the combined list.
            return differenceList.Count > 0 ? differenceList.ToArray() : null;
        }

        /// <summary>
        /// Are all receivers on same machine.
        /// </summary>
        /// <param name="machineName">The current machine name.</param>
        /// <param name="receiversHost">The receivers host list.</param>
        /// <returns>True if all the receivers are on the same machine; else false.</returns>
        public bool AreAllReceiversOnSameMachine(string machineName, string[] receiversHost)
        {
            // For each host
            foreach (string item in receiversHost)
            {
                // If the current client host is different
                // then the contact exists on another machine.
                if (!machineName.ToLower().Equals(item.ToLower()))
                {
                    // Found item.
                    return false;
                }
            }

            // If all on the same machine then true.
            return true;
        }

        /// <summary>
        /// Match up all hosts to receivers.
        /// </summary>
        /// <param name="machineName">The current machine name.</param>
        /// <param name="receivers">The receivers list.</param>
        /// <param name="receiversHost">The receivers host list.</param>
        /// <param name="receiversOut">The receivers result list.</param>
        /// <param name="receiversHostOut">The receivers host result list.</param>
        /// <returns>True if all the receivers are on the same machine; else false.</returns>
        public bool MatchReceivers(string machineName, string[] receivers, string[] receiversHost,
            out string[] receiversOut, out string[] receiversHostOut)
        {
            object monitor = new object();
            List<string> receiversIn = new List<string>();
            List<string> receiversHostIn = new List<string>();

            // Look for only host data that exists or has been found.
            Parallel.For(0, receivers.Length, () => -1, (i, state, local) =>
            {
                // Look for only host data that exists or has been
                // found, some host data may contain empty string ("")
                // these clients can not be contacted because no host
                // has been assigned or the client if offline.
                if (!String.IsNullOrEmpty(receiversHost[i]))
                {
                    // If the current client host is different
                    // then the contact exists on another machine.
                    if (!machineName.ToLower().Equals(receiversHost[i].ToLower()))
                    {
                        // Found item.
                        return i;
                    }
                }
                return -1;

            }, local =>
            {
                // Add one to the count.
                lock (monitor)
                {
                    // If a valid index.
                    if (local > -1)
                    {
                        // Add the receiver associated with the host.
                        receiversIn.Add(receivers[local]);
                        receiversHostIn.Add(receiversHost[local]);
                    }
                }
            });

            // Assign the new online lists that are not empty.
            receiversOut = receiversIn.ToArray();
            receiversHostOut = receiversHostIn.ToArray();

            // If all on the same machine then true.
            return receiversHostIn.Count < 1 ? true : false;
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
				// Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_clientList != null)
                    {
                        // Close and dispose of all connections.
                        foreach (KeyValuePair<string, Nequeo.Client.Manage.IInteractionClient> item in _clientList)
                        {
                            if (item.Value != null)
                            {
                                try
                                {
                                    item.Value.Close();
                                    item.Value.Dispose();
                                }
                                finally 
                                {
                                    Nequeo.Client.Manage.IInteractionClient client = item.Value;
                                    client = null;
                                }
                            }
                        }
                    }

                    if (_activeList != null)
                    {
                        // Close and dispose of all connections.
                        foreach (KeyValuePair<string, ActiveConnectionModel> item in _activeList)
                        {
                            if (item.Value != null)
                            {
                                try
                                {
                                    // For each host.
                                    foreach (SendToHostModel host in item.Value.Hosts)
                                    {
                                        try
                                        {
                                            // Release the reference.
                                            host.Host = null;
                                            host.Receivers = null;
                                        }
                                        catch { }
                                    }
                                    SendToHostModel[] hosts = item.Value.Hosts;
                                    hosts = null;

                                    // For each port.
                                    foreach (SendToPortModel port in item.Value.Ports)
                                    {
                                        try
                                        {
                                            // Release the reference.
                                            port.Name = null;
                                        }
                                        catch { }
                                    }
                                    SendToPortModel[] ports = item.Value.Ports;
                                    ports = null;
                                }
                                finally
                                {
                                    ActiveConnectionModel model = item.Value;
                                    model = null;
                                }
                            }
                        }
                    }

                    // Dispose managed resources.
                    if (_clientList != null)
                        _clientList.Clear();

                    if (_activeList != null)
                        _activeList.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _clientList = null;
                _activeList = null;
                _lockClientObject = null;
                _lockActiveObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~InteractionManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
