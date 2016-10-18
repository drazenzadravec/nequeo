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
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class Employee : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Employee window
        /// </summary>
        public Employee()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IEmployee _employee;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();

        /// <summary>
        /// Gets sets, the employee injected interface.
        /// </summary>
        public Service.NequeoCompany.IEmployee EmployeeDataSource
        {
            get { return _employee; }
            set
            {
                _employee = value;

                // Assign each dependency data source
                employeeSuperFundAccount.EmployeeDataSource = _employee;
                employeeBankAccount.EmployeeDataSource = _employee;
                employeeExtraPayg.EmployeeDataSource = _employee;
                employeeSuperannuation.EmployeeDataSource = _employee;
                employeeWage.EmployeeDataSource = _employee;
                employeeAccountReport.EmployeeDataSource = _employee;
                employeeReport.EmployeeDataSource = _employee;
                employeeCustomReport.EmployeeDataSource = _employee;

                // Set the connection type model mapping.
                if (_employee != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.Employees, DataAccess.NequeoCompany.Data.Employees>(_employee.Current.Extension.Common);
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
                details.Add("Employee property/properties have changed.");

            // If super fund has changed
            if (employeeSuperFundAccount.GetDetails().Count > 0)
                details.AddRange(employeeSuperFundAccount.GetDetails().ToArray());

            // If super fund has changed
            if (employeeBankAccount.GetDetails().Count > 0)
                details.AddRange(employeeBankAccount.GetDetails().ToArray());

            // If super fund has changed
            if (employeeExtraPayg.GetDetails().Count > 0)
                details.AddRange(employeeExtraPayg.GetDetails().ToArray());

            // If super has changed
            if (employeeSuperannuation.GetDetails().Count > 0)
                details.AddRange(employeeSuperannuation.GetDetails().ToArray());

            // If super has changed
            if (employeeWage.GetDetails().Count > 0)
                details.AddRange(employeeWage.GetDetails().ToArray());

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
                case DataAccess.NequeoCompany.Enum.EnumUserType.Finance:
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
                    selectItem.OrderByClause = "EmployeeID DESC";
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.Employees data = (DataAccess.NequeoCompany.Data.Employees)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "EmployeeID = " + data.EmployeeID.ToString();
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtEmployeeID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Employees data = (DataAccess.NequeoCompany.Data.Employees)bindingExpression.DataItem;

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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtEmployeeID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Employees data = (DataAccess.NequeoCompany.Data.Employees)bindingExpression.DataItem;
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
            DataAccess.NequeoCompany.Data.Employees[] returnedDataList = (DataAccess.NequeoCompany.Data.Employees[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.Employees data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridEmployee.DataContext = data;
            SetData(data.EmployeeID);

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
                txtEmployeeID.Text = "0";
                txtTFN.Text = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtDateOfBirth.SelectedDate = null;
                txtAddress.Text = "";
                txtSuburb.Text = "";
                txtState.SelectedIndex = -1;
                txtPostcode.Text = "";
                txtPayIntervalType.SelectedIndex = -1;
                txtPhoneNumber.Text = "";
                txtMobileNumber.Text = "";
                txtEmaillAddress.Text = "";
                txtWage.Text = "";
                txtTaxRate.Text = "";
                txtBaseHours.Text = "";
                txtSuperRate.Text = "";
                txtWageInclude.Text = "";
                txtBCoefficient.Text = "";
                txtComments.Text = "";

                // Load all list items.
                LoadListItems();
                IsSelectedIndexValid();
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
            ((DataAccess.NequeoCompany.Data.Employees)gridEmployee.DataContext).State = stateName;

            // Indicate that the property has changed.
            SetChangePropertyState(true);
            IsSelectedIndexValid();
        }

        /// <summary>
        /// Postal state selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPayIntervalType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get and assign the value from the selected item.
            string payIntervalName = e.AddedItems.FirstValue<DataAccess.NequeoCompany.Data.PayIntervalType, string>("Name");
            ((DataAccess.NequeoCompany.Data.Employees)gridEmployee.DataContext).PayInterval = payIntervalName;

            if (!String.IsNullOrEmpty(payIntervalName))
            {
                // Set the pay period interval type calculations
                employeeSuperFundAccount.PayPeriodIntervalType = ((Data.Enum.EnumPayPeriodIntervalType)Enum.Parse(typeof(Data.Enum.EnumPayPeriodIntervalType), payIntervalName));
                employeeBankAccount.PayPeriodIntervalType = ((Data.Enum.EnumPayPeriodIntervalType)Enum.Parse(typeof(Data.Enum.EnumPayPeriodIntervalType), payIntervalName));
                employeeExtraPayg.PayPeriodIntervalType = ((Data.Enum.EnumPayPeriodIntervalType)Enum.Parse(typeof(Data.Enum.EnumPayPeriodIntervalType), payIntervalName));
                employeeSuperannuation.PayPeriodIntervalType = ((Data.Enum.EnumPayPeriodIntervalType)Enum.Parse(typeof(Data.Enum.EnumPayPeriodIntervalType), payIntervalName));
                employeeWage.PayPeriodIntervalType = ((Data.Enum.EnumPayPeriodIntervalType)Enum.Parse(typeof(Data.Enum.EnumPayPeriodIntervalType), payIntervalName));
                employeeReport.PayPeriodIntervalType = ((Data.Enum.EnumPayPeriodIntervalType)Enum.Parse(typeof(Data.Enum.EnumPayPeriodIntervalType), payIntervalName));
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
                    groupBoxEmployeeContainer.Header = " * changed";
                    groupBoxEmployeeContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxEmployeeContainer.Header = null;
                    groupBoxEmployeeContainer.Foreground = Brushes.Black;
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
                txtState.ItemsSource = _employee.Current.Extension.DataContext.Extension.State.GetStateTypeList();

            // If no items exists then load.
            if (txtPayIntervalType.Items.Count < 1)
                txtPayIntervalType.ItemsSource = _employee.Current.Extension6.GetPayIntervalTypeList();
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
            txtPayIntervalType.SelectedIndex = txtPayIntervalType.Items.SelectedIndex<
                DataAccess.NequeoCompany.Data.PayIntervalType, TData>("Name", data, "PayInterval");
        }

        /// <summary>
        /// Are the selected index value valid.
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsSelectedIndexValid()
        {
            bool ret = true;

            if (!IsStateSelectedIndexValid()) ret = false;
            if (!IsPayIntervalSelectedIndexValid()) ret = false;

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
        /// Is the pay interval selected index valid
        /// </summary>
        /// <returns>True else false.</returns>
        private bool IsPayIntervalSelectedIndexValid()
        {
            bool ret = true;

            // If the index value is valid then
            if (txtPayIntervalType.SelectedIndex > -1)
            {
                // Hide the rectangle
                rectangleTxtPayIntervalType.Visibility = System.Windows.Visibility.Hidden;
                txtPayIntervalType.ToolTip = null;
                ret = true;
            }
            else
            {
                // Show the rectangle
                rectangleTxtPayIntervalType.Visibility = System.Windows.Visibility.Visible;
                txtPayIntervalType.ToolTip = "Select a pay interval";
                ret = false;
            }

            // Data item validation
            DataItemValid(txtPayIntervalType, new Custom.ValidationArgs(ret));

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
                    groupBoxEmployeeContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxEmployeeContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxEmployeeContainer.IsEnabled = true;
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
            employeeSuperFundAccount.EmployeeID = idValue;
            employeeBankAccount.EmployeeID = idValue;
            employeeExtraPayg.EmployeeID = idValue;
            employeeSuperannuation.EmployeeID = idValue;
            employeeWage.EmployeeID = idValue;

            employeeSuperannuation.AnnualWage = Decimal.Parse(txtWage.Text);
            employeeSuperannuation.SuperRate = Double.Parse(txtSuperRate.Text);

            employeeWage.AnnualWage = Decimal.Parse(txtWage.Text);
            employeeWage.BCoefficient = Double.Parse(txtBCoefficient.Text);
            employeeWage.WageInclude = Double.Parse(txtWageInclude.Text);
            employeeWage.WeekBaseHours = Double.Parse(txtBaseHours.Text);
            employeeWage.TaxRate = Double.Parse(txtTaxRate.Text);
        }

        /// <summary>
        /// Text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (employeeSuperannuation != null)
                employeeSuperannuation.AnnualWage = Decimal.Parse(txtWage.Text);

            if (employeeWage != null)
                employeeWage.AnnualWage = Decimal.Parse(txtWage.Text);
        }

        /// <summary>
        /// Text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTaxRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (employeeWage != null)
                employeeWage.TaxRate = Double.Parse(txtTaxRate.Text);
        }

        /// <summary>
        /// Text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBaseHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (employeeWage != null)
                employeeWage.WeekBaseHours = Double.Parse(txtBaseHours.Text);
        }

        /// <summary>
        /// Text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSuperRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (employeeSuperannuation != null)
                employeeSuperannuation.SuperRate = Double.Parse(txtSuperRate.Text);
        }

        /// <summary>
        /// Text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWageInclude_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (employeeWage != null)
                employeeWage.WageInclude = Double.Parse(txtWageInclude.Text);
        }

        /// <summary>
        /// Text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBCoefficient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (employeeWage != null)
                employeeWage.BCoefficient = Double.Parse(txtBCoefficient.Text);
        }
    }
}
