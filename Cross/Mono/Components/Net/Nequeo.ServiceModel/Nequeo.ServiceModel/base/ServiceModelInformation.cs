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
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// WCF service model general information class.
    /// </summary>
    public sealed class ServiceModelInformation
    {
        /// <summary>
        /// Gets all service endpoints for the contract interface type
        /// through the MEX exposed endpoint.
        /// </summary>
        /// <param name="contractType">The interface type contract of the service.</param>
        /// <param name="serviceEndpointAddress">The endpoint address of the service MEX.</param>
        /// <returns>The collection of service endpoints.</returns>
        public static ServiceEndpointCollection GetServiceEndpoints(Type contractType,
            string serviceEndpointAddress)
        {
            // Get all service endpoints for the current contract
            // at the specified service address, specifically get
            // the service information from the exposed 
            // MEX (Metadata Exchange) endpoint address.
            ServiceEndpointCollection serviceEndpoints =
                MetadataResolver.Resolve(contractType, new EndpointAddress(serviceEndpointAddress));

            // Return the collection of endpoints.
            return serviceEndpoints;
        }

        /// <summary>
        /// Gets all service endpoints for the contract interface type
        /// through the MEX exposed endpoint.
        /// </summary>
        /// <param name="contractType">The interface type contract of the service.</param>
        /// <param name="serviceEndpointAddress">The endpoint address of the service MEX.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <returns>The collection of service endpoints.</returns>
        public static ServiceEndpointCollection GetServiceEndpoints(Type contractType,
            Uri serviceEndpointAddress, MetadataExchangeClientMode mexMode)
        {
            // Get all service endpoints for the current contract
            // at the specified service address, specifically get
            // the service information from the exposed 
            // MEX (Metadata Exchange) endpoint address.
            ServiceEndpointCollection serviceEndpoints =
                MetadataResolver.Resolve(contractType, serviceEndpointAddress, mexMode);

            // Return the collection of endpoints.
            return serviceEndpoints;
        }

        /// <summary>
        /// Gets the service metadata information.
        /// </summary>
        /// <param name="metadataAddress">The metadata endpoint address.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <param name="operationTimeout">The operation time.</param>
        /// <returns>The metadata document set.</returns>
        public static MetadataSet GetServiceMetadata(
            EndpointAddress metadataAddress, MetadataExchangeClientMode mexMode, TimeSpan operationTimeout)
        {
            // Create a new metadata client to return
            // metadata information for the service.
            MetadataExchangeClient mexClient = new MetadataExchangeClient(metadataAddress.Uri, mexMode);

            // Assign each property
            mexClient.OperationTimeout = operationTimeout;
            mexClient.ResolveMetadataReferences = true;

            // Get the metadata from the service
            MetadataSet metaDocs = mexClient.GetMetadata();

            // Return the metadata document set.
            return metaDocs;
        }

        /// <summary>
        /// Gets the service metadata information.
        /// </summary>
        /// <param name="metadataAddress">The metadata endpoint address.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <param name="httpCredentials">The http credentials.</param>
        /// <param name="operationTimeout">The operation time.</param>
        /// <returns>The metadata document set.</returns>
        public static MetadataSet GetServiceMetadata(
            EndpointAddress metadataAddress, MetadataExchangeClientMode mexMode,
            ICredentials httpCredentials, TimeSpan operationTimeout)
        {
            // Create a new metadata client to return
            // metadata information for the service.
            MetadataExchangeClient mexClient = new MetadataExchangeClient(metadataAddress.Uri, mexMode);

            // Assign each property
            mexClient.OperationTimeout = operationTimeout;
            mexClient.HttpCredentials = httpCredentials;
            mexClient.ResolveMetadataReferences = true;

            // Get the metadata from the service
            MetadataSet metaDocs = mexClient.GetMetadata();

            // Return the metadata document set.
            return metaDocs;
        }

        /// <summary>
        /// Gets the service metadata information.
        /// </summary>
        /// <param name="metadataAddress">The metadata endpoint address.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <param name="soapCredentials">The soap client credentials.</param>
        /// <param name="operationTimeout">The operation time.</param>
        /// <returns>The metadata document set.</returns>
        public static MetadataSet GetServiceMetadata(
            EndpointAddress metadataAddress, MetadataExchangeClientMode mexMode,
            ClientCredentials soapCredentials, TimeSpan operationTimeout)
        {
            // Create a new metadata client to return
            // metadata information for the service.
            MetadataExchangeClient mexClient = new MetadataExchangeClient(metadataAddress.Uri, mexMode);

            // Assign each property
            mexClient.OperationTimeout = operationTimeout;
            mexClient.SoapCredentials = soapCredentials;
            mexClient.ResolveMetadataReferences = true;

            // Get the metadata from the service
            MetadataSet metaDocs = mexClient.GetMetadata();

            // Return the metadata document set.
            return metaDocs;
        }

        /// <summary>
        /// Gets the contract description.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <returns>The contract description</returns>
        public static ContractDescription GetContractDescription(Type contractType)
        {
            ContractDescription contractDescription = ContractDescription.GetContract(contractType);
            return contractDescription;
        }

        /// <summary>
        /// Gets the contract description.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="serviceImplementation">The service implementation instance.</param>
        /// <returns>The contract description</returns>
        public static ContractDescription GetContractDescription(Type contractType, object serviceImplementation)
        {
            ContractDescription contractDescription = ContractDescription.GetContract(contractType, serviceImplementation);
            return contractDescription;
        }

        /// <summary>
        /// Gets the contract description.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The contract description</returns>
        public static ContractDescription GetContractDescription(Type contractType, Type serviceType)
        {
            ContractDescription contractDescription = ContractDescription.GetContract(contractType, serviceType);
            return contractDescription;
        }

        /// <summary>
        /// Gets the service description.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service description</returns>
        public static ServiceDescription GetServiceDescription(Type serviceType)
        {
            ServiceDescription serviceDescription = ServiceDescription.GetService(serviceType);
            return serviceDescription;
        }

        /// <summary>
        /// Gets the service description.
        /// </summary>
        /// <param name="serviceImplementation">The service implementation instance.</param>
        /// <returns>The service description</returns>
        public static ServiceDescription GetServiceDescription(object serviceImplementation)
        {
            ServiceDescription serviceDescription = ServiceDescription.GetService(serviceImplementation);
            return serviceDescription;
        }

        /// <summary>
        /// Gets the service description for the contract interface type
        /// through the MEX exposed endpoint.
        /// </summary>
        /// <param name="contractType">The interface type contract of the service.</param>
        /// <param name="serviceEndpointAddress">The endpoint address of the service MEX.</param>
        /// <returns>The service endpoint description.</returns>
        public static ServiceDescription GetServiceDescription(Type contractType,
            string serviceEndpointAddress)
        {
            ServiceEndpointCollection svcEndpoints = GetServiceEndpoints(contractType, serviceEndpointAddress);
            ServiceDescription svcDescription = new ServiceDescription(svcEndpoints.ToArray());
            return svcDescription;
        }

        /// <summary>
        /// Gets the service description for the contract interface type
        /// through the MEX exposed endpoint.
        /// </summary>
        /// <param name="contractType">The interface type contract of the service.</param>
        /// <param name="serviceEndpointAddress">The endpoint address of the service MEX.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <returns>The service endpoint description.</returns>
        public static ServiceDescription GetServiceDescription(Type contractType,
            Uri serviceEndpointAddress, MetadataExchangeClientMode mexMode)
        {
            ServiceEndpointCollection svcEndpoints = GetServiceEndpoints(contractType, serviceEndpointAddress, mexMode);
            ServiceDescription svcDescription = new ServiceDescription(svcEndpoints.ToArray());
            return svcDescription;
        }

        /// <summary>
        /// Gets the operation description
        /// </summary>
        /// <param name="name">The name of the operation description</param>
        /// <param name="contractDescription">A contract description instance.</param>
        /// <returns>The operation description</returns>
        public static OperationDescription GetOperationDescription(string name, ContractDescription contractDescription)
        {
            OperationDescription operationDescription = new OperationDescription(name, contractDescription);
            return operationDescription;
        }

        /// <summary>
        /// Gets the message description
        /// </summary>
        /// <param name="operationDescription">An operation description instance.</param>
        /// <param name="messageIndex">The operation description message index.</param>
        /// <returns>The message description</returns>
        public static MessageDescription GetMessageDescription(OperationDescription operationDescription, int messageIndex)
        {
            MessageDescription messageDescription = operationDescription.Messages[messageIndex];
            return messageDescription;
        }

        /// <summary>
        /// Gets the message body description
        /// </summary>
        /// <param name="messageDescription">A message description instance</param>
        /// <returns>The message body description</returns>
        public static MessageBodyDescription GetMessageBodyDescription(MessageDescription messageDescription)
        {
            MessageBodyDescription messageBodyDescription = messageDescription.Body;
            return messageBodyDescription;
        }

        /// <summary>
        /// Gets the message part description
        /// </summary>
        /// <param name="messageBodyDescription">A message body description instance</param>
        /// <param name="partIndex">The message body description part index.</param>
        /// <returns>The message part description</returns>
        public static MessagePartDescription GetMessagePartDescription(MessageBodyDescription messageBodyDescription, int partIndex)
        {
            MessagePartDescription messagePartDescription = messageBodyDescription.Parts[partIndex];
            return messagePartDescription;
        }
    }
}
