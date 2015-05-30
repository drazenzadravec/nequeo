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
using System.Linq;

namespace Nequeo.Threading.Extension
{
    /// <summary>
    /// Parallel extensions for the Delegate class.
    /// </summary>
    public static class DelegateExtensions
    {
        /// <summary>Dynamically invokes (late-bound) in parallel the methods represented by the delegate.</summary>
        /// <param name="multicastDelegate">The delegate to be invoked.</param>
        /// <param name="args">An array of objects that are the arguments to pass to the delegates.</param>
        /// <returns>The return value of one of the delegate invocations.</returns>
        public static object ParallelDynamicInvoke(this Delegate multicastDelegate, params object[] args)
        {
            if (multicastDelegate == null) throw new ArgumentNullException("multicastDelegate");
            if (args == null) throw new ArgumentNullException("args");
            return multicastDelegate.GetInvocationList()
                   .AsParallel().AsOrdered()
                   .Select(d => d.DynamicInvoke(args))
                   .Last();
        }

        /// <summary>
        /// Provides a delegate that runs the specified action and fails fast if the action throws an exception.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The wrapper delegate.</returns>
        public static Action WithFailFast(this Action action)
        {
            return () =>
            {
                try { action(); }
                catch (Exception exc)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    else Environment.FailFast("An unhandled exception occurred.", exc);
                }
            };
        }

        /// <summary>
        /// Provides a delegate that runs the specified function and fails fast if the function throws an exception.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <returns>The wrapper delegate.</returns>
        public static Func<T> WithFailFast<T>(this Func<T> function)
        {
            return () =>
            {
                try { return function(); }
                catch (Exception exc)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    else Environment.FailFast("An unhandled exception occurred.", exc);
                }
                throw new Exception("Will never get here");
            };
        }
    }
}
