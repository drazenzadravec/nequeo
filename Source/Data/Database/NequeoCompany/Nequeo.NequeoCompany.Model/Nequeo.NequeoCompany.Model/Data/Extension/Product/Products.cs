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
    /// The Products data member extension.
    /// </summary>
    public partial class Products
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
                case "GetProduct":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.Products>> callbackGetAccount = (Action<Nequeo.Threading.AsyncOperationResult<Data.Products>>)_callback[e2];
                    callbackGetAccount(new Nequeo.Threading.AsyncOperationResult<Data.Products>(((Data.Products)e1), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Get the Products information.
        /// </summary>
        /// <param name="productID">The product id.</param>
        /// <returns>The product data.</returns>
        public virtual Data.Products GetProduct(int productID)
        {
            return Select.SelectDataEntity(
                u => u.ProductID == productID);
        }

        /// <summary>
        /// Get the Products information.
        /// </summary>
        /// <param name="productID">The product id.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetProduct(int productID, Action<Nequeo.Threading.AsyncOperationResult<Data.Products>> callback, object state = null)
        {
            string keyName = "GetProduct";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.Products>(u => u.GetProduct(productID), keyName);
        }
    }
}
