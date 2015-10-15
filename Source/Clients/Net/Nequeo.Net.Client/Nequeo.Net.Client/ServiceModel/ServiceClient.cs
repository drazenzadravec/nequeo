/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Nequeo.Model;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// WCF service client.
    /// </summary>
    public sealed class ServiceClient : Nequeo.Net.ServiceModel.DataServiceClient
    {
        /// <summary>
        /// WCF service client.
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        public ServiceClient(Uri serviceRoot) : base(serviceRoot)
        {
        }

        /// <summary>
        /// Request data from the service entity name.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>The array of data.</returns>
        public override T[] Request<T>(string serviceEntityName)
        {
            return base.Request<T>(serviceEntityName);
        }

        /// <summary>
        /// Request data from the service entity name.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>The array of data.</returns>
        public override T[] Request<T>(string serviceEntityName, NameObject[] queries)
        {
            return base.Request<T>(serviceEntityName, queries);
        }

        /// <summary>
        /// Request data from the service entity name.
        /// </summary>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>The array of bytes.</returns>
        public byte[] Request(string serviceEntityName)
        {
            return Request(serviceEntityName, null);
        }

        /// <summary>
        /// Request data from the service entity name.
        /// </summary>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>The array of bytes.</returns>
        public byte[] Request(string serviceEntityName, NameObject[] queries)
        {
            // Get the constructed URI.
            Uri uri = base.CreateUri(serviceEntityName, queries);

            // Return the response data.
            return Nequeo.Net.HttpDataClient.Request(uri, credentials: base.Credentials);
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public override List<PropertyInfoModel[]> Response(byte[] xmlData)
        {
            return base.Response(xmlData);
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <typeparam name="T">The type to translate the xml data to.</typeparam>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public override T[] Response<T>(byte[] xmlData)
        {
            return base.Response<T>(xmlData);
        }

        /// <summary>
        /// Process the response of xml data to the type array, from a WCF request.
        /// </summary>
        /// <param name="xmlData">The byte array containing the xml data.</param>
        /// <returns>The transformed data type array.</returns>
        public override object[] ResponseEx(byte[] xmlData)
        {
            return base.ResponseEx(xmlData);
        }

        /// <summary>
        /// Request async data from the service entity name.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>The array of data.</returns>
        public Task<T[]> RequestAsync<T>(string serviceEntityName)
        {
            return Task<T[]>.Factory.StartNew(() =>
            {
                T[] data = null;

                try
                {
                    // Make the request.
                    data = Request<T>(serviceEntityName);
                }
                catch (Exception) { }

                // Return the result.
                return data;
            });
        }

        /// <summary>
        /// Request async data from the service entity name.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>The array of data.</returns>
        public Task<T[]> RequestAsync<T>(string serviceEntityName, NameObject[] queries)
        {
            return Task<T[]>.Factory.StartNew(() =>
            {
                T[] data = null;

                try
                {
                    // Make the request.
                    data = Request<T>(serviceEntityName, queries);
                }
                catch (Exception) { }

                // Return the result.
                return data;
            });
        }

        /// <summary>
        /// Request async data from the service entity name.
        /// </summary>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <returns>The array of data.</returns>
        public Task<byte[]> RequestAsync(string serviceEntityName)
        {
            return Task<byte[]>.Factory.StartNew(() =>
            {
                byte[] data = null;

                try
                {
                    // Make the request.
                    data = Request(serviceEntityName);
                }
                catch (Exception) { }

                // Return the result.
                return data;
            });
        }

        /// <summary>
        /// Request async data from the service entity name.
        /// </summary>
        /// <param name="serviceEntityName">The service entity name.</param>
        /// <param name="queries">The array of name and value query pairs.</param>
        /// <returns>The array of data.</returns>
        public Task<byte[]> RequestAsync(string serviceEntityName, NameObject[] queries)
        {
            return Task<byte[]>.Factory.StartNew(() =>
            {
                byte[] data = null;

                try
                {
                    // Make the request.
                    data = Request(serviceEntityName, queries);
                }
                catch (Exception) { }

                // Return the result.
                return data;
            });
        }
    }
}
