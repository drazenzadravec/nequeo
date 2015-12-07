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
using Nequeo.Web;

namespace Nequeo.DataAccess.CloudInteraction
{
    /// <summary>
    /// Cloud Interaction membership provider base.
    /// </summary>
    public class MembershipProviderBase
    {
        /// <summary>
        /// Gets the current MembershipProvider context
        /// </summary>
        public System.Web.Security.MembershipProvider Context
        {
            get
            {
                // Create the membership provider instance.
                Nequeo.Web.Provider.DataBaseMembershipProvider provider = new Web.Provider.DataBaseMembershipProvider();
                Nequeo.Data.DataType.IMembershipProvider cloudProvider = new MembershipProvider();

                // Assign the current cloud membership provider instance.
                provider.MembershipProviderTypeInstance = cloudProvider;
                return provider;
            }
        }   
    }

    /// <summary>
    /// Cloud Interaction membership provider.
    /// </summary>
    [Export(typeof(Nequeo.Data.DataType.IMembershipProvider))]
    [ContentMetadata(Name = "CloudInteractionMembershipProvider", Index = 0)]
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

            // Get the user data.
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(username);

            // User exists.
            if (user != null)
            {
                // Update the question and answer.
                ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().
                    Update.UpdateItemPredicate(
                        new Data.User()
                        {
                            Password = newPassword,
                            PasswordAnswer = user.PasswordAnswer,
                            LastPasswordChangedDate = DateTime.Now
                        }, u =>
                            (u.Username == username) &&
                            (u.ApplicationName == ApplicationName)
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
            // Validate the user.
            if (!ValidateUser(username, password))
                return false;

            bool ret = false;

            // Get the user data.
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(username);

            // User exists.
            if (user != null)
            {
                // Update the question and answer.
                ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().
                    Update.UpdateItemPredicate(
                        new Data.User()
                        {
                            Password = user.Password,
                            PasswordQuestion = newPasswordQuestion,
                            PasswordAnswer = newPasswordAnswer
                        }, u =>
                            (u.Username == username) &&
                            (u.ApplicationName == ApplicationName)
                    );
            }

            // Return the result.
            return ret;
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

                // Create the new user.
                Nequeo.DataAccess.CloudInteraction.Data.Extension.User user = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();
                Data.User userDate =
                    Data.User.CreateUser(
                            0,
                            ApplicationName,
                            createdDate,
                            email,
                            0,
                            createdDate,
                            0,
                            createdDate,
                            isApproved,
                            createdDate,
                            createdDate,
                            createdDate,
                            false,
                            password,
                            0,
                            username,
                            false,
                            createdDate);

                // Assign extra properties
                userDate.PasswordQuestion = passwordQuestion;
                userDate.PasswordAnswer = passwordAnswer;

                // Attemtp to insert the new item.
                bool ret = user.Insert.InsertItem(userDate);

                // Set the status of the create user operation.
                if(ret)
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
            // Attempt to delete the user.
            bool ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().
                Delete.DeleteItemPredicate(
                    u =>
                        (u.Username == username) &&
                        (u.ApplicationName == ApplicationName)
                );

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
            Nequeo.DataAccess.CloudInteraction.Data.Extension.User user = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();

            // Get all the users for the match.
            long usersMatched = user.Select.
                GetRecordCount(
                    u =>
                        (u.ApplicationName == ApplicationName) &&
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Email, ("%" + emailToMatch + "%")))
                    );

            // Get the total number of uses.
            totalRecords = Int32.Parse(usersMatched.ToString());
            int skipNumber = (pageIndex * pageSize);

