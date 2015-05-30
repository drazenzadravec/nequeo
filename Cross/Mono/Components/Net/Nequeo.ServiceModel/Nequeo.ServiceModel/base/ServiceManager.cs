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
    /// WCF service host controller.
    /// </summary>
    public class ServiceManager : ServiceHostBase, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        public ServiceManager(Type serviceType) 
            : base(serviceType)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        /// <param name="baseAddresses">The collection of uri addresses to listen on.</param>
        public ServiceManager(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
        #endregion

        #region Public Events
        /// <summary>
        /// General error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> Error;

        #endregion

        #region Virtual Methods
        /// <summary>
        /// General error.
        /// </summary>
        /// <param name="e">Error message arguments.</param>
        protected virtual void OnError(ErrorMessageArgs e)
        {
            if (Error != null)
                Error(this, e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new instance of the service host so
        /// additional settings can be applied before opening.
        /// </summary>
        public override void Initialise()
        {
            try
            {
                if (_baseAddresses == null)
                    _serviceHost = new ServiceHost(_serviceType);
                else
                    _serviceHost = new ServiceHost(_serviceType, _baseAddresses);
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        public override void Open()
        {
            try
            {
                OpenHost(null, new TimeSpan(0));
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public override void Open(TimeSpan timeSpan)
        {
            if (timeSpan.Ticks < 1)
                throw new ArgumentNullException("timeSpan");

            try
            {
                OpenHost(null, timeSpan);
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        public override void Open(X509Certificate2 serviceCertificate)
        {
            if (serviceCertificate == null)
                throw new ArgumentNullException("serviceCertificate");

            try
            {
                OpenHost(serviceCertificate, new TimeSpan(0));
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public override void Open(X509Certificate2 serviceCertificate, TimeSpan timeSpan)
        {
            if (serviceCertificate == null)
                throw new ArgumentNullException("serviceCertificate");

            if (timeSpan.Ticks < 1)
                throw new ArgumentNullException("timeSpan");

            try
            {
                OpenHost(serviceCertificate, timeSpan);
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public override void Close()
        {
            try
            {
                CloseHost();
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
            finally
            {
                _serviceHost = null;
            }
        }
        #endregion

        #region Dispose Object Methods
        private bool _disposed = false;

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
        ~ServiceManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// WCF web service host controller.
    /// </summary>
    public class WebServiceManager : WebServiceHostBase, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        public WebServiceManager(Type serviceType)
            : base(serviceType)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        /// <param name="baseAddresses">The collection of uri addresses to listen on.</param>
        public WebServiceManager(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
        #endregion

        #region Public Events
        /// <summary>
        /// General error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> Error;

        #endregion

        #region Virtual Methods
        /// <summary>
        /// General error.
        /// </summary>
        /// <param name="e">Error message arguments.</param>
        protected virtual void OnError(ErrorMessageArgs e)
        {
            if (Error != null)
                Error(this, e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new instance of the service host so
        /// additional settings can be applied before opening.
        /// </summary>
        public override void Initialise()
        {
            try
            {
                if (_baseAddresses == null)
                    _serviceHost = new WebServiceHost(_serviceType);
                else
                    _serviceHost = new WebServiceHost(_serviceType, _baseAddresses);
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        public override void Open()
        {
            try
            {
                OpenHost(null, new TimeSpan(0));
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public override void Open(TimeSpan timeSpan)
        {
            if (timeSpan.Ticks < 1)
                throw new ArgumentNullException("timeSpan");

            try
            {
                OpenHost(null, timeSpan);
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        public override void Open(X509Certificate2 serviceCertificate)
        {
            if (serviceCertificate == null)
                throw new ArgumentNullException("serviceCertificate");

            try
            {
                OpenHost(serviceCertificate, new TimeSpan(0));
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public override void Open(X509Certificate2 serviceCertificate, TimeSpan timeSpan)
        {
            if (serviceCertificate == null)
                throw new ArgumentNullException("serviceCertificate");

            if (timeSpan.Ticks < 1)
                throw new ArgumentNullException("timeSpan");

            try
            {
                OpenHost(serviceCertificate, timeSpan);
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public override void Close()
        {
            try
            {
                CloseHost();
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
            finally
            {
                _serviceHost = null;
            }
        }
        #endregion

        #region Dispose Object Methods
        private bool _disposed = false;

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
        ~WebServiceManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// WCF base service host base.
    /// </summary>
    public abstract class ServiceHostBase
    {
        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        public ServiceHostBase(Type serviceType)
        {
            // Get the validate value.
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            _serviceType = serviceType;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        /// <param name="baseAddresses">The collection of uri addresses to listen on.</param>
        public ServiceHostBase(Type serviceType, Uri[] baseAddresses)
        {
            // Get the validate value.
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            if (baseAddresses == null)
                throw new ArgumentNullException("baseAddresses");

            _serviceType = serviceType;
            _baseAddresses = baseAddresses;
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// 
        /// </summary>
        protected Type _serviceType;
        /// <summary>
        /// 
        /// </summary>
        protected Uri[] _baseAddresses = null;
        /// <summary>
        /// 
        /// </summary>
        protected ServiceHost _serviceHost = null;
        /// <summary>
        /// 
        /// </summary>
        protected UserNamePasswordValidator _userNamePasswordValidator = null;
        /// <summary>
        /// 
        /// </summary>
        protected X509CertificateValidator _clientX509CertificateValidator = null;
        /// <summary>
        /// 
        /// </summary>
        protected X509Certificate2 _clientCertificate = null;
        /// <summary>
        /// 
        /// </summary>
        protected X509Certificate2 _serviceCertificate = null;
        /// <summary>
        /// 
        /// </summary>
        protected TimeSpan _timeSpan;
        /// <summary>
        /// 
        /// </summary>
        protected CommunicationState _communicationState = CommunicationState.Closed;
        /// <summary>
        /// 
        /// </summary>
        protected int _manualFlowControlLimit = -1;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<string> _onClosed = null;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<string> _onOpened = null;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<string> _onFaulted = null;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<UnknownMessageReceivedEventArgs> _onUnknownMessageReceived = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Sets the on service closed handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<string> OnClosed
        {
            set { _onClosed = value; }
        }

        /// <summary>
        /// Sets the on service opened handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<string> OnOpened
        {
            set { _onOpened = value; }
        }

        /// <summary>
        /// Sets the on service fault handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<string> OnFaulted
        {
            set { _onFaulted = value; }
        }

        /// <summary>
        /// Sets the on service unknown message received handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<UnknownMessageReceivedEventArgs> OnUnknownMessageReceived
        {
            set { _onUnknownMessageReceived = value; }
        }

        /// <summary>
        /// Gets the service host control.
        /// </summary>
        public ServiceHost ServiceHost
        {
            get { return _serviceHost; }
        }

        /// <summary>
        /// Gets the service communication state.
        /// </summary>
        public CommunicationState CommunicationState
        {
            get { return _communicationState; }
        }

        /// <summary>
        /// Gets the x509 service certificate.
        /// </summary>
        public X509Certificate2 ServiceCertificate
        {
            get { return _serviceCertificate; }
        }

        /// <summary>
        /// Gets the service connection time out.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
        }

        /// <summary>
        /// Gets the collection of base uri addresses the service is hosting.
        /// </summary>
        public Uri[] BaseAddresses
        {
            get { return _baseAddresses; }
        }

        /// <summary>
        /// Gets the service type base host.
        /// </summary>
        public Type ServiceType
        {
            get { return _serviceType; }
        }

        /// <summary>
        /// Gets sets, the flow control limit for messages received by the service hosted.
        /// </summary>
        public int ManualFlowControlLimit
        {
            get { return _manualFlowControlLimit; }
            set { _manualFlowControlLimit = value; }
        }

        /// <summary>
        /// Gets sets, the x509 client certificate.
        /// </summary>
        public X509Certificate2 ClientCertificate
        {
            get { return _clientCertificate; }
            set { _clientCertificate = value; }
        }

        /// <summary>
        /// Gets sets, the user name password validation object.
        /// </summary>
        public UserNamePasswordValidator UserNamePasswordValidator
        {
            get { return _userNamePasswordValidator; }
            set { _userNamePasswordValidator = value; }
        }

        /// <summary>
        /// Gets sets, the x509 client certificate validation object.
        /// </summary>
        public X509CertificateValidator ClientX509CertificateValidator
        {
            get { return _clientX509CertificateValidator; }
            set { _clientX509CertificateValidator = value; }
        }
        #endregion

        #region Public Abstract Methods
        /// <summary>
        /// Creates a new instance of the service host so
        /// additional settings can be applied before opening.
        /// </summary>
        public abstract void Initialise();

        /// <summary>
        /// Open a new service host.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public abstract void Open(TimeSpan timeSpan);

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        public abstract void Open(X509Certificate2 serviceCertificate);

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public abstract void Open(X509Certificate2 serviceCertificate, TimeSpan timeSpan);

        /// <summary>
        /// Close the service host.
        /// </summary>
        public abstract void Close();

        #endregion

        #region Private Methods
        /// <summary>
        /// Close the service host.
        /// </summary>
        protected void CloseHost()
        {
            if (_serviceHost != null)
                _serviceHost.Close();

            _serviceHost = null;
        }

        /// <summary>
        /// Open a new host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        protected void OpenHost(X509Certificate2 serviceCertificate, TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
            _serviceCertificate = serviceCertificate;

            // If the service host is null then
            // initialise a new hosting service.
            if (_serviceHost == null)
                Initialise();

            // If the current service state is not
            // closed then return, a service host is
            // currently active.
            if (_serviceHost.State == CommunicationState.Opened)
                return;

            // Attach to each event handler.
            _serviceHost.Closed += new System.EventHandler(ServiceHostClosed);
            _serviceHost.Opened += new System.EventHandler(ServiceHostOpened);
            _serviceHost.Faulted += new System.EventHandler(ServiceHostFaulted);
            _serviceHost.UnknownMessageReceived +=
                new System.EventHandler<UnknownMessageReceivedEventArgs>(ServiceHostUnknownMessageReceived);

            // If a service certificate has been included.
            if (_serviceCertificate != null)
            {
                // Assign the service certificate
                // make sure it contains a private key.
                _serviceHost.Credentials.ServiceCertificate.Certificate = _serviceCertificate;
                if (!_serviceHost.Credentials.ServiceCertificate.Certificate.HasPrivateKey)
                    throw new Exception("The service certificate does not contain a private key.");
            }

            // If a custom username and password validator has been included.
            if (_userNamePasswordValidator != null)
            {
                // Indicate the validator is in custom mode.
                _serviceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode =
                    System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;

                // Assign the validator.
                _serviceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator =
                    _userNamePasswordValidator;
            }

            // If a custom x509 client certificate has been included.
            if (_clientX509CertificateValidator != null)
            {
                // Indicate the validator is in custom mode.
                _serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                    System.ServiceModel.Security.X509CertificateValidationMode.Custom;

                // Assign the validator.
                _serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator =
                    _clientX509CertificateValidator;
            }

            // If a client certificate has been issued
            if (_clientCertificate != null)
            {
                // Assign the service certificate
                // make sure it contains a private key.
                _serviceHost.Credentials.ClientCertificate.Certificate = _clientCertificate;
                if (!_serviceHost.Credentials.ClientCertificate.Certificate.HasPrivateKey)
                    throw new Exception("The client certificate does not contain a private key.");
            }

            // Assign the manual flow control limit.
            if (_manualFlowControlLimit > -1)
                _serviceHost.ManualFlowControlLimit = _manualFlowControlLimit;

            // Open the service host listener.
            if (timeSpan.Ticks > 0)
                _serviceHost.Open(timeSpan);
            else
                _serviceHost.Open();

            // Get the current service host communication state.
            _communicationState = _serviceHost.State;
        }

        /// <summary>
        /// Service host received unknown message event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onUnknownMessageReceived != null)
                _onUnknownMessageReceived(e);
        }

        /// <summary>
        /// Service host communication fault event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostFaulted(object sender, EventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onFaulted != null)
                _onFaulted("Service " + _serviceType.FullName + " has encounted communication fault.");
        }

        /// <summary>
        /// Service host opened event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostOpened(object sender, EventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onOpened != null)
                _onOpened("Service " + _serviceType.FullName + " has opened.");
        }

        /// <summary>
        /// Service host closed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostClosed(object sender, EventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onClosed != null)
                _onClosed("Service " + _serviceType.FullName + " has closed.");
        }
        #endregion
    }

    /// <summary>
    /// WCF base web service host base.
    /// </summary>
    public abstract class WebServiceHostBase
    {
        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        public WebServiceHostBase(Type serviceType)
        {
            // Get the validate value.
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            _serviceType = serviceType;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceType">The service type to host.</param>
        /// <param name="baseAddresses">The collection of uri addresses to listen on.</param>
        public WebServiceHostBase(Type serviceType, Uri[] baseAddresses)
        {
            // Get the validate value.
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            if (baseAddresses == null)
                throw new ArgumentNullException("baseAddresses");

            _serviceType = serviceType;
            _baseAddresses = baseAddresses;
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// 
        /// </summary>
        protected Type _serviceType;
        /// <summary>
        /// 
        /// </summary>
        protected Uri[] _baseAddresses = null;
        /// <summary>
        /// 
        /// </summary>
        protected WebServiceHost _serviceHost = null;
        /// <summary>
        /// 
        /// </summary>
        protected UserNamePasswordValidator _userNamePasswordValidator = null;
        /// <summary>
        /// 
        /// </summary>
        protected X509CertificateValidator _clientX509CertificateValidator = null;
        /// <summary>
        /// 
        /// </summary>
        protected X509Certificate2 _clientCertificate = null;
        /// <summary>
        /// 
        /// </summary>
        protected X509Certificate2 _serviceCertificate = null;
        /// <summary>
        /// 
        /// </summary>
        protected TimeSpan _timeSpan;
        /// <summary>
        /// 
        /// </summary>
        protected CommunicationState _communicationState = CommunicationState.Closed;
        /// <summary>
        /// 
        /// </summary>
        protected int _manualFlowControlLimit = -1;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<string> _onClosed = null;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<string> _onOpened = null;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<string> _onFaulted = null;
        /// <summary>
        /// 
        /// </summary>
        protected Nequeo.Threading.ActionHandler<UnknownMessageReceivedEventArgs> _onUnknownMessageReceived = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Sets the on service closed handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<string> OnClosed
        {
            set { _onClosed = value; }
        }

        /// <summary>
        /// Sets the on service opened handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<string> OnOpened
        {
            set { _onOpened = value; }
        }

        /// <summary>
        /// Sets the on service fault handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<string> OnFaulted
        {
            set { _onFaulted = value; }
        }

        /// <summary>
        /// Sets the on service unknown message received handler.
        /// </summary>
        public Nequeo.Threading.ActionHandler<UnknownMessageReceivedEventArgs> OnUnknownMessageReceived
        {
            set { _onUnknownMessageReceived = value; }
        }

        /// <summary>
        /// Gets the service host control.
        /// </summary>
        public WebServiceHost ServiceHost
        {
            get { return _serviceHost; }
        }

        /// <summary>
        /// Gets the service communication state.
        /// </summary>
        public CommunicationState CommunicationState
        {
            get { return _communicationState; }
        }

        /// <summary>
        /// Gets the x509 service certificate.
        /// </summary>
        public X509Certificate2 ServiceCertificate
        {
            get { return _serviceCertificate; }
        }

        /// <summary>
        /// Gets the service connection time out.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
        }

        /// <summary>
        /// Gets the collection of base uri addresses the service is hosting.
        /// </summary>
        public Uri[] BaseAddresses
        {
            get { return _baseAddresses; }
        }

        /// <summary>
        /// Gets the service type base host.
        /// </summary>
        public Type ServiceType
        {
            get { return _serviceType; }
        }

        /// <summary>
        /// Gets sets, the flow control limit for messages received by the service hosted.
        /// </summary>
        public int ManualFlowControlLimit
        {
            get { return _manualFlowControlLimit; }
            set { _manualFlowControlLimit = value; }
        }

        /// <summary>
        /// Gets sets, the x509 client certificate.
        /// </summary>
        public X509Certificate2 ClientCertificate
        {
            get { return _clientCertificate; }
            set { _clientCertificate = value; }
        }

        /// <summary>
        /// Gets sets, the user name password validation object.
        /// </summary>
        public UserNamePasswordValidator UserNamePasswordValidator
        {
            get { return _userNamePasswordValidator; }
            set { _userNamePasswordValidator = value; }
        }

        /// <summary>
        /// Gets sets, the x509 client certificate validation object.
        /// </summary>
        public X509CertificateValidator ClientX509CertificateValidator
        {
            get { return _clientX509CertificateValidator; }
            set { _clientX509CertificateValidator = value; }
        }
        #endregion

        #region Public Abstract Methods
        /// <summary>
        /// Creates a new instance of the service host so
        /// additional settings can be applied before opening.
        /// </summary>
        public abstract void Initialise();

        /// <summary>
        /// Open a new service host.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public abstract void Open(TimeSpan timeSpan);

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        public abstract void Open(X509Certificate2 serviceCertificate);

        /// <summary>
        /// Open a new service host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        public abstract void Open(X509Certificate2 serviceCertificate, TimeSpan timeSpan);

        /// <summary>
        /// Close the service host.
        /// </summary>
        public abstract void Close();

        #endregion

        #region Private Methods
        /// <summary>
        /// Close the service host.
        /// </summary>
        protected void CloseHost()
        {
            if (_serviceHost != null)
                _serviceHost.Close();

            _serviceHost = null;
        }

        /// <summary>
        /// Open a new host.
        /// </summary>
        /// <param name="serviceCertificate">The x509 service certificate used for communication.</param>
        /// <param name="timeSpan">The time span for connection timeout.</param>
        protected void OpenHost(X509Certificate2 serviceCertificate, TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
            _serviceCertificate = serviceCertificate;

            // If the service host is null then
            // initialise a new hosting service.
            if (_serviceHost == null)
                Initialise();

            // If the current service state is not
            // closed then return, a service host is
            // currently active.
            if (_serviceHost.State == CommunicationState.Opened)
                return;

            // Attach to each event handler.
            _serviceHost.Closed += new System.EventHandler(ServiceHostClosed);
            _serviceHost.Opened += new System.EventHandler(ServiceHostOpened);
            _serviceHost.Faulted += new System.EventHandler(ServiceHostFaulted);
            _serviceHost.UnknownMessageReceived +=
                new System.EventHandler<UnknownMessageReceivedEventArgs>(ServiceHostUnknownMessageReceived);

            // If a service certificate has been included.
            if (_serviceCertificate != null)
            {
                // Assign the service certificate
                // make sure it contains a private key.
                _serviceHost.Credentials.ServiceCertificate.Certificate = _serviceCertificate;
                if (!_serviceHost.Credentials.ServiceCertificate.Certificate.HasPrivateKey)
                    throw new Exception("The service certificate does not contain a private key.");
            }

            // If a custom username and password validator has been included.
            if (_userNamePasswordValidator != null)
            {
                // Indicate the validator is in custom mode.
                _serviceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode =
                    System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;

                // Assign the validator.
                _serviceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator =
                    _userNamePasswordValidator;
            }

            // If a custom x509 client certificate has been included.
            if (_clientX509CertificateValidator != null)
            {
                // Indicate the validator is in custom mode.
                _serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                    System.ServiceModel.Security.X509CertificateValidationMode.Custom;

                // Assign the validator.
                _serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator =
                    _clientX509CertificateValidator;
            }

            // If a client certificate has been issued
            if (_clientCertificate != null)
            {
                // Assign the service certificate
                // make sure it contains a private key.
                _serviceHost.Credentials.ClientCertificate.Certificate = _clientCertificate;
                if (!_serviceHost.Credentials.ClientCertificate.Certificate.HasPrivateKey)
                    throw new Exception("The client certificate does not contain a private key.");
            }

            // Assign the manual flow control limit.
            if (_manualFlowControlLimit > -1)
                _serviceHost.ManualFlowControlLimit = _manualFlowControlLimit;

            // Open the service host listener.
            if (timeSpan.Ticks > 0)
                _serviceHost.Open(timeSpan);
            else
                _serviceHost.Open();

            // Get the current service host communication state.
            _communicationState = _serviceHost.State;
        }

        /// <summary>
        /// Service host received unknown message event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onUnknownMessageReceived != null)
                _onUnknownMessageReceived(e);
        }

        /// <summary>
        /// Service host communication fault event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostFaulted(object sender, EventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onFaulted != null)
                _onFaulted("Service " + _serviceType.FullName + " has encounted communication fault.");
        }

        /// <summary>
        /// Service host opened event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostOpened(object sender, EventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onOpened != null)
                _onOpened("Service " + _serviceType.FullName + " has opened.");
        }

        /// <summary>
        /// Service host closed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceHostClosed(object sender, EventArgs e)
        {
            _communicationState = _serviceHost.State;
            if (_onClosed != null)
                _onClosed("Service " + _serviceType.FullName + " has closed.");
        }
        #endregion
    }
}
