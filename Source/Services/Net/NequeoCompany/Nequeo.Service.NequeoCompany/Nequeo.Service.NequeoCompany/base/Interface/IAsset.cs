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
    /// The Asset service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Asset")]
    public interface IAsset
    {
        /// <summary>
        /// Gets the current Asset logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.Asset.IAssets Current { get; }

        /// <summary>
        /// Get the Asset information.
        /// </summary>
        /// <param name="assetID">The asset id.</param>
        /// <returns>The asset data.</returns>
        [OperationContract(Name = "GetAsset")]
        DataAccess.NequeoCompany.Data.Assets GetAsset(int assetID);

    }
}
