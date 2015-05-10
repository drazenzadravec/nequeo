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

namespace Nequeo.Custom
{
    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    /// <typeparam name="T">The type of result to store.</typeparam>
    public class AsyncGenericResultArgs<T> : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="result">The result object</param>
        public AsyncGenericResultArgs(T result)
        {
            _result = result;
        }
        #endregion

        #region Private Fields
        // The result that is received from the server.
        private T _result = default(T);
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the result that is received from the server.
        /// </summary>
        public T Result
        {
            get { return _result; }
        }
        #endregion
    }

    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class ClientCommandArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="command">The command that is received from the server.</param>
        /// <param name="data">The data that is received from the server.</param>
        /// <param name="code">The code that is received from the server.</param>
        public ClientCommandArgs(string command, string data, long code)
        {
            this.command = command;
            this.data = data;
            this.code = code;
        }
        #endregion

        #region Private Fields
        // The command that is received from the server.
        private string command = string.Empty;
        // The data that is received from the server.
        private string data = string.Empty;
        // The code that is received from the server.
        private long code = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the command that is received from the server.
        /// </summary>
        public string Command
        {
            get { return command; }
        }

        /// <summary>
        /// Contains the data that is received from the server.
        /// </summary>
        public string Data
        {
            get { return data; }
        }

        /// <summary>
        /// Contains the code that is received from the server.
        /// </summary>
        public long Code
        {
            get { return code; }
        }
        #endregion
    }

    /// <summary>
    /// Operation argument class containing event handler
    /// information for the server operation delegate.
    /// </summary>
    public class ClientOperationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the operation event argument.
        /// </summary>
        /// <param name="localFile">The local file name.</param>
        /// <param name="remoteFile">The remote file name.</param>
        /// <param name="fileSize">The file size.</param>
        public ClientOperationArgs(string localFile, string remoteFile, long fileSize)
        {
            this.localFile = localFile;
            this.remoteFile = remoteFile;
            this.fileSize = fileSize;
        }
        #endregion

        #region Private Fields
        // The local file name.
        private string localFile = string.Empty;
        // The remote file name.
        private string remoteFile = string.Empty;
        // The file size.
        private long fileSize = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the local file name.
        /// </summary>
        public string LocalFile
        {
            get { return localFile; }
        }

        /// <summary>
        /// Contains the remote file name.
        /// </summary>
        public string RemoteFile
        {
            get { return remoteFile; }
        }

        /// <summary>
        /// Contains the file size.
        /// </summary>
        public long FileSize
        {
            get { return fileSize; }
        }
        #endregion
    }

    /// <summary>
    /// Progress argument class containing event handler
    /// information for the server progress delegate.
    /// </summary>
    public class ClientProgressArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the progress event argument.
        /// </summary>
        /// <param name="size">The current number of 
        /// bytes sent or received.</param>
        public ClientProgressArgs(long size)
        {
            this.size = size;
        }
        #endregion

        #region Private Fields
        // The current number of bytes 
        // sent or received.
        private long size = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains The current number of bytes 
        /// sent or received.
        /// </summary>
        public long Size
        {
            get { return size; }
        }
        #endregion
    }

    /// <summary>
    /// Thread error argument class containing event handler
    /// information for the server thread error delegate.
    /// </summary>
    public class ClientThreadErrorArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the thread error event argument.
        /// </summary>
        /// <param name="data">The current thread error.</param>
        /// <param name="code">The code that is received from the server.</param>
        public ClientThreadErrorArgs(string data, long code)
        {
            this.data = data;
            this.code = code;
        }
        #endregion

        #region Private Fields
        // current thread error.
        private string data = string.Empty;
        // The code that is received from the server.
        private long code = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the current thread error. 
        /// sent or received.
        /// </summary>
        public string Data
        {
            get { return data; }
        }

        /// <summary>
        /// Contains the code that is received from the server.
        /// </summary>
        public long Code
        {
            get { return code; }
        }
        #endregion
    }

    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class MessageArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="message">The message that is received from the server.</param>
        public MessageArgs(string message)
        {
            this.message = message;
        }
        #endregion

        #region Private Fields
        // The message that is received from the server.
        private string message = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the message that is received from the server.
        /// </summary>
        public string Message
        {
            get { return message; }
        }
        #endregion
    }

    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class AsyncArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="exception">The message that is received from the server.</param>
        public AsyncArgs(Nequeo.Exceptions.AsyncException exception)
        {
            this.exception = exception;
        }
        #endregion

        #region Private Fields
        // The message that is received from the server.
        private Nequeo.Exceptions.AsyncException exception = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the message that is received from the server.
        /// </summary>
        public Nequeo.Exceptions.AsyncException Exception
        {
            get { return exception; }
        }
        #endregion
    }

    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class OperationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="cancel">True if the operation should be cancelled.</param>
        public OperationArgs(bool cancel)
        {
            this.cancel = cancel;
        }
        #endregion

        #region Private Fields
        // The cancel that is received from the server.
        private bool cancel = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the cancel that is received from the server.
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
        #endregion
    }

    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class ValidationArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="valid">True if the validation was successful; else false.</param>
        public ValidationArgs(bool valid)
        {
            this.valid = valid;
        }
        #endregion

        #region Private Fields
        // The cancel that is received from the server.
        private bool valid = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the valid that is received from the server.
        /// </summary>
        public bool Valid
        {
            get { return valid; }
            set { valid = value; }
        }
        #endregion
    }
}
