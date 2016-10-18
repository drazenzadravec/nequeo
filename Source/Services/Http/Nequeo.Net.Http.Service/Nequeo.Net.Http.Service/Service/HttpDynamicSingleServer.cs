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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Http.Service
{
    /// <summary>
    /// Http dynamic single server.
    /// </summary>
    partial class HttpDynamicSingleServer : ServiceBase
    {
        /// <summary>
        /// Http dynamic single server.
        /// </summary>
        public HttpDynamicSingleServer()
        {
            InitializeComponent();
        }

        private Nequeo.Net.Http.ServerSingleDynamic _httpServerSingle = null;

        /// <summary>
        /// Start.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                // Start the server.
                _httpServerSingle = new Nequeo.Net.Http.ServerSingleDynamic(Nequeo.Net.Http.Properties.Settings.Default.HttpDynamicSingleBasePath);
                _httpServerSingle.HttpServer.Name = Nequeo.Net.Http.Service.Program.CommonServerName;
                _httpServerSingle.HttpServer.Timeout = 30;
                _httpServerSingle.HttpServer.SocketThreadCount = 1;
                _httpServerSingle.Start();
            }
            catch { }
        }

        /// <summary>
        /// Stop.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (_httpServerSingle != null)
                {
                    // Stop the server.
                    _httpServerSingle.Stop();
                    _httpServerSingle.Dispose();
                }
            }
            catch { }

            _httpServerSingle = null;
        }
    }
}
