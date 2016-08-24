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

using Nequeo.Net;
using Nequeo.Net.Sockets;
using Nequeo.Data.Provider;

namespace Nequeo.Client.Manage
{
    /// <summary>
    /// Interaction client interface.
    /// </summary>
    public interface IInteractionClient : Nequeo.Net.Sockets.IClient, IInteraction
    {
        /// <summary>
        /// The on error event handler, triggered when data received from the server is any type of error.
        /// </summary>
        event Nequeo.Threading.EventHandler<string, string, string> OnError;

        /// <summary>
        /// Gets sets, attempts to reconnect to the server when a connection is lost.
        /// </summary>
        bool ReconnectWhenNoConnection { get; set; }

        /// <summary>
        /// Gets the remote configuration type port name.
        /// </summary>
        string RemoteTypePortName { get; }

        /// <summary>
        /// Initialise the client, use this method when loading from the configuration file.
        /// </summary>
        void Initialisation();

    }
}
