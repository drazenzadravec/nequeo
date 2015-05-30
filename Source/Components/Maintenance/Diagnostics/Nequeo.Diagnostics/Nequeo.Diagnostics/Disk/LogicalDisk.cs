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

namespace Nequeo.Diagnostics.Disk
{
    /// <summary>
    /// Logical disk.
    /// </summary>
    public class LogicalDisk
    {
        /// <summary>
        /// Get the total percent free space performance counter for all disks.
        /// </summary>
        /// <returns>Percent Free Space is the percentage of total usable space on the selected logical disk drive that was free.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalFreeSpace()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("LogicalDisk", "% Free Space", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total percent free space performance counter for all disks.
        /// </summary>
        /// <param name="diskVolumeName">The disk volume name.</param>
        /// <returns>Percent Free Space is the percentage of total usable space on the selected logical disk drive that was free.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter FreeSpace(string diskVolumeName = "C:")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("LogicalDisk", "% Free Space", diskVolumeName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total free megabytes performance counter for all disks.
        /// </summary>
        /// <returns>Free Megabytes displays the unallocated space, in megabytes, on the disk drive in megabytes. One megabyte is equal to 1,048,576 bytes.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalFreeMegabytes()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("LogicalDisk", "Free Megabytes", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total free megabytes performance counter for all disks.
        /// </summary>
        /// <param name="diskVolumeName">The disk volume name.</param>
        /// <returns>Free Megabytes displays the unallocated space, in megabytes, on the disk drive in megabytes. One megabyte is equal to 1,048,576 bytes.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter FreeMegabytes(string diskVolumeName = "C:")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("LogicalDisk", "Free Megabytes", diskVolumeName);

            // Return the logical processor.
            return pc;
        }
    }
}
