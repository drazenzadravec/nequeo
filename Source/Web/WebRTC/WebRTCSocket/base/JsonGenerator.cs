using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebRTCSocket
{
	/// <summary>
	/// JSON type builder.
	/// </summary>
	public class JsonGenerator : IJsonConfig
	{
		/// <summary>
		/// JSON type builder.
		/// </summary>
		/// <param name="json">The json data.</param>
		public JsonGenerator(string json)
		{
			try
			{
				// Load the data into the json reader.
				using (var sr = new StringReader(json))
				using (var reader = new JsonTextReader(sr))
				{
					JToken jtoken = JToken.ReadFrom(reader);
					if (jtoken is JArray)
					{
						_jobjects = ((JArray)jtoken).Cast<JObject>().ToArray();
					}
					else if (jtoken is JObject)
					{
						_jobjects = new[] { (JObject)jtoken };
					}
					else
					{
						throw new Exception("Sample JSON must be either a JSON array, or a JSON object.");
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// JSON type builder.
		/// </summary>
		/// <param name="stream">The stream containing the json data.</param>
		public JsonGenerator(StreamReader stream)
		{
			try
			{
				// Load the data into the json reader.
				using (var reader = new JsonTextReader(stream))
				{
					JToken jtoken = JToken.ReadFrom(reader);
					if (jtoken is JArray)
					{
						_jobjects = ((JArray)jtoken).Cast<JObject>().ToArray();
					}
					else if (jtoken is JObject)
					{
						_jobjects = new[] { (JObject)jtoken };
					}
					else
					{
						throw new Exception("Sample JSON must be either a JSON array, or a JSON object.");
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private IList<JsonType> _types = null;
		private JObject[] _jobjects = null;
		private JsonType _rootType = null;
		private HashSet<string> _names = null;

		/// <summary>
		/// Gets or sets the base namespace.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Gets or sets the root class name.
		/// </summary>
		public string RootClassName { get; set; }

		/// <summary>
		/// Extract tthe types from the json data.
		/// </summary>
		/// <returns>The array of json types extracted fron the json data.</returns>
		public IList<JsonType> Extract()
		{
			_types = null;
			_names = null;

			// Create and assign the root class.
			_types = new List<JsonType>();
			_names = new HashSet<string>();

			_names.Add(RootClassName);
			_rootType = new JsonType(this, _jobjects[0]);
			_rootType.IsRoot = true;
			_rootType.AssignName(RootClassName);

			// Generate the class from the json.
			GenerateClass(_jobjects, _rootType);
			return _types;
		}

		/// <summary>
		/// Get the JSON data objects from the JSON data.
		/// </summary>
		/// <returns></returns>
		public JObject[] GetJsonObjects()
		{
			return _jobjects;
		}

		/// <summary>
		/// Gets the JSON data.
		/// </summary>
		/// <returns>The JSON data.</returns>
		public string GetJson()
		{
			var jobjects = GetJsonObjects();
			var jsonData = jobjects[0].Root.ToString().Replace("\r\n", "");
			return jsonData;
		}

		/// <summary>
		/// Get the contact data.
		/// </summary>
		/// <param name="found">True if found; else false.</param>
		/// <returns>The contact data.</returns>
		public WebSocketContact GetContactData(out bool found)
		{
			found = false;
			WebSocketContact contact = new WebSocketContact();

			try
			{
				// Get the list.
				foreach (JsonType item in _types)
				{
					// For each field.
					foreach (JsonFieldInfo field in item.Fields)
					{
						// If found contact unique id.
						if (field.JsonMemberName == "contactUniqueID")
						{
							found = true;
							contact.UniqueID = field.Jobjects[0].ToString();
						}

						// If found contact application id.
						if (field.JsonMemberName == "contactApplicationID")
						{
							found = true;
							contact.ApplicationID = field.Jobjects[0].ToString();
						}

						// If found server id.
						if (field.JsonMemberName == "serverID")
						{
							found = true;
							contact.ServerID = field.Jobjects[0].ToString();
						}
					}
				}
			}
			catch { }

			// Return the contact.
			return contact;
		}

		/// <summary>
		/// Get the client data.
		/// </summary>
		/// <param name="found">True if found; else false.</param>
		/// <returns>The client data.</returns>
		public WebSocketClient GetClientData(out bool found)
		{
			found = false;
			WebSocketClient client = new WebSocketClient();
			
			try
			{
				// Get the list.
				foreach (JsonType item in _types)
				{
					// For each field.
					foreach (JsonFieldInfo field in item.Fields)
					{
						// If found contact unique id.
						if (field.JsonMemberName == "uniqueID")
						{
							found = true;
							client.UniqueID = field.Jobjects[0].ToString();
						}

						// If found contact application id.
						if (field.JsonMemberName == "applicationID")
						{
							found = true;
							client.ApplicationID = field.Jobjects[0].ToString();
						}

						// If found contact available.
						if (field.JsonMemberName == "available")
						{
							found = true;
							bool available = false;
							if (Boolean.TryParse(field.Jobjects[0].ToString(), out available))
								client.Available = available;
						}

						// If found contact broadcast.
						if (field.JsonMemberName == "broadcast")
						{
							found = true;
							bool broadcast = false;
							if (Boolean.TryParse(field.Jobjects[0].ToString(), out broadcast))
								client.Broadcast = broadcast;
						}

						// If found contact broadcastAppID.
						if (field.JsonMemberName == "broadcastAppID")
						{
							found = true;
							bool broadcastAppID = false;
							if (Boolean.TryParse(field.Jobjects[0].ToString(), out broadcastAppID))
								client.BroadcastAppID = broadcastAppID;
						}

						// If found access token.
						if (field.JsonMemberName == "accessToken")
						{
							found = true;
							client.AccessToken = field.Jobjects[0].ToString();
						}
					}
				}
			}
			catch { }

			// Return the client.
			return client;
		}

		/// <summary>
		/// Get the access token data.
		/// </summary>
		/// <param name="found">True if found; else false.</param>
		/// <returns>The access token data.</returns>
		public WebSocketAccessToken GetAccessTokenData(out bool found)
		{
			found = false;
			WebSocketAccessToken accessToken = new WebSocketAccessToken();

			try
			{
				// Get the list.
				foreach (JsonType item in _types)
				{
					// For each field.
					foreach (JsonFieldInfo field in item.Fields)
					{
						// If found unique id.
						if (field.JsonMemberName == "uniqueid")
						{
							found = true;
							accessToken.UniqueID = field.Jobjects[0].ToString();
						}

						// If found grant.
						if (field.JsonMemberName == "grant")
						{
							found = true;
							bool grant = false;
							if (Boolean.TryParse(field.Jobjects[0].ToString(), out grant))
								accessToken.Grant = grant;
						}

						// If found expiry.
						if (field.JsonMemberName == "expiry")
						{
							found = true;
							int expiry = 0;
							if (Int32.TryParse(field.Jobjects[0].ToString(), out expiry))
								accessToken.Expiry = expiry;
						}

						// If found token id.
						if (field.JsonMemberName == "tokenid")
						{
							found = true;
							accessToken.TokenID = field.Jobjects[0].ToString();
						}

						// If found uniqueidmatch.
						if (field.JsonMemberName == "uniqueidmatch")
						{
							found = true;
							bool uniqueidmatch = false;
							if (Boolean.TryParse(field.Jobjects[0].ToString(), out uniqueidmatch))
								accessToken.UniqueIDMatch = uniqueidmatch;
						}
					}
				}
			}
			catch { }

			// Return the access token details.
			return accessToken;
		}

		/// <summary>
		/// Get the client location.
		/// </summary>
		/// <param name="found">True if found; else false.</param>
		/// <returns>The client location data.</returns>
		public WebSocketClientLocation GetClientLocation(out bool found)
		{
			found = false;
			WebSocketClientLocation location = new WebSocketClientLocation();

			try
			{
				// Get the list.
				foreach (JsonType item in _types)
				{
					// For each field.
					foreach (JsonFieldInfo field in item.Fields)
					{
						// If found client unique id.
						if (field.JsonMemberName == "uniqueid")
						{
							found = true;
							location.UniqueID = field.Jobjects[0].ToString();
						}

						// If found client IP address.
						if (field.JsonMemberName == "address")
						{
							found = true;
							location.Address = field.Jobjects[0].ToString();
						}

						// If found client subnet mask.
						if (field.JsonMemberName == "mask")
						{
							found = true;
							location.Mask = field.Jobjects[0].ToString();
						}
					}
				}
			}
			catch { }

			// Return the contact.
			return location;
		}

		/// <summary>
		/// Create new JSON data with the original and name value list.
		/// </summary>
		/// <param name="json">The original JSON data.</param>
		/// <param name="nameValues">The name values to add to the JSON data.</param>
		/// <returns>The new JSON data.</returns>
		public string Create(string json, JsonNameValue[] nameValues)
		{
			// Using string builder.
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			// Create the writer.
			using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
			{
				writer.WriteStartObject();
				writer.WriteRaw(json.Trim(new char[] { '{', '}' }) + ",");

				// For each name value pair.
				foreach(JsonNameValue item in nameValues)
				{
					writer.WritePropertyName(item.Name);
					writer.WriteValue(item.Value);
				}
				
				// Write the end object.
				writer.WriteEndObject();
			}

			// Return the JSON data.
			return sb.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nameValues"></param>
		public void Create(JsonNameValue[] nameValues)
		{
			// For each name value pair.
			foreach (JsonNameValue item in nameValues)
			{
				// Add a new JSON property.
				_jobjects[0].Add(item.Name, JToken.FromObject(item.Value));
			}
		}

		/// <summary>
		/// Replace the json item with the name value.
		/// </summary>
		/// <param name="nameValues">The colelction of name values to replace.</param>
		public void Replace(JsonNameValue[] nameValues)
		{
			// For each name value pair.
			foreach (JsonNameValue item in nameValues)
			{
				JProperty property = _jobjects[0].Property(item.Name);
				property.Value = JToken.FromObject(item.Value);
			}
		}

		/// <summary>
		/// Start entracting the json type information.
		/// </summary>
		/// <param name="jobjects">The json objects.</param>
		/// <param name="type">The json type.</param>
		private void GenerateClass(JObject[] jobjects, JsonType type)
		{
			var jsonFields = new Dictionary<string, JsonType>();
			var fieldExamples = new Dictionary<string, IList<object>>();

			var first = true;

			foreach (var obj in jobjects)
			{
				foreach (var prop in obj.Properties())
				{
					JsonType fieldType;
					var currentType = new JsonType(this, prop.Value);
					var propName = prop.Name;

					// Try get property.
					if (jsonFields.TryGetValue(propName, out fieldType))
					{

						var commonType = fieldType.GetCommonType(currentType);

						jsonFields[propName] = commonType;
					}
					else
					{
						var commonType = currentType;
						if (first) commonType = commonType.MaybeMakeNullable(this);
						else commonType = commonType.GetCommonType(JsonType.GetNull(this));
						jsonFields.Add(propName, commonType);
						fieldExamples[propName] = new List<object>();
					}
					var fe = fieldExamples[propName];
					var val = prop.Value;
					if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
					{
						if (!fe.Contains(null))
						{
							fe.Insert(0, null);
						}
					}
					else
					{
						var v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
						if (!fe.Any(x => v.Equals(x)))
							fe.Add(v);
					}
				}
				first = false;
			}

			//if (UseNestedClasses)
			//{
			//    foreach (var field in jsonFields)
			//    {
			//        _names.Add(field.Key.ToLower());
			//    }
			//}

			foreach (var field in jsonFields)
			{
				var fieldType = field.Value;
				if (fieldType.Type == JsonTypeEnum.Object)
				{
					var subexamples = new List<JObject>(jobjects.Length);
					foreach (var obj in jobjects)
					{
						JToken value;
						if (obj.TryGetValue(field.Key, out value))
						{
							if (value.Type == JTokenType.Object)
							{
								subexamples.Add((JObject)value);
							}
						}
					}

					fieldType.AssignName(CreateUniqueClassName(field.Key));
					GenerateClass(subexamples.ToArray(), fieldType);
				}

				if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
				{
					var subexamples = new List<JObject>(jobjects.Length);
					foreach (var obj in jobjects)
					{
						JToken value;
						if (obj.TryGetValue(field.Key, out value))
						{
							if (value.Type == JTokenType.Array)
							{
								foreach (var item in (JArray)value)
								{
									if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
									subexamples.Add((JObject)item);
								}

							}
							else if (value.Type == JTokenType.Object)
							{
								foreach (var item in (JObject)value)
								{
									if (!(item.Value is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");

									subexamples.Add((JObject)item.Value);
								}
							}
						}
					}

					field.Value.InternalType.AssignName(CreateUniqueClassNameFromPlural(field.Key));
					GenerateClass(subexamples.ToArray(), field.Value.InternalType);
				}

				if (fieldType.InternalType == null)
				{
					if (fieldType.Type == JsonTypeEnum.Object)
					{
						// Assign an internal type.
						fieldType.SetInternalType(this, fieldType.AssignedName);
					}
				}
			}

			// Select the field info
			type.Fields = jsonFields.Select(x => new JsonFieldInfo(this, x.Key, x.Value, true, fieldExamples[x.Key])).ToArray();

			// Add the type.
			_types.Add(type);

		}

		/// <summary>
		/// Create a unique class name.
		/// </summary>
		/// <param name="name">The current name.</param>
		/// <returns>The unique name.</returns>
		private string CreateUniqueClassName(string name)
		{
			name = ToTitleCase(name);

			var finalName = name;
			var i = 2;
			while (_names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
			{
				finalName = name + i.ToString();
				i++;
			}

			_names.Add(finalName);
			return finalName;
		}

		/// <summary>
		/// Create a unique member name.
		/// </summary>
		/// <param name="memberNames">The current member list.</param>
		/// <param name="name">The current name.</param>
		/// <returns>The unique name.</returns>
		private string CreateUniqueMemberName(HashSet<string> memberNames, string name)
		{
			name = ToTitleCase(name);

			var finalName = name;
			var i = 2;
			while (memberNames.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
			{
				finalName = name + i.ToString();
				i++;
			}

			memberNames.Add(finalName);
			return finalName;
		}

		/// <summary>
		/// Pluralise the word.
		/// </summary>
		/// <param name="plural">The string tp pluralise.</param>
		/// <returns>The pluralised string.</returns>
		private string CreateUniqueClassNameFromPlural(string plural)
		{
			plural = ToTitleCase(plural);
			return CreateUniqueClassName(Plural(plural));
		}

		/// <summary>
		/// To title case.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The new title case.</returns>
		internal static string ToTitleCase(string str)
		{
			var sb = new StringBuilder(str.Length);
			var flag = true;

			for (int i = 0; i < str.Length; i++)
			{
				var c = str[i];
				if (char.IsLetterOrDigit(c))
				{
					sb.Append(flag ? char.ToUpper(c) : c);
					flag = false;
				}
				else
				{
					flag = true;
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Converts the word from singular to plural.
		/// </summary>
		/// <param name="name">The word to convert.</param>
		/// <returns>The converted word as plural.</returns>
		private string Plural(string name)
		{
			if (name.EndsWith("x", StringComparison.InvariantCultureIgnoreCase)
				 || name.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)
				 || name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
			{
				return name + "es";
			}
			else if (name.EndsWith("y", StringComparison.InvariantCultureIgnoreCase))
			{
				return name.Substring(0, name.Length - 1) + "ies";
			}
			else if (!name.EndsWith("s"))
			{
				return name + "s";
			}
			return name;
		}
	}
}