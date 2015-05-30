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
using System.Net.Sockets;
using System.Threading;
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
using Nequeo.Net.WebSockets.Protocol;

namespace Nequeo.Net.WebSockets
{
    /// <summary>
    /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
    /// </summary>
    public class WebSocketException : Exception
    {
        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        public WebSocketException()
            : this(CloseCodeStatus.AbnormalClosure, null, null)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        /// <param name="innerException">Inner exception.</param>
        public WebSocketException(Exception innerException)
            : this(CloseCodeStatus.AbnormalClosure, null, innerException)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        /// <param name="message">The message.</param>
        public WebSocketException(string message)
            : this(CloseCodeStatus.AbnormalClosure, message, null)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        /// <param name="code">The close code status.</param>
        public WebSocketException(CloseCodeStatus code)
            : this(code, null, null)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">Inner exception.</param>
        public WebSocketException(string message, Exception innerException)
            : this(CloseCodeStatus.AbnormalClosure, message, innerException)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        /// <param name="code">The close code status.</param>
        /// <param name="innerException">Inner exception.</param>
        public WebSocketException(CloseCodeStatus code, Exception innerException)
            : this(code, null, innerException)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        /// <param name="code">The close code status.</param>
        /// <param name="message">The message.</param>
        public WebSocketException(CloseCodeStatus code, string message)
            : this(code, message, null)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="WebSocket"/> gets a fatal error.
        /// </summary>
        /// <param name="code">The close code status.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">Inner exception.</param>
        public WebSocketException(CloseCodeStatus code, string message, Exception innerException)
            : base(message ?? code.GetMessage(), innerException)
        {
            _code = code;
        }

        private CloseCodeStatus _code;

        /// <summary>
        /// Gets the status code indicating the cause of the exception.
        /// </summary>
        /// <value>
        /// One of the <see cref="CloseCodeStatus"/> enum values, represents the status code
        /// indicating the cause of the exception.
        /// </value>
        public CloseCodeStatus Code
        {
            get { return _code; }
        }
    }
}
