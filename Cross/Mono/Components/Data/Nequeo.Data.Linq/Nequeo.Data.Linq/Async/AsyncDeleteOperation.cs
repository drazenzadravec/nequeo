﻿/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
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

namespace Nequeo.Data.Async
{
    /// <summary>
    /// Asynchronouns generic dataset data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public sealed class AsyncDeleteItemPredicate<TDataEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        public AsyncDeleteItemPredicate(IDeleteDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private string _predicate = null;
        private object[] _values = null;
        private IDeleteDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataBase.DeleteItemPredicate(_predicate, _values);

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
    /// Asynchronouns generic dataset data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
    public sealed class AsyncDeleteItemPredicate<TDataEntity, TypeDataEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataEntity : class, new()
        where TypeDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        public AsyncDeleteItemPredicate(IDeleteDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private string _predicate = null;
        private object[] _values = null;
        private IDeleteDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataBase.DeleteItemPredicate<TypeDataEntity>(_predicate, _values);

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
    /// Asynchronouns generic dataset data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public sealed class AsyncDeleteDataEntities<TDataEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        public AsyncDeleteDataEntities(IDeleteDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, TDataEntity[] dataEntities, bool useRowVersion)
            : base(callback, state)
        {
            _useRowVersion = useRowVersion;
            _dataEntities = dataEntities;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private bool _useRowVersion = false;
        private TDataEntity[] _dataEntities = null;
        private IDeleteDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataBase.DeleteDataEntities(_dataEntities, _useRowVersion);

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
    /// Asynchronouns generic dataset data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
    public sealed class AsyncDeleteDataEntities<TDataEntity, TypeDataEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataEntity : class, new()
        where TypeDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        public AsyncDeleteDataEntities(IDeleteDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, TypeDataEntity[] dataEntities, bool useRowVersion)
            : base(callback, state)
        {
            _useRowVersion = useRowVersion;
            _dataEntities = dataEntities;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private bool _useRowVersion = false;
        private TypeDataEntity[] _dataEntities = null;
        private IDeleteDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataBase.DeleteDataEntities<TypeDataEntity>(_dataEntities, _useRowVersion);

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
