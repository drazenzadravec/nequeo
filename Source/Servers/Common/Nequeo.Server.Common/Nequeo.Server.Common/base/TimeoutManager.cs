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

namespace Nequeo.Server
{
    /// <summary>
    /// Server context sorted binary timeout manager. Stores all server context clients and
    /// managers the searching of context items within the current machine.
    /// </summary>
    /// <remarks>This object is thread-safe.</remarks>
    public class TimeoutManager : IDisposable, Nequeo.Net.IServerContextManager, Nequeo.Net.IMemberContextManager, Nequeo.Net.ISingleContextManager
    {
        /// <summary>
        /// Server context sorted binary timeout manager. Stores all server context clients and
        /// managers the searching of context items within the current machine.
        /// </summary>
        public TimeoutManager()
        {
            _interval = new Maintenance.Timing.IntervalControl((double)0.0);
            _contextList = new SortedDictionary<long, Net.Sockets.IServerContext>();
            _memberList = new SortedDictionary<long, Nequeo.Net.IMemberContext>();
            _singleList = new SortedDictionary<long, Nequeo.Net.Provider.ISingleContextBase>();
        }

        private object _lockObject = new object();

        private bool _hasStarted = false;
        private Nequeo.Maintenance.Timing.IntervalControl _interval = null;

        private int _timeout = -1;
        private long _indexContext = -1;
        private long _indexMember = -1;
        private long _indexSingle = -1;
        SortedDictionary<long, Net.Sockets.IServerContext> _contextList = null;
        SortedDictionary<long, Nequeo.Net.IMemberContext> _memberList = null;
        SortedDictionary<long, Nequeo.Net.Provider.ISingleContextBase> _singleList = null;

        /// <summary>
        /// Gets the total count.
        /// </summary>
        public long Count
        {
            get
            {
                lock (_lockObject)
                {
                    return (long)(_contextList.Count + _memberList.Count + _singleList.Count);
                }
            }
        }

        /// <summary>
        /// Find all unique identifiers that have been set; that is none null or empty.
        /// </summary>
        /// <returns>The collection of unique identifiers.</returns>
        public string[] FindAllUniqueIdentifiers()
        {
            lock (_lockObject)
            {
                List<string> uiList = new List<string>();

                // Get all none null unique identifiers.
                IEnumerable<KeyValuePair<long, Net.Sockets.IServerContext>> uis1 = _contextList.Where(u => !String.IsNullOrEmpty(u.Value.UniqueIdentifier));
                foreach (KeyValuePair<long, Net.Sockets.IServerContext> item in uis1)
                    uiList.Add(item.Value.UniqueIdentifier);

                // Get all none null unique identifiers.
                IEnumerable<KeyValuePair<long, Nequeo.Net.IMemberContext>> uis2 = _memberList.Where(u => !String.IsNullOrEmpty(u.Value.UniqueIdentifier));
                foreach (KeyValuePair<long, Nequeo.Net.IMemberContext> item in uis2)
                    uiList.Add(item.Value.UniqueIdentifier);

                // Get all none null unique identifiers.
                IEnumerable<KeyValuePair<long, Nequeo.Net.Provider.ISingleContextBase>> uis3 = _singleList.Where(u => !String.IsNullOrEmpty(u.Value.UniqueIdentifier));
                foreach (KeyValuePair<long, Nequeo.Net.Provider.ISingleContextBase> item in uis3)
                    uiList.Add(item.Value.UniqueIdentifier);

                // Return the list.
                return uiList.ToArray();
            }
        }

        /// <summary>
        /// Add the server context item.
        /// </summary>
        /// <param name="context">The server context to add to the list.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Add(Net.Sockets.IServerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            lock (_lockObject)
            {
                try
                {
                    _indexContext++;

                    // Add the item.
                    _contextList.Add(_indexContext, context);
                }
                catch { }
            }
        }

