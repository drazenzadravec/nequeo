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
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;

namespace Nequeo.DataAccess.ApplicationLogin.Data.Extension
{
    /// <summary>
    /// The Screen data member extension.
    /// </summary>
    partial class Screen
    {
        /// <summary>
        /// Gets the screen list.
        /// </summary>
        /// <returns>The screen list.</returns>
        public virtual Data.Extended.ScreenList[] GetScreenList()
        {
            IQueryable<Data.Extended.ScreenList> query =
                Select.
                SelectIQueryableItems(s => s.ScreenVisible == true).
                OrderBy(s => s.ScreenGroupOrder).
                Select(s => new Data.Extended.ScreenList()
                {
                    ScreenID = s.ScreenID,
                    ScreenName = s.ScreenName,
                    ScreenCodeID = s.ScreenCodeID,
                    ApplicationID = s.ApplicationID
                });

            return query.ToArray();
        }

        /// <summary>
        /// Gets the screen list.
        /// </summary>
        /// <param name="applicationID">The application id to search on.</param>
        /// <returns>The screen list.</returns>
        public virtual Data.Extended.ScreenList[] GetScreenList(long applicationID)
        {
            IQueryable<Data.Extended.ScreenList> query =
                Select.
                SelectIQueryableItems(s => (s.ScreenVisible == true) && (s.ApplicationID == applicationID)).
                OrderBy(s => s.ScreenGroupOrder).
                Select(s => new Data.Extended.ScreenList()
                {
                    ScreenID = s.ScreenID,
                    ScreenName = s.ScreenName,
                    ScreenCodeID = s.ScreenCodeID,
                    ApplicationID = s.ApplicationID
                });

            return query.ToArray();
        }

        /// <summary>
        /// Gets the auto complete screen data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of screen.</returns>
        public virtual string[] GetScreenAutoComplete(string prefixText, Int32 numberToReturn)
        {
            int i = 0;

            var query = Select.
                        SelectIQueryableItems(s => SqlQueryMethods.Like(s.ScreenName, (prefixText + "%"))).
                        Select(s => new
                        {
                            ScreenName = (string)s.ScreenName
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.ScreenName;

            return ret;
        }
    }
}