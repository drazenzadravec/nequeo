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

namespace Nequeo.Report.NequeoCompany.Customer
{
    /// <summary>
    /// Customer reports
    /// </summary>
    public partial class Customers
    {
        /// <summary>
        /// Show IncomeFromToDate report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void IncomeFromToDateShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Income.FromToDate.rdlc",
                "Customer Income Date Interval");
        }

        /// <summary>
        /// Create IncomeFromToDate binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] IncomeFromToDate(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceDetailsSummaryBetweenDateIncome";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetInvoiceProductsSummaryBetweenDateIncome";
            bindingSource1.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetInvoiceDetailsBetweenDateIncome";
            bindingSource2.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource3 = new System.Windows.Forms.BindingSource();
            bindingSource3.DataMember = "GetInvoiceProductsBetweenDateIncome";
            bindingSource3.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryBetweenDateIncomeTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryBetweenDateIncomeTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceDetailsSummaryBetweenDateIncome, fromDate, toDate);

            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryBetweenDateIncomeTableAdapter tableAdapter1 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryBetweenDateIncomeTableAdapter();
            tableAdapter1.ClearBeforeFill = true;
            tableAdapter1.Fill(dataSet.GetInvoiceProductsSummaryBetweenDateIncome, fromDate, toDate);

            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateIncomeTableAdapter tableAdapter2 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateIncomeTableAdapter();
            tableAdapter2.ClearBeforeFill = true;
            tableAdapter2.Fill(dataSet.GetInvoiceDetailsBetweenDateIncome, fromDate, toDate);

            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateIncomeTableAdapter tableAdapter3 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateIncomeTableAdapter();
            tableAdapter3.ClearBeforeFill = true;
            tableAdapter3.Fill(dataSet.GetInvoiceProductsBetweenDateIncome, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceDetailsSummaryBetweenDateIncome", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Customer Income From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetInvoiceProductsSummaryBetweenDateIncome" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetInvoiceDetailsBetweenDateIncome" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource3, DataSourceName = "GetInvoiceProductsBetweenDateIncome" },
            };
        }

        /// <summary>
        /// Show IncomeFromToDateID report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void IncomeFromToDateIDShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Income.FromToDateID.rdlc",
                "Customer Income Date Interval");
        }

        /// <summary>
        /// Create IncomeFromToDateID binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="customerID">The customerID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] IncomeFromToDateID(DateTime? fromDate, DateTime? toDate, int? customerID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedCustomerIncomeInvoiceDetailsBetweenDate";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetSelectedCustomerIncomeInvoiceProductsBetweenDate";
            bindingSource1.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedCustomerIncomeInvoiceDetailsBetweenDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedCustomerIncomeInvoiceDetailsBetweenDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedCustomerIncomeInvoiceDetailsBetweenDate, fromDate, toDate, customerID);

            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedCustomerIncomeInvoiceProductsBetweenDateTableAdapter tableAdapter1 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedCustomerIncomeInvoiceProductsBetweenDateTableAdapter();
            tableAdapter1.ClearBeforeFill = true;
            tableAdapter1.Fill(dataSet.GetSelectedCustomerIncomeInvoiceProductsBetweenDate, fromDate, toDate, customerID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedCustomerIncomeInvoiceDetailsBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Customer Income From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetSelectedCustomerIncomeInvoiceProductsBetweenDate" },
            };
        }
    }
}
