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
    /// Media device.
    /// </summary>
    public class MMDevice
    {
        #region Variables
        private readonly IMMDevice deviceInterface;
        private PropertyStore propertyStore;
        private AudioMeterInformation audioMeterInformation;
        private AudioEndpointVolume audioEndpointVolume;
        private AudioSessionManager audioSessionManager;
        #endregion

        #region Guids
        private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");
        private static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
        private static Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");
        private static Guid IDD_IAudioSessionManager = new Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4");
        #endregion

        #region Init
        private void GetPropertyInformation()
        {
            IPropertyStore propstore;
            Marshal.ThrowExceptionForHR(deviceInterface.OpenPropertyStore(StorageAccessMode.Read, out propstore));
            propertyStore = new PropertyStore(propstore);
        }

        private AudioClient GetAudioClient()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioClient, CLSCTX.ALL, IntPtr.Zero, out result));
            return new AudioClient(result as IAudioClient);
        }

        private void GetAudioMeterInformation()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioMeterInformation, CLSCTX.ALL, IntPtr.Zero, out result));
            audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        }

        private void GetAudioEndpointVolume()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioEndpointVolume, CLSCTX.ALL, IntPtr.Zero, out result));
            audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
        }

        private void GetAudioSessionManager()
        {
            object result;
            Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IDD_IAudioSessionManager, CLSCTX.ALL, IntPtr.Zero, out result));
            audioSessionManager = new AudioSessionManager(result as IAudioSessionManager);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Audio Client
        /// </summary>
        public AudioClient AudioClient
        {
            get
            {
                // now makes a new one each call to allow caller to manage when to dispose
                // n.b. should probably not be a property anymore
                return GetAudioClient();
            }
        }

        /// <summary>
        /// Audio Meter Information
        /// </summary>
        public AudioMeterInformation AudioMeterInformation
        {
            get
            {
                if (audioMeterInformation == null)
                    GetAudioMeterInformation();

                return audioMeterInformation;
            }
        }

        /// <summary>
        /// Audio Endpoint Volume
        /// </summary>
        public AudioEndpointVolume AudioEndpointVolume
        {
            get
            {
                if (audioEndpointVolume == null)
                    GetAudioEndpointVolume();

                return audioEndpointVolume;
            }
        }

        /// <summary>
        /// AudioSessionManager instance
        /// </summary>
        public AudioSessionManager AudioSessionManager
        {
            get
            {
                if (audioSessionManager == null)
                {
                    GetAudioSessionManager();
                }
                return audioSessionManager;
            }
        }

        /// <summary>
        /// Properties
        /// </summary>
        public PropertyStore Properties
        {
            get
            {
                if (propertyStore == null)
                    GetPropertyInformation();
                return propertyStore;
            }
        }

        /// <summary>
        /// Friendly name for the endpoint
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if (propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName))
                {
                    return (string)propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
                }
                else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Friendly name of device
        /// </summary>
        public string DeviceFriendlyName
        {
            get
            {
                if (propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (propertyStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
                {
                    return (string)propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        /// <summary>
        /// Device ID
        /// </summary>
        public string ID
        {
            get
            {
                string result;
                Marshal.ThrowExceptionForHR(deviceInterface.GetId(out result));
                return result;
            }
        }

        /// <summary>
        /// Data Flow
        /// </summary>
        public EDataFlow DataFlow
        {
            get
            {
                EDataFlow result;
                var ep = deviceInterface as IMMEndpoint;
                ep.GetDataFlow(out result);
                return result;
            }
        }

        /// <summary>
        /// Device State
        /// </summary>
        public EDeviceState State
        {
            get
            {
                EDeviceState result;
                Marshal.ThrowExceptionForHR(deviceInterface.GetState(out result));
                return result;
            }
        }
        #endregion

        #region Constructor
        internal MMDevice(IMMDevice realDevice)
        {
            deviceInterface = realDevice;
        }
        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return FriendlyName;
        }

    }
}
