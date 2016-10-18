/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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

using Nequeo.IO.Audio.Capture;

namespace Nequeo.IO.Audio.Directx.Formats
{
    /// <summary>
    /// Pulse-code modulation 
    /// </summary>
    public class Pcm : IFormat
    {
        /// <summary>
        /// Pulse-code modulation 
        /// </summary>
        public Pcm()
        {}

        private int _sampleRate = 44100;
        private short _bitsPerSample = 16;
        private short _channels = 2;

        /// <summary>
        /// Gets the samples per second
        /// </summary>
        public int SampleRate
        {
            get { return _sampleRate; }
        }

        /// <summary>
        /// Gets the bits per sample.
        /// </summary>
        public short BitsPerSample
        {
            get { return _bitsPerSample; }
        }

        /// <summary>
        /// Gets the channels 1 - mono, 2 - stereo.
        /// </summary>
        public short Channels
        {
            get { return _channels; }
        }

        /// <summary>
        /// Gets the block alignment.
        /// </summary>
        public short BlockAlign
        {
            get { return (short)(Channels * (BitsPerSample / 8)); }
        }

        /// <summary>
        /// Gets the average bytes per second
        /// </summary>
        public int AverageBytesPerSecond
        {
            get { return BlockAlign * SampleRate; }
        }

        /// <summary>
        /// Gets the wave format tag type.
        /// </summary>
        public WaveFormatTagType WaveFormatTag
        {
            get { return WaveFormatTagType.Pcm;  }
        }

        /// <summary>
        /// The string that represents the value of this instance.
        /// </summary>
        /// <returns>The string representing the instance.</returns>
        public override string ToString()
        {
            string channelsText;
            switch (_channels)
            {
                case 1:
                    channelsText = "Mono";
                    break;
                case 2:
                    channelsText = "Stereo";
                    break;
                default:
                    channelsText = _channels + " channels";
                    break;
            };

            return string.Format("{0} Hz, {1} bit, {2}", _sampleRate, _bitsPerSample, channelsText);
        }
    }
}
