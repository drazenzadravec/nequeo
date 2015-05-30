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
    /// System threads.
    /// </summary>
    public class Threads
    {
        /// <summary>
        /// Get the total performance counter for all processors.
        /// </summary>
        /// <returns>% Processor Time is the percentage of elapsed time that 
        /// all of process threads used the processor to execution instructions. 
        /// An instruction is the basic unit of execution in a computer, a thread 
        /// is the object that executes instructions, and a process is the object 
        /// created when a program is run. Code executed to handle some hardware 
        /// interrupts and trap conditions are included in this count.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter Total()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "% Processor Time", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>% Processor Time is the percentage of elapsed time that 
        /// all of process threads used the processor to execution instructions. 
        /// An instruction is the basic unit of execution in a computer, a thread 
        /// is the object that executes instructions, and a process is the object 
        /// created when a program is run. Code executed to handle some hardware 
        /// interrupts and trap conditions are included in this count.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ProcessorTime(string processName = "csrss/0")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "% Processor Time", processName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total context switches per second performance counter for all processors.
        /// </summary>
        /// <returns>Context Switches/sec is the rate of switches from one thread to another.  
        /// Thread switches can occur either inside of a single process or across processes.  
        /// A thread switch can be caused either by one thread asking another for information, 
        /// or by a thread being preempted by another, higher priority thread becoming ready to run.  
        /// Unlike some early operating systems, Windows NT uses process boundaries for subsystem 
        /// protection in addition to the traditional protection of user and privileged modes.  
        /// These subsystem processes provide additional protection.  Therefore, some work done 
        /// by Windows NT on behalf of an application  appear in other subsystem processes in 
        /// addition to the privileged time in the application.  Switching to the subsystem process 
        /// causes one Context Switch in the application thread.  Switching back causes another 
        /// Context Switch in the subsystem thread.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalContextSwitchesPerSec()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "Context Switches/sec", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the context switches per second performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>Context Switches/sec is the rate of switches from one thread to another.  
        /// Thread switches can occur either inside of a single process or across processes.  
        /// A thread switch can be caused either by one thread asking another for information, 
        /// or by a thread being preempted by another, higher priority thread becoming ready to run.  
        /// Unlike some early operating systems, Windows NT uses process boundaries for subsystem 
        /// protection in addition to the traditional protection of user and privileged modes.  
        /// These subsystem processes provide additional protection.  Therefore, some work done 
        /// by Windows NT on behalf of an application  appear in other subsystem processes in 
        /// addition to the privileged time in the application.  Switching to the subsystem process 
        /// causes one Context Switch in the application thread.  Switching back causes another 
        /// Context Switch in the subsystem thread.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ContextSwitchesPerSec(string processName = "csrss/0")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "Context Switches/sec", processName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total elapsed time performance counter for all processors.
        /// </summary>
        /// <returns>The total elapsed time (in seconds) this thread has been running.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalElapsedTime()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "Elapsed Time", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the elapsed time performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>The total elapsed time (in seconds) this thread has been running.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ElapsedTime(string processName = "csrss/0")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "Elapsed Time", processName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total id process performance counter for all processors.
        /// </summary>
        /// <returns>ID Process is the unique identifier of this process. ID Process 
        /// numbers are reused, so they only identify a process for the lifetime of that process.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalIDProcess()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "ID Process", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the id process performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>ID Process is the unique identifier of this process. 
        /// ID Process numbers are reused, so they only identify a process for the lifetime of that process.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter IDProcess(string processName = "csrss/0")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "ID Process", processName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the total id thread performance counter for all processors.
        /// </summary>
        /// <returns>ID Thread is the unique identifier of this thread.  
        /// ID Thread numbers are reused, so they only identify a thread for the lifetime of that thread.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter TotalIDThread()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "ID Thread", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the id thread performance counter for all processors.
        /// </summary>
        /// <param name="processName">The process name.</param>
        /// <returns>ID Thread is the unique identifier of this thread.  
        /// ID Thread numbers are reused, so they only identify a thread for the lifetime of that thread.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter IDThread(string processName = "csrss/0")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Thread", "ID Thread", processName);

            // Return the logical processor.
            return pc;
        }
    }
}
