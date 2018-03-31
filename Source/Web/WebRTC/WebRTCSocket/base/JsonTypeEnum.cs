using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebRTCSocket
{
	/// <summary>
	/// Json type.
	/// </summary>
	public enum JsonTypeEnum
	{
		/// <summary>
		/// Anything.
		/// </summary>
		Anything,
		/// <summary>
		/// String.
		/// </summary>
		String,
		/// <summary>
		/// Boolean.
		/// </summary>
		Boolean,
		/// <summary>
		/// Integer.
		/// </summary>
		Integer,
		/// <summary>
		/// Long.
		/// </summary>
		Long,
		/// <summary>
		/// Float.
		/// </summary>
		Float,
		/// <summary>
		/// Date.
		/// </summary>
		Date,
		/// <summary>
		/// Nullable integer.
		/// </summary>
		NullableInteger,
		/// <summary>
		/// Nullable long.
		/// </summary>
		NullableLong,
		/// <summary>
		/// Nullable float.
		/// </summary>
		NullableFloat,
		/// <summary>
		/// Nullable boolean.
		/// </summary>
		NullableBoolean,
		/// <summary>
		/// Nullable date.
		/// </summary>
		NullableDate,
		/// <summary>
		/// Object.
		/// </summary>
		Object,
		/// <summary>
		/// Array.
		/// </summary>
		Array,
		/// <summary>
		/// Dictionary.
		/// </summary>
		Dictionary,
		/// <summary>
		/// Nullable something.
		/// </summary>
		NullableSomething,
		/// <summary>
		/// None.
		/// </summary>
		NonConstrained
	}
}