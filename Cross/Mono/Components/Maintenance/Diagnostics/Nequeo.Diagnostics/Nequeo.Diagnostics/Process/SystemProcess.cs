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

namespace Nequeo.Diagnostics.Process
{
    /// <summary>
    /// System process.
    /// </summary>
    public class SystemProcess
    {
        /// <summary>
        /// Get the total process time performance counter for all processors.
        /// </summary>
        /// <returns>% Processor Time is the percentage of elapsed time that all of process 
        /// threads used the processor to execution instructions. An instruction is the basic 
        /// unit of execution in a computer, a thread is the object that executes instructions, 
        /// and a process is the object created when a program is run. Code executed to handle 
        /// some hardware interrupts and trap conditions are included in this count.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter Total()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Process", "% Processor Time", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the processor time performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>% Processor Time is the percentage of elapsed time that all of process 
        /// threads used the processor to execution instructions. An instruction is the basic 
        /// unit of execution in a computer, a thread is the object that executes instructions, 
        /// and a process is the object created when a program is run. Code executed to handle 
        /// some hardware interrupts and trap conditions are included in this count.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ProcessorTime(string processName)
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Process", "% Processor Time", processName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total handle count performance counter for all processors.
        /// </summary>
        /// <returns>The total number of handles currently open by this process. 
        /// This number is equal to the sum of the handles currently open by each 
        /// thread in this process.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalHandles()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Process", "Handle Count", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the handle count performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>The total number of handles currently open by this process. 
        /// This number is equal to the sum of the handles currently open by each 
        /// thread in this process.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter HandleCount(string processName)
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Process", "Handle Count", processName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the process id performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>ID Process is the unique identifier of this process. 
        /// ID Process numbers are reused, so they only identify a process 
        /// for the lifetime of that process.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ID(string processName)
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Process", "ID Process", processName);

            // Return the logical processor.
            return pc;
        }


        /// <summary>
        /// Get the total thread count performance counter for all processors.
        /// </summary>
        /// <returns>The number of threads currently active in this process. An instruction is 
        /// the basic unit of execution in a processor, and a thread is the object that executes 
        /// instructions. Every running process has at least one thread.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalThreads()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Process", "Thread Count", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the thread count performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>The number of threads currently active in this process. An instruction is 
        /// the basic unit of execution in a processor, and a thread is the object that executes 
        /// instructions. Every running process has at least one thread.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ThreadCount(string processName)
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Process", "Thread Count", processName);

            // Return the logical processor.
            return pc;
        }
    }
}
