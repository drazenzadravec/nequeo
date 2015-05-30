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

using Nequeo.Model.Conversion;

namespace Nequeo.Conversion
{
    /// <summary>
    /// Conversion interface
    /// </summary>
    public interface IConvert
    {
        /// <summary>
        /// Write the data from the transform model.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        bool Write(Object data);

        /// <summary>
        /// Read the data and return into the transform model.
        /// </summary>
        /// <returns>The collection of data.</returns>
        Object Read();

        /// <summary>
        /// Before a write operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        bool OnWriteBegin();

        /// <summary>
        /// When a write operation from another process has completed execute this method.
        /// </summary>
        /// <param name="processResult">The process result from the preceeding operation.</param>
        /// <param name="errors">The list of errors if any.</param>
        /// <param name="recordsWritten">The number of records wriiten.</param>
        /// <returns>True if the execution was successful else false.</returns>
        bool OnWriteComplete(bool processResult, String[] errors, long recordsWritten);

        /// <summary>
        /// Before a read operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        bool OnReadBegin();

        /// <summary>
        /// When a read operation from another process has completed execute this method.
        /// </summary>
        /// <param name="processResult">The process result from the preceeding operation.</param>
        /// <param name="errors">The list of errors if any.</param>
        /// <param name="recordsRead">The number of records read.</param>
        /// <returns>True if the execution was successful else false.</returns>
        bool OnReadComplete(bool processResult, String[] errors, long recordsRead);

        /// <summary>
        /// After a write operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        bool OnWriteEnd();

        /// <summary>
        /// After a read operation execute this method.
        /// </summary>
        /// <returns>True if the execution was successful else false.</returns>
        bool OnReadEnd();

        /// <summary>
        /// Gets the list of write errors if any.
        /// </summary>
        /// <returns>The list of errors; else empty collection.</returns>
        String[] GetWriteErrors();

        /// <summary>
        /// Gets the list of read errors if any.
        /// </summary>
        /// <returns>The list of errors; else empty collection.</returns>
        String[] GetReadErrors();

        /// <summary>
        /// Get the number of records read.
        /// </summary>
        /// <returns>The number of records read.</returns>
        long GetRecordsRead();

        /// <summary>
        /// Get the number of records written.
        /// </summary>
        /// <returns>The number of records written.</returns>
        long GetRecordsWritten();

        /// <summary>
        /// Gets all the irs documents that failed to write.
        /// </summary>
        /// <returns>The list of failures; else empty collection.</returns>
        Int64[] GetAllWriteFailedIrsDocuments();

        /// <summary>
        /// Gets the final conversion path after processing has completed.
        /// </summary>
        /// <returns>The final conversion path.</returns>
        string GetFinalConversionPath();

        /// <summary>
        /// Gets the returned data from the conversion write process
        /// </summary>
        /// <returns>The list of returned data.</returns>
        List<ExportBatchModel> GetWriteData();

        /// <summary>
        /// The final conversion path extension if any
        /// </summary>
        /// <returns>The final conversion extension name.</returns>
        string GetFinalConversionPathExtension();
    }

    /// <summary>
    /// Convert action execution type.
    /// </summary>
    public class ConvertAction
    {
        /// <summary>
        /// Execute the convert action.
        /// </summary>
        /// <param name="action">The convert action to perform.</param>
        /// <param name="instance">The instance of the object implementer.</param>
        public static void Execute(Action<IConvert> action, IConvert instance)
        {
            action(instance);
        }
    }
}
