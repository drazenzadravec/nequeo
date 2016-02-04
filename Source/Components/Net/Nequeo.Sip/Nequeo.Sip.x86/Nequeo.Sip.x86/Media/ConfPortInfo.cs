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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Sip
{
    /// <summary>
    /// This structure descibes information about a particular media port that
    /// has been registered into the conference bridge.
    /// </summary>
    public class ConfPortInfo
    {
        /// <summary>
        /// This structure descibes information about a particular media port that
        /// has been registered into the conference bridge.
        /// </summary>
        public ConfPortInfo() { }

        private int _portId;
        private string _name;
		private MediaFormatAudio _format;
		private float _txLevelAdj;
        private float _rxLevelAdj;
        private int[] _listeners;

        /// <summary>
        /// Gets or sets the conference port number.
        /// </summary>
        public int PortId
        {
            get { return _portId; }
            set { _portId = value; }
        }

        /// <summary>
        /// Gets or sets the port name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the media audio format information.
        /// </summary>
        public MediaFormatAudio Format
        {
            get { return _format; }
            set { _format = value; }
        }

        /// <summary>
        /// Gets or sets the Tx level adjustment. Value 1.0 means no adjustment, value 0 means
        /// the port is muted, value 2.0 means the level is amplified two times.
        /// </summary>
        public float TxLevelAdj
        {
            get { return _txLevelAdj; }
            set { _txLevelAdj = value; }
        }

        /// <summary>
        /// Gets or sets the Rx level adjustment. Value 1.0 means no adjustment, value 0 means
        /// the port is muted, value 2.0 means the level is amplified two times.
        /// </summary>
        public float RxLevelAdj
        {
            get { return _rxLevelAdj; }
            set { _rxLevelAdj = value; }
        }

        /// <summary>
        /// Gets or sets the Array of listeners (in other words, ports where this port is transmitting to.
        /// </summary>
        public int[] Listeners
        {
            get { return _listeners; }
            set { _listeners = value; }
        }
    }
}
