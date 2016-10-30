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
    public class AudioEndpointVolumeChannels
    {
        IAudioEndpointVolume _AudioEndPointVolume;
        AudioEndpointVolumeChannel[] _Channels;
        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.GetChannelCount(out result));
                return result;
            }
        }

        public AudioEndpointVolumeChannel this[int index]
        {
            get
            {
                return _Channels[index];
            }
        }

        internal AudioEndpointVolumeChannels(IAudioEndpointVolume parent)
        {
            int ChannelCount;
            _AudioEndPointVolume = parent;

            ChannelCount = Count;
            _Channels = new AudioEndpointVolumeChannel[ChannelCount];
            for (int i = 0; i < ChannelCount; i++)
            {
                _Channels[i] = new AudioEndpointVolumeChannel(_AudioEndPointVolume, i);
            }
        }


    }
}
