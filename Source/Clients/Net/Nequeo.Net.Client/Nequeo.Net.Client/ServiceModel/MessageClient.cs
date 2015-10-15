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
    /// Simple streamed data message client
    /// </summary>
    public class MessageClient : Nequeo.Net.ServiceModel.ClientManager<Nequeo.Service.Message.IStream>
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
        public MessageClient(
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.MessageClientBaseAddress),
                new System.ServiceModel.BasicHttpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
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
        public MessageClient(string endPointAddress,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.BasicHttpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize
                    }
                }, username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
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
        public MessageClient(System.ServiceModel.MessageCredentialType messageCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.MessageClientBaseAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize
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
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
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
        public MessageClient(string endPointAddress, System.ServiceModel.MessageCredentialType messageCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize
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
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
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
        public MessageClient(System.ServiceModel.TcpClientCredentialType tcpClientCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.MessageClientBaseAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize
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
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
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
        public MessageClient(string endPointAddress, System.ServiceModel.TcpClientCredentialType tcpClientCredentialType,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.NetTcpBinding()
                {
                    MaxReceivedMessageSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    MaxBufferPoolSize = Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    MaxBufferSize = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                    {
                        MaxArrayLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxBytesPerRead = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxDepth = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxNameTableCharCount = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize,
                        MaxStringContentLength = (int)Nequeo.Net.Properties.Settings.Default.MessageClientMaxReceivedMessageSize
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
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
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
        public MessageClient(System.ServiceModel.Channels.Binding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Net.Properties.Settings.Default.MessageClientBaseAddress),
                binding,
            username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
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
        public MessageClient(string endPointAddress, System.ServiceModel.Channels.Binding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                binding,
            username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator
            )
        {
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
        }
#else
        /// <summary>
        /// Basic Http Binding constructor
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        public MessageClient(string endPointAddress,
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
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
        }

        /// <summary>
        /// Basic Http Binding constructor
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="binding">The endpoint binding.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        public MessageClient(string endPointAddress, System.ServiceModel.Channels.Binding binding,
            string username = null, string password = null) :
            base(
                new Uri(endPointAddress),
                binding, username, password
            )
        {
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
        }

        /// <summary>
        /// Transport security Basic Http binding constructor.
        /// </summary>
        /// <param name="endPointAddress">The endpoint address to connect to.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        public MessageClient(Uri endPointAddress,
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
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<MessageClient>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);
        }
#endif

        private Nequeo.Threading.AsyncExecutionHandler<MessageClient> _asyncAccount = null;
        private Exception _exception = null;

        private Dictionary<object, object> _callback = new Dictionary<object, object>();
        private Dictionary<object, object> _state = new Dictionary<object, object>();

        /// <summary>
        /// Gets the current async exception; else null;
        /// </summary>
        new public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Async complete action handler
        /// </summary>
        /// <param name="sender">The current object handler</param>
        /// <param name="e1">The action execution result</param>
        /// <param name="e2">The unique action name.</param>
        private void _asyncAccount_AsyncComplete(object sender, object e1, string e2)
        {
            switch (e2)
            {
                case "MessageString":
                    Action<Nequeo.Threading.AsyncOperationResult<string>> callbackMessageString = (Action<Nequeo.Threading.AsyncOperationResult<string>>)_callback[e2];
                    callbackMessageString(new Nequeo.Threading.AsyncOperationResult<string>(((string)e1), _state[e2], e2));
                    break;

                case "MessageByte":
                    Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callbackMessageByte = (Action<Nequeo.Threading.AsyncOperationResult<byte[]>>)_callback[e2];
                    callbackMessageByte(new Nequeo.Threading.AsyncOperationResult<byte[]>(((byte[])e1), _state[e2], e2));
                    break;

                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Async error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e1"></param>
        private void _asyncAccount_AsyncError(object sender, Exception e1)
        {
            _exception = e1;
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
        /// Send and recieve string message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <returns>The send message</returns>
        public string MessageString(string message)
        {
            try
            {
                return base.Channel.MessageString(message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Send and recieve string message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <returns>The send message</returns>
        public void MessageString(string message, Action<Nequeo.Threading.AsyncOperationResult<string>> callback, object state = null)
        {
            string keyName = "MessageString";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<string>(u => u.MessageString(message), keyName);
        }

        /// <summary>
        /// Send and recieve byte message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <returns>The send message</returns>
        public byte[] MessageByte(byte[] message)
        {
            try
            {
                return base.Channel.MessageByte(message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Send and recieve byte message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <returns>The send message</returns>
        public void MessageByte(byte[] message, Action<Nequeo.Threading.AsyncOperationResult<byte[]>> callback, object state = null)
        {
            string keyName = "MessageByte";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<byte[]>(u => u.MessageByte(message), keyName);
        }
    }
}
