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

using Nequeo.Data.Enum;
using Nequeo.Data.Provider;
using Nequeo.Net;

namespace Nequeo.Server
{
    /// <summary>
    /// Interact interface.
    /// </summary>
    public interface IInteraction : Nequeo.Net.IInteractionContext
    {
        /// <summary>
        /// Gets sets, should a direct connection to services be used.
        /// </summary>
        bool DirectConnection { get; set; }

        /// <summary>
        /// Gets sets, the receiver action handler, triggered when send to client data arrives.
        /// </summary>
        Action<string, string, string, string[], byte[]> Receiver { get; set; }

        /// <summary>
        /// Release active connections resources.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceNameReceivers">The service name the receivers are connected to.</param>
        void ReleaseConnections(string uniqueIdentifier, string serviceNameReceivers);

    }

    /// <summary>
    /// Stores the connection data for a active connection.
    /// </summary>
    public class ActiveConnectionModel
    {
        /// <summary>
        /// Gets sets, send information to host model.
        /// </summary>
        public SendToHostModel[] Hosts { get; set; }

        /// <summary>
        /// Gets sets, send information to port model.
        /// </summary>
        public SendToPortModel[] Ports { get; set; }
    }

    /// <summary>
    /// Send information to host model.
    /// </summary>
    public class SendToHostModel
    {
        /// <summary>
        /// Gets sets, the host name.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets sets, the list of receivers for the host.
        /// </summary>
        public string[] Receivers { get; set; }
    }

    /// <summary>
    /// Send information to port model.
    /// </summary>
    public class SendToPortModel
    {
        /// <summary>
        /// Gets sets, port type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets sets, port type number
        /// </summary>
        public int Number { get; set; }
    }
}
