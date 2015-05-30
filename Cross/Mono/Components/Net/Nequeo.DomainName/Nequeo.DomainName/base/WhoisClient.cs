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
    /// Class that can be used for whois requests for domains.
    /// </summary>
    [Export(typeof(IWhoisClient))]
    public class WhoisClient : IDisposable, IWhoisClient
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WhoisClient()
        {
        }

        /// <summary>
        /// Whois server constructor.
        /// </summary>
        /// <param name="whoisServer">The whois server to query.</param>
        public WhoisClient(string whoisServer)
        {
            _whoisServer = whoisServer;
        }

        /// <summary>
        /// Connection adapter constructor.
        /// </summary>
        /// <param name="connection">The connection adapter used to connect to the server.</param>
        public WhoisClient(WhoisConnectionAdapter connection)
        {
            _connection = connection;
            _whoisServer = connection.Server;
            _whoisPort = connection.Port;
            _domain = connection.Domain;
        }
        #endregion

        #region Private Fields
        private Socket _socket = null;
        private WhoisConnectionAdapter _connection = null;
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;
        private Nequeo.Net.Dns.ProtocolType _protocolType = Nequeo.Net.Dns.ProtocolType.Tcp;
        private Nequeo.Threading.FunctionHandler<string, string> _whoisServerName = null;

        private bool _useIPv4EndPoint = false;
        private string _whoisServer = string.Empty;
        private string _domain = string.Empty;
        private int _whoisPort = 43;
        private bool _disposed = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        public WhoisConnectionAdapter Connection
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
        /// Gets sets, the ip address or host name of the whois server
        /// that contains the records.
        /// </summary>
        public string WhoisServer
        {
            get { return _whoisServer; }
            set { _whoisServer = value; }
        }

        /// <summary>
        /// Gets sets, the whois port to lookup.
        /// </summary>
        public int WhoisPort
        {
            get { return _whoisPort; }
            set { _whoisPort = value; }
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
        public event Nequeo.Threading.EventHandler<ClientCommandArgs> OnQueryComplete;
        #endregion

        #region Public Methods
        /// <summary>
        /// Send a request to a whois server for domain
        /// owner records for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <returns>The response from the whois server.</returns>
        public virtual string Query(string domain)
        {
            // Validate the inputs.
            if (String.IsNullOrEmpty(domain))
                throw new System.ArgumentNullException("Domain can not be null.",
                    new System.Exception("A valid domain should be specified."));

            if (String.IsNullOrEmpty(_whoisServer))
                throw new System.ArgumentNullException("Whois server can not be null.",
                    new System.Exception("A valid whois server should be specified."));

            string response = null;

            // As result says - certain domain authorities 
            // like to keep their whois service private.
            // There maybe extra tlds to add.
            if (domain.IndexOf(".tv") != -1 ||
                domain.IndexOf(".pro") != -1 ||
                domain.IndexOf(".name") != -1)
            {
                response = "'.pro','.name', and '.tv' domains require an account for a whois";
            }
            else
            {
                // Attempt a connection to the
                // whois server.
                _socket = GetSocket(_whoisServer, _whoisPort, _protocolType, _useIPv4EndPoint);

                if (_socket != null)
                    // If a connection has been made.
                    if (_socket.Connected)
                    {
                        // Send the domain string to the server.
                        SendCommand(domain);

                        // Get the response from the server.
                        response = GetServerResponse();

                        // Close the connection.
                        Close();
                    }
            }

            // If the event has been attached
            // send to the client the data through
            // the delegate.
            if (OnQueryComplete != null)
                OnQueryComplete(this, new ClientCommandArgs(
                    "Query", response.Replace("\n", "\r\n"), 200));

            // The response from the whois server.
            return response.Replace("\n", "\r\n");
        }

        /// <summary>
        /// Begin send a request to a whois server for domain
        /// owner records for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginQuery(string domain,
            AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncWhoisQueryRequest(domain, this, callback, state);
        }

        /// <summary>
        /// End send a request to a whois server for domain
        /// owner records for the specified domain.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        public virtual string EndQuery(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncWhoisQueryRequest.End(ar);
        }

        /// <summary>
        /// Get the whois server for the current domain.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <returns>The whois server.</returns>
        public virtual string GetWhoisServer(string domain)
        {
            return GetWhoisServerEx(domain);
        }

        /// <summary>
        /// Begin get the whois server for the current domain.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginGetWhoisServer(string domain,
            AsyncCallback callback, object state)
        {
            if (_whoisServerName == null)
                _whoisServerName = new Nequeo.Threading.FunctionHandler<string, string>(GetWhoisServer);

            return _whoisServerName.BeginInvoke(domain, callback, state);
        }

        /// <summary>
        /// End get the whois server for the current domain.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The whois server.</returns>
        public virtual string EndGetWhoisServer(IAsyncResult ar)
        {
            if (_whoisServerName == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not yet begun.");

            // Use the AsyncResult to complete that async operation.
            return _whoisServerName.EndInvoke(ar);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the whois server for the current domain.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <returns>The whois server.</returns>
        private string GetWhoisServerEx(string domain)
        {
            // Split the domian parts
            // the top level domain value
            // and create a new whois server
            // generic domain name.
            string[] parts = domain.Split('.');
            string tld = parts[parts.Length - 1];
            string host = tld + ".whois-servers.net";

            // .tk doesn't resolve, but it's 
            // whois server is public
            if (tld == "tk")
                return "whois.dot.tk";

            try
            {
                // Get the dns server for the
                // specified whois generic server.
                IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(host);

                // If the resolution returned the
                // original domain, that is, the
                // resolve did not succeed.
                if (ipHostEntry.HostName == host)
                {
                    // Assign the top level (tld) and the
                    // country code level (cctld) domains.
                    tld = parts[parts.Length - 2] + "." + parts[parts.Length - 1];
                    host = tld + ".whois-servers.net";

                    // Get the dns server for the
                    // specified whois generic server.
                    ipHostEntry = System.Net.Dns.GetHostEntry(host);

                    // If the resolution returned the
                    // original domain, that is, the
                    // resolve did not succeed.
                    if (ipHostEntry.HostName == host)
                        // Assign the default whois server.
                        host = "whois.internic.net";
                    else
                        // Assign the host name
                        // for the domain whois server.
                        host = ipHostEntry.HostName;
                }
                else
                    // Assign the host name
                    // for the domain whois server.
                    host = ipHostEntry.HostName;

                return host;
            }
            catch
            {
                // Return the default whois server.
                return "whois.internic.net";
            }
        }

        /// <summary>
        /// Send a command to the server.
        /// </summary>
        /// <param name="data">The command data to send to the server.</param>
        private void SendCommand(String data)
        {
            try
            {
                // Convert the string data to byte data 
                // using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

                // Send the command to the server.
                _socket.Send(byteData);
            }
            catch (Exception e)
            {
                Close();
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Get the current server response.
        /// </summary>
        /// <returns>The server response data.</returns>
        private string GetServerResponse()
        {
            try
            {
                // Create a new buffer byte array, stores the response.
                byte[] buffer = new byte[8192];
                int byteCount = 0;

                StringBuilder builder = new StringBuilder();

                // Get the data from the server placed it in the buffer.
                byteCount = _socket.Receive(buffer, buffer.Length, 0);

                // Decode the data in the buffer to a string.
                builder.Append(Encoding.ASCII.GetString(buffer, 0, byteCount));

                // While data is avaliable.
                while (byteCount > 0)
                {
                    // Get the data from the server placed it in the buffer.
                    byteCount = _socket.Receive(buffer, buffer.Length, 0);

                    // Decode the data in the buffer to a string.
                    builder.Append(Encoding.ASCII.GetString(buffer, 0, byteCount));
                }

                // Return the response from the server.
                return builder.ToString();
            }
            catch (Exception e)
            {
                Close();
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Close the current connection.
        /// </summary>
        private void Close()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }

        /// <summary>
        /// Get the client connection socket.
        /// </summary>
        /// <param name="ipAddress">the ip address of the domain name server.</param>
        /// <param name="port">the port number used by the domain name server.</param>
        /// <param name="protocolType">the protocol type used by the domain name server.</param>
        /// <param name="useIPv4EndPoint">use only an IPv4 connection and disregard all other address families (IPv6).</param>
        /// <returns>The client connection socket.</returns>
        private Socket GetSocket(string ipAddress, int port,
            Nequeo.Net.Dns.ProtocolType protocolType, bool useIPv4EndPoint)
        {
            Socket socket = null;

            try
            {
                IPHostEntry hostEntry = null;

                // Get host related information.
                hostEntry = System.Net.Dns.GetHostEntry(ipAddress);

                // Loop through the AddressList to obtain the supported 
                // AddressFamily. This is to avoid an exception that 
                // occurs when the host IP Address is not compatible 
                // with the address family 
                // (typical in the IPv6 case).
                foreach (IPAddress address in hostEntry.AddressList)
                {
                    // If the connection used should only be IPv4.
                    if (useIPv4EndPoint && address.AddressFamily == AddressFamily.InterNetworkV6)
                        continue;

                    // Get the current server endpoint for
                    // the current address.
                    IPEndPoint endPoint = new IPEndPoint(address, port);
                    Socket tempSocket = null;

                    // Get the current socket type.
                    switch (protocolType)
                    {
                        case Nequeo.Net.Dns.ProtocolType.Tcp:
                            // Create a new client socket for the
                            // current endpoint.
                            tempSocket = new Socket(endPoint.AddressFamily,
                                SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                            break;

                        case Nequeo.Net.Dns.ProtocolType.Udp:
                            // Create a new client socket for the
                            // current endpoint.
                            tempSocket = new Socket(endPoint.AddressFamily,
                                SocketType.Stream, System.Net.Sockets.ProtocolType.Udp);
                            break;
                    }

                    // Connect to the server with the
                    // current end point.
                    try
                    {
                        tempSocket.Connect(endPoint);
                    }
                    catch { }

                    // If this connection succeeded then
                    // asiign the client socket and
                    // break put of the loop.
                    if (tempSocket.Connected)
                    {
                        // A client connection has been found.
                        // Break out of the loop.
                        socket = tempSocket;
                        break;
                    }
                    else continue;
                }

                // Return the client socket.
                return socket;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
        ~WhoisClient()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Interface that can be used for whois requests for domains.
    /// </summary>
    public interface IWhoisClient
    {
        #region Public Properties
        /// <summary>
        /// Get set, the connection adapter.
        /// </summary>
        WhoisConnectionAdapter Connection { get; set; }

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
        /// Gets sets, the ip address or host name of the whois server
        /// that contains the records.
        /// </summary>
        string WhoisServer { get; set; }

        /// <summary>
        /// Gets sets, the whois port to lookup.
        /// </summary>
        int WhoisPort { get; set; }
        #endregion

        #region Public Events
        /// <summary>
        /// On query complate event handler.
        /// </summary>
        event Nequeo.Threading.EventHandler<ClientCommandArgs> OnQueryComplete;
        #endregion

        #region Public Methods
        /// <summary>
        /// Send a request to a whois server for domain
        /// owner records for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <returns>The response from the whois server.</returns>
        string Query(string domain);

        /// <summary>
        /// Begin send a request to a whois server for domain
        /// owner records for the specified domain.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginQuery(string domain,
            AsyncCallback callback, object state);

        /// <summary>
        /// End send a request to a whois server for domain
        /// owner records for the specified domain.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The response object containing the collection 
        /// of domain name server information.</returns>
        string EndQuery(IAsyncResult ar);

        /// <summary>
        /// Get the whois server for the current domain.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <returns>The whois server.</returns>
        string GetWhoisServer(string domain);

        /// <summary>
        /// Begin get the whois server for the current domain.
        /// </summary>
        /// <param name="domain">The domain name.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        IAsyncResult BeginGetWhoisServer(string domain,
            AsyncCallback callback, object state);

        /// <summary>
        /// End get the whois server for the current domain.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>The whois server.</returns>
        string EndGetWhoisServer(IAsyncResult ar);

        #endregion
    }
}
