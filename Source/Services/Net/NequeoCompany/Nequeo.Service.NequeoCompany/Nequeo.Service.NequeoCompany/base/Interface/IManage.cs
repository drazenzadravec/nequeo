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
    /// The manage service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Manage")]
    public interface IManage
    {
        /// <summary>
        /// Gets the current manage logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.IManage Current { get; }
    }
}
