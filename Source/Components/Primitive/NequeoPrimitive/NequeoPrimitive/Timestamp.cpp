/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Timestamp.cpp
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

#include "stdafx.h"

#include "Timestamp.h"

Nequeo::Primitive::Timestamp::Timestamp()
{
	update();
}


Nequeo::Primitive::Timestamp::Timestamp(TimeVal tv)
{
	_ts = tv;
}


Nequeo::Primitive::Timestamp::Timestamp(const Nequeo::Primitive::Timestamp& other)
{
	_ts = other._ts;
}


Nequeo::Primitive::Timestamp::~Timestamp()
{
}


Nequeo::Primitive::Timestamp& Nequeo::Primitive::Timestamp::operator = (const Nequeo::Primitive::Timestamp& other)
{
	_ts = other._ts;
	return *this;
}


Nequeo::Primitive::Timestamp& Nequeo::Primitive::Timestamp::operator = (TimeVal tv)
{
	_ts = tv;
	return *this;
}


void Nequeo::Primitive::Timestamp::swap(Nequeo::Primitive::Timestamp& timestamp)
{
	std::swap(_ts, timestamp._ts);
}


Nequeo::Primitive::Timestamp Nequeo::Primitive::Timestamp::fromEpochTime(std::time_t t)
{
	return Nequeo::Primitive::Timestamp(TimeVal(t)*resolution());
}


Nequeo::Primitive::Timestamp Nequeo::Primitive::Timestamp::fromUtcTime(UtcTimeVal val)
{
	val -= (TimeDiff(0x01b21dd2) << 32) + 0x13814000;
	val /= 10;
	return Timestamp(val);
}


void Nequeo::Primitive::Timestamp::update()
{
	FILETIME ft;

	GetSystemTimeAsFileTime(&ft);

	ULARGE_INTEGER epoch; // UNIX epoch (1970-01-01 00:00:00) expressed in Windows NT FILETIME
	epoch.LowPart = 0xD53E8000;
	epoch.HighPart = 0x019DB1DE;

	ULARGE_INTEGER ts;
	ts.LowPart = ft.dwLowDateTime;
	ts.HighPart = ft.dwHighDateTime;
	ts.QuadPart -= epoch.QuadPart;
	_ts = ts.QuadPart / 10;
}


Nequeo::Primitive::Timestamp Nequeo::Primitive::Timestamp::fromFileTimeNP(UInt32 fileTimeLow, UInt32 fileTimeHigh)
{
	ULARGE_INTEGER epoch; // UNIX epoch (1970-01-01 00:00:00) expressed in Windows NT FILETIME
	epoch.LowPart = 0xD53E8000;
	epoch.HighPart = 0x019DB1DE;

	ULARGE_INTEGER ts;
	ts.LowPart = fileTimeLow;
	ts.HighPart = fileTimeHigh;
	ts.QuadPart -= epoch.QuadPart;

	return Timestamp(ts.QuadPart / 10);
}


void Nequeo::Primitive::Timestamp::toFileTimeNP(UInt32& fileTimeLow, UInt32& fileTimeHigh) const
{
	ULARGE_INTEGER epoch; // UNIX epoch (1970-01-01 00:00:00) expressed in Windows NT FILETIME
	epoch.LowPart = 0xD53E8000;
	epoch.HighPart = 0x019DB1DE;

	ULARGE_INTEGER ts;
	ts.QuadPart = _ts * 10;
	ts.QuadPart += epoch.QuadPart;
	fileTimeLow = ts.LowPart;
	fileTimeHigh = ts.HighPart;
}

//
// inlines
//
inline bool Nequeo::Primitive::Timestamp::operator == (const Nequeo::Primitive::Timestamp& ts) const
{
	return _ts == ts._ts;
}


inline bool Nequeo::Primitive::Timestamp::operator != (const Nequeo::Primitive::Timestamp& ts) const
{
	return _ts != ts._ts;
}


inline bool Nequeo::Primitive::Timestamp::operator >  (const Nequeo::Primitive::Timestamp& ts) const
{
	return _ts > ts._ts;
}


inline bool Nequeo::Primitive::Timestamp::operator >= (const Nequeo::Primitive::Timestamp& ts) const
{
	return _ts >= ts._ts;
}


inline bool Nequeo::Primitive::Timestamp::operator <  (const Nequeo::Primitive::Timestamp& ts) const
{
	return _ts < ts._ts;
}


inline bool Nequeo::Primitive::Timestamp::operator <= (const Nequeo::Primitive::Timestamp& ts) const
{
	return _ts <= ts._ts;
}


inline Nequeo::Primitive::Timestamp Nequeo::Primitive::Timestamp::operator + (Nequeo::Primitive::Timestamp::TimeDiff d) const
{
	return Timestamp(_ts + d);
}


inline Nequeo::Primitive::Timestamp Nequeo::Primitive::Timestamp::operator - (Nequeo::Primitive::Timestamp::TimeDiff d) const
{
	return Timestamp(_ts - d);
}


inline Nequeo::Primitive::Timestamp::TimeDiff Nequeo::Primitive::Timestamp::operator - (const Nequeo::Primitive::Timestamp& ts) const
{
	return _ts - ts._ts;
}


inline Nequeo::Primitive::Timestamp& Nequeo::Primitive::Timestamp::operator += (Nequeo::Primitive::Timestamp::TimeDiff d)
{
	_ts += d;
	return *this;
}


inline Nequeo::Primitive::Timestamp& Nequeo::Primitive::Timestamp::operator -= (Nequeo::Primitive::Timestamp::TimeDiff d)
{
	_ts -= d;
	return *this;
}


inline std::time_t Nequeo::Primitive::Timestamp::epochTime() const
{
	return std::time_t(_ts / resolution());
}


inline Nequeo::Primitive::Timestamp::UtcTimeVal Nequeo::Primitive::Timestamp::utcTime() const
{
	return _ts * 10 + (TimeDiff(0x01b21dd2) << 32) + 0x13814000;
}


inline Nequeo::Primitive::Timestamp::TimeVal Nequeo::Primitive::Timestamp::epochMicroseconds() const
{
	return _ts;
}


inline Nequeo::Primitive::Timestamp::TimeDiff Nequeo::Primitive::Timestamp::elapsed() const
{
	Nequeo::Primitive::Timestamp now;
	return now - *this;
}


inline bool Nequeo::Primitive::Timestamp::isElapsed(Nequeo::Primitive::Timestamp::TimeDiff interval) const
{
	Timestamp now;
	Timestamp::TimeDiff diff = now - *this;
	return diff >= interval;
}


inline Nequeo::Primitive::Timestamp::TimeVal Nequeo::Primitive::Timestamp::resolution()
{
	return 1000000;
}


inline void swap(Nequeo::Primitive::Timestamp& s1, Nequeo::Primitive::Timestamp& s2)
{
	s1.swap(s2);
}