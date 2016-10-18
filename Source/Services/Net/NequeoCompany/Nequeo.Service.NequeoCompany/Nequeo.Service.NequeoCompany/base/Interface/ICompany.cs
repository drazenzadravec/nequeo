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
    /// The Company service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Company")]
    public interface ICompany
    {
        /// <summary>
        /// Gets the current company logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.Company.ICompanies Current { get; }

        /// <summary>
        /// Get the Company information.
        /// </summary>
        /// <param name="companyID">The company id.</param>
        /// <returns>The company data.</returns>
        [OperationContract(Name = "GetCompany")]
        DataAccess.NequeoCompany.Data.Companies GetCompany(int companyID);

    }
}
