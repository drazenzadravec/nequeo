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
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Security;
using Nequeo.Threading;
using Nequeo.Collections;

namespace Nequeo.Net.Sockets
{
    /// <summary>
    /// A callback interface. It's methods get called from
    /// <see cref="Multiplexer.Poll"/> method when a <see cref="Socket"/>
    /// asociated with the object implementing the interface changes it's state.
    /// </summary>
    public interface IMultiplexed
    {
        #region Public Methods
        /// <summary>
        /// Called when the socket has some data to read.
        /// </summary>
        void ReadyRead();

        /// <summary>
        /// Called when the socket is able to send data.
        /// </summary>
        /// <returns>True if data exists to write: else false.</returns>
        bool ReadyWrite();

        /// <summary>
        /// Called when the socket goes to error state.
        /// </summary>
        void ErrorState();

        /// <summary>
        /// Gets sets, suspends the notifications from the socket.
        /// </summary>
        bool Suspend { get; set; }

        /// <summary>
        /// Gets or sets, is the current mux a listener; else false.
        /// </summary>
        bool IsListener { get; set; }

        #endregion
    }

    /// <summary>
	/// Represents a <see cref="Socket.Select">Socket.Select</see> multiplexer.
	/// </summary>
    public sealed class Multiplexer : IDisposable
    {
        /// <summary>
        /// Represents a <see cref="Socket.Select">Socket.Select</see> multiplexer.
        /// </summary>
        /// <param name="timeoutMicroseconds">Polling timeout (in microseconds).</param>
        /// <param name="maxPollingSockets">The maximum number of polling sockets (including all listener sockets).</param>
        /// <param name="socketThreadCount">The total number of threads to use.</param>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Multiplexer(int timeoutMicroseconds = -1, int maxPollingSockets = 10000, int socketThreadCount = 1)
        {
            if (maxPollingSockets <= 0 || maxPollingSockets > 0x10000)
                throw new IndexOutOfRangeException("The maxPollingSockets must be between 1 and " + (0x10000).ToString());

            _timeoutMicroseconds = timeoutMicroseconds;
            _maxSelectSockets = maxPollingSockets;
            _socketThreadCount = socketThreadCount;

            _writeQueues = new List<IMultiplexed>[_socketThreadCount];
            _writeQueueToRemoves = new List<IMultiplexed>[_socketThreadCount];
            _pollWriterInt = new bool[_socketThreadCount];

            // For each thread.
            for(int i = 0; i < _socketThreadCount; i++)
            {
                _pollWriterInt[i] = true;
                _writeQueues[i] = new List<IMultiplexed>();
                _writeQueueToRemoves[i] = new List<IMultiplexed>();
            }
        }

        // Microseconds.
        private int _timeoutMicroseconds = -1; // 100000 micro-seconds, 100 milli-seconds, 0.1 seconds.

        private readonly CustomDictionary<Socket, IMultiplexed> _multiplexers = new CustomDictionary<Socket, IMultiplexed>();
        private List<IMultiplexed>[] _writeQueues = null;
        private List<IMultiplexed>[] _writeQueueToRemoves = null;

        private bool _pollReader = true;
        private bool _pollWriter = true;
        private bool _pollError = false;

        private bool[] _pollWriterInt = null;

        private object _lockSocketsObject = new object();

        private int _socketThreadCount = 1; // The total number of threads to use.
        private bool _continuePolling = true;
        private int _maxSelectSockets = 10000;  // Limit 0 to 65536.
                                        /* 
                                        I checked winsock2.h from the platform SDK, and an fd_set is just
                                        implemented like that. A single unsigned number containing the number of
                                        sockets in the set, and then an array of SOCKET handles. When you set
                                        FD_SETSIZE in C++, you have to set it before you include windows.h (or
                                        winsock2.h). This way, the fd_set type is declared with a constant size
                                        of FD_SETSIZE. Although this is a C# newsgroup, I hope some C++ is not a
                                        problem. So here is the part from winsock2.h:

                                        #ifndef FD_SETSIZE
                                        #define FD_SETSIZE 64
                                        #endif

                                        typedef struct fd_set {
                                        u_int fd_count; // how many are SET?
                                        SOCKET fd_array[FD_SETSIZE]; // an array of SOCKETs
                                        } fd_set;

                                        This means that you should not worry about this in .NET, because it is
                                        implemented for you transparently. Go ahead and use Socket.Select() for
                                        socket arrays that contain less than 0x10000 elements. (thats 65536 in
                                        decimal). It will work.
                                        */

