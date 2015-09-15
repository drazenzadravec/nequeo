/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
	Permission is hereby granted, free of charge, to any person
	obtaining a copy of this software and associated documentation
	files (the "Software"), to deal in the Software without
	restriction, including without limitation the rights to use,
	copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the
	Software is furnished to do so, subject to the following
	conditions:

	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
	OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
	NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
	WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
	OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo
{
	/// <summary>
	/// Property changed
	/// </summary>
	public interface IPropertyChanged
	{
		/// <summary>
		/// Has the property changed.
		/// </summary>
		bool PropertyChanged { get; set; }

		/// <summary>
		/// Get the details of the interface.
		/// </summary>
		/// <returns>The implementation details.</returns>
		List<string> GetDetails();
	}

	/// <summary>
	/// Common data type descripter.
	/// </summary>
	public class DataType
	{
		#region System Data Type Descripter

		/// <summary>
		/// Gets the system data type.
		/// </summary>
		/// <param name="systemType">The system string data type.</param>
		/// <returns>>The system data type.</returns>
		public static Type GetSystemType(string systemType)
		{
			switch (systemType.ToLower())
			{
				case "system.boolean":
				case "boolean":
				case "bool":
					return typeof(System.Boolean);
				case "system.byte":
				case "byte":
					return typeof(System.Byte);
				case "system.char":
				case "char":
					return typeof(System.Char);
				case "system.datetime":
				case "datetime":
					return typeof(System.DateTime);
				case "system.dbnull":
				case "dbnull":
					return typeof(System.DBNull);
				case "system.decimal":
				case "decimal":
					return typeof(System.Decimal);
				case "system.double":
				case "double":
					return typeof(System.Double);
				case "system.int16":
				case "int16":
				case "short":
					return typeof(System.Int16);
				case "system.int32":
				case "int32":
				case "int":
					return typeof(System.Int32);
				case "system.int64":
				case "int64":
				case "long":
					return typeof(System.Int64);
				case "system.object":
				case "object":
					return typeof(System.Object);
				case "system.sbyte":
				case "sbyte":
					return typeof(System.SByte);
				case "system.single":
				case "single":
				case "float":
					return typeof(System.Single);
				case "system.string":
				case "string":
					return typeof(System.String);
				case "system.uint16":
				case "uint16":
					return typeof(System.UInt16);
				case "system.uint32":
				case "uint32":
				case "uint":
					return typeof(System.UInt32);
				case "system.uint64":
				case "uint64":
				case "ulong":
					return typeof(System.UInt64);
				case "system.guid":
				case "guid":
					return typeof(System.Guid);
				case "system.timespan":
				case "timespan":
					return typeof(System.TimeSpan);
				case "system.array":
				case "array":
					return typeof(System.Array);
				case "system.boolean[]":
				case "boolean[]":
				case "bool[]":
					return typeof(System.Boolean[]);
				case "system.byte[]":
				case "byte[]":
					return typeof(System.Byte[]);
				case "system.char[]":
				case "char[]":
					return typeof(System.Char[]);
				case "system.datetime[]":
				case "datetime[]":
					return typeof(System.DateTime[]);
				case "system.dbnull[]":
				case "dbnull[]":
					return typeof(System.DBNull[]);
				case "system.decimal[]":
				case "decimal[]":
					return typeof(System.Decimal[]);
				case "system.double[]":
				case "double[]":
					return typeof(System.Double[]);
				case "system.int16[]":
				case "int16[]":
				case "short[]":
					return typeof(System.Int16[]);
				case "system.int32[]":
				case "int32[]":
				case "int[]":
					return typeof(System.Int32[]);
				case "system.int64[]":
				case "int64[]":
				case "long[]":
					return typeof(System.Int64[]);
				case "system.object[]":
				case "object[]":
					return typeof(System.Object[]);
				case "system.sbyte[]":
				case "sbyte[]":
					return typeof(System.SByte[]);
				case "system.single[]":
				case "single[]":
				case "float[]":
					return typeof(System.Single[]);
				case "system.string[]":
				case "string[]":
					return typeof(System.String[]);
				case "system.uint16[]":
				case "uint16[]":
					return typeof(System.UInt16[]);
				case "system.uint32[]":
				case "uint32[]":
				case "uint[]":
					return typeof(System.UInt32[]);
				case "system.uint64[]":
				case "uint64[]":
				case "ulong[]":
					return typeof(System.UInt64[]);
				case "system.guid[]":
				case "guid[]":
					return typeof(System.Guid[]);
				case "system.timespan[]":
				case "timespan[]":
					return typeof(System.TimeSpan[]);
				default:
					return typeof(System.Object);
			}
		}

		/// <summary>
		/// Gets the formatted value if any.
		/// </summary>
		/// <param name="type">The system data type.</param>
		/// <param name="value">The value of the type.</param>
		/// <param name="format">The new format for the value.</param>
		/// <param name="dateTimeFormatControl">The date time format to convert from.</param>
		/// <returns>The formatted value string.</returns>
		public static string GetFormattedValue(Type type, object value, string format = "", string dateTimeFormatControl = "")
		{
			if (value == null)
				return string.Empty;

			if (String.IsNullOrEmpty(value.ToString()))
				return string.Empty;

			try
			{
				switch (type.Name.ToLower())
				{
					case "system.boolean":
					case "boolean":
					case "bool":
						Boolean itemBoolean = (Boolean)Convert.ToBoolean(value);
						return itemBoolean.ToString().ToLower();

					case "system.byte":
					case "byte":
						Byte itemByte = (Byte)Convert.ToByte(value);
						return !string.IsNullOrEmpty(format) ? itemByte.ToString(format) : itemByte.ToString();

					case "system.char":
					case "char":
						Char itemChar = (Char)Convert.ToChar(value);
						return itemChar.ToString();

					case "system.datetime":
					case "datetime":
						int yearDate = 0;
						DateTime itemDateTime = DateTime.Now;

						if (dateTimeFormatControl.ToLower() == "ddmmyy")
						{
							int year = Int32.Parse(value.ToString().Substring(4, 2));
							if (year > 80)
								yearDate = 1900 + year;
							else
								yearDate = 2000 + year;

							itemDateTime =
								new DateTime(
									yearDate,
									Int32.Parse(value.ToString().Substring(2, 2)),
									Int32.Parse(value.ToString().Substring(0, 2)));
						}
						else if (dateTimeFormatControl.ToLower() == "ddmmyyyy")
						{
							itemDateTime =
								new DateTime(
									Int32.Parse(value.ToString().Substring(4, 4)),
									Int32.Parse(value.ToString().Substring(2, 2)),
									Int32.Parse(value.ToString().Substring(0, 2)));
						}
						else if (dateTimeFormatControl.ToLower() == "yyyymmdd")
						{
							itemDateTime =
								new DateTime(
									Int32.Parse(value.ToString().Substring(0, 4)),
									Int32.Parse(value.ToString().Substring(4, 2)),
									Int32.Parse(value.ToString().Substring(6, 2)));
						}
						else if (dateTimeFormatControl.ToLower() == "yymmdd")
						{
							int year = Int32.Parse(value.ToString().Substring(0, 2));
							if (year > 80)
								yearDate = 1900 + year;
							else
								yearDate = 2000 + year;

							itemDateTime =
								new DateTime(
									yearDate,
									Int32.Parse(value.ToString().Substring(2, 2)),
									Int32.Parse(value.ToString().Substring(4, 2)));
						}
						else if ((dateTimeFormatControl.ToLower() == "dd/mm/yy") ||
							(dateTimeFormatControl.ToLower() == "dd-mm-yy") ||
							(dateTimeFormatControl.ToLower() == "dd.mm.yy"))
						{
							int year = Int32.Parse(value.ToString().Substring(6, 2));
							if (year > 80)
								yearDate = 1900 + year;
							else
								yearDate = 2000 + year;

							itemDateTime =
								new DateTime(
									yearDate,
									Int32.Parse(value.ToString().Substring(3, 2)),
									Int32.Parse(value.ToString().Substring(0, 2)));
						}
						else if ((dateTimeFormatControl.ToLower() == "dd/mm/yyyy") ||
							(dateTimeFormatControl.ToLower() == "dd-mm-yyyy") ||
							(dateTimeFormatControl.ToLower() == "dd.mm.yyyy"))
						{
							itemDateTime =
								new DateTime(
									Int32.Parse(value.ToString().Substring(6, 4)),
									Int32.Parse(value.ToString().Substring(3, 2)),
									Int32.Parse(value.ToString().Substring(0, 2)));
						}
						else if ((dateTimeFormatControl.ToLower() == "yyyy/mm/dd") ||
							(dateTimeFormatControl.ToLower() == "yyyy-mm-dd") ||
							(dateTimeFormatControl.ToLower() == "yyyy.mm.dd"))
						{
							itemDateTime =
								new DateTime(
									Int32.Parse(value.ToString().Substring(0, 4)),
									Int32.Parse(value.ToString().Substring(5, 2)),
									Int32.Parse(value.ToString().Substring(8, 2)));
						}
						else if ((dateTimeFormatControl.ToLower() == "yy/mm/dd") ||
							(dateTimeFormatControl.ToLower() == "yy-mm-dd") ||
							(dateTimeFormatControl.ToLower() == "yy.mm.dd"))
						{
							int year = Int32.Parse(value.ToString().Substring(0, 2));
							if (year > 80)
								yearDate = 1900 + year;
							else
								yearDate = 2000 + year;

							itemDateTime =
								new DateTime(
									yearDate,
									Int32.Parse(value.ToString().Substring(3, 2)),
									Int32.Parse(value.ToString().Substring(6, 2)));
						}
						else
							itemDateTime = DateTime.Parse(value.ToString());

						// Return the datetime format
						return !string.IsNullOrEmpty(format) ? itemDateTime.ToString(format) : itemDateTime.ToString();

					case "system.dbnull":
					case "dbnull":
						String dbnullString = System.DBNull.Value.ToString();
						return "null";

					case "system.timespan":
					case "timespan":
						TimeSpan itemTimeSpan = (TimeSpan)Convert.ChangeType(value, typeof(TimeSpan));
						return itemTimeSpan.ToString();

					case "system.decimal":
					case "decimal":
						Decimal itemDecimal = (Decimal)Convert.ToDecimal(value);
						return !string.IsNullOrEmpty(format) ? itemDecimal.ToString(format) : itemDecimal.ToString();

					case "system.double":
					case "double":
						Double itemDouble = (Double)Convert.ToDouble(value);
						return !string.IsNullOrEmpty(format) ? itemDouble.ToString(format) : itemDouble.ToString();

					case "system.int16":
					case "int16":
					case "short":
						Int16 itemInt16 = (Int16)Convert.ToInt16(value);
						return !string.IsNullOrEmpty(format) ? itemInt16.ToString(format) : itemInt16.ToString();

					case "system.int32":
					case "int32":
					case "int":
						Int32 itemInt32 = (Int32)Convert.ToInt32(value);
						return !string.IsNullOrEmpty(format) ? itemInt32.ToString(format) : itemInt32.ToString();

					case "system.int64":
					case "int64":
					case "long":
						Int64 itemInt64 = (Int64)Convert.ToInt64(value);
						return !string.IsNullOrEmpty(format) ? itemInt64.ToString(format) : itemInt64.ToString();

					case "system.sbyte":
					case "sbyte":
						SByte itemSByte = (SByte)Convert.ToSByte(value);
						return !string.IsNullOrEmpty(format) ? itemSByte.ToString(format) : itemSByte.ToString();

					case "system.single":
					case "single":
					case "float":
						Single itemSingle = (Single)Convert.ToSingle(value);
						return !string.IsNullOrEmpty(format) ? itemSingle.ToString(format) : itemSingle.ToString();

					case "system.string":
					case "string":
						String itemString = (String)value.ToString();
						return itemString;

					case "system.uint16":
					case "uint16":
						UInt16 itemUInt16 = (UInt16)Convert.ToUInt16(value);
						return !string.IsNullOrEmpty(format) ? itemUInt16.ToString(format) : itemUInt16.ToString();

					case "system.uint32":
					case "uint32":
					case "uint":
						UInt32 itemUInt32 = (UInt32)Convert.ToUInt32(value);
						return !string.IsNullOrEmpty(format) ? itemUInt32.ToString(format) : itemUInt32.ToString();

					case "system.uint64":
					case "uint64":
					case "ulong":
						UInt64 itemUInt64 = (UInt64)Convert.ToUInt64(value);
						return !string.IsNullOrEmpty(format) ? itemUInt64.ToString(format) : itemUInt64.ToString();

					case "system.nullable`1":
					case "nullable`1":
						Type[] genericArguments = type.GetGenericArguments();
						return GetFormattedValue(genericArguments[0], value, format, dateTimeFormatControl);

					default:
						return value.ToString();
				}
			}
			catch { return value.ToString(); }
		}

		/// <summary>
		/// Gets the formatted value if any.
		/// </summary>
		/// <param name="type">The system data type.</param>
		/// <param name="value">The value of the type.</param>
		/// <param name="format">The new format for the value.</param>
		/// <param name="dateTimeFormatControl">The date time format to convert from.</param>
		/// <returns>The formatted value string.</returns>
		/// <exception cref="System.ArgumentException">The format type cna not be found.</exception>
		public static Object GetFormattedValue(string type, object value, string format = "", string dateTimeFormatControl = "")
		{
			if (value == null)
				return value;


			switch (type.ToLower())
			{
				case "system.boolean":
				case "boolean":
				case "bool":
					Boolean itemBoolean = (Boolean)Convert.ToBoolean(value);
					return itemBoolean;

				case "system.byte":
				case "byte":
					Byte itemByte = (Byte)Convert.ToByte(value);
					return itemByte;

				case "system.char":
				case "char":
					Char itemChar = (Char)Convert.ToChar(value);
					return itemChar;

				case "system.datetime":
				case "datetime":
					int yearDate = 0;
					DateTime itemDateTime = DateTime.Now;

					if (dateTimeFormatControl.ToLower() == "ddmmyy")
					{
						int year = Int32.Parse(value.ToString().Substring(4, 2));
						if (year > 80)
							yearDate = 1900 + year;
						else
							yearDate = 2000 + year;

						itemDateTime =
							new DateTime(
								yearDate,
								Int32.Parse(value.ToString().Substring(2, 2)),
								Int32.Parse(value.ToString().Substring(0, 2)));
					}
					else if (dateTimeFormatControl.ToLower() == "ddmmyyyy")
					{
						itemDateTime =
							new DateTime(
								Int32.Parse(value.ToString().Substring(4, 4)),
								Int32.Parse(value.ToString().Substring(2, 2)),
								Int32.Parse(value.ToString().Substring(0, 2)));
					}
					else if (dateTimeFormatControl.ToLower() == "yyyymmdd")
					{
						itemDateTime =
							new DateTime(
								Int32.Parse(value.ToString().Substring(0, 4)),
								Int32.Parse(value.ToString().Substring(4, 2)),
								Int32.Parse(value.ToString().Substring(6, 2)));
					}
					else if (dateTimeFormatControl.ToLower() == "yymmdd")
					{
						int year = Int32.Parse(value.ToString().Substring(0, 2));
						if (year > 80)
							yearDate = 1900 + year;
						else
							yearDate = 2000 + year;

						itemDateTime =
							new DateTime(
								yearDate,
								Int32.Parse(value.ToString().Substring(2, 2)),
								Int32.Parse(value.ToString().Substring(4, 2)));
					}
					else if ((dateTimeFormatControl.ToLower() == "dd/mm/yy") ||
						(dateTimeFormatControl.ToLower() == "dd-mm-yy") ||
						(dateTimeFormatControl.ToLower() == "dd.mm.yy"))
					{
						int year = Int32.Parse(value.ToString().Substring(6, 2));
						if (year > 80)
							yearDate = 1900 + year;
						else
							yearDate = 2000 + year;

						itemDateTime =
							new DateTime(
								yearDate,
								Int32.Parse(value.ToString().Substring(3, 2)),
								Int32.Parse(value.ToString().Substring(0, 2)));
					}
					else if ((dateTimeFormatControl.ToLower() == "dd/mm/yyyy") ||
						(dateTimeFormatControl.ToLower() == "dd-mm-yyyy") ||
						(dateTimeFormatControl.ToLower() == "dd.mm.yyyy"))
					{
						itemDateTime =
							new DateTime(
								Int32.Parse(value.ToString().Substring(6, 4)),
								Int32.Parse(value.ToString().Substring(3, 2)),
								Int32.Parse(value.ToString().Substring(0, 2)));
					}
					else if ((dateTimeFormatControl.ToLower() == "yyyy/mm/dd") ||
						(dateTimeFormatControl.ToLower() == "yyyy-mm-dd") ||
						(dateTimeFormatControl.ToLower() == "yyyy.mm.dd"))
					{
						itemDateTime =
							new DateTime(
								Int32.Parse(value.ToString().Substring(0, 4)),
								Int32.Parse(value.ToString().Substring(5, 2)),
								Int32.Parse(value.ToString().Substring(8, 2)));
					}
					else if ((dateTimeFormatControl.ToLower() == "yy/mm/dd") ||
						(dateTimeFormatControl.ToLower() == "yy-mm-dd") ||
						(dateTimeFormatControl.ToLower() == "yy.mm.dd"))
					{
						int year = Int32.Parse(value.ToString().Substring(0, 2));
						if (year > 80)
							yearDate = 1900 + year;
						else
							yearDate = 2000 + year;

						itemDateTime =
							new DateTime(
								yearDate,
								Int32.Parse(value.ToString().Substring(3, 2)),
								Int32.Parse(value.ToString().Substring(6, 2)));
					}
					else
					{
						try
						{
							itemDateTime = DateTime.Parse(value.ToString());
						}
						catch { }
					}

					// Return the datetime format
					return itemDateTime;

				case "system.timespan":
				case "timespan":
					TimeSpan itemTimeSpan = (TimeSpan)Convert.ChangeType(value, typeof(TimeSpan));
					return itemTimeSpan;

				case "system.decimal":
				case "decimal":
					Decimal itemDecimal = (Decimal)Convert.ToDecimal(value);
					return itemDecimal;

				case "system.double":
				case "double":
					Double itemDouble = (Double)Convert.ToDouble(value);
					return itemDouble;

				case "system.int16":
				case "int16":
				case "short":
					Int16 itemInt16 = (Int16)Convert.ToInt16(value);
					return itemInt16;

				case "system.int32":
				case "int32":
				case "int":
					Int32 itemInt32 = (Int32)Convert.ToInt32(value);
					return itemInt32;

				case "system.int64":
				case "int64":
				case "long":
					Int64 itemInt64 = (Int64)Convert.ToInt64(value);
					return itemInt64;

				case "system.sbyte":
				case "sbyte":
					SByte itemSByte = (SByte)Convert.ToSByte(value);
					return itemSByte;

				case "system.single":
				case "single":
				case "float":
					Single itemSingle = (Single)Convert.ToSingle(value);
					return itemSingle;

				case "system.string":
				case "string":
					String itemString = (String)value.ToString();
					return itemString;

				case "system.uint16":
				case "uint16":
					UInt16 itemUInt16 = (UInt16)Convert.ToUInt16(value);
					return itemUInt16;

				case "system.uint32":
				case "uint32":
				case "uint":
					UInt32 itemUInt32 = (UInt32)Convert.ToUInt32(value);
					return itemUInt32;

				case "system.uint64":
				case "uint64":
				case "ulong":
					UInt64 itemUInt64 = (UInt64)Convert.ToUInt64(value);
					return itemUInt64;

				default:
					throw new System.ArgumentException("System type not supported");
			}
		}

		/// <summary>
		/// Convert from one type to another.
		/// </summary>
		/// <param name="value">The value containing the data to convert.</param>
		/// <param name="type">The type to convert to.</param>
		/// <returns>The new type containing the data.</returns>
		public static object ConvertType(object value, Type type)
		{
			switch (type.Name.ToLower())
			{
				case "system.byte[]":
				case "byte[]":
				case "system.sbyte[]":
				case "sbyte[]":
					return Encoding.Default.GetBytes(value.ToString());

				case "system.guid":
				case "guid":
					return new Guid(value.ToString());

				case "system.nullable`1":
				case "nullable`1":
					Type[] genericArguments = type.GetGenericArguments();
					return ConvertType(value, genericArguments[0]);

				default:
					return Convert.ChangeType(value, type);
			}
		}
		#endregion

	}
}
