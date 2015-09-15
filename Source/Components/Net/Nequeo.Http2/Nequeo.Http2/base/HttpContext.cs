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
using System.Text.RegularExpressions;

using Nequeo.Model;
using Nequeo.Model.Message;
using Nequeo.Net.Http2.Protocol;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Http server context app domain host.
    /// </summary>
    public sealed class HttpContextHost : MarshalByRefObject
    {
        /// <summary>
        /// Http server context app domain host.
        /// </summary>
        /// <param name="context">The http context.</param>
        public HttpContextHost(HttpContext context)
        {
            _context = context;
        }

        private HttpContext _context = null;

        /// <summary>
        /// Gets the http context.
        /// </summary>
        public HttpContext Context
        {
            get { return _context; }
        }
    }

    /// <summary>
    /// Http server context.
    /// </summary>
    public sealed class HttpContext : Nequeo.Net.WebContext, IDisposable
    {
        /// <summary>
        /// Http server context.
        /// </summary>
        /// <param name="requestInput">The request input stream containing the raw data.</param>
        /// <param name="responseOutput">The response output stream containing the raw data.</param>
        public HttpContext(System.IO.Stream requestInput, System.IO.Stream responseOutput)
        {
            // Create the initial settings.
            _settings = new SettingsPair[6];
            _settings[0] = new SettingsPair(SettingsRegistry.Header_Table_Size, 4096);
            _settings[1] = new SettingsPair(SettingsRegistry.Enable_Push, 1);
            _settings[2] = new SettingsPair(SettingsRegistry.Max_Concurrent_Streams, Constants.DefaultMaxConcurrentStreams);
            _settings[3] = new SettingsPair(SettingsRegistry.Initial_Window_Size, Constants.InitialFlowControlWindowSize);
            _settings[4] = new SettingsPair(SettingsRegistry.Max_Frame_Size, Constants.MaxFramePayloadSize);
            _settings[5] = new SettingsPair(SettingsRegistry.Max_Header_List_Size, Int32.MaxValue);

            // Create the stream collections.
            _contextStreamDictionary = new Collections.CustomDictionary<int, ContextStream>();
            _promisedResources = new Dictionary<int, string>();

            // Set the request and response streams.
            Input = requestInput;
            Output = responseOutput;
        }

        /// <summary>
        /// Create the http context from the web context.
        /// </summary>
        /// <param name="webContext">The web context to create from.</param>
        /// <param name="requestInput">The request input stream containing the raw data.</param>
        /// <param name="responseOutput">The response output stream containing the raw data.</param>
        /// <returns>The http server context.</returns>
        public static HttpContext CreateFrom(Nequeo.Net.WebContext webContext, System.IO.Stream requestInput, System.IO.Stream responseOutput)
        {
            Nequeo.Net.Http2.HttpContext httpContext = new Nequeo.Net.Http2.HttpContext(requestInput, responseOutput);
            httpContext.Context = webContext.Context;
            httpContext.IsStartOfConnection = webContext.IsStartOfConnection;
            httpContext.IsAuthenticated = webContext.IsAuthenticated;
            httpContext.IsSecureConnection = webContext.IsSecureConnection;
            httpContext.Name = webContext.Name;
            httpContext.NumberOfClients = webContext.NumberOfClients;
            httpContext.Port = webContext.Port;
            httpContext.RemoteEndPoint = webContext.RemoteEndPoint;
            httpContext.ServerEndPoint = webContext.ServerEndPoint;
            httpContext.ServiceName = webContext.ServiceName;
            httpContext.UniqueIdentifier = webContext.UniqueIdentifier;
            httpContext.ConnectionID = webContext.ConnectionID;
            httpContext.SessionID = webContext.SessionID;
            httpContext.User = webContext.User;
            httpContext.SocketState = webContext.SocketState;
            httpContext.IsAsyncMode = webContext.IsAsyncMode;
            return httpContext;
        }

        private object _lockResponse = new object();

        private int _currentStreamId = 0;
        private bool _usePriorities = false;
        private bool _useFlowControl = false;

        private readonly Dictionary<int, string> _promisedResources = null;
        private readonly SettingsPair[] _settings = null;
        private readonly Nequeo.Collections.CustomDictionary<int, ContextStream> _contextStreamDictionary = null;

        private const string _lowerCasePattern = "^((?![A-Z]).)*$";
        private readonly Regex _matcher = new Regex(_lowerCasePattern);

        /// <summary>
        /// Gets or sets the session window size.
        /// </summary>
        internal int SessionWindowSize { get; set; }

        /// <summary>
        /// Gets or sets an indicator specifying if priorities should be used.
        /// </summary>
        internal bool UsePriorities 
        {
            get { return _usePriorities; }
            set { _usePriorities = value; }
        }

        /// <summary>
        /// Gets or sets an indicator specifying if flow control should be used.
        /// </summary>
        internal bool UseFlowControl
        {
            get { return _useFlowControl; }
            set { _useFlowControl = value; }
        }

        /// <summary>
        /// Gets or sets the request input stream.
        /// </summary>
        internal System.IO.Stream Input
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the response output stream.
        /// </summary>
        internal System.IO.Stream Output
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the promised resources.
        /// </summary>
        internal Dictionary<int, string> PromisedResources
        {
            get { return _promisedResources; }
        }

        /// <summary>
        /// Gets the current stream id that is in the request queue.
        /// </summary>
        public int StreamId 
        {
            get { return _currentStreamId; }
            internal set { _currentStreamId = value; }
        }

        /// <summary>
        /// Gets the request settings.
        /// </summary>
        public SettingsPair[] Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Gets the context stream collection.
        /// </summary>
        internal Nequeo.Collections.CustomDictionary<int, ContextStream> ContextStreams
        {
            get { return _contextStreamDictionary; }
        }

        /// <summary>
        /// Write to the response output stream.
        /// </summary>
        /// <param name="data">The data to write.</param>
        internal void ResponseWrite(byte[] data)
        {
            // Lock the response output stream.
            lock(_lockResponse)
            {
                if (data != null && data.Length > 0)
                {
                    // Write the data.
                    Output.Write(data, 0, data.Length);
                }
            }
        } 

        /// <summary>
        /// Get all current http streams
        /// </summary>
        /// <returns>The collection of http streams.</returns>
        public HttpStream[] GetHttpStreams()
        {
            int index = 0;
            HttpStream[] streams = new HttpStream[_contextStreamDictionary.Count];

            // For each context stream.
            foreach (KeyValuePair<int, ContextStream> stream in _contextStreamDictionary)
            {
                // Assign the http stream.
                streams[index] = (HttpStream)stream.Value;
                index++;
            }

            // Return the streams
            return streams;
        }

        /// <summary>
        /// Get the current http stream for the stream id.
        /// </summary>
        /// <param name="streamId">The current stream id that is in the request queue.</param>
        /// <returns>The http stream if found; else null.</returns>
        public HttpStream GetHttpStream(int streamId)
        {
            ContextStream stream;
            if (_contextStreamDictionary.TryGetValue(streamId, out stream))
            {
                // Return the http stream from the context stream.
                return (HttpStream)stream;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets or creates a new stream for the dictionary.
        /// </summary>
        /// <param name="streamId">The stream id.</param>
        /// <returns>The current stream.</returns>
        internal ContextStream GetStream(int streamId)
        {
            ContextStream stream;
            if (!_contextStreamDictionary.TryGetValue(streamId, out stream))
            {
                // Get the max stream concurrent streams
                SettingsPair max = _settings[2];

                // If more streams can be created.
                if (_contextStreamDictionary.Count < max.Value)
                {
                    // Try to create the stream.
                    stream = new ContextStream(streamId, this);
                    stream.Opened = true;

                    // Add the stream to the connection.
                    _contextStreamDictionary.Add(streamId, stream);
                    return stream;
                }
                else
                    return null;
            }
            return stream;
        }

        /// <summary>
        /// Remove the stream from the collection.
        /// </summary>
        /// <param name="streamId">The strream id.</param>
        internal void RemoveStream(int streamId)
        {
            ContextStream stream;
            if (_contextStreamDictionary.TryGetValue(streamId, out stream))
            {
                try
                {
                    // Release resources.
                    stream.Dispose();
                }
                catch { }
            }
            _contextStreamDictionary.Remove(streamId);
        }

        /// <summary>
        /// Validate all headers.
        /// </summary>
        /// <param name="stream">The current stream.</param>
        internal void ValidateHeaders(ContextStream stream)
        {
            /* 12 -> 8.1.3
            Header field names MUST be converted to lowercase prior to their 
            encoding in HTTP/2.0. A request or response containing uppercase 
            header field names MUST be treated as malformed. */
            foreach (NameValue header in stream.HttpRequest.OriginalHeaders)
            {
                string key = header.Name;
                if (!_matcher.IsMatch(key) || key == ":")
                {
                    // Clear all headers found from the list.
                    stream.HttpRequest.HeadersFound = false;
                    stream.HttpRequest.OriginalHeaders.Clear();

                    // Write to the client indicating that the 
                    // headers are incorrect.
                    Utility.ProcessCloseRstStreamFrame(this, ErrorCodeRegistry.Refused_Stream, stream.StreamId);
                    Utility.ProcessClose(this, ErrorCodeRegistry.Refused_Stream, stream);
                    break;
                }
            }
        }

        /// <summary>
        /// Set the settings value internally.
        /// </summary>
        /// <param name="id">The setting id.</param>
        /// <param name="value">The setting value.</param>
        internal void SetSettingsPair(SettingsRegistry id, int value)
        {
            // For each setting.
            for (int i = 0; i < _settings.Length; i++)
            {
                // Get the item.
                SettingsPair item = _settings[i];

                // If the id has been found.
                if (id == item.Id)
                {
                    // Set the new value.
                    _settings[i].Value = value;
                }
            }
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
                    // Release the context stream resources.
                    for (int i = 0; i < _contextStreamDictionary.Count; i++)
                    {
                        try
                        {
                            _contextStreamDictionary[i].Dispose();
                            _contextStreamDictionary[i] = null;
                        }
                        catch { }
                    }

                    // Dispose managed resources.
                    if (_contextStreamDictionary != null)
                        _contextStreamDictionary.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _lockResponse = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~HttpContext()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
