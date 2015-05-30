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
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Management.ServiceModel
{
    /// <summary>
    /// Message server contract
    /// </summary>
    [ServiceContract]
    public interface IServer
    {
        /// <summary>
        /// Send the messasge to the queue
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>The message response</returns>
        [OperationContract(Name = "SendMessage")]
        Message SendMessage(Message message);
    }

    /// <summary>
    /// Message server implementation
    /// </summary>
    internal class Server : IServer
	{
        /// <summary>
        /// Send the messasge to the queue
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>The message response</returns>
        public Message SendMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Message REST server implementation
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    internal class RestServer
    {
        /// <summary>
        /// A RESTful implementation of WCF example only. UriTemplate = "*" will route all URIs to one method.
        /// When using WebGet all transfered data is in the query string.
        /// </summary>
        [OperationContract()]
        [WebGet(UriTemplate = "*")]
        public System.ServiceModel.Channels.Message AllRestFulMessages1(System.ServiceModel.Channels.Message message)
        {
            WebOperationContext context = WebOperationContext.Current;
            Uri uri = context.IncomingRequest.UriTemplateMatch.RequestUri;
            NameValueCollection queryString = context.IncomingRequest.UriTemplateMatch.QueryParameters;

            System.ServiceModel.Channels.Message response = System.ServiceModel.Channels.Message.CreateMessage(
                System.ServiceModel.Channels.MessageVersion.None, "*", "Simple response string");
            return response;
        }

        /// <summary>
        /// A RESTful implementation of WCF example only. UriTemplate = "*" will route all URIs to one method.
        /// When using WebInvoke Method = GET, all transfered data is in the query string.
        /// </summary>
        [OperationContract()]
        [WebInvoke(UriTemplate = "*", Method = "GET")]
        public System.ServiceModel.Channels.Message AllRestFulMessages2(System.ServiceModel.Channels.Message message)
        {
            WebOperationContext context = WebOperationContext.Current;
            Uri uri = context.IncomingRequest.UriTemplateMatch.RequestUri;
            NameValueCollection queryString = context.IncomingRequest.UriTemplateMatch.QueryParameters;

            System.ServiceModel.Channels.Message response = System.ServiceModel.Channels.Message.CreateMessage(
                System.ServiceModel.Channels.MessageVersion.None, "*", "Simple response string");
            return response;
        }

        /// <summary>
        /// A RESTful implementation of WCF example only. UriTemplate = "*" will route all URIs to one method.
        /// When using WebInvoke Method = POST, all transfered data is in the form collection.
        /// </summary>
        [OperationContract()]
        [WebInvoke(UriTemplate = "*", Method = "POST")]
        public System.ServiceModel.Channels.Message AllRestFulMessages3(System.ServiceModel.Channels.Message message)
        {
            WebOperationContext context = WebOperationContext.Current;
            Uri uri = context.IncomingRequest.UriTemplateMatch.RequestUri;
            NameValueCollection form = HttpContext.Current.Request.Form;

            System.ServiceModel.Channels.Message response = System.ServiceModel.Channels.Message.CreateMessage(
                System.ServiceModel.Channels.MessageVersion.None, "*", "Simple response string");
            return response;
        }
    }

    /// <summary>
    /// Sevice Model (Basic, NetTcp, WS) service host
    /// </summary>
    public class ServiceModelHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Stream Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serviceHostType">The service host type</param>
        public ServiceModelHost(Type serviceHostType)
            : base(serviceHostType)
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        /// <param name="serviceHostType">The service host type</param>
        public ServiceModelHost(Type serviceHostType, Uri[] baseAddresses)
            : base(serviceHostType, baseAddresses)
        {
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        public void OpenServiceHost()
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                OpenConnection();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Management.ServiceModel.IServer), binding, "");
                OpenConnection();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Management.ServiceModel.IServer), binding, address);
                OpenConnection();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }

        /// <summary>
        /// Open the new service connection.
        /// </summary>
        private void OpenConnection()
        {
            X509Certificate2 certificate = null;
            try
            {
                // Load the server certificate.
                certificate = new Nequeo.Security.Configuration.Reader().GetServerCredentials();
            }
            catch (Exception)
            {
            }

            if (certificate != null)
                base.Open(certificate);
            else
                base.Open();
        }
        #endregion
    }

    /// <summary>
    /// Sevice Model RESTful (Web) service host
    /// </summary>
    public class RestServiceModelHost : Nequeo.Net.ServiceModel.WebServiceManager
    {
        #region Stream Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serviceHostType">The service host type</param>
        public RestServiceModelHost(Type serviceHostType)
            : base(serviceHostType)
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        /// <param name="serviceHostType">The service host type</param>
        public RestServiceModelHost(Type serviceHostType, Uri[] baseAddresses)
            : base(serviceHostType, baseAddresses)
        {
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        public void OpenServiceHost()
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                OpenConnection();
            }
        }

        /// <summary>
        /// Open the service host.
        /// </summary>
        /// <param name="binding">A specific biding instance.</param>
        /// <param name="address">The endpoint address.</param>
        public void OpenServiceHost(System.ServiceModel.WebHttpBinding binding, string address)
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.ServiceHost.AddServiceEndpoint(base.ServiceType, binding, address);
                OpenConnection();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }

        /// <summary>
        /// Open the new service connection.
        /// </summary>
        private void OpenConnection()
        {
            X509Certificate2 certificate = null;
            try
            {
                // Load the server certificate.
                certificate = new Nequeo.Security.Configuration.Reader().GetServerCredentials();
            }
            catch (Exception)
            {
            }

            if (certificate != null)
                base.Open(certificate);
            else
                base.Open();
        }
        #endregion
    }
}
