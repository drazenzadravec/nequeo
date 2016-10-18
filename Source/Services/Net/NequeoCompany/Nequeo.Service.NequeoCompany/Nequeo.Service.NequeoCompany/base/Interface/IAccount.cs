/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Nequeo.Service.NequeoCompany
{
    /// <summary>
    /// The Account service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Account")]
    public interface IAccount
    {
        /// <summary>
        /// Gets the current Account logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.Account.IAccounts Current { get; }

        /// <summary>
        /// Get the Account information.
        /// </summary>
        /// <param name="accountID">The account id.</param>
        /// <returns>The account data.</returns>
        [OperationContract(Name = "GetAccount")]
        DataAccess.NequeoCompany.Data.Accounts GetAccount(int accountID);

    }
}
