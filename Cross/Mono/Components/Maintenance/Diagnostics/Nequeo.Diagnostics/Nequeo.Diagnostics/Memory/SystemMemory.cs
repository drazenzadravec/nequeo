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

namespace Nequeo.Diagnostics.Memory
{
    /// <summary>
    /// System memory.
    /// </summary>
    public class SystemMemory
    {
        /// <summary>
        /// Get the available mbytes performance counter for all disks.
        /// </summary>
        /// <returns>Available MBytes is the amount of physical memory, in Megabytes, 
        /// immediately available for allocation to a process or for system use. 
        /// It is equal to the sum of memory assigned to the standby (cached), 
        /// free and zero page lists. For a full explanation of the memory manager.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter AvailableMBytes()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Memory", "Available MBytes");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the committed bytes performance counter for all disks.
        /// </summary>
        /// <returns>Committed Bytes is the amount of committed virtual memory, in bytes. 
        /// Committed memory is the physical memory which has space reserved on the disk paging file(s).
        /// There can be one or more paging files on each physical drive. This counter displays the last 
        /// observed value only; it is not an average.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter CommittedBytes()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Memory", "Committed Bytes");

            // Return the logical processor.
            return pc;
        }
    }
}
