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
    /// MPEG Version Flags
    /// </summary>
    internal enum MpegVersion
    {
        /// <summary>
        /// Version 2.5
        /// </summary>
        Version25,
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved,
        /// <summary>
        /// Version 2
        /// </summary>
        Version2,
        /// <summary>
        /// Version 1
        /// </summary>
        Version1
    }

    /// <summary>
    /// Channel Mode
    /// </summary>
    internal enum ChannelMode
    {
        /// <summary>
        /// Stereo
        /// </summary>
        Stereo,
        /// <summary>
        /// Joint Stereo
        /// </summary>
        JointStereo,
        /// <summary>
        /// Dual Channel
        /// </summary>
        DualChannel,
        /// <summary>
        /// Mono
        /// </summary>
        Mono
    }

    /// <summary>
    /// MPEG Layer flags
    /// </summary>
    internal enum MpegLayer
    {
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved,
        /// <summary>
        /// Layer 3
        /// </summary>
        Layer3,
        /// <summary>
        /// Layer 2
        /// </summary>
        Layer2,
        /// <summary>
        /// Layer 1
        /// </summary>
        Layer1
    }
}
