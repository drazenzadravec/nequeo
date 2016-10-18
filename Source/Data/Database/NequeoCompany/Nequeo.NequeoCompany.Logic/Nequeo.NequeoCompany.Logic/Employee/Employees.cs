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

using Nequeo.Data.Control;
using Nequeo.DataAccess.NequeoCompany;
using Nequeo.Report;
using Nequeo.Report.NequeoCompany;
using Nequeo.Report.Common;

namespace Nequeo.Logic.NequeoCompany.Employee
{
    /// <summary>
    /// Complete data Employees logic control
    /// </summary>
    public partial interface IEmployees
    {
        /// <summary>
        /// Gets the employee data extension implementation.
        /// </summary>
        DataEmployeeExtender Extension { get; }

        /// <summary>
        /// Gets the employee PAYG data extension implementation.
        /// </summary>
        DataEmployeePAYGExtender Extension1 { get; }

        /// <summary>
        /// Gets the super data extension implementation.
        /// </summary>
        DataSuperExtender Extension2 { get; }

        /// <summary>
        /// Gets the wage data extension implementation.
        /// </summary>
        DataWageExtender Extension3 { get; }

        /// <summary>
        /// Gets the super account data extension implementation.
        /// </summary>
        DataEmployeeSuperAccountExtender Extension4 { get; }

        /// <summary>
        /// Gets the bank account data extension implementation.
        /// </summary>
        DataEmployeeBankAccountExtender Extension5 { get; }

        /// <summary>
        /// Gets the pay interval type data extension implementation.
        /// </summary>
        DataPayIntervalTypeExtender Extension6 { get; }

        /// <summary>
        /// Gets the employees report extension implementation.
        /// </summary>
        ReportEmployeeExtender ReportExtension { get; }
    }

    /// <summary>
    /// Complete data employees logic control
    /// </summary>
    public partial class Employees : DataEmployeeExtender,
        Data.Control.IExtension<
            DataEmployeeExtender,
            DataEmployeePAYGExtender,
            DataSuperExtender,
            DataWageExtender,
            DataEmployeeSuperAccountExtender,
            DataEmployeeBankAccountExtender,
            DataPayIntervalTypeExtender>,
        Report.Common.IReportExtension<ReportEmployeeExtender>
    {
        /// <summary>
        /// Gets the employee data extension implementation.
        /// </summary>
        public virtual DataEmployeeExtender Extension
        {
            get { return new DataEmployeeExtender(); }
        }

        /// <summary>
        /// Gets the employee PAYG data extension implementation.
        /// </summary>
        public virtual DataEmployeePAYGExtender Extension1
        {
            get { return new DataEmployeePAYGExtender(); }
        }

        /// <summary>
        /// Gets the super data extension implementation.
        /// </summary>
        public virtual DataSuperExtender Extension2
        {
            get { return new DataSuperExtender(); }
        }

        /// <summary>
        /// Gets the wage data extension implementation.
        /// </summary>
        public virtual DataWageExtender Extension3
        {
            get { return new DataWageExtender(); }
        }

        /// <summary>
        /// Gets the super account data extension implementation.
        /// </summary>
        public virtual DataEmployeeSuperAccountExtender Extension4
        {
            get { return new DataEmployeeSuperAccountExtender(); }
        }

        /// <summary>
        /// Gets the bank account data extension implementation.
        /// </summary>
        public virtual DataEmployeeBankAccountExtender Extension5
        {
            get { return new DataEmployeeBankAccountExtender(); }
        }

        /// <summary>
        /// Gets the pay interval type data extension implementation.
        /// </summary>
        public virtual DataPayIntervalTypeExtender Extension6
        {
            get { return new DataPayIntervalTypeExtender(); }
        }

        /// <summary>
        /// Gets the employees report extension implementation.
        /// </summary>
        public virtual ReportEmployeeExtender ReportExtension
        {
            get { return new ReportEmployeeExtender(); }
        }
    }
}
