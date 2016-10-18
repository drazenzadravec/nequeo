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

using Nequeo.Collections.Extension;
using Nequeo.Wpf.Extension;
using Nequeo.Wpf.NequeoCompany.Common;

namespace Nequeo.Wpf.NequeoCompany.Controls.Accounts
{
    /// <summary>
    /// Interaction logic for AccountReport.xaml
    /// </summary>
    public partial class AccountReport : UserControl
    {
        /// <summary>
        /// Account report window
        /// </summary>
        public AccountReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IAccount _account;
        private long _accountID = 0;

        private DataAccess.NequeoCompany.Data.DataMemberTables[] _dataMemberTables = null;
        private int _dataMemberTableID = -1;
        private int _dataMemberSelectdIndex = -1;
        private string _dataMemberName = null;

        private int _dataMemberTableIDTrans = -1;
        private int _dataMemberSelectdIndexTrans = -1;
        private string _dataMemberNameTrans = null;

        /// <summary>
        /// Gets sets, the account injected interface.
        /// </summary>
        public Service.NequeoCompany.IAccount AccountDataSource
        {
            get { return _account; }
            set { _account = value; }
        }

        /// <summary>
        /// Gets sets, the current account id.
        /// </summary>
        public long AccountID
        {
            get { return _accountID; }
            set { _accountID = value; }
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtFromAccountDate.SelectedDate = DateTime.Now;
            txtToAccountDate.SelectedDate = DateTime.Now;
            txtFromTransactionPaymentDate.SelectedDate = DateTime.Now;
            txtToTransactionPaymentDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Select account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAccountReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Accounts, DataAccess.NequeoCompany.Data.Accounts>(_account.Current.Extension.Common);

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
                txtAccountReportID.Text = data.AccountID.ToString();
            }
        }

