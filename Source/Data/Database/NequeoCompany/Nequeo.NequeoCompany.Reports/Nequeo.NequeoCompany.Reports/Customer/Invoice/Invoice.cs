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
        /// Show InvoiceFromToInvoicesDateNotPaid report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceFromToInvoicesDateNotPaidShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.FromToInvoiceDateNotPaid.rdlc",
                "Invoice Not Paid Date Interval");
        }

        /// <summary>
        /// Create InvoiceFromToInvoicesDateNotPaid binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceFromToInvoicesDateNotPaid(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceBetweenDateNotPaid";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceBetweenDateNotPaidTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceBetweenDateNotPaidTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceBetweenDateNotPaid, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceBetweenDateNotPaid", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Not Paid From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceFromToInvoicesDateIncome report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceFromToInvoicesDateIncomeShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.FromToInvoiceDateIncome.rdlc",
                "Invoice Income Date Interval");
        }

        /// <summary>
        /// Create InvoiceFromToInvoicesDateIncome binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceFromToInvoicesDateIncome(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceBetweenDateIncome";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceBetweenDateIncomeTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceBetweenDateIncomeTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceBetweenDateIncome, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceBetweenDateIncome", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Income From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceFromToInvoicesDate report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceFromToInvoicesDateShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.FromToInvoiceDate.rdlc",
                "Invoice Date Interval");
        }

        /// <summary>
        /// Create InvoiceFromToInvoicesDate binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceFromToInvoicesDate(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceBetweenDate";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceBetweenDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceBetweenDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceBetweenDate, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceFromToDateSummaryNotPaid report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceFromToDateSummaryNotPaidShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.FromToDateSummaryNotPaid.rdlc",
                "Invoice Not Paid Date Interval Summary");
        }

        /// <summary>
        /// Create InvoiceFromToDateSummaryNotPaid binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceFromToDateSummaryNotPaid(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceDetailsSummaryNotPaidBetweenDate";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetInvoiceProductsSummaryNotPaidBetweenDate";
            bindingSource1.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryNotPaidBetweenDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryNotPaidBetweenDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceDetailsSummaryNotPaidBetweenDate, fromDate, toDate);

            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryNotPaidBetweenDateTableAdapter tableAdapter1 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryNotPaidBetweenDateTableAdapter();
            tableAdapter1.ClearBeforeFill = true;
            tableAdapter1.Fill(dataSet.GetInvoiceProductsSummaryNotPaidBetweenDate, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetInvoiceProductsSummaryBetweenDateNotPaid" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceDetailsSummaryBetweenDateNotPaid", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Not Paid Summary From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceFromToDateSummaryIncome report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceFromToDateSummaryIncomeShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.FromToDateSummaryIncome.rdlc",
                "Invoice Income Date Interval Summary");
        }

        /// <summary>
        /// Create InvoiceFromToDateSummaryIncome binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceFromToDateSummaryIncome(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceDetailsSummaryPaidBetweenDate";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetInvoiceProductsSummaryPaidBetweenDate";
            bindingSource1.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryPaidBetweenDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryPaidBetweenDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceDetailsSummaryPaidBetweenDate, fromDate, toDate);

            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryPaidBetweenDateTableAdapter tableAdapter1 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryPaidBetweenDateTableAdapter();
            tableAdapter1.ClearBeforeFill = true;
            tableAdapter1.Fill(dataSet.GetInvoiceProductsSummaryPaidBetweenDate, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetInvoiceProductsSummaryBetweenDateIncome" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceDetailsSummaryBetweenDateIncome", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Income Summary From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceFromToDateSummary report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceFromToDateSummaryShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.FromToDateSummary.rdlc",
                "Invoice Date Interval Summary");
        }

        /// <summary>
        /// Create InvoiceFromToDateSummary binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceFromToDateSummary(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceDetailsSummaryBetweenDate";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetInvoiceProductsSummaryBetweenDate";
            bindingSource1.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryBetweenDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsSummaryBetweenDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceDetailsSummaryBetweenDate, fromDate, toDate);

            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryBetweenDateTableAdapter tableAdapter1 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsSummaryBetweenDateTableAdapter();
            tableAdapter1.ClearBeforeFill = true;
            tableAdapter1.Fill(dataSet.GetInvoiceProductsSummaryBetweenDate, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetInvoiceProductsSummaryBetweenDate" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceDetailsSummaryBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Summary From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }
    }
}