        /// <summary>
        /// Gets the number of client sockets.
        /// </summary>
        public int SocketCount
        {
            get 
            {
                lock (_lockSocketsObject)
                    return _multiplexers.Values.Count(u => u.IsListener == false);
            }
        }

        /// <summary>
        /// Gets the number of listener sockets.
        /// </summary>
        public int ListenerCount
        {
            get
            {
                lock (_lockSocketsObject)
                    return _multiplexers.Values.Count(u => u.IsListener == true);
            }
        }

        /// <summary>
        /// Gets or sets an indicator if auto read polling is active.
        /// </summary>
        public bool PollReader
        {
            get { return _pollReader; }
            set { _pollReader = value; }
        }

        /// <summary>
        /// Gets or sets an indicator if auto write polling is active.
        /// </summary>
        public bool PollWriter
        {
            get { return _pollWriter; }
            set { _pollWriter = value; }
        }

        /// <summary>
        /// Gets or sets an indicator if auto error polling is active.
        /// </summary>
        public bool PollError
        {
            get { return _pollError; }
            set { _pollError = value; }
        }
        
        /// <summary>
        /// Polls all registered <see cref="Socket"/>s and calls proper
        /// callbacks.
        /// </summary>
        /// <param name="continuePolling">
        /// Continue the polling.
        /// </param>
        /// <returns>
        /// <c>true</c> if there are some multiplexed object,
        /// <c>false</c> otherwise.
        /// </returns>
        /// <remarks>
        /// You can use the return value as a signal if the application can be
        /// shut down. However, the application may continue to call
        /// the <see cref="Poll"/> method.
        /// </remarks>
        public bool Poll(bool continuePolling = true)
        {
            // Make sure listener socket exists.
            if (_multiplexers.Count < 1)
                return false;

            // If no threads are to be used.
            if (_socketThreadCount < 1)
                return false;

            // Poll.
            _continuePolling = continuePolling;
            SpinWaitSelector(_continuePolling);

            // Should the polling continue.
            if (continuePolling)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Stop polling.
        /// </summary>
        public void StopPolling()
        {
            _continuePolling = false;
        }

        /// <summary>
        /// Determines the status of the System.Net.Sockets.Socket.
        /// </summary>
        /// <param name="socket">The socket to poll.</param>
        /// <param name="microSeconds">The time to wait for a response, in microseconds.</param>
        /// <param name="mode">One of the System.Net.Sockets.SelectMode values.</param>
        /// <returns>
        /// The status of the System.Net.Sockets.Socket based on the polling mode value
        /// passed in the mode parameter.Mode Return Value System.Net.Sockets.SelectMode.SelectReadtrue
        /// if System.Net.Sockets.Socket.Listen(System.Int32) has been called and a connection
        /// is pending; -or- true if data is available for reading; -or- true if the
        /// connection has been closed, reset, or terminated; otherwise, returns false.
        /// System.Net.Sockets.SelectMode.SelectWritetrue, if processing a System.Net.Sockets.Socket.Connect(System.Net.EndPoint),
        /// and the connection has succeeded; -or- true if data can be sent; otherwise,
        /// returns false. System.Net.Sockets.SelectMode.SelectErrortrue if processing
        /// a System.Net.Sockets.Socket.Connect(System.Net.EndPoint) that does not block,
        /// and the connection has failed; -or- true if System.Net.Sockets.SocketOptionName.OutOfBandInline
        /// is not set and out-of-band data is available; otherwise, returns false.
        /// </returns>
        public bool PollSocket(Socket socket, int microSeconds, SelectMode mode)
        {
            if (socket == null) throw new ArgumentNullException("socket");

            // Return the result.
            return socket.Poll(microSeconds, mode);
        }

        /// <summary>
        /// Register a <see cref="Socket"/> for issuing notifications.
        /// </summary>
        /// <param name="socket">The listener socket to register.</param>
        /// <param name="multiplexed">
        /// An object that implements the <see cref="IMultiplexed"/> inteface.
        /// </param>
        public void AddSocketListener(Socket socket, IMultiplexed multiplexed)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            if (multiplexed == null) throw new ArgumentNullException("multiplexed");

            lock (_lockSocketsObject)
                _multiplexers.Add(socket, multiplexed);
        }

