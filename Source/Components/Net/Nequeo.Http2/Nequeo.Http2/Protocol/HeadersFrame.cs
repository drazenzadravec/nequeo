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
    +---------------+
    |Pad Length? (8)|
    +-+-------------+-----------------------------------------------+
    |E|                 Stream Dependency? (31)                     |
    +-+-------------+-----------------------------------------------+
    |  Weight? (8)  |
    +-+-------------+-----------------------------------------------+
    |                   Header Block Fragment (*)                 ...
    +---------------------------------------------------------------+
    |                           Padding (*)                       ...
    +---------------------------------------------------------------+
    */


    /// <summary>
    /// HEADERS frame class
    /// see 12 -> 6.2
    /// </summary>
    internal class HeadersFrame : Frame, IEndStreamFrame, IHeadersFrame, IPaddingFrame
    {
        /// <summary>
        /// HEADERS frame class
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public HeadersFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// HEADERS frame class
        /// </summary>
        /// <param name="streamId">The stream id.</param>
        /// <param name="hasPadding">Has padding.</param>
        /// <param name="streamDependency">Stream dependency.</param>
        /// <param name="weight">Weight.</param>
        /// <param name="exclusive">Exclusive.</param>
        public HeadersFrame(int streamId, bool hasPadding, int streamDependency = -1, byte weight = 0, bool exclusive = false)
        {
            /* 12 -> 5.3 
            A client can assign a priority for a new stream by including
            prioritization information in the HEADERS frame */
            bool hasPriority = (streamDependency != -1 && weight != 0);

            int preambleLength = Constants.FramePreambleSize;
            if (hasPadding) preambleLength += PadHighLowLength;
            if (hasPriority) preambleLength += DependencyLength + WeightLength;

            // construct frame without Headers Block and Padding bytes
            Buffer = new byte[preambleLength];

            /* 12 -> 6.2 
            The HEADERS frame includes optional padding.  Padding fields and
            flags are identical to those defined for DATA frames. */

            if (hasPadding)
            {
                // generate padding
                var padHigh = (byte)1;
                var padLow = (byte)new Random().Next(1, 7);

                
            }

            if (hasPriority)
            {
                HasPriority = true;
                Exclusive = exclusive;
                StreamDependency = streamDependency;
                Weight = weight;
            }

            PayloadLength = Buffer.Length - Constants.FramePreambleSize;
            FrameType = OpCodeFrame.Headers;
            StreamId = streamId;
        }

        // 1 byte Pad High, 1 byte Pad Low field
        private const int PadHighLowLength = 2;

        // 4 bytes Stream Dependency field
        private const int DependencyLength = 4;

        // 1 byte Weight field
        private const int WeightLength = 1;

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
        /// Gets or sets is end headers.
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
        /// Gets or sets has priority.
        /// </summary>
        public bool HasPriority
        {
            get
            {
                return (Flags & FrameFlags.Priority) == FrameFlags.Priority;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.Priority;
                }
            }
        }

        /// <summary>
        /// Gets or sets is exclusive.
        /// </summary>
        public bool Exclusive
        {
            get
            {
                if (HasPriority)
                {
                    if (HasPadding)
                    {
                        return FrameHelper.GetBit(Buffer[Constants.FramePreambleSize + PadHighLowLength], 7);
                    }
                    return FrameHelper.GetBit(Buffer[Constants.FramePreambleSize], 7);
                }
                return false;
            }
            set
            {
                if (HasPadding)
                {
                    FrameHelper.SetBit(ref Buffer[Constants.FramePreambleSize + PadHighLowLength], value, 7);
                    return;
                }
                FrameHelper.SetBit(ref Buffer[Constants.FramePreambleSize], value, 7);
            }
        }

        /// <summary>
        /// Gets or sets stream dependency.
        /// </summary>
        public int StreamDependency
        {
            get
            {
                if (HasPriority)
                {
                    if (HasPadding)
                    {
                        return FrameHelper.Get31BitsAt(Buffer, Constants.FramePreambleSize + PadHighLowLength);
                    }
                    return FrameHelper.Get31BitsAt(Buffer, Constants.FramePreambleSize);
                }
                return 0;
            }
            set
            {
                if (HasPadding)
                {
                    FrameHelper.Set31BitsAt(Buffer, Constants.FramePreambleSize + PadHighLowLength, value);
                    return;
                }
                FrameHelper.Set31BitsAt(Buffer, Constants.FramePreambleSize, value);
            }
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        public byte Weight
        {
            get
            {
                if (HasPriority)
                {
                    if (HasPadding)
                    {
                        return Buffer[Constants.FramePreambleSize + PadHighLowLength + DependencyLength];
                    }
                    return Buffer[Constants.FramePreambleSize + DependencyLength];
                }
                return 0;
            }
            set
            {
                if (HasPadding)
                {
                    Buffer[Constants.FramePreambleSize + PadHighLowLength + DependencyLength] = value;
                    return;
                }
                Buffer[Constants.FramePreambleSize + DependencyLength] = value;
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
                if (HasPriority) offset += DependencyLength + WeightLength;

                int count = Buffer.Length - offset - padLength;

                return new ArraySegment<byte>(Buffer, offset, count);
            }
        }
    }
}
