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

namespace Nequeo.Report.NequeoCompany.Company
{
    /// <summary>
    /// Company reports
    /// </summary>
    public partial class Companies
    {
        /// <summary>
        /// Show ProfitLoss report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void ProfitLossShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Company.ProfitLoss.ProfitLoss.rdlc",
                "Company Profit or Loss");
        }

        /// <summary>
        /// Create ProfitLoss binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] ProfitLoss(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetCompanyEmployeePAYGBetweenDate";
            bindingSource.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Companies().GetCompanyEmployeePAYGBetweenDate(fromDate, toDate);

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetCompanyEmployeeSuperBetweenDate";
            bindingSource1.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Companies().GetCompanyEmployeeSuperBetweenDate(fromDate, toDate);

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetCompanyEmployeeWagesBetweenDate";
            bindingSource2.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Companies().GetCompanyEmployeeWagesBetweenDate(fromDate, toDate);

            System.Windows.Forms.BindingSource bindingSource3 = new System.Windows.Forms.BindingSource();
            bindingSource3.DataMember = "GetCompanyInvoiceDetailsBetweenDate";
            bindingSource3.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Companies().GetCompanyInvoiceDetailsBetweenDate(fromDate, toDate);

            System.Windows.Forms.BindingSource bindingSource4 = new System.Windows.Forms.BindingSource();
            bindingSource4.DataMember = "GetCompanyInvoiceProductsBetweenDate";
            bindingSource4.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Companies().GetCompanyInvoiceProductsBetweenDate(fromDate, toDate);

            System.Windows.Forms.BindingSource bindingSource5 = new System.Windows.Forms.BindingSource();
            bindingSource5.DataMember = "GetCompanyVendorDetailsBetweenDate";
            bindingSource5.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Companies().GetCompanyVendorDetailsBetweenDate(fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetCompanyEmployeePAYGBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = "Company Profilt Loss From :" + fromDate.Value.ToShortDateString() + " To : " + toDate.Value.ToShortDateString(), 
                            ValueType = typeof(String) 
                        },
                    }},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetCompanyEmployeeSuperBetweenDate"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetCompanyEmployeeWagesBetweenDate"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource3, DataSourceName = "GetCompanyInvoiceDetailsBetweenDate"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource4, DataSourceName = "GetCompanyInvoiceProductsBetweenDate"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource5, DataSourceName = "GetCompanyVendorDetailsBetweenDate"},
            };
        }
    }
}
