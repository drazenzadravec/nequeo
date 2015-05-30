/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace Nequeo.Diagnostics
{
    /// <summary>
    /// Debug conditional handler.
    /// </summary>
	public class Debug
	{
        /// <summary>
        /// Default constuctor
        /// </summary>
        public Debug()
        {
        }

        private Action<string> _messageHandler = null;
        private Action<Exception> _exceptionHandler = null;
        private Action<Object> _objectHandler = null;

        /// <summary>
        /// Gets sets the string message handler action.
        /// </summary>
        public Action<string> MessageStringHandler
        {
            get { return _messageHandler; }
            set { _messageHandler = value; }
        }

        /// <summary>
        /// Gets sets the exception message handler action.
        /// </summary>
        public Action<Exception> MessageExceptionHandler
        {
            get { return _exceptionHandler; }
            set { _exceptionHandler = value; }
        }

        /// <summary>
        /// Gets sets the object message handler action.
        /// </summary>
        public Action<Object> MessageObjectHandler
        {
            get { return _objectHandler; }
            set { _objectHandler = value; }
        }

        /// <summary>
        /// Writes the message to the output console.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Conditional("DEBUG")]
        public static void ConsoleMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Writes the message to the output file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="filePath">The file and path to the output file.</param>
        [Conditional("DEBUG")]
        public static void FileMessage(string message, string filePath)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;

            try
            {
                // Create a new handler to the file.
                using(fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (streamWriter = new StreamWriter(fileStream))
                {
                    // Write the message to the file.
                    streamWriter.Write(message);
                }
            }
            catch (Exception)
            { }
            finally
            {
                if (streamWriter != null)
                    streamWriter.Close();

                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// Writes the message to the string message handler.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Conditional("DEBUG")]
        public void MessageHandler(string message)
        {
            if (_messageHandler != null)
                _messageHandler(message);
        }

        /// <summary>
        /// Writes the exception to the exception message handler.
        /// </summary>
        /// <param name="message">The exception to write.</param>
        [Conditional("DEBUG")]
        public void ExceptionHandler(Exception message)
        {
            if (_exceptionHandler != null)
                _exceptionHandler(message);
        }

        /// <summary>
        /// Writes the object to the object message handler.
        /// </summary>
        /// <param name="message">The object to write.</param>
        [Conditional("DEBUG")]
        public void ObjectHandler(Object message)
        {
            if (_objectHandler != null)
                _objectHandler(message);
        }
	}
}
