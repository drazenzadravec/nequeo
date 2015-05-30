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
using System.ServiceModel.Activation;
using System.Web;

using Nequeo.Net.ServiceModel.Configuration;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Create a custom service factory to handle mutiple addresses
    /// on a windows server machine.
    /// Factory that provides instances of System.ServiceModel.ServiceHost in managed
    /// hosting environments where the host instance is created dynamically in response
    /// to incoming messages.
    /// </summary>
    public class MultipleHttpSchemaServiceFactory : ServiceHostFactory
    {
        /// <summary>
        /// Create a new service host from the specified base address.
        /// </summary>
        /// <param name="serviceType">The current service type.</param>
        /// <param name="baseAddresses">The collection of base addresses.</param>
        /// <returns>The selected service host</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHostExtensionElement[] items = ServiceHostConfigurationManager.ServiceHostExtensionElements();
            if (items != null)
            {
                if (items.Count() > 0)
                {
                    // For each service host  configuration find
                    // the corresponding service type.
                    foreach (ServiceHostExtensionElement item in items)
                    {
                        // Get the current type name
                        // and create a instance of the type.
                        Type typeName = Type.GetType(item.ServiceTypeName, true, true);
                        object typeNameInstance = Activator.CreateInstance(typeName);

                        // If the current service type is equal
                        // to the service type name.
                        if (serviceType.FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
                            return new ServiceHost(serviceType, baseAddresses[item.BaseAddressIndex]);
                    }

                    // Choose the first address.
                    return new ServiceHost(serviceType, baseAddresses[0]);
                }
            }

            // Chosen is the first multi-pint address on the server, that is.
            // wwww.nequeo.com.au   port 80   (0)
            // nequeo.net.au        port 80   (1)
            return new ServiceHost(serviceType, baseAddresses[0]);
        }
    }

    /// <summary>
    /// Create a custom service factory to handle mutiple addresses
    /// on a windows server machine.
    /// Factory that provides instances of System.ServiceModel.ServiceHost in managed
    /// hosting environments where the host instance is created dynamically in response
    /// to incoming messages.
    /// </summary>
    public class AllBindingsHttpSchemaServiceFactory : ServiceHostFactory
    {
        /// <summary>
        /// Create a new service host from the specified base address.
        /// </summary>
        /// <param name="serviceType">The current service type.</param>
        /// <param name="baseAddresses">The collection of base addresses.</param>
        /// <returns>The selected service host</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            // Chosen is the first multi-pint address on the server, that is.
            // wwww.nequeo.com.au   port 80   (0)
            // nequeo.net.au        port 80   (1)
            return new ServiceHost(serviceType, baseAddresses);
        }
    }

    /// <summary>
    /// Create a custom service factory to handle mutiple addresses
    /// on a windows server machine.
    /// Automatically adds an ASP.NET AJAX endpoint to a service, without requiring 
    /// configuration, in a managed hosting environment that dynamically activates host 
    /// instances for the service in response to incoming messages.
    /// </summary>
    public class MultipleHttpSchemaWebScriptServiceFactory : WebScriptServiceHostFactory
    {
        /// <summary>
        /// Create a new service host from the specified base address.
        /// </summary>
        /// <param name="serviceType">The current service type.</param>
        /// <param name="baseAddresses">The collection of base addresses.</param>
        /// <returns>The selected service host</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHostExtensionElement[] items = ServiceHostConfigurationManager.ServiceHostExtensionElements();
            if (items != null)
            {
                if (items.Count() > 0)
                {
                    // For each service host  configuration find
                    // the corresponding service type.
                    foreach (ServiceHostExtensionElement item in items)
                    {
                        // Get the current type name
                        // and create a instance of the type.
                        Type typeName = Type.GetType(item.ServiceTypeName, true, true);
                        object typeNameInstance = Activator.CreateInstance(typeName);

                        // If the current service type is equal
                        // to the service type name.
                        if (serviceType.FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
                            return new ServiceHost(serviceType, baseAddresses[item.BaseAddressIndex]);
                    }

                    // Choose the first address.
                    return new ServiceHost(serviceType, baseAddresses[0]);
                }
            }

            // Chosen is the first multi-pint address on the server, that is.
            // wwww.nequeo.com.au   port 80   (0)
            // nequeo.net.au        port 80   (1)
            return new ServiceHost(serviceType, baseAddresses[0]);
        }
    }

    /// <summary>
    /// Create a custom service factory to handle mutiple addresses
    /// on a windows server machine.
    /// Automatically adds an ASP.NET AJAX endpoint to a service, without requiring 
    /// configuration, in a managed hosting environment that dynamically activates host 
    /// instances for the service in response to incoming messages.
    /// </summary>
    public class AllBindingsHttpSchemaWebScriptServiceFactory : WebScriptServiceHostFactory
    {
        /// <summary>
        /// Create a new service host from the specified base address.
        /// </summary>
        /// <param name="serviceType">The current service type.</param>
        /// <param name="baseAddresses">The collection of base addresses.</param>
        /// <returns>The selected service host</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            // Chosen is the first multi-pint address on the server, that is.
            // wwww.nequeo.com.au   port 80   (0)
            // nequeo.net.au        port 80   (1)
            return new ServiceHost(serviceType, baseAddresses);
        }
    }

    /// <summary>
    /// Create a custom service factory to handle mutiple addresses
    /// on a windows server machine.
    /// A factory that provides instances of WebServiceHost in managed 
    /// hosting environments where the host instance is created dynamically 
    /// in response to incoming messages.
    /// </summary>
    public class MultipleHttpSchemaWebServiceFactory : WebServiceHostFactory
    {
        /// <summary>
        /// Create a new service host from the specified base address.
        /// </summary>
        /// <param name="serviceType">The current service type.</param>
        /// <param name="baseAddresses">The collection of base addresses.</param>
        /// <returns>The selected service host</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHostExtensionElement[] items = ServiceHostConfigurationManager.ServiceHostExtensionElements();
            if (items != null)
            {
                if (items.Count() > 0)
                {
                    // For each service host  configuration find
                    // the corresponding service type.
                    foreach (ServiceHostExtensionElement item in items)
                    {
                        // Get the current type name
                        // and create a instance of the type.
                        Type typeName = Type.GetType(item.ServiceTypeName, true, true);
                        object typeNameInstance = Activator.CreateInstance(typeName);

                        // If the current service type is equal
                        // to the service type name.
                        if (serviceType.FullName.ToLower() == typeNameInstance.GetType().FullName.ToLower())
                            return new ServiceHost(serviceType, baseAddresses[item.BaseAddressIndex]);
                    }

                    // Choose the first address.
                    return new ServiceHost(serviceType, baseAddresses[0]);
                }
            }

            // Chosen is the first multi-pint address on the server, that is.
            // wwww.nequeo.com.au   port 80   (0)
            // nequeo.net.au        port 80   (1)
            return new ServiceHost(serviceType, baseAddresses[0]);
        }
    }

    /// <summary>
    /// Create a custom service factory to handle mutiple addresses
    /// on a windows server machine.
    /// A factory that provides instances of WebServiceHost in managed 
    /// hosting environments where the host instance is created dynamically 
    /// in response to incoming messages.
    /// </summary>
    public class AllBindingsHttpSchemaWebServiceFactory : WebServiceHostFactory
    {
        /// <summary>
        /// Create a new service host from the specified base address.
        /// </summary>
        /// <param name="serviceType">The current service type.</param>
        /// <param name="baseAddresses">The collection of base addresses.</param>
        /// <returns>The selected service host</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            // Chosen is the first multi-pint address on the server, that is.
            // wwww.nequeo.com.au   port 80   (0)
            // nequeo.net.au        port 80   (1)
            return new ServiceHost(serviceType, baseAddresses);
        }
    }
}
