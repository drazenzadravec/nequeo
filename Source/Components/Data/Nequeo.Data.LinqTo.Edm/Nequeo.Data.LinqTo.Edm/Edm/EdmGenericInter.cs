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
using System.Data.Objects;
using System.Data.Common;

using Nequeo.Data.DataType;
using Nequeo.Data.Control;

namespace Nequeo.Data.Edm
{
    /// <summary>
    /// The interface linq base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface IEdmAnonymousGenericBase<TDataContext, TLinqEntity> : IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region IEdmAnonymousGenericBase Interface

        #region Select Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectEdmGenericBase<TDataContext, TLinqEntity> Select { get; }

        #endregion

        #region Select Properties
        /// <summary>
        /// Gets, the Delete generic members.
        /// </summary>
        IDeleteEdmGenericBase<TDataContext, TLinqEntity> Delete { get; }

        #endregion

        #region Insert Properties
        /// <summary>
        /// Gets, the insert generic members.
        /// </summary>
        IInsertEdmGenericBase<TDataContext, TLinqEntity> Insert { get; }

        #endregion

        #region Update Properties
        /// <summary>
        /// Gets, the update generic members.
        /// </summary>
        IUpdateEdmGenericBase<TDataContext, TLinqEntity> Update { get; }

        #endregion

