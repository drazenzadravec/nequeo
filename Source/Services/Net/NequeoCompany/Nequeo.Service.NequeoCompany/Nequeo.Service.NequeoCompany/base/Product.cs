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

using Nequeo.Net.ServiceModel.Common;

namespace Nequeo.Service.NequeoCompany
{
    /// <summary>
    /// The Product data member extension.
    /// </summary>
    [ErrorBehavior(typeof(CustomErrorHandler), "ProductErrorBehavior")]
    public class Product : Nequeo.Logic.NequeoCompany.Product.Products, IProduct
    {
    }
}