        /// <summary>
        /// Add the member context item.
        /// </summary>
        /// <param name="context">The member context to add to the list.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Add(Nequeo.Net.IMemberContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            lock (_lockObject)
            {
                try
                {
                    _indexMember++;

                    // Add the item.
                    _memberList.Add(_indexMember, context);
                }
                catch { }
            }
        }

        /// <summary>
        /// Add the single context item.
        /// </summary>
        /// <param name="context">The single context to add to the list.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Add(Nequeo.Net.Provider.ISingleContextBase context)
        {
            if (context == null) throw new ArgumentNullException("context");

            lock (_lockObject)
            {
                try
                {
                    _indexSingle++;

                    // Add the item.
                    _singleList.Add(_indexSingle, context);
                }
                catch { }
            }
        }

        /// <summary>
        /// Remove the server context item.
        /// </summary>
        /// <param name="context">The server context to remove from the list.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Remove(Net.Sockets.IServerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            lock (_lockObject)
            {
                try
                {
                    long index = -1;

                    // For each context item
                    foreach (KeyValuePair<long, Net.Sockets.IServerContext> item in _contextList)
                    {
                        // If the contexts are equal.
                        if (item.Value.Equals(context))
                        {
                            // Get the item index
                            // and stop the search.
                            index = item.Key;
                            break;
                        }
                    }

                    // Remove the item.
                    if (index > -1)
                        _contextList.Remove(index);
                }
                catch { }
            }
        }

        /// <summary>
        /// Remove the member context item.
        /// </summary>
        /// <param name="context">The member context to remove from the list.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Remove(Nequeo.Net.IMemberContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            lock (_lockObject)
            {
                try
                {
                    long index = -1;

                    // For each context item
                    foreach (KeyValuePair<long, Nequeo.Net.IMemberContext> item in _memberList)
                    {
                        // If the contexts are equal.
                        if (item.Value.Equals(context))
                        {
                            // Get the item index
                            // and stop the search.
                            index = item.Key;
                            break;
                        }
                    }

                    // Remove the item.
                    if (index > -1)
                        _memberList.Remove(index);
                }
                catch { }
            }
        }

        /// <summary>
        /// Remove the single context item.
        /// </summary>
        /// <param name="context">The single context to remove from the list.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Remove(Nequeo.Net.Provider.ISingleContextBase context)
        {
            if (context == null) throw new ArgumentNullException("context");

            lock (_lockObject)
            {
                try
                {
                    long index = -1;

                    // For each context item
                    foreach (KeyValuePair<long, Nequeo.Net.Provider.ISingleContextBase> item in _singleList)
                    {
                        // If the contexts are equal.
                        if (item.Value.Equals(context))
                        {
                            // Get the item index
                            // and stop the search.
                            index = item.Key;
                            break;
                        }
                    }

                    // Remove the item.
                    if (index > -1)
                        _singleList.Remove(index);
                }
                catch { }
            }
        }

        /// <summary>
        /// Find all server context list.
        /// </summary>
        /// <returns>The context list; else null.</returns>
        public Net.Sockets.IServerContext[] FindAll()
        {
            lock (_lockObject)
            {
                return _contextList.Values.ToArray();
            }
        }

        /// <summary>
        /// Find all member context list.
        /// </summary>
        /// <returns>The context list; else null.</returns>
        public Net.IMemberContext[] FindAllMember()
        {
            lock (_lockObject)
            {
                return _memberList.Values.ToArray();
            }
        }

        /// <summary>
        /// Find all server context list.
        /// </summary>
        /// <returns>The context list; else null.</returns>
        public Net.Provider.ISingleContextBase[] FindAllSingle()
        {
            lock (_lockObject)
            {
                return _singleList.Values.ToArray();
            }
        }

        /// <summary>
        /// Find the server context.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>The server context; else null.</returns>
        public Net.Sockets.IServerContext Find(string uniqueIdentifier)
        {
            // Attempt to find the context.
            Net.Sockets.IServerContext[] items = Find(new string[] { uniqueIdentifier });

            // If items have been found then return the first item.
            if (items != null && items.Length > 0)
                return items[0];
            else
                return null;
        }

        /// <summary>
        /// Find the member context.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>The member context; else null.</returns>
        public Nequeo.Net.IMemberContext FindMember(string uniqueIdentifier)
        {
            // Attempt to find the context.
            Nequeo.Net.IMemberContext[] items = FindMember(new string[] { uniqueIdentifier });

            // If items have been found then return the first item.
            if (items != null && items.Length > 0)
                return items[0];
            else
                return null;
        }

        /// <summary>
        /// Find the single context.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>The single context; else null.</returns>
        public Nequeo.Net.Provider.ISingleContextBase FindSingle(string uniqueIdentifier)
        {
            // Attempt to find the context.
            Nequeo.Net.Provider.ISingleContextBase[] items = FindSingle(new string[] { uniqueIdentifier });

            // If items have been found then return the first item.
            if (items != null && items.Length > 0)
                return items[0];
            else
                return null;
        }

        /// <summary>
        /// Find server context list for the unique identifiers.
        /// </summary>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        /// <returns>The context list; else null.</returns>
        public Net.Sockets.IServerContext[] Find(string[] uniqueIdentifiers)
        {
            if (uniqueIdentifiers != null && uniqueIdentifiers.Length > 0)
            {
                // Set up the number of identifiers to find.
                object monitor = new object();
                int numberToFind = uniqueIdentifiers.Length;
                bool[] found = new bool[numberToFind];

                // Attempt to find server context items in the cache.
                List<Net.Sockets.IServerContext> contextList = null;

                lock (_lockObject)
                {
                    // If null then no existing server context items exist.
                    if (contextList == null)
                    {
                        // Create a new instance.
                        contextList = new List<Net.Sockets.IServerContext>();

                        // For each server context within the list.
                        foreach (KeyValuePair<long, Net.Sockets.IServerContext> item in _contextList)
                        {
                            int numberFound = 0;

                            // For each unique identifier.
                            for (int i = 0; i < numberToFind; i++)
                            {
                                // If the unique identifier has been found.
                                if (item.Value.UniqueIdentifier.ToLower() == uniqueIdentifiers[i].ToLower())
                                {
                                    // Add the server context item.
                                    contextList.Add(item.Value);
                                    found[i] = true;
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
                    }

                    // Return the server context list else null.
                    return (contextList.Count > 0 ? contextList.ToArray() : null);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Find member context list for the unique identifiers.
        /// </summary>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        /// <returns>The context list; else null.</returns>
        public Nequeo.Net.IMemberContext[] FindMember(string[] uniqueIdentifiers)
        {
            if (uniqueIdentifiers != null && uniqueIdentifiers.Length > 0)
            {
                // Set up the number of identifiers to find.
                object monitor = new object();
                int numberToFind = uniqueIdentifiers.Length;
                bool[] found = new bool[numberToFind];

                // Attempt to find server context items in the cache.
                List<Nequeo.Net.IMemberContext> contextList = null;

                lock (_lockObject)
                {
                    // If null then no existing server context items exist.
                    if (contextList == null)
                    {
                        // Create a new instance.
                        contextList = new List<Nequeo.Net.IMemberContext>();

                        // For each server context within the list.
                        foreach (KeyValuePair<long, Nequeo.Net.IMemberContext> item in _memberList)
                        {
                            int numberFound = 0;

                            // For each unique identifier.
                            for (int i = 0; i < numberToFind; i++)
                            {
                                // If the unique identifier has been found.
                                if (item.Value.UniqueIdentifier.ToLower() == uniqueIdentifiers[i].ToLower())
                                {
                                    // Add the server context item.
                                    contextList.Add(item.Value);
                                    found[i] = true;
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
                    }

                    // Return the server context list else null.
                    return (contextList.Count > 0 ? contextList.ToArray() : null);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Find single context list for the unique identifiers.
        /// </summary>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        /// <returns>The context list; else null.</returns>
        public Nequeo.Net.Provider.ISingleContextBase[] FindSingle(string[] uniqueIdentifiers)
        {
            if (uniqueIdentifiers != null && uniqueIdentifiers.Length > 0)
            {
                // Set up the number of identifiers to find.
                object monitor = new object();
                int numberToFind = uniqueIdentifiers.Length;
                bool[] found = new bool[numberToFind];

                // Attempt to find server context items in the cache.
                List<Nequeo.Net.Provider.ISingleContextBase> contextList = null;

                lock (_lockObject)
                {
                    // If null then no existing server context items exist.
                    if (contextList == null)
                    {
                        // Create a new instance.
                        contextList = new List<Nequeo.Net.Provider.ISingleContextBase>();

                        // For each server context within the list.
                        foreach (KeyValuePair<long, Nequeo.Net.Provider.ISingleContextBase> item in _singleList)
                        {
                            int numberFound = 0;

                            // For each unique identifier.
                            for (int i = 0; i < numberToFind; i++)
                            {
                                // If the unique identifier has been found.
                                if (item.Value.UniqueIdentifier.ToLower() == uniqueIdentifiers[i].ToLower())
                                {
                                    // Add the server context item.
                                    contextList.Add(item.Value);
                                    found[i] = true;
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
                    }

                    // Return the server context list else null.
                    return (contextList.Count > 0 ? contextList.ToArray() : null);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Stop the timeout monitor for all the context connections.
        /// </summary>
        public void Stop()
        {
            // Stop the timer.
            if (_hasStarted)
                _interval.Stop();

            _hasStarted = false;
        }

        /// <summary>
        /// Start the timeout monitor for all the context connections.
        /// </summary>
        /// <param name="timeout">The timeout (minutes) for each client connection when in-active; -1 wait indefinitely.</param>
        /// <exception cref="System.Exception">The timeout must be zero or greater.</exception>
        public void Start(int timeout)
        {
            if (timeout < 0) throw new Exception("The timeout must be zero or greater.");
            _timeout = timeout;

            // Start the timer.
            if (!_hasStarted)
            {
                // Create the action handler.
                object state = null;
                Action<object> action = (object s) => ActionMethod(s);

                // Assign the interval and start the timer.
                _interval.Interval = (double)(timeout * 60 * 1000);
                _interval.Start(action, state);

                _hasStarted = true;
            }
        }

        /// <summary>
        /// The action to perform.
        /// </summary>
        /// <param name="state">The state object passed.</param>
        private void ActionMethod(object state)
        {
            lock (_lockObject)
            {
                // For each context item
                foreach (KeyValuePair<long, Net.Sockets.IServerContext> item in _contextList)
                {
                    try
                    {
                        // Get the current server context.
                        Net.Sockets.IServerContext current = item.Value;
                        if (current != null)
                        {
                            // If the context has timed out
                            // then close the connection.
                            if (current.HasTimedOut(_timeout))
                                current.Close();
                        }
                    }
                    catch { }
                }

                // For each context item
                foreach (KeyValuePair<long, Nequeo.Net.IMemberContext> item in _memberList)
                {
                    try
                    {
                        // Get the current server context.
                        Nequeo.Net.IMemberContext current = item.Value;
                        if (current != null)
                        {
                            // If the context has timed out
                            // then close the connection.
                            if (current.HasTimedOut(_timeout))
                                current.Close();
                        }
                    }
                    catch { }
                }

                // For each context item
                foreach (KeyValuePair<long, Nequeo.Net.Provider.ISingleContextBase> item in _singleList)
                {
                    try
                    {
                        // Get the current server context.
                        Nequeo.Net.Provider.ISingleContextBase current = item.Value;
                        if (current != null)
                        {
                            // If the context has timed out
                            // then close the connection.
                            if (current.HasTimedOut(_timeout))
                                current.Close();
                        }
                    }
                    catch { }
                }
            }
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
                    // Dispose managed resources.
                    if (_contextList != null)
                        _contextList.Clear();

                    if (_memberList != null)
                        _memberList.Clear();

                    if (_singleList != null)
                        _singleList.Clear();

                    if (_interval != null)
                        _interval.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _contextList = null;
                _memberList = null;
                _singleList = null;
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
        ~TimeoutManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }
}
