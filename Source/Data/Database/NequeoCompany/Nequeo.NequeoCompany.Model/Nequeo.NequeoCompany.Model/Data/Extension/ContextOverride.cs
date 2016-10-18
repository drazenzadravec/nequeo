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
using System.Threading.Tasks;

namespace Nequeo.DataAccess.NequeoCompany.Data
{
    /// <summary>
    /// The datacontext data context object class.
    /// </summary>
    partial class DataContext
    {
        /// <summary>
        /// Gets the insert object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The insert data entity object.</returns>
        public override Nequeo.Data.IInsertDataGenericBase<TDataEntity> Insert<TDataEntity>()
        {
            // If type is data User.
            if (typeof(TDataEntity) == typeof(Data.User))
            {
                Nequeo.Data.IInsertDataGenericBase<Data.User> user = new Extension.UserInsertOverride(base.Insert<Data.User>());
                return (Nequeo.Data.IInsertDataGenericBase<TDataEntity>)user;
            }
            else
                return base.Insert<TDataEntity>();
        }

        /// <summary>
        /// Gets the update object.
        /// </summary>
        /// <typeparam name="TDataEntity">The data entity to examine.</typeparam>
        /// <returns>The update data entity object.</returns>
        public override Nequeo.Data.IUpdateDataGenericBase<TDataEntity> Update<TDataEntity>()
        {
            // If type is data User.
            if (typeof(TDataEntity) == typeof(Data.User))
            {
                Nequeo.Data.IUpdateDataGenericBase<Data.User> user = new Extension.UserUpdateOverride(base.Update<Data.User>());
                return (Nequeo.Data.IUpdateDataGenericBase<TDataEntity>)user;
            }
            else
                return base.Update<TDataEntity>();
        }
    }
}
