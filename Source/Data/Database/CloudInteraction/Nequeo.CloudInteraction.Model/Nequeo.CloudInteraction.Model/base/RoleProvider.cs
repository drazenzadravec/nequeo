/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          MembershipProvider.cs
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Configuration.Provider;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using Nequeo.ComponentModel.Composition;
using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;
using Nequeo.Web;

namespace Nequeo.DataAccess.CloudInteraction
{
	/// <summary>
	/// Cloud Interaction role provider base.
	/// </summary>
	public class RoleProviderBase
	{
		/// <summary>
		/// Gets the current RoleProvider context
		/// </summary>
		public System.Web.Security.RoleProvider Context
		{
			get
			{
				// Create the role provider instance.
				Nequeo.Web.Provider.DataBaseRoleProvider provider = new Web.Provider.DataBaseRoleProvider();
				Nequeo.Data.DataType.IRoleProvider cloudProvider = new RoleProvider();

				// Assign the current cloud role provider instance.
				provider.RoleProviderTypeInstance = cloudProvider;
				return provider;
			}
		}
	}

	/// <summary>
	/// Cloud Interaction role provider.
	/// </summary>
	[Export(typeof(Nequeo.Data.DataType.IRoleProvider))]
	[ContentMetadata(Name = "CloudInteractionRoleProvider", Index = 0)]
	public class RoleProvider : Nequeo.Data.DataType.IRoleProvider
	{
		private string _providerName = string.Empty;
		private string _applicationName = string.Empty;

		/// <summary>
		/// Gets sets the application name.
		/// </summary>
		public string ApplicationName
		{
			get { return _applicationName; }
			set { _applicationName = value; }
		}

		/// <summary>
		/// Gets sets the provider name.
		/// </summary>
		public string ProviderName
		{
			get { return _providerName; }
			set { _providerName = value; }
		}

		/// <summary>
		/// Add users to the first role name in the role names collection (only one iteration).
		/// </summary>
		/// <param name="usernames">The user list.</param>
		/// <param name="roleNames">The role list.</param>
		public void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			// For each user.
			foreach (string username in usernames)
			{
				// For each role.
				foreach (string rolename in roleNames)
				{
					// If the user is not in the role
					if (!IsUserInRole(username, rolename))
					{
						// Find the user.
						var user = GetSpecificUser(username);
						var role = GetSpecificRole(rolename);

						// If the user and role exists
						// then assign the role to the user.
						if (user != null && role != null)
						{
							// Update the user role with roleID.
							user.RoleID = role.RoleID;
							new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
						}
					}

					// Only for the first role name.
					break;
				}
			}
		}

		/// <summary>
		/// Create a new role
		/// </summary>
		/// <param name="roleName">The role name</param>
		public void CreateRole(string roleName)
		{
			// Gets a value indicating whether the specified role name already exists in
			// the role data source for the configured applicationName.
			bool roleExists = RoleExists(roleName);

			// If roles exists
			if (roleExists)
				throw new ProviderException("Role rolename : " + roleName + " already exists.");
			else
			{
				// Add the new role.
				new Nequeo.DataAccess.CloudInteraction.Data.Extension.Role().Insert.InsertItem
					(
						new Nequeo.DataAccess.CloudInteraction.Data.Role() 
						{
							ApplicationName = ApplicationName,
							RoleName = roleName,
							RoleDescription = roleName + " Role"
						}
					);
			}
		}

		/// <summary>
		/// Removes a role from the data source for the configured applicationName.
		/// </summary>
		/// <param name="roleName">The role name</param>
		/// <param name="throwOnPopulatedRole">This always throws, throw an exception if roleName has more tna one members and do not delete roleName.</param>
		/// <returns>true if the role was successfully deleted; otherwise, false.</returns>
		public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			// Gets a list of users in the specified role for the configured applicationName.
			string[] usersInRole = GetUsersInRole(roleName);

