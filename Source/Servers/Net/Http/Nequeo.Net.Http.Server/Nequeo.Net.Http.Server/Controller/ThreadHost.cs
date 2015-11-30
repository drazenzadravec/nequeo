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
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Threading;

using Nequeo.Net.Configuration;

namespace Nequeo.Net.Controller
{
    /// <summary>
    /// Class that controls all custom server instances
    /// and threads.
    /// </summary>
    public class Control : IDisposable
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Control()
        {
        }

        private Nequeo.Net.Server.HttpHtmlProvider _httpHtmlProviderHost = null;
        private Nequeo.Net.Server.HttpXmlFeed _httpXmlFeedHost = null;
        private Nequeo.Net.Server.HttpHtmlAuthProvider _httpHtmlAuthProviderHost = null;
        private Nequeo.Net.Server.HttpXmlAuthFeed _httpXmlAuthFeedHost = null;
        private Nequeo.Net.Server.HttpBasicProvider _httpBasicProviderHost = null;

        private Thread _hostHtmlThread = null;
        private Thread _hostHtmlAuthThread = null;
        private Thread _hostXmlThread = null;
        private Thread _hostXmlAuthThread = null;
        private Thread _hostBasicThread = null;

        /// <summary>
        /// Initialse all objects, create new
        /// instances of all servers and cleanup
        /// objects when complete.
        /// </summary>
        /// <param name="create"></param>
        private void Initialise(bool create)
        {
            // Create new instances.
            if (create)
            {
                Configuration.ReaderHttp reader = new Configuration.ReaderHttp();

                _httpHtmlProviderHost = new Server.HttpHtmlProvider();
                _httpHtmlProviderHost.UriList = reader.GetProviderPathPrefix(_httpHtmlProviderHost.ProviderName);

                _httpHtmlAuthProviderHost = new Server.HttpHtmlAuthProvider();
                _httpHtmlAuthProviderHost.UriList = reader.GetProviderPathPrefix(_httpHtmlAuthProviderHost.ProviderName);

                _httpXmlFeedHost = new Server.HttpXmlFeed();
                _httpXmlFeedHost.UriList = reader.GetProviderPathPrefix(_httpXmlFeedHost.ProviderName);

                _httpXmlAuthFeedHost = new Server.HttpXmlAuthFeed();
                _httpXmlAuthFeedHost.UriList = reader.GetProviderPathPrefix(_httpXmlAuthFeedHost.ProviderName);

                _httpBasicProviderHost = new Server.HttpBasicProvider();
                _httpBasicProviderHost.UrlBaseAddress = reader.GetProviderPathPrefix(_httpBasicProviderHost.ProviderName).First();
                _httpBasicProviderHost.LocalBaseDirectory = ReaderHttp.GetBaseDirectoryPath().TrimEnd('\\');
            }
            else
            {
                // Clean up each host.
                if (_httpHtmlProviderHost != null)
                    _httpHtmlProviderHost = null;

                // Clean up each host.
                if (_httpHtmlAuthProviderHost != null)
                    _httpHtmlAuthProviderHost = null;

                // Clean up each host.
                if (_httpXmlFeedHost != null)
                    _httpXmlFeedHost = null;

                // Clean up each host.
                if (_httpXmlAuthFeedHost != null)
                    _httpXmlAuthFeedHost = null;

                // Clean up each host.
                if (_httpBasicProviderHost != null)
                    _httpBasicProviderHost = null;
            }
        }

