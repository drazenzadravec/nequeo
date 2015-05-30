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
using System.Reflection.Emit;
using System.Data.Common;

using Nequeo.Data.Control;
using Nequeo.Data.DataType;

namespace Nequeo.Data.DataSet
{
    /// <summary>
    /// The interface linq base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IDataSetBase<TDataContext, TDataTable> : IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IDataSetBase Interface

        #region Select Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectDataSetGenericBase<TDataContext, TDataTable> Select { get; }

        #endregion

        #region Delete Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IDeleteDataSetGenericBase<TDataContext, TDataTable> Delete { get; }

        #endregion

        #region Insert Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IInsertDataSetGenericBase<TDataContext, TDataTable> Insert { get; }

        #endregion

        #region Update Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IUpdateDataSetGenericBase<TDataContext, TDataTable> Update { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The interface linq base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IDataAnonymousSetGenericBase<TDataContext, TDataTable> : IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IDataAnonymousSetGenericBase Interface

        #region Select Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectDataSetGenericBase<TDataContext, TDataTable> Select { get; }

        #endregion

        #region Delete Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IDeleteDataSetGenericBase<TDataContext, TDataTable> Delete { get; }

        #endregion

        #region Insert Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IInsertDataSetGenericBase<TDataContext, TDataTable> Insert { get; }

        #endregion

        #region Update Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IUpdateDataSetGenericBase<TDataContext, TDataTable> Update { get; }

        #endregion

