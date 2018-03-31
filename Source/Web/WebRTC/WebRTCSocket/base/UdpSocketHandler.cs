using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Web.WebSockets;
using System.Net.WebSockets;

namespace WebRTCSocket
{
	/// <summary>
	/// UDP broadcast handler.
	/// </summary>
	public class UdpSocketHandler
	{
		/// <summary>
		/// UDP broadcast handler.
		/// </summary>
		/// <param name="enabled">Is broadcasting enabled.</param>
		/// <param name="broadcastListenerPort">The broadcast listening port.</param>
		/// <param name="callbackListenerPort">The callback listening port.</param>
		public UdpSocketHandler(bool enabled, int broadcastListenerPort, int callbackListenerPort)
		{
			_enabled = enabled;
			_broadcastListenerPort = broadcastListenerPort;
			_callbackListenerPort = callbackListenerPort;
		}

		private bool _started = false;
		private bool _enabled = false;
		private int _broadcastListenerPort = 0;
		private int _callbackListenerPort = 0;

		private UdpClient _broadcastListener = null;
		private UdpClient _callbackListener = null;

		private IPEndPoint _broadcastEndPoint = null;
		private IPEndPoint _callbackEndPoint = null;

		/// <summary>
		/// Start the broadcast handler.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		public void Start(CancellationToken token)
		{
			// If not started.
			if (!_started && _enabled)
			{
				_started = true;

				_broadcastEndPoint = new IPEndPoint(IPAddress.Any, _broadcastListenerPort);
				_callbackEndPoint = new IPEndPoint(IPAddress.Any, _callbackListenerPort);
				_broadcastListener = new UdpClient(_broadcastEndPoint);
				_callbackListener = new UdpClient(_callbackEndPoint);

				// Start the spinner.
				Task.Factory.StartNew(() =>
				{
					// Start.
					BroadcastListener(token);

				}, token);

				// Start the spinner.
				Task.Factory.StartNew(() =>
				{
					// Start.
					CallbackListener(token);

				}, token);
			}
		}

		/// <summary>
		/// Stop the broadcast handler.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		public void Stop(CancellationTokenSource token)
		{
			// If started.
			if (_started && _enabled)
			{
				try
				{
					if (_broadcastListener != null)
						_broadcastListener.Close();
				}
				catch { }

				try
				{
					if (_callbackListener != null)
						_callbackListener.Close();
				}
				catch { }

				// Cancel the  operation.
				token.Cancel();
			}
		}

