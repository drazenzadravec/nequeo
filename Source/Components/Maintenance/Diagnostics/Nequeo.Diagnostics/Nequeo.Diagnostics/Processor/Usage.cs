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

namespace Nequeo.Diagnostics.Processor
{
    /// <summary>
    /// Processor usage provider.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// Get the number of processors.
        /// </summary>
        /// <returns>The number of processors.</returns>
        public int GetNumberOfProcessors()
        {
            return Environment.ProcessorCount;
        }

        /// <summary>
        /// Get the total performance counter for all processors.
        /// </summary>
        /// <returns>The total percentage processor time.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter Total()
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the processor time performance counter for all processors.
        /// </summary>
        /// <param name="processorName">The processor name.</param>
        /// <returns>The total percentage processor time.</returns>
        /// <exception cref="System.Exception"></exception>
        public PerformanceCounter ProcessorTime(string processorName = "0")
        {
            // Get the performance counter of all processors.
            PerformanceCounter pc = new PerformanceCounter("Processor", "% Processor Time", processorName);

            // Return the logical processor.
            return pc;
        }

        /// <summary>
        /// Get the individual performance counter for each processor.
        /// </summary>
        /// <returns>The performance counter for each processor.</returns>
        /// <exception cref="System.Exception"></exception>
        public List<PerformanceCounter> Individual()
        {
            // Get the number of processors.
            List<PerformanceCounter> logical = new List<PerformanceCounter>();
            int processorCount = Environment.ProcessorCount;

            // For each processor
            for (int i = 0; i < processorCount; i++)
            {
                // Get the performance counter of each processor.
                PerformanceCounter pc = new PerformanceCounter("Processor", "% Processor Time", i.ToString());
                logical.Add(pc);
            }

            // Return the logical processor.
            return logical;
        }

        /// <summary>
        /// Get the active percentage time for a processor.
        /// </summary>
        /// <param name="counters">The performance counter for each processor.</param>
        /// <param name="action">The action to perform for each sample (arg1: Processor Name, arg12:% Processor Time, return: End Sampling (true)).</param>
        /// <param name="samplingTime">The total sampling time.</param>
        /// <param name="samplingComplete">The sampling has completed handler.</param>
        /// <exception cref="System.Exception"></exception>
        public void Active(List<PerformanceCounter> counters, Func<string, float, bool> action, TimeSpan samplingTime, Action samplingComplete = null)
        {
            bool end = false;
            Stopwatch timer = null;

            try
            {
                // Start the timer.
                timer = new Stopwatch();
                timer.Start();

                // While still sampling and not ended.
                while (!end && timer.ElapsedTicks < samplingTime.Ticks)
                {
                    // Send the current counter data.
                    foreach (PerformanceCounter counter in counters)
                        end = action(counter.InstanceName, counter.NextValue());
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (timer != null)
                    // Stop the timer.
                    timer.Stop();
            }

            try
            {
                // Execute the complete action.
                if (samplingComplete != null)
                    samplingComplete();
            }
            catch { }
        }

        /// <summary>
        /// Get the active percentage time for a processor.
        /// </summary>
        /// <param name="counters">The performance counter for each processor.</param>
        /// <param name="action">The action to perform for each sample (arg1: Processor Name, arg12:% Processor Time, return: End Sampling (true)).</param>
        /// <param name="samplingTime">The total sampling time.</param>
        ///  <param name="samplingComplete">The sampling has completed handler.</param>
        /// <exception cref="System.Exception"></exception>
        public async void ActiveAsync(List<PerformanceCounter> counters, Func<string, float, bool> action, TimeSpan samplingTime, Action samplingComplete = null)
        {
            var result = Nequeo.Threading.AsyncOperationResult<bool>.
                RunTask(() => 
                    {
                        try
                        {
                            Active(counters, action, samplingTime, samplingComplete);
                        }
                        catch { }
                    });

            await result;
        }

        /// <summary>
        /// Get the active percentage time for each processor.
        /// </summary>
        /// <param name="processorName">The processor name.</param>
        /// <returns>The percentage of processor time.</returns>
        /// <exception cref="System.Exception"></exception>
        public List<ulong> Active(ref List<string> processorName)
        {
            List<ulong> loadPercentage = new List<ulong>();
            processorName = new List<string>();

            // Try and connect to the remote (or local) machine.
            try
            {
                // Create a management searcher for the processor information
                ManagementObjectSearcher moSearch = new ManagementObjectSearcher(
                    new ObjectQuery("select Name, PercentProcessorTime from Win32_PerfFormattedData_PerfOS_Processor WHERE name != '_Total'"));
                ManagementObjectCollection moReturn = moSearch.Get();

                // For each processor found
                foreach (ManagementObject mo in moReturn)
                {
                    processorName.Add(mo["Name"].ToString().Trim());
                    loadPercentage.Add(ulong.Parse(mo["PercentProcessorTime"].ToString()));
                }

                // Return the load percentage of each processor.
                return loadPercentage;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
