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

namespace Nequeo.Report.NequeoCompany.Viewer
{
    /// <summary>
    /// Viewer control base
    /// </summary>
    public abstract class ViewerControl : Nequeo.Report.Common.ViewerControl
    {
        /// <summary>
        /// Show the embedded report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        /// <param name="embeddedReport">The embedded report namespace.</param>
        /// <param name="formTitle">The title for the report.</param>
        protected void ShowEmbedded(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection,
            string embeddedReport, string formTitle)
        {
            // Display the report
            Viewer.ReportViewer viewer = new Viewer.ReportViewer(
                reportBindingSourceCollection,
                embeddedReport,
                formTitle);
            viewer.Show();
        }

        /// <summary>
        /// Show the local path report
        /// </summary>
        /// <param name="reportBindingSourceCollection">The report binding source collection.</param>
        /// <param name="reportPath">The report full path.</param>
        /// <param name="formTitle">The title for the report.</param>
        protected new void ShowReport(Nequeo.Model.DataSource.BindingSourceData[] reportBindingSourceCollection,
            string reportPath, string formTitle)
        {
            base.ShowReport(reportBindingSourceCollection, reportPath, formTitle);
        }
    }
}
