/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Threading.Tasks;

using Nequeo.Maintenance.File;

namespace Nequeo.Maintenance
{
    /// <summary>
    /// Class that controls all custom server instances
    /// and threads.
    /// </summary>
    public class LogFileControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LogFileControl()
        {
        }

        private LogFile _logFileHost1 = null;

        private Thread _logFileHostThread1 = null;

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
                // Create a new host
                // with default configuration setting.
                _logFileHost1 = new LogFile();
            }
            else
            {
                // Dispose of all the servers.
                if (_logFileHost1 != null)
                    _logFileHost1.Dispose();

                // Cleanup threads.
                _logFileHostThread1 = null;
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
            _logFileHostThread1 = new Thread(new ThreadStart(_logFileHost1.Start));
            _logFileHostThread1.IsBackground = true;
            _logFileHostThread1.Start();
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
            if (_logFileHost1 != null)
                _logFileHost1.Stop();

            // Abort all threads created
            // for file transfer instances.
            if (_logFileHostThread1 != null)
                if (_logFileHostThread1.IsAlive)
                {
                    _logFileHostThread1.Abort();
                    _logFileHostThread1.Join();
                    Thread.Sleep(20);
                }

            // Clean up objects.
            Initialise(false);
        }
    }
}
