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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Threading;

namespace Nequeo.Threading.Extension
{
    /// <summary>
    /// Provides LINQ support for Tasks by implementing the primary standard query operators.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task<TResult> Select<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> selector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            // Use a continuation to run the selector function
            return source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.NotOnCanceled);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task<TResult> SelectMany<TSource, TResult>(this Task<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            // Use a continuation to run the selector function.
            return source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="collectionSelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Task<TResult> SelectMany<TSource, TCollection, TResult>(
            this Task<TSource> source, 
            Func<TSource, Task<TCollection>> collectionSelector, 
            Func<TSource, TCollection, TResult> resultSelector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException("source");
            if (collectionSelector == null) throw new ArgumentNullException("collectionSelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            // When the source completes, run the collectionSelector to get the next Task,
            // and continue off of it to run the result selector
            return source.ContinueWith(t =>
            {
                return collectionSelector(t.Result).
                    ContinueWith(c => resultSelector(t.Result, c.Result), TaskContinuationOptions.NotOnCanceled);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task<TSource> Where<TSource>(this Task<TSource> source, Func<TSource, bool> predicate)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            // Create a continuation to run the predicate and return the source's result.
            // If the predicate returns false, cancel the returned Task.
            var cts = new CancellationTokenSource();
            return source.ContinueWith(t =>
            {
                var result = t.Result;
                if (!predicate(result)) cts.CancelAndThrow();
                return result;
            }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner, 
            Func<TOuter, TKey> outerKeySelector, 
            Func<TInner, TKey> innerKeySelector, 
            Func<TOuter, TInner, TResult> resultSelector)
        {
            // Argument validation handled by delegated method call
            return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner, 
            Func<TOuter, TKey> outerKeySelector, 
            Func<TInner, TKey> innerKeySelector, 
            Func<TOuter, TInner, TResult> resultSelector, 
            IEqualityComparer<TKey> comparer)
        {
            // Validate arguments
            if (outer == null) throw new ArgumentNullException("outer");
            if (inner == null) throw new ArgumentNullException("inner");
            if (outerKeySelector == null) throw new ArgumentNullException("outerKeySelector");
            if (innerKeySelector == null) throw new ArgumentNullException("innerKeySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            if (comparer == null) throw new ArgumentNullException("comparer");

            // First continue off of the outer and then off of the inner.  Two separate
            // continuations are used so that each may be canceled easily using the NotOnCanceled option.
            return outer.ContinueWith(delegate
            {
                var cts = new CancellationTokenSource();
                return inner.ContinueWith(delegate
                {
                    // Propagate all exceptions
                    Task.WaitAll(outer, inner);

                    // Both completed successfully, so if their keys are equal, return the result
                    if (comparer.Equals(outerKeySelector(outer.Result), innerKeySelector(inner.Result)))
                    {
                        return resultSelector(outer.Result, inner.Result);
                    }
                    // Otherwise, cancel this task.  
                    else
                    {
                        cts.CancelAndThrow();
                        return default(TResult); // won't be reached
                    }
                }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner, 
            Func<TOuter, TKey> outerKeySelector, 
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, Task<TInner>, TResult> resultSelector)
        {
            // Argument validation handled by delegated method call
            return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TInner"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer, Task<TInner> inner, 
            Func<TOuter, TKey> outerKeySelector, 
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, Task<TInner>, TResult> resultSelector, 
            IEqualityComparer<TKey> comparer)
        {
            // Validate arguments
            if (outer == null) throw new ArgumentNullException("outer");
            if (inner == null) throw new ArgumentNullException("inner");
            if (outerKeySelector == null) throw new ArgumentNullException("outerKeySelector");
            if (innerKeySelector == null) throw new ArgumentNullException("innerKeySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            if (comparer == null) throw new ArgumentNullException("comparer");

            // First continue off of the outer and then off of the inner.  Two separate
            // continuations are used so that each may be canceled easily using the NotOnCanceled option.
            return outer.ContinueWith(delegate
            {
                var cts = new CancellationTokenSource();
                return inner.ContinueWith(delegate
                {
                    // Propagate all exceptions
                    Task.WaitAll(outer, inner);

                    // Both completed successfully, so if their keys are equal, return the result
                    if (comparer.Equals(outerKeySelector(outer.Result), innerKeySelector(inner.Result)))
                    {
                        return resultSelector(outer.Result, inner);
                    }
                    // Otherwise, cancel this task.
                    else
                    {
                        cts.CancelAndThrow();
                        return default(TResult); // won't be reached
                    }
                }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public static Task<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
            this Task<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            // Validate arguments
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");

            // When the source completes, return a grouping of just the one element
            return source.ContinueWith(t =>
            {
                var result = t.Result;
                var key = keySelector(result);
                var element = elementSelector(result);
                return (IGrouping<TKey,TElement>)new OneElementGrouping<TKey,TElement> { Key = key, Element = element };
            }, TaskContinuationOptions.NotOnCanceled);
        }

        /// <summary>Represents a grouping of one element.</summary>
        /// <typeparam name="TKey">The type of the key for the element.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        private class OneElementGrouping<TKey,TElement> : IGrouping<TKey, TElement>
        {
            public TKey Key { get; internal set; }
            internal TElement Element { get; set; }
            public IEnumerator<TElement> GetEnumerator() { yield return Element; }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Task<TSource> OrderBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException("source");
            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Task<TSource> OrderByDescending<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException("source");
            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Task<TSource> ThenBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException("source");
            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Task<TSource> ThenByDescending<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector)
        {
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            if (source == null) throw new ArgumentNullException("source");
            return source;
        }

        #region ContinueWith accepting TaskFactory
        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationAction">The continuation action.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task ContinueWith(
            this Task task, Action<Task> continuationAction, TaskFactory factory)
        {
            return task.ContinueWith(continuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        }

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task<TResult> ContinueWith<TResult>(
            this Task task, Func<Task, TResult> continuationFunction, TaskFactory factory)
        {
            return task.ContinueWith(continuationFunction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        }
        #endregion

        #region ContinueWith accepting TaskFactory<TResult>
        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationAction">The continuation action.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task ContinueWith<TResult>(
            this Task<TResult> task, Action<Task<TResult>> continuationAction, TaskFactory<TResult> factory)
        {
            return task.ContinueWith(continuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        }

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="continuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task<TNewResult> ContinueWith<TResult, TNewResult>(
            this Task<TResult> task, Func<Task<TResult>, TNewResult> continuationFunction, TaskFactory<TResult> factory)
        {
            return task.ContinueWith(continuationFunction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        }
        #endregion

        #region ToAsync(AsyncCallback, object)
        /// <summary>
        /// Creates a Task that represents the completion of another Task, and 
        /// that schedules an AsyncCallback to run upon completion.
        /// </summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="callback">The AsyncCallback to run.</param>
        /// <param name="state">The object state to use with the AsyncCallback.</param>
        /// <returns>The new task.</returns>
        public static Task ToAsync(this Task task, AsyncCallback callback, object state)
        {
            if (task == null) throw new ArgumentNullException("task");

            var tcs = new TaskCompletionSource<object>(state);
            task.ContinueWith(_ =>
            {
                tcs.SetFromTask(task);
                if (callback != null) callback(tcs.Task);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Creates a Task that represents the completion of another Task, and 
        /// that schedules an AsyncCallback to run upon completion.
        /// </summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="callback">The AsyncCallback to run.</param>
        /// <param name="state">The object state to use with the AsyncCallback.</param>
        /// <returns>The new task.</returns>
        public static Task<TResult> ToAsync<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            if (task == null) throw new ArgumentNullException("task");

            var tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(_ =>
            {
                tcs.SetFromTask(task);
                if (callback != null) callback(tcs.Task);
            });
            return tcs.Task;
        }
        #endregion

        #region Exception Handling
        /// <summary>Suppresses default exception handling of a Task that would otherwise reraise the exception on the finalizer thread.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task IgnoreExceptions(this Task task)
        {
            task.ContinueWith(t => { var ignored = t.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        /// <summary>Suppresses default exception handling of a Task that would otherwise reraise the exception on the finalizer thread.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task<T> IgnoreExceptions<T>(this Task<T> task)
        {
            return (Task<T>)((Task)task).IgnoreExceptions();
        }

        /// <summary>Fails immediately when an exception is encountered.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task FailFastOnException(this Task task)
        {
            task.ContinueWith(t => Environment.FailFast("A task faulted.", t.Exception),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        /// <summary>Fails immediately when an exception is encountered.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task<T> FailFastOnException<T>(this Task<T> task)
        {
            return (Task<T>)((Task)task).FailFastOnException();
        }

        /// <summary>Propagates any exceptions that occurred on the specified task.</summary>
        /// <param name="task">The Task whose exceptions are to be propagated.</param>
        public static void PropagateExceptions(this Task task)
        {
            if (!task.IsCompleted) throw new InvalidOperationException("The task has not completed.");
            if (task.IsFaulted) task.Wait();
        }

        /// <summary>Propagates any exceptions that occurred on the specified tasks.</summary>
        /// <param name="tasks">The Tassk whose exceptions are to be propagated.</param>
        public static void PropagateExceptions(this Task[] tasks)
        {
            if (tasks == null) throw new ArgumentNullException("tasks");
            if (tasks.Any(t => t == null)) throw new ArgumentException("tasks");
            if (tasks.Any(t => !t.IsCompleted)) throw new InvalidOperationException("A task has not completed.");
            Task.WaitAll(tasks);
        }
        #endregion

        #region Observables
        /// <summary>Creates an IObservable that represents the completion of a Task.</summary>
        /// <typeparam name="TResult">Specifies the type of data returned by the Task.</typeparam>
        /// <param name="task">The Task to be represented as an IObservable.</param>
        /// <returns>An IObservable that represents the completion of the Task.</returns>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task)
        {
            if (task == null) throw new ArgumentNullException("task");
            return new TaskObservable<TResult> { _task = task };
        }

        /// <summary>An implementation of IObservable that wraps a Task.</summary>
        /// <typeparam name="TResult">The type of data returned by the task.</typeparam>
        private class TaskObservable<TResult> : IObservable<TResult>
        {
            internal Task<TResult> _task;

            public IDisposable Subscribe(IObserver<TResult> observer)
            {
                // Validate arguments
                if (observer == null) throw new ArgumentNullException("observer");

                // Support cancelling the continuation if the observer is unsubscribed
                var cts = new CancellationTokenSource();

                // Create a continuation to pass data along to the observer
                _task.ContinueWith(t =>
                {
                    switch (t.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            observer.OnNext(_task.Result);
                            observer.OnCompleted();
                            break;

                        case TaskStatus.Faulted:
                            observer.OnError(_task.Exception);
                            break;

                        case TaskStatus.Canceled:
                            observer.OnError(new TaskCanceledException(t));
                            break;
                    }
                }, cts.Token);

                // Support unsubscribe simply by canceling the continuation if it hasn't yet run
                return new CancelOnDispose { Source = cts };
            }
        }

        /// <summary>Translate a call to IDisposable.Dispose to a CancellationTokenSource.Cancel.</summary>
        private class CancelOnDispose : IDisposable
        {
            internal CancellationTokenSource Source;
            void IDisposable.Dispose() { Source.Cancel(); }
        }
        #endregion

        #region Timeouts
        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task WithTimeout(this Task task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<object>(task.AsyncState);
            var timer = new Timer(state => ((TaskCompletionSource<object>)state).TrySetCanceled(), result, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result.Task;
        }

        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<TResult>(task.AsyncState);
            var timer = new Timer(state => ((TaskCompletionSource<TResult>)state).TrySetCanceled(), result, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result.Task;
        }
        #endregion

        #region Children
        /// <summary>
        /// Ensures that a parent task can't transition into a completed state
        /// until the specified task has also completed, even if it's not
        /// already a child task.
        /// </summary>
        /// <param name="task">The task to attach to the current task as a child.</param>
        public static void AttachToParent(this Task task)
        {
            if (task == null) throw new ArgumentNullException("task");
            task.ContinueWith(t => t.Wait(), CancellationToken.None,
                TaskContinuationOptions.AttachedToParent |
                TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        #endregion

        #region Waiting
        /// <summary>Waits for the task to complete execution, pumping in the meantime.</summary>
        /// <param name="task">The task for which to wait.</param>
        /// <remarks>This method is intended for usage with Windows Presentation Foundation.</remarks>
        public static void WaitWithPumping(this Task task)
        {
            if (task == null) throw new ArgumentNullException("task");
            var nestedFrame = new DispatcherFrame();
            task.ContinueWith(_ => nestedFrame.Continue = false);
            Dispatcher.PushFrame(nestedFrame);
            task.Wait();
        }

        /// <summary>Waits for the task to complete execution, returning the task's final status.</summary>
        /// <param name="task">The task for which to wait.</param>
        /// <returns>The completion status of the task.</returns>
        /// <remarks>Unlike Wait, this method will not throw an exception if the task ends in the Faulted or Canceled state.</remarks>
        public static TaskStatus WaitForCompletionStatus(this Task task)
        {
            if (task == null) throw new ArgumentNullException("task");
            ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
            return task.Status;
        }
        #endregion
    }
}