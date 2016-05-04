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
using System.ServiceModel.Configuration;
using System.Runtime.Serialization;

namespace Nequeo.Net.CodeGeneration.ServiceModel
{
    /// <summary>
    /// Service model code generation.
    /// </summary>
    public sealed class ServiceModelMex
    {
        /// <summary>
        /// Generates the service model client code for the endpoint.
        /// </summary>
        /// <param name="endpointAddress">The metadata endpoint address.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <param name="operationTimeout">The operation time.</param>
        /// <param name="codeFilePath">The full path an file name where the C# code is written</param>
        public static void GenerateServiceModelClientCode(EndpointAddress endpointAddress, 
            MetadataExchangeClientMode mexMode, TimeSpan operationTimeout, string codeFilePath)
        {
            // Get the meta data set from the endpoint.
            System.ServiceModel.Description.MetadataSet docs =
                 Nequeo.Net.ServiceModel.ServiceModelInformation.GetServiceMetadata(endpointAddress, mexMode, operationTimeout);

            // Generate the code file
            GenerateServiceModelClient(docs, codeFilePath);
        }

        /// <summary>
        /// Generates the service model client configuration code for the endpoint.
        /// </summary>
        /// <param name="endpointAddress">The metadata endpoint address.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <param name="operationTimeout">The operation time.</param>
        /// <param name="xmlFile">The name of the file where client configuration is written, 
        /// the file is created in the same path as the application configuration file (debug or release).</param>
        public static void GenerateServiceModelClientConfigurationCode(EndpointAddress endpointAddress, 
            MetadataExchangeClientMode mexMode, TimeSpan operationTimeout, string xmlFile)
        {
            // Get the meta data set from the endpoint.
            System.ServiceModel.Description.MetadataSet docs =
                 Nequeo.Net.ServiceModel.ServiceModelInformation.GetServiceMetadata(endpointAddress, mexMode, operationTimeout);

            // Generate the code file
            GenerateServiceModelEndpoints(docs, xmlFile, Nequeo.Handler.Common.InfoHelper.GetApplicationConfigurationFile());
        }

        /// <summary>
        /// Generates the service model client configuration code for the endpoint.
        /// </summary>
        /// <param name="endpointAddress">The metadata endpoint address.</param>
        /// <param name="mexMode">The metadata exchange client mode.</param>
        /// <param name="operationTimeout">The operation time.</param>
        public static void GenerateServiceModelClientConfigurationCode(EndpointAddress endpointAddress,
            MetadataExchangeClientMode mexMode, TimeSpan operationTimeout)
        {
            // Get the meta data set from the endpoint.
            System.ServiceModel.Description.MetadataSet docs =
                 Nequeo.Net.ServiceModel.ServiceModelInformation.GetServiceMetadata(endpointAddress, mexMode, operationTimeout);

            // Generate the code file
            GenerateServiceModelEndpoints(docs, string.Empty, Nequeo.Handler.Common.InfoHelper.GetApplicationConfigurationFile());
        }

