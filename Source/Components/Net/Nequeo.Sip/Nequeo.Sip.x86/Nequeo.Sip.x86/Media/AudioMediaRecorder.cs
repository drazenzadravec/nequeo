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
    /// Audio media recorder.
    /// </summary>
    public class AudioMediaRecorder : IDisposable
    {
        /// <summary>
        /// Audio media recorder.
        /// </summary>
        public AudioMediaRecorder()
        {
            _pjAudioMediaRecorder = new pjsua2.AudioMediaRecorder();
        }

        private pjsua2.AudioMediaRecorder _pjAudioMediaRecorder = null;

        /// <summary>
        /// Create a file recorder, and automatically connect this recorder to
        /// the conference bridge.The recorder currently supports recording WAV
        /// file.The type of the recorder to use is determined by the extension of
        /// the file(e.g. ".wav").
        /// </summary>
        /// <param name="filename">Output file name. The function will determine the
        ///	default format to be used based on the file extension.
        ///	Currently ".wav" is supported on all platforms.</param>
        /// <param name="encoderType">Optionally specify the type of encoder to be used to
        ///	compress the media, if the file can support different
        ///	encodings.This value must be zero for now.</param>
        public void CreateRecorder(string filename, uint encoderType)
        {
            _pjAudioMediaRecorder.createRecorder(filename, encoderType);
        }

        /// <summary>
        /// Create a file recorder, and automatically connect this recorder to
        /// the conference bridge.The recorder currently supports recording WAV
        /// file.The type of the recorder to use is determined by the extension of
        /// the file(e.g. ".wav").
        /// </summary>
        /// <param name="filename">Output file name. The function will determine the
        ///	default format to be used based on the file extension.
        ///	Currently ".wav" is supported on all platforms.</param>
        /// <param name="encoderType">Optionally specify the type of encoder to be used to
        ///	compress the media, if the file can support different
        ///	encodings.This value must be zero for now.</param>
        /// <param name="maxSize">Maximum file size. Specify zero or -1 to remove size
        ///	limitation. This value must be zero or -1 for now.</param>
        /// <param name="options">Optional options, which can be used to specify the
        /// recording file format. Default is zero or PJMEDIA_FILE_WRITE_PCM.</param>
        public void CreateRecorder(string filename, uint encoderType, long maxSize, uint options)
        {
            _pjAudioMediaRecorder.createRecorder(filename, encoderType, new PjSizeType(), options);
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        /// <param name="captureMedia">The audio capture media.</param>
        public void Start(AudioMedia captureMedia)
        {
            pjsua2.AudioMedia media = captureMedia.PjAudioMedia;
            media.startTransmit(_pjAudioMediaRecorder);
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        /// <param name="captureMedia">The audio capture media.</param>
        public void Stop(AudioMedia captureMedia)
        {
            pjsua2.AudioMedia media = captureMedia.PjAudioMedia;
            media.stopTransmit(_pjAudioMediaRecorder);
        }

        /// <summary>
        /// Start recoding a conversation between one or more calls.
        /// </summary>
        /// <param name="captureMedia">The capture media; e.g the local microphone.</param>
        /// <param name="conferenceCalls">Array of remote conference calls.</param>
        public void StartRecordingConversation(AudioMedia captureMedia, AudioMedia[] conferenceCalls)
        {
            pjsua2.AudioMedia media = captureMedia.PjAudioMedia;
            media.startTransmit(_pjAudioMediaRecorder);

            // For each call.
            for (int i = 0; i < conferenceCalls.Length; i++)
            {
                pjsua2.AudioMedia mediaCall = conferenceCalls[i].PjAudioMedia;
                mediaCall.startTransmit(_pjAudioMediaRecorder);
            }
        }

        /// <summary>
        /// Stop recoding a conversation between one or more calls.
        /// </summary>
        /// <param name="captureMedia">The capture media; e.g the local microphone.</param>
        /// <param name="conferenceCalls">Array of remote conference calls.</param>
        public void StopRecordingConversation(AudioMedia captureMedia, AudioMedia[] conferenceCalls)
        {
            pjsua2.AudioMedia media = captureMedia.PjAudioMedia;
            media.stopTransmit(_pjAudioMediaRecorder);

            // For each call.
            for (int i = 0; i < conferenceCalls.Length; i++)
            {
                pjsua2.AudioMedia mediaCall = conferenceCalls[i].PjAudioMedia;
                mediaCall.stopTransmit(_pjAudioMediaRecorder);
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
                    if (_pjAudioMediaRecorder != null)
                        _pjAudioMediaRecorder.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _pjAudioMediaRecorder = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~AudioMediaRecorder()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
