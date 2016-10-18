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

namespace Nequeo.Logic.NequeoCompany.Company
{
    /// <summary>
    /// Complete data Companies logic control
    /// </summary>
    public partial interface ICompanies
    {
        /// <summary>
        /// Gets the company data extension implementation.
        /// </summary>
        DataCompanyExtender Extension { get; }

        /// <summary>
        /// Gets the BAS data extension implementation.
        /// </summary>
        DataBASExtender Extension1 { get; }

        /// <summary>
        /// Gets the PAYG instalment data extension implementation.
        /// </summary>
        DataPAYGInstalmentExtender Extension2 { get; }

        /// <summary>
        /// Gets the tax return data extension implementation.
        /// </summary>
        DataTaxReturnExtender Extension3 { get; }

        /// <summary>
        /// Gets the customer extension implementation.
        /// </summary>
        Customer.ICustomers Extension4 { get; }

        /// <summary>
        /// Gets the generic data extension implementation.
        /// </summary>
        DataGenericDataExtender DataGenericDataExtension { get; }

        /// <summary>
        /// Gets the company edm extension implementation.
        /// </summary>
        EdmCompanyExtender EdmCompanyExtension { get; }

        /// <summary>
        /// Gets the company report extension implementation.
        /// </summary>
        ReportCompanyExtender ReportExtension { get; }
    }

    /// <summary>
    /// Complete data companies logic control
    /// </summary>
    public partial class Companies : DataCompanyExtender,
        Data.Control.IExtension<
            DataCompanyExtender,
            DataBASExtender,
            DataPAYGInstalmentExtender,
            DataTaxReturnExtender,
            Customer.ICustomers>,
        Report.Common.IReportExtension<ReportCompanyExtender>
    {
        /// <summary>
        /// Gets the company data extension implementation.
        /// </summary>
        public virtual DataCompanyExtender Extension
        {
            get { return new DataCompanyExtender(); }
        }

        /// <summary>
        /// Gets the BAS data extension implementation.
        /// </summary>
        public virtual DataBASExtender Extension1
        {
            get { return new DataBASExtender(); }
        }

        /// <summary>
        /// Gets the PAYG instalment data extension implementation.
        /// </summary>
        public virtual DataPAYGInstalmentExtender Extension2
        {
            get { return new DataPAYGInstalmentExtender(); }
        }

        /// <summary>
        /// Gets the tax return data extension implementation.
        /// </summary>
        public virtual DataTaxReturnExtender Extension3
        {
            get { return new DataTaxReturnExtender(); }
        }

        /// <summary>
        /// Gets the customer extension implementation.
        /// </summary>
        public virtual Customer.ICustomers Extension4
        {
            get { return new Customer.Customers(); }
        }

        /// <summary>
        /// Gets the generic data extension implementation.
        /// </summary>
        public virtual DataGenericDataExtender DataGenericDataExtension
        {
            get { return new DataGenericDataExtender(); }
        }

        /// <summary>
        /// Gets the company edm extension implementation.
        /// </summary>
        public virtual EdmCompanyExtender EdmCompanyExtension
        {
            get { return new EdmCompanyExtender(); }
        }

        /// <summary>
        /// Gets the company report extension implementation.
        /// </summary>
        public virtual ReportCompanyExtender ReportExtension
        {
            get { return new ReportCompanyExtender(); }
        }
    }
}
