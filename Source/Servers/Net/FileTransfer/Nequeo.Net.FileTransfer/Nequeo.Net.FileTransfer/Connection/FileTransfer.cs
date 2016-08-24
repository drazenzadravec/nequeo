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
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Nequeo.Net.FileTransfer.Common;
using Nequeo.Handler;
using Nequeo.Security;

namespace Nequeo.Net.FileTransfer.Connection
{
    /// <summary>
    /// Delegate that handles commands received from the client.
    /// </summary>
    /// <param name="sender">The current object sending the data.</param>
    /// <param name="dataReceived">The command data send by the client.</param>
    internal delegate void FileTransferReceiveHandler(FileTransferConnection sender, string dataReceived);

    /// <summary>
    /// Class that contains the data for the current tcp client 
    /// connected to the server.
    /// </summary>
    internal class FileTransferConnection : ServiceBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tcpClient">The tcp client channel connection from server to client.</param>
        public FileTransferConnection(TcpClient tcpClient)
        {
            try
            {
                // The tcp client to server channel.
                // Assign the network stream from the client
                // channel stream.
                this.tcpClient = tcpClient;
                this.networkStream = tcpClient.GetStream();

                // Create a new state object.
                ServerSocketState state = new ServerSocketState();
                state.NetworkStream = this.networkStream;

                // This starts the asynchronous read thread. 
                // The data will be saved into readBuffer.
                this.networkStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                    new AsyncCallback(DataReceiver), state);

                // Send a welcome message to the client.
                WriteCommand("WELC 101;Welcome to File Transfer Server - Authorized Accounts Only", networkStream);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "Constructor", e.Message,
                    59, WriteTo.EventLog, LogType.Error);
            }
        }
        #endregion

        #region Constants
        private const int READ_BUFFER_SIZE = 8192;
        private const int WRITE_BUFFER_SIZE = 8192;
        #endregion

        #region Private Fields
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
        private byte[] writeBuffer = new byte[WRITE_BUFFER_SIZE];

        private string connectionName = string.Empty;
        private bool clientConnected = false;
        private bool downloadingFile = false;
        private bool sendReply = false;
        private bool stopSending = false;
        private string rootDirectory = string.Empty;
        private string currentDirectory = string.Empty;

        private string targetFile = string.Empty;
        private string destinationFile = string.Empty;
        private long targetFileSize = 0;
        private long destinationFileSize = 0;

        private long readLoopCount = 0;
        private long readDownLoadLoopCount = 0;
        private int clientIndex = 0;

        private Timer _timeOut = null;
        private int _timeOutInterval = -1;
        #endregion

        #region Public Events
        /// <summary>
        /// The on data received event handler.
        /// </summary>
        public event FileTransferReceiveHandler OnDataReceived;
        #endregion

        #region Public Properties
        /// <summary>
        /// Sets, the client time out interval.
        /// </summary>
        public int TimeOut
        {
            set
            {
                _timeOutInterval = value;
                if (_timeOutInterval > -1)
                {
                    _timeOut = new Timer(ClientTimedOut, null,
                        new TimeSpan(0, _timeOutInterval, 0),
                        new TimeSpan(0, _timeOutInterval, 0));
                }
            }
        }

        /// <summary>
        /// Gets sets, the current client index.
        /// </summary>
        public int ClientIndex
        {
            get { return clientIndex; }
            set { clientIndex = value; }
        }

        /// <summary>
        /// Gets sets, the unique user name
        /// currenlty connected.
        /// </summary>
        public string ConnectionName
        {
            get { return connectionName; }
            set { connectionName = value; }
        }

        /// <summary>
        /// Gets sets, the root directory path for the user.
        /// </summary>
        public string RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }

        /// <summary>
        /// Gets sets, the current sub directory path.
        /// </summary>
        public string CurrentDirectory
        {
            get { return currentDirectory; }
            set { currentDirectory = value; }
        }

        /// <summary>
        /// Gets sets, current connection status
        /// for the current user.
        /// </summary>
        public bool Connected
        {
            get { return clientConnected; }
            set { clientConnected = value; }
        }

        /// <summary>
        /// Gets sets, should the server send a reply
        /// to the current client.
        /// </summary>
        public bool SendReply
        {
            get { return sendReply; }
            set { sendReply = value; }
        }

        /// <summary>
        /// Gets sets, stop sending the file.
        /// </summary>
        public bool StopSending
        {
            get { return stopSending; }
            set { stopSending = value; }
        }

        /// <summary>
        /// Gets sets, is the server in download mode,
        /// that is, is the server receiving data.
        /// </summary>
        public bool DownloadingFile
        {
            get { return downloadingFile; }
            set { downloadingFile = value; }
        }

        /// <summary>
        /// Gets sets, the file that is being send.
        /// </summary>
        public string TargetFile
        {
            get { return targetFile; }
            set { targetFile = value; }
        }

        /// <summary>
        /// Gets sest, the file that is being received.
        /// </summary>
        public string DestinationFile
        {
            get { return destinationFile; }
            set { destinationFile = value; }
        }

        /// <summary>
        /// Gets sets, the upload file size.
        /// </summary>
        public long TargetFileSize
        {
            get { return targetFileSize; }
            set { targetFileSize = value; }
        }

        /// <summary>
        /// Gets sets, the download file size.
        /// </summary>
        public long DestinationFileSize
        {
            get { return destinationFileSize; }
            set { destinationFileSize = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Disconnects the current client.
        /// </summary>
        /// <param name="state">The current state object.</param>
        public void Disconnect(ServerSocketState state)
        {
            try
            {
                if (this.tcpClient != null)
                {
                    this.tcpClient.Client.Shutdown(SocketShutdown.Both);
                    this.tcpClient.Client.Disconnect(false);
                    this.tcpClient.Close();
                    this.networkStream.Close();
                }

                tcpClient = null;
                networkStream = null;
            }
            catch { }

            if (state != null)
            {
                try
                {
                    // Get the current network stream from the
                    // asynchronus result state object.
                    ServerSocketState stateObject = (ServerSocketState)state;

                    // Get the current file stream.
                    FileStream fileStream = stateObject.FileStream;

                    // Close the file and and dispose
                    // of the object.
                    if (fileStream != null)
                    {
                        // Close the file.
                        fileStream.Close();

                        // Delete the current upload file.
                        File.Delete(destinationFile);

                        // Release all resources
                        fileStream.Dispose();
                    }
                }
                catch { }
            }

            // Send a single to end the connection.
            if (OnDataReceived != null)
                OnDataReceived(this, "ENDC");
        }

        /// <summary>
        /// Sends a command to the client.
        /// </summary>
        /// <param name="data">The command and data to send to the client.</param>
        public void SendCommand(string data)
        {
            WriteCommand(data, networkStream);
        }

        /// <summary>
        /// Starts the upload process.
        /// </summary>
        public void Upload()
        {
            FileStream fileStream = null;
            ServerSocketState state = new ServerSocketState();

            try
            {
                // Open the upload file.
                fileStream = new FileStream(targetFile,
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Create a new file state object
                // assign the current network stream
                // and the upload file stream.
                state.NetworkStream = networkStream;
                state.FileStream = fileStream;

                // Get the first set of bytes from
                // the upload file.
                int readBytes = fileStream.Read(writeBuffer, 0, WRITE_BUFFER_SIZE);

                // Begin a new asynochronus write operation
                // to send the client the requested file.
                lock (networkStream)
                    networkStream.BeginWrite(writeBuffer, 0, readBytes,
                        new AsyncCallback(UploadFile), state);
            }
            catch (Exception e)
            {
                if (fileStream != null)
                    fileStream.Close();

                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "Upload", e.Message,
                    484, WriteTo.EventLog, LogType.Error);

                // On any error close the connection.
                state.FileStream = null;
                Disconnect(state);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Disconnects the current client after the time out
        /// interval has elapsed.
        /// </summary>
        /// <param name="state">A passed object state.</param>
        private void ClientTimedOut(object state)
        {
            if (_timeOut != null)
                _timeOut.Dispose();

            Disconnect(null);
        }

        /// <summary>
        /// Data received asynchronus result method, all client commands
        /// are processed through this asynchronus result method.
        /// </summary>
        /// <param name="ar">The current asynchronus result.</param>
        private void DataReceiver(IAsyncResult ar)
        {
            int bytesRead;

            // Get the current network stream from the
            // asynchronus result state object.
            ServerSocketState state = (ServerSocketState)ar.AsyncState;
            NetworkStream nStream = state.NetworkStream;

            try
            {
                // If the current operation is downloading data.
                if (downloadingFile)
                {
                    // Reset download loop read count.
                    readDownLoadLoopCount = 0;

                    // Start the asynchronus download operation.
                    DownloadFile(ar);
                }
                else
                {
                    // Close the file.
                    if (state.FileStream != null)
                        state.FileStream.Close();

                    // Clean up the file stream.
                    state.FileStream = null;

                    // Finish asynchronous read into readBuffer 
                    // and get number of bytes read.
                    lock (nStream)
                        bytesRead = nStream.EndRead(ar);

                    if (bytesRead > 0)
                    {
                        // Convert the byte array the message command.
                        // Trigger the data receiver delegate and send
                        // the command and data to the host.
                        try
                        {
                            string command = Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
                            OnDataReceived(this, command);

                            // If the time out control has been created
                            // then reset the timer.
                            if (_timeOut != null)
                                _timeOut.Change(
                                    new TimeSpan(0, _timeOutInterval, 0),
                                    new TimeSpan(0, _timeOutInterval, 0));
                        }
                        catch { }
                    }
                    else
                    {
                        // If the connection is closed
                        // then throw a new exception.
                        if (!tcpClient.Connected)
                            throw new Exception("Connection has closed.");

                        // Start counting loops.
                        readLoopCount++;

                        // If count greater than
                        // 10 then throw exception
                        // and close the connection.
                        if (readLoopCount > 4)
                            throw new Exception("Connection has been disconnected, read loop detected. " +
                            "Connection has timed out or, may occur when client calls Shutdown(SocketShutdown.Both)");
                    }

                    // Start a new asynchronous read into readBuffer.
                    // more data maybe present. This will wait for more
                    // data from the client.
                    lock (nStream)
                        nStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                            new AsyncCallback(DataReceiver), state);
                }
            }
            catch (IOException ioe)
            {
                // Handles the forced disconnection such as
                // exiting a client program without closing
                // the connection.
                string error = ioe.Message;

                // On any error close the connection.
                Disconnect(state);
            }
            catch (ObjectDisposedException ode)
            {
                // Handles the exception when the client
                // closes the current network channel
                // and disposes of the current connection
                // to client class, this object.
                string error = ode.Message;

                // On any error close the connection.
                Disconnect(state);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "DataReceiver", e.Message,
                    236, WriteTo.EventLog, LogType.Error);

                // On any error close the connection.
                Disconnect(state);
            }
        }

        /// <summary>
        /// This method handles the download asynchronus result
        /// when the server receives a file from the client.
        /// </summary>
        /// <param name="ar">The current asynchronus result</param>
        private void DownloadFile(IAsyncResult ar)
        {
            // Get the current network stream from the
            // asynchronus result state object.
            ServerSocketState state = (ServerSocketState)ar.AsyncState;
            NetworkStream nStream = state.NetworkStream;

            long totalBytesRead = 0;
            FileStream fileStream = null;

            try
            {
                // If the file state file stream has
                // been created then get the stream
                // from the state object.
                if (state.FileStream != null)
                {
                    fileStream = state.FileStream;
                    totalBytesRead = state.TotalBytesRead;
                }
                else
                {
                    // Create a new download file stream.
                    fileStream = new FileStream(destinationFile,
                        FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                    // Get the network stream and file stream
                    // form the state object.
                    state.NetworkStream = nStream;
                    state.FileStream = fileStream;
                    state.TotalBytesRead = 0;
                }
            }
            catch (Exception e)
            {
                // Stop the download.
                downloadingFile = false;

                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "DownloadFile", e.Message,
                    333, WriteTo.EventLog, LogType.Error);
            }

            try
            {
                int bytesRead;
                // End the current read process.

                lock (nStream)
                    bytesRead = nStream.EndRead(ar);

                if (bytesRead < 1)
                {
                    // If the connection is closed
                    // then throw a new exception.
                    if (!tcpClient.Connected)
                        throw new Exception("Connection has closed.");

                    // Start counting loops.
                    readDownLoadLoopCount++;

                    // If count greater than
                    // 10 then throw exception
                    // and close the connection.
                    if (readDownLoadLoopCount > 4)
                        throw new Exception("Connection has been disconnected, read loop detected. " +
                            "Connection has timed out or, may occur when client calls Shutdown(SocketShutdown.Both)");
                }

                // If not the first time reading
                // data from the network stream
                // the load the data received
                // to the file.
                fileStream.Write(readBuffer, 0, bytesRead);
                totalBytesRead += (long)bytesRead;

                // Download the current data.
                totalBytesRead += DownloadFileEx(fileStream, nStream, totalBytesRead);
            }
            catch (IOException ioe)
            {
                // Handles the forced disconnection such as
                // exiting a client program without closing
                // the connection.
                string error = ioe.Message;

                // Stop the download.
                downloadingFile = false;
            }
            catch (ObjectDisposedException ode)
            {
                // Handles the exception when the client
                // closes the current network channel
                // and disposes of the current connection
                // to client class, this object.
                string error = ode.Message;

                // Stop the download.
                downloadingFile = false;
            }
            catch (Exception e)
            {
                // Stop the download.
                downloadingFile = false;

                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "DownloadFile", e.Message,
                    286, WriteTo.EventLog, LogType.Error);
            }

            try
            {
                // If the download file is the same size
                // as the file current being written then
                // the download has completed and data has
                // been received.
                if (destinationFileSize == totalBytesRead)
                    downloadingFile = false;

                // If all data has been read.
                if (!downloadingFile)
                {
                    // Close the file.
                    if (fileStream != null)
                        fileStream.Close();

                    // Release the object.
                    fileStream = null;
                    state.FileStream = null;

                    // Start a new asynchronous read into readBuffer.
                    // more data maybe present. This will wait for more
                    // data from the client.
                    System.Threading.Thread.Sleep(5);
                    lock (nStream)
                        nStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                            new AsyncCallback(DataReceiver), state);

                    // Send to the client a command indicating that
                    // all the data has been received.
                    System.Threading.Thread.Sleep(5);
                    WriteCommand("CDDN 100", nStream);
                }
                else
                {
                    // The current bytes read in this async threa.
                    state.TotalBytesRead = totalBytesRead;

                    // Start a new asynchronous read into, more
                    // of the file being download exists so read
                    // from the network stream until all data 
                    // has been received.
                    lock (nStream)
                        nStream.BeginRead(readBuffer, 0, READ_BUFFER_SIZE,
                            new AsyncCallback(DownloadFile), state);
                }
            }
            catch (IOException ioe)
            {
                // Handles the forced disconnection such as
                // exiting a client program without closing
                // the connection.
                string error = ioe.Message;

                // On any error close the connection.
                Disconnect(state);
            }
            catch (ObjectDisposedException ode)
            {
                // Handles the exception when the client
                // closes the current network channel
                // and disposes of the current connection
                // to client class, this object.
                string error = ode.Message;

                // On any error close the connection.
                Disconnect(state);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "DownloadFile", e.Message,
                    368, WriteTo.EventLog, LogType.Error);

                // On any error close the connection.
                Disconnect(state);
            }
        }

        /// <summary>
        /// Load the data into the file.
        /// </summary>
        /// <param name="fileStream">The current file stream object.</param>
        /// <param name="nStream">The current network client stream.</param>
        /// <param name="numberBytesRead">The current number of bytes read.</param>
        private long DownloadFileEx(FileStream fileStream, NetworkStream nStream, long numberBytesRead)
        {
            int readBytes = 0;
            long totalBytesRead = 0;
            const int bufferLength = READ_BUFFER_SIZE;
            byte[] buffer = new byte[READ_BUFFER_SIZE];

            // If client is connected and the number of bytes read
            // is less than the total size then read all bytes.
            while ((tcpClient.Connected) &&
                (totalBytesRead + numberBytesRead < destinationFileSize))
            {
                // Read all the data available in the network stream
                // and write the data to the file stream.
                while (nStream.DataAvailable)
                {
                    readBytes = nStream.Read(buffer, 0, bufferLength);
                    fileStream.Write(buffer, 0, readBytes);
                    totalBytesRead += (long)readBytes;

                    // If the time out control has been created
                    // then reset the timer.
                    if (_timeOut != null)
                        _timeOut.Change(
                            new TimeSpan(0, _timeOutInterval, 0),
                            new TimeSpan(0, _timeOutInterval, 0));
                }

                // Allow any other threads to work by
                // changing context.
                System.Threading.Thread.Sleep(2);
            }

            // Return the number of byters read.
            return totalBytesRead;
        }

        /// <summary>
        /// This method handles the upload asynchronus result
        /// when the server sends a file to the client.
        /// </summary>
        /// <param name="ar">The current asynchronus result</param>
        private void UploadFile(IAsyncResult ar)
        {
            // Get the current network stream from the
            // asynchronus result state object.
            ServerSocketState state = (ServerSocketState)ar.AsyncState;
            NetworkStream nStream = state.NetworkStream;
            FileStream fileStream = state.FileStream;

            // End the current asynchronus write
            // operation.
            lock (nStream)
                nStream.EndWrite(ar);

            try
            {
                int readBytes = 0;
                const int bufferLength = WRITE_BUFFER_SIZE;
                byte[] buffer = new byte[bufferLength];

                // Read all the data in the upload file and
                // send the data from the file to the client 
                // through the current network stream.
                do
                {
                    readBytes = fileStream.Read(buffer, 0, bufferLength);
                    nStream.Write(buffer, 0, readBytes);

                    // If the time out control has been created
                    // then reset the timer.
                    if (_timeOut != null)
                        _timeOut.Change(
                            new TimeSpan(0, _timeOutInterval, 0),
                            new TimeSpan(0, _timeOutInterval, 0));
                }
                while (readBytes != 0);

                // Close the current upload file.
                fileStream.Close();
            }
            catch (IOException ioe)
            {
                // Handles the forced disconnection such as
                // exiting a client program without closing
                // the connection.
                string error = ioe.Message;

                // On any error close the connection.
                state.FileStream = null;
                Disconnect(state);
            }
            catch (ObjectDisposedException ode)
            {
                // Handles the exception when the client
                // closes the current network channel
                // and disposes of the current connection
                // to client class, this object.
                string error = ode.Message;

                // On any error close the connection.
                state.FileStream = null;
                Disconnect(state);
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "UploadFile", e.Message,
                    484, WriteTo.EventLog, LogType.Error);

                // On any error close the connection.
                state.FileStream = null;
                Disconnect(state);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// Sends a command and data to the current client.
        /// </summary>
        /// <param name="command">The command and data to write.</param>
        /// <param name="stream">The current client network stream.</param>
        private void WriteCommand(string command, NetworkStream stream)
        {
            StreamWriter writer = null;

            try
            {
                // Write the command to the client.
                lock (stream)
                {
                    writer = new StreamWriter(stream);
                    writer.Write(command);
                    writer.Flush();
                    writer = null;
                }
            }
            catch (Exception e)
            {
                // Detect a thread abort exception.
                if (e is ThreadAbortException)
                    Thread.ResetAbort();

                base.Write("FileTransferConnection", "WriteCommand", e.Message,
                    542, WriteTo.EventLog, LogType.Error);
            }
            finally
            {
                if (writer != null)
                    writer = null;
            }
        }
        #endregion
    }
}
