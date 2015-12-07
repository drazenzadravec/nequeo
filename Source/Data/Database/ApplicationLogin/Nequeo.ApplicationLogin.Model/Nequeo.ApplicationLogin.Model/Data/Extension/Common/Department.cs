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
    /// The Department data member extension.
    /// </summary>
    partial class Department
    {
        /// <summary>
        /// Gets the department.
        /// </summary>
        /// <param name="departmentID">the deparment id.</param>
        /// <returns>The department item.</returns>
        public virtual Data.Department GetDepartment(long departmentID)
        {
            IEnumerable<Data.Department> query =
                from d in DataContext.Departments
                where d.DepartmentID == departmentID
                orderby d.DepartmentName
                select d;

            return query.First();
        }

        /// <summary>
        /// Gets the department list.
        /// </summary>
        /// <param name="companyID">the company id.</param>
        /// <returns>The department list.</returns>
        public virtual Data.Extended.DepartmentList[] GetDepartmentList(long companyID)
        {
            IEnumerable<Data.Extended.DepartmentList> query =
                from d in DataContext.Departments
                where d.CompanyID == companyID
                orderby d.DepartmentName
                select new Data.Extended.DepartmentList()
                {
                    CompanyID = d.CompanyID,
                    DepartmentID = d.DepartmentID,
                    DepartmentName = d.DepartmentName
                };

            return query.ToArray();
        }

        /// <summary>
        /// Gets the auto complete department data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of department.</returns>
        public static string[] GetDepartmentAutoComplete(string prefixText, Int32 numberToReturn)
        {
            Department dep = new Department();
            
            int i = 0;

            var query = dep.Select.
                        SelectIQueryableItems(d => (SqlQueryMethods.Like(d.DepartmentName, (prefixText + "%")))).
                        Select(d => new
                        {
                            DepartmentName = (string)d.DepartmentName
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.DepartmentName;

            return ret;
        }
    }
}