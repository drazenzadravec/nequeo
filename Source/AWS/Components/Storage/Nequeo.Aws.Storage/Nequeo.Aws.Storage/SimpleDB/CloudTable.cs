/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

using Amazon.Runtime;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

namespace Nequeo.Aws.Storage.SimpleDB
{
    /// <summary>
    /// Represents the base object type for a table entity in the Table service.
    /// </summary>
    /// <typeparam name="T">The table entity type.</typeparam>
    public class CloudTable<T> where T : class, new()
    {
        /// <summary>
        /// Represents the base object type for a table entity in the Table service.
        /// </summary>
        /// <param name="cloudAccount">Cloud account provider.</param>
        /// <param name="domainName">The name of the domain used in this cloud table.</param>
        public CloudTable(CloudAccount cloudAccount, string domainName)
        {
            if (String.IsNullOrEmpty(domainName)) throw new ArgumentNullException(nameof(domainName));

            _account = cloudAccount;
            _domainName = domainName;
            _client = cloudAccount.AmazonSimpleDBClient;
        }

        private string _domainName = null;
        private CloudAccount _account = null;
        private AmazonSimpleDBClient _client = null;

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public T[] Select(System.Linq.Expressions.Expression<Func<T, bool>> where, int take = 100)
        {
            T[] data = null;

            // Create the Generic Queryable Provider Inspector and
            // Assign the execution function handler.
            Linq.GenericQueryableProviderInspector<T> query = new Linq.GenericQueryableProviderInspector<T>();
            query.ExecuteAction = (Nequeo.Model.ExpressionTreeModel model) => GetQueryData(model);

            // If take is positive.
            if (take > -1)
            {
                // Take the amount of data.
                data = query.QueryableProvider().Where(where).Take(take).ToArray();
            }
            else
            {
                // Get all the data.
                data = query.QueryableProvider().Where(where).ToArray();
            }

            // Return the data.
            return data;
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public Task<T[]> SelectAsync(System.Linq.Expressions.Expression<Func<T, bool>> where, CancellationToken token, int take = 100)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Select(where, take);

            }, token);
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="selector">The select clause predicate.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public T[] Select(
            System.Linq.Expressions.Expression<Func<T, bool>> where, 
            System.Linq.Expressions.Expression<Func<T, T>> selector, int take = 100)
        {
            T[] data = null;

            // Create the Generic Queryable Provider Inspector and
            // Assign the execution function handler.
            Linq.GenericQueryableProviderInspector<T> query = new Linq.GenericQueryableProviderInspector<T>();
            query.ExecuteAction = (Nequeo.Model.ExpressionTreeModel model) => GetQueryData(model);

            // If take is positive.
            if (take > -1)
            {
                // Take the amount of data.
                data = query.QueryableProvider().Where(where).Take(take).Select(selector).ToArray();
            }
            else
            {
                // Get all the data.
                data = query.QueryableProvider().Where(where).Select(selector).ToArray();
            }

            // Return the data.
            return data;
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="selector">The select clause predicate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public Task<T[]> SelectAsync(
            System.Linq.Expressions.Expression<Func<T, bool>> where,
            System.Linq.Expressions.Expression<Func<T, T>> selector, CancellationToken token, int take = 100)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Select(where, selector, take);

            }, token);
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <typeparam name="TKey">The order by key selector type.</typeparam>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="orderBy">The order by clause predicate.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public T[] Select<TKey>(
            System.Linq.Expressions.Expression<Func<T, bool>> where,
            System.Linq.Expressions.Expression<Func<T, TKey>> orderBy, int take = 100)
        {
            T[] data = null;

            // Create the Generic Queryable Provider Inspector and
            // Assign the execution function handler.
            Linq.GenericQueryableProviderInspector<T> query = new Linq.GenericQueryableProviderInspector<T>();
            query.ExecuteAction = (Nequeo.Model.ExpressionTreeModel model) => GetQueryData(model);

            // If take is positive.
            if (take > -1)
            {
                // Take the amount of data.
                data = query.QueryableProvider().Where(where).Take(take).OrderBy(orderBy).ToArray();
            }
            else
            {
                // Get all the data.
                data = query.QueryableProvider().Where(where).OrderBy(orderBy).ToArray();
            }

            // Return the data.
            return data;
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <typeparam name="TKey">The order by key selector type.</typeparam>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="orderBy">The order by clause predicate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public Task<T[]> SelectAsync<TKey>(
            System.Linq.Expressions.Expression<Func<T, bool>> where,
            System.Linq.Expressions.Expression<Func<T, TKey>> orderBy, CancellationToken token, int take = 100)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Select(where, orderBy, take);

            }, token);
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <typeparam name="TKey">The order by key selector type.</typeparam>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="selector">The select clause predicate.</param>
        /// <param name="orderBy">The order by clause predicate.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public T[] Select<TKey>(
            System.Linq.Expressions.Expression<Func<T, bool>> where,
            System.Linq.Expressions.Expression<Func<T, T>> selector,
            System.Linq.Expressions.Expression<Func<T, TKey>> orderBy, int take = 100)
        {
            T[] data = null;

            // Create the Generic Queryable Provider Inspector and
            // Assign the execution function handler.
            Linq.GenericQueryableProviderInspector<T> query = new Linq.GenericQueryableProviderInspector<T>();
            query.ExecuteAction = (Nequeo.Model.ExpressionTreeModel model) => GetQueryData(model);

            // If take is positive.
            if (take > -1)
            {
                // Take the amount of data.
                data = query.QueryableProvider().Where(where).Take(take).OrderBy(orderBy).Select(selector).ToArray();
            }
            else
            {
                // Get all the data.
                data = query.QueryableProvider().Where(where).OrderBy(orderBy).Select(selector).ToArray();
            }

            // Return the data.
            return data;
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <typeparam name="TKey">The order by key selector type.</typeparam>
        /// <param name="where">The where clause predicate.</param>
        /// <param name="selector">The select clause predicate.</param>
        /// <param name="orderBy">The order by clause predicate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>The array of entity types.</returns>
        public Task<T[]> SelectAsync<TKey>(
            System.Linq.Expressions.Expression<Func<T, bool>> where,
            System.Linq.Expressions.Expression<Func<T, T>> selector,
            System.Linq.Expressions.Expression<Func<T, TKey>> orderBy, CancellationToken token, int take = 100)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Select(where, selector, orderBy, take);

            }, token);
        }

        /// <summary>
        /// The Select operation returns a set of attributes for ItemNames that match the
        /// select expression. Select is similar to the standard SQL SELECT statement.
        /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB
        /// automatically adjusts the number of items returned per page to enforce this limit.
        /// For example, if the client asks to retrieve 2500 items, but each individual item
        /// is 10 kB in size, the system returns 100 items and an appropriate NextToken so
        /// the client can access the next page of results.
        /// For information on how to construct select expressions, see Using Select to Create
        /// Amazon SimpleDB Queries in the Developer Guide.
        /// </summary>
        /// <returns>The queryable provider.</returns>
        public Nequeo.Linq.QueryableProvider<T> Select()
        {
            // Create the Generic Queryable Provider Inspector and
            // Assign the execution function handler.
            Linq.GenericQueryableProviderInspector<T> query = new Linq.GenericQueryableProviderInspector<T>();
            query.ExecuteAction = (Nequeo.Model.ExpressionTreeModel model) => GetQueryData(model);

            // Return QueryableProvider.
            return query.QueryableProvider();
        }

        /// <summary>
        /// Execute the query.
        /// </summary>
        /// <param name="query">Provides functionality to evaluate queries against a specific data source wherein
        /// the type of the data is known.</param>
        /// <returns>The array of entity types.</returns>
        public T[] Execute(IQueryable<T> query)
        {
            return query.ToArray();
        }

        /// <summary>
        /// Execute the query async.
        /// </summary>
        /// <param name="query">Provides functionality to evaluate queries against a specific data source wherein
        /// the type of the data is known.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The async task.</returns>
        public Task<T[]> ExecuteAsync(IQueryable<T> query, CancellationToken token)
        {
            // Create a new task.
            return Task<T[]>.Factory.StartNew(() =>
            {
                // Wait until data has been sent.
                return Execute(query);

            }, token);
        }

        /// <summary>
        /// Get the query result data for the current expression.
        /// </summary>
        /// <param name="model">Expression tree model containing the expression inspector data.</param>
        /// <returns>The collection of message data.</returns>
        private T[] GetQueryData(Nequeo.Model.ExpressionTreeModel model)
        {
            List<T> items = new List<T>();
            bool foundEnought = false;
            string token = null;
            int take = Int32.MaxValue;

            // Create the query string.
            string query = GetQuery(model, out take);

            // If a query has been created.
            if (!String.IsNullOrEmpty(query))
            {
                do
                {
                    // Get the set of data.
                    SelectRequest request = new SelectRequest(query);
                    if (token != null)
                        request.NextToken = token;

                    // Get the response.
                    SelectResponse response = _client.Select(request);

                    // Get the items.
                    List<Item> segment = response.Items;
                    token = response.NextToken;
                    foreach (Item entity in segment)
                    {
                        // Add the item found.
                        items.Add(GetType(entity));

                        // If we have enought.
                        if (items.Count >= take)
                            break;
                    }

                    // If we have enought.
                    if (items.Count >= take)
                        foundEnought = true;

                }
                while (token != null && !foundEnought);
            }

            // Return the collection of data.
            return items.ToArray();
        }

        /// <summary>
        /// Create the query string.
        /// </summary>
        /// <param name="model">The expression tree model containing the query data.</param>
        /// <param name="take">The amount of data to take.</param>
        /// <returns>The query; else null.</returns>
        private string GetQuery(Nequeo.Model.ExpressionTreeModel model, out int take)
        {
            string query = "";
            int takeInternal = 0;
            string select = "*";

            take = Int32.MaxValue;

            // Get all the expression model data.
            Nequeo.Model.ExpressionModel expressionModel = model.GetExpression(true);

            // Get the select.
            if (!String.IsNullOrEmpty(model.Select))
            {
                // Get the select query.
                select = model.Select.Replace("[", "").Replace("]", "");
                query += " SELECT " + select;
            }
            else
            {
                // Get the default select.
                query += " SELECT " + select;
            }

            // From
            query += " FROM " + _domainName;

            // Get the where.
            if (!String.IsNullOrEmpty(model.Where))
            {
                // Set the where.
                query += " WHERE " + model.Where.
                    Replace("[", "").Replace("]", "").
                    Replace("\"", "'").Replace("==", "=").
                    Replace("&&", "AND").Replace("||", "OR");
            }

            // Get the orderby.
            if (!String.IsNullOrEmpty(model.Orderby))
            {
                // Set the order by.
                query += " ORDER BY " + model.Orderby.Replace("[", "").Replace("]", "");
            }
            else if (!String.IsNullOrEmpty(model.OrderbyDescending))
            {
                // Set the order by descending.
                query += " ORDER BY " + model.OrderbyDescending.Replace("[", "").Replace("]", "");
            }

            // Is there a take.
            bool isTakeSet = Int32.TryParse(model.Take, out takeInternal);
            if (isTakeSet)
            {
                // If take has been set.
                if (takeInternal > 0)
                    take = takeInternal;

                query += " LIMIT " + take.ToString();
            }

            // Return the query.
            return query.Trim();
        }

        /// <summary>
        /// Get the type for the attributes.
        /// </summary>
        /// <param name="entity">The AWS entity attributes.</param>
        /// <returns>The new type.</returns>
        private T GetType(Item entity)
        {
            // Create a new instance of the
            // object type.
            T data = new T();

            // Get the list of all properties wthin the
            // current business data type.
            List<PropertyInfo> properties = GetPublicProperties(typeof(T));

            // Get the attributes.
            List<Amazon.SimpleDB.Model.Attribute> attributes = entity.Attributes;
            foreach (Amazon.SimpleDB.Model.Attribute item in attributes)
            {
                // Get the name and value.
                string attName = item.Name;
                string attValue = item.Value;

                try
                {
                    // Find in the property collection the current property that matches
                    // the current column. Use the Predicate delegate object to
                    // initiate a search for the specified match.
                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = properties.First(p => p.Name.ToLower() == attName.ToLower());
                    }
                    catch { }
                    if (propertyInfo != null)
                    {
                        // Set the object type property
                        // with the property found in the
                        // current object collection.
                        propertyInfo.SetValue(data, Convert.ChangeType(attValue, propertyInfo.PropertyType), null);
                    }
                }
                catch { }
            }

            // Return the type.
            return data;
        }

        /// <summary>
        /// Get all public properties within the current type.
        /// </summary>
        /// <param name="t">The current type to retreive properties within.</param>
        /// <returns>The collection of all proerties within the type.</returns>
        private List<PropertyInfo> GetPublicProperties(Type t)
        {
            // Create a new instance of the property collection.
            List<PropertyInfo> properties = new List<PropertyInfo>();

            // Get the base type and the derived type.
            Type type = t;

            // Add the complete property range.
            properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            // Return all the properties within
            // the type.
            return properties;
        }
    }
}