        /// <summary>
        /// Uregisters the socket from issuing notifications.
        /// </summary>
        /// <param name="socket">The listener socket to register.</param>
        public void RemoveSocketListener(Socket socket)
        {
            if (socket == null) throw new ArgumentNullException("socket");

            lock (_lockSocketsObject)
            {
                if (_multiplexers.ContainsKey(socket))
                    _multiplexers.Remove(socket);
            }
        }

        /// <summary>
        /// Register a <see cref="Socket"/> for issuing notifications.
        /// </summary>
        /// <param name="socket">The socket to register.</param>
        /// <param name="multiplexed">
        /// An object that implements the <see cref="IMultiplexed"/> inteface.
        /// </param>
        public void AddSocket(Socket socket, IMultiplexed multiplexed)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            if (multiplexed == null) throw new ArgumentNullException("multiplexed");

            lock (_lockSocketsObject)
                _multiplexers.Add(socket, multiplexed);
        }

        /// <summary>
        /// Uregisters the socket from issuing notifications.
        /// </summary>
        /// <param name="socket">The socket to register.</param>
        public void RemoveSocket(Socket socket)
        {
            if (socket == null) throw new ArgumentNullException("socket");

            lock (_lockSocketsObject)
            {
                if (_multiplexers.ContainsKey(socket))
                    _multiplexers.Remove(socket);
            }
        }

        /// <summary>
        /// Should the error state on a socket be polled, (default : false).
        /// </summary>
        /// <param name="poll">True if it is polled; else false.</param>
        public void SetPollError(bool poll)
        {
            _pollError = poll;
        }

        /// <summary>
        /// Should the read state on a socket be polled, (default : true).
        /// </summary>
        /// <param name="poll">True if it is polled; else false.</param>
        public void SetPollReader(bool poll)
        {
            _pollReader = poll;
        }

        /// <summary>
        /// Should the write state on a socket be polled, (default : false).
        /// </summary>
        /// <param name="poll">True if it is polled; else false.</param>
        public void SetPollWriter(bool poll)
        {
            _pollWriter = poll;
        }

        /// <summary>
        /// Get socket mux data.
        /// </summary>
        /// <param name="all">True to get all; else false.</param>
        /// <param name="listener">True to get all listeners; else false.</param>
        /// <returns>The list of sockets</returns>
        private IList GetSockets(bool all = false, bool listener = false)
        {
            // Get all sockets.
            lock (_lockSocketsObject)
            {
                // If get all.
                if (all)
                    // Get all keys.
                    return _multiplexers.GetKeys();
                else
                    // Get specific.
                    return _multiplexers.GetKeys(u => u.Value.IsListener == listener);
            }
        }

        

        /// <summary>
        /// Get write socket mux data.
        /// </summary>
        /// <returns>The list of sockets</returns>
        private IList GetWriteSockets()
        {
            // Get all sockets.
            lock (_lockSocketsObject)
            {
                // Get one socket.
                return _multiplexers.GetKeys(u => u.Value.IsListener == false, 1);
            }
        }

        /// <summary>
        /// Get the mux for the socket.
        /// </summary>
        /// <param name="socket">The socket to search.</param>
        /// <returns>The mux.</returns>
        private IMultiplexed GetMux(Socket socket)
        {
            lock (_lockSocketsObject)
                return _multiplexers[socket];
        }

        /// <summary>
        /// Get socket mux data.
        /// </summary>
        /// <param name="all">True to get all; else false.</param>
        /// <param name="listener">True to get all listeners; else false.</param>
        /// <returns>The list of sockets</returns>
        private IEnumerable<IMultiplexed> GetMuxs(bool all = false, bool listener = false)
        {
            // Get all sockets.
            lock (_lockSocketsObject)
            {
                // If get all.
                if (all)
                    // Get all keys.
                    return _multiplexers.Values;
                else
                    // Get specific.
                    return _multiplexers.Values.Where(u => u.IsListener == listener);
            }
        }

