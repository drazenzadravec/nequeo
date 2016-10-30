/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Nequeo.IO.Audio.Api.Interfaces;

namespace Nequeo.IO.Audio.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioSessionManager
    {
        private IAudioSessionManager audioSessionInterface;

        private SimpleAudioVolume simpleAudioVolume = null;
        private AudioSessionControl audioSessionControl = null;

        internal AudioSessionManager(IAudioSessionManager audioSessionManager)
        {
            audioSessionInterface = audioSessionManager;
        }

        /// <summary>
        /// SimpleAudioVolume object
        /// for adjusting the volume for the user session
        /// </summary>
        public SimpleAudioVolume SimpleAudioVolume
        {
            get
            {
                if (simpleAudioVolume == null)
                {
                    ISimpleAudioVolume simpleAudioInterface;

                    audioSessionInterface.GetSimpleAudioVolume(Guid.Empty, 0, out simpleAudioInterface);

                    simpleAudioVolume = new SimpleAudioVolume(simpleAudioInterface);
                }
                return simpleAudioVolume;
            }
        }

        /// <summary>
        /// AudioSessionControl object
        /// for registring for callbacks and other session information
        /// </summary>
        public AudioSessionControl AudioSessionControl
        {
            get
            {
                if (audioSessionControl == null)
                {
                    IAudioSessionControl audioSessionControlInterface;

                    audioSessionInterface.GetAudioSessionControl(Guid.Empty, 0, out audioSessionControlInterface);

                    audioSessionControl = new AudioSessionControl(audioSessionControlInterface);
                }
                return audioSessionControl;
            }
        }
    }
}
