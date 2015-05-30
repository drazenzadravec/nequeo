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
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Win32;

namespace Nequeo.Management.Process
{
    /// <summary>
    /// Machine process management.
    /// </summary>
    public class Details
    {
        /// <summary>
        /// Creates a new System.Diagnostics.Process component 
        /// for each process resource on the local computer.
        /// </summary>
        /// <returns>The list of processes.</returns>
        public System.Diagnostics.Process[] GetProcesses()
        {
            return System.Diagnostics.Process.GetProcesses();
        }

        /// <summary>
        /// Creates a new System.Diagnostics.Process component 
        /// for each process resource on the local computer.
        /// </summary>
        /// <param name="name">The name of the process to get.</param>
        /// <returns>The list of processes.</returns>
        public System.Diagnostics.Process[] GetProcesses(string name)
        {
            return System.Diagnostics.Process.GetProcessesByName(name);
        }

        /// <summary>
        /// Returns a new System.Diagnostics.Process component, given 
        /// the identifier of a process on the local computer.
        /// </summary>
        /// <param name="processID">The id of the process to get.</param>
        /// <returns>The process.</returns>
        public System.Diagnostics.Process GetProcess(int processID)
        {
            return System.Diagnostics.Process.GetProcessById(processID);
        }

        /// <summary>
        /// Stop the process
        /// </summary>
        /// <param name="name">The name of the process to stop.</param>
        /// <param name="waitTimeout">The amount of time, in milliseconds, to wait for the 
        /// associated process to exit. The maximum is the largest possible value of a 
        /// 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>True if the process has stopped; else false.</returns>
        public bool StopProcess(string name, int waitTimeout = 10000)
        {
            bool ret = false;

            // Get the process by name.
            System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName(name);

            // For each process.
            for (int i = 0; i < proc.Length; i++)
            {
                using (proc[i])
                {
                    proc[i].Kill();
                    proc[i].WaitForExit(waitTimeout);

                    if (!proc[i].HasExited)
                        proc[i].WaitForExit(waitTimeout);

                    if (!proc[i].HasExited)
                        ret = true;
                }
            }

            // Return the result.
            return ret;
        }

        /// <summary>
        /// Stop the process
        /// </summary>
        /// <param name="processID">The id of the process to stop.</param>
        /// <param name="waitTimeout">The amount of time, in milliseconds, to wait for the 
        /// associated process to exit. The maximum is the largest possible value of a 
        /// 32-bit integer, which represents infinity to the operating system.</param>
        /// <returns>True if the process has stopped; else false.</returns>
        public bool StopProcess(int processID, int waitTimeout = 10000)
        {
            bool ret = false;

            // Get the process by name.
            using (System.Diagnostics.Process proc = System.Diagnostics.Process.GetProcessById(processID))
            {
                proc.Kill();
                proc.WaitForExit(waitTimeout);

                if (!proc.HasExited)
                    proc.WaitForExit(waitTimeout);

                if (!proc.HasExited)
                    ret = true;

                // Return the result.
                return ret;
            }
        }

        /// <summary>
        /// Start the process with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        public Nequeo.Invention.ApplicationResult? StartProcess(string applicationExecutable)
        {
            return StartProcess(applicationExecutable, String.Empty, String.Empty, false);
        }

        /// <summary>
        /// Start the process with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        public Nequeo.Invention.ApplicationResult? StartProcess(string applicationExecutable,
            string applicationArguments)
        {
            return StartProcess(applicationExecutable, applicationArguments, String.Empty, false);
        }

