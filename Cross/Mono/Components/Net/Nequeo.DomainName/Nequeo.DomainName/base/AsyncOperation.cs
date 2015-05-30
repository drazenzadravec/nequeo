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
    /// Class for asynchronous domain name operations.
    /// </summary>
    internal class AsyncDnsQueryRequest : Nequeo.Threading.AsynchronousResult<Response>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="dnsType">The dns record type to return.</param>
        /// <param name="dnsClass">The class to search in.</param>
        /// <param name="client">The current domain name client reference.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDnsQueryRequest(string domain, Nequeo.Net.Dns.DnsType dnsType,
            Nequeo.Net.Dns.DnsClass dnsClass, DomainNameClient client, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _domain = domain;
            _dnsType = dnsType;
            _dnsClass = dnsClass;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncQueryRequestThread1));
            Thread.Sleep(20);
        }

        private Nequeo.Net.Dns.DnsType _dnsType = Nequeo.Net.Dns.DnsType.NS;
        private Nequeo.Net.Dns.DnsClass _dnsClass = Nequeo.Net.Dns.DnsClass.None;
        private string _domain = string.Empty;
        private DomainNameClient _client = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncQueryRequestThread1(Object stateInfo)
        {
            // Get the query result.
            Response data = _client.Query(_domain, _dnsType, _dnsClass);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous domain name operations.
    /// </summary>
    internal class AsyncDomainNameServerRequest : Nequeo.Threading.AsynchronousResult<DomainNameServer[]>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="client">The current domain name client reference.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDomainNameServerRequest(string domain,
            DomainNameClient client, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _domain = domain;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDomainNameServerThread1));
            Thread.Sleep(20);
        }

        private string _domain = string.Empty;
        private DomainNameClient _client = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDomainNameServerThread1(Object stateInfo)
        {
            // Get the query result.
            DomainNameServer[] data = _client.GetDomainNameServers(_domain);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data != null)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous whois operations.
    /// </summary>
    internal class AsyncWhoisQueryRequest : Nequeo.Threading.AsynchronousResult<String>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous query request operation.
        /// </summary>
        /// <param name="domain">The domain to get records for.</param>
        /// <param name="client">The current whois client reference.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncWhoisQueryRequest(string domain, WhoisClient client,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            _client = client;
            _domain = domain;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncQueryRequestThread1));
            Thread.Sleep(20);
        }

        private string _domain = string.Empty;
        private WhoisClient _client = null;

        /// <summary>
        /// The async query request method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncQueryRequestThread1(Object stateInfo)
        {
            // Get the query result.
            string data = _client.Query(_domain);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (!String.IsNullOrEmpty(data))
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }
}