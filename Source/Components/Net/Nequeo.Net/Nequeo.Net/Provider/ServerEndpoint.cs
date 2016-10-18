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
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Reflection;

namespace Nequeo.Net.Provider
{
    /// <summary>
    /// Socket server endpoints.
    /// </summary>
    public sealed partial class ServerEndpoint : Nequeo.Net.Sockets.MultiEndpointServer
    {
        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public ServerEndpoint(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
            : base(typeof(Nequeo.Net.Provider.ServerContextBase), addresses, port, maxNumClients)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");
        }

        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public ServerEndpoint(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(typeof(Nequeo.Net.Provider.ServerContextBase), multiEndpointModels, maxNumClients)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");
        }

        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="serverContextType">The server context type, must inherit Nequeo.Net.Sockets.ServerContext.</param>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public ServerEndpoint(Type serverContextType, IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
            : base(serverContextType, addresses, port, maxNumClients)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");
        }

        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="serverContextType">The server context type, must inherit Nequeo.Net.Sockets.ServerContext.</param>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public ServerEndpoint(Type serverContextType, Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(serverContextType, multiEndpointModels, maxNumClients)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");
        }
    }

    /// <summary>
    /// Socket server single endpoints.
    /// </summary>
    public sealed partial class ServerSingleEndpoint : Nequeo.Net.Sockets.MultiEndpointServerSingle
    {
        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public ServerSingleEndpoint(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
            : base(addresses, port, maxNumClients)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");
        }

        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public ServerSingleEndpoint(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");
        }
    }

    /// <summary>
    /// UDP Socket server endpoints.
    /// </summary>
    public sealed partial class UdpServerEndpoint : Nequeo.Net.Sockets.MultiEndpointUdpServer
    {
        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public UdpServerEndpoint(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
            : base(addresses, port, maxNumClients)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");
        }

        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public UdpServerEndpoint(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");
        }
    }

    /// <summary>
    /// UDP Socket server endpoints.
    /// </summary>
    public sealed partial class UdpSingleServerEndpoint : Nequeo.Net.Sockets.MultiEndpointUdpSingleServer
    {
        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="addresses">The collection of IP address endpoints.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public UdpSingleServerEndpoint(IPAddress[] addresses, int port, int maxNumClients = Int32.MaxValue)
            : base(addresses, port, maxNumClients)
        {
            if (addresses == null) throw new ArgumentNullException("addresses");
            if (port < 1) throw new IndexOutOfRangeException("The port must be greater than zero.");
        }

        /// <summary>
        /// Server socket endpoints.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public UdpSingleServerEndpoint(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            if (multiEndpointModels == null) throw new ArgumentNullException("multiEndpointModels");
        }
    }
}
