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
using System.Threading;
using System.Diagnostics;

using Nequeo.Handler;

namespace Nequeo.Net.Common
{
    /// <summary>
    /// The base service class.
    /// </summary>
    public abstract class ServiceBase : Nequeo.Handler.LogHandler, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public ServiceBase()
            : base(_applicationName, _eventNamespace)
        {
        }
        #endregion

        #region Private Constants
        private const string _applicationName = "Nequeo.Smtp.Proxy.Server";
        private const string _eventNamespace = "Smtp";
        #endregion

        #region Protected Virtual Error and Process Handling Methods
        /// <summary>
        /// Writes to the selected error/process reporting location.
        /// </summary>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        protected virtual void Write(string message,
            WriteTo writeTo, LogType logType)
        {
            base.Write("General", "Write General",
                message, 0, writeTo, logType);
        }

        /// <summary>
        /// Writes to the selected error/process reporting location.
        /// </summary>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        protected virtual void Write(string method, string message, int lineNumber,
            WriteTo writeTo, LogType logType)
        {
            base.Write("General", method,
                message, lineNumber, writeTo, logType);
        }

        /// <summary>
        /// Writes to the selected error/process reporting location.
        /// </summary>
        /// <param name="objectClass">String, the class that the error occurred in.</param>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        new protected virtual void Write(string objectClass, string method, string message, int lineNumber,
            WriteTo writeTo, LogType logType)
        {
            base.Write(objectClass, method, message, lineNumber, writeTo, logType);
        }

        /// <summary>
        /// Writes to the selected error/process reporting location.
        /// </summary>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="code">Integer, the error/processing code for the operation.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        protected virtual void Write(string method, string message, int lineNumber,
            int code, WriteTo writeTo, LogType logType)
        {
            base.Write("General", method,
                message, lineNumber, code, writeTo, logType);
        }

        /// <summary>
        /// Writes to the selected error/process reporting location.
        /// </summary>
        /// <param name="objectClass">String, the class that the error occurred in.</param>
        /// <param name="method">String, the method that the error occurred in.</param>
        /// <param name="message">String, the error message to write.</param>
        /// <param name="lineNumber">Integer, the line number of the error.</param>
        /// <param name="code">Integer, the error/processing code for the operation.</param>
        /// <param name="writeTo">WriteTo, write log to location.</param>
        /// <param name="logType">Enum, the log file type.</param>
        new protected virtual void Write(string objectClass, string method, string message, int lineNumber,
            int code, WriteTo writeTo, LogType logType)
        {
            base.Write(objectClass, method, message, lineNumber, code, writeTo, logType);
        }
        #endregion
    }
}
