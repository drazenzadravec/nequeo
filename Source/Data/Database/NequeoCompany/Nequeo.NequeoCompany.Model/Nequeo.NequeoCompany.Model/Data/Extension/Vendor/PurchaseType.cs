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
    /// The Purchase type data member extension.
    /// </summary>
    public partial class PurchaseType
    {
        /// <summary>
        /// Get the collection of purchase types.
        /// </summary>
        /// <returns>The collection of purchase types</returns>
        public virtual Extended.PurchaseTypeList GetPurchaseTypeList()
        {
            // Get the collection of purchase types.
            Data.PurchaseType[] data = Select.SelectDataEntities();

            // Add the collection of purchase types.
            Extended.PurchaseTypeList list = new Extended.PurchaseTypeList();
            list.AddRange(data);
            return list;
        }
    }
}
