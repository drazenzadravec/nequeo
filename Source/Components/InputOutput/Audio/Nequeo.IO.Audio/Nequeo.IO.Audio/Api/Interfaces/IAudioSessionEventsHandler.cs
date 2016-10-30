/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;

namespace Nequeo.IO.Audio.Api.Interfaces
{
    /// <summary>
    /// interface to receive session related events
    /// </summary>
    public interface IAudioSessionEventsHandler
    {
        /// <summary>
        /// notification of volume changes including muting of audio session
        /// </summary>
        /// <param name="volume">the current volume</param>
        /// <param name="isMuted">the current mute state, true muted, false otherwise</param>
        void OnVolumeChanged(float volume, bool isMuted);
    }
}