        /// <summary>
        /// View account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAccountReport_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtAccountReportID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _account.Current.ReportExtension.Account();
                _account.Current.ReportExtension.AccountShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _account.Current.ReportExtension.Account(Int32.Parse(txtAccountReportID.Text));
                _account.Current.ReportExtension.AccountShow(data);
            }
        }

        /// <summary>
        /// Select account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAccount_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Accounts, DataAccess.NequeoCompany.Data.Accounts>(_account.Current.Extension.Common);

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
        /// View transaction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxDataMember.IsChecked.Value && !String.IsNullOrEmpty(_dataMemberName))
            {
                if (checkBoxMemberID.IsChecked.Value)
                {
                    // Get transaction for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _account.Current.ReportExtension.TransactionFromToDatePaidToByAndID(
                            txtFromAccountDate.SelectedDate, txtToAccountDate.SelectedDate, Int32.Parse(txtAccountID.Text), 
                            _dataMemberName, Int32.Parse(txtAccountMemberID.Text));
                    _account.Current.ReportExtension.TransactionFromToDatePaidToByAndIDShow(data);
                }
                else
                {
                    // Get transaction for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _account.Current.ReportExtension.TransactionFromToDatePaidToBy(
                            txtFromAccountDate.SelectedDate, txtToAccountDate.SelectedDate, Int32.Parse(txtAccountID.Text), _dataMemberName);
                    _account.Current.ReportExtension.TransactionFromToDatePaidToByShow(data);
                }
            }
            else
            {
                // Get transaction for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _account.Current.ReportExtension.TransactionFromToDate(
                        txtFromAccountDate.SelectedDate, txtToAccountDate.SelectedDate,  Int32.Parse(txtAccountID.Text));
                _account.Current.ReportExtension.TransactionFromToDateShow(data);
            }
        }

        /// <summary>
        /// Data member checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxDataMember_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxMemberID.Visibility = System.Windows.Visibility.Visible;
            txtDataMember.Visibility = System.Windows.Visibility.Visible;
            LoadListItems();
        }

        /// <summary>
        /// Data member un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxDataMember_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxMemberID.IsChecked = false;
            checkBoxMemberID.Visibility = System.Windows.Visibility.Hidden;
            txtDataMember.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Member id checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxMemberID_Checked(object sender, RoutedEventArgs e)
        {
            txtAccountMemberID.Visibility = System.Windows.Visibility.Visible;
            btnSelectAccountMemberID.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Member id un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxMemberID_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAccountMemberID.Visibility = System.Windows.Visibility.Hidden;
            btnSelectAccountMemberID.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Data member selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDataMember_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Load the data member table data.
            if (_dataMemberTables == null)
                _dataMemberTables = _account.Current.Extension.DataContext.DataMemberTables.Where(u => u.DataMemberID > -1).ToArray();

            // Get and assign the value from the selected item.
            _dataMemberName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.DataMember, string>("Name");

            // Attempt to find the data member.
            DataAccess.NequeoCompany.Data.DataMemberTables dataMember = null;
            try {
                dataMember = _dataMemberTables.First(u => u.TableName.ToLower() == _dataMemberName.ToLower()); }
            catch { }

            // Get the text to display
            string selectText = (dataMember != null ? "Select " + dataMember.NameTo : string.Empty);

            // Assign the text for the transaction type slection button
            if (String.IsNullOrEmpty(selectText))
            {
                btnSelectAccountMemberID.IsEnabled = false;
                _dataMemberTableID = -1;
            }
            else
            {
                btnSelectAccountMemberID.IsEnabled = true;
                _dataMemberTableID = dataMember.DataMemberID;
            }

            txtAccountMemberID.Text = "1";
            _dataMemberSelectdIndex = txtDataMember.SelectedIndex;
        }

        /// <summary>
        /// Load all relevant list items into controls.
        /// </summary>
        private void LoadListItems()
        {
            // If no items exists then load.
            if (txtDataMember.Items.Count < 1)
                txtDataMember.ItemsSource = _account.Current.Extension3.GetDataMemberList();
        }

        /// <summary>
        /// Load all relevant list items into controls.
        /// </summary>
        private void LoadListItemsTrans()
        {
            // If no items exists then load.
            if (txtTransactionPaymentDataMember.Items.Count < 1)
                txtTransactionPaymentDataMember.ItemsSource = _account.Current.Extension3.GetDataMemberList();
        }

        /// <summary>
        /// Select the account data member id.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAccountMemberID_Click(object sender, RoutedEventArgs e)
        {
            // Attempt to find the data member.
            DataAccess.NequeoCompany.Data.DataMemberTables dataMember = null;
            try{
                dataMember = _dataMemberTables.First(u => u.DataMemberID == _dataMemberTableID);}
            catch { }

            // If a data member has been found
            if (dataMember != null)
            {
                // Find the table name.
                string tableName = _dataMemberTables.First(u => u.DataMemberID.ToString().ToLower() == dataMember.Reference.ToLower()).DataTables;

                // Find the table key name
                string tableKeyName = _dataMemberTables.First(u => u.DataMemberID.ToString().ToLower() == dataMember.Reference.ToLower()).DataTableKeyName;

                // Get a new instance of the connection type model.
                Nequeo.ComponentModel.ConnectionTypeModel connectionModel = 
                    new ComponentModel.ConnectionTypeModel(
                        "Nequeo.DataAccess.NequeoCompany.Data." + tableName + ",Nequeo.NequeoCompany.Model",
                        _account.Current.Extension.Common.ConfigurationDatabaseConnection,
                        _account.Current.Extension.Common.ConnectionType, 
                        _account.Current.Extension.Common.ConnectionDataType,
                        _account.Current.Extension.Common.DataAccessProvider.GetType().AssemblyQualifiedName);

                // Show the selection form
                Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
                selectItem.ConnectionTypeModel = connectionModel;
                selectItem.LoadOnStart = true;
                selectItem.MaxRecords = 50;
                selectItem.OrderByClause = tableKeyName + " DESC";
                selectItem.ShowDialog();

                // Has an item been selected.
                if (selectItem.SelectedRecord != null)
                {
                    // Get the selected item.
                    object data = selectItem.SelectedRecord;
                    txtAccountMemberID.Text = data.GetType().GetProperty(tableKeyName).GetValue(data, null).ToString();
                }
            }
        }

        /// <summary>
        /// Transaction data meber changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTransactionPaymentDataMember_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Load the data member table data.
            if (_dataMemberTables == null)
                _dataMemberTables = _account.Current.Extension.DataContext.DataMemberTables.Where(u => u.DataMemberID > -1).ToArray();

            // Get and assign the value from the selected item.
            _dataMemberNameTrans = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.DataMember, string>("Name");

            // Attempt to find the data member.
            DataAccess.NequeoCompany.Data.DataMemberTables dataMember = null;
            try
            {
                dataMember = _dataMemberTables.First(u => u.TableName.ToLower() == _dataMemberNameTrans.ToLower());
            }
            catch { }

            // Get the text to display
            string selectText = (dataMember != null ? "Select " + dataMember.NameTo : string.Empty);

            // Assign the text for the transaction type slection button
            if (String.IsNullOrEmpty(selectText))
            {
                btnSelectTransactionPaymentMemberID.IsEnabled = false;
                _dataMemberTableIDTrans = -1;
            }
            else
            {
                btnSelectTransactionPaymentMemberID.IsEnabled = true;
                _dataMemberTableIDTrans = dataMember.DataMemberID;
            }

            txtTransactionPaymentMemberID.Text = "1";
            _dataMemberSelectdIndexTrans = txtTransactionPaymentDataMember.SelectedIndex;
        }

        /// <summary>
        /// Transaction data member checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxTransactionPaymentDataMember_Checked(object sender, RoutedEventArgs e)
        {
            LoadListItemsTrans();
        }

        /// <summary>
        /// Transaction data member un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxTransactionPaymentDataMember_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// View transaction payment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewTransactionPaymentReport_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxTransactionPaymentDataMember.IsChecked.Value && !String.IsNullOrEmpty(_dataMemberNameTrans))
            {
                if (checkBoxTransactionPaymentMemberID.IsChecked.Value)
                {
                    // Get transaction for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _account.Current.ReportExtension.TransactionFromToDataMemberDateID(
                            txtFromTransactionPaymentDate.SelectedDate, txtToTransactionPaymentDate.SelectedDate,
                            _dataMemberNameTrans, Int32.Parse(txtTransactionPaymentMemberID.Text));
                    _account.Current.ReportExtension.TransactionFromToDataMemberDateIDShow(data);
                }
                else
                {
                    // Get transaction for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _account.Current.ReportExtension.TransactionFromToDataMemberDate(
                            txtFromTransactionPaymentDate.SelectedDate, txtToTransactionPaymentDate.SelectedDate, _dataMemberNameTrans);
                    _account.Current.ReportExtension.TransactionFromToDataMemberDateShow(data);
                }
            }
            else
            {
                // Get transaction for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _account.Current.ReportExtension.TransactionFromToDate(
                        txtFromTransactionPaymentDate.SelectedDate, txtToTransactionPaymentDate.SelectedDate, Int32.Parse(_accountID.ToString()));
                _account.Current.ReportExtension.TransactionFromToDateShow(data);
            }
        }

        /// <summary>
        /// Transaction data member id checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxTransactionPaymentMemberID_Checked(object sender, RoutedEventArgs e)
        {
            txtTransactionPaymentMemberID.Visibility = System.Windows.Visibility.Visible;
            btnSelectTransactionPaymentMemberID.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Transaction data member id un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxTransactionPaymentMemberID_Unchecked(object sender, RoutedEventArgs e)
        {
            txtTransactionPaymentMemberID.Visibility = System.Windows.Visibility.Hidden;
            btnSelectTransactionPaymentMemberID.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Dlect member id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectTransactionPaymentMemberID_Click(object sender, RoutedEventArgs e)
        {
            // Attempt to find the data member.
            DataAccess.NequeoCompany.Data.DataMemberTables dataMember = null;
            try
            {
                dataMember = _dataMemberTables.First(u => u.DataMemberID == _dataMemberTableIDTrans);
            }
            catch { }

            // If a data member has been found
            if (dataMember != null)
            {
                // Find the table name.
                string tableName = _dataMemberTables.First(u => u.DataMemberID.ToString().ToLower() == dataMember.Reference.ToLower()).DataTables;

                // Find the table key name
                string tableKeyName = _dataMemberTables.First(u => u.DataMemberID.ToString().ToLower() == dataMember.Reference.ToLower()).DataTableKeyName;

                // Get a new instance of the connection type model.
                Nequeo.ComponentModel.ConnectionTypeModel connectionModel =
                    new ComponentModel.ConnectionTypeModel(
                        "Nequeo.DataAccess.NequeoCompany.Data." + tableName + ",Nequeo.NequeoCompany.Model",
                        _account.Current.Extension.Common.ConfigurationDatabaseConnection,
                        _account.Current.Extension.Common.ConnectionType,
                        _account.Current.Extension.Common.ConnectionDataType,
                        _account.Current.Extension.Common.DataAccessProvider.GetType().AssemblyQualifiedName);

                // Show the selection form
                Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
                selectItem.ConnectionTypeModel = connectionModel;
                selectItem.LoadOnStart = true;
                selectItem.MaxRecords = 50;
                selectItem.OrderByClause = tableKeyName + " DESC";
                selectItem.ShowDialog();

                // Has an item been selected.
                if (selectItem.SelectedRecord != null)
                {
                    // Get the selected item.
                    object data = selectItem.SelectedRecord;
                    txtTransactionPaymentMemberID.Text = data.GetType().GetProperty(tableKeyName).GetValue(data, null).ToString();
                }
            }
        }
    }
}
