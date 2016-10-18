/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Linq.Expressions;

using Nequeo.Extension;
using Nequeo.DataAccess.SearchProvider.Data;

namespace Nequeo.DataAccess.SearchProvider
{
    /// <summary>
    /// Search provider database.
    /// </summary>
    public partial class Provider : IDisposable
    {
        /// <summary>
        /// Nequeo database search provider.
        /// </summary>
        public Provider()
        {
            _context = new DataContext();
        }

        private DataContext _context = null;

        /// <summary>
        /// Update the data.
        /// </summary>
        /// <typeparam name="T">The data type to update.</typeparam>
        /// <param name="dataEntities">The data entities to update.</param>
        /// <returns>True if updated; else false.</returns>
        public virtual bool Update<T>(T[] dataEntities)
            where T : class, new()
        {
            return _context.Update<T>().UpdateDataEntities(dataEntities);
        }

        /// <summary>
        /// Delete the data.
        /// </summary>
        /// <typeparam name="T">The data type to delete.</typeparam>
        /// <param name="dataEntities">The data entities to delete.</param>
        /// <returns>True if deleted; else false.</returns>
        public virtual bool Delete<T>(T[] dataEntities)
            where T : class, new()
        {
            return _context.Delete<T>().DeleteDataEntities(dataEntities);
        }

        /// <summary>
        /// Select the data.
        /// </summary>
        /// <typeparam name="T">The data type to select.</typeparam>
        /// <param name="query">The query to search for.</param>
        /// <returns>The list of items found.</returns>
        public virtual T[] Select<T>(string query)
            where T : class, new()
        {
            return _context.GetTable<T>().Where(CreateLambdaExpression<T>(query)).ToArray();
        }

        /// <summary>
        /// Insert the data.
        /// </summary>
        /// <typeparam name="T">The data type to insert.</typeparam>
        /// <param name="dataEntity">The data type to insert.</param>
        /// <param name="checkIfExists">If true then do not insert the entity if it already exists; else false.</param>
        /// <returns>The identity of the inserted data; else 0.</returns>
        public virtual long Insert<T>(T dataEntity, bool checkIfExists = true)
            where T : class, new()
        {
            long id = 0;
            List<object> ids = null;

            // If true then do not insert the entity 
            // if it already exists; else false.
            if (checkIfExists)
            {
                bool exists = false;

                // Get the current entity.
                T[] result = _context.GetTable<T>().Where(CreateLambdaExpression<T>(dataEntity)).ToArray();

                // The current entity exists.
                if (result.Length > 0)
                {
                    PropertyInfo searchTextProp = result.First().GetType().GetProperty("SearchText");
                    id = (long)searchTextProp.GetValue(result.First());
                    exists = true;
                }

                // If it does not exist then insert the entity.
                if (!exists)
                {
                    // Select the current data type
                    switch (_context.ProviderConnectionDataType)
                    {
                        case Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                            ids = _context.Insert<T>().InsertDataEntity(dataEntity, "RETURNING DataStoreID");
                            break;
                        case Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.OracleDataType:
                            ids = _context.Insert<T>().InsertDataEntity(dataEntity, "RETURNING DataStoreID INTO :newrowid");
                            break;
                        default:
                            ids = _context.Insert<T>().InsertDataEntity(dataEntity);
                            break;
                    }
                }
            }
            else
            {
                // Select the current data type
                switch (_context.ProviderConnectionDataType)
                {
                    case Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.PostgreSqlDataType:
                        ids = _context.Insert<T>().InsertDataEntity(dataEntity, "RETURNING DataStoreID");
                        break;
                    case Nequeo.Data.DataType.ConnectionContext.ConnectionDataType.OracleDataType:
                        ids = _context.Insert<T>().InsertDataEntity(dataEntity, "RETURNING DataStoreID INTO :newrowid");
                        break;
                    default:
                        ids = _context.Insert<T>().InsertDataEntity(dataEntity);
                        break;
                }
            }

            // If ids exist, then get the first.
            if (ids != null && ids.Count > 0)
                id = (long)ids.First();

            // Return the inserted id.
            return id;
        }

