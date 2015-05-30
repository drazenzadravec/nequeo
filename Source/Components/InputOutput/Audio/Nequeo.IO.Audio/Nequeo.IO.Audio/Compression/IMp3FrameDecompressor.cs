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

namespace Nequeo.IO.Audio.Compression
{
    /// <summary>
    /// Interface for MP3 frame by frame decoder
    /// </summary>
    internal interface IMp3FrameDecompressor : IDisposable
    {
        /// <summary>
        /// Decompress a single MP3 frame
        /// </summary>
        /// <param name="frame">Frame to decompress</param>
        /// <param name="dest">Output buffer</param>
        /// <param name="destOffset">Offset within output buffer</param>
        /// <returns>Bytes written to output buffer</returns>
        int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset);

        /// <summary>
        /// Tell the decoder that we have repositioned
        /// </summary>
        void Reset();

        /// <summary>
        /// PCM format that we are converting into
        /// </summary>
        WaveFormatProvider OutputFormat { get; }
    }
}
