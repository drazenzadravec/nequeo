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
    /// Database source membership provider
    /// </summary>
    public sealed class DataBaseMembershipProvider : MembershipProvider
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataBaseMembershipProvider()
        {
        }

        private MachineKeySection _machineKey;
        private string _membershipProviderType = null;
        private Nequeo.Data.DataType.IMembershipProvider _membershipProvider = null;

        /// <summary>
        /// Gets the membership provider type.
        /// </summary>
        /// <remarks>The membership provider type must implement 'Nequeo.Data.DataType.IMembershipProvider'.</remarks>
        public string MembershipProviderType
        {
            get { return _membershipProviderType; }
        }

        /// <summary>
        /// Gets or sets the current Nequeo.Data.DataType.IMembershipProvider.
        /// </summary>
        public Nequeo.Data.DataType.IMembershipProvider MembershipProviderTypeInstance
        {
            get { return _membershipProvider; }
            set { _membershipProvider = value; }
        }

        #region Abstract Property Overrides
        /// <summary>
        /// Gets sets the application name.
        /// </summary>
        public override string ApplicationName
        {
            get { return _membershipProvider.ApplicationName; }
            set { _membershipProvider.ApplicationName = value; }
        }

        /// <summary>
        /// Gets enable password reset.
        /// </summary>
        public override bool EnablePasswordReset
        {
            get { return _membershipProvider.EnablePasswordReset; }
        }

        /// <summary>
        /// Gets enable password retrieval.
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get { return _membershipProvider.EnablePasswordRetrieval; }
        }

        /// <summary>
        /// Gets maximum invalid password attempts.
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { return _membershipProvider.MaxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Gets minimum required non alphanumeric characters
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _membershipProvider.MinRequiredNonAlphanumericCharacters; }
        }

        /// <summary>
        /// Gets minimum required password length.
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get { return _membershipProvider.MinRequiredPasswordLength; }
        }

        /// <summary>
        /// Gets password attempt window.
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get { return _membershipProvider.PasswordAttemptWindow; }
        }

        /// <summary>
        /// Gets password format.
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _membershipProvider.PasswordFormat; }
        }

        /// <summary>
        /// Gets password strength regular expression.
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get { return _membershipProvider.PasswordStrengthRegularExpression; }
        }

        /// <summary>
        /// Gets requires question and answer.
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get { return _membershipProvider.RequiresQuestionAndAnswer; }
        }

        /// <summary>
        /// Gets requires unique email.
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get { return _membershipProvider.RequiresUniqueEmail; }
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
                name = "DataBaseMembershipProvider";

            // Get the role application Name.
            if (String.IsNullOrEmpty(config["applicationName"]))
                throw new Exception("Attribute : applicationName, is missing this must be a valid application name.");

            // Get the membership provider type.
            if (String.IsNullOrEmpty(config["membershipProviderType"]))
                throw new Exception("Attribute : membershipProviderType, is missing (this type must implement : 'Nequeo.Data.DataType.IMembershipProvider')");
            else
                _membershipProviderType = config["membershipProviderType"].ToString();

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Nequeo Pty Limited generic data access membership provider.");
            }

            // Get the current membership provider type
            // and create a instance of the type.
            Type membershipProviderType = BuildManager.GetType(_membershipProviderType, true, true);
            _membershipProvider = (Nequeo.Data.DataType.IMembershipProvider)Activator.CreateInstance(membershipProviderType);

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _membershipProvider.ProviderName = name;
            _membershipProvider.ApplicationName = GetConfigValue(config["applicationName"], "ApplicationName");
            _membershipProvider.EnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "false"));
            _membershipProvider.EnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "false"));
            _membershipProvider.MaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "0"));
            _membershipProvider.MinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "0"));
            _membershipProvider.MinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "0"));
            _membershipProvider.PasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "0"));
            _membershipProvider.PasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            _membershipProvider.RequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _membershipProvider.RequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "false"));
            
            string temp_format = config["passwordFormat"];
            if (temp_format == null)
                temp_format = "Clear";

            switch (temp_format)
            {
                case "Hashed":
                    _membershipProvider.PasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _membershipProvider.PasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _membershipProvider.PasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            // Get encryption and decryption key information from the configuration.
            System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (_machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                "are not supported with auto-generated keys.");
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if complete; else false.</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return _membershipProvider.ChangePassword(username, oldPassword, newPassword);
        }

        /// <summary>
        /// Change password question and answer.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="newPasswordQuestion">The new password question.</param>
        /// <param name="newPasswordAnswer">The new password question.</param>
        /// <returns>True if complete; else false.</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return _membershipProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
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
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return _membershipProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        /// <summary>
        /// Delete the user
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="deleteAllRelatedData">Delete all related data.</param>
        /// <returns>True if complete; else false.</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return _membershipProvider.DeleteUser(username, deleteAllRelatedData);
        }

        /// <summary>
        /// Find users by email.
        /// </summary>
        /// <param name="emailToMatch">The email to match.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return _membershipProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Find users by name.
        /// </summary>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return _membershipProvider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalRecords">Total number of records.</param>
        /// <returns>The membership user collection.</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return _membershipProvider.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Get the number of users online.
        /// </summary>
        /// <returns>The number of users online.</returns>
        public override int GetNumberOfUsersOnline()
        {
            return _membershipProvider.GetNumberOfUsersOnline();
        }

        /// <summary>
        /// Get password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="answer">The answer.</param>
        /// <returns>The password.</returns>
        public override string GetPassword(string username, string answer)
        {
            return _membershipProvider.GetPassword(username, answer);
        }

        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userIsOnline">Is the user online.</param>
        /// <returns>The membership user.</returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return _membershipProvider.GetUser(username, userIsOnline);
        }

        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="providerUserKey">Provider user key.</param>
        /// <param name="userIsOnline">Is the user online.</param>
        /// <returns>The membership user.</returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return _membershipProvider.GetUser(providerUserKey, userIsOnline);
        }

        /// <summary>
        /// Get username by email.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>The username.</returns>
        public override string GetUserNameByEmail(string email)
        {
            return _membershipProvider.GetUserNameByEmail(email);
        }

        /// <summary>
        /// Reset password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="answer">The answer.</param>
        /// <returns>The new password.</returns>
        public override string ResetPassword(string username, string answer)
        {
            return _membershipProvider.ResetPassword(username, answer);
        }

        /// <summary>
        /// Unlock the user.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <returns>True if complete; else false.</returns>
        public override bool UnlockUser(string userName)
        {
            return _membershipProvider.UnlockUser(userName);
        }

        /// <summary>
        /// Update the user.
        /// </summary>
        /// <param name="user">The membership user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            _membershipProvider.UpdateUser(user);
        }

        /// <summary>
        /// Validate the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if complete; else false.</returns>
        public override bool ValidateUser(string username, string password)
        {
            return _membershipProvider.ValidateUser(username, password);
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
