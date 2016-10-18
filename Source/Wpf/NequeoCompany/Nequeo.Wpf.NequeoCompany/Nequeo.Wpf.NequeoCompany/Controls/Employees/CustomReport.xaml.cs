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
    /// Interaction logic for CustomReport.xaml
    /// </summary>
    public partial class CustomReport : UserControl
    {
        /// <summary>
        /// Employee report
        /// </summary>
        public CustomReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IEmployee _employee;

        /// <summary>
        /// Gets sets, the customer injected interface.
        /// </summary>
        public Service.NequeoCompany.IEmployee EmployeeDataSource
        {
            get { return _employee; }
            set { _employee = value; }
        }

        /// <summary>
        /// Loaded user control event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Select employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectEmployeeReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Employees, DataAccess.NequeoCompany.Data.Employees>(_employee.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "EmployeeID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Employees data = (DataAccess.NequeoCompany.Data.Employees)selectItem.SelectedRecord;
                txtEmployeeReportID.Text = data.EmployeeID.ToString();
            }
        }

        /// <summary>
        /// View employee report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewEmployeeReport_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtEmployeeReportID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _employee.Current.ReportExtension.Employee();
                _employee.Current.ReportExtension.EmployeeShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _employee.Current.ReportExtension.Employee(Int32.Parse(txtEmployeeReportID.Text));
                _employee.Current.ReportExtension.EmployeeShow(data);
            }
        }
    }
}
