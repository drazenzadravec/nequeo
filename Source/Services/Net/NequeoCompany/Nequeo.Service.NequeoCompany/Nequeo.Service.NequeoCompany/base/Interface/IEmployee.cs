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
    /// The employee service interface data member extension.
    /// </summary>
    [ServiceContract(Name = "Employee")]
    public interface IEmployee
    {
        /// <summary>
        /// Gets the current employee logic implemetation.
        /// </summary>
        Nequeo.Logic.NequeoCompany.Employee.IEmployees Current { get; }

        /// <summary>
        /// Get the employee information.
        /// </summary>
        /// <param name="employeeID">The employee id.</param>
        /// <returns>The emplyee data.</returns>
        [OperationContract(Name = "GetEmployee")]
        DataAccess.NequeoCompany.Data.Employees GetEmployee(int employeeID);


    }
}
