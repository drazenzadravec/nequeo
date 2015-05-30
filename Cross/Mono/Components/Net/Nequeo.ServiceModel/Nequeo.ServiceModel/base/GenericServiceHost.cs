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
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Compilation;
using System.ServiceModel.Web;

using Nequeo.Handler.Global;
using Nequeo.Handler;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Sevice Model (Basic, NetTcp, WS) service host
    /// </summary>
    /// <typeparam name="TImplementation">The service implementation type.</typeparam>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    public class GenericServiceModelHost<TImplementation, TContract> : ServiceManager
    {
        #region Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public GenericServiceModelHost()
            : base(typeof(TImplementation))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public GenericServiceModelHost(Uri[] baseAddresses)
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
        public void OpenServiceHost(System.ServiceModel.Channels.Binding binding)
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
        public void OpenServiceHost(System.ServiceModel.Channels.Binding binding, string address)
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
    /// <typeparam name="TImplementation">The service implementation type.</typeparam>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    public class GenericRestServiceModelHost<TImplementation, TContract> : WebServiceManager
    {
        #region Web Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public GenericRestServiceModelHost()
            : base(typeof(TImplementation))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public GenericRestServiceModelHost(Uri[] baseAddresses)
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
