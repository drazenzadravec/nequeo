/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;

namespace Nequeo.IO.Audio.Api
{
    /// <summary>
    /// AUDCLNT_STREAMFLAGS
    /// </summary>
    [Flags]
    public enum AudioClientStreamFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// AUDCLNT_STREAMFLAGS_CROSSPROCESS
        /// </summary>
        CrossProcess = 0x00010000,
        /// <summary>
        /// AUDCLNT_STREAMFLAGS_LOOPBACK
        /// </summary>
        Loopback = 0x00020000,
        /// <summary>
        /// AUDCLNT_STREAMFLAGS_EVENTCALLBACK 
        /// </summary>
        EventCallback = 0x00040000,
        /// <summary>
        /// AUDCLNT_STREAMFLAGS_NOPERSIST     
        /// </summary>
        NoPersist = 0x00080000,
    }
}
