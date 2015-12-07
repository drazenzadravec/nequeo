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
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
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

namespace Nequeo.DataAccess.ApplicationLogin
{
    /// <summary>
    /// Application login membership provider.
    /// </summary>
    [Export(typeof(Nequeo.Data.DataType.IMembershipProvider))]
    [ContentMetadata(Name = "ApplicationLoginMembershipProvider", Index = 0)]
    public class MembershipProvider : Nequeo.Data.DataType.IMembershipProvider
    {
        private string _providerName = string.Empty;
        private string _applicationName = string.Empty;
        private bool _enablePasswordReset = false;
        private bool _enablePasswordRetrieval = false;
        private int _maxInvalidPasswordAttempts = 0;
        private int _minRequiredNonAlphanumericCharacters = 0;
        private int _minRequiredPasswordLength = 0;
        private int _passwordAttemptWindow = 0;
        private System.Web.Security.MembershipPasswordFormat _passwordFormat = System.Web.Security.MembershipPasswordFormat.Clear;
        private string _passwordStrengthRegularExpression = string.Empty;
        private bool _requiresQuestionAndAnswer = false;
        private bool _requiresUniqueEmail = false;

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
        /// Gets enable password reset.
        /// </summary>
        public bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
            set { _enablePasswordReset = value; }
        }

