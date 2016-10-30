/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Nequeo.IO.Audio.Wave;
using Nequeo.IO.Audio.Utils;
using Nequeo.IO.Audio.Api;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// System volum control.
    /// </summary>
    public class Volume
    {
        /// <summary>
        /// System volum control.
        /// </summary>
        public Volume() { }

        /// <summary>
        /// On speaker changed notification.
        /// </summary>
        public event AudioEndpointVolumeNotificationDelegate OnSpeakerNotification;

        /// <summary>
        /// On microphone changed notification.
        /// </summary>
        public event AudioEndpointVolumeNotificationDelegate OnMicrophoneNotification;

        /// <summary>
        /// Set speaker notifications.
        /// </summary>
        public void SetSpeakerNotification()
        {
            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Render, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("speakers") || deviceAt.DataFlow == EDataFlow.Render)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        deviceAt.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnSpeakerNotification;
                    }
                }
            }
        }

        /// <summary>
        /// Set microphone notifications.
        /// </summary>
        public void SetMicrophoneNotification()
        {
            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Capture, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("microphone") || deviceAt.DataFlow == EDataFlow.Capture)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        deviceAt.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnMicrophoneNotification;
                    }
                }
            }
        }

        /// <summary>
        /// Speaker notify.
        /// </summary>
        /// <param name="data">The data.</param>
        private void AudioEndpointVolume_OnSpeakerNotification(Nequeo.IO.Audio.Api.AudioVolumeNotificationData data)
        {
            OnSpeakerNotification?.Invoke(data);
        }

        /// <summary>
        /// Microphone notify.
        /// </summary>
        /// <param name="data">The data.</param>
        private void AudioEndpointVolume_OnMicrophoneNotification(Nequeo.IO.Audio.Api.AudioVolumeNotificationData data)
        {
            OnMicrophoneNotification?.Invoke(data);
        }

        /// <summary>
        /// Get the system volume (between 0 and 10).
        /// </summary>
        /// <returns>The system volume.</returns>
        public static ushort GetVolume()
        {
            int currentVolume = 0;

            // At this point, CurrVol gets assigned the volume
            WaveInterop.waveOutGetVolume(IntPtr.Zero, out currentVolume);

            // Calculate the volume
            ushort calcVol = (ushort)(currentVolume & 0x0000ffff);

            // Get the volume on a scale of 1 to 10 (to fit the trackbar)
            return (ushort)(calcVol / (ushort.MaxValue / (ushort)10));
        }

        /// <summary>
        /// Set the system volume (between 0 and 10).
        /// </summary>
        /// <param name="level">The volume (between 0 and 10)</param>
        public static void SetVolume(ushort level)
        {
            // Calculate the volume that's being set
            int newVolume = ((ushort.MaxValue / 10) * level);

            // Set the same volume for both the left and the right channels
            int newVolumeAllChannels = (((int)newVolume & 0x0000ffff) | ((int)newVolume << 16));

            // Set the volume
            WaveInterop.waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
        }

        /// <summary>
        /// Get the device volume.
        /// </summary>
        /// <param name="device">The current device.</param>
        /// <returns>The device volume.</returns>
        public static float GetVolume(MMDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            return device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        /// <summary>
        /// Set the device volume.
        /// </summary>
        /// <param name="device">The current device.</param>
        /// <param name="level">The volume level.</param>
        public static void SetVolume(MMDevice device, float level)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            device.AudioEndpointVolume.MasterVolumeLevelScalar = level;
        }

        /// <summary>
        /// Get all the output devices.
        /// </summary>
        /// <returns>The list of output devices.</returns>
        public static MMDevice[] GetAllOutputDevices()
        {
            List<MMDevice> mmdevices = new List<MMDevice>();

            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Render, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                mmdevices.Add(devices[i]);
            }

            // Return the device list.
            return mmdevices.ToArray();
        }

        /// <summary>
        /// Get all the input devices.
        /// </summary>
        /// <returns>The list of input devices.</returns>
        public static MMDevice[] GetAllInputDevices()
        {
            List<MMDevice> mmdevices = new List<MMDevice>();

            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Capture, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                mmdevices.Add(devices[i]);
            }

            // Return the device list.
            return mmdevices.ToArray();
        }

        /// <summary>
        /// Mute the output device.
        /// </summary>
        /// <param name="device">The current device.</param>
        /// <param name="mute">True to mute; else false.</param>
        public static void MuteVolume(MMDevice device, bool mute)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            device.AudioEndpointVolume.Mute = mute;
        }

        /// <summary>
        /// Mute all output devices.
        /// </summary>
        /// <param name="mute">True to mute; else false.</param>
        public static void MuteVolume(bool mute)
        {
            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Render, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // If device found.
                if (deviceAt != null)
                {
                    try
                    {
                        // Mute or un mute the device.
                        deviceAt.AudioEndpointVolume.Mute = mute;
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Mute all microphones.
        /// </summary>
        /// <param name="mute">True to mute; else false.</param>
        public static void MuteMicrophone(bool mute)
        {
            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Capture, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("microphone") || deviceAt.DataFlow == EDataFlow.Capture)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        // Mute or un mute the device.
                        deviceAt.AudioEndpointVolume.Mute = mute;
                    }
                }
            }
        }

        /// <summary>
        /// Mute the microphone.
        /// </summary>
        /// <param name="device">The current device.</param>
        /// <param name="mute">True to mute; else false.</param>
        public static void MuteMicrophone(MMDevice device, bool mute)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            device.AudioEndpointVolume.Mute = mute;
        }

        /// <summary>
        /// Get the device volume.
        /// </summary>
        /// <param name="device">The current device.</param>
        /// <returns>The device volume.</returns>
        public static float GetMicrophoneVolume(MMDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            return device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        /// <summary>
        /// Set the device volume.
        /// </summary>
        /// <param name="device">The current device.</param>
        /// <param name="level">The volume level.</param>
        public static void SetMicrophoneVolume(MMDevice device, float level)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            device.AudioEndpointVolume.MasterVolumeLevelScalar = level;
        }

        /// <summary>
        /// Set the device volume.
        /// </summary>
        /// <param name="level">The volume level.</param>
        public static void SetMicrophoneVolume(float level)
        {
            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Capture, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("microphone") || deviceAt.DataFlow == EDataFlow.Capture)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        // Mute or un mute the device.
                        deviceAt.AudioEndpointVolume.MasterVolumeLevelScalar = level;
                    }
                }
            }
        }

        /// <summary>
        /// Get the device volume.
        /// </summary>
        /// <returns>The device volume of each device.</returns>
        public static float[] GetMicrophoneVolume()
        {
            List<float> volums = new List<float>();

            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Capture, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("microphone") || deviceAt.DataFlow == EDataFlow.Capture)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        // Mute or un mute the device.
                        volums.Add(deviceAt.AudioEndpointVolume.MasterVolumeLevelScalar);
                    }
                }
            }

            // Return the volumes.
            return volums.ToArray();
        }

        /// <summary>
        /// Get the device volume.
        /// </summary>
        /// <returns>The device volume of each device.</returns>
        public static float[] GetSpeakerVolume()
        {
            List<float> volums = new List<float>();

            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Render, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("speakers") || deviceAt.DataFlow == EDataFlow.Render)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        // Mute or un mute the device.
                        volums.Add(deviceAt.AudioEndpointVolume.MasterVolumeLevelScalar);
                    }
                }
            }

            // Return the volumes.
            return volums.ToArray();
        }

        /// <summary>
        /// Set the device volume.
        /// </summary>
        /// <param name="level">The volume level. (between 0.0 and 1.0)</param>
        public static void SetSpeakerVolume(float level)
        {
            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Render, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("speakers") || deviceAt.DataFlow == EDataFlow.Render)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        // Mute or un mute the device.
                        deviceAt.AudioEndpointVolume.MasterVolumeLevelScalar = level;
                    }
                }
            }
        }

        /// <summary>
        /// Get the device mute state.
        /// </summary>
        /// <returns>The device mute state of each device.</returns>
        public static bool[] GetSpeakerMute()
        {
            List<bool> volums = new List<bool>();

            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Render, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("speakers") || deviceAt.DataFlow == EDataFlow.Render)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        // Mute or un mute the device.
                        volums.Add(deviceAt.AudioEndpointVolume.Mute);
                    }
                }
            }

            // Return the volumes.
            return volums.ToArray();
        }

        /// <summary>
        /// Get the device mute state.
        /// </summary>
        /// <returns>The device mute state of each device.</returns>
        public static bool[] GetMicrophoneMute()
        {
            List<bool> volums = new List<bool>();

            // Get the device.
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDeviceCollection devices = DevEnum.EnumerateAudioEndPoints(EDataFlow.Capture, EDeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                MMDevice deviceAt = devices[i];

                // Find the device.
                if (deviceAt.FriendlyName.ToLower().Contains("microphone") || deviceAt.DataFlow == EDataFlow.Capture)
                {
                    // If device found.
                    if (deviceAt != null)
                    {
                        // Mute or un mute the device.
                        volums.Add(deviceAt.AudioEndpointVolume.Mute);
                    }
                }
            }

            // Return the volumes.
            return volums.ToArray();
        }
    }
}
