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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Net.Dns
{
    /// <summary>
    /// Logical representation of a pointer, but in fact a byte array reference and a position in it. This
    /// is used to read logical units (bytes, shorts, integers, domain names etc.) from a byte array, keeping
    /// the pointer updated and pointing to the next record. This type of Pointer can be considered the logical
    /// equivalent of an (unsigned char*) in C++
    /// </summary>
    internal class Pointer
    {
        #region Constructors
        // pointers can only be created by passing on an existing message
        public Pointer(byte[] message, int position)
        {
            _message = message;
            _position = position;
        }
        #endregion

        #region Private Fields
        // a pointer is a reference to the message and an index
        private byte[] _message;
        private int _position;
        #endregion

        #region Public Methods
        /// <summary>
        /// Shallow copy function
        /// </summary>
        /// <returns></returns>
        public Pointer Copy()
        {
            return new Pointer(_message, _position);
        }

        /// <summary>
        /// Adjust the pointers position within the message
        /// </summary>
        /// <param name="position">new position in the message</param>
        public void SetPosition(int position)
        {
            _position = position;
        }

        /// <summary>
        /// Overloads the + operator to allow advancing the pointer by so many bytes
        /// </summary>
        /// <param name="pointer">the initial pointer</param>
        /// <param name="offset">the offset to add to the pointer in bytes</param>
        /// <returns>a reference to a new pointer moved forward by offset bytes</returns>
        public static Pointer operator +(Pointer pointer, int offset)
        {
            return new Pointer(pointer._message, pointer._position + offset);
        }

        /// <summary>
        /// Reads a single byte at the current pointer, does not advance pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public byte Peek()
        {
            return _message[_position];
        }

        /// <summary>
        /// Reads a single byte at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public byte ReadByte()
        {
            return _message[_position++];
        }

        /// <summary>
        /// Reads two bytes to form a short at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public short ReadShort()
        {
            return (short)(ReadByte() << 8 | ReadByte());
        }

        /// <summary>
        /// Reads four bytes to form a int at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public int ReadInt()
        {
            return (ushort)ReadShort() << 16 | (ushort)ReadShort();
        }

        /// <summary>
        /// Reads a single byte as a char at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public char ReadChar()
        {
            return (char)ReadByte();
        }

        /// <summary>
        /// Reads a domain name from the byte array. The method by which this works is described
        /// in RFC1035 - 4.1.4. Essentially to minimise the size of the message, if part of a domain
        /// name already been seen in the message, rather than repeating it, a pointer to the existing
        /// definition is used. Each word in a domain name is a label, and is preceded by its length
        /// 
        /// eg. bigdevelopments.co.uk
        /// 
        /// is [15] (size of bigdevelopments) + "bigdevelopments"
        ///    [2]  "co"
        ///    [2]  "uk"
        ///    [1]  0 (NULL)
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public string ReadDomain()
        {
            StringBuilder domain = new StringBuilder();
            int length = 0;

            // get  the length of the first label
            while ((length = ReadByte()) != 0)
            {
                // top 2 bits set denotes domain name compression and to reference elsewhere
                if ((length & 0xc0) == 0xc0)
                {
                    // work out the existing domain name, copy this pointer
                    Pointer newPointer = Copy();

                    // and move it to where specified here
                    newPointer.SetPosition((length & 0x3f) << 8 | ReadByte());

                    // repeat call recursively
                    domain.Append(newPointer.ReadDomain());
                    return domain.ToString();
                }

                // if not using compression, copy a char at a time to the domain name
                while (length > 0)
                {
                    domain.Append(ReadChar());
                    length--;
                }

                // if size of next label isn't null (end of domain name) add a period ready for next label
                if (Peek() != 0) domain.Append('.');
            }

            // and return
            return domain.ToString();
        }
        #endregion
    }

    /// <summary>
    /// Represents a DNS Question, comprising of a domain to query, the type of query (QTYPE) and the class
    /// of query (QCLASS). This class is an encapsulation of these three things, and extensive argument checking
    /// in the constructor as this may well be created outside the assembly (public protection)
    /// </summary>
    [Serializable]
    public sealed class Question
    {
        #region Constructors
        /// <summary>
        /// Construct the question reading from a DNS Server response. Consult RFC1035 4.1.2
        /// for byte-wise details of this structure in byte array form
        /// </summary>
        /// <param name="pointer">a logical pointer to the Question in byte array form</param>
        internal Question(Pointer pointer)
        {
            // extract from the message
            _domain = pointer.ReadDomain();
            _dnsType = (Nequeo.Net.Dns.DnsType)pointer.ReadShort();
            _dnsClass = (Nequeo.Net.Dns.DnsClass)pointer.ReadShort();
        }

        /// <summary>
        /// Construct the question from parameters, checking for safety
        /// </summary>
        /// <param name="domain">the domain name to query eg. bigdevelopments.co.uk</param>
        /// <param name="dnsType">the QTYPE of query eg. DnsType.MX</param>
        /// <param name="dnsClass">the CLASS of query, invariably DnsClass.IN</param>
        internal Question(string domain, Nequeo.Net.Dns.DnsType dnsType,
            Nequeo.Net.Dns.DnsClass dnsClass)
        {
            // check the input parameters
            if (domain == null) throw new ArgumentNullException("domain");

            // do a sanity check on the domain name to make sure its legal
            if (domain.Length == 0 || domain.Length > 255 || !Regex.IsMatch(domain, @"^[a-z|A-Z|0-9|-|_]{1,63}(\.[a-z|A-Z|0-9|-|_]{1,63})+$"))
            {
                // domain names can't be bigger tan 255 chars, and individal labels can't be bigger than 63 chars
                throw new ArgumentException("The supplied domain name was not in the correct form", "domain");
            }

            // sanity check the DnsType parameter
            if (!Enum.IsDefined(typeof(Nequeo.Net.Dns.DnsType), dnsType) || dnsType == Nequeo.Net.Dns.DnsType.None)
            {
                throw new ArgumentOutOfRangeException("dnsType", "Not a valid value");
            }

            // sanity check the DnsClass parameter
            if (!Enum.IsDefined(typeof(Nequeo.Net.Dns.DnsClass), dnsClass) || dnsClass == Nequeo.Net.Dns.DnsClass.None)
            {
                throw new ArgumentOutOfRangeException("dnsClass", "Not a valid value");
            }

            // just remember the values
            _domain = domain;
            _dnsType = dnsType;
            _dnsClass = dnsClass;
        }
        #endregion

        #region Private Fields
        // A question is these three things combined
        private readonly string _domain;
        private readonly Nequeo.Net.Dns.DnsType _dnsType;
        private readonly Nequeo.Net.Dns.DnsClass _dnsClass;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the current domain.
        /// </summary>
        public string Domain 
        { 
            get { return _domain; } 
        }

        /// <summary>
        /// Gets, the current dns record type.
        /// </summary>
        public Nequeo.Net.Dns.DnsType Type
        { 
            get { return _dnsType; } 
        }

        /// <summary>
        /// Gets, the current dns class type.
        /// </summary>
        public Nequeo.Net.Dns.DnsClass Class 
        { 
            get { return _dnsClass; } 
        }
        #endregion
    }

    /// <summary>
    /// Class the contians root domain server information.
    /// </summary>
    public sealed class RootNameServers
    {
        #region Private Fields
        private static List<string> _RootServers = new List<string>()
        {
            "m.root-servers.net", "a.root-servers.net", "b.root-servers.net",
            "c.root-servers.net", "d.root-servers.net", "e.root-servers.net",
            "f.root-servers.net", "g.root-servers.net", "h.root-servers.net",
            "i.root-servers.net", "j.root-servers.net", "k.root-servers.net",
            "l.root-servers.net"
        };

        private static List<string> _RootIPAddresses = new List<string>()
        {
            "202.12.27.33", "198.41.0.4", "192.228.79.201",
            "192.33.4.12", "128.8.10.90", "192.203.230.10",
            "192.5.5.241", "192.112.36.4", "128.63.2.53",
            "192.36.148.17", "192.58.128.30", "193.0.14.129",
            "198.32.64.12"
        };
        #endregion

        #region Public Static
        /// <summary>
        /// Gets, the collection of root domain name servers.
        /// </summary>
        public static List<string> RootServers
        {
            get
            {
                // Return the collection of root
                // domain name servers.
                return _RootServers;
            }
        }

        /// <summary>
        /// Gets, the collection of root domain name servers ip addresses
        /// </summary>
        public static List<string> RootIPAddresses
        {
            get
            {
                // Return the collection of root
                // domain name servers ip address.
                return _RootIPAddresses;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class the contians whois server information.
    /// </summary>
    public sealed class WhoisServers
    {
        #region Private Fields
        /// <summary>
        /// North America Root.
        /// </summary>
        public readonly static List<string> NorthAmerica = new List<string>()
        {
            // North America
            "whois.arin.net", 
        };

        /// <summary>
        /// Europe, Middle East Root
        /// </summary>
        public readonly static List<string> EuropeMiddleEast = new List<string>()
        {
            // Europe, Middle East
            "whois.ripe.net", 
        };

        /// <summary>
        /// Asia, Pasific Root
        /// </summary>
        public readonly static List<string> AsiaPacific = new List<string>()
        {
            // Asia, Pacific
            "whois.apnic.net", 
        };

        /// <summary>
        /// South America Root
        /// </summary>
        public readonly static List<string> SouthAmerica = new List<string>()
        {
            // South America
            "whois.lacnic.net",
        };

        /// <summary>
        /// Africa Root
        /// </summary>
        public readonly static List<string> Africa = new List<string>()
        {
            // Africa
            "whois.afrinic.net", 
        };

        /// <summary>
        /// Australian AU.
        /// </summary>
        public readonly static List<string> Australia = new List<string>()
        {
            "whois.audns.net.au", "whois.aunic.net", "whois.ausregistry.net", "whois.net.au",  
        };

        /// <summary>
        /// New Zealand NZ.
        /// </summary>
        public readonly static List<string> NewZealand = new List<string>()
        {
            "srs-ak.srs.net.nz", 
        };

        /// <summary>
        /// United Kingdom UK.
        /// </summary>
        public readonly static List<string> UnitedKingdom = new List<string>()
        {
            "whois.nic.uk",
        };

        /// <summary>
        /// Education EDU.
        /// </summary>
        public readonly static List<string> Education = new List<string>()
        {
            "whois.educause.net",
        };

        /// <summary>
        /// Government GOV.
        /// </summary>
        public readonly static List<string> Government = new List<string>()
        {
            "whois.nic.gov",
        };

        /// <summary>
        /// Top-Level General COM, NET, ORG, EDU, BIZ, INFO.
        /// </summary>
        public readonly static List<string> TopLevelGeneral = new List<string>()
        {
            "whois.internic.net", "whois.iana.org",
        };

        /// <summary>
        /// Whois country-code (two-letter) top-level domains.
        /// </summary>
        public readonly static List<string> CountryCodeGeneral = new List<string>()
        {
            "uwhois.com",
        };

        /// <summary>
        /// General whois servers.
        /// </summary>
        public readonly static List<string> General = new List<string>()
        {
            "whois.afilias.net", "whois.verisign-grs.com",
            "whois.dot.tk", "whois.publicinterestregistry.net", 
        };
        #endregion

        #region Public Static
        /// <summary>
        /// Gets, the collection of all whois servers.
        /// </summary>
        public static List<string> AllWhoisServers
        {
            get
            {
                List<string> listWS = new List<string>();
                listWS.AddRange(NorthAmerica);
                listWS.AddRange(EuropeMiddleEast);
                listWS.AddRange(AsiaPacific);
                listWS.AddRange(SouthAmerica);
                listWS.AddRange(Africa);
                listWS.AddRange(Australia);
                listWS.AddRange(NewZealand);
                listWS.AddRange(UnitedKingdom);
                listWS.AddRange(Education);
                listWS.AddRange(Government);
                listWS.AddRange(TopLevelGeneral);
                listWS.AddRange(CountryCodeGeneral);
                listWS.AddRange(General);

                // Return the collection of
                // whois servers.
                return listWS;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class that contains the domain name server information.
    /// </summary>
    public sealed class DomainNameServer
    {
        #region Private Fields
        private string _nameServer = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the domain name server.
        /// </summary>
        public string DNS
        {
            get { return _nameServer; }
            set { _nameServer = value; }
        }
        #endregion
    }

    /// <summary>
    /// Command argument class containing event handler
    /// information for the server command delegate.
    /// </summary>
    public class ClientCommandArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the command event argument.
        /// </summary>
        /// <param name="command">The command that is received from the server.</param>
        /// <param name="data">The data that is received from the server.</param>
        /// <param name="code">The code that is received from the server.</param>
        public ClientCommandArgs(string command, string data, long code)
        {
            this.command = command;
            this.data = data;
            this.code = code;
        }
        #endregion

        #region Private Fields
        // The command that is received from the server.
        private string command = string.Empty;
        // The data that is received from the server.
        private string data = string.Empty;
        // The code that is received from the server.
        private long code = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the command that is received from the server.
        /// </summary>
        public string Command
        {
            get { return command; }
        }

        /// <summary>
        /// Contains the data that is received from the server.
        /// </summary>
        public string Data
        {
            get { return data; }
        }

        /// <summary>
        /// Contains the code that is received from the server.
        /// </summary>
        public long Code
        {
            get { return code; }
        }
        #endregion
    }

    /// <summary>
    /// This enum holds the protocol type.
    /// </summary>
    public enum ProtocolType
    {
        /// <summary>
        /// Tcp connection protocol type.
        /// </summary>
        Tcp = 0,
        /// <summary>
        /// Udp connection protocol type.
        /// </summary>
        Udp = 1
    }

    /// <summary>
    /// This enum holds the dns type.
    /// </summary>
    public enum DnsType
    {
        /// <summary>
        /// No type specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// ANAME record type.
        /// </summary>
        A = 1,
        /// <summary>
        /// Name server record type.
        /// </summary>
        NS = 2,
        /// <summary>
        /// Server authority record type.
        /// </summary>
        SOA = 6,
        /// <summary>
        /// A well known service description record type.
        /// </summary>
        WKS = 11,
        /// <summary>
        /// A domain name pointer record type.
        /// </summary>
        PTR = 12,
        /// <summary>
        /// Host information record type.
        /// </summary>
        HINFO = 13,
        /// <summary>
        /// Mail exchange record type.
        /// </summary>
        MX = 15,
        /// <summary>
        /// Text strings record type.
        /// </summary>
        TXT = 16,
        /// <summary>
        /// IP v6 A record type.
        /// </summary>
        AAAA = 17
    }

    /// <summary>
    /// This enum holds the dns class type.
    /// </summary>
    public enum DnsClass
    {
        /// <summary>
        /// No type specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// Internet class type.
        /// </summary>
        IN = 1,
        /// <summary>
        /// CSNET class type.
        /// </summary>
        CS = 2,
        /// <summary>
        /// CHAOS class type.
        /// </summary>
        CH = 3,
        /// <summary>
        /// Hesiod class type.
        /// </summary>
        HS = 4
    }

    /// <summary>
    /// This enum holds the return code type.
    /// </summary>
    public enum ReturnCode
    {
        /// <summary>
        /// Successful response type.
        /// </summary>
        Success = 0,
        /// <summary>
        /// Request format error response type.
        /// </summary>
        FormatError = 1,
        /// <summary>
        /// Server failure response type.
        /// </summary>
        ServerFailure = 2,
        /// <summary>
        /// Request name error response type.
        /// </summary>
        NameError = 3,
        /// <summary>
        /// Not implemented response type.
        /// </summary>
        NotImplemented = 4,
        /// <summary>
        /// Request refused response type.
        /// </summary>
        Refused = 5,
        /// <summary>
        /// Other response type.
        /// </summary>
        Other = 6
    }

    /// <summary>
    /// This enum holds the operation code type.
    /// </summary>
    public enum Opcode
    {
        /// <summary>
        /// Standard query request type.
        /// </summary>
        StandardQuery = 0,
        /// <summary>
        /// Inverse query request type.
        /// </summary>
        InverseQuery = 1,
        /// <summary>
        /// Status request type.
        /// </summary>
        StatusRequest = 2
    }
}
