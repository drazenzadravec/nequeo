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
using System.Threading;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Security;
using Nequeo.Threading;

namespace Nequeo.Net.Sockets
{
    /// <summary>
    /// General socket client interface.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Gets or sets the current unique connection identifier.
        /// </summary>
        string ConnectionID { get; set; }

        /// <summary>
        /// Gets, a value that indicates whether a connection to a remote host exits.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Gets sets, the client time out (minutes) interval.
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// Gets sets, the send timeout.
        /// </summary>
        int SendTimeout { get; set; }

        /// <summary>
        /// Gets sets, the receive timeout.
        /// </summary>
        int ReceiveTimeout { get; set; }

        /// <summary>
        /// Gets sets, the host name or IP address to resolve.
        /// </summary>
        string HostNameOrAddress { get; set; }

        /// <summary>
        /// Gets sets, port to use.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Gets sets, use a secure connection.
        /// </summary>
        bool UseSslConnection { get; set; }

        /// <summary>
        /// The on disconnected event handler, triggered when a connection has been closed.
        /// </summary>
        event System.EventHandler OnDisconnected;

        /// <summary>
        /// Triggers when an internal error occurs.
        /// </summary>
        event Nequeo.Threading.EventHandler<Exception, string> OnInternalError;

        /// <summary>
        /// Triggered when the client in-active time out has been reached.
        /// </summary>
        event System.EventHandler OnTimedOut;

        /// <summary>
        /// Connect to the host socket.
        /// </summary>
        void Connect();

        /// <summary>
        /// Close the current socket connection and disposes of all resources.
        /// </summary>
        void Close();

        /// <summary>
        /// Disposes of all resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        void Send(String data);

        /// <summary>
        /// Send data to the host.
        /// </summary>
        /// <param name="data">The data to send to the server.</param>
        void Send(byte[] data);

        /// <summary>
        /// Receive data from the host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        byte[] Receive();

        /// <summary>
        /// Receive data from the host.
        /// </summary>
        /// <returns>The data received; else null.</returns>
        string Read();

    }
}
