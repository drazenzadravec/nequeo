/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Nequeo.Composite
{
    /// <summary>
    /// The composite service context interface.
    /// </summary>
    public interface ICompositeContext
	{
        /// <summary>
        /// Gets the composite server interface.
        /// </summary>
        IServer CompositeServer { get; }

        /// <summary>
        /// Sends data to the client.
        /// </summary>
        /// <param name="data">The command and data to send to the client.</param>
        void SendData(byte[] data);

        /// <summary>
        /// Send data to the specified connected clients.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="serviceClientNames">The list of service clients names to send data to.</param>
        /// <param name="sendComplete">Sending data to clients has completed.</param>
        /// <param name="isFirstIteration">Is this the first set of data to be sent to the clients (Including command; if any).</param>
        void SendDataToClient(byte[] data, string[] serviceClientNames,
            bool sendComplete = false, bool isFirstIteration = false);

        /// <summary>
        /// Are any of the clients connected to the current service.
        /// </summary>
        /// <param name="serviceClientNames">The list of service clients names to search for.</param>
        /// <returns>True if any one or all of the clients are connected; else false.</returns>
        bool IsClientNameConected(string[] serviceClientNames);

        /// <summary>
        /// Gets all the clients that are currently connected to this server.
        /// </summary>
        /// <returns>The collection of clients.</returns>
        string[] GetConnectedServiceClients();

        /// <summary>
        /// Get the client ip endpoint.
        /// </summary>
        /// <returns>The client ip endpoint.</returns>
        IPEndPoint GetClientIPEndPoint();

        /// <summary>
        /// Get the server ip endpoint.
        /// </summary>
        /// <returns>The server ip endpoint.</returns>
        IPEndPoint GetServerIPEndPoint();
	}
}
