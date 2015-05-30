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
using System.IO;
using System.Text;
using System.Security;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.ComponentModel;

namespace Nequeo.Runtime
{
    /// <summary>
    /// Service locator interface type.
    /// </summary>
    public interface IServiceLocator : IDisposable
    {
        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>The service type instance.</returns>
        TService Resolve<TService>();

        /// <summary>
        /// Registers the specified service as singleton.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="service">The service.</param>
        /// <returns>The service instance.</returns>
        IServiceLocator Register<TService>(TService service);

        /// <summary>
        /// Registers the specified factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <returns>The service instance.</returns>
        IServiceLocator Register<TService>(Func<IServiceLocator, object> factory);
    }

    /// <summary>
    /// Service locator for the current application domain.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Func<IServiceLocator> _defaultSingletonFactory =
            () => new ServiceLocatorRuntimeTypeHandle();

        private static readonly object _syncLock = new object();
        private static Func<IServiceLocator> _singletonFactory;
        private static IServiceLocator _singleton;

        /// <summary>
        /// Get of create the current singleton.
        /// </summary>
        public static IServiceLocator Current
        {
            get
            {
                if (_singleton == null)
                    lock (_syncLock)
                        if (_singleton == null)
                        {
                            // Create a new singleton for the current view.
                            _singleton = (_singletonFactory != null) ?
                                         _singletonFactory() : _defaultSingletonFactory();
                        }

                // Return the
                return _singleton;
            }
        }

        /// <summary>
        /// Set the current signleton service locator.
        /// </summary>
        /// <param name="factory">Executes the function call on the current service factory locator.</param>
        public static void SetCurrent(Func<IServiceLocator> factory)
        {
            // If the instance object is null.
            if (factory == null) throw new System.ArgumentNullException("factory");

            lock (_syncLock)
            {
                if (_singleton != null)
                {
                    IDisposable disposable = _singleton as IDisposable;

                    if (disposable != null)
                        disposable.Dispose();

                    _singleton = null;
                }

                // Return the current service factory.
                _singletonFactory = factory;
            }
        }
    }

    /// <summary>
    /// Default Service locator runtime type handler.
    /// </summary>
    public class ServiceLocatorRuntimeTypeHandle : DisposableBase, IServiceLocator
    {
        private readonly object _servicesSyncLock;
        private readonly IDictionary<RuntimeTypeHandle, object> _services;
        private readonly IDictionary<RuntimeTypeHandle, Func<IServiceLocator, object>> _factories;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ServiceLocatorRuntimeTypeHandle()
        {
            _services = new Dictionary<RuntimeTypeHandle, object>();
            _servicesSyncLock = new object();
            _factories = CreateDefaultFactories();
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>The service requested.</returns>
        public virtual TService Resolve<TService>()
        {
            RuntimeTypeHandle handle = typeof(TService).TypeHandle;
            object service;

            // If the service can not be found.
            if (!_services.TryGetValue(handle, out service))
            {
                lock (_servicesSyncLock)
                {
                    if (!_services.TryGetValue(handle, out service))
                    {
                        Func<IServiceLocator, object> factory;

                        // Find the service in the collection
                        if (_factories.TryGetValue(handle, out factory))
                        {
                            service = factory(this);
                            _services.Add(handle, service);
                        }
                    }
                }
            }

            // Return the current service in the application.
            return (TService)service;
        }

        /// <summary>
        /// Registers the specified service as singleton.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="service">The service.</param>
        /// <returns>Returns this current service locator.</returns>
        public virtual IServiceLocator Register<TService>(TService service)
        {
            // If the instance object is null.
            if (service == null)
                throw new System.ArgumentNullException("service");

            RuntimeTypeHandle handle = typeof(TService).TypeHandle;

            // Lock the curernt thread.
            lock (_servicesSyncLock)
            {
                object existing;

                // If found then
                if (_services.TryGetValue(handle, out existing))
                {
                    if (existing != null)
                    {
                        IDisposable disposable = existing as IDisposable;

                        if (disposable != null)
                        {
                            // Dispose of the object.
                            disposable.Dispose();
                        }
                    }
                }

                // Assign the new value for the current runtime type.
                _services[handle] = service;
            }

            return this;
        }

        /// <summary>
        /// Registers the specified factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <returns>Returns this current service locator.</returns>
        public virtual IServiceLocator Register<TService>(Func<IServiceLocator, object> factory)
        {
            // If the instance object is null.
            if (factory == null)
                throw new System.ArgumentNullException("factory");

            // For the current type assign the registry action.
            _factories[typeof(TService).TypeHandle] = factory;
            return this;
        }

        /// <summary>
        /// Disposes the resources.
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            lock (_servicesSyncLock)
            {
                // For each runtome type dispose of the service.
                foreach (KeyValuePair<RuntimeTypeHandle, object> pair in _services)
                {
                    if (pair.Value != null)
                    {
                        IDisposable disposable = pair.Value as IDisposable;

                        if (disposable != null)
                            // Dispose of the runtime type
                            disposable.Dispose();
                    }
                }

                _services.Clear();
            }
        }

        /// <summary>
        /// Create the default runtime types within the factory.
        /// </summary>
        /// <returns>The collection of runtime types and service instances.</returns>
        protected virtual IDictionary<RuntimeTypeHandle, Func<IServiceLocator, object>> CreateDefaultFactories()
        {
            IDictionary<RuntimeTypeHandle, Func<IServiceLocator, object>> defaultFactories = 
                new Dictionary<RuntimeTypeHandle, Func<IServiceLocator, object>>();

                /* Usage Examples
                 {
                     { typeof(RouteCollection).TypeHandle, locator => RouteTable.Routes },
                     { typeof(IVirtualPathProvider).TypeHandle, locator => new VirtualPathProviderWrapper() },
                     { typeof(IWebAssetLocator).TypeHandle, locator => new WebAssetLocator(debugMode, locator.Resolve<IVirtualPathProvider>()) },
                     { typeof(ScriptWrapperBase).TypeHandle, locator => new ScriptWrapper() }
                 };
                */

            // Return the collection of internal
            // common used types service factory.
            return defaultFactories;
        }
    }
}
