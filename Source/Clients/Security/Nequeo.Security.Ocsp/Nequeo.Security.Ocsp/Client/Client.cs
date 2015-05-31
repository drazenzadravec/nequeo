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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Model;
using Nequeo.Extension;
using Nequeo.Cryptography.Key;

namespace Nequeo.Security.Ocsp
{
    /// <summary>
    /// Online certificate status protocol client.
    /// </summary>
    public sealed partial class Client : Nequeo.Net.NetClient, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Online certificate status protocol client.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client()
        {
            Initialise();
        }

        /// <summary>
        /// Online certificate status protocol client.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client(IPAddress address, int port)
            : base(address, port)
        {
            Initialise();
        }

        /// <summary>
        /// Online certificate status protocol client.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Client(string hostNameOrAddress, int port)
            : base(hostNameOrAddress, port)
        {
            Initialise();
        }
        #endregion

        #region Private Fields
        private object _lockConnectObject = new object();
        private object _lockRequestObject = new object();

        private Dictionary<string, object> _callback = new Dictionary<string, object>();
        private Dictionary<string, object> _state = new Dictionary<string, object>();
        private Dictionary<string, string> _actionNameReference = new Dictionary<string, string>();
        #endregion

        #region Public Events
        /// <summary>
        /// The on error event handler, triggered when data received from the server is any type of error.
        /// </summary>
        public event Nequeo.Threading.EventHandler<string, string, string> OnError;

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the status for the certificate.
        /// </summary>
        /// <param name="certificates">The certificates to get status for.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        /// <param name="resourcePath">The full path to the remote resource.</param>
        /// <param name="actionName">The current action callback name.</param>
        /// <param name="state">The state callback handler object.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void CertificateStatus(X509Certificate2[] certificates, Action<List<string>, string, List<byte[]>, object> callback, 
            string resourcePath = "/ocsp", string actionName = "", object state = null)
        {
            // If not connected.
            if (!base.Connected)
                throw new Exception("A connection does not exist, create a connection first.");

            // If connected.
            if (base.Connected)
            {
                // Only allow one request at a time.
                lock (_lockRequestObject)
                {
                    // Set the default action name.
                    string actName = (String.IsNullOrEmpty(actionName) ? "OCSP_CS" : actionName);

                    // Assign the call back.
                    _callback[actName] = callback;
                    _state[actName] = state;
                    _actionNameReference["OCSP_CS"] = actName;

                    // Get the ocsp request.
                    byte[] ocspData = GetOcspRequest(certificates);

                    // Write the request to the server.
                    Nequeo.Net.NetRequest request = base.GetRequest();
                    request.Method = "POST";
                    request.ContentLength = (long)ocspData.Length;
                    request.KeepAlive = true;
                    request.Host = base.HostNameOrAddress + ":" + base.Port.ToString();
                    request.Path = resourcePath;
                    request.ContentType = "application/ocsp-request";
                    request.AddHeader("Member", "OCSP_CS");
                    request.AddHeader("ActionName", actName);
                    request.WriteNetRequestHeaders();
                    request.Write(ocspData);
                }
            }
        }
        #endregion

        #region Private Receive Methods
        /// <summary>
        /// On net context receive handler.
        /// </summary>
        /// <param name="sender">The application sender.</param>
        /// <param name="context">The current net context.</param>
        private void Client_OnNetContext(object sender, Net.NetContext context)
        {
            Net.NetRequest request = null;
            Net.NetResponse response = null;
            bool keepAlive = true;
            bool isError = true;

            try
            {
                string resource = "";
                string executionMember = "";
                string statusCode = "";
                string statusDescription = "";

                request = context.NetRequest;
                response = context.NetResponse;

                // Get the response headers, and set the response headers.
                List<NameValue> headers = base.ParseHeaders(response.Input, out resource, base.HeaderTimeout, base.MaximumReadLength);
                if (headers != null)
                {
                    // Set the response headers.
                    response.ReadNetResponseHeaders(headers, resource);

                    // Should the connection be kept alive.
                    keepAlive = response.KeepAlive;
                    statusCode = response.StatusCode.ToString();
                    statusDescription = response.StatusDescription;

                    // Get the execution member.
                    if (!String.IsNullOrEmpty(response.Headers["Member"]))
                    {
                        // Get the execution member.
                        executionMember = response.Headers["Member"].Trim();
                        switch (executionMember.ToUpper())
                        {
                            case "OCSP_CS":
                                // CertificateStatus
                                isError = CertificateStatus(response);
                                break;
                        }
                    }
                    else
                    {
                        // Find the response type sent from the server.
                        if (!String.IsNullOrEmpty(response.ContentType))
                        {
                            // If the response is ocsp.
                            if(response.ContentType.ToLower().Contains("ocsp-response"))
                                // CertificateStatus
                                isError = CertificateStatus(response);
                        }
                    }
                }
                else
                {
                    // No headers have been found.
                    keepAlive = false;
                    throw new Exception("No headers have been found.");
                }

                // If error.
                if (isError)
                    // An internal client error.
                    AnyError(executionMember, statusCode, statusDescription);
            }
            catch (Exception ex)
            {
                try
                {
                    // An internal client error.
                    AnyError("Error", "500", ex.Message);
                }
                catch { }
            }

            // If keep alive.
            if (keepAlive)
            {
                // Look for a new response if this one has been completed.
                if (response != null)
                {
                    try
                    {
                        // If the response stream is active.
                        if (response.Input != null)
                        {
                            // If another response is pending
                            // then start the process again.
                            if (response.Input.Length > 0)
                                Client_OnNetContext(this, context);
                        }
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region Private Action Members
        /// <summary>
        /// CertificateStatus
        /// </summary>
        /// <param name="response">The current response stream.</param>
        /// <returns>True if error; else false.</returns>
        private bool CertificateStatus(Net.NetResponse response)
        {
            bool isError = true;
            byte[] result = null;
            string actionName = "";
            List<string> certificatesStatus = null;
            string ocspRespStatus = null;
            List<byte[]> serialNumbers = null;

            MemoryStream sendBuffer = null;
            try
            {
                // If an action name was return then use it
                // else use a defualt action name.
                if (!String.IsNullOrEmpty(response.Headers["ActionName"]))
                    actionName = response.Headers["ActionName"];
                else
                    actionName = _actionNameReference["OCSP_CS"];

                // Read the response stream and write to the send buffer.
                sendBuffer = new MemoryStream();
                bool copied = Nequeo.IO.Stream.Operation.CopyStream(response.Input, sendBuffer, response.ContentLength, base.ResponseTimeout);
                
                // If all the data has been copied.
                if (copied)
                {
                    // Get the data read.
                    result = sendBuffer.ToArray();
                    ocspRespStatus = GetOcspResponse(result, ref certificatesStatus, ref serialNumbers);
                    isError = false;
                }
            }
            catch { isError = true; }
            finally
            {
                // Dispose of the buffer.
                if (sendBuffer != null)
                    sendBuffer.Dispose();
            }

            // Call the handler.
            object callback = null;
            object state = null;
            if (_callback.TryGetValue(actionName, out callback))
            {
                _state.TryGetValue(actionName, out state);
                if (callback != null)
                {
                    Action<List<string>, string, List<byte[]>, object> callbackAction = (Action<List<string>, string, List<byte[]>, object>)callback;
                    callbackAction(certificatesStatus, ocspRespStatus, serialNumbers, state);
                }
            }

            // Return the result.
            return isError;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Send to the client any type of error.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="code">The code.</param>
        /// <param name="message">The message</param>
        private void AnyError(string command, string code, string message)
        {
            // Send any type of error.
            if (OnError != null)
                OnError(this, command, code, message);
        }

        /// <summary>
        /// Initialise.
        /// </summary>
        private void Initialise()
        {
            // Assign the on connect action handler.
            base.Timeout = 60;
            base.HeaderTimeout = 30000;
            base.RequestTimeout = 30000;
            base.ResponseTimeout = 30000;
            base.RemoteHostPrefix = "OcspClient_";
            base.OnNetContext += Client_OnNetContext;
        }

        /// <summary>
        /// Get the ocsp request.
        /// </summary>
        /// <param name="certificates">The certificates to get status for.</param>
        /// <returns>The ocsp request encoder data.</returns>
        private byte[] GetOcspRequest(X509Certificate2[] certificates)
        {
            byte[] ocspData = null;

            // Create the ocsp request.
            Cryptography.Key.Ocsp.OcspReqGenerator gen = new Cryptography.Key.Ocsp.OcspReqGenerator();

            // For each certificate.
            foreach (X509Certificate2 certificate in certificates)
            {
                // Convert X509Certificate2 to X509.X509Certificate 
                Cryptography.Key.X509.X509CertificateParser certParser = new Cryptography.Key.X509.X509CertificateParser();
                Cryptography.Key.X509.X509Certificate certBouncy = certParser.ReadCertificate(certificate.RawData);

                // Create the certificate ID.
                Cryptography.Key.Ocsp.CertificateID certID =
                    new Cryptography.Key.Ocsp.CertificateID(Cryptography.Key.Ocsp.CertificateID.HashSha1, certBouncy, certBouncy.SerialNumber);

                // Add the certificate ID.
                gen.AddRequest(certID);
                gen.SetRequestExtensions(GetExtentions());
            }

            // Generate the request.
            Cryptography.Key.Ocsp.OcspReq req = gen.Generate();
            ocspData = req.GetEncoded();

            // Return the request.
            return ocspData;
        }

        /// <summary>
        /// Get the extensions.
        /// </summary>
        /// <returns>Get the etensions.</returns>
        private Cryptography.Key.Asn1.X509.X509Extensions GetExtentions()
        {
            // Create the dictionary.
            byte[] nonce = new byte[16];
            IDictionary exts = new Dictionary<Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier, Cryptography.Key.Asn1.X509.X509Extension>();
            
            // Create the extension.
            Cryptography.Key.Asn1.X509.X509Extension nonceext = 
                new Cryptography.Key.Asn1.X509.X509Extension(false, new Cryptography.Key.Asn1.DerOctetString(nonce));

            // Add the extension to the hash table.
            exts.Add(Cryptography.Key.Asn1.Ocsp.OcspObjectIdentifiers.PkixOcspNonce, nonceext);

            // Return the extension from the hash table.
            return new Cryptography.Key.Asn1.X509.X509Extensions(exts);
        }

        /// <summary>
        /// Get the ocsp response.
        /// </summary>
        /// <param name="response">The response data.</param>
        /// <param name="certificatesStatus">The certificates status.</param>
        /// <param name="serialNumbers">The certificates serial numbers.</param>
        /// <returns>The ocsp response status.</returns>
        private string GetOcspResponse(byte[] response, ref List<string> certificatesStatus, ref List<byte[]> serialNumbers)
        {
            string ocspRespStatus = "";
            
            // Load the response into the ocsp handler.
            Cryptography.Key.Ocsp.OcspResp resp = new Cryptography.Key.Ocsp.OcspResp(response);
            ocspRespStatus = GetOcspResponseStatus(resp.Status);

            // If the status is 'Succesfull'
            if (resp.Status == 0)
            {
                // Get the certificate status.
                certificatesStatus = GetCertificateStatus(resp, ref serialNumbers);
            }

            // Return the status
            return ocspRespStatus;
        }

        /// <summary>
        /// Get the ocsp response status.
        /// </summary>
        /// <param name="status">The response status.</param>
        /// <returns>The response status as string.</returns>
        private string GetOcspResponseStatus(int status)
        {
            string ocspResponseStatus = "";
            switch (status)
            {
                case 0: ocspResponseStatus = "Succesfull";
                    break;
                case 1: ocspResponseStatus = "MalformedRequest";
                    break;
                case 2: ocspResponseStatus = "InternalError";
                    break;
                case 3: ocspResponseStatus = "TryLater";
                    break;
                case 5: ocspResponseStatus = "SigRequired";
                    break;
                case 6: ocspResponseStatus = "Unauthorized";
                    break;
            }
            return ocspResponseStatus;
        }

        /// <summary>
        /// Get the certificate status.
        /// </summary>
        /// <param name="ocspResponse">The ocsp response handler.</param>
        /// <param name="serialNumbers">The certificates serial number.</param>
        /// <returns>The certificates status.</returns>
        private List<string> GetCertificateStatus(Cryptography.Key.Ocsp.OcspResp ocspResponse, ref List<byte[]> serialNumbers)
        {
            List<string> certificatesStatus = new List<string>();
            serialNumbers = new List<byte[]>();

            // Get the ocsp response.
            Cryptography.Key.Ocsp.BasicOcspResp brep = (Cryptography.Key.Ocsp.BasicOcspResp)ocspResponse.GetResponseObject();
            Cryptography.Key.Ocsp.SingleResp[] singleResps = brep.Responses;

            // For each response.
            foreach (Cryptography.Key.Ocsp.SingleResp resp in singleResps)
            {
                Cryptography.Key.Ocsp.SingleResp singleResp = resp;
                object status = singleResp.GetCertStatus();
                string certificateStatus = "";

                // If null the good.
                if (status == null)
                    certificateStatus = "GOOD";

                // If revoked
                if (status is Cryptography.Key.Ocsp.RevokedStatus)
                    certificateStatus = "REVOKED";

                // If unknown.
                if (status is Cryptography.Key.Ocsp.UnknownStatus)
                    certificateStatus = "UNKNOWN";

                // Get the certificate ID.
                Cryptography.Key.Ocsp.CertificateID certID = singleResp.GetCertID();
                serialNumbers.Add(certID.SerialNumber.ToByteArrayUnsigned());
                certificatesStatus.Add(certificateStatus);
            }

            // Return the certificate status.
            return certificatesStatus;
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
        /// equals true, the method has been called directly or indirectly by a user's
        /// code. Managed and unmanaged resources can be disposed.  If disposing equals
        /// false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can
        /// be disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_callback != null)
                        _callback.Clear();

                    if (_state != null)
                        _state.Clear();

                    if (_actionNameReference != null)
                        _actionNameReference.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _actionNameReference = null;
                _callback = null;
                _state = null;

                _lockConnectObject = null;
                _lockRequestObject = null;
            }
        }
        #endregion
    }
}
