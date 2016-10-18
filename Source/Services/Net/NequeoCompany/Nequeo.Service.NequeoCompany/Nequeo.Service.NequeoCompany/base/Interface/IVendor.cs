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
    /// The Vendor service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Vendor")]
    public interface IVendor
    {
        /// <summary>
        /// Gets the current Vendor logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.Vendor.IVendors Current { get; }

        /// <summary>
        /// Get the Vendor information.
        /// </summary>
        /// <param name="vendorID">The vendor id.</param>
        /// <returns>The vendor data.</returns>
        [OperationContract(Name = "GetVendor")]
        DataAccess.NequeoCompany.Data.Vendors GetVendor(int vendorID);

    }
}
