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

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Supported audo format.
    /// </summary>
    public enum AudioFormat
    {
        /// <summary>
        /// Wav format.
        /// </summary>
        Wav = 0,
        /// <summary>
        /// Mp3 format.
        /// </summary>
        Mp3 = 1,
        /// <summary>
        /// Aiff format.
        /// </summary>
        Aiff = 2,
    }

    /// <summary>
    /// Supported audo recording formats.
    /// </summary>
    public enum AudioRecordingFormat
    {
        /// <summary>
        /// Allows recording using the Windows waveIn APIs.
        /// </summary>
        WaveIn = 0,
        /// <summary>
        /// Recording using waveIn api with event callbacks.
        /// Use this for recording in non-gui applications.
        /// </summary>
        WaveInEvent = 1,
        /// <summary>
        /// Audio recording using Wasapi
        /// </summary>
        Wasapi = 2,
        /// <summary>
        /// Audio recording using Wasapi loopback.
        /// </summary>
        WasapiLoopback= 3,
    }
}
