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

using Nequeo.Collections;

namespace Nequeo.Net.Http2.Protocol
{
    /*
    +---------------------------------------------------------------+
    |                   Header Block Fragment (*)                 ...
    +---------------------------------------------------------------+
    */


    /// <summary>
    /// CONTINUATION frame class
    /// see 12 -> 6.10
    /// </summary>
    internal class ContinuationFrame : Frame, IHeadersFrame, IEndStreamFrame, IPaddingFrame
    {
        /// <summary>
        /// CONTINUATION frame class
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public ContinuationFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// CONTINUATION frame class
        /// </summary>
        /// <param name="streamId">The stream id.</param>
        /// <param name="headers">Headers.</param>
        /// <param name="isEndHeaders">Is end of headers.</param>
        /// <param name="hasPadding">Has padding.</param>
        public ContinuationFrame(int streamId, byte[] headers, bool isEndHeaders, bool hasPadding = false)
        {
            /* 12 -> 6.10
            The CONTINUATION frame includes optional padding.  Padding fields and
            flags are identical to those defined for DATA frames. */
            if (hasPadding)
            {
                // generate padding
                var padHigh = (byte)1;
                var padLow = (byte)new Random().Next(1, 7);
                int padLength = padHigh * 256 + padLow;

                // construct frame with padding
                Buffer = new byte[Constants.FramePreambleSize + PadHighLowLength + headers.Length + padLength];
                PayloadLength = PadHighLowLength + headers.Length + padLength;

                System.Buffer.BlockCopy(headers, 0, Buffer, Constants.FramePreambleSize + PadHighLowLength, headers.Length);
            }
            else
            {
                // construct frame without padding
                Buffer = new byte[Constants.FramePreambleSize + headers.Length];
                PayloadLength = headers.Length;

                System.Buffer.BlockCopy(headers, 0, Buffer, Constants.FramePreambleSize, headers.Length);
            }

            StreamId = streamId;
            FrameType = OpCodeFrame.Continuation;
            IsEndHeaders = isEndHeaders;
        }

        // 1 byte Pad High, 1 byte Pad Low field
        private const int PadHighLowLength = 2;

        /// <summary>
        /// Gets or sets is end stream.
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
        /// Gets or sets is end of header indicator.
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
        /// Gets the compressed headers.
        /// </summary>
        public ArraySegment<byte> CompressedHeaders
        {
            get
            {
                int padLength = Buffer[Constants.FramePreambleSize];
                int offset = Constants.FramePreambleSize;

                if (HasPadding) offset += PadHighLowLength;

                int count = Buffer.Length - offset - padLength;

                return new ArraySegment<byte>(Buffer, offset, count);
            }
        }
    }
}
