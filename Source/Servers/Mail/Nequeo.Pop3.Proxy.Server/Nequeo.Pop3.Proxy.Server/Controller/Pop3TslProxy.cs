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
using System.Threading;

using Nequeo.Net.Server;

namespace Nequeo.Net.Controller
{
    /// <summary>
    /// Class that controls all custom server instances
    /// and threads.
    /// </summary>
    public class Pop3TlsProxyControl
    {
        #region Transport layer secure Pop3 Controller
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Pop3TlsProxyControl()
        {
        }

        private Pop3TlsProxyServer ftHost1 = null;
        private Pop3TlsProxyServer ftHost2 = null;
        private Pop3TlsProxyServer ftHost3 = null;
        private Pop3TlsProxyServer ftHost4 = null;

        private Thread ftHostThread1 = null;
        private Thread ftHostThread2 = null;
        private Thread ftHostThread3 = null;
        private Thread ftHostThread4 = null;

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
                // Create a new proxy pop3 host
                // with default configuration setting.
                ftHost1 = new Pop3TlsProxyServer();
                ftHost1.Pop3ServerName = "Pop3TlsListenServer4";
                ftHost1.HostName = "TlsProxyNequeoNet5";

                // Create a new proxy pop3 host
                // and user the specified configuration
                // settings.
                ftHost2 = new Pop3TlsProxyServer();
                ftHost2.Pop3ServerName = "Pop3TlsListenServer5";
                ftHost2.HostName = "TlsProxyNequeoNet6";

                // Create a new proxy pop3 host
                // and user the specified configuration
                // settings.
                ftHost3 = new Pop3TlsProxyServer();
                ftHost3.Pop3ServerName = "Pop3TlsListenServer6";
                ftHost3.HostName = "TlsProxyNequeoNet7";

                // Create a new proxy pop3 host
                // and user the specified configuration
                // settings.
                ftHost4 = new Pop3TlsProxyServer();
                ftHost4.Pop3ServerName = "Pop3TlsListenServer7";
                ftHost4.HostName = "TlsProxyNequeoNet8";
            }
            else
            {
                // Dispose of all the servers.
                if (ftHost1 != null)
                    ftHost1.Dispose();

                if (ftHost2 != null)
                    ftHost2.Dispose();

                if (ftHost3 != null)
                    ftHost3.Dispose();

                if (ftHost4 != null)
                    ftHost4.Dispose();

                // Cleanup threads.
                ftHostThread1 = null;
                ftHostThread2 = null;
                ftHostThread3 = null;
                ftHostThread4 = null;
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
            ftHostThread1 = new Thread(new ThreadStart(ftHost1.StartListen));
            ftHostThread1.IsBackground = true;
            ftHostThread1.Start();
            Thread.Sleep(20);

            ftHostThread2 = new Thread(new ThreadStart(ftHost2.StartListen));
            ftHostThread2.IsBackground = true;
            ftHostThread2.Start();
            Thread.Sleep(20);

            ftHostThread3 = new Thread(new ThreadStart(ftHost3.StartListen));
            ftHostThread3.IsBackground = true;
            ftHostThread3.Start();
            Thread.Sleep(20);

            ftHostThread4 = new Thread(new ThreadStart(ftHost4.StartListen));
            ftHostThread4.IsBackground = true;
            ftHostThread4.Start();
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
            if (ftHost1 != null)
                ftHost1.StopListen();

            if (ftHost2 != null)
                ftHost2.StopListen();

            if (ftHost3 != null)
                ftHost3.StopListen();

            if (ftHost4 != null)
                ftHost4.StopListen();

            // Abort all threads created
            // for file transfer instances.
            if (ftHostThread1 != null)
                if (ftHostThread1.IsAlive)
                {
                    ftHostThread1.Abort();
                    ftHostThread1.Join();
                    Thread.Sleep(20);
                }

            if (ftHostThread2 != null)
                if (ftHostThread2.IsAlive)
                {
                    ftHostThread2.Abort();
                    ftHostThread2.Join();
                    Thread.Sleep(20);
                }

            if (ftHostThread3 != null)
                if (ftHostThread3.IsAlive)
                {
                    ftHostThread3.Abort();
                    ftHostThread3.Join();
                    Thread.Sleep(20);
                }

            if (ftHostThread4 != null)
                if (ftHostThread4.IsAlive)
                {
                    ftHostThread4.Abort();
                    ftHostThread4.Join();
                    Thread.Sleep(20);
                }

            // Clean up objects.
            Initialise(false);
        }
        #endregion
    }
}
