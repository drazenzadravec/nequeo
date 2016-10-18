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
    /// The Assets data member extension.
    /// </summary>
    public partial class Assets
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
                case "GetAsset":
                    Action<Nequeo.Threading.AsyncOperationResult<Data.Assets>> callbackGetAsset = (Action<Nequeo.Threading.AsyncOperationResult<Data.Assets>>)_callback[e2];
                    callbackGetAsset(new Nequeo.Threading.AsyncOperationResult<Data.Assets>(((Data.Assets)e1), _state[e2], e2));
                    break;
                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Get the Asset information.
        /// </summary>
        /// <param name="assetID">The asset id.</param>
        /// <returns>The asset data.</returns>
        public virtual Data.Assets GetAsset(int assetID)
        {
            return Select.SelectDataEntity(
                u => u.AssetID == assetID);
        }

        /// <summary>
        /// Get the Asset information.
        /// </summary>
        /// <param name="assetID">The asset id.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void GetAccount(int assetID, Action<Nequeo.Threading.AsyncOperationResult<Data.Assets>> callback, object state = null)
        {
            string keyName = "GetAsset";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<Data.Assets>(u => u.GetAsset(assetID), keyName);
        }
    }
}
