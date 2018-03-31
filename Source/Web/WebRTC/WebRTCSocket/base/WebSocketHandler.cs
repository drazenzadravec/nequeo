using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;

namespace WebRTCSocket
{
	/// <summary>
	/// Http web socket handler.
	/// </summary>
	public abstract class WebSocketHandler : IHttpHandler
	{
		/// <summary>
		/// Http web socket handler.
		/// </summary>
		protected WebSocketHandler()
		{
		}

		/// <summary>
		/// Called when a new web socket connection has been established.
		/// </summary>
		/// <param name="webSocketContext">The web socket context.</param>
		public abstract Task WebSocketContext(System.Web.WebSockets.AspNetWebSocketContext webSocketContext);

		/// <summary>
		/// Return resuse state option
		/// </summary>
		public virtual bool IsReusable
		{
			// Return false in case your Managed Handler cannot be reused for another request.
			// Usually this would be false in case you have some state information preserved per request.
			get { return true; }
		}

		/// <summary>
		/// Process request method.
		/// </summary>
		/// <param name="context">The current http context.</param>
		public void ProcessRequest(System.Web.HttpContext context)
		{
			HttpResponse response = null;

			// If the context exists.
			if (context != null)
			{
				// Get the request and response context.
				response = context.Response;

				// If the request is a web socket protocol
				if (context.IsWebSocketRequest)
				{
					// Process the request.
					ProcessWebSocketRequest(context);
				}
				else
				{
					try
					{
						if (response != null)
						{
							// Get the response OutputStream and write the response to it.
							response.AddHeader("Content-Length", (0).ToString());
							response.StatusCode = (int)HttpStatusCode.BadRequest;
							response.StatusDescription = "Bad Request";
							response.Close();
						}
					}
					catch { }
				}
			}
			else
			{
				try
				{
					if (response != null)
					{
						// Get the response OutputStream and write the response to it.
						response.AddHeader("Content-Length", (0).ToString());
						response.StatusCode = (int)HttpStatusCode.InternalServerError;
						response.StatusDescription = "Internal Server Error";
						response.Close();
					}
				}
				catch { }
			}
		}

		/// <summary>
		/// Process the request.
		/// </summary>
		/// <param name="httpContext">The http context.</param>
		private void ProcessWebSocketRequest(System.Web.HttpContext httpContext)
		{
			HttpResponse response = null;

			try
			{
				// Get the request and response context.
				response = httpContext.Response;

				// Process the request asynchronously.
				httpContext.AcceptWebSocketRequest(ProcessWebSocketRequestAsync);
			}
			catch (Exception)
			{
				try
				{
					if (response != null)
					{
						// Get the response OutputStream and write the response to it.
						response.AddHeader("Content-Length", (0).ToString());
						response.StatusCode = (int)HttpStatusCode.InternalServerError;
						response.StatusDescription = "Internal Server Error";
						response.Close();
					}
				}
				catch { }
			}
		}

		/// <summary>
		/// Process the request asynchronously.
		/// </summary>
		/// <param name="webSocketContext">The web socket context.</param>
		/// <returns>The task to execute.</returns>
		private Task ProcessWebSocketRequestAsync(System.Web.WebSockets.AspNetWebSocketContext webSocketContext)
		{
			// Process the request.
			return WebSocketContext(webSocketContext);
		}
	}
}