		/// <summary>
		/// Broadcast to subnet (e.g. IPAddress 192.168.1.10 with SubnetMask 255.255.0.0 will result
		/// in sending to all hosts with IPAddress 192.168.0.1 to 192.168.254.254).
		/// </summary>
		/// <param name="data">The data to send.</param>
		/// <param name="baseIPAddress">The base IP address (e.g. 192.168.1.10)</param>
		/// <param name="IPSubnetMask">The subnet-mask to apply on the IP address (e.g. 255.255.0.0)</param>
		/// <param name="wsClient">The WebSocket client.</param>
		/// <param name="wsContact">The WebSocket contact.</param>
		public async void Broadcast(string data, string baseIPAddress, string IPSubnetMask, WebSocketClient wsClient, WebSocketContact wsContact)
		{
			// If started.
			if (_started && _enabled)
			{
				string udpAddress = baseIPAddress;
				string udpMask = IPSubnetMask;

				// If client location request enabled.
				if (Global.ClientLocationRequestEnabled)
				{
					try
					{
						// Make a connection to the access token verification service.
						using (HttpClient httpClient = new HttpClient())
						{
							// Add header with access token.
							string authValueBearer = "Bearer ";
							string authValueToken = wsClient.AccessToken;
							string authValue = authValueBearer + authValueToken;
							httpClient.DefaultRequestHeaders.Add("Authorization", authValue);
							httpClient.DefaultRequestHeaders.Add("UniqueID", wsClient.UniqueID);
							httpClient.DefaultRequestHeaders.Add("ApplicationID", wsClient.ApplicationID);
							httpClient.DefaultRequestHeaders.Add("ContactUniqueID", wsContact.UniqueID);
							httpClient.DefaultRequestHeaders.Add("ContactApplicationID", wsContact.ApplicationID);

							// Verify the access token for the client.
							HttpResponseMessage response = await httpClient.GetAsync(new Uri(Global.ClientLocationRequestURL));
							if (response.IsSuccessStatusCode)
							{
								// Get the json response.
								string jsonData = await response.Content.ReadAsStringAsync();

								// Get the JSON data.
								JsonGenerator genAccess = new JsonGenerator(jsonData);
								genAccess.Namespace = "Lake.WebSocketServer";
								genAccess.RootClassName = "RootWebSocketData";
								var extractAccess = genAccess.Extract();

								// Search for the client loaction.
								bool foundClientLocation = false;
								var clientLocation = genAccess.GetClientLocation(out foundClientLocation);

								// If the client location has been found.
								if (foundClientLocation)
								{
									udpAddress = clientLocation.Address;
									udpMask = clientLocation.Mask;
								}
							}
						}
					}
					catch { }
				}

				// Open the client.
				using (UdpClient client = new UdpClient())
				{
					// Enable broadcasting.
					client.EnableBroadcast = true;

					// If either is empty.
					if (String.IsNullOrEmpty(udpAddress) || String.IsNullOrEmpty(udpMask))
					{
						// If data exists.
						if (!String.IsNullOrEmpty(data))
						{
							// Broadcast to all.
							byte[] toSent = System.Text.Encoding.ASCII.GetBytes(data);

							try
							{
								// Sent the message.
								IPEndPoint endPointAll = new IPEndPoint(IPAddress.Broadcast, _broadcastListenerPort);
								int sent = await client.SendAsync(toSent, toSent.Length, endPointAll);
							}
							catch { }
						}
					}
					else
					{
						// Get the IP address.
						IPAddress address = null;
						bool isIP = IPAddress.TryParse(udpAddress, out address);

						// Get the subnet mask.
						IPAddress mask = null;
						bool isIPMask = IPAddress.TryParse(udpMask, out mask);

						// If both are valid.
						if (isIP && isIPMask)
						{
							// If data exists.
							if (!String.IsNullOrEmpty(data))
							{
								// Broadcast to all.
								byte[] toSent = System.Text.Encoding.ASCII.GetBytes(data);

								try
								{
									// Broadcast to subnet.
									IPAddress broadcast = GetBroadcastAddress(address, mask);
									IPEndPoint endPointSub = new IPEndPoint(broadcast, _broadcastListenerPort);
									int sent = await client.SendAsync(toSent, toSent.Length, endPointSub);
								}
								catch { }
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Send data to the host.
		/// </summary>
		/// <param name="data">The data to send.</param>
		/// <param name="host">The host name or address.</param>
		private async void Callback(string data, string host)
		{
			// Open the client.
			using (UdpClient client = new UdpClient())
			{
				// If data exists.
				if (!String.IsNullOrEmpty(data))
				{
					// Broadcast to all.
					byte[] toSent = System.Text.Encoding.ASCII.GetBytes(data);

					try
					{
						// Has IP address or host name.
						if (!String.IsNullOrEmpty(host))
						{
							var hostEntry = System.Net.Dns.GetHostEntry(host);

							// Loop through the AddressList to obtain the supported 
							// AddressFamily. This is to avoid an exception that 
							// occurs when the host IP Address is not compatible 
							// with the address family 
							// (typical in the IPv6 case).
							foreach (IPAddress address in hostEntry.AddressList)
							{
								try
								{
									// Send to this endpoint.
									IPEndPoint endPointSub = new IPEndPoint(address, _callbackListenerPort);
									int sent = await client.SendAsync(toSent, toSent.Length, endPointSub);

									// Send complete break;
									break;
								}
								catch (Exception)
								{
									// If error then wrong IP address
									// Try the next IP address.
								}
							}
						}
					}
					catch { }
				}
			}
		}

		/// <summary>
		/// Broadcast listener.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		private void BroadcastListener(CancellationToken token)
		{
			bool exitIndicator = false;

			// Create the tasks.
			Task[] tasks = new Task[1];

			// Poller task.
			Task poller = Task.Factory.StartNew(async () =>
			{
				// Create a new spin wait.
				SpinWait sw = new SpinWait();

				// Action to perform.
				while (!exitIndicator)
				{
					try
					{
						// Get the result.
						UdpReceiveResult result = await _broadcastListener.ReceiveAsync();
						if (result.Buffer != null)
						{
							// Get the data.
							string data = System.Text.Encoding.ASCII.GetString(result.Buffer);

							// Get the JSON data.
							JsonGenerator gen = new JsonGenerator(data);
							gen.Namespace = "Lake.WebSocketServer";
							gen.RootClassName = "RootWebSocketData";
							var extract = gen.Extract();

							// Search for the contact.
							bool foundContact = false;
							var contact = gen.GetContactData(out foundContact);

							// Search for the client.
							bool foundClient = false;
							var client = gen.GetClientData(out foundClient);

							// If contact data has been found.
							if (foundContact && foundClient && !String.IsNullOrEmpty(contact.ServerID) && contact.ServerID != Global.ServerID)
							{
								bool found = false;

								// Find the client on this WebSocket.
								// Get a copy of all current clients.
								var copyAllClients = Global.Clients.ToArray();
								for (int i = 0; i < copyAllClients.Length; i++)
								{
									var u = copyAllClients[i];
									if (contact.UniqueID == u.Key.UniqueID && contact.ApplicationID == u.Key.ApplicationID)
									{
										found = true;

										// If available
										if (u.Key.Available)
										{
											// Send the message to the contact.
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes(data);
											await u.Value.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);

											// The response to the caller.
											string jsonResponse =
											"{" +
												"\"response\":" + "\"ok\"," +
												"\"error\":" + "\"\"," +
												"\"serverID\":" + "\"" + Global.ServerID + "\"," +
												"\"available\":" + "true," +
												"\"uniqueID\":" + "\"" + client.UniqueID + "\"," +
												"\"applicationID\":" + "\"" + client.ApplicationID + "\"," +
												"\"contactUniqueID\":" + "\"" + contact.UniqueID + "\"," +
												"\"contactApplicationID\":" + "\"" + contact.ApplicationID + "\"" +
											"}";

											// Sent callback to the call.
											Callback(jsonResponse, result.RemoteEndPoint.Address.ToString());
										}
										else
										{
											// The response to the caller.
											string jsonResponse =
											"{" +
												"\"response\":" + "\"ok\"," +
												"\"error\":" + "\"\"," +
												"\"serverID\":" + "\"" + Global.ServerID + "\"," +
												"\"available\":" + "false," +
												"\"uniqueID\":" + "\"" + client.UniqueID + "\"," +
												"\"applicationID\":" + "\"" + client.ApplicationID + "\"," +
												"\"contactUniqueID\":" + "\"" + contact.UniqueID + "\"," +
												"\"contactApplicationID\":" + "\"" + contact.ApplicationID + "\"" +
											"}";

											// Sent callback to the call.
											Callback(jsonResponse, result.RemoteEndPoint.Address.ToString());
										}
										break;
									}
								}

								// If the contact has not been found.
								if (!found)
								{
									// The response to the caller.
									string jsonResponse =
									"{" +
										"\"response\":" + "\"error\"," +
										"\"error\":" + "\"No contacts, searching\"," +
										"\"serverID\":" + "\"" + Global.ServerID + "\"," +
										"\"available\":" + "false," +
										"\"uniqueID\":" + "\"" + client.UniqueID + "\"," +
										"\"applicationID\":" + "\"" + client.ApplicationID + "\"," +
										"\"contactUniqueID\":" + "\"" + contact.UniqueID + "\"," +
										"\"contactApplicationID\":" + "\"" + contact.ApplicationID + "\"" +
									"}";

									// Sent callback to the call.
									Callback(jsonResponse, result.RemoteEndPoint.Address.ToString());
								}
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

			try
			{
				// Clean-up the resource.
				if (_broadcastListener != null)
					_broadcastListener.Dispose();
			}
			catch { }

			tasks = null;
			_broadcastListener = null;
		}

		/// <summary>
		/// Callback listener.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		private void CallbackListener(CancellationToken token)
		{
			bool exitIndicator = false;

			// Create the tasks.
			Task[] tasks = new Task[1];

			// Poller task.
			Task poller = Task.Factory.StartNew(async () =>
			{
				// Create a new spin wait.
				SpinWait sw = new SpinWait();

				// Action to perform.
				while (!exitIndicator)
				{
					try
					{
						// Get the result.
						UdpReceiveResult result = await _callbackListener.ReceiveAsync();
						if (result.Buffer != null)
						{
							// Get the data.
							string data = System.Text.Encoding.ASCII.GetString(result.Buffer);

							// Get the JSON data.
							JsonGenerator gen = new JsonGenerator(data);
							gen.Namespace = "Lake.WebSocketServer";
							gen.RootClassName = "RootWebSocketData";
							var extract = gen.Extract();

							// Search for the client.
							bool foundClient = false;
							var client = gen.GetClientData(out foundClient);

							// If client data has been found.
							if (foundClient)
							{
								// Find the client on this WebSocket.
								// Get a copy of all current clients.
								var copyAllClients = Global.Clients.ToArray();
								for (int i = 0; i < copyAllClients.Length; i++)
								{
									var u = copyAllClients[i];
									if (client.UniqueID == u.Key.UniqueID && client.ApplicationID == u.Key.ApplicationID)
									{
										// Send the message to the client.
										byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes(data);
										await u.Value.SendAsync(new ArraySegment<byte>(sendBuffer),
											WebSocketMessageType.Text, true, CancellationToken.None);
										break;
									}
								}
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

			try
			{
				// Clean-up the resource.
				if (_callbackListener != null)
					_callbackListener.Dispose();
			}
			catch { }

			tasks = null;
			_callbackListener = null;
		}

		/// <summary>
		/// Get the broadcast IP address from the mask.
		/// </summary>
		/// <param name="address">The IP address to mask.</param>
		/// <param name="subnetMask">The IP address of the subnet to mask with.</param>
		/// <returns>The new broadcast IP address.</returns>
		private IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
		{
			byte[] ipAdressBytes = address.GetAddressBytes();
			byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

			if (ipAdressBytes.Length != subnetMaskBytes.Length)
				throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

			byte[] broadcastAddress = new byte[ipAdressBytes.Length];
			for (int i = 0; i < broadcastAddress.Length; i++)
			{
				broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
			}
			return new IPAddress(broadcastAddress);
		}

		/// <summary>
		/// Get the network IP address from the mask.
		/// </summary>
		/// <param name="address">The IP address to mask.</param>
		/// <param name="subnetMask">The IP address of the subnet to mask with.</param>
		/// <returns>The new network IP address.</returns>
		private IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
		{
			byte[] ipAdressBytes = address.GetAddressBytes();
			byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

			if (ipAdressBytes.Length != subnetMaskBytes.Length)
				throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

			byte[] broadcastAddress = new byte[ipAdressBytes.Length];
			for (int i = 0; i < broadcastAddress.Length; i++)
			{
				broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
			}
			return new IPAddress(broadcastAddress);
		}

		/// <summary>
		/// Are IP addresses in the same subnet.
		/// </summary>
		/// <param name="address2">IP address to compare.</param>
		/// <param name="address">IP address to compare.</param>
		/// <param name="subnetMask">The IP address submask.</param>
		/// <returns>True if in the same subnet; else false.</returns>
		private bool IsInSameSubnet(IPAddress address2, IPAddress address, IPAddress subnetMask)
		{
			IPAddress network1 = GetNetworkAddress(address, subnetMask);
			IPAddress network2 = GetNetworkAddress(address2, subnetMask);
			
			return network1.Equals(network2);
		}
	}
}