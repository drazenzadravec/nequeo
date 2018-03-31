using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebRTCSocket
{
	/// <summary>
	/// Timeout model.
	/// </summary>
	public class TimeoutModel
	{
		/// <summary>
		/// Gets or sets the start time of the timeout.
		/// </summary>
		public DateTime Start { get; set; }

		/// <summary>
		/// Gets or sets the time span timeout.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// Gets or sets the web socket client.
		/// </summary>
		public WebSocketClient Client { get; set; }

		/// <summary>
		/// Gets or sets the web socket.
		/// </summary>
		public WebSocket Socket { get; set; }

		/// <summary>
		/// Gets or sets the receive cancel token.
		/// </summary>
		public CancellationTokenSource ReceiveCancelToken { get; set; }
	}

	/// <summary>
	/// Timeout handler.
	/// </summary>
	public class TimeoutHandler
	{
		/// <summary>
		/// Timeout handler.
		/// </summary>
		public TimeoutHandler()
		{
			_timeout = new ConcurrentDictionary<TimeoutModel, Action<TimeoutModel, WebSocket>>();
		}

		private bool _started = false;
		private ConcurrentDictionary<TimeoutModel, Action<TimeoutModel, WebSocket>> _timeout = null;

		/// <summary>
		/// Register the timeout.
		/// </summary>
		/// <param name="timeout">The timeout time span.</param>
		/// <param name="handler">The active handler to execute.</param>
		public void Register(TimeoutModel timeout, Action<TimeoutModel, WebSocket> handler)
		{
			// If started.
			if (_started)
			{
				_timeout.TryAdd(timeout, handler);
			}
		}

		/// <summary>
		/// Unregister the timeout.
		/// </summary>
		/// <param name="timeout">The timeout time span.</param>
		public void Unregister(TimeoutModel timeout)
		{
			// If started.
			if (_started)
			{
				Action<TimeoutModel, WebSocket> action = null;
				_timeout.TryRemove(timeout, out action);
			}
		}

		/// <summary>
		/// Start the timeout handler.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		public void Start(CancellationToken token)
		{
			// If not started.
			if (!_started)
			{
				_started = true;

				// Start the spinner.
				Task.Factory.StartNew(() =>
				{
					// Start.
					Spin(token);

				}, token);
			}
		}

		/// <summary>
		/// Stop the timeout handler.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		public void Stop(CancellationTokenSource token)
		{
			// If started.
			if (_started)
			{
				try
				{
					_timeout.Clear();
				}
				catch { }

				// Cancel the  operation.
				token.Cancel();
			}
		}

		/// <summary>
		/// Spin through the timeout handlers.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		private void Spin(CancellationToken token)
		{
			bool exitIndicator = false;

			// Create the tasks.
			Task[] tasks = new Task[1];

			// Poller task.
			Task poller = Task.Factory.StartNew(() =>
			{
				// Create a new spin wait.
				SpinWait sw = new SpinWait();

				// Action to perform.
				while (!exitIndicator)
				{
					DateTime now = DateTime.Now;

					try
					{
						// Get a copy of all timeouts.
						var tm = _timeout.ToArray();
						for (int i = 0; i < tm.Length; i++)
						{
							var u = tm[i];

							// Determin if is timeout.
							DateTime elapsed = u.Key.Start.Add(u.Key.Timeout);
							if (now.Ticks > elapsed.Ticks)
							{
								// Call the action.
								u.Value(u.Key, u.Key.Socket);

								// Remove this timeout.
								Unregister(u.Key);
							}
						}
					}
					catch { }

					// The NextSpinWillYield property returns true if 
					// calling sw.SpinOnce() will result in yielding the 
					// processor instead of simply spinning. 
					if (sw.NextSpinWillYield)
					{
						if (token.IsCancellationRequested) exitIndicator = true;
					}
					sw.SpinOnce();
				}
			});

			// Assign the listener task.
			tasks[0] = poller;

			// Wait for all tasks to complete.
			Task.WaitAll(tasks);

			// For each task.
			foreach (Task item in tasks)
			{
				try
				{
					// Release the resources.
					item.Dispose();
				}
				catch { }
			}
			tasks = null;
		}
	}
}