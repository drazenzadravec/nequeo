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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Composition;

namespace Nequeo.Net.Dns
{
    /// <summary>
    /// Class contains methods that return domain name
    /// information from domain name servers, this includes
    /// the ability to query domian name servers for
    /// specified records for a domain.
    /// </summary>
    [Export(typeof(IDomainNameClient))]
    public class DomainNameClient : IDisposable, IDomainNameClient
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DomainNameClient()
        {
        }

        /// <summary>
        /// Domain name server constructor.
        /// </summary>
        /// <param name="dnsServer">The domain name server to query.</param>
        public DomainNameClient(string dnsServer)
        {
            _dnsServer = dnsServer;
        }

        /// <summary>
        /// Connection adapter constructor.
        /// </summary>
        /// <param name="connection">The connection adapter used to connect to the server.</param>
        public DomainNameClient(DnsConnectionAdapter connection)
        {
            _connection = connection;
            _dnsServer = connection.Server;
            _dnsPort = connection.Port;
            _domain = connection.Domain;
        }
        #endregion

        #region Private Fields
        private DnsConnectionAdapter _connection = null;
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;
        private Nequeo.Net.Dns.ProtocolType _protocolType = Nequeo.Net.Dns.ProtocolType.Tcp;

        private bool _useIPv4EndPoint = false;
        private string _dnsServer = string.Empty;
        private string _domain = string.Empty;
        private int _dnsPort = 53;
        private bool _disposed = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        public DnsConnectionAdapter Connection
        {
            set { _connection = value; }
            get { return _connection; }
        }

        /// <summary>
        /// Get set, the protocol type the dns server is using.
        /// </summary>
        public Nequeo.Net.Dns.ProtocolType ProtocolType
        {
            set { _protocolType = value; }
            get { return _protocolType; }
        }

        /// <summary>
        /// Get, the secure certificate.
        /// </summary>
        public Nequeo.Security.X509Certificate2Info Certificate
        {
            get { return _sslCertificate; }
        }

        /// <summary>
        /// Gets, the domain to lookup.
        /// </summary>
        public string Domain
        {
            get { return _domain; }
        }

        /// <summary>
        /// Gets sets, the ip address or host name of the domain name server
        /// that contains the domain records.
        /// </summary>
        public string DnsServer
        {
            get { return _dnsServer; }
            set { _dnsServer = value; }
        }

        /// <summary>
        /// Gets sets, the dns port to lookup.
        /// </summary>
        public int DnsPort
        {
            get { return _dnsPort; }
            set { _dnsPort = value; }
        }

        /// <summary>
        /// Gets sets, use only an IPv4 connection and disregard all other address families (IPv6).
        /// </summary>
        public bool UseIPv4EndPoint
        {
            get { return _useIPv4EndPoint; }
            set { _useIPv4EndPoint = value; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// On query complate event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Response> OnQueryComplete;
        #endregion

        #region Public Methods
        /// <summary>
        /// Send a query to a domain name server to perform a 
        /// lookup on the specified domian and return the records.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="dnsType">The dns record type to return.</param>
        /// <param name="dnsClass">The class to search in.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        public virtual Response Query(string domain, Nequeo.Net.Dns.DnsType dnsType,
            Nequeo.Net.Dns.DnsClass dnsClass)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(domain))
                throw new System.ArgumentNullException("Domain can not be null.",
                    new System.Exception("A valid domain should be specified."));

            if (String.IsNullOrEmpty(_dnsServer))
                throw new System.ArgumentNullException("Domain name server can not be null.",
                    new System.Exception("A valid domain name server should be specified."));

            _domain = domain;

            // create a DNS request
            Request request = new Request();
            Resolver resolver = new Resolver();

            // create a question for this domain and DNS class.
            request.AddQuestion(new Question(domain, dnsType, dnsClass));

            // send it to the DNS server and get the response
            Response response = resolver.Lookup(request, _domain, _dnsPort, _dnsServer, _protocolType, _useIPv4EndPoint);

            // If the event has been attached
            // send to the client the data through
            // the delegate.
            if (OnQueryComplete != null)
                OnQueryComplete(this, response);

