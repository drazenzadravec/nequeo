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
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.ServiceModel.Security;

using Nequeo.Handler.Global;
using Nequeo.Handler;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Client service base manager.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TChannel">The channel interface contract.</typeparam>
    public class ClientServiceManager<TService, TChannel> : Nequeo.Threading.AsyncExecutionHandler<TService>
        where TService : ClientBase<TChannel>
        where TChannel : class
    {
        /// <summary>
        /// Client service base manager.
        /// </summary>
        /// <param name="service">The service client type instance.</param>
        public ClientServiceManager(TService service)
        {
            base.InitiliseAsyncInstance(service);
        }

        /// <summary>
        /// Client service base manager.
        /// </summary>
        /// <param name="service">The service client type instance.</param>
        /// <param name="clientManager">The service model client manager.</param>
        public ClientServiceManager(TService service, ClientManager<TChannel> clientManager)
        {
            _clientManager = clientManager;
            base.InitiliseAsyncInstance(service);
        }

        private ClientManager<TChannel> _clientManager = null;

        /// <summary>
        /// Gets the service model client manager.
        /// </summary>
        public ClientManager<TChannel> ClientManager
        {
            get { return _clientManager; }
        }

        /// <summary>
        /// Create the callback to validate a server certificate. Manages the collection of System.Net.ServicePoint objects.
        /// </summary>
        /// <param name="validationCallback">The callback to validate a server certificate.</param>
        public virtual void CreateRemoteCertificateValidation(Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validationCallback)
        {
            // Create the callback to validate a server certificate.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(validationCallback);
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
        /// Create a custom service certificate validator.
        /// </summary>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        /// <remarks>If x509CertificateValidator is null the service certificates are always passed.</remarks>
        public virtual void CustomCertificateValidation(X509CertificateValidator x509CertificateValidator = null)
        {
            // Set the validation to custom.
            base.Instance.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = 
                System.ServiceModel.Security.X509CertificateValidationMode.Custom;

            // If not null the assign the validator.
            if (x509CertificateValidator != null)
                base.Instance.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator = x509CertificateValidator;
            else
                // Use the nequeo in-built custom validator, and pass all.
                base.Instance.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                    new Nequeo.Security.ServiceX509CertificateValidator("none", Nequeo.Security.X509CertificateLevel.None);
        }
    }

    /// <summary>
    /// Service model client manager
    /// </summary>
    /// <typeparam name="TChannel">The channel interface contract</typeparam>
    public abstract class ClientManager<TChannel> : ChannelFactory<TChannel>
        where TChannel : class
	{
        /// <summary>
        /// Client manager interface
        /// </summary>
        /// <param name="address">The System.ServiceModel.EndpointAddress that provides the location of the service.</param>
        /// <param name="username">The UserName username</param>
        /// <param name="password">The UserName password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        protected ClientManager(Uri address, 
            string username = null, string password = null,
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null)
            : base(new WSHttpBinding(), new EndpointAddress(address))
        {
            if(address == null)
                throw new ArgumentNullException("address");

            // Create the connection to the service through the channel.
            //_channel = ChannelFactory<TChannel>.CreateChannel(new WSHttpBinding(), new EndpointAddress(address));
            base.Credentials.UserName.UserName = username;
            base.Credentials.UserName.Password = password;
            base.Credentials.Windows.ClientCredential.UserName = usernameWindows;
            base.Credentials.Windows.ClientCredential.Password = passwordWindows;
            base.Credentials.ClientCertificate.Certificate = clientCertificate;

            // Set the validation to custom.
            base.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = validationMode;

            // If not null the assign the validator.
            if (x509CertificateValidator != null)
                base.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = x509CertificateValidator;
            else
                // Use the nequeo in-built custom validator, and pass all.
                base.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                    new Nequeo.Security.ServiceX509CertificateValidator("none", Nequeo.Security.X509CertificateLevel.None);

            // Create the channel
            _channel = base.CreateChannel();
        }

        /// <summary>
        /// Client manager interface
        /// </summary>
        /// <param name="address">The System.ServiceModel.EndpointAddress that provides the location of the service.</param>
        /// <param name="binding">The System.ServiceModel.Channels.Binding used to configure the endpoint.</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="usernameWindows">The Windows ClientCredential username</param>
        /// <param name="passwordWindows">The Windows ClientCredential password</param>
        /// <param name="clientCertificate">The client x509 certificate.</param>
        /// <param name="validationMode">An enumeration that lists the ways of validating a certificate.</param>
        /// <param name="x509CertificateValidator">The certificate validator. If null then the certificate is always passed.</param>
        protected ClientManager(Uri address, Binding binding, 
            string username = null, string password = null, 
            string usernameWindows = null, string passwordWindows = null,
            X509Certificate2 clientCertificate = null,
            X509CertificateValidationMode validationMode = X509CertificateValidationMode.Custom,
            X509CertificateValidator x509CertificateValidator = null)
            : base(binding, new EndpointAddress(address))
        {
            if (address == null)
                throw new ArgumentNullException("address");

            if (binding == null)
                throw new ArgumentNullException("binding");

            // Create the connection to the service through the channel.
            //_channel = ChannelFactory<TChannel>.CreateChannel(binding, new EndpointAddress(address));
            base.Credentials.UserName.UserName = username;
            base.Credentials.UserName.Password = password;
            base.Credentials.Windows.ClientCredential.UserName = usernameWindows;
            base.Credentials.Windows.ClientCredential.Password = passwordWindows;
            base.Credentials.ClientCertificate.Certificate = clientCertificate;

            // Set the validation to custom.
            base.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = validationMode;

            // If not null the assign the validator.
            if (x509CertificateValidator != null)
                base.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = x509CertificateValidator;
            else
                // Use the nequeo in-built custom validator, and pass all.
                base.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                    new Nequeo.Security.ServiceX509CertificateValidator("none", Nequeo.Security.X509CertificateLevel.None);

            // Create the channel
            _channel = base.CreateChannel();
        }

        /// <summary>
        /// Channel of communication with the adapter.
        /// </summary>
        private readonly TChannel _channel;
        private Exception _exception = null;
        private Dictionary<object, object> _results = new Dictionary<object, object>();
        private Dictionary<object, Exception> _exceptionMessage = new Dictionary<object, Exception>();
        private Dictionary<object, bool> _executionExceptionResult = new Dictionary<object, bool>();

        /// <summary>
        /// The async execute complete event.
        /// </summary>
        public event Nequeo.Threading.EventHandler<object, bool, Exception> AsyncExecuteComplete;

        /// <summary>
        /// Gets the current channel type.
        /// </summary>
        public TChannel Channel
        {
            get { return _channel; }
        }

        /// <summary>
        /// Gets the current exception is any.
        /// </summary>
        protected Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Create the callback to validate a server certificate. Manages the collection of System.Net.ServicePoint objects.
        /// </summary>
        /// <param name="validationCallback">The callback to validate a server certificate.</param>
        protected virtual void CreateRemoteCertificateValidation(Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> validationCallback)
        {
            // Create the callback to validate a server certificate.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(validationCallback);
        }

        /// <summary>
        /// Get the execution exception.
        /// </summary>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <returns>The exception of the execution; else null.</returns>
        public Exception GetExecuteAsyncException(object actionName)
        {
            return _exceptionMessage[actionName];
        }

        /// <summary>
        /// Get the execution exception result.
        /// </summary>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <returns>True is an exceptionhas occurred in the action; else false.</returns>
        public bool GetExecuteAsyncExceptionResult(object actionName)
        {
            return _executionExceptionResult[actionName];
        }

        /// <summary>
        /// Get the result of the async execution.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <returns>The result type.</returns>
        protected TResult GetExecuteAsyncResult<TResult>(object actionName)
        {
            return ((TResult)_results[actionName]);
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <param name="channelAction">The action handler</param>
        protected async Task<Boolean> Execute(Action<TChannel> channelAction)
        {
            // Start the async object
            AsyncClientChannelExecute<TChannel, Boolean> ret = new AsyncClientChannelExecute<TChannel, Boolean>(channelAction, _channel, null, null);

            // Start the action asynchronously
            Task<Boolean> data = Task<Boolean>.Factory.FromAsync(ret.BeginActionNoResult(), ret.EndActionNoResult);
            object actionAsyncResult = await data;

            // Return the result.
            return (Boolean)actionAsyncResult;
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <param name="channelAction">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        protected async void Execute(Action<TChannel> channelAction, object actionName, Action<Nequeo.Threading.AsyncOperationResult<Object>> callback = null, object state = null)
        {
            try
            {
                // Start the async object
                AsyncClientChannelExecute<TChannel, Boolean> ret = new AsyncClientChannelExecute<TChannel, Boolean>(channelAction, _channel, null, null);

                // Start the action asynchronously
                Task<Boolean> data = Task<Boolean>.Factory.FromAsync(ret.BeginActionNoResult(), ret.EndActionNoResult);
                object actionAsyncResult = await data;

                // Get the current error.
                Exception exception = ret.GetCurrentError();
                if (exception != null)
                {
                    _exception = exception;
                    _exceptionMessage[actionName] = exception;
                    _executionExceptionResult[actionName] = true;
                }
                else
                {
                    _exceptionMessage[actionName] = null;
                    _executionExceptionResult[actionName] = false;
                }

                // Set the async value.
                _results[actionName] = actionAsyncResult;

                // Send the result back to the client
                if (AsyncExecuteComplete != null)
                    AsyncExecuteComplete(this, actionName, _executionExceptionResult[actionName], _exceptionMessage[actionName]);

                // If sending the result to a callback mentod.
                if (callback != null)
                    callback(new Nequeo.Threading.AsyncOperationResult<Object>(_results[actionName], state, actionName));
            }
            catch { }
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <typeparam name="TResult">The action result type.</typeparam>
        /// <param name="channelAction">The action handler</param>
        protected async Task<TResult> Execute<TResult>(Func<TChannel, TResult> channelAction)
        {
            // Start the async object
            AsyncClientChannelExecute<TChannel, TResult> ret = new AsyncClientChannelExecute<TChannel, TResult>(channelAction, _channel, null, null);

            // Start the action asynchronously
            Task<TResult> data = Task<TResult>.Factory.FromAsync(ret.BeginActionResult(), ret.EndActionResult);
            object actionAsyncResult = await data;

            // Return the result.
            return (TResult)actionAsyncResult;
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <typeparam name="TResult">The action result type.</typeparam>
        /// <param name="channelAction">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        protected async void Execute<TResult>(Func<TChannel, TResult> channelAction, object actionName, Action<Nequeo.Threading.AsyncOperationResult<Object>> callback = null, object state = null)
        {
            try
            {
                // Start the async object
                AsyncClientChannelExecute<TChannel, TResult> ret = new AsyncClientChannelExecute<TChannel, TResult>(channelAction, _channel, null, null);

                // Start the action asynchronously
                Task<TResult> data = Task<TResult>.Factory.FromAsync(ret.BeginActionResult(), ret.EndActionResult);
                object actionAsyncResult = await data;

                // Get the current error.
                Exception exception = ret.GetCurrentError();
                if (exception != null)
                {
                    _exception = exception;
                    _exceptionMessage[actionName] = exception;
                    _executionExceptionResult[actionName] = true;
                }
                else
                {
                    _exceptionMessage[actionName] = null;
                    _executionExceptionResult[actionName] = false;
                }

                // Set the async value.
                _results[actionName] = actionAsyncResult;

                // Send the result back to the client
                if (AsyncExecuteComplete != null)
                    AsyncExecuteComplete(this, actionName, _executionExceptionResult[actionName], _exceptionMessage[actionName]);

                // If sending the result to a callback mentod.
                if (callback != null)
                    callback(new Nequeo.Threading.AsyncOperationResult<Object>(_results[actionName], state, actionName));
            }
            catch { }
        }
	}

    /// <summary>
    /// Asyncronous client channel execute
    /// </summary>
    internal sealed class AsyncClientChannelExecute<TChannel, TResult> : Nequeo.Threading.AsynchronousResult<TResult>
    {
        /// <summary>
        /// Default async function handler
        /// </summary>
        /// <param name="channelAction">The function operation channel</param>
        /// <param name="channel">The current channel</param>
        /// <param name="callback">The call back handler</param>
        /// <param name="state">The state object</param>
        public AsyncClientChannelExecute(Func<TChannel, TResult> channelAction, TChannel channel, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _channelAction = channelAction;
            _channel = channel;
            _exception = null;
        }

        /// <summary>
        /// Default async action handler
        /// </summary>
        /// <param name="channelAction">The action operation channel</param>
        /// <param name="channel">The current channel</param>
        /// <param name="callback">The call back handler</param>
        /// <param name="state">The state object</param>
        public AsyncClientChannelExecute(Action<TChannel> channelAction, TChannel channel, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _channelActionEx = channelAction;
            _channel = channel;
            _exception = null;
        }

        private Func<TResult> _actionHandler = null;
        private Func<Boolean> _actionNoResultHandler = null;
        private Func<TChannel, TResult> _channelAction = null;
        private Action<TChannel> _channelActionEx = null;
        private TChannel _channel = default(TChannel);
        private Exception _exception = null;

        /// <summary>
        /// Begin the async operation.
        /// </summary>
        /// <returns>The async result.</returns>
        public IAsyncResult BeginActionResult()
        {
            if (_actionHandler == null)
                _actionHandler = new Func<TResult>(FuncAsyncActionResult);

            // Begin the async call.
            return _actionHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End the async operation.
        /// </summary>
        /// <param name="ar">The async result</param>
        /// <returns>The result type.</returns>
        public TResult EndActionResult(IAsyncResult ar)
        {
            if (_actionHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            return _actionHandler.EndInvoke(ar);
        }

        /// <summary>
        /// Begin async action
        /// </summary>
        /// <returns>The async result</returns>
        public IAsyncResult BeginActionNoResult()
        {
            if (_actionNoResultHandler == null)
                _actionNoResultHandler = new Func<Boolean>(FuncAsyncActionNoResult);

            // Begin the async call.
            return _actionNoResultHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End async action
        /// </summary>
        /// <param name="ar">The async result</param>
        public Boolean EndActionNoResult(IAsyncResult ar)
        {
            if (_actionNoResultHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            return _actionNoResultHandler.EndInvoke(ar);
        }

        /// <summary>
        /// Gets the current error if any.
        /// </summary>
        /// <returns>The current exception if any.</returns>
        public Exception GetCurrentError()
        {
            return _exception;
        }

        /// <summary>
        /// Execute the asyn result.
        /// </summary>
        /// <returns>The result type to return.</returns>
        private TResult FuncAsyncActionResult()
        {
            try
            {
                return _channelAction(_channel);
            }
            catch (Exception ex)
            {
                _exception = ex;
                return default(TResult);
            }
        }

        /// <summary>
        /// Execute the asyn result.
        /// </summary>
        private Boolean FuncAsyncActionNoResult()
        {
            try
            {
                _channelActionEx(_channel);
                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }
    }
}
