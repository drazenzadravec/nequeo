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
using System.Reflection;

using Nequeo.Handler;
using Nequeo.Collections.Extension;
using Nequeo.Wpf.Extension;
using Nequeo.Wpf.NequeoCompany.Common;

namespace Nequeo.Wpf.NequeoCompany.Controls
{
    /// <summary>
    /// Interaction logic for Vendor.xaml
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    public partial class Vendor : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Vendor window
        /// </summary>
        public Vendor()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IVendor _vendor;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();

        /// <summary>
        /// Gets sets, the vendor injected interface.
        /// </summary>
        public Service.NequeoCompany.IVendor VendorDataSource
        {
            get { return _vendor; }
            set
            {
                _vendor = value;

                // Assign each dependency data source
                vendorReport.VendorDataSource = _vendor;
                vendorDetails.VendorDataSource = _vendor;

                // Set the connection type model mapping.
                if (_vendor != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.Vendors, DataAccess.NequeoCompany.Data.Vendors>(_vendor.Current.Extension.Common);
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
                details.Add("Vendor property/properties have changed.");

            // If vendor details has changed
            if (vendorDetails.GetDetails().Count > 0)
                details.AddRange(vendorDetails.GetDetails().ToArray());

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
                case DataAccess.NequeoCompany.Enum.EnumUserType.Purchase:
                    this.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Email button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmailAddress_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtEmaillAddress.Text))
                System.Diagnostics.Process.Start("mailto:" + txtEmaillAddress.Text);
        }

        /// <summary>
        /// Website button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblWebSite_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtWebSite.Text))
                System.Diagnostics.Process.Start("http://" + txtWebSite.Text);
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
                    selectItem.OrderByClause = "VendorID DESC";
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.Vendors data = (DataAccess.NequeoCompany.Data.Vendors)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "VendorID = " + data.VendorID.ToString();
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtVendorID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Vendors data = (DataAccess.NequeoCompany.Data.Vendors)bindingExpression.DataItem;

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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtVendorID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Vendors data = (DataAccess.NequeoCompany.Data.Vendors)bindingExpression.DataItem;
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
            DataAccess.NequeoCompany.Data.Vendors[] returnedDataList = (DataAccess.NequeoCompany.Data.Vendors[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.Vendors data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridVendor.DataContext = data;
            SetData(data.VendorID);

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
            try
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
                    txtVendorID.Text = "0";
                    txtABN.Text = "";
                    txtCompanyName.Text = "";
                    txtFirstName.Text = "";
                    txtLastName.Text = "";
                    txtEmaillAddress.Text = "";
                    txtMobileNumber.Text = "";
                    txtAddress.Text = "";
                    txtSuburb.Text = "";
                    txtState.SelectedIndex = -1;
                    txtPostcode.Text = "";
                    txtPhoneNumber.Text = "";
                    txtFaxNumber.Text = "";
                    txtWebSite.Text = "";
                    txtPostalAddress.Text = "";
                    txtPostalSuburb.Text = "";
                    txtPostalState.SelectedIndex = -1;
                    txtPostalPostcode.Text = "";
                    txtComments.Text = "";

                    // Load all list items.
                    LoadListItems();
                    IsSelectedIndexValid();
                }
            }
            catch (Exception ex)
            {
                // Log the error.
                Nequeo.Handler.AsyncLogHandler.Execute(
                    u => u.WriteType(ex.Message, MethodInfo.GetCurrentMethod(),
                        Nequeo.Wpf.NequeoCompany.Common.ServiceLocator.EventApplicationName));

                // Display the error
                Helper.ShowMessage(ex.Message, "Vendor");
            }
        }

        /// <summary>
        /// State selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string stateName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.State, string>("ShortName");
            ((DataAccess.NequeoCompany.Data.Vendors)gridVendor.DataContext).State = stateName;

            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
        }

        /// <summary>
        /// Postal state selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPostalState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string postalStateName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.State, string>("ShortName");
            ((DataAccess.NequeoCompany.Data.Vendors)gridVendor.DataContext).PostalState = postalStateName;

            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
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
                    groupBoxVendorContainer.Header = " * changed";
                    groupBoxVendorContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxVendorContainer.Header = null;
                    groupBoxVendorContainer.Foreground = Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Load all relevant list items into controls.
        /// </summary>
        private void LoadListItems()
        {
            // If no items exists then load.
            if (txtState.Items.Count < 1)
                txtState.ItemsSource = _vendor.Current.Extension.DataContext.Extension.State.GetStateTypeList();

            // If no items exists then load.
            if (txtPostalState.Items.Count < 1)
                txtPostalState.ItemsSource = _vendor.Current.Extension.DataContext.Extension.State.GetStateTypeList();
        }

        /// <summary>
        /// Set the selected index list bindings.
        /// </summary>
        /// <param name="data">The current data instance</param>
        private void ListSelectedIndex<TData>(TData data)
        {
            // Get the selected index of item collection of the income type.
            txtState.SelectedIndex = txtState.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.State, TData>("ShortName", data, "State");

            // Get the selected index of item collection of the gst income type.
            txtPostalState.SelectedIndex = txtPostalState.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.State, TData>("ShortName", data, "PostalState");
        }

        /// <summary>
        /// Are the selected index value valid.
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsSelectedIndexValid()
        {
            bool ret = true;

            //if (!IsStateSelectedIndexValid()) ret = false;
            //if (!IsPostalStateSelectedIndexValid()) ret = false;

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the state selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsStateSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtState.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtState.Visibility = System.Windows.Visibility.Hidden;
                txtState.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtState.Visibility = System.Windows.Visibility.Visible;
                txtState.ToolTip = "Select a state";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtState, new Custom.ValidationArgs(ret));

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the postal state selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsPostalStateSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtPostalState.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtPostalState.Visibility = System.Windows.Visibility.Hidden;
                txtPostalState.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtPostalState.Visibility = System.Windows.Visibility.Visible;
                txtPostalState.ToolTip = "Select a postal state";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtPostalState, new Custom.ValidationArgs(ret));

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
                    groupBoxVendorContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxVendorContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxVendorContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = true;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// Set all data for sub controls.
        /// </summary>
        /// <param name="idValue">The current key value</param>
        private void SetData(int idValue)
        {
            vendorDetails.VendorID = idValue;
        }
    }
}
