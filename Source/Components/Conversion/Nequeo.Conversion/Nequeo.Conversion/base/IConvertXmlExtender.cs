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
using System.Xml;
using System.Xml.Linq;

using Nequeo.Model.Conversion;

namespace Nequeo.Conversion
{
    /// <summary>
    /// Conversion extender interface
    /// </summary>
    public interface IConvertXmlExtender
    {
        /// <summary>
        /// Write the data to the file.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="fileName">The file name and path where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        bool Write(List<TransformModel[]> data, string fileName);

        /// <summary>
        /// Write the data to the file stream.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="fileStream">The file stream where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        /// <exception cref="System.ArgumentNullException">Array is null</exception>
        /// <exception cref="System.ArgumentNullException">Stream is null</exception>
        /// <exception cref="System.IndexOutOfRangeException">Array contains no data.</exception>
        bool Write(List<TransformModel[]> data, FileStream fileStream);

        /// <summary>
        /// Write the data to the file.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="fileName">The file name and path where the data is to be written.</param>
        /// <returns>True is all data has been written else false.</returns>
        bool Write(TransformXmlModel data, string fileName);

        /// <summary>
        /// Write the data to the file.
        /// </summary>
        /// <param name="node">The xml node to create.</param>
        /// <param name="data">The data specific to the current node.</param>
        /// <param name="fileName">The file name to write to.</param>
        /// <param name="version">The xml version.</param>
        /// <param name="encoding">The xml encoding</param>
        /// <param name="standalone">The xml standalone</param>
        /// <returns>True is all data has been written else false.</returns>
        bool Write(XElement node, TransformModel data, string fileName, string version, string encoding, string standalone);

    }
}
