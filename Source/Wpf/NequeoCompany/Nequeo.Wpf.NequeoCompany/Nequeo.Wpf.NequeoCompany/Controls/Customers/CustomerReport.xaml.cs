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

namespace Nequeo.Wpf.NequeoCompany.Controls.Customers
{
    /// <summary>
    /// Interaction logic for CustomerReport.xaml
    /// </summary>
    public partial class CustomerReport : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomerReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.ICustomer _customer;

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Service.NequeoCompany.ICustomer CustomerDataSource
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

        /// <summary>
        /// Customer report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCustomerReport_Click(object sender, RoutedEventArgs e)
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
                txtCustomerReportID.Text = data.CustomerID.ToString();
            }
        }

        /// <summary>
        /// View customer report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewCustomerReport_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCustomerReportID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.Customer();
                _customer.Current.ReportExtension.CustomerShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.Customer(Int32.Parse(txtCustomerReportID.Text));
                _customer.Current.ReportExtension.CustomerShow(data);
            }
        }
    }
}
