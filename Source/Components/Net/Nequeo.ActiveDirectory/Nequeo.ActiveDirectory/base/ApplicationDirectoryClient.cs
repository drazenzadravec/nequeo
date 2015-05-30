/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          DomainDirectoryClient.cs
 *  Purpose :       This file contains classes that can be
 *                  used to access the domain machine
 *                  directory service.
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

using Nequeo.Net.ActiveDirectory.Model;

namespace Nequeo.Net.ActiveDirectory
{
    /// <summary>
    /// Lightweight Directory Access Protocol (LDAP),
    /// application directory services client (AD LDS).
    /// </summary>
    public partial class ApplicationDirectoryClient : Nequeo.Security.IAuthorisationProvider, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>Context principal relates to the current application machine.</remarks>
        public ApplicationDirectoryClient()
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="authUsername">The authorisation username</param>
        /// <param name="authPassword">The password username</param>
        /// <remarks>Context principal relates to the current application machine.</remarks>
        public ApplicationDirectoryClient(string authUsername, string authPassword)
        {
            OnCreated();
            _authUsername = authUsername;
            _authPassword = authPassword;
        }
        #endregion

        #region Private Fields
        private bool _disposed = false;
        private bool _secureConnection = false;
        private string _secure = string.Empty;

        private string _authUsername = null;
        private string _authPassword = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the secure connection indicator.
        /// </summary>
        public bool SecureConnection
        {
            get
            {
                if (_secureConnection)
                    _secure = "S";
                else
                    _secure = string.Empty;

                return _secureConnection;
            }
            set
            {
                _secureConnection = value;

                if (_secureConnection)
                    _secure = "S";
                else
                    _secure = string.Empty;
            }
        }
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
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool AuthenticateUser(Security.UserCredentials userCredentials)
        {
            _authUsername = userCredentials.AuthorisationCredentials.Username;
            _authPassword = userCredentials.AuthorisationCredentials.Password;
            return AuthenticateUser(
                userCredentials.AuthorisationCredentials.Server,
                userCredentials.Username,
                userCredentials.Password,
                userCredentials.AuthorisationCredentials.ContainerDN,
                userCredentials.AuthorisationCredentials.SecureConnection);
        }

        /// <summary>
        /// Authenticate the user on the active directory server (AD LDS).
        /// </summary>
        /// <param name="server">The server name and port; port is optional [server[:port]].</param>
        /// <param name="username">The user name to authentication.</param>
        /// <param name="password">The password to authentication.</param>
        /// <param name="containerDN">The path distinguished name for the user name.</param>
        /// <param name="secureConnection">Is the connection to the server secure.</param>
        /// <returns>True if the user was authenticated else false.</returns>
        public bool AuthenticateUser(string server, string username, string password, string containerDN, bool secureConnection)
        {
            bool authenticated = false;

            // Attempt to make a connection to the active directory server.
            using (PrincipalContext context = new PrincipalContext(ContextType.ApplicationDirectory, server, containerDN, _authUsername, _authPassword))
            {
                // If the authentication requires a secure conenction.
                if (secureConnection)
                {
                    // Attempt to authenticated the user on the server
                    // for a secure connection.
                    authenticated = context.ValidateCredentials(username, password, ContextOptions.SecureSocketLayer);
                }
                else
                {
                    // Attempt to authenticated the user on the server
                    // for a simple binding connection.
                    authenticated = context.ValidateCredentials(username, password, ContextOptions.SimpleBind);

                    // If it fails then try a different connection
                    // context.
                    if (!authenticated)
                    {
                        // Attempt to authenticated the user on the server
                        // for a negotiate connection.
                        authenticated = context.ValidateCredentials(username, password, ContextOptions.Negotiate);
                    }
                }
            }

            // Return the result.
            return authenticated;
        }

