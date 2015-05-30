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

namespace Nequeo.Data.DataSet.Async
{
    /// <summary>
    /// Asynchronouns generic dataset data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public sealed class AsyncDeleteItemPredicate<TDataContext, TDataTable> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataSetBase">The base generic dataset data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        public AsyncDeleteItemPredicate(IDeleteDataSetGenericBase<TDataContext, TDataTable> dataSetBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _dataSetBase = dataSetBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private string _predicate = null;
        private object[] _values = null;
        private IDeleteDataSetGenericBase<TDataContext, TDataTable> _dataSetBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataSetBase.DeleteItemPredicate(_predicate, _values);

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
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    /// <typeparam name="TypeDataTable">The data table entity type.</typeparam>
    public sealed class AsyncDeleteItemPredicate<TDataContext, TDataTable, TypeDataTable> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
        where TypeDataTable : System.Data.DataTable, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataSetBase">The base generic dataset data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        public AsyncDeleteItemPredicate(IDeleteDataSetGenericBase<TDataContext, TDataTable> dataSetBase,
            AsyncCallback callback, object state, string predicate, params object[] values)
            : base(callback, state)
        {
            _values = values;
            _predicate = predicate;
            _dataSetBase = dataSetBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private string _predicate = null;
        private object[] _values = null;
        private IDeleteDataSetGenericBase<TDataContext, TDataTable> _dataSetBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataSetBase.DeleteItemPredicate<TypeDataTable>(_predicate, _values);

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
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public sealed class AsyncDeleteDataTable<TDataContext, TDataTable> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataSetBase">The base generic dataset data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        public AsyncDeleteDataTable(IDeleteDataSetGenericBase<TDataContext, TDataTable> dataSetBase,
            AsyncCallback callback, object state, TDataTable dataTable)
            : base(callback, state)
        {
            _dataTable = dataTable;
            _dataSetBase = dataSetBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private TDataTable _dataTable = null;
        private IDeleteDataSetGenericBase<TDataContext, TDataTable> _dataSetBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataSetBase.DeleteDataTable(_dataTable);

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
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
    public sealed class AsyncDeleteDataTable<TDataContext, TDataTable, TypeDataTable> : Nequeo.Threading.AsynchronousResult<Boolean>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
        where TypeDataTable : System.Data.DataTable, new()
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="dataSetBase">The base generic dataset data access layer.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        public AsyncDeleteDataTable(IDeleteDataSetGenericBase<TDataContext, TDataTable> dataSetBase,
            AsyncCallback callback, object state, TypeDataTable dataTable)
            : base(callback, state)
        {
            _dataTable = dataTable;
            _dataSetBase = dataSetBase;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDeleteThread1));
            Thread.Sleep(20);
        }

        private TypeDataTable _dataTable = null;
        private IDeleteDataSetGenericBase<TDataContext, TDataTable> _dataSetBase = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDeleteThread1(Object stateInfo)
        {
            // Get the query result.
            bool data = _dataSetBase.DeleteDataTable<TypeDataTable>(_dataTable);

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
