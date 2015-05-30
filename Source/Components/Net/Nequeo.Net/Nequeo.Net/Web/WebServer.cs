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
using System.Security.Principal;

using Nequeo.Handler;
using Nequeo.Extension;
using Nequeo.Net.Configuration;

namespace Nequeo.Net
{
    /// <summary>
    /// Web server.
    /// </summary>
    public class WebServer : Nequeo.Net.Provider.ServerSocket
    {
        #region Constructors
        /// <summary>
        /// Web server, constructs the server from the configuration.
        /// </summary>
        public WebServer()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Web server, does not use the configuration to construct the server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public WebServer(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on web context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.WebContext> OnWebContext;

        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.OnReceivedHandler = (Nequeo.Net.Provider.Context context) => OnReceivedActionHandler(context);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="context">The current server context.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Provider.Context context)
        {
            try
            {
                // Create the web context and set the headers.
                Nequeo.Net.WebContext webContext = CreateWebContext(context);

                // If the event has been assigned.
                if (OnWebContext != null)
                    OnWebContext(this, webContext);

                // Save the web context state objects.
                SaveWebContext(context, webContext);
            }
            catch (Exception)
            {
                // Close the connection and release all resources used for communication.
                context.Close();
            }
        }
        #endregion
    }

    /// <summary>
    /// Web server single.
    /// </summary>
    public class WebServerSingle : Nequeo.Net.Provider.ServerSingleSocket
    {
        #region Constructors
        /// <summary>
        /// Web server, constructs the server from the configuration.
        /// </summary>
        public WebServerSingle()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Web server, does not use the configuration to construct the server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public WebServerSingle(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = 10000)
            : base(multiEndpointModels, maxNumClients)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Private Fields
        private int _maxHeaderBufferStore = 20000;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the maximum header request buffer store read size before closing the connection. 
        /// </summary>
        public int MaxHeaderBufferStore
        {
            get { return _maxHeaderBufferStore; }
            set { _maxHeaderBufferStore = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on web context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.WebContext> OnWebContext;

        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.OnReceivedHandler = (Nequeo.Net.Provider.Context context) => OnReceivedActionHandler(context);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="context">The current server context.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Provider.Context context)
        {
            try
            {
                // Create the web context and set the headers.
                Nequeo.Net.WebContext webContext = CreateWebContext(context);

                // If the event has been assigned.
                if (OnWebContext != null)
                    OnWebContext(this, webContext);

                // Save the web context state objects.
                SaveWebContext(context, webContext);
            }
            catch (Exception)
            {
                // Close the connection and release all resources used for communication.
                context.Close();
            }
        }
        #endregion
    }

    /// <summary>
    /// UDP Web server.
    /// </summary>
    public class WebUdpServer : Nequeo.Net.Provider.UdpServerSocket
    {
        #region Constructors
        /// <summary>
        /// Web server, constructs the server from the configuration.
        /// </summary>
        public WebUdpServer()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Web server, does not use the configuration to construct the server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public WebUdpServer(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on web context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.WebContext> OnWebContext;

        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.OnReceivedHandler = (Nequeo.Net.Provider.Context context) => OnReceivedActionHandler(context);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="context">The current server context.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Provider.Context context)
        {
            try
            {
                // Create the web context and set the headers.
                Nequeo.Net.WebContext webContext = CreateWebContext(context);

                // If the event has been assigned.
                if (OnWebContext != null)
                    OnWebContext(this, webContext);

                // Save the web context state objects.
                SaveWebContext(context, webContext);
            }
            catch (Exception)
            {
                // Close the connection and release all resources used for communication.
                context.Close();
            }
        }
        #endregion
    }

    /// <summary>
    /// UDP Web server.
    /// </summary>
    public class WebUdpSingleServer : Nequeo.Net.Provider.UdpSingleServerSocket
    {
        #region Constructors
        /// <summary>
        /// Web server, constructs the server from the configuration.
        /// </summary>
        public WebUdpSingleServer()
        {
            AssignOnConnectedActionHandler();
        }

        /// <summary>
        /// Web server, does not use the configuration to construct the server.
        /// </summary>
        /// <param name="multiEndpointModels">The multi-endpoint model.</param>
        /// <param name="maxNumClients">The combined maximum number of clients for all servers with each endpoint.</param>
        public WebUdpSingleServer(Net.Sockets.MultiEndpointModel[] multiEndpointModels, int maxNumClients = Int32.MaxValue)
            : base(multiEndpointModels, maxNumClients)
        {
            AssignOnConnectedActionHandler();
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The on web context event handler, triggered when a new connection is establised or data is present. Should be used when implementing a new connection.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Nequeo.Net.Sockets.IUdpSingleServer, byte[], IPEndPoint> OnWebContext;

        #endregion

        #region Private Methods
        /// <summary>
        /// Assign the on connected action handler.
        /// </summary>
        private void AssignOnConnectedActionHandler()
        {
            // Assign the on connect action handler.
            base.OnReceivedHandler = (server, data, endpoint) => OnReceivedActionHandler(server, data, endpoint);
        }

        /// <summary>
        /// On received action handler.
        /// </summary>
        /// <param name="server">The UDP server.</param>
        /// <param name="data">The data from the client.</param>
        /// <param name="endpoint">The client endpoint sending the data.</param>
        private void OnReceivedActionHandler(Nequeo.Net.Sockets.IUdpSingleServer server, byte[] data, IPEndPoint endpoint)
        {
            try
            {
                // If the event has been assigned.
                if (OnWebContext != null)
                    OnWebContext(this, server, data, endpoint);
            }
            catch { }
        }
        #endregion
    }
}
