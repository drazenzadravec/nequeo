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
using System.Threading;

namespace Nequeo.Threading.Extension
{
    /// <summary>
    /// Extension methods for CancellationToken.
    /// </summary>
    public static class CancellationTokenExtensions
    {
        /// <summary>Cancels a CancellationTokenSource and throws a corresponding OperationCanceledException.</summary>
        /// <param name="source">The source to be canceled.</param>
        public static void CancelAndThrow(this CancellationTokenSource source)
        {
            source.Cancel();
            source.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Creates a CancellationTokenSource that will be canceled when the specified token has cancellation requested.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The created CancellationTokenSource.</returns>
        public static CancellationTokenSource CreateLinkedSource(this CancellationToken token)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationToken());
        }
    }
}
