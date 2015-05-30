/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.IO.Stream.Extension;

namespace Nequeo.Net.Http2.Protocol
{
    /// <summary>
    /// This class reads frames and gets their type.
    /// </summary>
    internal class FrameReader : IDisposable
    {
        /// <summary>
        /// This class reads frames and gets their type.
        /// </summary>
        /// <param name="stream">The stream containing the data.</param>
        public FrameReader(Stream stream)
        {
            _stream = stream;
        }

        private readonly Stream _stream;
        private long _timeout = -1;

        /// <summary>
        /// Read the frame.
        /// </summary>
        /// <returns>The current frame.</returns>
        public Frame ReadFrame(long timeout = -1)
        {
            bool completed = true;
            _timeout = timeout;

            if (_disposed)
                return null;

            // Set the buffer.
            var preamble = new Frame();
            preamble.Buffer = TryFill(preamble.Buffer.Length, out completed);
            if (!completed)
                return null;

            Frame wholeFrame;
            try
            {
                // Get the frame.
                wholeFrame = GetFrameType(preamble);
            }
            //09 -> 4.1.  Frame Format 
            //Implementations MUST ignore frames of unsupported or unrecognized types
            catch (NotImplementedException)
            {
                return preamble;
            }

            // Attempt to get the data from the stream.
            wholeFrame.Buffer = TryFill(wholeFrame.Buffer.Length - Constants.FramePreambleSize, out completed);
            if (!completed)
                return null;

            // Return the frame.
            return wholeFrame;
        }

        /// <summary>
        /// Get the frame type.
        /// </summary>
        /// <param name="preamble">The preamble frame.</param>
        /// <returns>The specific frame.</returns>
        private static Frame GetFrameType(Frame preamble)
        {
            // Select the frame type.
            switch (preamble.FrameType)
            {
                case OpCodeFrame.Go_Away:
                    return new GoAwayFrame(preamble);

                case OpCodeFrame.Ping:
                    return new PingFrame(preamble);

                case OpCodeFrame.Reset_Stream:
                    return new RstStreamFrame(preamble);

                case OpCodeFrame.Settings:
                    return new SettingsFrame(preamble);

                case OpCodeFrame.Headers:
                    return new HeadersFrame(preamble);

                case OpCodeFrame.Continuation:
                    return new ContinuationFrame(preamble);

                case OpCodeFrame.Window_Update:
                    return new WindowUpdateFrame(preamble);

                case OpCodeFrame.Data:
                    return new DataFrame(preamble);

                case OpCodeFrame.Push_Promise:
                    return new PushPromiseFrame(preamble);

                case OpCodeFrame.Priority:
                    return new PriorityFrame(preamble);

                default:
                    throw new NotImplementedException("Frame type: " + preamble.FrameType);
            }
        }

        /// <summary>
        /// Try to get data from the stream.
        /// </summary>
        /// <param name="count">The bytes to read.</param>
        /// <param name="completed">Have all the bytes been read.</param>
        /// <returns></returns>
        private byte[] TryFill(int count, out bool completed)
        {
            return _stream.ReadBytesTimer(count, out completed, _timeout);
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
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~FrameReader()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
