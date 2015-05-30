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
using System.Globalization;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// The globalization information
    /// </summary>
    public class GlobalizationInfo
    {
        private readonly IDictionary<string, object> globalization = new Dictionary<string, object>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cultureInfo">The culture information</param>
        public GlobalizationInfo(CultureInfo cultureInfo)
        {
            DateTimeFormatInfo dateTimeFormats = cultureInfo.DateTimeFormat;
            NumberFormatInfo numberFormats = cultureInfo.NumberFormat;

            globalization["shortDate"] = dateTimeFormats.ShortDatePattern;
            globalization["longDate"] = dateTimeFormats.LongDatePattern;
            globalization["fullDateTime"] = dateTimeFormats.FullDateTimePattern;
            globalization["sortableDateTime"] = dateTimeFormats.SortableDateTimePattern;
            globalization["universalSortableDateTime"] = dateTimeFormats.UniversalSortableDateTimePattern;
            globalization["generalDateShortTime"] = dateTimeFormats.ShortDatePattern + " " + dateTimeFormats.ShortTimePattern;
            globalization["generalDateTime"] = dateTimeFormats.ShortDatePattern + " " + dateTimeFormats.LongTimePattern;
            globalization["monthDay"] = dateTimeFormats.MonthDayPattern;
            globalization["monthYear"] = dateTimeFormats.YearMonthPattern;
            globalization["days"] = dateTimeFormats.DayNames;
            globalization["abbrDays"] = dateTimeFormats.AbbreviatedDayNames;
            globalization["abbrMonths"] = dateTimeFormats.AbbreviatedMonthNames;
            globalization["months"] = dateTimeFormats.MonthNames;
            globalization["am"] = dateTimeFormats.AMDesignator;
            globalization["pm"] = dateTimeFormats.PMDesignator;
            globalization["dateSeparator"] = dateTimeFormats.DateSeparator;
            globalization["timeSeparator"] = dateTimeFormats.TimeSeparator;

            globalization["currencydecimaldigits"] = numberFormats.CurrencyDecimalDigits;
            globalization["currencydecimalseparator"] = numberFormats.CurrencyDecimalSeparator;
            globalization["currencygroupseparator"] = numberFormats.CurrencyGroupSeparator;
            globalization["currencygroupsize"] = numberFormats.CurrencyGroupSizes[0];
            globalization["currencynegative"] = numberFormats.CurrencyNegativePattern;
            globalization["currencypositive"] = numberFormats.CurrencyPositivePattern;
            globalization["currencysymbol"] = numberFormats.CurrencySymbol;

            globalization["numericdecimaldigits"] = numberFormats.NumberDecimalDigits;
            globalization["numericdecimalseparator"] = numberFormats.NumberDecimalSeparator;
            globalization["numericgroupseparator"] = numberFormats.NumberGroupSeparator;
            globalization["numericgroupsize"] = numberFormats.NumberGroupSizes[0];
            globalization["numericnegative"] = numberFormats.NumberNegativePattern;

            globalization["percentdecimaldigits"] = numberFormats.PercentDecimalDigits;
            globalization["percentdecimalseparator"] = numberFormats.PercentDecimalSeparator;
            globalization["percentgroupseparator"] = numberFormats.PercentGroupSeparator;
            globalization["percentgroupsize"] = numberFormats.PercentGroupSizes[0];
            globalization["percentnegative"] = numberFormats.PercentNegativePattern;
            globalization["percentpositive"] = numberFormats.PercentPositivePattern;
            globalization["percentsymbol"] = numberFormats.PercentSymbol;
        }

        /// <summary>
        /// Converts to dictionary
        /// </summary>
        /// <returns>The collection of global data.</returns>
        public IDictionary<string, object> ToDictionary()
        {
            return globalization;
        }
    }
}
