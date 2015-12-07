/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          State.cs
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;

namespace Nequeo.DataAccess.ApplicationLogin.Data.Extension
{
    /// <summary>
    /// The state data member extension.
    /// </summary>
    partial class State
    {
        /// <summary>
        /// Gets the state list.
        /// </summary>
        /// <returns>The state list.</returns>
        public virtual Data.Extended.StateList[] GetShortNameList()
        {
            return
                Select.
                SelectIQueryableItems(s => s.StateVisible == true).
                OrderBy(s => s.GroupOrder).
                Select(s => new
                {
                    StateID = s.StateID,
                    StateName = s.StateShortName,
                    StateCodeID = s.StateCodeID,
                    CountryID = s.CountryID
                }).ToTypeArray<Data.Extended.StateList>();
        }

        /// <summary>
        /// Gets the state list.
        /// </summary>
        /// <returns>The state list.</returns>
        public virtual Data.Extended.StateList[] GetLongNameList()
        {
            return
                Select.
                SelectIQueryableItems(s => s.StateVisible == true).
                OrderBy(s => s.GroupOrder).
                Select(s => new
                {
                    StateID = s.StateID,
                    StateName = s.StateLongName,
                    StateCodeID = s.StateCodeID,
                    CountryID = s.CountryID
                }).ToTypeArray<Data.Extended.StateList>();
        }

