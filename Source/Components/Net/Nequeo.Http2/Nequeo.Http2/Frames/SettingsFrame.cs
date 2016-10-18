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
    +-------------------------------+
    |       Identifier (16)         |
    +-------------------------------+-------------------------------+
    |                        Value (32)                             |
    +---------------------------------------------------------------+
    */


    /// <summary>
    /// SETTINGS frame class
    /// see 12 -> 6.5
    /// </summary>
    internal class SettingsFrame : Frame
    {
        /// <summary>
        /// SETTINGS frame class
        /// </summary>
        /// <param name="preamble">The incoming frame.</param>
        public SettingsFrame(Frame preamble)
            : base(preamble)
        {
        }

        /// <summary>
        /// SETTINGS frame class
        /// </summary>
        /// <param name="settings">The settings list.</param>
        /// <param name="isAck">Is acknowledged.</param>
        public SettingsFrame(IList<SettingsPair> settings, bool isAck)
            : base(new byte[Constants.FramePreambleSize + (settings.Count * SettingsPair.PairSize)])
        {
            FrameType = OpCodeFrame.Settings;
            PayloadLength = settings.Count * SettingsPair.PairSize;
            StreamId = 0;
            IsAck = isAck;

            for (int i = 0; i < settings.Count; i++)
            {
                ArraySegment<byte> segment = settings[i].BufferSegment;
                System.Buffer.BlockCopy(segment.Array, segment.Offset, Buffer,
                    Constants.FramePreambleSize + i * SettingsPair.PairSize, SettingsPair.PairSize);
            }
        }

        /// <summary>
        /// Gets or sets is acknowledged.
        /// </summary>
        public bool IsAck
        {
            get
            {
                return (Flags & FrameFlags.Ack) == FrameFlags.Ack;
            }
            set
            {
                if (value)
                {
                    Flags |= FrameFlags.Ack;
                }
            }
        }

        /// <summary>
        /// Gets the entry count.
        /// </summary>
        public int EntryCount
        {
            get { return (Buffer.Length - Constants.FramePreambleSize) / SettingsPair.PairSize; }
        }

        /// <summary>
        /// Settings indexer.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The setting.</returns>
        public SettingsPair this[int index]
        {
            get
            {
                Contract.Assert(index < EntryCount);
                return new SettingsPair(new ArraySegment<byte>(Buffer,
                    Constants.FramePreambleSize + index * SettingsPair.PairSize, SettingsPair.PairSize));
            }
        }
    }
}