        #region DataSet Base Properties
        /// <summary>
        /// Gets, the data generic base members.
        /// </summary>
        IDataSetDataGenericBase<TDataContext, TDataTable> Common { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The interface linq base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IDataSetGenericBase<TDataContext, TDataTable> : IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IDataSetGenericBase Interface

        #region Select Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectDataSetGenericBase<TDataContext, TDataTable> Select { get; }

        #endregion

        #region Delete Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IDeleteDataSetGenericBase<TDataContext, TDataTable> Delete { get; }

        #endregion

        #region Insert Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IInsertDataSetGenericBase<TDataContext, TDataTable> Insert { get; }

        #endregion

        #region Update Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        IUpdateDataSetGenericBase<TDataContext, TDataTable> Update { get; }

        #endregion

        #region DataSet Base Properties
        /// <summary>
        /// Gets, the data generic base members.
        /// </summary>
        IDataSetDataGenericBase<TDataContext, TDataTable> Common { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IDataSetDataGenericBase<TDataContext, TDataTable> :
        IDataSetContextProvider<TDataContext>, IProcessProvider, IExecuteQuery, IExecuteCommand, IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IDataSetDataGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the database configuration key containing
        /// the database connection string.
        /// </summary>
        String ConfigurationDatabaseConnection { get; set; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        #endregion

        #region Reflection, Attribute Methods
        /// <summary>
        /// Get all fields within the current type.
        /// </summary>
        /// <param name="t">The current type to retreive fields within.</param>
        /// <returns>The collection of all fields within the type.</returns>
        List<FieldInfo> GetFields(Type t);

        /// <summary>
        /// Get all properties within the current type.
        /// </summary>
        /// <param name="t">The current type to retreive properties within.</param>
        /// <returns>The collection of all properties within the type.</returns>
        List<PropertyInfo> GetProperties(Type t);
        
        /// <summary>
        /// Get all columns in the current type that are primary keys.
        /// </summary>
        /// <returns>Collection of columns that are primary keys.</returns>
        DataColumn[] GetAllPrimaryKeys();
        
        /// <summary>
        /// Get all properties in the current type that are primary keys.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>Collection of properties that are primary keys.</returns>
        DataColumn[] GetAllPrimaryKeys<T>()
            where T : System.Data.DataTable, new();
        
        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        DataColumn GetPrimaryKey();
        
        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        DataColumn GetPrimaryKey<T>()
            where T : System.Data.DataTable, new();
        
        /// <summary>
        /// Is the current column value a primary key.
        /// </summary>
        /// <param name="column">The current column to examine.</param>
        /// <returns>True if the column is a primary key.</returns>
        bool IsPrimaryKey(DataColumn column);
        
        /// <summary>
        /// Is the current column value a primary key.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <param name="column">The current column to examine.</param>
        /// <returns>True if the column is a primary key.</returns>
        bool IsPrimaryKey<T>(DataColumn column)
            where T : System.Data.DataTable, new();

        #endregion

        #region Convertion Methods

        /// <summary>
        /// Convert all the object data into a data table.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <returns>The data table with IQueryable anonymous types.</returns>
        DataTable ConvertToDataTable(IQueryable query, string tableName);

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="data">The object array containing the data.</param>
        /// <returns>The array of the type of object.</returns>
        T[] ConvertToTypeArray<T>(Object[] data)
            where T : new();

        /// <summary>
        /// Convert all the object data into a data array collection
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of the type of object.</returns>
        T[] ConvertToTypeArray<T>(IQueryable query)
            where T : new();

        /// <summary>
        /// Converts a data table to the specified type array.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="dataTable">The data table to convert.</param>
        /// <returns>The array of the type.</returns>
        T[] ConvertToTypeArray<T>(DataTable dataTable)
            where T : new();
        
        /// <summary>
        /// Convert all the object data into a IQueryable generic type.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="data">The object array containing the data.</param>
        /// <returns>The IQueryable generic type.</returns>
        IQueryable<T> ConvertToIQueryable<T>(Object[] data)
            where T : new();

        #endregion

        #region Public Connection Methods
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string DefaultConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Gets the alternative database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string AlternativeConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartDefaultConnection(string databaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartAlternativeConnection(string databaseConnection);

        #endregion

        #endregion
    }

    /// <summary>
    /// The select interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface ISelectDataSetGenericBase<TDataContext, TDataTable> :
        IDataSetContextProvider<TDataContext>, ICacheControl, IProcessProvider,
        IDataSetCacheControl<TDataContext, TDataTable>, IExecuteQuery, IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region ISelectDataSetGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the database configuration key containing
        /// the database connection string.
        /// </summary>
        String ConfigurationDatabaseConnection { get; set; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        /// <summary>
        /// Gets the data table containing the
        /// collection of table data.
        /// </summary>
        TDataTable DataTable { get; }

        #endregion

        #region Select DataTable Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataTable SelectDataTable(string queryText, CommandType commandType,
            params DbParameter[] values);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataTable SelectDataTable<TypeDataTable>(string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();
       
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataTable SelectDataTable(string queryText);
       
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataTable SelectDataTable<TypeDataTable>(
            string queryText)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataTable SelectDataTable();
        
        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataTable SelectDataTable<TypeDataTable>()
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Select Table Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>True if data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectTable(string queryText, CommandType commandType,
            params DbParameter[] values);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>True if data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectTable(string queryText);
       
        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <returns>True if data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectTable();
        
        #endregion

        #region Select Collection Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollection(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollection<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollection(ref TDataTable dataTable,
            string queryText);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollection<TypeDataTable>(ref TypeDataTable dataTable,
            string queryText)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollection(ref TDataTable dataTable);
        
        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollection<TypeDataTable>(ref TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Select Predicate Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TDataRow">The current data row type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data table type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataTable SelectDataTablePredicate<TDataRow>(
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataRow>> predicate)
            where TDataRow : System.Data.DataRow;

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <typeparam name="TypeDataRow">The current data row type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data table type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataTable SelectDataTablePredicate<TypeDataTable, TypeDataRow>(
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataRow>> predicate)
            where TypeDataTable : System.Data.DataTable, new()
            where TypeDataRow : System.Data.DataRow;

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataTable SelectDataTablePredicate(
            string predicate, params object[] values);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataTable SelectDataTablePredicate<TypeDataTable>(
            string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollectionPredicate(ref TDataTable dataTable,
            string predicate, params object[] values);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollectionPredicate<TypeDataTable>(ref TypeDataTable dataTable,
            string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Select Query Methods
        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values);
        
        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Select the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Int32 SelectCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values);
        
        #endregion

        #region Asynchronous Select Methods
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TDataRow">The current data row type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTableRowPredicate<TDataRow>(AsyncCallback callback, object state,
            Expression<Nequeo.Threading.FunctionHandler<bool, TDataRow>> predicate)
            where TDataRow : System.Data.DataRow;

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TDataRow">The current data row type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TDataTable EndSelectDataTableRowPredicate<TDataRow>(IAsyncResult ar)
            where TDataRow : System.Data.DataRow;

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <typeparam name="TypeDataRow">The current data row type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTableRowPredicate<TypeDataTable, TypeDataRow>(AsyncCallback callback, object state,
            Expression<Nequeo.Threading.FunctionHandler<bool, TypeDataRow>> predicate)
            where TypeDataTable : System.Data.DataTable, new()
            where TypeDataRow : System.Data.DataRow;

        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <typeparam name="TypeDataRow">The current data row type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TypeDataTable EndSelectDataTableRowPredicate<TypeDataTable, TypeDataRow>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new()
            where TypeDataRow : System.Data.DataRow;

        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTablePredicate(AsyncCallback callback, object state,
            string predicate, params object[] values);
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TDataTable EndSelectDataTablePredicate(IAsyncResult ar);
       
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTablePredicate<TypeDataTable>(AsyncCallback callback, object state,
            string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TypeDataTable EndSelectDataTablePredicate<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values);
       
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state,
            string queryText);
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state);
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TDataTable EndSelectDataTable(IAsyncResult ar);
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable<TypeDataTable>(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable<TypeDataTable>(AsyncCallback callback, object state,
            string queryText)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable<TypeDataTable>(AsyncCallback callback, object state)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TypeDataTable EndSelectDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Public Connection Methods
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string DefaultConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Gets the alternative database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string AlternativeConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartDefaultConnection(string databaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartAlternativeConnection(string databaseConnection);

        #endregion
        
        #endregion
    }

    /// <summary>
    /// The delete interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IDeleteDataSetGenericBase<TDataContext, TDataTable> :
        IDataSetContextProvider<TDataContext>, IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IDeleteDataSetGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the database configuration key containing
        /// the database connection string.
        /// </summary>
        String ConfigurationDatabaseConnection { get; set; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        #endregion

        #region Delete Item Methods
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey(object keyValue, string keyName);
        
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey<TypeDataTable>(object keyValue, string keyName)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey(object keyValue, string keyName,
            object rowVersionData, string rowVersionName);
       
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey<TypeDataTable>(object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Delete Predicate Methods
        
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate(string predicate, params object[] values);
        
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate<TypeDataTable>(string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Delete Collection Methods
        /// <summary>
        /// Deletes the data table from the database.
        /// </summary>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteDataTable(TDataTable dataTable);
        
        /// <summary>
        /// Deletes the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteDataTable<TypeDataTable>(TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Delete Query Methods
        /// <summary>
        /// Delete the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand DeleteQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values);
        
        /// <summary>
        /// Delete the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand DeleteQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Delete the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Int32 DeleteCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values);
        
        #endregion

        #region Asynchronous Delete Methods
        /// <summary>
        /// Begin deletes the specified item.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteItemPredicate(AsyncCallback callback,
            object state, string predicate, params object[] values);

        /// <summary>
        /// End deletes the specified item.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was deleted else false.</returns>
        Boolean EndDeleteItemPredicate(IAsyncResult ar);

        /// <summary>
        /// Begin deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteItemPredicate<TypeDataTable>(AsyncCallback callback,
            object state, string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// End deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was deleted else false.</returns>
        Boolean EndDeleteItemPredicate<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// Begin deletes the data table from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteDataTable(AsyncCallback callback,
            object state, TDataTable dataTable);

        /// <summary>
        /// End deletes the data table from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        Boolean EndDeleteDataTable(IAsyncResult ar);

        /// <summary>
        /// Begin deletes the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteDataTable<TypeDataTable>(AsyncCallback callback,
            object state, TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// End deletes the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        Boolean EndDeleteDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();

        #endregion

        #region Public Connection Methods
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string DefaultConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Gets the alternative database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string AlternativeConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartDefaultConnection(string databaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartAlternativeConnection(string databaseConnection);

        #endregion

        #endregion
    }

    /// <summary>
    /// The insert interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IInsertDataSetGenericBase<TDataContext, TDataTable> :
        IDataSetContextProvider<TDataContext>, IProcessProvider, IExecuteCommand, IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IInsertDataSetGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the database configuration key containing
        /// the database connection string.
        /// </summary>
        String ConfigurationDatabaseConnection { get; set; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        #endregion

        #region Insert Collection Methods
        /// <summary>
        /// Inserts the specified data table.
        /// </summary>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertDataTable(TDataTable dataTable);
        
        /// <summary>
        /// Inserts the specified data table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertDataTable<TypeDataTable>(TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Insert Item Methods

        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <param name="dataRow">The data row to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertItem(DataRow dataRow);
        
        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertItem<TypeDataTable>(DataRow dataRow)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Insert Identity Methods
        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<object> InsertDataRow(DataRow dataRow, string identitySqlQuery);

        /// <summary>
        /// Inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<object> InsertDataRow<TypeDataTable>(DataRow dataRow, string identitySqlQuery)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Insert Query Methods
        /// <summary>
        /// Insert the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand InsertQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values);
        
        /// <summary>
        /// Insert the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand InsertQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();
       
        /// <summary>
        /// Insert the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Int32 InserCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values);
        
        #endregion

        #region Asynchronous Insert Methods
        /// <summary>
        /// Begin inserts the specified data row.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataRow(AsyncCallback callback,
            object state, DataRow dataRow, string identitySqlQuery);

        /// <summary>
        /// End inserts the specified data row.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        List<Object> EndInsertDataRow(IAsyncResult ar);

        /// <summary>
        /// Begin inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataRow<TypeDataTable>(AsyncCallback callback,
            object state, DataRow dataRow, string identitySqlQuery)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// End inserts the specified data row.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        List<Object> EndInsertDataRow<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// Begin inserts the specified data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataTable(AsyncCallback callback,
            object state, TDataTable dataTable);
        
