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
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Runtime.InteropServices;

using Nequeo.Extension;

namespace Nequeo.Invention
{
    /// <summary>
    /// Convert specified objects to standard formats.
    /// </summary>
    public sealed class Converter
    {
        /// <summary>
        /// Convert a time span to the standard duration format.
        /// </summary>
        /// <param name="timeSpan">The time span to convert</param>
        /// <returns>The formatted string.</returns>
        /// <remarks>P[n]Y[n]M[n]DT[n]H[n]M[n]S : TimeSpan in the ISO 8601 Duration format</remarks>
        /// <![CDATA[
        /// P is the duration designator (historically called "period") placed at the start of the duration representation.
        /// Y is the year designator that follows the value for the number of years.
        /// M is the month designator that follows the value for the number of months.
        /// W is the week designator that follows the value for the number of weeks.
        /// D is the day designator that follows the value for the number of days.
        /// T is the time designator that precedes the time components of the representation.
        /// H is the hour designator that follows the value for the number of hours.
        /// M is the minute designator that follows the value for the number of minutes.
        /// S is the second designator that follows the value for the number of seconds.]]>
        public static string TimeSpanToDurationFormat(TimeSpan timeSpan)
        {
            StringBuilder builder = new StringBuilder();

            // Append the starting period.
            builder.Append("P");

            // Append the days
            if(timeSpan.Days > 0)
                builder.Append(timeSpan.Days.ToString() + "D");

            // Append the starting time.
            builder.Append("T");

            // Append the hours
            if(timeSpan.Hours > 0)
                builder.Append(timeSpan.Hours.ToString() + "H");

            // Append the minutes
            if (timeSpan.Minutes > 0)
                builder.Append(timeSpan.Minutes.ToString() + "M");

            // Append the seconds and milliseconds
            if (timeSpan.Seconds > 0 && timeSpan.Milliseconds > 0)
                builder.Append(timeSpan.Seconds.ToString() + "." + timeSpan.Milliseconds.ToString() + "S");
            else if (timeSpan.Seconds > 0 && timeSpan.Milliseconds < 1)
                builder.Append(timeSpan.Seconds.ToString() + "S");
            else if (timeSpan.Seconds < 1 && timeSpan.Milliseconds > 0)
            {
                if(timeSpan.Milliseconds >= 1000)
                    builder.Append((timeSpan.Milliseconds / 1000).ToString() + "S");
                else
                    builder.Append(timeSpan.Milliseconds.ToString() + "S");
            }

            // Return the formatted string.
            return builder.ToString();
        }

        /// <summary>
        /// Convert a standard duration format to time span.
        /// </summary>
        /// <param name="timeSpan">The time span duration.</param>
        /// <returns>The new time span.</returns>
        /// <remarks>P[n]Y[n]M[n]DT[n]H[n]M[n]S : TimeSpan in the ISO 8601 Duration format</remarks>
        /// <![CDATA[
        /// P is the duration designator (historically called "period") placed at the start of the duration representation.
        /// Y is the year designator that follows the value for the number of years.
        /// M is the month designator that follows the value for the number of months.
        /// W is the week designator that follows the value for the number of weeks.
        /// D is the day designator that follows the value for the number of days.
        /// T is the time designator that precedes the time components of the representation.
        /// H is the hour designator that follows the value for the number of hours.
        /// M is the minute designator that follows the value for the number of minutes.
        /// S is the second designator that follows the value for the number of seconds.]]>
        public static TimeSpan DurationFormatToTimeSpan(string timeSpan)
        {
            double ticks = 0;
            double divideFactor = 1000;

            // Split between the time indicator.
            string[] peroidTime = timeSpan.ToUpper().Split(new char[] { 'T' }, StringSplitOptions.None);

            // Get the peroid and time split values.
            string peroid = peroidTime[0].ToUpper().TrimStart('P');
            string time = (peroidTime.Length > 1 ? peroidTime[1].ToUpper() : string.Empty);

            // If time exists then get the time value.
            if (!String.IsNullOrEmpty(time))
                ticks = GetDurationTime(time, ticks);

            // Get the period duration.
            ticks = GetDurationPeriod(peroid, ticks);

            // Return the new time span.
            return new TimeSpan(Convert.ToInt64(ticks) * Convert.ToInt64(10000000 / divideFactor));
        }

        /// <summary>
        /// Get the duration time.
        /// </summary>
        /// <param name="time">The current time string.</param>
        /// <param name="value">The current time value.</param>
        /// <returns>The total number of ticks.</returns>
        private static double GetDurationTime(string time, double value)
        {
            double ticks = 0;
            double multFactor = 1000;

            if (time.Contains("H"))
                ticks = GetDurationTimeEx(time, "H", multFactor * 60 * 60) + value;

            else if (time.Contains("M"))
                ticks = GetDurationTimeEx(time, "M", multFactor * 60) + value;

            else if (time.Contains("S"))
                ticks = GetDurationTimeEx(time, "S", multFactor) + value;

            else
                ticks = value;

            return ticks;
        }

