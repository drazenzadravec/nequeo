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

using Nequeo.IO.Audio.Wave;

namespace Nequeo.IO.Audio.Compression
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    internal enum AcmFormatSuggestFlags
    {
        /// <summary>
        /// ACM_FORMATSUGGESTF_WFORMATTAG
        /// </summary>
        FormatTag = 0x00010000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_NCHANNELS
        /// </summary>
        Channels = 0x00020000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_NSAMPLESPERSEC
        /// </summary>
        SamplesPerSecond = 0x00040000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_WBITSPERSAMPLE
        /// </summary>
        BitsPerSample = 0x00080000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_TYPEMASK
        /// </summary>
        TypeMask = 0x00FF0000,
    }
}
