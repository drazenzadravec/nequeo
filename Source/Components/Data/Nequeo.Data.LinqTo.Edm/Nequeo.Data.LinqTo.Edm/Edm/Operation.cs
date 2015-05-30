/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nequeo.ComponentModel;

namespace Nequeo.Data.Edm
{
    /// <summary>
    /// Common operation handler
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Get the connection type model.
        /// </summary>
        /// <typeparam name="TDataContext">The data context type</typeparam>
        /// <typeparam name="TLinqEntity">The model type</typeparam>
        /// <param name="dataAccess">The data access instance.</param>
        /// <param name="connectionConfigKey">The database connection configuration key.</param>
        /// <returns>The connection type model</returns>
        public static ConnectionTypeModel GetTypeModel<TDataContext, TLinqEntity>(IEdmDataGenericBase<TDataContext, TLinqEntity> dataAccess, string connectionConfigKey)
            where TDataContext : System.Data.Entity.DbContext, new()
            where TLinqEntity : class, new()
        {
            ConnectionTypeModel connectionModel = new ConnectionTypeModel();
            connectionModel.ConnectionDataType = dataAccess.ConnectionDataType;
            connectionModel.ConnectionType = dataAccess.ConnectionType;
            connectionModel.DataAccessProvider = dataAccess.DataAccessProvider.GetType().AssemblyQualifiedName;
            connectionModel.DatabaseConnection = connectionConfigKey;
            connectionModel.DataObjectTypeName = typeof(TLinqEntity).AssemblyQualifiedName;
            return connectionModel;
        }
    }
}