        /// <summary>
        /// Start the process with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        public Nequeo.Invention.ApplicationResult? StartProcess(string applicationExecutable,
            string applicationArguments, string verb)
        {
            return StartProcess(applicationExecutable, applicationArguments, verb, false);
        }

        /// <summary>
        /// Start the process with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <param name="redirectOutput">Should the output/error streams be captured.</param>
        /// <param name="waitForExit">The amount of time to wait before exit; -1 indicates do not wait; 0 indicates wait indefinitely</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        public Nequeo.Invention.ApplicationResult? StartProcess(string applicationExecutable,
            string applicationArguments, string verb, bool redirectOutput, int waitForExit = -1)
        {
            // Create the application handler and
            // start the application.
            Nequeo.Invention.Application appl = new Invention.Application();
            return appl.RunApplication(applicationExecutable, applicationArguments, verb, redirectOutput, waitForExit);
        }

        /// <summary>
        /// Get the process.
        /// </summary>
        /// <param name="processID">The id of the process to get.</param>
        /// <param name="process">The process.</param>
        /// <param name="host">The host name to get system information for (Default = localhost)</param>
        /// <returns>The status the the connection.</returns>
        public Nequeo.Management.Sys.Status GetProcess(int processID, out ProcessDetails process, String host = "localhost")
        {
            ProcessDetails[] processes = null;
            process = processes[0];
            return GetProcessesEx(host, null, null, out processes, processID);
        }

        /// <summary>
        /// Get the process.
        /// </summary>
        /// <param name="host">The host name to get system information for</param>
        /// <param name="username">The username for the host</param>
        /// <param name="password">The password for the host</param>
        /// <param name="processID">The id of the process to get.</param>
        /// <param name="process">The process.</param>
        /// <returns>The status the the connection.</returns>
        public Nequeo.Management.Sys.Status GetProcess(String host, String username, String password, int processID, out ProcessDetails process)
        {
            ProcessDetails[] processes = null;
            process = processes[0];
            return GetProcessesEx(host, username, password, out processes, processID);
        }

        /// <summary>
        /// Get the processes.
        /// </summary>
        /// <param name="processes">The collection of processes.</param>
        /// <param name="host">The host name to get system information for (Default = localhost)</param>
        /// <returns>The status the the connection.</returns>
        public Nequeo.Management.Sys.Status GetProcesses(out ProcessDetails[] processes, String host = "localhost")
        {
            return GetProcessesEx(host, null, null, out processes, -1);
        }

        /// <summary>
        /// Get the processes.
        /// </summary>
        /// <param name="host">The host name to get system information for</param>
        /// <param name="username">The username for the host</param>
        /// <param name="password">The password for the host</param>
        /// <param name="processes">The collection of processes.</param>
        /// <returns>The status the the connection.</returns>
        public Nequeo.Management.Sys.Status GetProcesses(String host, String username, String password, out ProcessDetails[] processes)
        {
            return GetProcessesEx(host, username, password, out processes, -1);
        }

        /// <summary>
        /// Get the processes.
        /// </summary>
        /// <param name="host">The host name to get system information for</param>
        /// <param name="username">The username for the host</param>
        /// <param name="password">The password for the host</param>
        /// <param name="processes">The collection of processes.</param>
        /// <param name="processID">The id of the process to get.</param>
        /// <returns>The status the the connection.</returns>
        private Nequeo.Management.Sys.Status GetProcessesEx(String host, String username, String password, out ProcessDetails[] processes, int processID = -1)
        {
            processes = null;

            // No blank username's allowed.
            if (username == string.Empty)
            {
                username = null;
                password = null;
            }

            // Configure the connection settings.
            ConnectionOptions options = new ConnectionOptions();
            options.Username = username;
            options.Password = password;

            // Set the scope for the connection.
            ManagementPath path = new ManagementPath(String.Format("\\\\{0}\\root\\cimv2", host));
            ManagementScope scope = new ManagementScope(path, options);

            // Try and connect to the remote (or local) machine.
            try
            {
                // Connect to the machine
                scope.Connect();
            }
            catch (ManagementException)
            {
                return Nequeo.Management.Sys.Status.AuthenticateFailure;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return Nequeo.Management.Sys.Status.RPCServicesUnavailable;
            }
            catch (System.UnauthorizedAccessException)
            {
                return Nequeo.Management.Sys.Status.UnauthorizedAccess;
            }
            catch (System.Exception)
            {
                return Nequeo.Management.Sys.Status.Failed;
            }

            // Create a management searcher for all processes
            ManagementObjectSearcher moSearch = new ManagementObjectSearcher(scope,
                new ObjectQuery("SELECT " +
                                    "Handle, ProcessId, Name, Caption, Description, " +
                                    "ExecutablePath, HandleCount, Status, ThreadCount, " +
                                    "VirtualSize, ReadTransferCount, WriteTransferCount " +
                                "FROM Win32_Process" + (processID > -1 ? " WHERE ProcessId = " + processID.ToString() : "")));
            ManagementObjectCollection moReturn = moSearch.Get();

            // Create the process collection.
            processes = new ProcessDetails[moReturn.Count];
            int i = 0;

            // For each process found load into process structure.
            foreach (ManagementObject mo in moReturn)
            {
                processes[i].Handle = mo["Handle"].ToString();
                processes[i].ProcessId = uint.Parse(mo["ProcessId"].ToString());
                processes[i].Name = mo["Name"].ToString();
                processes[i].Caption = mo["Caption"].ToString();
                processes[i].Description = mo["Description"].ToString();
                processes[i].ExecutablePath = mo["ExecutablePath"].ToString();
                processes[i].HandleCount = uint.Parse(mo["HandleCount"].ToString());
                processes[i].Status = mo["Status"].ToString();
                processes[i].ThreadCount = uint.Parse(mo["ThreadCount"].ToString());
                processes[i].VirtualSize = ulong.Parse(mo["VirtualSize"].ToString());
                processes[i].ReadTransferCount = ulong.Parse(mo["ReadTransferCount"].ToString());
                processes[i].WriteTransferCount = ulong.Parse(mo["WriteTransferCount"].ToString());
                i++;
            }

            // Successful.
            return Nequeo.Management.Sys.Status.Success;
        }
    }
}