        /// <summary>
        /// End inserts the specified data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        Boolean EndInsertDataTable(IAsyncResult ar);

        /// <summary>
        /// Begin inserts the specified data table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to insert.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataTable<TypeDataTable>(AsyncCallback callback,
            object state, TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new();

        /// <summary>
        /// End inserts the specified data table.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        Boolean EndInsertDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Public Connection Methods
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string DefaultConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Gets the alternative database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string AlternativeConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartDefaultConnection(string databaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartAlternativeConnection(string databaseConnection);

        #endregion

        #endregion
    }

    /// <summary>
    /// The update interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IUpdateDataSetGenericBase<TDataContext, TDataTable> :
        IDataSetContextProvider<TDataContext>, IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IUpdateDataSetGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the database configuration key containing
        /// the database connection string.
        /// </summary>
        String ConfigurationDatabaseConnection { get; set; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        #endregion

        #region Update Item Methods
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(DataRow dataRow, object keyValue, string keyName);
        
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data row type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeDataTable>(
            DataRow dataRow, object keyValue, string keyName)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(DataRow dataRow, object keyValue, string keyName,
            object rowVersionData, string rowVersionName);
        
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data row type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeDataTable>(DataRow dataRow, object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Update Predicate Methods
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(
            DataRow dataRow, string predicate, params object[] values);
       
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeDataTable>(
            DataRow dataRow, string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Update Collection Methods
        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateDataTable(TDataTable dataTable);
        
        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateDataTable<TypeDataTable>(TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new();
        
        #endregion

        #region Update Query Methods
        /// <summary>
        /// Update the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand UpdateQueryItem(ref TDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values);
        
        /// <summary>
        /// Update the item through the query text.
        /// </summary>
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand UpdateQueryItem<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// Update the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Int32 UpdateCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values);
        
        #endregion

        #region Asynchronous Update Methods
        /// <summary>
        /// Begin updates the specified item.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateItemPredicate(AsyncCallback callback,
            object state, DataRow dataRow, string predicate, params object[] values);
        
        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        Boolean EndUpdateItemPredicate(IAsyncResult ar);
        
        /// <summary>
        /// Begin updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateItemPredicate<TypeDataTable>(AsyncCallback callback,
            object state, DataRow dataRow, string predicate, params object[] values)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        Boolean EndUpdateItemPredicate<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();
       
        /// <summary>
        /// Begin updates the data table from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateDataTable(AsyncCallback callback,
            object state, TDataTable dataTable);
        
        /// <summary>
        /// End updates the data table from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        Boolean EndUpdateDataTable(IAsyncResult ar);
        
        /// <summary>
        /// Begin updates the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to delete.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateDataTable<TypeDataTable>(AsyncCallback callback,
            object state, TypeDataTable dataTable)
            where TypeDataTable : System.Data.DataTable, new();
        
