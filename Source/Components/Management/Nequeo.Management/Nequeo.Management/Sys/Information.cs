/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using Microsoft.Win32;

namespace Nequeo.Management.Sys
{
    /// <summary>
    /// System information
    /// </summary>
    [Serializable]
    public class Information
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Information()
        {
            _stderr = null;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="errStream">The error stream to use</param>
        public Information(System.IO.Stream errStream)
		{
			// Note that we use the property to set this as it
			// does checks on it to make sure it's valid.
            _stderr = errStream;
		}

        private int _extendederror;
        private System.IO.Stream _stderr;
        private NetworkAdapter[] _adapters;

        private String _bios;

        private String _osname;
        private String _osmanufacturer;
        private OperatingSystemVersion _osversion;
        private String _locale;
        private String _windowsdirectory;
        private ulong _freephysicalmemory;
        private ulong _totalphysicalmemory;
        private ulong _freevirtualmemory;
        private ulong _totalvirtualmemory;
        private ulong _pagefilesize;
        private TimeZoneInfo _timezone;
        private String _computername;

        private String _domain;
        private String _systemmanufacturer;
        private String _systemmodel;
        private String _systemtype;
        private uint _numberofprocessors;
        private Processor[] _processors;

        private LogicalDrive[] _drives;

        /// <summary>
        /// Gets the extended error
        /// </summary>
        public int ExtendedError
        {
            get { return _extendederror; }
        }

        /// <summary>
        /// Gets sets the standard error
        /// </summary>
        public System.IO.Stream StdErr
        {
            get { return _stderr; }
            set
            {
                if (value != null && value.CanWrite)
                    _stderr = value;
                else
                    throw new System.IO.IOException("Stream is not open or cannot be written to.");
            }
        }

        /// <summary>
        /// Gets the logical drive collection
        /// </summary>
        public LogicalDrive[] LogicalDrives
        {
            get { return _drives; }
        }

        /// <summary>
        /// Gets the system BIOS
        /// </summary>
        public String Bios
        {
            get { return _bios; }
        }

        /// <summary>
        /// Gets the network adapter collection
        /// </summary>
        public NetworkAdapter[] Adapters
        {
            get { return _adapters; }
        }

        /// <summary>
        /// Gets the operating system name
        /// </summary>
        public String OSName
        {
            get { return _osname; }
        }

        /// <summary>
        /// Gets the operating system manufacturer
        /// </summary>
        public String OSManufacturer
        {
            get { return _osmanufacturer; }
        }

        /// <summary>
        /// Gets the operating system version
        /// </summary>
        public OperatingSystemVersion OSVersion
        {
            get { return _osversion; }
        }

        /// <summary>
        /// Gets the operating local
        /// </summary>
        public String Locale
        {
            get { return _locale; }
        }

        /// <summary>
        /// Gets the operating system directory
        /// </summary>
        public String WindowsDirectory
        {
            get { return _windowsdirectory; }
        }

        /// <summary>
        /// Gets the free physical memory
        /// </summary>
        public ulong FreePhysicalMemory
        {
            get { return _freephysicalmemory; }
        }

        /// <summary>
        /// Gets the total physical memory
        /// </summary>
        public ulong TotalPhysicalMemory
        {
            get { return _totalphysicalmemory; }
        }

        /// <summary>
        /// Gets the free virtual memory
        /// </summary>
        public ulong FreeVirtualMemory
        {
            get { return _freevirtualmemory; }
        }

        /// <summary>
        /// Gets the total virtual memory
        /// </summary>
        public ulong TotalVirtualMemory
        {
            get { return _totalvirtualmemory; }
        }

        /// <summary>
        /// Gets the page file size
        /// </summary>
        public ulong PageFileSize
        {
            get { return _pagefilesize; }
        }

        /// <summary>
        /// Gets the current time zone
        /// </summary>
        public TimeZoneInfo Timezone
        {
            get { return _timezone; }
        }

        /// <summary>
        /// Gets the computer name
        /// </summary>
        public String ComputerName
        {
            get { return _computername; }
        }

        /// <summary>
        /// Gets the computer domain
        /// </summary>
        public String Domain
        {
            get { return _domain; }
        }

        /// <summary>
        /// Gets the system manufacturer
        /// </summary>
        public String SystemManufacturer
        {
            get { return _systemmanufacturer; }
        }

        /// <summary>
        /// Gets the system model
        /// </summary>
        public String SystemModel
        {
            get { return _systemmodel; }
        }

        /// <summary>
        /// Gets the system type
        /// </summary>
        public String SystemType
        {
            get { return _systemtype; }
        }

        /// <summary>
        /// Gets the number of processors
        /// </summary>
        public uint NumberOfProcessors
        {
            get { return _numberofprocessors; }
        }

        /// <summary>
        /// Gets the collection of processors
        /// </summary>
        public Processor[] Processors
        {
            get { return _processors; }
        }

        /// <summary>
        /// Get the system information
        /// </summary>
        /// <param name="host">The host name to get system information for (Default = localhost)</param>
        /// <returns>The status the the connection.</returns>
        public Status Get(String host = "localhost")
        {
            return Get(host, null, null);
        }

        /// <summary>
        /// Get the system information
        /// </summary>
        /// <param name="host">The host name to get system information for</param>
        /// <param name="username">The username for the host</param>
        /// <param name="password">The password for the host</param>
        /// <returns>The status the the connection.</returns>
        public Status Get(String host, String username, String password)
        {
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
            catch (ManagementException ex)
            {
                // Failed to authenticate properly.
                LogError("Failed to authenticate: " + ex.Message);

                _extendederror = (int)ex.ErrorCode;
                return Status.AuthenticateFailure;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                // Unable to connect to the RPC service on the remote machine.
                LogError("Unable to connect to RPC service: " + ex.Message);

                _extendederror = ex.ErrorCode;
                return Status.RPCServicesUnavailable;
            }
            catch (System.UnauthorizedAccessException ex)
            {
                // User not authorized.
                LogError("Unauthorized access: " + ex.Message);

                _extendederror = 0;
                return Status.UnauthorizedAccess;
            }

            // Populate the system information.
            GetSystemInformation(scope);
            GetNetworkAddresses(scope);
            GetLogicalDrives(scope);

            _extendederror = 0;
            return Status.Success;
        }

        /// <summary>
        /// Serialise the result
        /// </summary>
        /// <returns>The serialised information.</returns>
        public string SerialiseResult()
        {
            // Attempt to serialise the result of the current object.
            Nequeo.Serialisation.GeneralSerialisation serialise = new Serialisation.GeneralSerialisation();
            byte[] data = serialise.Serialise(this, typeof(Nequeo.Management.Sys.Information));
            return Encoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Log the error
        /// </summary>
        /// <param name="message">The message to log</param>
        private void LogError(String message)
        {
            if (_stderr != null && _stderr.CanWrite)
            {
                // Write the error to the stream.
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(message);
                _stderr.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Get the logical drives
        /// </summary>
        /// <param name="scope">The current management scope</param>
        private void GetLogicalDrives(ManagementScope scope)
        {
            // Create a management searcher for all logical drives
            ManagementObjectSearcher moSearch = new ManagementObjectSearcher(scope, 
                new ObjectQuery("Select Name, DriveType, Size, FreeSpace, FileSystem from Win32_LogicalDisk Where DriveType = 3 Or DriveType = 6"));
            ManagementObjectCollection moReturn = moSearch.Get();

            // Create the drive collection
            _drives = new LogicalDrive[moReturn.Count];
            int i = 0;

            // For each drive found load into drive structure.
            foreach (ManagementObject mo in moReturn)
            {
                _drives[i].DriveType = (DriveTypes)int.Parse(mo["DriveType"].ToString());
                _drives[i].FileSystem = mo["FileSystem"].ToString();
                _drives[i].FreeSpace = ulong.Parse(mo["FreeSpace"].ToString());
                _drives[i].Size = ulong.Parse(mo["Size"].ToString());
                _drives[i].Name = mo["Name"].ToString();
                i++;
            }
        }

        /// <summary>
        /// Get the system information
        /// </summary>
        /// <param name="scope">The current management scope</param>
        private void GetSystemInformation(ManagementScope scope)
        {
            // Only get the first BIOS in the list. Usually this is all there is.
            foreach (ManagementObject mo in new ManagementClass(scope, new ManagementPath("Win32_BIOS"), null).GetInstances())
            {
                _bios = mo["Version"].ToString();
                break;
            }

            // For each management object found
            foreach (ManagementObject mo in new ManagementClass(scope, new ManagementPath("Win32_OperatingSystem"), null).GetInstances())
            {
                // Get the OS buld information
                _osversion.Build = uint.Parse(mo["BuildNumber"].ToString());
                _osversion.Description = String.Format("{0} {1} Build {2}", mo["Version"], mo["CSDVersion"], mo["BuildNumber"]);
                _osversion.ServicepackMajor = uint.Parse(mo["ServicePackMajorVersion"].ToString());
                _osversion.ServicepackMinor = uint.Parse(mo["ServicePackMinorVersion"].ToString());
                _osversion.Type = uint.Parse(mo["OSType"].ToString());

                // Get the major and minor version numbers.
                String[] numbers = mo["Version"].ToString().Split(".".ToCharArray());
                _osversion.Major = uint.Parse(numbers[0]);
                _osversion.Minor = uint.Parse(numbers[1]);

                // Get the rest of the fields.
                _osname = mo["Name"].ToString().Split("|".ToCharArray())[0];
                _osmanufacturer = mo["Manufacturer"].ToString();
                _locale = mo["Locale"].ToString();
                _windowsdirectory = mo["WindowsDirectory"].ToString();
                _freevirtualmemory = ulong.Parse(mo["FreeVirtualMemory"].ToString());
                _totalvirtualmemory = ulong.Parse(mo["TotalVirtualMemorySize"].ToString());
                _freephysicalmemory = ulong.Parse(mo["FreePhysicalMemory"].ToString());
                _totalphysicalmemory = ulong.Parse(mo["TotalVisibleMemorySize"].ToString());
                _pagefilesize = ulong.Parse(mo["SizeStoredInPagingFiles"].ToString());
                _computername = mo["CSName"].ToString();

                // Get the information related to the timezone.
                _timezone = TimeZoneInfo.Local;
                break;
            }

            // For each management object found
            foreach (ManagementObject mo in new ManagementClass(scope, new ManagementPath("Win32_ComputerSystem"), null).GetInstances())
            {
                // Get the system information
                _systemmanufacturer = mo["Manufacturer"].ToString();
                _systemmodel = mo["Model"].ToString();
                _systemtype = mo["SystemType"].ToString();
                _domain = mo["Domain"].ToString();
                _numberofprocessors = uint.Parse(mo["NumberOfProcessors"].ToString());
                break;
            }

            // Create a management searcher for the processor information
            ManagementObjectSearcher moSearch = new ManagementObjectSearcher(scope, 
                new ObjectQuery("Select Name, CurrentClockSpeed, Architecture, NumberOfCores, NumberOfLogicalProcessors from Win32_Processor"));
            ManagementObjectCollection moReturn = moSearch.Get();

            // Create the processor collection
            _processors = new Processor[moReturn.Count];
            int i = 0;

            // For each processor found
            foreach (ManagementObject mo in moReturn)
            {
                _processors[i].Name = mo["Name"].ToString().Trim();
                _processors[i].Architecture = mo["Architecture"].ToString();
                _processors[i].Speed = uint.Parse(mo["CurrentClockSpeed"].ToString());
                _processors[i].NumberOfCores = uint.Parse(mo["NumberOfCores"].ToString());
                _processors[i].NumberOfLogicalProcessors = uint.Parse(mo["NumberOfLogicalProcessors"].ToString());
                i++;
            }
        }

        /// <summary>
        /// Getnetwork adapter information
        /// </summary>
        /// <param name="scope">The current management scope</param>
        private void GetNetworkAddresses(ManagementScope scope)
        {
            ManagementObjectCollection adapters = null;
            ManagementObjectSearcher search = null;

            // Create a management searcher for the network adapter information
            search = new ManagementObjectSearcher(scope, 
                new ObjectQuery("Select Description, DHCPEnabled, IPAddress, DatabasePath, IPSubnet from Win32_NetworkAdapterConfiguration Where IPEnabled = True"));
            adapters = search.Get();

            // Create the network adapter collection
            _adapters = new NetworkAdapter[adapters.Count];

            int i = 0;

            // For each adpater found
            foreach (ManagementObject adapter in adapters)
            {
                // Assign the details
                _adapters[i].Name = adapter["Description"].ToString();
                _adapters[i].DHCPEnabled = Boolean.Parse(adapter["DHCPEnabled"].ToString());
                _adapters[i].DatabasePath = adapter["DatabasePath"].ToString();

                // If the ip address exits
                if (adapter["IPAddress"] != null)
                {
                    // Assign the network address
                    _adapters[i].NetworkAddress = new IPAddresses[((string[])adapter["IPAddress"]).Length];

                    // For each ip address assign the information
                    for (int j = 0; j < ((string[])adapter["IPAddress"]).Length; j++)
                    {
                        // Assign the network ip address and subnet mask.
                        _adapters[i].NetworkAddress[j].Address = IPAddress.Parse(((string[])adapter.Properties["IPAddress"].Value)[j]);
                        _adapters[i].NetworkAddress[j].Subnet = IPAddress.Parse(((string[])adapter.Properties["IPSubnet"].Value)[j]);
                    }
                }
                i++;
            }
        }
    }
}