        /// <summary>
        /// Get the duration period.
        /// </summary>
        /// <param name="period">The current period string.</param>
        /// <param name="value">The current period value.</param>
        /// <returns>The total number of ticks.</returns>
        private static double GetDurationPeriod(string period, double value)
        {
            double ticks = 0;
            double multFactor = 1000;

            if (period.Contains("Y"))
                ticks = GetDurationPeriodEx(period, "Y", multFactor * 60 * 60 * 24 * 365) + value;

            else if (period.Contains("M"))
                ticks = GetDurationPeriodEx(period, "M", multFactor * 60 * 60 * 24 * 30) + value;

            else if (period.Contains("W"))
                ticks = GetDurationPeriodEx(period, "W", multFactor * 60 * 60 * 24 * 7) + value;

            else if (period.Contains("D"))
                ticks = GetDurationPeriodEx(period, "D", multFactor * 60 * 60 * 24) + value;

            else
                ticks = value;

            return ticks;
        }

        /// <summary>
        /// Get the duration time.
        /// </summary>
        /// <param name="time">The current time string.</param>
        /// <param name="search">The current search time.</param>
        /// <param name="value">The current time value.</param>
        /// <returns>The total number of ticks.</returns>
        private static double GetDurationTimeEx(string time, string search, double value)
        {
            double ticks = 0;

            string[] splitTime = time.Split(new string[] { search }, StringSplitOptions.None);
            ticks = Convert.ToDouble(splitTime[0].Trim()) * value;

            if (splitTime.Length > 1)
                ticks = GetDurationTime(splitTime[1].Trim(), ticks);

            return ticks;
        }

        /// <summary>
        /// Get the duration period.
        /// </summary>
        /// <param name="period">The current period string.</param>
        /// <param name="search">The current search period.</param>
        /// <param name="value">The current period value.</param>
        /// <returns>The total number of ticks.</returns>
        private static double GetDurationPeriodEx(string period, string search, double value)
        {
            double ticks = 0;

            string[] splitTime = period.Split(new string[] { search }, StringSplitOptions.None);
            ticks = Convert.ToDouble(splitTime[0].Trim()) * value;

            if (splitTime.Length > 1)
                ticks = GetDurationPeriod(splitTime[1].Trim(), ticks);

            return ticks;
        }