        /// <summary>
        /// Generates the service model client code for the metadat set.
        /// </summary>
        /// <param name="metaDocs">The metadata set document.</param>
        /// <param name="codeFile">The output file to write the code to.</param>
        public static void GenerateServiceModelClient(MetadataSet metaDocs, string codeFile)
        {
            // Make sure the page reference exists.
            if (metaDocs == null) throw new ArgumentNullException("metaDocs");
            if (codeFile == null) throw new ArgumentNullException("codeFile");

            WsdlImporter importer = new WsdlImporter(metaDocs);
            ServiceContractGenerator generator = new ServiceContractGenerator();
            
            // Add our custom DCAnnotationSurrogate 
            // to write XSD annotations into the comments.
            object dataContractImporter;
            XsdDataContractImporter xsdDCImporter;
            if (!importer.State.TryGetValue(typeof(XsdDataContractImporter), out dataContractImporter))
            {
                xsdDCImporter = new XsdDataContractImporter();
                xsdDCImporter.Options = new ImportOptions();
                importer.State.Add(typeof(XsdDataContractImporter), xsdDCImporter);
            }
            else
            {
                xsdDCImporter = (XsdDataContractImporter)dataContractImporter;
                if (xsdDCImporter.Options == null)
                    xsdDCImporter.Options = new ImportOptions();
            }

            // Get all the contract type bindings
            System.Collections.ObjectModel.Collection<ContractDescription> contracts = importer.ImportAllContracts();

            // Generate all the contracts
            foreach (ContractDescription contract in contracts)
                generator.GenerateServiceContractType(contract);

            if (generator.Errors.Count != 0)
                throw new System.Exception("There were errors during code compilation.");

            // Write the code dom
            System.CodeDom.Compiler.CodeGeneratorOptions options = new System.CodeDom.Compiler.CodeGeneratorOptions();
            options.BracingStyle = "C";

            System.CodeDom.Compiler.IndentedTextWriter textWriter = null;
            try
            {
                // Write data to the code file.
                System.CodeDom.Compiler.CodeDomProvider codeDomProvider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("C#");
                textWriter = new System.CodeDom.Compiler.IndentedTextWriter(new System.IO.StreamWriter(codeFile));
                codeDomProvider.GenerateCodeFromCompileUnit(generator.TargetCompileUnit, textWriter, options);
            }
            finally
            {
                if (textWriter != null)
                    textWriter.Close();
            }
        }

        /// <summary>
        /// Generates the service model endpoints for the metadata set in the application configuration file.
        /// </summary>
        /// <param name="metaDocs">The metadata set document.</param>
        /// <returns>The collection of channel endpoint elements.</returns>
        public static ChannelEndpointElement[] GenerateServiceModelEndpoints(MetadataSet metaDocs)
        {
            // Make sure the page reference exists.
            if (metaDocs == null) throw new ArgumentNullException("metaDocs");

            return GenerateServiceModelEndpoints(metaDocs, string.Empty, Nequeo.Handler.Common.InfoHelper.GetApplicationConfigurationFile());
        }

        /// <summary>
        /// Generates the service model endpoints for the metadata set in the xml file for the application configuration location.
        /// </summary>
        /// <param name="metaDocs">The metadata set document.</param>
        /// <param name="xmlFile">The name of the xml file to write the configuration information to.</param>
        /// <param name="configuration">The current application configuration file instance.</param>
        /// <returns>The collection of channel endpoint elements.</returns>
        public static ChannelEndpointElement[] GenerateServiceModelEndpoints(MetadataSet metaDocs, 
            string xmlFile, System.Configuration.Configuration configuration)
        {
            // Make sure the page reference exists.
            if (metaDocs == null) throw new ArgumentNullException("metaDocs");
            if (xmlFile == null) throw new ArgumentNullException("xmlFile");
            if (configuration == null) throw new ArgumentNullException("configuration");

            WsdlImporter importer = new WsdlImporter(metaDocs);
            ServiceContractGenerator generator = new ServiceContractGenerator(configuration);

            // Get all the endpoint type bindings
            ServiceEndpointCollection serviceEndpoints = importer.ImportAllEndpoints();

            // Create a new endpoint collection.
            List<ChannelEndpointElement> endpointElements = new List<ChannelEndpointElement>();
            System.ServiceModel.Configuration.ChannelEndpointElement channelEndpoint = null;

            // For each endpoint found add the
            // type to the collection.
            foreach (ServiceEndpoint serviceEndpoint in serviceEndpoints)
            {
                generator.GenerateServiceEndpoint(serviceEndpoint, out channelEndpoint);
                endpointElements.Add(channelEndpoint);
            }
            
            // If no file has been specified then save to the application configuration
            // file else save to a new file specified.
            if (String.IsNullOrEmpty(xmlFile))
                generator.Configuration.Save();
            else
                generator.Configuration.SaveAs(xmlFile);

            // Return the collection of endpoints.
            return endpointElements.ToArray();
        }
    }
}
