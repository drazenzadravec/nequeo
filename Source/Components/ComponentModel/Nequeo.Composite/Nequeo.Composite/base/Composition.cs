/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using Nequeo.ComponentModel.Composition;

namespace Nequeo.Composite
{
    /// <summary>
    /// Composition clone implementation handler.
    /// </summary>
    /// <typeparam name="ICT">The composite export interface.</typeparam>
    /// <typeparam name="IMT">The composite metadata interface.</typeparam>
    public class CompositionClone<ICT, IMT> : Composition<ICT, IMT>
        where ICT : ICloneable
        where IMT : IContentMetadata
    {
        /// <summary>
        /// Find the composite instance for the current service name.
        /// </summary>
        /// <param name="name">The name of the composite instance.</param>
        /// <param name="serviceExits">Has the composite service been found.</param>
        /// <returns>The composite service.</returns>
        public override ICT FindCompositeContext(string name, out bool serviceExits)
        {
            // Get the current value.
            ICT cloneServer = default(ICT);
            serviceExits = false;

            // For each message import in the collection.
            foreach (Lazy<ICT, IMT> item in CompositionContext)
            {
                // Get the current value.
                ICT messageValue = item.Value;

                // Get the current metadata.
                IMT metadata = item.Metadata;

                // If the name of the service matches.
                if (metadata.Name.ToLower() == name.ToLower())
                {
                    serviceExits = true;

                    // Clone the current service.
                    cloneServer = (ICT)messageValue.Clone();
                    break;
                }
            }

            // Return the service.
            return cloneServer;
        }

        /// <summary>
        /// Find the composite instance for the current service name.
        /// </summary>
        /// <param name="directories">The list of directories that contain the composite instance.</param>
        /// <param name="serviceExits">Has the composite service been found.</param>
        /// <returns>The collection of composite services.</returns>
        public override ICT[] FindCompositeContext(string[] directories, out bool serviceExits)
        {
            // Get the current value.
            List<ICT> contextServers = new List<ICT>();
            serviceExits = false;

            // For each message import in the collection.
            foreach (Lazy<ICT, IMT> item in CompositionContext)
            {
                // Get the current value.
                ICT messageValue = item.Value;

                // Get the current metadata.
                IMT metadata = item.Metadata;

                // If the metadata name matches the request path.
                if (ServiceExistsInRequest(directories, metadata))
                {
                    // The http service has been found.
                    serviceExits = true;

                    // Clone the current http server context
                    // and add to the context collection.
                    ICT cloneServer = (ICT)messageValue.Clone();
                    contextServers.Add(cloneServer);
                }
            }

            // Return the http context.
            return contextServers.ToArray();
        }
    }

    /// <summary>
    /// Composition implementation handler.
    /// </summary>
    /// <typeparam name="ICT">The composite export interface.</typeparam>
    /// <typeparam name="IMT">The composite metadata interface.</typeparam>
    public class Composition<ICT, IMT> : IDisposable
        where IMT : IContentMetadata 
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Composition()
        {
            _catalog = new AggregateCatalog();
        }

        private CompositionContainer _container = null;
        private AggregateCatalog _catalog = null;

        /// <summary>
        /// Composition context interface
        /// </summary>
        [ImportMany]
        public IEnumerable<Lazy<ICT, IMT>> CompositionContext { get; set; }

        /// <summary>
        /// Compose the catalog items, loads the types to the CompositionContext.
        /// </summary>
        public void Compose()
        {
            // Add the collection catalog to the composite container
            _container = new CompositionContainer(_catalog);
            _container.ComposeParts(this);
        }

        /// <summary>
        /// Add the composite service.
        /// </summary>
        /// <param name="paths">The path to the directory to scan for assemblies to add to the catalog.The
        ///  path must be absolute or relative to System.AppDomain.BaseDirectory.</param>
        /// <param name="searchPattern">The search string. The format of the string should be the same as specified
        ///  for the System.IO.Directory.GetFiles(System.String,System.String) method.</param>
        public void AddCatalogItem(string[] paths, string searchPattern)
        {
            // For each directory found search for composite server assemplies
            // and add the reference of the assemblies into the handler collection
            foreach (string path in paths)
                _catalog.Catalogs.Add(new DirectoryCatalog(path, searchPattern));
        }

