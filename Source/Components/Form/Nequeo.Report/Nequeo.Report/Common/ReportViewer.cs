/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.Report.Common
{
    /// <summary>
    /// Report viewer converter
    /// </summary>
    public class ReportViewer
    {
        /// <summary>
        /// Get the collection of win form report data sources.
        /// </summary>
        /// <param name="reportBindingSource">The report binding source collection.</param>
        /// <returns>The report data source collection</returns>
        public static Microsoft.Reporting.WinForms.ReportDataSource[] WinFormsReportDataSource(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSource)
        {
            // Create a new generic report data source
            Microsoft.Reporting.WinForms.ReportDataSource[] reportDataSource =
                new Microsoft.Reporting.WinForms.ReportDataSource[reportBindingSource.Length];

            for (int i = 0; i < reportBindingSource.Length; i++)
                reportDataSource[i] = new Microsoft.Reporting.WinForms.ReportDataSource()
                {
                    Name = reportBindingSource[i].DataSourceName,
                    Value = reportBindingSource[i].DataSource
                };

            // Return the report data source collection.
            return reportDataSource;
        }

        /// <summary>
        /// Get the collection of win form report parameters.
        /// </summary>
        /// <param name="reportBindingSource">The report binding source collection.</param>
        /// <returns>The report parameter collection</returns>
        public static Microsoft.Reporting.WinForms.ReportParameter[] WinFormsReportParameter(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSource)
        {
            // Create a new generic report parameter
            List<Microsoft.Reporting.WinForms.ReportParameter> reportParameters = 
                new List<Microsoft.Reporting.WinForms.ReportParameter>();

            // For each parameter found.
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

            // Return the report parameter collection.
            return reportParameters.Count() > 0 ? reportParameters.ToArray() : null;
        }

        /// <summary>
        /// Get the collection of web form report data sources.
        /// </summary>
        /// <param name="reportBindingSource">The report binding source collection.</param>
        /// <returns>The report data source collection</returns>
        public static Microsoft.Reporting.WebForms.ReportDataSource[] WebFormsReportDataSource(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSource)
        {
            // Create a new generic report data source
            Microsoft.Reporting.WebForms.ReportDataSource[] reportDataSource =
                new Microsoft.Reporting.WebForms.ReportDataSource[reportBindingSource.Length];

            for (int i = 0; i < reportBindingSource.Length; i++)
                reportDataSource[i] = new Microsoft.Reporting.WebForms.ReportDataSource()
                {
                    Name = reportBindingSource[i].DataSourceName,
                    Value = reportBindingSource[i].DataSource
                };

            // Return the report data source collection.
            return reportDataSource;
        }

        /// <summary>
        /// Get the collection of web form report parameters.
        /// </summary>
        /// <param name="reportBindingSource">The report binding source collection.</param>
        /// <returns>The report parameter collection</returns>
        public static Microsoft.Reporting.WebForms.ReportParameter[] WebFormsReportParameter(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSource)
        {
            // Create a new generic report parameter
            List<Microsoft.Reporting.WebForms.ReportParameter> reportParameters =
                new List<Microsoft.Reporting.WebForms.ReportParameter>();

            // For each parameter found.
            foreach (Nequeo.Model.DataSource.BindingSourceData item in reportBindingSource)
            {
                if (item.BindingSourceParameters != null && item.BindingSourceParameters.Count() > 0)
                {
                    foreach (Nequeo.Model.DataSource.BindingSourceParameter reportParameter in item.BindingSourceParameters)
                    {
                        Microsoft.Reporting.WebForms.ReportParameter parameter = new Microsoft.Reporting.WebForms.ReportParameter();
                        parameter.Name = reportParameter.Name;
                        parameter.Values.Add(reportParameter.Value.ToString());
                        parameter.Visible = true;
                        reportParameters.Add(parameter);
                    }
                }
            }

            // Return the report parameter collection.
            return reportParameters.Count() > 0 ? reportParameters.ToArray() : null;
        }
    }
}
