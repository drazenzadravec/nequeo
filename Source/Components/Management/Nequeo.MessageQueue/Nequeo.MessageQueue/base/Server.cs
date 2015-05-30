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
using System.Configuration;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.MsmqIntegration;

namespace Nequeo.Management.MessageQueue
{
    /// <summary>
    /// Message queue server contract
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(Message))]
    [ServiceKnownType(typeof(String))]
    public interface IServer
    {
        /// <summary>
        /// Send the messasge to the queue
        /// </summary>
        /// <param name="message">The message to send</param>
        [OperationContract(IsOneWay = true, Name = "SendMessage", Action = "*")]
        void SendMessage(MsmqMessage<Message> message);
    }

    /// <summary>
    /// Message queue server implementation
    /// </summary>
    internal class Server : IServer
	{
        /// <summary>
        /// Send the messasge to the queue
        /// </summary>
        /// <param name="message">The message to send</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void SendMessage(MsmqMessage<Message> message)
        {
            Message messageData = (Message)message.Body;
            object data = messageData.Data;
        }
	}

    /// <summary>
    /// Message queue service host
    /// </summary>
    public class MqServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Stream Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serviceHostType">The service host type</param>
        public MqServiceHost(Type serviceHostType)
            : base(serviceHostType)
        {
            // Get MSMQ queue name from app settings in configuration
            string queueName = Nequeo.Management.MessageQueue.Properties.Settings.Default.QueueName;

            // Create the transacted MSMQ queue if necessary.
            if (!System.Messaging.MessageQueue.Exists(queueName))
                System.Messaging.MessageQueue.Create(queueName, true);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        /// <param name="serviceHostType">The service host type</param>
        public MqServiceHost(Type serviceHostType, Uri[] baseAddresses)
            : base(serviceHostType, baseAddresses)
        {
            // Get MSMQ queue name from app settings in configuration
            string queueName = Nequeo.Management.MessageQueue.Properties.Settings.Default.QueueName;

            // Create the transacted MSMQ queue if necessary.
            if (!System.Messaging.MessageQueue.Exists(queueName))
                System.Messaging.MessageQueue.Create(queueName, true);
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        public void OpenServiceHost()
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.Open();
            }
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        /// <param name="binding">A specific biding instance.</param>
        public void OpenServiceHost(System.ServiceModel.Channels.Binding binding)
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Management.MessageQueue.IServer), binding, "");
                base.Open();
            }
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        /// <param name="binding">A specific biding instance.</param>
        /// <param name="address">The endpoint address.</param>
        public void OpenServiceHost(System.ServiceModel.Channels.Binding binding, string address)
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Management.MessageQueue.IServer), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }
}
