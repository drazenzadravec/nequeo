/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Nequeo.Service.NequeoCompany
{
    /// <summary>
    /// Employee service host
    /// </summary>
    public class EmployeeServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Employee Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public EmployeeServiceHost()
            : base(typeof(Nequeo.Service.NequeoCompany.Employee))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public EmployeeServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.NequeoCompany.Employee), baseAddresses)
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
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IEmployee), binding, "");
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IEmployee), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }

    /// <summary>
    /// Employee service host
    /// </summary>
    public class CustomerServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Customer Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomerServiceHost()
            : base(typeof(Nequeo.Service.NequeoCompany.Customer))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public CustomerServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.NequeoCompany.Customer), baseAddresses)
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
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.ICustomer), binding, "");
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.ICustomer), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }

    /// <summary>
    /// Account service host
    /// </summary>
    public class AccountServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Account Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public AccountServiceHost()
            : base(typeof(Nequeo.Service.NequeoCompany.Account))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public AccountServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.NequeoCompany.Account), baseAddresses)
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
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IAccount), binding, "");
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IAccount), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }

    /// <summary>
    /// Company service host
    /// </summary>
    public class CompanyServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Company Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public CompanyServiceHost()
            : base(typeof(Nequeo.Service.NequeoCompany.Company))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public CompanyServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.NequeoCompany.Company), baseAddresses)
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
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.ICompany), binding, "");
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.ICompany), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }

    /// <summary>
    /// Asset service host
    /// </summary>
    public class AssetServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Asset Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public AssetServiceHost()
            : base(typeof(Nequeo.Service.NequeoCompany.Asset))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public AssetServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.NequeoCompany.Asset), baseAddresses)
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
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IAsset), binding, "");
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IAsset), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }

    /// <summary>
    /// Product service host
    /// </summary>
    public class ProductServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Product Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductServiceHost()
            : base(typeof(Nequeo.Service.NequeoCompany.Product))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public ProductServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.NequeoCompany.Product), baseAddresses)
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
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IProduct), binding, "");
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IProduct), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }

    /// <summary>
    /// Vendor service host
    /// </summary>
    public class VendorServiceHost : Nequeo.Net.ServiceModel.ServiceManager
    {
        #region Vendor Service Host
        /// <summary>
        /// Default constructor
        /// </summary>
        public VendorServiceHost()
            : base(typeof(Nequeo.Service.NequeoCompany.Vendor))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddresses">The base uri addresses.</param>
        public VendorServiceHost(Uri[] baseAddresses)
            : base(typeof(Nequeo.Service.NequeoCompany.Vendor), baseAddresses)
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
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IVendor), binding, "");
                base.Open();
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
                base.ServiceHost.AddServiceEndpoint(typeof(Nequeo.Service.NequeoCompany.IVendor), binding, address);
                base.Open();
            }
        }

        /// <summary>
        /// Close the service host.
        /// </summary>
        public void CloseServiceHost()
        {
            base.Close();
        }
        #endregion
    }
}
