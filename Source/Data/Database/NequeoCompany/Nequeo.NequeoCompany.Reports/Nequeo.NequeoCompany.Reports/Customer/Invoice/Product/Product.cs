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
        /// Show InvoiceProductTaxInvoice report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceProductTaxInvoiceShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Product.TaxInvoice.rdlc",
                "Tax Invoice");
        }

        /// <summary>
        /// Create InvoiceProductTaxInvoice binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceProductTaxInvoice(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceProducts";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceProducts, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceProducts"},
            };
        }

        /// <summary>
        /// Show InvoiceProductQuotation report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceProductQuotationShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Product.Quotation.rdlc",
                "Tax Invoice");
        }

        /// <summary>
        /// Create InvoiceProductQuotation binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceProductQuotation(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceProducts";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceProducts, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceProducts"},
            };
        }

        /// <summary>
        /// Show InvoiceProductFromToInvoicesDateNotPaid report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceProductlFromToInvoicesDateNotPaidShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Product.FromToInvoiceDateNotPaid.rdlc",
                "Invoice Not Paid Date Interval Products");
        }

        /// <summary>
        /// Create InvoiceProductFromToInvoicesDateNotPaid binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceProductFromToInvoicesDateNotPaid(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceProductsBetweenDateNotPaid";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateNotPaidTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateNotPaidTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceProductsBetweenDateNotPaid, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceProductsBetweenDateNotPaid", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Not Paid Products From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceProductFromToInvoicesDateIncome report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceProductFromToInvoicesDateIncomeShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Product.FromToInvoiceDateIncome.rdlc",
                "Invoice Income Date Interval Products");
        }

        /// <summary>
        /// Create InvoiceProductFromToInvoicesDateIncome binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceProductFromToInvoicesDateIncome(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceProductsBetweenDateIncome";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateIncomeTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateIncomeTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceProductsBetweenDateIncome, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceProductsBetweenDateIncome", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Income Products From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceProductFromToInvoicesDate report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceProductFromToInvoicesDateShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Product.FromToInvoiceDate.rdlc",
                "Invoice Date Interval Products");
        }

        /// <summary>
        /// Create InvoiceProductFromToInvoicesDate binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceProductFromToInvoicesDate(DateTime? fromDate, DateTime? toDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetInvoiceProductsBetweenDate";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetInvoiceProductsBetweenDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetInvoiceProductsBetweenDate, fromDate, toDate);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetInvoiceProductsBetweenDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = Convert.ToString("Invoice Products From: " + fromDate.Value.ToShortDateString() + " To: " + toDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }
                },
            };
        }

        /// <summary>
        /// Show InvoiceProductEmptyTaxInvoice report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceProductEmptyTaxInvoiceShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Product.EmptyTaxInvoice.rdlc",
                "Empty Tax Invoice");
        }

        /// <summary>
        /// Create InvoiceProductEmptyTaxInvoice binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceProductEmptyTaxInvoice(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceProducts";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceProducts, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceProducts"},
            };
        }

        /// <summary>
        /// Show InvoiceProductEmptyQuotation report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void InvoiceProductEmptyQuotationShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Customer.Invoice.Product.EmptyQuotation.rdlc",
                "Empty Quoation");
        }

        /// <summary>
        /// Create InvoiceProductEmptyQuotation binding s ource data
        /// </summary>
        /// <param name="invoiceID">The invoiceID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns>The report binding source collection.</returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] InvoiceProductEmptyQuotation(int? invoiceID, int? companyID, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();
            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedInvoiceProducts";
            bindingSource.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedInvoiceProductsTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedInvoiceProducts, invoiceID, companyID, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedInvoiceProducts"},
            };
        }
    }
}
