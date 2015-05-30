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
using System.Data;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Compilation;
using System.Xml.Serialization;

using Nequeo.Handler.Global;
using Nequeo.Handler;

namespace Nequeo.Net.ServiceModel.Common
{
    /// <summary>
    /// Handles errors that occur within public services when
    /// client to service interaction ocurres.
    /// </summary>
    public class CustomErrorHandler : IErrorHandler
    {
        string _configurationName = string.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        public CustomErrorHandler(string configurationName)
        {
            _configurationName = configurationName;
        }

        /// <summary>
        /// Handles any error that occured between the client and the service.
        /// </summary>
        /// <param name="error">The error that occurred in the service.</param>
        /// <returns>True if the error was handled else false.</returns>
        public bool HandleError(Exception error)
        {
            LogHandler.WriteTypeMessage(error.Message, _configurationName);
            return true;
        }

        /// <summary>
        /// Provides the error message back to the client through the message provider.
        /// </summary>
        /// <param name="error">The current error that has occured.</param>
        /// <param name="version">The SOAP message version.</param>
        /// <param name="fault">The communication channel between the client and server,
        /// sends the message with the error to the client.</param>
        public void ProvideFault(Exception error, 
            System.ServiceModel.Channels.MessageVersion version, 
            ref System.ServiceModel.Channels.Message fault)
        {
            // Construct the fault message to send to
            // the client.
            FaultException faultException = new FaultException(error.Message);
            MessageFault messageFault = faultException.CreateMessageFault();
            fault = System.ServiceModel.Channels.Message.CreateMessage(version, messageFault, faultException.Action);
        }
    }

    /// <summary>
    /// Custom error behavior attribute for service
    /// error handling.
    /// </summary>
    public sealed class ErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        #region Error Behavior Attribute
        private Type _errorHandlerType;
        string _configurationName = string.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="errorHandlerType">The current error handler type.</param>
        /// <param name="configurationName">The BaseTypeHandlerDefaultSection Extension name.</param>
        public ErrorBehaviorAttribute(Type errorHandlerType, string configurationName)
        {
            _errorHandlerType = errorHandlerType;
            _configurationName = configurationName;
        }

        /// <summary>
        /// Gets the current error handler type.
        /// </summary>
        public Type ErrorHandlerType
        {
            get { return _errorHandlerType; }
        }

        /// <summary>
        /// Gets the current error handler type.
        /// </summary>
        public String ConfigurationName
        {
            get { return _configurationName; }
        }

        /// <summary>
        /// Add the binding parameters.
        /// </summary>
        /// <param name="serviceDescription">The current service description.</param>
        /// <param name="serviceHostBase">The service host base.</param>
        /// <param name="endpoints">The current end point collection.</param>
        /// <param name="bindingParameters">The parameters.</param>
        void IServiceBehavior.AddBindingParameters(
            ServiceDescription serviceDescription, 
            System.ServiceModel.ServiceHostBase serviceHostBase, 
            System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, 
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Apply the dispatch error behavior.
        /// </summary>
        /// <param name="serviceDescription">The current service description.</param>
        /// <param name="serviceHostBase">The service host base.</param>
        void IServiceBehavior.ApplyDispatchBehavior(
            ServiceDescription serviceDescription, 
            System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler;

            try
            {
                // Create a new instance of the error handler.
                errorHandler = (IErrorHandler)Activator.CreateInstance(
                    _errorHandlerType, new object[] { (String.IsNullOrEmpty(_configurationName) ? "" : _configurationName) });
            }
            catch (MissingMethodException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the " +
                    "ErrorBehaviorAttribute constructor must have a public empty constructor.", e);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the " +
                    "ErrorBehaviorAttribute constructor must implement " +
                    "System.ServiceModel.Dispatcher.IErrorHandler.", e);
            }

            // For each channel add the error handler types.
            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }

        /// <summary>
        /// Validate the current error behavior.
        /// </summary>
        /// <param name="serviceDescription">The current service description.</param>
        /// <param name="serviceHostBase">The service host base.</param>
        void IServiceBehavior.Validate(
            ServiceDescription serviceDescription, 
            System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }
        #endregion
    }

    /// <summary>
    /// The JSON Data Table Service object.
    /// </summary>
    [Serializable()]
    public class JSonDataTableService
    {
        #region JSon DataTable Service
        /// <summary>
        /// Gets sets, information for datatables to use for rendering.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string sEcho
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, total records
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int iTotalRecords
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, total records after filtering
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int iTotalDisplayRecords
        {
            get;
            set;
        }

        /// <summary>
        /// Gets sets, the two dimensional array of data.
        /// </summary>
        [XmlArray(IsNullable = true)]
        public string[,] aaData
        {
            get;
            set;
        }
        #endregion
    }
}
