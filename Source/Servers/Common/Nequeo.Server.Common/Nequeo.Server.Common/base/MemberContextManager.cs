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
    /// Member context sorted binary search manager. Stores all member context clients and
    /// managers the searching of context items within the current machine.
    /// </summary>
    /// <remarks>This object is thread-safe.</remarks>
    public class MemberContextManager : IDisposable, Nequeo.Net.IMemberContextManager
    {
        /// <summary>
        /// Member context sorted binary search manager. Stores all member context clients and
        /// managers the searching of context items within the current machine.
        /// </summary>
        public MemberContextManager()
        {
            _contextList = new SortedDictionary<long, Nequeo.Net.IMemberContext>();
            _cachedContextList = new SortedList<string, List<Nequeo.Net.IMemberContext>>();
            _cachedTimeLength = new SortedList<string, DateTime>();
        }

        private object _lockObject = new object();

        private long _index = -1;
        private int _indexCache = -1;
        private int _maxCacheSize = 100000;
        private bool _useCache = false;

        SortedDictionary<long, Nequeo.Net.IMemberContext> _contextList = null;
        SortedList<string, List<Nequeo.Net.IMemberContext>> _cachedContextList = null;
        SortedList<string, DateTime> _cachedTimeLength = null;

        /// <summary>
        /// Gets sets, the maximum cache size used when storing client server context contacts.
        /// </summary>
        public int MaxCacheSize
        {
            get { return _maxCacheSize; }
            set { _maxCacheSize = value; }
        }

        /// <summary>
        /// Gets sets, should the cache be used.
        /// </summary>
        public bool UseCache
        {
            get { return _useCache; }
            set { _useCache = value; }
        }

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
                IEnumerable<KeyValuePair<long, Nequeo.Net.IMemberContext>> uis = _contextList.Where(u => !String.IsNullOrEmpty(u.Value.UniqueIdentifier));
                foreach (KeyValuePair<long, Nequeo.Net.IMemberContext> item in uis)
                    uiList.Add(item.Value.UniqueIdentifier);

                // Return the list.
                return uiList.ToArray();
            }
        }

        /// <summary>
        /// Are the arrays the same.
        /// </summary>
        /// <param name="existing">The exists list.</param>
        /// <param name="match">The list to match against.</param>
        /// <returns>True if the lists are the same; else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool AreTheArraysTheSame(string[] existing, string[] match)
        {
            if (existing != null && match != null)
            {
                if (existing.Length > 0 && match.Length > 0)
                {
                    // Sort the list items.
                    string[] existingSorted = Nequeo.Invention.ArrayComparer.Sort<string>(existing, false);
                    string[] matchSorted = Nequeo.Invention.ArrayComparer.Sort<string>(match, false);

                    // Get the unique hash codes.
                    string existingHash = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(String.Join("|", existingSorted));
                    string matchHash = Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(String.Join("|", matchSorted));

                    // If the hash codes are the same.
                    if (existingHash.Equals(matchHash))
                        return true;
                    else
                        return false;
                }
                else return false;
            }
            else return false;
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
                _index++;

                // Add the item.
                _contextList.Add(_index, context);
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
                    foreach (KeyValuePair<long, Nequeo.Net.IMemberContext> item in _contextList)
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
        /// Find all member context list.
        /// </summary>
        /// <returns>The context list; else null.</returns>
        public Net.IMemberContext[] FindAllMember()
        {
            lock (_lockObject)
            {
                return _contextList.Values.ToArray();
            }
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
                    // Should the cache be used.
                    if (_useCache)
                        contextList = FindInCache(uniqueIdentifiers);

                    // If null then no existing server context items exist.
                    if (contextList == null)
                    {
                        // Create a new instance.
                        contextList = new List<Nequeo.Net.IMemberContext>();

                        // For each server context within the list.
                        foreach (KeyValuePair<long, Nequeo.Net.IMemberContext> item in _contextList)
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

                        // Should the cache be used.
                        if (_useCache)
                            // Add or remove items from the cache.
                            AddRemoveCache(contextList, uniqueIdentifiers);
                    }

                    // Return the server context list else null.
                    return (contextList.Count > 0 ? contextList.ToArray() : null);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Find all context members within the cache;
        /// </summary>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        /// <returns>The context list of clients; else null;</returns>
        private List<Nequeo.Net.IMemberContext> FindInCache(string[] uniqueIdentifiers)
        {
            if (_cachedContextList.Count > 0)
            {
                if (uniqueIdentifiers != null && uniqueIdentifiers.Length > 0)
                {
                    // Get the unique has code for the identifiers.
                    string hashCode = GetHashCode(uniqueIdentifiers);

                    // Attempt to find the context list for the hash code.
                    List<Nequeo.Net.IMemberContext> contextList = null;
                    bool ret = _cachedContextList.TryGetValue(hashCode, out contextList);

                    // If the hash code was found the return the context list.
                    if (ret)
                        return contextList;
                    else
                        return null;
                }
                else return null;
            }
            else return null;
        }

        /// <summary>
        /// Add or remove items from the cache.
        /// </summary>
        /// <param name="contextList">The context list to add to the cache.</param>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        private void AddRemoveCache(List<Nequeo.Net.IMemberContext> contextList, string[] uniqueIdentifiers)
        {
            // If the maximum number of cached items is reached
            // then replace the cache item else add a new cache item.
            if (_indexCache <= _maxCacheSize)
            {
                // Get the unique has code for the identifiers.
                string hashCode = GetHashCode(uniqueIdentifiers);

                _indexCache++;

                // Add the new cache items.
                _cachedContextList.Add(hashCode, contextList);
                _cachedTimeLength.Add(hashCode, DateTime.Now);
            }
            else
            {
                _indexCache--;
                string hashCode = null;

                // Set the current difference.
                DateTime now = DateTime.Now;
                TimeSpan differenceCurrent = now.Subtract(now);

                // Find the longest time in cache.
                foreach (KeyValuePair<string, DateTime> item in _cachedTimeLength)
                {
                    // Get the difference in time.
                    TimeSpan difference = item.Value.Subtract(now);

                    // If the longest then re-assign.
                    if (difference < differenceCurrent)
                    {
                        // Get the hash code and assign.
                        hashCode = item.Key;
                        differenceCurrent = difference;
                    }
                }

                // Replace the item that is longest in the cache.
                _cachedContextList[hashCode] = contextList;
                _cachedTimeLength[hashCode] = DateTime.Now;
            }
        }

        /// <summary>
        /// Generate the hash code for the inique identifiers
        /// </summary>
        /// <param name="uniqueIdentifiers">The list of unique identifiers.</param>
        /// <returns>The hash code.</returns>
        private string GetHashCode(string[] uniqueIdentifiers)
        {
            return Nequeo.Cryptography.Hashcode.GetHashcodeSHA1(String.Join("|", uniqueIdentifiers));
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

                    if (_cachedContextList != null)
                        _cachedContextList.Clear();

                    if (_cachedTimeLength != null)
                        _cachedTimeLength.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _contextList = null;
                _cachedContextList = null;
                _cachedTimeLength = null;
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
        ~MemberContextManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }
}
