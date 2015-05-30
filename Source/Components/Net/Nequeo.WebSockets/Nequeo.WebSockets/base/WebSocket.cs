/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Model;
using Nequeo.Extension;
using Nequeo.IO.Compression;
using Nequeo.Net.WebSockets.Protocol;
using Nequeo.IO.Stream.Extension;
using Nequeo.Net.WebSockets.Common;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// The WebSocket class allows applications to send and receive data after the
    /// WebSocket upgrade has completed.
    /// </summary>
    public class WebSocket : IDisposable
    {
        #region Constructors
        /// <summary>
        /// The WebSocket class allows applications to send and receive data after the
        /// WebSocket upgrade has completed.
        /// </summary>
        /// <param name="context">Web socket server context.</param>
        /// <param name="protocol">The web socket protocol version.</param>
        internal WebSocket(CentralWebSocketContext context, string protocol)
        {
            _context = context;
            _protocol = protocol;

            _client = false;
            Init();

            _readStream = _context.Context.WebRequest.Input;
            _writeStream = _context.Context.WebResponse.Output;
        }

        /// <summary>
        /// The WebSocket class allows applications to send and receive data after the
        /// WebSocket upgrade has completed.
        /// </summary>
        /// <param name="context">Web socket client context.</param>
        /// <param name="protocol">The web socket protocol version.</param>
        internal WebSocket(CentralWebSocketNetContext context, string protocol)
        {
            _contextClient = context;
            _protocol = protocol;

            _client = true;
            Init();

            _readStream = _contextClient.Context.NetResponse.Input;
            _writeStream = _contextClient.Context.NetRequest.Output;
        }
        #endregion

        #region Private Fields
        private bool _client = false;

        private Stream _readStream = null;
        private Stream _writeStream = null;

        private Queue<MessageEventArgs> _messageEventQueue;
        private ReceiveResultWebContext _receiveResult = null;

        private TimeSpan _waitTime;
        private string _protocol = null;
        private CentralWebSocketContext _context = null;
        private CentralWebSocketNetContext _contextClient = null;
        private CompressionAlgorithmStreaming _compression = CompressionAlgorithmStreaming.None;
        
        private volatile WebSocketConnectionState _readyState = WebSocketConnectionState.Closed;

        // Max value is Int32.MaxValue - 14.
        private const int FragmentLength = 1016;

        private object _lockConnect = new object();
        private object _lockReceive = new object();
        private object _lockEnqueue = null;

        private AutoResetEvent _receivePong = null;
        private AutoResetEvent _exitReceiving = null;
        private AutoResetEvent _recevedFrame = null;
        private AutoResetEvent _frameReturnedComplete = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the current state of the WebSocket connection.
        /// </summary>
        public WebSocketConnectionState State
        {
            get { return _readyState; }
        }

        /// <summary>
        /// Gets or sets the web socket protocol version.
        /// </summary>
        public string Protocol
        {
            get { return _protocol; }
        }

        /// <summary>
        /// Gets or sets the compression algorithm to use when sending data.
        /// </summary>
        public CompressionAlgorithmStreaming Compression
        {
            get { return _compression; }
            set { _compression = value; }
        }

        /// <summary>
        /// Gets or sets the wait time for the response to the Ping or Close request.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return _waitTime; }
            set { _waitTime = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Closes the WebSocket connection using the close handshake defined in the
        /// WebSocket protocol specification section 7.
        /// </summary>
        /// <param name="closeStatus">Indicates the reason for closing the WebSocket connection.</param>
        /// <param name="statusDescription">Specifies a human readable explanation as to why the connection is closed.</param>
        /// <param name="cancellationToken">The token that can be used to propagate notification that operations should be canceled.</param>
        /// <returns>Returns System.Threading.Tasks.Task.</returns>
        public Task CloseAsync(CloseCodeStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            // Create a new task.
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    // Check can be closed.
                    var msg = _readyState.CheckIfClosable() ?? closeStatus.CheckIfValidCloseParameters(statusDescription);
                    if (!String.IsNullOrEmpty(msg))
                    {
                        return;
                    }

                    // If cancellation has not been requested.
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // If no status code.
                        if (closeStatus.IsNoStatusCode())
                        {
                            // Close the connection.
                            Close(new CloseEventArgs(), true, true);
                            return;
                        }

                        // Close the connection.
                        var send = !closeStatus.IsReserved();
                        Close(new CloseEventArgs(closeStatus, statusDescription), send, send);
                    }
                }
                catch { }
                
            }, cancellationToken);
        }

        /// <summary>
        /// Sends data over the WebSocket connection asynchronously.
        /// </summary>
        /// <param name="buffer">The buffer to be sent over the connection.</param>
        /// <param name="messageType">Indicates whether the application is sending a binary or text message.</param>
        /// <param name="cancellationToken">The token that propagates the notification that operations should be canceled.</param>
        /// <returns>Returns System.Threading.Tasks.Task.</returns>
        public Task SendAsync(ArraySegment<byte> buffer, MessageType messageType, CancellationToken cancellationToken)
        {
            // Create a new task.
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    // Select the message type to send.
                    switch (messageType)
                    {
                        case MessageType.Close:
                            CloseOpCodeFrame(buffer, cancellationToken);
                            break;

                        case MessageType.Ping:
                            PingOpCodeFrame(buffer, cancellationToken);
                            break;

                        case MessageType.Text:
                            TextOpCodeFrame(buffer, cancellationToken);
                            break;

                        case MessageType.Binary:
                        default:
                            BinaryOpCodeFrame(buffer, cancellationToken);
                            break;
                    }
                }
                catch { }

            }, cancellationToken);
        }

        /// <summary>
        /// Receives data from the WebSocket connection asynchronously.
        /// </summary>
        /// <param name="buffer">References the application buffer that is the storage location for the received data.</param>
        /// <param name="cancellationToken">Propagate the notification that operations should be canceled.</param>
        /// <returns>Returns System.Threading.Tasks.Task.</returns>
        public Task<ReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            // Create a new task.
            return Task<ReceiveResult>.Factory.StartNew(() =>
            {
                bool notWait = false;
                ReceiveResult result = null;

                // Lock until complete.
                lock (_lockReceive)
                {
                    // Should the process wait.
                    notWait = _receiveResult.ReturnComplete;
                }

                // If more data needs to be returned
                // then wait until complete.
                if (!notWait)
                    _recevedFrame.WaitOne();

                // Lock until complete.
                lock (_lockReceive)
                {
                    try
                    {
                        // Denqueue the result.
                        result = _receiveResult.Result;
                        byte[] data = _receiveResult.Data;
                        int dataOffet = _receiveResult.Offset;

                        // Result exists.
                        if (result != null)
                        {
                            // If data exists.
                            if (data != null && buffer != null)
                            {
                                // Get the current offset positions
                                // to start reading from.
                                int left = data.Length - dataOffet;
                                int arraySize = buffer.Array.Length;
                                int end = arraySize >= left ? left : arraySize;

                                // Assign the current set of data
                                // into the buffer array.
                                for (int i = 0; i < end; i++)
                                    buffer.Array[i] = data[i + dataOffet];

                                // Set the data offset position.
                                _receiveResult.Offset = dataOffet + end;

                                // If no more to send to the caller.
                                if(_receiveResult.Offset >= data.Length)
                                {
                                    // Let the async mode continue.
                                    _receiveResult.ReturnComplete = false;
                                    _frameReturnedComplete.Set();
                                }
                                else
                                {
                                    // Run this method again to get the next set  of data.
                                    _receiveResult.ReturnComplete = true;
                                }
                            }
                        }
                    }
                    catch 
                    {
                        // Let the async mode continue.
                        _receiveResult.ReturnComplete = false;
                        _frameReturnedComplete.Set();
                    }
                }

                // Return the result.
                return result;

            }, cancellationToken);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Async mode receiver, triggered each time data arrives.
        /// </summary>
        private void AsyncModeReceiver()
        {
            bool currentComplete = false;

            // Lock until complete.
            lock (_lockReceive)
            {
                // Has the current frame message been returned.
                currentComplete = _receiveResult.ReturnComplete;
            }

            // If not complete.
            if (!currentComplete)
                _frameReturnedComplete.WaitOne();

            // Process the received data and create the receive frame.
            Frame.Read(_readStream, false, f => ReceiveFrame(f), ex => ReceiveFrameException(ex));
        }

        /// <summary>
        /// Process receive frame.
        /// </summary>
        /// <param name="frame">The current frame.</param>
        private void ReceiveFrame(Frame frame)
        {
            // Process the frame that was received.
            if (ProcessReceivedFrame(frame) && _readyState != WebSocketConnectionState.Closed)
            {
                // If is not data frame.
                if (!frame.IsData)
                    return;

                // Lock until complete.
                lock (_lockReceive)
                {
                    try
                    {
                        // Is data frame.
                        MessageEventArgs messsage = DequeueFromMessageEventQueue();
                        if (messsage != null && _readyState == WebSocketConnectionState.Open)
                        {
                            // Get all the frame data.
                            byte[] data = messsage.RawData;
                            MessageType opcode = OpCodeTypeHelper.GetMessageType(messsage.Type);
                            bool endOfMessage = frame.IsFinal;
                            int count = (data != null ? data.Length : 0);

                            // Create the result;
                            ReceiveResult result = new ReceiveResult(count, opcode, endOfMessage);
                            _receiveResult.Data = data;
                            _receiveResult.Offset = 0;
                            _receiveResult.Result = result;
                            _receiveResult.ReturnComplete = false;

                            // Enqueue the result.
                            _recevedFrame.Set();
                        }
                    }
                    catch (Exception ex)
                    {
                        ProcessException(ex, "An exception has occurred during an OnMessage event.");
                    }
                }
            }
            else
            {
                // Unable to process the frame close the connection.
                _exitReceiving.Set();
            }
        }

        /// <summary>
        /// Process receive frame exception.
        /// </summary>
        /// <param name="ex">The receive exception.</param>
        private void ReceiveFrameException(Exception ex)
        {
            ProcessException(ex, "An exception has occurred while receiving a message.");
        }

        /// <summary>
        /// Initialise the members.
        /// </summary>
        private void Init()
        {
            _waitTime = TimeSpan.FromSeconds(1);
            _readyState = WebSocketConnectionState.Open;

            _messageEventQueue = new Queue<MessageEventArgs>();
            _receiveResult = new ReceiveResultWebContext() { ReturnComplete = true };
            _lockEnqueue = ((ICollection)_messageEventQueue).SyncRoot;

            if (_context != null)
            {
                // Receive data in async mode.
                _context.Context.Context.ReceivedAsyncMode = () => AsyncModeReceiver();
            }

            _exitReceiving = new AutoResetEvent(false);
            _receivePong = new AutoResetEvent(false);
            _recevedFrame = new AutoResetEvent(false);
            _frameReturnedComplete = new AutoResetEvent(false);
        }

        /// <summary>
        /// Send a text frame.
        /// </summary>
        /// <param name="buffer">The buffer to be sent over the connection.</param>
        /// <param name="cancellationToken">The token that propagates the notification that operations should be canceled.</param>
        private void TextOpCodeFrame(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            // If cancellation has not been requested.
            if (!cancellationToken.IsCancellationRequested)
            {
                bool compressed = false;
                MemoryStream stream = null;

                // If a message exists.
                if (buffer != null && buffer.Count > 0)
                {
                    byte[] data = new byte[buffer.Count];
                    for (int i = buffer.Offset; i < (buffer.Offset + buffer.Count); i++)
                        data[i - buffer.Offset] = buffer.Array[i];

                    // Check if valid control data.
                    string msg = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
                    if (!String.IsNullOrEmpty(msg))
                    {
                        // An error has occured.
                        return;
                    }

                    try
                    {
                        // If data needs to be compressed.
                        if (_compression != CompressionAlgorithmStreaming.None)
                        {
                            // Compress the data.
                            byte[] compressedData = Compresss.Compress(data, _compression);
                            string compressedDataStr = Encoding.Default.GetString(compressedData);
                            stream = new MemoryStream(Encoding.UTF8.GetBytes(compressedDataStr));

                            // Compressed with no data.
                            compressed = true;
                        }
                        else
                        {
                            // No compression.
                            string dataStr = Encoding.Default.GetString(data);
                            stream = new MemoryStream(Encoding.UTF8.GetBytes(dataStr));
                        }

                        // The frame.
                        Frame frame = DataOpCodeFrame(stream, OpCodeFrame.Text, compressed);
                        if (frame != null)
                        {
                            // Send the frame data.
                            SendBytes(frame.ToByteArray());
                        }
                    }
                    catch { }
                    finally
                    {
                        if (stream != null)
                            stream.Dispose();
                    }
                }
                else
                {
                    // Check if valid control data.
                    string msg = _readyState.CheckIfOpen();
                    if (!String.IsNullOrEmpty(msg))
                    {
                        // An error has occured.
                        return;
                    }

                    // If data needs to be compressed.
                    if (_compression != CompressionAlgorithmStreaming.None)
                    {
                        // Compressed with no data.
                        compressed = true;
                    }

                    // No data to send.
                    Frame frame = new Frame(Fin.Final, OpCodeFrame.Text, new byte[0], compressed, _client);
                    SendBytes(frame.ToByteArray());
                }
            }
        }

        /// <summary>
        /// Send a binary frame.
        /// </summary>
        /// <param name="buffer">The buffer to be sent over the connection.</param>
        /// <param name="cancellationToken">The token that propagates the notification that operations should be canceled.</param>
        private void BinaryOpCodeFrame(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            // If cancellation has not been requested.
            if (!cancellationToken.IsCancellationRequested)
            {
                bool compressed = false;
                MemoryStream stream = null;

                // If a message exists.
                if (buffer != null && buffer.Count > 0)
                {
                    byte[] data = new byte[buffer.Count];
                    for (int i = buffer.Offset; i < (buffer.Offset + buffer.Count); i++)
                        data[i - buffer.Offset] = buffer.Array[i];

                    // Check if valid control data.
                    string msg = _readyState.CheckIfOpen() ?? data.CheckIfValidSendData();
                    if (!String.IsNullOrEmpty(msg))
                    {
                        // An error has occured.
                        return;
                    }

                    try
                    {
                        // If data needs to be compressed.
                        if (_compression != CompressionAlgorithmStreaming.None)
                        {
                            // Compress the data.
                            byte[] compressedData = Compresss.Compress(data, _compression);
                            stream = new MemoryStream(compressedData);

                            // Compressed with no data.
                            compressed = true;
                        }
                        else
                        {
                            // No compression.
                            stream = new MemoryStream(data);
                        }

                        // The frame.
                        Frame frame = DataOpCodeFrame(stream, OpCodeFrame.Binary, compressed);
                        if (frame != null)
                        {
                            // Send the frame data.
                            SendBytes(frame.ToByteArray());
                        }
                    }
                    catch { }
                    finally
                    {
                        if (stream != null)
                            stream.Dispose();
                    }
                }
                else
                {
                    // Check if valid control data.
                    string msg = _readyState.CheckIfOpen();
                    if (!String.IsNullOrEmpty(msg))
                    {
                        // An error has occured.
                        return;
                    }

                    // If data needs to be compressed.
                    if(_compression != CompressionAlgorithmStreaming.None)
                    {
                        // Compressed with no data.
                        compressed = true;
                    }

                    // No data to send.
                    Frame frame = new Frame(Fin.Final, OpCodeFrame.Binary, new byte[0], compressed, _client);
                    SendBytes(frame.ToByteArray());
                }
            }
        }

        /// <summary>
        /// Send a data frame.
        /// </summary>
        /// <param name="stream">The stream containing the data.</param>
        /// <param name="opcode">The opcode frame type.</param>
        /// <param name="compressed">Is the data compressed.</param>
        /// <returns>The data frame to send.</returns>
        private Frame DataOpCodeFrame(MemoryStream stream, OpCodeFrame opcode, bool compressed)
        {
            // Determin the if more  will be sent.
            long dataLength = stream.Length;
            long quo = dataLength / FragmentLength;
            int rem = (int)(dataLength % FragmentLength);

            // Frame.
            Frame frame = null;

            // Not fragmented.

            byte[] buff = null;
            int bytesRead = 0;

            if (quo == 0)
            {
                // Read the data from the stream.
                buff = new byte[rem];
                bytesRead = stream.Read(buff, 0, rem);

                // Create the fragment frame. Return the frame.
                frame = new Frame(Fin.Final, opcode, buff, compressed, _client);
                return frame;
            }

            buff = new byte[FragmentLength];
            if (quo == 1 && rem == 0)
            {
                // Read the data from the stream.
                bytesRead = stream.Read(buff, 0, FragmentLength);

                // Create the fragment frame. Return the frame.
                frame = new Frame(Fin.Final, opcode, buff, compressed, _client);
                return frame;
            }

            // Send fragmented.
            // Begin
            // Read the data from the stream.
            bytesRead = stream.Read(buff, 0, FragmentLength);
            
            // If the bytes read is equal to the fragment length.
            if (bytesRead > 0)
            {
                // Create the fragment frame. Return the frame.
                frame = new Frame(Fin.More, opcode, buff, compressed, _client);
                if (frame != null)
                {
                    // Send the frame data.
                    SendBytes(frame.ToByteArray());
                }
            }

            long n = rem == 0 ? quo - 2 : quo - 1;
            for (long i = 0; i < n; i++)
            {
                // Read the data from the stream.
                bytesRead = stream.Read(buff, 0, FragmentLength);

                // If the bytes read is equal to the fragment length.
                if (bytesRead > 0)
                {
                    // Create the fragment frame. Return the frame.
                    frame = new Frame(Fin.More, OpCodeFrame.Continuation, buff, compressed, _client);
                    if (frame != null)
                    {
                        // Send the frame data.
                        SendBytes(frame.ToByteArray());
                    }
                }
            }
            // End

            if (rem == 0)
                rem = FragmentLength;
            else
                buff = new byte[rem];

            // Read the data from the stream.
            bytesRead = stream.Read(buff, 0, rem);

            // Create the fragment frame. Return the frame.
            frame = new Frame(Fin.Final, OpCodeFrame.Continuation, buff, compressed, _client);
            return frame;
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        /// <param name="buffer">The buffer to be sent over the connection.</param>
        /// <param name="cancellationToken">The token that propagates the notification that operations should be canceled.</param>
        private void CloseOpCodeFrame(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            // If cancellation has not been requested.
            if (!cancellationToken.IsCancellationRequested)
            {
                string reason = string.Empty;

                // If a message exists.
                if(buffer != null && buffer.Count > 0)
                {
                    byte[] data = new byte[buffer.Count];
                    for (int i = buffer.Offset; i < (buffer.Offset + buffer.Count); i++)
                        data[i - buffer.Offset] = buffer.Array[i];

                    // Get the reason.
                    reason = Encoding.Default.GetString(data);
                }

                // Close the connection.
                var send = !CloseCodeStatus.NormalClosure.IsReserved();
                Close(new CloseEventArgs(CloseCodeStatus.NormalClosure, reason), send, send);
            }
        }

        /// <summary>
        /// Send a ping frame.
        /// </summary>
        /// <param name="buffer">The buffer to be sent over the connection.</param>
        /// <param name="cancellationToken">The token that propagates the notification that operations should be canceled.</param>
        private void PingOpCodeFrame(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            // If cancellation has not been requested.
            if (!cancellationToken.IsCancellationRequested)
            {
                byte[] message;
                
                // If a message exists.
                if (buffer != null && buffer.Count > 0)
                {
                    byte[] data = new byte[buffer.Count];
                    for (int i = buffer.Offset; i < (buffer.Offset + buffer.Count); i++)
                        data[i - buffer.Offset] = buffer.Array[i];

                    // Check if valid control data.
                    string msg = data.CheckIfValidControlData("message");
                    if (!String.IsNullOrEmpty(msg))
                    {
                        // An error has occured.
                        return;
                    }

                    // Send the ping frame.
                    Ping(Frame.CreatePingFrame(data, _client).ToByteArray(), _waitTime);
                }
                else
                {
                    // Create the empty ping.
                    message = _client
                        ? Frame.CreatePingFrame(true).ToByteArray()
                        : Frame.EmptyUnmaskPingBytes;

                    // Send the ping frame.
                    Ping(message, _waitTime);
                }
            }
        }

        /// <summary>
        /// Send a ping frame.
        /// </summary>
        /// <param name="frameAsBytes">The frame to send.</param>
        /// <param name="timeout">The wait timeout.</param>
        /// <returns>True if sent; else false.</returns>
        private bool Ping(byte[] frameAsBytes, TimeSpan timeout)
        {
            try
            {
                AutoResetEvent pong;

                // Send the ping frame.
                return _readyState == WebSocketConnectionState.Open && Send(frameAsBytes) &&
                    (pong = _receivePong) != null && pong.WaitOne(timeout);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Process the recived frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <returns>True if processed; else false.</returns>
        private bool ProcessReceivedFrame(Frame frame)
        {
            var msg = CheckIfValidReceivedFrame(frame);
            if (!String.IsNullOrEmpty(msg))
                return ProcessUnsupportedFrame(frame, CloseCodeStatus.ProtocolError, msg);

            frame.Unmask();
            return frame.IsFragmented
                   ? ProcessFragmentedFrame(frame)
                   : frame.IsData
                     ? ProcessDataFrame(frame)
                     : frame.IsPing
                       ? ProcessPingFrame(frame)
                       : frame.IsPong
                         ? ProcessPongFrame(frame)
                         : frame.IsClose
                           ? ProcessCloseFrame(frame)
                           : ProcessUnsupportedFrame(frame, CloseCodeStatus.UnsupportedData, null);
        }

        /// <summary>
        /// Process fragments.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <returns>True if processed; else false.</returns>
        private bool ProcessFragmentedFrame(Frame frame)
        {
            // Must process first fragment.
            return frame.IsContinuation || ProcessFragments(frame);
        }

        /// <summary>
        /// Process fragments.
        /// </summary>
        /// <param name="first">The first frame.</param>
        /// <returns>True if processed; else false.</returns>
        private bool ProcessFragments(Frame first)
        {
            using (var buff = new MemoryStream())
            {
                // Write the payload data.
                buff.WriteBytes(first.PayloadData.ApplicationData);
                if (!ReadFragments(buff))
                    return false;

                byte[] data;

                // If frame data is compressed.
                if (_compression != CompressionAlgorithmStreaming.None)
                {
                    // Decompress the data.
                    data = buff.Decompress(_compression);
                }
                else
                {
                    // Get the complete frame data.
                    buff.Close();
                    data = buff.ToArray();
                }

                // Enqueue the message.
                EnqueueToMessageEventQueue(new MessageEventArgs(first.Opcode, data));
                return true;
            }
        }

        /// <summary>
        /// Read the complete frame from the stream.
        /// </summary>
        /// <param name="store">The stream to store data.</param>
        /// <returns>True if complete; else false.</returns>
        private bool ReadFragments(Stream store)
        {
            while (true)
            {
                // Read all the frame data.
                Frame frame = Frame.Read(_readStream, false);

                // Check to see if the frame if valid.
                string isValidMessage = CheckIfValidReceivedFrame(frame);
                if (!String.IsNullOrEmpty(isValidMessage))
                    return ProcessUnsupportedFrame(frame, CloseCodeStatus.ProtocolError, isValidMessage);

                // Un mask the frame.
                frame.Unmask();

                // Is this the final set of data.
                if (frame.IsFinal)
                {
                    // More data exists.
                    if (frame.IsContinuation)
                    {
                        // Store the current data.
                        store.WriteBytes(frame.PayloadData.ApplicationData);
                        break;
                    }

                    // Process a Ping frame.
                    if (frame.IsPing)
                    {
                        ProcessPingFrame(frame);
                        continue;
                    }

                    // Process a Pong frame.
                    if (frame.IsPong)
                    {
                        ProcessPongFrame(frame);
                        continue;
                    }

                    // Process close frame.
                    if (frame.IsClose)
                        return ProcessCloseFrame(frame);
                }
                else
                {
                    // More data exists.
                    if (frame.IsContinuation)
                    {
                        // Store the current data.
                        store.WriteBytes(frame.PayloadData.ApplicationData);
                        continue;
                    }
                }

                // Something went wrong at this point; exit.
                return ProcessUnsupportedFrame(frame, CloseCodeStatus.UnsupportedData,
                    "An incorrect data has been received while receiving the fragmented data.");
            }

            return true;
        }

        /// <summary>
        /// Enqueue the current message.
        /// </summary>
        /// <param name="e">The message event.</param>
        private void EnqueueToMessageEventQueue(MessageEventArgs e)
        {
            lock (_lockEnqueue)
                _messageEventQueue.Enqueue(e);
        }

        /// <summary>
        /// Dequeue the current message.
        /// </summary>
        /// <returns>The message event.</returns>
        private MessageEventArgs DequeueFromMessageEventQueue()
        {
            lock (_lockEnqueue)
                return _messageEventQueue.Count > 0
                       ? _messageEventQueue.Dequeue()
                       : null;
        }

        /// <summary>
        /// Send the frame.
        /// </summary>
        /// <param name="bytes">The frame.</param>
        /// <returns>True if sent; else false.</returns>
        private bool SendBytes(byte[] bytes)
        {
            try
            {
                // If data exists.
                if (bytes != null && bytes.Length > 0)
                {
                    // Write the frame to the stream.
                    _writeStream.Write(bytes, 0, bytes.Length);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send the frame.
        /// </summary>
        /// <param name="frameAsBytes">The frame.</param>
        /// <returns>True if sent; else false.</returns>
        private bool Send(byte[] frameAsBytes)
        {
            lock (_lockConnect)
            {
                // If not open then can not send.
                if (_readyState != WebSocketConnectionState.Open)
                    return false;

                // Send the frame.
                return SendBytes(frameAsBytes);
            }
        }

        /// <summary>
        /// Process data frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <returns>Return tre.</returns>
        private bool ProcessDataFrame(Frame frame)
        {
            EnqueueToMessageEventQueue(
                frame.IsCompressed
                ? new MessageEventArgs(frame.Opcode, Compresss.Decompress(frame.PayloadData.ApplicationData, _compression))
                : new MessageEventArgs(frame));

            return true;
        }

        /// <summary>
        /// Process the close frame.
        /// </summary>
        /// <param name="frame">The close frame to send.</param>
        /// <returns>Return false.</returns>
        private bool ProcessCloseFrame(Frame frame)
        {
            var payload = frame.PayloadData;
            Close(new CloseEventArgs(payload), !payload.IncludesReservedCloseStatusCode, false);
            return false;
        }

        /// <summary>
        /// Process a Ping frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <returns>Return true.</returns>
        private bool ProcessPingFrame(Frame frame)
        {
            Send(new Frame(OpCodeFrame.Pong, frame.PayloadData, _client).ToByteArray());
            return true;
        }

        /// <summary>
        /// Process a Pong frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <returns>Return true.</returns>
        private bool ProcessPongFrame(Frame frame)
        {
            _receivePong.Set();
            return true;
        }

        /// <summary>
        /// Check to see if the frame if valid.
        /// </summary>
        /// <param name="frame">The frame to check.</param>
        /// <returns>The frame description; else null.</returns>
        private string CheckIfValidReceivedFrame(Frame frame)
        {
            var masked = frame.IsMasked;
            return _client && masked
                   ? "A frame from the server is masked."
                   : !_client && !masked
                     ? "A frame from a client isn't masked."
                     : frame.IsCompressed && _compression == CompressionAlgorithmStreaming.None
                       ? "A compressed frame is without the available decompression method."
                       : null;
        }

        /// <summary>
        /// Process unsupported frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="code">The close code.</param>
        /// <param name="reason">The invalid description.</param>
        /// <returns>Returns false.</returns>
        private bool ProcessUnsupportedFrame(Frame frame, CloseCodeStatus code, string reason)
        {
            ProcessException(new WebSocketException(code, reason), null);
            return false;
        }

        /// <summary>
        /// Process the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The messsage.</param>
        private void ProcessException(Exception exception, string message)
        {
            var code = CloseCodeStatus.AbnormalClosure;
            var reason = message;

            // If the exception is WebSocketException.
            if (exception is WebSocketException)
            {
                var wsex = (WebSocketException)exception;
                code = wsex.Code;
                reason = wsex.Message;
            }

            // Close the connection.
            Close(new CloseEventArgs (code, reason ?? code.GetMessage ()), !code.IsReserved(), false);
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        /// <param name="close">The close event arguments.</param>
        /// <param name="send">Is sending.</param>
        /// <param name="wait">Should wait.</param>
        private void Close(CloseEventArgs close, bool send, bool wait)
        {
            lock (_lockConnect)
            {
                // If already closed or closing.
                if (_readyState == WebSocketConnectionState.Closing || _readyState == WebSocketConnectionState.Closed)
                    return;

                // Wait if sending.
                send = send && _readyState == WebSocketConnectionState.Open;
                wait = wait && send;

                // Closing.
                _readyState = WebSocketConnectionState.Closing;
            }

            // Close the handshake.
            close.WasClean = CloseHandshake(
                send ? Frame.CreateCloseFrame(close.PayloadData, _client).ToByteArray() : null,
                wait ? _waitTime : TimeSpan.Zero,
                _client ? (Action)ReleaseClientResources : ReleaseServerResources);

            // Connection state is closed.
            _readyState = WebSocketConnectionState.Closed;
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        /// <param name="frameAsBytes">The close frame data.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="release">The release action.</param>
        /// <returns>True if closed clean; else false.</returns>
        private bool CloseHandshake(byte[] frameAsBytes, TimeSpan timeout, Action release)
        {
            var sent = frameAsBytes != null && SendBytes(frameAsBytes);
            var received = timeout == TimeSpan.Zero || (sent && _exitReceiving != null && _exitReceiving.WaitOne(timeout));

            // Release the resources.
            release();

            // Return the result.
            var res = sent && received;
            return res;
        }

        /// <summary>
        /// Close and release server resources.
        /// </summary>
        private void ReleaseServerResources()
        {
            if(_context != null)
            {
                // Close a release all resources.
                _context.Close();
            }
        }

        /// <summary>
        /// Close and release client resources.
        /// </summary>
        private void ReleaseClientResources()
        {
            if (_contextClient != null)
            {
                // Close a release all resources.
                _contextClient.Close();
            }
        }
        #endregion

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
                    // Dispose managed resources.
                    if (_receivePong != null)
                        _receivePong.Dispose();

                    if (_exitReceiving != null)
                        _exitReceiving.Dispose();

                    if (_recevedFrame != null)
                        _recevedFrame.Dispose();

                    if (_frameReturnedComplete != null)
                        _frameReturnedComplete.Dispose();

                    if (_readStream != null)
                        _readStream.Dispose();

                    if (_writeStream != null)
                        _writeStream.Dispose();

                    if (_messageEventQueue != null)
                        _messageEventQueue.Clear();

                    if (_receiveResult != null)
                    {
                        _receiveResult.Result = null;
                        _receiveResult.Data = null;
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _lockConnect = null;
                _lockReceive = null;
                _lockEnqueue = null;

                _receiveResult = null;
                _messageEventQueue = null;

                _receivePong = null;
                _exitReceiving = null;
                _recevedFrame = null;
                _frameReturnedComplete = null;

                _readStream = null;
                _writeStream = null;

                _context = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~WebSocket()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
