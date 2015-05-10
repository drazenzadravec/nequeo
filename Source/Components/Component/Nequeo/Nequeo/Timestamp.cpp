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

Nequeo::Timestamp::Timestamp()
{
	update();
}


Nequeo::Timestamp::Timestamp(TimeVal tv)
{
	_ts = tv;
}


Nequeo::Timestamp::Timestamp(const Nequeo::Timestamp& other)
{
	_ts = other._ts;
}


Nequeo::Timestamp::~Timestamp()
{
}


Nequeo::Timestamp& Nequeo::Timestamp::operator = (const Nequeo::Timestamp& other)
{
	_ts = other._ts;
	return *this;
}


Nequeo::Timestamp& Nequeo::Timestamp::operator = (TimeVal tv)
{
	_ts = tv;
	return *this;
}


void Nequeo::Timestamp::swap(Nequeo::Timestamp& timestamp)
{
	std::swap(_ts, timestamp._ts);
}


Nequeo::Timestamp Nequeo::Timestamp::fromEpochTime(std::time_t t)
{
	return Nequeo::Timestamp(TimeVal(t)*resolution());
}


Nequeo::Timestamp Nequeo::Timestamp::fromUtcTime(UtcTimeVal val)
{
	val -= (TimeDiff(0x01b21dd2) << 32) + 0x13814000;
	val /= 10;
	return Timestamp(val);
}


void Nequeo::Timestamp::update()
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


Nequeo::Timestamp Nequeo::Timestamp::fromFileTimeNP(UInt32 fileTimeLow, UInt32 fileTimeHigh)
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


void Nequeo::Timestamp::toFileTimeNP(UInt32& fileTimeLow, UInt32& fileTimeHigh) const
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
inline bool Nequeo::Timestamp::operator == (const Nequeo::Timestamp& ts) const
{
	return _ts == ts._ts;
}


inline bool Nequeo::Timestamp::operator != (const Nequeo::Timestamp& ts) const
{
	return _ts != ts._ts;
}


inline bool Nequeo::Timestamp::operator >  (const Nequeo::Timestamp& ts) const
{
	return _ts > ts._ts;
}


inline bool Nequeo::Timestamp::operator >= (const Nequeo::Timestamp& ts) const
{
	return _ts >= ts._ts;
}


inline bool Nequeo::Timestamp::operator <  (const Nequeo::Timestamp& ts) const
{
	return _ts < ts._ts;
}


inline bool Nequeo::Timestamp::operator <= (const Nequeo::Timestamp& ts) const
{
	return _ts <= ts._ts;
}


inline Nequeo::Timestamp Nequeo::Timestamp::operator + (Nequeo::Timestamp::TimeDiff d) const
{
	return Timestamp(_ts + d);
}


inline Nequeo::Timestamp Nequeo::Timestamp::operator - (Nequeo::Timestamp::TimeDiff d) const
{
	return Timestamp(_ts - d);
}


inline Nequeo::Timestamp::TimeDiff Nequeo::Timestamp::operator - (const Nequeo::Timestamp& ts) const
{
	return _ts - ts._ts;
}


inline Nequeo::Timestamp& Nequeo::Timestamp::operator += (Nequeo::Timestamp::TimeDiff d)
{
	_ts += d;
	return *this;
}


inline Nequeo::Timestamp& Nequeo::Timestamp::operator -= (Nequeo::Timestamp::TimeDiff d)
{
	_ts -= d;
	return *this;
}


inline std::time_t Nequeo::Timestamp::epochTime() const
{
	return std::time_t(_ts / resolution());
}


inline Nequeo::Timestamp::UtcTimeVal Nequeo::Timestamp::utcTime() const
{
	return _ts * 10 + (TimeDiff(0x01b21dd2) << 32) + 0x13814000;
}


inline Nequeo::Timestamp::TimeVal Nequeo::Timestamp::epochMicroseconds() const
{
	return _ts;
}


inline Nequeo::Timestamp::TimeDiff Nequeo::Timestamp::elapsed() const
{
	Nequeo::Timestamp now;
	return now - *this;
}


inline bool Nequeo::Timestamp::isElapsed(Nequeo::Timestamp::TimeDiff interval) const
{
	Timestamp now;
	Timestamp::TimeDiff diff = now - *this;
	return diff >= interval;
}


inline Nequeo::Timestamp::TimeVal Nequeo::Timestamp::resolution()
{
	return 1000000;
}


inline void swap(Nequeo::Timestamp& s1, Nequeo::Timestamp& s2)
{
	s1.swap(s2);
}