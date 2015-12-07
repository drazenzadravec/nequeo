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
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

using Nequeo.Handler;

namespace Nequeo.Service
{
    /// <summary>
    /// Class that controls all custom server instances
    /// and threads.
    /// </summary>
    public class StreamServiceHostControl
    {
        #region Stream Service Host Controller

        private StreamServiceHost streamHost = null;
        private ByteStreamServiceHost streamByteHost = null;
        private StreamMessageServiceHost streamMessageHost = null;

        private Thread threadStreamHost = null;
        private Thread threadStreamByteHost = null;
        private Thread threadStreamMessageHost = null;

        /// <summary>
        /// Initialse all objects, create new
        /// instances of all servers and cleanup
        /// objects when complete.
        /// </summary>
        /// <param name="create"></param>
        private void Initialise(bool create)
        {
            // Create new instances.
            if (create)
            {
                // Create a new file transfer host
                // with default configuration setting.
                streamHost = new StreamServiceHost();

                // Create a new file transfer host
                // with default configuration setting.
                streamByteHost = new ByteStreamServiceHost();

                // Create a new file transfer host
                // with default configuration setting.
                streamMessageHost = new StreamMessageServiceHost();
            }
            else
            {
                // Dispose of all the servers.
                if (streamHost != null)
                    streamHost.Dispose();

                // Dispose of all the servers.
                if (streamByteHost != null)
                    streamByteHost.Dispose();

                // Dispose of all the servers.
                if (streamMessageHost != null)
                    streamMessageHost.Dispose();

                // Cleanup threads.
                threadStreamHost = null;

                // Cleanup threads.
                threadStreamByteHost = null;

                // Cleanup threads.
                threadStreamMessageHost = null;
            }
        }

        /// <summary>
        /// Starts all server threads.
        /// </summary>
        public void StartServerThreads()
        {
            // Initialise all custom server
            // instances.
            Initialise(true);

            // Create new threads for each
            // file transfer server.
            threadStreamHost = new Thread(new ThreadStart(streamHost.OpenServiceHost));
            threadStreamHost.IsBackground = true;
            threadStreamHost.Start();
            Thread.Sleep(20);

            // Create new threads for each
            // file transfer server.
            threadStreamByteHost = new Thread(new ThreadStart(streamByteHost.OpenServiceHost));
            threadStreamByteHost.IsBackground = true;
            threadStreamByteHost.Start();
            Thread.Sleep(20);

            // Create new threads for each
            // file transfer server.
            threadStreamMessageHost = new Thread(new ThreadStart(streamMessageHost.OpenServiceHost));
            threadStreamMessageHost.IsBackground = true;
            threadStreamMessageHost.Start();
            Thread.Sleep(20);
        }

        /// <summary>
        /// Stop all server from listening and
        /// abort all server threads.
        /// </summary>
        public void StopServerThreads()
        {
            // Stop all file transfer
            // servers from listening.
            if (streamHost != null)
                streamHost.CloseServiceHost();

            // Stop all file transfer
            // servers from listening.
            if (streamByteHost != null)
                streamByteHost.CloseServiceHost();

            // Stop all file transfer
            // servers from listening.
            if (streamMessageHost != null)
                streamMessageHost.CloseServiceHost();

            // Abort all threads created
            // for file transfer instances.
            if (threadStreamHost != null)
                if (threadStreamHost.IsAlive)
                {
                    threadStreamHost.Abort();
                    threadStreamHost.Join();
                    Thread.Sleep(20);
                }

            // Abort all threads created
            // for file transfer instances.
            if (threadStreamByteHost != null)
                if (threadStreamByteHost.IsAlive)
                {
                    threadStreamByteHost.Abort();
                    threadStreamByteHost.Join();
                    Thread.Sleep(20);
                }

            // Abort all threads created
            // for file transfer instances.
            if (threadStreamMessageHost != null)
                if (threadStreamMessageHost.IsAlive)
                {
                    threadStreamMessageHost.Abort();
                    threadStreamMessageHost.Join();
                    Thread.Sleep(20);
                }

            // Clean up objects.
            Initialise(false);
        }
        #endregion
    }

    /// <summary>
    /// Stream service host
    /// </summary>
    public class StreamServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Stream Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public StreamServiceHost()
            : base(typeof(Nequeo.Service.Transfer.Stream))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public StreamServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.Transfer.Stream), baseAddresses)
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.Transfer.IStream), binding, "");
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.Transfer.IStream), binding, address);
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
            catch { }

            if (certificate != null)
                base.Open(certificate);
            else
                base.Open();
        }
        #endregion
    }

    /// <summary>
    /// Byte stream service host
    /// </summary>
    public class ByteStreamServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Stream Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public ByteStreamServiceHost()
            : base(typeof(Nequeo.Service.Transfer.ByteStream))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public ByteStreamServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.Transfer.ByteStream), baseAddresses)
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.Transfer.IByteStream), binding, "");
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.Transfer.IByteStream), binding, address);
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
            catch { }

            if (certificate != null)
                base.Open(certificate);
            else
                base.Open();
        }
        #endregion
    }

    /// <summary>
    /// Message stream service host
    /// </summary>
    public class StreamMessageServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Stream Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public StreamMessageServiceHost()
            : base(typeof(Nequeo.Service.Message.Stream))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public StreamMessageServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.Message.Stream), baseAddresses)
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.Message.IStream), binding, "");
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.Message.IStream), binding, address);
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
            catch { }

            if (certificate != null)
                base.Open(certificate);
            else
                base.Open();
        }
        #endregion
    }

    /// <summary>
    /// Sevice Model RESTful service host
    /// </summary>
    /// <typeparam name="TImplementation">The service implementation type.</typeparam>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    public class RestFullServiceModelHost<TImplementation, TContract> : Nequeo.Net.ServiceModel.WebServiceManager
    {
        #region Web Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public RestFullServiceModelHost()
            : base(typeof(TImplementation))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public RestFullServiceModelHost(Uri[] baseAddresses)
            : base(typeof(TImplementation), baseAddresses)
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
        public void OpenServiceHost(System.ServiceModel.WebHttpBinding binding)
        {
            if (base.CommunicationState == CommunicationState.Closed)
            {
                base.Initialise();
                base.ServiceHost.AddServiceEndpoint(typeof(TContract), binding, "");
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
                base.ServiceHost.AddServiceEndpoint(typeof(TContract), binding, address);
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
            catch { }

            if (certificate != null)
                base.Open(certificate);
            else
                base.Open();
        }
        #endregion
    }
}
