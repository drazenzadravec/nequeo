/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Net.Data;
using Nequeo.Extension;

namespace Nequeo.Net.Message
{
    /// <summary>
    /// System log server.
    /// </summary>
    public class SyslogServer : IDisposable
    {
        /// <summary>
        /// System log server.
        /// </summary>
        /// <param name="systemLogPath">The system log path.</param>
        /// <param name="maxClient">The maximum number of connections.</param>
        public SyslogServer(SyslogPath systemLogPath, int maxClient = 50000)
        {
            if (systemLogPath == null) throw new ArgumentNullException(nameof(systemLogPath));

            _maxClient = maxClient;
            _systemLogPath = systemLogPath;
            _receiveSize = systemLogPath.MaximumReceiveSize;
            _fileSize = systemLogPath.MaximumFileSize;

            // Initialise the server.
            Init();
        }

        private object _writeLogLock = new object();

        private long _fileSize = 5000000;
        private int _receiveSize = 2048;
        private int _maxClient = 50000;

        private Nequeo.Net.ServerSingle _server = null;
        private Nequeo.Net.UdpServer _udpServer = null;
        private SyslogPath _systemLogPath = null;

        private ConcurrentDictionary<short, Stream> _syslogFiles = null;
        private ConcurrentDictionary<string, SyslogContext> _syslogList = null;

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                // Start the server.
                if (_server != null)
                    _server.Start();

