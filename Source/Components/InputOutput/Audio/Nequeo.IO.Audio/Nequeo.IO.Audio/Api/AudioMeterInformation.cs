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
    public class AudioMeterInformation
    {
        private IAudioMeterInformation _AudioMeterInformation;
        private EEndpointHardwareSupport _HardwareSupport;
        private AudioMeterInformationChannels _Channels;

        internal AudioMeterInformation(IAudioMeterInformation realInterface)
        {
            int HardwareSupp;

            _AudioMeterInformation = realInterface;
            Marshal.ThrowExceptionForHR(_AudioMeterInformation.QueryHardwareSupport(out HardwareSupp));
            _HardwareSupport = (EEndpointHardwareSupport)HardwareSupp;
            _Channels = new AudioMeterInformationChannels(_AudioMeterInformation);

        }

        /// <summary>
        /// 
        /// </summary>
        public AudioMeterInformationChannels PeakValues
        {
            get
            {
                return _Channels;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EEndpointHardwareSupport HardwareSupport
        {
            get
            {
                return _HardwareSupport;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float MasterPeakValue
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioMeterInformation.GetPeakValue(out result));
                return result;
            }
        }

       

      


    }
}
