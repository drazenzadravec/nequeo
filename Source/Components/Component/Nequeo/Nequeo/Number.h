/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Number.h
*  Purpose :       Number class.
*
*/

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

#pragma once

#include "Global.h"

namespace Nequeo
{
	/// <summary>
	/// Represents a number type.
	/// </summary>
	enum class NumberType
	{
		//
		// Summary:
		//     An integral type representing signed 16-bit integers with values between -32768
		//     and 32767.
		Int16 = 0,
		//
		// Summary:
		//     An integral type representing unsigned 16-bit integers with values between 0
		//     and 65535.
		UInt16 = 1,
		//
		// Summary:
		//     An integral type representing signed 32-bit integers with values between -2147483648
		//     and 2147483647.
		Int32 = 2,
		//
		// Summary:
		//     An integral type representing unsigned 32-bit integers with values between 0
		//     and 4294967295.
		UInt32 = 3,
		//
		// Summary:
		//     An integral type representing signed 64-bit integers with values between -9223372036854775808
		//     and 9223372036854775807.
		Int64 = 4,
		//
		// Summary:
		//     An integral type representing unsigned 64-bit integers with values between 0
		//     and 18446744073709551615.
		UInt64 = 5,
		//
		// Summary:
		//     A floating point type representing values ranging from approximately 1.5 x 10
		//     -45 to 3.4 x 10 38 with a precision of 7 digits.
		Single = 6,
		//
		// Summary:
		//     A floating point type representing values ranging from approximately 5.0 x 10
		//     -324 to 1.7 x 10 308 with a precision of 15-16 digits.
		Double = 7,
	};

	/// <summary>
	/// Represents a number value.
	/// </summary>
	class Number
	{
	public:
		explicit Number(short number) : _number_short(number), _numberType(Nequeo::NumberType::Int16) {}
		explicit Number(unsigned short number) : _number_ushort(number), _numberType(Nequeo::NumberType::UInt16) {}

		explicit Number(int number) : _number_int(number), _numberType(Nequeo::NumberType::Int32) {}
		explicit Number(unsigned int number) : _number_uint(number), _numberType(Nequeo::NumberType::UInt32) {}

		explicit Number(long number) : _number_long(number), _numberType(Nequeo::NumberType::Int64) {}
		explicit Number(unsigned long number) : _number_ulong(number), _numberType(Nequeo::NumberType::UInt64) {}

		explicit Number(double number) : _number_double(number), _numberType(Nequeo::NumberType::Double) {}
		explicit Number(float number) : _number_float(number), _numberType(Nequeo::NumberType::Single) {}

