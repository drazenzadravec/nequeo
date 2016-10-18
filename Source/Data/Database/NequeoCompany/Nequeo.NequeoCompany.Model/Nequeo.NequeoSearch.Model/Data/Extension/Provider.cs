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
using System.Threading;
using System.Threading.Tasks;

using Nequeo.Extension;
using Nequeo.DataAccess.NequeoSearch.Data;
using Nequeo.DataAccess.SearchProvider;

namespace Nequeo.DataAccess.NequeoSearch
{
    /// <summary>
    /// Nequeo database search provider.
    /// </summary>
    public partial class Provider : IDisposable
    {
        /// <summary>
        /// Nequeo database search provider.
        /// </summary>
        public Provider()
        {
            _context = new DataContext();
            _searchProvider = new SearchProvider.Provider();
        }

        private DataContext _context = null;
        private SearchProvider.Provider _searchProvider = null;

        private int _take = 30;
        private int _indexes = 0;
        private long _count = 0;
        private long[] _resultDataStoreIDs = null;
        private Dictionary<SearchProvider.Enum.EnumDataStoreTableName, long[]> _results = null;

        /// <summary>
        /// Gets the total number of indexes for the search.
        /// </summary>
        public int IndexCount
        {
            get { return _indexes; }
        }

        /// <summary>
        /// Delete the table rows.
        /// </summary>
        /// <typeparam name="T">The table row identity type.</typeparam>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowIDs">The list of table row identities.</param>
        public async void Delete<T>(Enum.EnumTableName tableName, T[] tableRowIDs)
        {
            if (tableRowIDs != null && tableRowIDs.Length > 0)
            {
                // Convert the table row ids to string types.
                string[] tableRows = tableRowIDs.ToStringArray();

                await Nequeo.Threading.AsyncOperationResult<bool>.
                    RunTask(() =>
                    {
                        // Delete the table rows.
                        DeleteTableRows(tableName, tableRows);
                    });
            }
        }

        /// <summary>
        /// Update the table row.
        /// </summary>
        /// <typeparam name="T">The table row identity type.</typeparam>
        /// <param name="queries">The list of queries to update.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowID">The table row id to update.</param>
        public async void Update<T>(Nequeo.Model.QueryModel[] queries, Enum.EnumTableName tableName, T tableRowID)
        {
            if (queries != null && queries.Length > 0)
            {
                // Convert the table row id to string types.
                string tableRow = tableRowID.ToString();

                await Nequeo.Threading.AsyncOperationResult<bool>.
                    RunTask(() =>
                    {
                        // Delete the table rows.
                        DeleteTableRows(tableName, new string[] { tableRow });

                        // Insert the table rows.
                        InsertTableRows(queries, tableName, tableRow);
                    });
            }
        }

        /// <summary>
        /// Insert the table row.
        /// </summary>
        /// <typeparam name="T">The table row identity type.</typeparam>
        /// <param name="queries">The list of queries to insert.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowID">The table row id to insert.</param>
        public async void Insert<T>(Nequeo.Model.QueryModel[] queries, Enum.EnumTableName tableName, T tableRowID)
        {
            if (queries != null && queries.Length > 0)
            {
                // Convert the table row id to string types.
                string tableRow = tableRowID.ToString();

                await Nequeo.Threading.AsyncOperationResult<bool>.
                    RunTask(() =>
                    {
                        // Insert the table rows.
                        InsertTableRows(queries, tableName, tableRow);
                    });
            }
        }

