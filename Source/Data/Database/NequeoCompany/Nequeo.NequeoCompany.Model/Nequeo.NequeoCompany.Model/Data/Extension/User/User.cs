/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
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

namespace Nequeo.DataAccess.NequeoCompany.Data.Extension
{
    /// <summary>
    /// The User data member extension.
    /// </summary>
    [Export(typeof(Nequeo.Security.IAuthorisationProvider))]
    [ContentMetadata(Name = "NequeoNequeoCompanyModel_User", Index = 0, Description = "Nequeo NequeoCompany User data model.")]
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
                case "GetVendor":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.User>> callbackGetAccount = (Action<Nequeo.Threading.AsyncOperationResult<Data.User>>)_callback[e2];
                    callbackGetAccount(new Nequeo.Threading.AsyncOperationResult<Data.User>(((Data.User)e1), _state[e2], e2));
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
            Data.User user = Validate(userCredentials.Username, userCredentials.Password);
            if (user != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validate the user
        /// </summary>
        /// <param name="loginUserName">The login username</param>
        /// <param name="loginPassword">The login password</param>
        /// <returns>The validated user; else null</returns>
        public virtual Data.User Validate(string loginUserName, string loginPassword)
        {
            Data.User user = null;

            try
            {
                // Find the user.
                user = Select.SelectDataEntity(u => (u.LoginUserName == loginUserName));

                // If user exists.
                if (user != null)
                {
                    // Encode password.
                    Nequeo.Cryptography.IPasswordEncryption encoder = PasswordAuthorisationCode.GetEncoder();
                    string password = encoder.Decode(user.LoginPassword, encoder.PasswordFormat, loginPassword);

                    // If not equal then reject.
                    if (password != loginPassword)
                        user = null;
                }
            }
            catch { user = null; }

            // Return the user.
            return user;
        }

        /// <summary>
        /// Get the Users information.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>The User data.</returns>
        public virtual Data.User GetUser(int userID)
        {
            return Select.SelectDataEntity(
                u => u.UserID == userID);
        }

        /// <summary>
        /// Get the Users information.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetUser(int userID, Action<Nequeo.Threading.AsyncOperationResult<Data.User>> callback, object state = null)
        {
            string keyName = "GetUser";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.User>(u => u.GetUser(userID), keyName);
        }
    }
}
