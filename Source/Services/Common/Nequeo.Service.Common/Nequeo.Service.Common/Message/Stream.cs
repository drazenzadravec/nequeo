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
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Web.Hosting;
using System.Web;

using Nequeo.Handler;
using Nequeo.Composite.Configuration;

namespace Nequeo.Service.Message
{
    /// <summary>
    /// Web host simple message transfer streaming.
    /// </summary>
    public class StreamWebHost : Stream
    {
        /// <summary>
        /// Compose the MEF instance.
        /// </summary>
        internal override void Compose()
        {
            string assemblyPath = HostingEnvironment.ApplicationPhysicalPath + "\\Bin";
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(assemblyPath, "*MessageCompositionAssembly.dll"));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }

    /// <summary>
    /// Simple message transfer streaming.
    /// </summary>
    public class Stream : IStream
    {
        /// <summary>
        /// Message handler interface
        /// </summary>
        [ImportMany]
        internal IEnumerable<IMessageHandler> MessageHandler { get; set; }

        /// <summary>
        /// Compose the MEF instance.
        /// </summary>
        internal virtual void Compose()
        {
            // Read from the configuration file
            // all the directories that contain
            // any composite services.
            Nequeo.Composite.Configuration.Reader reader = new Nequeo.Composite.Configuration.Reader();
            string[] paths = reader.GetServicePaths();

            AggregateCatalog catalog = new AggregateCatalog();

            // For each directory found search for composite server assemplies
            // and add the reference of the assemblies into the handler collection
            foreach (string path in paths)
                catalog.Catalogs.Add(new DirectoryCatalog(path, "*MessageCompositionAssembly.dll"));

            // Add the collection catalog to the composite container
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        /// <summary>
        /// Send and recieve byte message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <returns>The send message</returns>
        public byte[] MessageByte(byte[] message)
        {
            Serialisation.GenericSerialisation<Model.Message.InformationRequest> serlRequest = null;
            Serialisation.GenericSerialisation<Model.Message.InformationResponse> serlResponse = null;
            Model.Message.InformationResponse response = null;
            Model.Message.InformationRequest request = null;

            try
            {
                // Initialise the composition assembly collection.
                Compose();

                // Create the instance of the request and response serialised objects.
                serlRequest = new Serialisation.GenericSerialisation<Model.Message.InformationRequest>();
                serlResponse = new Serialisation.GenericSerialisation<Model.Message.InformationResponse>();

                // Get the request data.
                request = serlRequest.Deserialise(message);

                // Is there a collection of imported assemblies.
                if (MessageHandler.Count() < 1)
                    throw new Exception("No composition assemblies have been loaded.");

                // Get response.
                response = MessageHandler.First(u => u.GetResponse(request, true) != null).GetResponse(request, false);

                // If no response.
                if (response == null)
                    throw new Exception("Request name is not supported.");

                // Return the response data.
                return serlResponse.Serialise(response);
            }
            catch (Exception ex)
            {
                // Create the response data.
                response = new Model.Message.InformationResponse()
                {
                    RequestName = (request != null ? request.RequestName : ""),
                    ReturnCode = 1,
                    ErrorMessage = ex.Message,
                    Body = ""
                };

                // Return the response data.
                return serlResponse.Serialise(response);
            }
            finally
            {
                // Clean-up
                if (serlRequest != null)
                    serlRequest.Dispose();

                // Clean-up
                if (serlResponse != null)
                    serlResponse.Dispose();
            }
        }

        /// <summary>
        /// Send and recieve string message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <returns>The send message</returns>
        public string MessageString(string message)
        {
            Serialisation.GenericSerialisation<Model.Message.InformationRequest> serlRequest = null;
            Serialisation.GenericSerialisation<Model.Message.InformationResponse> serlResponse = null;
            Model.Message.InformationResponse response = null;
            Model.Message.InformationRequest request = null;

            try
            {
                // Initialise the composition assembly collection.
                Compose();

                // Create the instance of the request and response serialised objects.
                serlRequest = new Serialisation.GenericSerialisation<Model.Message.InformationRequest>();
                serlResponse = new Serialisation.GenericSerialisation<Model.Message.InformationResponse>();

                // Get the request data.
                request = serlRequest.Deserialise(Encoding.UTF8.GetBytes(message));

                // Is there a collection of imported assemblies.
                if (MessageHandler.Count() < 1)
                    throw new Exception("No composition assemblies have been loaded.");

                // Get response.
                response = MessageHandler.First(u => u.GetResponse(request, true) != null).GetResponse(request, false);

                // If no response.
                if (response == null)
                    throw new Exception("Request name is not supported.");

                // Return the response data.
                return Encoding.UTF8.GetString(serlResponse.Serialise(response));
            }
            catch (Exception ex)
            {
                // Create the response data.
                response = new Model.Message.InformationResponse()
                {
                    RequestName = (request != null ? request.RequestName : ""),
                    ReturnCode = 1,
                    ErrorMessage = ex.Message,
                    Body = ""
                };

                // Return the response data.
                return Encoding.UTF8.GetString(serlResponse.Serialise(response));
            }
            finally
            {
                // Clean-up
                if (serlRequest != null)
                    serlRequest.Dispose();

                // Clean-up
                if (serlResponse != null)
                    serlResponse.Dispose();
            }
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="request">The request information.</param>
        /// <returns>The response information.</returns>
        public Model.Message.InformationResponse SendMessage(Model.Message.InformationRequest request)
        {
            Model.Message.InformationResponse response = null;

            try
            {
                // Initialise the composition assembly collection.
                Compose();

                // Is there a collection of imported assemblies.
                if (MessageHandler.Count() < 1)
                    throw new Exception("No composition assemblies have been loaded.");

                // Get response.
                response = MessageHandler.First(u => u.GetResponse(request, true) != null).GetResponse(request, false);

                // If no response.
                if (response == null)
                    throw new Exception("Request name is not supported.");

                // Return the response data.
                return response;
            }
            catch (Exception ex)
            {
                // Create the response data.
                response = new Model.Message.InformationResponse()
                {
                    RequestName = request.RequestName,
                    ReturnCode = 1,
                    ErrorMessage = ex.Message,
                    Body = ""
                };

                // Return the response data.
                return response;
            }
        }
    }
}
