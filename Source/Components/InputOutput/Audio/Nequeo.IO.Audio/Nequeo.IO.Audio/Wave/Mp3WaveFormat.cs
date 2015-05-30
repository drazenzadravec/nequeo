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

using Nequeo.IO.Audio.Provider;
using Nequeo.IO.Audio.Wave;

namespace Nequeo.IO.Audio.Wave
{
    /// <summary>
    /// MP3 WaveFormat, MPEGLAYER3WAVEFORMAT from mmreg.h
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal class Mp3WaveFormat : WaveFormatProvider
    {
        /// <summary>
        /// Wave format ID (wID)
        /// </summary>
        public Mp3WaveFormatId id;
        /// <summary>
        /// Padding flags (fdwFlags)
        /// </summary>
        public Mp3WaveFormatFlags flags;
        /// <summary>
        /// Block Size (nBlockSize)
        /// </summary>
        public ushort blockSize;
        /// <summary>
        /// Frames per block (nFramesPerBlock)
        /// </summary>
        public ushort framesPerBlock;
        /// <summary>
        /// Codec Delay (nCodecDelay)
        /// </summary>
        public ushort codecDelay;

        private const short Mp3WaveFormatExtraBytes = 12; // MPEGLAYER3_WFX_EXTRA_BYTES

        /// <summary>
        /// Creates a new MP3 WaveFormat
        /// </summary>
        public Mp3WaveFormat(int sampleRate, int channels, int blockSize, int bitRate)
        {
            waveFormatTag = WaveFormatEncoding.MpegLayer3;
            this.channels = (short)channels;
            this.averageBytesPerSecond = bitRate / 8;
            this.bitsPerSample = 0; // must be zero
            this.blockAlign = 1; // must be 1
            this.sampleRate = sampleRate;

            this.extraSize = Mp3WaveFormatExtraBytes;
            this.id = Mp3WaveFormatId.Mpeg;
            this.flags = Mp3WaveFormatFlags.PaddingIso;
            this.blockSize = (ushort)blockSize;
            this.framesPerBlock = 1;
            this.codecDelay = 0;
        }
    }

    /// <summary>
    /// Wave Format Padding Flags
    /// </summary>
    [Flags]
    internal enum Mp3WaveFormatFlags
    {
        /// <summary>
        /// MPEGLAYER3_FLAG_PADDING_ISO
        /// </summary>
        PaddingIso = 0,
        /// <summary>
        /// MPEGLAYER3_FLAG_PADDING_ON
        /// </summary>
        PaddingOn = 1,
        /// <summary>
        /// MPEGLAYER3_FLAG_PADDING_OFF
        /// </summary>
        PaddingOff = 2,
    }

    /// <summary>
    /// Wave Format ID
    /// </summary>
    internal enum Mp3WaveFormatId : ushort
    {
        /// <summary>MPEGLAYER3_ID_UNKNOWN</summary>
        Unknown = 0,
        /// <summary>MPEGLAYER3_ID_MPEG</summary>
        Mpeg = 1,
        /// <summary>MPEGLAYER3_ID_CONSTANTFRAMESIZE</summary>
        ConstantFrameSize = 2
    }
}
