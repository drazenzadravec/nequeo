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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nequeo.Wpf.Common
{
    /// <summary>
    /// Stores a Func&lt;T&gt; without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the Func that will be stored
    /// by this weak reference.</typeparam>
    ////[ClassInfo(typeof(WeakAction)]
    public class WeakFunc<TResult>
    {
        private Func<TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakFunc's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// Get a value indicating whether the WeakFunc is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return _staticFunc != null;
            }
        }

        /// <summary>
        /// Gets the name of the method that this WeakFunc represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.Method.Name;
                }

                return Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakFunc's action's target.
        /// This is not necessarily the same as
        /// <see cref="Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference FuncReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakFunc. This is not necessarily the same as
        /// <see cref="FuncReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes an empty instance of the WeakFunc class.
        /// </summary>
        protected WeakFunc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(Func<TResult> func)
            : this(func == null ? null : func.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1062:Validate arguments of public methods",
            MessageId = "1",
            Justification = "Method should fail with an exception if func is null.")]
        public WeakFunc(object target, Func<TResult> func)
        {
            if (func.Method.IsStatic)
            {
                _staticFunc = func;

                if (target != null)
                {
                    // Keep a reference to the target to control the
                    // WeakAction's lifetime.
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = func.Method;
            FuncReference = new WeakReference(func.Target);
            Reference = new WeakReference(target);
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticFunc == null
                    && Reference == null)
                {
                    return false;
                }

                if (_staticFunc != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Func's owner. This object is stored as a 
        /// <see cref="WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (Reference == null)
                {
                    return null;
                }

                return Reference.Target;
            }
        }

        /// <summary>
        /// Gets the owner of the Func that was passed as parameter.
        /// This is not necessarily the same as
        /// <see cref="Target" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected object FuncTarget
        {
            get
            {
                if (FuncReference == null)
                {
                    return null;
                }

                return FuncReference.Target;
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }

            var funcTarget = FuncTarget;

            if (IsAlive)
            {
                if (Method != null
                    && FuncReference != null
                    && funcTarget != null)
                {
                    return (TResult)Method.Invoke(funcTarget, null);
                }
            }

            return default(TResult);
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            Method = null;
            _staticFunc = null;
        }
    }
}