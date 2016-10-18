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
    /// Interaction logic for Transaction.xaml
    /// </summary>
    public partial class Transaction : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Transaction window
        /// </summary>
        public Transaction()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IAccount _account;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();
        private long _accountID = 0;
        private DataAccess.NequeoCompany.Data.DataMemberTables[] _dataMemberTables = null;
        private int _dataMemberTableID = -1;
        private int _dataMemberSelectdIndex = -1;

        /// <summary>
        /// Gets sets, the account injected interface.
        /// </summary>
        public Service.NequeoCompany.IAccount AccountDataSource
        {
            get { return _account; }
            set
            {
                _account = value;

                // Assign each dependency data source

                // Set the connection type model mapping.
                if (_account != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.AccountTransactions, DataAccess.NequeoCompany.Data.Accounts>(_account.Current.Extension.Common);
            }
        }

        /// <summary>
        /// Gets sets, the current account id.
        /// </summary>
        public long AccountID
        {
            get { return _accountID; }
            set
            {
                _accountID = value;

                if (_accountID > 0)
                {
                    dataAccess.IsLoadEnabled = true;
                    btnAddNew.IsEnabled = true;
                }
                else
                {
                    dataAccess.IsLoadEnabled = false;
                    btnAddNew.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Gets sets, has the property changed.
        /// </summary>
        public bool PropertyChanged
        {
            get { return _hasChanged; }
            set { _hasChanged = value; }
        }

        /// <summary>
        /// Get the details of the interface.
        /// </summary>
        /// <returns>The implementation details.</returns>
        public List<string> GetDetails()
        {
            List<string> details = new List<string>();

            if (PropertyChanged)
                details.Add("Account Transaction property/properties have changed.");

            return details;
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Before load data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnBeforeLoad(object sender, Custom.OperationArgs e)
        {
            // Determine if the data has change.
            if (PropertyChanged)
            {
                // Indicate that the data has changed.
                MessageBox.Show("The data has changed, please update first", "Load", MessageBoxButton.OK);
                _updateAttempt = true;
                e.Cancel = true;
            }
            else
            {
                // If in Add new state
                if (_addNew)
                {
                    // Indicate that the data has changed.
                    MessageBoxResult result = MessageBox.Show("Insert the changes before loading. Disregard the changes (all changes will be lost)?", "Load", MessageBoxButton.YesNo);
                    if (result != MessageBoxResult.Yes)
                        e.Cancel = true;
                }

                // If loading should take place.
                if (!e.Cancel)
                {
                    // Show the selection form
                    Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
                    selectItem.ConnectionTypeModel = dataAccess.ConnectionTypeModel;
                    selectItem.LoadOnStart = true;
                    selectItem.MaxRecords = 50;
                    selectItem.OrderByClause = "AccountTransactionID DESC";
                    selectItem.WhereClause = "AccountID = " + _accountID.ToString();
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.AccountTransactions data = (DataAccess.NequeoCompany.Data.AccountTransactions)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "AccountTransactionID = " + data.AccountTransactionID.ToString();
                    }
                    else
                        e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Before update data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnBeforeUpdate(object sender, Custom.OperationArgs e)
        {
            // Get the binding source data
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtTransactionID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.AccountTransactions data = (DataAccess.NequeoCompany.Data.AccountTransactions)bindingExpression.DataItem;

            // If a previous operation was attempted while
            // the data was changed an not updated.
            if (_updateAttempt)
            {
                MessageBoxResult result = MessageBox.Show("Disregard the changes (all changes will be lost)?", "Update", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    e.Cancel = true;
            }
            _updateAttempt = false;

            // Determine if the data has not change.
            if (!PropertyChanged)
                e.Cancel = true;

            // If cancel update then set property state.
            if (e.Cancel)
                SetChangePropertyState(false);
        }

        /// <summary>
        /// Before insert data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnBeforeInsert(object sender, Custom.OperationArgs e)
        {
            // Get the binding source data
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtTransactionID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.AccountTransactions data = (DataAccess.NequeoCompany.Data.AccountTransactions)bindingExpression.DataItem;
            dataAccess.DataModel = data;

            MessageBoxResult result = MessageBox.Show("Are you sure you wish to insert this record?", "Insert", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                e.Cancel = true;
        }

        /// <summary>
        /// Before delete data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnBeforeDelete(object sender, Custom.OperationArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to delete this record?", "Delete", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                e.Cancel = true;
        }

        /// <summary>
        /// On load error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnLoadError(object sender, Custom.MessageArgs e)
        {
            MessageBox.Show("Load error has occured. " + e.Message, "Load Error");
            _loading = false;
            _addNew = false;
        }

        /// <summary>
        /// On update error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnUpdateError(object sender, Custom.MessageArgs e)
        {
            MessageBox.Show("Update error has occured. " + e.Message, "Update Error");
            _loading = false;
            _addNew = false;
        }

        /// <summary>
        /// On insert error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnInsertError(object sender, Custom.MessageArgs e)
        {
            MessageBox.Show("Insert error has occured. " + e.Message, "Insert Error");
            _loading = false;
            _addNew = false;
        }

        /// <summary>
        /// On delete error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnDeleteError(object sender, Custom.MessageArgs e)
        {
            MessageBox.Show("Delete error has occured. " + e.Message, "Delete Error");
            _loading = false;
            _addNew = false;
        }

        /// <summary>
        /// On load complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnLoad(object sender, EventArgs e)
        {
            // Get the data that has been returned.
            DataAccess.NequeoCompany.Data.AccountTransactions[] returnedDataList = (DataAccess.NequeoCompany.Data.AccountTransactions[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.AccountTransactions data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridTransaction.DataContext = data;

            // Enable the controls.
            EnableDisable(0);

            // Load all list items.
            LoadListItems();

            // Start the change (load) process.
            _loading = true;
            _addNew = false;

            // Set the list selected index values.
            ListSelectedIndex(data);
            _loading = false;

            // Assign the current data member selected index.
            _dataMemberSelectdIndex = txtDataMember.SelectedIndex;
        }

        /// <summary>
        /// The value of a property has changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetChangePropertyState(true);
        }

        /// <summary>
        /// On update complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnUpdate(object sender, EventArgs e)
        {
            MessageBox.Show("Update of record succeeded.", "Update");
            SetChangePropertyState(false);
            EnableDisable(1);
        }

        /// <summary>
        /// On insert complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnInsert(object sender, EventArgs e)
        {
            MessageBox.Show("Insertion of record succeeded.", "Insert");
            EnableDisable(2);
            _loading = false;
            _addNew = false;
        }

        /// <summary>
        /// On delete complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnDelete(object sender, EventArgs e)
        {
            MessageBox.Show("Deletion of record succeeded.", "Delete");
            EnableDisable(3);
        }

        /// <summary>
        /// Add a new record.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            // Determine if the data has change.
            if (PropertyChanged)
            {
                // Indicate that the data has changed.
                MessageBox.Show("The data has changed, please update first", "Add New", MessageBoxButton.OK);
                _updateAttempt = true;
            }
            else
            {
                // Start the change (load) process.
                _addNew = true;
                _loading = false;

                // Enable the controls.
                EnableDisable(4);

                // Set the default values.
                txtTransactionID.Text = "0";
                txtAccountID.Text = _accountID.ToString();
                txtDataMemberID.Text = "";
                txtPaymentDate.SelectedDate = DateTime.Now;
                txtTransactionType.SelectedIndex = -1;
                txtAmount.Text = "";
                txtDescription.Text = "";
                txtReferenceNumber.Text = "";
                txtDataMember.SelectedIndex = -1;
                txtPaymentDataMemberID.Text = "0";
                txtPaymentDataMemberRefID.Text = "0";
                txtComments.Text = "";

                // Load all list items.
                LoadListItems();
                IsSelectedIndexValid();
            }
        }

        /// <summary>
        /// On select new transaction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectTransaction_Click(object sender, RoutedEventArgs e)
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
                string tableRefName = dataMember.DataTables;

                // Find the table key name
                string tableKeyName = _dataMemberTables.First(u => u.DataMemberID.ToString().ToLower() == dataMember.Reference.ToLower()).DataTableKeyName;
                string tableKeyRefName = dataMember.DataTableKeyName;

                // Get a new instance of the connection type model.
                Nequeo.ComponentModel.ConnectionTypeModel connectionModel =
                    Nequeo.Wpf.Common.Operation.GetTypeModel("Nequeo.DataAccess.NequeoCompany.Data." + tableName + ",Nequeo.NequeoCompany.Model", dataAccess);

                // Get a new instance of the connection type model.
                Nequeo.ComponentModel.ConnectionTypeModel connectionModelRef =
                    Nequeo.Wpf.Common.Operation.GetTypeModel("Nequeo.DataAccess.NequeoCompany.Data." + tableRefName + ",Nequeo.NequeoCompany.Model", dataAccess);

                // Show the selection form
                Nequeo.Wpf.DataGridTwinWindow selectItem = new DataGridTwinWindow();
                selectItem.ConnectionTypeModel = connectionModel;
                selectItem.ConnectionTypeModelReference = connectionModelRef;
                selectItem.LoadOnStart = true;
                selectItem.MaxRecords = 50;
                selectItem.OrderByClause = tableKeyName + " DESC";
                selectItem.OrderByClauseReference = tableKeyRefName + " DESC";
                selectItem.PrimarySearchPropertyName = tableKeyName;
                selectItem.ShowDialog();

                // Has an item been selected.
                if (selectItem.SelectedRecord != null && selectItem.SelectedReferenceRecord != null)
                {
                    // Get the selected item.
                    object data = selectItem.SelectedRecord;
                    object dataRef = selectItem.SelectedReferenceRecord;
                    txtPaymentDataMemberID.Text = data.GetType().GetProperty(tableKeyName).GetValue(data, null).ToString();
                    txtPaymentDataMemberRefID.Text = dataRef.GetType().GetProperty(tableKeyRefName).GetValue(dataRef, null).ToString();

                    // If the current operation is not in 
                    // the loading or add new state.
                    if (!_loading && !_addNew)
                    {
                        // If items exist
                        if (txtDataMember.Items.Count > 0)
                        {
                            // If data member changes then set the payment data members to null.
                            dataAccess.IsUpdateEnabled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// TransactionType selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTransactionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string transactionTypeName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.TransactionType, string>("Name");
            ((DataAccess.NequeoCompany.Data.AccountTransactions)gridTransaction.DataContext).TransactionType = transactionTypeName;

            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
        }

        /// <summary>
        /// DataMember selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDataMember_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Load the data member table data.
            if (_dataMemberTables == null)
                _dataMemberTables = _account.Current.Extension.DataContext.DataMemberTables.Where(u => u.DataMemberID > -1).ToArray();

            // Get and assign the value from the selected item.
            string dataMemberName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.DataMember, string>("Name");
            ((DataAccess.NequeoCompany.Data.AccountTransactions)gridTransaction.DataContext).PaidToFrom = dataMemberName;

            // Attempt to find the data member.
            DataAccess.NequeoCompany.Data.DataMemberTables dataMember = null;
            try{
                dataMember = _dataMemberTables.First(u => u.TableName.ToLower() == dataMemberName.ToLower());}
            catch { }

            // Get the text to display
            string selectText = (dataMember != null ? "Select " + dataMember.NameTo : string.Empty);

            // Assign the text for the transaction type slection button
            if (String.IsNullOrEmpty(selectText))
            {
                btnSelectTransaction.IsEnabled = false;
                btnSelectTransactionTextBlock.Text = "";
                txtDataMemberID.Text = "-1";
                _dataMemberTableID = -1;
            }
            else
            {
                btnSelectTransaction.IsEnabled = true;
                btnSelectTransactionTextBlock.Text = selectText;
                txtDataMemberID.Text = dataMember.DataMemberID.ToString();
                _dataMemberTableID = dataMember.DataMemberID;
            }
            
            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();

            // If the current operation is not in 
            // the loading or add new state.
            if (!_loading && !_addNew)
            {
                // If items exist
                if (txtDataMember.Items.Count > 0)
                {
                    if (_dataMemberSelectdIndex == txtDataMember.SelectedIndex)
                        // If data member changes then set the payment data members to null.
                        dataAccess.IsUpdateEnabled = true;
                    else
                        // If data member changes then set the payment data members to null.
                        dataAccess.IsUpdateEnabled = false;
                }
            }

            // If adding a new item
            if (_addNew)
            {
                // Assign the text for the transaction type slection button
                if (String.IsNullOrEmpty(selectText))
                {
                    txtPaymentDataMemberID.Text = "0";
                    txtPaymentDataMemberRefID.Text = "0";
                }
                else
                {
                    txtPaymentDataMemberID.Text = "";
                    txtPaymentDataMemberRefID.Text = "";
                }
            }
        }

        /// <summary>
        /// Is the current item valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataItemValid(object sender, Custom.ValidationArgs e)
        {
            // Add the current validation item.
            if (!_valid.Keys.Contains(sender.GetHashCode()))
                _valid.Add(sender.GetHashCode(), e.Valid);
            else
                _valid[sender.GetHashCode()] = e.Valid;

            // If not valid then disable controls.
            if (!e.Valid)
            {
                dataAccess.IsUpdateEnabled = false;
                dataAccess.IsInsertEnabled = false;
            }
            else
            {
                // Find all invalid items, if no invalid items then
                // enable operation controls.
                IEnumerable<bool> inValid = _valid.Values.Where(u => (u == false));
                if ((inValid.Count() < 1))
                {
                    // If currently not in any mode.
                    dataAccess.IsUpdateEnabled = true;
                    dataAccess.IsInsertEnabled = false;

                    // If currently in the add new mode
                    if (_addNew)
                    {
                        dataAccess.IsUpdateEnabled = false;
                        dataAccess.IsInsertEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Set the current change property state.
        /// </summary>
        /// <param name="changed">The current change state.</param>
        private void SetChangePropertyState(bool changed)
        {
            // If the current operation is not in the loading or add new state.
            if (!_loading && !_addNew)
            {
                PropertyChanged = changed;
                if (PropertyChanged)
                {
                    groupBoxTransactionContainer.Header = " * changed";
                    groupBoxTransactionContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxTransactionContainer.Header = null;
                    groupBoxTransactionContainer.Foreground = Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Load all relevant list items into controls.
        /// </summary>
        private void LoadListItems()
        {
            // If no items exists then load.
            if (txtTransactionType.Items.Count < 1)
                txtTransactionType.ItemsSource = _account.Current.Extension5.GetTransactionTypeList();

            // If no items exists then load.
            if (txtDataMember.Items.Count < 1)
                txtDataMember.ItemsSource = _account.Current.Extension3.GetDataMemberList();
        }

        /// <summary>
        /// Set the selected index list bindings.
        /// </summary>
        /// <param name="data">The current data instance</param>
        private void ListSelectedIndex<TData>(TData data)
        {
            // Get the selected index of item collection of the income type.
            txtTransactionType.SelectedIndex = txtTransactionType.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.TransactionType, TData>("Name", data, "TransactionType");

            // Get the selected index of item collection of the gst income type.
            txtDataMember.SelectedIndex = txtDataMember.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.DataMember, TData>("Name", data, "PaidToFrom");
        }

        /// <summary>
        /// Are the selected index value valid.
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsSelectedIndexValid()
        {
            bool ret = true;

            if (!IsTransactionTypeSelectedIndexValid()) ret = false;
            if (!IsDataMemberSelectedIndexValid()) ret = false;

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the state selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsTransactionTypeSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtTransactionType.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtTransactionType.Visibility = System.Windows.Visibility.Hidden;
                txtTransactionType.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtTransactionType.Visibility = System.Windows.Visibility.Visible;
                txtTransactionType.ToolTip = "Select a transaction type";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtTransactionType, new Custom.ValidationArgs(ret));

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the postal state selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsDataMemberSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtDataMember.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtDataMember.Visibility = System.Windows.Visibility.Hidden;
                txtDataMember.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtDataMember.Visibility = System.Windows.Visibility.Visible;
                txtDataMember.ToolTip = "Select a data member";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtDataMember, new Custom.ValidationArgs(ret));

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Enable or disable operations.
        /// </summary>
        /// <param name="index"></param>
        private void EnableDisable(int index)
        {
            dataAccess.IsLoadEnabled = true;
            btnAddNew.IsEnabled = true;

            switch (index)
            {
                // Load, Update
                case 0:
                case 1:
                    groupBoxTransactionContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    btnSelectTransaction.IsEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxTransactionContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectTransaction.IsEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxTransactionContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = true;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectTransaction.IsEnabled = true;
                    break;
            }
        }
    }
}
