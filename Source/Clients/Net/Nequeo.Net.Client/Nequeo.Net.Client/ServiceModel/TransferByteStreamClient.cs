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
using System.IO;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

using Nequeo.IO.Stream.Extension;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Simple streamed data transfer byte client
    /// </summary>
    public class TransferByteStreamClient : Nequeo.Net.ServiceModel.ClientServiceManager<Nequeo.Net.ServiceModel.TransferByteStream.ByteStreamClient, Nequeo.Net.ServiceModel.TransferByteStream.IByteStream>
	{
        /// <summary>
        /// Secure Net Tcp Binding. Simple streamed data message client.
        /// </summary>
        public TransferByteStreamClient() :
            base(new Nequeo.Net.ServiceModel.TransferByteStream.ByteStreamClient("NetTcpBinding_IByteStream")) 
        {
            base.CreateRemoteCertificateValidation(base.OnCertificateValidationOverride);
            base.CustomCertificateValidation();
        }

        /// <summary>
        /// Simple streamed data message client.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        public TransferByteStreamClient(string endpointConfigurationName) :
            base(new Nequeo.Net.ServiceModel.TransferByteStream.ByteStreamClient(endpointConfigurationName)) { }

        /// <summary>
        /// Simple streamed data message client.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        public TransferByteStreamClient(string endpointConfigurationName, string remoteAddress) :
            base(new Nequeo.Net.ServiceModel.TransferByteStream.ByteStreamClient(endpointConfigurationName, remoteAddress)) { }

        /// <summary>
        /// Simple streamed data message client.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service endpoint.</param>
        public TransferByteStreamClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(new Nequeo.Net.ServiceModel.TransferByteStream.ByteStreamClient(endpointConfigurationName, remoteAddress)) { }

        /// <summary>
        /// Simple streamed data message client.
        /// </summary>
        /// <param name="binding">The binding with which to make calls to the service.</param>
        /// <param name="remoteAddress">The address of the service endpoint.</param>
        public TransferByteStreamClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(new Nequeo.Net.ServiceModel.TransferByteStream.ByteStreamClient(binding, remoteAddress)) { }

        /// <summary>
        /// Gets the current StreamClient instance.
        /// </summary>
        public Nequeo.Net.ServiceModel.TransferByteStream.ByteStreamClient Client
        {
            get { return base.Instance; }
        }

        /// <summary>
        /// Causes the System.ServiceModel.ClientBase TChannel object to transition
        /// from its current state into the closed state.
        /// </summary>
        public void Close()
        {
            base.Instance.Close();
        }

        /// <summary>
        /// Causes the System.ServiceModel.ClientBase TChannel object to transition
        /// from the created state into the opened state.
        /// </summary>
        public void Open()
        {
            base.Instance.Open();
        }
	}
}
