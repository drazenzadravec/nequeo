using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nequeo.Report.NequeoCompany.Viewer
{
    /// <summary>
    /// Report viewer form.
    /// </summary>
    public partial class ReportViewer : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="bindingSource">The data source binding, contains the data to load.</param>
        /// <param name="reportEmbeddedResource">The name of the report to load.</param>
        /// <param name="reportDataSourceName">The report data source name used for loading the report.</param>
        /// <param name="reportText">The title for the report.</param>
        /// <param name="reportParameters">The collection of report parameters.</param>
        public ReportViewer(System.Windows.Forms.BindingSource[] bindingSource, string reportEmbeddedResource,
            string[] reportDataSourceName, string reportText, IEnumerable<Microsoft.Reporting.WinForms.ReportParameter> reportParameters = null)
        {
            InitializeComponent();

            this.Text = reportText;
            this.reportViewerControl1.BindingSource = bindingSource;
            this.reportViewerControl1.ReportEmbeddedResource = reportEmbeddedResource;
            this.reportViewerControl1.ReportDataSourceName = reportDataSourceName;
            this.reportViewerControl1.ReportParameters = reportParameters;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="reportBindingSource">The report binding source collection.</param>
        /// <param name="reportEmbeddedResource">The name of the report to load.</param>
        /// <param name="reportText">The title for the report.</param>
        public ReportViewer(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSource, string reportEmbeddedResource, string reportText)
        {
            InitializeComponent();

            this.Text = reportText;
            this.reportViewerControl1.BindingSource = reportBindingSource.Select(u => u.DataSource).ToArray();
            this.reportViewerControl1.ReportEmbeddedResource = reportEmbeddedResource;
            this.reportViewerControl1.ReportDataSourceName = reportBindingSource.Select(u => u.DataSourceName).ToArray();

            List<Microsoft.Reporting.WinForms.ReportParameter> reportParameters = new List<Microsoft.Reporting.WinForms.ReportParameter>();
            foreach (Nequeo.Model.DataSource.BindingSourceData item in reportBindingSource)
            {
                if (item.BindingSourceParameters != null && item.BindingSourceParameters.Count() > 0)
                {
                    foreach (Nequeo.Model.DataSource.BindingSourceParameter reportParameter in item.BindingSourceParameters)
                    {
                        Microsoft.Reporting.WinForms.ReportParameter parameter = new Microsoft.Reporting.WinForms.ReportParameter();
                        parameter.Name = reportParameter.Name;
                        parameter.Values.Add(reportParameter.Value.ToString());
                        parameter.Visible = true;
                        reportParameters.Add(parameter);
                    }
                }
            }
            if (reportParameters.Count() > 0)
                this.reportViewerControl1.ReportParameters = reportParameters.ToArray();
        }

        /// <summary>
        /// Gets sets, the sub-report data source binding, contains the data to load.
        /// </summary>
        public System.Windows.Forms.BindingSource[] BindingSourceSubreport
        {
            get { return this.reportViewerControl1.BindingSourceSubreport; }
            set { this.reportViewerControl1.BindingSourceSubreport = value; }
        }

        /// <summary>
        /// Gets sets, the sub-report data source name used for loading the report.
        /// </summary>
        public string[] ReportDataSourceNameSubreport
        {
            get { return this.reportViewerControl1.ReportDataSourceNameSubreport; }
            set { this.reportViewerControl1.ReportDataSourceNameSubreport = value; }
        }
    }
}
