using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebRTCSocket
{
	/// <summary>
	/// JSON field info.
	/// </summary>
	public class JsonFieldInfo
	{
		/// <summary>
		/// JSON field info.
		/// </summary>
		/// <param name="generator">The genrator.</param>
		/// <param name="jsonMemberName">The json memeber name.</param>
		/// <param name="type">The json type.</param>
		/// <param name="usePascalCase">Use pacal case.</param>
		/// <param name="jobjects">The json objects.</param>
		public JsonFieldInfo(IJsonConfig generator, string jsonMemberName, JsonType type, bool usePascalCase, IList<object> jobjects)
		{
			this.generator = generator;
			this.JsonMemberName = jsonMemberName;
			this.MemberName = jsonMemberName;
			if (usePascalCase) MemberName = JsonGenerator.ToTitleCase(MemberName);
			this.Type = type;
			this.Jobjects = jobjects;
		}

		private IJsonConfig generator;

		/// <summary>
		/// Gets the member name.
		/// </summary>
		public string MemberName { get; private set; }

		/// <summary>
		/// Gets the json member name.
		/// </summary>
		public string JsonMemberName { get; private set; }

		/// <summary>
		/// Gets the json type.
		/// </summary>
		public JsonType Type { get; private set; }

		/// <summary>
		/// Gets the jobjects.
		/// </summary>
		public IList<object> Jobjects { get; set; }

	}
}