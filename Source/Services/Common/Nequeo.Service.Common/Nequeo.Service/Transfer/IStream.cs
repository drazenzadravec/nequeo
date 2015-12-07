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
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web.Hosting;
using System.Web;

namespace Nequeo.Service.Transfer
{
    /// <summary>
    /// Simple streaming interface
    /// </summary>
    [ServiceContract]
    public interface IStream
    {
        /// <summary>
        /// Upload a structured format file.
        /// </summary>
        /// <param name="stream">The uploaded file stream.</param>
        /// <returns>True if uploaded; else false.</returns>
        [OperationContract]
        bool UploadStructuredFile(System.IO.Stream stream);

        /// <summary>
        /// Upload file.
        /// </summary>
        /// <param name="stream">The uploaded file stream.</param>
        /// <returns>True if uploaded; else false.</returns>
        [OperationContract]
        bool UploadFile(System.IO.Stream stream);

        /// <summary>
        /// Download file.
        /// </summary>
        /// <param name="fileName">The file name to download.</param>
        /// <returns>The download file stream.</returns>
        [OperationContract]
        System.IO.Stream DownloadFile(string fileName);

        /// <summary>
        /// Gets the file size.
        /// </summary>
        /// <param name="fileName">The name if the file</param>
        /// <returns>The size in bytes of the file.</returns>
        [OperationContract]
        long GetFileSize(string fileName);

    }
}