                // Start the server.
                if (_udpServer != null)
                    _udpServer.Start();
            }
            catch (Exception)
            {
                // Close all files.
                CloseFiles();

                if (_server != null)
                    _server.Dispose();

                if (_udpServer != null)
                    _udpServer.Dispose();

                _server = null;
                _udpServer = null;
                throw;
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Close all files.
                CloseFiles();

                // Stop the server.
                if (_server != null)
                    _server.Stop();

                // Stop the server.
                if (_udpServer != null)
                    _udpServer.Stop();
            }
            catch { }
            finally
            {
                if (_server != null)
                    _server.Dispose();

                if (_udpServer != null)
                    _udpServer.Dispose();

                _server = null;
                _udpServer = null;
            }
        }

        /// <summary>
        /// Initialise the server.
        /// </summary>
        private void Init()
        {
            try
            {
                string socketProviderHostPrefix = "SyslogServerSingle_";
                string hostProviderFullName = socketProviderHostPrefix + "SocketProviderV6";

                // Get the certificate reader.
                Nequeo.Security.Configuration.Reader certificateReader = new Nequeo.Security.Configuration.Reader();
                Nequeo.Net.Configuration.Reader hostReader = new Nequeo.Net.Configuration.Reader();

                // Create the syslog context list.
                _syslogList = new ConcurrentDictionary<string, SyslogContext>();
                _syslogFiles = new ConcurrentDictionary<short, Stream>();

                // Open or create the log files.
                OpenFiles(Message.SyslogUtility.GetPriorities());

                // Create the server endpoint.
                Nequeo.Net.Sockets.MultiEndpointModel[] model = new Nequeo.Net.Sockets.MultiEndpointModel[]
                {
                    // None secure.
                    new Nequeo.Net.Sockets.MultiEndpointModel()
                    {
                        Port = hostReader.GetServerHost(hostProviderFullName).Port,
                        Addresses = new System.Net.IPAddress[]
                        {
                            System.Net.IPAddress.IPv6Any,
                            System.Net.IPAddress.Any
                        }
                    },
                };

                // Start the server.
                _server = new Nequeo.Net.ServerSingle(model, _maxClient);
                _server.OnContext += Server_OnContext;
                _server.Servers.OnClientDisconnected = (Provider.ISingleContextBase context) => Server_OnDisconnected(context);

                // Start the server.
                _udpServer = new Nequeo.Net.UdpServer(model, _maxClient);
                _udpServer.OnContext += Server_OnContext;
                _udpServer.Servers.OnClientDisconnected = (Nequeo.Net.Sockets.IUdpServerContext context) => ServerUdp_OnDisconnected(context);
            }
            catch (Exception)
            {
                // Close all files.
                CloseFiles();

                if (_server != null)
                    _server.Dispose();

                if (_udpServer != null)
                    _udpServer.Dispose();

                _server = null;
                _udpServer = null;
                throw;
            }
        }

        /// <summary>
        /// Disconnected client.
        /// </summary>
        /// <param name="context">The server context.</param>
        private void Server_OnDisconnected(Provider.ISingleContextBase context)
        {
            SyslogContext syslogContext = null;
            bool ret = _syslogList.TryRemove(context.ConnectionID, out syslogContext);

            // If found then release.
            if (ret)
            {
                // Release all resources.
                if (syslogContext != null)
                    syslogContext.Dispose();

                syslogContext = null;
            }
        }

        /// <summary>
        /// Disconnected client.
        /// </summary>
        /// <param name="context">The server context.</param>
        private void ServerUdp_OnDisconnected(Nequeo.Net.Sockets.IUdpServerContext context)
        {
            SyslogContext syslogContext = null;
            bool ret = _syslogList.TryRemove(context.ConnectionID, out syslogContext);

            // If found then release.
            if (ret)
            {
                // Release all resources.
                if (syslogContext != null)
                    syslogContext.Dispose();

                syslogContext = null;
            }
        }

        /// <summary>
        /// Client context hanlder.
        /// </summary>
        /// <param name="sender">The current server handling the message.</param>
        /// <param name="context">The currrent client web context.</param>
        private void Server_OnContext(object sender, WebContext context)
        {
            context.IsAsyncMode = true;
            SyslogContext syslogContext = new SyslogContext(context, _receiveSize);
            syslogContext.WriteLogHandler = (facility, severity, data) => WriteLog(facility, severity, data);
            _syslogList.TryAdd(context.ConnectionID, syslogContext);
        }

        /// <summary>
        /// Syslog context handler.
        /// </summary>
        internal class SyslogContext : IDisposable
        {
            /// <summary>
            /// Syslog context handler.
            /// </summary>
            /// <param name="context">The currrent client web context.</param>
            /// <param name="receiveSize">The maximum data receive size before truncating.</param>
            public SyslogContext(WebContext context, int receiveSize = 2048)
            {
                _priorityExtract = new byte[_priorityNumber];
                _versionExtract = new byte[_versionNumber];

                _receiveSize = receiveSize;
                _messageStore = new byte[_receiveSize];

                _context = context;
                _context.Context.ReceivedAsyncMode = () => AsyncReceived();
            }

            private int _receiveSize = 2048;
            private bool _isLastByte = false;

            private bool _messageStart = false;
            private bool _messageComplete = false;
            private byte[] _messageStore = null;
            private int _messageIndex = 0;

            private bool _isStart = false;
            private bool _isEnd = false;

            private short _facility = -1;
            private short _severity = -1;

            private int _length = 0;
            private short _version = 0;
            private int _versionCount = 0;
            private int _versionNumber = 10;
            private bool _versionFound = false;
            private byte[] _versionExtract = null;

            private short _priority = 0;
            private bool _priorityFound = false;
            private int _priorityNumber = 7;
            private int _priorityCount = 0;
            private byte[] _priorityExtract = null;

            private WebContext _context = null;
            internal Action<short, short, byte[]> WriteLogHandler = null;

            /// <summary>
            /// Receives data from the client in async mode.
            /// </summary>
            private void AsyncReceived()
            {
                // If active.
                if (_context != null && _context.WebRequest != null && _context.WebRequest.Input != null)
                {
                    // If data exists.
                    if (_context.WebRequest.Input.Length > 0)
                    {
                        // Syslog message format '<priority>[version][_MessageLength] header message'.
                        // If using TCP then the _MessageLength is manditory, for UDP _MessageLength
                        // is optional.
                        byte[] buffer = new byte[_priorityNumber + _versionNumber + 1 + _receiveSize];

                        // Read the incomming log data.
                        int count = _context.WebRequest.Input.Read(buffer, 0, buffer.Length);

                        // For each byte read.
                        for (int i = 0; i < count; i++)
                        {
                            // If not start.
                            if (!_isStart)
                            {
                                // Find the start of the priority message.
                                if (buffer[i] == '<')
                                {
                                    _isStart = true;
                                    _priorityExtract[_priorityCount] = (byte)'<';
                                    _priorityCount++;

                                    // If the start is the last byte.
                                    if (i == (count - 1))
                                        _isLastByte = true;

                                    continue;
                                }
                            }

                            // If not end.
                            if (!_isEnd && _isStart)
                            {
                                // Find the end of the priority message.
                                if (buffer[i] == '>')
                                {
                                    _isEnd = true;
                                    _priorityExtract[_priorityCount] = (byte)'>';
                                    continue;
                                }
                            }

                            // If start of message has been found.
                            if (_isStart && !_priorityFound)
                            {
                                // If not end.
                                if (!_isEnd)
                                {
                                    _priorityExtract[_priorityCount] = buffer[i];
                                    _priorityCount++;
                                }

                                // If priority count is greater than number then
                                // not a priority.
                                if (_priorityCount > _priorityNumber - 1)
                                {
                                    _isStart = false;
                                    _isEnd = false;
                                    _priorityCount = 0;
                                    _priorityFound = false;
                                }
                            }

                            // If end of message has been found.
                            if (_isEnd && !_priorityFound)
                            {
                                // Get the priority.
                                _priorityFound = true;
                                string priorityValue = "";

                                // For each byte priority.
                                for (int j = 1; j < _priorityCount; j++)
                                {
                                    // Get the priority.
                                    priorityValue += Convert.ToChar(_priorityExtract[j]).ToString();
                                }

                                // Get the priority, facility and severity.
                                _priority = Convert.ToInt16(priorityValue.ToString());
                                _facility = SyslogUtility.GetFacility(_priority);
                                _severity = SyslogUtility.GetSeverity(_priority);
                            }

                            // If the message priority has been found.
                            if (_priorityFound)
                            {
                                // Keep count until the header
                                // used to find the version number.
                                _versionExtract[_versionCount] = buffer[i];
                                _versionCount++;

                                // Find the start of the header.
                                // A space after the version (if exists) number
                                // is the start of the header.
                                if (buffer[i] == ' ')
                                {
                                    // Found version if exists.
                                    _versionFound = true;
                                    _versionCount--;
                                }

                                // If found version, find out if it exits.
                                if (_versionFound)
                                {
                                    // Get the version if exists
                                    if (_versionCount > 0)
                                    {
                                        string versionValue = "";

                                        // For each byte priority.
                                        for (int j = 0; j < _versionCount; j++)
                                        {
                                            // Get the priority.
                                            versionValue += Convert.ToChar(_versionExtract[j]).ToString();
                                        }

                                        // Attempt to spit the version and message lenth.
                                        string[] split = versionValue.Split(new char[] { '_' }, StringSplitOptions.None);
                                        if (split.Length > 1)
                                        {
                                            // Contains a version and message length.
                                            // May contain a version number.
                                            if (!String.IsNullOrEmpty(split[0]))
                                            {
                                                // Get the version.
                                                _version = Convert.ToInt16(split[0]);
                                            }

                                            // Get the message length.
                                            _length = Convert.ToInt32(split[1]);
                                        }
                                        else
                                        {
                                            // Get the version.
                                            _version = Convert.ToInt16(split[0]);
                                            _length = 0;
                                        }
                                    }

                                    // Reset the search.
                                    _isStart = false;
                                    _isEnd = false;
                                    _priorityCount = 0;
                                    _priorityFound = false;
                                    _versionCount = 0;
                                    _versionFound = false;

                                    // Indicates the begining of the message.
                                    _messageIndex = 0;
                                    _messageStart = true;
                                    _messageComplete = false;
                                }
                            }
                            else
                            {
                                // If the start of the message has been found.
                                if (_messageStart)
                                {
                                    // If the complete message has not been found.
                                    if (!_messageComplete)
                                    {
                                        // Add the message data to the store.
                                        _messageStore[_messageIndex] = buffer[i];
                                        _messageIndex++;

                                        // If a length exists then TCP wait for the message.
                                        if (_length > 0)
                                        {
                                            // The message is complete.
                                            if ((_messageIndex == _receiveSize) || (_messageIndex == _length))
                                            {
                                                // If a facility and severity has been found.
                                                if (_facility >= 0 && _severity >= 0)
                                                {
                                                    // Write the header and message for the current
                                                    // priority, facility and severity.
                                                    WriteLog(_facility, _severity,
                                                        (_length <= _receiveSize ? _messageStore.Take(_length).ToArray() : _messageStore.Take(_receiveSize).ToArray()));
                                                }

                                                // Reset the message handler.
                                                _messageIndex = 0;
                                                _messageStart = false;
                                                _messageComplete = true;
                                            }
                                        }
                                        else
                                        {
                                            // If no length exists then UDP not not wait,
                                            // write what has been received or get the
                                            // message until the start of a new message.
                                            // If start or then end of the data.
                                            if (_isStart || i == (count - 1))
                                            {
                                                // If a facility and severity has been found.
                                                if (_facility >= 0 && _severity >= 0)
                                                {
                                                    // Write the header and message for the current
                                                    // priority, facility and severity.
                                                    WriteLog(_facility, _severity, _messageStore.Take(_messageIndex).ToArray());
                                                }

                                                // Reset the message handler.
                                                _messageIndex = 0;
                                                _messageStart = false;
                                                _messageComplete = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // If the last byte is '<' and length is not specified.
                        if(_isLastByte && _length == 0)
                        {
                            _isLastByte = false;

                            // If a facility and severity has been found.
                            if (_facility >= 0 && _severity >= 0)
                            {
                                // Write the header and message for the current
                                // priority, facility and severity.
                                WriteLog(_facility, _severity, _messageStore.Take(_messageIndex).ToArray());
                            }

                            // Reset the message handler.
                            _messageIndex = 0;
                            _messageStart = false;
                            _messageComplete = true;
                        }
                    }
                }
            }

            /// <summary>
            /// Write the data to the log entity.
            /// </summary>
            /// <param name="facility">The syslog facility.</param>
            /// <param name="severity">The syslog severity.</param>
            /// <param name="data">The data to write.</param>
            private void WriteLog(short facility, short severity, byte[] data)
            {
                if(WriteLogHandler != null)
                {
                    // Get the end of the line
                    byte[] crlf = Encoding.Default.GetBytes("\r\n");

                    // Write the log data to the handler.
                    WriteLogHandler(facility, severity, data.Combine(crlf));
                }
            }

            #region Dispose Object Methods
            /// <summary>
            /// Track whether Dispose has been called.
            /// </summary>
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
            /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
            /// equals true, the method has been called directly or indirectly by a user's
            /// code. Managed and unmanaged resources can be disposed.  If disposing equals
            /// false, the method has been called by the runtime from inside the finalizer
            /// and you should not reference other objects. Only unmanaged resources can
            /// be disposed.
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
                    }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    _versionExtract = null;
                    _priorityExtract = null;
                    _messageStore = null;
                }
            }

            /// <summary>
            /// Use C# destructor syntax for finalization code.
            /// This destructor will run only if the Dispose method
            /// does not get called.
            /// It gives your base class the opportunity to finalize.
            /// Do not provide destructors in types derived from this class.
            /// </summary>
            ~SyslogContext()
            {
                // Do not re-create Dispose clean-up code here.
                // Calling Dispose(false) is optimal in terms of
                // readability and maintainability.
                Dispose(false);
            }
            #endregion
        }

        /// <summary>
        /// Write the data to the log entity.
        /// </summary>
        /// <param name="facility">The syslog facility.</param>
        /// <param name="severity">The syslog severity.</param>
        /// <param name="data">The data to write.</param>
        private void WriteLog(short facility, short severity, byte[] data)
        {
            // Make sure data exists.
            if (data != null && data.Length > 0)
            {
                // Only allow one thread at a time
                // to write data to a log file.
                lock (_writeLogLock)
                {
                    // Get the current priority.
                    short priority = SyslogUtility.GetPriority(facility, severity);

                    // Determine if the priority file should be archived.
                    ArchiveFile(priority);

                    // Write the data.
                    Write(priority, data);
                }
            }
        }

        /// <summary>
        /// Write the data to the log entity.
        /// </summary>
        /// <param name="priority">The current log priority.</param>
        /// <param name="data">The data to write.</param>
        private void Write(short priority, byte[] data)
        {
            try
            {
                // Try and get the file.
                Stream stream = null;
                bool ret = _syslogFiles.TryGetValue(priority, out stream);
                if (ret)
                {
                    // Write the data.
                    stream.Write(data, 0, data.Length);
                }
                else
                    // File can not be found.
                    throw new Exception();
            }
            catch
            {
                try
                {
                    // Attempt to erite to the internal error log.
                    short prioritySystem = SyslogUtility.GetPriority(5, 2);

                    // Try and get the file.
                    Stream fileSystem = null;
                    bool retSystem = _syslogFiles.TryGetValue(prioritySystem, out fileSystem);
                    if (retSystem)
                    {
                        // Write to this log.
                        DateTime timeSystem = DateTime.Now;
                        byte[] buffer = Encoding.Default.GetBytes(
                            timeSystem.ToShortDateString() + " " + timeSystem.ToShortTimeString() + " " +
                            Environment.MachineName + " " + "Could not open priority log file : " + priority.ToString() + "\r\n");

                        // Write the data to the file.
                        fileSystem.Write(buffer, 0, buffer.Length);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Archive the priority file.
        /// </summary>
        /// <param name="priority">The current log priority.</param>
        private void ArchiveFile(short priority)
        {
            try
            {
                // Try and get the file.
                Stream stream = null;
                bool ret = _syslogFiles.TryGetValue(priority, out stream);
                if (ret)
                {
                    // Get the file name.
                    string directory = _systemLogPath.Path.TrimEnd(new char[] { '\\' });
                    string file = directory + "\\" + priority.ToString() + ".log";

                    // If the maximum file size has been reached.
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Length >= _systemLogPath.MaximumFileSize)
                    {
                        // Archive the file.
                        try
                        {
                            // Release the file.
                            stream.Close();
                            stream.Dispose();
                        }
                        catch { }

                        try
                        {
                            // Get the archive file.
                            DateTime dateTimeArchive = DateTime.Now;
                            string directoryArchive = _systemLogPath.ArchivePath.TrimEnd(new char[] { '\\' }) + "\\" +
                                dateTimeArchive.Year.ToString() + "\\" +
                                dateTimeArchive.Month.ToString() + "\\" +
                                dateTimeArchive.Day.ToString();

                            // Get the file archive.
                            string fileArchive = directoryArchive + "\\" + priority.ToString() + "_" + 
                                dateTimeArchive.Hour.ToString() + "-" +
                                dateTimeArchive.Minute.ToString() + "-" + 
                                dateTimeArchive.Second.ToString() + ".log";

                            // If directory does not exists.
                            if (!Directory.Exists(directoryArchive))
                            {
                                // Create the directory.
                                Directory.CreateDirectory(directoryArchive);
                            }

                            // Move the file.
                            File.Move(file, fileArchive);
                        }
                        catch { }

                        try
                        {
                            // Create a new priority log file.
                            // Add the file to the files list.
                            _syslogFiles.TryUpdate(priority, new FileStream(file, FileMode.Append, FileAccess.Write), stream);
                        }
                        catch { }
                    }
                }
                else
                    // File can not be found.
                    throw new Exception();
            }
            catch 
            {
                try
                {
                    // Attempt to erite to the internal error log.
                    short prioritySystem = SyslogUtility.GetPriority(5, 2);

                    // Try and get the file.
                    Stream fileSystem = null;
                    bool retSystem = _syslogFiles.TryGetValue(prioritySystem, out fileSystem);
                    if (retSystem)
                    {
                        // Write to this log.
                        DateTime timeSystem = DateTime.Now;
                        byte[] buffer = Encoding.Default.GetBytes(
                            timeSystem.ToShortDateString() + " " + timeSystem.ToShortTimeString() + " " +
                            Environment.MachineName + " " + "Could not open priority log file : " + priority.ToString() + "\r\n");

                        // Write the data to the file.
                        fileSystem.Write(buffer, 0, buffer.Length);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Open or create the log files.
        /// </summary>
        /// <param name="priorities">The priorities.</param>
        private void OpenFiles(short[] priorities)
        {
            // Get the log base path.
            string directory = _systemLogPath.Path.TrimEnd(new char[] { '\\' });

            // If directory does not exists.
            if (!Directory.Exists(directory))
            {
                // Create the directory.
                Directory.CreateDirectory(directory);
            }

            // For each priority.
            foreach (short priority in priorities)
            {
                // The file name.
                string file = directory + "\\" + priority.ToString() + ".log";

                // Add the file to the files list.
                _syslogFiles.TryAdd(priority, new FileStream(file, FileMode.Append, FileAccess.Write));
            }
        }

        /// <summary>
        /// Close all files.
        /// </summary>
        private void CloseFiles()
        {
            // If active.
            if (_syslogFiles != null)
            {
                // For each file.
                foreach (KeyValuePair<short, Stream> item in _syslogFiles)
                {
                    try
                    {
                        Stream file = null;
                        bool ret = _syslogFiles.TryGetValue(item.Key, out file);
                        if (ret)
                        {
                            // Release the file.
                            file.Close();
                            file.Dispose();
                        }
                    }
                    catch { }
                }

                // Clear the list.
                _syslogFiles.Clear();
                _syslogFiles = null;
            }
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
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
        /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
        /// equals true, the method has been called directly or indirectly by a user's
        /// code. Managed and unmanaged resources can be disposed.  If disposing equals
        /// false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can
        /// be disposed.
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
                    if (_server != null)
                        _server.Dispose();

                    if (_udpServer != null)
                        _udpServer.Dispose();

                    if (_syslogList != null)
                        _syslogList.Clear();

                    // Close all files.
                    CloseFiles();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _server = null;
                _udpServer = null;
                _syslogList = null;
                _syslogFiles = null;
                _writeLogLock = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~SyslogServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
