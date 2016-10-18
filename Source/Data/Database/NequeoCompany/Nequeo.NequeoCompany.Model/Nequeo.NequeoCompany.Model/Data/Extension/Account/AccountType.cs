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
    /// The Account type data member extension.
    /// </summary>
    public partial class AccountType
    {
        /// <summary>
        /// Get the collection of account types.
        /// </summary>
        /// <returns>The collection of account types</returns>
        public virtual Extended.AccountTypeList GetAccountTypeList()
        {
            // Get the collection of income types.
            Data.AccountType[] data = Select.SelectDataEntities();

            // Add the collection of imcome types.
            Extended.AccountTypeList list = new Extended.AccountTypeList();
            list.AddRange(data);
            return list;
        }
    }
}
