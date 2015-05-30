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

namespace Nequeo.Diagnostics.SystemEx
{
    /// <summary>
    /// System usage.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// Get the context switches per second performance counter for all disks.
        /// </summary>
        /// <returns>Context Switches/sec is the combined rate at which all processors on the 
        /// computer are switched from one thread to another.  Context switches occur when a 
        /// running thread voluntarily relinquishes the processor, is preempted by a higher 
        /// priority ready thread, or switches between user-mode and privileged (kernel) mode 
        /// to use an Executive or subsystem service.  It is the sum of Thread\\Context Switches/sec 
        /// for all threads running on all processors in the computer and is measured in numbers 
        /// of switches.  There are context switch counters on the System and Thread objects. 
        /// This counter displays the difference between the values observed in the last two samples, 
        /// divided by the duration of the sample interval.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ContextSwitchesPerSec()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("System", "Context Switches/sec");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the exception dispatches per second performance counter for all disks.
        /// </summary>
        /// <returns>Exception Dispatches/sec is the rate, in incidents per second, 
        /// at which exceptions were dispatched by the system.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ExceptionDispatchesPerSec()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("System", "Exception Dispatches/sec");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the file read bytes per second performance counter for all disks.
        /// </summary>
        /// <returns>File Read Bytes/sec is the overall rate at which bytes are read to satisfy 
        /// file system read requests to all devices on the computer, including reads from the 
        /// file system cache.  It is measured in number of bytes per second.  This counter 
        /// displays the difference between the values observed in the last two samples, 
        /// divided by the duration of the sample interval.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter FileReadBytesPerSec()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("System", "File Read Bytes/sec");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the file write bytes per second performance counter for all disks.
        /// </summary>
        /// <returns>File Write Bytes/sec is the overall rate at which bytes are written 
        /// to satisfy file system write requests to all devices on the computer, 
        /// including writes to the file system cache.  It is measured in number of bytes 
        /// per second.  This counter displays the difference between the values observed 
        /// in the last two samples, divided by the duration of the sample interval.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter FileWriteBytesPerSec()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("System", "File Write Bytes/sec");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the processes performance counter for all disks.
        /// </summary>
        /// <returns>Processes is the number of processes in the computer at the time of data 
        /// collection. This is an instantaneous count, not an average over the time interval.  
        /// Each process represents the running of a program.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter Processes()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("System", "Processes");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the system up time performance counter for all disks.
        /// </summary>
        /// <returns>System Up Time is the elapsed time (in seconds) that the computer has 
        /// been running since it was last started.  This counter displays the difference 
        /// between the start time and the current time.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter SystemUpTime()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("System", "System Up Time");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the threads performance counter for all disks.
        /// </summary>
        /// <returns>Threads is the number of threads in the computer at the time of data collection. 
        /// This is an instantaneous count, not an average over the time interval.  
        /// A thread is the basic executable entity that can execute instructions in a processor.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter Threads()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("System", "Threads");

            // Return the logical processor.
            return pc;
        }
    }
}
