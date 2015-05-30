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
using System.Runtime.Serialization;
using System.Linq.Expressions;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Channels;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Linq;
using Nequeo.Linq.Extension;

namespace Nequeo.Management.ServiceModel
{
    /// <summary>
    /// Message client implementation
    /// </summary>
    public partial class Client : Nequeo.Net.ServiceModel.ClientManager<Nequeo.Management.ServiceModel.IServer>
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        /// <remarks>
        /// Represents an interoperable binding that supports distributed transactions
        /// and secure, reliable sessions (WSHttpBinding binding).
        /// </remarks>
        public Client(string username = null, string password = null, 
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Management.ServiceModel.Properties.Settings.Default.ServiceAddress),
                new System.ServiceModel.WSHttpBinding(),
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endPointAddress">The specific end point address</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        /// <remarks>
        /// Represents an interoperable binding that supports distributed transactions
        /// and secure, reliable sessions (WSHttpBinding binding).
        /// </remarks>
        public Client(string endPointAddress, 
            string username = null, string password = null, 
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress),
                new System.ServiceModel.WSHttpBinding(),
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(Binding binding, string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Management.ServiceModel.Properties.Settings.Default.ServiceAddress),
                binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endPointAddress">The specific end point address</param>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(string endPointAddress, Binding binding, 
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress), binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(System.ServiceModel.WSHttpBinding binding, string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Management.ServiceModel.Properties.Settings.Default.ServiceAddress),
                binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endPointAddress">The specific end point address</param>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(string endPointAddress, System.ServiceModel.WSHttpBinding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress), binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(System.ServiceModel.BasicHttpBinding binding, string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Management.ServiceModel.Properties.Settings.Default.ServiceAddress),
                binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endPointAddress">The specific end point address</param>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(string endPointAddress, System.ServiceModel.BasicHttpBinding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress), binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(System.ServiceModel.Channels.CustomBinding binding, string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Management.ServiceModel.Properties.Settings.Default.ServiceAddress),
                binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endPointAddress">The specific end point address</param>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(string endPointAddress, System.ServiceModel.Channels.CustomBinding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress), binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(System.ServiceModel.WebHttpBinding binding, string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(Nequeo.Management.ServiceModel.Properties.Settings.Default.ServiceAddress),
                binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="endPointAddress">The specific end point address</param>
        /// <param name="binding">Contains the binding elements that specify the protocols, 
        /// transports, and message encoders used for communication between clients and services.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        public Client(string endPointAddress, System.ServiceModel.WebHttpBinding binding,
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null) :
            base(
                new Uri(endPointAddress), binding,
                username, password, usernameWindows, passwordWindows, clientCertificate, validationMode, x509CertificateValidator)
        {
            OnCreated();
        }

        private Message _message = null;
        private Exception[] _exceptions = null;

        /// <summary>
        /// Gets the colelction of expression exceptions that may
        /// have occured when calling SendMessageExpression methods.
        /// </summary>
        public Exception[] ExpressionExceptions
        {
            get { return _exceptions; }
        }

        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The response message.</returns>
        public Message SendMessage(Message message)
        {
            return base.Channel.SendMessage(message);
        }

        /// <summary>
        /// Send a message to the server through the expression.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="predicate">The predicate query to use for the call.</param>
        /// <param name="values">The array of query values to apply to the predicate query.</param>
        /// <returns>The response message collection.</returns>
        public Message[] SendMessageExpression(Message message, string predicate, params object[] values)
        {
            _message = message;
            _exceptions = null;
            return SendMessageExpressionEx(message, predicateString: predicate, values: values);
        }

        /// <summary>
        /// Send a message to the server through the expression.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The response message collection.</returns>
        public Message[] SendMessageExpression(Message message, Expression<Func<Message, bool>> predicate)
        {
            _message = message;
            _exceptions = null;
            return SendMessageExpressionEx(message, predicateExpression: predicate);
        }

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

        /// <summary>
        /// Send a message to the server through the expression.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="predicateExpression">A function to test each element for a condition.</param>
        /// <param name="predicateString">The predicate query to use for the call.</param>
        /// <param name="values">The array of query values to apply to the predicate query.</param>
        /// <returns>The response message collection.</returns>
        private Message[] SendMessageExpressionEx(Message message,
            Expression<Func<Message, bool>> predicateExpression = null,
            string predicateString = null, params object[] values)
        {
            Message[] data = null;

            // Create the Generic Queryable Provider Inspector and
            // Assign the execution function handler.
            Linq.GenericQueryableProviderInspector<Message> query = new Linq.GenericQueryableProviderInspector<Message>();
            query.ExecuteAction = (Nequeo.Model.ExpressionTreeModel model) => GetQueryData(model);

            // If an expression predicate is used.
            if (predicateExpression != null)
                data = query.QueryableProvider().Where(predicateExpression).ToArray();

            // If a string predicate is used.
            if (!String.IsNullOrEmpty(predicateString))
                data = query.QueryableProvider().Where(predicateString, values).ToArray();

            // Return the data.
            return data;
        }

        /// <summary>
        /// Get the query result data for the current expression.
        /// </summary>
        /// <param name="model">Expression tree model containing the expression inspector data.</param>
        /// <returns>The collection of message data.</returns>
        private Message[] GetQueryData(Nequeo.Model.ExpressionTreeModel model)
        {
            List<Message> data = new List<Message>();
            List<Exception> exceptions = new List<System.Exception>();

            // Get all the expression model data.
            Nequeo.Model.ExpressionModel expressionModel = model.GetExpression(true);

            // If where query expression data exists.
            if (expressionModel.Where.Length > 0)
            {
                // For each where query expression.
                foreach (string item in expressionModel.Where)
                {
                    // Get the name and value and operator information.
                    string[] split = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string name = split[0];
                    string operatorNode = split[1];
                    string value = split[2].Trim(new char[] { '"' });

                    // If the current item is the 'Message' model 'Name' property.
                    if (name.ToLower().Trim() == "name") // "name" = This is the 'Name' property in the 'Message' model
                    {
                        // Create a new message.
                        Message messageData = new Message() { Name = value, Data = _message.Data };

                        try
                        {
                            // Get the message data.
                            data.Add(SendMessage(messageData));
                        }
                        catch (Exception ex) { exceptions.Add(ex); }
                    }
                }
            }

            // get the list of exceptions.
            _exceptions = exceptions.ToArray();

            // Return the collection of data.
            return data.ToArray();
        }
	}
}
