using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using WebRTCSocket;

namespace WebRTCSocket
{
	/// <summary>
	/// WebRTC web socket.
	/// </summary>
	public class Handler : WebRTCSocket.WebSocketHandler
	{
		/// <summary>
		/// Called when a new web socket connection has been established.
		/// </summary>
		/// <param name="webSocketContext">The web socket context.</param>
		public async override Task WebSocketContext(AspNetWebSocketContext webSocketContext)
		{
			byte[] store = null;
			byte[] receiveBuffer = null;
			WebSocket webSocket = null;
			WebSocketClient wsClient = null;
			TimeoutModel tmConnection = null;
			TimeoutModel tmExpiry = null;

			try
			{
				// Get the web socket ref.
				webSocket = webSocketContext.WebSocket;

				// Random GUID
				string clientGUID = Guid.NewGuid().ToString();
				wsClient = new WebSocketClient()
				{
					UniqueID = clientGUID,
					ApplicationID = clientGUID,
					Available = false,
					Broadcast = false,
					BroadcastAppID = false,
					HasAccess = false,
					AccessToken = null
				};

				// Set the buffer.
				store = new byte[0];
				CancellationTokenSource receiveCancelToken = new CancellationTokenSource();

				// Connection timeout.
				tmConnection = new TimeoutModel()
				{
					Client = wsClient,
					Socket = webSocket,
					Start = DateTime.Now,
					Timeout = new TimeSpan(0, 0, (int)Global.ClientTimeoutConnect),
					ReceiveCancelToken = receiveCancelToken
				};

				// Add the client to the collection.
				bool itemAdded = Global.Clients.TryAdd(wsClient, webSocket);

				// If a connect timeout exists.
				if ((int)Global.ClientTimeoutConnect > 0)
				{
					// Add timeout connect handler.
					Global.Timeout.Register(tmConnection, (c, w) => ConnectTimeoutActionHandler(c, w));
				}

				// While the WebSocket connection remains open run a 
				// simple loop that receives data and sends it back.
				while (webSocket.State == WebSocketState.Open)
				{
					receiveBuffer = null;
					receiveBuffer = new byte[16384];

					// Receive the next set of data.
					ArraySegment<byte> arrayBuffer = new ArraySegment<byte>(receiveBuffer);
					WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(arrayBuffer, receiveCancelToken.Token);

					// If the connection has been closed.
					if (receiveResult.MessageType == WebSocketMessageType.Close)
					{
						// Exit the loop
						break;
					}
					else
					{
						// If a message exists.
						if (receiveResult.Count > 0)
						{
							// If text.
							if (receiveResult.MessageType == WebSocketMessageType.Text)
							{
								// If this is the end of the message.
								if (receiveResult.EndOfMessage)
								{
									// Get the number of bytes sent.
									byte[] received = receiveBuffer.Take(receiveResult.Count).ToArray();

									// Get the complete message.
									byte[] completeMessage = null;
									completeMessage = store.CombineParallel(received);
									string message = System.Text.Encoding.ASCII.GetString(completeMessage);

									// If requesting application IDs.
									if (message == "applicationids")
									{
										// If no access.
										if (!wsClient.HasAccess)
										{
											// Send a message back.
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"Access Denied\"}");
											await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);
											continue;
										}

										// If requesting application IDs.
										string listText = "";

										// For each client.
										try
										{
											// Get a copy of all clients.
											var copyAllClients = Global.Clients.ToArray();
											for (int i = 0; i < copyAllClients.Length; i++)
											{
												var u = copyAllClients[i];
												// If can braodcast.
												if (u.Key.BroadcastAppID)
												{
													// Add each client.
													listText += "{\"application\":\"" + u.Key.ApplicationID + "\"},";
												}
											}

											// Trim end.
											string listTextTrimed = listText.TrimEnd(new char[] { ',' });

											// Send to client
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"ok\",\"applications\":[" + listTextTrimed + "]}");
											await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);
										}
										catch { }
									}
									else if (message == "uniqueids")
									{
										// If no access.
										if (!wsClient.HasAccess)
										{
											// Send a message back.
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"Access Denied\"}");
											await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);
											continue;
										}

										// If requesting unique IDs.
										string listText = "";

										// For each client.
										try
										{
											// Get a copy of all clients.
											var copyAllClients = Global.Clients.ToArray();
											for (int i = 0; i < copyAllClients.Length; i++)
											{
												var u = copyAllClients[i];
												// If can braodcast.
												if (u.Key.Broadcast)
												{
													// Add each client.
													listText += "{\"unique\":\"" + u.Key.UniqueID + "\"},";
												}
											}

											// Trim end.
											string listTextTrimed = listText.TrimEnd(new char[] { ',' });

											// Send to client
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"ok\",\"uniques\":[" + listTextTrimed + "]}");
											await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);
										}
										catch { }

									}
									else if (message == "uniqueapplication")
									{
										// If no access.
										if (!wsClient.HasAccess)
										{
											// Send a message back.
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"Access Denied\"}");
											await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);
											continue;
										}

										// If requesting unique and application group pair.
										string listText = "";

										// For each client.
										try
										{
											// Get a copy of all clients.
											var copyAllClients = Global.Clients.ToArray();
											for (int i = 0; i < copyAllClients.Length; i++)
											{
												var u = copyAllClients[i];
												// If can braodcast.
												if (u.Key.BroadcastAppID && u.Key.Broadcast)
												{
													// Add each client.
													listText += "{\"unique\":\"" + u.Key.UniqueID + "\",\"application\":\"" + u.Key.ApplicationID + "\"},";
												}
											}

											// Trim end.
											string listTextTrimed = listText.TrimEnd(new char[] { ',' });

											// Send to client
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"ok\",\"groups\":[" + listTextTrimed + "]}");
											await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);
										}
										catch { }
									}
									else
									{
										try
										{
											// Get the JSON data.
											JsonGenerator gen = new JsonGenerator(message);
											gen.Namespace = "Lake.WebSocketServer";
											gen.RootClassName = "RootWebSocketData";
											var extract = gen.Extract();

											// Search for the contact.
											bool foundContact = false;
											var contact = gen.GetContactData(out foundContact);

											// Search for the client.
											bool foundClient = false;
											var client = gen.GetClientData(out foundClient);

											// Then change the client details.
											if (foundClient)
											{
												// Change.
												wsClient.UniqueID = client.UniqueID;
												wsClient.ApplicationID = client.ApplicationID;
												wsClient.Available = client.Available;
												wsClient.Broadcast = client.Broadcast;
												wsClient.BroadcastAppID = client.BroadcastAppID;
												wsClient.AccessToken = client.AccessToken;

												// If nothing has been sent.
												if (String.IsNullOrEmpty(client.UniqueID) || String.IsNullOrEmpty(client.ApplicationID))
												{
													// Send to client
													byte[] sendBufferNoData = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"ok\",\"settings\":false}");
													await webSocket.SendAsync(new ArraySegment<byte>(sendBufferNoData),
														WebSocketMessageType.Text, true, CancellationToken.None);
													continue;
												}

												// Send to client
												byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"ok\",\"settings\":true}");
												await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
													WebSocketMessageType.Text, true, CancellationToken.None);

												// If no access.
												if (!wsClient.HasAccess)
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

															// Verify the access token for the client.
															HttpResponseMessage response = await httpClient.GetAsync(new Uri(Global.AccessTokenVerifyURL));
															if (response.IsSuccessStatusCode)
															{
																// Get the json response.
																string jsonData = await response.Content.ReadAsStringAsync();

																// Get the JSON data.
																JsonGenerator genAccess = new JsonGenerator(jsonData);
																genAccess.Namespace = "Lake.WebSocketServer";
																genAccess.RootClassName = "RootWebSocketData";
																var extractAccess = genAccess.Extract();

																// Search for the access token.
																bool foundAccessToken = false;
																var accessToken = genAccess.GetAccessTokenData(out foundAccessToken);

																// If the access token has been found.
																if (foundAccessToken)
																{
																	// Has access.
																	bool hasAccess = accessToken.Grant;
																	if (hasAccess)
																	{
																		// Should the uniqueid be matched with what is in the returned token
																		// and what was entered by the user.
																		if (accessToken.UniqueIDMatch)
																		{
																			// If not the same.
																			if (accessToken.UniqueID != wsClient.UniqueID)
																			{
																				// No access is allowed.
																				hasAccess = false;
																			}
																		}
																	}

																	// Grant or not.
																	wsClient.HasAccess = hasAccess;
																	if (hasAccess)
																	{
																		try
																		{
																			// Expiry timeout.
																			tmExpiry = new TimeoutModel()
																			{
																				Client = wsClient,
																				Socket = webSocket,
																				Start = DateTime.Now,
																				Timeout = new TimeSpan(0, (int)accessToken.Expiry, 0),
																				ReceiveCancelToken = receiveCancelToken
																			};

																			// If an expiry exists.
																			if (accessToken.Expiry > 0)
																			{
																				// Add timeout connect handler.
																				Global.Timeout.Register(tmExpiry, (c, w) => ExpiryTimeoutActionHandler(c, w));
																			}

																			// Remove connection timeout.
																			// The client has access.
																			Global.Timeout.Unregister(tmConnection);
																		}
																		catch { }
																	}
																}
															}
														}
													}
													catch (Exception)
													{
														// Send a message back.
														byte[] sendBufferNoAccess = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"Unable to grant access\"}");
														await webSocket.SendAsync(new ArraySegment<byte>(sendBufferNoAccess),
															WebSocketMessageType.Text, true, CancellationToken.None);

														try
														{
															// Send close the connection.
															await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unable to grant access", CancellationToken.None);
														}
														catch { }
													}
												}
											}
											else
											{
												// If no access.
												if (!wsClient.HasAccess)
												{
													// Send a message back.
													byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"Access Denied\"}");
													await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
														WebSocketMessageType.Text, true, CancellationToken.None);
													continue;
												}

												// If contact details have been sent.
												if (foundContact)
												{
													try
													{
														// Contact;
														bool contactMatchFound = false;
														bool contactAvailable = false;
														KeyValuePair<WebRTCSocket.WebSocketClient, WebSocket>
															contactMatch = new KeyValuePair<WebSocketClient, WebSocket>();

														try
														{
															// Get a copy of all clients.
															var copyAllClients = Global.Clients.ToArray();
															for (int i = 0; i < copyAllClients.Length; i++)
															{
																var u = copyAllClients[i];
																if ((u.Key.UniqueID == contact.UniqueID) &&
																	(u.Key.ApplicationID == contact.ApplicationID))
																{
																	// Found, contact is also found is available.
																	contactMatchFound = true;
																	contactAvailable = u.Key.Available;
																	contactMatch = u;
																	break;
																}
															}
														}
														catch (Exception)
														{
															// No match.
															contactMatchFound = false;
														}

														// If the contact has been found and is available.
														if (contactMatchFound && contactAvailable)
														{
															// Send to client
															byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"ok\",\"available\":true}");
															await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
																WebSocketMessageType.Text, true, CancellationToken.None);

															// Replace the contact properties.
															gen.Replace(new JsonNameValue[]
															{
																new WebRTCSocket.JsonNameValue() { Name = "contactUniqueID", Value = wsClient.UniqueID },
																new WebRTCSocket.JsonNameValue() { Name = "contactApplicationID", Value = wsClient.ApplicationID }
															});

															// Add the response.
															gen.Create(new JsonNameValue[]
															{
																new WebRTCSocket.JsonNameValue() { Name = "response", Value = "ok" },
																new WebRTCSocket.JsonNameValue() { Name = "available", Value = true }
															});

															extract = gen.Extract();
															var jsonDataNew = gen.GetJson();
															byte[] sendBufferContact = System.Text.Encoding.ASCII.GetBytes(jsonDataNew);

															// Send the client data to the contact.
															await contactMatch.Value.SendAsync(new ArraySegment<byte>(sendBufferContact),
																WebSocketMessageType.Text, true, CancellationToken.None);
														}
														else
														{
															// If the contact has been found and is not available.
															if (contactMatchFound && !contactAvailable)
															{
																// Send to client
																byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"ok\",\"available\":false}");
																await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
																	WebSocketMessageType.Text, true, CancellationToken.None);
															}
															else
															{
																// Indicate to the client that we are searching.
																byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"No contacts, searching\"}");
																await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
																	WebSocketMessageType.Text, true, CancellationToken.None);

																// If broadcast enabled.
																if (Global.UdpBroadcastEnabled)
																{
																	// The contact has not been found.
																	// Broadcast to servers asking where
																	// this contact is located.

																	// Add the response.
																	gen.Create(new JsonNameValue[]
																	{
																		new WebRTCSocket.JsonNameValue() { Name = "response", Value = "ok" },
																		new WebRTCSocket.JsonNameValue() { Name = "available", Value = false },
																		new WebRTCSocket.JsonNameValue() { Name = "error", Value = "" },
																		new WebRTCSocket.JsonNameValue() { Name = "serverID", Value = Global.ServerID },
																		new WebRTCSocket.JsonNameValue() { Name = "uniqueID", Value = wsClient.UniqueID },
																		new WebRTCSocket.JsonNameValue() { Name = "applicationID", Value = wsClient.ApplicationID }
																	});

																	// Get the new JSON data.
																	extract = gen.Extract();
																	var jsonDataNew = gen.GetJson();

																	// Broadcast.
																	Global.UdpSocket.Broadcast(jsonDataNew, Global.UdpBroadcastAddress, Global.UdpBroadcastMask, wsClient, contact);
																}
															}
														}
													}
													catch
													{
														// Send to client
														byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"No contacts, searching\"}");
														await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
															WebSocketMessageType.Text, true, CancellationToken.None);
													}
												}
											}
										}
										catch
										{
											// Send to client
											byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes("{\"response\":\"error\",\"error\":\"Unable to read request\"}");
											await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer),
												WebSocketMessageType.Text, true, CancellationToken.None);
										}
									}

									store = null;
									received = null;
									store = new byte[0];
									completeMessage = null;
								}
								else
								{
									// Get the number of bytes sent.
									byte[] received = receiveBuffer.Take(receiveResult.Count).ToArray();

									// Store the data until the
									// end of the message.
									byte[] temp = null;
									temp = store.CombineParallel(received);

									// Store the data until the end.
									store = temp;
									temp = null;
									received = null;
								}
							}
							else
							{
								// Exit the loop
								break;
							}
						}
					}
				}

				// Cancel the receive request.
				if (webSocket.State != WebSocketState.Open)
				{
					// Cancel the receive request.
					receiveCancelToken.Cancel();
				}
			}
			catch (Exception) { }
			finally
			{
				// Clean-up connection timeout.
				if (tmConnection != null)
				{
					try
					{
						// Remove connection timeout.
						Global.Timeout.Unregister(tmConnection);
					}
					catch { }
				}

				// Clean-up expiry timeout.
				if (tmExpiry != null)
				{
					try
					{
						// Remove connection timeout.
						Global.Timeout.Unregister(tmExpiry);
					}
					catch { }
				}

				// Clean up by disposing the WebSocket client.
				if (wsClient != null)
				{
					try
					{
						// Remove the client from the list.
						Global.Clients.TryRemove(wsClient, out webSocket);
					}
					catch { }
				}

				// Clean up by disposing the WebSocket.
				if (webSocket != null)
				{
					try
					{
						// Abort the operation.
						webSocket.Abort();
					}
					catch { }
				}

				store = null;
				receiveBuffer = null;
				webSocket = null;
				wsClient = null;
				tmConnection = null;
				tmExpiry = null;
			}
		}

		/// <summary>
		/// Connect timeout action handler.
		/// </summary>
		/// <param name="timeout">The current timeout.</param>
		/// <param name="webSocket">The client websocket connection.</param>
		private async void ConnectTimeoutActionHandler(TimeoutModel timeout, WebSocket webSocket)
		{
			try
			{
				// Cancel the receive request.
				timeout.ReceiveCancelToken.Cancel();
				if (webSocket.State == WebSocketState.Open)
				{
					// Send a close connection to the client.
					await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection Timeout", CancellationToken.None);
				}
			}
			catch { }
		}

		/// <summary>
		/// Expiry timeout action handler.
		/// </summary>
		/// <param name="timeout">The current timeout.</param>
		/// <param name="webSocket">The client websocket connection.</param>
		private async void ExpiryTimeoutActionHandler(TimeoutModel timeout, WebSocket webSocket)
		{
			try
			{
				// Cancel the receive request.
				timeout.ReceiveCancelToken.Cancel();
				if (webSocket.State == WebSocketState.Open)
				{
					// Send a close connection to the client.
					await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Expiry Timeout", CancellationToken.None);
				}
			}
			catch { }
		}
	}
}