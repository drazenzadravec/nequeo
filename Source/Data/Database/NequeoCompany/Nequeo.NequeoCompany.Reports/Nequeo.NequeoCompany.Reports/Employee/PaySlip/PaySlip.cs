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

namespace Nequeo.Report.NequeoCompany.Employee
{
    /// <summary>
    /// Employee reports
    /// </summary>
    public partial class Employees
    {
        /// <summary>
        /// Show PaySlipFromToYear report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        public virtual void PaySlipFromToYearShow(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection)
        {
            // Display the report
            ShowEmbedded(
                reportBindingSourceCollection,
                "Nequeo.Report.NequeoCompany.Employee.PaySlip.FromToYear.rdlc",
                "Employee Pay Slip");
        }

        /// <summary>
        /// Create PaySlipFromToYear binding s ource data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="employeeID">The employeeID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="payPeriodFromDate">The payPeriodFromDate value</param>
        /// <param name="payPeriodToDate">The payPeriodToDate value</param>
        /// <returns><returns>The report binding source collection.</returns></returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] PaySlipFromToYear(
            DateTime? fromDate, DateTime? toDate, int? employeeID, int? companyID, DateTime? payPeriodFromDate, DateTime? payPeriodToDate)
        {
            // Assign the binding sources
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures dataSet = new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProcedures();

            System.Windows.Forms.BindingSource bindingSource = new System.Windows.Forms.BindingSource();
            bindingSource.DataMember = "GetSelectedEmployeePAYGPaymentSlipBetweenYear";
            bindingSource.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource1 = new System.Windows.Forms.BindingSource();
            bindingSource1.DataMember = "GetSelectedEmployeeSuperPaymentSlipBetweenYear";
            bindingSource1.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource2 = new System.Windows.Forms.BindingSource();
            bindingSource2.DataMember = "GetSelectedEmployeeWagesPaymentSlipBetweenYear";
            bindingSource2.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource3 = new System.Windows.Forms.BindingSource();
            bindingSource3.DataMember = "Employees";
            bindingSource3.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Employees().GetEmployee((int)employeeID);

            System.Windows.Forms.BindingSource bindingSource4 = new System.Windows.Forms.BindingSource();
            bindingSource4.DataMember = "Companies";
            bindingSource4.DataSource = new Nequeo.DataAccess.NequeoCompany.Data.Extension.Companies().GetCompany((int)companyID);

            System.Windows.Forms.BindingSource bindingSource5 = new System.Windows.Forms.BindingSource();
            bindingSource5.DataMember = "GetSelectedEmployeeWagesPaymentSlipBetweenDate";
            bindingSource5.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource6 = new System.Windows.Forms.BindingSource();
            bindingSource6.DataMember = "GetSelectedEmployeeSuperPaymentSlipBetweenDate";
            bindingSource6.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource7 = new System.Windows.Forms.BindingSource();
            bindingSource7.DataMember = "GetSelectedEmployeePAYGPaymentSlipBetweenDate";
            bindingSource7.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource8 = new System.Windows.Forms.BindingSource();
            bindingSource8.DataMember = "GetSelectedEmployeeWagesPaymentSlipBetweenDateALL";
            bindingSource8.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource9 = new System.Windows.Forms.BindingSource();
            bindingSource9.DataMember = "GetSelectedEmployeeSuperPaymentSlipBetweenDateALL";
            bindingSource9.DataSource = dataSet;

            System.Windows.Forms.BindingSource bindingSource10 = new System.Windows.Forms.BindingSource();
            bindingSource10.DataMember = "GetSelectedEmployeePAYGPaymentSlipBetweenDateALL";
            bindingSource10.DataSource = dataSet;

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeePAYGPaymentSlipBetweenYearTableAdapter tableAdapter =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeePAYGPaymentSlipBetweenYearTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataSet.GetSelectedEmployeePAYGPaymentSlipBetweenYear, fromDate, toDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeSuperPaymentSlipBetweenYearTableAdapter tableAdapter1 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeSuperPaymentSlipBetweenYearTableAdapter();
            tableAdapter1.ClearBeforeFill = true;
            tableAdapter1.Fill(dataSet.GetSelectedEmployeeSuperPaymentSlipBetweenYear, fromDate, toDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeWagesPaymentSlipBetweenYearTableAdapter tableAdapter2 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeWagesPaymentSlipBetweenYearTableAdapter();
            tableAdapter2.ClearBeforeFill = true;
            tableAdapter2.Fill(dataSet.GetSelectedEmployeeWagesPaymentSlipBetweenYear, fromDate, toDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeWagesPaymentSlipBetweenDateTableAdapter tableAdapter3 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeWagesPaymentSlipBetweenDateTableAdapter();
            tableAdapter3.ClearBeforeFill = true;
            tableAdapter3.Fill(dataSet.GetSelectedEmployeeWagesPaymentSlipBetweenDate, payPeriodFromDate, payPeriodToDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeSuperPaymentSlipBetweenDateTableAdapter tableAdapter4 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeSuperPaymentSlipBetweenDateTableAdapter();
            tableAdapter4.ClearBeforeFill = true;
            tableAdapter4.Fill(dataSet.GetSelectedEmployeeSuperPaymentSlipBetweenDate, payPeriodFromDate, payPeriodToDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeePAYGPaymentSlipBetweenDateTableAdapter tableAdapter5 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeePAYGPaymentSlipBetweenDateTableAdapter();
            tableAdapter5.ClearBeforeFill = true;
            tableAdapter5.Fill(dataSet.GetSelectedEmployeePAYGPaymentSlipBetweenDate, payPeriodFromDate, payPeriodToDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeWagesPaymentSlipBetweenDateALLTableAdapter tableAdapter6 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeWagesPaymentSlipBetweenDateALLTableAdapter();
            tableAdapter6.ClearBeforeFill = true;
            tableAdapter6.Fill(dataSet.GetSelectedEmployeeWagesPaymentSlipBetweenDateALL, payPeriodFromDate, payPeriodToDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeSuperPaymentSlipBetweenDateALLTableAdapter tableAdapter7 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeeSuperPaymentSlipBetweenDateALLTableAdapter();
            tableAdapter7.ClearBeforeFill = true;
            tableAdapter7.Fill(dataSet.GetSelectedEmployeeSuperPaymentSlipBetweenDateALL, payPeriodFromDate, payPeriodToDate, employeeID, companyID);

            // Get the data
            Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeePAYGPaymentSlipBetweenDateALLTableAdapter tableAdapter8 =
                new Nequeo.DataAccess.NequeoCompany.DataSet.StoredProceduresTableAdapters.GetSelectedEmployeePAYGPaymentSlipBetweenDateALLTableAdapter();
            tableAdapter8.ClearBeforeFill = true;
            tableAdapter8.Fill(dataSet.GetSelectedEmployeePAYGPaymentSlipBetweenDateALL, payPeriodFromDate, payPeriodToDate, employeeID, companyID);

            // Return the data within the binding source.
            return new Model.DataSource.BindingSourceData[]
            {
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource, DataSourceName = "GetSelectedEmployeePAYGPaymentSlipBetweenYear"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource1, DataSourceName = "GetSelectedEmployeeSuperPaymentSlipBetweenYear"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource2, DataSourceName = "GetSelectedEmployeeWagesPaymentSlipBetweenYear"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource3, DataSourceName = "GetEmployee", 
                    BindingSourceParameters = new Nequeo.Model.DataSource.BindingSourceParameter[] 
                    {
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "PayPeriodEnding", 
                            Value = Convert.ToString(payPeriodToDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                        new Nequeo.Model.DataSource.BindingSourceParameter() 
                        { 
                            Name = "ReportFooterTitle", 
                            Value = "Pay Period To : " + Convert.ToString(payPeriodToDate.Value.ToShortDateString()), 
                            ValueType = typeof(String) 
                        },
                    }},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource4, DataSourceName = "GetCompany"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource5, DataSourceName = "GetSelectedEmployeeWagesPaymentSlipBetweenDate"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource6, DataSourceName = "GetSelectedEmployeeSuperPaymentSlipBetweenDate"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource7, DataSourceName = "GetSelectedEmployeePAYGPaymentSlipBetweenDate"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource8, DataSourceName = "GetSelectedEmployeeWagesPaymentSlipBetweenDateALL"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource9, DataSourceName = "GetSelectedEmployeeSuperPaymentSlipBetweenDateALL"},
                new Model.DataSource.BindingSourceData() { DataSource = bindingSource10, DataSourceName = "GetSelectedEmployeePAYGPaymentSlipBetweenDateALL"},
            };
        }
    }
}