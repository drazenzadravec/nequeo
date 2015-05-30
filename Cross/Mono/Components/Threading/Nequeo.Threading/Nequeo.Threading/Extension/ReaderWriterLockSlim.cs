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
using System.Diagnostics;
using System.Threading;

namespace Nequeo.Threading.Extension
{
    /// <summary>
    /// Contains extension methods of <see cref="ReaderWriterLockSlim"/>.
    /// </summary>
    public static class ReaderWriterLockSlimExtensions
    {
        /// <summary>
        /// Starts thread safe read write code block.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static IDisposable ReadAndWrite(this ReaderWriterLockSlim instance)
        {
            // Validate arguments
            if (instance == null) throw new ArgumentNullException("instance");

            instance.EnterUpgradeableReadLock();

            return new DisposableCodeBlock(instance.ExitUpgradeableReadLock);
        }

        /// <summary>
        /// Starts thread safe read code block.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static IDisposable Read(this ReaderWriterLockSlim instance)
        {
            // Validate arguments
            if (instance == null) throw new ArgumentNullException("instance");

            instance.EnterReadLock();

            return new DisposableCodeBlock(instance.ExitReadLock);
        }

        /// <summary>
        /// Starts thread safe write code block.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static IDisposable Write(this ReaderWriterLockSlim instance)
        {
            // Validate arguments
            if (instance == null) throw new ArgumentNullException("instance");

            instance.EnterWriteLock();

            return new DisposableCodeBlock(instance.ExitWriteLock);
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class DisposableCodeBlock : IDisposable
        {
            private readonly Action action;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="action"></param>
            public DisposableCodeBlock(Action action)
            {
                this.action = action;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                action();
            }
        }
    }
}
