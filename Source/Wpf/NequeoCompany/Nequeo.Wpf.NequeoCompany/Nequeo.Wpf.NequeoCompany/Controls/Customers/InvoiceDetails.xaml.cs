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
    /// Interaction logic for InvoiceDetails.xaml
    /// </summary>
    public partial class InvoiceDetails : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvoiceDetails()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.ICustomer _customer;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();
        private decimal _gstRate = (decimal)-1;
        private DataAccess.NequeoCompany.Enum.EnumGstType _gst = DataAccess.NequeoCompany.Enum.EnumGstType.Calculated;
        private long _invoiceID = 0;

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Service.NequeoCompany.ICustomer CustomerDataSource
        {
            get { return _customer; }
            set
            {
                _customer = value;

                // Set the connection type model mapping.
                if (_customer != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.InvoiceDetails, DataAccess.NequeoCompany.Data.Customers>(_customer.Current.Extension.Common);
            }
        }

        /// <summary>
        /// Gets sets the GST calculation type.
        /// </summary>
        public DataAccess.NequeoCompany.Enum.EnumGstType GST
        {
            get { return _gst; }
            set { _gst = value; }
        }

        /// <summary>
        /// Gets sets, the current invoice id.
        /// </summary>
        public long InvoiceID
        {
            get { return _invoiceID; }
            set 
            {
                _invoiceID = value;

                if (_invoiceID > 0)
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
                details.Add("Invoice Details property/properties have changed.");

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
                    selectItem.OrderByClause = "InvoiceDetailsID DESC";
                    selectItem.WhereClause = "InvoiceID = " + _invoiceID.ToString();
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.InvoiceDetails data = (DataAccess.NequeoCompany.Data.InvoiceDetails)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "InvoiceDetailsID = " + data.InvoiceDetailsID.ToString();
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtProductDetailsID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.InvoiceDetails data = (DataAccess.NequeoCompany.Data.InvoiceDetails)bindingExpression.DataItem;

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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtProductDetailsID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.InvoiceDetails data = (DataAccess.NequeoCompany.Data.InvoiceDetails)bindingExpression.DataItem;
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
            DataAccess.NequeoCompany.Data.InvoiceDetails[] returnedDataList = (DataAccess.NequeoCompany.Data.InvoiceDetails[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.InvoiceDetails data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridInvoiceProductDetails.DataContext = data;

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
                txtProductDetailsID.Text = "0";
                txtInvoiceID.Text = _invoiceID.ToString();
                txtHours.Text = "";
                txtDescription.Text = "";
                txtComments.Text = "";
                txtRatePrice.Text = "";
                txtTotalPrice.Text = "";
                txtTotalGST.Text = "";

                // Load all list items.
                LoadListItems();
                IsSelectedIndexValid();
            }
        }

        /// <summary>
        /// On select new invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectInvoice_Click(object sender, RoutedEventArgs e)
        {
            // Get a new instance of the connection type model.
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel =
                Nequeo.Wpf.Common.Operation.GetTypeModel<DataAccess.NequeoCompany.Data.Invoices>(dataAccess);

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
        /// Hours text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePrice();
        }

        /// <summary>
        /// Rate price text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRatePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePrice();
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
                    groupBoxInvoiceProductDetailsContainer.Header = " * changed";
                    groupBoxInvoiceProductDetailsContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxInvoiceProductDetailsContainer.Header = null;
                    groupBoxInvoiceProductDetailsContainer.Foreground = Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Load all relevant list items into controls.
        /// </summary>
        private void LoadListItems()
        {
        }

        /// <summary>
        /// Set the selected index list bindings.
        /// </summary>
        /// <param name="data">The current data instance</param>
        private void ListSelectedIndex<TData>(TData data)
        {
        }

        /// <summary>
        /// Are the selected index value valid.
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsSelectedIndexValid()
        {
            bool ret = true;

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Calculate the price of the product
        /// </summary>
        private void CalculatePrice()
        {
            // If both objects have been created.
            if (txtHours != null && txtRatePrice != null)
            {
                // If both objects contain text.
                if (!String.IsNullOrEmpty(txtHours.Text) && !String.IsNullOrEmpty(txtRatePrice.Text))
                {
                    try
                    {
                        decimal unitPrice = 0;
                        double hours = 0;

                        // If both objects contain valid text.
                        if ((System.Double.TryParse(txtHours.Text, out hours)) && (System.Decimal.TryParse(txtRatePrice.Text, out unitPrice)))
                        {
                            // If the GST rate is less than zero then
                            // get the GST rate from the data base.
                            if (_gstRate < 0)
                                _gstRate = _customer.Current.Extension.DataContext.Extension.GenericData.GetGstRate();

                            // Calculate the total of the product and the GST
                            decimal total = ((decimal)((decimal)hours * unitPrice));
                            decimal gst = ((total * _gstRate) / 100);

                            // Set the GST calculation point.
                            switch(_gst)
                            {
                                case DataAccess.NequeoCompany.Enum.EnumGstType.Calculated:
                                    break;
                                case DataAccess.NequeoCompany.Enum.EnumGstType.Included:
                                    gst = 0;
                                    break;
                                default:
                                    throw new Exception("GST calculation type has not been specified.");
                            }

                            // Assign the total and the GST to the objects.
                            txtTotalPrice.Text = total.ToString();
                            txtTotalGST.Text = gst.ToString();
                        }
                    }
                    catch { }
                }
                else
                {
                    // If no values exist.
                    txtTotalPrice.Text = "";
                    txtTotalGST.Text = "";
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
                    groupBoxInvoiceProductDetailsContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    btnSelectInvoice.IsEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxInvoiceProductDetailsContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectInvoice.IsEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxInvoiceProductDetailsContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = true;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectInvoice.IsEnabled = true;
                    break;
            }
        }
    }
}
