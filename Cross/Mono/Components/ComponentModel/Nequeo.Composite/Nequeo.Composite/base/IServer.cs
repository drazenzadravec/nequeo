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

namespace Nequeo.Composite
{
    /// <summary>
    /// Composition server interface provider
    /// </summary>
    public interface IServer : ICloneable
    {
        /// <summary>
        /// Gets sets the composite service context interface.
        /// </summary>
        ICompositeContext CompositeContext { get; set; }

        /// <summary>
        /// Get the unique client service name.
        /// </summary>
        /// <returns>The current unique client name that is associated with this service.</returns>
        string GetServiceClientName();

        /// <summary>
        /// Close the client connection.
        /// </summary>
        /// <returns>True if connection is to be closed; else false.</returns>
        bool CloseConnection();

        /// <summary>
        /// Is the current server enabled
        /// </summary>
        /// <returns>True if the server is enabled else false.</returns>
        bool IsEnabled();

        /// <summary>
        /// Recieve all data complete
        /// </summary>
        /// <returns>True if all data has been received; else false.</returns>
        bool ReceiveComplete();

        /// <summary>
        /// Send all data complete
        /// </summary>
        /// <returns>True if all data has been send; else false.</returns>
        bool SendComplete();

        /// <summary>
        /// Request the received data.
        /// </summary>
        /// <param name="request">The request data.</param>
        void Request(byte[] request);

        /// <summary>
        /// Responed with the send data.
        /// </summary>
        /// <returns>The response data.</returns>
        byte[] Response();
    }
}
