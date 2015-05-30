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

using Nequeo.Data.DataType;
using Nequeo.Data.Control;
using Nequeo.Linq.Extension;

namespace Nequeo.Data.LinqToSql
{
    /// <summary>
    /// The interface linq base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface ILinqToSqlAnonymousGenericBase<TDataContext, TLinqEntity> : IDisposable
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region ILinqGenericBase Interface

        #region Select Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> Select { get; }

        #endregion

        #region Delete Properties
        /// <summary>
        /// Gets, the delete generic members.
        /// </summary>
        IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> Delete { get; }

        #endregion

        #region Insert Properties
        /// <summary>
        /// Gets, the insert generic members.
        /// </summary>
        IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> Insert { get; }

        #endregion

        #region Update Properties
        /// <summary>
        /// Gets, the update generic members.
        /// </summary>
        IUpdateLinqToSqlGenericBase<TDataContext, TLinqEntity> Update { get; }

        #endregion

        #region Data Base Properties
        /// <summary>
        /// Gets, the data generic base members.
        /// </summary>
        ILinqToSqlDataGenericBase<TDataContext, TLinqEntity> Common { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The interface linq base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface ILinqToSqlGenericBase<TDataContext, TLinqEntity> : IDisposable
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region ILinqGenericBase Interface

        #region Select Properties
        /// <summary>
        /// Gets, the select generic members.
        /// </summary>
        ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> Select { get; }

        #endregion

        #region Delete Properties
        /// <summary>
        /// Gets, the delete generic members.
        /// </summary>
        IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> Delete { get; }

        #endregion

        #region Insert Properties
        /// <summary>
        /// Gets, the insert generic members.
        /// </summary>
        IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> Insert { get; }

        #endregion

        #region Update Properties
        /// <summary>
        /// Gets, the update generic members.
        /// </summary>
        IUpdateLinqToSqlGenericBase<TDataContext, TLinqEntity> Update { get; }

        #endregion

        #region Data Base Properties
        /// <summary>
        /// Gets, the data generic base members.
        /// </summary>
        ILinqToSqlDataGenericBase<TDataContext, TLinqEntity> Common { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The interface base generic data access layer.
    /// </summary>
    /// <typeparam name="TDataContext">The linq connection context type.</typeparam>
    /// <typeparam name="TLinqEntity">The linq entity type.</typeparam>
    public interface ILinqToSqlDataGenericBase<TDataContext, TLinqEntity> :
        IDataContextProvider<TDataContext>, IProcessProvider, IExecuteQuery, IExecuteCommand, IDisposable
            where TDataContext : System.Data.Linq.DataContext, new()
            where TLinqEntity : class, new()
    {
        #region ILinqDataGenericBase Interface

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
        /// Get the row version property for the current type.
        /// </summary>
        /// <returns>The property that is the row version.</returns>
        /// <remarks>This method should only be used if one row version exists.</remarks>
        PropertyInfo GetRowVersion();

        /// <summary>
        /// Get the row version property for the current type.
        /// </summary>
        /// <typeparam name="T">The type to get property information on.</typeparam>
        /// <returns>The property that is the row version.</returns>
        /// <remarks>This method should only be used if one row version exists.</remarks>
        PropertyInfo GetRowVersion<T>();

        /// <summary>
        /// Get all row versioning properties for the current type.
        /// </summary>
        /// <returns>The collection of properties.</returns>
        List<PropertyInfo> GetAllRowVersions();

        /// <summary>
        /// Get all row versioning properties for the current type.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The collection of properties</returns>
        List<PropertyInfo> GetAllRowVersions<T>();

        /// <summary>
        /// Is the current property value a row versioning property.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a row versioner.</returns>
        bool IsRowVersion(PropertyInfo property);

        /// <summary>
        /// Is the current property value auto generated by the database.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is database auto generated.</returns>
        bool IsAutoGenerated(PropertyInfo property);

        /// <summary>
        /// Is the current property value a primary key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a primary key.</returns>
        bool IsPrimaryKey(PropertyInfo property);

        /// <summary>
        /// Is the current property value a foreign key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a foreign key.</returns>
        bool IsForeignKey(PropertyInfo property);

        /// <summary>
        /// Is the current property value an aAssociation key.
        /// </summary>
        /// <param name="property">The current property to examine.</param>
        /// <returns>True if the property is a foreign key.</returns>
        bool IsAssociationKey(PropertyInfo property);

        /// <summary>
        /// Gets the current linq entity column name and schema.
        /// </summary>
        /// <typeparam name="T">The type to examine.</typeparam>
        /// <returns>The column name.</returns>
        string GetDbColumnName<T>(PropertyInfo property);
        
        #endregion

        #region Query Execution Methods
        /// <summary>
        /// Execute an SQL query directly to the database.
        /// </summary>
        /// <param name="sqlQuery">The sql command to execute to the database.</param>
        /// <param name="values">The parameter values for the command, can be null.</param>
        /// <returns>The enumerable collection type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IEnumerable<TLinqEntity> ExecuteQuery(string sqlQuery, params object[] values);

        /// <summary>
        /// Execute an SQL query directly to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="sqlQuery">The sql command to execute to the database.</param>
        /// <param name="values">The parameter values for the command, can be null.</param>
        /// <returns>The enumerable collection type.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IEnumerable<TypeLinqEntity> ExecuteQuery<TypeLinqEntity>(string sqlQuery, params object[] values)
            where TypeLinqEntity : class, new();

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
    public interface ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        IDataContextProvider<TDataContext>, ICacheControl, IProcessProvider,
        ILinqToSqlCacheControl<TDataContext, TLinqEntity>, IExecuteQuery, IDisposable
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region ISelectLinqGenericBase Interface

        #region Properties
        /// <summary>
        /// Gets sets, the data access provider.
        /// </summary>
        IDataAccess DataAccessProvider { get; set; }

        /// <summary>
        /// Gets sets the selection type.
        /// </summary>
        Nequeo.Data.LinqToSql.SelectDataType SelectType { get; set; }

        /// <summary>
        /// Gets sets the change type.
        /// </summary>
        Nequeo.Data.LinqToSql.ChangeDataType ChangeType { get; set; }

        /// <summary>
        /// Gets the IQueryable generic provider
        /// for the current linq entity.
        /// </summary>
        IQueryable<TLinqEntity> IQueryable { get; }

        /// <summary>
        /// Gets the data table containing the
        /// collection of table data.
        /// </summary>
        DataTable DataTable { get; }

        /// <summary>
        /// Gets the current linq entity type.
        /// </summary>
        TLinqEntity LinqEntity { get; }

        /// <summary>
        /// Gets the linq entity type collection.
        /// </summary>
        List<TLinqEntity> LinqCollection { get; }

        /// <summary>
        /// Gets sets, the database connection type.
        /// </summary>
        ConnectionContext.ConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets sets, the database connection data type.
        /// </summary>
        ConnectionContext.ConnectionDataType ConnectionDataType { get; set; }

        #endregion

        #region Select Methods
        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity[] SelectLinqEntities();

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity[] SelectLinqEntities(object keyValue);

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>()
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>(object keyValue)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity[] SelectLinqEntities(string predicate, params object[] values);

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        ///<param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>(string predicate, params object[] values)
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
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TLinqEntity> SelectIQueryableItems(object keyValue);

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TLinqEntity> SelectIQueryableItems(string predicate, params object[] values);

        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTable();

        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTable(object keyValue);

        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTable(string predicate, params object[] values);

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity SelectLinqEntity(object keyValue);

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity SelectLinqEntity<TypeLinqEntity>(object keyValue)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity SelectLinqEntity(string predicate, params object[] values);

        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity SelectLinqEntity<TypeLinqEntity>(string predicate, params object[] values)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity[] SelectLinqEntities(Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Gets all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The array of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity[] SelectLinqEntities<TypeLinqEntity>(Expression<Func<TypeLinqEntity, bool>> predicate)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets the IQueryable generic linq entities.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The collection of linq entities.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        IQueryable<TLinqEntity> SelectIQueryableItems(Expression<Func<TLinqEntity, bool>> predicate);
       
        /// <summary>
        /// Gets the data table of rows.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The data table.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        DataTable SelectDataTable(Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity SelectLinqEntity(Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Gets the linq entity item.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>The linq entity item.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity SelectLinqEntity<TypeLinqEntity>(Expression<Func<TypeLinqEntity, bool>> predicate)
            where TypeLinqEntity : class, new();

        #endregion

        #region Select Data Type Methods
        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <remarks>Uses the current primary key to get single item data.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectDataTypeKey(object keyValue);

        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectDataTypeKey(object keyValue, string keyName);

        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectDataTypePredicate(string predicate, params object[] values);

        /// <summary>
        /// Get the specified data for the selected data type.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectDataTypePredicate(Expression<Func<TLinqEntity, bool>> predicate);

        #endregion

        #region Select Item Methods
        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectItemKey(ref TLinqEntity linqEntity,
            object keyValue, string keyName);

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectItemKey<TypeLinqEntity>(
            ref TypeLinqEntity linqEntity, object keyValue, string keyName)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectItemPredicate(ref TLinqEntity linqEntity,
            string predicate, params object[] values);

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectItemPredicate<TypeLinqEntity>(
            ref TypeLinqEntity linqEntity, string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectItemPredicate(
            ref TLinqEntity linqEntity, Expression<Func<TLinqEntity, bool>> predicate);

        /// <summary>
        /// Gets the first linq entity.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntity">The referenced generic linq entity.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectItemPredicate<TypeLinqEntity>(
            ref TypeLinqEntity linqEntity, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();

        #endregion

        #region Select Collection Methods
        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionKey(ref List<TLinqEntity> linqCollection,
            object keyValue, string keyName);

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionKey<TypeLinqEntity>(
            ref List<TypeLinqEntity> linqCollection, object keyValue, string keyName)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate(
            ref List<TLinqEntity> linqCollection,
            string predicate, params object[] values);

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate<TypeLinqEntity>(
            ref List<TypeLinqEntity> linqCollection, string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionKey(ref DataTable dataTable,
            object keyValue, string keyName);

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionKey<TypeLinqEntity>(
            ref DataTable dataTable, object keyValue, string keyName)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate(ref DataTable dataTable,
            string predicate, params object[] values);

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate<TypeLinqEntity>(
            ref DataTable dataTable, string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionKey(ref IQueryable<TLinqEntity> queryResult,
            object keyValue, string keyName);

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="keyValue">The primary key value, null indicates return all data.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionKey<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult, object keyValue, string keyName)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate(
            ref IQueryable<TLinqEntity> queryResult,
            string predicate, params object[] values);

        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult,
            string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate(
            ref List<TLinqEntity> linqCollection, Expression<Func<TLinqEntity, bool>> predicate);

        /// <summary>
        /// Gets a generic linq collection of linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqCollection">The referenced generic linq collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate<TypeLinqEntity>(
            ref List<TypeLinqEntity> linqCollection, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate(
            ref DataTable dataTable, Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Gets a collection of rows inserted into a data table.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The referenced data table.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate<TypeLinqEntity>(
            ref DataTable dataTable, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();
        
        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate(
            ref IQueryable<TLinqEntity> queryResult,
            Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Gets a collection of IQueryable provider linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="queryResult">The referenced IQueryable provider collection.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the data was returned else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool SelectCollectionPredicate<TypeLinqEntity>(
            ref IQueryable<TypeLinqEntity> queryResult,
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();

        #endregion

        #region Asynchronous Select Methods

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectLinqToSqlEntities(AsyncCallback callback, object state);

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectLinqToSqlEntities(AsyncCallback callback,
            object state, object keyValue);

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectLinqToSqlEntities(AsyncCallback callback,
            object state, string predicate, params object[] values);

        /// <summary>
        /// End get all linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        TLinqEntity[] EndSelectLinqToSqlEntities(IAsyncResult ar);

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback, object state)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, object keyValue)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Begin get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// End get all linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        TypeLinqEntity[] EndSelectLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Begin get the data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable(AsyncCallback callback, object state);

        /// <summary>
        /// Begin get the data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable(AsyncCallback callback,
            object state, object keyValue);

        /// <summary>
        /// Begin get the data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectDataTable(AsyncCallback callback,
            object state, string predicate, params object[] values);

        /// <summary>
        /// End get the data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        DataTable EndSelectDataTable(IAsyncResult ar);

        /// <summary>
        /// Begin get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback, object state);

        /// <summary>
        /// Begin get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback,
           object state, object keyValue);

        /// <summary>
        /// Begin get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginSelectIQueryableItems(AsyncCallback callback,
            object state, string predicate, params object[] values);

        /// <summary>
        /// End get the IQueryable generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The array of linq entities.</returns>
        IQueryable<TLinqEntity> EndSelectIQueryableItems(IAsyncResult ar);

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
    public interface IDeleteLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        IDataContextProvider<TDataContext>, IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region IDeleteLinqGenericBase Interface

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

        #region Delete Collection Methods
        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionKey(object keyValue, string keyName);

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionKey<TypeLinqEntity>(
            object keyValue, string keyName)
                where TypeLinqEntity : class;

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionKey(object keyValue, string keyName,
            object rowVersionData, string rowVersionName);


        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionKey<TypeLinqEntity>(
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class;

        #endregion

        #region Delete Item Methods
        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey(object keyValue, string keyName);

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey<TypeLinqEntity>(
            object keyValue, string keyName)
                where TypeLinqEntity : class;

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey(object keyValue, string keyName,
            object rowVersionData, string rowVersionName);

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey<TypeLinqEntity>(
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class;

        #endregion

        #region Delete Predicate Methods
        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate(
            string predicate, params object[] values);

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate<TypeLinqEntity>(
            string predicate, params object[] values)
                where TypeLinqEntity : class;

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate(
            string predicate, params object[] values);

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate<TypeLinqEntity>(
            string predicate, params object[] values)
                where TypeLinqEntity : class;

        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate(
            Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Deletes the collection of linq entities found.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollectionPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class;

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate(
            Expression<Func<TLinqEntity, bool>> predicate);
        
        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemPredicate<TypeLinqEntity>(
            Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class;

        #endregion

        #region Delete Item With Key Methods
        /// <summary>
        /// Deletes the linq entity from the database.
        /// </summary>
        /// <param name="linqEntityItem">The linq entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItem(TLinqEntity linqEntityItem, bool useRowVersion);

        /// <summary>
        /// Deletes the linq entity from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItem<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, bool useRowVersion)
                where TypeLinqEntity : class, new();

        #endregion

        #region Delete Collection With Key Methods
        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollection(TLinqEntity[] linqEntityItems, bool useRowVersion);

        /// <summary>
        /// Deletes the data entities from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to delete items.</param>
        /// <returns>True if delete was successful else false.</returns>
        /// <remarks>A false result may indicated that no delete was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteCollection<TypeLinqEntity>(
            TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new();

        #endregion

        #region Delete Methods
        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <param name="keyValue">The value to search on.</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey(object keyValue);

        /// <summary>
        /// Deletes the linq entity item from the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="keyValue">The value to search on</param>
        /// <returns>True if the item was deleted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool DeleteItemKey<TypeLinqEntity>(object keyValue)
            where TypeLinqEntity : class;

        #endregion

        #region Asynchronous Delete Methods
        /// <summary>
        /// Begin delete linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteLinqToSqlEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems, bool useRowVersion);

        /// <summary>
        /// End delete linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        Boolean EndDeleteLinqToSqlEntities(IAsyncResult ar);

        /// <summary>
        /// Begin delete linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to delete.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// End delete linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        Boolean EndDeleteLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Begin delete collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteCollection(AsyncCallback callback,
            object state, string predicate, params object[] values);

        /// <summary>
        /// End delete collection.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        Boolean EndDeleteCollection(IAsyncResult ar);

        /// <summary>
        /// Begin delete collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginDeleteCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// End delete collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if deleted items else false.</returns>
        Boolean EndDeleteCollection<TypeLinqEntity>(IAsyncResult ar)
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
    public interface IInsertLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        IDataContextProvider<TDataContext>, IProcessProvider, IExecuteCommand, IDisposable
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region IInsertLinqGenericBase Interface

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

        /// <summary>
        /// Insert the collection of data rows.
        /// </summary>
        /// <param name="dataTable">The data table with data rows.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity[] InsertCollection(DataTable dataTable);

        /// <summary>
        /// Insert the collection of data rows.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The data table with data rows.</param>
        /// <returns>The array of linq entities with new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity[] InsertCollection<TypeLinqEntity>(
            DataTable dataTable)
                where TypeLinqEntity : class, new();

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
        TypeLinqEntity InsertItem<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Insert the Column Enity to the database.
        /// </summary>
        /// <param name="dataRow">The data row containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TLinqEntity InsertItem(DataRow dataRow);

        /// <summary>
        /// Insert the Column Enity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row containing the data to insert.</param>
        /// <returns>The linq entity containg the new values.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        TypeLinqEntity InsertItem<TypeLinqEntity>(
            DataRow dataRow)
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
        /// Inserts the data table rows.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertDataTable(DataTable dataTable);

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

        /// <summary>
        /// Inserts the data row.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        /// <returns>True if inserted else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool InsertDataRow(DataRow dataRow);

        #endregion

        #region Asynchronous Insert Methods

        /// <summary>
        /// Begin insert generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The linq entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertLinqToSqlEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntities);

        /// <summary>
        /// End insert generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        Boolean EndInsertLinqToSqlEntities(IAsyncResult ar);

        /// <summary>
        /// Begin insert data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertDataTable(AsyncCallback callback,
            object state, DataTable dataTable);

        /// <summary>
        /// End insert data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        Boolean EndInsertDataTable(IAsyncResult ar);

        /// <summary>
        /// Begin insert generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The linq entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertTypeLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntities)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// End insert generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if inserted items else false.</returns>
        Boolean EndInsertTypeLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Begin insert collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The linq entities to insert.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertCollection(AsyncCallback callback,
            object state, TLinqEntity[] linqEntities);

        /// <summary>
        /// Begin insert collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertCollection(AsyncCallback callback,
            object state, DataTable dataTable);

        /// <summary>
        /// End insert collection.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>Collection of type data.</returns>
        TLinqEntity[] EndInsertCollection(IAsyncResult ar);

        /// <summary>
        /// Begin insert collection.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntities">The data table.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginInsertTypeCollection<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntities)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// End insert collection.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>Collection of type data.</returns>
        TypeLinqEntity[] EndInsertTypeCollection<TypeLinqEntity>(IAsyncResult ar)
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
    public interface IUpdateLinqToSqlGenericBase<TDataContext, TLinqEntity> :
        IDataContextProvider<TDataContext>, IProcessProvider, IConcurrencyControl, IExecuteCommand, IDisposable
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region IUpdateLinqGenericBase Interface

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
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(TLinqEntity linqEntityItem,
            object keyValue, string keyName);

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, object keyValue, string keyName)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(TLinqEntity linqEntityItem,
            object keyValue, string keyName, object rowVersionData, string rowVersionName);

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem,
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(DataRow dataRow, object keyValue, string keyName);

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeLinqEntity>(
            DataRow dataRow, object keyValue, string keyName)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey(DataRow dataRow,
            object keyValue, string keyName, object rowVersionData,
            string rowVersionName);

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="keyValue">The primary key value.</param>
        /// <param name="keyName">The primary key name.</param>
        /// <param name="rowVersionData">The row version data.</param>
        /// <param name="rowVersionName">The row version name.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemKey<TypeLinqEntity>(
            DataRow dataRow,
            object keyValue, string keyName, object rowVersionData, string rowVersionName)
                where TypeLinqEntity : class, new();

        #endregion

        #region Update Predicate Methods
        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(
            DataRow dataRow, Expression<Func<TLinqEntity, bool>> predicate);

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeLinqEntity>(
            DataRow dataRow, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(
            TLinqEntity linqEntityItem, Expression<Func<TLinqEntity, bool>> predicate);
       
        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, Expression<Func<TypeLinqEntity, bool>> predicate)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(TLinqEntity linqEntityItem,
            string predicate, params object[] values);

        /// <summary>
        /// Updates the Linq Entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem,
            string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate(DataRow dataRow,
            string predicate, params object[] values);

        /// <summary>
        /// Updates the DataRow to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataRow">The data row to update.</param>
        /// <param name="predicate">The predicate string to search.</param>
        /// <param name="values">The values associated with the predicate string.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItemPredicate<TypeLinqEntity>(
            DataRow dataRow, string predicate, params object[] values)
                where TypeLinqEntity : class, new();

