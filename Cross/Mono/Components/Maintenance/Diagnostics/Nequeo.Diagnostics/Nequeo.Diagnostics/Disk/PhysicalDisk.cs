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
    /// Physical disk.
    /// </summary>
    public class PhysicalDisk
    {
        /// <summary>
        /// Get the total percent disk read time performance counter for all disks.
        /// </summary>
        /// <returns>Percent Disk Read Time is the percentage of elapsed time that the selected disk drive was busy servicing read requests.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalDiskReadTime()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("PhysicalDisk", "% Disk Read Time", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total percent disk write time performance counter for all disks.
        /// </summary>
        /// <returns>Percent Disk Write Time is the percentage of elapsed time that the selected disk drive was busy servicing write requests.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalDiskWriteTime()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("PhysicalDisk", "% Disk Write Time", "_Total");

            // Return the logical processor.
            return pc;
        }


        /// <summary>
        /// Get the total percent disk read time performance counter for all disks.
        /// </summary>
        /// <param name="diskVolumeName">The disk volume name.</param>
        /// <returns>Percent Disk Read Time is the percentage of elapsed time that the selected disk drive was busy servicing read requests.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalDiskReadTime(string diskVolumeName = "1 C:")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("PhysicalDisk", "% Disk Read Time", diskVolumeName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total percent disk write time performance counter for all disks.
        /// </summary>
        /// <param name="diskVolumeName">The disk volume name.</param>
        /// <returns>Percent Disk Write Time is the percentage of elapsed time that the selected disk drive was busy servicing write requests.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalDiskWriteTime(string diskVolumeName = "1 C:")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("PhysicalDisk", "% Disk Write Time", diskVolumeName);

            // Return the logical processor.
            return pc;
        }
    }
}
