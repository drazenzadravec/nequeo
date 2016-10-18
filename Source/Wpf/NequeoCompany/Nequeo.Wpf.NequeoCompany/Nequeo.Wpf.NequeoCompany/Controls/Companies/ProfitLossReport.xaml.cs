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

namespace Nequeo.Wpf.NequeoCompany.Controls.Companies
{
    /// <summary>
    /// Interaction logic for ProfitLossReport.xaml
    /// </summary>
    public partial class ProfitLossReport : UserControl
    {
        /// <summary>
        /// Company reports
        /// </summary>
        public ProfitLossReport()
        {
            InitializeComponent();
        }

        private Service.NequeoCompany.ICompany _company;

        /// <summary>
        /// Gets sets, the company injected interface.
        /// </summary>
        public Service.NequeoCompany.ICompany CompanyDataSource
        {
            get { return _company; }
            set { _company = value; }
        }

        /// <summary>
        /// User control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtStartFinancialYearDate.SelectedDate = DateTime.Now;
            txtStartBASQuarterDate.SelectedDate = DateTime.Now;
            txtStartTaxReturnDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// View profit loss report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewProfitLossReport_Click(object sender, RoutedEventArgs e)
        {
            // Get profit loss for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _company.Current.ReportExtension.ProfitLoss(txtStartFinancialYearDate.SelectedDate, 
                txtStartFinancialYearDate.SelectedDate.Value.AddYears(1).Subtract(new TimeSpan(1, 0, 0, 0, 0)));
            _company.Current.ReportExtension.ProfitLossShow(data);
        }

        /// <summary>
        /// View BAS report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewBASReport_Click(object sender, RoutedEventArgs e)
        {
            // Get profit loss for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _company.Current.ReportExtension.Bas(txtStartBASQuarterDate.SelectedDate, 
                txtStartBASQuarterDate.SelectedDate.Value.AddMonths(3).Subtract(new TimeSpan(1, 0, 0, 0, 0)));
            _company.Current.ReportExtension.BasShow(data);
        }

        /// <summary>
        /// View tax return
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewTaxReturnReport_Click(object sender, RoutedEventArgs e)
        {
            // Get profit loss for the interval
            Nequeo.Model.DataSource.BindingSourceData[] data =
                _company.Current.ReportExtension.TaxReturn(txtStartTaxReturnDate.SelectedDate,
                txtStartTaxReturnDate.SelectedDate.Value.AddYears(1).Subtract(new TimeSpan(1, 0, 0, 0, 0)), Int32.Parse(txtCompanyReportID.Text));
            _company.Current.ReportExtension.TaxReturnShow(data);
        }

        /// <summary>
        /// Select company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCompanyReport_Click(object sender, RoutedEventArgs e)
        {
            Nequeo.ComponentModel.ConnectionTypeModel connectionModel = Nequeo.Data.Operation.GetTypeModel
                       <DataAccess.NequeoCompany.Data.Companies, DataAccess.NequeoCompany.Data.Companies>(_company.Current.Extension.Common);

            // Show the selection form
            Nequeo.Wpf.DataGridWindow selectItem = new DataGridWindow();
            selectItem.ConnectionTypeModel = connectionModel;
            selectItem.LoadOnStart = true;
            selectItem.MaxRecords = 50;
            selectItem.OrderByClause = "CompanyID DESC";
            selectItem.ShowDialog();

            // Has an item been selected.
            if (selectItem.SelectedRecord != null)
            {
                // Get the selected item.
                DataAccess.NequeoCompany.Data.Companies data = (DataAccess.NequeoCompany.Data.Companies)selectItem.SelectedRecord;
                txtCompanyReportID.Text = data.CompanyID.ToString();
            }
        }
    }
}