        /// <summary>
        /// Gets the auto complete state data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of state.</returns>
        public virtual string[] GetStateAutoComplete(string prefixText, Int32 numberToReturn)
        {
            int i = 0;

            var query = Select.
                        SelectIQueryableItems(s => (SqlQueryMethods.Like(s.StateLongName, (prefixText + "%")) ||
                             SqlQueryMethods.Like(s.StateShortName, (prefixText + "%")))).
                        Select(s => new
                        {
                            StateName = (string)s.StateLongName
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.StateName;

            return ret;
        }

        /// <summary>
        /// Gets the state list.
        /// </summary>
        /// <param name="countryID">The country id to search on.</param>
        /// <returns>The state list.</returns>
        public virtual Data.Extended.StateList[] GetShortNameList(long countryID)
        {
            // Create the new cache control container.
            Nequeo.Data.Control.ICacheControl selectCache = Select;

            selectCache.CacheItems = true;
            selectCache.CachedItemName = "StateListShort" + countryID.ToString();
            selectCache.CacheTimeout = 200;

            Data.Extended.StateList[] list = null;

            // Is the item to be cached.
            if (selectCache.CacheItems)
            {
                // Get the item from the cache.
                object item = selectCache.GetItemFromCache(selectCache.CachedItemName);
                if (item != null)
                    if (item is Data.Extended.StateList[])
                        list = item as Data.Extended.StateList[];
            }

            // If the item has not been cached
            // then get the items from the database.
            if (list == null)
                list = Select.
                    SelectIQueryableItems(s => (s.StateVisible == true) && (s.CountryID == countryID)).
                    OrderBy(s => s.GroupOrder).
                    Select(s => new
                    {
                        StateID = s.StateID,
                        StateName = s.StateShortName,
                        StateCodeID = s.StateCodeID,
                        CountryID = s.CountryID
                    }).ToTypeArray<Data.Extended.StateList>();

            // Cache the item.
            if (selectCache.CacheItems)
                selectCache.AddItemToCache(selectCache.CachedItemName, selectCache.CacheTimeout, list);

            return list;
        }

        /// <summary>
        /// Gets the state list.
        /// </summary>
        /// <param name="countryID">The country id to search on.</param>
        /// <returns>The state list.</returns>
        public virtual Data.Extended.StateList[] GetLongNameList(long countryID)
        {
            // Create the new cache control container.
            Nequeo.Data.Control.ICacheControl selectCache = Select;

            selectCache.CacheItems = true;
            selectCache.CachedItemName = "StateListLong" + countryID.ToString();
            selectCache.CacheTimeout = 200;

            Data.Extended.StateList[] list = null;

            // Is the item to be cached.
            if (selectCache.CacheItems)
            {
                // Get the item from the cache.
                object item = selectCache.GetItemFromCache(selectCache.CachedItemName);
                if (item != null)
                    if (item is Data.Extended.StateList[])
                        list = item as Data.Extended.StateList[];
            }

            // If the item has not been cached
            // then get the items from the database.
            if (list == null)
                list = Select.
                    SelectIQueryableItems(s => (s.StateVisible == true) && (s.CountryID == countryID)).
                    OrderBy(s => s.GroupOrder).
                    Select(s => new
                    {
                        StateID = s.StateID,
                        StateName = s.StateLongName,
                        StateCodeID = s.StateCodeID,
                        CountryID = s.CountryID
                    }).ToTypeArray<Data.Extended.StateList>();

            // Cache the item.
            if (selectCache.CacheItems)
                selectCache.AddItemToCache(selectCache.CachedItemName, selectCache.CacheTimeout, list);

            return list;
        }

        /// <summary>
        /// Gets the state list.
        /// </summary>
        /// <param name="languageCountryCode">The language country code to search on.</param>
        /// <returns>The state list.</returns>
        public virtual Data.Extended.StateList[] GetShortNameList(string languageCountryCode)
        {
            // Create the new cache control container.
            Nequeo.Data.Control.ICacheControl selectCache = Select;

            selectCache.CacheItems = true;
            selectCache.CachedItemName = "StateListShort" + languageCountryCode.ToString();
            selectCache.CacheTimeout = 200;

            Data.Extended.StateList[] list = null;

            // Is the item to be cached.
            if (selectCache.CacheItems)
            {
                // Get the item from the cache.
                object item = selectCache.GetItemFromCache(selectCache.CachedItemName);
                if (item != null)
                    if (item is Data.Extended.StateList[])
                        list = item as Data.Extended.StateList[];
            }

            // If the item has not been cached
            // then get the items from the database.
            if (list == null)
            {
                // Get the langauge details.
                Data.Language language = DataContext.Languages.First(l => l.LanguageCountryCode == languageCountryCode);
                Data.Country country = DataContext.Countries.First(c => c.CountryName == language.CountryName);
                long countryID = country.CountryID;

                list = Select.
                    SelectIQueryableItems(s => (s.StateVisible == true) && (s.CountryID == countryID)).
                    OrderBy(s => s.GroupOrder).
                    Select(s => new
                    {
                        StateID = s.StateID,
                        StateName = s.StateShortName,
                        StateCodeID = s.StateCodeID,
                        CountryID = s.CountryID
                    }).ToTypeArray<Data.Extended.StateList>();
            }

            // Cache the item.
            if (selectCache.CacheItems)
                selectCache.AddItemToCache(selectCache.CachedItemName, selectCache.CacheTimeout, list);

            return list;
        }

        /// <summary>
        /// Gets the state list.
        /// </summary>
        /// <param name="languageCountryCode">The language country code to search on.</param>
        /// <returns>The state list.</returns>
        public virtual Data.Extended.StateList[] GetLongNameList(string languageCountryCode)
        {
            // Create the new cache control container.
            Nequeo.Data.Control.ICacheControl selectCache = Select;

            selectCache.CacheItems = true;
            selectCache.CachedItemName = "StateListLong" + languageCountryCode.ToString();
            selectCache.CacheTimeout = 200;

            Data.Extended.StateList[] list = null;

            // Is the item to be cached.
            if (selectCache.CacheItems)
            {
                // Get the item from the cache.
                object item = selectCache.GetItemFromCache(selectCache.CachedItemName);
                if (item != null)
                    if (item is Data.Extended.StateList[])
                        list = item as Data.Extended.StateList[];
            }

            // If the item has not been cached
            // then get the items from the database.
            if (list == null)
            {
                // Get the langauge details.
                Data.Language language = DataContext.Languages.First(l => l.LanguageCountryCode == languageCountryCode);
                Data.Country country = DataContext.Countries.First(c => c.CountryName == language.CountryName);
                long countryID = country.CountryID;

                list = Select.
                    SelectIQueryableItems(s => (s.StateVisible == true) && (s.CountryID == countryID)).
                    OrderBy(s => s.GroupOrder).
                    Select(s => new
                    {
                        StateID = s.StateID,
                        StateName = s.StateLongName,
                        StateCodeID = s.StateCodeID,
                        CountryID = s.CountryID
                    }).ToTypeArray<Data.Extended.StateList>();
            }

            // Cache the item.
            if (selectCache.CacheItems)
                selectCache.AddItemToCache(selectCache.CachedItemName, selectCache.CacheTimeout, list);

            return list;
        }
    }
}
