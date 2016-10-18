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
        /// Show InvoiceDetailTaxInvoice report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceDetailTaxInvoiceShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Detail.TaxInvoice.rdlc",
                "Tax Invoice");
        }

        /// <summary>
        /// Create InvoiceDetailTaxInvoice binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceDetailTaxInvoice(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceDetails";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceDetails, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceDetails"},
            };
        }

        /// <summary>
        /// Show InvoiceDetailQuotation report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceDetailQuotationShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Detail.Quotation.rdlc",
                "Quotation");
        }

        /// <summary>
        /// Create InvoiceDetailQuotation binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceDetailQuotation(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceDetails";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceDetails, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceDetails"},
            };
        }

        /// <summary>
        /// Show InvoiceDetailEmptyTaxInvoice report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceDetailEmptyTaxInvoiceShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Detail.EmptyTaxInvoice.rdlc",
                "Empty Tax Invoice");
        }

        /// <summary>
        /// Create InvoiceDetailEmptyTaxInvoice binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceDetailEmptyTaxInvoice(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceDetails";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceDetails, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceDetails"},
            };
        }

        /// <summary>
        /// Show InvoiceDetailEmptyQuotation report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceDetailEmptyQuotationShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Detail.EmptyQuotation.rdlc",
                "Empty Quoation");
        }

        /// <summary>
        /// Create InvoiceDetailEmptyQuotation binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceDetailEmptyQuotation(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceDetails";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceDetailsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceDetails, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceDetails"},
            };
        }

        /// <summary>
        /// Show InvoiceDetailFromToInvoicesDate report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceDetailFromToInvoicesDateShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Detail.FromToInvoiceDate.rdlc",
                "Invoice Date Interval Details");
        }

        /// <summary>
        /// Create InvoiceDetailFromToInvoicesDate binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceDetailFromToInvoicesDate(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceDetailsBetweenDate";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceDetailsBetweenDate, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceDetailsBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Details From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceDetailFromToInvoicesDateIncome report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceDetailFromToInvoicesDateIncomeShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Detail.FromToInvoiceDateIncome.rdlc",
                "Invoice Income Date Interval Details");
        }

        /// <summary>
        /// Create InvoiceDetailFromToInvoicesDateIncome binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceDetailFromToInvoicesDateIncome(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceDetailsBetweenDateIncome";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateIncomeTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateIncomeTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceDetailsBetweenDateIncome, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceDetailsBetweenDateIncome", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Income Details From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceDetailFromToInvoicesDateNotPaid report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceDetailFromToInvoicesDateNotPaidShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Detail.FromToInvoiceDateNotPaid.rdlc",
                "Invoice Not Paid Date Interval Details");
        }

        /// <summary>
        /// Create InvoiceDetailFromToInvoicesDateNotPaid binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceDetailFromToInvoicesDateNotPaid(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceDetailsBetweenDateNotPaid";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateNotPaidTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceDetailsBetweenDateNotPaidTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceDetailsBetweenDateNotPaid, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceDetailsBetweenDateNotPaid", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Not Paid Details From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }
    }
}