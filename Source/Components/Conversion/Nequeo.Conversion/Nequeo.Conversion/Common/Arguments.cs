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

namespace Nequeo.Conversion.Common
{
    /// <summary>
    /// Conversion event argument type.
    /// </summary>
    public class ConversionArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Default type constructor.
        /// </summary>
        /// <param name="index">The current index within the list.</param>
        /// <param name="message">The message for the current index.</param>
        public ConversionArgs(long index, string message)
        {
            _index = index;
            _message = message;
        }
        #endregion

        #region Private Fields
        private long _index = -1;
        private string _message = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets, the index within the list.
        /// </summary>
        public long Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets, the message for the conversion.
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
        #endregion
    }
}