        /// <summary>
        /// End updates the data table from the database.
        /// </summary>
        /// <typeparam name="TypeDataTable">The data table type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        Boolean EndUpdateDataTable<TypeDataTable>(IAsyncResult ar)
            where TypeDataTable : System.Data.DataTable, new();
       
        #endregion

        #region Public Connection Methods
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string DefaultConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Gets the alternative database connection string.
        /// </summary>
        /// <param name="configurationDatabaseConnection">The configuration key value.</param>
        /// <returns>The database connection string.</returns>
        string AlternativeConnection(string configurationDatabaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartDefaultConnection(string databaseConnection);

        /// <summary>
        /// Creates a new instance of the sql connection class.
        /// </summary>
        /// <param name="databaseConnection">The database connection string.</param>
        /// <returns>A sql connection instance.</returns>
        DbConnection StartAlternativeConnection(string databaseConnection);

        #endregion

        #endregion
    }

    /// <summary>
    /// Common base data context members.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    public interface IDataSetContextProvider<TDataContext>
        where TDataContext : System.Data.DataSet, new()
    {
        #region IDataContextProvider Interface

        #region Properties
        /// <summary>
        /// Gets sets the current data context object.
        /// </summary>
        TDataContext DataContext { get; set; }

        #endregion

        #region Data Context Methods
        /// <summary>
        /// Create a new data context instance.
        /// </summary>
        /// <returns>The new instance of the data context.</returns>
        /// <remarks>This data context uses the default connection 
        /// string in the application configuration file.
        /// <para>.</para>
        /// <para>[connectionStrings]</para>
        /// <para>[add name="Nequeo.Data.Client.Access.Properties.Settings.Nequeo.Data.ClientBaseConnectionString"</para>
        /// <para>connectionString="Data Source=DEVELOP\SQL2005DEV;Initial Catalog=Nequeo.Data.ClientBase;Integrated Security=True"</para>
        /// <para>providerName="System.Data.SqlClient" /]</para>
        /// <para>[/connectionStrings]</para></remarks>
        TDataContext CreateDataContextInstance();

        /// <summary>
        /// Dispose of the current data context instance.
        /// </summary>
        void DisposeDataContextInstance();

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic base cache control.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TDataTable">The data table entity type.</typeparam>
    public interface IDataSetCacheControl<TDataContext, TDataTable>
        where TDataContext : System.Data.DataSet, new()
        where TDataTable : System.Data.DataTable, new()
    {
        #region IDataSetCacheControl Interface

        #region Caching Control Methods
        /// <summary>
        /// Sets all relevant properties to indicate
        /// that caching is to be used for all selection
        /// operations.
        /// </summary>
        /// <param name="cachedItemName">The key name of the cached item.</param>
        /// <param name="cacheTimeout">The length of time in second the item(s)
        /// are stored in the cache before being removed.</param>
        /// <returns>The current select object.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        ISelectDataSetGenericBase<TDataContext, TDataTable>
            AddCachingControl(string cachedItemName, Int32 cacheTimeout);

        /// <summary>
        /// Sets all relevant properties to indicate
        /// that caching is not to be used.
        /// </summary>
        void RemoveCachingControl();
        #endregion

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
        /// <typeparam name="TypeDataTable">The current data table type.</typeparam>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand ExecuteQuery<TypeDataTable>(ref TypeDataTable dataTable, string queryText,
            CommandType commandType, string connectionString, params DbParameter[] values)
            where TypeDataTable : System.Data.DataTable, new();

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

    /// <summary>
    /// Interface base for all data context objects.
    /// </summary>
    /// <typeparam name="T1DataContextBase">The data context type.</typeparam>
    public interface IDataSetContext<T1DataContextBase>
        where T1DataContextBase : System.Data.DataSet, new()
    {
        #region IDataContext Interface

        #region Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T1DataContextBase DataContext1 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Interface base for all data context objects.
    /// </summary>
    /// <typeparam name="T1DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T2DataContextBase">The data context type.</typeparam>
    public interface IDataSetContext<T1DataContextBase, T2DataContextBase>
        where T1DataContextBase : System.Data.DataSet, new()
        where T2DataContextBase : System.Data.DataSet, new()
    {
        #region IDataContext Interface

        #region Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T1DataContextBase DataContext1 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T2DataContextBase DataContext2 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Interface base for all data context objects.
    /// </summary>
    /// <typeparam name="T1DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T2DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T3DataContextBase">The data context type.</typeparam>
    public interface IDataSetContext<T1DataContextBase, T2DataContextBase, T3DataContextBase>
        where T1DataContextBase : System.Data.DataSet, new()
        where T2DataContextBase : System.Data.DataSet, new()
        where T3DataContextBase : System.Data.DataSet, new()
    {
        #region IDataContext Interface

        #region Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T1DataContextBase DataContext1 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T2DataContextBase DataContext2 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T3DataContextBase DataContext3 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Interface base for all data context objects.
    /// </summary>
    /// <typeparam name="T1DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T2DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T3DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T4DataContextBase">The data context type.</typeparam>
    public interface IDataSetContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase>
        where T1DataContextBase : System.Data.DataSet, new()
        where T2DataContextBase : System.Data.DataSet, new()
        where T3DataContextBase : System.Data.DataSet, new()
        where T4DataContextBase : System.Data.DataSet, new()
    {
        #region IDataContext Interface

        #region Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T1DataContextBase DataContext1 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T2DataContextBase DataContext2 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T3DataContextBase DataContext3 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T4DataContextBase DataContext4 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Interface base for all data context objects.
    /// </summary>
    /// <typeparam name="T1DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T2DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T3DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T4DataContextBase">The data context type.</typeparam>
    /// <typeparam name="T5DataContextBase">The data context type.</typeparam>
    public interface IDataSetContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase, T5DataContextBase>
        where T1DataContextBase : System.Data.DataSet, new()
        where T2DataContextBase : System.Data.DataSet, new()
        where T3DataContextBase : System.Data.DataSet, new()
        where T4DataContextBase : System.Data.DataSet, new()
        where T5DataContextBase : System.Data.DataSet, new()
    {
        #region IDataContext Interface

        #region Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T1DataContextBase DataContext1 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T2DataContextBase DataContext2 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T3DataContextBase DataContext3 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T4DataContextBase DataContext4 { get; }

        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        T5DataContextBase DataContext5 { get; }

        #endregion

        #endregion
    }
}