			if (usersInRole == null)
				return true;
			else if (usersInRole.Length > 1)
				throw new ProviderException("Role rolename : " + roleName + " has more than one member.");
			else
			{
				// Delete the role.
				return new Nequeo.DataAccess.CloudInteraction.Data.Extension.Role().Delete.DeleteItemPredicate(
					u =>
						(u.RoleName == roleName) &&
						(u.ApplicationName == ApplicationName)
					);
			}
		}

		/// <summary>
		/// Gets an array of user names in a role where the user name contains the specified user name to match.
		/// </summary>
		/// <param name="roleName">The role to search in.</param>
		/// <param name="usernameToMatch">The user name to search for.</param>
		/// <returns>A string array containing the names of all the users where the user name
		///  matches usernameToMatch and the user is a member of the specified role.</returns>
		public string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			// Get the role for the user.
			var role = GetSpecificRole(roleName);

			// If role exists.
			if (role != null)
			{
				long roleID = role.RoleID;

				// Get the user extender.
				Nequeo.DataAccess.CloudInteraction.Data.Extension.User userExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();
				Nequeo.DataAccess.CloudInteraction.Data.User[] users = userExt.Select.SelectDataEntitiesPredicate(u =>
					(Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Email, ("%" + usernameToMatch + "%"))) &&
					(u.RoleID != null) &&
					(u.RoleID == roleID));

				// Add to the collection the users.
				List<string> usersInRoles = new List<string>();
				foreach (Nequeo.DataAccess.CloudInteraction.Data.User user in users)
					usersInRoles.Add(user.Username);

				// Return the array of users in the role.
				return usersInRoles.ToArray();
			}
			else
				throw new ProviderException("Role rolename : " + roleName + " does not exist.");
		}

		/// <summary>
		/// Gets a list of all the roles for the configured applicationName.
		/// </summary>
		/// <returns>A string array containing the names of all the roles stored in the data source
		///  for the configured applicationName.</returns>
		public string[] GetAllRoles()
		{
			Nequeo.DataAccess.CloudInteraction.Data.Extension.Role roleExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Role();
			Nequeo.DataAccess.CloudInteraction.Data.Role[] roles = roleExt.Select.SelectDataEntitiesPredicate(u => (u.ApplicationName == ApplicationName));

			// Add to the collection the roles.
			List<string> rolesCol = new List<string>();
			foreach (Nequeo.DataAccess.CloudInteraction.Data.Role role in roles)
				rolesCol.Add(role.RoleName);

			// Return the array of roles for user.
			return rolesCol.ToArray();
		}

		/// <summary>
		/// Gets a list of the roles that a specified user is in for the configured applicationName.
		/// </summary>
		/// <param name="username">The user to return a list of roles for.</param>
		/// <returns>A string array containing the names of all the roles that the specified user
		///  is in for the configured applicationName.</returns>
		public string[] GetRolesForUser(string username)
		{
			// Find the user.
			var user = GetSpecificUser(username);
			if (user != null)
			{
				// If a role has been assigned to the user.
				if (user.RoleID != null)
				{
					long roleID = user.RoleID.Value;

					Nequeo.DataAccess.CloudInteraction.Data.Extension.Role roleExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Role();
					Nequeo.DataAccess.CloudInteraction.Data.Role[] roles = roleExt.Select.SelectDataEntitiesPredicate(u => (u.RoleID == roleID));

					// Add to the collection the roles.
					List<string> rolesForUser = new List<string>();
					foreach (Nequeo.DataAccess.CloudInteraction.Data.Role role in roles)
						rolesForUser.Add(role.RoleName);

					// Return the array of roles for user.
					return rolesForUser.ToArray();
				}
				else
					return new string[0];
			}
			else
				throw new ProviderException("User username : " + username + " does not exist.");
		}

		/// <summary>
		/// Gets a list of users in the specified role for the configured applicationName.
		/// </summary>
		/// <param name="roleName">The name of the role to get the list of users for.</param>
		/// <returns>A string array containing the names of all the users who are members of the
		///  specified role for the configured applicationName.</returns>
		public string[] GetUsersInRole(string roleName)
		{
			// Get the role for the user.
			var role = GetSpecificRole(roleName);

			// If role exists.
			if (role != null)
			{
				long roleID = role.RoleID;

				// Get the user extender.
				Nequeo.DataAccess.CloudInteraction.Data.Extension.User userExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();
				Nequeo.DataAccess.CloudInteraction.Data.User[] users = userExt.Select.SelectDataEntitiesPredicate(u => (u.RoleID != null) && (u.RoleID == roleID));

				// Add to the collection the users.
				List<string> usersInRoles = new List<string>();
				foreach (Nequeo.DataAccess.CloudInteraction.Data.User user in users)
					usersInRoles.Add(user.Username);

				// Return the array of users in the role.
				return usersInRoles.ToArray();
			}
			else
				throw new ProviderException("Role rolename : " + roleName + " does not exist.");
		}

		/// <summary>
		/// Gets a value indicating whether the specified user is in the specified role
		///  for the configured applicationName.
		/// </summary>
		/// <param name="username">The user name to search for.</param>
		/// <param name="roleName">The role to search in.</param>
		/// <returns>true if the specified user is in the specified role for the configured applicationName;
		///  otherwise, false.</returns>
		public bool IsUserInRole(string username, string roleName)
		{
			// Find the user.
			var user = GetSpecificUser(username);
			if (user != null)
			{
				// If a role has been assigned to the user.
				if (user.RoleID != null)
				{
					// Get the role for the user.
					var role = GetSpecificRole(user.RoleID.Value);

					// Has the user been assigned to the role.
					if (role.RoleName.ToLower() == roleName.ToLower())
						return true;
					else
						return false;
				}
				else
					return false;
			}
			else
				throw new ProviderException("User username : " + username + " does not exist.");
		}

		/// <summary>
		/// Removes the specified user names from the specified roles for the configured
		///  applicationName.
		/// </summary>
		/// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
		/// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
		public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			// For each user.
			foreach (string username in usernames)
			{
				// For each role.
				foreach (string rolename in roleNames)
				{
					// If the user is in the role
					if (IsUserInRole(username, rolename))
					{
						// Find the user.
						var user = GetSpecificUser(username);

						// Update the user role with null, not assigned.
						user.RoleID = null;
						new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the specified role name already exists in
		///  the role data source for the configured applicationName.
		/// </summary>
		/// <param name="roleName">The name of the role to search for in the data source.</param>
		/// <returns>true if the role name already exists in the data source for the configured
		///  applicationName; otherwise, false.</returns>
		public bool RoleExists(string roleName)
		{
			var role = GetSpecificRole(roleName);
			if (role != null)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Get the specific role for the current application.
		/// </summary>
		/// <param name="roleName">The rolename.</param>
		/// <returns>The role; else null.</returns>
		private Nequeo.DataAccess.CloudInteraction.Data.Role GetSpecificRole(string roleName)
		{
			// Get the role data.
			Nequeo.DataAccess.CloudInteraction.Data.Extension.Role roleExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Role();
			Nequeo.DataAccess.CloudInteraction.Data.Role role =
				roleExt.Select.SelectDataEntity(
					u =>
						(u.RoleName == roleName) &&
						(u.ApplicationName == ApplicationName)
				);

			// Return the role.
			return role;
		}

		/// <summary>
		/// Get the specific role for the current application.
		/// </summary>
		/// <param name="roleID">The roleID.</param>
		/// <returns>The role; else null.</returns>
		private Nequeo.DataAccess.CloudInteraction.Data.Role GetSpecificRole(long roleID)
		{
			// Get the role data.
			Nequeo.DataAccess.CloudInteraction.Data.Extension.Role roleExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.Role();
			Nequeo.DataAccess.CloudInteraction.Data.Role role =
				roleExt.Select.SelectDataEntity(
					u =>
						(u.RoleID == roleID)
				);

			// Return the role.
			return role;
		}

		/// <summary>
		/// Get the specific user for the current application.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns>The user; else null.</returns>
		private Nequeo.DataAccess.CloudInteraction.Data.User GetSpecificUser(string username)
		{
			// Get the user data.
			Nequeo.DataAccess.CloudInteraction.Data.Extension.User userExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();
			Nequeo.DataAccess.CloudInteraction.Data.User user =
				userExt.Select.SelectDataEntity(
					u =>
						(u.Username == username) &&
						(u.ApplicationName == ApplicationName)
				);

			// Return the user.
			return user;
		}

		/// <summary>
		/// Get the specific user for the current application.
		/// </summary>
		/// <param name="userID">The userid.</param>
		/// <returns>The user; else null.</returns>
		private Nequeo.DataAccess.CloudInteraction.Data.User GetSpecificUser(long userID)
		{
			// Get the user data.
			Nequeo.DataAccess.CloudInteraction.Data.Extension.User userExt = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();
			Nequeo.DataAccess.CloudInteraction.Data.User user =
				userExt.Select.SelectDataEntity(
					u =>
						(u.UserID == userID)
				);

			// Return the user.
			return user;
		}
	}
}
