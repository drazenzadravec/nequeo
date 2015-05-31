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
using System.IO;
using System.Drawing;

namespace Nequeo.Media
{
    /// <summary>
    /// Audio container structure.
    /// </summary>
    [Serializable]
    public struct AudioModel
    {
        /// <summary>
        /// The audio header.
        /// </summary>
        public AudioHeader Header;

        /// <summary>
        /// The number of sound models.
        /// </summary>
        public int SoundCount;

        /// <summary>
        /// The sound collection.
        /// </summary>
        public SoundModel[] Audio;

    }

    /// <summary>
    /// Audio header container structure.
    /// </summary>
    [Serializable]
    public struct AudioHeader
    {
        /// <summary>
        /// True if audio data is included; else false.
        /// </summary>
        public bool ContainsAudio;

        /// <summary>
        /// The number of channels in the waveform-audio data (1 mono; 2 stereo).
        /// </summary>
        public short Channels;

        /// <summary>
        /// The number of audio samples taken per second.
        /// </summary>
        /// <remarks>
        /// Common sampling rates are 8.0 kHz, 11.025 kHz, 22.05 kHz, and  44.1 kHz.
        /// </remarks>
        public int SamplingRate;

        /// <summary>
        /// The number of bits recorded per sample. 
        /// </summary>
        /// <remarks>
        /// Common sample sizes are 8 bit and 16 bit.
        /// </remarks>
        public short SampleSize;

        /// <summary>
        /// The sound collection type.
        /// </summary>
        public SoundCaptureType SoundType;

        /// <summary>
        /// The Compression algorithm.
        /// </summary>
        public Nequeo.IO.Compression.CompressionAlgorithmStreaming CompressionAlgorithm;

        /// <summary>
        /// The audio time length.
        /// </summary>
        public double Duration;

    }

    /// <summary>
    /// Sound container structure.
    /// </summary>
    [Serializable]
    public struct SoundModel
    {
        /// <summary>
        /// The index of the video frame at which the sound is going to start.
        /// </summary>
        public int StartAtFrameIndex;

        /// <summary>
        /// The size of the data collection.
        /// </summary>
        public int Size;

        /// <summary>
        /// The collection containing the sound data.
        /// </summary>
        public byte[] Data;

    }

    /// <summary>
    /// Audio details container structure.
    /// </summary>
    [Serializable]
    public struct AudioDetails
    {
        /// <summary>
        /// True if default audio configuration is used.
        /// </summary>
        public bool UseDefault;

        /// <summary>
        /// The number of channels in the waveform-audio data (1 mono; 2 stereo).
        /// </summary>
        public short Channels;

        /// <summary>
        /// The number of audio samples taken per second.
        /// </summary>
        /// <remarks>
        /// Common sampling rates are 8.0 kHz, 11.025 kHz, 22.05 kHz, and  44.1 kHz.
        /// </remarks>
        public int SamplingRate;

        /// <summary>
        /// The number of bits recorded per sample. 
        /// </summary>
        /// <remarks>
        /// Common sample sizes are 8 bit and 16 bit.
        /// </remarks>
        public short SampleSize;

    }
}
