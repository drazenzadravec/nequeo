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
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Nequeo.Data.Extension
{
    /// <summary>
    /// DbCommand extension.
    /// </summary>
    public static class DbCommandExtensions
    {
        /// <summary>
        /// Asyncronous load data command
        /// </summary>
        /// <param name="source">The source object</param>
        /// <returns>The async task</returns>
        public static Task<DataTable> LoadAsync(this DbCommand source)
        {
            // Create the new async operation
            AsyncLoadDataTable ret = new AsyncLoadDataTable(source, null, null);
            ret.OnError += new Threading.EventHandler<Exception>(ret_OnError_LoadAsync);

            // Return the async task.
            return Task<DataTable>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
        }

        /// <summary>
        /// Error handler event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e1">The current error.</param>
        internal static void ret_OnError_LoadAsync(object sender, Exception e1)
        {
        }

        /// <summary>
        /// Asyncronous execute non query command
        /// </summary>
        /// <param name="source">The source object</param>
        /// <returns>The async task</returns>
        public static Task<Int32> NonQueryAsync(this DbCommand source)
        {
            // Create the new async operation
            AsyncExecuteCommand ret = new AsyncExecuteCommand(source, null, null);
            ret.OnError += new Threading.EventHandler<Exception>(ret_OnError_NonQueryAsync);

            // Return the async task.
            return Task<Int32>.Factory.FromAsync(ret.BeginExecute(), ret.EndExecute);
        }

        /// <summary>
        /// Error handler event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e1">The current error.</param>
        internal static void ret_OnError_NonQueryAsync(object sender, Exception e1)
        {
        }
    }

    /// <summary>
    /// Asyncronous load data command
    /// </summary>
    internal sealed class AsyncLoadDataTable : Nequeo.Threading.AsynchronousResult<DataTable>
    {
        /// <summary>
        /// Async load data
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="callback">The callback</param>
        /// <param name="state">The sate</param>
        public AsyncLoadDataTable(DbCommand command, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _command = command;
        }

        private DbCommand _command = null;
        private Func<DataTable> _loadHandler = null;
        private Exception _exception = null;

        /// <summary>
        /// Gets the current execution exception
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Load error.
        /// </summary>
        public event Nequeo.Threading.EventHandler<System.Exception> OnError;

        /// <summary>
        /// Begin the async load.
        /// </summary>
        /// <returns>The async result</returns>
        public IAsyncResult BeginLoad()
        {
            if (_loadHandler == null)
                _loadHandler = new Func<DataTable>(FuncAsyncLoad);

            // Begin the async call.
            return _loadHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End the async load.
        /// </summary>
        /// <param name="ar">The async result</param>
        /// <returns>The data table.</returns>
        public DataTable EndLoad(IAsyncResult ar)
        {
            if (_loadHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            return _loadHandler.EndInvoke(ar);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        internal DataTable FuncAsyncLoad()
        {
            DataTable data = null;
            IDataReader dataReader = null;

            try
            {
                // Create a new connection.
                using (_command)
                {
                    // Open the connection.
                    _command.Connection.Open();

                    // Load the data into the table.
                    using (dataReader = _command.ExecuteReader())
                    {
                        // Load the table after the schema is
                        // returned.
                        data = dataReader.GetSchemaTable();
                        data = new DataTable("TableData");
                        System.Data.DataSet localDataSet = new System.Data.DataSet();
                        localDataSet.EnforceConstraints = false;
                        localDataSet.Tables.Add(data);
                        data.Load(dataReader);

                        // Close the data reader.
                        dataReader.Close();
                    }

                    // Close the database connection.
                    _command.Connection.Close();
                }
            }
            catch(Exception ex)
            {
                _exception = ex;
                if (OnError != null)
                    OnError(this, ex);
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();

                if (_command.Connection != null)
                    _command.Connection.Close();
            }

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);

            // Return the data table.
            return data;
        }
    }

    /// <summary>
    /// Asyncronous execute non query command
    /// </summary>
    internal sealed class AsyncExecuteCommand : Nequeo.Threading.AsynchronousResult<Int32>
    {
        /// <summary>
        /// Async load data
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="callback">The callback</param>
        /// <param name="state">The sate</param>
        public AsyncExecuteCommand(DbCommand command, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _command = command;
        }

        private DbCommand _command = null;
        private Func<Int32> loadHandler = null;
        private Exception _exception = null;

        /// <summary>
        /// Gets the current execution exception
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Load error.
        /// </summary>
        public event Nequeo.Threading.EventHandler<System.Exception> OnError;

        /// <summary>
        /// Begin the async execute.
        /// </summary>
        /// <returns></returns>
        public IAsyncResult BeginExecute()
        {
            if (loadHandler == null)
                loadHandler = new Func<Int32>(FuncAsyncExecute);

            // Begin the async call.
            return loadHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End the async execute.
        /// </summary>
        /// <param name="ar">The async result</param>
        /// <returns>The data table.</returns>
        public Int32 EndExecute(IAsyncResult ar)
        {
            if (loadHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            return loadHandler.EndInvoke(ar);
        }

        /// <summary>
        /// The async query request method.
        /// </summary>
        internal Int32 FuncAsyncExecute()
        {
            Int32 data = -1;
            DbTransaction transaction = null;

            try
            {
                // Create a new connection.
                using (_command)
                {
                    // Open the connection.
                    _command.Connection.Open();

                    // Start a new transaction.
                    transaction = _command.Connection.BeginTransaction();
                    _command.Transaction = transaction;

                    // Execute the command.
                    data = _command.ExecuteNonQuery();

                    // Commit the transaction.
                    transaction.Commit();

                    // Close the database connection.
                    _command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                if (OnError != null)
                    OnError(this, ex);

                try
                {
                    // Attempt to roll back the transaction.
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch { }
            }
            finally
            {
                if (_command.Connection != null)
                    _command.Connection.Close();
            }

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data > -1)
                base.Complete(data, true);
            else
                base.Complete(false);

            // Return the data table.
            return data;
        }
    }
}