        /// <summary>
        /// Gets the short scale name for the number (e.g. Billion).
        /// </summary>
        /// <param name="number">The number to get the name for.</param>
        /// <param name="newNumber">The new number for the given name; else the original number.</param>
        /// <param name="exponentialNotation">The exponential notation value (e.g. 9); else zero.</param>
        /// <param name="metricPrefixName">The metric prefix name (e.g. giga); else empty.</param>
        /// <param name="metricPrefixSymbol">The metric prefix symbol (e.g. G); else empty.</param>
        /// <returns>The name of the number; else null;</returns>
        public static string GetNumberShortScaleName(double number, out string newNumber, out int exponentialNotation, out string metricPrefixName, out string metricPrefixSymbol)
        {
            bool isPositiveExponent = true;
            string[] split = number.ToString().Split(new char[] { 'e', 'E' }, StringSplitOptions.None);

            // If only one item then.
            if (split.Length < 2)
            {
                // Term the number into scientific format.
                string expoNumber = string.Format("{0:E}", double.Parse(split[0]));

                // Get the number and exponent.
                split = expoNumber.Split(new char[] { 'e', 'E' }, StringSplitOptions.None);
            }

            // Get the exponential notation value.
            exponentialNotation = Int32.Parse(split[1]);

            // Is the exponential notation negative.
            if (exponentialNotation < 0)
                isPositiveExponent = false;

            int difference = 0;
            int differenceFactor = 1;
            string newNumberValue = string.Empty;
            string scaleName = "None";
            metricPrefixName = "";
            metricPrefixSymbol = "";

            // Select the exponential notation value.
            switch (exponentialNotation)
            {
                case -1:
                    difference = exponentialNotation + 1;
                    scaleName = "Tenth";
                    metricPrefixName = "deci";
                    metricPrefixSymbol = "d";
                    break;

                case -2:
                    difference = exponentialNotation + 2;
                    scaleName = "Hundredth";
                    metricPrefixName = "centi";
                    metricPrefixSymbol = "c";
                    break;

                case -3:
                case -4:
                case -5:
                    difference = exponentialNotation + 3;
                    scaleName = "Thousandth";
                    metricPrefixName = "milli";
                    metricPrefixSymbol = "m";
                    break;

                case -6:
                case -7:
                case -8:
                    difference = exponentialNotation + 6;
                    scaleName = "Millionth";
                    metricPrefixName = "micro";
                    metricPrefixSymbol = "u";
                    break;

                case -9:
                case -10:
                case -11:
                    difference = exponentialNotation + 9;
                    scaleName = "Billionth";
                    metricPrefixName = "nano";
                    metricPrefixSymbol = "n";
                    break;

                case -12:
                case -13:
                case -14:
                    difference = exponentialNotation + 12;
                    scaleName = "Trillionth";
                    metricPrefixName = "pico";
                    metricPrefixSymbol = "p";
                    break;

                case -15:
                case -16:
                case -17:
                    difference = exponentialNotation + 15;
                    scaleName = "Quadrillionth";
                    metricPrefixName = "femto";
                    metricPrefixSymbol = "f";
                    break;

                case -18:
                case -19:
                case -20:
                    difference = exponentialNotation + 18;
                    scaleName = "Quintillionth";
                    metricPrefixName = "atto";
                    metricPrefixSymbol = "a";
                    break;

                case -21:
                case -22:
                case -23:
                    difference = exponentialNotation + 21;
                    scaleName = "Sextillionth";
                    metricPrefixName = "zepto";
                    metricPrefixSymbol = "z";
                    break;

                case -24:
                case -25:
                case -26:
                    difference = exponentialNotation + 24;
                    scaleName = "Septillionth";
                    metricPrefixName = "yocto";
                    metricPrefixSymbol = "y";
                    break;

                case 0:
                    difference = exponentialNotation - 0;
                    scaleName = "One";
                    metricPrefixName = "";
                    metricPrefixSymbol = "";
                    break;

                case 1:
                    difference = exponentialNotation - 1;
                    scaleName = "Ten";
                    metricPrefixName = "deca";
                    metricPrefixSymbol = "da";
                    break;

                case 2:
                    difference = exponentialNotation - 2;
                    scaleName = "Hundred";
                    metricPrefixName = "hecto";
                    metricPrefixSymbol = "h";
                    break;

                case 3:
                case 4:
                case 5:
                    difference = exponentialNotation - 3;
                    scaleName = "Thousand";
                    metricPrefixName = "kilo";
                    metricPrefixSymbol = "K";
                    break;

                case 6:
                case 7:
                case 8:
                    difference = exponentialNotation - 6;
                    scaleName = "Million";
                    metricPrefixName = "mega";
                    metricPrefixSymbol = "M";
                    break;

                case 9:
                case 10:
                case 11:
                    difference = exponentialNotation - 9;
                    scaleName = "Billion";
                    metricPrefixName = "giga";
                    metricPrefixSymbol = "G";
                    break;

                case 12:
                case 13:
                case 14:
                    difference = exponentialNotation - 12;
                    scaleName = "Trillion";
                    metricPrefixName = "tera";
                    metricPrefixSymbol = "T";
                    break;

                case 15:
                case 16:
                case 17:
                    difference = exponentialNotation - 15;
                    scaleName = "Quadrillion";
                    metricPrefixName = "peta";
                    metricPrefixSymbol = "P";
                    break;

                case 18:
                case 19:
                case 20:
                    difference = exponentialNotation - 18;
                    scaleName = "Quintillion";
                    metricPrefixName = "exa";
                    metricPrefixSymbol = "E";
                    break;

                case 21:
                case 22:
                case 23:
                    difference = exponentialNotation - 21;
                    scaleName = "Sextillion";
                    metricPrefixName = "zetta";
                    metricPrefixSymbol = "Z";
                    break;

                case 24:
                case 25:
                case 26:
                    difference = exponentialNotation - 24;
                    scaleName = "Septillion";
                    metricPrefixName = "yotta";
                    metricPrefixSymbol = "Y";
                    break;

                case 27:
                case 28:
                case 29:
                    difference = exponentialNotation - 27;
                    scaleName = "Octillion";
                    break;

                case 30:
                case 31:
                case 32:
                    difference = exponentialNotation - 30;
                    scaleName = "Nonillion";
                    break;

                case 33:
                case 34:
                case 35:
                    difference = exponentialNotation - 33;
                    scaleName = "Decillion";
                    break;

                case 36:
                case 37:
                case 38:
                    difference = exponentialNotation - 36;
                    scaleName = "Undecillion";
                    break;

                case 39:
                case 40:
                case 41:
                    difference = exponentialNotation - 39;
                    scaleName = "Duodecillion";
                    break;

                case 42:
                case 43:
                case 44:
                    difference = exponentialNotation - 42;
                    scaleName = "Tredecillion";
                    break;

                case 45:
                case 46:
                case 47:
                    difference = exponentialNotation - 45;
                    scaleName = "Quattuordecillion";
                    break;

                case 48:
                case 49:
                case 50:
                    difference = exponentialNotation - 48;
                    scaleName = "Quindecillion";
                    break;

                case 51:
                case 52:
                case 53:
                    difference = exponentialNotation - 51;
                    scaleName = "Sexdecillion";
                    break;

                case 54:
                case 55:
                case 56:
                    difference = exponentialNotation - 54;
                    scaleName = "Septendecillion";
                    break;

                case 57:
                case 58:
                case 59:
                    difference = exponentialNotation - 57;
                    scaleName = "Octodecillion";
                    break;

                case 60:
                case 61:
                case 62:
                    difference = exponentialNotation - 60;
                    scaleName = "Novendecillion";
                    break;

                case 63:
                case 64:
                case 65:
                    difference = exponentialNotation - 63;
                    scaleName = "Vigintillion";
                    break;

                case 66:
                case 67:
                case 68:
                    difference = exponentialNotation - 66;
                    scaleName = "Unvigintillion";
                    break;

                case 69:
                case 70:
                case 71:
                    difference = exponentialNotation - 69;
                    scaleName = "Duovigintillion";
                    break;

                case 72:
                case 73:
                case 74:
                    difference = exponentialNotation - 72;
                    scaleName = "Tresvigintillion";
                    break;

                case 75:
                case 76:
                case 77:
                    difference = exponentialNotation - 75;
                    scaleName = "Quattuorvigintillion";
                    break;

                case 78:
                case 79:
                case 80:
                    difference = exponentialNotation - 78;
                    scaleName = "Quinquavigintillion";
                    break;

                case 81:
                case 82:
                case 83:
                    difference = exponentialNotation - 81;
                    scaleName = "Sexvigintillion";
                    break;

                case 84:
                case 85:
                case 86:
                    difference = exponentialNotation - 84;
                    scaleName = "Septemvigintillion";
                    break;

                case 87:
                case 88:
                case 89:
                    difference = exponentialNotation - 87;
                    scaleName = "Octovigintillion";
                    break;

                case 90:
                case 91:
                case 92:
                    difference = exponentialNotation - 90;
                    scaleName = "Novemvigintillion";
                    break;

                case 93:
                case 94:
                case 95:
                    difference = exponentialNotation - 93;
                    scaleName = "Trigintillion";
                    break;

                case 96:
                case 97:
                case 98:
                    difference = exponentialNotation - 96;
                    scaleName = "Untrigintillion";
                    break;

                case 99:
                case 100:
                case 101:
                    difference = exponentialNotation - 99;
                    scaleName = "Duotrigintillion";
                    break;

                case 102:
                case 103:
                case 104:
                    difference = exponentialNotation - 102;
                    scaleName = "Trestrigintillion";
                    break;

                case 105:
                case 106:
                case 107:
                    difference = exponentialNotation - 105;
                    scaleName = "Quattuortrigintillion";
                    break;

                case 108:
                case 109:
                case 110:
                    difference = exponentialNotation - 108;
                    scaleName = "Quinquatrigintillion";
                    break;

                case 111:
                case 112:
                case 113:
                    difference = exponentialNotation - 111;
                    scaleName = "Sextrigintillion";
                    break;

                case 114:
                case 115:
                case 116:
                    difference = exponentialNotation - 114;
                    scaleName = "Septentrigintillion";
                    break;

                case 117:
                case 118:
                case 119:
                    difference = exponentialNotation - 117;
                    scaleName = "Octotrigintillion";
                    break;

                case 120:
                case 121:
                case 122:
                    difference = exponentialNotation - 120;
                    scaleName = "Noventrigintillion";
                    break;

                case 123:
                case 124:
                case 125:
                    difference = exponentialNotation - 123;
                    scaleName = "Quadragintillion";
                    break;

                default:
                    scaleName = "None";
                    break;
            }

            // If the exponent is positive.
            if (isPositiveExponent)
            {
                // Create the new number value.
                newNumberValue = split[0].ToString().Replace(".", "").Insert(difference + differenceFactor, ".");
            }
            else
            {
                // Select the appropriate difference.
                switch (difference)
                {
                    case -2:
                        // Create the new number value.
                        newNumberValue = "0.0" + split[0].ToString().Replace(".", "");
                        break;

                    case -1:
                        // Create the new number value.
                        newNumberValue = "0." + split[0].ToString().Replace(".", "");
                        break;

                    case 0:
                    default:
                        // Create the new number value.
                        newNumberValue = split[0].ToString();
                        break;
                }
            }

            // Create the new number.
            newNumber = newNumberValue.ToString() + "E" + (isPositiveExponent ? "+" : "-") + System.Math.Abs(exponentialNotation - difference).ToString();

            // Return the scale name.
            return scaleName;
        }
    }
}

