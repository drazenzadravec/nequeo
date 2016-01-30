/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaStreamStat.h
*  Purpose :       SIP MediaStreamStat class.
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

#ifndef _MEDIASTREAMSTAT_H
#define _MEDIASTREAMSTAT_H

#include "stdafx.h"

#include "TimeVal.h"

#include "pjsua2.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	Media stream statistic.
			///	</summary>
			public ref class MediaStreamStat sealed
			{
			public:
				///	<summary>
				///	Media stream statistic.
				///	</summary>
				MediaStreamStat();

				///	<summary>
				///	Media stream statistic.
				///	</summary>
				~MediaStreamStat();

				///	<summary>
				///	Gets or sets the time when session was created.
				///	</summary>
				property TimeVal^ Start
				{
					TimeVal^ get();
					void set(TimeVal^ value);
				}

				///	<summary>
				///	Gets or sets the last TX RTP timestamp.
				///	</summary>
				property unsigned int RtpTxLastTs
				{
					unsigned int get();
					void set(unsigned int value);
				}

				///	<summary>
				///	Gets or sets the last TX RTP sequence. 
				///	</summary>
				property unsigned short RtpTxLastSeq
				{
					unsigned short get();
					void set(unsigned short value);
				}

				///	<summary>
				///	Gets or sets the Individual frame size, in bytes. 
				///	</summary>
				property unsigned FrameSize
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Minimum allowed prefetch, in frms. 
				///	</summary>
				property unsigned MinPrefetch
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Maximum allowed prefetch, in frms. 
				///	</summary>
				property unsigned MaxPrefetch
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Current burst level, in frames.
				///	</summary>
				property unsigned Burst
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Current prefetch value, in frames.
				///	</summary>
				property unsigned Prefetch
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Current buffer size, in frames.
				///	</summary>
				property unsigned Size
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Average delay, in ms.
				///	</summary>
				property unsigned AvgDelayMsec
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Minimum delay, in ms.
				///	</summary>
				property unsigned MinDelayMsec
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Maximum delay, in ms.
				///	</summary>
				property unsigned MaxDelayMsec
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Standard deviation of delay, in ms.
				///	</summary>
				property unsigned DevDelayMsec
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Average burst, in frames.
				///	</summary>
				property unsigned AvgBurst
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Number of lost frames.
				///	</summary>
				property unsigned Lost
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Number of discarded frames.
				///	</summary>
				property unsigned Discard
				{
					unsigned get();
					void set(unsigned value);
				}

				///	<summary>
				///	Gets or sets the Number of empty on GET events.
				///	</summary>
				property unsigned Empty
				{
					unsigned get();
					void set(unsigned value);
				}

			private:
				bool _disposed;
				TimeVal^ _start;
				unsigned int _rtpTxLastTs;
				unsigned short _rtpTxLastSeq;
				unsigned _frameSize;
				unsigned _minPrefetch;
				unsigned _maxPrefetch;
				unsigned _burst;
				unsigned _prefetch;
				unsigned _size;
				unsigned _avgDelayMsec;
				unsigned _minDelayMsec;
				unsigned _maxDelayMsec;
				unsigned _devDelayMsec;
				unsigned _avgBurst;
				unsigned _lost;
				unsigned _discard;
				unsigned _empty;
			};
		}
	}
}
#endif