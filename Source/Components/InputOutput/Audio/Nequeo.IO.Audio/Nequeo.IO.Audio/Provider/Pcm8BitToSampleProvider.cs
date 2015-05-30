/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.IO.Audio.Wave;

namespace Nequeo.IO.Audio.Provider
{
    /// <summary>
    /// Converts an IWaveProvider containing 8 bit PCM to an
    /// ISampleProvider
    /// </summary>
    internal class Pcm8BitToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initialises a new instance of Pcm8BitToSampleProvider
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public Pcm8BitToSampleProvider(IWaveProvider source) :
            base(source)
        {
        }

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples to read</param>
        /// <returns>Number of samples read</returns>
        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n++)
            {
                buffer[outIndex++] = sourceBuffer[n] / 128f - 1.0f;
            }
            return bytesRead;
        }
    }
}
