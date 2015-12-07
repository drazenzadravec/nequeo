/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          Application.cs
 *  Purpose :       
 */

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
using Nequeo.Linq.Extension;

namespace Nequeo.DataAccess.ApplicationLogin.Data.Extension
{
    /// <summary>
    /// The application data member extension.
    /// </summary>
    partial class Application
    {
        /// <summary>
        /// Gets the application list.
        /// </summary>
        /// <returns>The application list.</returns>
        public virtual Data.Extended.ApplicationList[] GetApplicationList()
        {
            return
                Select.
                SelectIQueryableItems(a => a.ApplicationVisible == true).
                OrderBy(a => a.ApplicationGroupOrder).
                Select(a => new
                {
                    ApplicationID = a.ApplicationID,
                    ApplicationName = a.ApplicationName,
                    ApplicationCodeID = a.ApplicationCodeID,
                    DepartmentID = a.DepartmentID
                }).ToTypeArray<Data.Extended.ApplicationList>();
        }

        /// <summary>
        /// Gets the application list of items that have been hidden (ApplicationVisible = False).
        /// </summary>
        /// <returns>The application list.</returns>
        public virtual Data.Extended.ApplicationList[] GetApplicationListNonVisible()
        {
            IQueryable<Data.Extended.ApplicationList> data =
                Select.
                SelectIQueryableItems(a => a.ApplicationVisible == false).
                OrderBy(a => a.ApplicationGroupOrder).
                Select(a => new Data.Extended.ApplicationList()
                {
                    ApplicationID = a.ApplicationID,
                    ApplicationName = a.ApplicationName,
                    ApplicationCodeID = a.ApplicationCodeID,
                    DepartmentID = a.DepartmentID
                });

            return data.ToArray();
        }

        /// <summary>
        /// Gets the auto complete application data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of applications.</returns>
        public virtual string[] GetApplicationAutoComplete(string prefixText, Int32 numberToReturn)
        {
            int i = 0;

            var query = Select.
                        SelectIQueryableItems(u => (SqlQueryMethods.Like(u.ApplicationName, (prefixText + "%")))).
                        Select(a => new 
                        {
                            ApplicationName = (string)a.ApplicationName 
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.ApplicationName;

            return ret;
        }

        /// <summary>
        /// Gets the application list.
        /// </summary>
        /// <param name="departmentID">the department id.</param>
        /// <returns>The application list.</returns>
        public virtual Data.Extended.ApplicationList[] GetApplicationList(long departmentID)
        {
            return
                DataContext.Applications.
                    Where("DepartmentID == @0 && ApplicationVisible == true", departmentID).
                    OrderBy("ApplicationGroupOrder").
                    Select("New (ApplicationID as ApplicationID, ApplicationName as ApplicationName, " +
                        "ApplicationCodeID as ApplicationCodeID, DepartmentID as DepartmentID)").
                    ToTypeArray<Data.Extended.ApplicationList>();
        }
    }
}
