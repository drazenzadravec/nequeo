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

namespace Nequeo.Wpf.NequeoCompany.Controls.Companies
{
    /// <summary>
    /// Interaction logic for CompanyReport.xaml
    /// </summary>
    public partial class CompanyReport : UserControl
    {
        /// <summary>
        /// Company reports
        /// </summary>
        public CompanyReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.ICompany _company;
        private Logic.NequeoCompany.Customer.ICustomers _customer;

        /// <summary>
        /// Gets sets, the company injected interface.
        /// </summary>
        public Service.NequeoCompany.ICompany CompanyDataSource
        {
            get { return _company; }
            set { _company = value; }
        }

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Logic.NequeoCompany.Customer.ICustomers CustomerDataSource
        {
            get { return _customer; }
            set { _customer = value; }
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtFromInvoiceDate.SelectedDate = DateTime.Now;
            txtToInvoiceDate.SelectedDate = DateTime.Now;
            txtFromInvoiceTaxDate.SelectedDate = DateTime.Now;
            txtToInvoiceTaxDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Select a company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCompanyReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Companies, DataAccess.NequeoCompany.Data.Companies>(_company.Current.Extension.Common);

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
                txtCompanyReportID.Text = data.CompanyID.ToString();
            }
        }

        /// <summary>
        /// View the company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewCompanyReport_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCompanyReportID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _company.Current.ReportExtension.Company();
                _company.Current.ReportExtension.CompanyShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _company.Current.ReportExtension.Company(Int32.Parse(txtCompanyReportID.Text));
                _company.Current.ReportExtension.CompanyShow(data);
            }
        }

        /// <summary>
        /// Select a customer event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Customers, DataAccess.NequeoCompany.Data.Customers>(_customer.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "CustomerID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Customers data = (DataAccess.NequeoCompany.Data.Customers)selectItem.SelectedRecord;
                txtCustomerID.Text = data.CustomerID.ToString();
            }
        }

        /// <summary>
        /// Customer date interval invoice checked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCustomerDateInterval_Checked(object sender, RoutedEventArgs e)
        {
            gridCustomerInvocieTax.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Customer date interval invoice un-checked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCustomerDateInterval_Unchecked(object sender, RoutedEventArgs e)
        {
            gridCustomerInvocieTax.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// View income report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewReport_Click(object sender, RoutedEventArgs e)
        {
            if (cbCustomerDateInterval.IsChecked.Value)
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.IncomeFromToDateID(
                    txtFromInvoiceTaxDate.SelectedDate, txtToInvoiceTaxDate.SelectedDate, Int32.Parse(txtCustomerID.Text));
                _customer.Current.ReportExtension.IncomeFromToDateIDShow(data);
            }
            else
            {
                // Get customer income for the customer.
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.IncomeFromToDateID(Int32.Parse(txtCustomerID.Text));
                _customer.Current.ReportExtension.IncomeFromToDateIDShow(data);
            }
        }

        /// <summary>
        /// View all income report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAllReport_Click(object sender, RoutedEventArgs e)
        {
            // Show the paid income summary
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _customer.Current.ReportExtension.IncomeFromToDate(
                    txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
            _customer.Current.ReportExtension.IncomeFromToDateShow(data);
        }
    }
}
