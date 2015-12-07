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

namespace Nequeo.DataAccess.ApplicationLogin.Data.Extension
{
    /// <summary>
    /// The user data member extension.
    /// </summary>
    [Export(typeof(Nequeo.Security.IAuthorisationProvider))]
    [ContentMetadata(Name = "NequeoApplicationLoginModel_User", Index = 0, Description = "Nequeo ApplicationLogin User data model.")]
    partial class User : Nequeo.Security.IAuthorisationProvider
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
        /// <param name="loginUsername">The login username.</param>
        /// <param name="loginPassword">The login password.</param>
        /// <returns>The current user else null.</returns>
        public virtual Data.User ValidateUser(string loginUsername, string loginPassword)
        {
            // Find the user.
            Data.User user = Select.SelectDataEntity(u => (u.LoginUsername == loginUsername));

            try
            {
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
            return user;
        }

        /// <summary>
        /// Validates the current user credentials.
        /// </summary>
        /// <param name="loginUsername">The login username.</param>
        /// <param name="loginPassword">The login password.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void ValidateUser(string loginUsername, string loginPassword, Action<Nequeo.Threading.AsyncOperationResult<Data.User>> callback, object state = null)
        {
            string keyName = "ValidateUser";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.User>(u => u.ValidateUser(loginUsername, loginPassword), keyName);
        }

        /// <summary>
        /// Insert error log.
        /// </summary>
        /// <param name="applicationIdentifier">The initial ApplicationIdentifier value.</param>
        /// <param name="processName">The initial ProcessName value.</param>
        /// <param name="errorCode">The initial ErrorCode value.</param>
        /// <param name="errorDescription">The initial ErrorDescription value.</param>
        /// <returns>The execution result.</returns>
        [FunctionRoutineAttribute("dbo.InsertErrorLog", FunctionRoutineType.StoredProcedure)]
        public virtual int InsertNewErrorLog(
            [FunctionParameterAttribute("@ApplicationIdentifier", "bigint", 8, ParameterDirection.Input, true)] System.Nullable<System.Int64> applicationIdentifier,
            [FunctionParameterAttribute("@ProcessName", "varchar", 200, ParameterDirection.Input, true)] string processName,
            [FunctionParameterAttribute("@ErrorCode", "varchar", 50, ParameterDirection.Input, true)] string errorCode,
            [FunctionParameterAttribute("@ErrorDescription", "varchar", -1, ParameterDirection.Input, true)] string errorDescription)
        {
            IExecuteFunctionResult result = Common.ExecuteFunction(Common, ((MethodInfo)(MethodInfo.GetCurrentMethod())), applicationIdentifier, processName, errorCode, errorDescription);
            return ((int)(result.ReturnValue));
        }

        /// <summary>
        /// Get user screen access.
        /// </summary>
        /// <param name="userID">The initial UserID value.</param>
        /// <param name="applicationID">The initial ApplicationID value.</param>
        /// <returns>The execution result.</returns>
        [FunctionRoutineAttribute("dbo.GetUserScreenAccess", FunctionRoutineType.StoredProcedure)]
        public virtual List<Data.Extended.GetUserScreenAccessResult> GetUserScreenAccessList(
            [FunctionParameterAttribute("@UserID", "bigint", 8, ParameterDirection.Input, true)] System.Nullable<System.Int64> userID,
            [FunctionParameterAttribute("@ApplicationID", "bigint", 8, ParameterDirection.Input, true)] System.Nullable<System.Int64> applicationID)
        {
            IExecuteFunctionResult result = Common.ExecuteFunction(Common, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userID, applicationID);
            return ((List<Data.Extended.GetUserScreenAccessResult>)(result.ReturnValue));
        }

        /// <summary>
        /// Get user screen access.
        /// </summary>
        /// <param name="userID">The initial UserID value.</param>
        /// <param name="applicationID">The initial ApplicationID value.</param>
        /// <returns>The execution result.</returns>
        [FunctionMultiReturnType(typeof(Data.Extended.GetUserScreenAccessResult), 0)]
        [FunctionMultiReturnType(typeof(Data.Extended.CurrentTableValuesResult), 1)]
        [FunctionMultiReturnType(typeof(Data.Extended.RoleTypeList), 2)]
        [FunctionMultiReturnType(typeof(Data.Extended.ScreenList), 3)]
        [FunctionMultiReturnType(typeof(Data.Extended.DepartmentList), 4)]
        [FunctionRoutineAttribute("dbo.GetUserScreenAccess", FunctionRoutineType.StoredProcedure, IsMultiResultSet = true)]
        public virtual IFunctionMultipleResults GetUserScreenAccessListMulti(
            [FunctionParameterAttribute("@UserID", "bigint", 8, ParameterDirection.Input, true)] System.Nullable<System.Int64> userID,
            [FunctionParameterAttribute("@ApplicationID", "bigint", 8, ParameterDirection.Input, true)] System.Nullable<System.Int64> applicationID)
        {
            IExecuteFunctionResult result = Common.ExecuteFunction(Common, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userID, applicationID);
            return ((IFunctionMultipleResults)(result.ReturnValue));
        }

        /// <summary>
        /// Execute the getdatabasecurrenttablevalues routine.
        /// </summary>
        /// <param name="dataBase">Initial value of DataBase.</param>
        /// <param name="tableName">Initial value of TableName.</param>
        /// <param name="primaryKeyName">Initial value of PrimaryKeyName.</param>
        /// <param name="primaryKeyID">Initial value of PrimaryKeyID.</param>
        /// <returns>The execution result.</returns>
        [FunctionRoutineAttribute("dbo.GetDatabaseCurrentTableValues", FunctionRoutineType.StoredProcedure)]
        public virtual List<Data.Extended.CurrentTableValuesResult> GetDatabaseCurrentTableValues(
            [FunctionParameterAttribute("@DataBase", "varchar", -1, ParameterDirection.Input, true)] string dataBase, 
            [FunctionParameterAttribute("@TableName", "varchar", -1, ParameterDirection.Input, true)] string tableName, 
            [FunctionParameterAttribute("@PrimaryKeyName", "varchar", -1, ParameterDirection.Input, true)] string primaryKeyName, 
            [FunctionParameterAttribute("@PrimaryKeyID", "varchar", -1, ParameterDirection.Input, true)] string primaryKeyID)
        {
            IExecuteFunctionResult result = Common.ExecuteFunction(Common, ((MethodInfo)(MethodInfo.GetCurrentMethod())), dataBase, tableName, primaryKeyName, primaryKeyID);
            return ((List<Data.Extended.CurrentTableValuesResult>)(result.ReturnValue));
        }
    }
}