        #region Edm Base Properties
        /// <summary>
        /// Gets, the data generic base members.
        /// </summary>
        IEdmDataGenericBase<TDataContext, TLinqEntity> Common { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The interface linq base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface IEdmGenericBase<TDataContext, TLinqEntity> : IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region IEdmGenericBase Interface

        #region Select Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectEdmGenericBase<TDataContext, TLinqEntity> Select { get; }

        #endregion

        #region Select Properties
        /// <summary>
        /// Gets, the Delete generic members.
        /// </summary>
        IDeleteEdmGenericBase<TDataContext, TLinqEntity> Delete { get; }

        #endregion

        #region Insert Properties
        /// <summary>
        /// Gets, the insert generic members.
        /// </summary>
        IInsertEdmGenericBase<TDataContext, TLinqEntity> Insert { get; }

        #endregion

        #region Update Properties
        /// <summary>
        /// Gets, the update generic members.
        /// </summary>
        IUpdateEdmGenericBase<TDataContext, TLinqEntity> Update { get; }

        #endregion

        #region Edm Base Properties
        /// <summary>
        /// Gets, the data generic base members.
        /// </summary>
        IEdmDataGenericBase<TDataContext, TLinqEntity> Common { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface IEdmDataGenericBase<TDataContext, TLinqEntity> :
        IObjectContextProvider<TDataContext>, IProcessProvider, IExecuteQuery, IExecuteCommand, IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region IEdmDataGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

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
        /// Get all properties in the current type that are primary keys.
        /// </summary>
        /// <returns>Collection of properties that are primary keys.</returns>
        List<PropertyInfo> GetAllPrimaryKeys();
        
        /// <summary>
        /// Get all properties in the current type that are primary keys.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>Collection of properties that are primary keys.</returns>
        List<PropertyInfo> GetAllPrimaryKeys<T>();

        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        PropertyInfo GetPrimaryKey();
        
        /// <summary>
        /// Get the primary key property for the current type.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>The property that is the primary key.</returns>
        /// <remarks>This method should only be used if one primary key exists.</remarks>
        PropertyInfo GetPrimaryKey<T>();

        /// <summary>
        /// Is the current property value a primary key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a primary key.</returns>
        bool IsPrimaryKey(PropertyInfo property);

        /// <summary>
        /// Is the current property value a association key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a foreign key.</returns>
        bool IsAssociationKey(PropertyInfo property);
        
        /// <summary>
        /// Is the current property value a foreign key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a foreign key.</returns>
        bool IsForeignKey(PropertyInfo property);
        
        /// <summary>
        /// Gets the current linq entity table name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The ttable name.</returns>
        string GetTableName<T>();
        
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

        #endregion
    }

    /// <summary>
    /// The select interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface ISelectEdmGenericBase<TDataContext, TLinqEntity> :
        IObjectContextProvider<TDataContext>, ICacheControl, IProcessProvider,
        IEdmCacheControl<TDataContext, TLinqEntity>, IExecuteQuery, IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region ISelectEdmGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets the IQueryable generic provider
        /// for the current linq entity.
        /// </summary>
        IQueryable<TLinqEntity> IQueryable { get; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        /// <summary>
        /// Gets sets, are the entity items plural.
        /// </summary>
        Boolean PluralEntity { get; set; }
        #endregion

        #region Select Items Methods
        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TLinqEntity> SelectIQueryableItems(string queryString,
            params System.Data.Objects.ObjectParameter[] values);
       
        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TypeLinqEntity> SelectIQueryableItems<TypeLinqEntity>(
            string queryString, params System.Data.Objects.ObjectParameter[] values)
                where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TLinqEntity> SelectIQueryableItems();
        
        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TypeLinqEntity> SelectIQueryableItems<TypeLinqEntity>()
                where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TLinqEntity> SelectIQueryableItems(
            Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TypeLinqEntity> SelectIQueryableItems<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();
        
        #endregion

        #region Select Collection Methods
        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollection(
            ref IQueryable<TLinqEntity> queryResult, string queryString,
            params System.Data.Objects.ObjectParameter[] values);
        
        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollection<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult, string queryString,
            params System.Data.Objects.ObjectParameter[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollection(ref IQueryable<TLinqEntity> queryResult);
        
        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollection<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollection(ref IQueryable<TLinqEntity> queryResult,
            Expression<Func<TLinqEntity, bool>> predicate);

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollection<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();
        
        #endregion

        #region Asynchronous Select Methods
        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems<TypeLinqEntity>(AsyncCallback callback, object state,
            string queryString, params System.Data.Objects.ObjectParameter[] values)
                where TypeLinqEntity : class, new();
        
        /// <summary>
        /// End gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        IQueryable<TypeLinqEntity> EndSelectIQueryableItems<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="queryString">The primary key value.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state,
            string queryString, params System.Data.Objects.ObjectParameter[] values);
        
        /// <summary>
        /// End gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        IQueryable<TLinqEntity> EndSelectIQueryableItems(IAsyncResult ar);
        
        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems<TypeLinqEntity>(AsyncCallback callback, object state)
                where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state);
        
        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems<TypeLinqEntity>(AsyncCallback callback, object state,
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Begin gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state,
            Expression<Func<TLinqEntity, bool>> predicate);
        
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

        #endregion
    }

    /// <summary>
    /// The delete interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface IDeleteEdmGenericBase<TDataContext, TLinqEntity> :
        IObjectContextProvider<TDataContext>, IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region IDeleteEdmGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

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
        /// Deletes the current entity item.
        /// </summary>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteLinqEntity(TDataContext dataContext, TLinqEntity linqEntity);
        
        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <typeparam name="TypeDataContext">The data context to examine.</typeparam>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteLinqEntity<TypeDataContext, TypeLinqEntity>(
            TypeDataContext dataContext, TypeLinqEntity linqEntity)
            where TypeDataContext : System.Data.Entity.DbContext, new()
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate(
            string predicate, params ObjectParameter[] parameters);
        
        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate<TypeLinqEntity>(
            string predicate, params ObjectParameter[] parameters)
                where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate(
            Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Deletes the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();
        
        #endregion

        #region Delete Collection Methods
        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntities">The linq entities to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollection(
            TDataContext dataContext, TLinqEntity[] linqEntities);
        
        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeDataContext">The data context to examine.</typeparam>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntities">The linq entities to delete.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollection<TypeDataContext, TypeLinqEntity>(
            TypeDataContext dataContext, TypeLinqEntity[] linqEntities)
            where TypeDataContext : System.Data.Entity.DbContext, new()
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate(
            string predicate, params ObjectParameter[] parameters);
        
        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate<TypeLinqEntity>(
            string predicate, params ObjectParameter[] parameters)
            where TypeLinqEntity : class, new();
       
        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate(
            Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();
        
        #endregion

        #region Asynchronous Delete Methods
        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteCollectionPredicate(AsyncCallback callback,
            object state, string predicate, params ObjectParameter[] parameters);
        
        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteCollectionPredicate(AsyncCallback callback,
            object state, Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// End deletes the collection of entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        Boolean EndDeleteCollectionPredicate(IAsyncResult ar);
        
        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteCollectionPredicate<TypeLinqEntity>(AsyncCallback callback,
            object state, string predicate, params ObjectParameter[] parameters)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Begin deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteCollectionPredicate<TypeLinqEntity>(AsyncCallback callback,
            object state, Expression<Func<TypeLinqEntity, bool>> predicate)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// End deletes the collection of entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        Boolean EndDeleteCollectionPredicate<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();
        
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

        #endregion
    }

    /// <summary>
    /// The insert interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface IInsertEdmGenericBase<TDataContext, TLinqEntity> :
        IObjectContextProvider<TDataContext>, IProcessProvider, IExecuteCommand, IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region IInsertEdmGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        #endregion

        #region Insert Item Methods
        /// <summary>
        /// Insert the Linq Enity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The linq entity containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity InsertItem(TLinqEntity linqEntityItem);
        
        /// <summary>
        /// Insert the Linq Enity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity InsertItem<TypeLinqEntity>(TypeLinqEntity linqEntityItem)
            where TypeLinqEntity : class, new();
        
        #endregion

        #region Insert Collection Methods
        /// <summary>
        /// Insert the collection of linq entities.
        /// </summary>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity[] InsertCollection(TLinqEntity[] linqEntityItems);
        
        /// <summary>
        /// Insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity[] InsertCollection<TypeLinqEntity>(
            TypeLinqEntity[] linqEntityItems)
                where TypeLinqEntity : class, new();
        
        #endregion

        #region Insert Methods
        /// <summary>
        /// Inserts the array of linq entities.
        /// </summary>
        /// <param name="linqEntities">The array of linq entities.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertLinqEntities(TLinqEntity[] linqEntities);
        
        /// <summary>
        /// Inserts the array of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntities">The array of linq entities.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertLinqEntities<TypeLinqEntity>(TypeLinqEntity[] linqEntities)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Inserts the linq entity.
        /// </summary>
        /// <param name="linqEntity">The linq entity.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertLinqEntity(TLinqEntity linqEntity);
        
        /// <summary>
        /// Inserts the linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The linq entity.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertLinqEntity<TypeLinqEntity>(TypeLinqEntity linqEntity)
            where TypeLinqEntity : class, new();
        
        #endregion

        #region Asynchronous Insert Methods
        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertCollection(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems);
        
        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities with new values.</returns>
        TLinqEntity[] EndInsertCollection(IAsyncResult ar);

        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities with new values.</returns>
        TypeLinqEntity[] EndInsertCollection<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertLinqEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems);
        
        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        Boolean EndInsertLinqEntities(IAsyncResult ar);
        
