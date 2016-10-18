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

namespace Nequeo.Logic.NequeoCompany.Product
{
    /// <summary>
    /// Complete data Products logic control
    /// </summary>
    public partial interface IProducts
    {
        /// <summary>
        /// Gets the product data extension implementation.
        /// </summary>
        DataProductExtender Extension { get; }

        /// <summary>
        /// Gets the product category data extension implementation.
        /// </summary>
        DataProductCategoryExtender Extension1 { get; }

        /// <summary>
        /// Gets the product sub-category data extension implementation.
        /// </summary>
        DataProductSubCategoryExtender Extension2 { get; }

        /// <summary>
        /// Gets the product status data extension implementation.
        /// </summary>
        DataProductStatusExtender Extension3 { get; }

        /// <summary>
        /// Gets the product edm extension implementation.
        /// </summary>
        EdmProductExtender EdmProductExtension { get; }

        /// <summary>
        /// Gets the product report extension implementation.
        /// </summary>
        ReportProductExtender ReportExtension { get; }
    }

    /// <summary>
    /// Complete data product logic control
    /// </summary>
    public partial class Products : ProductExtender,
        Data.Control.IExtension<
            DataProductExtender,
            DataProductCategoryExtender,
            DataProductSubCategoryExtender,
            DataProductStatusExtender>,
        Report.Common.IReportExtension<ReportProductExtender>
    {
        /// <summary>
        /// Gets the product data extension implementation.
        /// </summary>
        public virtual DataProductExtender Extension
        {
            get { return new DataProductExtender(); }
        }

        /// <summary>
        /// Gets the product category data extension implementation.
        /// </summary>
        public virtual DataProductCategoryExtender Extension1
        {
            get { return new DataProductCategoryExtender(); }
        }

        /// <summary>
        /// Gets the product sub-category data extension implementation.
        /// </summary>
        public virtual DataProductSubCategoryExtender Extension2
        {
            get { return new DataProductSubCategoryExtender(); }
        }

        /// <summary>
        /// Gets the product status data extension implementation.
        /// </summary>
        public virtual DataProductStatusExtender Extension3
        {
            get { return new DataProductStatusExtender(); }
        }

        /// <summary>
        /// Gets the product edm extension implementation.
        /// </summary>
        public virtual EdmProductExtender EdmProductExtension
        {
            get { return new EdmProductExtender(); }
        }

        /// <summary>
        /// Gets the product report extension implementation.
        /// </summary>
        public virtual ReportProductExtender ReportExtension
        {
            get { return new ReportProductExtender(); }
        }
    }
}
