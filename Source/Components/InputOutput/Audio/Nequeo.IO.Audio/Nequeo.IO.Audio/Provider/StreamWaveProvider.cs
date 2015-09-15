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
using System.Runtime.InteropServices.ComTypes;

using Nequeo.IO.Audio.Wave;
using Nequeo.IO.Audio.Foundation;

namespace Nequeo.IO.Audio.Provider
{
    /// <summary>
    /// Stream wave provider.
    /// </summary>
    public class StreamWaveProvider : WaveStream
    {
        /// <summary>
        /// Stream wave provider.
        /// </summary>
        /// <param name="stream">The stream containing the data.</param>
        public StreamWaveProvider(Stream stream)
        {
            _stream = stream;

            _waveFormat = new WaveFormatProvider(44100, 16, 2);
        }

        private Stream _stream;
        private WaveFormatProvider _waveFormat;

        /// <summary>
        /// Gets the WaveFormat
        /// </summary>
        public override WaveFormatProvider WaveFormat
        {
            get { return _waveFormat; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public override long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }

        /// <summary>
        /// Fill the specified buffer with wave data.
        /// </summary>
        /// <param name="buffer">The buffer to fill of wave data.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>the number of bytes written to the buffer.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }
    }
}
