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
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using System.Data.Common;
using System.Threading.Tasks;

using Nequeo.Data.Extension;
using Nequeo.Data.DataType;
using Nequeo.Data;

namespace Nequeo.Data.Custom
{
    /// <summary>
    /// Custom data access class.
    /// </summary>
    public sealed class DataAccess
    {
        /// <summary>
        /// Execute an SQL query directly to the database.
        /// </summary>
        /// <typeparam name="TEntity">The type to examine.</typeparam>
        /// <param name="dataContext">The data context to the database.</param>
        /// <param name="sqlQuery">The sql command to execute to the database.</param>
        /// <param name="values">The parameter values for the command, can be null.</param>
        /// <returns>The enumerable collection type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(
            System.Data.Linq.DataContext dataContext, string sqlQuery, params object[] values)
            where TEntity : class, new()
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (sqlQuery == null) throw new ArgumentNullException("sqlQuery");

            // Execute the query.
            IEnumerable<TEntity> query =
                dataContext.ExecuteQuery<TEntity>(sqlQuery, values);

            // Return the enumerable result
            // of the specified type.
            return query;
        }

        /// <summary>
        /// Execute an SQL command directly to the database.
        /// </summary>
        /// <param name="dataContext">The data context to the database.</param>
        /// <param name="sqlCommand">The sql command to execute to the database.</param>
        /// <param name="values">The parameter values for the command, can be null.</param>
        /// <returns>A value indicating the result of the command.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static Int32 ExecuteCommand(System.Data.Linq.DataContext dataContext,
            string sqlCommand, params object[] values)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (sqlCommand == null) throw new ArgumentNullException("sqlCommand");

            // Execute the command and return
            // the result from the command.
            return dataContext.ExecuteCommand(sqlCommand, values);
        }

        /// <summary>
        /// Load data.
        /// </summary>
        /// <param name="command">The complete command</param>
        /// <returns>The data table containing the data.</returns>
        public static DataTable ExecuteQuery(DbCommand command)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (command == null) throw new ArgumentNullException("command");

            // Create the new async operation
            AsyncLoadDataTable ret = new AsyncLoadDataTable(command, null, null);
            DataTable data = ret.FuncAsyncLoad();

            // If an error occures then thow the excption.
            if (ret.Exception != null)
                throw new Exception(ret.Exception.Message, ret.Exception.InnerException);

            return data;
        }

        /// <summary>
        /// Load data asyncronously.
        /// </summary>
        /// <param name="command">The complete command</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="callbackException">The callback action exception handler.</param>
        /// <param name="state">The action state.</param>
        public static async void ExecuteQuery(DbCommand command,
            Action<Nequeo.Threading.AsyncOperationResult<DataTable>> callback, 
            Action<Nequeo.Threading.AsyncOperationResult<System.Exception>> callbackException, object state = null)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (command == null) throw new ArgumentNullException("command");
            if (callback == null) throw new ArgumentNullException("callback");

            AsyncLoadDataTable ret = new AsyncLoadDataTable(command, null, null);
            Task<DataTable> data = Task<DataTable>.Factory.FromAsync(ret.BeginLoad(), ret.EndLoad);
            DataTable dataTableAsyncResult = await data;

            if (callback != null)
                callback(new Nequeo.Threading.AsyncOperationResult<DataTable>(dataTableAsyncResult, state, "ExecuteQuery"));

            if (callbackException != null)
                callbackException(new Nequeo.Threading.AsyncOperationResult<System.Exception>(ret.Exception, state, "ExecuteQuery"));
        }

        /// <summary>
        /// Execute non query command.
        /// </summary>
        /// <param name="command">The complete command</param>
        /// <returns>The data table containing the data.</returns>
        public static Int32 ExecuteCommand(DbCommand command)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (command == null) throw new ArgumentNullException("command");

            // Create the new async operation
            AsyncExecuteCommand ret = new AsyncExecuteCommand(command, null, null);
            Int32 data = ret.FuncAsyncExecute();

            // If an error occures then thow the excption.
            if (ret.Exception != null)
                throw new Exception(ret.Exception.Message, ret.Exception.InnerException);

            return data;
        }

        /// <summary>
        /// Execute non query command asyncronously.
        /// </summary>
        /// <param name="command">The complete command</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="callbackException">The callback action exception handler.</param>
        /// <param name="state">The action state.</param>
        public static async void ExecuteCommand(DbCommand command, 
            Action<Nequeo.Threading.AsyncOperationResult<Int32>> callback, 
            Action<Nequeo.Threading.AsyncOperationResult<System.Exception>> callbackException, object state = null)
        {
            // Check to see if the critical parameters
            // have been set, throw exception on each.
            if (command == null) throw new ArgumentNullException("command");
            if (callback == null) throw new ArgumentNullException("callback");

            AsyncExecuteCommand ret = new AsyncExecuteCommand(command, null, null);
            System.Threading.Tasks.Task<Int32> data = Task<Int32>.Factory.FromAsync(ret.BeginExecute(), ret.EndExecute);
            Int32 commandAsyncResult = await data;

            if (callback != null)
                callback(new Nequeo.Threading.AsyncOperationResult<Int32>(commandAsyncResult, state, "ExecuteCommand"));

            if (callbackException != null)
                callbackException(new Nequeo.Threading.AsyncOperationResult<System.Exception>(ret.Exception, state, "ExecuteCommand"));
        }
    }
}
