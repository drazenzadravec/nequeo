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
    /// Abstract base class.
    /// </summary>
    [Serializable]
    public abstract class RecordBase
    {
    }

    /// <summary>
    /// A record validation class.
    /// </summary>
    internal class ARecord : RecordBase
    {
        #region Constructors
        /// <summary>
        /// Constructs an ANAME record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal ARecord(Pointer pointer)
        {
            byte b1 = pointer.ReadByte();
            byte b2 = pointer.ReadByte();
            byte b3 = pointer.ReadByte();
            byte b4 = pointer.ReadByte();

            // this next line's not brilliant - couldn't find a better way though
            _ipAddress = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}", b1, b2, b3, b4));
        }
        #endregion

        #region Internal Fields
        internal IPAddress _ipAddress;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the current A record ip address.
        /// </summary>
        public IPAddress IPAddress
        {
            get { return _ipAddress; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// ToString method override.
        /// </summary>
        /// <returns>The new ip address.</returns>
        public override string ToString()
        {
            return _ipAddress.ToString();
        }
        #endregion
    }

    /// <summary>
    /// MX record validation class.
    /// </summary>
    [Serializable]
    internal class MXRecord : RecordBase, IComparable
    {
        #region Constructors
        /// <summary>
        /// Constructs an MX record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal MXRecord(Pointer pointer)
        {
            _preference = pointer.ReadShort();
            _domainName = pointer.ReadDomain();
        }
        #endregion

        #region Private Fields
        private readonly string _domainName;
        private readonly int _preference;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the current domain name.
        /// </summary>
        public string DomainName 
        { 
            get { return _domainName; } 
        }

        /// <summary>
        /// Gets, the current unique preference.
        /// </summary>
        public int Preference 
        { 
            get { return _preference; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Implements the IComparable interface so that we can sort the MX records by their
        /// lowest preference
        /// </summary>
        /// <param name="obj">the other MxRecord to compare against</param>
        /// <returns>1, 0, -1</returns>
        public int CompareTo(object obj)
        {
            MXRecord mxOther = (MXRecord)obj;

            // we want to be able to sort them by preference
            if (mxOther._preference < _preference) return 1;
            if (mxOther._preference > _preference) return -1;

            // order mail servers of same preference by name
            return -mxOther._domainName.CompareTo(_domainName);
        }
        #endregion

        #region Public Operator Overrides
        /// <summary>
        /// Equality operator, is record 1 the same as record 2.
        /// </summary>
        /// <param name="record1">MX record one.</param>
        /// <param name="record2">MX record two.</param>
        /// <returns>If record 1 is equal to record 2 then true else false.</returns>
        public static bool operator ==(MXRecord record1, MXRecord record2)
        {
            if (record1 == null) throw new ArgumentNullException("record1");

            return record1.Equals(record2);
        }

        /// <summary>
        /// Not equal operator, is record 1 different from record 2.
        /// </summary>
        /// <param name="record1">MX record one.</param>
        /// <param name="record2">MX record two.</param>
        /// <returns>If record 1 is not equal to record 2 then true else false.</returns>
        public static bool operator !=(MXRecord record1, MXRecord record2)
        {
            return !(record1 == record2);
        }

        /// <summary>
        /// Less than operator, is record 1 less than record 2.
        /// </summary>
        /// <param name="record1">MX record one.</param>
        /// <param name="record2">MX record two.</param>
        /// <returns>If record 1 is less than record 2 then true else false.</returns>
        public static bool operator <(MXRecord record1, MXRecord record2)
        {
            if (record1._preference > record2._preference) return false;
            return false;
        }

        /// <summary>
        /// Greater than operator, is record 1 greater than record 2.
        /// </summary>
        /// <param name="record1">MX record one.</param>
        /// <param name="record2">MX record two.</param>
        /// <returns>If record 1 is greater than record 2 then true else false.</returns>
        public static bool operator >(MXRecord record1, MXRecord record2)
        {
            if (record1._preference < record2._preference) return false;
            return false;
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Gets the equality indicator of another MX record class.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if the specified object is equal to this object.</returns>
        public override bool Equals(object obj)
        {
            // this object isn't null
            if (obj == null) return false;

            // must be of same type
            if (this.GetType() != obj.GetType()) return false;

            MXRecord mxOther = (MXRecord)obj;

            // preference must match
            if (mxOther._preference != _preference) return false;

            // and so must the domain name
            if (mxOther._domainName != _domainName) return false;

            // its a match
            return true;
        }

        /// <summary>
        /// Gets the current information on the current class.
        /// </summary>
        /// <returns>The string containg the information.</returns>
        public override string ToString()
        {
            return string.Format("Mail Server = {0}, Preference = {1}", 
                _domainName, _preference.ToString());
        }

        /// <summary>
        /// Gets the current preference index for the current class.
        /// </summary>
        /// <returns>The current preference for the current class.</returns>
        public override int GetHashCode()
        {
            return _preference;
        }
        #endregion
    }

    /// <summary>
    /// NS record validation class.
    /// </summary>
    internal class NSRecord : RecordBase
    {
        #region Constructors
        /// <summary>
        /// Constructs a NS record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal NSRecord(Pointer pointer)
        {
            _domainName = pointer.ReadDomain();
        }
        #endregion

        #region Private Fields
        private readonly string _domainName;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the current domain name.
        /// </summary>
        public string DomainName 
        { 
            get { return _domainName; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Gets the domain name.
        /// </summary>
        /// <returns>The current domain name.</returns>
        public override string ToString()
        {
            return _domainName;
        }
        #endregion
    }

    /// <summary>
    /// SOA record validation class.
    /// </summary>
    internal class SoaRecord : RecordBase
    {
        #region Constructors
        /// <summary>
        /// Constructs an SOA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal SoaRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            _primaryNameServer = pointer.ReadDomain();
            _responsibleMailAddress = pointer.ReadDomain();
            _serial = pointer.ReadInt();
            _refresh = pointer.ReadInt();
            _retry = pointer.ReadInt();
            _expire = pointer.ReadInt();
            _defaultTtl = pointer.ReadInt();
        }
        #endregion

        #region Private Fields
        // these fields constitute an SOA RR
        private readonly string _primaryNameServer;
        private readonly string _responsibleMailAddress;
        private readonly int _serial;
        private readonly int _refresh;
        private readonly int _retry;
        private readonly int _expire;
        private readonly int _defaultTtl;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the primary name server.
        /// </summary>
        public string PrimaryNameServer 
        { 
            get { return _primaryNameServer; } 
        }

        /// <summary>
        /// Gets, the responsible mail address.
        /// </summary>
        public string ResponsibleMailAddress 
        { 
            get { return _responsibleMailAddress; } 
        }

        /// <summary>
        /// Gets, the serial number.
        /// </summary>
        public int Serial 
        { 
            get { return _serial; } 
        }

        /// <summary>
        /// Gets, the refresh time interval.
        /// </summary>
        public int Refresh 
        { 
            get { return _refresh; } 
        }

        /// <summary>
        /// Gets, the retry interval.
        /// </summary>
        public int Retry 
        { 
            get { return _retry; } 
        }

        /// <summary>
        /// Gets, the expire time interval.
        /// </summary>
        public int Expire 
        { 
            get { return _expire; } 
        }

        /// <summary>
        /// Gets, the time to live interval.
        /// </summary>
        public int DefaultTtl 
        { 
            get { return _defaultTtl; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Gets the soa information.
        /// </summary>
        /// <returns>The current soa information.</returns>
        public override string ToString()
        {
            return string.Format("primary name server = {0}\nresponsible mail addr = {1}" +
                "\nserial  = {2}\nrefresh = {3}\nretry   = {4}\nexpire  = {5}\ndefault TTL = {6}",
                _primaryNameServer, _responsibleMailAddress, _serial.ToString(), _refresh.ToString(),
                _retry.ToString(), _expire.ToString(), _defaultTtl.ToString());
        }
        #endregion
    }

    /// <summary>
    /// TXT record validation class.
    /// </summary>
    internal class TxtRecord : RecordBase
    {
        #region Constructors
        /// <summary>
        /// Constructs an TXT record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal TxtRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            _textData = pointer.ReadDomain();
        }
        #endregion

        #region Private Fields
        // these fields constitute an TXT RR
        private readonly string _textData;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the text data.
        /// </summary>
        public string TextData
        {
            get { return _textData; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// ToString method override.
        /// </summary>
        /// <returns>The new ip address.</returns>
        public override string ToString()
        {
            return _textData.ToString();
        }
        #endregion
    }

    /// <summary>
    /// WKS record validation class.
    /// </summary>
    internal class WksRecord : RecordBase
    {
        #region Constructors
        /// <summary>
        /// Constructs an WKS record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal WksRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            _address = pointer.ReadDomain();
            _protocol = pointer.ReadInt();
        }
        #endregion

        #region Private Fields
        // these fields constitute an WKS RR
        private readonly string _address;
        private readonly Int32 _protocol;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the address data.
        /// </summary>
        public string Address
        {
            get { return _address; }
        }

        /// <summary>
        /// Gets, the protocol data.
        /// </summary>
        public Int32 Protocol
        {
            get { return _protocol; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// ToString method override.
        /// </summary>
        /// <returns>The new ip address.</returns>
        public override string ToString()
        {
            return string.Format("Address = {0}\nProtocol = {1}",
                _address, _protocol);
        }
        #endregion
    }

    /// <summary>
    /// PTR record validation class.
    /// </summary>
    internal class PtrRecord : RecordBase
    {
        #region Constructors
        /// <summary>
        /// Constructs an PTR record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal PtrRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            _ptrData = pointer.ReadDomain();
        }
        #endregion

        #region Private Fields
        // these fields constitute an PTR RR
        private readonly string _ptrData;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the pointer data.
        /// </summary>
        public string PointerData
        {
            get { return _ptrData; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// ToString method override.
        /// </summary>
        /// <returns>The new ip address.</returns>
        public override string ToString()
        {
            return _ptrData.ToString();
        }
        #endregion
    }

    /// <summary>
    /// WKS record validation class.
    /// </summary>
    internal class HInfoRecord : RecordBase
    {
        #region Constructors
        /// <summary>
        /// Constructs an WKS record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal HInfoRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            _cpu = pointer.ReadDomain();
            _os = pointer.ReadDomain();
        }
        #endregion

        #region Private Fields
        // these fields constitute an HInfo RR
        private readonly string _cpu;
        private readonly string _os;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the CPU data.
        /// </summary>
        public string CPU
        {
            get { return _cpu; }
        }

        /// <summary>
        /// Gets, the OS data.
        /// </summary>
        public string OS
        {
            get { return _os; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// ToString method override.
        /// </summary>
        /// <returns>The new ip address.</returns>
        public override string ToString()
        {
            return string.Format("CPU = {0}\nOS = {1}", _cpu, _os);
        }
        #endregion
    }

    /// <summary>
    /// Resource record validation class.
    /// </summary>
    [Serializable]
    public class ResourceRecord
    {
        #region Constructors
        /// <summary>
        /// Construct a resource record from a pointer to a byte array
        /// </summary>
        /// <param name="pointer">the position in the byte array of the record</param>
        internal ResourceRecord(Pointer pointer)
        {
            // extract the domain, question type, question class and Ttl
            _domain = pointer.ReadDomain();
            _dnsType = (Nequeo.Net.Dns.DnsType)pointer.ReadShort();
            _dnsClass = (Nequeo.Net.Dns.DnsClass)pointer.ReadShort();
            _Ttl = pointer.ReadInt();

            // the next short is the record length, we only use it for unrecognised record types
            int recordLength = pointer.ReadShort();

            // and create the appropriate RDATA record based on the dnsType
            switch (_dnsType)
            {
                case Nequeo.Net.Dns.DnsType.NS: 
                    // Set the name server record.
                    _record = new NSRecord(pointer); 
                    break;

                case Nequeo.Net.Dns.DnsType.MX:
                    // Set the name mail exchange record.
                    _record = new MXRecord(pointer); 
                    break;

                case Nequeo.Net.Dns.DnsType.A:
                    // Set the name A name record.
                    _record = new ARecord(pointer); 
                    break;

                case Nequeo.Net.Dns.DnsType.SOA:
                    // Set the name SOA record.
                    _record = new SoaRecord(pointer); 
                    break;

                case Nequeo.Net.Dns.DnsType.TXT:
                    // Set the name TXT record.
                    _record = new TxtRecord(pointer);
                    break;

                case Nequeo.Net.Dns.DnsType.WKS:
                    // Set the name WKS record.
                    _record = new WksRecord(pointer);
                    break;

                case Nequeo.Net.Dns.DnsType.PTR:
                    // Set the name PTR record.
                    _record = new PtrRecord(pointer);
                    break;

                case Nequeo.Net.Dns.DnsType.HINFO:
                    // Set the name HINFO record.
                    _record = new HInfoRecord(pointer);
                    break;

                default:
                    // move the pointer over this unrecognised record
                    pointer += recordLength;
                    break;
            }
        }
        #endregion

        #region Private Fields
        // private, constructor initialised fields
        private readonly string _domain;
        private readonly Nequeo.Net.Dns.DnsType _dnsType;
        private readonly Nequeo.Net.Dns.DnsClass _dnsClass;
        private readonly int _Ttl;
        private readonly RecordBase _record;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the domain.
        /// </summary>
        public string Domain 
        { 
            get { return _domain; } 
        }

        /// <summary>
        /// Gets, the dns record type.
        /// </summary>
        public Nequeo.Net.Dns.DnsType Type 
        { 
            get { return _dnsType; } 
        }

        /// <summary>
        /// Gets, the dns class type.
        /// </summary>
        public Nequeo.Net.Dns.DnsClass Class 
        { 
            get { return _dnsClass; } 
        }

        /// <summary>
        /// Gets, the time to live value.
        /// </summary>
        public int Ttl 
        { 
            get { return _Ttl; } 
        }

        /// <summary>
        /// Gets, the record base abstract class.
        /// </summary>
        public RecordBase Record 
        { 
            get { return _record; } 
        }
        #endregion   
    }

    /// <summary>
    /// Answer class record information.
    /// </summary>
    [Serializable]
    public class Answer : ResourceRecord
    {
        internal Answer(Pointer pointer) 
            : base(pointer) { }
    }

    /// <summary>
    /// Name server class record information.
    /// </summary>
    [Serializable]
    public class NameServer : ResourceRecord
    {
        internal NameServer(Pointer pointer) 
            : base(pointer) { }
    }

    /// <summary>
    /// Additional record class record information.
    /// </summary>
    [Serializable]
    public class AdditionalRecord : ResourceRecord
    {
        internal AdditionalRecord(Pointer pointer) 
            : base(pointer) { }
    }
}
