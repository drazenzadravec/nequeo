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
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nequeo.Collections
{
    /// <summary>
    /// Global store buffer.
    /// </summary>
    public class StoreBuffer : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Global store buffer.
        /// </summary>
        /// <param name="readBufferSize">The read buffer size.</param>
        /// <param name="writeBufferSize">The write buffer size.</param>
        public StoreBuffer(int readBufferSize, int writeBufferSize)
        {
            READ_BUFFER_SIZE = readBufferSize;
            WRITE_BUFFER_SIZE = writeBufferSize;

            ReadBuffer = new byte[READ_BUFFER_SIZE];
            WriteBuffer = new byte[WRITE_BUFFER_SIZE];
        }
        #endregion

        #region Private Fields
        private object _lockObject = new object();

        /// <summary>
        /// The read buffer size.
        /// </summary>
        public int READ_BUFFER_SIZE = 8192;
        /// <summary>
        /// The write buffer size.
        /// </summary>
        public int WRITE_BUFFER_SIZE = 8192;

        /// <summary>
        /// The read buffer.
        /// </summary>
        public byte[] ReadBuffer = null;
        /// <summary>
        /// The write buffer.
        /// </summary>
        public byte[] WriteBuffer = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the thread lock synchronisation object.
        /// </summary>
        public object SyncObject
        {
            get { return _lockObject; }
        }

        /// <summary>
        /// Gets sets, the read buffer size.
        /// </summary>
        public int ReadBufferSize
        {
            get { return READ_BUFFER_SIZE; }
            set
            {
                ReadBuffer = null;
                READ_BUFFER_SIZE = value;
                ReadBuffer = new byte[READ_BUFFER_SIZE];
            }
        }

        /// <summary>
        /// Gets sets, the write buffer size.
        /// </summary>
        public int WriteBufferSize
        {
            get { return WRITE_BUFFER_SIZE; }
            set
            {
                WriteBuffer = null;
                WRITE_BUFFER_SIZE = value;
                WriteBuffer = new byte[WRITE_BUFFER_SIZE];
            }
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
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
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                WriteBuffer = null;
                ReadBuffer = null;
                _lockObject = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~StoreBuffer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