        /// <summary>
        /// Add the composite service.
        /// </summary>
        /// <param name="types">An array of attributed System.Type objects to add to the 
        /// System.ComponentModel.Composition.Hosting.TypeCatalog object.</param>
        public void AddCatalogItem(params Type[] types)
        {
            // Add the type collection catalog.
            _catalog.Catalogs.Add(new TypeCatalog(types));
        }

        /// <summary>
        /// Add the composite service.
        /// </summary>
        /// <param name="types">A collection of attributed System.Type objects to add to the 
        /// System.ComponentModel.Composition.Hosting.TypeCatalog object.</param>
        public void AddCatalogItem(IEnumerable<Type> types)
        {
            // Add the type collection catalog.
            _catalog.Catalogs.Add(new TypeCatalog(types));
        }

        /// <summary>
        /// Removes a collection of exports from composition and releases their resources if possible.
        /// </summary>
        public void Release()
        {
            try
            {
                // Release the exported composite services.
                _container.ReleaseExports<ICT, IMT>(CompositionContext);

                // Release the handle.
                CompositionContext = null;
            }
            catch { }
        }

        /// <summary>
        /// Find the composite instance for the current service name.
        /// </summary>
        /// <param name="name">The name of the composite instance.</param>
        /// <param name="serviceExits">Has the composite service been found.</param>
        /// <returns>The composite service.</returns>
        public virtual ICT FindCompositeContext(string name, out bool serviceExits)
        {
            // Get the current value.
            ICT messageValue = default(ICT);
            serviceExits = false;

            // For each message import in the collection.
            foreach (Lazy<ICT, IMT> item in CompositionContext)
            {
                // Get the current metadata.
                IMT metadata = item.Metadata;

                // If the name of the service matches.
                if (metadata.Name.ToLower() == name.ToLower())
                {
                    messageValue = item.Value;
                    serviceExits = true;
                    break;
                }
            }

            // Return the service.
            return messageValue;
        }

        /// <summary>
        /// Find the composite instance for the current service name.
        /// </summary>
        /// <param name="directories">The list of directories that contain the composite instance.</param>
        /// <param name="serviceExits">Has the composite service been found.</param>
        /// <returns>The collection of composite services.</returns>
        public virtual ICT[] FindCompositeContext(string[] directories, out bool serviceExits)
        {
            // Get the current value.
            List<ICT> contextServers = new List<ICT>();
            serviceExits = false;

            // For each message import in the collection.
            foreach (Lazy<ICT, IMT> item in CompositionContext)
            {
                // Get the current value.
                ICT messageValue = item.Value;

                // Get the current metadata.
                IMT metadata = item.Metadata;

                // If the metadata name matches the request path.
                if (ServiceExistsInRequest(directories, metadata))
                {
                    // The http service has been found.
                    serviceExits = true;

                    // Add to the context collection.
                    contextServers.Add(messageValue);
                }
            }

            // Return the http context.
            return contextServers.ToArray();
        }

        /// <summary>
        /// If there is a match between the current service request and the metadata service store.
        /// </summary>
        /// <param name="directories">The list of directories that contain the composite service.</param>
        /// <param name="metadata">The current service metadata.</param>
        /// <returns>True if the service has been found; else false.</returns>
        public virtual bool ServiceExistsInRequest(string[] directories, IMT metadata)
        {
            // Search for the current metadata service name
            // within the URL directory list requested from
            // the current client.
            IEnumerable<string> services = directories.Where(u => u.ToLower() == metadata.Name.ToLower());

            // Has a http service been found.
            if (services != null)
                if (services.Count() > 0)
                    // A http service match has been found.
                    return true;

            // Return false no http service match found.
            return false;
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

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
            if (!this.disposed)
            {
                // Note disposing has been done.
                disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_container != null)
                        _container.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _container = null;

                // Release the handle.
                CompositionContext = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Composition()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
