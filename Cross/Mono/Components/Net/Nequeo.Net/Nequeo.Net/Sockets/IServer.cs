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
    /// General socket server interface.
    /// </summary>
    public interface IServerBase
    {
        #region Public Properties
        /// <summary>
        /// Gets, the timeout for each client connection when in-active.
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// Gets, the maximum number of concurrent clients that are allowed to be connected.
        /// </summary>
        int MaxNumClients { get; }

        /// <summary>
        /// Gets, the number of clients currently connected to the server.
        /// </summary>
        int NumberOfClients { get; }

        /// <summary>
        /// Gets, an indicator to see if the server is listening (accepting connections).
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Gets, the current server name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets, the common service name.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets, the protocol type.
        /// </summary>
        System.Net.Sockets.ProtocolType ProtocolType { get; }

        /// <summary>
        /// Gets, the socket type.
        /// </summary>
        System.Net.Sockets.SocketType SocketType { get; }

        /// <summary>
        /// Gets, IP address.
        /// </summary>
        IPAddress IPAddress { get; }

        /// <summary>
        /// Gets, port to use.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets, the addressing scheme that an instance of the connection to use.
        /// </summary>
        System.Net.Sockets.AddressFamily AddressFamily { get; }

        /// <summary>
        /// Gets, should this server control maximum number of clients
        /// independent of all other servers within the multi-endpoint.
        /// Is this server part of a collection of multi-endpoint servers.
        /// </summary>
        bool MaxNumClientsIndividualControl { get; }

        /// <summary>
        /// Gets, use a secure connection.
        /// </summary>
        bool UseSslConnection { get; }

        /// <summary>
        /// Gets, the x.509 certificate used for a secure connection.
        /// </summary>
        X509Certificate2 X509Certificate { get; }

        /// <summary>
        /// Gets, defines the possible versions of System.Security.Authentication.SslProtocols.
        /// </summary>
        SslProtocols SslProtocols { get; }

        /// <summary>
        /// Gets the maximum request buffer capacity when using buffered stream.
        /// </summary>
        int RequestBufferCapacity { get; }

        /// <summary>
        /// Gets the maximum response buffer capacity when using buffered stream.
        /// </summary>
        int ResponseBufferCapacity { get; }

        #endregion
    }

    /// <summary>
    /// General socket server interface.
    /// </summary>
    public interface IServer : IServerBase
    {
    }

    /// <summary>
    /// General socket server interface.
    /// </summary>
    public interface IUdpServer
    {
        #region Public Properties
        /// <summary>
        /// Gets, the timeout for each client connection when in-active.
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// Gets, the maximum number of concurrent clients that are allowed to be connected.
        /// </summary>
        int MaxNumClients { get; }

        /// <summary>
        /// Gets, the number of clients currently connected to the server.
        /// </summary>
        int NumberOfClients { get; }

        /// <summary>
        /// Gets, an indicator to see if the server is listening (accepting connections).
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Gets, the current server name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets, the common service name.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets, IP address.
        /// </summary>
        IPAddress IPAddress { get; }

        /// <summary>
        /// Gets, port to use.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets, the addressing scheme that an instance of the connection to use.
        /// </summary>
        System.Net.Sockets.AddressFamily AddressFamily { get; }

        /// <summary>
        /// Gets, should this server control maximum number of clients
        /// independent of all other servers within the multi-endpoint.
        /// Is this server part of a collection of multi-endpoint servers.
        /// </summary>
        bool MaxNumClientsIndividualControl { get; }

        /// <summary>
        /// Gets the maximum request buffer capacity when using buffered stream.
        /// </summary>
        int RequestBufferCapacity { get; }

        /// <summary>
        /// Gets the maximum response buffer capacity when using buffered stream.
        /// </summary>
        int ResponseBufferCapacity { get; }

        #endregion
    }

    /// <summary>
    /// General socket server interface.
    /// </summary>
    public interface IUdpSingleServer
    {
        #region Public Properties
        /// <summary>
        /// Gets, the maximum number of concurrent clients that are allowed to be connected.
        /// </summary>
        int MaxNumClients { get; }

        /// <summary>
        /// Gets, the number of clients currently connected to the server.
        /// </summary>
        int NumberOfClients { get; }

        /// <summary>
        /// Gets, an indicator to see if the server is listening (accepting connections).
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Gets, the current server name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets, the common service name.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets, IP address.
        /// </summary>
        IPAddress IPAddress { get; }

        /// <summary>
        /// Gets, port to use.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets, the addressing scheme that an instance of the connection to use.
        /// </summary>
        System.Net.Sockets.AddressFamily AddressFamily { get; }

        /// <summary>
        /// Gets, should this server control maximum number of clients
        /// independent of all other servers within the multi-endpoint.
        /// Is this server part of a collection of multi-endpoint servers.
        /// </summary>
        bool MaxNumClientsIndividualControl { get; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Send the data to the client end point.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="client">The client end point to send to.</param>
        void SendTo(byte[] data, IPEndPoint client);

        #endregion
    }

    /// <summary>
    /// General socket server (multi-client), single thread for all client.
    /// until the client is disconnected.
    /// </summary>
    public interface IServerSingle
    {
        #region Methods Properties

        /// <summary>
        /// Should the error state on a socket be polled, (default : false).
        /// </summary>
        /// <param name="poll">True if it is polled; else false.</param>
        void PollError(bool poll);

        /// <summary>
        /// Should the read state on a socket be polled, (default : true).
        /// </summary>
        /// <param name="poll">True if it is polled; else false.</param>
        void PollReader(bool poll);

        /// <summary>
        /// Should the write state on a socket be polled, (default : false).
        /// </summary>
        /// <param name="poll">True if it is polled; else false.</param>
        void PollWriter(bool poll);

        #endregion
    }
}