        /// <summary>
        /// Begin insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The array of linq entities.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertLinqEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// End insert the collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        Boolean EndInsertLinqEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();
        
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

        #endregion
    }

    /// <summary>
    /// The update interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface IUpdateEdmGenericBase<TDataContext, TLinqEntity> :
        IObjectContextProvider<TDataContext>, IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region IUpdateEdmGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

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
        /// Updates the current entity item.
        /// </summary>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateLinqEntity(TDataContext dataContext, TLinqEntity linqEntity);
        
        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <typeparam name="TypeDataContext">The data context to examine.</typeparam>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="dataContext">The data context containg the reference.</param>
        /// <param name="linqEntity">The linq entity to update.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateLinqEntity<TypeDataContext, TypeLinqEntity>(
            TypeDataContext dataContext, TypeLinqEntity linqEntity)
            where TypeDataContext : System.Data.Entity.DbContext, new()
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(TLinqEntity linqEntity,
            string predicate, params ObjectParameter[] parameters);
        
        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate query string to find.</param>
        /// <param name="parameters">The predicate parameters.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeLinqEntity>(TypeLinqEntity linqEntity,
            string predicate, params ObjectParameter[] parameters)
                where TypeLinqEntity : class, new();
    
        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(TLinqEntity linqEntity,
            Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Updates the current entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The linq entity to examine.</typeparam>
        /// <param name="linqEntity">The linq entity containing the data to update.</param>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>True if the item was updated else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeLinqEntity>(TypeLinqEntity linqEntity,
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();
        
        #endregion

        #region Update Collection Methods
        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateCollection(TLinqEntity[] linqEntityItems);
        
        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateCollection<TypeLinqEntity>(TypeLinqEntity[] linqEntityItems)
                where TypeLinqEntity : class, new();

        #endregion

        #region Asynchronous Update Methods
        /// <summary>
        /// Begin updates the linq entities to the database.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateCollection(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems);
        
        /// <summary>
        /// End updates the linq entities to the database.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if update was successful else false.</returns>
        bool EndUpdateCollection(IAsyncResult ar);
        
        /// <summary>
        /// Begin updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems)
            where TypeLinqEntity : class, new();
        
        /// <summary>
        /// End updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if update was successful else false.</returns>
        bool EndUpdateCollection<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();
       
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
    /// Common base data context members.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    public interface IObjectContextProvider<TDataContext>
        where TDataContext : System.Data.Entity.DbContext, new()
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
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface IEdmCacheControl<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Entity.DbContext, new()
        where TLinqEntity : class, new()
    {
        #region IEdmCacheControl Interface

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
        ISelectEdmGenericBase<TDataContext, TLinqEntity>
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
    /// Interface base for all data context objects.
    /// </summary>
    /// <typeparam name="T1DataContextBase">The data context type.</typeparam>
    public interface IEdmContext<T1DataContextBase>
        where T1DataContextBase : System.Data.Entity.DbContext, new()
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
    public interface IEdmContext<T1DataContextBase, T2DataContextBase>
        where T1DataContextBase : System.Data.Entity.DbContext, new()
        where T2DataContextBase : System.Data.Entity.DbContext, new()
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
    public interface IEdmContext<T1DataContextBase, T2DataContextBase, T3DataContextBase>
        where T1DataContextBase : System.Data.Entity.DbContext, new()
        where T2DataContextBase : System.Data.Entity.DbContext, new()
        where T3DataContextBase : System.Data.Entity.DbContext, new()
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
    public interface IEdmContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase>
        where T1DataContextBase : System.Data.Entity.DbContext, new()
        where T2DataContextBase : System.Data.Entity.DbContext, new()
        where T3DataContextBase : System.Data.Entity.DbContext, new()
        where T4DataContextBase : System.Data.Entity.DbContext, new()
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
    public interface IEdmContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase, T5DataContextBase>
        where T1DataContextBase : System.Data.Entity.DbContext, new()
        where T2DataContextBase : System.Data.Entity.DbContext, new()
        where T3DataContextBase : System.Data.Entity.DbContext, new()
        where T4DataContextBase : System.Data.Entity.DbContext, new()
        where T5DataContextBase : System.Data.Entity.DbContext, new()
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
