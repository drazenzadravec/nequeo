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
    /// <summary>Provides a reduction variable for aggregating data across multiple threads involved in a computation.</summary>
    /// <typeparam name="T">Specifies the type of the data being aggregated.</typeparam>
    [DebuggerDisplay("Count={_values.Count}")]
    [DebuggerTypeProxy(typeof(ReductionVariable_DebugView<>))]
    public sealed class ReductionVariable<T> : IDisposable
    {
        /// <summary>The factory used to initialize a value on a thread.</summary>
        private readonly Func<T> _seedFactory;
        /// <summary>Thread-local storage for each thread's value.</summary>
        private readonly ThreadLocal<StrongBox<T>> _threadLocal;
        /// <summary>The list of all thread-local values for later enumeration.</summary>
        private readonly ConcurrentQueue<StrongBox<T>> _values = new ConcurrentQueue<StrongBox<T>>();

        /// <summary>Initializes the instances.</summary>
        public ReductionVariable()
        {
            _threadLocal = new ThreadLocal<StrongBox<T>>(CreateValue);
        }

        /// <summary>Initializes the instances.</summary>
        /// <param name="seedFactory">
        /// The function invoked to provide the initial value for a thread.  
        /// If null, the default value of T will be used as the seed.
        /// </param>
        public ReductionVariable(Func<T> seedFactory) : this()
        {
            _seedFactory = seedFactory;
        }

        /// <summary>Creates a value for the current thread and stores it in the central list of values.</summary>
        /// <returns>The boxed value.</returns>
        private StrongBox<T> CreateValue()
        {
            var s = new StrongBox<T>(_seedFactory != null ? _seedFactory() : default(T));
            _values.Enqueue(s);
            return s;
        }

        /// <summary>Gets or sets the value for the current thread.</summary>
        public T Value
        {
            get { return _threadLocal.Value.Value; }
            set { _threadLocal.Value.Value = value; }
        }

        /// <summary>Gets the values for all of the threads that have used this instance.</summary>
        public IEnumerable<T> Values { get { return _values.Select(s => s.Value); } }

        /// <summary>Applies an accumulator function over the values in this variable.</summary>
        /// <param name="function">An accumulator function to be invoked on each value.</param>
        /// <returns>The accumulated value.</returns>
        public T Reduce(Func<T, T, T> function)
        {
            return Values.Aggregate(function);
        }

        /// <summary>
        /// Applies an accumulator function over the values in this variable.
        /// The specified seed is used as the initial accumulator value.
        /// </summary>
        /// <param name="seed">The seed the apply.</param>
        /// <param name="function">An accumulator function to be invoked on each value.</param>
        /// <returns>The accumulated value.</returns>
        public TAccumulate Reduce<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> function)
        {
            return Values.Aggregate(seed, function);
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if(_threadLocal != null)
                        _threadLocal.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ReductionVariable()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>Debug view for the reductino variable</summary>
    /// <typeparam name="T">Specifies the type of the data being aggregated.</typeparam>
    internal sealed class ReductionVariable_DebugView<T>
    {
        private ReductionVariable<T> _variable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        public ReductionVariable_DebugView(ReductionVariable<T> variable)
        {
            _variable = variable;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Values { get { return _variable.Values.ToArray(); } }
    }
}