        /// <summary>
        /// Does the mux contain items.
        /// </summary>
        /// <returns>True if the mux contains items; else false.</returns>
        private bool MuxContainsItems()
        {
            lock (_lockSocketsObject)
                return (_multiplexers.Count > 0 ? true : false);
        }

        /// <summary>
        /// Is the write queue empty.
        /// </summary>
        /// <returns>True if the write queue is not empty; else false.</returns>
        private bool WriteQueueNotEmpty()
        {
            // Determin if all the write queues are empty.
            return (_pollWriterInt.Count(u => u == false) >= _socketThreadCount ? false : true);
        }

        /// <summary>
        /// Split the list of sockets.
        /// </summary>
        /// <param name="socketList">The list of sockets to split.</param>
        /// <returns>The list of socket collection.</returns>
        private ArrayList[] GetSocketList(ArrayList socketList)
        {
            List<ArrayList> list = new List<ArrayList>();

            int count = socketList.Count;
            int totalNumSegments = 0;

            // Get the total number of clients per thread.
            // Initially the same as max number of clients;
            // Add one segiment if a remainder exists.
            if ((count % _socketThreadCount) > 0)
                totalNumSegments = ((int)(count / _socketThreadCount)) + 1;
            else
                totalNumSegments = ((int)(count / _socketThreadCount));

            // If socket number is more than thread count.
            if (count >= _socketThreadCount)
            {
                // For each thread get the number of items.
                for (int i = 0; i < _socketThreadCount; i++)
                {
                    int start = i * totalNumSegments;
                    int number = totalNumSegments;
                    int left = count - start;

                    // If the number left is less than
                    // the current start index, then
                    // take what is left.
                    if (left < number)
                        number = left;

                    // Get the array list.
                    list.Add(socketList.GetRange(start, number));
                }
            }
            else
            {
                // Return the original array list.
                list.Add(socketList);
            }

            // Return the list.
            return list.ToArray();
        }

        /// <summary>
        /// Get the list of muxes.
        /// </summary>
        /// <param name="socketList">The list of sockets to split.</param>
        /// <returns>The collection of muxes;</returns>
        private IMultiplexed[] GetMuxes(ArrayList socketList)
        {
            List<IMultiplexed> list = new List<IMultiplexed>();

            // For each socket that is ready to read.
            foreach (Socket socket in socketList)
            {
                IMultiplexed mux = GetMux(socket);
                list.Add(mux);
            }

            // Return the mux
            return list.ToArray();
        }

        /// <summary>
        /// Spin wait selector.
        /// </summary>
        /// <param name="continuePolling">
        /// Continue the polling.
        /// </param>
        private void SpinWaitSelector(bool continuePolling)
        {
            // If polling.
            if (continuePolling)
            {
                bool exitIndicator = false;
                Task[] tasks = null;

                try
                {
                    // Multiple socket job threads,
                    // one client socket thread, one exit thread.
                    tasks = new Task[1 + 1];

                    // Listener task.
                    Task pollSockets = Task.Factory.StartNew(() =>
                    {
                        // Create a new spin wait.
                        SpinWait sw = new SpinWait();

                        // Action to perform.
                        while (!exitIndicator)
                        {
                            // The NextSpinWillYield property returns true if 
                            // calling sw.SpinOnce() will result in yielding the 
                            // processor instead of simply spinning. 
                            if (sw.NextSpinWillYield)
                            {
                                // Make sure a sockets exists.
                                if (MuxContainsItems())
                                {
                                    // Poll the sockets.
                                    PollSocketsEx();
                                }
                            }
                            sw.SpinOnce();
                        }
                    });

                    // Assign the socket task.
                    tasks[0] = pollSockets;

                    // Wait timeout, then set exitIndicator to true
                    Task timeout = Task.Factory.StartNew(() =>
                    {
                        // Create a new spin wait.
                        SpinWait sw = new SpinWait();

                        // Action to perform.
                        while (!exitIndicator)
                        {
                            // The NextSpinWillYield property returns true if 
                            // calling sw.SpinOnce() will result in yielding the 
                            // processor instead of simply spinning. 
                            if (sw.NextSpinWillYield)
                            {
                                if (!_continuePolling)
                                    exitIndicator = true;
                            }
                            sw.SpinOnce();
                        }
                    });

                    // Add the timeout task.
                    tasks[1] = timeout;

                    // For each socket task.
                    for (int i = 2; i < tasks.Length; i++) { }

                    // Wait for all tasks to complete.
                    Task.WaitAll(tasks);
                }
                catch { }

                // For each task.
                foreach (Task item in tasks)
                {
                    try
                    {
                        // Release the resources.
                        item.Dispose();
                        _multiplexers.Clear();

                        foreach (List<IMultiplexed> queue in _writeQueues)
                            queue.Clear();

                        foreach (List<IMultiplexed> queueRemove in _writeQueueToRemoves)
                            queueRemove.Clear();
                    }
                    catch { }
                }
                tasks = null;
            }
        }

