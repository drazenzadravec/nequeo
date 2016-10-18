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

namespace Nequeo.Service.NequeoCompany
{
    /// <summary>
    /// Service locator for the current.
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// Get of create the current singleton for the Mvc view and the current http context
        /// </summary>
        public static Nequeo.Runtime.IServiceLocator Current
        {
            get { return Nequeo.Runtime.ServiceLocator.Current; }
        }

        /// <summary>
        /// Register the current service locator services.
        /// </summary>
        public static void RegisterCurrent()
        {
            // Get the current service locator singleton instance.
            Nequeo.Runtime.IServiceLocator locator = Nequeo.Service.NequeoCompany.ServiceLocator.Current;
            Nequeo.Service.NequeoCompany.ServiceLocator.Register(locator);
        }

        /// <summary>
        /// Register the initial services.
        /// </summary>
        /// <param name="serviceLocator">The current singleton service locator.</param>
        internal static void Register(Nequeo.Runtime.IServiceLocator serviceLocator)
        {
            serviceLocator.Register<IEmployee>(locator => new Nequeo.Service.NequeoCompany.Employee());
            serviceLocator.Register<ICustomer>(locator => new Nequeo.Service.NequeoCompany.Customer());
            serviceLocator.Register<IAccount>(locator => new Nequeo.Service.NequeoCompany.Account());
            serviceLocator.Register<ICompany>(locator => new Nequeo.Service.NequeoCompany.Company());
            serviceLocator.Register<IAsset>(locator => new Nequeo.Service.NequeoCompany.Asset());
            serviceLocator.Register<IProduct>(locator => new Nequeo.Service.NequeoCompany.Product());
            serviceLocator.Register<IVendor>(locator => new Nequeo.Service.NequeoCompany.Vendor());
            serviceLocator.Register<IUser>(locator => new Nequeo.Service.NequeoCompany.User());
            serviceLocator.Register<IManage>(locator => new Nequeo.Service.NequeoCompany.Manage());
        }
    }
}
