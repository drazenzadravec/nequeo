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

namespace Nequeo.Wpf.NequeoCompany.Controls.Assets
{
    /// <summary>
    /// Interaction logic for AssetReport.xaml
    /// </summary>
    public partial class AssetReport : UserControl
    {
        /// <summary>
        /// Asset reports
        /// </summary>
        public AssetReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IAsset _asset;

        /// <summary>
        /// Gets sets, the asset injected interface.
        /// </summary>
        public Service.NequeoCompany.IAsset AssetDataSource
        {
            get { return _asset; }
            set { _asset = value; }
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtFromAssetDate.SelectedDate = DateTime.Now;
            txtToAssetDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Select asset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAssetReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Assets, DataAccess.NequeoCompany.Data.Assets>(_asset.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "AssetID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Assets data = (DataAccess.NequeoCompany.Data.Assets)selectItem.SelectedRecord;
                txtAssetReportID.Text = data.AssetID.ToString();
            }
        }

        /// <summary>
        /// View asset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAssetReport_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtAssetReportID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _asset.Current.ReportExtension.Asset();
                _asset.Current.ReportExtension.AssetShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _asset.Current.ReportExtension.Asset(Int32.Parse(txtAssetReportID.Text));
                _asset.Current.ReportExtension.AssetShow(data);
            }
        }

        /// <summary>
        /// Select vendor asset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAsset_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Assets, DataAccess.NequeoCompany.Data.Assets>(_asset.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "AssetID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Assets data = (DataAccess.NequeoCompany.Data.Assets)selectItem.SelectedRecord;
                txtAssetID.Text = data.AssetID.ToString();
            }
        }

        /// <summary>
        /// View vendor asset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAsset_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtAssetID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _asset.Current.ReportExtension.Vendor(txtFromAssetDate.SelectedDate, txtToAssetDate.SelectedDate);
                _asset.Current.ReportExtension.VendorShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _asset.Current.ReportExtension.VendorAssetID(txtFromAssetDate.SelectedDate, txtToAssetDate.SelectedDate, Int32.Parse(txtAssetID.Text));
                _asset.Current.ReportExtension.VendorAssetIDShow(data);
            }
        }
    }
}
