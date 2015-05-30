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
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Data.Common;

namespace Nequeo.Data.Control
{
    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class InformationProviderArgs : EventArgs
    {
        #region InformationProviderArgs class

        #region Constructors
        /// <summary>
        /// Constructor for the information event argument.
        /// </summary>
        /// <param name="information">The data that is received from the server.</param>
        public InformationProviderArgs(string information)
        {
            this.data = information;
        }
        #endregion

        #region Private Fields
        // The data that is received from the server.
        private string data = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the data that is received from the server.
        /// </summary>
        public string Information
        {
            get { return data; }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Disposable implementation.
    /// </summary>
    [Serializable]
    [DataContract(Name = "Disposable")]
    public class Disposable : IDisposable
    {
        #region Disposable class

        #region Private Fields
        private bool disposed = false;
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Disposable()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Cache control interface implementation members.
    /// </summary>
    public interface ICacheControl
    {
        #region ICacheControl Interface

        #region Properties
        /// <summary>
        /// Gets sets an indicator specifying that
        /// the current selected item(s) should be
        /// stored and retreived from the cache.
        /// </summary>
        bool CacheItems { get; set; }

        /// <summary>
        /// Gets sets the key name of the cached item.
        /// </summary>
        string CachedItemName { get; set; }

        /// <summary>
        /// Gets sets the length of time in seconds the item(s)
        /// are stored in the cache before being removed.
        /// </summary>
        Int32 CacheTimeout { get; set; }

        #endregion

        #region Caching Control Methods
        /// <summary>
        /// Get the specified item from the cache.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        /// <returns>The object that was cached or null if the object does not exist.</returns>
        Object GetItemFromCache(string cachedItemName);

        /// <summary>
        /// Remove the specified item from the cache.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        void RemoveItemFromCache(string cachedItemName);

        /// <summary>
        /// Adds the object to the cache.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        /// <param name="cacheTimeout">The length of time in second the item(s)
        /// are stored in the cache before being removed.</param>
        /// <param name="value">The object to add to the cache.</param>
        void AddItemToCache(string cachedItemName, Int32 cacheTimeout, Object value);

        #endregion

        #endregion
    }

    /// <summary>
    /// Information and error provider interface implementation members.
    /// </summary>
    public interface IProcessProvider
    {
        #region IProcessProvider Interface

        #region Properties
        /// <summary>
        /// Sets, The specific path of the configuration file, 
        /// used for web applications.
        /// </summary>
        String ConfigurationPath { set; }

        /// <summary>
        /// Gets the currrent error for the operation.
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Gets the current information for the operation.
        /// </summary>
        string Information { get; }

        /// <summary>
        /// Gets sets the location where errors are to be written.
        /// </summary>
        Nequeo.Handler.WriteTo ErrorWriteTo { get; set; }

        /// <summary>
        /// Gets sets the location where information is to be written.
        /// </summary>
        Nequeo.Handler.WriteTo InformationWriteTo { get; set; }

        #endregion

        #region Events
        /// <summary>
        /// Event handler for providing error information.
        /// </summary>
        event Nequeo.Threading.EventHandler<InformationProviderArgs> OnErrorProvider;

        /// <summary>
        /// Event handler for providing information.
        /// </summary>
        event Nequeo.Threading.EventHandler<InformationProviderArgs> OnInformationProvider;

        #endregion

        #region Provider Methods
        /// <summary>
        /// Provides error information in a method when called.
        /// </summary>
        /// <param name="exception">An exception that has been constructed.</param>
        /// <param name="method">The method the error occurred in.</param>
        /// <param name="methodDescription">The description of the method.</param>
        void ErrorProvider(Exception exception, string method, string methodDescription);

        /// <summary>
        /// Provides information back to the derived class.
        /// </summary>
        /// <param name="method">The method the error occurred in.</param>
        /// <param name="information">The information that is provided.</param>
        void InformationProvider(string method, string information);

        #endregion

        #endregion
    }

    /// <summary>
    /// Concurrency control interface implementation members.
    /// </summary>
    public interface IConcurrencyControl
    {
        #region IConcurrencyControl Interface

        #region Properties
        /// <summary>
        /// Gets sets, concurrency error indicator.
        /// </summary>
        bool ConcurrencyError { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    public interface IExtension<TExtension>
        where TExtension : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension Extension { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    public interface IExtension<TExtension, TExtension1>
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension Extension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 Extension1 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    public interface IExtension<TExtension, TExtension1, TExtension2>
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension Extension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 Extension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 Extension2 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    /// <typeparam name="TExtension3">The extension type.</typeparam>
    public interface IExtension<TExtension, TExtension1, TExtension2, TExtension3>
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension Extension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 Extension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 Extension2 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension3 Extension3 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    /// <typeparam name="TExtension3">The extension type.</typeparam>
    /// <typeparam name="TExtension4">The extension type.</typeparam>
    public interface IExtension<TExtension, TExtension1, TExtension2, TExtension3, TExtension4>
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension Extension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 Extension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 Extension2 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension3 Extension3 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension4 Extension4 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    /// <typeparam name="TExtension3">The extension type.</typeparam>
    /// <typeparam name="TExtension4">The extension type.</typeparam>
    /// <typeparam name="TExtension5">The extension type.</typeparam>
    public interface IExtension<TExtension, TExtension1, TExtension2, TExtension3, TExtension4, TExtension5>
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension Extension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 Extension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 Extension2 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension3 Extension3 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension4 Extension4 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension5 Extension5 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    /// <typeparam name="TExtension3">The extension type.</typeparam>
    /// <typeparam name="TExtension4">The extension type.</typeparam>
    /// <typeparam name="TExtension5">The extension type.</typeparam>
    /// <typeparam name="TExtension6">The extension type.</typeparam>
    public interface IExtension<TExtension, TExtension1, TExtension2, TExtension3, TExtension4, TExtension5, TExtension6>
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension Extension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 Extension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 Extension2 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension3 Extension3 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension4 Extension4 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension5 Extension5 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension6 Extension6 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Data entity type descriptor.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
    public class DataEntityTypeDescriptor<TDataEntity> : Nequeo.Reflection.TypeDescriptorExtender<TDataEntity>
        where TDataEntity : class, new()
    {
        #region Data Entity Type Descriptor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataEntityTypeDescriptor() : base() { }

        #endregion
    }


    /// <summary>
    /// Execute command interface
    /// </summary>
    public interface IExecuteCommand
    {
        #region Execute Command Interface
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="dbCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, params DbParameter[] values);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="dbCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        Int32 ExecuteCommand(ref DbCommand dbCommand, string commandText,
            CommandType commandType, string connectionString, params DbParameter[] values);

        #endregion
    }

    /// <summary>
    /// Execute query interface
    /// </summary>
    public interface IExecuteQuery
    {
        #region Execute Command Interface
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The tables names to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The datatable schema to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref DataTable dataTable, string queryText, CommandType commandType,
            string connectionString, bool getSchemaTable, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The tables names to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, string[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The datatable schema to add.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery(ref System.Data.DataSet dataSet, DataTable[] tables, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values);

        #endregion
    }
}
