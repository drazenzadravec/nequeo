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
    public class SimpleAudioVolume
    {
        ISimpleAudioVolume _SimpleAudioVolume;
        internal SimpleAudioVolume(ISimpleAudioVolume realSimpleVolume)
        {
            _SimpleAudioVolume = realSimpleVolume;
        }

        public float MasterVolume
        {
            get
            {
                float ret;
                Marshal.ThrowExceptionForHR(_SimpleAudioVolume.GetMasterVolume(out ret));
                return ret;
            }
            set
            {
                Guid Empty = Guid.Empty;
                Marshal.ThrowExceptionForHR(_SimpleAudioVolume.SetMasterVolume(value, ref Empty));
            }
        }

        public bool Mute
        {
            get
            {
                bool ret;
                Marshal.ThrowExceptionForHR(_SimpleAudioVolume.GetMute(out ret));
                return ret;
            }
            set
            {
                Guid Empty = Guid.Empty;
                Marshal.ThrowExceptionForHR(_SimpleAudioVolume.SetMute(value, ref Empty));
            }
        }
    }
}
