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
    /// The Vendors data member extension.
    /// </summary>
    public partial class Vendors
    {
        /// <summary>
        /// Async complete action handler
        /// </summary>
        /// <param name="sender">The current object handler</param>
        /// <param name="e1">The action execution result</param>
        /// <param name="e2">The unique action name.</param>
        private void _asyncAccount_AsyncComplete(object sender, object e1, string e2)
        {
            switch (e2)
            {
                case "GetVendor":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.Vendors>> callbackGetAccount = (Action<Nequeo.Threading.AsyncOperationResult<Data.Vendors>>)_callback[e2];
                    callbackGetAccount(new Nequeo.Threading.AsyncOperationResult<Data.Vendors>(((Data.Vendors)e1), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Get the Vendors information.
        /// </summary>
        /// <param name="vendorID">The vendor id.</param>
        /// <returns>The Vendor data.</returns>
        public virtual Data.Vendors GetVendor(int vendorID)
        {
            return Select.SelectDataEntity(
                u => u.VendorID == vendorID);
        }

        /// <summary>
        /// Get the Vendors information.
        /// </summary>
        /// <param name="vendorID">The vendor id.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetVendor(int vendorID, Action<Nequeo.Threading.AsyncOperationResult<Data.Vendors>> callback, object state = null)
        {
            string keyName = "GetVendor";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.Vendors>(u => u.GetVendor(vendorID), keyName);
        }
    }
}
