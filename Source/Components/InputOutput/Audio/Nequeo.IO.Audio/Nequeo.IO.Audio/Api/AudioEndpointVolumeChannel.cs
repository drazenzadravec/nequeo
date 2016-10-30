/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;
using Nequeo.IO.Audio.Api.Interfaces;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioEndpointVolumeChannel
    {
        private uint _Channel;
        private IAudioEndpointVolume _AudioEndpointVolume;

        internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel)
        {
            _Channel = (uint)channel;
            _AudioEndpointVolume = parent;
        }

        public float VolumeLevel
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.GetChannelVolumeLevel(_Channel,out result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.SetChannelVolumeLevel(_Channel, value,Guid.Empty));
            }
        }

        public float VolumeLevelScalar
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.GetChannelVolumeLevelScalar(_Channel, out result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.SetChannelVolumeLevelScalar(_Channel, value, Guid.Empty));
            }
        }

    }
}
