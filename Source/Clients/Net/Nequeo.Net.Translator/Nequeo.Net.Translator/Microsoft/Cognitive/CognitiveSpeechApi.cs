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
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Translator.Microsoft.Cognitive
{
    /// <summary>
    /// Microsoft cognitive speech api translator.
    /// </summary>
    public sealed class SpeechApi : IDisposable
    {
        /// <summary>
        /// Microsoft api translator.
        /// </summary>
        public SpeechApi()
        {
            // Gets the microsoft translator service URI.
            _service = new Uri(Nequeo.Net.Translator.Properties.Settings.Default.MicrosoftTranslatorServiceURI_WSV1);
        }

        /// <summary>
        /// Microsoft api translator.
        /// </summary>
        /// <param name="service">An absolute URI that identifies the root of a data service.</param>
        public SpeechApi(Uri service)
        {
            _service = service;
        }

        private Uri _service = null;
        private Uri _serviceToken = null;
        private NetworkCredential _credentials = null;

        private string _osPlatform = "Windows 10";
        private string _clientVersion = "2016.26.1.1";
        private string _xCorrelationId = "Hq106Z7w8fR90Y2rJ4d7";
        private string _clientTraceId = null;

        private short _waveChannels = 1;
        private int _waveSampleRate = 8000;
        private short _waveBitsPerSample = 16;
        private int _bufferMilliseconds = 100;

        private object _syncObject = new object();

        private Nequeo.Net.WebSocketClient _client = null;
        private Nequeo.IO.Audio.Device _device;
        private Nequeo.IO.Audio.WaveRecorder _recorder = null;
        private Nequeo.IO.Stream.StreamBufferBase _audioBuffer = null;
        private System.IO.Stream _writeStream = null;

        /// <summary>
        /// A connection has been established and audio recording has started.
        /// </summary>
        public event EventHandler OnRecording;

        /// <summary>
        /// A connection has been closed and audio recording has stopped.
        /// </summary>
        public event EventHandler OnStopRecording;

        /// <summary>
        /// When a translation response has been received.
        /// </summary>
        public event EventHandler OnTranslationReceived;

        /// <summary>
        /// Gets or sets the translator service.
        /// </summary>
        public Uri Service
        {
            get { return _service; }
            set { _service = value; }
        }

        /// <summary>
        /// Gets or sets the network credentials used to access the service (the api key for the username and password).
        /// </summary>
        public NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        /// <summary>
        /// Gets or sets the audio device to use for recording.
        /// </summary>
        public Nequeo.IO.Audio.Device AudioDevice
        {
            get { return _device; }
            set { _device = value; }
        }

        /// <summary>
        /// Gets or sets milliseconds for the buffer. Recommended value is 100ms
        /// </summary>
        public int BufferMilliseconds
        {
            get { return _bufferMilliseconds; }
            set { _bufferMilliseconds = value; }
        }

        /// <summary>
        /// Gets or sets the stream to write the translated data to.
        /// </summary>
        public System.IO.Stream WriteStream
        {
            get { return _writeStream; }
            set { _writeStream = value; }
        }

        /// <summary>
        /// Gets or sets the Identifies the name and version of the 
        /// operating system the client application is running on. 
        /// Examples: "Android 5.0", "iOs 8.1.3", "Windows 8.1".
        /// </summary>
        public string OsPlatform
        {
            get { return _osPlatform; }
            set { _osPlatform = value; }
        }

        /// <summary>
        /// Gets or sets A client-generated GUID used to trace a request. 
        /// For proper troubleshooting of issues, clients should provide 
        /// a new value with each request and log it.
        /// </summary>
        public string ClientTraceId
        {
            get { return _clientTraceId; }
            set { _clientTraceId = value; }
        }

        /// <summary>
        /// Gets or sets the Identifies the version of the 
        /// client application. Example: "2.1.0.123".
        /// </summary>
        public string ClientVersion
        {
            get { return _clientVersion; }
            set { _clientVersion = value; }
        }

        /// <summary>
        /// Gets or sets A client-generated identifier used to correlate 
        /// multiple channels in a conversation. Multiple speech translation 
        /// sessions can be created to enable conversations between users. 
        /// In such scenario, all speech translation sessions use the same 
        /// correlation ID to tie the channels together. This facilitates 
        /// tracing and diagnostics. The identifier should conform 
        /// to: ^[a-zA-Z0-9-_.]{1,64}$
        /// </summary>
        public string CorrelationId
        {
            get { return _xCorrelationId; }
            set { _xCorrelationId = value; }
        }

        /// <summary>
        /// Start translating the speech to text.
        /// </summary>
        /// <param name="to">Specifies the language to translate the transcribed text into. The value is one of the language identifiers from the text scope in the response from the Languages API.</param>
        /// <param name="from">Specifies the language of the incoming speech. The value is one of the language identifiers from the speech scope in the response from the Languages API.</param>
        /// <param name="authorization">Specifies the value of the client's bearer token. Use the prefix Bearer followed by the value of the access_token value returned by the authentication token service.</param>
        /// <param name="features">Comma-separated set of features selected by the client. Available features include:
        /// TextToSpeech: specifies that the service must return the translated audio of the final translated sentence.
        /// Partial: specifies that the service must return intermediate recognition results while the audio is streaming to the service.
        /// TimingInfo: specifies that the service must return timing information associated with each recognition.
        /// As an example, a client would specify features= partial, texttospeech to receive partial results and text-to-speech, but no timing information.Note that final results are always streamed to the client.
        /// </param>
        /// <param name="voice">Identifies what voice to use for text-to-speech rendering of the translated text. The value is one of the voice identifiers from the tts scope in the response from the Languages API. If a voice is not specified the system will automatically choose one when the text-to-speech feature is enabled.</param>
        /// <param name="format">Specifies the format of the text-to-speech audio stream returned by the service. Available options are:
        /// audio/wav: Waveform audio stream.
        /// audio/mp3: MP3 audio stream.
        /// Default is audio/wav.
        /// </param>
        /// <param name="profanityAction">Specifies how the service should handle profanities recognized in the speech. Valid actions are:
        /// NoAction: Profanities are left as is.
        /// Marked: Profanities are replaced with a marker.See ProfanityMarker parameter.
        /// Deleted: Profanities are deleted.For example, if the word "jackass" is treated as a profanity, the phrase "He is a jackass." will become "He is a .".
        /// The default is Marked.
        /// </param>
        /// <param name="profanityMarker">Specifies how detected profanities are handled when ProfanityAction is set to Marked. Valid options are: 
        /// Asterisk: Profanities are replaced with the string***. For example, if the word "jackass" is treated as a profanity, the phrase "He is a jackass." will become "He is a *.".
        /// Tag: Profanity are surrounded by a profanity XML tag.For example, if the word "jackass" is treated as a profanity, the phrase "He is a jackass." will become "He is a jackass.".
        /// The default is Asterisk.
        /// </param>
        /// <param name="accessToken">Alternate way to pass a valid OAuth access token. The bearer token is usually provided with header Authorization. Some websocket libraries do not allow client code to set headers. In such case, the client can use the access_token query parameter to pass a valid token. If Authorization header is not set, then access_token must be set. If both header and query parameter are set, then the query parameter is ignored. Clients should only use one method to pass the token.</param>
        /// <param name="apiVersion">Version of the API requested by the client. Allowed values are: 1.0.</param>
        public void Translate(string to, string from, string authorization, string features = null, string voice = null, string format = null,
            string profanityAction = null, string profanityMarker = null, string accessToken = null, string apiVersion = "1.0")
        {
            string queryString = "";

            if ((from != null))
            {
                queryString += "&from=" + System.Uri.EscapeDataString(from);
            }
            if ((to != null))
            {
                queryString += "&to=" + System.Uri.EscapeDataString(to);
            }
            if ((features != null))
            {
                queryString += "&features=" + System.Uri.EscapeDataString(features);
            }
            if ((voice != null))
            {
                queryString += "&voice=" + System.Uri.EscapeDataString(voice);
            }
            if ((format != null))
            {
                queryString += "&format=" + System.Uri.EscapeDataString(format);
            }
            if ((profanityAction != null))
            {
                queryString += "&profanityAction=" + System.Uri.EscapeDataString(profanityAction);
            }
            if ((profanityMarker != null))
            {
                queryString += "&profanityMarker=" + System.Uri.EscapeDataString(profanityMarker);
            }
            if ((accessToken != null))
            {
                queryString += "&access_token=" + System.Uri.EscapeDataString(accessToken);
            }
            if ((apiVersion != null))
            {
                queryString += "&api-version=" + System.Uri.EscapeDataString(apiVersion);
            }

            ProcessWebSocketRequest("/" + "translate", queryString, authorization);
        }

        /// <summary>
        /// Stop the translation process.
        /// </summary>
        public void StopTranslate()
        {
            if (_recorder != null)
                _recorder.Stop();

            if (_client != null)
                _client.Close();
        }

        /// <summary>
        /// Get the access token.
        /// </summary>
        /// <param name="serviceToken">The service token root URI.</param>
        /// <param name="serviceExtension">The service extension ([serviceRoot]/issueToken).</param>
        /// <returns>The Bearer + access token.</returns>
        public string GetAccessToken(Uri serviceToken, string serviceExtension = "issueToken")
        {
            // Set the root service token URI.
            _serviceToken = serviceToken;

            // Make the request.
            byte[] accessToken = ProcessRequest("/" + serviceExtension, null, new NetRequest(), _credentials.UserName, "POST", false);
            string accessTokenBearer = Encoding.Default.GetString(accessToken);
            return "Bearer " + accessTokenBearer;
        }

        /// <summary>
        /// Translate the write stream data to speech translation mode.
        /// </summary>
        /// <returns>The speech translation model.</returns>
        public SpeechTranslation GetSpeechTranslation()
        {
            SpeechTranslation speech = new SpeechTranslation();

            // If data has been eritten to the write stream.
            if (_writeStream != null && _writeStream.Length > 0)
            {
                // Got the begining of the stream.
                _writeStream.Seek(0, System.IO.SeekOrigin.Begin);

                // Read the data.
                byte[] buffer = new byte[(int)_writeStream.Length];
                int read = _writeStream.Read(buffer, 0, (int)_writeStream.Length);

                // Create the object from the json data.
                string json = Encoding.Default.GetString(buffer);
                speech = Nequeo.Serialisation.JavaObjectNotation.Deserializer<SpeechTranslation>(json);
            }

            // Return the translation.
            return speech;
        }

        /// <summary>
        /// Process a websocket request.
        /// </summary>
        /// <param name="serviceName">The service name endpoint.</param>
        /// <param name="query">The query to sent with the request.</param>
        /// <param name="authorization">The query to sent with the request.</param>
        private void ProcessWebSocketRequest(string serviceName, string query, string authorization)
        {
            // Construct the URI.
            Uri serviceUri = new Uri(
                _service.AbsoluteUri.TrimEnd(new char[] { '/' }) +
                (String.IsNullOrEmpty(serviceName) ? "" : serviceName) +
                (String.IsNullOrEmpty(query) ? "" : "?" + query.TrimStart(new char[] { '&' })));

            // Create websocket headers.
            Nequeo.Model.NameValue[] headers = new Model.NameValue[]
            {
                new Model.NameValue() { Name = "Authorization", Value = authorization },
                new Model.NameValue() { Name = "X-ClientTraceId", Value = (!String.IsNullOrEmpty(_clientTraceId) ? _clientTraceId : Guid.NewGuid().ToString()) },
                new Model.NameValue() { Name = "X-ClientVersion", Value = _clientVersion },
                new Model.NameValue() { Name = "X-OsPlatform", Value = _osPlatform },
                new Model.NameValue() { Name = "X-CorrelationId", Value = _xCorrelationId }
            };

            // Release the client.
            if (_client != null)
                _client.Dispose();

            _client = null;

            // Make a request to the server.
            _client = Nequeo.Net.WebSocketClient.Request(
                serviceUri, (d, r) => ReceiveHandler(d, r), headers, 
                (o, e) => OnConnected(o, e), (o, e) => OnDisconnected(o, e));
        }

        /// <summary>
        /// On receiving data handler.
        /// </summary>
        /// <param name="data">The data received.</param>
        /// <param name="result">The received result.</param>
        private void ReceiveHandler(byte[] data, System.Net.WebSockets.WebSocketReceiveResult result)
        {
            // If data exists.
            if (data != null && data.Length > 0)
            {
                // Write the buffer.
                _writeStream.Write(data, 0, result.Count);
            }

            // Is the end of the data.
            if (result.EndOfMessage)
            {
                // Got translation.
                OnTranslationReceived?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// On connection open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnected(object sender, EventArgs e)
        {
            // Create the recorder and audio buffer.
            _recorder = new IO.Audio.WaveRecorder(_device);
            _recorder.DataAvailable += _recorder_DataAvailable;
            _recorder.RecordingStopped += _recorder_RecordingStopped;
            _recorder.BufferMilliseconds = _bufferMilliseconds;

            // Open the audio buffer.
            _audioBuffer = new IO.Stream.StreamBufferBase();
            _recorder.Open(_audioBuffer, new IO.Audio.Wave.WaveFormatProvider(_waveSampleRate, _waveBitsPerSample, _waveChannels), IO.Audio.AudioRecordingFormat.WaveInEvent);

            // Create a default audio sample rate.
            // Write the first header data to the buffer.
            Nequeo.IO.Audio.WaveStructure waveStructure = Nequeo.IO.Audio.WaveStructure.CreateDefaultStructure(_waveChannels, _waveSampleRate, _waveBitsPerSample, new byte[0]);
            Nequeo.IO.Audio.WaveFormat waveFormat = new Nequeo.IO.Audio.WaveFormat();
            bool ret = waveFormat.Write(_audioBuffer, waveStructure);

            // Get the number of wave header bytes.
            long headerSize = _audioBuffer.Length;

            // Lock the client
            lock (_syncObject)
            {
                byte[] buffer = new byte[(int)headerSize];
                int read = _audioBuffer.Read(buffer, 0, (int)headerSize);

                // Send the header data to the speech server.
                _client.Send(buffer.Take(read).ToArray(), false);
            }

            // Start recording.
            _recorder.Start();
            OnRecording?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Recording has stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _recorder_RecordingStopped(object sender, IO.Audio.StoppedEventArgs e)
        {
            // Lock the client
            lock (_syncObject)
            {
                // Get the number of bytes left in the buffer.
                long length = _audioBuffer.Length;

                // Read audio data in the buffer.
                byte[] buffer = new byte[(int)length];
                int read = _audioBuffer.Read(buffer, 0, (int)length);

                // Send the header data to the speech server.
                _client.Send(buffer.Take(read).ToArray(), true);

                // Stopped.
                OnStopRecording?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Audio data is available.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The number of bytes recorded.</param>
        private void _recorder_DataAvailable(object sender, long e)
        {
            // Audio recording data exists.
            if (e > 0)
            {
                // Lock the client
                lock (_syncObject)
                {
                    // Read audio data in the buffer.
                    byte[] buffer = new byte[(int)e];
                    int read = _audioBuffer.Read(buffer, 0, (int)e);

                    // Send the header data to the speech server.
                    _client.Send(buffer.Take(read).ToArray(), false);
                }
            }
        }

        /// <summary>
        /// On connection closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisconnected(object sender, EventArgs e)
        {
            if (_recorder != null)
                _recorder.Close();

            if (_audioBuffer != null)
                _audioBuffer.Dispose();

            if (_recorder != null)
                _recorder.Dispose();

            _audioBuffer = null;
            _recorder = null;
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="serviceName">The service name to call.</param>
        /// <param name="query">The query to apply.</param>
        /// <param name="request">The request.</param>
        /// <param name="apiKey">The client access api key.</param>
        /// <param name="method">The request method.</param>
        /// <param name="hasAccessToken">Does the request have an access token.</param>
        /// <returns>The returned type.</returns>
        private byte[] ProcessRequest(string serviceName, string query, Net.NetRequest request, 
            string apiKey = null, string method = "GET", bool hasAccessToken = true)
        {
            // Construct the URI.
            Uri constructedServiceUri = new Uri(
                _serviceToken.AbsoluteUri.TrimEnd(new char[] { '/' }) +
                (String.IsNullOrEmpty(serviceName) ? "" : serviceName) +
                (String.IsNullOrEmpty(query) ? "" : "?" + query.TrimStart(new char[] { '&' })));

            // Create the request.
            request.Method = method;
            request.AddHeader("Content-Length", (0).ToString());

            // If requesting an access token.
            if (!hasAccessToken)
            {
                request.AddHeader("Ocp-Apim-Subscription-Key", apiKey);
            }

            // Return the result.
            return Nequeo.Net.HttpDataClient.Request(constructedServiceUri, request);
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
        private void Dispose(bool disposing)
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
                    if (_audioBuffer != null)
                        _audioBuffer.Dispose();

                    if (_recorder != null)
                        _recorder.Dispose();

                    if (_client != null)
                        _client.Dispose();
                }

                _audioBuffer = null;
                _recorder = null;
                _client = null;
                _syncObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SpeechApi()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
