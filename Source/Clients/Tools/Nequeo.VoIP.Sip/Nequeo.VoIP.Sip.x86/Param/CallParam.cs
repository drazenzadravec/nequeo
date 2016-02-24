/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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

using Nequeo.Net.Sip;

namespace Nequeo.VoIP.Sip.Param
{
    /// <summary>
    /// Call information.
    /// </summary>
    public class CallParam : IDisposable
    {
        /// <summary>
        /// Call information.
        /// </summary>
        /// <param name="call">The current call.</param>
        /// <param name="mediaManager">The media manager.</param>
        /// <param name="recordFilename">The path and filename where the conversation is to be recorded to. Currently ".wav" is supported on all platforms.</param>
        internal CallParam(Call call, MediaManager mediaManager, string recordFilename = null)
        {
            _call = call;
            _call.OnCallState += _call_OnCallState;
            _call.OnCallMediaState += _call_OnCallMediaState;
            _call.OnDtmfDigit += _call_OnDtmfDigit;
            _callID = call.GetId();

            _mediaManager = mediaManager;
            _recordFilename = recordFilename;
            _audioMedias = new List<AudioMedia>();
            _guid = Guid.NewGuid().ToString();
        }

        private Call _call = null;
        private int _callID = 0;
        private string _guid = null;
        private Param.CallInfoParam _info = null;
        private MediaManager _mediaManager = null;
        private bool _isTransmitting = false;

        private AudioMediaPlayer _player = null;
        private AudioMediaRecorder _recorder = null;
        private AudioMediaRecorder _recorderAutoAnswer = null;
        private string _recordFilename = null;

        private List<AudioMedia> _audioMedias = null;

        /// <summary>
        /// Notify application when call state has changed.
        /// </summary>
        public event System.EventHandler<Param.CallStateParam> OnCallState;

        /// <summary>
        /// Notify application when media state in the call has changed.
        /// </summary>
        public event System.EventHandler<Param.CallMediaStateParam> OnCallMediaState;

        /// <summary>
        /// Notify application when the player has reached the end of the sound file.
        /// </summary>
        public event System.EventHandler OnPlayerEndOfFile;

        /// <summary>
        /// Notify application that the call has ended and disconnected.
        /// </summary>
        public event System.EventHandler<Param.CallInfoParam> OnCallDisconnected;

        /// <summary>
        /// Notify application upon incoming DTMF digits.
        /// </summary>
        public event System.EventHandler<Param.OnDtmfDigitParam> OnDtmfDigit;

        /// <summary>
        /// Gets the audio media list.
        /// </summary>
        internal List<AudioMedia> AudioMedia
        {
            get { return _audioMedias; }
        }

        /// <summary>
        /// Gets an idicator specifying if caller is transmitting.
        /// </summary>
        public bool IsTransmitting
        {
            get { return _isTransmitting; }
        }

        /// <summary>
        /// Gets the call id.
        /// </summary>
        public int CallID
        {
            get { return _callID; }
        }

        /// <summary>
        /// Gets the unique id.
        /// </summary>
        public string ID
        {
            get { return _guid; }
        }

        /// <summary>
        /// Gets the call information at the end of the call.
        /// </summary>
        public Param.CallInfoParam CallInfo
        {
            get { return _info; }
        }

        /// <summary>
        /// Start the conversation between callers.
        /// </summary>
        /// <param name="caller">The caller to have a conversation with.</param>
        public void StartConversation(CallParam caller)
        {
            // Combine the audio media.
            List<AudioMedia> local = new List<AudioMedia>();
            local.AddRange(_audioMedias);
            local.AddRange(caller.AudioMedia);

            // For each call.
            for (int i = 0; i < local.Count; i++)
            {
                // Get first group.
                AudioMedia mediaCall_1 = local[i];

                // For each call.
                for (int j = 0; j < local.Count; j++)
                {
                    // Get second group.
                    AudioMedia mediaCall_2 = local[j];

                    // If the two audio media are not equal.
                    if (mediaCall_1.GetPortId() != mediaCall_2.GetPortId())
                    {
                        // Allow these two calls to communicate.
                        mediaCall_1.StartTransmit(mediaCall_2);
                    }
                }
            }
        }

        /// <summary>
        /// Stop the conversationbetween callers.
        /// </summary>
        /// <param name="caller">The caller to stop the conversation with.</param>
        public void StopConversation(CallParam caller)
        {
            // Combine the audio media.
            List<AudioMedia> local = new List<AudioMedia>();
            local.AddRange(_audioMedias);
            local.AddRange(caller.AudioMedia);

            // For each call.
            for (int i = 0; i < local.Count; i++)
            {
                // Get first group.
                AudioMedia mediaCall_1 = local[i];

                // For each call.
                for (int j = 0; j < local.Count; j++)
                {
                    // Get second group.
                    AudioMedia mediaCall_2 = local[j];

                    // If the two audio media are not equal.
                    if (mediaCall_1.GetPortId() != mediaCall_2.GetPortId())
                    {
                        // Stop these two calls from communicating.
                        mediaCall_1.StopTransmit(mediaCall_2);
                    }
                }
            }
        }

