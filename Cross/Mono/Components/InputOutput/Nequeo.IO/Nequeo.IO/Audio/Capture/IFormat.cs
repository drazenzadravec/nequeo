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

namespace Nequeo.IO.Audio.Capture
{
    /// <summary>
    /// Global audio capture format.
    /// </summary>
    public interface IFormat
    {
        /// <summary>
        /// Samples per second
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        /// Gets the bits per sample.
        /// </summary>
        short BitsPerSample { get; }

        /// <summary>
        /// Gets the channels; 1 - mono, 2 - stereo.
        /// </summary>
        short Channels { get; }

        /// <summary>
        /// Gets the block alignment.
        /// </summary>
        short BlockAlign { get; }

        /// <summary>
        /// Gets the average bytes per second.
        /// </summary>
        int AverageBytesPerSecond { get; }

        /// <summary>
        /// Wave format tag type.
        /// </summary>
        WaveFormatTagType WaveFormatTag { get; }

    }

    /// <summary>
    /// Wave format tag type.
    /// </summary>
    public enum WaveFormatTagType
    {
        /// <summary>
        /// Pulse-code modulation.
        /// </summary>
        Pcm = 1
    }
}
