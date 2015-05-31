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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Forms.UI.Control
{
    /// <summary>
    /// The keyboard type.
    /// </summary>
    [Category("Keyboard Type"), Description("Type of keyboard to use")]
    public enum KeyboardType 
    { 
        /// <summary>
        /// 
        /// </summary>
        Standard = 0, 
        /// <summary>
        /// 
        /// </summary>
        Alphabetical = 1, 
        /// <summary>
        /// 
        /// </summary>
        Kids = 2,
    };

    /// <summary>
    /// Keyboard delegate.
    /// </summary>
    /// <param name="sender">The current sender.</param>
    /// <param name="e">Key board events.</param>
    public delegate void KeyboardDelegate(object sender, KeyboardEventArgs e);

    /// <summary>
    /// Keyboard event arguments.
    /// </summary>
    public class KeyboardEventArgs : EventArgs
    {
        private readonly string _pvtKeyboardKeyPressed;

        /// <summary>
        /// Keyboard event arguments.
        /// </summary>
        /// <param name="KeyboardKeyPressed">The keyboard pressed key.</param>
        public KeyboardEventArgs(string KeyboardKeyPressed)
        {
            _pvtKeyboardKeyPressed = KeyboardKeyPressed;
        }

        /// <summary>
        /// Gets the keyboard pressed key.
        /// </summary>
        public string KeyboardKeyPressed
        {
            get { return _pvtKeyboardKeyPressed; }
        }
    }
}
