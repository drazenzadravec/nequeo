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
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Wave
{
    /// <summary>
    /// IWaveBuffer interface use to store wave datas. 
    /// Data can be manipulated with arrays (<see cref="ByteBuffer"/>,<see cref="FloatBuffer"/>,
    /// <see cref="ShortBuffer"/>,<see cref="IntBuffer"/> ) that are pointing to the same memory buffer.
    /// This is a requirement for all subclasses.
    /// 
    /// Use the associated Count property based on the type of buffer to get the number of data in the 
    /// buffer.
    /// 
    /// <see cref="WaveBuffer"/> for the standard implementation using C# unions.
    /// </summary>
    internal interface IWaveBuffer
    {
        /// <summary>
        /// Gets the byte buffer.
        /// </summary>
        /// <value>The byte buffer.</value>
        byte[] ByteBuffer { get; }

        /// <summary>
        /// Gets the float buffer.
        /// </summary>
        /// <value>The float buffer.</value>
        float[] FloatBuffer { get; }

        /// <summary>
        /// Gets the short buffer.
        /// </summary>
        /// <value>The short buffer.</value>
        short[] ShortBuffer { get; }

        /// <summary>
        /// Gets the int buffer.
        /// </summary>
        /// <value>The int buffer.</value>
        int[] IntBuffer { get; }

        /// <summary>
        /// Gets the max size in bytes of the byte buffer..
        /// </summary>
        /// <value>Maximum number of bytes in the buffer.</value>
        int MaxSize { get; }

        /// <summary>
        /// Gets the byte buffer count.
        /// </summary>
        /// <value>The byte buffer count.</value>
        int ByteBufferCount { get; }

        /// <summary>
        /// Gets the float buffer count.
        /// </summary>
        /// <value>The float buffer count.</value>
        int FloatBufferCount { get; }

        /// <summary>
        /// Gets the short buffer count.
        /// </summary>
        /// <value>The short buffer count.</value>
        int ShortBufferCount { get; }

        /// <summary>
        /// Gets the int buffer count.
        /// </summary>
        /// <value>The int buffer count.</value>
        int IntBufferCount { get; }
    }
}
