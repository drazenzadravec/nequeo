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

namespace Nequeo.Logic.NequeoCompany.Customer
{
    /// <summary>
    /// Complete data Customers logic control
    /// </summary>
    public partial interface ICustomers
    {
        /// <summary>
        /// Gets the customer data extension implementation.
        /// </summary>
        DataCustomerExtender Extension { get; }

        /// <summary>
        /// Gets the invoice PAYG data extension implementation.
        /// </summary>
        DataInvoiceExtender Extension1 { get; }

        /// <summary>
        /// Gets the invoice details data extension implementation.
        /// </summary>
        DataInvoiceDetailExtender Extension2 { get; }

        /// <summary>
        /// Gets the invoice products data extension implementation.
        /// </summary>
        DataInvoiceProductExtender Extension3 { get; }

        /// <summary>
        /// Gets the income type data extension implementation.
        /// </summary>
        DataIncomeTypeExtender Extension4 { get; }

        /// <summary>
        /// Gets the gst type data extension implementation.
        /// </summary>
        DataGstIncomeTypeExtender Extension5 { get; }

        /// <summary>
        /// Gets the customer report extension implementation.
        /// </summary>
        ReportCustomerExtender ReportExtension { get; }
    }

    /// <summary>
    /// Complete data customers logic control
    /// </summary>
    public partial class Customers : DataCustomerExtender,
        Data.Control.IExtension<
            DataCustomerExtender,
            DataInvoiceExtender,
            DataInvoiceDetailExtender,
            DataInvoiceProductExtender,
            DataIncomeTypeExtender,
            DataGstIncomeTypeExtender>,
        Report.Common.IReportExtension<ReportCustomerExtender>
    {
        /// <summary>
        /// Gets the customer data extension implementation.
        /// </summary>
        public virtual DataCustomerExtender Extension
        {
            get { return new DataCustomerExtender(); }
        }

        /// <summary>
        /// Gets the invoice PAYG data extension implementation.
        /// </summary>
        public virtual DataInvoiceExtender Extension1
        {
            get { return new DataInvoiceExtender(); }
        }

        /// <summary>
        /// Gets the invoice details data extension implementation.
        /// </summary>
        public virtual DataInvoiceDetailExtender Extension2
        {
            get { return new DataInvoiceDetailExtender(); }
        }

        /// <summary>
        /// Gets the invoice products data extension implementation.
        /// </summary>
        public virtual DataInvoiceProductExtender Extension3
        {
            get { return new DataInvoiceProductExtender(); }
        }

        /// <summary>
        /// Gets the income type data extension implementation.
        /// </summary>
        public virtual DataIncomeTypeExtender Extension4
        {
            get { return new DataIncomeTypeExtender(); }
        }

        /// <summary>
        /// Gets the gst type data extension implementation.
        /// </summary>
        public virtual DataGstIncomeTypeExtender Extension5
        {
            get { return new DataGstIncomeTypeExtender(); }
        }

        /// <summary>
        /// Gets the customer report extension implementation.
        /// </summary>
        public virtual ReportCustomerExtender ReportExtension
        {
            get { return new ReportCustomerExtender(); }
        }
    }
}
