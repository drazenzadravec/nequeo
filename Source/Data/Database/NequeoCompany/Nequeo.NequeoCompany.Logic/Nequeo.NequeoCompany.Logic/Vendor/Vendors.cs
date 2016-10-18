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

namespace Nequeo.Logic.NequeoCompany.Vendor
{
    /// <summary>
    /// Complete data Vendors logic control
    /// </summary>
    public partial interface IVendors
    {
        /// <summary>
        /// Gets the vendor data extension implementation.
        /// </summary>
        DataVendorExtender Extension { get; }

        /// <summary>
        /// Gets the vendor details data extension implementation.
        /// </summary>
        DataVendorDetailExtender Extension1 { get; }

        /// <summary>
        /// Gets the expense type data extension implementation.
        /// </summary>
        DataExpenseTypeExtender Extension2 { get; }

        /// <summary>
        /// Gets the purchase type data extension implementation.
        /// </summary>
        DataPurchaseTypeExtender Extension3 { get; }

        /// <summary>
        /// Gets the vendor report extension implementation.
        /// </summary>
        ReportVendorExtender ReportExtension { get; }
    }

    /// <summary>
    /// Complete data vendor logic control
    /// </summary>
    public partial class Vendors : DataVendorExtender,
        Data.Control.IExtension<
            DataVendorExtender,
            DataVendorDetailExtender,
            DataExpenseTypeExtender,
            DataPurchaseTypeExtender>,
        Report.Common.IReportExtension<ReportVendorExtender>
    {
        /// <summary>
        /// Gets the vendor data extension implementation.
        /// </summary>
        public virtual DataVendorExtender Extension
        {
            get { return new DataVendorExtender(); }
        }

        /// <summary>
        /// Gets the vendor details data extension implementation.
        /// </summary>
        public virtual DataVendorDetailExtender Extension1
        {
            get { return new DataVendorDetailExtender(); }
        }

        /// <summary>
        /// Gets the expense type data extension implementation.
        /// </summary>
        public virtual DataExpenseTypeExtender Extension2
        {
            get { return new DataExpenseTypeExtender(); }
        }

        /// <summary>
        /// Gets the purchase type data extension implementation.
        /// </summary>
        public virtual DataPurchaseTypeExtender Extension3
        {
            get { return new DataPurchaseTypeExtender(); }
        }

        /// <summary>
        /// Gets the vendor report extension implementation.
        /// </summary>
        public virtual ReportVendorExtender ReportExtension
        {
            get { return new ReportVendorExtender(); }
        }
    }
}
