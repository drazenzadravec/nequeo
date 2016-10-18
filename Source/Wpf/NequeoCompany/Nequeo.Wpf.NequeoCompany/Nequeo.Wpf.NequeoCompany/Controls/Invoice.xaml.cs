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

namespace Nequeo.Wpf.NequeoCompany.Controls
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class Invoice : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Invoice window
        /// </summary>
        public Invoice()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.ICustomer _customer;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Service.NequeoCompany.ICustomer CustomerDataSource
        {
            get { return _customer; }
            set 
            { 
                _customer = value;

                // Assign each dependency data source
                invoiceReport.CustomerDataSource = _customer;
                invoiceDetails.CustomerDataSource = _customer;
                invoiceProducts.CustomerDataSource = _customer;

                // Set the connection type model mapping.
                if (_customer != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.Invoices, DataAccess.NequeoCompany.Data.Customers>(_customer.Current.Extension.Common);
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
                details.Add("Invoice property/properties have changed.");

            // If invoice details has changed
            if (invoiceDetails.GetDetails().Count > 0)
                details.AddRange(invoiceDetails.GetDetails().ToArray());

            // If invoice products has changed
            if (invoiceProducts.GetDetails().Count > 0)
                details.AddRange(invoiceProducts.GetDetails().ToArray());

            return details;
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            dataAccess.IsLoadEnabled = true;

            // Set permission.
            switch (Common.Helper.UserType)
            {
                case DataAccess.NequeoCompany.Enum.EnumUserType.Administrator:
                case DataAccess.NequeoCompany.Enum.EnumUserType.Sales:
                    this.IsEnabled = true;
                    break;
            }
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
                    selectItem.OrderByClause = "InvoiceID DESC";
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.Invoices data = (DataAccess.NequeoCompany.Data.Invoices)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "InvoiceID = " + data.InvoiceID.ToString();
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtInvoiceID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Invoices data = (DataAccess.NequeoCompany.Data.Invoices)bindingExpression.DataItem;

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
            if(e.Cancel)
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtInvoiceID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Invoices data = (DataAccess.NequeoCompany.Data.Invoices)bindingExpression.DataItem;
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
        /// On load complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataAccess_OnLoad(object sender, EventArgs e)
        {
            // Get the data that has been returned.
            DataAccess.NequeoCompany.Data.Invoices[] returnedDataList = (DataAccess.NequeoCompany.Data.Invoices[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.Invoices data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridInvoice.DataContext = data;
            SetData(data.InvoiceID);

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
                SetData(0);

                // Set the default values.
                txtInvoiceID.Text = "0";
                txtCustomerID.Text = "";
                txtOrderID.Text = "";
                txtContact.Text = "";
                txtComments.Text = "";
                txtInvoiceDate.SelectedDate = DateTime.Now;
                txtPaymentDate.SelectedDate = null;
                txtIncomeType.SelectedIndex = -1;
                txtGstIncomeType.SelectedIndex = -1;

                // Load all list items.
                LoadListItems();
                IsSelectedIndexValid();
            }
        }

        /// <summary>
        /// On select new customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Get a new instance of the connection type model.
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = 
                Nequeo.Wpf.Common.Operation.GetTypeModel<DataAccess.NequeoCompany.Data.Customers>(dataAccess);

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
        /// Income type selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtIncomeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string incomeTypeName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.IncomeType, string>("Name");
            ((DataAccess.NequeoCompany.Data.Invoices)gridInvoice.DataContext).IncomeType = incomeTypeName;

            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
        }

        /// <summary>
        /// Gst Income type selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGstIncomeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string gstIncomeTypeName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.GstIncomeType, string>("Name");
            ((DataAccess.NequeoCompany.Data.Invoices)gridInvoice.DataContext).GSTIncluded = gstIncomeTypeName;

            if (!String.IsNullOrEmpty(gstIncomeTypeName))
            {
                // Set the GST type calculations
                invoiceDetails.GST = ((DataAccess.NequeoCompany.Enum.EnumGstType)Enum.Parse(typeof(DataAccess.NequeoCompany.Enum.EnumGstType), gstIncomeTypeName));
                invoiceProducts.GST = ((DataAccess.NequeoCompany.Enum.EnumGstType)Enum.Parse(typeof(DataAccess.NequeoCompany.Enum.EnumGstType), gstIncomeTypeName));
            }
            
            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
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
                    groupBoxInvoiceContainer.Header = " * changed";
                    groupBoxInvoiceContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxInvoiceContainer.Header = null;
                    groupBoxInvoiceContainer.Foreground = Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Load all relevant list items into controls.
        /// </summary>
        private void LoadListItems()
        {
            // If no items exists then load.
            if (txtIncomeType.Items.Count < 1)
                txtIncomeType.ItemsSource = _customer.Current.Extension4.GetIncomeTypeList();

            // If no items exists then load.
            if (txtGstIncomeType.Items.Count < 1)
                txtGstIncomeType.ItemsSource = _customer.Current.Extension5.GetGstTypeList();
        }

        /// <summary>
        /// Set the selected index list bindings.
        /// </summary>
        /// <param name="data">The current data instance</param>
        private void ListSelectedIndex<TData>(TData data)
        {
            // Get the selected index of item collection of the income type.
            txtIncomeType.SelectedIndex = txtIncomeType.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.IncomeType, TData>("Name", data, "IncomeType");

            // Get the selected index of item collection of the gst income type.
            txtGstIncomeType.SelectedIndex = txtGstIncomeType.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.GstIncomeType, TData>("Name", data, "GSTIncluded");
        }

        /// <summary>
        /// Are the selected index value valid.
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsSelectedIndexValid()
        {
            bool ret = true;

            if (!IsGstIncomeTypeSelectedIndexValid()) ret = false;
            if (!IsIncomeTypeSelectedIndexValid()) ret = false;

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the gst income type selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsGstIncomeTypeSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtGstIncomeType.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtGstIncomeType.Visibility = System.Windows.Visibility.Hidden;
                txtGstIncomeType.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtGstIncomeType.Visibility = System.Windows.Visibility.Visible;
                txtGstIncomeType.ToolTip = "Select a GST income type";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtGstIncomeType, new Custom.ValidationArgs(ret));

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the income type selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsIncomeTypeSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtIncomeType.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtIncomeType.Visibility = System.Windows.Visibility.Hidden;
                txtIncomeType.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtIncomeType.Visibility = System.Windows.Visibility.Visible;
                txtIncomeType.ToolTip = "Select an income type";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtIncomeType, new Custom.ValidationArgs(ret));

            // Return the result.
            return ret;
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
                    groupBoxInvoiceContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    btnSelectCustomer.IsEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxInvoiceContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectCustomer.IsEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxInvoiceContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = true;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectCustomer.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Set all data for sub controls.
        /// </summary>
        /// <param name="idValue">The current key value</param>
        private void SetData(int idValue)
        {
            invoiceDetails.InvoiceID = idValue;
            invoiceProducts.InvoiceID = idValue;
        }
    }
}
