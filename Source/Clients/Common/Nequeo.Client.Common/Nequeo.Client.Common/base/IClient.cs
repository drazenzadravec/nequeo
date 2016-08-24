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

using Nequeo.Net.Sockets;
using Nequeo.Security;
using Nequeo.Threading;

namespace Nequeo.Client
{
    /// <summary>
    /// Common client interface.
    /// </summary>
    public interface IClient : Nequeo.Net.Sockets.IClient
    {
        /// <summary>
        /// Gets, a unique client identifier.
        /// </summary>
        string UniqueIdentifier { get; set; }

        /// <summary>
        /// Gets, has the client been authosied.
        /// </summary>
        bool IsAuthorised { get; }

        /// <summary>
        /// Gets sets, attempts to reconnect to the server when a connection is lost.
        /// </summary>
        bool ReconnectWhenNoConnection { get; set; }

        /// <summary>
        /// The on error event handler, triggered when data received from the server is any type of error.
        /// </summary>
        event Nequeo.Threading.EventHandler<string, string, string> OnError;

        /// <summary>
        /// The on valid connection event handler, triggered when a valid connection has been established,
        /// such as a welcome return from the server.
        /// </summary>
        event Nequeo.Threading.EventHandler<string, string, string> OnValidConnection;

        /// <summary>
        /// The on authorise event handler, triggered when a valid connection has been established,
        /// such as a join from the server indicating the client has been authenticated.
        /// </summary>
        event Nequeo.Threading.EventHandler<string, string, string> OnAuthorise;

        /// <summary>
        /// The on valid command event handler, triggered when a valid command is sent.
        /// </summary>
        event Nequeo.Threading.EventHandler<string, string, string> OnValidCommand;

        /// <summary>
        /// Initialise the client, use this method when loading from the configuration file.
        /// </summary>
        void Initialisation();

        /// <summary>
        /// Authorise this connection to the server.
        /// </summary>
        /// <param name="username">The authorise username credentials.</param>
        /// <param name="password">The authorise password credentials.</param>
        /// <param name="domain">The authorise domain credentials.</param>
        void Authorise(string username, string password, string domain = null);

    }
}
