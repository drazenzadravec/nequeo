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
using System.Configuration;
using System.Configuration.Provider;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web.Hosting;
using System.IO;
using System.Web.Compilation;

namespace Nequeo.Web.Provider
{
    /// <summary>
    /// Database source role provider
    /// </summary>
    public sealed class DataBaseRoleProvider : RoleProvider
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataBaseRoleProvider()
        {
        }

        private MachineKeySection _machineKey;
        private string _roleProviderType = null;
        private Nequeo.Data.DataType.IRoleProvider _roleProvider = null;

        /// <summary>
        /// Gets the role provider type.
        /// </summary>
        /// <remarks>The role provider type must implement 'Nequeo.Data.DataType.IRoleProvider'.</remarks>
        public string RoleProviderType
        {
            get { return _roleProviderType; }
        }

        /// <summary>
        /// Gets or sets the current Nequeo.Data.DataType.IRoleProvider.
        /// </summary>
        public Nequeo.Data.DataType.IRoleProvider RoleProviderTypeInstance
        {
            get { return _roleProvider; }
            set { _roleProvider = value; }
        }

        #region Abstract Property Overrides
        /// <summary>
        /// Gets sets the application name.
        /// </summary>
        public override string ApplicationName
        {
            get { return _roleProvider.ApplicationName; }
            set { _roleProvider.ApplicationName = value; }
        }

        #endregion

        #region Abstract Method Overrides
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs 
        /// representing the provider-specific attributes specified 
        /// in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "DataBaseRoleProvider";

            // Get the role application Name.
            if (String.IsNullOrEmpty(config["applicationName"]))
                throw new Exception("Attribute : applicationName, is missing this must be a valid application name.");

            // Get the role provider type.
            if (String.IsNullOrEmpty(config["roleProviderType"]))
                throw new Exception("Attribute : roleProviderType, is missing (this type must implement : 'Nequeo.Data.DataType.IRoleProvider')");
            else
                _roleProviderType = config["roleProviderType"].ToString();

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Nequeo Pty Limited generic data access role provider.");
            }

            // Get the current role provider type
            // and create a instance of the type.
            Type roleProviderType = BuildManager.GetType(_roleProviderType, true, true);
            _roleProvider = (Nequeo.Data.DataType.IRoleProvider)Activator.CreateInstance(roleProviderType);

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _roleProvider.ProviderName = name;
            _roleProvider.ApplicationName = GetConfigValue(config["applicationName"], "ApplicationName");

            // Get encryption and decryption key information from the configuration.
            System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
        }

        /// <summary>
        /// Add users to roles.
        /// </summary>
        /// <param name="usernames">The user list.</param>
        /// <param name="roleNames">The role list.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            _roleProvider.AddUsersToRoles(usernames, roleNames);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="roleName">The role name</param>
        public override void CreateRole(string roleName)
        {
            _roleProvider.CreateRole(roleName);
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
        /// <returns>true if the role was successfully deleted; otherwise, false.</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return _roleProvider.DeleteRole(roleName, throwOnPopulatedRole);
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name
        ///  matches usernameToMatch and the user is a member of the specified role.</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return _roleProvider.FindUsersInRole(roleName, usernameToMatch);
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source
        ///  for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            return _roleProvider.GetAllRoles();
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user
        ///  is in for the configured applicationName.</returns>
        public override string[] GetRolesForUser(string username)
        {
            return _roleProvider.GetRolesForUser(username);
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>A string array containing the names of all the users who are members of the
        ///  specified role for the configured applicationName.</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            return _roleProvider.GetUsersInRole(roleName);
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role
        ///  for the configured applicationName.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>true if the specified user is in the specified role for the configured applicationName;
        ///  otherwise, false.</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            return _roleProvider.IsUserInRole(username, roleName);
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured
        ///  applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            _roleProvider.RemoveUsersFromRoles(usernames, roleNames);
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in
        ///  the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>true if the role name already exists in the data source for the configured
        ///  applicationName; otherwise, false.</returns>
        public override bool RoleExists(string roleName)
        {
            return _roleProvider.RoleExists(roleName);
        }
        #endregion

        /// <summary>
        /// Get the configration value.
        /// </summary>
        /// <param name="configValue">The configuration value.</param>
        /// <param name="defaultValue">The default value to set.</param>
        /// <returns>The string value.</returns>
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }
    }
}
