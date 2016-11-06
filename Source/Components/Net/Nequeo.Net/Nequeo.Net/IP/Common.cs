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
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Nequeo.Net.IP
{
    /// <summary>
    /// IP address help
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Get th IP host entry details.
        /// </summary>
        /// <param name="server">The server to get the ip information for.</param>
        /// <returns>The ip host entry details.</returns>
        public static IPHostEntry GetIPHostEntry(string server = "")
        {
            // Get the host entry of the local or specified server.
            if (String.IsNullOrEmpty(server))
                return Dns.GetHostEntry(Dns.GetHostName());
            else
                return Dns.GetHostEntry(server);
        }

        /// <summary>
        /// The collection of IP addreses for the host.
        /// </summary>
        /// <param name="server">The server to get the ip information for.</param>
        /// <returns>The collection of IP addreses for the host.</returns>
        public static IPAddress[] GetHostEntryIPAddress(string server = "")
        {
            // Get the host entry.
            IPHostEntry hostEntry = GetIPHostEntry(server);

            // Return the list of ip address.
            return hostEntry.AddressList;
        }

        /// <summary>
        /// Get a random unused port.
        /// </summary>
        /// <returns>A random unused port.</returns>
        private static int GetRandomUnusedPort()
        {
            // Set port = 0, will get random unused port.
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }

    /// <summary>
    /// IP conversion help
    /// </summary>
    public class Convert
    {
        /// <summary>
        /// Convert the ip version 4 address to the equivalent ip version 6 address.
        /// </summary>
        /// <param name="ipv4Address">The ip version 4 address to convert</param>
        /// <param name="ipv6Version">The ip version 6 version number</param>
        /// <returns>The ip version 6 address.</returns>
        public static string IPv4ToIPv6(string ipv4Address, string ipv6Version = "")
        {
            // Get the ip version 4 parts from the ip address and
            // return the ip version 6 address.
            string[] ipV4Parts = ipv4Address.Split(new char[] { '.' }, System.StringSplitOptions.None);
            return
                (String.IsNullOrEmpty(ipv6Version) ? "" : ipv6Version + ":") +
                Int32.Parse(ipV4Parts[0]).ToString("x2") + Int32.Parse(ipV4Parts[1]).ToString("x2") + ":" +
                Int32.Parse(ipV4Parts[2]).ToString("x2") + Int32.Parse(ipV4Parts[3]).ToString("x2") + ":" +
                "0000:0000:0000:0000:0000";
        }

        /// <summary>
        /// Convert the ip version 6 address to the equivalent ip version 4 address.
        /// </summary>
        /// <param name="ipv6Address">The ip version 6 address to convert</param>
        /// <param name="isFirstOctetIPv6Version">Is the first octet the ip version 6 version number.</param>
        /// <returns>The ip version 4 address.</returns>
        public static string IPv6ToIPv4(string ipv6Address, bool isFirstOctetIPv6Version = false)
        {
            string[] ipV6Parts = ipv6Address.Split(new char[] { ':' }, System.StringSplitOptions.None);
            if (isFirstOctetIPv6Version)
                return 
                    System.Convert.ToInt32(ipV6Parts[1].Substring(0, 2), 16) + "." + 
                    System.Convert.ToInt32(ipV6Parts[1].Substring(2, 2), 16) + 
                    System.Convert.ToInt32(ipV6Parts[2].Substring(0, 2), 16) + "." +
                    System.Convert.ToInt32(ipV6Parts[2].Substring(2, 2), 16);
            else
                return 
                    Int32.Parse(ipV6Parts[0].Substring(0, 2), System.Globalization.NumberStyles.HexNumber) + "." + 
                    Int32.Parse(ipV6Parts[0].Substring(2, 2), System.Globalization.NumberStyles.HexNumber) + 
                    Int32.Parse(ipV6Parts[1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber) + "." + 
                    Int32.Parse(ipV6Parts[1].Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Converts a long representing an ip address to an ip address
        /// </summary>
        /// <param name="ipAddress">The long representing an ip address</param>
        /// <param name="reverseOrder">The reverse order of the ip address</param>
        /// <returns>The converted value.</returns>
        public static string LongToIPAddress(long ipAddress, bool reverseOrder)
        {
            string ip = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                int num = (int)(System.Math.Abs(ipAddress) / System.Math.Pow(256, (3 - i)));
                ipAddress = System.Math.Abs(ipAddress) - (long)(num * System.Math.Pow(256, (3 - i)));
                if (i == 0)
                {
                    ip = num.ToString();
                }
                else
                {
                    if (reverseOrder)
                        ip = num.ToString() + "." + ip;
                    else
                        ip = ip + "." + num.ToString();
                }
            }
            return ip;
        }

        /// <summary>
        /// Converts an ip address to its long representation.
        /// </summary>
        /// <param name="ipAddress">The ip address</param>
        /// <param name="reverseOrder">The reverse order of the ip address</param>
        /// <returns>The converted value</returns>
        public static long IPAddressToLong(string ipAddress, bool reverseOrder)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ipAddress))
            {
                ipBytes = ipAddress.Split('.');
                if (reverseOrder)
                {
                    for (int i = 0; i < ipBytes.Length; i++)
                    {
                        num += ((int.Parse(ipBytes[i]) % 256) * System.Math.Pow(256, (i)));
                    }
                }
                else
                {
                    for (int i = ipBytes.Length - 1; i >= 0; i--)
                    {
                        num += ((int.Parse(ipBytes[i]) % 256) * System.Math.Pow(256, (3 - i)));
                    }
                }
            }
            return (long)num;
        }

        /// <summary>
        /// Convert the network range to the starting and ending ip addresses
        /// </summary>
        /// <param name="networkRange">The network range (192.168.3.2/27)</param>
        /// <param name="startIP">The starting ip address</param>
        /// <param name="endIP">The ending ip address</param>
        public static void NetworkTwoIPAddressRange(string networkRange, out uint startIP, out uint endIP)
        {
            uint ip,                /* ip address */
                mask,           /* subnet mask */
                broadcast,      /* Broadcast address */
                network;        /* Network address */

            int bits;

            string[] elements = networkRange.Split(new Char[] { '/' });

            ip = IPAddressToUInt(elements[0]);
            bits = System.Convert.ToInt32(elements[1]);

            mask = ~(0xFFFFFFFF >> bits);

            network = ip & mask;
            broadcast = network + ~mask;
            uint usableIps = (bits > 30) ? 0 : (broadcast - network - 1);

            if (usableIps <= 0)
            {
                startIP = endIP = 0;
            }
            else
            {
                startIP = network & mask;
                endIP = (network & mask) | ~mask;
            }
        }

        /// <summary>
        /// Converts an ip address to its long representation.
        /// </summary>
        /// <param name="ipAddress">The ip address</param>
        /// <returns>The converted value</returns>
        public static uint IPAddressToUInt(string ipAddress)
        {
            uint ip = 0;
            string[] elements = ipAddress.Split(new Char[] { '.' });

            if (elements.Length == 4)
            {
                ip = System.Convert.ToUInt32(elements[0]) << 24;
                ip += System.Convert.ToUInt32(elements[1]) << 16;
                ip += System.Convert.ToUInt32(elements[2]) << 8;
                ip += System.Convert.ToUInt32(elements[3]);
            }

            return ip;
        }

        /// <summary>
        /// Convert the subnetmask to an un-signed int
        /// </summary>
        /// <param name="subnetMask">The subnetmask</param>
        /// <returns>The un-signed long value</returns>
        public static uint SubnetMaskToUInt(string subnetMask)
        {
            return BitConverter.ToUInt32(System.Net.IPAddress.Parse(subnetMask).GetAddressBytes(), 0);
        }

        /// <summary>
        /// Convert the subnetmask to a int
        /// </summary>
        /// <param name="subnetMask">The subnetmask</param>
        /// <returns>The long value</returns>
        public static int SubnetMaskToInt(string subnetMask)
        {
            return BitConverter.ToInt32(System.Net.IPAddress.Parse(subnetMask).GetAddressBytes(), 0);
        }

        /// <summary>
        /// Convert the subnet mask to a signed value.
        /// </summary>
        /// <param name="subnetMask">The subnetmask</param>
        /// <returns>The signed subnet mask value</returns>
        public static int SubnetMaskToSignedInt(string subnetMask)
        {
            uint ip = IPAddressToUInt(subnetMask);
            uint maskSigned = ~ip;
            return (-1 * ((int)(maskSigned + 1)));
        }

        /// <summary>
        /// Validate the given IPv4 or IPv6 address.
        /// </summary>
        /// <param name="address">The IP address as a string</param>
        /// <returns>true if a valid address, false otherwise</returns>
        public static bool IsValid(
            string address)
        {
            return IsValidIPv4(address) || IsValidIPv6(address);
        }

        /// <summary>
        /// Validate the given IPv4 or IPv6 address and netmask.
        /// </summary>
        /// <param name="address">the IP address as a string.</param>
        /// <returns>true if a valid address with netmask, false otherwise</returns>
        public static bool IsValidWithNetMask(
            string address)
        {
            return IsValidIPv4WithNetmask(address) || IsValidIPv6WithNetmask(address);
        }

        /// <summary>
        /// Validate the given IPv4 address.
        /// </summary>
        /// <param name="address">the IP address as a string.</param>
        /// <returns>true if a valid IPv4 address, false otherwise</returns>
        public static bool IsValidIPv4(
            string address)
        {
            try
            {
                return unsafeIsValidIPv4(address);
            }
            catch (FormatException) { }
            catch (OverflowException) { }
            return false;
        }

        /// <summary>
        /// Validate the given IPv4 address.
        /// </summary>
        /// <param name="address">the IP address as a string.</param>
        /// <returns>true if a valid IPv4 address, false otherwise</returns>
        private static bool unsafeIsValidIPv4(
            string address)
        {
            if (address.Length == 0)
                return false;

            int octets = 0;
            string temp = address + ".";

            int pos;
            int start = 0;
            while (start < temp.Length
                && (pos = temp.IndexOf('.', start)) > start)
            {
                if (octets == 4)
                    return false;

                string octetStr = temp.Substring(start, pos - start);
                int octet = Int32.Parse(octetStr);

                if (octet < 0 || octet > 255)
                    return false;

                start = pos + 1;
                octets++;
            }

            return octets == 4;
        }

        /// <summary>
        /// Is valid IPv4 with mask.
        /// </summary>
        /// <param name="address">the IP address as a string.</param>
        /// <returns>true if a valid IPv4 address, false otherwise</returns>
        public static bool IsValidIPv4WithNetmask(
            string address)
        {
            int index = address.IndexOf("/");
            string mask = address.Substring(index + 1);

            return (index > 0) && IsValidIPv4(address.Substring(0, index))
                && (IsValidIPv4(mask) || IsMaskValue(mask, 32));
        }

        /// <summary>
        /// Is valid IPv6 with mask.
        /// </summary>
        /// <param name="address">the IP address as a string.</param>
        /// <returns>true if a valid IPv4 address, false otherwise</returns>
        public static bool IsValidIPv6WithNetmask(
            string address)
        {
            int index = address.IndexOf("/");
            string mask = address.Substring(index + 1);

            return (index > 0) && (IsValidIPv6(address.Substring(0, index))
                && (IsValidIPv6(mask) || IsMaskValue(mask, 128)));
        }

        /// <summary>
        /// Is masked value.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static bool IsMaskValue(
            string component,
            int size)
        {
            int val = Int32.Parse(component);
            try
            {
                return val >= 0 && val <= size;
            }
            catch (FormatException) { }
            catch (OverflowException) { }
            return false;
        }

        /// <summary>
        /// Validate the given IPv6 address.
        /// </summary>
        /// <param name="address">the IP address as a string.</param>
        /// <returns>true if a valid IPv4 address, false otherwise</returns>
        public static bool IsValidIPv6(
            string address)
        {
            try
            {
                return unsafeIsValidIPv6(address);
            }
            catch (FormatException) { }
            catch (OverflowException) { }
            return false;
        }

        /// <summary>
        /// Validate the given IPv6 address.
        /// </summary>
        /// <param name="address">the IP address as a string.</param>
        /// <returns>true if a valid IPv4 address, false otherwise</returns>
        private static bool unsafeIsValidIPv6(
            string address)
        {
            if (address.Length == 0)
            {
                return false;
            }

            int octets = 0;

            string temp = address + ":";
            bool doubleColonFound = false;
            int pos;
            int start = 0;
            while (start < temp.Length
                && (pos = temp.IndexOf(':', start)) >= start)
            {
                if (octets == 8)
                {
                    return false;
                }

                if (start != pos)
                {
                    string value = temp.Substring(start, pos - start);

                    if (pos == (temp.Length - 1) && value.IndexOf('.') > 0)
                    {
                        if (!IsValidIPv4(value))
                        {
                            return false;
                        }

                        octets++; // add an extra one as address covers 2 words.
                    }
                    else
                    {
                        string octetStr = temp.Substring(start, pos - start);
                        int octet = Int32.Parse(octetStr, NumberStyles.AllowHexSpecifier);

                        if (octet < 0 || octet > 0xffff)
                            return false;
                    }
                }
                else
                {
                    if (pos != 1 && pos != temp.Length - 1 && doubleColonFound)
                    {
                        return false;
                    }
                    doubleColonFound = true;
                }
                start = pos + 1;
                octets++;
            }

            return octets == 8 || doubleColonFound;
        }
    }
}
