/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timespan.h
*  Purpose :       Timespan class.
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

#ifndef _TIMESPAN_H
#define _TIMESPAN_H

#include "Global.h"

#include "Timestamp.h"

namespace Nequeo
{
	/// A class that represents time spans up to microsecond resolution.
	class Timespan
	{
	public:
		typedef Timestamp::TimeDiff TimeDiff;

		Timespan();
		/// Creates a zero Timespan.

		Timespan(TimeDiff microseconds);
		/// Creates a Timespan.

		Timespan(long seconds, long microseconds);
		/// Creates a Timespan. Useful for creating
		/// a Timespan from a struct timeval.

		Timespan(int days, int hours, int minutes, int seconds, int microseconds);
		/// Creates a Timespan.

		Timespan(const Timespan& timespan);
		/// Creates a Timespan from another one.

		~Timespan();
		/// Destroys the Timespan.

		Timespan& operator = (const Timespan& timespan);
		/// Assignment operator.

		Timespan& operator = (TimeDiff microseconds);
		/// Assignment operator.

		Timespan& assign(int days, int hours, int minutes, int seconds, int microseconds);
		/// Assigns a new span.

		Timespan& assign(long seconds, long microseconds);
		/// Assigns a new span. Useful for assigning
		/// from a struct timeval.

		void swap(Timespan& timespan);
		/// Swaps the Timespan with another one.

		bool operator == (const Timespan& ts) const;
		bool operator != (const Timespan& ts) const;
		bool operator >  (const Timespan& ts) const;
		bool operator >= (const Timespan& ts) const;
		bool operator <  (const Timespan& ts) const;
		bool operator <= (const Timespan& ts) const;

		bool operator == (TimeDiff microseconds) const;
		bool operator != (TimeDiff microseconds) const;
		bool operator >  (TimeDiff microseconds) const;
		bool operator >= (TimeDiff microseconds) const;
		bool operator <  (TimeDiff microseconds) const;
		bool operator <= (TimeDiff microseconds) const;

		Timespan operator + (const Timespan& d) const;
		Timespan operator - (const Timespan& d) const;
		Timespan& operator += (const Timespan& d);
		Timespan& operator -= (const Timespan& d);

		Timespan operator + (TimeDiff microseconds) const;
		Timespan operator - (TimeDiff microseconds) const;
		Timespan& operator += (TimeDiff microseconds);
		Timespan& operator -= (TimeDiff microseconds);

		int days() const;
		/// Returns the number of days.

		int hours() const;
		/// Returns the number of hours (0 to 23).

		int totalHours() const;
		/// Returns the total number of hours.

		int minutes() const;
		/// Returns the number of minutes (0 to 59).

		int totalMinutes() const;
		/// Returns the total number of minutes.

		int seconds() const;
		/// Returns the number of seconds (0 to 59).

		int totalSeconds() const;
		/// Returns the total number of seconds.

		int milliseconds() const;
		/// Returns the number of milliseconds (0 to 999).

		TimeDiff totalMilliseconds() const;
		/// Returns the total number of milliseconds.

		int microseconds() const;
		/// Returns the fractions of a millisecond
		/// in microseconds (0 to 999).

		int useconds() const;
		/// Returns the fractions of a second
		/// in microseconds (0 to 999999).

		TimeDiff totalMicroseconds() const;
		/// Returns the total number of microseconds.

		static const TimeDiff MILLISECONDS; /// The number of microseconds in a millisecond.
		static const TimeDiff SECONDS;      /// The number of microseconds in a second.
		static const TimeDiff MINUTES;      /// The number of microseconds in a minute.
		static const TimeDiff HOURS;        /// The number of microseconds in a hour.
		static const TimeDiff DAYS;         /// The number of microseconds in a day.

	private:
		TimeDiff _span;
	};
}
#endif