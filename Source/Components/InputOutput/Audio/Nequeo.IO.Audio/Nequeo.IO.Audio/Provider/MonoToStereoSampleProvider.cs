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
    /// No nonsense mono to stereo provider, no volume adjustment,
    /// just copies input to left and right. 
    /// </summary>
    internal class MonoToStereoSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly WaveFormatProvider waveFormat;
        private float[] sourceBuffer;

        /// <summary>
        /// Initializes a new instance of MonoToStereoSampleProvider
        /// </summary>
        /// <param name="source">Source sample provider</param>
        public MonoToStereoSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 1)
            {
                throw new ArgumentException("Source must be mono");
            }
            this.source = source;
            this.waveFormat = WaveFormatProvider.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
        }

        /// <summary>
        /// WaveFormat of this provider
        /// </summary>
        public WaveFormatProvider WaveFormat
        {
            get { return this.waveFormat; }
        }

        /// <summary>
        /// Reads samples from this provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples required</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            int sourceSamplesRequired = count / 2;
            int outIndex = offset;
            EnsureSourceBuffer(sourceSamplesRequired);
            int sourceSamplesRead = source.Read(sourceBuffer, 0, sourceSamplesRequired);
            for (int n = 0; n < sourceSamplesRead; n++)
            {
                buffer[outIndex++] = sourceBuffer[n];
                buffer[outIndex++] = sourceBuffer[n];
            }
            return sourceSamplesRead * 2;
        }

        private void EnsureSourceBuffer(int count)
        {
            if (this.sourceBuffer == null || this.sourceBuffer.Length < count)
            {
                this.sourceBuffer = new float[count];
            }
        }
    }
}
