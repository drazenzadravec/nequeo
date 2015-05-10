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

namespace Nequeo.Extension
{
    /// <summary>
    /// Class that extends the System.Int32 type.
    /// </summary>
    public static class Int32Extensions
    {
        #region Public Methods
        /// <summary>
        /// Get the multiplication result.
        /// </summary>
        /// <param name="intValue">The current integer value.</param>
        /// <param name="multiplyBy">The multiplication value.</param>
        /// <returns>The new multiplied value.</returns>
        public static int Multiplication(this Int32 intValue, Int32 multiplyBy)
        {
            // Return the new value.
            return (intValue * multiplyBy);
        }

        /// <summary>
        /// Get the division result.
        /// </summary>
        /// <param name="intValue">The current integer value.</param>
        /// <param name="divideBy">The division value.</param>
        /// <returns>The new divided value.</returns>
        public static int Division(this Int32 intValue, Int32 divideBy)
        {
            // Can not divide by zero.
            if (divideBy == 0)
                throw new System.DivideByZeroException();

            // Return the new value.
            return (intValue / divideBy);
        }
        #endregion
    }
}
