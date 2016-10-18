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
    /// The Product service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Product")]
    public interface IProduct
    {
        /// <summary>
        /// Gets the current Product logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.Product.IProducts Current { get; }

        /// <summary>
        /// Get the Product information.
        /// </summary>
        /// <param name="productID">The product id.</param>
        /// <returns>The product data.</returns>
        [OperationContract(Name = "GetProduct")]
        DataAccess.NequeoCompany.Data.Products GetProduct(int productID);

    }
}
