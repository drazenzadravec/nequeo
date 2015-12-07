using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.SqlClient;

using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Linq.Extension;
using Nequeo.Data.Extension;
using Nequeo.Data.TypeExtenders;

namespace Nequeo.DataAccess.ApplicationLogin.Data.Extension
{
    /// <summary>
    /// The Language data member extension.
    /// </summary>
    partial class Language
    {
        /// <summary>
        /// Gets the language list.
        /// </summary>
        /// <returns>The language list.</returns>
        public virtual Data.Extended.LanguageList[] GetLanguageList()
        {
            return
                Select.
                SelectIQueryableItems(l => l.LanguageVisible == true).
                OrderBy(l => l.GroupOrder).
                Select(l => new
                {
                    LanguageID = l.LanguageID,
                    LanguageName = l.LanguageName,
                    LanguageCode = l.LanguageCode,
                    LanguageCountryCode = l.LanguageCountryCode,
                    CountryName = l.CountryName
                }).ToTypeArray<Data.Extended.LanguageList>();
        }

        /// <summary>
        /// Gets the language list.
        /// </summary>
        /// <param name="countryName">The country name to search on.</param>
        /// <returns>The language list.</returns>
        public virtual Data.Extended.LanguageList[] GetLanguageList(string countryName)
        {
            return
                Select.
                SelectIQueryableItems(l => (l.LanguageVisible == true) && (l.CountryName == countryName)).
                OrderBy(l => l.GroupOrder).
                Select(l => new
                {
                    LanguageID = l.LanguageID,
                    LanguageName = l.LanguageName,
                    LanguageCode = l.LanguageCode,
                    LanguageCountryCode = l.LanguageCountryCode,
                    CountryName = l.CountryName
                }).ToTypeArray<Data.Extended.LanguageList>();
        }

        /// <summary>
        /// Gets the auto complete language data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of language.</returns>
        public virtual string[] GetLanguageyAutoComplete(string prefixText, Int32 numberToReturn)
        {
            int i = 0;

            var query = Select.
                        SelectIQueryableItems(l => SqlQueryMethods.Like(l.LanguageName, (prefixText + "%"))).
                        Select(l => new
                        {
                            LanguageName = (string)l.LanguageName
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.LanguageName;

            return ret;
        }
    }
}