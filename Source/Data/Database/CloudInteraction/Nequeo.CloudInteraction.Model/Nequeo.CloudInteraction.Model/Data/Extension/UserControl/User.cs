/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          User.cs
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
using Nequeo.ComponentModel.Composition;

namespace Nequeo.DataAccess.CloudInteraction.Data.Extension
{
    /// <summary>
    /// The user data member extension.
    /// </summary>
    [Export(typeof(Nequeo.Security.IAuthorisationProvider))]
    [ContentMetadata(Name = "NequeoCloudInteractionModel_User", Index = 0, Description = "Nequeo CloudInteraction User data model.")]
    public partial class User : Nequeo.Security.IAuthorisationProvider
    {
        /// <summary>
        /// Async complete action handler
        /// </summary>
        /// <param name="sender">The current object handler</param>
        /// <param name="e1">The action execution result</param>
        /// <param name="e2">The unique action name.</param>
        private void _asyncAccount_AsyncComplete(object sender, object e1, string e2)
        {
            switch (e2)
            {
                case "ValidateUser":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.User>> callbackValidateUser = (Action<Nequeo.Threading.AsyncOperationResult<Data.User>>)_callback[e2];
                    callbackValidateUser(new Nequeo.Threading.AsyncOperationResult<Data.User>(((Data.User)e1), _state[e2], e2));
                    break;

                case "UpdateLoggedIn":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackUpdateLoggedIn = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackUpdateLoggedIn(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                case "UpdateUserActivity":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackUpdateUserActivity = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackUpdateUserActivity(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Gets, the insert property override.
        /// </summary>
        public override IInsertDataGenericBase<Data.User> Insert
        {
            get
            {
                this.InsertContext = new UserInsertOverride(base.Insert);
                return this.InsertContext;
            }
        }

        /// <summary>
        /// Gets, the update property override.
        /// </summary>
        public override IUpdateDataGenericBase<Data.User> Update
        {
            get
            {
                this.UpdateContext = new UserUpdateOverride(base.Update);
                return this.UpdateContext;
            }
        }

        /// <summary>
        /// Authenticate user credentials.
        /// </summary>
        /// <param name="userCredentials">The user credentials.</param>
        /// <returns>True if authenticated; else false.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool AuthenticateUser(Security.UserCredentials userCredentials)
        {
            Data.User user = ValidateUser(userCredentials.Username, userCredentials.Password);
            if (user != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validates the current user credentials.
        /// </summary>
        /// <param name="username">The login username.</param>
        /// <param name="password">The login password.</param>
        /// <param name="applicationName">The application name.</param>
        /// <returns>The current user else null.</returns>
        public virtual Data.User ValidateUser(string username, string password, string applicationName = null)
        {
            Data.User user = null;

            try
            {
                if (String.IsNullOrEmpty(applicationName))
                {
                    user = Select.SelectDataEntity(u => (u.Username == username));
                }
                else
                {
                    user = Select.SelectDataEntity(
                                    u => (u.Username == username) &&
                                         (u.ApplicationName == applicationName));
                }

                // If user exists.
                if (user != null)
                {
                    // Encode password.
                    Nequeo.Cryptography.IPasswordEncryption encoder = PasswordAuthorisationCode.GetEncoder();
                    string pass = encoder.Decode(user.Password, encoder.PasswordFormat, password);

                    // If not equal then reject.
                    if (pass != password)
                        user = null;
                }
            }
            catch { user = null; }
            return user;
        }

        /// <summary>
        /// Get the user contacts that are currently logged.
        /// </summary>
        /// <param name="username">The current username.</param>
        /// <param name="applicationName">The current application name.</param>
        /// <param name="contactTypeName">The contact type name to search against; (All, Friends, Family, etc).</param>
        /// <param name="lastActiveDate">The date and time in the past, search for users that have been active in the time fame.</param>
        /// <returns>The array of user contacts.</returns>
        public virtual string[] GetUserContactsLoggedIn(string username, string applicationName, string contactTypeName, DateTime lastActiveDate)
        {
            List<string> contacts = new List<string>();
            Data.User user = null;

            // Get the selected user.
            if (String.IsNullOrEmpty(applicationName))
            {
                user = Select.SelectDataEntity(
                                u => (u.Username == username));
            }
            else
            {
                user = Select.SelectDataEntity(
                                u => (u.Username == username) &&
                                     (u.ApplicationName == applicationName));
            }

            // If user exists then get the contacts.
            if (user != null)
            {
                // Select the criteria.
                switch (contactTypeName.ToLower().Trim())
                {
                    case "all":
                    default:
                        // Get all the contacts for the current user.
                        List<Data.Extended.GetActiveLoggedOnUserContactsResult> userContacts = GetActiveLoggedOnUserContacts(lastActiveDate, user.UserID);
                        if (userContacts != null && userContacts.Count > 0)
                        {
                            // For each contact found iterate through
                            // the list to find the current user contact.
                            foreach (Data.Extended.GetActiveLoggedOnUserContactsResult item in userContacts)
                            {
                                // Add the username to the ;ist of contacts that are active and logged.
                                contacts.Add(item.Username);
                            }
                        }
                        break;
                }
            }
            
            // Return the contact list.
            return contacts.ToArray();
        }

        /// <summary>
        /// Update the logged in data.
        /// </summary>
        /// <param name="username">The current username.</param>
        /// <param name="applicationName">The current application name.</param>
        /// <param name="loggedIn">The logged in indicator.</param>
        /// <param name="lastLoginDate">The last login date.</param>
        /// <returns>True if the data was updated; else false.</returns>
        public virtual bool UpdateLoggedIn(string username, string applicationName, bool loggedIn, DateTime? lastLoginDate)
        {
            // If the last login date has been set
            // then update the date.
            if (lastLoginDate != null)
            {
                // Update the loggin details.
                return Update.UpdateItemPredicate(
                                new Data.User()
                                {
                                    LastActivityDate = lastLoginDate.Value,
                                    LastLoginDate = lastLoginDate.Value,
                                    LoggedIn = loggedIn
                                }, u =>
                                    (u.Username == username) &&
                                    (u.ApplicationName == applicationName)
                            );
            }
            else
            {
                // Update the loggin details.
                return Update.UpdateItemPredicate(
                                new Data.User()
                                {
                                    LastActivityDate = DateTime.Now,
                                    LoggedIn = loggedIn
                                }, u =>
                                    (u.Username == username) &&
                                    (u.ApplicationName == applicationName)
                            );
            }
        }

        /// <summary>
        /// Update the users activity status as the current data time; indicating that the user is active.
        /// </summary>
        /// <param name="username">The current username.</param>
        /// <param name="applicationName">The current application name.</param>
        /// <returns>True if the data was updated; else false.</returns>
        public virtual bool UpdateUserActivity(string username, string applicationName)
        {
            // Update the user activity date time.
            return Update.UpdateItemPredicate(
                            new Data.User()
                            {
                                LastActivityDate = DateTime.Now
                            }, u =>
                                (u.Username == username) &&
                                (u.ApplicationName == applicationName)
                        );
        }

        /// <summary>
        /// Validates the current user credentials.
        /// </summary>
        /// <param name="username">The login username.</param>
        /// <param name="password">The login password.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void ValidateUser(string username, string password, string applicationName, 
            Action<Nequeo.Threading.AsyncOperationResult<Data.User>> callback, object state = null)
        {
            string keyName = "ValidateUser";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.User>(u => u.ValidateUser(username, password, applicationName), keyName);
        }

        /// <summary>
        /// Update the logged in data.
        /// </summary>
        /// <param name="username">The current username.</param>
        /// <param name="applicationName">The current application name.</param>
        /// <param name="loggedIn">The logged in indicator.</param>
        /// <param name="lastLoginDate">The last login date.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void UpdateLoggedIn(string username, string applicationName, bool loggedIn, DateTime? lastLoginDate,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "UpdateLoggedIn";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(u => u.UpdateLoggedIn(username, applicationName, loggedIn, lastLoginDate), keyName);
        }

        /// <summary>
        /// Update the users activity status as the current data time; indicating that the user is active.
        /// </summary>
        /// <param name="username">The current username.</param>
        /// <param name="applicationName">The current application name.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void UpdateUserActivity(string username, string applicationName,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "UpdateUserActivity";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(u => u.UpdateUserActivity(username, applicationName), keyName);
        }
    }
}
