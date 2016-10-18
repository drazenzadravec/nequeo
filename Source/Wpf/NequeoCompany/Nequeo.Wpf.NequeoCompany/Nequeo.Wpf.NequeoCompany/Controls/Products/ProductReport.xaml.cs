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

namespace Nequeo.Wpf.NequeoCompany.Controls.Products
{
    /// <summary>
    /// Interaction logic for ProductReport.xaml
    /// </summary>
    public partial class ProductReport : UserControl
    {
        /// <summary>
        /// Product window
        /// </summary>
        public ProductReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.IProduct _product;

        /// <summary>
        /// Gets sets, the product injected interface.
        /// </summary>
        public Service.NequeoCompany.IProduct ProductDataSource
        {
            get { return _product; }
            set { _product = value; }
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
        /// Product report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectProductReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Products, DataAccess.NequeoCompany.Data.Products>(_product.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "ProductID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Products data = (DataAccess.NequeoCompany.Data.Products)selectItem.SelectedRecord;
                txtProductReportID.Text = data.ProductID.ToString();
            }
        }

        /// <summary>
        /// View product report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewProductReport_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtProductReportID.Text))
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _product.Current.ReportExtension.Product();
                _product.Current.ReportExtension.ProductShow(data);
            }
            else
            {
                // Get customer income for the interval
                Nequeo.Model.DataSource.BindingSourceData[] data =
                    _product.Current.ReportExtension.Product(Int32.Parse(txtProductReportID.Text));
                _product.Current.ReportExtension.ProductShow(data);
            }
        }
    }
}
