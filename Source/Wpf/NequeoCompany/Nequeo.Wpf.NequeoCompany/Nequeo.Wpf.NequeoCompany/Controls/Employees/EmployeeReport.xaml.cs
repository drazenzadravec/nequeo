using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nequeo.Wpf.NequeoCompany.Controls.Employees
{
    /// <summary>
    /// Interaction logic for EmployeeReport.xaml
    /// </summary>
    public partial class EmployeeReport : UserControl
    {
        /// <summary>
        /// Employee report
        /// </summary>
        public EmployeeReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IEmployee _employee;
        private Data.Enum.EnumPayPeriodIntervalType _payPeriodInterval = Data.Enum.EnumPayPeriodIntervalType.Weekly;

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Service.NequeoCompany.IEmployee EmployeeDataSource
        {
            get { return _employee; }
            set { _employee = value; }
        }

        /// <summary>
        /// Gets sets the Pay Interval calculation type.
        /// </summary>
        public Data.Enum.EnumPayPeriodIntervalType PayPeriodIntervalType
        {
            get { return _payPeriodInterval; }
            set { _payPeriodInterval = value; }
        }

        /// <summary>
        /// Loaded user control event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtFromEmployeeWageDate.SelectedDate = DateTime.Now;
            txtToEmployeeWageDate.SelectedDate = DateTime.Now;

            txtFromEmployeeSuperDate.SelectedDate = DateTime.Now;
            txtToEmployeeSuperDate.SelectedDate = DateTime.Now;

            txtEndingEmployeePaySlipDate.SelectedDate = DateTime.Now;
            txtStartEmployeePaySlipDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Employee wage report checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmployeeWageReportID_Checked(object sender, RoutedEventArgs e)
        {
            txtEmployeeWageReportID.Visibility = System.Windows.Visibility.Visible;
            btnSelectEmployeeWageReport.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Employee wage report un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmployeeWageReportID_Unchecked(object sender, RoutedEventArgs e)
        {
            txtEmployeeWageReportID.Visibility = System.Windows.Visibility.Hidden;
            btnSelectEmployeeWageReport.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Select an employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEmployeeWageReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Employees, DataAccess.NequeoCompany.Data.Employees>(_employee.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "EmployeeID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Employees data = (DataAccess.NequeoCompany.Data.Employees)selectItem.SelectedRecord;
                txtEmployeeWageReportID.Text = data.EmployeeID.ToString();
            }
        }

        /// <summary>
        /// View employee wage report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEmployeeWageReport_Click(object sender, RoutedEventArgs e)
        {
            if (lblEmployeeWageReportID.IsChecked.Value)
            {
                if (String.IsNullOrEmpty(txtEmployeeWageReportID.Text))
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _employee.Current.ReportExtension.WageFromToDate(txtFromEmployeeWageDate.SelectedDate, txtToEmployeeWageDate.SelectedDate);
                    _employee.Current.ReportExtension.WageFromToDateShow(data);
                }
                else
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _employee.Current.ReportExtension.WageFromToDateID
                        (txtFromEmployeeWageDate.SelectedDate, txtToEmployeeWageDate.SelectedDate, Int32.Parse(txtEmployeeWageReportID.Text));
                    _employee.Current.ReportExtension.WageFromToDateIDShow(data);
                }
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _employee.Current.ReportExtension.WageFromToDate(txtFromEmployeeWageDate.SelectedDate, txtToEmployeeWageDate.SelectedDate);
                _employee.Current.ReportExtension.WageFromToDateShow(data);
            }
        }

        /// <summary>
        /// View employee wage summary report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmployeeWageSummaryAll_Click(object sender, RoutedEventArgs e)
        {
            // Get customer income for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _employee.Current.ReportExtension.WageFromToDateSummary(txtFromEmployeeWageDate.SelectedDate, txtToEmployeeWageDate.SelectedDate);
            _employee.Current.ReportExtension.WageFromToDateSummaryShow(data);
        }

        /// <summary>
        /// Employee super report checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmployeeSuperReportID_Checked(object sender, RoutedEventArgs e)
        {
            txtEmployeeSuperReportID.Visibility = System.Windows.Visibility.Visible;
            btnSelectEmployeeSuperReport.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Employee super report un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmployeeSuperReportID_Unchecked(object sender, RoutedEventArgs e)
        {
            txtEmployeeSuperReportID.Visibility = System.Windows.Visibility.Hidden;
            btnSelectEmployeeSuperReport.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Select an employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEmployeeSuperReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Employees, DataAccess.NequeoCompany.Data.Employees>(_employee.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "EmployeeID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Employees data = (DataAccess.NequeoCompany.Data.Employees)selectItem.SelectedRecord;
                txtEmployeeSuperReportID.Text = data.EmployeeID.ToString();
            }
        }

        /// <summary>
        /// View employee super report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEmployeeSuperReport_Click(object sender, RoutedEventArgs e)
        {
            if (lblEmployeeSuperReportID.IsChecked.Value)
            {
                if (String.IsNullOrEmpty(txtEmployeeSuperReportID.Text))
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _employee.Current.ReportExtension.SuperFromToDate(txtFromEmployeeSuperDate.SelectedDate, txtToEmployeeSuperDate.SelectedDate);
                    _employee.Current.ReportExtension.SuperFromToDateShow(data);
                }
                else
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _employee.Current.ReportExtension.SuperFromToDateID
                        (txtFromEmployeeSuperDate.SelectedDate, txtToEmployeeSuperDate.SelectedDate, Int32.Parse(txtEmployeeSuperReportID.Text));
                    _employee.Current.ReportExtension.SuperFromToDateIDShow(data);
                }
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _employee.Current.ReportExtension.SuperFromToDate(txtFromEmployeeSuperDate.SelectedDate, txtToEmployeeSuperDate.SelectedDate);
                _employee.Current.ReportExtension.SuperFromToDateShow(data);
            }
        }

        /// <summary>
        /// View employee super summary report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmployeeSuperSummaryAll_Click(object sender, RoutedEventArgs e)
        {
            // Get customer income for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _employee.Current.ReportExtension.SuperFromToDateSummary(txtFromEmployeeSuperDate.SelectedDate, txtToEmployeeSuperDate.SelectedDate);
            _employee.Current.ReportExtension.SuperFromToDateSummaryShow(data);
        }

        /// <summary>
        /// Slect employee pay slip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEmployee_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Employees, DataAccess.NequeoCompany.Data.Employees>(_employee.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "EmployeeID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Employees data = (DataAccess.NequeoCompany.Data.Employees)selectItem.SelectedRecord;
                txtEmployeeID.Text = data.EmployeeID.ToString();
            }
        }

        /// <summary>
        /// Select company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCompany_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Companies, DataAccess.NequeoCompany.Data.Employees>(_employee.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "CompanyID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Companies data = (DataAccess.NequeoCompany.Data.Companies)selectItem.SelectedRecord;
                txtCompanyID.Text = data.CompanyID.ToString();
            }
        }

        /// <summary>
        /// View employee pay slip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEmployeePaySlipReport_Click(object sender, RoutedEventArgs e)
        {
            // Get customer income for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _employee.Current.ReportExtension.PaySlipFromToYear(
                    txtStartEmployeePaySlipDate.SelectedDate,
                    txtEndingEmployeePaySlipDate.SelectedDate, 
                    Int32.Parse(txtEmployeeID.Text),
                    Int32.Parse(txtCompanyID.Text), 
                    _payPeriodInterval);
            _employee.Current.ReportExtension.PaySlipFromToYearShow(data);
        }
    }
}
