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
    public sealed class AsyncInsertLinqToSqlEntities<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
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
        /// <param name="linqEntities">The linq entities to insert.</param>
        public AsyncInsertLinqToSqlEntities(IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, TLinqEntity[] linqEntities)
            : base(callback, state)
        {
            _linqEntityItems = linqEntities;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private TLinqEntity[] _linqEntityItems = null;
        private IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _linqDataBase.InsertLinqEntities(_linqEntityItems);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
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
    public sealed class AsyncInsertDataTable<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
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
        /// <param name="dataTable">The data table.</param>
        public AsyncInsertDataTable(IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, DataTable dataTable)
            : base(callback, state)
        {
            _dataTable = dataTable;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private DataTable _dataTable = null;
        private IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _linqDataBase.InsertDataTable(_dataTable);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
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
    public sealed class AsyncInsertCollection<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<TLinqEntity[]>
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
        /// <param name="linqEntities">The linq entities to insert.</param>
        public AsyncInsertCollection(IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, TLinqEntity[] linqEntities)
            : base(callback, state)
        {
            _linqEntityItems = linqEntities;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread2));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="linqDataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table.</param>
        public AsyncInsertCollection(IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, DataTable dataTable)
            : base(callback, state)
        {
            _dataTable = dataTable;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread3));
            Thread.Sleep(20);
        }

        private DataTable _dataTable = null;
        private TLinqEntity[] _linqEntityItems = null;
        private IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread2(Object stateInfo)
        {
            // Get the query result.
            TLinqEntity[] data = _linqDataBase.InsertCollection(_linqEntityItems);

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
        private void AsyncInsertThread3(Object stateInfo)
        {
            // Get the query result.
            TLinqEntity[] data = _linqDataBase.InsertCollection(_dataTable);

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
    public sealed class AsyncInsertCollection<TDataContext, TLinqEntity, TypeLinqEntity> : Nequeo.Threading.AsynchronousResult<TypeLinqEntity[]>
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
        /// <param name="linqEntities">The linq entities to insert.</param>
        public AsyncInsertCollection(IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, TypeLinqEntity[] linqEntities)
            : base(callback, state)
        {
            _linqEntityItems = linqEntities;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private TypeLinqEntity[] _linqEntityItems = null;
        private IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            TypeLinqEntity[] data = _linqDataBase.InsertCollection<TypeLinqEntity>(_linqEntityItems);

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
    public sealed class AsyncInsertLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
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
        /// <param name="linqEntities">The linq entities to insert.</param>
        public AsyncInsertLinqToSqlEntities(IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, TypeLinqEntity[] linqEntities)
            : base(callback, state)
        {
            _linqEntityItems = linqEntities;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private TypeLinqEntity[] _linqEntityItems = null;
        private IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _linqDataBase.InsertLinqEntities<TypeLinqEntity>(_linqEntityItems);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }
}