            // Get the current set on data.
            IQueryable<Data.User> users = user.Select.QueryableProvider().
                Where(u =>
                    (u.ApplicationName == ApplicationName) &&
                    (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Email, ("%" + emailToMatch + "%")))).
                OrderBy(u => u.Username).
                Take(pageSize).
                Skip(skipNumber);

            // For each user found.
            foreach (Data.User item in users)
            {
                // Create the membership user.
                System.Web.Security.MembershipUser memShipUser =
                    new System.Web.Security.MembershipUser(
                        ProviderName,
                        item.Username,
                        item.UserID,
                        item.Email,
                        item.PasswordQuestion,
                        item.Comments,
                        item.IsApproved,
                        item.UserSuspended,
                        item.CreationDate,
                        item.LastLoginDate,
                        item.LastActivityDate,
                        item.LastPasswordChangedDate,
                        item.UserSuspendedDate);

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
            Nequeo.DataAccess.CloudInteraction.Data.Extension.User user = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();

            // Get all the users for the match.
            long usersMatched = user.Select.
                GetRecordCount(
                    u =>
                        (u.ApplicationName == ApplicationName) &&
                        (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))
                    );

            // Get the total number of uses.
            totalRecords = Int32.Parse(usersMatched.ToString());
            int skipNumber = (pageIndex * pageSize);

            // Get the current set on data.
            IQueryable<Data.User> users = user.Select.QueryableProvider().
                Where(u =>
                    (u.ApplicationName == ApplicationName) &&
                    (Nequeo.Data.TypeExtenders.SqlQueryMethods.Like(u.Username, ("%" + usernameToMatch + "%")))).
                OrderBy(u => u.Username).
                Take(pageSize).
                Skip(skipNumber);

            // For each user found.
            foreach (Data.User item in users)
            {
                // Create the membership user.
                System.Web.Security.MembershipUser memShipUser = 
                    new System.Web.Security.MembershipUser(
                        ProviderName,
                        item.Username,
                        item.UserID,
                        item.Email,
                        item.PasswordQuestion,
                        item.Comments,
                        item.IsApproved,
                        item.UserSuspended,
                        item.CreationDate,
                        item.LastLoginDate,
                        item.LastActivityDate,
                        item.LastPasswordChangedDate,
                        item.UserSuspendedDate);

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
            Nequeo.DataAccess.CloudInteraction.Data.Extension.User user = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User();

            // Get all the users for the match.
            long usersMatched = user.Select.
                GetRecordCount(
                    u =>
                        (u.ApplicationName == ApplicationName)
                    );

            // Get the total number of uses.
            totalRecords = Int32.Parse(usersMatched.ToString());
            int skipNumber = (pageIndex * pageSize);

            // Get the current set on data.
            IQueryable<Data.User> users = user.Select.QueryableProvider().
                Where(u =>
                    (u.ApplicationName == ApplicationName)).
                OrderBy(u => u.Username).
                Take(pageSize).
                Skip(skipNumber);

            // For each user found.
            foreach (Data.User item in users)
            {
                // Create the membership user.
                System.Web.Security.MembershipUser memShipUser =
                    new System.Web.Security.MembershipUser(
                        ProviderName,
                        item.Username,
                        item.UserID,
                        item.Email,
                        item.PasswordQuestion,
                        item.Comments,
                        item.IsApproved,
                        item.UserSuspended,
                        item.CreationDate,
                        item.LastLoginDate,
                        item.LastActivityDate,
                        item.LastPasswordChangedDate,
                        item.UserSuspendedDate);

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
            long usersOnline = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Select.
                GetRecordCount(
                    u => 
                        (u.ApplicationName == ApplicationName) && 
                        (u.LastActivityDate > compareTime)
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
            string password = "";
            string passwordAnswer = "";

            if (!EnablePasswordRetrieval)
                throw new Exception("Password Retrieval Not Enabled.");

            if (PasswordFormat == System.Web.Security.MembershipPasswordFormat.Hashed)
                throw new Exception("Cannot retrieve Hashed passwords.");

            // Get the user data.
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(username);

            if (user == null)
                throw new NotSupportedException("The supplied user name has not been found.");

            // Assing the password data.
            password = user.Password;
            passwordAnswer = user.PasswordAnswer;

            // If a password answer is required.
            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer", user);
                throw new Exception("Incorrect password answer.");
            }

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

            // Get the user data.
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(long.Parse(providerUserKey.ToString()));

            // Make sure that the user exists.
            if (user != null)
            {
                // Create the membership user.
                memShipUser = new System.Web.Security.MembershipUser(
                    ProviderName,
                    user.Username,
                    user.UserID,
                    user.Email,
                    user.PasswordQuestion,
                    user.Comments,
                    user.IsApproved,
                    user.UserSuspended,
                    user.CreationDate,
                    user.LastLoginDate,
                    user.LastActivityDate,
                    user.LastPasswordChangedDate,
                    user.UserSuspendedDate);

                // If user is on line.
                if (userIsOnline)
                {
                    user.LastActivityDate = DateTime.Now;
                    bool ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
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

            // Get the user data.
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(username);

            // Make sure that the user exists.
            if (user != null)
            {
                // Create the membership user.
                memShipUser = new System.Web.Security.MembershipUser(
                    ProviderName,
                    username,
                    user.UserID,
                    user.Email,
                    user.PasswordQuestion,
                    user.Comments,
                    user.IsApproved,
                    user.UserSuspended,
                    user.CreationDate,
                    user.LastLoginDate,
                    user.LastActivityDate,
                    user.LastPasswordChangedDate,
                    user.UserSuspendedDate);

                // If user is on line.
                if (userIsOnline)
                {
                    user.LastActivityDate = DateTime.Now;
                    bool ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
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

            // Get the users.
            Nequeo.DataAccess.CloudInteraction.Data.User[] users = 
                new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Select.
                    SelectDataEntitiesPredicate(u => 
                        (u.Email == email) && 
                        (u.ApplicationName == ApplicationName)
                    );

            // Return the username.
            if (users != null)
                if (users.Count() > 0)
                    username = users.First().Username;

            // Return an empty string.
            if (users == null || users.Count() < 1)
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
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(username);

            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            if (user == null)
                throw new NotSupportedException("The supplied user name has not been found.");

            // If a password answer is required.
            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer", user);
                throw new Exception("Password answer required for password reset.");
            }

            // Generate the new password.
            string newPassword = System.Web.Security.Membership.GeneratePassword(MinRequiredPasswordLength, MinRequiredNonAlphanumericCharacters);

            // If a password answer is required.
            if (RequiresQuestionAndAnswer && !CheckPassword(answer, user.PasswordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer", user);
                throw new Exception("Incorrect password answer.");
            }

            // Update the password.
            user.Password = newPassword;
            user.LastPasswordChangedDate = DateTime.Now;
            bool ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);

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
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(userName);

            // Update the user.
            if (user != null)
            {
                user.UserSuspended = false;
                ret = new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
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
            Nequeo.DataAccess.CloudInteraction.Data.User userData = GetSpecificUser(user.UserName);

            // Update the user.
            if (user != null)
            {
                new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().
                    Update.UpdateItemPredicate(
                        new Data.User()
                        {
                            Password = userData.Password,
                            PasswordAnswer = userData.PasswordAnswer,
                            Email = user.Email,
                            LastLoginDate = user.LastLoginDate,
                            LoggedIn = user.IsOnline,
                            UserSuspended = user.IsLockedOut,
                            LastActivityDate = user.LastActivityDate,
                            PasswordQuestion = user.PasswordQuestion,
                            LastPasswordChangedDate = user.LastPasswordChangedDate,
                            UserSuspendedDate = user.LastLockoutDate,
                            Comments = user.Comment

                        }, u =>
                            (u.Username == user.UserName) &&
                            (u.ApplicationName == ApplicationName)
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
            Nequeo.DataAccess.CloudInteraction.Data.User user = GetSpecificUser(username);

            // User maybe suspended (LockedOut).
            if (user != null)
            {
                // If user is suspended.
                if (user.UserSuspended)
                    isValid = false;
                else
                {
                    // Check the password format.
                    if (CheckPassword(password, user.Password))
                    {
                        // If the user has been approved.
                        if (user.IsApproved)
                        {
                            // User is valid.
                            isValid = true;

                            // Update the user data.
                            user.LastLoginDate = DateTime.Now;
                            new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
                        }
                    }
                    else
                        UpdateFailureCount(username, "password", user);
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

        /// <summary>
        /// Update the current user failure count.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="failureType">The failure type.</param>
        /// <param name="user">The current user.</param>
        private void UpdateFailureCount(string username, string failureType, Nequeo.DataAccess.CloudInteraction.Data.User user)
        {
            DateTime windowStart = new DateTime();
            int failureCount = 0;

            // Get the failure type 'Password'
            if (failureType == "password")
            {
                failureCount = user.FailedPasswordAttemptCount;
                windowStart = user.FailedPasswordAttemptWindowStart;
            }

            // Get the failure type 'Password Answer'
            if (failureType == "passwordAnswer")
            {
                failureCount = user.FailedPasswordAnswerAttemptCount;
                windowStart = user.FailedPasswordAnswerAttemptWindowStart;
            }

            // Get the number of minutes to lockout the user
            // from getting the password again.
            DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

            // First password failure or outside of PasswordAttemptWindow. 
            // Start a new password failure count from 1 and a new window starting now.
            if (failureCount == 0 || DateTime.Now > windowEnd)
            {
                // Get the failure type 'Password'
                if (failureType == "password")
                {
                    user.FailedPasswordAttemptCount = 1;
                    user.FailedPasswordAttemptWindowStart = DateTime.Now;
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
                }

                // Get the failure type 'Password Answer'
                if (failureType == "passwordAnswer")
                {
                    user.FailedPasswordAnswerAttemptCount = 1;
                    user.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
                }
            }
            else
            {
                // Password attempts have exceeded the failure threshold. Lock out the user.
                if (failureCount++ >= MaxInvalidPasswordAttempts)
                {
                    user.UserSuspended = true;
                    user.UserSuspendedDate = DateTime.Now;
                    new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
                }
                else
                {
                    // Get the failure type 'Password'
                    if (failureType == "password")
                    {
                        user.FailedPasswordAttemptCount = failureCount;
                        new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
                    }

                    // Get the failure type 'Password Answer'
                    if (failureType == "passwordAnswer")
                    {
                        user.FailedPasswordAnswerAttemptCount = failureCount;
                        new Nequeo.DataAccess.CloudInteraction.Data.Extension.User().Update.UpdateItem(user);
                    }
                }
            }
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
