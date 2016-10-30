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
    public class AudioMeterInformationChannels
    {
        IAudioMeterInformation _AudioMeterInformation;

        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(_AudioMeterInformation.GetMeteringChannelCount(out result));
                return result;
            }
        }

        public float this[int index]
        {
            get
            {
                float[] peakValues = new float[Count];
                GCHandle Params = GCHandle.Alloc(peakValues, GCHandleType.Pinned);
                Marshal.ThrowExceptionForHR(_AudioMeterInformation.GetChannelsPeakValues(peakValues.Length, Params.AddrOfPinnedObject()));
                Params.Free();
                return peakValues[index];
            }
        }

        internal AudioMeterInformationChannels(IAudioMeterInformation parent)
        {
            _AudioMeterInformation = parent;
        }
    }
}
