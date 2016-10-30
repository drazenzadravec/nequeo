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
    // This class implements the IAudioEndpointVolumeCallback interface,
    // it is implemented in this class because implementing it on AudioEndpointVolume 
    // (where the functionality is really wanted, would cause the OnNotify function 
    // to show up in the public API. 
    internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback    
    {
        private AudioEndpointVolume _Parent;
        
        internal AudioEndpointVolumeCallback(AudioEndpointVolume parent)
        {
            _Parent = parent;
        }
        
        [PreserveSig] public int OnNotify(IntPtr NotifyData)
        {
            //Since AUDIO_VOLUME_NOTIFICATION_DATA is dynamic in length based on the
            //number of audio channels available we cannot just call PtrToStructure 
            //to get all data, thats why it is split up into two steps, first the static
            //data is marshalled into the data structure, then with some IntPtr math the
            //remaining floats are read from memory.
            //
            AUDIO_VOLUME_NOTIFICATION_DATA data = (AUDIO_VOLUME_NOTIFICATION_DATA) Marshal.PtrToStructure(NotifyData, typeof(AUDIO_VOLUME_NOTIFICATION_DATA));
            
            //Determine offset in structure of the first float
            IntPtr Offset = Marshal.OffsetOf(typeof(AUDIO_VOLUME_NOTIFICATION_DATA),"ChannelVolume");
            //Determine offset in memory of the first float
            IntPtr FirstFloatPtr = (IntPtr)((long)NotifyData + (long)Offset);

            float[] voldata = new float[data.nChannels];
            
            //Read all floats from memory.
            for (int i = 0; i < data.nChannels; i++)
            {
                voldata[i] = (float)Marshal.PtrToStructure(FirstFloatPtr, typeof(float));
            }

            //Create combined structure and Fire Event in parent class.
            AudioVolumeNotificationData NotificationData = new AudioVolumeNotificationData(data.guidEventContext, data.bMuted, data.fMasterVolume, voldata);
            _Parent.FireNotification(NotificationData);
            return 0; //S_OK
        }
    }
}
