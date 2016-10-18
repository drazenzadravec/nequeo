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
    /// Interaction logic for Asset.xaml
    /// </summary>
    public partial class Asset : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Asset window
        /// </summary>
        public Asset()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IAsset _asset;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Service.NequeoCompany.IAsset AssetDataSource
        {
            get { return _asset; }
            set
            {
                _asset = value;

                // Assign each dependency data source
                assetReport.AssetDataSource = _asset;

                if (_asset != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.Assets, DataAccess.NequeoCompany.Data.Assets>(_asset.Current.Extension.Common);
            }
        }

        /// <summary>
        /// Has the property changed.
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
                details.Add("Asset property/properties have changed.");

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
                    selectItem.OrderByClause = "AssetID DESC";
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.Assets data = (DataAccess.NequeoCompany.Data.Assets)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "AssetID = " + data.AssetID.ToString();
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtAssetID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Assets data = (DataAccess.NequeoCompany.Data.Assets)bindingExpression.DataItem;

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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtAssetID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Assets data = (DataAccess.NequeoCompany.Data.Assets)bindingExpression.DataItem;
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
            DataAccess.NequeoCompany.Data.Assets[] returnedDataList = (DataAccess.NequeoCompany.Data.Assets[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.Assets data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridAsset.DataContext = data;

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
                txtAssetID.Text = "0";
                txtVendorID.Text = "";
                txtVendorDetailID.Text = "";
                txtLocation.Text = "";
                txtModel.Text = "";
                txtSerialNumber.Text = "";
                txtCategory.SelectedIndex = -1;
                txtUnits.Text = "";
                txtStatus.SelectedIndex = -1;
                txtManufacturer.Text = "";
                txtWebSite.Text = "";
                txtAmount.Text = "";
                txtDescription.Text = "";
                txtDetails.Text = "";
                txtComments.Text = "";

                // Load all list items.
                LoadListItems();
                IsSelectedIndexValid();
            }
        }

        /// <summary>
        /// Select the Vendor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectVendor_Click(object sender, RoutedEventArgs e)
        {
            // Get a new instance of the connection type model.
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel =
                Nequeo.Wpf.Common.Operation.GetTypeModel<DataAccess.NequeoCompany.Data.Vendors>(dataAccess);

            // Get a new instance of the connection type model.
            Nequeo.ComponentModel.ConnectionTypeModel connectionModelRef =
                Nequeo.Wpf.Common.Operation.GetTypeModel<DataAccess.NequeoCompany.Data.VendorDetails>(dataAccess);

            // Show the selection form
            Nequeo.Wpf.DataGridTwinWindow selectItem = new DataGridTwinWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.ConnectionTypeModelReference = connectionModelRef;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "VendorID DESC";
            selectItem.OrderByClauseReference = "VendorDetailsID DESC";
            selectItem.PrimarySearchPropertyName = "VendorID";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null && selectItem.SelectedReferenceRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Vendors data = (DataAccess.NequeoCompany.Data.Vendors)selectItem.SelectedRecord;
                DataAccess.NequeoCompany.Data.VendorDetails dataRef = (DataAccess.NequeoCompany.Data.VendorDetails)selectItem.SelectedReferenceRecord;
                txtVendorID.Text = data.VendorID.ToString();
                txtVendorDetailID.Text = dataRef.VendorDetailsID.ToString();
            }
        }

        /// <summary>
        /// Category selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string stateName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.ProductCategory, string>("Category");
            ((DataAccess.NequeoCompany.Data.Assets)gridAsset.DataContext).Category = stateName;

            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
        }

        /// <summary>
        /// Status selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string stateName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.ProductStatus, string>("StatusName");
            ((DataAccess.NequeoCompany.Data.Assets)gridAsset.DataContext).Status = stateName;

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
                    groupBoxAssetContainer.Header = " * changed";
                    groupBoxAssetContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxAssetContainer.Header = null;
                    groupBoxAssetContainer.Foreground = Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Load all relevant list items into controls.
        /// </summary>
        private void LoadListItems()
        {
            // If no items exists then load.
            if (txtCategory.Items.Count < 1)
                txtCategory.ItemsSource = _asset.Current.Extension.DataContext.Extension.ProductCategory.GetProductCategoryTypeList();

            // If no items exists then load.
            if (txtStatus.Items.Count < 1)
                txtStatus.ItemsSource = _asset.Current.Extension.DataContext.Extension.ProductStatus.GetProductStatusTypeList();
        }

        /// <summary>
        /// Set the selected index list bindings.
        /// </summary>
        /// <param name="data">The current data instance</param>
        private void ListSelectedIndex<TData>(TData data)
        {
            // Get the selected index of item collection of the income type.
            txtCategory.SelectedIndex = txtCategory.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.ProductCategory, TData>("Category", data, "Category");

            // Get the selected index of item collection of the gst income type.
            txtStatus.SelectedIndex = txtStatus.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.ProductStatus, TData>("StatusName", data, "Status");
        }

        /// <summary>
        /// Are the selected index value valid.
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsSelectedIndexValid()
        {
            bool ret = true;

            if (!IsCategorySelectedIndexValid()) ret = false;
            if (!IsStatusSelectedIndexValid()) ret = false;

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the state selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsCategorySelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtCategory.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtCategory.Visibility = System.Windows.Visibility.Hidden;
                txtCategory.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtCategory.Visibility = System.Windows.Visibility.Visible;
                txtCategory.ToolTip = "Select a category";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtCategory, new Custom.ValidationArgs(ret));

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Is the state selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsStatusSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtStatus.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtStatus.Visibility = System.Windows.Visibility.Hidden;
                txtStatus.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtStatus.Visibility = System.Windows.Visibility.Visible;
                txtStatus.ToolTip = "Select a status";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtStatus, new Custom.ValidationArgs(ret));

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
                    groupBoxAssetContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    btnSelectVendor.IsEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxAssetContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectVendor.IsEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxAssetContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = true;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectVendor.IsEnabled = true;
                    break;
            }
        }
    }
}
