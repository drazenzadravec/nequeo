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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Globalization;

using Nequeo.Extension;
using Nequeo.Collections.Extension;
using Nequeo.Threading.Extension;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Class used to build initialization script of jQuery plugin.
    /// </summary>
    public class ClientSideObjectWriter : IClientSideObjectWriter
    {
        private readonly string _id;
        private readonly string _type;
        private readonly TextWriter _writer;

        private bool _hasStarted;
        private bool _appended;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="type">The type.</param>
        /// <param name="textWriter">The text writer.</param>
        public ClientSideObjectWriter(string id, string type, TextWriter textWriter)
        {
            // If the instance object is null.
            if (id == null) throw new System.ArgumentNullException("id");
            if (type == null) throw new System.ArgumentNullException("type");
            if (textWriter == null) throw new System.ArgumentNullException("textWriter");

            _id = id;
            _type = type;
            _writer = textWriter;
        }

        /// <summary>
        /// Starts writing this instance.
        /// </summary>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Start()
        {
            if (_hasStarted)
                throw new InvalidOperationException("You cannot call start more than once");

            _writer.Write("jQuery('#{0}').{1}(".FormatWith(_id, _type));
            _hasStarted = true;

            return this;
        }

        /// <summary>
        /// Appends the specified key value pair to the end of this instance.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string keyValuePair)
        {
            EnsureStart();

            if (!string.IsNullOrEmpty(keyValuePair))
            {
                _writer.Write(_appended ? ", " : "{");
                _writer.Write(keyValuePair);

                if (!_appended)
                {
                    _appended = true;
                }
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, string value)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                string formattedValue = QuoteString(value);

                Append("{0}:'{1}'".FormatWith(name, formattedValue));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and nullable value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter AppendNullableString(string name, string value)
        {

            if (!string.IsNullOrEmpty(name) && value != null)
            {
                string formattedValue = QuoteString(value);

                Append("{0}:'{1}'".FormatWith(name, formattedValue));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, int value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Append("{0}:{1}".FormatWith(name, value));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, int value, int defaultValue)
        {
            if (value != defaultValue)
            {
                Append(name, value);
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, int? value)
        {
            if (value.HasValue)
            {
                Append(name, value.Value);
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, double value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Append("{0}:'{1}'".FormatWith(name, value));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, double? value)
        {
            if (value.HasValue)
            {
                Append(name, value.Value);
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, decimal value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Append("{0}:'{1}'".FormatWith(name, value));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, decimal? value)
        {
            if (value.HasValue)
            {
                Append(name, value.Value);
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, bool value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Append("{0}:{1}".FormatWith(name, value.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture)));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, bool value, bool defaultValue)
        {
            if (value != defaultValue)
            {
                Append(name, value);
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and only the date of the passed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="jQueryToolName">The jquery tool name. e.g. 'nequeo.datetime' that takes three parameters.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter AppendDateOnly(string name, DateTime value, string jQueryToolName)
        {
            if (!string.IsNullOrEmpty(name) && (value != DateTime.MinValue))
            {
                string dateValue = "new $." + jQueryToolName + "({0},{1},{2})".
                    FormatWith(value.Year.ToString("0000", CultureInfo.InvariantCulture), 
                    (value.Month - 1).ToString("00", CultureInfo.InvariantCulture), value.Day.ToString("00", CultureInfo.InvariantCulture));

                Append("{0}:{1}".FormatWith(name, dateValue));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and only the date of the passed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="jQueryToolName">The jquery tool name. e.g. 'nequeo.datetime' that takes three parameters.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter AppendDateOnly(string name, DateTime? value, string jQueryToolName)
        {
            if (!string.IsNullOrEmpty(name) && (value != null && value != DateTime.MinValue))
            {
                string dateValue = "new $." + jQueryToolName + "({0},{1},{2})".FormatWith(value.Value.Year.ToString("0000",
                    CultureInfo.InvariantCulture), (value.Value.Month - 1).ToString("00", CultureInfo.InvariantCulture),
                    value.Value.Day.ToString("00", CultureInfo.InvariantCulture));

                Append("{0}:{1}".FormatWith(name, dateValue));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and only the date of the passed.
        /// </summary>
        /// <param name="name">The name of the query</param>
        /// <param name="collection">The collection of dates</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter AppendDatesOnly(string name, IEnumerable<DateTime> collection)
        {
            if (collection.Count() > 0)
            {
                List<DateTime> dates = collection.ToList();
                dates.Sort();

                StringBuilder builder = new StringBuilder();

                int year = -1;
                int month = -1;
                bool yearAppended = false;
                bool monthAppended = false;

                foreach (DateTime date in dates)
                {
                    if (year != date.Year)
                    {
                        if (yearAppended)
                        {
                            if (monthAppended)
                            {
                                builder.Append("]");
                            }
                            builder.Append("}");
                            builder.Append(",");
                            yearAppended = false;
                        }
                        builder.Append("'");
                        builder.Append(date.Year);
                        builder.Append("':{");

                        monthAppended = false;
                    }
                    if (month != date.Month)
                    {
                        if (monthAppended)
                        {
                            builder.Append("]");
                            builder.Append(",");
                            monthAppended = false;
                        }
                        builder.Append("'");
                        builder.Append(date.Month - 1);
                        builder.Append("':[");
                    }

                    if (year == date.Year && month == date.Month)
                    {
                        builder.Append(",");
                    }
                    builder.Append(date.Day);

                    if (month != date.Month)
                    {
                        month = date.Month;
                        monthAppended = true;
                    }

                    if (year != date.Year)
                    {
                        year = date.Year;
                        yearAppended = true;
                    }
                }
                builder.Append("]}");
                Append("{0}:{{{1}}}".FormatWith(name, builder.ToString()));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, DateTime value)
        {
            if (!string.IsNullOrEmpty(name) && (value != DateTime.MinValue))
            {
                string dateValue = "new $.nequeo.datetime({0},{1},{2},{3},{4},{5},{6})".FormatWith(value.Year.ToString("0000",
                    CultureInfo.InvariantCulture), (value.Month - 1).ToString("00",
                    CultureInfo.InvariantCulture), value.Day.ToString("00",
                    CultureInfo.InvariantCulture), value.Hour.ToString("00",
                    CultureInfo.InvariantCulture), value.Minute.ToString("00",
                    CultureInfo.InvariantCulture), value.Second.ToString("00",
                    CultureInfo.InvariantCulture), value.Millisecond.ToString("000",
                    CultureInfo.InvariantCulture));

                Append("{0}:{1}".FormatWith(name, dateValue));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, DateTime? value)
        {
            if (value.HasValue)
            {
                Append(name, value.Value);
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, Action action)
        {
            if (!string.IsNullOrEmpty(name) && (action != null))
            {
                Append("{0}:".FormatWith(name));
                action();
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, IList<string> values)
        {
            if (!string.IsNullOrEmpty(name) && !values.IsNullOrEmpty())
            {
                List<string> stringValues = new List<string>(values.Count);

                foreach (string value in values)
                {
                    stringValues.Add("'{0}'".FormatWith(QuoteString(value)));
                }

                Append("{0}:[{1}]".FormatWith(name, string.Join(",", stringValues.ToArray())));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append(string name, IList<int> values)
        {
            if (!string.IsNullOrEmpty(name) && !values.IsNullOrEmpty())
            {
                List<string> stringValues = new List<string>();

                foreach (int value in values)
                {
                    stringValues.Add(value.ToString(CultureInfo.InvariantCulture));
                }

                Append("{0}:[{1}]".FormatWith(name, string.Join(",", stringValues.ToArray())));
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append<TEnum>(string name, TEnum value) where TEnum : IComparable, IFormattable
        {
            if (!string.IsNullOrEmpty(name))
            {
                ClientSideEnumValueAttribute valueAttribute = value.GetType().GetField(value.ToString())
                                                                             .GetCustomAttributes(true)
                                                                             .OfType<ClientSideEnumValueAttribute>()
                                                                             .FirstOrDefault();

                if (valueAttribute != null)
                {
                    Append("{0}:{1}".FormatWith(name, valueAttribute.Value));
                }
            }

            return this;
        }

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter Append<TEnum>(string name, TEnum value, TEnum defaultValue) where TEnum : IComparable, IFormattable
        {
            if (!value.Equals(defaultValue))
            {
                Append(name, value);
            }

            return this;
        }

        /// <summary>
        /// Append the object value. The value is formatted as a JSON.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="name">The name</param>
        /// <param name="value">The object value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter AppendCollection<T>(string name, IEnumerable<T> value)
        {
            return Append("{0}:{1}".FormatWith(name, new JavaScriptSerializer().Serialize(value)));
        }

        /// <summary>
        /// Append the object value. The value is formatted as a JSON.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The object value.</param>
        /// <returns>The client writer</returns>
        public IClientSideObjectWriter AppendObject(string name, object value)
        {
            return Append("{0}:{1}".FormatWith(name, new JavaScriptSerializer().Serialize(value)));
        }

        /// <summary>
        /// Completes this instance.
        /// </summary>
        public void Complete()
        {
            EnsureStart();

            if (_appended)
            {
                _writer.Write("}");
            }

            _writer.Write(");");

            _hasStarted = false;
            _appended = false;
        }

        /// <summary>
        /// Add the query string to the result.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>The query result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Needs refactoring")]
        private static string QuoteString(string value)
        {
            StringBuilder result = new StringBuilder();

            if (!string.IsNullOrEmpty(value))
            {
                int startIndex = 0;
                int count = 0;

                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];

                    if (c == '\r' || c == '\t' || c == '\"' || c == '\'' || c == '<' || c == '>' ||
                        c == '\\' || c == '\n' || c == '\b' || c == '\f' || c < ' ')
                    {
                        if (count > 0)
                        {
                            result.Append(value, startIndex, count);
                        }

                        startIndex = i + 1;
                        count = 0;
                    }

                    switch (c)
                    {
                        case '\r':
                            result.Append("\\r");
                            break;
                        case '\t':
                            result.Append("\\t");
                            break;
                        case '\"':
                            result.Append("\\\"");
                            break;
                        case '\\':
                            result.Append("\\\\");
                            break;
                        case '\n':
                            result.Append("\\n");
                            break;
                        case '\b':
                            result.Append("\\b");
                            break;
                        case '\f':
                            result.Append("\\f");
                            break;
                        case '\'':
                        case '>':
                        case '<':
                            AppendAsUnicode(result, c);
                            break;
                        default:
                            if (c < ' ')
                            {
                                AppendAsUnicode(result, c);
                            }
                            else
                            {
                                count++;
                            }

                            break;
                    }
                }

                if (result.Length == 0)
                {
                    result.Append(value);
                }
                else if (count > 0)
                {
                    result.Append(value, startIndex, count);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Append as unicode type.
        /// </summary>
        /// <param name="builder">The current string builder.</param>
        /// <param name="c">The character to append.</param>
        private static void AppendAsUnicode(StringBuilder builder, char c)
        {
            builder.Append("\\u");
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0:x4}", (int)c);
        }

        /// <summary>
        /// Make sure that start is called before.
        /// </summary>
        private void EnsureStart()
        {
            if (!_hasStarted)
            {
                throw new InvalidOperationException("You must call start prior calling this method");
            }
        }
    }
}