        /// <summary>
        /// Insert the data.
        /// </summary>
        /// <typeparam name="T">The data type to insert.</typeparam>
        /// <param name="dataEntities">The data type to insert.</param>
        /// <param name="checkIfExists">If true then do not insert the entity if it already exists; else false.</param>
        /// <returns>True if inserted; else false.</returns>
        public virtual bool Insert<T>(T[] dataEntities, bool checkIfExists = true)
            where T : class, new()
        {
            // If true then do not insert the entity 
            // if it already exists; else false.
            if (checkIfExists)
            {
                // For each entity.
                for (int i = 0; i < dataEntities.Length; i++)
                {
                    bool exists = false;

                    // Get the current entity.
                    T[] result = _context.GetTable<T>().Where(CreateLambdaExpression<T>(dataEntities[i])).ToArray();

                    // The current entity exists.
                    if (result.Length > 0)
                        exists = true;

                    // If it does not exist then insert the entity.
                    if (!exists)
                        _context.Insert<T>().InsertDataEntity(dataEntities[i]);
                }
                return true;
            }
            else
                return _context.Insert<T>().InsertDataEntities(dataEntities);
        }

        /// <summary>
        /// Get the total count for the search.
        /// </summary>
        /// <param name="queries">The queries to serach for.</param>
        /// <param name="skip">The number of items to skip before taking.</param>
        /// <returns>The total count.</returns>
        public virtual long GetCount(Nequeo.Model.QueryModel[] queries, int skip = 0)
        {
            long count = 0;

            // Get all items.
            Dictionary<Enum.EnumDataStoreTableName, long[]> result = GetSearchIdentifiers(queries, skip: skip);

            // For each query.
            foreach (KeyValuePair<Enum.EnumDataStoreTableName, long[]> item in result)
            {
                // Count the items
                count += (long)item.Value.Length;
            }

            // Return the count.
            return count;
        }

        /// <summary>
        /// Get unique identifiers for the queries that correspond to the table name.
        /// </summary>
        /// <param name="queries">The queries to serach for.</param>
        /// <param name="take">The number of items to take (-1 : take all).</param>
        /// <param name="skip">The number of items to skip before taking.</param>
        /// <returns>The list of data store ids for each table name.</returns>
        public virtual Dictionary<Enum.EnumDataStoreTableName, long[]> GetSearchIdentifiers(Nequeo.Model.QueryModel[] queries, int take = -1, int skip = 0)
        {
            Dictionary<Enum.EnumDataStoreTableName, long[]> result = new Dictionary<Enum.EnumDataStoreTableName, long[]>();
            Dictionary<Enum.EnumDataStoreTableName, Nequeo.Model.QueryModel[]> queryItems =
                new Dictionary<Enum.EnumDataStoreTableName, Nequeo.Model.QueryModel[]>();

            // For each query.
            foreach (Nequeo.Model.QueryModel query in queries)
            {
                // If text exists.
                if (query.Query.Length > 0)
                {
                    // If the query is a phrase.
                    if (query.QueryType == Model.QueryType.Phrase)
                    {
                        // Add the phrase query.
                        AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Phrase);
                    }
                    else
                    {
                        // Find all other queries.
                        FindQuery(queryItems, query);
                    }
                }
            }

            // For each query.
            foreach (KeyValuePair<Enum.EnumDataStoreTableName, Nequeo.Model.QueryModel[]> item in queryItems)
            {
                // Add the query result.
                result.Add(item.Key, GetSearchIdentifiers(item.Value, item.Key, take, skip));
            }

            // Return the result.
            return result;
        }

