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
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Net.Sockets;
using Nequeo.Security;
using Nequeo.Threading;
using Nequeo.Data.Enum;
using Nequeo.Data.Provider;
using Nequeo.Extension;

namespace Nequeo.Client
{
    /// <summary>
    /// Client sorted binary search manager. Stores all clients and
    /// managers the searching of context items within the current machine.
    /// </summary>
    /// <remarks>This object is thread-safe.</remarks>
    public class Manager : IDisposable
    {
        /// <summary>
        /// Client sorted binary search manager. Stores all clients and
        /// managers the searching of context items within the current machine.
        /// </summary>
        public Manager()
        {
            _clientList = new SortedDictionary<string, Nequeo.Net.Sockets.IClient>();
            _messageQueue = new SortedDictionary<string, List<Tuple<string, string, byte[]>>>();
            _clientTokens = new SortedDictionary<string, string>();
        }

        private object _lockObject = new object();

        private SortedDictionary<string, string> _clientTokens = null;
        private SortedDictionary<string, Nequeo.Net.Sockets.IClient> _clientList = null;
        private SortedDictionary<string, List<Tuple<string, string, byte[]>>> _messageQueue = null;

        /// <summary>
        /// Add the token item.
        /// </summary>
        /// <param name="key">The key for the token.</param>
        /// <param name="token">The client token.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void AddToken(string key, string token)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (String.IsNullOrEmpty(token)) throw new ArgumentNullException("token");

            lock (_lockObject)
            {
                // Add the item.
                _clientTokens.Add(key, token);
            }
        }

        /// <summary>
        /// Remove the token..
        /// </summary>
        /// <param name="key">The key for the token.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void RemoveToken(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            lock (_lockObject)
            {
                // Remove the item.
                _clientTokens.Remove(key);
            }
        }

        /// <summary>
        /// Find the token.
        /// </summary>
        /// <param name="key">The key for the token.</param>
        /// <returns>The token; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual string FindToken(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            // Attempt to find the token.
            string token = null;
            bool ret = _clientTokens.TryGetValue(key, out token);

            if (ret)
                return token;
            else
                return null;
        }

        /// <summary>
        /// Add the client item.
        /// </summary>
        /// <param name="key">The key for the client.</param>
        /// <param name="client">The client connected to the machine.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void AddClient(string key, Nequeo.Net.Sockets.IClient client)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (client == null) throw new ArgumentNullException("client");

            lock (_lockObject)
            {
                // Add the item.
                _clientList.Add(key, client);
            }
        }

        /// <summary>
        /// Remove the client item.
        /// </summary>
        /// <param name="key">The key for the client.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void RemoveClient(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            lock (_lockObject)
            {
                // Find the client.
                Nequeo.Net.Sockets.IClient client = FindClient(key);
                if (client != null)
                {
                    // Close the connection.
                    client.Close();
                    client.Dispose();
                }

                // Remove the item.
                _clientList.Remove(key);
            }
        }

        /// <summary>
        /// Find the client.
        /// </summary>
        /// <param name="key">The key for the client.</param>
        /// <returns>The client; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Nequeo.Net.Sockets.IClient FindClient(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            // Attempt to find the client.
            Nequeo.Net.Sockets.IClient client = null;
            bool ret = _clientList.TryGetValue(key, out client);

            if (ret)
                return client;
            else
                return null;
        }

        /// <summary>
        /// Find all the clients.
        /// </summary>
        /// <param name="keys">The list of keys.</param>
        /// <returns>The collection of clients; else null.</returns>
        public virtual Nequeo.Net.Sockets.IClient[] FindClients(string[] keys)
        {
            // Make sure references exists.
            if (keys != null && keys.Length > 0)
            {
                List<Nequeo.Net.Sockets.IClient> clients = new List<Nequeo.Net.Sockets.IClient>();

                // Set up the number of identifiers to find.
                object monitor = new object();
                int numberToFind = keys.Count();
                bool[] found = new bool[numberToFind];

                // For each client in the collection.
                foreach (KeyValuePair<string, Nequeo.Net.Sockets.IClient> item in _clientList)
                {
                    int i = 0;
                    int numberFound = 0;

                    // For each machine name.
                    foreach (string key in keys)
                    {
                        // If the machine name has been found.
                        if (key.ToLower() == keys[i].ToLower())
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
        /// Add the queued message.
        /// </summary>
        /// <param name="key">The key for the message.</param>
        /// <param name="message">The message to add.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void AddMessage(string key, Tuple<string, string, byte[]> message)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (message == null) throw new ArgumentNullException("message");

            lock (_lockObject)
            {
                // Find the messages.
                var messages = FindMessages(key);

                // If messages do not exist.
                if (messages == null)
                {
                    // Create the instance and add the message.
                    List<Tuple<string, string, byte[]>> add = new List<Tuple<string, string, byte[]>>();
                    add.Add(message);

                    // Add the item.
                    _messageQueue.Add(key, add);
                }
                else
                {
                    // Attempt to find the message.
                    List<Tuple<string, string, byte[]>> update = null;
                    bool ret = _messageQueue.TryGetValue(key, out update);

                    // If found then update the message.
                    if (ret) update.Add(message);
                }
            }
        }

        /// <summary>
        /// Remove the queued messages.
        /// </summary>
        /// <param name="key">The key for the message.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void RemoveMessages(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            lock (_lockObject)
            {
                // Remove the item.
                _messageQueue.Remove(key);
            }
        }

        /// <summary>
        /// Remove the queued messages.
        /// </summary>
        /// <param name="key">The key for the message.</param>
        /// <param name="predicate">The predicate to search against..</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual void RemoveMessages(string key, Func<Tuple<string, string, byte[]>, bool> predicate)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (predicate == null) throw new ArgumentNullException("predicate");

            lock (_lockObject)
            {
                // Find the messages.
                var messages = FindMessages(key);

                // If messages do not exist.
                if (messages != null)
                {
                    // Attempt to find the message.
                    List<Tuple<string, string, byte[]>> update = null;
                    bool ret = _messageQueue.TryGetValue(key, out update);

                    // Get the list of new messages.
                    Tuple<string, string, byte[]>[] newMessages = update.Remove(predicate);

                    // Remove the current message list.
                    _messageQueue.Remove(key);
                    _messageQueue.Add(key, new List<Tuple<string, string, byte[]>>(newMessages));
                }
            }
        }

        /// <summary>
        /// Find the queued messages.
        /// </summary>
        /// <param name="key">The key for the message.</param>
        /// <returns>The message; else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual Tuple<string, string, byte[]>[] FindMessages(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            // Attempt to find the message.
            List<Tuple<string, string, byte[]>> message = null;
            bool ret = _messageQueue.TryGetValue(key, out message);

            if (ret)
                return message.ToArray();
            else
                return null;
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

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
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
                        foreach (KeyValuePair<string, Nequeo.Net.Sockets.IClient> item in _clientList)
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
                                    Nequeo.Net.Sockets.IClient client = item.Value;
                                    client = null;
                                }
                            }
                        }
                    }

                    // Dispose managed resources.
                    if (_clientList != null)
                        _clientList.Clear();

                    if (_messageQueue != null)
                        _messageQueue.Clear();

                    if (_clientTokens != null)
                        _clientTokens.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _clientList = null;
                _messageQueue = null;
                _clientTokens = null;
                _lockObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Manager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
