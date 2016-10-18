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
    /// Interaction logic for AccountReport.xaml
    /// </summary>
    public partial class AccountReport : UserControl
    {
        /// <summary>
        /// Employee account report
        /// </summary>
        public AccountReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IEmployee _employee;

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Service.NequeoCompany.IEmployee EmployeeDataSource
        {
            get { return _employee; }
            set { _employee = value; }
        }

        /// <summary>
        /// Loaded user control event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtFromEmployeeExtraDate.SelectedDate = DateTime.Now;
            txtToEmployeeExtraDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Select employee super fund
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEmployeeFundReport_Click(object sender, RoutedEventArgs e)
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
                txtEmployeeFundReportID.Text = data.EmployeeID.ToString();
            }
        }

        /// <summary>
        /// Select employee super fund
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEmployeeFundReport_Click(object sender, RoutedEventArgs e)
        {
            // Get customer income for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _employee.Current.ReportExtension.SuperAccount(Int32.Parse(txtEmployeeFundReportID.Text));
            _employee.Current.ReportExtension.SuperAccountShow(data);
        }

        /// <summary>
        /// Select employee bank account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEmployeeBankReport_Click(object sender, RoutedEventArgs e)
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
                txtEmployeeBankReportID.Text = data.EmployeeID.ToString();
            }
        }

        /// <summary>
        /// Select employee bank account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEmployeeBankReport_Click(object sender, RoutedEventArgs e)
        {
            // Get customer income for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _employee.Current.ReportExtension.BankAccount(Int32.Parse(txtEmployeeBankReportID.Text));
            _employee.Current.ReportExtension.BankAccountShow(data);
        }

        /// <summary>
        /// Employee report checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmployeeExtraReportID_Checked(object sender, RoutedEventArgs e)
        {
            txtEmployeeExtraReportID.Visibility = System.Windows.Visibility.Visible;
            btnSelectEmployeeExtraReport.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Employee report un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmployeeExtraReportID_Unchecked(object sender, RoutedEventArgs e)
        {
            txtEmployeeExtraReportID.Visibility = System.Windows.Visibility.Hidden;
            btnSelectEmployeeExtraReport.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Select employee extra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEmployeeExtraReport_Click(object sender, RoutedEventArgs e)
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
                txtEmployeeExtraReportID.Text = data.EmployeeID.ToString();
            }
        }

        /// <summary>
        /// Select employee extra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEmployeeExtraReport_Click(object sender, RoutedEventArgs e)
        {
            if (lblEmployeeExtraReportID.IsChecked.Value)
            {
                if (String.IsNullOrEmpty(txtEmployeeExtraReportID.Text))
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _employee.Current.ReportExtension.PAYGExtra(txtFromEmployeeExtraDate.SelectedDate, txtToEmployeeExtraDate.SelectedDate);
                    _employee.Current.ReportExtension.PAYGExtraShow(data);
                }
                else
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _employee.Current.ReportExtension.PAYG
                        (txtFromEmployeeExtraDate.SelectedDate, txtToEmployeeExtraDate.SelectedDate, Int32.Parse(txtEmployeeExtraReportID.Text));
                    _employee.Current.ReportExtension.PAYGShow(data);
                }
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _employee.Current.ReportExtension.PAYGExtra(txtFromEmployeeExtraDate.SelectedDate, txtToEmployeeExtraDate.SelectedDate);
                _employee.Current.ReportExtension.PAYGExtraShow(data);
            }
        }

        /// <summary>
        /// Employee extra summary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSummaryAll_Click(object sender, RoutedEventArgs e)
        {
            // Get customer income for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _employee.Current.ReportExtension.PAYGExtra(txtFromEmployeeExtraDate.SelectedDate, txtToEmployeeExtraDate.SelectedDate);
            _employee.Current.ReportExtension.PAYGExtraShow(data);
        }
    }
}