        /// <summary>
        /// Start transmitting media to the caller.
        /// </summary>
        public void StartTransmitting()
        {
            // Combine the audio media.
            List<AudioMedia> local = new List<AudioMedia>();
            local.AddRange(_audioMedias);

            // For each call.
            for (int i = 0; i < local.Count; i++)
            {
                // Get first group.
                AudioMedia audioMedia = local[i];

                // Connect the call audio media to sound device.
                audioMedia.StartTransmit(_mediaManager.GetPlaybackDeviceMedia());
                _mediaManager.GetCaptureDeviceMedia().StartTransmit(audioMedia);
            }

            // Transmitting.
            _isTransmitting = true;
        }

        /// <summary>
        /// Stop transmitting media to the caller.
        /// </summary>
        public void StopTransmitting()
        {
            // Combine the audio media.
            List<AudioMedia> local = new List<AudioMedia>();
            local.AddRange(_audioMedias);

            // For each call.
            for (int i = 0; i < local.Count; i++)
            {
                // Get first group.
                AudioMedia audioMedia = local[i];

                // Connect the call audio media to sound device.
                audioMedia.StopTransmit(_mediaManager.GetPlaybackDeviceMedia());
                _mediaManager.GetCaptureDeviceMedia().StopTransmit(audioMedia);
            }

            // Transmitting.
            _isTransmitting = false;
        }

        /// <summary>
        /// Hangup the current call.
        /// </summary>
        public void Hangup()
        {
            // Create the call settings.
            CallSetting setting = new CallSetting(true);
            CallOpParam parm = new CallOpParam(true);
            setting.AudioCount = 1;
            parm.Setting = setting;
            parm.Code = StatusCode.SC_BUSY_HERE;

            if (_call != null)
            {
                // Hangup the call.
                _call.Hangup(parm);
            }
        }

        /// <summary>
        /// Answer the current call.
        /// </summary>
        public void Answer()
        {
            // Create the call settings.
            CallSetting setting = new CallSetting(true);
            CallOpParam parm = new CallOpParam(true);
            setting.AudioCount = 1;
            parm.Setting = setting;
            parm.Code = StatusCode.SC_OK;

            if (_call != null)
            {
                // Answer the call.
                _call.Answer(parm);
            }
        }

        /// <summary>
        /// Transfer the current call.
        /// </summary>
        /// <param name="destination">The URI of new target to be contacted. The URI may be in name address or addr format.</param>
        public void Transfer(string destination)
        {
            // Create the call settings.
            CallSetting setting = new CallSetting(true);
            CallOpParam parm = new CallOpParam(true);
            setting.AudioCount = 1;
            parm.Setting = setting;
            parm.Code = StatusCode.SC_OK;

            if (_call != null)
            {
                // Answer the call.
                _call.Answer(parm);
                _call.Transfer(destination, parm);
            }
        }

        /// <summary>
        /// Send DTMF digits to remote using RFC 2833 payload formats.
        /// </summary>
        /// <param name="digits">DTMF string digits to be sent.</param>
        public void DialDtmf(string digits)
        {
            if (_call != null)
            {
                // Hangup the call.
                _call.DialDtmf(digits);
            }
        }

        /// <summary>
        /// Play a sound file to the current caller.
        /// </summary>
        /// <param name="filename">The filename and path of the sound file.</param>
        /// <param name="option">Optional option flag. Application may specify PJMEDIA_FILE_NO_LOOP = 1 to prevent playback loop.</param>
        public void PlaySoundFile(string filename, uint option = 1)
        {
            if (_call != null)
            {
                try
                {
                    List<AudioMedia> audioMedias = new List<AudioMedia>();

                    // Get the call info
                    CallInfo ci = _call.GetInfo();
                    if (ci != null)
                    {
                        // Create the player.
                        _player = new AudioMediaPlayer();
                        _player.OnPlayerEndOfFile += _player_OnPlayerEndOfFile;
                        _player.CreatePlayer(filename, option);

                        // For each media.
                        for (int i = 0; i < ci.Media.Length; i++)
                        {
                            // If objects exist.
                            if (ci.Media != null && ci.Media[i] != null && _call != null)
                            {
                                // If audio type.
                                if ((ci.Media[i].Type == Nequeo.Net.Sip.MediaType.PJMEDIA_TYPE_AUDIO) &&
                                    (_call.GetMedia((uint)i) != null))
                                {
                                    // Get the audio media.
                                    AudioMedia audioMedia = (AudioMedia)_call.GetMedia((uint)i);
                                    audioMedias.Add(audioMedia);
                                }
                            }
                        }
                    }

                    // If created.
                    if (_player != null && audioMedias != null && audioMedias.Count > 0)
                    {
                        // Start playing the file to the caller.
                        _player.Start(audioMedias[0]);
                    }
                }
                catch
                {
                    try
                    {
                        if (_player != null)
                            _player.Dispose();
                    }
                    catch { }
                    _player = null;
                }
            }
        }

