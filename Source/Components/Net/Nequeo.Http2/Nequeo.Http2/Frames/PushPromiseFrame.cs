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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Collections;

namespace Nequeo.Net.Http2.Protocol
{
    /*
    +---------------+
    |Pad Length? (8)|
    +-+-------------+-----------------------------------------------+
    |R|                  Promised Stream ID (31)                    |
    +-+-----------------------------+-------------------------------+
    |                   Header Block Fragment (*)                 ...
    +---------------------------------------------------------------+
    |                           Padding (*)                       ...
    +---------------------------------------------------------------+
    */


    /// <summary>
    /// PUSH_PROMISE frame class
    /// see 12 -> 6.6
    /// </summary>
    internal class PushPromiseFrame : Frame, IHeadersFrame, IPaddingFrame
    {
        /// <summary>
        /// PUSH_PROMISE frame class
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public PushPromiseFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// PUSH_PROMISE frame class
        /// </summary>
        /// <param name="streamId">Stream id.</param>
        /// <param name="promisedStreamId">Promised stream id.</param>
        /// <param name="hasPadding">Has padding.</param>
        /// <param name="isEndHeaders">Is end of headers.</param>
        /// <param name="headers">The header list.</param>
        public PushPromiseFrame(Int32 streamId, Int32 promisedStreamId, bool hasPadding, bool isEndHeaders,
            HeadersList headers = null)
        {
            Contract.Assert(streamId > 0 && promisedStreamId > 0);

            int preambleLength = Constants.FramePreambleSize + PromisedIdLength;
            if (hasPadding) preambleLength += PadHighLowLength;

            // construct frame without Headers Block and Padding bytes
            Buffer = new byte[preambleLength];

            /* 12 -> 6.6 
            The PUSH_PROMISE frame includes optional padding. Padding fields and
            flags are identical to those defined for DATA frames. */

            if (hasPadding)
            {
                // generate padding
                var padHigh = (byte)1;
                var padLow = (byte)new Random().Next(1, 7);

                
            }

            PayloadLength = Buffer.Length - Constants.FramePreambleSize;
            FrameType = OpCodeFrame.Push_Promise;
            StreamId = streamId;

            PromisedStreamId = promisedStreamId;
            IsEndHeaders = isEndHeaders;

            if (headers != null) Headers = headers;
        }

        // 1 byte Pad High, 1 byte Pad Low field
        private const int PadHighLowLength = 2;

        // 4 bytes Promised Stream Id field
        private const int PromisedIdLength = 4;

        private HeadersList _headers = new HeadersList();

        /// <summary>
        /// Gets or sets is end of headers.
        /// </summary>
        public bool IsEndHeaders
        {
            get
            {
                return (Flags & FrameFlags.EndHeaders) == FrameFlags.EndHeaders;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.EndHeaders;
                }
            }
        }

        /// <summary>
        /// Gets or sets has padding.
        /// </summary>
        public bool HasPadding
        {
            get
            {
                return (Flags & FrameFlags.Padded) == FrameFlags.Padded;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.Padded;
                }
            }
        }

        /// <summary>
        /// Gets or sets promised stream id.
        /// </summary>
        public Int32 PromisedStreamId
        {
            get
            {
                if (HasPadding)
                {
                    return FrameHelper.Get31BitsAt(Buffer, Constants.FramePreambleSize + PadHighLowLength);
                }
                return FrameHelper.Get31BitsAt(Buffer, Constants.FramePreambleSize);
            }
            set
            {
                Contract.Assert(value >= 0 && value <= 255);

                if (HasPadding)
                {
                    FrameHelper.Set31BitsAt(Buffer, Constants.FramePreambleSize + PadHighLowLength, value);
                    return;
                }
                FrameHelper.Set31BitsAt(Buffer, Constants.FramePreambleSize, value);
            }
        }

        /// <summary>
        /// Gets compressed headers.
        /// </summary>
        public ArraySegment<byte> CompressedHeaders
        {
            get
            {
                int padLength = Buffer[Constants.FramePreambleSize];
                int offset = Constants.FramePreambleSize + PromisedIdLength;

                if (HasPadding) offset += PadHighLowLength;

                int count = Buffer.Length - offset - padLength;

                return new ArraySegment<byte>(Buffer, offset, count);
            }
        }

        /// <summary>
        /// Gets or sets the headers list.
        /// </summary>
        public HeadersList Headers
        {
            get
            {
                return _headers;
            }
            set
            {
                if (value == null)
                    return;

                _headers = value;
            }
        }
    }
}
