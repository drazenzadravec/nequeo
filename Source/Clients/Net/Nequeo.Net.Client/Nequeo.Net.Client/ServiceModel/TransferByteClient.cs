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
using System.Net;
using System.ServiceModel.Channels;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

#if !WINDOWS_PHONE
using System.Net.Security;
using System.ServiceModel.Security;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
#endif

using Nequeo.IO.Stream.Extension;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Simple streamed data transfer client.
    /// </summary>
    public class TransferByteClient : Nequeo.Net.ServiceModel.ClientManager<Nequeo.Service.Transfer.IByteStream>
    {

#if !WINDOWS_PHONE
        /// <summary>
        /// Basic Http Binding constructor
        /// </summary>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.TransferClientByteBaseAddress),
                new System.ServiceModel.BasicHttpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Basic Http Binding constructor
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(string endPointAddress,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.BasicHttpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Message security NetTcp binding constructor.
        /// </summary>
        /// <param name="messageCredentialType">The secure message credential type</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(System.ServiceModel.MessageCredentialType messageCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.TransferClientByteBaseAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize
                    },
                    Security = new System.ServiceModel.NetTcpSecurity()
                    {
                        Mode = System.ServiceModel.SecurityMode.Message,
                        Message = new System.ServiceModel.MessageSecurityOverTcp()
                        {
                            ClientCredentialType = messageCredentialType
                        }
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Message security NetTcp binding constructor.
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="messageCredentialType">The secure message credential type</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(string endPointAddress, System.ServiceModel.MessageCredentialType messageCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize
                    },
                    Security = new System.ServiceModel.NetTcpSecurity()
                    {
                        Mode = System.ServiceModel.SecurityMode.Message,
                        Message = new System.ServiceModel.MessageSecurityOverTcp()
                        {
                            ClientCredentialType = messageCredentialType
                        }
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Transport security NetTcp binding constructor.
        /// </summary>
        /// <param name="tcpClientCredentialType">The secure tcp client credential type</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(System.ServiceModel.TcpClientCredentialType tcpClientCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.TransferClientByteBaseAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize
                    },
                    Security = new System.ServiceModel.NetTcpSecurity()
                    {
                        Mode = System.ServiceModel.SecurityMode.Transport,
                        Transport = new System.ServiceModel.TcpTransportSecurity()
                        {
                            ClientCredentialType = tcpClientCredentialType
                        }
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Transport security NetTcp binding constructor.
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="tcpClientCredentialType">The secure tcp client credential type</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(string endPointAddress, System.ServiceModel.TcpClientCredentialType tcpClientCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.TransferClientByteMaxReceivedMessageSize
                    },
                    Security = new System.ServiceModel.NetTcpSecurity()
                    {
                        Mode = System.ServiceModel.SecurityMode.Transport,
                        Transport = new System.ServiceModel.TcpTransportSecurity()
                        {
                            ClientCredentialType = tcpClientCredentialType
                        }
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
           // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="binding">The endpoint binding.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(System.ServiceModel.Channels.Binding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.TransferClientByteBaseAddress),
                binding, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="binding">The endpoint binding.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public TransferByteClient(string endPointAddress, System.ServiceModel.Channels.Binding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                binding, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }
#else
        /// <summary>
        /// Basic Http Binding constructor
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        public TransferByteClient(string endPointAddress,
            string username = null, string password = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.BasicHttpBinding()
                {
                    MaxReceivedMessageSize = 167108864,
                    MaxBufferSize = 167108864
                }, username, password
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Basic Http Binding constructor
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="binding">The endpoint binding.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        public TransferByteClient(string endPointAddress, System.ServiceModel.Channels.Binding binding,
            string username = null, string password = null) :
            base(
                new Uri(endPointAddress),
                binding, username, password
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }

        /// <summary>
        /// Transport security Basic Http binding constructor.
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        public TransferByteClient(Uri endPointAddress,
            string username = null, string password = null) :
            base(
                endPointAddress,
                new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.Transport)
                {
                    MaxReceivedMessageSize = 167108864,
                    MaxBufferSize = 167108864
                }, username, password
            )
        {
            // Attach to the async execute complete
            // event handler.
            base.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(TransferClient_AsyncExecuteComplete);
        }
#endif

        private System.IO.Stream _localDestinationDownload = null;
        private System.IO.StreamReader _localReaderUpload = null;
        private FileStream _localSource = null;
        private string _localDestinationPathDownload = string.Empty;

        /// <summary>
        /// Upload asynchronous complete event handler
        /// </summary>
        public event EventHandler<Nequeo.Custom.AsyncGenericResultArgs<bool>> AsyncUploadComplete;

        /// <summary>
        /// Download asynchronous complete event handler
        /// </summary>
        public event EventHandler<Nequeo.Custom.AsyncGenericResultArgs<bool>> AsyncDownloadComplete;

        /// <summary>
        /// Asynchronous error event handler
        /// </summary>
        public event EventHandler<Nequeo.Custom.AsyncArgs> AsyncError;

        /// <summary>
        /// Async execute complete method
        /// </summary>
        /// <param name="sender">The sender info</param>
        /// <param name="e1">The name of the async operation.</param>
        /// <param name="e2">Has an async error occured.</param>
        /// <param name="e3">The async exception if any.</param>
        private void TransferClient_AsyncExecuteComplete(object sender, object e1, bool e2, System.Exception e3)
        {
            try
            {
                // If the e1 is of type string
                if (e1 is string)
                {
                    // Get the e1
                    switch (e1.ToString().ToLower())
                    {
                        case "uploadfile":
                            // Upload the data asynchronously
                            bool uploadResult = base.GetExecuteAsyncResult<bool>(e1.ToString());

                            if (!uploadResult)
                                // A remote stream instance could not be established
                                throw new Exception("A remote stream instance could not be established.");
                            else
                                OnAsyncUploadComplete(uploadResult);
                            break;

                        case "downloadfile":
                            // Download the data asynchronously
                            byte[] remoteSourceData = base.GetExecuteAsyncResult<byte[]>(e1.ToString());

                            // If a valid stream instance has been established.
                            if (remoteSourceData != null)
                            {
                                _localDestinationDownload.Write(remoteSourceData, 0, remoteSourceData.Length);
                                OnAsyncDownloadComplete(true);
                            }
                            else
                                // A remote stream instance could not be established
                                throw new Exception("A remote stream instance could not be established.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Clean-up
                if (_localReaderUpload != null)
                    _localReaderUpload.Close();

                // Clean-up
                if (_localSource != null)
                    _localSource.Close();

                // Clean-up
                if (_localDestinationDownload != null)
                    _localDestinationDownload.Close();

                try
                {
                    // Clean-up delete the download local file.
                    File.Delete(_localDestinationPathDownload);
                }
                catch { }

                // Send the error message to the client
                // from the async call.
                if (AsyncError != null)
                    AsyncError(this,
                        new Nequeo.Custom.AsyncArgs(
                            new Nequeo.Exceptions.AsyncException(ex.Message, base.GetExecuteAsyncException(e1.ToString()))));
            }
        }

        /// <summary>
        /// On async upload complete
        /// </summary>
        /// <param name="result">The result of the async operation</param>
        private void OnAsyncUploadComplete(bool result)
        {
            // Clean-up
            if (_localReaderUpload != null)
                _localReaderUpload.Close();

            // Clean-up
            if (_localSource != null)
                _localSource.Close();

            // Send the event to the connected client
            if (AsyncUploadComplete != null)
                AsyncUploadComplete(this, new Custom.AsyncGenericResultArgs<bool>(result));
        }

        /// <summary>
        /// On async download complete
        /// </summary>
        /// <param name="result">The result of the async operation</param>
        private void OnAsyncDownloadComplete(bool result)
        {
            // Clean-up
            if (_localDestinationDownload != null)
                _localDestinationDownload.Close();

            // Send the event to the connected client
            if (AsyncDownloadComplete != null)
                AsyncDownloadComplete(this, new Custom.AsyncGenericResultArgs<bool>(result));
        }

#if !WINDOWS_PHONE
        /// <summary>
        /// Create the callback to validate a server certificate. Manages the collection of System.Net.ServicePoint objects.
        /// </summary>
        /// <param name="validationCallback">The callback to validate a server certificate.</param>
        public virtual void RemoteCertificateValidation(Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validationCallback)
        {
            CreateRemoteCertificateValidation(validationCallback);
        }

        /// <summary>
        /// Certificate override validator. Always true.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        public virtual bool OnCertificateValidationOverride(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Create the callback to validate a server certificate. Manages the collection of System.Net.ServicePoint objects.
        /// </summary>
        /// <param name="validationCallback">The callback to validate a server certificate.</param>
        protected override void CreateRemoteCertificateValidation(Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validationCallback)
        {
            base.CreateRemoteCertificateValidation(validationCallback);
        }
#endif

        /// <summary>
        /// Upload a file to the remote server.
        /// </summary>
        /// <param name="localSourcePath">The local source file and path to read data from.</param>
        /// <param name="asyncOperation">Should the download be execute asynchronously.</param>
        /// <returns>True if the operation was successful; else false.</returns>
        public bool UploadFile(string localSourcePath, bool asyncOperation = false)
        {
            try
            {
                // Open the local source file where
                // the data will be read from.
                _localSource = new FileStream(localSourcePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                _localReaderUpload = new StreamReader(_localSource);

                // Return the stream instance.
                byte[] data = Encoding.UTF8.GetBytes(_localReaderUpload.ReadToEnd());

                // Get the source file name
                string fileName = Path.GetFileName(localSourcePath);

                // Get the file info from the specified file
                FileInfo fileInfo = new FileInfo(localSourcePath);

                // If no async operation is requested.
                if (!asyncOperation)
                {
                    // Upload the data to the server.
                    base.Channel.UploadFile(data, fileName, fileInfo.Length);
                    _localSource.Close();
                }
                else
                    // Start a new async upload
                    base.Execute<bool>(u => u.UploadFile(data, fileName, fileInfo.Length), "UploadFile");

                // return true.
                return true;
            }
            catch (Exception ex)
            {
                // Clean-up
                if (_localReaderUpload != null)
                    _localReaderUpload.Close();

                // Clean-up
                if (_localSource != null)
                    _localSource.Close();

                if (base.Exception != null)
                    throw new Exception(ex.Message, new Exception(base.Exception.Message));
                else
                    throw ex;
            }
            finally
            {
                // Clean-up
                if (!asyncOperation)
                    if (_localReaderUpload != null)
                        _localReaderUpload.Close();

                // Clean-up
                if (!asyncOperation)
                    if (_localSource != null)
                        _localSource.Close();
            }
        }

        /// <summary>
        /// Download a file from a remote server.
        /// </summary>
        /// <param name="localDestinationPath">The local destination file and path to write data to.</param>
        /// <param name="remoteSourceFilename">The remote source file name to read data from.</param>
        /// <param name="asyncOperation">Should the download be execute asynchronously.</param>
        /// <returns>True if the operation was successful; else false.</returns>
        public bool DownloadFile(string localDestinationPath, string remoteSourceFilename, bool asyncOperation = false)
        {
            try
            {
                _localDestinationPathDownload = localDestinationPath;

                // Create the local destination file where
                // the data will be written to.
                _localDestinationDownload = new FileStream(localDestinationPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                // If no async operation is requested.
                if (!asyncOperation)
                {
                    long fileSize = base.Channel.GetFileSize(remoteSourceFilename);
                    byte[] remoteSourceData = base.Channel.DownloadFile(remoteSourceFilename);

                    // If a valid stream instance has been established.
                    if (remoteSourceData != null)
                    {
                        // Transfer the data from the server to the loacl source.
                        _localDestinationDownload.Write(remoteSourceData, 0, Convert.ToInt32(fileSize));

                        // Clean-up
                        if (_localDestinationDownload != null)
                            _localDestinationDownload.Close();
                    }
                    else
                        // A remote stream instance could not be established
                        throw new Exception("A remote stream instance could not be established.");
                }
                else
                    // Start a new async download
                    base.Execute<byte[]>(u => u.DownloadFile(remoteSourceFilename), "DownloadFile");

                // return true.
                return true;

            }
            catch (Exception ex)
            {
                // Clean-up
                if (_localDestinationDownload != null)
                    _localDestinationDownload.Close();

                try
                {
                    // Clean-up delete the download local file.
                    File.Delete(_localDestinationPathDownload);
                }
                catch { }

                if (base.Exception != null)
                    throw new Exception(ex.Message, new Exception(base.Exception.Message));
                else
                    throw ex;
            }
            finally
            {
                // Clean-up
                if (!asyncOperation)
                    if (_localDestinationDownload != null)
                        _localDestinationDownload.Close();
            }
        }
    }
}
