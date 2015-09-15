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
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Wave provider.
    /// </summary>
    internal class WaveProvider : IWaveProvider
    {
        /// <summary>
        /// Gets the WaveFormat of this WaveProvider.
        /// </summary>
        /// <value>The wave format.</value>
        public WaveFormatProvider WaveFormat
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Fill the specified buffer with wave data.
        /// </summary>
        /// <param name="buffer">The buffer to fill of wave data.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>the number of bytes written to the buffer.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
