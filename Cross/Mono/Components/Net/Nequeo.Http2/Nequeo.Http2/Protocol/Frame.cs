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
    +-----------------------------------------------+
    |                 Length (24)                   |
    +---------------+---------------+---------------+
    |   Type (8)    |   Flags (8)   |
    +-+-------------+---------------+-------------------------------+
    |R|                 Stream Identifier (31)                      |
    +=+=============================================================+
    |                   Frame Payload (0...)                      ...
    +---------------------------------------------------------------+
    */

    /// <summary>
    /// Represents the initial frame fields on every frame.
    /// </summary>
    internal class Frame
    {
        /// <summary>
        /// Represents the initial frame fields on every frame.
        /// </summary>
        public Frame()
            : this(new byte[Constants.FramePreambleSize])
        {
        }

        /// <summary>
        /// Represents the initial frame fields on every frame.
        /// </summary>
        /// <param name="preamble">The incoming preamble frame.</param>
        protected Frame(Frame preamble)
            : this(new byte[Constants.FramePreambleSize + preamble.PayloadLength])
        {
            System.Buffer.BlockCopy(preamble.Buffer, 0, Buffer, 0, Constants.FramePreambleSize);
        }

        /// <summary>
        /// Represents the initial frame fields on every frame.
        /// </summary>
        /// <param name="buffer">The outgoing buffer.</param>
        protected Frame(byte[] buffer)
        {
            _buffer = buffer;
        }

        protected byte[] _buffer = null;

        /// <summary>
        /// Gets or sets the frame buffer.
        /// </summary>
        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        public ArraySegment<byte> Payload
        {
            get
            {
                if (_buffer != null && _buffer.Length > 0)
                {
                    return new ArraySegment<byte>(_buffer, Constants.FramePreambleSize, _buffer.Length - Constants.FramePreambleSize);
                }
                return new ArraySegment<byte>();
            }
        }

        /// <summary>
        /// Gets an indicator specifying if the frame is a data frame.
        /// </summary>
        public bool IsControl
        {
            get { return FrameType != OpCodeFrame.Data; }
        }

        /// <summary>
        /// Gets or sets the payload length.
        /// </summary>
        public int PayloadLength
        {
            get { return FrameHelper.Get24BitsAt(Buffer, 0); }
            set { FrameHelper.Set24BitsAt(Buffer, 0, value); }
        }

        /// <summary>
        /// Gets or sets the current frame type.
        /// </summary>
        public OpCodeFrame FrameType
        {
            get { return (OpCodeFrame)Buffer[3]; }
            set { Buffer[3] = (byte)value; }
        }

        /// <summary>
        /// Gets or sets the current frame flag.
        /// </summary>
        public FrameFlags Flags
        {
            get { return (FrameFlags)Buffer[4]; }
            set { Buffer[4] = (byte)value; }
        }

        /// <summary>
        /// Gets or sets the stream id.
        /// </summary>
        public Int32 StreamId
        {
            get { return FrameHelper.Get31BitsAt(Buffer, 5); }
            set { FrameHelper.Set31BitsAt(Buffer, 5, value); }
        }
    }
}