        /// <summary>
        /// On player end of file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _player_OnPlayerEndOfFile(object sender, bool e)
        {
            if (_player != null)
            {
                try
                {
                    // Call end of file.
                    OnPlayerEndOfFile?.Invoke(this, new EventArgs());

                    // Close the player.
                    _player.Dispose();
                }
                catch { }
                _player = null;
            }
        }

        /// <summary>
        /// Stop the sound file and cleanup.
        /// </summary>
        public void StopSoundFile()
        {
            if (_player != null)
            {
                try
                {
                    _player.Dispose();
                }
                catch { }
            }
            _player = null;
        }

        /// <summary>
        /// Start the auto answer recorder.
        /// </summary>
        /// <param name="recordFilename">The path and filename where the conversation is to be recorded to. Currently ".wav" is supported on all platforms.</param>
        public void StartAutoAnswerRecorder(string recordFilename)
        {
            if (_recorderAutoAnswer == null)
            {
                try
                {
                    // If audio device exists.
                    if (_audioMedias.Count > 0)
                    {
                        // Create the recorder.
                        _recorderAutoAnswer = new AudioMediaRecorder();
                        _recorderAutoAnswer.CreateRecorder(recordFilename, 0, 0, 0);
                        _recorderAutoAnswer.Start(_audioMedias[0]);
                    }
                    else
                    {
                        // No audio media device has been captured.
                        throw new Exception("No audio media has been detected.");
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        _recorderAutoAnswer.Dispose();
                    }
                    catch { }
                    _recorderAutoAnswer = null;
                    throw;
                }
            }
        }

        /// <summary>
        /// Stop the auto answer recorder and cleanup.
        /// </summary>
        public void StopAutoAnswerRecorder()
        {
            if (_recorderAutoAnswer != null)
            {
                try
                {
                    _recorderAutoAnswer.Dispose();
                }
                catch { }
            }
            _recorderAutoAnswer = null;
        }

