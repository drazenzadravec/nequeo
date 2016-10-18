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

namespace Nequeo.Report.NequeoCompany.Account
{
    /// <summary>
    /// Account reports
    /// </summary>
    public partial class Accounts
    {
        /// <summary>
        /// Show TransactionFromToDate report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void TransactionFromToDateShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Account.Transaction.FromToDate.rdlc",
                "Account Transaction");
        }

        /// <summary>
        /// Show TransactionFromToDataMemberDate report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void TransactionFromToDataMemberDateShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Account.Transaction.FromToDataMemberDate.rdlc",
                "Account Transaction Payment");
        }

        /// <summary>
        /// Show TransactionFromToDataMemberDateID report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void TransactionFromToDataMemberDateIDShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Account.Transaction.FromToDataMemberDateID.rdlc",
                "Account Transaction Payment");
        }

        /// <summary>
        /// Show TransactionFromToDatePaidToBy report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void TransactionFromToDatePaidToByShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Account.Transaction.FromToDatePaidToBy.rdlc",
                "Account Transaction");
        }

        /// <summary>
        /// Show TransactionFromToDatePaidToByAndID report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void TransactionFromToDatePaidToByAndIDShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Account.Transaction.FromToDatePaidToByAndID.rdlc",
                "Account Transaction");
        }

        /// <summary>
        /// Create TransactionFromToDate binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="accountID">The accountID value</param>
        /// <returns><returns>The report binding source collection.</returns></returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] TransactionFromToDate(DateTime? fromDate, DateTime? toDate, int? accountID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetAccountTransactionsFromToDate";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetAccountTransactionCreditsFromToDate";
            bindingSource1.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionCreditsFromToDate(fromDate, toDate, accountID);

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetAccountTransactionDebitsFromToDate";
            bindingSource2.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionDebitsFromToDate(fromDate, toDate, accountID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsFromToDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsFromToDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetAccountTransactionsFromToDate, fromDate, toDate, accountID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetAccountTransactionsFromToDate" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetAccountTransactionCreditsFromToDate" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetAccountTransactionDebitsFromToDate" },
            };
        }

        /// <summary>
        /// Create TransactionFromToDataMemberDate binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="dataMember">The dataMember value</param>
        /// <returns><returns>The report binding source collection.</returns></returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] TransactionFromToDataMemberDate(DateTime? fromDate, DateTime? toDate, string dataMember)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetAccountTransactionsMemberFromToDate";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetAccountTransactionCreditsMemberFromToDate";
            bindingSource1.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionCreditsMemberFromToDate(fromDate, toDate, dataMember);

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetAccountTransactionDebitsMemberFromToDate";
            bindingSource2.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionDebitsMemberFromToDate(fromDate, toDate, dataMember);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsMemberFromToDateTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsMemberFromToDateTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetAccountTransactionsMemberFromToDate, fromDate, toDate, dataMember);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetAccountTransactionsMemberFromToDate", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = "Account Transaction Payment " + dataMember, 
                            ValueType = typeof(String) 
                        },
                    }
                },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetAccountTransactionCreditsMemberFromToDate" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetAccountTransactionDebitsMemberFromToDate" },
            };
        }

        /// <summary>
        /// Create TransactionFromToDataMemberDateID binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="dataMember">The dataMember value</param>
        /// <param name="dataMemberID">The dataMemberID value</param>
        /// <returns><returns>The report binding source collection.</returns></returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] TransactionFromToDataMemberDateID(DateTime? fromDate, DateTime? toDate, string dataMember, int? dataMemberID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetAccountTransactionsMemberFromToDateByID";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetAccountTransactionCreditsMemberFromToDateByID";
            bindingSource1.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionCreditsMemberFromToDateByID(fromDate, toDate, dataMember, dataMemberID);

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetAccountTransactionDebitsMemberFromToDateByID";
            bindingSource2.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionDebitsMemberFromToDateByID(fromDate, toDate, dataMember, dataMemberID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsMemberFromToDateByIDTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsMemberFromToDateByIDTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetAccountTransactionsMemberFromToDateByID, fromDate, toDate, dataMember, dataMemberID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetAccountTransactionsMemberFromToDateByID", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportTitle", 
                            Value = "Account Transaction Payment " + dataMember + ", Member ID " + dataMemberID.ToString(), 
                            ValueType = typeof(String) 
                        },
                    }
                },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetAccountTransactionCreditsMemberFromToDateByID" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetAccountTransactionDebitsMemberFromToDateByID" },
            };
        }

        /// <summary>
        /// Create TransactionFromToDatePaidToBy binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="accountID">The accountID value</param>
        /// <param name="dataMember">The dataMember value</param>
        /// <returns><returns>The report binding source collection.</returns></returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] TransactionFromToDatePaidToBy(DateTime? fromDate, DateTime? toDate, int? accountID, string dataMember)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetAccountTransactionsFromToDatePaidToBy";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetAccountTransactionCreditsFromToDatePaidToBy";
            bindingSource1.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionCreditsFromToDatePaidToBy(fromDate, toDate, accountID, dataMember);

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetAccountTransactionDebitsFromToDatePaidToBy";
            bindingSource2.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionDebitsFromToDatePaidToBy(fromDate, toDate, accountID, dataMember);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsFromToDatePaidToByTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsFromToDatePaidToByTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetAccountTransactionsFromToDatePaidToBy, fromDate, toDate, accountID, dataMember);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetAccountTransactionsFromToDatePaidToBy" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetAccountTransactionCreditsFromToDatePaidToBy" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetAccountTransactionDebitsFromToDatePaidToBy" },
            };
        }

        /// <summary>
        /// Create TransactionFromToDatePaidToByAndID binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="accountID">The accountID value</param>
        /// <param name="dataMember">The dataMember value</param>
        /// <param name="dataMemberID">The dataMemberID value</param>
        /// <returns><returns>The report binding source collection.</returns></returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] TransactionFromToDatePaidToByAndID(DateTime? fromDate, DateTime? toDate, int? accountID, string dataMember, int? dataMemberID)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetAccountTransactionsFromToDatePaidToByAndID";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetAccountTransactionCreditsFromToDatePaidToByAndID";
            bindingSource1.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionCreditsFromToDatePaidToByAndID(fromDate, toDate, accountID, dataMember, dataMemberID);

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetAccountTransactionDebitsFromToDatePaidToByAndID";
            bindingSource2.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.AccountTransactions().GetAccountTransactionDebitsFromToDatePaidToByAndID(fromDate, toDate, accountID, dataMember, dataMemberID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsFromToDatePaidToByAndIDTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetAccountTransactionsFromToDatePaidToByAndIDTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetAccountTransactionsFromToDatePaidToByAndID, fromDate, toDate, accountID, dataMember, dataMemberID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetAccountTransactionsFromToDatePaidToByAndID" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetAccountTransactionCreditsFromToDatePaidToByAndID" },
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetAccountTransactionDebitsFromToDatePaidToByAndID" },
            };
        }
    }
}