        /// <summary>
        /// Polls all registered <see cref="Socket"/>s and calls proper
        /// callbacks.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there are some multiplexed object,
        /// <c>false</c> otherwise.
        /// </returns>
        /// <remarks>
        /// You can use the return value as a signal if the application can be
        /// shut down. However, the application may continue to call
        /// the <see cref="Poll"/> method.
        /// </remarks>
        private void PollSocketsEx()
        {
            ArrayList readSockets = null;
            ArrayList writeSockets = null;
            ArrayList errorSockets = null;
            IList writeSocket = null;
            IList sockets = null;
            ParallelOptions options = null;
            
            try
            {
                // Should the reader be polled.
                if (_pollReader)
                {
                    sockets = GetSockets(true);
                    readSockets = new ArrayList();
                    readSockets.AddRange(sockets);

                    // If using more than one thread.
                    if (_socketThreadCount > 1)
                        options = new ParallelOptions();
                }

                // Should the writer be polled.
                if (_pollWriter)
                {
                    // If a socket exists then
                    // only add one write socket.
                    if (SocketCount > 0)
                        writeSocket = GetWriteSockets();
                    else
                        writeSocket = sockets;

                    // If internal writer control is on.
                    if (WriteQueueNotEmpty())
                    {
                        writeSockets = new ArrayList();
                        writeSockets.AddRange(writeSocket);
                    }

                    // Create the parallel options.
                    if (options == null && _socketThreadCount > 1)
                        options = new ParallelOptions();
                }

                // Should the error be polled.
                if (_pollError)
                {
                    // Create the sockets if not created.
                    if (sockets == null)
                        sockets = GetSockets(true);

                    errorSockets = new ArrayList();
                    errorSockets.AddRange(sockets);

                    // Create the parallel options.
                    if (options == null && _socketThreadCount > 1)
                        options = new ParallelOptions();
                }

                // Determines the status of one or more sockets.
                // Select returns when at least one of the sockets of interest 
                // (the sockets in the checkRead, checkWrite, and checkError lists) 
                // meets its specified criteria, or the microSeconds parameter is 
                // exceeded, whichever comes first. Setting microSeconds to -1 
                // specifies an infinite time-out.
                Socket.Select(readSockets, writeSockets, errorSockets, _timeoutMicroseconds);

                // If using more than one thread.
                if (_socketThreadCount > 1)
                {
                    // Should the error be polled.
                    if (_pollError)
                    {
                        // For each error socket.
                        Parallel.ForEach<IMultiplexed>(GetMuxes(errorSockets), options, (mux, loopState, index) =>
                        {
                            try
                            {
                                // Poll the error.
                                PollErrorEx(mux);
                            }
                            catch { }
                        });
                    }
                    
                    // Should the reader be polled.
                    if (_pollReader)
                    {
                        // Get the socket list.
                        ArrayList[] readList = GetSocketList(readSockets);

                        // For each read socket.
                        Parallel.ForEach<ArrayList>(readList, options, (socketsRead, loopState, index) =>
                        {
                            try
                            {
                                // Get the current list index.
                                int z = (int)index;

                                // For each socket that is ready to read.
                                foreach (IMultiplexed mux in GetMuxes(readSockets))
                                    PollReadEx(mux, z);
                            }
                            catch { }
                        });
                    }

                    // Should the writer be polled.
                    if (_pollWriter)
                    {
                        // For each write queue.
                        Parallel.For(0, _socketThreadCount, index =>
                        {
                            try
                            {
                                // Get the current list index.
                                int z = (int)index;

                                // If internal writer control is on.
                                if (_pollWriterInt[z])
                                {
                                    // For each write operation.
                                    for (int i = 0; i < _writeQueues[z].Count; i++)
                                        PollWriteEx(i, z);

                                    // Clear the write queue.
                                    ClearWriteQueue(z);
                                }
                            }
                            catch { }
                        });
                    }
                }
                else
                {
                    // Should the error be polled.
                    if (_pollError)
                    {
                        // For each socket that contains an error.
                        foreach (IMultiplexed mux in GetMuxes(errorSockets))
                            PollErrorEx(mux);
                    }

                    // Should the reader be polled.
                    if (_pollReader)
                    {
                        // For each socket that is ready to read.
                        foreach (IMultiplexed mux in GetMuxes(readSockets))
                            PollReadEx(mux, 0);
                    }

                    // Should the writer be polled.
                    if (_pollWriter)
                    {
                        // If internal writer control is on.
                        if (_pollWriterInt[0])
                        {
                            // For each write operation.
                            for (int i = 0; i < _writeQueues[0].Count; i++)
                                PollWriteEx(i, 0);

                            // Clear the write queue.
                            ClearWriteQueue(0);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                // Clear the socket collection.
                if (readSockets != null)
                    readSockets.Clear();

                if (writeSockets != null)
                    writeSockets.Clear();

                if (errorSockets != null)
                    errorSockets.Clear();

                readSockets = null;
                writeSockets = null;
                errorSockets = null;
                writeSocket = null;
                sockets = null;
                options = null;
            }
        }

        /// <summary>
        /// Poll errors.
        /// </summary>
        /// <param name="mux">The current mux.</param>
        private void PollErrorEx(IMultiplexed mux)
        {
            if (null == mux) return;
            if (mux.Suspend) return;
            mux.ErrorState();
        }

        /// <summary>
        /// Poll readers.
        /// </summary>
        /// <param name="mux">The current mux.</param>
        /// <param name="threadIndex">The current thread index.</param>
        private void PollReadEx(IMultiplexed mux, int threadIndex)
        {
            if (null == mux) return;
            if (mux.Suspend) return;
            mux.ReadyRead();

            // Only if write polling is enabled.
            if (_pollWriter)
            {
                // If not listener.
                if (!mux.IsListener)
                {
                    _pollWriterInt[threadIndex] = true;

                    // If the write job does not exist.
                    if (!_writeQueues[threadIndex].Contains(mux))
                        _writeQueues[threadIndex].Add(mux);
                }
            }
        }

        /// <summary>
        /// Poll writers.
        /// </summary>
        /// <param name="index">The current write queue index.</param>
        /// <param name="threadIndex">The current thread index.</param>
        private void PollWriteEx(int index, int threadIndex)
        {
            // Get the current write operation.
            IMultiplexed mux = _writeQueues[threadIndex][index];;

            // If not listener.
            if (!mux.IsListener)
            {
                // Write operation.
                bool allSent = mux.ReadyWrite();
                if (!allSent)
                {
                    // Remove the mux from the queue
                    // If all data has been writen.
                    _writeQueueToRemoves[threadIndex].Add(mux);
                }
            }
        }

        /// <summary>
        /// Clear the write queue.
        /// </summary>
        /// <param name="threadIndex">The current thread index.</param>
        private void ClearWriteQueue(int threadIndex)
        {
            // Write queue to remove.
            _writeQueues[threadIndex].RemoveAll((mux) =>
            {
                // If the mux needs to be removed.
                if (_writeQueueToRemoves[threadIndex].Contains(mux))
                    return true;
                else
                    return false;
            });

            // Clear the remove list.
            _writeQueueToRemoves[threadIndex].Clear();

            // If all data has been sent
            // from all sockets then exit.
            if (_writeQueues[threadIndex].Count < 1)
                _pollWriterInt[threadIndex] = false;
        }

        #region Dispose Object Methods

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
                    if (_multiplexers != null)
                        _multiplexers.Dispose();

                    if (_writeQueues != null)
                        foreach (List<IMultiplexed> queue in _writeQueues)
                            queue.Clear();

                    if (_writeQueueToRemoves != null)
                        foreach (List<IMultiplexed> queueRemove in _writeQueueToRemoves)
                            queueRemove.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _lockSocketsObject = null;
                _writeQueues = null;
                _writeQueueToRemoves = null;
                _pollWriterInt = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Multiplexer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
