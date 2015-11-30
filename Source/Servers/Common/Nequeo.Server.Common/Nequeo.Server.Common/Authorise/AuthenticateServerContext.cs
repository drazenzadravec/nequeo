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

namespace Nequeo.Server.Authorise
{
    /// <summary>
    /// Common authentication server context helper.
    /// </summary>
    public class AuthenticateServerContext : Authentication
    {
        /// <summary>
        /// Common authentication server context helper.
        /// </summary>
        public AuthenticateServerContext()
        {
        }

        private int _maxNumberOfConnectionAttempts = 4;
        private int _connectionAttempts = 0;

        /// <summary>
        /// Gets sets, max number of connection attempts.
        /// </summary>
        public int MaxNumberOfConnectionAttempts
        {
            get { return _maxNumberOfConnectionAttempts; }
            set { _maxNumberOfConnectionAttempts = value; }
        }

        /// <summary>
        /// Data sent from the server context to the server.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data sent from the server context to the server.</param>
        /// <returns>True if the command was found and executed; else false.</returns>
        new public bool Receiver(Nequeo.Net.Sockets.ServerContext context, string command, string data)
        {
            // Delay.
            System.Threading.Thread.Sleep(10);

            // Process the command.
            switch (command.ToUpper())
            {
                case "AORE":
                    // Authorise this connection to the server.
                    AuthoriseConnection(context, true, data);
                    return true;

                case "AUTH":
                    // Attempt to autherticate the credentials.
                    Authenticate(context, true, data);
                    return true;

                default:
                    return base.Receiver(context, command, data);
            }
        }

        /// <summary>
        /// Data sent from the server context to the server.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="command">The command sent from the server context to the server.</param>
        /// <param name="data">The data sent from the server context to the server.</param>
        /// <returns>True if the command was found and executed; else false.</returns>
        new public bool ReceiveFromServer(Nequeo.Net.Sockets.ServerContext context, string command, string data)
        {
            // Delay.
            System.Threading.Thread.Sleep(10);

            // Process the command.
            switch (command.ToUpper())
            {
                case "AORE":
                    // Authorise this connection to the server.
                    AuthoriseConnection(context, false, data);
                    return true;

                case "AUTH":
                    // Authertication result.
                    Authenticate(context, false, data);
                    return true;

                default:
                    return base.ReceiveFromServer(context, command, data);
            }
        }

        /// <summary>
        /// Authenticate the client to the server.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="request">Is in request mode.</param>
        /// <param name="data">The data from the client.</param>
        protected override void Authenticate(Net.Sockets.ServerContext context, bool request, string data)
        {
            // Has the credentials been Authorised.
            if (base.IsAuthenticated)
            {
                // If in request mode.
                if (request)
                {
                    // Send a message to the server asking
                    // to authenticate the credentials.
                    context.SendToServer(Encoding.ASCII.GetBytes("AUTH" + (String.IsNullOrEmpty(data) ? "" : " " + data) + "\r\n"));
                }
                else
                {
                    // If no data has been passed then
                    // not authenticated.
                    if (String.IsNullOrEmpty(data))
                        // Send a message to the client.
                        context.Send("AUTH 400" + "\r\n");
                    else
                        // Send a message to the client.
                        context.Send("AUTH 200" + (String.IsNullOrEmpty(data) ? "" : " " + data) + "\r\n");
                }
            }
        }

        /// <summary>
        /// Authorise this connection to the server.
        /// </summary>
        /// <param name="context">The server context sending the data.</param>
        /// <param name="request">Is in request mode.</param>
        /// <param name="data">The data from the client.</param>
        private void AuthoriseConnection(Net.Sockets.ServerContext context, bool request, string data)
        {
            // If in request mode.
            if (request)
            {
                // If more attempts exits then send invalid credentials.
                if (_connectionAttempts <= MaxNumberOfConnectionAttempts)
                {
                    // Send a message to the server asking
                    // to authorise the connection.
                    context.SendToServer(Encoding.ASCII.GetBytes("AORE" + (String.IsNullOrEmpty(data) ? "" : " " + data) + "\r\n"));
                }
                else
                {
                    // Sent to the server context a close command,
                    // too many authorise connection attempts.
                    context.SentFromServer(Encoding.ASCII.GetBytes("CLOS" + "\r\n"));
                }

                if (!base.IsAuthenticated)
                    // Increment the connection attempt count.
                    _connectionAttempts++;
            }
            else
            {
                // If data exists.
                if (!String.IsNullOrEmpty(data))
                {
                    // The client has been authenticated.
                    base.IsAuthenticated = true;
                    context.SetAuthenticated();

                    // Send a message to the client
                    // indicating that the client
                    // is allowed to join the conversation.
                    context.Send("JOIN 200" + "\r\n");
                }
                else
                {
                    // Send a message to the client
                    // indicating that the credentials
                    // are invalid.
                    context.Send("REJD 402 Authorise connection credentials are invalid." + "\r\n");
                }
            }
        }
    }
}
