/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Nequeo.IO.Audio.Api.Interfaces
{
    internal struct AUDIO_VOLUME_NOTIFICATION_DATA
    {
        public Guid   guidEventContext;
        public bool   bMuted;
        public float  fMasterVolume;
        public uint   nChannels;
        public float ChannelVolume;

        //Code Should Compile at warning level4 without any warnings, 
        //However this struct will give us Warning CS0649: Field [Fieldname] 
        //is never assigned to, and will always have its default value
        //You can disable CS0649 in the project options but that will disable
        //the warning for the whole project, it's a nice warning and we do want 
        //it in other places so we make a nice dummy function to keep the compiler
        //happy.
        private void FixCS0649()
        {
            guidEventContext = Guid.Empty;
            bMuted = false;
            fMasterVolume = 0;
            nChannels = 0;
            ChannelVolume = 0;
        }

    }
}
