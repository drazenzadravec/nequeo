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
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Net.Http2.Protocol;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Generic protocol error exception.
    /// </summary>
    internal class ProtocolError : Exception
    {
        /// <summary>
        /// Generic protocol error exception.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The message.</param>
        public ProtocolError(ErrorCodeRegistry code, string message)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public ErrorCodeRegistry Code { get; set; }
    }

    /// <summary>
    /// Max concurrent streams limit exception.
    /// </summary>
    internal class MaxConcurrentStreamsLimitException : Exception
    {
        /// <summary>
        /// Max concurrent streams limit exception.
        /// </summary>
        public MaxConcurrentStreamsLimitException()
            : base("Endpoint is trying to create more streams then allowed!")
        { }
    }

    /// <summary>
    /// Stream not found exception.
    /// </summary>
    internal class StreamNotFoundException : Exception
    {
        /// <summary>
        /// Stream not found exception.
        /// </summary>
        /// <param name="streamId">The stream id.</param>
        public StreamNotFoundException(int streamId)
            : base(String.Format("Stream was not found by provided id: {0}", streamId))
        {
            StreamId = streamId;
        }

        /// <summary>
        /// Gets the stream id.
        /// </summary>
        public int StreamId { get; private set; }

    }
}