        /// <summary>
        /// Get unique identifiers for the queries that correspond to the table name.
        /// </summary>
        /// <param name="queries">The queries to search for.</param>
        /// <param name="take">The number of items to take (-1 : take all).</param>
        /// <param name="skip">The number of items to skip before taking.</param>
        /// <returns>The total data store identifiers for this search.</returns>
        public virtual long GetSearchIdentifiers(Nequeo.Model.QueryModel[] queries, int take = -1, int skip = 0)
        {
            _count = 0;

            long[] temp = null;
            long[] dataStoreIDs = new long[0];
            _resultDataStoreIDs = null;

            // Get the initial query results.
            _results = _searchProvider.GetSearchIdentifiers(queries, take, skip);

            // For each query.
            foreach (KeyValuePair<SearchProvider.Enum.EnumDataStoreTableName, long[]> item in _results)
            {
                // Count the items.
                _count += (long)item.Value.Length;
                temp = dataStoreIDs.CombineParallel(item.Value);
                dataStoreIDs = temp;
            }

            // Assign the data store ids.
            _resultDataStoreIDs = dataStoreIDs;

            // Make ids exist.
            if (_count > 0)
            {
                // Get the total number of id indexes.
                if ((_count % _take) > 0)
                    _indexes = ((int)(_count / _take)) + 1;
                else
                    _indexes = ((int)(_count / _take));
            }

            // Return the count.
            return _count;
        }

        /// <summary>
        /// Get the table row identities
        /// </summary>
        /// <typeparam name="T">The table row identity type.</typeparam>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="index">The zero based index of the search provider (IndexCount property is directly related).</param>
        /// <param name="take">The number of row groups to take for the current index.</param>
        /// <param name="skip">The number of row groups to skip for the current index.</param>
        /// <returns>The collection of table row identities.</returns>
        public virtual T[] GetTableRowIdentities<T>(Enum.EnumTableName tableName, int index, int take = -1, int skip = 0)
        {
            // If items exist.
            if (_results != null && _results.Count > 0)
            {
                int i = index;

                // If an incorrect index has been specified.
                if (index < 0)
                    i = 0;
                else if (index >= _indexes)
                    i = _indexes - 1;

                // Get the data store ids.
                long[] dsIDS = GetDataStoreIDs(_resultDataStoreIDs, _take, (i * _take));

                // Get the table row ids.
                T[] tableRowIDs = GetSearchIdentifiersTableName<T>(tableName, dsIDS, take, skip);

                // Return the table row ids.
                return tableRowIDs;
            }
            else
                // Return an empty array.
                return new T[0];
        }

