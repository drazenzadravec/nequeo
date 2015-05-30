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
using System.Threading.Tasks;

namespace Nequeo.Threading.Extension
{
    /// <summary>Extensions for TaskFactory.</summary>
    public static partial class TaskFactoryExtensions
    {
        /// <summary>Creates a generic TaskFactory from a non-generic one.</summary>
        /// <typeparam name="TResult">Specifies the type of Task results for the Tasks created by the new TaskFactory.</typeparam>
        /// <param name="factory">The TaskFactory to serve as a template.</param>
        /// <returns>The created TaskFactory.</returns>
        public static TaskFactory<TResult> ToGeneric<TResult>(this TaskFactory factory)
        {
            return new TaskFactory<TResult>(
                factory.CancellationToken, factory.CreationOptions, factory.ContinuationOptions, factory.Scheduler);
        }

        /// <summary>Creates a generic TaskFactory from a non-generic one.</summary>
        /// <typeparam name="TResult">Specifies the type of Task results for the Tasks created by the new TaskFactory.</typeparam>
        /// <param name="factory">The TaskFactory to serve as a template.</param>
        /// <returns>The created TaskFactory.</returns>
        public static TaskFactory ToNonGeneric<TResult>(this TaskFactory<TResult> factory)
        {
            return new TaskFactory(
                factory.CancellationToken, factory.CreationOptions, factory.ContinuationOptions, factory.Scheduler);
        }

        /// <summary>Gets the TaskScheduler instance that should be used to schedule tasks.</summary>
        public static TaskScheduler GetTargetScheduler(this TaskFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return factory.Scheduler ?? TaskScheduler.Current;
        }

        /// <summary>Gets the TaskScheduler instance that should be used to schedule tasks.</summary>
        public static TaskScheduler GetTargetScheduler<TResult>(this TaskFactory<TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return factory.Scheduler != null ? factory.Scheduler : TaskScheduler.Current;
        }

        /// <summary>Converts TaskCreationOptions into TaskContinuationOptions.</summary>
        /// <param name="creationOptions"></param>
        /// <returns></returns>
        private static TaskContinuationOptions ContinuationOptionsFromCreationOptions(TaskCreationOptions creationOptions)
        {
            return (TaskContinuationOptions)
                ((creationOptions & TaskCreationOptions.AttachedToParent) |
                 (creationOptions & TaskCreationOptions.PreferFairness) |
                 (creationOptions & TaskCreationOptions.LongRunning));
        }
    }
}