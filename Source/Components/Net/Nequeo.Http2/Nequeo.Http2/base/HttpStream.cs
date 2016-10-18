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
using System.Threading.Tasks;

using Nequeo.Net.Http2.Protocol;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Http stream provider.
    /// </summary>
    public class HttpStream
    {
        /// <summary>
        /// Http stream provider.
        /// </summary>
        /// <param name="streamId">The context stream id.</param>
        /// <param name="httpContext">The http context that controls this context stream.</param>
        public HttpStream(int streamId, HttpContext httpContext)
        {
            if (streamId <= 0)
                throw new ArgumentOutOfRangeException("Invalid id for stream, must be greater then zero.");

            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            _httpContext = httpContext;
            _streamId = streamId;

            HttpRequest = new HttpRequest();
            HttpResponse = new HttpResponse();

            // Triggered when written to the response output stream.
            HttpResponse.WriteComplete = (count) => ResponseWriteComplete(count);
        }

        private readonly int _streamId;
        private OpCodeFrame _frameType = OpCodeFrame.Go_Away;
        private HttpContext _httpContext = null;

        /// <summary>
        /// Gets the stream id.
        /// </summary>
        public int StreamId
        {
            get { return _streamId; }
        }

        /// <summary>
        /// Gets the frame type that is in the request queue.
        /// </summary>
        public OpCodeFrame FrameType
        {
            get { return _frameType; }
            internal set { _frameType = value; }
        }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the http context.
        /// </summary>
        internal HttpContext HttpContext
        {
            get { return _httpContext; }
        }

        /// <summary>
        /// Response write complete.
        /// </summary>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        private void ResponseWriteComplete(int count)
        {
            byte[] buffer = new byte[count];

            // Read from the stream and then
            // Send the data to the response stream.
            int bytesRead = HttpResponse.Output.Read(buffer, 0, count);
            _httpContext.ResponseWrite(buffer.Take(bytesRead).ToArray());
        }

        /// <summary>
        /// Gets the http request.
        /// </summary>
        public Nequeo.Net.Http2.HttpRequest HttpRequest
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the http response.
        /// </summary>
        public Nequeo.Net.Http2.HttpResponse HttpResponse
        {
            get;
            internal set;
        }
    }
}
