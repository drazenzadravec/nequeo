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
using System.Data.Objects;
using System.Linq.Expressions;

namespace Nequeo.Data.Edm.Async
{
    /// <summary>
    /// Asynchronouns generic edm data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public sealed class AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataEdm">The base generic edm data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        public AsyncDeleteCollectionPredicate(IDeleteEdmGenericBase<TDataContext, TLinqEntity> dataEdm,
            AsyncCallback callback, object state, string predicate, params ObjectParameter[] parameters)
            : base(callback, state)
        {
            _parameters = parameters;
            _predicate = predicate;
            _dataEdm = dataEdm;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataEdm">The base generic edm data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        public AsyncDeleteCollectionPredicate(IDeleteEdmGenericBase<TDataContext, TLinqEntity> dataEdm,
            AsyncCallback callback, object state, Expression<Func<TLinqEntity, bool>> predicate)
            : base(callback, state)
        {
            _predicateExpression = predicate;
            _dataEdm = dataEdm;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread2));
            Thread.Sleep(20);
        }

        private Expression<Func<TLinqEntity, bool>> _predicateExpression = null;
        private string _predicate = null;
        private ObjectParameter[] _parameters = null;
        private IDeleteEdmGenericBase<TDataContext, TLinqEntity> _dataEdm = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataEdm.DeleteCollectionPredicate(_predicate, _parameters);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread2(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataEdm.DeleteCollectionPredicate(_predicateExpression);

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
    /// Asynchronouns generic edm data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    /// <typeparam name="TypeLinqEntity">The linq entity type.</typeparam>
    public sealed class AsyncDeleteCollectionPredicate<TDataContext, TLinqEntity, TypeLinqEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
        where TypeLinqEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataEdm">The base generic edm data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        public AsyncDeleteCollectionPredicate(IDeleteEdmGenericBase<TDataContext, TLinqEntity> dataEdm,
            AsyncCallback callback, object state, string predicate, params ObjectParameter[] parameters)
            : base(callback, state)
        {
            _parameters = parameters;
            _predicate = predicate;
            _dataEdm = dataEdm;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataEdm">The base generic edm data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        public AsyncDeleteCollectionPredicate(IDeleteEdmGenericBase<TDataContext, TLinqEntity> dataEdm,
            AsyncCallback callback, object state, Expression<Func<TypeLinqEntity, bool>> predicate)
            : base(callback, state)
        {
            _predicateExpression = predicate;
            _dataEdm = dataEdm;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread2));
            Thread.Sleep(20);
        }

        private Expression<Func<TypeLinqEntity, bool>> _predicateExpression = null;
        private string _predicate = null;
        private ObjectParameter[] _parameters = null;
        private IDeleteEdmGenericBase<TDataContext, TLinqEntity> _dataEdm = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataEdm.DeleteCollectionPredicate<TypeLinqEntity>(_predicate, _parameters);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
                base.Complete(data, true);
            else
                base.Complete(false);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread2(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataEdm.DeleteCollectionPredicate<TypeLinqEntity>(_predicateExpression);

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
