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
using System.Data;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nequeo.Threading
{
    /// <summary>
    /// Asynchronous operation result
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    public class AsyncOperationResult<T>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="state">The user state.</param>
        /// <param name="name">The unique name of the operation.</param>
        public AsyncOperationResult(T result, object state, object name)
        {
            _result = result;
            _state = state;
            _name = name;
        }

        private T _result = default(T);
        private object _state = null;
        private object _name = null;

        /// <summary>
        /// Gets the result of the operation.
        /// </summary>
        public T Result
        {
            get { return _result; }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public object State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets the unique name of the operation.
        /// </summary>
        public object Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <param name="action">The action to perform; where the action delegate method implements the 'async' and 'await' keywords.</param>
        /// <returns>The result of the action.</returns>
        public async static Task RunTask(Action action)
        {
            await System.Threading.Tasks.Task.Run(action);
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <typeparam name="TResult">The task generic type.</typeparam>
        /// <param name="action">The action to perform; where the action delegate method implements the 'async' and 'await' keywords.</param>
        /// <returns>The result of the action.</returns>
        public async static Task<TResult> RunTask<TResult>(Func<TResult> action)
        {
            var result = System.Threading.Tasks.Task.Run(action);
            return await result;
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <typeparam name="TResult">The task generic type.</typeparam>
        /// <param name="action">The action to perform; where the action delegate method implements the 'async' and 'await' keywords.</param>
        /// <returns>The result of the action.</returns>
        public async static Task<TResult> RunTaskEx<TResult>(Func<Task<TResult>> action)
        {
            var result = System.Threading.Tasks.Task.Run<TResult>(action);
            return await result;
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <returns>The result of the action.</returns>
        public async static Task<T> RunTaskResult(Task<T> task)
        {
            // Start a new thread to execute the operation asynchronously
            return await System.Threading.Tasks.Task.Run<T>(async () =>
            {
                var result = await task;
                return result;
            });
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <param name="action">The method to execute on the instance type.</param>
        /// <returns>The task result.</returns>
        public async static Task RunTaskResult(Action action)
        {
            // Start a new thread to execute the operation asynchronously
            await System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    // Execute the action.
                    action();
                }
                catch { }

                await Task.Delay(5);
                return;
            });
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <param name="action">The method to execute on the instance type.</param>
        /// <returns>The task result.</returns>
        public async static Task<T> RunTaskResult(Func<T> action)
        {
            // Start a new thread to execute the operation asynchronously
            return await System.Threading.Tasks.Task.Run<T>(async () =>
            {
                T result = default(T);
                try
                {
                    // Execute the action.
                    result = action();
                }
                catch { }

                await Task.Delay(5);
                return result;
            });
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <typeparam name="I">The instance of the type I.</typeparam>
        /// <param name="instance">The instnce of the type.</param>
        /// <param name="action">The method to execute on the instance type.</param>
        /// <returns>The task result.</returns>
        public async static Task RunTaskResult<I>(I instance, Action<I> action) where I : class
        {
            // Start a new thread to execute the operation asynchronously
            await System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    // Execute the action.
                    action(instance);
                }
                catch { }
                
                await Task.Delay(5);
                return;
            });
        }

        /// <summary>
        /// Run a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <typeparam name="I">The instance of the type I.</typeparam>
        /// <param name="instance">The instnce of the type.</param>
        /// <param name="action">The method to execute on the instance type.</param>
        /// <returns>The task result.</returns>
        public async static Task<T> RunTaskResult<I>(I instance, System.Func<I, T> action) where I : class
        {
            // Start a new thread to execute the operation asynchronously
            return await System.Threading.Tasks.Task.Run<T>(async () =>
            {
                T result = default(T);
                try
                {
                    // Execute the action.
                    result = action(instance);
                }
                catch { }
                
                // Return the result.
                await Task.Delay(5);
                return result;
            });
        }

        /// <summary>
        /// Start a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <param name="action">The method to execute on the instance type.</param>
        /// <returns>The task result.</returns>
        public static Task<T> Start(Func<T> action)
        {
            // Create a new task.
            return Task<T>.Factory.StartNew(action);
        }

        /// <summary>
        /// Start a task, where the action delegate method implements the 'async' and 'await' keywords.
        /// </summary>
        /// <param name="action">The method to execute on the instance type.</param>
        /// <param name="state">The state object to pass.</param>
        /// <returns>The task result.</returns>
        public static Task<T> Start(Func<object, T> action, object state)
        {
            // Create a new task.
            return Task<T>.Factory.StartNew(action, state);
        }
    }

    /// <summary>
    /// The asynchronous execution handler class.
    /// </summary>
    /// <typeparam name="T">The type that to execute asynchronously</typeparam>
    public class AsyncExecutionHandler<T> where T : class
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AsyncExecutionHandler()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="instance">The instance of the current type.</param>
        public AsyncExecutionHandler(T instance)
        {
            InitiliseAsyncInstance(instance);
        }

        private T _instance = default(T);
        private Nequeo.Threading.AsyncExecution<T> _asyncExecute = null;
        private Dictionary<object, object> _callback = new Dictionary<object, object>();
        private Dictionary<object, object> _state = new Dictionary<object, object>();

        /// <summary>
        /// Async error event handler
        /// </summary>
        public event Nequeo.Threading.EventHandler<System.Exception> AsyncError;

        /// <summary>
        /// Async complete event handler, with result and unique execution name
        /// </summary>
        public event Nequeo.Threading.EventHandler<System.Object, System.String> AsyncComplete;

        /// <summary>
        /// Gets the current type instance.
        /// </summary>
        public T Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <param name="instance">The instance of the type T.</param>
        /// <param name="action">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public static void ExecuteAsync(T instance, System.Action<T> action, string actionName, 
            Action<Nequeo.Threading.AsyncOperationResult<Boolean>> callback, object state = null)
        {
            new AsyncExecution<T>(instance).ExecuteInternal(action, actionName, callback, state);
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <typeparam name="TResult">The action result type.</typeparam>
        /// <param name="instance">The instance of the type T.</param>
        /// <param name="action">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public static void ExecuteAsync<TResult>(T instance, System.Func<T, TResult> action, string actionName, 
            Action<Nequeo.Threading.AsyncOperationResult<TResult>> callback, object state = null)
        {
            new AsyncExecution<T>(instance).ExecuteInternal<TResult>(action, actionName, callback, state);
        }

        /// <summary>
        /// Initilise Async Instance
        /// </summary>
        /// <param name="instance">The type instance.</param>
        public void InitiliseAsyncInstance(T instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            _instance = instance;
            _asyncExecute = new Nequeo.Threading.AsyncExecution<T>(instance);
            _asyncExecute.AsyncExecuteComplete += new Nequeo.Threading.EventHandler<object, bool, System.Exception>(AsyncHandler_AsyncExecuteComplete);
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <param name="action">The action handler</param>
        public virtual async Task<Boolean> Execute(Action<T> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            // Return the result.
            return await _asyncExecute.Execute(action);
        }

        /// <summary>
        /// Execute the asynchronous action.
        /// </summary>
        /// <param name="action">The action handler.</param>
        /// <param name="actionName">The unique action name.</param>
        public virtual void Execute(System.Action<T> action, string actionName)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (String.IsNullOrEmpty(actionName)) throw new ArgumentNullException("actionName");
            _asyncExecute.Execute(action, actionName);
        }

        /// <summary>
        /// Execute the asynchronous action.
        /// </summary>
        /// <param name="action">The action handler.</param>
        /// <param name="actionName">The unique action name.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void Execute(System.Action<T> action, string actionName, Action<Nequeo.Threading.AsyncOperationResult<Object>> callback, object state = null)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            if (action == null) throw new ArgumentNullException("action");
            if (String.IsNullOrEmpty(actionName)) throw new ArgumentNullException("actionName");

            // Set the async value.
            _callback[actionName] = callback;
            _state[actionName] = state;
            _asyncExecute.Execute(action, actionName);
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <typeparam name="TResult">The action result type.</typeparam>
        /// <param name="action">The action handler</param>
        public virtual async Task<TResult> Execute<TResult>(Func<T, TResult> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            // Return the result.
            return await _asyncExecute.Execute<TResult>(action);
        }

        /// <summary>
        /// Execute the asynchronous action.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="action">The action handler.</param>
        /// <param name="actionName">The unique action name.</param>
        public virtual void Execute<TResult>(System.Func<T, TResult> action, string actionName)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (String.IsNullOrEmpty(actionName)) throw new ArgumentNullException("actionName");
            _asyncExecute.Execute<TResult>(action, actionName);
        }

        /// <summary>
        /// Execute the asynchronous action.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="action">The action handler.</param>
        /// <param name="actionName">The unique action name.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public virtual void Execute<TResult>(System.Func<T, TResult> action, string actionName, Action<Nequeo.Threading.AsyncOperationResult<Object>> callback, object state = null)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            if (action == null) throw new ArgumentNullException("action");
            if (String.IsNullOrEmpty(actionName)) throw new ArgumentNullException("actionName");

            // Set the async value.
            _callback[actionName] = callback;
            _state[actionName] = state;
            _asyncExecute.Execute<TResult>(action, actionName);
        }

        /// <summary>
        /// Asynchronous Execution Complete
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e1">The unique async name reference.</param>
        /// <param name="e2">The operation result.</param>
        /// <param name="e3">The current async exception.</param>
        private void AsyncHandler_AsyncExecuteComplete(object sender, object e1, bool e2, System.Exception e3)
        {
            try
            {
                if (e1 is string)
                {
                    // Send the result.
                    object result = _asyncExecute.GetExecuteAsyncResult<object>(e1.ToString());
                    if (AsyncComplete != null)
                    {
                        AsyncComplete(this, result, e1.ToString());
                    }

                    // Sent the error.
                    if (AsyncError != null)
                    {
                        if (e3 != null)
                        {
                            AsyncError(this, e3);
                        }
                    }

                    // Send the async result.
                    object callback = null;
                    if (_callback.TryGetValue(e1, out callback))
                    {
                        Action<Nequeo.Threading.AsyncOperationResult<object>> callbackObject = (Action<Nequeo.Threading.AsyncOperationResult<object>>)callback;
                        callbackObject(new Nequeo.Threading.AsyncOperationResult<object>(result, _state[e1], e1));
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (AsyncError != null)
                {
                    AsyncError(this, new System.Exception(ex.Message, _asyncExecute.GetExecuteAsyncException(e1.ToString())));
                }
            }
        }
    }

    /// <summary>
    /// Asynchronous execution type handler.
    /// </summary>
    /// <typeparam name="T">The type that to execute asynchronously</typeparam>
	public class AsyncExecution<T> where T : class
	{
        /// <summary>
        /// Defualt constructor.
        /// </summary>
        /// <param name="instance">The instance of the current type.</param>
        public AsyncExecution(T instance)
        {
            _instance = instance;
        }

        private T _instance = default(T);
        private Exception _exception = null;
        private Dictionary<object, object> _results = new Dictionary<object, object>();
        private Dictionary<object, Exception> _exceptionMessage = new Dictionary<object, Exception>();
        private Dictionary<object, bool> _executionExceptionResult = new Dictionary<object, bool>();

        /// <summary>
        /// The async execute complete event.
        /// </summary>
        public event Nequeo.Threading.EventHandler<object, bool, Exception> AsyncExecuteComplete;

        /// <summary>
        /// Gets the current type instance.
        /// </summary>
        public T Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Get most recent exception.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Get the execution exception.
        /// </summary>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <returns>The exception of the execution; else null.</returns>
        public Exception GetExecuteAsyncException(object actionName)
        {
            return _exceptionMessage[actionName];
        }

        /// <summary>
        /// Get the execution exception result.
        /// </summary>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <returns>True is an exceptionhas occurred in the action; else false.</returns>
        public bool GetExecuteAsyncExceptionResult(object actionName)
        {
            return _executionExceptionResult[actionName];
        }

        /// <summary>
        /// Get the result of the async execution.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <returns>The result type.</returns>
        public TResult GetExecuteAsyncResult<TResult>(object actionName)
        {
            return ((TResult)_results[actionName]);
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <param name="channelAction">The action handler</param>
        public async Task<Boolean> Execute(Action<T> channelAction)
        {
            // Start the async object
            AsyncClientExecute<T, Boolean> ret = new AsyncClientExecute<T, Boolean>(channelAction, _instance, null, null);

            // Start the action asynchronously
            Task<Boolean> data = Task<Boolean>.Factory.FromAsync(ret.BeginActionNoResult(), ret.EndActionNoResult);
            object actionAsyncResult = await data;

            // Return the result.
            return (Boolean)actionAsyncResult;
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <param name="channelAction">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        internal async void ExecuteInternal(Action<T> channelAction, object actionName, 
            Action<Nequeo.Threading.AsyncOperationResult<Boolean>> callback, object state = null)
        {
            try
            {
                // Start the async object
                AsyncClientExecute<T, Boolean> ret = new AsyncClientExecute<T, Boolean>(channelAction, _instance, null, null);

                // Start the action asynchronously
                Task<Boolean> data = Task<Boolean>.Factory.FromAsync(ret.BeginActionNoResult(), ret.EndActionNoResult);
                object actionAsyncResult = await data;

                // If sending the result to a callback method.
                if (callback != null)
                    callback(new AsyncOperationResult<Boolean>((Boolean)actionAsyncResult, state, actionName));
            }
            catch { }
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <param name="channelAction">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public async void Execute(Action<T> channelAction, object actionName, Action<Nequeo.Threading.AsyncOperationResult<Object>> callback = null, object state = null)
        {
            try
            {
                // Start the async object
                AsyncClientExecute<T, Boolean> ret = new AsyncClientExecute<T, Boolean>(channelAction, _instance, null, null);

                // Start the action asynchronously
                Task<Boolean> data = Task<Boolean>.Factory.FromAsync(ret.BeginActionNoResult(), ret.EndActionNoResult);
                object actionAsyncResult = await data;

                // Get the current error.
                Exception exception = ret.GetCurrentError();
                if (exception != null)
                {
                    _exception = exception;
                    _exceptionMessage[actionName] = exception;
                    _executionExceptionResult[actionName] = true;
                }
                else
                {
                    _exceptionMessage[actionName] = null;
                    _executionExceptionResult[actionName] = false;
                }
                    
                // Set the async value.
                _results[actionName] = actionAsyncResult;

                // Send the result back to the client
                if (AsyncExecuteComplete != null)
                    AsyncExecuteComplete(this, actionName, _executionExceptionResult[actionName], _exceptionMessage[actionName]);

                // If sending the result to a callback method.
                if (callback != null)
                    callback(new AsyncOperationResult<Object>(_results[actionName], state, actionName));
            }
            catch { }
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <typeparam name="TResult">The action result type.</typeparam>
        /// <param name="channelAction">The action handler</param>
        public async Task<TResult> Execute<TResult>(Func<T, TResult> channelAction)
        {
            // Start the async object
            AsyncClientExecute<T, TResult> ret = new AsyncClientExecute<T, TResult>(channelAction, _instance, null, null);

            // Start the action asynchronously
            Task<TResult> data = Task<TResult>.Factory.FromAsync(ret.BeginActionResult(), ret.EndActionResult);
            object actionAsyncResult = await data;

            // Return the result.
            return (TResult)actionAsyncResult;
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <typeparam name="TResult">The action result type.</typeparam>
        /// <param name="channelAction">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        internal async void ExecuteInternal<TResult>(Func<T, TResult> channelAction, object actionName, 
            Action<Nequeo.Threading.AsyncOperationResult<TResult>> callback, object state = null)
        {
            try
            {
                // Start the async object
                AsyncClientExecute<T, TResult> ret = new AsyncClientExecute<T, TResult>(channelAction, _instance, null, null);

                // Start the action asynchronously
                Task<TResult> data = Task<TResult>.Factory.FromAsync(ret.BeginActionResult(), ret.EndActionResult);
                object actionAsyncResult = await data;

                // If sending the result to a callback method.
                if (callback != null)
                    callback(new AsyncOperationResult<TResult>((TResult)actionAsyncResult, state, actionName));
            }
            catch { }
        }

        /// <summary>
        /// Execute the asynchronous action for the channel.
        /// </summary>
        /// <typeparam name="TResult">The action result type.</typeparam>
        /// <param name="channelAction">The action handler</param>
        /// <param name="actionName">The unique action name; passed to the object sender of the AsyncExecuteComplete handler.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        public async void Execute<TResult>(Func<T, TResult> channelAction, object actionName, Action<Nequeo.Threading.AsyncOperationResult<Object>> callback = null, object state = null)
        {
            try
            {
                // Start the async object
                AsyncClientExecute<T, TResult> ret = new AsyncClientExecute<T, TResult>(channelAction, _instance, null, null);

                // Start the action asynchronously
                Task<TResult> data = Task<TResult>.Factory.FromAsync(ret.BeginActionResult(), ret.EndActionResult);
                object actionAsyncResult = await data;

                // Get the current error.
                Exception exception = ret.GetCurrentError();
                if (exception != null)
                {
                    _exception = exception;
                    _exceptionMessage[actionName] = exception;
                    _executionExceptionResult[actionName] = true;
                }
                else
                {
                    _exceptionMessage[actionName] = null;
                    _executionExceptionResult[actionName] = false;
                }

                // Set the async value.
                _results[actionName] = actionAsyncResult;

                // Send the result back to the client
                if (AsyncExecuteComplete != null)
                    AsyncExecuteComplete(this, actionName, _executionExceptionResult[actionName], _exceptionMessage[actionName]);

                // If sending the result to a callback method.
                if (callback != null)
                    callback(new AsyncOperationResult<Object>(_results[actionName], state, actionName));
            }
            catch { }
        }
	}

    /// <summary>
    /// Asyncronous client channel execute
    /// </summary>
    /// <typeparam name="T">The type to execute</typeparam>
    /// <typeparam name="TResult">The return type function handler</typeparam>
    internal sealed class AsyncClientExecute<T, TResult> : Nequeo.Threading.AsynchronousResult<TResult>
    {
        /// <summary>
        /// Default async action handler
        /// </summary>
        /// <param name="channelAction">The action operation channel</param>
        /// <param name="instance">The current instance</param>
        /// <param name="callback">The call back handler</param>
        /// <param name="state">The state object</param>
        public AsyncClientExecute(Func<T, TResult> channelAction, T instance, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _channelAction = channelAction;
            _instance = instance;
            _exception = null;
        }

        /// <summary>
        /// Default async action handler
        /// </summary>
        /// <param name="channelAction">The action operation channel</param>
        /// <param name="instance">The current instance</param>
        /// <param name="callback">The call back handler</param>
        /// <param name="state">The state object</param>
        public AsyncClientExecute(Action<T> channelAction, T instance, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _channelActionEx = channelAction;
            _instance = instance;
            _exception = null;
        }

        private Func<TResult> _actionHandler = null;
        private Func<Boolean> _actionNoResultHandler = null;
        private Func<T, TResult> _channelAction = null;
        private Action<T> _channelActionEx = null;
        private T _instance = default(T);
        private Exception _exception = null;

        /// <summary>
        /// Begin the async operation.
        /// </summary>
        /// <returns>The async result.</returns>
        public IAsyncResult BeginActionResult()
        {
            if (_actionHandler == null)
                _actionHandler = new Func<TResult>(FuncAsyncActionResult);

            // Begin the async call.
            return _actionHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End the async operation.
        /// </summary>
        /// <param name="ar">The async result</param>
        /// <returns>The result type.</returns>
        public TResult EndActionResult(IAsyncResult ar)
        {
            if (_actionHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            return _actionHandler.EndInvoke(ar);
        }

        /// <summary>
        /// Begin async action
        /// </summary>
        /// <returns>The async result</returns>
        public IAsyncResult BeginActionNoResult()
        {
            if (_actionNoResultHandler == null)
                _actionNoResultHandler = new Func<Boolean>(FuncAsyncActionNoResult);

            // Begin the async call.
            return _actionNoResultHandler.BeginInvoke(base.Callback, base.AsyncState);
        }

        /// <summary>
        /// End async action
        /// </summary>
        /// <param name="ar">The async result</param>
        public Boolean EndActionNoResult(IAsyncResult ar)
        {
            if (_actionNoResultHandler == null)
                throw new System.InvalidOperationException("End of asynchronous" +
                    " operation attempted when one has not begun.");

            // Use the AsyncResult to complete that async operation.
            return _actionNoResultHandler.EndInvoke(ar);
        }

        /// <summary>
        /// Gets the current error if any.
        /// </summary>
        /// <returns>The current exception if any.</returns>
        public Exception GetCurrentError()
        {
            return _exception;
        }

        /// <summary>
        /// Execute the asyn result.
        /// </summary>
        /// <returns>The result type to return.</returns>
        private TResult FuncAsyncActionResult()
        {
            try
            {
                return _channelAction(_instance);
            }
            catch (Exception ex)
            {
                _exception = ex;
                return default(TResult);
            }
        }

        /// <summary>
        /// Execute the asyn result.
        /// </summary>
        private Boolean FuncAsyncActionNoResult()
        {
            try
            {
                _channelActionEx(_instance);
                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }
    }
}
