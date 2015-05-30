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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Handler;
using Nequeo.Extension;
using Nequeo.Net.Configuration;

namespace Nequeo.Net
{
    /// <summary>
    /// Web client.
    /// </summary>
    public class NetClient : Nequeo.Net.Provider.ClientSocket
    {
        #region Constructors
        /// <summary>
        /// Client socket provider.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public NetClient()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Client socket provider.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public NetClient(IPAddress address, int port)
            : base(address, port)
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Socket client constructor.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public NetClient(string hostNameOrAddress, int port)
            : base(hostNameOrAddress, port)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on net context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.NetContext> OnNetContext;

        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.OnReceivedHandler = (Nequeo.Net.Provider.ClientContext context) => OnReceivedActionHandler(context);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="context">The current client context.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Provider.ClientContext context)
        {
            try
            {
                // Create the web context and set the headers.
                Nequeo.Net.NetContext netContext = CreateNetContext(context);

                // If the event has been assigned.
                if (OnNetContext != null)
                    OnNetContext(this, netContext);

                // Save the web context state objects.
                SaveNetContext(context, netContext);
            }
            catch (Exception)
            {
                // Close the connection and release all resources used for communication.
                context.Close();
            }
        }
        #endregion
    }
}
