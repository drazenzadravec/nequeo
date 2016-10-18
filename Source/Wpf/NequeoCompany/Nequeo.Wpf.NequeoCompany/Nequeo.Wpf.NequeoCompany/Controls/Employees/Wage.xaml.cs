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

namespace Nequeo.Wpf.NequeoCompany.Controls.Employees
{
    /// <summary>
    /// Interaction logic for Wage.xaml
    /// </summary>
    public partial class Wage : UserControl, Nequeo.IPropertyChanged
    {
        /// <summary>
        /// Wage window
        /// </summary>
        public Wage()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IEmployee _employee;
        private bool _hasChanged = false;
        private bool _loading = false;
        private bool _addNew = false;
        private bool _updateAttempt = false;
        private Dictionary<int, bool> _valid = new Dictionary<int, bool>();
        private Data.Enum.EnumPayPeriodIntervalType _payPeriodInterval = Data.Enum.EnumPayPeriodIntervalType.Weekly;
        private long _employeeID = 0;
        private double _taxRate = 0;
        private decimal _annualWage = 0;
        private double _baseHours = 0;
        private double _wageInclude = 0;
        private double _bCoefficient = 0;

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

                // Set the connection type model mapping.
                if (_employee != null)
                    dataAccess.ConnectionTypeModel = Nequeo.Data.Operation.GetTypeModel
                        <DataAccess.NequeoCompany.Data.Wages, DataAccess.NequeoCompany.Data.Employees>(_employee.Current.Extension.Common);
            }
        }

        /// <summary>
        /// Gets sets the Pay Interval calculation type.
        /// </summary>
        public Data.Enum.EnumPayPeriodIntervalType PayPeriodIntervalType
        {
            get { return _payPeriodInterval; }
            set { _payPeriodInterval = value; }
        }

        /// <summary>
        /// Gets sets the b Coefficient.
        /// </summary>
        public double BCoefficient
        {
            get { return _bCoefficient; }
            set { _bCoefficient = value; }
        }

        /// <summary>
        /// Gets sets the wage include.
        /// </summary>
        public double WageInclude
        {
            get { return _wageInclude; }
            set { _wageInclude = value; }
        }

        /// <summary>
        /// Gets sets the week base hour.
        /// </summary>
        public double WeekBaseHours
        {
            get { return _baseHours; }
            set { _baseHours = value; }
        }

        /// <summary>
        /// Gets sets the tax rate.
        /// </summary>
        public double TaxRate
        {
            get { return _taxRate; }
            set { _taxRate = value; }
        }

        /// <summary>
        /// Gets sets the annual wage.
        /// </summary>
        public decimal AnnualWage
        {
            get { return _annualWage; }
            set { _annualWage = value; }
        }

        /// <summary>
        /// Gets sets, the current employee id.
        /// </summary>
        public long EmployeeID
        {
            get { return _employeeID; }
            set
            {
                _employeeID = value;

                if (_employeeID > 0)
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
                details.Add("Employee Wages property/properties have changed.");

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
        /// Hours changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal isDecimal = 0;

            // Make sure a valid value exits.
            if (txtHours != null)
                if (Decimal.TryParse(txtHours.Text, out isDecimal))
                    if (!String.IsNullOrEmpty(txtHours.Text))
                        // Calculated value.
                        CalculateChange();
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
                    selectItem.OrderByClause = "WageID DESC";
                    selectItem.WhereClause = "EmployeeID = " + _employeeID.ToString();
                    selectItem.ShowDialog();

                    // Has an item been selected.
                    if (selectItem.SelectedRecord != null)
                    {
                        // Get the selected item.
                        DataAccess.NequeoCompany.Data.Wages data = (DataAccess.NequeoCompany.Data.Wages)selectItem.SelectedRecord;

                        // Assign the load item.
                        dataAccess.OrderByClause = selectItem.OrderByClause;
                        dataAccess.WhereClause = "WageID = " + data.WageID.ToString();
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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtEmployeeWageID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Wages data = (DataAccess.NequeoCompany.Data.Wages)bindingExpression.DataItem;

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
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(txtEmployeeWageID, TextBox.TextProperty);
            DataAccess.NequeoCompany.Data.Wages data = (DataAccess.NequeoCompany.Data.Wages)bindingExpression.DataItem;
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
            DataAccess.NequeoCompany.Data.Wages[] returnedDataList = (DataAccess.NequeoCompany.Data.Wages[])dataAccess.DataModel;
            DataAccess.NequeoCompany.Data.Wages data = returnedDataList[0];

            // Attach to the property changed event within the model
            data.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(data_PropertyChanged);
            SetChangePropertyState(false);

            // Assign the data to the data context.
            gridEmployeeWage.DataContext = data;

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
                txtEmployeeWageID.Text = "0";
                txtEmployeeID.Text = _employeeID.ToString();
                txtEmployeeAccountID.Text = "";
                txtPaymentDate.SelectedDate = DateTime.Now;
                txtHours.Text = "";
                txtRate.Text = "";
                txtNetAmount.Text = "";
                txtPayg.Text = "";
                txtDescription.Text = "";
                txtComments.Text = "";

                // Load all list items.
                LoadListItems();
                IsSelectedIndexValid();

                // Calculated value.
                Calculate();
            }
        }

        /// <summary>
        /// On select new super fund
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectBankAccount_Click(object sender, RoutedEventArgs e)
        {
            // Get a new instance of the connection type model.
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel =
                Nequeo.Wpf.Common.Operation.GetTypeModel<DataAccess.NequeoCompany.Data.EmployeeBankAccounts>(dataAccess);

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
                DataAccess.NequeoCompany.Data.EmployeeBankAccounts data = (DataAccess.NequeoCompany.Data.EmployeeBankAccounts)selectItem.SelectedRecord;
                txtEmployeeAccountID.Text = data.AccountID.ToString();
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
                    groupBoxEmployeeWageContainer.Header = " * changed";
                    groupBoxEmployeeWageContainer.Foreground = Brushes.Red;
                }
                else
                {
                    groupBoxEmployeeWageContainer.Header = null;
                    groupBoxEmployeeWageContainer.Foreground = Brushes.Black;
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
                    groupBoxEmployeeWageContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = true;
                    dataAccess.IsUpdateEnabled = true;
                    btnSelectBankAccount.IsEnabled = true;
                    break;

                // Insert, Delete
                case 2:
                case 3:
                    groupBoxEmployeeWageContainer.IsEnabled = false;
                    dataAccess.IsInsertEnabled = false;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectBankAccount.IsEnabled = false;
                    break;

                // Add New
                case 4:
                    groupBoxEmployeeWageContainer.IsEnabled = true;
                    dataAccess.IsInsertEnabled = true;
                    dataAccess.IsDeleteEnabled = false;
                    dataAccess.IsUpdateEnabled = false;
                    btnSelectBankAccount.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Calculate hours onchange values.
        /// </summary>
        private void CalculateChange()
        {
            // If annual wage and tax rate exists.
            if (_annualWage > 0 && _taxRate > 0 && _baseHours > 0 && _wageInclude > 0 && _bCoefficient > 0)
            {
                string delimStr = ".";
                char[] delimiter = delimStr.ToCharArray();

                // Calculate the weekly rate.
                decimal hours = decimal.Parse(txtHours.Text);
                decimal rate = ((decimal)_annualWage / (decimal)52) / (decimal)_baseHours;

                // Get the total amount whole dollars
                string[] splitAmount = Convert.ToString(hours * rate).Split(delimiter, 100);
                int intAmount = Convert.ToInt32(splitAmount[0].ToString().Trim());

                // Set the Pay Interval calculation point.
                switch (_payPeriodInterval)
                {
                    case Data.Enum.EnumPayPeriodIntervalType.Weekly:
                        splitAmount = Convert.ToString(hours * rate).Split(delimiter, 100);
                        intAmount = Convert.ToInt32(splitAmount[0].ToString().Trim());
                        break;

                    case Data.Enum.EnumPayPeriodIntervalType.Fortnightly:
                        splitAmount = Convert.ToString((hours / (decimal)2) * rate).Split(delimiter, 100);
                        intAmount = Convert.ToInt32(splitAmount[0].ToString().Trim());
                        break;

                    case Data.Enum.EnumPayPeriodIntervalType.Monthly:
                        splitAmount = Convert.ToString((hours / (decimal)4) * rate).Split(delimiter, 100);
                        intAmount = Convert.ToInt32(splitAmount[0].ToString().Trim());
                        break;

                    default:
                        throw new Exception("Pay Period Interval calculation type has not been specified.");
                }

                // Get the total amount including wage include whole dollars
                string[] splitAmountWageIncl = Convert.ToString(intAmount + _wageInclude).Split(delimiter, 100);
                int amount = Convert.ToInt32(splitAmountWageIncl[0].ToString().Trim());

                // Get the payg amount apply formula ( y = mx - b, y = payg, m = wage, x = tax rate, b = coefficient).
                decimal payg = ((decimal)amount * ((decimal)_taxRate / (decimal)100)) - (decimal)_bCoefficient;
                decimal accountAmount = 0;

                // Set the Pay Interval calculation point.
                switch (_payPeriodInterval)
                {
                    case Data.Enum.EnumPayPeriodIntervalType.Weekly:
                        accountAmount = (decimal)hours * (decimal)rate;
                        break;

                    case Data.Enum.EnumPayPeriodIntervalType.Fortnightly:
                        payg = payg * (decimal)2;
                        accountAmount = ((decimal)hours / (decimal)2) * (decimal)rate;
                        accountAmount = accountAmount * (decimal)2;
                        break;

                    case Data.Enum.EnumPayPeriodIntervalType.Monthly:
                        payg = payg * (decimal)4;
                        accountAmount = ((decimal)hours / (decimal)4) * (decimal)rate;
                        accountAmount = accountAmount * (decimal)4;
                        break;

                    default:
                        throw new Exception("Pay Period Interval calculation type has not been specified.");
                }

                // Calculate the net amount
                decimal netAmount = accountAmount - payg;

                // Assign the calculated super amount.
                txtRate.Text = (rate > 0 ? rate.ToString("#.#####") : "");
                txtNetAmount.Text = (netAmount > 0 ? netAmount.ToString("#.##") : "");
                txtPayg.Text = (payg > 0 ? payg.ToString("#.##") : "");
            }
        }

        /// <summary>
        /// Calculate values
        /// </summary>
        private void Calculate()
        {
            // If base hours exits
            if(_baseHours > 0)
            {
                double hours = 0;

                // Set the Pay Interval calculation point.
                switch (_payPeriodInterval)
                {
                    case Data.Enum.EnumPayPeriodIntervalType.Weekly:
                        hours = _baseHours;
                        break;
                    case Data.Enum.EnumPayPeriodIntervalType.Fortnightly:
                        hours = _baseHours * (double)2;
                        break;
                    case Data.Enum.EnumPayPeriodIntervalType.Monthly:
                        hours = _baseHours * (double)4;
                        break;
                    default:
                        throw new Exception("Pay Period Interval calculation type has not been specified.");
                }

                // Assign the total hours
                txtHours.Text = hours.ToString("#.####");
            }
        }
    }
}
