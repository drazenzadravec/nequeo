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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace Nequeo.Data.MongoDb
{
    /// <summary>
    /// MongoDB data access connection provider.
    /// </summary>
    public partial class Connection : IDisposable
    {
        /// <summary>
        /// MongoDB data access connection provider.
        /// </summary>
        /// <param name="host">The mongodb remote host name.</param>
        /// <param name="port">The mongodb remote port number.</param>
        /// <param name="credentials">The client credentials.</param>
        public Connection(string host, int port = 27017, IEnumerable<MongoCredential> credentials = null)
        {
            _serverAddress = new MongoServerAddress(host, port);
            _clientSettings = new MongoClientSettings();

            // Assign the server.
            _clientSettings.Server = _serverAddress;

            // Add the credentials.
            if(credentials != null)
                _clientSettings.Credentials = credentials;

            // Assign the client.
            _client = new MongoClient(_clientSettings);
            _server = _client.GetServer();
        }

        private MongoDB.Driver.MongoClientSettings _clientSettings = null;
        private MongoDB.Driver.MongoServerAddress _serverAddress = null;
        private MongoDB.Driver.MongoClient _client = null;
        private MongoDB.Driver.MongoServer _server = null;
        private IEnumerable<MongoCredential> _credentials = null;

        /// <summary>
        /// Gets the client.
        /// </summary>
        public MongoDB.Driver.MongoClient Client
        {
            get { return _client; }
        }

        /// <summary>
        /// Gets the server.
        /// </summary>
        public MongoDB.Driver.MongoServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Gets the client settings.
        /// </summary>
        public MongoDB.Driver.MongoClientSettings ClientSettings
        {
            get { return _clientSettings; }
        }

        /// <summary>
        /// Gets the server address.
        /// </summary>
        public MongoDB.Driver.MongoServerAddress ServerAddress
        {
            get { return _serverAddress; }
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        public IEnumerable<MongoCredential> Credentials
        {
            get { return _credentials; }
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_server != null)
                    {
                        try
                        {
                            // Disconnect from the server.
                            _server.Disconnect();
                        }
                        catch { }
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _server = null;
                _client = null;
                _serverAddress = null;
                _clientSettings = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Connection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
