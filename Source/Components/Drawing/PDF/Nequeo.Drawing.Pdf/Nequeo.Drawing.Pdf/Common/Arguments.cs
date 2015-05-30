/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2011 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.Drawing.Pdf.Common
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
