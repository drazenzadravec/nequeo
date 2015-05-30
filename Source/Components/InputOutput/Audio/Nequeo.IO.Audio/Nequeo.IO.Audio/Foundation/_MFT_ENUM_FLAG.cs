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

namespace Nequeo.IO.Audio.Foundation
{
    /// <summary>
    /// Contains flags for registering and enumeration Media Foundation transforms (MFTs).
    /// </summary>
    [Flags]
    internal enum _MFT_ENUM_FLAG
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// The MFT performs synchronous data processing in software. 
        /// </summary>
        MFT_ENUM_FLAG_SYNCMFT = 0x00000001,
        /// <summary>
        /// The MFT performs asynchronous data processing in software.
        /// </summary>
        MFT_ENUM_FLAG_ASYNCMFT = 0x00000002,
        /// <summary>
        /// The MFT performs hardware-based data processing, using either the AVStream driver or a GPU-based proxy MFT. 
        /// </summary>
        MFT_ENUM_FLAG_HARDWARE = 0x00000004,
        /// <summary>
        /// The MFT that must be unlocked by the application before use.
        /// </summary>
        MFT_ENUM_FLAG_FIELDOFUSE = 0x00000008,
        /// <summary>
        /// For enumeration, include MFTs that were registered in the caller's process.
        /// </summary>
        MFT_ENUM_FLAG_LOCALMFT = 0x00000010,
        /// <summary>
        /// The MFT is optimized for transcoding rather than playback.
        /// </summary>
        MFT_ENUM_FLAG_TRANSCODE_ONLY = 0x00000020,
        /// <summary>
        /// For enumeration, sort and filter the results.
        /// </summary>
        MFT_ENUM_FLAG_SORTANDFILTER = 0x00000040,
        /// <summary>
        /// Bitwise OR of all the flags, excluding MFT_ENUM_FLAG_SORTANDFILTER.
        /// </summary>
        MFT_ENUM_FLAG_ALL = 0x0000003F
    }
}
