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
using System.IO;

namespace Nequeo.Web.Mvc.Extended.Factory
{
    /// <summary>
    /// Defines the basic building block of creating client side object.
    /// </summary>
    public interface IClientSideObjectWriter
    {
        /// <summary>
        /// Starts writing this instance.
        /// </summary>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Start();

        /// <summary>
        /// Appends the specified key value pair to the end of this instance.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string keyValuePair);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, string value);

        /// <summary>
        /// Appends the specified name and nullable value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter AppendNullableString(string name, string value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, int value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, int value, int defaultValue);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, int? value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, double value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, double? value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, decimal value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, decimal? value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, bool value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, bool value, bool defaultValue);

        /// <summary>
        /// Appends the specified name and only the date of the passed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="date">The value.</param>
        /// <param name="jQueryToolName">The jquery tool name. e.g. 'nequeo.datetime' that takes three parameters.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter AppendDateOnly(string name, DateTime date, string jQueryToolName);

        /// <summary>
        /// Appends the specified name and only the date of the passed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="date">The value.</param>
        /// <param name="jQueryToolName">The jquery tool name. e.g. 'nequeo.datetime' that takes three parameters.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter AppendDateOnly(string name, DateTime? date, string jQueryToolName);

        /// <summary>
        /// Appends the specified name and only the dates of the passed.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dates">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter AppendDatesOnly(string name, IEnumerable<DateTime> dates);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, DateTime value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, DateTime? value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, Action action);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, IList<string> values);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append(string name, IList<int> values);

        /// <summary>
        /// Appends the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter AppendCollection<T>(string name, IEnumerable<T> value);

        /// <summary>
        /// Appends the object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter AppendObject(string name, object value);

        /// <summary>
        /// Appends the specified name and value to the end of this instance.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The client side object writer.</returns>
        IClientSideObjectWriter Append<TEnum>(string name, TEnum value, TEnum defaultValue) where TEnum : IComparable, IFormattable;

        /// <summary>
        /// Completes this instance.
        /// </summary>
        void Complete();
    }
}
