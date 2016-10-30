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
    public class AudioEndPointVolumeVolumeRange
    {
        float _VolumeMindB;
        float _VolumeMaxdB;
        float _VolumeIncrementdB;

        internal AudioEndPointVolumeVolumeRange(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeRange(out _VolumeMindB,out _VolumeMaxdB,out _VolumeIncrementdB));
        }

        public float MindB
        {
            get
            {
                return _VolumeMindB;
            }
        }

        public float MaxdB
        {
            get
            {
                return _VolumeMaxdB;
            }
        }

        public float IncrementdB
        {
            get
            {
                return _VolumeIncrementdB;
            }
        }
    }
}
