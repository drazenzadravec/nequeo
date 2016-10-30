/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

namespace Nequeo.IO.Audio.Api
{
    /// <summary>
    /// Defines values that describe the characteristics of an audio stream.
    /// </summary>
    internal enum AudioClientStreamOptions
    {
        /// <summary>
        /// No stream options.
        /// </summary>
        None = 0,
        /// <summary>
        /// The audio stream is a 'raw' stream that bypasses all signal processing except for endpoint specific, always-on processing in the APO, driver, and hardware.
        /// </summary>
        Raw = 0x1
    }
}