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
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Data.Async
{
    /// <summary>
    /// Asynchronouns generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public sealed class AsyncInsertDataEntity<TDataEntity> : Nequeo.Threading.AsynchronousResult<List<Object>>
        where TDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The data row to insert.</param>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        public AsyncInsertDataEntity(IInsertDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, TDataEntity dataEntity, string identitySqlQuery)
            : base(callback, state)
        {
            _dataEntity = dataEntity;
            _identitySqlQuery = identitySqlQuery;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private TDataEntity _dataEntity = null;
        private string _identitySqlQuery = null;
        private IInsertDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            List<Object> data = _dataBase.InsertDataEntity(_dataEntity, _identitySqlQuery);

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
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
    public sealed class AsyncInsertDataEntity<TDataEntity, TypeDataEntity> : Nequeo.Threading.AsynchronousResult<List<Object>>
        where TDataEntity : class, new()
        where TypeDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The data row to insert.</param>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        public AsyncInsertDataEntity(IInsertDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, TypeDataEntity dataEntity, string identitySqlQuery)
            : base(callback, state)
        {
            _dataEntity = dataEntity;
            _identitySqlQuery = identitySqlQuery;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private TypeDataEntity _dataEntity = default(TypeDataEntity);
        private string _identitySqlQuery = null;
        private IInsertDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            List<Object> data = _dataBase.InsertDataEntity<TypeDataEntity>(_dataEntity, _identitySqlQuery);

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
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public sealed class AsyncInsertDataEntities<TDataEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The data row to insert.</param>
        /// <param name="dataEntities">The data entities to insert.</param>
        public AsyncInsertDataEntities(IInsertDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, TDataEntity[] dataEntities)
            : base(callback, state)
        {
            _dataEntities = dataEntities;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private TDataEntity[] _dataEntities = null;
        private IInsertDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataBase.InsertDataEntities(_dataEntities);

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
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
    public sealed class AsyncInsertDataEntities<TDataEntity, TypeDataEntity> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataEntity : class, new()
        where TypeDataEntity : class, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataBase">The base generic data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The data row to insert.</param>
        /// <param name="dataEntities">The data entities to insert.</param>
        public AsyncInsertDataEntities(IInsertDataGenericBase<TDataEntity> dataBase,
            AsyncCallback callback, object state, TypeDataEntity[] dataEntities)
            : base(callback, state)
        {
            _dataEntities = dataEntities;
            _dataBase = dataBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInsertThread1));
            Thread.Sleep(20);
        }

        private TypeDataEntity[] _dataEntities = null;
        private IInsertDataGenericBase<TDataEntity> _dataBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncInsertThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataBase.InsertDataEntities<TypeDataEntity>(_dataEntities);

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
