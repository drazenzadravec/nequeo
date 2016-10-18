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

namespace Nequeo.Wpf.NequeoCompany.Controls.Vendors
{
    /// <summary>
    /// Interaction logic for VendorReport.xaml
    /// </summary>
    public partial class VendorReport : UserControl
    {
        /// <summary>
        /// Vendor window
        /// </summary>
        public VendorReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IVendor _vendor;

        /// <summary>
        /// Gets sets, the vendor injected interface.
        /// </summary>
        public Service.NequeoCompany.IVendor VendorDataSource
        {
            get { return _vendor; }
            set { _vendor = value; }
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtFromVendorDate.SelectedDate = DateTime.Now;
            txtToVendorDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Vendor report checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblVendorReportID_Checked(object sender, RoutedEventArgs e)
        {
            txtVendorReportID.Visibility = System.Windows.Visibility.Visible;
            btnSelectVendorReport.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Vendor report un-checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblVendorReportID_Unchecked(object sender, RoutedEventArgs e)
        {
            txtVendorReportID.Visibility = System.Windows.Visibility.Hidden;
            btnSelectVendorReport.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Vendor details report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectVendorReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Vendors, DataAccess.NequeoCompany.Data.Vendors>(_vendor.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "VendorID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Vendors data = (DataAccess.NequeoCompany.Data.Vendors)selectItem.SelectedRecord;
                txtVendorReportID.Text = data.VendorID.ToString();
            }
        }

        /// <summary>
        /// View report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewVendorReport_Click(object sender, RoutedEventArgs e)
        {
            if (lblVendorReportID.IsChecked.Value)
            {
                if (String.IsNullOrEmpty(txtVendorReportID.Text))
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _vendor.Current.ReportExtension.DetailFromToDate(txtFromVendorDate.SelectedDate, txtToVendorDate.SelectedDate);
                    _vendor.Current.ReportExtension.DetailFromToDateShow(data);
                }
                else
                {
                    // Get customer income for the interval
                    Nequeo.Model.DataSource.BindingSourceData[] data =
                        _vendor.Current.ReportExtension.DetailFromToDateID
                        (txtFromVendorDate.SelectedDate, txtToVendorDate.SelectedDate, Int32.Parse(txtVendorReportID.Text));
                    _vendor.Current.ReportExtension.DetailFromToDateIDShow(data);
                }
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _vendor.Current.ReportExtension.DetailFromToDate(txtFromVendorDate.SelectedDate, txtToVendorDate.SelectedDate);
                _vendor.Current.ReportExtension.DetailFromToDateShow(data);
            }
        }

        /// <summary>
        /// View all summary report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSummaryAll_Click(object sender, RoutedEventArgs e)
        {
            // Get customer income for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _vendor.Current.ReportExtension.DetailFromToDateSummary(txtFromVendorDate.SelectedDate, txtToVendorDate.SelectedDate);
            _vendor.Current.ReportExtension.DetailFromToDateSummaryShow(data);
        }

        /// <summary>
        /// Vendor report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectVendorDataReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Vendors, DataAccess.NequeoCompany.Data.Vendors>(_vendor.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "VendorID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Vendors data = (DataAccess.NequeoCompany.Data.Vendors)selectItem.SelectedRecord;
                txtVendorDataReportID.Text = data.VendorID.ToString();
            }
        }

        /// <summary>
        /// View vendor report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewVendorDataReport_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtVendorDataReportID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _vendor.Current.ReportExtension.Vendor();
                _vendor.Current.ReportExtension.VendorShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _vendor.Current.ReportExtension.Vendor(Int32.Parse(txtVendorDataReportID.Text));
                _vendor.Current.ReportExtension.VendorShow(data);
            }
        }
    }
}
