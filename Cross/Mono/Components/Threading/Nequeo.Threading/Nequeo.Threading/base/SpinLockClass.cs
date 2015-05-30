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
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Nequeo.Threading
{
    /// <summary>Provides a simple, reference type wrapper for SpinLock.</summary>
    public class SpinLockWrapper
    {
        private SpinLock _spinLock; // NOTE: must *not* be readonly due to SpinLock being a mutable struct

        /// <summary>Initializes an instance of the SpinLockClass class.</summary>
        public SpinLockWrapper()
        {
            _spinLock = new SpinLock();
        }

        /// <summary>Initializes an instance of the SpinLockClass class.</summary>
        /// <param name="enableThreadOwnerTracking">
        /// Controls whether the SpinLockClass should track
        /// thread-ownership fo the lock.
        /// </param>
        public SpinLockWrapper(bool enableThreadOwnerTracking)
        {
            _spinLock = new SpinLock(enableThreadOwnerTracking);
        }

        /// <summary>Runs the specified delegate under the lock.</summary>
        /// <param name="runUnderLock">The delegate to be executed while holding the lock.</param>
        public void Execute(Action runUnderLock)
        {
            bool lockTaken = false;
            try
            {
                Enter(ref lockTaken);
                runUnderLock();
            }
            finally
            {
                if (lockTaken) Exit();
            }
        }

        /// <summary>Enters the lock.</summary>
        /// <param name="lockTaken">
        /// Upon exit of the Enter method, specifies whether the lock was acquired. 
        /// The variable passed by reference must be initialized to false.
        /// </param>
        public void Enter(ref bool lockTaken)
        {
            _spinLock.Enter(ref lockTaken);
        }

        /// <summary>Exits the SpinLock.</summary>
        public void Exit()
        {
            _spinLock.Exit();
        }

        /// <summary>Exits the SpinLock.</summary>
        /// <param name="useMemoryBarrier">
        /// A Boolean value that indicates whether a memory fence should be issued in
        /// order to immediately publish the exit operation to other threads.
        /// </param>
        public void Exit(bool useMemoryBarrier)
        {
            _spinLock.Exit(useMemoryBarrier);
        }
    }
}
