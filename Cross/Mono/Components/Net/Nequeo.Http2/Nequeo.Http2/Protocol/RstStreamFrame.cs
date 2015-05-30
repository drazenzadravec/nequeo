/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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

namespace Nequeo.Net.Http2.Protocol
{
    /*
    +---------------------------------------------------------------+
    |                        Error Code (32)                        |
    +---------------------------------------------------------------+
    */

    /// <summary>
    /// Reset stream frame class.
    /// See spec: http://tools.ietf.org/html/draft-ietf-httpbis-http2-04#section-6.4
    /// </summary>
    internal class RstStreamFrame : Frame
    {
        /// <summary>
        /// Reset stream frame class.
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public RstStreamFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// Reset stream frame class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="statusCode">The rest status code.</param>
        public RstStreamFrame(int id, ErrorCodeRegistry statusCode)
            : base(new byte[InitialFrameSize])
        {
            StreamId = id;//32 bit
            FrameType = OpCodeFrame.Reset_Stream;//8bit
            PayloadLength = InitialFrameSize - Constants.FramePreambleSize; // 32bit
            StatusCode = statusCode;//32bit
        }

        // The number of bytes in the frame.
        private const int InitialFrameSize = 13;

        /// <summary>
        /// Gets or sets the error status code.
        /// </summary>
        public ErrorCodeRegistry StatusCode
        {
            get { return (ErrorCodeRegistry)FrameHelper.Get32BitsAt(Buffer, 8); }
            set { FrameHelper.Set32BitsAt(Buffer, 8, (int)value); }
        }
    }
}
