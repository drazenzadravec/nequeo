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
    /// State context sorted binary search manager. Stores all state context clients and
    /// managers the searching of context items within the current machine.
    /// </summary>
    /// <remarks>This object is thread-safe.</remarks>
    public class StateContextManager : IDisposable, Nequeo.Net.IStateContextManager
    {
        /// <summary>
        /// State context sorted binary search manager. Stores all state context clients and
        /// managers the searching of context items within the current machine.
        /// </summary>
        public StateContextManager()
        {
            _contextList = new SortedDictionary<string, object>();
        }

        private object _lockObject = new object();
        SortedDictionary<string, object> _contextList = null;

        /// <summary>
        /// Gets the total count.
        /// </summary>
        public long Count
        {
            get
            {
                lock (_lockObject)
                {
                    return (long)(_contextList.Count);
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
                IEnumerable<KeyValuePair<string, object>> uis = _contextList.Where(u => !String.IsNullOrEmpty(u.Key));
                foreach (KeyValuePair<string, object> item in uis)
                    uiList.Add(item.Key);

                // Return the list.
                return uiList.ToArray();
            }
        }

        /// <summary>
        /// Add the state context item.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="context">The state context to add to the list.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Add(string uniqueIdentifier, object context)
        {
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            lock (_lockObject)
            {
                // Add the item.
                _contextList.Add(uniqueIdentifier, context);
            }
        }

        /// <summary>
        /// Remove the state context item.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Remove(string uniqueIdentifier)
        {
            if (String.IsNullOrEmpty(uniqueIdentifier)) throw new ArgumentNullException("uniqueIdentifier");

            lock (_lockObject)
            {
                _contextList.Remove(uniqueIdentifier);
            }
        }

        /// <summary>
        /// Find all state context list.
        /// </summary>
        /// <returns>The state context list; else null.</returns>
        public object[] FindAll()
        {
            lock (_lockObject)
            {
                return _contextList.Values.ToArray();
            }
        }

        /// <summary>
        /// Find the state context.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>The state context; else null.</returns>
        public object Find(string uniqueIdentifier)
        {
            // Attempt to find the context.
            object[] items = Find(new string[] { uniqueIdentifier });

            // If items have been found then return the first item.
            if (items != null && items.Length > 0)
                return items[0];
            else
                return null;
        }

        /// <summary>
        /// Find state context list for the unique identifiers.
        /// </summary>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        /// <returns>The state context list; else null.</returns>
        public object[] Find(string[] uniqueIdentifiers)
        {
            if (uniqueIdentifiers != null && uniqueIdentifiers.Length > 0)
            {
                // Set up the number of identifiers to find.
                object monitor = new object();
                int numberToFind = uniqueIdentifiers.Length;
                bool[] found = new bool[numberToFind];

                // Attempt to find server context items in the cache.
                List<object> contextList = null;

                lock (_lockObject)
                {
                    // If null then no existing server context items exist.
                    if (contextList == null)
                    {
                        // Create a new instance.
                        contextList = new List<object>();

                        // For each server context within the list.
                        foreach (KeyValuePair<string, object> item in _contextList)
                        {
                            int numberFound = 0;

                            // For each unique identifier.
                            for (int i = 0; i < numberToFind; i++)
                            {
                                // If the unique identifier has been found.
                                if (item.Key.ToLower() == uniqueIdentifiers[i].ToLower())
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
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _contextList = null;
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
        ~StateContextManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }
}
