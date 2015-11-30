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

using Nequeo.Net.Common;
using Nequeo.Handler;
using Nequeo.Net.Configuration;
using Nequeo.ComponentModel.Composition;
using Nequeo.Composite.Configuration;
using Nequeo.Net.Http;

namespace Nequeo.Net.Common
{
    /// <summary>
    /// Composition implementation handler
    /// </summary>
    [Logging(Handler.WriteTo.EventLog, Handler.LogType.Error)]
    internal class Composition : IDisposable
    {
        private CompositionContainer _container = null;

        /// <summary>
        /// Http server context interface
        /// </summary>
        [ImportMany]
        public IEnumerable<Lazy<IHttpServerContext, IContentMetadata>> HttpServerContext { get; set; }

        /// <summary>
        /// Load the composite http service list.
        /// </summary>
        /// <param name="create">Create the composition collection or release.</param>
        public void Compose(bool create = true)
        {
            if (create)
            {
                // Read from the configuration file
                // all the directories that contain
                // any composite services.
                Nequeo.Composite.Configuration.Reader reader = new Nequeo.Composite.Configuration.Reader();
                string[] paths = reader.GetServicePaths();

                AggregateCatalog catalog = new AggregateCatalog();

                // For each directory found search for composite server assemplies
                // and add the reference of the assemblies into the handler collection
                foreach (string path in paths)
                    catalog.Catalogs.Add(new DirectoryCatalog(path, "*HttpService.dll"));

                // Add the collection catalog to the composite container
                _container = new CompositionContainer(catalog);
                _container.ComposeParts(this);
            }
            else
            {
                // Release the exported composite services.
                _container.ReleaseExports<IHttpServerContext, IContentMetadata>(HttpServerContext);
                _container.Dispose();

                // Release the handle.
                HttpServerContext = null;
            }
        }

        /// <summary>
        /// Find the composite http server for the current service name.
        /// </summary>
        /// <param name="directories">The list of directories that contain the composite http service.</param>
        /// <param name="httpServiceExits">Has the http composite server been found.</param>
        /// <returns>The collection of cloned http composite servers.</returns>
        public Nequeo.Net.Http.IHttpServerContext[] FindCompositeContext(string[] directories, out bool httpServiceExits)
        {
            // Get the current value.
            List<Nequeo.Net.Http.IHttpServerContext> httpContextServers = new List<Nequeo.Net.Http.IHttpServerContext>();
            httpServiceExits = false;

            // For each message import in the collection.
            foreach (Lazy<Nequeo.Net.Http.IHttpServerContext, IContentMetadata> item in HttpServerContext)
            {
                // Get the current value.
                Nequeo.Net.Http.IHttpServerContext messageValue = item.Value;

                // Get the current metadata.
                IContentMetadata metadata = item.Metadata;

                // If the metadata name matches the request path.
                if (HttpServiceExistsInRequest(directories, metadata))
                {
                    // The http service has been found.
                    httpServiceExits = true;

                    // Clone the current http server context
                    // and add to the context collection.
                    Nequeo.Net.Http.IHttpServerContext cloneServer = (Nequeo.Net.Http.IHttpServerContext)messageValue.Clone();
                    httpContextServers.Add(cloneServer);
                }
            }

            // Return the http context.
            return httpContextServers.ToArray();
        }

        /// <summary>
        /// If there is a match between the current URL service request and the metadata service store.
        /// </summary>
        /// <param name="directories">The list of directories that contain the composite http service.</param>
        /// <param name="metadata">The current service metadata.</param>
        /// <returns>True if the http service has been found; else false.</returns>
        public bool HttpServiceExistsInRequest(string[] directories, IContentMetadata metadata)
        {
            // Search for the current metadata service name
            // within the URL directory list requested from
            // the current client.
            IEnumerable<string> httpServices = directories.Where(u => u.ToLower() == metadata.Name.ToLower());

            // Has a http service been found.
            if (httpServices != null)
                if (httpServices.Count() > 0)
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
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if(_container != null)
                        _container.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _container = null;

                // Note disposing has been done.
                disposed = true;
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
