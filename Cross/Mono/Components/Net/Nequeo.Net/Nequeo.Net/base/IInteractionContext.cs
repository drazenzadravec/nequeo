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

namespace Nequeo.Net
{
    /// <summary>
    /// Interact context interface.
    /// </summary>
    public interface IInteractionContext
    {
        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts).
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameReceivers">The service name the receivers are connected to.</param>
        /// <param name="receivers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameReceivers, string[] receivers, byte[] data);

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts). 
        /// Hosts and ports are included for an open channel to the remote nost.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameReceivers">The service name the receivers are connected to.</param>
        /// <param name="receivers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        /// <param name="hosts">The remote hosts that receivers (unique identities) are connected to.</param>
        /// <param name="ports">The remote host ports, each port must have a matching host.</param>
        void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameReceivers, string[] receivers, byte[] data, string[] hosts, string[] ports);
    }
}
