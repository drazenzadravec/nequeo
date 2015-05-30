/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timestamp.h
*  Purpose :       Timestamp class.
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

#ifndef _TIMESTAMP_H
#define _TIMESTAMP_H

#include "Global.h"

#include "Types.h"
#include <crtdefs.h>

namespace Nequeo
{
	/// A Timestamp stores a monotonic* time value
	/// with (theoretical) microseconds resolution.
	/// Timestamps can be compared with each other
	/// and simple arithmetics are supported.
	///
	/// [*] Note that Timestamp values are only monotonic as
	/// long as the systems's clock is monotonic as well
	/// (and not, e.g. set back).
	///
	/// Timestamps are UTC (Coordinated Universal Time)
	/// based and thus independent of the timezone
	/// in effect on the system.
	class Timestamp
	{
	public:
		typedef Int64 TimeVal;    /// monotonic UTC time value in microsecond resolution
		typedef Int64 UtcTimeVal; /// monotonic UTC time value in 100 nanosecond resolution
		typedef Int64 TimeDiff;   /// difference between two timestamps in microseconds

		Timestamp();
		/// Creates a timestamp with the current time.

		Timestamp(TimeVal tv);
		/// Creates a timestamp from the given time value.

		Timestamp(const Timestamp& other);
		/// Copy constructor.

		~Timestamp();
		/// Destroys the timestamp

		Timestamp& operator = (const Timestamp& other);
		Timestamp& operator = (TimeVal tv);

		void swap(Timestamp& timestamp);
		/// Swaps the Timestamp with another one.

		void update();
		/// Updates the Timestamp with the current time.

		bool operator == (const Timestamp& ts) const;
		bool operator != (const Timestamp& ts) const;
		bool operator >  (const Timestamp& ts) const;
		bool operator >= (const Timestamp& ts) const;
		bool operator <  (const Timestamp& ts) const;
		bool operator <= (const Timestamp& ts) const;

		Timestamp  operator +  (TimeDiff d) const;
		Timestamp  operator -  (TimeDiff d) const;
		TimeDiff   operator -  (const Timestamp& ts) const;
		Timestamp& operator += (TimeDiff d);
		Timestamp& operator -= (TimeDiff d);

		std::time_t epochTime() const;
		/// Returns the timestamp expressed in time_t.
		/// time_t base time is midnight, January 1, 1970.
		/// Resolution is one second.

		UtcTimeVal utcTime() const;
		/// Returns the timestamp expressed in UTC-based
		/// time. UTC base time is midnight, October 15, 1582.
		/// Resolution is 100 nanoseconds.

		TimeVal epochMicroseconds() const;
		/// Returns the timestamp expressed in microseconds
		/// since the Unix epoch, midnight, January 1, 1970.

		TimeDiff elapsed() const;
		/// Returns the time elapsed since the time denoted by
		/// the timestamp. Equivalent to Timestamp() - *this.

		bool isElapsed(TimeDiff interval) const;
		/// Returns true iff the given interval has passed
		/// since the time denoted by the timestamp.

		static Timestamp fromEpochTime(std::time_t t);
		/// Creates a timestamp from a std::time_t.

		static Timestamp fromUtcTime(UtcTimeVal val);
		/// Creates a timestamp from a UTC time value.

		static TimeVal resolution();
		/// Returns the resolution in units per second.
		/// Since the timestamp has microsecond resolution,
		/// the returned value is always 1000000.

#if defined(_WIN32)
		static Timestamp fromFileTimeNP(UInt32 fileTimeLow, UInt32 fileTimeHigh);
		void toFileTimeNP(UInt32& fileTimeLow, UInt32& fileTimeHigh) const;
#endif

	private:
		TimeVal _ts;
	};
}
#endif