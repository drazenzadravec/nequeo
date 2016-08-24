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
using Nequeo.Data.Enum;
using Nequeo.Data.Provider;

namespace Nequeo.Client.Manage
{
    /// <summary>
    /// Authentication client interface.
    /// </summary>
    public interface IAuthenticationClient : Nequeo.Net.Sockets.IClient, IAuthentication
    {
        /// <summary>
        /// The on error event handler, triggered when data received from the server is any type of error.
        /// </summary>
        event Nequeo.Threading.EventHandler<string, string, string> OnError;

        /// <summary>
        /// Gets sets, the blocking timeout. The number of milliseconds to wait.
        /// </summary>
        int MillisecondsTimeout { get; set; }

        /// <summary>
        /// Gets sets, the maximum number of data to receive in each request (-1 Infinte).
        /// </summary>
        int ReceiveMaxSize { get; set; }

        /// <summary>
        /// Gets sets, attempts to reconnect to the server when a connection is lost.
        /// </summary>
        bool ReconnectWhenNoConnection { get; set; }

        /// <summary>
        /// Initialise the client.
        /// </summary>
        /// <param name="useConfigFile">True to use the configuration file data to connect; else false.</param>
        void Initialisation(bool useConfigFile = false);

        /// <summary>
        /// Authorise this connection to the server.
        /// </summary>
        /// <param name="username">The authorise username credentials.</param>
        /// <param name="password">The authorise password credentials.</param>
        /// <param name="domain">The authorise domain credentials.</param>
        /// <exception cref="System.Exception"></exception>
        void AuthoriseConnection(string username, string password, string domain = null);

        /// <summary>
        /// Send a stop task command to the server.
        /// </summary>
        void Stop();
    }
}
