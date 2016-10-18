using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nequeo.Report.NequeoCompany.Viewer
{
    /// <summary>
    /// Report viewer control.
    /// </summary>
    public partial class ReportViewerControl : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReportViewerControl()
        {
            InitializeComponent();
        }

        private string _reportEmbeddedResource;
        private System.Windows.Forms.BindingSource[] _bindingSource;
        private string _reportPath;
        private string[] _reportDataSourceName;
        private IEnumerable<Microsoft.Reporting.WinForms.ReportParameter> _reportParameters;
        private System.Windows.Forms.BindingSource[] _bindingSourceSubreport;
        private string[] _reportDataSourceNameSubreport;

        /// <summary>
        /// Gets sets, the data source binding, contains the data to load.
        /// </summary>
        public System.Windows.Forms.BindingSource[] BindingSource
        {
            get { return _bindingSource; }
            set { _bindingSource = value; }
        }

        /// <summary>
        /// Gets sets, the sub-report data source binding, contains the data to load.
        /// </summary>
        public System.Windows.Forms.BindingSource[] BindingSourceSubreport
        {
            get { return _bindingSourceSubreport; }
            set { _bindingSourceSubreport = value; }
        }

        /// <summary>
        /// Gets sets, the file system path of the local report.
        /// </summary>
        public string ReportPath
        {
            get { return _reportPath; }
            set { _reportPath = value; }
        }

        /// <summary>
        /// Gets sets, the name of the report to load.
        /// </summary>
        public string ReportEmbeddedResource
        {
            get { return _reportEmbeddedResource; }
            set { _reportEmbeddedResource = value; }
        }

        /// <summary>
        /// Gets sets, the report data source name used for loading the report.
        /// </summary>
        public string[] ReportDataSourceName
        {
            get { return _reportDataSourceName; }
            set { _reportDataSourceName = value; }
        }

        /// <summary>
        /// Gets sets, the sub-report data source name used for loading the report.
        /// </summary>
        public string[] ReportDataSourceNameSubreport
        {
            get { return _reportDataSourceNameSubreport; }
            set { _reportDataSourceNameSubreport = value; }
        }

        /// <summary>
        /// Gets sets, the collection of report parameters.
        /// </summary>
        public IEnumerable<Microsoft.Reporting.WinForms.ReportParameter> ReportParameters
        {
            get { return _reportParameters; }
            set { _reportParameters = value; }
        }

        /// <summary>
        /// On load control event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportViewerControl_Load(object sender, EventArgs e)
        {
            if (_bindingSource != null)
            {
                // If sub-report data exists.
                if (_bindingSourceSubreport != null)
                    reportViewer.LocalReport.SubreportProcessing += 
                        new Microsoft.Reporting.WinForms.SubreportProcessingEventHandler(LocalReport_SubreportProcessing);

                // For each data binding source
                for (int i = 0; i < _bindingSource.Length; i++)
                {
                    // Create a new generic report data source
                    Microsoft.Reporting.WinForms.ReportDataSource reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();

                    // Add the report data source to the report viewer
                    reportDataSource.Name = _reportDataSourceName[i];
                    reportDataSource.Value = _bindingSource[i];
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                }

                if (!String.IsNullOrEmpty(_reportPath))
                {
                    // Set the report embedded resource
                    reportViewer.LocalReport.ReportPath = _reportPath;
                }
                else if (!String.IsNullOrEmpty(_reportEmbeddedResource))
                {
                    // Set the report embedded resource
                    reportViewer.LocalReport.ReportEmbeddedResource = _reportEmbeddedResource;
                }

                // If report parameters exist.
                if (_reportParameters != null)
                    reportViewer.LocalReport.SetParameters(_reportParameters);

                reportViewer.RefreshReport();
            }
        }

        /// <summary>
        /// The local sub-report process event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WinForms.SubreportProcessingEventArgs e)
        {
            if (_bindingSourceSubreport != null)
            {
                // For each data binding source
                for (int i = 0; i < _bindingSourceSubreport.Length; i++)
                {
                    // Create a new generic report data source
                    Microsoft.Reporting.WinForms.ReportDataSource reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();

                    // Add the report data source to the report viewer
                    reportDataSource.Name = _reportDataSourceNameSubreport[i];
                    reportDataSource.Value = _bindingSourceSubreport[i];

                    // Add the sub-report bindings
                    e.DataSources.Add(reportDataSource);
                }
            }
        }
    }
}
