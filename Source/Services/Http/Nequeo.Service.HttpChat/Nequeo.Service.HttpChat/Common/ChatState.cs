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
using System.Threading.Tasks;

namespace Nequeo.Service.Chat
{
    /// <summary>
    /// Member chat state model.
    /// </summary>
    internal class ChatWebSocketState
    {
        /// <summary>
        /// Gets or sets the request stream.
        /// </summary>
        public Nequeo.IO.Stream.StreamBufferBase RequestStream
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        public Nequeo.Net.WebSockets.WebSocketMember Member
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the web socket.
        /// </summary>
        public System.Net.WebSockets.WebSocket WebSocket
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of headers.
        /// </summary>
        public List<Nequeo.Model.NameValue> Headers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the execution member.
        /// </summary>
        public string ExecutionMember
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public Nequeo.Exceptions.ErrorCodeException ErrorCode
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Member chat state model.
    /// </summary>
    internal class ChatHttptState
    {
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        public Nequeo.Net.Http.HttpContextMember Member
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the http context.
        /// </summary>
        public System.Web.HttpContext HttpContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the execution member.
        /// </summary>
        public string ExecutionMember
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public Nequeo.Exceptions.ErrorCodeException ErrorCode
        {
            get;
            set;
        }
    }
}