            // Return the response from the server.
            return response;
        }

        /// <summary>
        /// Begin send a query to a domain name server to perform a 
        /// lookup on the specified domian and return the records.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="dnsType">The dns record type to return.</param>
        /// <param name="dnsClass">The class to search in.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginQuery(string domain, Nequeo.Net.Dns.DnsType dnsType,
            Nequeo.Net.Dns.DnsClass dnsClass, AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDnsQueryRequest(domain, dnsType,
                dnsClass, this, callback, state);
        }

        /// <summary>
        /// End send a query to a domain name server to perform a 
        /// lookup on the specified domian and return the records.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        public virtual Response EndQuery(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncDnsQueryRequest.End(ar);
        }

        /// <summary>
        /// Gets all the IP address for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <returns>Collection of IP addresses for the domain else null.</returns>
        public static IPAddress[] GetHostIPAddresses(string domain)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(domain))
                throw new System.ArgumentNullException("Domain can not be null.",
                    new System.Exception("A valid domain should be specified."));

            // Get host related information.
            IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(domain);

            // Return all IP address used for the
            // specified domain.
            return hostEntry.AddressList;
        }

        /// <summary>
        /// Perform a reverse dns lookup on the specified ip address.
        /// </summary>
        /// <param name="ipAddress">The ip address to resolve.</param>
        /// <param name="timeout">The time to wait for a response (milliseconds).</param>
        /// <returns>IP host entry for the ip address else null.</returns>
        /// <remarks>This method blocks the current thread for the specified time.</remarks>
        public static IPHostEntry ReverseDnsLookup(string ipAddress, int timeout)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(ipAddress))
                throw new System.ArgumentNullException("IP Address can not be null.",
                    new System.Exception("A valid ip address should be specified."));

            try
            {
                // Parse the ip address to a valid
                // ip address, get the host for
                // the ip address do lookup.
                IPAddress ip = IPAddress.Parse(ipAddress);

                // Create a new function call back delegate
                // for the dns get host entry method.
                Nequeo.Threading.FunctionHandler<IPHostEntry, IPAddress> callback =
                    new Nequeo.Threading.FunctionHandler<IPHostEntry, IPAddress>(System.Net.Dns.GetHostEntry);

                // Start the async operation.
                IAsyncResult ar = callback.BeginInvoke(ip, null, null);

                // If the operation has not timed out
                // then return the host entry.
                if (ar.AsyncWaitHandle.WaitOne(timeout, false))
                    // Return the host entry object.
                    return callback.EndInvoke(ar);
                else
                    // Return the null.
                    return null;
            }
            catch
            {
                // Return a null.
                return null;
            }
        }

        /// <summary>
        /// Gets the collection of domain name servers for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <returns>The collection of all name servers else null.</returns>
        public virtual DomainNameServer[] GetDomainNameServers(string domain)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(domain))
                throw new System.ArgumentNullException("Domain can not be null.",
                    new System.Exception("A valid domain should be specified."));

            DomainNameServer[] domainNameCol = null;

            // For each root server attempt to find the domain name servers
            // for the specified domian, once found then break out of
            // the loop no need to use all other root servers.
            foreach (string rootServer in RootNameServers.RootServers)
            {
                // Assign the current level dns server
                // to request NS records from.
                _dnsServer = rootServer.ToString();

                // Start a new level domian name server
                // query, this will get the next level
                // of NS records fro the domain.
                domainNameCol = GetNameServersEx(domain);

                // If a collection of name servers
                // has been found and collected
                // then break out of the loop.
                if (domainNameCol != null)
                    break;
            }

            // If the collection contains
            // data then assign the first
            // dns to the current dns server.
            if ((domainNameCol != null) && (domainNameCol.Length > 0))
                _dnsServer = domainNameCol[0].DNS;

            // Return the collection of
            // domain name servers.
            return domainNameCol;
        }

        /// <summary>
        /// Begin get the collection of domain name servers for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetDomainNameServers(string domain,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDomainNameServerRequest(
                domain, this, callback, state);
        }

        /// <summary>
        /// End get the collection of domain name servers for the specified domain.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        public virtual DomainNameServer[] EndGetDomainNameServers(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncDomainNameServerRequest.End(ar);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get all domain name servers for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <returns>The collection of domain name servers for the domain.</returns>
        private DomainNameServer[] GetNameServersEx(string domain)
        {
            DomainNameServer[] domainNameCol = null;

            // Query the dns server for NS records on the domain.
            Response response = Query(domain, Nequeo.Net.Dns.DnsType.NS, Nequeo.Net.Dns.DnsClass.IN);

            // If there is no additional record information
            // then the query request is a success.
            if ((response.AdditionalRecords != null) && (response.AdditionalRecords.Length > 0))
            {
                // If a successful answer has been
                // returned for the NS operation.
                if ((response.Answers != null) && (response.Answers.Length > 0))
                {
                    // Create a new instance of the
                    // domian name collection.
                    domainNameCol = new DomainNameServer[response.Answers.Length];

                    // For each answer (NS) returned
                    // create a new domain name server
                    // object and add the DNS data to
                    // the collection.
                    for (int i = 0; i < response.Answers.Length; i++)
                    {
                        // Add the DNS data to the
                        // domian name object.
                        DomainNameServer domainName = new DomainNameServer()
                        {
                            DNS = response.Answers[i].Record.ToString()
                        };

                        // Add the domain name server
                        // information to the collection.
                        domainNameCol[i] = domainName;
                    }
                }
                else
                {
                    // For each NS record found, request
                    // all next level NS records until
                    // the name servers have been found for
                    // the specified domain.
                    foreach (AdditionalRecord addRecord in response.AdditionalRecords)
                    {
                        // Assign the current level dns server
                        // to request NS records from.
                        _dnsServer = addRecord.Domain;

                        // Start a new level domian name server
                        // query, this will get the next level
                        // of NS records fro the domain.
                        domainNameCol = GetNameServersEx(domain);

                        // If a collection of name servers
                        // has been found and collected
                        // then break out of the loop.
                        if (domainNameCol != null)
                            break;
                    }
                }
            }
            // If there is no name server record information
            // then the query request is a success.
            else if ((response.NameServers != null) && (response.NameServers.Length > 0))
            {
                // Create a new instance of the
                // domian name collection.
                domainNameCol = new DomainNameServer[response.NameServers.Length];

                // For each answer (NS) returned
                // create a new domain name server
                // object and add the DNS data to
                // the collection.
                for (int i = 0; i < response.NameServers.Length; i++)
                {
                    // Add the DNS data to the
                    // domian name object.
                    DomainNameServer domainName = new DomainNameServer()
                    {
                        DNS = response.NameServers[i].Record.ToString()
                    };

                    // Add the domain name server
                    // information to the collection.
                    domainNameCol[i] = domainName;
                }

            }

            // Return the collection of
            // domain name servers for
            // the specified domain.
            return domainNameCol;
        }

        /// <summary>
        /// Validate all objects used by the client before connection.
        /// </summary>
        private void ClientValidation()
        {
            // Make sure that the Connection type has been created.
            if (_connection == null)
                throw new System.ArgumentNullException("Connection can not be null.",
                    new System.Exception("The Connection reference has not been set."));

            // Make sure that a valid host has been specified.
            if (String.IsNullOrEmpty(_connection.Server))
                throw new System.ArgumentNullException("A valid host name has not been specified.",
                    new System.Exception("No valid remote host was specified to send data to."));

            // Make sure the the port number is greater than one.
            if (_connection.Port < 1)
                throw new System.ArgumentOutOfRangeException("The port number must be greater than zero.",
                    new System.Exception("No valid port number was set, set a valid port number."));
        }

        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Certificate should be validated.
            if (_connection.ValidateCertificate)
            {
                // If the certificate is valid
                // return true.
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;
                else
                {
                    // Create a new certificate collection
                    // instance and return false.
                    _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                        certificate as X509Certificate2, chain, sslPolicyErrors);
                    return false;
                }
            }
            else
                // Return true.
                return true;
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.

                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~DomainNameClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Class contains methods that return domain name
    /// information from domain name servers, this includes
    /// the ability to query domian name servers for
    /// specified records for a domain.
    /// </summary>
    public interface IDomainNameClient
    {
        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        DnsConnectionAdapter Connection { get; set; }

        /// <summary>
        /// Get set, the protocol type the dns server is using.
        /// </summary>
        Nequeo.Net.Dns.ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// Get, the secure certificate.
        /// </summary>
        Nequeo.Security.X509Certificate2Info Certificate { get; }

        /// <summary>
        /// Gets, the domain to lookup.
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// Gets sets, the ip address or host name of the domain name server
        /// that contains the domain records.
        /// </summary>
        string DnsServer { get; set; }

        /// <summary>
        /// Gets sets, the dns port to lookup.
        /// </summary>
        int DnsPort { get; set; }
        #endregion

        #region Public Events
        /// <summary>
        /// On query complate event handler.
        /// </summary>
        event Nequeo.Threading.EventHandler<Response> OnQueryComplete;
        #endregion

        #region Public Methods
        /// <summary>
        /// Send a query to a domain name server to perform a 
        /// lookup on the specified domian and return the records.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="dnsType">The dns record type to return.</param>
        /// <param name="dnsClass">The class to search in.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        Response Query(string domain, Nequeo.Net.Dns.DnsType dnsType,
            Nequeo.Net.Dns.DnsClass dnsClass);

        /// <summary>
        /// Begin send a query to a domain name server to perform a 
        /// lookup on the specified domian and return the records.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="dnsType">The dns record type to return.</param>
        /// <param name="dnsClass">The class to search in.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginQuery(string domain, Nequeo.Net.Dns.DnsType dnsType,
            Nequeo.Net.Dns.DnsClass dnsClass, AsyncCallback callback, object state);

        /// <summary>
        /// End send a query to a domain name server to perform a 
        /// lookup on the specified domian and return the records.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        Response EndQuery(IAsyncResult ar);

        /// <summary>
        /// Gets the collection of domain name servers for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <returns>The collection of all name servers else null.</returns>
        DomainNameServer[] GetDomainNameServers(string domain);

        /// <summary>
        /// Begin get the collection of domain name servers for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetDomainNameServers(string domain,
            AsyncCallback callback, object state);

        /// <summary>
        /// End get the collection of domain name servers for the specified domain.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        DomainNameServer[] EndGetDomainNameServers(IAsyncResult ar);

        #endregion
    }
}


