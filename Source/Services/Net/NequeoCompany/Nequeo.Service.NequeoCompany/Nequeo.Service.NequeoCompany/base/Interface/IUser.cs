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
    /// The User service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Vendor")]
    public interface IUser
    {
        /// <summary>
        /// Gets the current User logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.User.IUser Current { get; }

        /// <summary>
        /// Validate the user
        /// </summary>
        /// <param name="loginUserName">The login username</param>
        /// <param name="loginPassword">The login password</param>
        /// <returns>The validated user; else null</returns>
        [OperationContract(Name = "ValidateUser1")]
        DataAccess.NequeoCompany.Edm.User ValidateUser1(string loginUserName, string loginPassword);

        /// <summary>
        /// Validate the user
        /// </summary>
        /// <param name="loginUserName">The login username</param>
        /// <param name="loginPassword">The login password</param>
        /// <returns>The validated user; else null</returns>
        [OperationContract(Name = "ValidateUser")]
        DataAccess.NequeoCompany.Data.User ValidateUser(string loginUserName, string loginPassword);
    }
}
