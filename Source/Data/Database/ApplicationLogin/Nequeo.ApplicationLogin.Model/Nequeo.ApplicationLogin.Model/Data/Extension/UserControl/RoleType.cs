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
    /// The RoleType data member extension.
    /// </summary>
    partial class RoleType
    {
        /// <summary>
        /// Gets the roleType list.
        /// </summary>
        /// <returns>The roleType list.</returns>
        public virtual Data.Extended.RoleTypeList[] GetRoleTypeList()
        {
            IQueryable<Data.Extended.RoleTypeList> query =
                Select.
                SelectIQueryableItems(r => r.RoleTypeVisible == true).
                OrderBy(r => r.RoleTypeGroupOrder).
                Select(r => new Data.Extended.RoleTypeList()
                {
                    RoleTypeID = r.RoleTypeID,
                    RoleTypeName = r.RoleTypeName,
                    RoleTypeCodeID = (long)r.RoleTypeCodeID
                });

            return query.ToArray();
        }

        /// <summary>
        /// Gets the auto complete roleType data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of roleType.</returns>
        public virtual string[] GetRoleTypeAutoComplete(string prefixText, Int32 numberToReturn)
        {
            int i = 0;

            var query = Select.
                        SelectIQueryableItems(r => SqlQueryMethods.Like(r.RoleTypeName, (prefixText + "%"))).
                        Select(r => new
                        {
                            RoleTypeName = (string)r.RoleTypeName
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.RoleTypeName;

            return ret;
        }
    }
}
