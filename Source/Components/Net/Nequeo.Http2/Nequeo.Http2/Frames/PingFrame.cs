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
    |                                                               |
    |                      Opaque Data (64)                         |
    |                                                               |
    +---------------------------------------------------------------+
    */


    /// <summary>
    /// PING frame class.
    /// see 12 -> 6.7.
    /// </summary>
    internal class PingFrame : Frame
    {
        /// <summary>
        /// PING frame class.
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public PingFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// PING frame class.
        /// </summary>
        /// <param name="isAck">Is acknowledged.</param>
        /// <param name="payload">The payload.</param>
        public PingFrame(bool isAck, byte[] payload = null)
            : base(new byte[FrameSize])
        {
            FrameType = OpCodeFrame.Ping;
            PayloadLength = FrameSize - Constants.FramePreambleSize; // 4
            StreamId = 0;

            if (payload != null)
            {
                System.Buffer.BlockCopy(Buffer, Constants.FramePreambleSize, Buffer,
                    Constants.FramePreambleSize, FrameSize - Constants.FramePreambleSize);
            }
        }

        /// <summary>
        /// Ping frame expected payload length
        /// </summary>
        public const int DefPayloadLength = 8;

        /// <summary>
        /// The number of bytes in the frame.
        /// </summary>
        public const int FrameSize = DefPayloadLength + Constants.FramePreambleSize;
    }
}
