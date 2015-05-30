﻿/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
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
    +-+-------------------------------------------------------------+
    |R|                  Last-Stream-ID (31)                        |
    +-+-------------------------------------------------------------+
    |                      Error Code (32)                          |
    +---------------------------------------------------------------+
    |                  Additional Debug Data (*)                    |
    +---------------------------------------------------------------+
    */


    /// <summary>
    /// This class defines GoAway frame.
    /// See spec: http://tools.ietf.org/html/draft-ietf-httpbis-http2-04#section-6.8
    /// </summary>
    internal class GoAwayFrame : Frame
    {
        /// <summary>
        /// This class defines GoAway frame.
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public GoAwayFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// This class defines GoAway frame.
        /// </summary>
        /// <param name="lastStreamId">The last stream id.</param>
        /// <param name="statusCode">The rest status code.</param>
        public GoAwayFrame(int lastStreamId, ErrorCodeRegistry statusCode)
            : base(new byte[InitialFrameSize])
        {
            FrameType = OpCodeFrame.Go_Away;
            PayloadLength = InitialFrameSize - Constants.FramePreambleSize; // 16 bytes
            LastGoodStreamId = lastStreamId;
            StatusCode = statusCode;
        }

        // The number of bytes in the frame.
        private const int InitialFrameSize = 24;

        /// <summary>
        /// Gets or sets the last good stream id.
        /// </summary>
        public int LastGoodStreamId
        {
            get { return FrameHelper.Get31BitsAt(Buffer, 8); }
            set { FrameHelper.Set31BitsAt(Buffer, 8, value); }
        }

        /// <summary>
        /// Gets or sets the error status code.
        /// </summary>
        public ErrorCodeRegistry StatusCode
        {
            get { return (ErrorCodeRegistry)FrameHelper.Get32BitsAt(Buffer, 12); }
            set { FrameHelper.Set32BitsAt(Buffer, 12, (int)value); }
        }
    }
}