        /// <summary>
        /// Gets enable password retrieval.
        /// </summary>
        public bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
            set { _enablePasswordRetrieval = value; }
        }

        /// <summary>
        /// Gets maximum invalid password attempts.
        /// </summary>
        public int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
            set { _maxInvalidPasswordAttempts = value; }
        }

        /// <summary>
        /// Gets minimum required non alphanumeric characters
        /// </summary>
        public int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
            set { _minRequiredNonAlphanumericCharacters = value; }
        }

        /// <summary>
        /// Gets minimum required password length.
        /// </summary>
        public int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
            set { _minRequiredPasswordLength = value; }
        }

        /// <summary>
        /// Gets password attempt window.
        /// </summary>
        public int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
            set { _passwordAttemptWindow = value; }
        }

        /// <summary>
        /// Gets password format.
        /// </summary>
        public System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
            set { _passwordFormat = value; }
        }

        /// <summary>
        /// Gets password strength regular expression.
        /// </summary>
        public string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
            set { _passwordStrengthRegularExpression = value; }
        }

        /// <summary>
        /// Gets requires question and answer.
        /// </summary>
        public bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
            set { _requiresQuestionAndAnswer = value; }
        }

        /// <summary>
        /// Gets requires unique email.
        /// </summary>
        public bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
            set { _requiresUniqueEmail = value; }
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if complete; else false.</returns>
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            // Validate the user.
            if (!ValidateUser(username, oldPassword))
                return false;

            bool ret = false;

            // Attempt to validate the user.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(username);

            // If user exists.
            if (user != null)
            {
                // Update the question and answer.
                ret = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().
                    Update.UpdateItemPredicate(
                        new Data.User()
                        {
                            LoginPassword = newPassword,
                            ModifiedDate = DateTime.Now
                        }, u =>
                            (u.LoginUsername == username)
                    );
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Change password question and answer.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="newPasswordQuestion">The new password question.</param>
        /// <param name="newPasswordAnswer">The new password question.</param>
        /// <returns>True if complete; else false.</returns>
        public bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            bool isValid = false;

            // Attempt to validate the user.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(username);

            // User maybe suspended (LockedOut).
            if (user != null)
            {
                // If user is suspended.
                if (user.UserSuspended)
                    isValid = false;
                else
                {
                    // Check the password format.
                    if (CheckPassword(password, user.LoginPassword))
                    {
                        // User is valid.
                        isValid = true;

                        // Update the user data.
                        user.ModifiedDate = DateTime.Now;
                        new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().Update.UpdateItem(user);
                    }
                }
            }

            // Return true if valid else false.
            return isValid;
        }

        /// <summary>
        /// Create user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email address.</param>
        /// <param name="passwordQuestion">The password question.</param>
        /// <param name="passwordAnswer">The password answer.</param>
        /// <param name="isApproved">Is approved.</param>
        /// <param name="providerUserKey">The provider key.</param>
        /// <param name="status">The status.</param>
        /// <returns>The membership user.</returns>
        public System.Web.Security.MembershipUser CreateUser(string username, string password, string email, 
            string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            // Attemtp to get a duplicate email address.
            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                // Duplicate email found.
                status = System.Web.Security.MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            // Attempt to get a use with the same username.
            System.Web.Security.MembershipUser membershipUser = GetUser(username, false);

            // If no user exists then no duplicates.
            if (membershipUser == null)
            {
                DateTime createdDate = DateTime.Now;
                long userAddressID = 0;

                // Create a new address.
                Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress userAddress = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress();
                Data.UserAddress userAddressData = Data.UserAddress.CreateUserAddress(username, username, 0, false);
                userAddressData.EmailAddress = email;

                // Insert the user address.
                List<object> identities = userAddress.Insert.InsertDataEntity(userAddressData);
                if (identities != null && identities.Count > 0)
                {
                    // Get the user address id.
                    userAddressID = (long)identities[0];
                }

                // Create the new user.
                Nequeo.DataAccess.ApplicationLogin.Data.Extension.User user = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User();
                Data.User userDate =
                    Data.User.CreateUser(
                            password,
                            username,
                            userAddressID,
                            0,
                            false);

                // Attemtp to insert the new item.
                bool ret = user.Insert.InsertItem(userDate);

                // Set the status of the create user operation.
                if (ret)
                    status = System.Web.Security.MembershipCreateStatus.Success;
                else
                    status = System.Web.Security.MembershipCreateStatus.UserRejected;

                // Return the created user.
                return GetUser(username, false);
            }
            else
            {
                // Duplicate username.
                status = System.Web.Security.MembershipCreateStatus.DuplicateUserName;
                return null;
            }
        }

        /// <summary>
        /// Delete the user
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="deleteAllRelatedData">Delete all related data.</param>
        /// <returns>True if complete; else false.</returns>
        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            bool ret = false;

            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(username);

            try
            {
                // Attempt to delete the user.
                ret = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().
                    Delete.DeleteItemPredicate(
                        u =>
                            (u.LoginUsername == username)
                    );

                // Delete any extra data.
                if (deleteAllRelatedData)
                {
                    // Attempt to delete the user address.
                    ret = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress().
                        Delete.DeleteItemPredicate(
                            u =>
                                (u.UserAddressID == user.UserAddressID)
                            );
                }
            }
            catch { }

            // Return the result of the deletion.
            return ret;
        }

        /// <summary>
        /// Find users by email.
        /// </summary>
        /// <param name="emailToMatch">The email to match.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        public System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            System.Web.Security.MembershipUserCollection memShipUsers = new System.Web.Security.MembershipUserCollection();
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.User user = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User();
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress userAddress = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress();

            // Get all the users for the match.
            long usersMatched = userAddress.Select.
                GetRecordCount(
                    u =>
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.EmailAddress, ("%" + emailToMatch + "%")))
                    );

            // Get the total number of uses.
            totalRecords = Int32.Parse(usersMatched.ToString());
            int skipNumber = (pageIndex * pageSize);
            DateTime createdDate = DateTime.Now;

            // Get the current set on data.
            IQueryable<Data.UserAddress> users = userAddress.Select.QueryableProvider().
                Where(u =>
                    (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.EmailAddress, ("%" + emailToMatch + "%")))).
                OrderBy(u => u.UserAddressID).
                Take(pageSize).
                Skip(skipNumber);

            // For each user found.
            foreach (Data.UserAddress item in users)
            {
                // Get the current users address details
                Data.User userData = user.Select.SelectDataEntity(u => u.UserAddressID == item.UserAddressID);

                // Create the membership user.
                System.Web.Security.MembershipUser memShipUser =
                    new System.Web.Security.MembershipUser(
                        ProviderName,
                        userData.LoginUsername,
                        userData.UserID,
                        item.EmailAddress,
                        "",
                        item.Comments,
                        true,
                        userData.UserSuspended,
                        createdDate,
                        createdDate,
                        createdDate,
                        createdDate,
                        createdDate);

                // Add the user to the collection.
                memShipUsers.Add(memShipUser);
            }

            // Return the collection of membership users.
            return memShipUsers;
        }

        /// <summary>
        /// Find users by name.
        /// </summary>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        public System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            System.Web.Security.MembershipUserCollection memShipUsers = new System.Web.Security.MembershipUserCollection();
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.User user = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User();
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress userAddress = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress();

            // Get all the users for the match.
            long usersMatched = user.Select.
                GetRecordCount(
                    u =>
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.LoginUsername, ("%" + usernameToMatch + "%")))
                    );

            // Get the total number of uses.
            totalRecords = Int32.Parse(usersMatched.ToString());
            int skipNumber = (pageIndex * pageSize);
            DateTime createdDate = DateTime.Now;

            // Get the current set on data.
            IQueryable<Data.User> users = user.Select.QueryableProvider().
                Where(u =>
                    (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.LoginUsername, ("%" + usernameToMatch + "%")))).
                OrderBy(u => u.UserID).
                Take(pageSize).
                Skip(skipNumber);

            // For each user found.
            foreach (Data.User item in users)
            {
                // Get the current users address details
                Data.UserAddress address = userAddress.Select.SelectDataEntity(u => u.UserAddressID == item.UserAddressID);

                // Create the membership user.
                System.Web.Security.MembershipUser memShipUser =
                    new System.Web.Security.MembershipUser(
                        ProviderName,
                        item.LoginUsername,
                        item.UserID,
                        address.EmailAddress,
                        "",
                        item.Comments,
                        true,
                        item.UserSuspended,
                        createdDate,
                        createdDate,
                        createdDate,
                        createdDate,
                        createdDate);

                // Add the user to the collection.
                memShipUsers.Add(memShipUser);
            }

            // Return the collection of membership users.
            return memShipUsers;
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        public System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            System.Web.Security.MembershipUserCollection memShipUsers = new System.Web.Security.MembershipUserCollection();
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.User user = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User();
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress userAddress = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress();

            // Get all the users for the match.
            long usersMatched = user.Select.GetRecordCount();
                    

            // Get the total number of uses.
            totalRecords = Int32.Parse(usersMatched.ToString());
            int skipNumber = (pageIndex * pageSize);
            DateTime createdDate = DateTime.Now;

            // Get the current set on data.
            IQueryable<Data.User> users = user.Select.QueryableProvider().
                OrderBy(u => u.UserID).
                Take(pageSize).
                Skip(skipNumber);

            // For each user found.
            foreach (Data.User item in users)
            {
                // Get the current users address details
                Data.UserAddress address = userAddress.Select.SelectDataEntity(u => u.UserAddressID == item.UserAddressID);

                // Create the membership user.
                System.Web.Security.MembershipUser memShipUser =
                    new System.Web.Security.MembershipUser(
                        ProviderName,
                        item.LoginUsername,
                        item.UserID,
                        address.EmailAddress,
                        "",
                        item.Comments,
                        true,
                        item.UserSuspended,
                        createdDate,
                        createdDate,
                        createdDate,
                        createdDate,
                        createdDate);

                // Add the user to the collection.
                memShipUsers.Add(memShipUser);
            }

            // Return the collection of membership users.
            return memShipUsers;
        }

        /// <summary>
        /// Get the number of users online.
        /// </summary>
        /// <returns>The number of users online.</returns>
        public int GetNumberOfUsersOnline()
        {
            // Get the window time interval.
            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            // Get the users online.
            long usersOnline = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().Select.
                GetRecordCount(
                    u =>
                        (u.ModifiedDate > compareTime)
                    );

            // Return the number of users online.
            return Int32.Parse(usersOnline.ToString());
        }

        /// <summary>
        /// Get password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="answer">The answer.</param>
        /// <returns>The password.</returns>
        public string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
                throw new Exception("Password Retrieval Not Enabled.");

            if (PasswordFormat == System.Web.Security.MembershipPasswordFormat.Hashed)
                throw new Exception("Cannot retrieve Hashed passwords.");

            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(username);

            if (user == null)
                throw new NotSupportedException("The supplied user name has not been found.");

            // Assing the password data.
            string password = user.LoginPassword;

            // Unencode the password.
            if (PasswordFormat == System.Web.Security.MembershipPasswordFormat.Encrypted)
                password = UnEncodePassword("", password);

            // Return the password.
            return password;
        }

        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="providerUserKey">Provider user key.</param>
        /// <param name="userIsOnline">Is the user online.</param>
        /// <returns>The membership user.</returns>
        public System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            System.Web.Security.MembershipUser memShipUser = null;
            DateTime createdDate = DateTime.Now;

            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(long.Parse(providerUserKey.ToString()));
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress userAddress = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress();

            // Make sure that the user exists.
            if (user != null)
            {
                // Get the current users address details
                Data.UserAddress address = userAddress.Select.SelectDataEntity(u => u.UserAddressID == user.UserAddressID);

                // Create the membership user.
                memShipUser = new System.Web.Security.MembershipUser(
                    ProviderName,
                    user.LoginUsername,
                    user.UserID,
                    address.EmailAddress,
                    "",
                    user.Comments,
                    true,
                    user.UserSuspended,
                    createdDate,
                    createdDate,
                    createdDate,
                    createdDate,
                    createdDate);

                // If user is on line.
                if (userIsOnline)
                {
                    user.ModifiedDate = createdDate;
                    bool ret = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().Update.UpdateItem(user);
                }
            }

            // Return the membership user.
            return memShipUser;
        }

        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userIsOnline">Is the user online.</param>
        /// <returns>The membership user.</returns>
        public System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            System.Web.Security.MembershipUser memShipUser = null;
            DateTime createdDate = DateTime.Now;

            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(username);
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress userAddress = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress();

            // Make sure that the user exists.
            if (user != null)
            {
                // Get the current users address details
                Data.UserAddress address = userAddress.Select.SelectDataEntity(u => u.UserAddressID == user.UserAddressID);

                // Create the membership user.
                memShipUser = new System.Web.Security.MembershipUser(
                    ProviderName,
                    username,
                    user.UserID,
                    address.EmailAddress,
                    "",
                    user.Comments,
                    true,
                    user.UserSuspended,
                    createdDate,
                    createdDate,
                    createdDate,
                    createdDate,
                    createdDate);

                // If user is on line.
                if (userIsOnline)
                {
                    user.ModifiedDate = createdDate;
                    bool ret = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().Update.UpdateItem(user);
                }
            }

            // Return the membership user.
            return memShipUser;
        }

        /// <summary>
        /// Get username by email.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>The username; else empty string.</returns>
        public string GetUserNameByEmail(string email)
        {
            string username = string.Empty;

            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.User user = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User();
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress userAddress = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.UserAddress();
            Nequeo.DataAccess.ApplicationLogin.Data.UserAddress address = userAddress.Select.SelectDataEntity(u => u.EmailAddress == email);
            Nequeo.DataAccess.ApplicationLogin.Data.User userData = user.Select.SelectDataEntity(u => u.UserAddressID == address.UserAddressID);
                
            // Return the username.
            if (userData != null)
                username = userData.LoginUsername;
                    
            // Return an empty string.
            if (userData == null)
                username = string.Empty;

            // Return the username.
            return username;
        }

        /// <summary>
        /// Reset password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="answer">The answer.</param>
        /// <returns>The new password.</returns>
        public string ResetPassword(string username, string answer)
        {
            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(username);

            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            if (user == null)
                throw new NotSupportedException("The supplied user name has not been found.");

            // Generate the new password.
            string newPassword = System.Web.Security.Membership.GeneratePassword(MinRequiredPasswordLength, MinRequiredNonAlphanumericCharacters);

            // Update the password.
            user.LoginPassword = newPassword;
            user.ModifiedDate = DateTime.Now;
            bool ret = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().Update.UpdateItem(user);

            // Return the password.
            if (ret)
                return newPassword;
            else
                throw new Exception("User not found, or user is locked out. Password not Reset.");
        }

        /// <summary>
        /// Unlock the user.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <returns>True if complete; else false.</returns>
        public bool UnlockUser(string userName)
        {
            bool ret = false;

            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(userName);

            // Update the user.
            if (user != null)
            {
                user.UserSuspended = false;
                ret = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().Update.UpdateItem(user);
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Update the user.
        /// </summary>
        /// <param name="user">The membership user.</param>
        public void UpdateUser(System.Web.Security.MembershipUser user)
        {
            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.User userData = GetSpecificUser(user.UserName);

            // Update the user.
            if (user != null)
            {
                new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().
                    Update.UpdateItemPredicate(
                        new Data.User()
                        {
                            LoginPassword = userData.LoginPassword,
                            ModifiedDate = user.LastLoginDate,
                            UserSuspended = user.IsLockedOut
                        }, u =>
                            (u.LoginUsername == user.UserName)
                    );
            }
        }

        /// <summary>
        /// Validate the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if complete; else false.</returns>
        public bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            // Attempt to validate the user.
            Nequeo.DataAccess.ApplicationLogin.Data.User user = GetSpecificUser(username);

            // User maybe suspended (LockedOut).
            if (user != null)
            {
                // If user is suspended.
                if (user.UserSuspended)
                    isValid = false;
                else
                {
                    // Check the password format.
                    if (CheckPassword(password, user.LoginPassword))
                    {
                        // User is valid.
                        isValid = true;

                        // Update the user data.
                        user.ModifiedDate = DateTime.Now;
                        new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User().Update.UpdateItem(user);
                    }
                }
            }

            // Return true if valid else false.
            return isValid;
        }

        /// <summary>
        /// Get the specific user for the current application.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user; else null.</returns>
        private Nequeo.DataAccess.ApplicationLogin.Data.User GetSpecificUser(string username)
        {
            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.User userExt = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User();
            Nequeo.DataAccess.ApplicationLogin.Data.User user =
                userExt.Select.SelectDataEntity(
                    u =>
                        (u.LoginUsername.ToLower() == username.ToLower())
                );

            // Return the user.
            return user;
        }

        /// <summary>
        /// Get the specific user for the current application.
        /// </summary>
        /// <param name="userID">The userid.</param>
        /// <returns>The user; else null.</returns>
        private Nequeo.DataAccess.ApplicationLogin.Data.User GetSpecificUser(long userID)
        {
            // Get the user data.
            Nequeo.DataAccess.ApplicationLogin.Data.Extension.User userExt = new Nequeo.DataAccess.ApplicationLogin.Data.Extension.User();
            Nequeo.DataAccess.ApplicationLogin.Data.User user =
                userExt.Select.SelectDataEntity(
                    u =>
                        (u.UserID == userID)
                );

            // Return the user.
            return user;
        }

        /// <summary>
        /// Check the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="encodedPassword">The encoded password.</param>
        /// <returns>True if the passwords match;else false.</returns>
        private bool CheckPassword(string password, string encodedPassword)
        {
            string pass = password;
            string passCheck = encodedPassword;

            // Switch to the correct password format.
            switch (PasswordFormat)
            {
                case System.Web.Security.MembershipPasswordFormat.Encrypted:
                    passCheck = UnEncodePassword(password, encodedPassword);
                    break;

                case System.Web.Security.MembershipPasswordFormat.Hashed:
                    passCheck = UnEncodePassword(password, encodedPassword);
                    break;

                default:
                    break;
            }

            // If the two passwords match
            // then return true.
            if (pass == passCheck)
                return true;

            return false;
        }

        /// <summary>
        /// Encode the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The encoded password.</returns>
        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            // Select the apprepriate password format.
            switch (PasswordFormat)
            {
                case System.Web.Security.MembershipPasswordFormat.Clear:
                    break;

                case System.Web.Security.MembershipPasswordFormat.Encrypted:
                    // Encrypt the password.
                    Nequeo.Cryptography.IPasswordEncryption encoder = PasswordAuthorisationCode.GetEncoder();
                    encodedPassword = encoder.Encode(password, Cryptography.PasswordFormat.Encrypted);
                    break;

                case System.Web.Security.MembershipPasswordFormat.Hashed:
                    // Encode the password.
                    Nequeo.Cryptography.IPasswordEncryption encoderHashed = PasswordAuthorisationCode.GetEncoder();
                    encodedPassword = encoderHashed.Encode(password, Cryptography.PasswordFormat.Hashed);
                    break;

                default:
                    throw new Exception("Unsupported password format.");
            }

            // Return the encoded password.
            return encodedPassword;
        }

        /// <summary>
        /// Unencode the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="encodedPassword">The encoded password.</param>
        /// <returns>The un-encoded password.</returns>
        private string UnEncodePassword(string password, string encodedPassword)
        {
            string pass = encodedPassword;

            // Select the apprepriate password format.
            switch (PasswordFormat)
            {
                case System.Web.Security.MembershipPasswordFormat.Clear:
                    break;

                case System.Web.Security.MembershipPasswordFormat.Encrypted:
                    // Decrypt the encrypted password.
                    Nequeo.Cryptography.IPasswordEncryption encoder = PasswordAuthorisationCode.GetEncoder();
                    pass = encoder.Decode(encodedPassword, Cryptography.PasswordFormat.Encrypted);
                    break;

                case System.Web.Security.MembershipPasswordFormat.Hashed:
                    // Validate with hash
                    Nequeo.Cryptography.IPasswordEncryption encoderHash = PasswordAuthorisationCode.GetEncoder();
                    pass = encoderHash.Decode(encodedPassword, Cryptography.PasswordFormat.Hashed, password);
                    break;

                default:
                    throw new Exception("Unsupported password format.");
            }

            // Return the un-encoded password.
            return pass;
        }
    }
}
