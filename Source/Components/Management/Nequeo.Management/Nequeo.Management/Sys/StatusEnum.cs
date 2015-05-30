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

namespace Nequeo.Management.Sys
{
    /// <summary>
    /// Logical drive types
    /// </summary>
    [Serializable]
    public enum DriveTypes
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// No Root Directory
        /// </summary>
        No_Root_Directory = 1,
        /// <summary>
        /// Removable Disk
        /// </summary>
        Removable_Disk = 2,
        /// <summary>
        /// Local Disk
        /// </summary>
        Local_Disk = 3,
        /// <summary>
        /// Network Drive
        /// </summary>
        Network_Drive = 4,
        /// <summary>
        /// Compact Disc
        /// </summary>
        Compact_Disc = 5,
        /// <summary>
        /// RAM Disk
        /// </summary>
        RAM_Disk = 6
    }

    /// <summary>
    /// Connection Status Types
    /// </summary>
    [Serializable]
    public enum Status
    {
        /// <summary>
        /// Success
        /// </summary>
        Success = 0,
        /// <summary>
        /// Authenticate Failure
        /// </summary>
        AuthenticateFailure = 1,
        /// <summary>
        /// Unauthorized Access
        /// </summary>
        UnauthorizedAccess = 2,
        /// <summary>
        /// RPC Services Unavailable
        /// </summary>
        RPCServicesUnavailable = 3,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 4,
    }

    /// <summary>
    /// Logical drive structure
    /// </summary>
    [Serializable]
    public struct LogicalDrive
    {
        /// <summary>
        /// Drive name
        /// </summary>
        public String Name;

        /// <summary>
        /// Drive type
        /// </summary>
        public DriveTypes DriveType;

        /// <summary>
        /// Drive size
        /// </summary>
        public ulong Size;

        /// <summary>
        /// Drive free space
        /// </summary>
        public ulong FreeSpace;

        /// <summary>
        /// Drive file system
        /// </summary>
        public String FileSystem;
    }

    /// <summary>
    /// IP Address structure
    /// </summary>
    [Serializable]
    public struct IPAddresses
    {
        /// <summary>
        /// Network ip address
        /// </summary>
        public IPAddress Address;

        /// <summary>
        /// Network ip submet mask
        /// </summary>
        public IPAddress Subnet;
    }

    /// <summary>
    /// Network adapter structure
    /// </summary>
    [Serializable]
    public struct NetworkAdapter
    {
        /// <summary>
        /// Network IP Address collection
        /// </summary>
        public IPAddresses[] NetworkAddress;

        /// <summary>
        /// Is DHCP enabled
        /// </summary>
        public Boolean DHCPEnabled;

        /// <summary>
        /// Network adapter name
        /// </summary>
        public String Name;

        /// <summary>
        /// Network database path
        /// </summary>
        public String DatabasePath;
    }

    /// <summary>
    /// Processor structure
    /// </summary>
    [Serializable]
    public struct Processor
    {
        /// <summary>
        /// Processor name
        /// </summary>
        public String Name;

        /// <summary>
        /// Processor speed GHz
        /// </summary>
        public uint Speed;

        /// <summary>
        /// Processor architecture
        /// </summary>
        public String Architecture;

        /// <summary>
        /// Number of logical processors
        /// </summary>
        public uint NumberOfLogicalProcessors;

        /// <summary>
        /// Number of cores
        /// </summary>
        public uint NumberOfCores;
    }

    /// <summary>
    /// Operating system version structure
    /// </summary>
    [Serializable]
    public struct OperatingSystemVersion
    {
        /// <summary>
        /// Service pack major version
        /// </summary>
        public uint ServicepackMajor;

        /// <summary>
        /// Service pack minor version
        /// </summary>
        public uint ServicepackMinor;

        /// <summary>
        /// Operating system major version
        /// </summary>
        public uint Major;

        /// <summary>
        /// Operating system minor version
        /// </summary>
        public uint Minor;

        /// <summary>
        /// Operating system type
        /// </summary>
        public uint Type;

        /// <summary>
        /// Operating system build version
        /// </summary>
        public uint Build;

        /// <summary>
        /// Operating system description
        /// </summary>
        public String Description;

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A System.String containing a fully qualified type name.</returns>
        public override string ToString()
        {
            return
                Major.ToString() + "." + Minor.ToString() + "." + Build.ToString() + " " +
                Description + " " +
                Type.ToString() + " " +
                ServicepackMajor.ToString() + "." + ServicepackMinor.ToString();
        }
    }
}
