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
    public sealed class AsyncDeleteLinqToSqlEntities<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
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
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        public AsyncDeleteLinqToSqlEntities(IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, TLinqEntity[] linqEntityItems, bool useRowVersion)
            : base(callback, state)
        {
            _useRowVersion = useRowVersion;
            _linqEntityItems = linqEntityItems;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private bool _useRowVersion = false;
        private TLinqEntity[] _linqEntityItems = null;
        private IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _linqDataBase.DeleteCollection(_linqEntityItems, _useRowVersion);

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
    /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
    public sealed class AsyncDeleteLinqToSqlEntities<TDataContext, TLinqEntity, TypeLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
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
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        public AsyncDeleteLinqToSqlEntities(IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, TypeLinqEntity[] linqEntityItems, bool useRowVersion)
            : base(callback, state)
        {
            _useRowVersion = useRowVersion;
            _linqEntityItems = linqEntityItems;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private bool _useRowVersion = false;
        private TypeLinqEntity[] _linqEntityItems = null;
        private IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _linqDataBase.DeleteCollection<TypeLinqEntity>(_linqEntityItems, _useRowVersion);

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
    public sealed class AsyncDeleteCollection<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
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
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        public AsyncDeleteCollection(IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private object[] _values = null;
        private string _predicate = null;
        private IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _linqDataBase.DeleteCollectionPredicate(_predicate, _values);

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
    /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
    public sealed class AsyncDeleteCollection<TDataContext, TLinqEntity, TypeLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
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
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        public AsyncDeleteCollection(IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> linqDataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _linqDataBase = linqDataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private object[] _values = null;
        private string _predicate = null;
        private IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> _linqDataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _linqDataBase.DeleteCollectionPredicate<TypeLinqEntity>(_predicate, _values);

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
