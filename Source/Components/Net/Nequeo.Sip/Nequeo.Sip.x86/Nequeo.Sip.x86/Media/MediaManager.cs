/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// Sip media manager.
    /// </summary>
    public class MediaManager : IDisposable
    {
        /// <summary>
        /// Sip media manager.
        /// </summary>
        /// <param name="pjAudDevManager">Audio device manager.</param>
        internal MediaManager(pjsua2.AudDevManager pjAudDevManager)
        {
            _pjAudDevManager = pjAudDevManager;
        }

        private pjsua2.AudDevManager _pjAudDevManager = null;

        /// <summary>
        /// Get all audio devices installed in the system.
        /// </summary>
        /// <returns>The array of audio devices installed in the system.</returns>
        public AudioDeviceInfo[] GetAllAudioDevices()
        {
            List<AudioDeviceInfo> audioDeviceInfo = new List<AudioDeviceInfo>();
            pjsua2.AudioDevInfoVector audioDevices = _pjAudDevManager.enumDev();

            // If devices exist.
            if (audioDevices != null && audioDevices.Count > 0)
            {
                // For each device.
                for (int i = 0; i < audioDevices.Count; i++)
                {
                    AudioDeviceInfo audoDevice = new AudioDeviceInfo();
                    audoDevice.Caps = audioDevices[i].caps;
                    audoDevice.DefaultSamplesPerSec = audioDevices[i].defaultSamplesPerSec;
                    audoDevice.Driver = audioDevices[i].driver;
                    audoDevice.InputCount = audioDevices[i].inputCount;
                    audoDevice.Name = audioDevices[i].name;
                    audoDevice.OutputCount = audioDevices[i].outputCount;
                    audoDevice.Routes = audioDevices[i].routes;

                    // Get the media format list.
                    pjsua2.MediaFormatVector mediaFormats = audioDevices[i].extFmt;

                    // if media format exists.
                    if (mediaFormats != null && mediaFormats.Count > 0)
                    {
                        List<MediaFormat> formats = new List<MediaFormat>();

                        // For each format.
                        for (int j = 0; j < mediaFormats.Count; j++)
                        {
                            MediaFormat mediaFormat = new MediaFormat();
                            mediaFormat.Id = mediaFormats[j].id;
                            mediaFormat.Type = MediaFormat.GetMediaTypeEx(mediaFormats[j].type);

                            // Add the media formats.
                            formats.Add(mediaFormat);
                        }

                        // Add the list of media formats.
                        audoDevice.MediaFormats = formats.ToArray();
                    }

                    // Add the audio device.
                    audioDeviceInfo.Add(audoDevice);
                }
            }

            // Return the list of devices.
            return audioDeviceInfo.ToArray();
        }

        /// <summary>
        /// Get device index based on the driver and device name.
        /// </summary>
        /// <param name="driverName">The driver name.</param>
        /// <param name="deviceName">The device name.</param>
        /// <returns>The device ID. If the device is not found, error will be thrown.</returns>
        public int GetAudioDeviceID(string driverName, string deviceName)
        {
            return _pjAudDevManager.lookupDev(driverName, deviceName);
        }

        /// <summary>
        /// Get currently active capture sound devices. If sound devices has not been
        /// created, it is possible that the function returns -1 as device IDs.
        /// </summary>
        /// <returns>Device ID of the capture device.</returns>
        public int GetCaptureDevice()
        {
            return _pjAudDevManager.getCaptureDev();
        }

        /// <summary>
        /// Set or change capture sound device. Application may call this
        /// function at any time to replace current sound device.
        /// </summary>
        /// <param name="deviceID">Device ID of the capture device.</param>
        public void SetCaptureDevice(int deviceID)
        {
            _pjAudDevManager.setCaptureDev(deviceID);
        }

        /// <summary>
        /// Get currently active playback sound devices. If sound devices has not
        /// been created, it is possible that the function returns -1 as device IDs.
        /// </summary>
        /// <returns>Device ID of the playback device.</returns>
        public int GetPlaybackDevice()
        {
            return _pjAudDevManager.getPlaybackDev();
        }

        /// <summary>
        /// Set or change playback sound device. Application may call this
        /// function at any time to replace current sound device.
        /// </summary>
        /// <param name="deviceID">Device ID of the playback device.</param>
        public void SetPlaybackDevice(int deviceID)
        {
            _pjAudDevManager.setPlaybackDev(deviceID);
        }

        /// <summary>
        /// Get the AudioMedia of the capture audio device.
        /// </summary>
        /// <returns>Audio media for the capture device.</returns>
        public AudioMedia GetCaptureDeviceMedia()
        {
            return new AudioMedia(_pjAudDevManager.getCaptureDevMedia());
        }

        /// <summary>
        /// Get the AudioMedia of the speaker/playback audio device.
        /// </summary>
        /// <returns>Audio media for the speaker/playback device.</returns>
        public AudioMedia GetPlaybackDeviceMedia()
        {
            return new AudioMedia(_pjAudDevManager.getPlaybackDevMedia());
        }

        /// <summary>
        /// Start conference call between remote parties; allow each party to talk to each other.
        /// </summary>
        /// <param name="conferenceCalls">Array of remote conference calls.</param>
        public void StartConferenceCall(AudioMedia[] conferenceCalls)
        {
            // For each call.
            for (int i = 0; i < conferenceCalls.Length; i++)
            {
                // Get first group.
                pjsua2.AudioMedia mediaCall_1 = conferenceCalls[i].PjAudioMedia;

                // For each call.
                for (int j = 0; j < conferenceCalls.Length; j++)
                {
                    // Get second group.
                    pjsua2.AudioMedia mediaCall_2 = conferenceCalls[j].PjAudioMedia;

                    // If the two audio media are not equal.
                    if (mediaCall_1.getPortId() != mediaCall_2.getPortId())
                    {
                        // Allow these two calls to communicate.
                        mediaCall_1.startTransmit(mediaCall_2);
                    }
                }
            }
        }

        /// <summary>
        /// Stop conference call between remote parties; allow each party to talk to each other.
        /// </summary>
        /// <param name="conferenceCalls">Array of remote conference calls.</param>
        public void StopConferenceCall(AudioMedia[] conferenceCalls)
        {
            // For each call.
            for (int i = 0; i < conferenceCalls.Length; i++)
            {
                // Get first group.
                pjsua2.AudioMedia mediaCall_1 = conferenceCalls[i].PjAudioMedia;

                // For each call.
                for (int j = 0; j < conferenceCalls.Length; j++)
                {
                    // Get second group.
                    pjsua2.AudioMedia mediaCall_2 = conferenceCalls[j].PjAudioMedia;

                    // If the two audio media are not equal.
                    if (mediaCall_1.getPortId() != mediaCall_2.getPortId())
                    {
                        // Stop these two calls from communicating.
                        mediaCall_1.stopTransmit(mediaCall_2);
                    }
                }
            }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_pjAudDevManager != null)
                        _pjAudDevManager.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _pjAudDevManager = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~MediaManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
