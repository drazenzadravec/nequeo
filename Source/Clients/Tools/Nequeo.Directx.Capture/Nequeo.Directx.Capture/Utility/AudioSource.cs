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
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Text;

using DirectShowLib;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using DirectShowLib.Dvd;
using DirectShowLib.MultimediaStreaming;
using DirectShowLib.SBE;

namespace Nequeo.Directx.Utility
{
    /// <summary>
    ///  Represents a physical connector or source on an 
    ///  audio device. This class is used on filters that
    ///  support the IAMAudioInputMixer interface such as 
    ///  source cards.
    /// </summary>
    public class AudioSource : Source
    {
        private IPin _pin;			// audio mixer interface (COM object)

        /// <summary>
        /// Constructor. This class cannot be created directly.
        /// </summary>
        /// <param name="pin">The Pin.</param>
        internal AudioSource(IPin pin)
        {
            if ((pin as IAMAudioInputMixer) == null)
                throw new NotSupportedException("The input pin does not support the IAMAudioInputMixer interface");

            _pin = pin;
            this.Name = GetName(pin);
        }

        /// <summary>
        /// Gets or sets; Enable or disable this source. For audio sources it is 
        /// usually possible to enable several sources. When setting Enabled=true,
        /// set Enabled=false on all other audio sources. 
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IAMAudioInputMixer mix = (IAMAudioInputMixer)_pin;
                bool e;
                mix.get_Enable(out e);
                return (e);
            }

            set
            {
                IAMAudioInputMixer mix = (IAMAudioInputMixer)_pin;
                mix.put_Enable(value);
            }
        }

        /// <summary>
        /// Retrieve the friendly name of a connectorType.
        /// </summary>
        /// <param name="pin">The Pin</param>
        /// <returns>The name of the Pin.</returns>
        private string GetName(IPin pin)
        {
            string s = "Unknown pin";
            PinInfo pinInfo = new PinInfo();

            // Direction matches, so add pin name to listbox
            int hr = pin.QueryPinInfo(out pinInfo);
            if (hr == 0)
            {
                s = pinInfo.name + "";
            }
            else
                Marshal.ThrowExceptionForHR(hr);

            // The pininfo structure contains a reference to an IBaseFilter,
            // so you must release its reference to prevent resource a leak.
            if (pinInfo.filter != null)
                Marshal.ReleaseComObject(pinInfo.filter); pinInfo.filter = null;

            return (s);
        }

        /// <summary>
        /// Release unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (_pin != null)
                Marshal.ReleaseComObject(_pin);

            _pin = null;
            base.Dispose();
        }
    }
}