        /// <summary>
        /// Starts all server threads.
        /// </summary>
        public void StartServerThreads()
        {
            // Initialise all custom server
            // instances.
            Initialise(true);

            // Create new threads for each
            // file transfer server.
            _hostHtmlThread = new Thread(new ThreadStart(_httpHtmlProviderHost.Start));
            _hostHtmlThread.IsBackground = true;
            _hostHtmlThread.Start();
            Thread.Sleep(20);

            // Create new threads for each
            // file transfer server.
            _hostHtmlAuthThread = new Thread(new ThreadStart(_httpHtmlAuthProviderHost.Start));
            _hostHtmlAuthThread.IsBackground = true;
            _hostHtmlAuthThread.Start();
            Thread.Sleep(20);

            // Create new threads for each
            // file transfer server.
            _hostXmlThread = new Thread(new ThreadStart(_httpXmlFeedHost.Start));
            _hostXmlThread.IsBackground = true;
            _hostXmlThread.Start();
            Thread.Sleep(20);

            // Create new threads for each
            // file transfer server.
            _hostXmlAuthThread = new Thread(new ThreadStart(_httpXmlAuthFeedHost.Start));
            _hostXmlAuthThread.IsBackground = true;
            _hostXmlAuthThread.Start();
            Thread.Sleep(20);

            // Create new threads for each
            // file transfer server.
            _hostBasicThread = new Thread(new ThreadStart(_httpBasicProviderHost.Start));
            _hostBasicThread.IsBackground = true;
            _hostBasicThread.Start();
            Thread.Sleep(20);
        }

        /// <summary>
        /// Stop all server from listening and
        /// abort all server threads.
        /// </summary>
        public void StopServerThreads()
        {
            // Stop all file transfer
            // servers from listening.
            if (_httpHtmlProviderHost != null)
                _httpHtmlProviderHost.Stop();

            // Stop all file transfer
            // servers from listening.
            if (_httpHtmlAuthProviderHost != null)
                _httpHtmlAuthProviderHost.Stop();

            // Stop all file transfer
            // servers from listening.
            if (_httpXmlFeedHost != null)
                _httpXmlFeedHost.Stop();

            // Stop all file transfer
            // servers from listening.
            if (_httpXmlAuthFeedHost != null)
                _httpXmlAuthFeedHost.Stop();

            // Stop all file transfer
            // servers from listening.
            if (_httpBasicProviderHost != null)
                _httpBasicProviderHost.Stop();

            // Abort all threads created
            // for file transfer instances.
            if (_hostHtmlThread != null)
                if (_hostHtmlThread.IsAlive)
                {
                    _hostHtmlThread.Abort();
                    _hostHtmlThread.Join();
                    Thread.Sleep(20);
                }

            // Abort all threads created
            // for file transfer instances.
            if (_hostHtmlAuthThread != null)
                if (_hostHtmlAuthThread.IsAlive)
                {
                    _hostHtmlAuthThread.Abort();
                    _hostHtmlAuthThread.Join();
                    Thread.Sleep(20);
                }

            // Abort all threads created
            // for file transfer instances.
            if (_hostXmlThread != null)
                if (_hostXmlThread.IsAlive)
                {
                    _hostXmlThread.Abort();
                    _hostXmlThread.Join();
                    Thread.Sleep(20);
                }

            // Abort all threads created
            // for file transfer instances.
            if (_hostXmlAuthThread != null)
                if (_hostXmlAuthThread.IsAlive)
                {
                    _hostXmlAuthThread.Abort();
                    _hostXmlAuthThread.Join();
                    Thread.Sleep(20);
                }

            // Abort all threads created
            // for file transfer instances.
            if (_hostBasicThread != null)
                if (_hostBasicThread.IsAlive)
                {
                    _hostBasicThread.Abort();
                    _hostBasicThread.Join();
                    Thread.Sleep(20);
                }

            // Clean up objects.
            Initialise(false);
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
                    if (_httpHtmlProviderHost != null)
                        _httpHtmlProviderHost.Dispose();

                    if (_httpXmlFeedHost != null)
                        _httpXmlFeedHost.Dispose();

                    if (_httpHtmlAuthProviderHost != null)
                        _httpHtmlAuthProviderHost.Dispose();

                    if (_httpXmlAuthFeedHost != null)
                        _httpXmlAuthFeedHost.Dispose();

                    if (_httpBasicProviderHost != null)
                        _httpBasicProviderHost.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _httpHtmlProviderHost = null;
                _httpXmlFeedHost = null;
                _httpHtmlAuthProviderHost = null;
                _httpXmlAuthFeedHost = null;
                _httpBasicProviderHost = null;

                _hostHtmlThread = null;
                _hostHtmlAuthThread = null;
                _hostXmlThread = null;
                _hostXmlAuthThread = null;
                _hostBasicThread = null;

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
        ~Control()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
