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
using System.Runtime.Serialization;
using System.Linq.Expressions;
using System.Data.Common;

using Nequeo.Data.Control;
using Nequeo.Data.DataType;
using Nequeo.Threading;

namespace Nequeo.Data
{
    /// <summary>
    /// Interface base for all data objects.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface IDataBase<TDataEntity>
        where TDataEntity : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectDataGenericBase<TDataEntity> Select { get; }
        
        /// <summary>
        /// Gets, the delete generic members.
        /// </summary>
        IDeleteDataGenericBase<TDataEntity> Delete { get; }

        /// <summary>
        /// Gets, the insert generic members.
        /// </summary>
        IInsertDataGenericBase<TDataEntity> Insert { get; }

        /// <summary>
        /// Gets, the update generic members.
        /// </summary>
        IUpdateDataGenericBase<TDataEntity> Update { get; }

        /// <summary>
        /// Gets, the common generic members.
        /// </summary>
        ICommonDataGenericBase<TDataEntity> Common { get; }
        
        #endregion

        #endregion
    }

    /// <summary>
    /// Interface base for all data objects.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface IDataBaseView<TDataEntity>
        where TDataEntity : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectDataGenericBase<TDataEntity> Select { get; }

        /// <summary>
        /// Gets, the common generic members.
        /// </summary>
        ICommonDataGenericBase<TDataEntity> Common { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Interface base for all data context objects.
    /// </summary>
    /// <typeparam name="T1DataContextBase">The data context type.</typeparam>
    public interface IDataContext<T1DataContextBase>
        where T1DataContextBase : IDataContextBase, new()
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
    public interface IDataContext<T1DataContextBase, T2DataContextBase>
        where T1DataContextBase : IDataContextBase, new()
        where T2DataContextBase : IDataContextBase, new()
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
    public interface IDataContext<T1DataContextBase, T2DataContextBase, T3DataContextBase>
        where T1DataContextBase : IDataContextBase, new()
        where T2DataContextBase : IDataContextBase, new()
        where T3DataContextBase : IDataContextBase, new()
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
    public interface IDataContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase>
        where T1DataContextBase : IDataContextBase, new()
        where T2DataContextBase : IDataContextBase, new()
        where T3DataContextBase : IDataContextBase, new()
        where T4DataContextBase : IDataContextBase, new()
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
    public interface IDataContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase, T5DataContextBase>
        where T1DataContextBase : IDataContextBase, new()
        where T2DataContextBase : IDataContextBase, new()
        where T3DataContextBase : IDataContextBase, new()
        where T4DataContextBase : IDataContextBase, new()
        where T5DataContextBase : IDataContextBase, new()
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

    /// <summary>
    /// The select base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface ISelectDataGenericBase<TDataEntity> : IFunctionHandler,
        ICacheControl, IProcessProvider, IDataCacheControl<TDataEntity>,
        IExecuteQuery, IDisposable
        where TDataEntity : class, new()
    {
        #region ISelectDataGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the foregin key reference lazy loading indicator.
        /// </summary>
        Boolean LazyLoading { get; set; }

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
        /// Gets the IQueryable generic provider
        /// for the current linq entity.
        /// </summary>
        IQueryable<TDataEntity> IQueryable { get; }

        /// <summary>
        /// Gets the data table containing the
        /// collection of table data.
        /// </summary>
        DataTable DataTable { get; }

        /// <summary>
        /// Gets the current data entities type.
        /// </summary>
        TDataEntity[] DataEntities { get; }

        #endregion

        #region Select Data Methods
        /// <summary>
        /// Gets the data entity item.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The data entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity SelectDataEntity(Expression<FunctionHandler<bool, TDataEntity>> predicate);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] SelectDataEntities(string queryText, CommandType commandType,
            params DbParameter[] values);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] SelectDataEntities<TypeDataEntity>(string queryText, CommandType commandType,
            params DbParameter[] values)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] SelectDataEntities(string queryText);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] SelectDataEntities<TypeDataEntity>(string queryText)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] SelectDataEntities();
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <returns>The current data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] SelectDataEntities<TypeDataEntity>()
            where TypeDataEntity : class, new();

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The current data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTable(string queryText, CommandType commandType,
            params DbParameter[] values);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTable(string queryText);

        /// <summary>
        /// Select all the collection of data for the table.
        /// </summary>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTable();
        
        #endregion

        #region Select Data Collection Methods
        
        /// <summary>
        /// Select the list collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The data collection items</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<TDataEntity> SelectListCollection(string queryText, CommandType commandType,
            params DbParameter[] values);
        
        /// <summary>
        /// Select the list collection of data for the table.
        /// </summary>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The data collection items</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<TDataEntity> SelectListCollection(string queryText);

        /// <summary>
        /// Select all the list collection of data for the table.
        /// </summary>
        /// <returns>The data collection items</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<TDataEntity> SelectListCollection();

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TDataEntity> SelectIQueryableItems(
            Expression<FunctionHandler<bool, TDataEntity>> predicate);
        
        #endregion

        #region Select Predicate Methods
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTablePredicate(Expression<FunctionHandler<bool, TDataEntity>> predicate);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTablePredicate(
            string predicate, params object[] values);
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollectionPredicate(ref DataTable dataTable,
            string predicate, params object[] values);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="lazyLoading">The lazy loading indicator.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] SelectDataEntitiesPredicate(
            string predicate, bool lazyLoading, params object[] values);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] SelectDataEntitiesPredicate(
            string predicate, params object[] values);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] SelectDataEntitiesPredicate<TypeDataEntity>(
            string predicate, params object[] values)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="dataEntities">The data entities to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollectionPredicate(ref TDataEntity[] dataEntities,
            string predicate, params object[] values);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="dataEntities">The data entities to return containing the data.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The sql command containing any return values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DbCommand SelectCollectionPredicate<TypeDataEntity>(ref TypeDataEntity[] dataEntities,
            string predicate, params object[] values)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] SelectDataEntitiesPredicate(
            Expression<FunctionHandler<bool, TDataEntity>> predicate);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] SelectDataEntitiesPredicate<TypeDataEntity>(
            Expression<FunctionHandler<bool, TypeDataEntity>> predicate)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] SelectDataEntitiesExpression(Expression expression);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="expression">The query expression.</param>
        /// <returns>The specified data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] SelectDataEntitiesExpression<TypeDataEntity>(Expression expression)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Select Query Methods
        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand SelectQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values);

        /// <summary>
        /// Select the item through the query text.
        /// </summary>
        /// <param name="dataSet">The data set to return containing the data.</param>
        /// <param name="tables">The collection of tables to include in the dataset.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand SelectQueryItem(ref System.Data.DataSet dataSet, System.Data.DataTable[] tables,
            string queryText, CommandType commandType, params DbParameter[] values);

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

        /// <summary>
        /// Gets the queryable provider.
        /// </summary>
        /// <returns>The object queryable provider.</returns>
        Nequeo.Linq.QueryableProvider<TDataEntity> QueryableProvider();

        /// <summary>
        /// Gets the queryable provider.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <returns>The object queryable provider.</returns>
        Nequeo.Linq.QueryableProvider<T> QueryableProvider<T>()
            where T : class, new();

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="orderBy"></param>
        /// <returns>The collection of data entity types.</returns>
        TDataEntity[] SelectData(int skip, string orderBy);

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="orderBy">The order by cluase.</param>
        /// <returns>The collection of data entity types.</returns>
        TDataEntity[] SelectData(int skip, int take, string orderBy);

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="orderBy">The order by cluase.</param>
        /// <param name="predicate">The where cluase.</param>
        /// <returns>The collection of data entity types.</returns>
        TDataEntity[] SelectData(int skip, string orderBy, string predicate);

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="orderBy">The order by cluase.</param>
        /// <param name="predicate">The where cluase.</param>
        /// <returns>The collection of data entity types.</returns>
        TDataEntity[] SelectData(int skip, int take, string orderBy, string predicate);

        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="predicate">The where clause.</param>
        /// <returns>The collection of data entity types.</returns>
        TDataEntity[] SelectData(int skip, string orderBy, Expression<Func<TDataEntity, bool>> predicate);
        
        /// <summary>
        /// Get all the data within the specified range.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <param name="predicate">The where clause.</param>
        /// <returns>The collection of data entity types.</returns>
        TDataEntity[] SelectData(int skip, int take, string orderBy, Expression<Func<TDataEntity, bool>> predicate);

        /// <summary>
        /// Get the total number of records.
        /// </summary>
        /// <returns>The total number of records.</returns>
        long GetRecordCount();

        /// <summary>
        /// Get the total number of records.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The total number of records.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        long GetRecordCount(string predicate);

        /// <summary>
        /// Get the total number of records.
        /// </summary>
        /// <param name="predicate">The expression containing the predicate.</param>
        /// <returns>The total number of records.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        long GetRecordCount(Expression<Func<TDataEntity, bool>> predicate);

        #endregion

        #region Asynchronous Select Methods
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntitiesPredicate(AsyncCallback callback, object state,
            string predicate, params object[] values);
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntitiesPredicate(AsyncCallback callback, object state,
            Expression<FunctionHandler<bool, TDataEntity>> predicate);
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TDataEntity[] EndSelectDataEntitiesPredicate(IAsyncResult ar);
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntitiesPredicate<TypeDataEntity>(AsyncCallback callback, object state,
            string predicate, params object[] values)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TypeDataEntity[] EndSelectDataEntitiesPredicate<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTablePredicate(AsyncCallback callback, object state,
            Expression<FunctionHandler<bool, TDataEntity>> predicate);
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        DataTable EndSelectDataTablePredicate(IAsyncResult ar);
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntities<TypeDataEntity>(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntities<TypeDataEntity>(AsyncCallback callback, object state,
            string queryText)
            where TypeDataEntity : class, new();
       
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntities<TypeDataEntity>(AsyncCallback callback, object state)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The current data entity type.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TypeDataEntity[] EndSelectDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntities(AsyncCallback callback, object state,
            string queryText, CommandType commandType, params DbParameter[] values);
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntities(AsyncCallback callback, object state,
            string queryText);
        
        /// <summary>
        /// Begin select the collection of data for the table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataEntities(AsyncCallback callback, object state);
        
        /// <summary>
        /// End select the collection of data for the table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The specified data table.</returns>
        TDataEntity[] EndSelectDataEntities(IAsyncResult ar);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="degreeOfParallelism">Degree of parallelism is
        /// the maximum number of concurrently executing tasks that will be used to process
        /// the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        ParallelQuery<TDataEntity> SelectDataEntitiesParallel(int degreeOfParallelism = 1);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to return.</typeparam>
        /// <param name="degreeOfParallelism">Degree of parallelism is the maximum number 
        /// of concurrently executing tasks that will be used to process the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        ParallelQuery<TypeDataEntity> SelectDataEntitiesParallel<TypeDataEntity>(int degreeOfParallelism = 1)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="degreeOfParallelism">Degree of parallelism is the maximum number 
        /// of concurrently executing tasks that will be used to process the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        ParallelQuery<TDataEntity> SelectDataEntitiesPredicateParallel(
            Expression<FunctionHandler<bool, TDataEntity>> predicate, int degreeOfParallelism = 1);

        /// <summary>
        /// Select the collection of data for the table.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to return.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="degreeOfParallelism">Degree of parallelism is the maximum number 
        /// of concurrently executing tasks that will be used to process the query.</param>
        /// <returns>The collection of data entities which can be queried in parallel</returns>
        ParallelQuery<TypeDataEntity> SelectDataEntitiesPredicateParallel<TypeDataEntity>(
            Expression<FunctionHandler<bool, TypeDataEntity>> predicate, int degreeOfParallelism = 1)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Translator Methods
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] Translator();
        
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] Translator<TypeDataEntity>()
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <param name="table">The table to translate.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] Translator(DataTable table);
        
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="table">The table to translate.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] Translator<TypeDataEntity>(DataTable table)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TDataEntity[] Translator(Expression<FunctionHandler<bool, TDataEntity>> predicate);
        
        /// <summary>
        /// Translate a data table to the corresponding data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The array of data entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeDataEntity[] Translator<TypeDataEntity>(Expression<FunctionHandler<bool, TypeDataEntity>> predicate)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Public Execute Function Methods
        /// <summary>
        /// Execute a function routine.
        /// </summary>
        /// <param name="instance">The current data base instance.</param>
        /// <param name="methodInfo">The method information to execute.</param>
        /// <param name="parameters">The function routine parameters.</param>
        /// <returns>The execution result.</returns>
        IExecuteFunctionResult ExecuteFunction(IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters);

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

        #endregion

        #region Public Analysis Methods
        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged(TDataEntity original, TDataEntity current);

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged<TData>(TData original, TData current);

        #endregion

        #endregion
    }

    /// <summary>
    /// The delete base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface IDeleteDataGenericBase<TDataEntity> : IFunctionHandler,
        IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataEntity : class, new()
    {
        #region IDeleteDataGenericBase Interface

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
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey<TypeDataEntity>(object keyValue, string keyName)
            where TypeDataEntity : class, new();
        
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
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey<TypeDataEntity>(object keyValue, string keyName,
            object rowVersionData, string rowVersionName)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItem(TDataEntity dataEntity, bool useRowVersion);

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItem<TypeDataEntity>(TypeDataEntity dataEntity, bool useRowVersion)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItem(TDataEntity dataEntity);

        /// <summary>
        /// Deletes the data entity from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItem<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new();

        #endregion

        #region Delete Predicate Methods
        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate(Expression<FunctionHandler<bool, TDataEntity>> predicate);

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
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate<TypeDataEntity>(string predicate, params object[] values)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Delete Collection Methods
        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteDataEntities(TDataEntity[] dataEntities, bool useRowVersion);
       
        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities, bool useRowVersion)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteDataEntities(TDataEntity[] dataEntities);

        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Delete Query Methods
        /// <summary>
        /// Delete the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand DeleteQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values);

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
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteItemPredicate<TypeDataEntity>(AsyncCallback callback,
            object state, string predicate, params object[] values)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// End deletes the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was deleted else false.</returns>
        Boolean EndDeleteItemPredicate<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Begin deletes the data entities from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteDataEntities(AsyncCallback callback,
            object state, TDataEntity[] dataEntities, bool useRowVersion);
        
        /// <summary>
        /// End deletes the data entities from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        Boolean EndDeleteDataEntities(IAsyncResult ar);
        
        /// <summary>
        /// Begin deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteDataEntities<TypeDataEntity>(AsyncCallback callback,
           object state, TypeDataEntity[] dataEntities, bool useRowVersion)
           where TypeDataEntity : class, new();
        
        /// <summary>
        /// End deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if delete was successful else false.</returns>
        Boolean EndDeleteDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Public Execute Function Methods
        /// <summary>
        /// Execute a function routine.
        /// </summary>
        /// <param name="instance">The current data base instance.</param>
        /// <param name="methodInfo">The method information to execute.</param>
        /// <param name="parameters">The function routine parameters.</param>
        /// <returns>The execution result.</returns>
        IExecuteFunctionResult ExecuteFunction(IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters);

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

        #endregion

        #region Public Analysis Methods
        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged(TDataEntity original, TDataEntity current);

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged<TData>(TData original, TData current);

        #endregion

        #endregion
    }

    /// <summary>
    /// The insert base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface IInsertDataGenericBase<TDataEntity> : IFunctionHandler,
        IProcessProvider, IExecuteCommand, IDisposable
        where TDataEntity : class, new()
    {
        #region IInsertDataGenericBase Interface

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
        /// Inserts the specified data entities.
        /// </summary>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertDataEntities(TDataEntity[] dataEntities);
        
        /// <summary>
        /// Inserts the specified data entities.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Insert Item Methods
        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertItem(TDataEntity dataEntity);

        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertItem<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Insert Identity Methods
        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<object> InsertDataEntity(TDataEntity dataEntity);

        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<object> InsertDataEntity<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<object> InsertDataEntity(TDataEntity dataEntity, string identitySqlQuery);
        
        /// <summary>
        /// Inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The new identity value else null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        List<object> InsertDataEntity<TypeDataEntity>(TypeDataEntity dataEntity, string identitySqlQuery)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Insert Query Methods
        /// <summary>
        /// Insert the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand InsertQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values);

        /// <summary>
        /// Insert the item through the command text.
        /// </summary>
        /// <param name="sqlCommand">The current sql command.</param>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>-1 if command execution failed.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Int32 InsertCommandItem(ref DbCommand sqlCommand, string commandText,
            CommandType commandType, params DbParameter[] values);

        #endregion

        #region Asynchronous Insert Methods
        /// <summary>
        /// Begin inserts the specified data entity.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataEntity(AsyncCallback callback,
            object state, TDataEntity dataEntity, string identitySqlQuery);
        
        /// <summary>
        /// End inserts the specified data entity.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        List<Object> EndInsertDataEntity(IAsyncResult ar);

        /// <summary>
        /// Begin inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntity">The data entity to insert.</param>
        /// <param name="identitySqlQuery">The identity query to return entity identifiers.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataEntity<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity dataEntity, string identitySqlQuery)
            where TypeDataEntity : class, new();

        /// <summary>
        /// End inserts the specified data entity.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The new identity value else null.</returns>
        List<Object> EndInsertDataEntity<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Begin inserts the specified data entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataEntities(AsyncCallback callback,
            object state, TDataEntity[] dataEntities);
        
        /// <summary>
        /// End inserts the specified data entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        Boolean EndInsertDataEntities(IAsyncResult ar);
        
        /// <summary>
        /// Begin inserts the specified data entities.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataEntities<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// End inserts the specified data entities.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted else false.</returns>
        Boolean EndInsertDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Public Execute Function Methods
        /// <summary>
        /// Execute a function routine.
        /// </summary>
        /// <param name="instance">The current data base instance.</param>
        /// <param name="methodInfo">The method information to execute.</param>
        /// <param name="parameters">The function routine parameters.</param>
        /// <returns>The execution result.</returns>
        IExecuteFunctionResult ExecuteFunction(IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters);

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

        #endregion

        #region Public Analysis Methods
        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged(TDataEntity original, TDataEntity current);

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged<TData>(TData original, TData current);

        #endregion

        #endregion
    }

    /// <summary>
    /// The update base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface IUpdateDataGenericBase<TDataEntity> : IFunctionHandler,
        IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataEntity : class, new()
    {
        #region IUpdateDataGenericBase Interface

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
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(TDataEntity dataEntity, object keyValue, string keyName);
        
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeDataEntity>(TypeDataEntity dataEntity, object keyValue, string keyName)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(TDataEntity dataEntity,
            object keyValue, string keyName, object rowVersionData, string rowVersionName);
        
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="keyValue">The value of the key column.</param>
        /// <param name="keyName">The name of the key column.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The name of the row version column.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeDataEntity>(TypeDataEntity dataEntity,
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItem(TDataEntity dataEntity, bool useRowVersion);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItem<TypeDataEntity>(TypeDataEntity dataEntity, bool useRowVersion)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItem(TDataEntity dataEntity);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItem<TypeDataEntity>(TypeDataEntity dataEntity)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Update Predicate Methods
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(
            TDataEntity dataEntity, Expression<FunctionHandler<bool, TDataEntity>> predicate);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(
            TDataEntity dataEntity, string predicate, params object[] values);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeDataEntity>(
            TypeDataEntity dataEntity, string predicate, params object[] values)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Update Collection Methods
        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateDataEntities(TDataEntity[] dataEntities, bool useRowVersion);
        
        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <param name="useRowVersion">Should row versioning data be used.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities, bool useRowVersion)
            where TypeDataEntity : class, new();

        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateDataEntities(TDataEntity[] dataEntities);

        /// <summary>
        /// Updates the specified items.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
            where TypeDataEntity : class, new();

        #endregion

        #region Update Query Methods
        /// <summary>
        /// Update the item through the query text.
        /// </summary>
        /// <param name="dataTable">The data table to return containing the data.</param>
        /// <param name="queryText">The query text to execute.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="getSchemaTable">Get the table schema from the database and then load the data. Used when
        /// returning data from the database for a particilar table.</param>
        /// <param name="values">The collection of sql parameters to include.</param>
        /// <returns>The sql command containing any return values.</returns>
        DbCommand UpdateQueryItem(ref DataTable dataTable, string queryText,
            CommandType commandType, bool getSchemaTable, params DbParameter[] values);

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
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateItemPredicate(AsyncCallback callback,
            object state, TDataEntity dataEntity, string predicate, params object[] values);
        
        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        Boolean EndUpdateItemPredicate(IAsyncResult ar);
        
        /// <summary>
        /// Begin updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntity">The data entity to update.</param>
        /// <param name="predicate">The where clause predicate string.</param>
        /// <param name="values">The values associated with the predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateItemPredicate<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity dataEntity, string predicate, params object[] values)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// End updates the specified item.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the item was updated else false.</returns>
        Boolean EndUpdateItemPredicate<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        /// <summary>
        /// Begin updates the data entities from the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateDataEntities(AsyncCallback callback,
            object state, TDataEntity[] dataEntities, bool useRowVersion);
        
        /// <summary>
        /// End updates the data entities from the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        Boolean EndUpdateDataEntities(IAsyncResult ar);
        
        /// <summary>
        /// Begin updates the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateDataEntities<TypeDataEntity>(AsyncCallback callback,
            object state, TypeDataEntity[] dataEntities, bool useRowVersion)
            where TypeDataEntity : class, new();
       
        /// <summary>
        /// End updates the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeDataEntity">The data entity type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated was successful else false.</returns>
        Boolean EndUpdateDataEntities<TypeDataEntity>(IAsyncResult ar)
            where TypeDataEntity : class, new();
        
        #endregion

        #region Public Execute Function Methods
        /// <summary>
        /// Execute a function routine.
        /// </summary>
        /// <param name="instance">The current data base instance.</param>
        /// <param name="methodInfo">The method information to execute.</param>
        /// <param name="parameters">The function routine parameters.</param>
        /// <returns>The execution result.</returns>
        IExecuteFunctionResult ExecuteFunction(IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters);

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

        #endregion

        #region Public Analysis Methods
        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged(TDataEntity original, TDataEntity current);

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged<TData>(TData original, TData current);

        #endregion

        #endregion
    }

    /// <summary>
    /// The common base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface ICommonDataGenericBase<TDataEntity> : IFunctionHandler,
        IProcessProvider, IExecuteQuery, IExecuteCommand, IDisposable
        where TDataEntity : class, new()
    {
        #region ICommonDataGenericBase Base Interface

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
        /// Gets, the data entity type descriptor.
        /// </summary>
        DataEntityTypeDescriptor<TDataEntity> DataEntityTypeDescriptor { get; }

        #endregion

        #region Reflection, Attrubute Methods
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
        /// Get all properties in the data entity that are primary keys.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>Collection of properties that are primary keys.</returns>
        List<PropertyInfo> GetAllPrimaryKeys<T>();
        
        /// <summary>
        /// Get all row versioning properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        List<PropertyInfo> GetAllRowVersions<T>();
        
        /// <summary>
        /// Get all reference properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        List<PropertyInfo> GetAllReference<T>();

        /// <summary>
        /// Get all foreign key properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        List<PropertyInfo> GetAllForeignKey<T>();
        
        /// <summary>
        /// Get all nullable properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        List<PropertyInfo> GetAllNullable<T>();
        
        /// <summary>
        /// Is the current property value a primary key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a primary key.</returns>
        /// <remarks>Check for a primary key for a data entity column.</remarks>
        bool IsPrimaryKey(PropertyInfo property);
        
        /// <summary>
        /// Is the current property value auto generated in the database.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is auto generated.</returns>
        /// <remarks>Check for auto generated for a data entity column.</remarks>
        bool IsAutoGenerated(PropertyInfo property);
        
        /// <summary>
        /// Is the current property value a row versioning column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a row versioning column.</returns>
        /// <remarks>Check for a row versioning column for a data entity column.</remarks>
        bool IsRowVersion(PropertyInfo property);
        
        /// <summary>
        /// Is the current property value a nullable column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a nullable column for a data entity column.</returns>
        bool IsNullable(PropertyInfo property);

        /// <summary>
        /// Is the current property value a foreign key column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a foreign key column for a data entity column.</returns>
        bool IsForeignKey(PropertyInfo property);
        
        /// <summary>
        /// Is the current property value a reference column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a reference column for a data entity column.</returns>
        bool IsReference(PropertyInfo property);
        
        /// <summary>
        /// Is the current property value a table column.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>Check for a nullable column for a data entity column.</returns>
        bool IsColumnData(PropertyInfo property);
        
        /// <summary>
        /// Get all table column properties for the data entity type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties.</returns>
        List<PropertyInfo> GetAllColumnData<T>();
        
        /// <summary>
        /// Gets the current data entity table name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The table name.</returns>
        string GetTableName<T>();

        /// <summary>
        /// Gets the current data entity column name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The column name.</returns>
        string GetDbColumnName<T>(PropertyInfo property);
        
        #endregion

        #region Public Execute Function Methods
        /// <summary>
        /// Execute a function routine.
        /// </summary>
        /// <param name="instance">The current data base instance.</param>
        /// <param name="methodInfo">The method information to execute.</param>
        /// <param name="parameters">The function routine parameters.</param>
        /// <returns>The execution result.</returns>
        IExecuteFunctionResult ExecuteFunction(IFunctionHandler instance, MethodInfo methodInfo, params Object[] parameters);

        #endregion

        #region Convertion Methods
        /// <summary>
        /// Convert all the IQueryable data into a array of
        /// anonymous types.
        /// </summary>
        /// <param name="query">The IQueryable collection of anonymous type objects.</param>
        /// <returns>The array of anonymous type data.</returns>
        Nequeo.Data.Control.AnonymousType[]
            ConvertToAnonymousType(IQueryable query);

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

        #endregion

        #region Public Analysis Methods
        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged(TDataEntity original, TDataEntity current);

        /// <summary>
        /// Is the data in the two types different.
        /// </summary>
        /// <typeparam name="TData">The data type to examine</typeparam>
        /// <param name="original">The original data.</param>
        /// <param name="current">The current data.</param>
        /// <returns>True if the data is different; else false.</returns>
        bool HasChanged<TData>(TData original, TData current);

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic base cache control.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface IDataCacheControl<TDataEntity>
        where TDataEntity : class, new()
    {
        #region IDataCacheControl Interface

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
        ISelectDataGenericBase<TDataEntity> AddCachingControl(string cachedItemName, Int32 cacheTimeout);

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

    /// <summary>
    /// Data context query base interface.
    /// </summary>
    public interface IDataContextBase : IDisposable
    {
        #region Data Context Base Interface
        /// <summary>
        /// Gets, the current database connection.
        /// </summary>
        DbConnection Connection { get; }

        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the foregin key reference lazy loading indicator.
        /// </summary>
        Boolean LazyLoading { get; set; }

        /// <summary>
        /// Gets the current connection type.
        /// </summary>
        ConnectionContext.ConnectionType ProviderConnectionType { get; }

        /// <summary>
        /// Gets the current connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ProviderConnectionDataType { get; }

        /// <summary>
        /// Gets the current data entity queryable object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The queryable data object type.</returns>
        Nequeo.Data.Linq.Query<TDataEntity> GetTable<TDataEntity>()
            where TDataEntity : class, new();

        /// <summary>
        /// Gets the common object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The common data entity object.</returns>
        ICommonDataGenericBase<TDataEntity> Common<TDataEntity>()
             where TDataEntity : class, new();

        /// <summary>
        /// Gets the select object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The insert data entity object.</returns>
        ISelectDataGenericBase<TDataEntity> Select<TDataEntity>()
            where TDataEntity : class, new();

        /// <summary>
        /// Gets the insert object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The insert data entity object.</returns>
        IInsertDataGenericBase<TDataEntity> Insert<TDataEntity>()
            where TDataEntity : class, new();

        /// <summary>
        /// Gets the update object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The update data entity object.</returns>
        IUpdateDataGenericBase<TDataEntity> Update<TDataEntity>()
            where TDataEntity : class, new();

        /// <summary>
        /// Gets the delete object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The delete data entity object.</returns>
        IDeleteDataGenericBase<TDataEntity> Delete<TDataEntity>()
            where TDataEntity : class, new();

        /// <summary>
        /// Executes a sql query text directly to the database.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <param name="queryText">The sql query text to execute.</param>
        /// <returns>The collection of data entities.</returns>
        TDataEntity[] ExecuteQuery<TDataEntity>(string queryText)
             where TDataEntity : class, new();

        /// <summary>
        /// Executes a sql query text directly to the database.
        /// </summary>
        /// <param name="queryText">The sql query text to execute.</param>
        /// <returns>The collection of data rows.</returns>
        DataTable ExecuteQuery(string queryText);

        /// <summary>
        /// Executes a sql query command directly to the database.
        /// </summary>
        /// <param name="queryText">The sql query text to execute.</param>
        /// <returns>The number of rows affected.</returns>
        Int32 ExecuteCommand(string queryText);

        #endregion
    }

    /// <summary>
    /// Data access generic data type transaction function handler.
    /// </summary>
    /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
    public interface IDataTransactions<TDataEntity> : IDisposable
        where TDataEntity : class, new()
    {
        #region IDataTransactions Class

        #region Public Methods
        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <param name="action">The insert action to execute.</param>
        void Insert(Nequeo.Threading.ActionHandler<IInsertDataGenericBase<TDataEntity>> action);

        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The insert action to execute.</param>
        /// <returns>The execute action return type.</returns>
        TResult Insert<TResult>(Nequeo.Threading.FunctionHandler<TResult, IInsertDataGenericBase<TDataEntity>> action);

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <param name="action">The delete action to execute.</param>
        void Delete(Nequeo.Threading.ActionHandler<IDeleteDataGenericBase<TDataEntity>> action);

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The delete action to execute.</param>
        /// <returns>The execute action return type.</returns>
        TResult Delete<TResult>(Nequeo.Threading.FunctionHandler<TResult, IDeleteDataGenericBase<TDataEntity>> action);

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <param name="action">The update action to execute.</param>
        void Update(Nequeo.Threading.ActionHandler<IUpdateDataGenericBase<TDataEntity>> action);

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <returns>The execute action return type.</returns>
        TResult Update<TResult>(Nequeo.Threading.FunctionHandler<TResult, IUpdateDataGenericBase<TDataEntity>> action);

        /// <summary>
        /// Execute a select action
        /// </summary>
        /// <param name="action">The select action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        void Select(Nequeo.Threading.ActionHandler<ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false);

        /// <summary>
        /// Execute a select action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <param name="action">The select action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        /// <returns>The execute action return type.</returns>
        TResult Select<TResult>(Nequeo.Threading.FunctionHandler<TResult, ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false);

        #endregion

        #endregion
    }

    /// <summary>
    /// Data access generic data type transaction function handler.
    /// </summary>
    public interface IDataTransactions : IDisposable
    {
        #region IDataTransactions Class

        #region Public Methods
        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The insert action to execute.</param>
        void Insert<TDataEntity>(Nequeo.Threading.ActionHandler<IInsertDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new();

        /// <summary>
        /// Execute an insert action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The insert action to execute.</param>
        /// <returns>The execute action return type.</returns>
        TResult Insert<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, IInsertDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new();

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The delete action to execute.</param>
        void Delete<TDataEntity>(Nequeo.Threading.ActionHandler<IDeleteDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new();

        /// <summary>
        /// Execute a delete action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The delete action to execute.</param>
        /// <returns>The execute action return type.</returns>
        TResult Delete<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, IDeleteDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new();

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        void Update<TDataEntity>(Nequeo.Threading.ActionHandler<IUpdateDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new();

        /// <summary>
        /// Execute an update action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <returns>The execute action return type.</returns>
        TResult Update<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, IUpdateDataGenericBase<TDataEntity>> action)
            where TDataEntity : class, new();

        /// <summary>
        /// Execute an select action
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        void Select<TDataEntity>(Nequeo.Threading.ActionHandler<ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false)
            where TDataEntity : class, new();

        /// <summary>
        /// Execute an select action
        /// </summary>
        /// <typeparam name="TResult">The data type to execute the action on.</typeparam>
        /// <typeparam name="TDataEntity">The data entity type to examine.</typeparam>
        /// <param name="action">The update action to execute.</param>
        /// <param name="lazyLoading">Should lazy loading of references be applied.</param>
        /// <returns>The execute action return type.</returns>
        TResult Select<TResult, TDataEntity>(Nequeo.Threading.FunctionHandler<TResult, ISelectDataGenericBase<TDataEntity>> action, Boolean lazyLoading = false)
            where TDataEntity : class, new();

        #endregion

        #endregion
    }
}
