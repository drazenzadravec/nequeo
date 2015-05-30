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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Extension;
using Nequeo.Model;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Http server request.
    /// </summary>
    public sealed class HttpRequest : Nequeo.Net.WebRequest
    {
        /// <summary>
        /// Parse the form data within the stream, including any uploaded file data.
        /// </summary>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely</param>
        /// <returns>The http form model parsed from the stream; else null.</returns>
        public Nequeo.Net.Http.Common.HttpFormModel ParseForm(long timeout = -1)
        {
            System.IO.BinaryReader reader = null;
            try
            {
                // Create the reader.
                reader = new System.IO.BinaryReader(Input, Encoding.Default, true);

                // Return the result.
                return Nequeo.Net.Http.Utility.ParseForm(reader, timeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }

        /// <summary>
        /// Read the request headers.
        /// </summary>
        /// <param name="headers">The header collection.</param>
        /// <param name="request">The request header.</param>
        public void ReadHttpHeaders(List<NameValue> headers, string request)
        {
            // If headers exist.
            if (headers != null)
            {
                // Set the request headers.
                ReadWebRequestHeaders(headers, request);
            }
        }
    }
}
