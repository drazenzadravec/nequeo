/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Runtime.Remoting;
using System.Globalization;

namespace Nequeo.Web.Hosting
{
    /// <summary>
    ///  ASP.NET page host.
    /// </summary>
    public class Host : MarshalByRefObject
    {
        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="page">The page to be requested (or the virtual path to the page, relative to the application directory).</param>
        /// <param name="output">A System.IO.TextWriter that captures output from the response.</param>
        /// <param name="query">The text of the query string.</param>
        /// <param name="processDefaultHttpHeaders">True if the http runtime process request is executed; else false.</param>
        public virtual void ProcessRequest(string page, TextWriter output, string query = null, bool processDefaultHttpHeaders = true)
        {
            // Provides a simple implementation of the System.Web.HttpWorkerRequest abstract
            // class that can be used to host ASP.NET applications outside an Internet Information
            // Services (IIS) application. You can employ SimpleWorkerRequest directly or
            // extend it.
            SimpleWorkerRequest swr = new SimpleWorkerRequest(page, query, output);

            // If headers should be procesed.
            if(processDefaultHttpHeaders)
                // Drives all ASP.NET Web processing execution.
                HttpRuntime.ProcessRequest(swr);

            // Clenup.
            swr = null;
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="virtualDir">The virtual path to the application directory; for example, "/app".</param>
        /// <param name="physicalDir">The physical path to the application directory; for example, "c:\app".</param>
        /// <param name="page">The page to be requested (or the virtual path to the page, relative to the application directory).</param>
        /// <param name="output">A System.IO.TextWriter that captures output from the response.</param>
        /// <param name="query">The text of the query string.</param>
        /// <param name="processDefaultHttpHeaders">True if the http runtime process request is executed; else false.</param>
        public virtual void ProcessRequest(string virtualDir, string physicalDir, 
            string page, TextWriter output, string query = null, bool processDefaultHttpHeaders = true)
        {
            // Provides a simple implementation of the System.Web.HttpWorkerRequest abstract
            // class that can be used to host ASP.NET applications outside an Internet Information
            // Services (IIS) application. You can employ SimpleWorkerRequest directly or
            // extend it.
            SimpleWorkerRequest swr = new SimpleWorkerRequest(virtualDir, physicalDir, page, query, output);

            // If headers should be procesed.
            if (processDefaultHttpHeaders)
                // Drives all ASP.NET Web processing execution.
                HttpRuntime.ProcessRequest(swr);

            // Clenup.
            swr = null;
        }
    }

    /// <summary>
    /// Enables hosting of ASP.NET pages outside the Internet Information Services (IIS) application. 
    /// This class enables the host to create application domains for processing ASP.NET requests.
    /// </summary>
    public class ApplicationHost : IDisposable
    {
        /// <summary>
        /// Enables hosting of ASP.NET pages outside the Internet Information Services (IIS) application. 
        /// This class enables the host to create application domains for processing ASP.NET requests.
        /// </summary>
        public ApplicationHost(){}

        /// <summary>
        /// Creates and configures an application domain for hosting ASP.NET.
        /// </summary>
        /// <param name="host">The name of a user-supplied class to be created in the new application domain.</param>
        /// <param name="virtualDir">The virtual path to the application directory; for example, "/app".</param>
        /// <param name="physicalDir">The physical path to the application directory; for example, "c:\app".</param>
        /// <param name="configurationFile">The application configuration file (e.g. "web.config").</param>
        /// <returns>An instance of a user-supplied class used to marshal calls into the newly created application domain.</returns>
        public virtual object CreateApplicationHost(Type host, string virtualDir, string physicalDir, string configurationFile = null)
        {
            // If using a configuration file.
            if (configurationFile != null)
            {
                if (!(physicalDir.EndsWith("\\")))
                    physicalDir = physicalDir + "\\";

                // Assign the application domian values.
                string aspDir = HttpRuntime.AspInstallDirectory;
                string domainId = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo).GetHashCode().ToString("x");
                string appName = (virtualDir + physicalDir).GetHashCode().ToString("x");

                // Assign the domain setup
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationName = appName;
                setup.ConfigurationFile = configurationFile;

                // Create the application domain.
                AppDomain ad = AppDomain.CreateDomain(domainId, null, setup);
                ad.SetData(".appDomain", "*");
                ad.SetData(".appPath", physicalDir);
                ad.SetData(".appVPath", virtualDir);
                ad.SetData(".domainId", domainId);
                ad.SetData(".hostingVirtualPath", virtualDir);
                ad.SetData(".hostingInstallDir", aspDir);

                // Return the application host.
                ObjectHandle oh = ad.CreateInstance(host.Module.Assembly.FullName, host.FullName);
                return oh.Unwrap();
            }
            else
            {
                // Return the application host object.
                return System.Web.Hosting.ApplicationHost.CreateApplicationHost(host, virtualDir, physicalDir);
            }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ApplicationHost()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Application provider.
    /// </summary>
    public class Application : IDisposable
    {
        /// <summary>
        /// Application provider.
        /// </summary>
        public Application() { }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="virtualDir">The virtual path to the application directory; for example, "/app".</param>
        /// <param name="physicalDir">The physical path to the application directory; for example, "c:\app".</param>
        /// <param name="page">The page to be requested (or the virtual path to the page, relative to the application directory).</param>
        /// <param name="output">A System.IO.TextWriter that captures output from the response.</param>
        /// <param name="query">The text of the query string.</param>
        /// <param name="processDefaultHttpHeaders">True if the http runtime process request is executed; else false.</param>
        /// <param name="configurationFile">The application configuration file (e.g. "web.config").</param>
        public virtual void ProcessRequest(string virtualDir, string physicalDir, string page, TextWriter output, 
            string query = null, bool processDefaultHttpHeaders = true, string configurationFile = null)
        {
            // Execute the request.
            using (ApplicationHost applicationHost = new ApplicationHost())
            {
                Nequeo.Web.Hosting.Host host = (Nequeo.Web.Hosting.Host)applicationHost.CreateApplicationHost(typeof(Nequeo.Web.Hosting.Host), virtualDir, physicalDir, configurationFile);
                host.ProcessRequest(page, output, query, processDefaultHttpHeaders);
            }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
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
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Application()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
