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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Net;
using Nequeo.Net.Sockets;
using Nequeo.Security;
using Nequeo.Threading;
using Nequeo.Extension;
using Nequeo.Data.Provider;

namespace Nequeo.Client.Manage
{
    /// <summary>
    /// Communication service manager.
    /// </summary>
    public class Communication : IDisposable
    {
        /// <summary>
        /// Communication service manager.
        /// </summary>
        /// <param name="communicationClient">The communication client implementation.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Communication(ICommunication communicationClient)
        {
            if (communicationClient == null) throw new ArgumentNullException("communicationClient");

            _communicationClient = communicationClient;
        }

        private ICommunication _communicationClient = null;

        /// <summary>
        /// Get the communication service URLs used for connection.
        /// </summary>
        /// <param name="type">The host type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        public void GetServiceURLs(string type, Action<string[], object> callback)
        {
            // Get the number of active connections
            // and the service URLs.
            _communicationClient.GetManageURLs(type, callback);
        }

        /// <summary>
        /// Gets the service URL after determining the appropriate server to connect to.
        /// </summary>
        /// <param name="type">The host type.</param>
        /// <param name="callback">The callback handler when a result is retured, a client web response.</param>
        public void GetServiceURL(string type, Action<string, string[]> callback)
        {
            // Get the list of service urls.
            GetServiceURLs(type, (responseURls, state) =>
            {
                string url = null;
                string[] urls = responseURls;

                // Make sure data exists.
                if (urls != null)
                {
                    // If urls exist.
                    if (urls.Length > 0)
                    {
                        // Randomly select the service index.
                        Nequeo.Invention.NumberGenerator number = new Invention.NumberGenerator();
                        string random = number.Random(1, urls.Length);

                        // Return the url.
                        url = urls[Int32.Parse(random)];
                    }
                    else
                        url = null;
                }
                else
                    url = null;

                // Send the callback.
                if (callback != null)
                    callback(url, urls);
            });
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
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
				// Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _communicationClient = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Communication()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
