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
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    public interface IReportExtension<TExtension>
        where TExtension : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension ReportExtension { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    public interface IReportExtension<TExtension, TExtension1>
        where TExtension : class, new()
        where TExtension1 : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension ReportExtension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 ReportExtension1 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    public interface IReportExtension<TExtension, TExtension1, TExtension2>
        where TExtension : class, new()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension ReportExtension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 ReportExtension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 ReportExtension2 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    /// <typeparam name="TExtension3">The extension type.</typeparam>
    public interface IReportExtension<TExtension, TExtension1, TExtension2, TExtension3>
        where TExtension : class, new()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
        where TExtension3 : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension ReportExtension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 ReportExtension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 ReportExtension2 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension3 ReportExtension3 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    /// <typeparam name="TExtension3">The extension type.</typeparam>
    /// <typeparam name="TExtension4">The extension type.</typeparam>
    public interface IReportExtension<TExtension, TExtension1, TExtension2, TExtension3, TExtension4>
        where TExtension : class, new()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
        where TExtension3 : class, new()
        where TExtension4 : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension ReportExtension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 ReportExtension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 ReportExtension2 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension3 ReportExtension3 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension4 ReportExtension4 { get; }

        #endregion

        #endregion
    }

    /// <summary>
    /// The generic extension type to extended.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <typeparam name="TExtension1">The extension type.</typeparam>
    /// <typeparam name="TExtension2">The extension type.</typeparam>
    /// <typeparam name="TExtension3">The extension type.</typeparam>
    /// <typeparam name="TExtension4">The extension type.</typeparam>
    /// <typeparam name="TExtension5">The extension type.</typeparam>
    public interface IReportExtension<TExtension, TExtension1, TExtension2, TExtension3, TExtension4, TExtension5>
        where TExtension : class, new()
        where TExtension1 : class, new()
        where TExtension2 : class, new()
        where TExtension3 : class, new()
        where TExtension4 : class, new()
        where TExtension5 : class, new()
    {
        #region IDataBase Interface

        #region Properties
        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension ReportExtension { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension1 ReportExtension1 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension2 ReportExtension2 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension3 ReportExtension3 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension4 ReportExtension4 { get; }

        /// <summary>
        /// Gets, the extension generic members.
        /// </summary>
        TExtension5 ReportExtension5 { get; }

        #endregion

        #endregion
    }
}
