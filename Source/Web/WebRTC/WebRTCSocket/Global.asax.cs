using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections.Concurrent;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebRTCSocket
{
	/// <summary>
	/// Application.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// The collection of all the clients connected.
		/// </summary>
		internal static ConcurrentDictionary<WebSocketClient, WebSocket> Clients = null;
		/// <summary>
		/// The current server id.
		/// </summary>
		internal static string ServerID = null;
		/// <summary>
		/// Global timeout handler.
		/// </summary>
		internal static TimeoutHandler Timeout = null;
		internal static CancellationTokenSource TimeoutCancelToken = null;
		internal static CancellationTokenSource UdpHandlerCancelToken = null;

		/// <summary>
		/// UDP socket handler.
		/// </summary>
		internal static UdpSocketHandler UdpSocket = null;

		internal static string AccessTokenVerifyURL = null;
		internal static string ClientLocationRequestURL = null;
		internal static bool ClientLocationRequestEnabled = false;
		internal static bool UdpBroadcastEnabled = false;
		internal static uint UdpBroadcastPort = 0;
		internal static string UdpBroadcastAddress = null;
		internal static string UdpBroadcastMask = null;
		internal static uint UdpBroadcastCallbackPort = 0;
		internal static uint ClientTimeoutConnect = 0;

		/// <summary>
		/// Application start.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Start(object sender, EventArgs e)
		{
			Global.Clients = new ConcurrentDictionary<WebSocketClient, WebSocket>();
			Global.ServerID = Guid.NewGuid().ToString();
			Global.Timeout = new TimeoutHandler();
			Global.TimeoutCancelToken = new CancellationTokenSource();
			Global.Timeout.Start(Global.TimeoutCancelToken.Token);

			Global.AccessTokenVerifyURL = WebRTCSocket.Properties.Settings.Default.AccessTokenVerifyURL;
			Global.ClientLocationRequestURL = WebRTCSocket.Properties.Settings.Default.ClientLocationRequestURL;
			Global.ClientLocationRequestEnabled = WebRTCSocket.Properties.Settings.Default.ClientLocationRequestEnabled;
			Global.UdpBroadcastEnabled = WebRTCSocket.Properties.Settings.Default.UdpBroadcastEnabled;
			Global.UdpBroadcastPort = WebRTCSocket.Properties.Settings.Default.UdpBroadcastPort;
			Global.UdpBroadcastAddress = WebRTCSocket.Properties.Settings.Default.UdpBroadcastAddress;
			Global.UdpBroadcastMask = WebRTCSocket.Properties.Settings.Default.UdpBroadcastMask;
			Global.UdpBroadcastCallbackPort = WebRTCSocket.Properties.Settings.Default.UdpBroadcastCallbackPort;
			Global.ClientTimeoutConnect = WebRTCSocket.Properties.Settings.Default.ClientTimeoutConnect;

			// UDP socket handler.
			Global.UdpSocket = new UdpSocketHandler(Global.UdpBroadcastEnabled, (int)Global.UdpBroadcastPort, (int)Global.UdpBroadcastCallbackPort);

			// If UDP broadcast is enabled.
			if (Global.UdpBroadcastEnabled && Global.UdpBroadcastPort > 0 && Global.UdpBroadcastCallbackPort > 0)
			{
				// Start the UDP handler.
				Global.UdpHandlerCancelToken = new CancellationTokenSource();
				Global.UdpSocket.Start(Global.UdpHandlerCancelToken.Token);
			}
		}
		/// <summary>
		/// Session start.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Session_Start(object sender, EventArgs e)
		{

		}
		/// <summary>
		/// Application begin request.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}
		/// <summary>
		/// Application auth request.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}
		/// <summary>
		/// Application error.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Error(object sender, EventArgs e)
		{

		}
		/// <summary>
		/// Session end.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Session_End(object sender, EventArgs e)
		{

		}
		/// <summary>
		/// Application end.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_End(object sender, EventArgs e)
		{
			if (Global.Clients != null)
			{
				try
				{
					// Stop the timeout handler.
					Global.Timeout.Stop(Global.TimeoutCancelToken);
				}
				catch { }

				try
				{
					// Stop the UDP broadcast handler.
					Global.UdpSocket.Stop(Global.UdpHandlerCancelToken);
				}
				catch { }

				// For each client.
				foreach (var client in Global.Clients)
				{
					try
					{
						// Close the connection.
						client.Value.Abort();
					}
					catch { }
				}

				try
				{
					Global.Clients.Clear();
				}
				catch { }
			}
			Global.Clients = null;
			Global.ServerID = null;
			Global.Timeout = null;
			Global.UdpSocket = null;
		}
	}
}