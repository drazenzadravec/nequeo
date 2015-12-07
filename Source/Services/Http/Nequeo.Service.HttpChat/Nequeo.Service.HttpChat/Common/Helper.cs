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
using System.Web;
using System.Text;

using Nequeo.Extension;

namespace Nequeo.Service.Common
{
    /// <summary>
    /// Intenal helper
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Token provider.
        /// </summary>
        public static Nequeo.Data.Provider.IToken Token = null;

        /// <summary>
        /// Communication provider.
        /// </summary>
        public static Nequeo.Data.Provider.ICommunication Communication = null;

        /// <summary>
        /// Integration context provider.
        /// </summary>
        public static Nequeo.Net.IInteractionContext IntegrationContext = null;

        /// <summary>
        /// Member context manager provider.
        /// </summary>
        public static Nequeo.Net.IMemberContextManager MemberContextManager = null;

        /// <summary>
        /// Timeout manager provider.
        /// </summary>
        public static Nequeo.Net.IMemberContextManager TimeoutManager = null;

        /// <summary>
        /// The web socket chat provider, used to receive messages from clients.
        /// </summary>
        public static ChatProvider ChatProvider = new ChatProvider();

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts).
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the unique identifiers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        public static void Receiver(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data)
        {
            // If the receiver has been created.
            if (ChatProvider != null)
            {
                // Assign the member context manger.
                ChatProvider.MemberContextManager = Helper.MemberContextManager;

                // Send the data back to the client.
                ChatProvider.SendToClients(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data);
            }
        }

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts).
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the unique identifiers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        public static void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data)
        {
            // If the receiver has been created.
            if (ChatProvider != null)
            {
                // Assign the member context manger.
                ChatProvider.MemberContextManager = Helper.MemberContextManager;
                ChatProvider.IntegrationContext = Helper.IntegrationContext;

                // Send data to all identities and maintain an active connection (receivers that are on hosts).
                ChatProvider.SendToReceivers(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data);
            }
        }
    }

    /// <summary>
    /// Chat provider used for base http context connectivilty.
    /// </summary>
    internal sealed class ChatProvider : Nequeo.Web.WebServiceBase
    {
        #region Constructors
        /// <summary>
        /// Chat provider used for base http context connectivilty.
        /// </summary>
        public ChatProvider()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts).
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the receivers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        /// <returns>True if all the unique identifiers exist on this server; else false.</returns>
        public bool SendToClients(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data)
        {
            // Construct the message headers.
            List<Nequeo.Model.NameValue> headers = new List<Model.NameValue>()
            {
                new Model.NameValue() { Name = "Member", Value = "ReceiveDataMessage"},
                new Model.NameValue() { Name = "UniqueIdentifier", Value = uniqueIdentifier},
                new Model.NameValue() { Name = "ServiceName", Value = serviceName},
                new Model.NameValue() { Name = "ServiceNameUniqueIdentifiers", Value = serviceNameUniqueIdentifiers},
                new Model.NameValue() { Name = "Content-Length", Value = data.Length.ToString()},
            };

            // Create the header list.
            string headerList = Nequeo.Net.Utility.CreateWebResponseHeaders(headers);
            byte[] headersData = Encoding.Default.GetBytes(headerList);

            // Get the combined data to send to all clients.
            byte[] combinedData = headersData.CombineParallel(data);

            // Send the data to the clients connected to this system.
            return base.SendToClients(combinedData, uniqueIdentifiers);
        }

        /// <summary>
        /// Send data to all identities and maintain an active connection (receivers that are on hosts).
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that is sending the data.</param>
        /// <param name="serviceName">The service name the unique identifier is connected to.</param>
        /// <param name="serviceNameUniqueIdentifiers">The service name the unique identifiers are connected to.</param>
        /// <param name="uniqueIdentifiers">The list of unique identities to send the data to.</param>
        /// <param name="data">The data to send to the receivers.</param>
        public override void SendToReceivers(string uniqueIdentifier, string serviceName, string serviceNameUniqueIdentifiers, string[] uniqueIdentifiers, byte[] data)
        {
            bool withinMachine = false;

            // If the same service name.
            if (serviceName.ToLower() == serviceNameUniqueIdentifiers.ToLower())
                withinMachine = SendToClients(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data);

            // If clients exist on other machines
            // then send the data to thows clients.
            if (!withinMachine)
            {
                // Send data to all identities and maintain an active connection (receivers that are on hosts).
                base.SendToReceivers(uniqueIdentifier, serviceName, serviceNameUniqueIdentifiers, uniqueIdentifiers, data);
            }
        }
        #endregion
    }
}