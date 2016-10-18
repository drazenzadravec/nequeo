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
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Product window
        /// </summary>
        public Product()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IProduct _product;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();

        /// <summary>
        /// Gets sets, the product injected interface.
        /// </summary>
        public Service.NequeoCompany.IProduct ProductDataSource
        {
            get { return _product; }
            set
            {
                _product = value;

                // Assign each dependency data source
                productReport.ProductDataSource = _product;

                // Set the connection type model mapping.
                if (_product != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.Products, DataAccess.NequeoCompany.Data.Products>(_product.Current.Extension.Common);
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
                details.Add("Product property/properties have changed.");

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
                    selectItem.OrderByClause = "ProductID DESC";
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.Products data = (DataAccess.NequeoCompany.Data.Products)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "ProductID = " + data.ProductID.ToString();
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtProductID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Products data = (DataAccess.NequeoCompany.Data.Products)bindingExpression.DataItem;

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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtProductID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Products data = (DataAccess.NequeoCompany.Data.Products)bindingExpression.DataItem;
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
            DataAccess.NequeoCompany.Data.Products[] returnedDataList = (DataAccess.NequeoCompany.Data.Products[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.Products data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridProduct.DataContext = data;

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
                txtProductID.Text = "0";
                txtProductNumber.Text = "";
                txtProductVersion.Text = "";
                txtProductModel.Text = "";
                txtSerialNumber.Text = "";
                txtCategory.SelectedIndex = -1;
                txtSubCategory.SelectedIndex = -1;
                txtUnits.Text = "";
                txtStatus.SelectedIndex = -1;
                txtUnitPrice.Text = "";
                txtProductionDate.SelectedDate = null;
                txtDescription.Text = "";
                txtComments.Text = "";

                // Load all list items.
                LoadListItems();
                IsSelectedIndexValid();
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
            ((DataAccess.NequeoCompany.Data.Products)gridProduct.DataContext).Category = stateName;

            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
        }

        /// <summary>
        /// SubCategory selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSubCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string stateName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.ProductSubCategory, string>("SubCategoryName");
            ((DataAccess.NequeoCompany.Data.Products)gridProduct.DataContext).SubCategory = stateName;

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
            ((DataAccess.NequeoCompany.Data.Products)gridProduct.DataContext).Status = stateName;

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
                    groupBoxProductContainer.Header = " * changed";
                    groupBoxProductContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxProductContainer.Header = null;
                    groupBoxProductContainer.Foreground = Brushes.Black;
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
                txtCategory.ItemsSource = _product.Current.Extension1.GetProductCategoryTypeList();

            // If no items exists then load.
            if (txtSubCategory.Items.Count < 1)
                txtSubCategory.ItemsSource = _product.Current.Extension2.GetProductSubCategoryTypeList();

            // If no items exists then load.
            if (txtStatus.Items.Count < 1)
                txtStatus.ItemsSource = _product.Current.Extension3.GetProductStatusTypeList();
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
            txtSubCategory.SelectedIndex = txtSubCategory.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.ProductSubCategory, TData>("SubCategoryName", data, "SubCategory");

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
            if (!IsSubCategorySelectedIndexValid()) ret = false;
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
        private bool IsSubCategorySelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtSubCategory.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtSubCategory.Visibility = System.Windows.Visibility.Hidden;
                txtSubCategory.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtSubCategory.Visibility = System.Windows.Visibility.Visible;
                txtSubCategory.ToolTip = "Select a sub-category";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtSubCategory, new Custom.ValidationArgs(ret));

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
                    groupBoxProductContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxProductContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxProductContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = true;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    break;
            }
        }
    }
}
