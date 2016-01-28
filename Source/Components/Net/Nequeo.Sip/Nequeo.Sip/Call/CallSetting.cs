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
    /// Call settings.
    /// </summary>
    public class CallSetting
    {
        /// <summary>
        /// Call settings.
        /// </summary>
        public CallSetting() { }

        /// <summary>
        /// Call settings.
        /// </summary>
        /// <param name="useDefaultValues">Use default values.</param>
        public CallSetting(bool useDefaultValues)
        {
            _useDefaultValues = useDefaultValues;
        }

        private bool _useDefaultValues = false;

        /// <summary>
        /// Gets the use default values.
        /// </summary>
        internal bool UseDefaultValues
        {
            get { return _useDefaultValues; }
        }

        ///	<summary>
        ///	Gets or sets the bitmask of CallFlag constants.
        ///	</summary>
        public CallFlag Flag { get; set; }

        ///	<summary>
        ///	Gets or sets this flag controls what methods to request keyframe are allowed on
        /// the call. Value is bitmask of VidReqKeyframeMethod.
        ///	</summary>
        public VidReqKeyframeMethod ReqKeyframeMethod { get; set; }

        ///	<summary>
        ///	Gets or sets the number of simultaneous active audio streams for this call. Setting
        /// this to zero will disable audio in this call.
        ///	</summary>
        public uint AudioCount { get; set; }

        ///	<summary>
        ///	Gets or sets the number of simultaneous active video streams for this call. Setting
        /// this to zero will disable video in this call.
        ///	</summary>
        public uint VideoCount { get; set; }
    }
}
