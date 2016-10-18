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

namespace Nequeo.DataAccess.NequeoCompany.Data.Extension
{
    /// <summary>
    /// The Accounts data member extension.
    /// </summary>
    public partial class Accounts
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
                case "GetAccount":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.Accounts>> callbackGetAccount = (Action<Nequeo.Threading.AsyncOperationResult<Data.Accounts>>)_callback[e2];
                    callbackGetAccount(new Nequeo.Threading.AsyncOperationResult<Data.Accounts>(((Data.Accounts)e1), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Get the Account information.
        /// </summary>
        /// <param name="accountID">The account id.</param>
        /// <returns>The account data.</returns>
        public virtual Data.Accounts GetAccount(int accountID)
        {
            return Select.SelectDataEntity(
                u => u.AccountID == accountID);
        }

        /// <summary>
        /// Get the Account information.
        /// </summary>
        /// <param name="accountID">The account id.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetAccount(int accountID, Action<Nequeo.Threading.AsyncOperationResult<Data.Accounts>> callback, object state = null)
        {
            string keyName = "GetAccount";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.Accounts>(u => u.GetAccount(accountID), keyName);
        }
    }
}
