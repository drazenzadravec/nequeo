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
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;

namespace Nequeo.Net.Http.Common
{
    /// <summary>
    /// Http form model.
    /// </summary>
    public class HttpFormModel
    {
        /// <summary>
        /// Gets or sets the form name value collection.
        /// </summary>
        public NameValueCollection Form { get; set; }

        /// <summary>
        /// Gets or sets the uploaded files collection.
        /// </summary>
        public List<HttpUploadFileModel> UploadedFiles { get; set; }
    }

    /// <summary>
    /// Http upload file model.
    /// </summary>
    public class HttpUploadFileModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the uploaded file stream.
        /// </summary>
        public System.IO.Stream UploadFile { get; set; }

        /// <summary>
        /// Save the upload file data to the destination stream.
        /// </summary>
        /// <param name="destination">The stream to write the upload file stream to.</param>
        public void Save(System.IO.Stream destination)
        {
            // Copy the data stream.
            Utility.TransferData(UploadFile, destination);
        }
    }
}
