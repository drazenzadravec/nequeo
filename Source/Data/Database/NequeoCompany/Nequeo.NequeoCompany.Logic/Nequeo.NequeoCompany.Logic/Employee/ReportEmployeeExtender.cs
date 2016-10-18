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

using Nequeo.Data.Control;
using Nequeo.DataAccess.NequeoCompany;
using Nequeo.Report;
using Nequeo.Report.NequeoCompany;
using Nequeo.Report.Common;
using Nequeo.Data.Enum;

namespace Nequeo.Logic.NequeoCompany.Employee
{
    /// <summary>
    /// Employee report extender
    /// </summary>
    public partial class ReportEmployeeExtender
    {
        /// <summary>
        /// Create PaySlipFromToYear binding source data
        /// </summary>
        /// <param name="fromDate">The fromDate value</param>
        /// <param name="toDate">The toDate value</param>
        /// <param name="emplyeeID">The emplyeeID value</param>
        /// <param name="companyID">The companyID value</param>
        /// <param name="payPeriodInterval">The payPeriodInterval value</param>
        /// <returns><returns>The report binding source collection.</returns></returns>
        public virtual Nequeo.Model.DataSource.BindingSourceData[] PaySlipFromToYear(
            DateTime? fromDate, DateTime? toDate, int? emplyeeID, int? companyID, 
            EnumPayPeriodIntervalType payPeriodInterval)
        {
            DateTime payPeriodFromDate = (DateTime)toDate;
            DateTime payPeriodToDate = (DateTime)toDate;

            switch(payPeriodInterval)
            {
                case EnumPayPeriodIntervalType.Weekly:
                    TimeSpan payIntervalEndWeek = new TimeSpan(6, 0, 0, 0);
                    payPeriodFromDate = toDate.Value.Subtract(payIntervalEndWeek);
                    break;

                case EnumPayPeriodIntervalType.Fortnightly:
                    TimeSpan payIntervalEndFortnight = new TimeSpan(13, 0, 0, 0);
                    payPeriodFromDate = toDate.Value.Subtract(payIntervalEndFortnight);
                    break;

                case EnumPayPeriodIntervalType.Monthly:
                    TimeSpan payIntervalEndMonth = new TimeSpan(27, 0, 0, 0);
                    payPeriodFromDate = toDate.Value.Subtract(payIntervalEndMonth);
                    break;

                default:
                    throw new Exception("Pay period interval type not supported.");
            }

            // Return the pay slip report data.
            return base.PaySlipFromToYear(fromDate, toDate, emplyeeID, companyID, payPeriodFromDate, payPeriodToDate);
        }
    }
}
