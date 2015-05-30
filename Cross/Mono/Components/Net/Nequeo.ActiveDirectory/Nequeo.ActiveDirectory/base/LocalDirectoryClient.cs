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
using System.Net;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace Nequeo.Net.ActiveDirectory
{
    /// <summary>
    /// WinNT, local machine directory services client.
    /// </summary>
    public partial class LocalDirectoryClient : Nequeo.Security.IAuthorisationProvider, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>Context principal relates to the current local machine.</remarks>
        public LocalDirectoryClient()
        {
            OnCreated();
        }
        #endregion

        #region Private Fields
        private bool _disposed = false;
        private static PrincipalContext _context =
            new PrincipalContext(ContextType.Machine, Environment.MachineName);
        #endregion

        #region Public Events
        /// <summary>
        /// On create account event handler;
        /// </summary>
        public event CreateAccountHandler OnAccountCreated;

        /// <summary>
        /// On remove account event handler;
        /// </summary>
        public event CreateAccountHandler OnAccountRemoved;
        #endregion

        #region Public Methods
        /// <summary>
        /// Authenticate user credentials.
        /// </summary>
        /// <param name="userCredentials">The user credentials.</param>
        /// <returns>True if authenticated; else false.</returns>
        public virtual bool AuthenticateUser(Security.UserCredentials userCredentials)
        {
            return Authenticate(userCredentials.Username, userCredentials.Password);
        }

        /// <summary>
        /// Authenticate the user on the on the local machine.
        /// </summary>
        /// <param name="username">The user name to authentication.</param>
        /// <param name="password">The password to authentication.</param>
        /// <returns>True if the user was authenticated else false.</returns>
        public virtual bool Authenticate(string username, string password)
        {
            bool authenticated = false;

            // Attempt to make a connection to the active directory server.
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName))
            {
                // Attempt to authenticated the user on the server
                // for a simple binding connection.
                authenticated = context.ValidateCredentials(username, password);
            }

            // Return the result.
            return authenticated;
        }

        /// <summary>
        /// Attempts to authenticate the user.
        /// </summary>
        /// <param name="username">The user username.</param>
        /// <param name="password">The user password.</param>
        /// <returns>True if the user was authenticated else false.</returns>
        public virtual bool AuthenticateUser(string username, string password)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(password))
                throw new System.ArgumentNullException("Password can not be null.",
                    new System.Exception("A valid password should be specified."));

            try
            {
                bool ret = false;

                // Create a new instance of the windows authentication class.
                using (Nequeo.Security.WindowsAuthentication auth = new Nequeo.Security.WindowsAuthentication())
                {
                    // Attach to the on authenticate
                    // event through an anonymous
                    // delegate.
                    auth.OnAuthenticate += delegate(Object sender, Nequeo.Security.AuthenticateArgs e)
                    {
                        // if the user has been authenticated
                        // then the IsAuthenticated property
                        // will return true else false.
                        // Assign the ret value.
                        ret = e.IsAuthenticated;
                    };

                    bool result = auth.AuthenticateUser(username, _context.Name, password);

                    // Return the result from the
                    // on authenticate delegate
                    // that was assigned.
                    return ret;
                }
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Attempts to authenticate the domain user.
        /// </summary>
        /// <param name="username">The user username.</param>
        /// <param name="password">The user password.</param>
        /// <param name="domain">The user domain.</param>
        /// <returns>True if the user was authenticated else false.</returns>
        public virtual bool AuthenticateUser(string username, string password, string domain)
        {
            string domainAndUsername = domain + "\\" + username;
            DirectoryEntry entry = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer", domainAndUsername, password);

            // Bind to the native AdsObject to force authentication.
            object native = entry.NativeObject;
            DirectorySearcher search = new DirectorySearcher(entry);

            search.Filter = "(SAMAccountName=" + username + ")";
            search.PropertiesToLoad.Add("cn");
            SearchResult result = search.FindOne();

            if (result == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the current user name.
        /// </summary>
        /// <param name="includeDomain">Should the domain be included.</param>
        /// <returns>The current user name.</returns>
        public virtual string GetCurrentUser(bool includeDomain)
        {
            // Get the current login details.
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            // If the domain should be included.
            if (includeDomain)
                // Send the complete identity
                // including domain.
                return identity.Name;
            else
            {
                // Get the length of the domain.
                // Get the starting point do not
                // include the domain.
                int length = Environment.MachineName.Length + 1;
                int startIndex = identity.Name.IndexOf(Environment.MachineName) + length;

                // Get the user name of the
                // current account.
                return identity.Name.Substring(startIndex).Replace("\\", "");
            }
        }

        /// <summary>
        /// Gets all users that are members of the group.
        /// </summary>
        /// <param name="group">The local machine group.</param>
        /// <returns>The list of users that are members of the group.</returns>
        public virtual List<DirectoryEntry> GetUserMembershipGroupAccount(string group)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(group))
                throw new System.ArgumentNullException("Group can not be null.",
                    new System.Exception("A valid group should be specified."));

            // Get all the users on the local machine.
            // Create a new list entry instance.
            List<DirectoryEntry> userList = GetAllUserAccounts();
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            // For each user in the user collection.
            foreach (DirectoryEntry user in userList)
            {
                // Get the groups that have the current
                // user as a member.
                object groups = user.Invoke("Groups", null);

                // For each user in the group.
                foreach (object groupValue in (System.Collections.IEnumerable)groups)
                {
                    // Get the current entry
                    // for the group found.
                    DirectoryEntry entry = new DirectoryEntry(groupValue);

                    try
                    {
                        // If the current entry path contains
                        // the group entry, that is, is the
                        // current group entry the required
                        // group to match then add the user.
                        // This indicates that the user is
                        // part of the specified group.
                        if (entry.Path.EndsWith(group))
                            list.Add(user);
                    }
                    catch { }
                }
            }

            // Return the list of users.
            return list;
        }

        /// <summary>
        /// Gets all groups that the user is a member of.
        /// </summary>
        /// <param name="username">The local machine user.</param>
        /// <returns>The list of all groups that the user is a member of.</returns>
        public virtual List<DirectoryEntry> GetGroupMembershipUserAccount(string username)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Get all the groups on the local machine.
            // Create a new list entry instance.
            List<DirectoryEntry> groupList = GetAllGroupAccounts();
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            // For each group in the group collection.
            foreach (DirectoryEntry group in groupList)
            {
                // Get the members that are part
                // of the group.
                object members = group.Invoke("Members", null);

                // For each member in the collection.
                foreach (object member in (System.Collections.IEnumerable)members)
                {
                    // Get the current entry
                    // for the member found.
                    DirectoryEntry entry = new DirectoryEntry(member);

                    try
                    {
                        // If the current entry path contains
                        // the member entry, that is, is the
                        // current member entry the required
                        // user to match then add the group.
                        // This indicates that the group has
                        // the user as a member.
                        if (entry.Path.EndsWith(username))
                            list.Add(group);
                    }
                    catch { }
                }
            }

            // Return the list of groups.
            return list;
        }

        /// <summary>
        /// Gets the collection of all users on the local machine.
        /// </summary>
        /// <returns>The list of all users.</returns>
        public virtual List<DirectoryEntry> GetAllUserAccounts()
        {
            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Create a new list collection instance
            // of directory entries.
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            // Get all the currrent accounts on the local machine.
            DirectoryEntries users = localMachine.Children;

            // Get the enumerator of the account collection.
            System.Collections.IEnumerator col = users.GetEnumerator();

            // For each account found.
            while (col.MoveNext())
            {
                // If the current schema class is 'user' then add
                // the entry to the collection, that is, if the current
                // account is of type 'user' then add to collection.
                if (((DirectoryEntry)col.Current).SchemaClassName.ToLower() == "user")
                    list.Add((DirectoryEntry)col.Current);
            }

            // Close the entry to the local machine.
            localMachine.Close();

            // Return the collection of users.
            return list;
        }

        /// <summary>
        /// Gets the collection of all groups on the local machine.
        /// </summary>
        /// <returns>The list of all groups.</returns>
        public virtual List<DirectoryEntry> GetAllGroupAccounts()
        {
            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Create a new list collection instance
            // of directory entries.
            List<DirectoryEntry> list = new List<DirectoryEntry>();

            // Get all the currrent accounts on the local machine.
            DirectoryEntries groups = localMachine.Children;

            // Get the enumerator of the account collection.
            System.Collections.IEnumerator col = groups.GetEnumerator();

            // For each account found.
            while (col.MoveNext())
            {
                // If the current schema class is 'group' then add
                // the entry to the collection, that is, if the current
                // account is of type 'group' then add to collection.
                if (((DirectoryEntry)col.Current).SchemaClassName.ToLower() == "group")
                    list.Add((DirectoryEntry)col.Current);
            }

            // Close the entry to the local machine.
            localMachine.Close();

            // Return the collection of groups.
            return list;
        }

        /// <summary>
        /// Creates a new user account on the local machine.
        /// </summary>
        /// <param name="username">The username for the account.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="fullName">The full name for the account.</param>
        /// <param name="description">The description for the account.</param>
        /// <returns>True if the user account was created else false.</returns>
        public virtual bool CreateUserAccount(string username, string password,
            string fullName, string description)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(password))
                throw new System.ArgumentNullException("Password can not be null.",
                    new System.Exception("A valid password should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(fullName))
                throw new System.ArgumentNullException("Fullname can not be null.",
                    new System.Exception("A valid fullname should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(description))
                throw new System.ArgumentNullException("Description can not be null.",
                    new System.Exception("A valid description should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Add the user account to the user collection.
            DirectoryEntry user = localMachine.Children.Add(username, "user");

            // Set the password, description and full name for the account.
            user.Invoke("SetPassword", new object[] { password });
            user.Invoke("Put", new object[] { "Description", description });
            user.Invoke("Put", new object[] { "FullName", fullName });

            // Commit the changes for the account.
            user.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // If the event has been attached
            // send to the client the data through
            // the delegate.
            if (OnAccountCreated != null)
                OnAccountCreated(this, true);

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Adds the specified group to the collection.
        /// </summary>
        /// <param name="name">The name of the group to add.</param>
        /// <param name="description">The description of the group.</param>
        /// <returns>True if the group was added else false.</returns>
        public virtual bool AddGroup(string name, string description)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("Name can not be null.",
                    new System.Exception("A valid name should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(description))
                throw new System.ArgumentNullException("Description can not be null.",
                    new System.Exception("A valid description should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Add the group account to the group collection.
            DirectoryEntry group = localMachine.Children.Add(name, "group");

            // Set description for the account.
            group.Invoke("Put", new object[] { "Description", description });

            // Commit the changes for the account.
            group.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            group.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Removes the specified group from the collection.
        /// </summary>
        /// <param name="name">The name of the group to remove.</param>
        /// <returns>True if the group was removed else false.</returns>
        public virtual bool RemoveGroup(string name)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("Name can not be null.",
                    new System.Exception("A valid name should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current group account.
            DirectoryEntry group = localMachine.Children.Find(name, "group");

            // Remove the group account.
            localMachine.Children.Remove(group);

            // Close the entry to the local machine.
            localMachine.Close();
            group.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Enable or disable the user account.
        /// </summary>
        /// <param name="username">The username for the account.</param>
        /// <param name="disable">Enable or disable the property.</param>
        /// <returns>True if the property was set else false.</returns>
        public virtual bool EnableDisableUserAccount(string username, bool disable)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");

            // Get the current user account disabled property
            // assign the status of the property as bool.
            object accountStatus = user.InvokeGet("AccountDisabled");
            bool status = (bool)accountStatus;

            // If account should be disabled
            if (disable)
            {
                // If the status is false.
                if (!status)
                    // Disable the current user account,
                    // set the property to true.
                    user.InvokeSet("AccountDisabled", new object[] { true });
            }
            // Account should be enabled.
            else
            {
                // If the status is true.
                if (status)
                    // Enable the current user account,
                    // set the property to false.
                    user.InvokeSet("AccountDisabled", new object[] { false });
            }

            // Commit the changes for the account.
            user.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Set the user account change password at next logon indicator.
        /// </summary>
        /// <param name="username">The username for the account.</param>
        /// <param name="enable">Enable or disable the property.</param>
        /// <returns>True if the change password at next logon property was set else false.</returns>
        public virtual bool UserAccountChangePasswordAtLogon(string username, bool enable)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");

            // Get the current user account disabled property
            // assign the status of the property as bool.
            object accountStatus = user.InvokeGet("PasswordExpired");
            int status = (int)accountStatus;

            if (enable)
                // If password change account should be enabled
                user.InvokeSet("PasswordExpired", new object[] { 1 });

            else
                // Password account should be disabled.
                user.InvokeSet("PasswordExpired", new object[] { 0 });

            // Commit the changes for the account.
            user.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Set the user account password never expires indicator.
        /// </summary>
        /// <param name="username">The username for the account.</param>
        /// <param name="enable">Enable or disable the property.</param>
        /// <returns>True if the password never expires property was set else false.</returns>
        public virtual bool UserAccountPasswordNeverExpires(string username, bool enable)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");

            // Get the current user flag information
            // for the account, assign the flag to
            // the status integer object.
            object accountStatus = user.Properties["UserFlags"].Value;
            int status = (int)accountStatus;

            // if true
            if (enable)
                // Add the no expired password index to
                // the current user flag value. Enable
                // the password never expires property.
                user.Properties["UserFlags"].Value = status | (int)ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD;

            else
                // Take awway the no expired password index
                // from the currrent user flag value. disable
                // the password never expires property.
                user.Properties["UserFlags"].Value = status - (int)ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD;

            // Commit the changes for the account.
            user.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Set the user account password can not change indicator
        /// </summary>
        /// <param name="username">The username for the account.</param>
        /// <param name="enable">Enable or disable the property.</param>
        /// <returns>True if the password can not change property was set else false.</returns>
        public virtual bool UserAccountPasswordCannotChange(string username, bool enable)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");

            // Get the current user flag information
            // for the account, assign the flag to
            // the status integer object.
            object accountStatus = user.Properties["UserFlags"].Value;
            int status = (int)accountStatus;

            // if true
            if (enable)
                // Add the no expired password index to
                // the current user flag value. Enable
                // the password never expires property.
                user.Properties["UserFlags"].Value = status | (int)ADS_USER_FLAG_ENUM.ADS_UF_PASSWD_CANT_CHANGE;

            else
                // Take awway the no expired password index
                // from the currrent user flag value. disable
                // the password never expires property.
                user.Properties["UserFlags"].Value = status - (int)ADS_USER_FLAG_ENUM.ADS_UF_PASSWD_CANT_CHANGE;

            // Commit the changes for the account.
            user.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Sets the password of the specified user account.
        /// </summary>
        /// <param name="username">The username of the account.</param>
        /// <param name="password">The password for the account.</param>
        /// <returns>True if the password was set else false.</returns>
        public virtual bool SetUserAccountPassword(string username, string password)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(password))
                throw new System.ArgumentNullException("Password can not be null.",
                    new System.Exception("A valid password should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");

            // Set the password for the account.
            user.Invoke("SetPassword", new object[] { password });

            // Commit the changes for the account.
            user.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Renames an existing account to the specified account name.
        /// </summary>
        /// <param name="username">The current user account name.</param>
        /// <param name="newUsername">The name to change to.</param>
        /// <returns>True if user account name was changed.</returns>
        public virtual bool RenameUserAccount(string username, string newUsername)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(newUsername))
                throw new System.ArgumentNullException("New username can not be null.",
                    new System.Exception("A valid new username should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");

            // Rename the current user account.
            user.Rename(newUsername);

            // Commit the changes for the account.
            user.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Removes the specified user account from the user collection.
        /// </summary>
        /// <param name="username">The current user account name.</param>
        /// <returns>True if the account was removed else false.</returns>
        public virtual bool RemoveUserAccount(string username)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");

            // Remove the user account.
            localMachine.Children.Remove(user);

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();

            // If the event has been attached
            // send to the client the data through
            // the delegate.
            if (OnAccountRemoved != null)
                OnAccountRemoved(this, true);

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Adds the user account to the group account.
        /// </summary>
        /// <param name="username">The current user account name.</param>
        /// <param name="group">The current group account name.</param>
        /// <returns>True if the user was added to the group else false.</returns>
        public virtual bool AddUserToGroup(string username, string group)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(group))
                throw new System.ArgumentNullException("Group can not be null.",
                    new System.Exception("A valid group should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            // Find the entry for the current group account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");
            DirectoryEntry grp = localMachine.Children.Find(group, "group");

            // Add the user to the group.
            grp.Invoke("Add", new object[] { user.Path.ToString() });

            // Commit the changes for the account.
            grp.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();
            grp.Close();

            // Return true if all
            // operations completed.
            return true;
        }

        /// <summary>
        /// Removes the user account from the group account.
        /// </summary>
        /// <param name="username">The current user account name.</param>
        /// <param name="group">The current group account name.</param>
        /// <returns>True if the user was removed from the group else false.</returns>
        public virtual bool RemoveUserFromGroup(string username, string group)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(group))
                throw new System.ArgumentNullException("Group can not be null.",
                    new System.Exception("A valid group should be specified."));

            // Create a new directory entry
            // instance to the local machine.
            DirectoryEntry localMachine = new DirectoryEntry(
                "WinNT://" + _context.Name + ",computer");

            // Find the entry for the current user account.
            // Find the entry for the current group account.
            DirectoryEntry user = localMachine.Children.Find(username, "user");
            DirectoryEntry grp = localMachine.Children.Find(group, "group");

            // Remove the user from the group.
            grp.Invoke("Remove", new object[] { user.Path.ToString() });

            // Commit the changes for the account.
            grp.CommitChanges();

            // Close the entry to the local machine.
            localMachine.Close();
            user.Close();
            grp.Close();

            // Return true if all
            // operations completed.
            return true;
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if(_context != null)
                        _context.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _context = null;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~LocalDirectoryClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
