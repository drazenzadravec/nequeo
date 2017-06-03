/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2017 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Speech.AudioFormat;

namespace Nequeo.IO.Audio
{
    /// <summary>
    /// Speech synthesizer.
    /// </summary>
    public class Speech : IDisposable
    {
        /// <summary>
        /// Speech synthesizer.
        /// </summary>
        public Speech()
        {
            _synth = new SpeechSynthesizer();
            _recon = new SpeechRecognizer();

            // Configure the audio output. 
            _synth.SetOutputToDefaultAudioDevice();
        }

        SpeechSynthesizer _synth = null;
        SpeechRecognizer _recon = null;

        /// <summary>
        /// Speak text async.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <returns>The prompt.</returns>
        public Prompt SpeakAsync(string text)
        {
            return _synth.SpeakAsync(text);
        }

        /// <summary>
        /// Speak text
        /// </summary>
        /// <param name="text">The text to speak.</param>
        public void Speak(string text)
        {
            _synth.Speak(text);
        }

        /// <summary>
        /// Pauses the speech synthesizer object.
        /// </summary>
        public void Pause()
        {
            _synth.Pause();
        }

        /// <summary>
        /// Resume the speech synthesizer object.
        /// </summary>
        public void Resume()
        {
            _synth.Resume();
        }

        /// <summary>
        /// Configures the speech synthesizer object to send output to the default audio device.
        /// </summary>
        public void SetOutputToDefaultAudioDevice()
        {
            _synth.SetOutputToDefaultAudioDevice();
        }

        /// <summary>
        /// Configures the speech synthesizer object to append output to an audio stream.
        /// </summary>
        /// <param name="audioDestination">The stream to which to append synthesis output.</param>
        /// <param name="formatInfo">The format to use for the synthesis output.</param>
        public void SetOutputToAudioStream(System.IO.Stream audioDestination, SpeechAudioFormatInfo formatInfo)
        {
            _synth.SetOutputToAudioStream(audioDestination, formatInfo);
        }

        /// <summary>
        /// Configures the speech synthesizer object to append output
        /// to a stream that contains Waveform format audio.
        /// </summary>
        /// <param name="audioDestination">The stream to which to append synthesis output.</param>
        public void SetOutputToWaveStream(System.IO.Stream audioDestination)
        {
            _synth.SetOutputToWaveStream(audioDestination);
        }

        /// <summary>
        /// Emulates input of a phrase to the shared speech recognizer, using text instead
        /// of audio for synchronous speech recognition.
        /// </summary>
        /// <param name="inputText">The input for the recognition operation.</param>
        /// <returns>The recognition result for the recognition operation, or null, if the operation
        /// is not successful or Windows Speech Recognition is in the Sleeping state.</returns>
        public RecognitionResult Recognize(string inputText)
        {
            return _recon.EmulateRecognize(inputText);
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
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
                    if (_synth != null)
                        _synth.Dispose();

                    if (_recon != null)
                        _recon.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If the stream was created internally.
                _synth = null;
                _recon = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Speech()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
