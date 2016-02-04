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

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// This structure describe detail information about an audio media.
    /// </summary>
    public class MediaFormatAudio : MediaFormat
    {
        /// <summary>
        /// This structure describe detail information about an audio media.
        /// </summary>
        public MediaFormatAudio() { }

        private uint _clockRate;
        private uint _channelCount;
        private uint _frameTimeUsec;
        private uint _bitsPerSample;
        private uint _avgBps;
        private uint _maxBps;

        /// <summary>
        /// Gets or sets the audio clock rate in samples or Hz.
        /// </summary>
        public uint ClockRate
        {
            get { return _clockRate; }
            set { _clockRate = value; }
        }

        /// <summary>
        /// Gets or sets the number of channels.
        /// </summary>
        public uint ChannelCount
        {
            get { return _channelCount; }
            set { _channelCount = value; }
        }

        /// <summary>
        /// Gets or sets the frame interval, in microseconds.
        /// </summary>
        public uint FrameTimeUsec
        {
            get { return _frameTimeUsec; }
            set { _frameTimeUsec = value; }
        }

        /// <summary>
        /// Gets or sets the number of bits per sample.
        /// </summary>
        public uint BitsPerSample
        {
            get { return _bitsPerSample; }
            set { _bitsPerSample = value; }
        }

        /// <summary>
        /// Gets or sets the average bitrate.
        /// </summary>
        public uint AvgBps
        {
            get { return _avgBps; }
            set { _avgBps = value; }
        }

        /// <summary>
        /// Gets or sets the maximum bitrate.
        /// </summary>
        public uint MaxBps
        {
            get { return _maxBps; }
            set { _maxBps = value; }
        }
    }
}