		explicit operator short() const 
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return _number_short;
			case Nequeo::NumberType::UInt16:
				return (unsigned short)_number_short;
			case Nequeo::NumberType::Int32:
				return (int)_number_short;
			case Nequeo::NumberType::UInt32:
				return (unsigned int)_number_short;
			case Nequeo::NumberType::Int64:
				return (long)_number_short;
			case Nequeo::NumberType::UInt64:
				return (unsigned long)_number_short;
			case Nequeo::NumberType::Single:
				return (float)_number_short;
			case Nequeo::NumberType::Double:
				return (double)_number_short;
			default:
				return _number_short;
			}
		}

		explicit operator unsigned short() const 
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return (short)_number_ushort;
			case Nequeo::NumberType::UInt16:
				return _number_ushort;
			case Nequeo::NumberType::Int32:
				return (int)_number_ushort;
			case Nequeo::NumberType::UInt32:
				return (unsigned int)_number_ushort;
			case Nequeo::NumberType::Int64:
				return (long)_number_ushort;
			case Nequeo::NumberType::UInt64:
				return (unsigned long)_number_ushort;
			case Nequeo::NumberType::Single:
				return (float)_number_ushort;
			case Nequeo::NumberType::Double:
				return (double)_number_ushort;
			default:
				return _number_ushort;
			}
		}

		explicit operator int() const 
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return (short)_number_int;
			case Nequeo::NumberType::UInt16:
				return (unsigned short)_number_int;
			case Nequeo::NumberType::Int32:
				return _number_int;
			case Nequeo::NumberType::UInt32:
				return (unsigned int)_number_int;
			case Nequeo::NumberType::Int64:
				return (long)_number_int;
			case Nequeo::NumberType::UInt64:
				return (unsigned long)_number_int;
			case Nequeo::NumberType::Single:
				return (float)_number_int;
			case Nequeo::NumberType::Double:
				return (double)_number_int;
			default:
				return _number_int;
			}
		}

		explicit operator unsigned int() const
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return (short)_number_uint;
			case Nequeo::NumberType::UInt16:
				return (unsigned short)_number_uint;
			case Nequeo::NumberType::Int32:
				return (int)_number_uint;
			case Nequeo::NumberType::UInt32:
				return _number_uint;
			case Nequeo::NumberType::Int64:
				return (long)_number_uint;
			case Nequeo::NumberType::UInt64:
				return (unsigned long)_number_uint;
			case Nequeo::NumberType::Single:
				return (float)_number_uint;
			case Nequeo::NumberType::Double:
				return (double)_number_uint;
			default:
				return _number_uint;
			}
		}

		explicit operator long() const
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return (short)_number_long;
			case Nequeo::NumberType::UInt16:
				return (unsigned short)_number_long;
			case Nequeo::NumberType::Int32:
				return (int)_number_long;
			case Nequeo::NumberType::UInt32:
				return (unsigned int)_number_long;
			case Nequeo::NumberType::Int64:
				return _number_long;
			case Nequeo::NumberType::UInt64:
				return (unsigned long)_number_long;
			case Nequeo::NumberType::Single:
				return (float)_number_long;
			case Nequeo::NumberType::Double:
				return (double)_number_long;
			default:
				return _number_long;
			}
		}

		explicit operator unsigned long() const
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return (short)_number_ulong;
			case Nequeo::NumberType::UInt16:
				return (unsigned short)_number_ulong;
			case Nequeo::NumberType::Int32:
				return (int)_number_ulong;
			case Nequeo::NumberType::UInt32:
				return (unsigned int)_number_ulong;
			case Nequeo::NumberType::Int64:
				return (long)_number_ulong;
			case Nequeo::NumberType::UInt64:
				return _number_ulong;
			case Nequeo::NumberType::Single:
				return (float)_number_ulong;
			case Nequeo::NumberType::Double:
				return (double)_number_ulong;
			default:
				return _number_ulong;
			}
		}

		explicit operator double() const
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return (short)_number_short;
			case Nequeo::NumberType::UInt16:
				return (unsigned short)_number_short;
			case Nequeo::NumberType::Int32:
				return (int)_number_short;
			case Nequeo::NumberType::UInt32:
				return (unsigned int)_number_short;
			case Nequeo::NumberType::Int64:
				return (long)_number_short;
			case Nequeo::NumberType::UInt64:
				return (unsigned long)_number_short;
			case Nequeo::NumberType::Single:
				return (float)_number_short;
			case Nequeo::NumberType::Double:
				return (double)_number_short;
			default:
				return _number_short;
			}
		}

		explicit operator float() const
		{
			switch (_numberType)
			{
			case Nequeo::NumberType::Int16:
				return (short)_number_double;
			case Nequeo::NumberType::UInt16:
				return (unsigned short)_number_double;
			case Nequeo::NumberType::Int32:
				return (int)_number_double;
			case Nequeo::NumberType::UInt32:
				return (unsigned int)_number_double;
			case Nequeo::NumberType::Int64:
				return (long)_number_double;
			case Nequeo::NumberType::UInt64:
				return (unsigned long)_number_double;
			case Nequeo::NumberType::Single:
				return _number_double;
			case Nequeo::NumberType::Double:
				return _number_double;
			default:
				return _number_double;
			}
		}

		/// <summary>
		/// Get the number type stored.
		/// </summary>
		inline const NumberType TypeCode() const { return _numberType; }

	private:
		NumberType _numberType;

		short _number_short;
		int _number_int;
		long _number_long;
		double _number_double;
		float _number_float;
		unsigned short _number_ushort;
		unsigned int _number_uint;
		unsigned long _number_ulong;
	};
}