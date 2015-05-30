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
    /// A Request logically consists of a number of questions to ask the DNS Server. Create a request and
    /// add questions to it, then pass the request to Resolver.Lookup to query the DNS Server. It is important
    /// to note that many DNS Servers DO NOT SUPPORT MORE THAN 1 QUESTION PER REQUEST, and it is advised that
    /// you only add one question to a request. If not ensure you check Response.ReturnCode to see what the
    /// server has to say about it.
    /// </summary>
    internal class Request
    {
        #region Constructors
        /// <summary>
        /// Construct this object with the default values and create an ArrayList to hold
        /// the questions as they are added
        /// </summary>
        public Request()
        {
            // default for a request is that recursion is desired and using standard query
            _recursionDesired = true;
            _opCode = Nequeo.Net.Dns.Opcode.StandardQuery;

            // create an expandable list of questions
            _questions = new List<Question>();
        }
        #endregion

        #region Private Fields
        private List<Question> _questions;
        private bool _recursionDesired;
        private Nequeo.Net.Dns.Opcode _opCode;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the recursion desired indicator.
        /// </summary>
        public bool RecursionDesired
        {
            get { return _recursionDesired; }
            set { _recursionDesired = value; }
        }

        /// <summary>
        /// Gets sets, the current operation code.
        /// </summary>
        public Nequeo.Net.Dns.Opcode Opcode
        {
            get { return _opCode; }
            set { _opCode = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a question to the request to be sent to the DNS server.
        /// </summary>
        /// <param name="question">The question to add to the request</param>
        public virtual void AddQuestion(Question question)
        {
            // abandon if null
            if (question == null) throw new ArgumentNullException("question");

            // add this question to our collection
            _questions.Add(question);
        }

        /// <summary>
        /// Convert this request into a byte array ready to send direct to the DNS server
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetMessage()
        {
            // construct a message for this request. This will be a byte array but we're using
            // an arraylist as we don't know how big it will be
            List<byte> data = new List<byte>();

            // the id of this message - this will be filled in by the resolver
            data.Add((byte)0);
            data.Add((byte)0);

            // write the bitfields
            data.Add((byte)(((byte)_opCode << 3) | (_recursionDesired ? 0x01 : 0)));
            data.Add((byte)0);

            // tell it how many questions
            unchecked
            {
                data.Add((byte)(_questions.Count >> 8));
                data.Add((byte)_questions.Count);
            }

            // the are no requests, name servers or additional records in a request
            data.Add((byte)0); data.Add((byte)0);
            data.Add((byte)0); data.Add((byte)0);
            data.Add((byte)0); data.Add((byte)0);

            // that's the header done - now add the questions
            foreach (Question question in _questions)
            {
                AddDomain(data, question.Domain);
                unchecked
                {
                    data.Add((byte)0);
                    data.Add((byte)question.Type);
                    data.Add((byte)0);
                    data.Add((byte)question.Class);
                }
            }

            // and convert that to an array
            byte[] message = new byte[data.Count];
            data.CopyTo(message);
            return message;
        }
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Adds a domain name to the ArrayList of bytes. This implementation does not use
        /// the domain name compression used in the class Pointer - maybe it should.
        /// </summary>
        /// <param name="data">The ArrayList representing the byte array message</param>
        /// <param name="domainName">the domain name to encode and add to the array</param>
        private static void AddDomain(List<byte> data, string domainName)
        {
            int position = 0;
            int length = 0;

            // start from the beginning and go to the end
            while (position < domainName.Length)
            {
                // look for a period, after where we are
                length = domainName.IndexOf('.', position) - position;

                // if there isn't one then this labels length is to the end of the string
                if (length < 0) length = domainName.Length - position;

                // add the length
                data.Add((byte)length);

                // copy a char at a time to the array
                while (length-- > 0)
                {
                    data.Add((byte)domainName[position++]);
                }

                // step over '.'
                position++;
            }

            // end of domain names
            data.Add((byte)0);
        }
        #endregion
    }

    /// <summary>
    /// A Response is a logical representation of the 
    /// byte data returned from a DNS query
    /// </summary>
    public sealed class Response
    {
        #region Constructor
        /// <summary>
        /// Construct a Response object from the supplied byte array
        /// </summary>
        /// <param name="message">a byte array returned from a DNS server query</param>
        internal Response(byte[] message)
        {
            // the bit flags are in bytes 2 and 3
            byte flags1 = message[2];
            byte flags2 = message[3];

            // get return code from lowest 4 bits of byte 3
            int returnCode = flags2 & 15;

            // if its in the reserved section, set to other
            if (returnCode > 6) returnCode = 6;
            _returnCode = (Nequeo.Net.Dns.ReturnCode)returnCode;

            // other bit flags
            _authoritativeAnswer = ((flags1 & 4) != 0);
            _recursionAvailable = ((flags2 & 128) != 0);
            _truncated = ((flags1 & 2) != 0);

            // create the arrays of response objects
            _questions = new Question[GetShort(message, 4)];
            _answers = new Answer[GetShort(message, 6)];
            _nameServers = new NameServer[GetShort(message, 8)];
            _additionalRecords = new AdditionalRecord[GetShort(message, 10)];

            // need a pointer to do this, position just after the header
            Pointer pointer = new Pointer(message, 12);

            // and now populate them, they always follow this order
            for (int index = 0; index < _questions.Length; index++)
            {
                try
                {
                    // try to build a quesion from the response
                    _questions[index] = new Question(pointer);
                }
                catch (Exception ex)
                {
                    // something grim has happened, we can't continue
                    throw new Exception(ex.Message);
                }
            }
            for (int index = 0; index < _answers.Length; index++)
            {
                _answers[index] = new Answer(pointer);
            }
            for (int index = 0; index < _nameServers.Length; index++)
            {
                _nameServers[index] = new NameServer(pointer);
            }
            for (int index = 0; index < _additionalRecords.Length; index++)
            {
                _additionalRecords[index] = new AdditionalRecord(pointer);
            }
        }
        #endregion

        #region Private Fields
        private readonly Nequeo.Net.Dns.ReturnCode _returnCode;
        private readonly bool _authoritativeAnswer;
        private readonly bool _recursionAvailable;
        private readonly bool _truncated;
        private readonly Question[] _questions;
        private readonly Answer[] _answers;
        private readonly NameServer[] _nameServers;
        private readonly AdditionalRecord[] _additionalRecords;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the response return code.
        /// </summary>
        public Nequeo.Net.Dns.ReturnCode ReturnCode
        {
            get { return _returnCode; }
        }

        /// <summary>
        /// Gets, the authoritative answer indicator.
        /// </summary>
        public bool AuthoritativeAnswer
        {
            get { return _authoritativeAnswer; }
        }

        /// <summary>
        /// Gets, the recursion available indicator.
        /// </summary>
        public bool RecursionAvailable
        {
            get { return _recursionAvailable; }
        }

        /// <summary>
        /// Gets, the message truncated indicator.
        /// </summary>
        public bool MessageTruncated
        {
            get { return _truncated; }
        }

        /// <summary>
        /// Gets, the collection of questions from the response.
        /// </summary>
        public Question[] Questions
        {
            get { return _questions; }
        }

        /// <summary>
        /// Gets, the collection of answers from the response.
        /// </summary>
        public Answer[] Answers
        {
            get { return _answers; }
        }

        /// <summary>
        /// Gets, the collection of name servers from the response.
        /// </summary>
        public NameServer[] NameServers
        {
            get { return _nameServers; }
        }

        /// <summary>
        /// Gets, the collection of additional records from the response.
        /// </summary>
        public AdditionalRecord[] AdditionalRecords
        {
            get { return _additionalRecords; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Convert 2 bytes to a short. It would have been nice to use BitConverter for this,
        /// it however reads the bytes in the wrong order (at least on Windows)
        /// </summary>
        /// <param name="message">byte array to look in</param>
        /// <param name="position">position to look at</param>
        /// <returns>short representation of the two bytes</returns>
        private short GetShort(byte[] message, int position)
        {
            return (short)(message[position] << 8 | message[position + 1]);
        }
        #endregion
    }

    /// <summary>
    /// Summary description for Dns.
    /// </summary>
    internal sealed class Resolver
    {
        #region Private Fields
        private int _uniqueId;
        private const int _udpRetryAttempts = 2;
        #endregion

        #region Internal Methods
        /// <summary>
        /// Shorthand form to make MX querying easier, essentially wraps up the retreival
        /// of the MX records, and sorts them by preference
        /// </summary>
        /// <param name="domain">domain name to retreive MX RRs for</param>
        /// <param name="port">the port number used by the domain name server.</param>
        /// <param name="ipAddress">the ip address of the domain name server.</param>
        /// <param name="protocolType">the protocol type used by the domain name server.</param>
        /// <param name="useIPv4EndPoint">use only an IPv4 connection and disregard all other address families (IPv6).</param>
        /// <returns>An array of MXRecords</returns>
        internal MXRecord[] MXLookup(string domain, int port,
            string ipAddress, Nequeo.Net.Dns.ProtocolType protocolType, bool useIPv4EndPoint)
        {
            // check the inputs
            if (domain == null) throw new ArgumentNullException("domain");
            if (port < 1) throw new IndexOutOfRangeException("port");

            // create a request for this
            Request request = new Request();

            // add one question - the MX IN lookup for the supplied domain
            request.AddQuestion(new Question(domain, Nequeo.Net.Dns.DnsType.MX, Nequeo.Net.Dns.DnsClass.IN));

            // fire it off
            Response response = Lookup(request, domain, port, ipAddress, protocolType, useIPv4EndPoint);

            // if we didn't get a response, then return null
            if (response == null) return null;

            // create a growable array of MX records
            ArrayList resourceRecords = new ArrayList();

            // add each of the answers to the array
            foreach (Answer answer in response.Answers)
            {
                // if the answer is an MX record
                if (answer.Record.GetType() == typeof(MXRecord))
                {
                    // add it to our array
                    resourceRecords.Add(answer.Record);
                }
            }

            // create array of MX records
            MXRecord[] mxRecords = new MXRecord[resourceRecords.Count];

            // copy from the array list
            resourceRecords.CopyTo(mxRecords);

            // sort into lowest preference order
            Array.Sort(mxRecords);

            // and return
            return mxRecords;
        }

        /// <summary>
        /// The principal look up function, which sends a request message to the given
        /// DNS server and collects a response. This implementation re-sends the message
        /// via UDP up to two times in the event of no response/packet loss
        /// </summary>
        /// <param name="request">The logical request to send to the server</param>
        /// <param name="domain">domain name to retreive MX RRs for</param>
        /// <param name="port">the port number used by the domain name server.</param>
        /// <param name="ipAddress">the ip address of the domain name server.</param>
        /// <param name="protocolType">the protocol type used by the domain name server.</param>
        /// <param name="useIPv4EndPoint">use only an IPv4 connection and disregard all other address families (IPv6).</param>
        /// <returns>The logical response from the DNS server or null if no response</returns>
        internal Response Lookup(Request request, string domain, int port,
            string ipAddress, Nequeo.Net.Dns.ProtocolType protocolType, bool useIPv4EndPoint)
        {
            // check the inputs
            if (request == null) throw new ArgumentNullException("request");
            if (port < 1) throw new IndexOutOfRangeException("port");

            // get the message
            byte[] requestMessage = request.GetMessage();

            // send the request and get the response
            byte[] responseMessage = Transfer(domain, port, ipAddress, protocolType, requestMessage, useIPv4EndPoint);

            string ff = Encoding.ASCII.GetString(responseMessage);

            // and populate a response object from that and return it
            return new Response(responseMessage);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the bytes of data from the domain name server socket connection.
        /// </summary>
        /// <param name="domain">domain name to retreive MX RRs for</param>
        /// <param name="port">the port number used by the domain name server.</param>
        /// <param name="ipAddress">the ip address of the domain name server.</param>
        /// <param name="protocolType">the protocol type used by the domain name server.</param>
        /// <param name="useIPv4EndPoint">use only an IPv4 connection and disregard all other address families (IPv6).</param>
        /// <param name="requestMessage"></param>
        /// <returns>The bytes received from the domain name server.</returns>
        private byte[] Transfer(string domain, int port, string ipAddress,
            Nequeo.Net.Dns.ProtocolType protocolType, byte[] requestMessage, bool useIPv4EndPoint)
        {
            // UDP can fail - if it does try again keeping track of how many attempts we've made
            int attempts = 0;

            // try repeatedly in case of failure
            while (attempts <= _udpRetryAttempts)
            {
                // firstly, uniquely mark this request with an id
                unchecked
                {
                    // substitute in an id unique to this lookup, the request has no idea about this
                    requestMessage[0] = (byte)(_uniqueId >> 8);
                    requestMessage[1] = (byte)_uniqueId;
                }

                // we'll be send and receiving a UDP packet
                Socket socket = GetSocket(ipAddress, port, protocolType, useIPv4EndPoint);

                // we will wait at most 10 second for a dns reply
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

                // send it off to the server
                socket.Send(requestMessage, requestMessage.Length, SocketFlags.None);


                // RFC1035 states that the maximum size of a UDP datagram is 512 octets (bytes)
                byte[] responseMessage = new byte[512];

                try
                {
                    // wait for a response upto 10 second
                    socket.Receive(responseMessage);

                    // make sure the message returned is ours
                    if (responseMessage[0] == requestMessage[0] &&
                        responseMessage[1] == requestMessage[1])
                    {
                        // its a valid response - return it, this is our successful exit point
                        return responseMessage;
                    }
                }
                catch (SocketException)
                {
                    // failure - we better try again, but remember how many attempts
                    attempts++;
                }
                finally
                {
                    // increase the unique id
                    _uniqueId++;

                    // close the socket
                    socket.Close();
                }
            }

            // the operation has failed, this is our unsuccessful exit point
            throw new Exception("A connection could not be establised. " +
                "The server may have actively refused the request.");
        }

        /// <summary>
        /// Get the client connection socket.
        /// </summary>
        /// <param name="ipAddress">the ip address of the domain name server.</param>
        /// <param name="port">the port number used by the domain name server.</param>
        /// <param name="protocolType">the protocol type used by the domain name server.</param>
        /// <param name="useIPv4EndPoint">use only an IPv4 connection and disregard all other address families (IPv6).</param>
        /// <returns>The client connection socket.</returns>
        private Socket GetSocket(string ipAddress, int port,
            Nequeo.Net.Dns.ProtocolType protocolType, bool useIPv4EndPoint)
        {
            Socket socket = null;

            try
            {
                IPHostEntry hostEntry = null;

                // Get host related information.
                hostEntry = System.Net.Dns.GetHostEntry(ipAddress);

                // Loop through the AddressList to obtain the supported 
                // AddressFamily. This is to avoid an exception that 
                // occurs when the host IP Address is not compatible 
                // with the address family 
                // (typical in the IPv6 case).
                foreach (IPAddress address in hostEntry.AddressList)
                {
                    // If the connection used should only be IPv4.
                    if (useIPv4EndPoint && address.AddressFamily == AddressFamily.InterNetworkV6)
                        continue;

                    // Get the current server endpoint for
                    // the current address.
                    IPEndPoint endPoint = new IPEndPoint(address, port);
                    Socket tempSocket = null;

                    // Get the current socket type.
                    switch (protocolType)
                    {
                        case Nequeo.Net.Dns.ProtocolType.Tcp:
                            // Create a new client socket for the
                            // current endpoint.
                            tempSocket = new Socket(endPoint.AddressFamily,
                                SocketType.Dgram, System.Net.Sockets.ProtocolType.Tcp);
                            break;

                        case Nequeo.Net.Dns.ProtocolType.Udp:
                            // Create a new client socket for the
                            // current endpoint.
                            tempSocket = new Socket(endPoint.AddressFamily,
                                SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
                            break;
                    }

                    // Connect to the server with the
                    // current end point.
                    try
                    {
                        tempSocket.Connect(endPoint);
                    }
                    catch { }

                    // If this connection succeeded then
                    // asiign the client socket and
                    // break put of the loop.
                    if (tempSocket.Connected)
                    {
                        // A client connection has been found.
                        // Break out of the loop.
                        socket = tempSocket;
                        break;
                    }
                    else continue;
                }

                // Return the client socket.
                return socket;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
    }
}
