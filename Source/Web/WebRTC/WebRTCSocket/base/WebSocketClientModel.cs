using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRTCSocket
{
	/// <summary>
	/// Web socket client model.
	/// </summary>
	public class WebSocketClient
	{
		/// <summary>
		/// Gets or sets the client unique id.
		/// </summary>
		public string UniqueID { get; set; }

		/// <summary>
		/// Gets or sets the client application id.
		/// </summary>
		public string ApplicationID { get; set; }

		/// <summary>
		/// Gets or sets an indicator specifying if 
		/// the client is avaiable for contact.
		/// </summary>
		public bool Available { get; set; }

		/// <summary>
		/// Gets or sets an indicator specifying if 
		/// the clients unique id can be broadcast to others.
		/// </summary>
		public bool Broadcast { get; set; }

		/// <summary>
		/// Gets or sets an indicator specifying if 
		/// the clients application id can be broadcast to others.
		/// </summary>
		public bool BroadcastAppID { get; set; }

		/// <summary>
		/// Gets or sets an indicator specifying if 
		/// the client has access to the server.
		/// </summary>
		public bool HasAccess { get; set; }

		/// <summary>
		/// Gets or sets the client access token.
		/// </summary>
		public string AccessToken { get; set; }
	}

	/// <summary>
	/// Json name value pair.
	/// </summary>
	public class JsonNameValue
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public object Value { get; set; }
	}

	/// <summary>
	/// Web socket contact model.
	/// </summary>
	public class WebSocketContact
	{
		/// <summary>
		/// Gets or sets the client unique id.
		/// </summary>
		public string UniqueID { get; set; }

		/// <summary>
		/// Gets or sets the client application id.
		/// </summary>
		public string ApplicationID { get; set; }

		/// <summary>
		/// Gets or sets the server id.
		/// </summary>
		public string ServerID { get; set; }
	}

	/// <summary>
	/// Web socket access token model.
	/// </summary>
	public class WebSocketAccessToken
	{
		/// <summary>
		/// Gets or sets the client unique id.
		/// </summary>
		public string UniqueID { get; set; }

		/// <summary>
		/// Gets or sets grant indicator.
		/// </summary>
		public bool Grant { get; set; }

		/// <summary>
		/// Gets or sets expiry time (minutes).
		/// </summary>
		public int Expiry { get; set; }

		/// <summary>
		/// Gets or sets the token id.
		/// </summary>
		public string TokenID { get; set; }

		/// <summary>
		/// Gets or sets match token unique id 
		/// with client settings unique id.
		/// </summary>
		public bool UniqueIDMatch { get; set; }
	}

	/// <summary>
	/// Web socket client location model.
	/// </summary>
	public class WebSocketClientLocation
	{
		/// <summary>
		/// Gets or sets the client unique id.
		/// </summary>
		public string UniqueID { get; set; }

		/// <summary>
		/// Gets or sets client IP address.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets client IP subnet mask.
		/// </summary>
		public string Mask { get; set; }

	}
}