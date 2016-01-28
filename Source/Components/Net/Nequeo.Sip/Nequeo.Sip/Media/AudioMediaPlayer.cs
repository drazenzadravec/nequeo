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
    /// Audio media player.
    /// </summary>
    public class AudioMediaPlayer : IDisposable
    {
        /// <summary>
        /// Audio media player.
        /// </summary>
        public AudioMediaPlayer()
        {
            _pjAudioMediaPlayer = new pjsua2.AudioMediaPlayer();
        }

        private pjsua2.AudioMediaPlayer _pjAudioMediaPlayer = null;

        /// <summary>
        /// Create a file player, and automatically add this 
        /// player to the conference bridge.
        /// </summary>
        /// <param name="filename">The filename to be played. Currently only
        ///	WAV files are supported, and the WAV file MUST be
        ///	formatted as 16bit PCM mono / single channel(any
        ///	clock rate is supported).</param>
        /// <param name="options">Optional option flag. Application may specify
        /// PJMEDIA_FILE_NO_LOOP to prevent playback loop; default is zero.</param>
        public void CreatePlayer(string filename, uint options)
        {
            _pjAudioMediaPlayer.createPlayer(filename, options);
        }

        /// <summary>
        /// Create a file playlist media port, and automatically add the port
        /// to the conference bridge.
        /// </summary>
        /// <param name="filenames">The Array of file names to be added to the play list.
        ///	Note that the files must have the same clock rate,
        /// number of channels, and number of bits per sample.</param>
        /// <param name="label">Optional label to be set for the media port; default is empty string.</param>
        /// <param name="options">Optional option flag. Application may specify
        /// PJMEDIA_FILE_NO_LOOP to prevent playback loop; default is zero.</param>
        public void CreatePlaylist(string[] filenames, string label, uint options)
        {
            pjsua2.StringVector vector = new pjsua2.StringVector(filenames);
            _pjAudioMediaPlayer.createPlaylist(vector, label, options);
        }

        /// <summary>
        /// Get current playback position in samples. This operation is not valid for playlist.
        /// </summary>
        /// <returns>The current playback position, in samples..</returns>
        public uint GetPosition()
        {
            return _pjAudioMediaPlayer.getPos();
        }

        /// <summary>
        /// Set playback position in samples. This operation is not valid for playlist.
        /// </summary>
        /// <param name="samples">The desired playback position, in samples.</param>
        public void SetPosition(uint samples)
        {
            _pjAudioMediaPlayer.setPos(samples);
        }

        /// <summary>
        /// Start playback.
        /// </summary>
        /// <param name="playbackMedia">The audio playback media.</param>
        public void Start(AudioMedia playbackMedia)
        {
            pjsua2.AudioMedia media = playbackMedia.PjAudioMedia;
            _pjAudioMediaPlayer.startTransmit(media);
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        /// <param name="playbackMedia">The audio playback media.</param>
        public void Stop(AudioMedia playbackMedia)
        {
            pjsua2.AudioMedia media = playbackMedia.PjAudioMedia;
            _pjAudioMediaPlayer.stopTransmit(media);
        }

        /// <summary>
        /// Start playing audio to each call.
        /// </summary>
        /// <param name="conferenceCalls">Array of remote conference calls.</param>
        public void StartPlayingConversation(AudioMedia[] conferenceCalls)
        {
            // For each call.
            for (int i = 0; i < conferenceCalls.Length; i++)
            {
                pjsua2.AudioMedia media = conferenceCalls[i].PjAudioMedia;
                _pjAudioMediaPlayer.startTransmit(media);
            }
        }

        /// <summary>
        /// Stop playing audio to each call.
        /// </summary>
        /// <param name="conferenceCalls">Array of remote conference calls.</param>
        public void StoptPlayingConversation(AudioMedia[] conferenceCalls)
        {
            // For each call.
            for (int i = 0; i < conferenceCalls.Length; i++)
            {
                pjsua2.AudioMedia media = conferenceCalls[i].PjAudioMedia;
                _pjAudioMediaPlayer.stopTransmit(media);
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
                    if (_pjAudioMediaPlayer != null)
                        _pjAudioMediaPlayer.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _pjAudioMediaPlayer = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~AudioMediaPlayer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