        #endregion

        #region Update Collection Methods

        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateCollection(TLinqEntity[] linqEntityItems, bool useRowVersion);

        /// <summary>
        /// Updates the linq entities to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateCollection<TypeLinqEntity>(
            TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// Updates the data table to the database.
        /// </summary>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateCollection(DataTable dataTable);

        /// <summary>
        /// Updates the data table to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateCollection<TypeLinqEntity>(
            DataTable dataTable)
                where TypeLinqEntity : class, new();

        #endregion

        #region Update Item With Key Methods

        /// <summary>
        /// Updates the linq entity to the database.
        /// </summary>
        /// <param name="linqEntityItem">The linq entity to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItem(TLinqEntity linqEntityItem, bool useRowVersion);

        /// <summary>
        /// Updates the linq entity to the database.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="linqEntityItem">The linq entity to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>True if update was successful else false.</returns>
        /// <remarks>A false result may indicated that no update was performed
        /// or a concurrency error may have occurred.</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool UpdateItem<TypeLinqEntity>(
            TypeLinqEntity linqEntityItem, bool useRowVersion)
                where TypeLinqEntity : class, new();

        #endregion

        #region Asynchronous Update Methods

        /// <summary>
        /// Begin update generic linq entities.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateLinqToSqlEntities(AsyncCallback callback,
            object state, TLinqEntity[] linqEntityItems, bool useRowVersion);

        /// <summary>
        /// End update generic linq entities.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated items else false.</returns>
        Boolean EndUpdateLinqToSqlEntities(IAsyncResult ar);

        /// <summary>
        /// Begin update generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="linqEntityItems">The linq entities to update.</param>
        /// <param name="useRowVersion">Should row version data be used to update items.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateTypeLinqToSqlEntities<TypeLinqEntity>(AsyncCallback callback,
            object state, TypeLinqEntity[] linqEntityItems, bool useRowVersion)
                where TypeLinqEntity : class, new();

        /// <summary>
        /// End update generic linq entities.
        /// </summary>
        /// <typeparam name="TypeLinqEntity">The type to examine.</typeparam>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated items else false.</returns>
        Boolean EndUpdateTypeLinqToSqlEntities<TypeLinqEntity>(IAsyncResult ar)
            where TypeLinqEntity : class, new();

        /// <summary>
        /// Begin update data table.
        /// </summary>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <param name="dataTable">The data table to update.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginUpdateDataTable(AsyncCallback callback,
            object state, DataTable dataTable);

        /// <summary>
        /// End update data table.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if updated items else false.</returns>
        Boolean EndUpdateDataTable(IAsyncResult ar);

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
    public interface IDataContextProvider<TDataContext>
        where TDataContext : System.Data.Linq.DataContext, new()
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
    public interface ILinqToSqlCacheControl<TDataContext, TLinqEntity>
        where TDataContext : System.Data.Linq.DataContext, new()
        where TLinqEntity : class, new()
    {
        #region ILinqCacheControl Interface

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
        ISelectLinqToSqlGenericBase<TDataContext, TLinqEntity>
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
    public interface ILinqToSqlContext<T1DataContextBase>
        where T1DataContextBase : System.Data.Linq.DataContext, new()
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
    public interface ILinqToSqlContext<T1DataContextBase, T2DataContextBase>
        where T1DataContextBase : System.Data.Linq.DataContext, new()
        where T2DataContextBase : System.Data.Linq.DataContext, new()
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
    public interface ILinqToSqlContext<T1DataContextBase, T2DataContextBase, T3DataContextBase>
        where T1DataContextBase : System.Data.Linq.DataContext, new()
        where T2DataContextBase : System.Data.Linq.DataContext, new()
        where T3DataContextBase : System.Data.Linq.DataContext, new()
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
    public interface ILinqToSqlContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase>
        where T1DataContextBase : System.Data.Linq.DataContext, new()
        where T2DataContextBase : System.Data.Linq.DataContext, new()
        where T3DataContextBase : System.Data.Linq.DataContext, new()
        where T4DataContextBase : System.Data.Linq.DataContext, new()
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
    public interface ILinqToSqlContext<T1DataContextBase, T2DataContextBase, T3DataContextBase, T4DataContextBase, T5DataContextBase>
        where T1DataContextBase : System.Data.Linq.DataContext, new()
        where T2DataContextBase : System.Data.Linq.DataContext, new()
        where T3DataContextBase : System.Data.Linq.DataContext, new()
        where T4DataContextBase : System.Data.Linq.DataContext, new()
        where T5DataContextBase : System.Data.Linq.DataContext, new()
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