        /// <summary>
        /// Get the table row identities.
        /// </summary>
        /// <typeparam name="T">The data type to create the expression for.</typeparam>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="dsIDS">The list of data store ids for the lambda.</param>
        /// <param name="take">The number to take.</param>
        /// <param name="skip">The number to skip.</param>
        /// <returns>The table row ids.</returns>
        private T[] GetSearchIdentifiersTableName<T>(Enum.EnumTableName tableName, long[] dsIDS, int take, int skip)
        {
            // Get the first set of data.
            List<T> tableRows = new List<T>();

            // Make sure ids exist.
            if (dsIDS.Length > 0)
            {
                // For each data store table.
                foreach (KeyValuePair<SearchProvider.Enum.EnumDataStoreTableName, long[]> item in _results)
                {
                    // Find the current store table name.
                    switch (item.Key)
                    {
                        case SearchProvider.Enum.EnumDataStoreTableName.A:
                            foreach (Data.DataStoreA ret in GetQueryable<Data.DataStoreA>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.B:
                            foreach (Data.DataStoreB ret in GetQueryable<Data.DataStoreB>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.C:
                            foreach (Data.DataStoreC ret in GetQueryable<Data.DataStoreC>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.D:
                            foreach (Data.DataStoreD ret in GetQueryable<Data.DataStoreD>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.E:
                            foreach (Data.DataStoreE ret in GetQueryable<Data.DataStoreE>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.F:
                            foreach (Data.DataStoreF ret in GetQueryable<Data.DataStoreF>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.G:
                            foreach (Data.DataStoreG ret in GetQueryable<Data.DataStoreG>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.H:
                            foreach (Data.DataStoreH ret in GetQueryable<Data.DataStoreH>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.I:
                            foreach (Data.DataStoreI ret in GetQueryable<Data.DataStoreI>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.J:
                            foreach (Data.DataStoreJ ret in GetQueryable<Data.DataStoreJ>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.K:
                            foreach (Data.DataStoreK ret in GetQueryable<Data.DataStoreK>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.L:
                            foreach (Data.DataStoreL ret in GetQueryable<Data.DataStoreL>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.M:
                            foreach (Data.DataStoreM ret in GetQueryable<Data.DataStoreM>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.N:
                            foreach (Data.DataStoreN ret in GetQueryable<Data.DataStoreN>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.O:
                            foreach (Data.DataStoreO ret in GetQueryable<Data.DataStoreO>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.P:
                            foreach (Data.DataStoreP ret in GetQueryable<Data.DataStoreP>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.Q:
                            foreach (Data.DataStoreQ ret in GetQueryable<Data.DataStoreQ>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.R:
                            foreach (Data.DataStoreR ret in GetQueryable<Data.DataStoreR>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.S:
                            foreach (Data.DataStoreS ret in GetQueryable<Data.DataStoreS>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.T:
                            foreach (Data.DataStoreT ret in GetQueryable<Data.DataStoreT>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.U:
                            foreach (Data.DataStoreU ret in GetQueryable<Data.DataStoreU>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.V:
                            foreach (Data.DataStoreV ret in GetQueryable<Data.DataStoreV>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.W:
                            foreach (Data.DataStoreW ret in GetQueryable<Data.DataStoreW>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.X:
                            foreach (Data.DataStoreX ret in GetQueryable<Data.DataStoreX>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.Y:
                            foreach (Data.DataStoreY ret in GetQueryable<Data.DataStoreY>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.Z:
                            foreach (Data.DataStoreZ ret in GetQueryable<Data.DataStoreZ>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.Number:
                            foreach (Data.DataStoreNumber ret in GetQueryable<Data.DataStoreNumber>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.Phrase:
                            foreach (Data.DataStorePhrase ret in GetQueryable<Data.DataStorePhrase>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        case SearchProvider.Enum.EnumDataStoreTableName.Special:
                            foreach (Data.DataStoreSpecial ret in GetQueryable<Data.DataStoreSpecial>(dsIDS, tableName, take, skip))
                                tableRows.Add((T)Convert.ChangeType(ret.TableRowID, typeof(T)));

                            break;

                        default:
                            break;
                    }
                }
            }

            // Return the result.
            return tableRows.ToArray();
        }

        /// <summary>
        /// Delete the table rows.
        /// </summary>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowIDs">The list of table rows ids to delete.</param>
        private void DeleteTableRows(Enum.EnumTableName tableName, string[] tableRowIDs)
        {
            // Invoke each operation in parallel.
            Parallel.Invoke(
                () => { DeleteTableRowsEx<Data.DataStoreA>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreB>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreC>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreD>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreE>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreF>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreG>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreH>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreI>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreJ>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreK>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreL>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreM>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreN>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreO>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreP>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreQ>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreR>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreS>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreT>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreU>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreV>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreW>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreX>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreY>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreZ>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreNumber>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStorePhrase>(tableName, tableRowIDs); },
                () => { DeleteTableRowsEx<Data.DataStoreSpecial>(tableName, tableRowIDs); }
            );
        }

        /// <summary>
        /// Insert the table rows.
        /// </summary>
        /// <param name="queries">The list of queries to insert.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowID">The table row id to insert.</param>
        private void InsertTableRows(Nequeo.Model.QueryModel[] queries, Enum.EnumTableName tableName, string tableRowID)
        {
            // Invoke each operation in parallel.
            Parallel.Invoke(
                () => { InsertTableRowsEx<Data.DataStoreA>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.A); },
                () => { InsertTableRowsEx<Data.DataStoreB>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.B); },
                () => { InsertTableRowsEx<Data.DataStoreC>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.C); },
                () => { InsertTableRowsEx<Data.DataStoreD>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.D); },
                () => { InsertTableRowsEx<Data.DataStoreE>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.E); },
                () => { InsertTableRowsEx<Data.DataStoreF>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.F); },
                () => { InsertTableRowsEx<Data.DataStoreG>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.G); },
                () => { InsertTableRowsEx<Data.DataStoreH>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.H); },
                () => { InsertTableRowsEx<Data.DataStoreI>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.I); },
                () => { InsertTableRowsEx<Data.DataStoreJ>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.J); },
                () => { InsertTableRowsEx<Data.DataStoreK>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.K); },
                () => { InsertTableRowsEx<Data.DataStoreL>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.L); },
                () => { InsertTableRowsEx<Data.DataStoreM>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.M); },
                () => { InsertTableRowsEx<Data.DataStoreN>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.N); },
                () => { InsertTableRowsEx<Data.DataStoreO>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.O); },
                () => { InsertTableRowsEx<Data.DataStoreP>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.P); },
                () => { InsertTableRowsEx<Data.DataStoreQ>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.Q); },
                () => { InsertTableRowsEx<Data.DataStoreR>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.R); },
                () => { InsertTableRowsEx<Data.DataStoreS>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.S); },
                () => { InsertTableRowsEx<Data.DataStoreT>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.T); },
                () => { InsertTableRowsEx<Data.DataStoreU>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.U); },
                () => { InsertTableRowsEx<Data.DataStoreV>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.V); },
                () => { InsertTableRowsEx<Data.DataStoreW>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.W); },
                () => { InsertTableRowsEx<Data.DataStoreX>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.X); },
                () => { InsertTableRowsEx<Data.DataStoreY>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.Y); },
                () => { InsertTableRowsEx<Data.DataStoreZ>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.Z); },
                () => { InsertTableRowsEx<Data.DataStoreNumber>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.Number); },
                () => { InsertTableRowsEx<Data.DataStorePhrase>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.Phrase); },
                () => { InsertTableRowsEx<Data.DataStoreSpecial>(queries, tableName, tableRowID, SearchProvider.Enum.EnumDataStoreTableName.Special); }
            );
        }

