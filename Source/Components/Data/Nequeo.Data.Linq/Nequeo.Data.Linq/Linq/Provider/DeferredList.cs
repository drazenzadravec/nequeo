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
using System.Collections;
using System.Collections.Generic;

using Nequeo.Data.Linq;
using Nequeo.Data.Linq.Common;
using Nequeo.Data.Linq.Language;
using Nequeo.Data.Linq.Provider;
using Nequeo.Data.Linq.Common.Expressions;
using Nequeo.Data.Linq.Common.Translation;

namespace Nequeo.Data.Linq.Provider
{
    /// <summary>
    /// Common interface for controlling defer-loadable types
    /// </summary>
    public interface IDeferLoadable
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsLoaded { get; }
        /// <summary>
        /// 
        /// </summary>
        void Load();
    }

    /// <summary>
    /// A list implementation that is loaded the first the contents are examined
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeferredList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IDeferLoadable
    {
        IEnumerable<T> source;
        List<T> values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public DeferredList(IEnumerable<T> source)
        {
            this.source = source;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            this.values = new List<T>(this.source);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLoaded
        {
            get { return this.values != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Check()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }
        }

        #region IList<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            this.Check();
            return this.values.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            this.Check();
            this.values.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.Check();
            this.values.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                this.Check();
                return this.values[index];
            }
            set
            {
                this.Check();
                this.values[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            this.Check();
            this.values.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.Check();
            this.values.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            this.Check();
            return this.values.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Check();
            this.values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { this.Check(); return this.values.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            this.Check();
            return this.values.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            this.Check();
            return this.values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(object value)
        {
            this.Check();
            return ((IList)this.values).Add(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            this.Check();
            return ((IList)this.values).Contains(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object value)
        {
            this.Check();
            return ((IList)this.values).IndexOf(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, object value)
        {
            this.Check();
            ((IList)this.values).Insert(index, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value)
        {
            this.Check();
            ((IList)this.values).Remove(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get
            {
                this.Check();
                return ((IList)this.values)[index];
            }
            set
            {
                this.Check();
                ((IList)this.values)[index] = value;
            }
        }

        #endregion

        #region ICollection Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            this.Check();
            ((IList)this.values).CopyTo(array, index);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public object SyncRoot
        {
            get { return null; }
        }

        #endregion
    }
}