        /// <summary>
        /// Get unique identifiers for the queries that correspond to the table name.
        /// </summary>
        /// <param name="queries">The set of query texts that match the table name.</param>
        /// <param name="tableName">The table name to search in.</param>
        /// <param name="take">The number of items to take (-1 : take all).</param>
        /// <param name="skip">The number of items to skip before taking.</param>
        /// <returns>The list (stored) of data store ids that correspond to the table name.</returns>
        public virtual long[] GetSearchIdentifiers(Nequeo.Model.QueryModel[] queries, Enum.EnumDataStoreTableName tableName, int take = -1, int skip = 0)
        {
            List<long> dataStoreIDs = new List<long>();

            // Get the search table name.
            switch (tableName)
            {
                case Enum.EnumDataStoreTableName.A:
                    foreach (Data.DataStoreA item in GetQueryable<Data.DataStoreA>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.B:
                    foreach (Data.DataStoreB item in GetQueryable<Data.DataStoreB>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.C:
                    foreach (Data.DataStoreC item in GetQueryable<Data.DataStoreC>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.D:
                    foreach (Data.DataStoreD item in GetQueryable<Data.DataStoreD>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.E:
                    foreach (Data.DataStoreE item in GetQueryable<Data.DataStoreE>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.F:
                    foreach (Data.DataStoreF item in GetQueryable<Data.DataStoreF>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.G:
                    foreach (Data.DataStoreG item in GetQueryable<Data.DataStoreG>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.H:
                    foreach (Data.DataStoreH item in GetQueryable<Data.DataStoreH>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.I:
                    foreach (Data.DataStoreI item in GetQueryable<Data.DataStoreI>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.J:
                    foreach (Data.DataStoreJ item in GetQueryable<Data.DataStoreJ>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.K:
                    foreach (Data.DataStoreK item in GetQueryable<Data.DataStoreK>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.L:
                    foreach (Data.DataStoreL item in GetQueryable<Data.DataStoreL>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.M:
                    foreach (Data.DataStoreM item in GetQueryable<Data.DataStoreM>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.N:
                    foreach (Data.DataStoreN item in GetQueryable<Data.DataStoreN>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.O:
                    foreach (Data.DataStoreO item in GetQueryable<Data.DataStoreO>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.P:
                    foreach (Data.DataStoreP item in GetQueryable<Data.DataStoreP>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.Q:
                    foreach (Data.DataStoreQ item in GetQueryable<Data.DataStoreQ>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.R:
                    foreach (Data.DataStoreR item in GetQueryable<Data.DataStoreR>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.S:
                    foreach (Data.DataStoreS item in GetQueryable<Data.DataStoreS>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.T:
                    foreach (Data.DataStoreT item in GetQueryable<Data.DataStoreT>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.U:
                    foreach (Data.DataStoreU item in GetQueryable<Data.DataStoreU>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.V:
                    foreach (Data.DataStoreV item in GetQueryable<Data.DataStoreV>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.W:
                    foreach (Data.DataStoreW item in GetQueryable<Data.DataStoreW>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.X:
                    foreach (Data.DataStoreX item in GetQueryable<Data.DataStoreX>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.Y:
                    foreach (Data.DataStoreY item in GetQueryable<Data.DataStoreY>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.Z:
                    foreach (Data.DataStoreZ item in GetQueryable<Data.DataStoreZ>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.Number:
                    foreach (Data.DataStoreNumber item in GetQueryable<Data.DataStoreNumber>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.Phrase:
                    foreach (Data.DataStorePhrase item in GetQueryable<Data.DataStorePhrase>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                case Enum.EnumDataStoreTableName.Special:
                    foreach (Data.DataStoreSpecial item in GetQueryable<Data.DataStoreSpecial>(queries, take, skip))
                        dataStoreIDs.Add(item.DataStoreID);

                    break;

                default:
                    break;
            }

            // Return the list of data store ids.
            dataStoreIDs.Sort();
            return dataStoreIDs.ToArray();
        }

        /// <summary>
        /// Create the orderby lambda expression.
        /// </summary>
        /// <typeparam name="TDataEntity">The expression parameter type.</typeparam>
        /// <typeparam name="TKey">The key property type for the expression parameter type.</typeparam>
        /// <returns>The lambda expression.</returns>
        private Expression<Func<TDataEntity, TKey>> CreateOrderByLambdaExpression<TDataEntity, TKey>()
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the initial expression.
            PropertyInfo property = typeof(TDataEntity).GetProperty("DataStoreID");
            Expression propertyMember = Expression.MakeMemberAccess(paramExpr, property);

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, TKey>> predicate =
                Expression<Func<TDataEntity, TKey>>.Lambda<Func<TDataEntity, TKey>>(propertyMember, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression.
        /// </summary>
        /// <typeparam name="TDataEntity">The expression parameter type.</typeparam>
        /// <param name="query">The data entity to search the value for.</param>
        /// <returns>The lambda expression.</returns>
        private Expression<Func<TDataEntity, bool>> CreateLambdaExpression<TDataEntity>(string query)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the initial expression.
            PropertyInfo property = typeof(TDataEntity).GetProperty("SearchText");
            Expression propertyMember = Expression.MakeMemberAccess(paramExpr, property);

            // The resulting expression.
            Expression result = Expression.Equal(propertyMember, Expression.Constant(query, typeof(string)));

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, bool>> predicate =
                Expression<Func<TDataEntity, bool>>.Lambda<Func<TDataEntity, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression.
        /// </summary>
        /// <typeparam name="TDataEntity">The expression parameter type.</typeparam>
        /// <param name="entity">The data entity to search the value for.</param>
        /// <returns>The lambda expression.</returns>
        private Expression<Func<TDataEntity, bool>> CreateLambdaExpression<TDataEntity>(TDataEntity entity)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the initial expression.
            PropertyInfo property = typeof(TDataEntity).GetProperty("SearchText");
            Expression propertyMember = Expression.MakeMemberAccess(paramExpr, property);

            // The resulting expression.
            Expression result = Expression.Equal(propertyMember, Expression.Constant(property.GetValue(entity), typeof(string)));

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, bool>> predicate =
                Expression<Func<TDataEntity, bool>>.Lambda<Func<TDataEntity, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the lambda expression.
        /// </summary>
        /// <typeparam name="TDataEntity">The expression parameter type.</typeparam>
        /// <param name="queries">The queries to search for.</param>
        /// <returns>The lambda expression.</returns>
        private Expression<Func<TDataEntity, bool>> CreateLambdaExpression<TDataEntity>(Nequeo.Model.QueryModel[] queries)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // The resulting expression.
            Expression result = null;
            Expression[] created = new Expression[queries.Length];
            Expression swap = null;

            // Create the initial expression.
            PropertyInfo property = typeof(TDataEntity).GetProperty("SearchText");
            Expression propertyMember = Expression.MakeMemberAccess(paramExpr, property);

            // If more than one expression.
            if (queries.Length > 1)
            {
                // For each query create the expression.
                for (int i = 0; i < queries.Length; i++)
                {
                    // Create the cuurent expression.
                    created[i] = CreateExpression(queries[i], propertyMember);
                }

                // Get the first two exressions.
                swap = CreateCombinedExpression(created[0], created[1], ExpressionType.OrElse);

                // For each query created.
                for (int i = 2; i < created.Length; i++)
                {
                    // Get the new swap.
                    swap = CreateCombinedExpression(swap, created[i], ExpressionType.OrElse);
                }

                // Assign the final expression result.
                result = swap;
            }
            else
            {
                // If only one expression.
                result = CreateExpression(queries[0], propertyMember);
            }

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, bool>> predicate =
                Expression<Func<TDataEntity, bool>>.Lambda<Func<TDataEntity, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the combined expression.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <param name="operand">The joining operand.</param>
        /// <returns>The combined expression.</returns>
        private Expression CreateCombinedExpression(Expression left, Expression right, ExpressionType operand)
        {
            switch (operand)
            {
                case ExpressionType.AndAlso:
                    return Expression.AndAlso(left, right);

                case ExpressionType.OrElse:
                    return Expression.OrElse(left, right);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Create the expression.
        /// </summary>
        /// <param name="query">The current query.</param>
        /// <param name="propertyMember">The current property member.</param>
        /// <returns>The new expression.</returns>
        private Expression CreateExpression(Nequeo.Model.QueryModel query, Expression propertyMember)
        {
            // If match partial
            if (query.MatchPartialQuery)
            {
                // Call the contains method.
                return CreateExpression(query, propertyMember, ExpressionType.Call);
            }
            else
            {
                // Call the equal method.
                return CreateExpression(query, propertyMember, ExpressionType.Equal);
            }
        }

        /// <summary>
        /// Create the expression.
        /// </summary>
        /// <param name="query">The current query.</param>
        /// <param name="propertyMember">The current property member.</param>
        /// <param name="operand">The operand.</param>
        /// <returns>The new expression.</returns>
        private Expression CreateExpression(Nequeo.Model.QueryModel query, Expression propertyMember, ExpressionType operand)
        {
            switch (operand)
            {
                case ExpressionType.Equal:
                    return Expression.Equal(propertyMember, Expression.Constant(query.Query, typeof(string)));

                case ExpressionType.GreaterThan:
                    return Expression.GreaterThan(propertyMember, Expression.Constant(query.Query, typeof(string)));

                case ExpressionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(propertyMember, Expression.Constant(query.Query, typeof(string)));

                case ExpressionType.LessThan:
                    return Expression.LessThan(propertyMember, Expression.Constant(query.Query, typeof(string)));

                case ExpressionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(propertyMember, Expression.Constant(query.Query, typeof(string)));

                case ExpressionType.NotEqual:
                    return Expression.NotEqual(propertyMember, Expression.Constant(query.Query, typeof(string)));

                case ExpressionType.Call:
                    return Expression.Call(
                        propertyMember,
                        typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                        Expression.Constant(query.Query));

                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the Queryable result.
        /// </summary>
        /// <typeparam name="T">The expression parameter type.</typeparam>
        /// <param name="queries">The set of query texts that match the table name.</param>
        /// <param name="take">The number of items to take (-1 : take all).</param>
        /// <param name="skip">The number of items to skip before taking.</param>
        /// <returns>The query result.</returns>
        private IQueryable<T> GetQueryable<T>(Nequeo.Model.QueryModel[] queries, int take, int skip)
            where T : class, new()
        {
            if (take > -1 && skip > 0)
                return _context.GetTable<T>().Where(CreateLambdaExpression<T>(queries)).OrderBy(CreateOrderByLambdaExpression<T, long>()).Take(take).Skip(skip);
            else if (skip > 0)
                return _context.GetTable<T>().Where(CreateLambdaExpression<T>(queries)).OrderBy(CreateOrderByLambdaExpression<T, long>()).Skip(skip);
            else if (take > -1)
                return _context.GetTable<T>().Where(CreateLambdaExpression<T>(queries)).OrderBy(CreateOrderByLambdaExpression<T, long>()).Take(take);
            else
                return _context.GetTable<T>().Where(CreateLambdaExpression<T>(queries)).OrderBy(CreateOrderByLambdaExpression<T, long>());
        }

        /// <summary>
        /// Add the query
        /// </summary>
        /// <param name="queryItems">The query list.</param>
        /// <param name="query">The current query.</param>
        /// <param name="tableName">The query table name.</param>
        private void AddQuery(
            Dictionary<Enum.EnumDataStoreTableName, Nequeo.Model.QueryModel[]> queryItems,
            Nequeo.Model.QueryModel query, Enum.EnumDataStoreTableName tableName)
        {
            // If the query does not exist.
            if (queryItems[tableName] == null)
            {
                // Add the new query.
                queryItems.Add(tableName, new Model.QueryModel[] { query });
            }
            else
            {
                // Combine the queries.
                Model.QueryModel[] model = queryItems[tableName];
                Model.QueryModel[] modelResult = model.CombineParallel(new Model.QueryModel[] { query });
                queryItems[tableName] = modelResult;
            }
        }

        /// <summary>
        /// Find the query
        /// </summary>
        /// <param name="queryItems">The query list.</param>
        /// <param name="query">The current query.</param>
        private void FindQuery(
            Dictionary<Enum.EnumDataStoreTableName, Nequeo.Model.QueryModel[]> queryItems, 
            Nequeo.Model.QueryModel query)
        {
            // If is number.
            if (query.QueryType == Model.QueryType.Number)
            {
                // Add the number query.
                AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Number);
            }
            else
            {
                // If is special.
                if (query.QueryType == Model.QueryType.Special)
                {
                    // Add the special query.
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Special);
                }
                else
                {
                    // If match partial
                    if (query.MatchPartialQuery)
                    {
                        // Look in all the data stores A - Z
                        FindQueryExAllDefault(queryItems, query);
                    }
                    else
                    {
                        // Only look in the appropriate.
                        FindQueryExDefault(queryItems, query);
                    }
                }
            }
        }

        /// <summary>
        /// Find the query default.
        /// </summary>
        /// <param name="queryItems">The query list.</param>
        /// <param name="query">The current query.</param>
        private void FindQueryExDefault(
            Dictionary<Enum.EnumDataStoreTableName, Nequeo.Model.QueryModel[]> queryItems,
            Nequeo.Model.QueryModel query)
        {
            // Find all letters and default all special chars.
            switch (query.Query[0])
            {
                case 'A':
                case 'a':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.A);
                    break;

                case 'B':
                case 'b':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.B);
                    break;

                case 'C':
                case 'c':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.C);
                    break;

                case 'D':
                case 'd':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.D);
                    break;

                case 'E':
                case 'e':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.E);
                    break;

                case 'F':
                case 'f':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.F);
                    break;

                case 'G':
                case 'g':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.G);
                    break;

                case 'H':
                case 'h':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.H);
                    break;

                case 'I':
                case 'i':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.I);
                    break;

                case 'J':
                case 'j':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.J);
                    break;

                case 'K':
                case 'k':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.K);
                    break;

                case 'L':
                case 'l':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.L);
                    break;

                case 'M':
                case 'm':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.M);
                    break;

                case 'N':
                case 'n':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.N);
                    break;

                case 'O':
                case 'o':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.O);
                    break;

                case 'P':
                case 'p':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.P);
                    break;

                case 'Q':
                case 'q':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Q);
                    break;

                case 'R':
                case 'r':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.R);
                    break;

                case 'S':
                case 's':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.S);
                    break;

                case 'T':
                case 't':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.T);
                    break;

                case 'U':
                case 'u':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.U);
                    break;

                case 'V':
                case 'v':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.V);
                    break;

                case 'W':
                case 'w':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.W);
                    break;

                case 'X':
                case 'x':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.X);
                    break;

                case 'Y':
                case 'y':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Y);
                    break;

                case 'Z':
                case 'z':
                    AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Z);
                    break;
            }
        }

        /// <summary>
        /// Find the query default.
        /// </summary>
        /// <param name="queryItems">The query list.</param>
        /// <param name="query">The current query.</param>
        private void FindQueryExAllDefault(
            Dictionary<Enum.EnumDataStoreTableName, Nequeo.Model.QueryModel[]> queryItems,
            Nequeo.Model.QueryModel query)
        {
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.A);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.B);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.C);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.D);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.E);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.F);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.G);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.H);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.I);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.J);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.K);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.L);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.M);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.N);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.O);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.P);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Q);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.R);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.S);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.T);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.U);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.V);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.W);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.X);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Y);
            AddQuery(queryItems, query, Enum.EnumDataStoreTableName.Z);
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
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
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_context != null)
                        _context.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _context = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Provider()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}