        /// <summary>
        /// Insert the table rows.
        /// </summary>
        /// <typeparam name="T">The data type to create the expression for.</typeparam>
        /// <param name="queries">The list of queries to insert.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowID">The table row id to insert.</param>
        /// <param name="storeTableName">The store table name.</param>
        private void InsertTableRowsEx<T>(Nequeo.Model.QueryModel[] queries, Enum.EnumTableName tableName, string tableRowID, SearchProvider.Enum.EnumDataStoreTableName storeTableName)
            where T : class, new()
        {
            // For each query.
            Parallel.ForEach<Nequeo.Model.QueryModel>(queries,
                i =>
                {
                    // Insert the current query.
                    InsertTableRowsExEx<T>(i, tableName, tableRowID, storeTableName);
                });
        }

        /// <summary>
        /// Delete the table rows.
        /// </summary>
        /// <typeparam name="T">The data type to create the expression for.</typeparam>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowIDs">The list of table rows ids to delete.</param>
        private void DeleteTableRowsEx<T>(Enum.EnumTableName tableName, string[] tableRowIDs)
            where T : class, new()
        {
            try
            {
                // Create a new provider connection.
                // After use dispose of all resources.
                using (DataContext provider = new DataContext())
                    // Delete the current table rows with the current table.
                    provider.Delete<T>().DeleteItemPredicate(ModifyLambdaExpression<T>(tableRowIDs, tableName));
            }
            catch { }
        }

        /// <summary>
        /// Insert the table rows.
        /// </summary>
        /// <typeparam name="T">The data type to create the expression for.</typeparam>
        /// <param name="query">The query to insert.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowID">The table row id to insert.</param>
        /// <param name="storeTableName">The store table name.</param>
        private void InsertTableRowsExEx<T>(Nequeo.Model.QueryModel query, Enum.EnumTableName tableName, string tableRowID, SearchProvider.Enum.EnumDataStoreTableName storeTableName)
            where T : class, new()
        {
            try
            {
                // If is number.
                if ((query.QueryType == Model.QueryType.Number) && (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.Number))
                    InsertTableRowsExExEx<T>(query, tableName, tableRowID, storeTableName);

                // If is phrase.
                if ((query.QueryType == Model.QueryType.Phrase) && (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.Phrase))
                    InsertTableRowsExExEx<T>(query, tableName, tableRowID, storeTableName);

                // If is special.
                if ((query.QueryType == Model.QueryType.Special) && (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.Special))
                    InsertTableRowsExExEx<T>(query, tableName, tableRowID, storeTableName);

                // If is default letter.
                if ((query.QueryType == Model.QueryType.Default))
                {
                    // Is search query match.
                    if (IsSearchQueryMatch(query, storeTableName))
                        InsertTableRowsExExEx<T>(query, tableName, tableRowID, storeTableName);
                }
            }
            catch { }
        }

