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
    /// <summary>
    /// Settings pair.
    /// see 12 -> 6.5.1
    /// </summary>
    public struct SettingsPair
    {
        /* The payload of a SETTINGS frame consists of zero or more parameters,
        each consisting of an unsigned 8-bit identifier and an unsigned 32-bit value. */

        
        /// <summary>
        /// Settings pair.
        /// </summary>
        /// <param name="bufferSegment">The buffer segment.</param>
        public SettingsPair(ArraySegment<byte> bufferSegment)
        {
            _bufferSegment = bufferSegment;
        }

        /// <summary>
        /// Settings pair.
        /// </summary>
        /// <param name="id">Settings id.</param>
        /// <param name="value">Setting value.</param>
        public SettingsPair(SettingsRegistry id, int value)
        {
            _bufferSegment = new ArraySegment<byte>(new byte[PairSize], 0, PairSize);
            Id = id;
            Value = value;
        }

        // 1 byte for identifier, 4 bytes for value
        internal const int PairSize = 5;

        private readonly ArraySegment<byte> _bufferSegment;

        /// <summary>
        /// Gets the buffer segment.
        /// </summary>
        public ArraySegment<byte> BufferSegment
        {
            get
            {
                return _bufferSegment;
            }
        }

        /// <summary>
        /// Gets or sets the settings id.
        /// </summary>
        public SettingsRegistry Id
        {
            get
            {
                return (SettingsRegistry)FrameHelper.Get8BitsAt(_bufferSegment.Array, _bufferSegment.Offset);
            }
            set
            {
                FrameHelper.Set8BitsAt(_bufferSegment.Array, _bufferSegment.Offset, (int)value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int Value
        {
            get
            {
                return FrameHelper.Get32BitsAt(_bufferSegment.Array, _bufferSegment.Offset + 1);
            }
            set
            {
                FrameHelper.Set32BitsAt(_bufferSegment.Array, _bufferSegment.Offset + 1, value);
            }
        }
    }
}
