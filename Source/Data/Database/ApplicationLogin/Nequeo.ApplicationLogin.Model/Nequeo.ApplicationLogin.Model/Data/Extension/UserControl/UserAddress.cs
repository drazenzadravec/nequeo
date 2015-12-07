/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          UserAddress.cs
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
    /// The user address data member extension.
    /// </summary>
    partial class UserAddress
    {
        /// <summary>
        /// Gets the user full name based on the user id.
        /// </summary>
        /// <param name="userAddressID">The user address id to search on.</param>
        /// <returns>The user full name.</returns>
        public virtual string GetUserNameForUserID(Int64 userAddressID)
        {
            string ret = null;

            // Get the current user id.
            System.Linq.IQueryable query =
                Select.
                SelectIQueryableItems(u => u.UserAddressID == userAddressID).
                Select(u => new { UserName = (u.FirstName + " " + u.LastName) });

            // Get the property.
            PropertyInfo property = query.ElementType.GetProperty("UserName");

            // Find the first user name.
            foreach (var item in query)
            {
                ret = (string)property.GetValue(item, null);
                break;
            }

            // Return the user full name.
            return ret;
        }

        /// <summary>
        /// Gets the user dddress id based on the user full name.
        /// </summary>
        /// <param name="userName">The user full name to search on.</param>
        /// <returns>The user id.</returns>
        public virtual Int64 GetUserIDForUserName(string userName)
        {
            Int64 ret = 0;

            // Split the user name.
            char[] delimeter = new char[] { ' ' };
            string[] splitList = userName.Split(delimeter);

            // Get the name of the user
            // from the split data.
            string firstName = splitList[0].Trim();
            string lastName = splitList[1].Trim();

            // Get the current user name.
            System.Linq.IQueryable query =
                Select.
                SelectIQueryableItems(u => (u.FirstName == firstName) && (u.LastName == lastName)).
                Select(u => new { UserIdentity = u.UserAddressID });

            // Get the property.
            PropertyInfo property = query.ElementType.GetProperty("UserIdentity");

            // Find the first user id.
            foreach (var item in query)
            {
                ret = (Int64)property.GetValue(item, null);
                break;
            }

            // Return the user id.
            return ret;
        }

        /// <summary>
        /// Gets the auto complete user data.
        /// </summary>
        /// <param name="prefixText">The prefix data to search for.</param>
        /// <param name="numberToReturn">The number of results to return.</param>
        /// <returns>The list of user.</returns>
        public virtual string[] GetUserAutoComplete(string prefixText, Int32 numberToReturn)
        {
            int i = 0;

            var query = Select.
                        SelectIQueryableItems(u => (SqlQueryMethods.Like(u.FirstName, (prefixText + "%"))) ||
                            (SqlQueryMethods.Like(u.LastName, (prefixText + "%")))).
                        Select(u => new
                        {
                            UserName = (string)u.FirstName + " " +
                             (string)u.LastName
                        }).Take(numberToReturn);

            string[] ret = new string[query.Count()];

            foreach (var item in query)
                ret[i++] = (string)item.UserName;

            return ret;
        }

        /// <summary>
        /// Gets the user email address.
        /// </summary>
        /// <param name="userAddressID">The user address id.</param>
        /// <returns>The email address.</returns>
        public virtual string GetUserEmail(Int64 userAddressID)
        {
            using (Data.DataContext context = new Data.DataContext())
            {
                var function = Nequeo.Linq.QueryCompiler.Compile(
                    (Int64 id) => context.UserAddresses.
                        Where(u => u.UserAddressID == id).
                        Select(u => u.EmailAddress));

                string email = function(userAddressID).First();
                return email;
            }
        }
    }
}