        /// <summary>
        /// Insert the table rows.
        /// </summary>
        /// <typeparam name="T">The data type to create the expression for.</typeparam>
        /// <param name="query">The query to insert.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="tableRowID">The table row id to insert.</param>
        /// <param name="storeTableName">The store table name.</param>
        private void InsertTableRowsExExEx<T>(Nequeo.Model.QueryModel query, Enum.EnumTableName tableName, string tableRowID, SearchProvider.Enum.EnumDataStoreTableName storeTableName)
            where T : class, new()
        {
            // Create a new provider connection.
            // After use dispose of all resources.
            using (DataContext provider = new DataContext())
            using (SearchProvider.Provider searchProvider = new SearchProvider.Provider())
            {
                long dataStoreID = 0;

                // Find the current store table name.
                switch (storeTableName)
                {
                    case SearchProvider.Enum.EnumDataStoreTableName.A:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreA>(SearchProvider.Data.DataStoreA.CreateDataStoreA(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.B:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreB>(SearchProvider.Data.DataStoreB.CreateDataStoreB(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.C:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreC>(SearchProvider.Data.DataStoreC.CreateDataStoreC(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.D:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreD>(SearchProvider.Data.DataStoreD.CreateDataStoreD(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.E:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreE>(SearchProvider.Data.DataStoreE.CreateDataStoreE(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.F:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreF>(SearchProvider.Data.DataStoreF.CreateDataStoreF(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.G:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreG>(SearchProvider.Data.DataStoreG.CreateDataStoreG(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.H:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreH>(SearchProvider.Data.DataStoreH.CreateDataStoreH(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.I:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreI>(SearchProvider.Data.DataStoreI.CreateDataStoreI(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.J:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreJ>(SearchProvider.Data.DataStoreJ.CreateDataStoreJ(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.K:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreK>(SearchProvider.Data.DataStoreK.CreateDataStoreK(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.L:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreL>(SearchProvider.Data.DataStoreL.CreateDataStoreL(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.M:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreM>(SearchProvider.Data.DataStoreM.CreateDataStoreM(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.N:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreN>(SearchProvider.Data.DataStoreN.CreateDataStoreN(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.O:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreO>(SearchProvider.Data.DataStoreO.CreateDataStoreO(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.P:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreP>(SearchProvider.Data.DataStoreP.CreateDataStoreP(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.Q:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreQ>(SearchProvider.Data.DataStoreQ.CreateDataStoreQ(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.R:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreR>(SearchProvider.Data.DataStoreR.CreateDataStoreR(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.S:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreS>(SearchProvider.Data.DataStoreS.CreateDataStoreS(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.T:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreT>(SearchProvider.Data.DataStoreT.CreateDataStoreT(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.U:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreU>(SearchProvider.Data.DataStoreU.CreateDataStoreU(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.V:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreV>(SearchProvider.Data.DataStoreV.CreateDataStoreV(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.W:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreW>(SearchProvider.Data.DataStoreW.CreateDataStoreW(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.X:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreX>(SearchProvider.Data.DataStoreX.CreateDataStoreX(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.Y:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreY>(SearchProvider.Data.DataStoreY.CreateDataStoreY(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.Z:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreZ>(SearchProvider.Data.DataStoreZ.CreateDataStoreZ(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.Number:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreNumber>(SearchProvider.Data.DataStoreNumber.CreateDataStoreNumber(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.Phrase:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStorePhrase>(SearchProvider.Data.DataStorePhrase.CreateDataStorePhrase(0, query.Query));
                        break;

                    case SearchProvider.Enum.EnumDataStoreTableName.Special:
                        // Insert the item into the search provider database.
                        dataStoreID = searchProvider.Insert<SearchProvider.Data.DataStoreSpecial>(SearchProvider.Data.DataStoreSpecial.CreateDataStoreSpecial(0, query.Query));
                        break;

                    default:
                        break;
                }

                // Create the T type instance.
                T row = new T();

                // Get the T type properties.
                PropertyInfo dataStoreIDProp = row.GetType().GetProperty("DataStoreID");
                PropertyInfo tableIDProp = row.GetType().GetProperty("TableID");
                PropertyInfo tableRowIDProp = row.GetType().GetProperty("TableRowID");

                // Assign the T type properties.
                dataStoreIDProp.SetValue(row, dataStoreID);
                tableIDProp.SetValue(row, (long)tableName);
                tableRowIDProp.SetValue(row, tableRowID);

                // Get the current entity.
                T[] result = provider.GetTable<T>().Where(CreateLambdaExpression<T>(row)).ToArray();

                // If records do not exist
                if (result == null || result.Length < 1)
                {
                    // Insert the data.
                    provider.Insert<T>().InsertDataEntity(row);
                }
            }
        }

        /// <summary>
        /// Get the data store ids.
        /// </summary>
        /// <param name="dsIDS">The collection of data ids.</param>
        /// <param name="take">The number to take.</param>
        /// <param name="skip">The number to skip.</param>
        /// <returns>The data store ids.</returns>
        private long[] GetDataStoreIDs(long[] dsIDS, int take, int skip)
        {
            if (take > -1 && skip > 0)
                return dsIDS.Take(take).Skip(skip).ToArray();
            else if (skip > 0)
                return dsIDS.Skip(skip).ToArray();
            else if (take > -1)
                return dsIDS.Take(take).ToArray();
            else
                return dsIDS;
        }

        /// <summary>
        /// Create the queryable interface.
        /// </summary>
        /// <typeparam name="T">The data type to create the expression for.</typeparam>
        /// <param name="dsIDS">The list of data store ids for the lambda.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <param name="take">The number to take.</param>
        /// <param name="skip">The number to skip.</param>
        /// <returns>The queryable type.</returns>
        private IQueryable<T> GetQueryable<T>(long[] dsIDS, Enum.EnumTableName tableName, int take, int skip)
            where T : class, new()
        {
            if (take > -1 && skip > 0)
                return _context.GetTable<T>().Where(SelectLambdaExpression<T>(dsIDS, tableName)).Take(take).Skip(skip);
            else if (skip > 0)
                return _context.GetTable<T>().Where(SelectLambdaExpression<T>(dsIDS, tableName)).Skip(skip);
            else if (take > -1)
                return _context.GetTable<T>().Where(SelectLambdaExpression<T>(dsIDS, tableName)).Take(take);
            else
                return _context.GetTable<T>().Where(SelectLambdaExpression<T>(dsIDS, tableName));
        }

        /// <summary>
        /// Create the select lambda expression.
        /// </summary>
        /// <typeparam name="TDataEntity">The data type to create the expression for.</typeparam>
        /// <param name="dsIDS">The list of data store ids for the lambda.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <returns>The lambda expression.</returns>
        private Expression<Func<TDataEntity, bool>> SelectLambdaExpression<TDataEntity>(long[] dsIDS, Enum.EnumTableName tableName)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the DataStoreID property initial expression.
            PropertyInfo propertyDataStoreID = typeof(TDataEntity).GetProperty("DataStoreID");
            Expression propertyMemberDataStoreID = Expression.MakeMemberAccess(paramExpr, propertyDataStoreID);

            // Create the TableID property initial expression.
            PropertyInfo propertyTableID = typeof(TDataEntity).GetProperty("TableID");
            Expression propertyMemberTableID = Expression.MakeMemberAccess(paramExpr, propertyTableID);

            // The resulting expression.
            Expression result = null;
            Expression[] created = new Expression[dsIDS.Length];
            Expression swap = null;

            // If more than one expression.
            if (dsIDS.Length > 1)
            {
                // For each query create the expression.
                for (int i = 0; i < dsIDS.Length; i++)
                {
                    // Create the cuurent expression.
                    created[i] = Expression.Equal(propertyMemberDataStoreID, Expression.Constant(dsIDS[i], typeof(long)));
                }

                // Get the first two exressions.
                swap = Expression.OrElse(created[0], created[1]);

                // For each query created.
                for (int i = 2; i < created.Length; i++)
                {
                    // Get the new swap.
                    swap = Expression.OrElse(swap, created[i]);
                }

                // Assign the final expression result.
                Expression right = Expression.Equal(propertyMemberTableID,
                    Expression.Convert(Expression.Constant(tableName, typeof(Enum.EnumTableName)), typeof(long)));
                result = Expression.AndAlso(swap, right);
            }
            else
            {
                // Create the single value expression.
                Expression left = Expression.Equal(propertyMemberDataStoreID, Expression.Constant(dsIDS[0], typeof(long)));
                Expression right = Expression.Equal(propertyMemberTableID,
                    Expression.Convert(Expression.Constant(tableName, typeof(Enum.EnumTableName)), typeof(long)));
                result = Expression.AndAlso(left, right);
            }

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, bool>> predicate =
                Expression<Func<TDataEntity, bool>>.Lambda<Func<TDataEntity, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Create the modify lambda expression.
        /// </summary>
        /// <typeparam name="TDataEntity">The data type to create the expression for.</typeparam>
        /// <param name="tableRowIDs">The list of table row ids for the lambda.</param>
        /// <param name="tableName">The current table name the expression will contain.</param>
        /// <returns>The lambda expression.</returns>
        private Expression<Threading.FunctionHandler<bool, TDataEntity>> ModifyLambdaExpression<TDataEntity>(string[] tableRowIDs, Enum.EnumTableName tableName)
        {
            // A parameter for the lambda expression.
            ParameterExpression paramExpr = Expression.Parameter(typeof(TDataEntity), "u");

            // Create the DataStoreID property initial expression.
            PropertyInfo propertyTableRowID = typeof(TDataEntity).GetProperty("TableRowID");
            Expression propertyMemberTableRowID = Expression.MakeMemberAccess(paramExpr, propertyTableRowID);

            // Create the TableID property initial expression.
            PropertyInfo propertyTableID = typeof(TDataEntity).GetProperty("TableID");
            Expression propertyMemberTableID = Expression.MakeMemberAccess(paramExpr, propertyTableID);

            // The resulting expression.
            Expression result = null;
            Expression[] created = new Expression[tableRowIDs.Length];
            Expression swap = null;

            // If more than one expression.
            if (tableRowIDs.Length > 1)
            {
                // For each query create the expression.
                for (int i = 0; i < tableRowIDs.Length; i++)
                {
                    // Create the cuurent expression.
                    created[i] = Expression.Equal(propertyMemberTableRowID, Expression.Constant(tableRowIDs[i], typeof(string)));
                }

                // Get the first two exressions.
                swap = Expression.OrElse(created[0], created[1]);

                // For each query created.
                for (int i = 2; i < created.Length; i++)
                {
                    // Get the new swap.
                    swap = Expression.OrElse(swap, created[i]);
                }

                // Assign the final expression result.
                Expression right = Expression.Equal(propertyMemberTableID,
                    Expression.Convert(Expression.Constant(tableName, typeof(Enum.EnumTableName)), typeof(long)));
                result = Expression.AndAlso(swap, right);
            }
            else
            {
                // Create the single value expression.
                Expression left = Expression.Equal(propertyMemberTableRowID, Expression.Constant(tableRowIDs[0], typeof(string)));
                Expression right = Expression.Equal(propertyMemberTableID,
                    Expression.Convert(Expression.Constant(tableName, typeof(Enum.EnumTableName)), typeof(long)));
                result = Expression.AndAlso(left, right);
            }

            // This expression represents a lambda expression 
            Expression<Threading.FunctionHandler<bool, TDataEntity>> predicate =
                Expression<Threading.FunctionHandler<bool, TDataEntity>>.Lambda<Threading.FunctionHandler<bool, TDataEntity>>(result, new List<ParameterExpression>() { paramExpr });

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
            PropertyInfo propertyDataStoreID = typeof(TDataEntity).GetProperty("DataStoreID");
            Expression propertyMemberDataStoreID = Expression.MakeMemberAccess(paramExpr, propertyDataStoreID);

            PropertyInfo propertyTableID = typeof(TDataEntity).GetProperty("TableID");
            Expression propertyMemberTableID = Expression.MakeMemberAccess(paramExpr, propertyTableID);

            PropertyInfo propertyTableRowID = typeof(TDataEntity).GetProperty("TableRowID");
            Expression propertyMemberTableRowID = Expression.MakeMemberAccess(paramExpr, propertyTableRowID);

            // The resulting expression.
            Expression resultDataStoreID = Expression.Equal(propertyMemberDataStoreID, Expression.Constant(propertyDataStoreID.GetValue(entity), typeof(long)));
            Expression resultTableID = Expression.Equal(propertyMemberTableID, Expression.Constant(propertyTableID.GetValue(entity), typeof(long)));
            Expression resultTableRowID = Expression.Equal(propertyMemberTableRowID, Expression.Constant(propertyTableRowID.GetValue(entity), typeof(string)));

            Expression left = Expression.AndAlso(resultDataStoreID, resultTableID);
            Expression result = Expression.AndAlso(left, resultTableRowID);

            // This expression represents a lambda expression 
            Expression<Func<TDataEntity, bool>> predicate =
                Expression<Func<TDataEntity, bool>>.Lambda<Func<TDataEntity, bool>>(result, new List<ParameterExpression>() { paramExpr });

            // Return the expression.
            return predicate;
        }

        /// <summary>
        /// Is the search query a match.
        /// </summary>
        /// <param name="query">The query to insert.</param>
        /// <param name="storeTableName">The store table name.</param>
        /// <returns>True if match; else fasle.</returns>
        private bool IsSearchQueryMatch(Nequeo.Model.QueryModel query, SearchProvider.Enum.EnumDataStoreTableName storeTableName)
        {
            bool result = false;

            // Find all letters and default all special chars.
            switch (query.Query[0])
            {
                case 'A':
                case 'a':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.A ? true : false);
                    break;

                case 'B':
                case 'b':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.B ? true : false);
                    break;

                case 'C':
                case 'c':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.C ? true : false);
                    break;

                case 'D':
                case 'd':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.D ? true : false);
                    break;

                case 'E':
                case 'e':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.E ? true : false);
                    break;

                case 'F':
                case 'f':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.F ? true : false);
                    break;

                case 'G':
                case 'g':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.G ? true : false);
                    break;

                case 'H':
                case 'h':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.H ? true : false);
                    break;

                case 'I':
                case 'i':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.I ? true : false);
                    break;

                case 'J':
                case 'j':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.J ? true : false);
                    break;

                case 'K':
                case 'k':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.K ? true : false);
                    break;

                case 'L':
                case 'l':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.L ? true : false);
                    break;

                case 'M':
                case 'm':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.M ? true : false);
                    break;

                case 'N':
                case 'n':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.N ? true : false);
                    break;

                case 'O':
                case 'o':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.O ? true : false);
                    break;

                case 'P':
                case 'p':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.P ? true : false);
                    break;

                case 'Q':
                case 'q':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.Q ? true : false);
                    break;

                case 'R':
                case 'r':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.R ? true : false);
                    break;

                case 'S':
                case 's':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.S ? true : false);
                    break;

                case 'T':
                case 't':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.T ? true : false);
                    break;

                case 'U':
                case 'u':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.U ? true : false);
                    break;

                case 'V':
                case 'v':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.V ? true : false);
                    break;

                case 'W':
                case 'w':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.W ? true : false);
                    break;

                case 'X':
                case 'x':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.X ? true : false);
                    break;

                case 'Y':
                case 'y':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.Y ? true : false);
                    break;

                case 'Z':
                case 'z':
                    result = (storeTableName == SearchProvider.Enum.EnumDataStoreTableName.Z ? true : false);
                    break;
            }
            return result;
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

                    if (_searchProvider != null)
                        _searchProvider.Dispose();

                    if (_results != null)
                        _results.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _context = null;
                _searchProvider = null;
                _results = null;
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
