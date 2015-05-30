/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Security.Principal;

using Nequeo.Net.Sockets;
using Nequeo.Handler;
using Nequeo.Extension;
using Nequeo.Net.Configuration;

namespace Nequeo.Net
{
    /// <summary>
    /// The algorithm type.
    /// </summary>
    public enum AlgorithmType
    {
        /// <summary>
        /// No algorithm, selects the first server.
        /// </summary>
        None = 0,
        /// <summary>
        /// User defined, executes a user defined algorithm.
        /// </summary>
        UserDefined = 1,
        /// <summary>
        /// Load balancer least connections, selects the lowest count.
        /// </summary>
        LeastConnections = 2,
        /// <summary>
        /// Load balancer round robin, selects the next.
        /// </summary>
        RoundRobin = 3,
    }

    /// <summary>
    /// Remote server.
    /// </summary>
    public class RemoteServer
    {
        private long _count = 0;
        private object _lockObject = new object();

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the remote host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the remote port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets secure connection indicator.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// Gets or sets the weight of the host.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the number of connections to this host.
        /// </summary>
        public long Count 
        {
            get
            {
                lock (_lockObject)
                    return _count;
            }
            set
            {
                lock (_lockObject)
                    _count = value;
            }
        }
    }

    /// <summary>
    /// Intercept command container.
    /// </summary>
    public class InterceptItem
    {
        /// <summary>
        /// Gets or sets the command to intercept.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the response to send back.
        /// </summary>
        public string Response { get; set; }
    }
}
