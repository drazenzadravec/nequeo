/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Wave
{
    /// <summary>
    /// GSM 610
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal class Gsm610WaveFormat : WaveFormatProvider
    {
        private short samplesPerBlock;

        /// <summary>
        /// Creates a GSM 610 WaveFormat
        /// For now hardcoded to 13kbps
        /// </summary>
        public Gsm610WaveFormat()
        {
            this.waveFormatTag = WaveFormatEncoding.Gsm610;
            this.channels = 1;
            this.averageBytesPerSecond = 1625;
            this.bitsPerSample = 0; // must be zero
            this.blockAlign = 65;
            this.sampleRate = 8000;

            this.extraSize = 2;
            this.samplesPerBlock = 320;
        }

        /// <summary>
        /// Samples per block
        /// </summary>
        public short SamplesPerBlock { get { return this.samplesPerBlock; } }

        /// <summary>
        /// Writes this structure to a BinaryWriter
        /// </summary>
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
        }
    }
}
