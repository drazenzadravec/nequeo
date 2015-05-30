/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Invalid header exception.
    /// </summary>
    internal class InvalidHeaderException : Exception
    {
        /// <summary>
        /// Gets the header.
        /// </summary>
        public KeyValuePair<string, string> Header { get; private set; }

        /// <summary>
        /// Invalid header exception.
        /// </summary>
        /// <param name="header">The header.</param>
        public InvalidHeaderException(KeyValuePair<string, string> header)
            : base("Incorrect header was provided for compression.")
        {
            Header = header;
        }
    }
}
