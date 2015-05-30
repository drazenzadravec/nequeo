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

namespace Nequeo.Diagnostics.Network
{
    /// <summary>
    /// Network interface.
    /// </summary>
    public class NetworkInterface
    {
        /// <summary>
        /// Get the bytes received per second performance counter for all disks.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>Bytes Received/sec is the rate at which bytes are received over each network adapter, 
        /// including framing characters. Network Interface\Bytes Received/sec is a subset of Network Interface\Bytes Total/sec.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter BytesReceivedPerSec(string networkInterfaceName)
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Network Interface", "Bytes Received/sec", networkInterfaceName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the bytes sent per second performance counter for all disks.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>Bytes Sent/sec is the rate at which bytes are sent over each network adapter, including 
        /// framing characters. Network Interface\Bytes Sent/sec is a subset of Network Interface\Bytes Total/sec.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter BytesSentPerSec(string networkInterfaceName)
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Network Interface", "Bytes Sent/sec", networkInterfaceName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the current bandwidth performance counter for all disks.
        /// </summary>
        /// <param name="networkInterfaceName">The network interface name.</param>
        /// <returns>Current Bandwidth is an estimate of the current bandwidth of the network interface in bits per second (BPS).  
        /// For interfaces that do not vary in bandwidth or for those where no accurate estimation can be made, this value is the nominal bandwidth.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter CurrentBandwidth(string networkInterfaceName)
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Network Interface", "Current Bandwidth", networkInterfaceName);

            // Return the logical processor.
            return pc;
        }
    }
}
