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
    /// Machine service process management.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="timeout">A System.TimeSpan object specifying the 
        /// amount of time to wait for the service to reach the specified status.</param>
        /// <returns>True if the service is running; else false.</returns>
        public bool Start(string name, TimeSpan timeout)
        {
            // Start the process.
            using (System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(name))
            {
                controller.Start();
                controller.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, timeout);

                if (controller.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="args">An array of arguments to pass to the service when it starts.</param>
        /// <param name="timeout">A System.TimeSpan object specifying the 
        /// amount of time to wait for the service to reach the specified status.</param>
        /// <returns>True if the service is running; else false.</returns>
        public bool Start(string name, string[] args, TimeSpan timeout)
        {
            // Start the process.
            using (System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(name))
            {
                controller.Start(args);
                controller.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, timeout);

                if (controller.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Stop the service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="timeout">A System.TimeSpan object specifying the 
        /// amount of time to wait for the service to reach the specified status.</param>
        /// <returns>True if the service has stopped; else false.</returns>
        public bool Stop(string name, TimeSpan timeout)
        {
            // Start the process.
            using (System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(name))
            {
                controller.Stop();
                controller.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, timeout);

                if (controller.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Get the status of the service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>The status of the service.</returns>
        public System.ServiceProcess.ServiceControllerStatus Status(string name)
        {
            // Start the process.
            using (System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(name))
            {
                // Return the status.
                return controller.Status;
            }
        }

        /// <summary>
        /// Get the service controller.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>The service process controller.</returns>
        public System.ServiceProcess.ServiceController GetService(string name)
        {
            return new System.ServiceProcess.ServiceController(name);
        }
    }
}
