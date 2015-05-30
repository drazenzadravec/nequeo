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
using System.IO;
using System.Net;
using System.Linq;
using System.Data;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Data.LinqToSql.Async
{
    /// <summary>
    /// Asynchronouns generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public sealed class AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<TLinqEntity[]>
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncSelectLinqToSqlEntities(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        public AsyncSelectLinqToSqlEntities(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, object keyValue)
            : base(callback, state)
        {
            _keyValue = keyValue;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        public AsyncSelectLinqToSqlEntities(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread3));
            Thread.Sleep(20);
        }

        private object[] _values = null;
        private string _predicate = null;
        private object _keyValue = null;
        private ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread1(Object stateInfo)
        {
            // Get the query result.
            TLinqEntity[] data = _linqDataBase.SelectLinqEntities();

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread2(Object stateInfo)
        {
            // Get the query result.
            TLinqEntity[] data = _linqDataBase.SelectLinqEntities(_keyValue);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread3(Object stateInfo)
        {
            // Get the query result.
            TLinqEntity[] data = _linqDataBase.SelectLinqEntities(_predicate, _values);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Asynchronouns generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
    public sealed class AsyncSelectLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity> :
        Nequeo.Threading.AsynchronousResult<TypeLinqEntity[]>
            where TDataContext : System.Data.Linq.DataContext, new()
            where TLinqEntity : class, new()
            where TypeLinqEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncSelectLinqToSqlEntities(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        public AsyncSelectLinqToSqlEntities(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, object keyValue)
            : base(callback, state)
        {
            _keyValue = keyValue;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        public AsyncSelectLinqToSqlEntities(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread3));
            Thread.Sleep(20);
        }

        private object[] _values = null;
        private string _predicate = null;
        private object _keyValue = null;
        private ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread1(Object stateInfo)
        {
            // Get the query result.
            TypeLinqEntity[] data = _linqDataBase.SelectLinqEntities<TypeLinqEntity>();

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread2(Object stateInfo)
        {
            // Get the query result.
            TypeLinqEntity[] data = _linqDataBase.SelectLinqEntities<TypeLinqEntity>(_keyValue);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread3(Object stateInfo)
        {
            // Get the query result.
            TypeLinqEntity[] data = _linqDataBase.SelectLinqEntities<TypeLinqEntity>(_predicate, _values);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Asynchronouns generic data table access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public sealed class AsyncSelectDataTable<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<DataTable>
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncSelectDataTable(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        public AsyncSelectDataTable(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, object keyValue)
            : base(callback, state)
        {
            _keyValue = keyValue;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        public AsyncSelectDataTable(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread3));
            Thread.Sleep(20);
        }

        private object[] _values = null;
        private string _predicate = null;
        private object _keyValue = null;
        private ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread1(Object stateInfo)
        {
            // Get the query result.
            DataTable data = _linqDataBase.SelectDataTable();

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread2(Object stateInfo)
        {
            // Get the query result.
            DataTable data = _linqDataBase.SelectDataTable(_keyValue);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread3(Object stateInfo)
        {
            // Get the query result.
            DataTable data = _linqDataBase.SelectDataTable(_predicate, _values);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Asynchronouns generic data table access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public sealed class AsyncSelectIQueryableItems<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<IQueryable<TLinqEntity>>
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncSelectIQueryableItems(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        public AsyncSelectIQueryableItems(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, object keyValue)
            : base(callback, state)
        {
            _keyValue = keyValue;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        public AsyncSelectIQueryableItems(ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncSelectThread3));
            Thread.Sleep(20);
        }

        private object[] _values = null;
        private string _predicate = null;
        private object _keyValue = null;
        private ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread1(Object stateInfo)
        {
            // Get the query result.
            IQueryable<TLinqEntity> data = _linqDataBase.SelectIQueryableItems();

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread2(Object stateInfo)
        {
            // Get the query result.
            IQueryable<TLinqEntity> data = _linqDataBase.SelectIQueryableItems(_keyValue);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncSelectThread3(Object stateInfo)
        {
            // Get the query result.
            IQueryable<TLinqEntity> data = _linqDataBase.SelectIQueryableItems(_predicate, _values);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }
}