        /// <summary>
        /// Notify application when media state in the call has changed.
        /// Normal application would need to implement this callback, e.g.
        /// to connect the call's media to sound device. When ICE is used,
        /// this callback will also be called to report ICE negotiation failure.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _call_OnCallMediaState(object sender, OnCallMediaStateParam e)
        {
            Nequeo.Net.Sip.CallInfo ci = e.Info;
            if (ci != null)
            {
                // For each media.
                for (int i = 0; i < ci.Media.Length; i++)
                {
                    bool recoderSet = false;

                    // If objects exist.
                    if (ci.Media != null && ci.Media[i] != null && e.CurrentCall != null)
                    {
                        // If audio type.
                        if ((ci.Media[i].Type == Nequeo.Net.Sip.MediaType.PJMEDIA_TYPE_AUDIO) &&
                            (e.CurrentCall.GetMedia((uint)i) != null))
                        {
                            // Get the audio media.
                            AudioMedia audioMedia = (AudioMedia)e.CurrentCall.GetMedia((uint)i);
                            _audioMedias.Add(audioMedia);

                            // Create the call media param.
                            CallMediaStateParam mediaState = new CallMediaStateParam();
                            mediaState.Suspend = false;
                            mediaState.CallID = ci.Id;

                            // Handle the event.
                            OnCallMediaState?.Invoke(this, mediaState);

                            // If not suspend, normal operations.
                            if (!mediaState.Suspend)
                            {
                                // Transmitting.
                                _isTransmitting = true;

                                // Connect the call audio media to sound device.
                                audioMedia.StartTransmit(_mediaManager.GetPlaybackDeviceMedia());
                                _mediaManager.GetCaptureDeviceMedia().StartTransmit(audioMedia);

                                // If recording.
                                if (!recoderSet && !String.IsNullOrEmpty(_recordFilename))
                                {
                                    // Get the capture audio device.
                                    AudioMedia audioMediaRecord = _mediaManager.GetCaptureDeviceMedia();

                                    try
                                    {
                                        // Create the recorder.
                                        _recorder = new AudioMediaRecorder();
                                        _recorder.CreateRecorder(_recordFilename, 0, 0, 0);
                                        _recorder.StartRecordingConversation(audioMediaRecord, new AudioMedia[] { audioMedia });
                                    }
                                    catch { }

                                    // Set one recorder.
                                    recoderSet = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Notify application when call state has changed.
        /// Application may then query the call info to get the
        /// detail call states by calling getInfo() function.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="e">The event parameter.</param>
        private void _call_OnCallState(object sender, OnCallStateParam e)
        {
            Nequeo.Net.Sip.CallInfo ci = e.Info;
            if (ci != null)
            {
                _info = null;
                try
                {
                    // Create the call info.
                    _info = new CallInfoParam();
                    _info.CallID = ci.Id;
                    _info.Guid = _guid;
                    _info.Contact = ci.RemoteContact;
                    _info.FromTo = ci.RemoteUri;
                    _info.Date = DateTime.Now;
                    _info.ConnectDuration = new TimeSpan(0, 0, 0, (int)ci.ConnectDuration.Seconds, (int)ci.ConnectDuration.Milliseconds);
                    _info.TotalDuration = new TimeSpan(0, 0, 0, (int)ci.TotalDuration.Seconds, (int)ci.TotalDuration.Milliseconds);
                }
                catch { _info = null; }

                Param.CallStateParam callState = new CallStateParam();
                callState.CallID = ci.Id;
                callState.State = ci.State;
                callState.CallInfo = _info;

                // If call is disconnected.
                if ((ci.State == InviteSessionState.PJSIP_INV_STATE_DISCONNECTED) ||
                    (ci.State == InviteSessionState.PJSIP_INV_STATE_NULL))
                {
                    // If current call.
                    if (e.CurrentCall != null)
                    {
                        try
                        {
                            // Cleanup the call.
                            e.CurrentCall.Dispose();
                            e.CurrentCall = null;
                        }
                        catch { }
                    }

                    // If recoder.
                    if (_recorder != null)
                    {
                        try
                        {
                            // Stop the recorder.
                            AudioMedia audioMedia = _mediaManager.GetCaptureDeviceMedia();
                            _recorder.Stop(audioMedia);
                        }
                        catch { }

                        try
                        {
                            // Cleanup the recoder.
                            _recorder.Dispose();
                            _recorder = null;
                        }
                        catch { }
                    }

                    // If auto answer recoder.
                    if (_recorderAutoAnswer != null)
                    {
                        try
                        {
                            // Cleanup the recoder.
                            _recorderAutoAnswer.Dispose();
                            _recorderAutoAnswer = null;
                        }
                        catch { }
                    }

                    // If sound player.
                    if (_player != null)
                    {
                        try
                        {
                            // Cleanup the recoder.
                            _player.Dispose();
                            _player = null;
                        }
                        catch { }
                    }

                    // Cleanup the audio media.
                    if (_audioMedias != null)
                    {
                        _audioMedias.Clear();
                        _audioMedias = null;
                    }
                }

                try
                {
                    // Handle the event.
                    OnCallState?.Invoke(this, callState);
                }
                catch { }

                // If call is disconnected.
                if ((ci.State == InviteSessionState.PJSIP_INV_STATE_DISCONNECTED) ||
                    (ci.State == InviteSessionState.PJSIP_INV_STATE_NULL))
                {
                    try
                    {
                        // Handle the event.
                        OnCallDisconnected?.Invoke(this, _info);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Notify application upon incoming DTMF digits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _call_OnDtmfDigit(object sender, Nequeo.Net.Sip.OnDtmfDigitParam e)
        {
            Param.OnDtmfDigitParam param = new OnDtmfDigitParam();
            param.Digit = e.Digit;

            Nequeo.Net.Sip.CallInfo ci = e.Info;
            if (ci != null)
            {
                param.From = ci.RemoteUri;
                param.FromContact = ci.RemoteContact;
            }

            try
            {
                // Handle the event.
                OnDtmfDigit?.Invoke(this, param);
            }
            catch { }
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
                    // If current call.
                    if (_call != null)
                    {
                        try
                        {
                            // Cleanup the call.
                            _call.Dispose();
                        }
                        catch { }
                    }

                    // If recoder.
                    if (_recorder != null)
                    {
                        try
                        {
                            // Cleanup the recoder.
                            _recorder.Dispose();
                        }
                        catch { }
                    }

                    // If auto answer recoder.
                    if (_recorderAutoAnswer != null)
                    {
                        try
                        {
                            // Cleanup the recoder.
                            _recorderAutoAnswer.Dispose();
                        }
                        catch { }
                    }

                    // If sound player.
                    if (_player != null)
                    {
                        try
                        {
                            // Cleanup the recoder.
                            _player.Dispose();
                        }
                        catch { }
                    }

                    // Cleanup the audio media.
                    if (_audioMedias != null)
                    {
                        _audioMedias.Clear();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _player = null;
                _recorder = null;
                _recorderAutoAnswer = null;
                _audioMedias = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~CallParam()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