        /// <summary>
        /// Get the list of group membership for the current user.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The username for the account.</param>
        /// <returns>The list of group distinguished names.</returns>
        public List<string> GetUserMembership(string ldapPath, string username)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));


            DirectoryEntry localMachine = null;
            List<string> membershipCol = new List<string>();

            try
            {
                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Create a new searcher and assign the query
                // to serach for, where user principle name is the username.
                DirectorySearcher searcher = new DirectorySearcher(localMachine);
                searcher.Filter = "userPrincipalName=" + username;
                searcher.PropertiesToLoad.Add("memberof");

                // Execute the seracher to fine the first result.
                SearchResult searchResult = searcher.FindOne();

                // get all the member of properties.
                ResultPropertyValueCollection values = searchResult.Properties["memberof"];

                // For each item found add to the membership collection.
                for (int i = 0; i < values.Count; i++)
                    membershipCol.Add(values[i].ToString());

                // Return the collection of group distinguished names
                return membershipCol;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();
            }
        }

        /// <summary>
        /// Gets the current user name.
        /// </summary>
        /// <param name="includeDomain">Should the domain be included.</param>
        /// <returns>The current user name.</returns>
        public string GetCurrentUser(bool includeDomain)
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
        /// Add the user account to the group.
        /// </summary>
        /// <param name="ldapPath">>The LDAP path to the domain controller.</param>
        /// <param name="userDN">The user account full path (DN = Distinguished Name).</param>
        /// <returns>True if the user account was created else false.</returns>
        public bool AddUserAccountToGroup(string ldapPath, string userDN)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(userDN))
                throw new System.ArgumentNullException("User DN can not be null.",
                    new System.Exception("A valid user DN should be specified."));

            DirectoryEntry localMachine = null;

            try
            {
                // Create a new directory entry
                // instance to the domain.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Remove the user account from the group
                localMachine.Properties["member"].Add(userDN);

                // Commit the changes for the account.
                localMachine.CommitChanges();

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();
            }
        }

        /// <summary>
        /// Remove the user account from the group.
        /// </summary>
        /// <param name="ldapPath">>The LDAP path to the domain controller.</param>
        /// <param name="userDN">The user account full path (DN = Distinguished Name).</param>
        /// <returns>True if the user account was created else false.</returns>
        public bool RemoveUserAccountFromGroup(string ldapPath, string userDN)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(userDN))
                throw new System.ArgumentNullException("User DN can not be null.",
                    new System.Exception("A valid user DN should be specified."));

            DirectoryEntry localMachine = null;

            try
            {
                // Create a new directory entry
                // instance to the domain.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Remove the user account from the group
                localMachine.Properties["member"].Remove(userDN);

                // Commit the changes for the account.
                localMachine.CommitChanges();

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();
            }
        }

        /// <summary>
        /// Assign the specified property value against the property name.
        /// </summary>
        /// <param name="ldapPath">>The LDAP path to the domain controller.</param>
        /// <param name="username">The username for the account.</param>
        /// <param name="propertyName">The property name to assign</param>
        /// <param name="propertyValue">The property value to set.</param>
        /// <returns>True if the user account was created else false.</returns>
        public bool AssignUserAccountProperty(string ldapPath, string username, string propertyName, object propertyValue)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(propertyName))
                throw new System.ArgumentNullException("Property name can not be null.",
                    new System.Exception("A valid property name should be specified."));

            if (propertyValue == null)
                throw new System.ArgumentNullException("Property value can not be null.",
                   new System.Exception("A valid property value should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Find the entry for the current user account.
                user = localMachine.Children.Find("CN=" + username, "user");

                // Set the password, description and full name for the account.
                user.Invoke("Put", new object[] { propertyName, propertyValue });

                // Commit the changes for the account.
                user.CommitChanges();

                // If the event has been attached
                // send to the client the data through
                // the delegate.
                if (OnAccountCreated != null)
                    OnAccountCreated(this, true);

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Creates a new user account on the domain controller.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The username for the account.</param>
        /// <param name="fullName">The full name for the account.</param>
        /// <param name="description">The description for the account.</param>
        /// <returns>True if the user account was created else false.</returns>
        public bool CreateUserAccount(string ldapPath, string username,
            string fullName, string description)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(fullName))
                throw new System.ArgumentNullException("Fullname can not be null.",
                    new System.Exception("A valid fullname should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(description))
                throw new System.ArgumentNullException("Description can not be null.",
                    new System.Exception("A valid description should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Add the user account to the user collection.
                user = localMachine.Children.Add("CN=" + username, "user");

                // Set the sam account name to the
                // usename specified.
                user.Properties["userPrincipalName"].Value = username;

                // Set the password, description and full name for the account.
                user.Invoke("Put", new object[] { "Description", description });
                user.Invoke("Put", new object[] { "DisplayName", fullName });

                // Commit the changes for the account.
                user.CommitChanges();

                // If the event has been attached
                // send to the client the data through
                // the delegate.
                if (OnAccountCreated != null)
                    OnAccountCreated(this, true);

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw ;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Creates a new user account on the domain controller.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The username for the account.</param>
        /// <param name="fullName">The full name for the account.</param>
        /// <param name="description">The description for the account.</param>
        /// <param name="emailAddress">The email address for the account.</param>
        /// <returns>True if the user account was created else false.</returns>
        public bool CreateUserAccount(string ldapPath, string username,
            string fullName, string description, string emailAddress)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(fullName))
                throw new System.ArgumentNullException("Fullname can not be null.",
                    new System.Exception("A valid fullname should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(description))
                throw new System.ArgumentNullException("Description can not be null.",
                    new System.Exception("A valid description should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(emailAddress))
                throw new System.ArgumentNullException("EmailAddress can not be null.",
                    new System.Exception("A valid email address should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Add the user account to the user collection.
                user = localMachine.Children.Add("CN=" + username, "user");

                // Set the sam account name to the
                // usename specified.
                user.Properties["userPrincipalName"].Value = username;

                // Set the password, description and full name for the account.
                user.Invoke("Put", new object[] { "Description", description });
                user.Invoke("Put", new object[] { "DisplayName", fullName });
                user.Invoke("Put", new object[] { "Mail", emailAddress });

                // Commit the changes for the account.
                user.CommitChanges();

                // If the event has been attached
                // send to the client the data through
                // the delegate.
                if (OnAccountCreated != null)
                    OnAccountCreated(this, true);

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Creates a new user account on the domain controller.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The username for the account.</param>
        /// <param name="cn">The common name of the account.</param>
        /// <param name="fullName">The full name for the account.</param>
        /// <param name="description">The description for the account.</param>
        /// <param name="emailAddress">The email address for the account.</param>
        /// <returns>True if the user account was created else false.</returns>
        public bool CreateUserAccount(string ldapPath, string username, string cn,
            string fullName, string description, string emailAddress)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(cn))
                throw new System.ArgumentNullException("CN can not be null.",
                    new System.Exception("A valid CN should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(fullName))
                throw new System.ArgumentNullException("Fullname can not be null.",
                    new System.Exception("A valid fullname should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(description))
                throw new System.ArgumentNullException("Description can not be null.",
                    new System.Exception("A valid description should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(emailAddress))
                throw new System.ArgumentNullException("EmailAddress can not be null.",
                    new System.Exception("A valid email address should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Add the user account to the user collection.
                user = localMachine.Children.Add("CN=" + cn, "user");

                // Set the sam account name to the
                // usename specified.
                user.Properties["userPrincipalName"].Value = username;

                // Set the password, description and full name for the account.
                user.Invoke("Put", new object[] { "Description", description });
                user.Invoke("Put", new object[] { "DisplayName", fullName });
                user.Invoke("Put", new object[] { "Mail", emailAddress });

                // Commit the changes for the account.
                user.CommitChanges();

                // If the event has been attached
                // send to the client the data through
                // the delegate.
                if (OnAccountCreated != null)
                    OnAccountCreated(this, true);

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Creates a new user account on the domain controller.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The username for the account.</param>
        /// <param name="usernamePropertyName">The property name to assign the username against.</param>
        /// <param name="fullName">The full name for the account.</param>
        /// <param name="fullNamePropertyName">The property name to assign the fill name against.</param>
        /// <param name="description">The description for the account.</param>
        /// <param name="descriptionPropertyName">The property name to assign the description against.</param>
        /// <param name="emailAddress">The email address for the account.</param>
        /// <param name="emailAddressPropertyName">The property name to assign the email address against.</param>
        /// <param name="attribute">The attribute within the current ldap path to assign against ( e.g. 'CN=').</param>
        /// <param name="schemaClassName">The schema class name to assign against ( e.g. 'user').</param>
        /// <returns>True if the user account was created else false.</returns>
        public bool CreateUserAccount(string ldapPath,
            string username, string usernamePropertyName,
            string fullName, string fullNamePropertyName,
            string description, string descriptionPropertyName,
            string emailAddress, string emailAddressPropertyName,
            string attribute, string schemaClassName)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(fullName))
                throw new System.ArgumentNullException("Fullname can not be null.",
                    new System.Exception("A valid fullname should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(description))
                throw new System.ArgumentNullException("Description can not be null.",
                    new System.Exception("A valid description should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(emailAddress))
                throw new System.ArgumentNullException("EmailAddress can not be null.",
                    new System.Exception("A valid email address should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Add the user account to the user collection.
                user = localMachine.Children.Add(attribute + username, schemaClassName);

                // Set the sam account name to the
                // usename specified.
                user.Properties[usernamePropertyName].Value = username;

                // Set the password, description and full name for the account.
                user.Invoke("Put", new object[] { descriptionPropertyName, description });
                user.Invoke("Put", new object[] { fullNamePropertyName, fullName });
                user.Invoke("Put", new object[] { emailAddressPropertyName, emailAddress });

                // Commit the changes for the account.
                user.CommitChanges();

                // If the event has been attached
                // send to the client the data through
                // the delegate.
                if (OnAccountCreated != null)
                    OnAccountCreated(this, true);

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Removes the specified user account from the domain controller.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The current user account name.</param>
        /// <returns>True if the account was removed else false.</returns>
        public bool RemoveUserAccount(string ldapPath, string username)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Find the entry for the current user account.
                user = localMachine.Children.Find("CN=" + username, "user");

                // Remove the user account.
                localMachine.Children.Remove(user);

                // If the event has been attached
                // send to the client the data through
                // the delegate.
                if (OnAccountRemoved != null)
                    OnAccountRemoved(this, true);

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Renames an existing account to the specified account name.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The current user account name.</param>
        /// <param name="newUsername">The name to change to.</param>
        /// <returns>True if user account name was changed.</returns>
        public bool RenameUserAccount(string ldapPath,
            string username, string newUsername)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(newUsername))
                throw new System.ArgumentNullException("New username can not be null.",
                    new System.Exception("A valid new username should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Find the entry for the current user account.
                user = localMachine.Children.Find("CN=" + username, "user");

                // Rename the current user account.
                user.Rename(newUsername);

                // Commit the changes for the account.
                user.CommitChanges();

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Sets the password of the specified user account.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The username of the account.</param>
        /// <param name="password">The password for the account.</param>
        /// <param name="useMustChangePassword">The user must change the password at next log on.</param>
        /// <param name="securePassword">Should a secure connection be used to set the password.</param>
        /// <param name="port">The port number to the server.</param>
        /// <returns>True if the password was set else false.</returns>
        public bool SetUserAccountPassword(string ldapPath,
            string username, string password, bool useMustChangePassword, bool securePassword, int port)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(password))
                throw new System.ArgumentNullException("Password can not be null.",
                    new System.Exception("A valid password should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
            const long ADS_OPTION_PASSWORD_METHOD = 7;

            const int ADS_PASSWORD_ENCODE_REQUIRE_SSL = 0;
            const int ADS_PASSWORD_ENCODE_CLEAR = 1;

            // Set authentication flags.
            // For non-secure connection, use LDAP port and
            //  ADS_USE_SIGNING |
            //  ADS_USE_SEALING |
            //  ADS_SECURE_AUTHENTICATION
            // For secure connection, use SSL port and
            //  ADS_USE_SSL | ADS_SECURE_AUTHENTICATION
            AuthenticationTypes authTypes;

            try
            {
                // If a secure connection is to be used or not
                // to make the connection.
                if (!securePassword)
                    authTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
                else
                    authTypes = AuthenticationTypes.SecureSocketsLayer | AuthenticationTypes.Secure;

                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword, authTypes);

                // Find the entry for the current user account.
                user = localMachine.Children.Find("CN=" + username, "user");

                //  Be aware that, for security, a password should
                //  not be entered in code, but should be obtained
                //  from the user interface.
                user.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_PORTNUMBER, port });

                // If a secure password is or is not required.
                if (!securePassword)
                    user.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR });
                else
                    user.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_REQUIRE_SSL });

                // Set the password for the account.
                // Unlock the account.
                user.Invoke("SetPassword", new object[] { password });

                // Should the user change the password.
                if (!useMustChangePassword)
                    user.Properties["LockOutTime"].Value = 0;

                // Commit the changes for the account.
                user.CommitChanges();

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Enable or disable the user account.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="username">The username for the account.</param>
        /// <param name="disable">Enable or disable the property.</param>
        /// <returns>True if the property was set else false.</returns>
        public bool EnableDisableUserAccount(string ldapPath,
            string username, bool disable)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(username))
                throw new System.ArgumentNullException("Username can not be null.",
                    new System.Exception("A valid username should be specified."));

            DirectoryEntry localMachine = null;
            DirectoryEntry user = null;

            try
            {
                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Find the entry for the current user account.
                user = localMachine.Children.Find("CN=" + username, "user");

                // Get the current user account disabled property
                // assign the status of the property as bool.
                object accountStatus = user.InvokeGet("msDS-UserAccountDisabled");
                bool status = true;
                if (accountStatus != null)
                    status = (bool)accountStatus;

                // If account should be disabled
                if (disable)
                {
                    // If the status is false.
                    if (!status)
                        // Disable the current user account,
                        // set the property to true.
                        user.InvokeSet("msDS-UserAccountDisabled", new object[] { true });
                }
                // Account should be enabled.
                else
                {
                    // If the status is true.
                    if (status)
                        // Enable the current user account,
                        // set the property to false.
                        user.InvokeSet("msDS-UserAccountDisabled", new object[] { false });
                }

                // Commit the changes for the account.
                user.CommitChanges();

                // Return true if all
                // operations completed.
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

                if (user != null)
                    // Close the entry to the local machine.
                    user.Close();
            }
        }

        /// <summary>
        /// Gets all users that are members of the group.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <param name="group">The local machine group.</param>
        /// <returns>The list of users that are members of the group.</returns>
        public List<DirectoryEntry> GetUserMembershipGroupAccount(string ldapPath, string group)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Validate the inputs.
            if (String.IsNullOrEmpty(group))
                throw new System.ArgumentNullException("Group can not be null.",
                    new System.Exception("A valid group should be specified."));

            // Get all the users on the local machine.
            // Create a new list entry instance.
            List<DirectoryEntry> userList = GetAllUserAccounts(ldapPath);
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
        /// Gets the collection of all users on the domain.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <returns>The list of all users.</returns>
        public List<DirectoryEntry> GetAllUserAccounts(string ldapPath)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            DirectoryEntry localMachine = null;

            try
            {
                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Create a new list collection instance
                // of directory entries.
                List<DirectoryEntry> list = new List<DirectoryEntry>();

                // Get all the currrent accounts on the local machine.
                DirectoryEntries users = localMachine.Children;

                // Get all the user accounts
                GetAllUserAccountsEx(users, list);

                // Return the collection of users.
                return list;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

            }
        }

        /// <summary>
        /// Gets the collection of all users on the domain.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <returns>The list of all users.</returns>
        public List<DirectoryEntryModel> GetAllUserAccountDetails(string ldapPath)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            // Create a new list collection instance
            // of directory entries.
            List<DirectoryEntryModel> list = new List<DirectoryEntryModel>();

            // Get all the current users.
            List<DirectoryEntry> users = GetAllUserAccounts(ldapPath);

            // For each user entry found add to the collection
            foreach (DirectoryEntry user in users)
            {
                try
                {
                    // Assign the user details to the instance.
                    DirectoryEntryModel item = new DirectoryEntryModel();

                    try
                    {
                        item.AccountUserName = user.Properties["userPrincipalName"].Value.ToString();
                    }
                    catch { item.AccountUserName = string.Empty; }
                    try
                    {
                        item.EmailAddress = user.Properties["mail"].Value.ToString();
                    }
                    catch { item.EmailAddress = string.Empty; }
                    try
                    {
                        item.Description = user.Properties["description"].Value.ToString();
                    }
                    catch { item.Description = string.Empty; }
                    try
                    {
                        item.FullName = user.Properties["displayName"].Value.ToString();
                    }
                    catch { item.FullName = string.Empty; }
                    try
                    {
                        item.FirstName = user.Properties["givenName"].Value.ToString();
                    }
                    catch { item.FirstName = string.Empty; }
                    try
                    {
                        item.LastName = user.Properties["lastName"].Value.ToString();
                    }
                    catch { item.LastName = string.Empty; }

                    // Add the item to the collection.
                    list.Add(item);
                }
                catch { }
            }

            // Return the collection of users.
            return list;
        }

        /// <summary>
        /// Gets the collection of all accounts on the domain.
        /// </summary>
        /// <param name="ldapPath">The LDAP path to the domain controller.</param>
        /// <returns>The list of all accounts.</returns>
        public List<DirectoryEntry> GetAllAccounts(string ldapPath)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ldapPath))
                throw new System.ArgumentNullException("LDAP path can not be null.",
                    new System.Exception("A valid LDAP path should be specified."));

            DirectoryEntry localMachine = null;

            try
            {
                // Create a new directory entry
                // instance to the domain machine.
                localMachine = new DirectoryEntry(
                    "LDAP" + _secure + "://" + ldapPath, _authUsername, _authPassword);

                // Create a new list collection instance
                // of directory entries.
                List<DirectoryEntry> list = new List<DirectoryEntry>();

                // Get all the currrent accounts on the local machine.
                DirectoryEntries users = localMachine.Children;

                // Get the enumerator of the account collection.
                System.Collections.IEnumerator col = users.GetEnumerator();

                // For each account found.
                while (col.MoveNext())
                    list.Add((DirectoryEntry)col.Current);

                // Return the collection of users.
                return list;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localMachine != null)
                    // Close the entry to the local machine.
                    localMachine.Close();

            }
        }

        /// <summary>
        /// Gets the collection of all users on the domain.
        /// </summary>
        /// <param name="entries">The current directory entries for the account</param>
        /// <param name="list">The list of entries for all user accounts</param>
        private void GetAllUserAccountsEx(DirectoryEntries entries, List<DirectoryEntry> list)
        {
            // Get the enumerator of the account collection.
            System.Collections.IEnumerator col = entries.GetEnumerator();

            // For each account found.
            while (col.MoveNext())
            {
                // If the current schema class is 'user' then add
                // the entry to the collection, that is, if the current
                // account is of type 'user' then add to collection.
                if (((DirectoryEntry)col.Current).SchemaClassName.ToLower() == "user")
                    list.Add((DirectoryEntry)col.Current);
                else
                {
                    // Get the current entry
                    DirectoryEntry current = (DirectoryEntry)col.Current;

                    // Get all the currrent accounts on the local machine.
                    DirectoryEntries users = current.Children;

                    // Iterate throught the current entry.
                    GetAllUserAccountsEx(users, list);
                }
            }

            // Reset the current enum.
            col.Reset();
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
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

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
        ~ApplicationDirectoryClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
