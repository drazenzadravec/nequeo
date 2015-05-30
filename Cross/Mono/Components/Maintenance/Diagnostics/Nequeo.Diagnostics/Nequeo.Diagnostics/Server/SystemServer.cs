/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.com.au/
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
using System.Web;
using System.Reflection;
using System.Threading;
using System.Management;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Win32;

namespace Nequeo.Diagnostics.Server
{
    /// <summary>
    /// System server.
    /// </summary>
    public class SystemServer
    {
        /// <summary>
        /// Get the server sessions performance counter for all disks.
        /// </summary>
        /// <returns>The number of sessions currently active in the server.
        /// Indicates current server activity.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ServerSessions()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Server", "Server Sessions");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the bytes received per second performance counter for all disks.
        /// </summary>
        /// <returns>The number of bytes the server has received from the network.
        /// Indicates how busy the server is.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter BytesReceivedPerSec()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Server", "Bytes Received/sec");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the bytes transmitted per second performance counter for all disks.
        /// </summary>
        /// <returns>The number of bytes the server has sent on the network.
        /// Indicates how busy the server is.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter BytesTransmittedPerSec()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Server", "Bytes Transmitted/sec");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the errors system performance counter for all disks.
        /// </summary>
        /// <returns>The number of times an internal Server Error was detected.  
        /// Unexpected errors usually indicate a problem with the Server.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ErrorsSystem()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Server", "Errors System");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the files open performance counter for all disks.
        /// </summary>
        /// <returns>The number of files currently opened in the server.  
        /// Indicates current server activity.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter FilesOpen()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Server", "Files Open");

            // Return the logical processor.
            return pc;
        }
    }
}
