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
    /// Interaction logic for InvoiceReport.xaml
    /// </summary>
    public partial class InvoiceReport : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvoiceReport()
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
        }

        /// <summary>
        /// View Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewReport_Click(object sender, RoutedEventArgs e)
        {
            if (rbDetailsTaxInvoice.IsChecked.Value)
            {
                // If an empty invoice is required
                if (cbEmptyInvoice.IsChecked.Value)
                {
                    // Show the empty tax invoice details
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceDetailEmptyTaxInvoice(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceDetailEmptyTaxInvoiceShow(data);
                }
                else
                {
                    // Show the tax invoice details
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceDetailTaxInvoice(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceDetailTaxInvoiceShow(data);
                }
                
            }
            else if (rbDetailsQuotation.IsChecked.Value)
            {
                // If an empty invoice is required
                if (cbEmptyInvoice.IsChecked.Value)
                {
                    // Show the empty quotation details
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceDetailEmptyQuotation(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceDetailEmptyQuotationShow(data);
                }
                else
                {
                    // Show the auotation details
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceDetailQuotation(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceDetailQuotationShow(data);
                }
                
            }
            else if (rbProductTaxInvoice.IsChecked.Value)
            {
                // If an empty invoice is required
                if (cbEmptyInvoice.IsChecked.Value)
                {
                    // Show the empty tax invoice product
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceProductEmptyTaxInvoice(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceProductEmptyTaxInvoiceShow(data);
                }
                else
                {
                    // Show the tax invoice product
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceProductTaxInvoice(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceProductTaxInvoiceShow(data);
                }
                
            }
            else if (rbProductQuotation.IsChecked.Value)
            {
                // If an empty invoice is required
                if (cbEmptyInvoice.IsChecked.Value)
                {
                    // Show the empty quotation product
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceProductEmptyQuotation(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceProductEmptyQuotationShow(data);
                }
                else
                {
                    // Show the quotation product
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceProductQuotation(
                            Int32.Parse(txtInvoiceID.Text), Int32.Parse(txtCompanyID.Text), Int32.Parse(txtAccountID.Text));
                    _customer.Current.ReportExtension.InvoiceProductQuotationShow(data);
                }
            }
        }

        /// <summary>
        /// Select an invoice event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectInvoice_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                    <DataAccess.NequeoCompany.Data.Invoices, DataAccess.NequeoCompany.Data.Customers>(_customer.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "InvoiceID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Invoices data = (DataAccess.NequeoCompany.Data.Invoices)selectItem.SelectedRecord;
                txtInvoiceID.Text = data.InvoiceID.ToString();
            }
        }

        /// <summary>
        /// Select a company event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCompany_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                    <DataAccess.NequeoCompany.Data.Companies, DataAccess.NequeoCompany.Data.Customers>(_customer.Current.Extension.Common);

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
        /// Select an account event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAccount_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                    <DataAccess.NequeoCompany.Data.Accounts, DataAccess.NequeoCompany.Data.Customers>(_customer.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "AccountID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Accounts data = (DataAccess.NequeoCompany.Data.Accounts)selectItem.SelectedRecord;
                txtAccountID.Text = data.AccountID.ToString();
            }
        }

        /// <summary>
        /// Payment invoice checked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPaymentInvoice_Checked(object sender, RoutedEventArgs e)
        {
            rbInvoicePaid.Visibility = System.Windows.Visibility.Visible;
            rbInvoiceNotPaid.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Payment invoice un-checked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPaymentInvoice_Unchecked(object sender, RoutedEventArgs e)
        {
            rbInvoicePaid.Visibility = System.Windows.Visibility.Hidden;
            rbInvoiceNotPaid.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Interval report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDateInterval_Click(object sender, RoutedEventArgs e)
        {
            if (cbPaymentInvoice.IsChecked.Value)
            {
                if (rbInvoicePaid.IsChecked.Value)
                {
                    if (rbAllInvoiceDate.IsChecked.Value)
                    {
                        // Show the not paid summary
                        Nequeo.Model.DataSource.BindingSourceData[] data =
                            _customer.Current.ReportExtension.InvoiceFromToInvoicesDateIncome(
                                txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                        _customer.Current.ReportExtension.InvoiceFromToInvoicesDateIncomeShow(data);
                    }
                    if (rbProductInvoiceDate.IsChecked.Value)
                    {
                        // Show the not paid summary
                        Nequeo.Model.DataSource.BindingSourceData[] data =
                            _customer.Current.ReportExtension.InvoiceProductFromToInvoicesDateIncome(
                                txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                        _customer.Current.ReportExtension.InvoiceProductFromToInvoicesDateIncomeShow(data);
                    }
                    if (rbDetailsInvoiceDate.IsChecked.Value)
                    {
                        // Show the not paid summary
                        Nequeo.Model.DataSource.BindingSourceData[] data =
                            _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDateIncome(
                                txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                        _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDateIncomeShow(data);
                    }
                }
                if (rbInvoiceNotPaid.IsChecked.Value)
                {
                    if (rbAllInvoiceDate.IsChecked.Value)
                    {
                        // Show the not paid summary
                        Nequeo.Model.DataSource.BindingSourceData[] data =
                            _customer.Current.ReportExtension.InvoiceFromToInvoicesDateNotPaid(
                                txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                        _customer.Current.ReportExtension.InvoiceFromToInvoicesDateNotPaidShow(data);
                    }
                    if (rbProductInvoiceDate.IsChecked.Value)
                    {
                        // Show the not paid summary
                        Nequeo.Model.DataSource.BindingSourceData[] data =
                            _customer.Current.ReportExtension.InvoiceProductFromToInvoicesDateNotPaid(
                                txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                        _customer.Current.ReportExtension.InvoiceProductlFromToInvoicesDateNotPaidShow(data);
                    }
                    if (rbDetailsInvoiceDate.IsChecked.Value)
                    {
                        // Show the not paid summary
                        Nequeo.Model.DataSource.BindingSourceData[] data =
                            _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDateNotPaid(
                                txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                        _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDateNotPaidShow(data);
                    }
                }
            }
            else
            {
                if (rbAllInvoiceDate.IsChecked.Value)
                {
                    // Show the not paid summary
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceFromToInvoicesDate(
                            txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                    _customer.Current.ReportExtension.InvoiceFromToInvoicesDateShow(data);
                }
                if (rbProductInvoiceDate.IsChecked.Value)
                {
                    // Show the not paid summary
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceProductFromToInvoicesDate(
                            txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                    _customer.Current.ReportExtension.InvoiceProductFromToInvoicesDateShow(data);
                }
                if (rbDetailsInvoiceDate.IsChecked.Value)
                {
                    // Show the not paid summary
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDate(
                            txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                    _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDateShow(data);
                }
            }
        }

        /// <summary>
        /// Summary report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSummaryAll_Click(object sender, RoutedEventArgs e)
        {
            if (cbPaymentInvoice.IsChecked.Value)
            {
                if (rbInvoicePaid.IsChecked.Value)
                {
                    // Show the paid income summary
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceFromToDateSummaryIncome(
                            txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                    _customer.Current.ReportExtension.InvoiceFromToDateSummaryIncomeShow(data);
                }
                if (rbInvoiceNotPaid.IsChecked.Value)
                {
                    // Show the not paid summary
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _customer.Current.ReportExtension.InvoiceFromToDateSummaryNotPaid(
                            txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                    _customer.Current.ReportExtension.InvoiceFromToDateSummaryNotPaidShow(data);
                }
            }
            else
            {
                // Show the summary
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.InvoiceFromToDateSummary(
                        txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                _customer.Current.ReportExtension.InvoiceFromToDateSummaryShow(data);
            }
        }

        /// <summary>
        /// All income for the interval report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAllIncome_Click(object sender, RoutedEventArgs e)
        {
            // Show the paid income summary
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _customer.Current.ReportExtension.IncomeFromToDate(
                    txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
            _customer.Current.ReportExtension.IncomeFromToDateShow(data);
        }

        /// <summary>
        /// Income for the interval report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtIncomeInterval_Click(object sender, RoutedEventArgs e)
        {
            if (rbAllInvoiceDate.IsChecked.Value)
            {
                // Show the not paid summary
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.InvoiceFromToInvoicesDateIncome(
                        txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                _customer.Current.ReportExtension.InvoiceFromToInvoicesDateIncomeShow(data);
            }
            if (rbProductInvoiceDate.IsChecked.Value)
            {
                // Show the not paid summary
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.InvoiceProductFromToInvoicesDateIncome(
                        txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                _customer.Current.ReportExtension.InvoiceProductFromToInvoicesDateIncomeShow(data);
            }
            if (rbDetailsInvoiceDate.IsChecked.Value)
            {
                // Show the not paid summary
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDateIncome(
                        txtFromInvoiceDate.SelectedDate, txtToInvoiceDate.SelectedDate);
                _customer.Current.ReportExtension.InvoiceDetailFromToInvoicesDateIncomeShow(data);
            }
        }
    }
}
