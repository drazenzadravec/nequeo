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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Exceptions
{
    /// <summary>
    /// Error code exception.
    /// </summary>
#if !WINDOWS_UWP
	public class ErrorCodeException : System.Exception
#else
    internal class ErrorCodeException : System.Exception
#endif
    {
        #region Constructors
        /// <summary>
        /// Constructor for message arguments.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public ErrorCodeException(string message)
            : base(message)
        {
            this.customMessage = message;
        }

        /// <summary>
        /// Constructor for inner exception argument.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="errorCode">The error code.</param>
        public ErrorCodeException(string message, int errorCode)
            : base(message)
        {
            this.customMessage = message;
            this.errorCode = errorCode;
        }

        /// <summary>
        /// Constructor for inner exception argument.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="innerException">The inner exception object to pass.</param>
        public ErrorCodeException(string message, System.Exception innerException)
            : base(message, innerException)
        {
            this.customMessage = message;
            this.customInnerException = innerException;
        }

        /// <summary>
        /// Constructor for inner exception argument.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="innerException">The inner exception object to pass.</param>
        /// <param name="errorCode">The error code.</param>
        public ErrorCodeException(string message, System.Exception innerException, int errorCode)
            : base(message, innerException)
        {
            this.customMessage = message;
            this.customInnerException = innerException;
            this.errorCode = errorCode;
        }
        #endregion

        #region Private Fields
        private int errorCode = 0;
        private string customMessage = string.Empty;
        private System.Exception customInnerException = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the current exception message.
        /// </summary>
        public string CustomMessage
        {
            get { return customMessage; }
        }

        /// <summary>
        /// Contains the current inner exception object.
        /// </summary>
        public System.Exception CustomInnerException
        {
            get { return customInnerException; }
        }

        /// <summary>
        /// Contains the current error code.
        /// </summary>
        public int ErrorCode
        {
            get { return errorCode; }
        }
        #endregion
    }
}
