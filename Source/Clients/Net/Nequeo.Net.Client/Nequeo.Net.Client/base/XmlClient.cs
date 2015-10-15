/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net
{
    /// <summary>
    /// Xml client.
    /// </summary>
    public class HttpXmlClient
    {
        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="method">The request method.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <param name="credentials">The request network credentials.</param>
        /// <returns>The returned type.</returns>
        public static T Request<T>(Uri serviceUri,
            string method = "GET", System.IO.Stream input = null, NetworkCredential credentials = null)
        {
            // Is the connection secure.
            bool isSecure = false;
            if (serviceUri.Scheme.ToLower() == "https")
                isSecure = true;

            T data = default(T);

            // Open a new connection.
            using (Nequeo.Net.Client client = new Client(serviceUri, isSecureConnection: isSecure))
            {
                client.IsHttpProtocol = true;

                // Create the request.
                Nequeo.Net.NetRequest request = client.GetRequest();
                request.Method = method;
                request.ContentLength = (input != null ? input.Length : 0);

                if (credentials != null)
                    request.Credentials = credentials;

                // Send the data.
                client.Transfer(request, input, CancellationToken.None);

                // Get the response context.
                Nequeo.Net.NetContext context = client.GetContext();

                // Read all the headers.
                string resources = null;
                List<Model.NameValue> headers = Nequeo.Net.Utility.ParseHeaders(context.NetResponse.Input, out resources, client.NetClient.ResponseTimeout);

                // Parse the headers and the resource response.
                context.NetResponse.ReadNetResponseHeaders(headers, resources);

                // Open a stream.
                using (MemoryStream stream = new MemoryStream())
                {
                    // If sending chunked.
                    if (context.NetResponse.SendChunked)
                        Nequeo.Net.Utility.ReadChunkedData(context.NetResponse.Input, stream, client.NetClient.ResponseTimeout);
                    else
                        // Copy the response stream.
                        Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, context.NetResponse.ContentLength, client.NetClient.ResponseTimeout);

                    // Create the object from the xml data.
                    data = Nequeo.Serialisation.GenericSerialisation<T>.Instance.Deserialise(stream.ToArray());
                }
            }

            // Data.
            return data;
        }

        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="request">The request provider used to send the data.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <returns>The returned type.</returns>
        public static T Request<T>(Uri serviceUri, Nequeo.Net.NetRequest request, System.IO.Stream input = null)
        {
            // Is the connection secure.
            bool isSecure = false;
            if (serviceUri.Scheme.ToLower() == "https")
                isSecure = true;

            T data = default(T);

            // Open a new connection.
            using (Nequeo.Net.Client client = new Client(serviceUri, isSecureConnection: isSecure))
            {
                client.IsHttpProtocol = true;
                request.Output = client.GetRequest().Output;

                // Send the data.
                client.Transfer(request, input, CancellationToken.None);

                // Get the response context.
                Nequeo.Net.NetContext context = client.GetContext();

                // Read all the headers.
                string resources = null;
                List<Model.NameValue> headers = Nequeo.Net.Utility.ParseHeaders(context.NetResponse.Input, out resources, client.NetClient.ResponseTimeout);

                // Parse the headers and the resource response.
                context.NetResponse.ReadNetResponseHeaders(headers, resources);

                // Open a stream.
                using (MemoryStream stream = new MemoryStream())
                {
                    // If sending chunked.
                    if (context.NetResponse.SendChunked)
                        Nequeo.Net.Utility.ReadChunkedData(context.NetResponse.Input, stream, client.NetClient.ResponseTimeout);
                    else
                        // Copy the response stream.
                        Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, context.NetResponse.ContentLength, client.NetClient.ResponseTimeout);

                    // Create the object from the xml data.
                    data = Nequeo.Serialisation.GenericSerialisation<T>.Instance.Deserialise(stream.ToArray());
                }
            }

            // Data.
            return data;
        }

        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="headerList">The array of response headers.</param>
        /// <param name="response">The response message.</param>
        /// <param name="method">The request method.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <param name="credentials">The request network credentials.</param>
        /// <returns>The returned type.</returns>
        public static T Request<T>(Uri serviceUri,
            out Model.NameValue[] headerList, out Nequeo.Model.Message.ResponseResource response,
            string method = "GET", System.IO.Stream input = null, NetworkCredential credentials = null)
        {
            // Is the connection secure.
            bool isSecure = false;
            if (serviceUri.Scheme.ToLower() == "https")
                isSecure = true;

            T data = default(T);

            // Open a new connection.
            using (Nequeo.Net.Client client = new Client(serviceUri, isSecureConnection: isSecure))
            {
                client.IsHttpProtocol = true;

                // Create the request.
                Nequeo.Net.NetRequest request = client.GetRequest();
                request.Method = method;
                request.ContentLength = (input != null ? input.Length : 0);

                if (credentials != null)
                    request.Credentials = credentials;

                // Send the data.
                client.Transfer(request, input, CancellationToken.None);

                // Get the response context.
                Nequeo.Net.NetContext context = client.GetContext();

                // Read all the headers.
                string resources = null;
                List<Model.NameValue> headers = Nequeo.Net.Utility.ParseHeaders(context.NetResponse.Input, out resources, client.NetClient.ResponseTimeout);
                headerList = headers.ToArray();

                // Parse the headers and the resource response.
                context.NetResponse.ReadNetResponseHeaders(headers, resources);
                response = new Model.Message.ResponseResource()
                {
                    Code = context.NetResponse.StatusCode,
                    Subcode = context.NetResponse.StatusSubcode,
                    Description = context.NetResponse.StatusDescription,
                    ProtocolVersion = context.NetResponse.ProtocolVersion
                };

                // Open a stream.
                using (MemoryStream stream = new MemoryStream())
                {
                    // If sending chunked.
                    if (context.NetResponse.SendChunked)
                        Nequeo.Net.Utility.ReadChunkedData(context.NetResponse.Input, stream, client.NetClient.ResponseTimeout);
                    else
                        // Copy the response stream.
                        Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, context.NetResponse.ContentLength, client.NetClient.ResponseTimeout);

                    // Create the object from the xml data.
                    data = Nequeo.Serialisation.GenericSerialisation<T>.Instance.Deserialise(stream.ToArray());
                }
            }

            // Data.
            return data;
        }

        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="request">The request provider used to send the data.</param>
        /// <param name="headerList">The array of response headers.</param>
        /// <param name="response">The response message.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <returns>The returned type.</returns>
        public static T Request<T>(Uri serviceUri, Nequeo.Net.NetRequest request,
            out Model.NameValue[] headerList, out Nequeo.Model.Message.ResponseResource response,
            System.IO.Stream input = null)
        {
            // Is the connection secure.
            bool isSecure = false;
            if (serviceUri.Scheme.ToLower() == "https")
                isSecure = true;

            T data = default(T);

            // Open a new connection.
            using (Nequeo.Net.Client client = new Client(serviceUri, isSecureConnection: isSecure))
            {
                client.IsHttpProtocol = true;
                request.Output = client.GetRequest().Output;

                // Send the data.
                client.Transfer(request, input, CancellationToken.None);

                // Get the response context.
                Nequeo.Net.NetContext context = client.GetContext();

                // Read all the headers.
                string resources = null;
                List<Model.NameValue> headers = Nequeo.Net.Utility.ParseHeaders(context.NetResponse.Input, out resources, client.NetClient.ResponseTimeout);
                headerList = headers.ToArray();

                // Parse the headers and the resource response.
                context.NetResponse.ReadNetResponseHeaders(headers, resources);
                response = new Model.Message.ResponseResource()
                {
                    Code = context.NetResponse.StatusCode,
                    Subcode = context.NetResponse.StatusSubcode,
                    Description = context.NetResponse.StatusDescription,
                    ProtocolVersion = context.NetResponse.ProtocolVersion
                };

                // Open a stream.
                using (MemoryStream stream = new MemoryStream())
                {
                    // If sending chunked.
                    if (context.NetResponse.SendChunked)
                        Nequeo.Net.Utility.ReadChunkedData(context.NetResponse.Input, stream, client.NetClient.ResponseTimeout);
                    else
                        // Copy the response stream.
                        Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, context.NetResponse.ContentLength, client.NetClient.ResponseTimeout);

                    // Create the object from the xml data.
                    data = Nequeo.Serialisation.GenericSerialisation<T>.Instance.Deserialise(stream.ToArray());
                }
            }

            // Data.
            return data;
        }

        /// <summary>
        /// Process a http response.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="response">The array of bytes to transform.</param>
        /// <returns>The returned type.</returns>
        public static T Response<T>(byte[] response)
        {
            T data = default(T);

            // Create the object from the xml data.
            data = Nequeo.Serialisation.GenericSerialisation<T>.Instance.Deserialise(response);

            // Data.
            return data;
        }

        /// <summary>
        /// Process a http response.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="context">The client context response.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>The returned type.</returns>
        public static T ResponseAsync<T>(Nequeo.Net.NetContext context, long timeout = -1)
        {
            T data = default(T);

            // Read all the headers.
            string resources = null;
            List<Model.NameValue> headers = Nequeo.Net.Utility.ParseHeaders(context.NetResponse.Input, out resources, timeout);

            // Parse the headers and the resource response.
            context.NetResponse.ReadNetResponseHeaders(headers, resources);

            // Open a stream.
            using (MemoryStream stream = new MemoryStream())
            {
                // If sending chunked.
                if (context.NetResponse.SendChunked)
                    Nequeo.Net.Utility.ReadChunkedData(context.NetResponse.Input, stream, timeout);
                else
                    // Copy the response stream.
                    Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, context.NetResponse.ContentLength, timeout);

                // Create the object from the xml data.
                data = Nequeo.Serialisation.GenericSerialisation<T>.Instance.Deserialise(stream.ToArray());
            }

            // Data.
            return data;
        }

        /// <summary>
        /// Process a http response.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="context">The client context response.</param>
        /// <param name="headerList">The array of response headers.</param>
        /// <param name="response">The response message.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <returns>The returned type.</returns>
        public static T ResponseAsync<T>(Nequeo.Net.NetContext context,
            out Model.NameValue[] headerList, out Nequeo.Model.Message.ResponseResource response,
            long timeout = -1)
        {
            T data = default(T);

            // Read all the headers.
            string resources = null;
            List<Model.NameValue> headers = Nequeo.Net.Utility.ParseHeaders(context.NetResponse.Input, out resources, timeout);
            headerList = headers.ToArray();

            // Parse the headers and the resource response.
            context.NetResponse.ReadNetResponseHeaders(headers, resources);
            response = new Model.Message.ResponseResource()
            {
                Code = context.NetResponse.StatusCode,
                Subcode = context.NetResponse.StatusSubcode,
                Description = context.NetResponse.StatusDescription,
                ProtocolVersion = context.NetResponse.ProtocolVersion
            };

            // Open a stream.
            using (MemoryStream stream = new MemoryStream())
            {
                // If sending chunked.
                if (context.NetResponse.SendChunked)
                    Nequeo.Net.Utility.ReadChunkedData(context.NetResponse.Input, stream, timeout);
                else
                    // Copy the response stream.
                    Nequeo.IO.Stream.Operation.CopyStream(context.NetResponse.Input, stream, context.NetResponse.ContentLength, timeout);

                // Create the object from the xml data.
                data = Nequeo.Serialisation.GenericSerialisation<T>.Instance.Deserialise(stream.ToArray());
            }

            // Data.
            return data;
        }

        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="method">The request method.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        /// <param name="credentials">The request network credentials.</param>
        public static void RequestAsync(Uri serviceUri,
            Action<Nequeo.Net.NetContext, object> callback, string method = "GET", 
            System.IO.Stream input = null, NetworkCredential credentials = null)
        {
            // Is the connection secure.
            bool isSecure = false;
            if (serviceUri.Scheme.ToLower() == "https")
                isSecure = true;

            // Open a new connection.
            Nequeo.Net.Client client = new Client(serviceUri, isSecureConnection: isSecure);
            client.IsHttpProtocol = true;

            // Create the request.
            Nequeo.Net.NetRequest request = client.GetRequest();
            request.Method = method;
            request.ContentLength = (input != null ? input.Length : 0);

            if (credentials != null)
                request.Credentials = credentials;

            // Send the data.
            client.Transfer(request, input, CancellationToken.None, callback, client);
        }

        /// <summary>
        /// Process a http request.
        /// </summary>
        /// <param name="serviceUri">The service name to call.</param>
        /// <param name="request">The request provider used to send the data.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="input">The stream containing the data to send (can be null).</param>
        public static void RequestAsync(Uri serviceUri, Nequeo.Net.NetRequest request,
            Action<Nequeo.Net.NetContext, object> callback, System.IO.Stream input = null)
        {
            // Is the connection secure.
            bool isSecure = false;
            if (serviceUri.Scheme.ToLower() == "https")
                isSecure = true;

            // Open a new connection.
            Nequeo.Net.Client client = new Client(serviceUri, isSecureConnection: isSecure);
            client.IsHttpProtocol = true;
            request.Output = client.GetRequest().Output;

            // Send the data.
            client.Transfer(request, input, CancellationToken.None, callback, client);
        }
    }
}