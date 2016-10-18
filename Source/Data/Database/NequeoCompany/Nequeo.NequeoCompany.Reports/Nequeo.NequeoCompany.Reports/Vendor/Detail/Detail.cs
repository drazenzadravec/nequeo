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

namespace Nequeo.Report.NequeoCompany.Vendor
{
    /// <summary>
    /// Vendor reports
    /// </summary>
    public partial class Vendors
    {
        /// <summary>
        /// Show DetailFromToDate report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void DetailFromToDateShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Vendor.Detail.FromToDate.rdlc",
                "Vendor Details");
        }

        /// <summary>
        /// Show DetailFromToDateID report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void DetailFromToDateIDShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Vendor.Detail.FromToDateID.rdlc",
                "Vendor Details");
        }

        /// <summary>
        /// Show DetailFromToDateSummary report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void DetailFromToDateSummaryShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Vendor.Detail.FromToDateSummary.rdlc",
                "Vendor Details Summary");
        }

        /// <summary>
        /// Create DetailFromToDate binding source data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] DetailFromToDate(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetVendorDetailsBetweenDate";
            bindingSource.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.VendorDetails().GetVendorDetailsBetweenDate(fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetVendorDetailsBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = "Vendor Details From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString(), 
                            ValueType = typeof(String) 
                        },
                    }},
            };
        }

        /// <summary>
        /// Create DetailFromToDateID binding source data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="vendorID">The vendorID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] DetailFromToDateID(DateTime? fromDate, DateTime? toDate, int? vendorID)
        {
            // Assign the binding sources
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedVendorDetailsBetweenDate";
            bindingSource.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.VendorDetails().GetSelectedVendorDetailsBetweenDate(fromDate, toDate, vendorID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedVendorDetailsBetweenDate" }
            };
        }

        /// <summary>
        /// Create DetailFromToDateSummary binding source data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] DetailFromToDateSummary(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetVendorDetailsBetweenDate";
            bindingSource.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.VendorDetails().GetVendorDetailsBetweenDate(fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetVendorDetailsBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = "Vendor Details Summary From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString(), 
                            ValueType = typeof(String) 
                        },
                    }},
            };
        }
    }
}
