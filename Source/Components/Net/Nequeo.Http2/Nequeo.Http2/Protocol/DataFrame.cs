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

namespace Nequeo.Net.Http2.Protocol
{
    /*
    +---------------+
    |Pad Length? (8)|
    +---------------+-----------------------------------------------+
    |                            Data (*)                         ...
    +---------------------------------------------------------------+
    |                           Padding (*)                       ...
    +---------------------------------------------------------------+
    */

    /// <summary>
    /// DATA frame class
    /// see 12 -> 6.1.
    /// </summary>
    internal class DataFrame : Frame, IEndStreamFrame, IPaddingFrame
    {
        /// <summary>
        /// DATA frame class
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public DataFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// DATA frame class
        /// </summary>
        /// <param name="streamId">The stream id.</param>
        /// <param name="data">The data.</param>
        /// <param name="isEndStream">Is end of stream.</param>
        /// <param name="hasPadding">Has padding.</param>
        public DataFrame(int streamId, ArraySegment<byte> data, bool isEndStream, bool hasPadding = false)
        {
            Contract.Assert(data.Array != null);

            /* 12 -> 6.1
            DATA frames MAY also contain arbitrary padding.  Padding can be added
            to DATA frames to hide the size of messages. The total number of padding
            octets is determined by multiplying the value of the Pad High field by 256 
            and adding the value of the Pad Low field. */

            if (hasPadding)
            {
                // generate padding
                var padHigh = (byte)1;
                var padLow = (byte)new Random().Next(1, 7);
                int padLength = padHigh * 256 + padLow;

                // construct frame with padding
                Buffer = new byte[Constants.FramePreambleSize + PadHighLowLength + data.Count + padLength];
                PayloadLength = PadHighLowLength + data.Count + padLength;

                // Write the data.
                System.Buffer.BlockCopy(data.Array, data.Offset, Buffer, Constants.FramePreambleSize + PadHighLowLength, data.Count);
            }
            else
            {
                // construct frame without padding
                Buffer = new byte[Constants.FramePreambleSize + data.Count];
                PayloadLength = data.Count;

                // Write the data.
                System.Buffer.BlockCopy(data.Array, data.Offset, Buffer, Constants.FramePreambleSize, data.Count);
            }

            IsEndStream = isEndStream;
            HasPadding = hasPadding;
            FrameType = OpCodeFrame.Data;
            StreamId = streamId;
        }

        // 1 byte Pad High and Pad Low.
        private const int PadHighLowLength = 1;

        /// <summary>
        /// Gets or sets is end of stream.
        /// </summary>
        public bool IsEndStream
        {
            get
            {
                return (Flags & FrameFlags.EndStream) == FrameFlags.EndStream;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.EndStream;
                }
            }
        }

        /// <summary>
        /// Gets has padding.
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
        /// Gets or sets is compressed.
        /// </summary>
        public bool IsCompressed
        {
            get
            {
                return (Flags & FrameFlags.Compressed) == FrameFlags.Compressed;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.Compressed;
                }
            }
        }

        /// <summary>
        /// Gets the data segment.
        /// </summary>
        public ArraySegment<byte> Data
        {
            get
            {
                if (HasPadding)
                {
                    // Padding length is the 1 byte after the preamble.
                    int padLength = Buffer[Constants.FramePreambleSize];

                    return new ArraySegment<byte>(Buffer, Constants.FramePreambleSize + PadHighLowLength,
                        Buffer.Length - Constants.FramePreambleSize - PadHighLowLength - padLength);
                }
                return new ArraySegment<byte>(Buffer, Constants.FramePreambleSize,
                        Buffer.Length - Constants.FramePreambleSize);
            }
        }
    }
}
