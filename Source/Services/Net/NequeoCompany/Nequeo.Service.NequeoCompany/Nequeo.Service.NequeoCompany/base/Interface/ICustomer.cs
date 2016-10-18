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
    /// The customer service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Customer")]
    public interface ICustomer
    {
        /// <summary>
        /// Gets the current customer logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.Customer.ICustomers Current { get; }

        /// <summary>
        /// Get the customer information.
        /// </summary>
        /// <param name="customerID">The customer id.</param>
        /// <returns>The customer data.</returns>
        [OperationContract(Name = "GetCustomer")]
        DataAccess.NequeoCompany.Data.Customers GetCustomer(int customerID);

    }
}
