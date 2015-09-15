/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timespan.cpp
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

#include "stdafx.h"

#include "Timespan.h"

namespace Nequeo {
	namespace Primitive
	{
		const Timespan::TimeDiff Timespan::MILLISECONDS = 1000;
		const Timespan::TimeDiff Timespan::SECONDS = 1000 * Timespan::MILLISECONDS;
		const Timespan::TimeDiff Timespan::MINUTES = 60 * Timespan::SECONDS;
		const Timespan::TimeDiff Timespan::HOURS = 60 * Timespan::MINUTES;
		const Timespan::TimeDiff Timespan::DAYS = 24 * Timespan::HOURS;
	}
}

Nequeo::Primitive::Timespan::Timespan() : _span(0)
{
}


Nequeo::Primitive::Timespan::Timespan(TimeDiff microseconds) : _span(microseconds)
{
}


Nequeo::Primitive::Timespan::Timespan(long seconds, long microseconds) : _span(TimeDiff(seconds)*SECONDS + microseconds)
{
}


Nequeo::Primitive::Timespan::Timespan(int days, int hours, int minutes, int seconds, int microseconds) :
	_span(TimeDiff(microseconds) + TimeDiff(seconds)*SECONDS + TimeDiff(minutes)*MINUTES + TimeDiff(hours)*HOURS + TimeDiff(days)*DAYS)
{
}


Nequeo::Primitive::Timespan::Timespan(const Nequeo::Primitive::Timespan& timespan) : _span(timespan._span)
{
}


Nequeo::Primitive::Timespan::~Timespan()
{
}


Nequeo::Primitive::Timespan& Nequeo::Primitive::Timespan::operator = (const Nequeo::Primitive::Timespan& timespan)
{
	_span = timespan._span;
	return *this;
}


Nequeo::Primitive::Timespan& Nequeo::Primitive::Timespan::operator = (TimeDiff microseconds)
{
	_span = microseconds;
	return *this;
}


Nequeo::Primitive::Timespan& Nequeo::Primitive::Timespan::assign(int days, int hours, int minutes, int seconds, int microseconds)
{
	_span = TimeDiff(microseconds) + TimeDiff(seconds)*SECONDS + TimeDiff(minutes)*MINUTES + TimeDiff(hours)*HOURS + TimeDiff(days)*DAYS;
	return *this;
}


Nequeo::Primitive::Timespan& Nequeo::Primitive::Timespan::assign(long seconds, long microseconds)
{
	_span = TimeDiff(seconds)*SECONDS + TimeDiff(microseconds);
	return *this;
}


void Nequeo::Primitive::Timespan::swap(Nequeo::Primitive::Timespan& timespan)
{
	std::swap(_span, timespan._span);
}


Nequeo::Primitive::Timespan Nequeo::Primitive::Timespan::operator + (const Nequeo::Primitive::Timespan& d) const
{
	return Timespan(_span + d._span);
}


Nequeo::Primitive::Timespan Nequeo::Primitive::Timespan::operator - (const Nequeo::Primitive::Timespan& d) const
{
	return Timespan(_span - d._span);
}


Nequeo::Primitive::Timespan& Nequeo::Primitive::Timespan::operator += (const Nequeo::Primitive::Timespan& d)
{
	_span += d._span;
	return *this;
}


Nequeo::Primitive::Timespan& Nequeo::Primitive::Timespan::operator -= (const Nequeo::Primitive::Timespan& d)
{
	_span -= d._span;
	return *this;
}


Nequeo::Primitive::Timespan Nequeo::Primitive::Timespan::operator + (TimeDiff microseconds) const
{
	return Timespan(_span + microseconds);
}


Nequeo::Primitive::Timespan Nequeo::Primitive::Timespan::operator - (TimeDiff microseconds) const
{
	return Timespan(_span - microseconds);
}


Nequeo::Primitive::Timespan&Nequeo::Primitive::Timespan::operator += (TimeDiff microseconds)
{
	_span += microseconds;
	return *this;
}


Nequeo::Primitive::Timespan& Nequeo::Primitive::Timespan::operator -= (TimeDiff microseconds)
{
	_span -= microseconds;
	return *this;
}

//
// inlines
//
inline int Nequeo::Primitive::Timespan::days() const
{
	return int(_span / DAYS);
}


inline int Nequeo::Primitive::Timespan::hours() const
{
	return int((_span / HOURS) % 24);
}


inline int Nequeo::Primitive::Timespan::totalHours() const
{
	return int(_span / HOURS);
}


inline int Nequeo::Primitive::Timespan::minutes() const
{
	return int((_span / MINUTES) % 60);
}


inline int Nequeo::Primitive::Timespan::totalMinutes() const
{
	return int(_span / MINUTES);
}


inline int Nequeo::Primitive::Timespan::seconds() const
{
	return int((_span / SECONDS) % 60);
}


inline int Nequeo::Primitive::Timespan::totalSeconds() const
{
	return int(_span / SECONDS);
}


inline int Nequeo::Primitive::Timespan::milliseconds() const
{
	return int((_span / MILLISECONDS) % 1000);
}


inline Nequeo::Primitive::Timespan::TimeDiff Nequeo::Primitive::Timespan::totalMilliseconds() const
{
	return _span / MILLISECONDS;
}


inline int Nequeo::Primitive::Timespan::microseconds() const
{
	return int(_span % 1000);
}


inline int Nequeo::Primitive::Timespan::useconds() const
{
	return int(_span % 1000000);
}


inline Nequeo::Primitive::Timespan::TimeDiff Nequeo::Primitive::Timespan::totalMicroseconds() const
{
	return _span;
}


inline bool Nequeo::Primitive::Timespan::operator == (const Nequeo::Primitive::Timespan& ts) const
{
	return _span == ts._span;
}


inline bool Nequeo::Primitive::Timespan::operator != (const Nequeo::Primitive::Timespan& ts) const
{
	return _span != ts._span;
}


inline bool Nequeo::Primitive::Timespan::operator >  (const Nequeo::Primitive::Timespan& ts) const
{
	return _span > ts._span;
}


inline bool Nequeo::Primitive::Timespan::operator >= (const Nequeo::Primitive::Timespan& ts) const
{
	return _span >= ts._span;
}


inline bool Nequeo::Primitive::Timespan::operator <  (const Nequeo::Primitive::Timespan& ts) const
{
	return _span < ts._span;
}


inline bool Nequeo::Primitive::Timespan::operator <= (const Nequeo::Primitive::Timespan& ts) const
{
	return _span <= ts._span;
}


inline bool Nequeo::Primitive::Timespan::operator == (TimeDiff microseconds) const
{
	return _span == microseconds;
}


inline bool Nequeo::Primitive::Timespan::operator != (TimeDiff microseconds) const
{
	return _span != microseconds;
}


inline bool Nequeo::Primitive::Timespan::operator >  (TimeDiff microseconds) const
{
	return _span > microseconds;
}


inline bool Nequeo::Primitive::Timespan::operator >= (TimeDiff microseconds) const
{
	return _span >= microseconds;
}


inline bool Nequeo::Primitive::Timespan::operator <  (TimeDiff microseconds) const
{
	return _span < microseconds;
}


inline bool Nequeo::Primitive::Timespan::operator <= (TimeDiff microseconds) const
{
	return _span <= microseconds;
}


inline void swap(Nequeo::Primitive::Timespan& s1, Nequeo::Primitive::Timespan& s2)
{
	s1.swap(s2);
}