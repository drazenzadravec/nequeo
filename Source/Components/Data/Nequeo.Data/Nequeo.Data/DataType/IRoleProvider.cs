/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace Nequeo.Data.DataType
{
    /// <summary>
    /// Custom role provider.
    /// </summary>
    public interface IRoleProvider
    {
        /// <summary>
        /// Gets sets the application name.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets sets the provider name.
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// Add users to roles.
        /// </summary>
        /// <param name="usernames">The user list.</param>
        /// <param name="roleNames">The role list.</param>
        void AddUsersToRoles(string[] usernames, string[] roleNames);

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="roleName">The role name</param>
        void CreateRole(string roleName);

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
        /// <returns>true if the role was successfully deleted; otherwise, false.</returns>
        bool DeleteRole(string roleName, bool throwOnPopulatedRole);

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name
        ///  matches usernameToMatch and the user is a member of the specified role.</returns>
        string[] FindUsersInRole(string roleName, string usernameToMatch);

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source
        ///  for the configured applicationName.</returns>
        string[] GetAllRoles();

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user
        ///  is in for the configured applicationName.</returns>
        string[] GetRolesForUser(string username);

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>A string array containing the names of all the users who are members of the
        ///  specified role for the configured applicationName.</returns>
        string[] GetUsersInRole(string roleName);

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role
        ///  for the configured applicationName.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>true if the specified user is in the specified role for the configured applicationName;
        ///  otherwise, false.</returns>
        bool IsUserInRole(string username, string roleName);

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured
        ///  applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        void RemoveUsersFromRoles(string[] usernames, string[] roleNames);

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in
        ///  the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>true if the role name already exists in the data source for the configured
        ///  applicationName; otherwise, false.</returns>
        bool RoleExists(string roleName);

    }
}
