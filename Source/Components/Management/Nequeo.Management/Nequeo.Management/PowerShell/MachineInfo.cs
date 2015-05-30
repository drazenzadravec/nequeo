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
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

using Nequeo.Management.Sys;

namespace Nequeo.Management.PowerShell
{
    /// <summary>
    /// Machine information base PowerShell command-let.
    /// </summary>
    public abstract class MachineInfoBase : PSCmdlet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected MachineInfoBase()
        {
            // Create a new information class.
            _information = new Information();
            _information.Get();
        }

        private Information _information = null;

        /// <summary>
        /// Gets the system information.
        /// </summary>
        public Information Information
        {
            get { return _information; }
        }
    }

    /// <summary>
    /// Machine version PowerShell command-let.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MachineInfoHelp")]
    public class MachineInfoHelp : MachineInfoBase
    {
        private string[] _commandList = new string[]
        {
            "get-MachineInfoHelp : Gets machine info help.",
            "get-MachineOperatingSystem : Gets machine operating system.",
            "get-MachineComputerSystem : Gets machine computer system."
        };

        /// <summary>
        /// Process the command.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Write the help details.
            WriteObject(Environment.NewLine + String.Join("\r\n", _commandList) + Environment.NewLine);
        }
    }

    /// <summary>
    /// Machine operating system PowerShell command-let.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MachineOperatingSystem")]
    public class OperatingSystem : MachineInfoBase
    {
        /// <summary>
        /// Process the command.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Write the version.
            WriteObject(Environment.NewLine +
                "OS Name : " + Information.OSName.ToString() + Environment.NewLine +
                "OS Manufacturer : " + Information.OSManufacturer.ToString() + Environment.NewLine +
                "Locale : " + Information.Locale.ToString() + Environment.NewLine +
                "Windows Directory : " + Information.WindowsDirectory.ToString() + Environment.NewLine +
                "Free Virtual Memory : " + Information.FreeVirtualMemory.ToString() + Environment.NewLine +
                "Free Physical Memory : " + Information.FreePhysicalMemory.ToString() + Environment.NewLine +
                "Total Physical Memory : " + Information.TotalPhysicalMemory.ToString() + Environment.NewLine +
                "Page File Size : " + Information.PageFileSize.ToString() + Environment.NewLine +
                "Computer Name : " + Information.ComputerName.ToString() + Environment.NewLine +
                "Timezone : " + Information.Timezone.DisplayName.ToString() + Environment.NewLine +
                "Major Version : " + Information.OSVersion.Major.ToString() + Environment.NewLine +
                "Minor Version : " + Information.OSVersion.Minor.ToString() + Environment.NewLine +
                "Servicepack Major Version : " + Information.OSVersion.ServicepackMajor.ToString() + Environment.NewLine +
                "Servicepack Minor Version : " + Information.OSVersion.ServicepackMinor.ToString() + Environment.NewLine +
                "Build : " + Information.OSVersion.Build.ToString() + Environment.NewLine +
                "Type : " + Information.OSVersion.Type.ToString() + Environment.NewLine +
                "Architecture : " + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + Environment.NewLine +
                "Description : " + Information.OSVersion.Description.ToString() + Environment.NewLine);
        }
    }

    /// <summary>
    /// Machine computer system PowerShell command-let.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MachineComputerSystem")]
    public class ComputerSystem : MachineInfoBase
    {
        /// <summary>
        /// Process the command.
        /// </summary>
        protected override void ProcessRecord()
        {
            string drives = "";
            string processors = "";
            string networkAdapters = "";

            // Get all the drive data.
            foreach (LogicalDrive drive in Information.LogicalDrives)
            {
                drives +=
                    "Name : " + drive.Name + Environment.NewLine +
                    "Size : " + drive.Size + Environment.NewLine +
                    "Free Space : " + drive.FreeSpace.ToString() + Environment.NewLine +
                    "File System : " + drive.FileSystem + Environment.NewLine +
                    "Drive Type : " + drive.DriveType + Environment.NewLine;
            }

            // Get all the processor data.
            foreach (Processor processor in Information.Processors)
            {
                processors +=
                    "Name : " + processor.Name + Environment.NewLine +
                    "Architecture : " + processor.Architecture + Environment.NewLine +
                    "Number Of Logical Processors : " + processor.NumberOfLogicalProcessors.ToString() + Environment.NewLine +
                    "Number Of Cores : " + processor.NumberOfCores + Environment.NewLine +
                    "Speed : " + processor.Speed + Environment.NewLine;
            }

            // Get all network adapter data.
            foreach (NetworkAdapter adapter in Information.Adapters)
            {
                networkAdapters +=
                    "Description : " + adapter.Name + Environment.NewLine +
                    "DHCP Enabled : " + adapter.DHCPEnabled + Environment.NewLine +
                    "DatabasePath : " + adapter.DatabasePath + Environment.NewLine;

                // Get the ip address for the adapter
                foreach (IPAddresses addresses in adapter.NetworkAddress)
                {
                    networkAdapters +=
                        "Address Family : " + addresses.Address.AddressFamily.ToString() + Environment.NewLine +
                        "Address : " + addresses.Address.ToString() + Environment.NewLine +
                        "Subnet : " + addresses.Subnet.ToString() + Environment.NewLine;
                }

                // Add a spacer.
                networkAdapters += Environment.NewLine;
            }

            // Write the version.
            WriteObject(Environment.NewLine +
                "System Manufacturer : " + Information.SystemManufacturer.ToString() + Environment.NewLine +
                "System Model : " + Information.SystemModel.ToString() + Environment.NewLine +
                "System Type : " + Information.SystemType.ToString() + Environment.NewLine +
                "Domain : " + Information.Domain.ToString() + Environment.NewLine +
                "Number Of Processors : " + Information.NumberOfProcessors.ToString() + Environment.NewLine + Environment.NewLine +
                "Processors" + Environment.NewLine +
                processors + Environment.NewLine +
                "Drives" + Environment.NewLine +
                drives + Environment.NewLine +
                "Network Adapters" + Environment.NewLine +
                networkAdapters);
        }
    }
}
