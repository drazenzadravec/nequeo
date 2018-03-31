using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebRTCSocket
{
	/// <summary>
	/// Class config.
	/// </summary>
	public interface IJsonConfig
	{
		/// <summary>
		/// Gets or sets the base namespace.
		/// </summary>
		string Namespace { get; set; }

		/// <summary>
		/// Gets or sets the root class name.
		/// </summary>
		string RootClassName { get; set; }

	}
}
