/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          StatusCode.h
*  Purpose :       SIP StatusCode class.
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

#ifndef _STATUSCODE_H
#define _STATUSCODE_H

#include "stdafx.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			/// <summary>
			/// Status code type.
			/// </summary>
			/// <remarks>
			/// This enumeration lists standard SIP status codes according to RFC 3261.
			/// In addition, it also declares new status class 7xx for errors generated
			/// by the stack.This status class however should not get transmitted on the wire.
			/// </remarks>
			public enum class StatusCode : unsigned
			{
				/// <summary>
				/// 
				/// </summary>
				SC_TRYING = 100,
				/// <summary>
				/// 
				/// </summary>
				SC_RINGING = 180,
				/// <summary>
				/// 
				/// </summary>
				SC_CALL_BEING_FORWARDED = 181,
				/// <summary>
				/// 
				/// </summary>
				SC_QUEUED = 182,
				/// <summary>
				/// 
				/// </summary>
				SC_PROGRESS = 183,
				/// <summary>
				/// 
				/// </summary>
				SC_OK = 200,
				/// <summary>
				/// 
				/// </summary>
				SC_ACCEPTED = 202,
				/// <summary>
				/// 
				/// </summary>
				SC_MULTIPLE_CHOICES = 300,
				/// <summary>
				/// 
				/// </summary>
				SC_MOVED_PERMANENTLY = 301,
				/// <summary>
				/// 
				/// </summary>
				SC_MOVED_TEMPORARILY = 302,
				/// <summary>
				/// 
				/// </summary>
				SC_USE_PROXY = 305,
				/// <summary>
				/// 
				/// </summary>
				SC_ALTERNATIVE_SERVICE = 380,
				/// <summary>
				/// 
				/// </summary>
				SC_BAD_REQUEST = 400,
				/// <summary>
				/// 
				/// </summary>
				SC_UNAUTHORIZED = 401,
				/// <summary>
				/// 
				/// </summary>
				SC_PAYMENT_REQUIRED = 402,
				/// <summary>
				/// 
				/// </summary>
				SC_FORBIDDEN = 403,
				/// <summary>
				/// 
				/// </summary>
				SC_NOT_FOUND = 404,
				/// <summary>
				/// 
				/// </summary>
				SC_METHOD_NOT_ALLOWED = 405,
				/// <summary>
				/// 
				/// </summary>
				SC_NOT_ACCEPTABLE = 406,
				/// <summary>
				/// 
				/// </summary>
				SC_PROXY_AUTHENTICATION_REQUIRED = 407,
				/// <summary>
				/// 
				/// </summary>
				SC_REQUEST_TIMEOUT = 408,
				/// <summary>
				/// 
				/// </summary>
				SC_TSX_TIMEOUT = 409,
				/// <summary>
				/// 
				/// </summary>
				SC_GONE = 410,
				/// <summary>
				/// 
				/// </summary>
				SC_REQUEST_ENTITY_TOO_LARGE = 413,
				/// <summary>
				/// 
				/// </summary>
				SC_REQUEST_URI_TOO_LONG = 414,
				/// <summary>
				/// 
				/// </summary>
				SC_UNSUPPORTED_MEDIA_TYPE = 415,
				/// <summary>
				/// 
				/// </summary>
				SC_UNSUPPORTED_URI_SCHEME = 416,
				/// <summary>
				/// 
				/// </summary>
				SC_BAD_EXTENSION = 420,
				/// <summary>
				/// 
				/// </summary>
				SC_EXTENSION_REQUIRED = 421,
				/// <summary>
				/// 
				/// </summary>
				SC_SESSION_TIMER_TOO_SMALL = 422,
				/// <summary>
				/// 
				/// </summary>
				SC_INTERVAL_TOO_BRIEF = 423,
				/// <summary>
				/// 
				/// </summary>
				SC_TEMPORARILY_UNAVAILABLE = 480,
				/// <summary>
				/// 
				/// </summary>
				SC_CALL_TSX_DOES_NOT_EXIST = 481,
				/// <summary>
				/// 
				/// </summary>
				SC_LOOP_DETECTED = 482,
				/// <summary>
				/// 
				/// </summary>
				SC_TOO_MANY_HOPS = 483,
				/// <summary>
				/// 
				/// </summary>
				SC_ADDRESS_INCOMPLETE = 484,
				/// <summary>
				/// 
				/// </summary>
				AC_AMBIGUOUS = 485,
				/// <summary>
				/// 
				/// </summary>
				SC_BUSY_HERE = 486,
				/// <summary>
				/// 
				/// </summary>
				SC_REQUEST_TERMINATED = 487,
				/// <summary>
				/// 
				/// </summary>
				SC_NOT_ACCEPTABLE_HERE = 488,
				/// <summary>
				/// 
				/// </summary>
				SC_BAD_EVENT = 489,
				/// <summary>
				/// 
				/// </summary>
				SC_REQUEST_UPDATED = 490,
				/// <summary>
				/// 
				/// </summary>
				SC_REQUEST_PENDING = 491,
				/// <summary>
				/// 
				/// </summary>
				SC_UNDECIPHERABLE = 493,
				/// <summary>
				/// 
				/// </summary>
				SC_INTERNAL_SERVER_ERROR = 500,
				/// <summary>
				/// 
				/// </summary>
				SC_NOT_IMPLEMENTED = 501,
				/// <summary>
				/// 
				/// </summary>
				SC_BAD_GATEWAY = 502,
				/// <summary>
				/// 
				/// </summary>
				SC_SERVICE_UNAVAILABLE = 503,
				/// <summary>
				/// 
				/// </summary>
				SC_TSX_TRANSPORT_ERROR = 533,
				/// <summary>
				/// 
				/// </summary>
				SC_SERVER_TIMEOUT = 504,
				/// <summary>
				/// 
				/// </summary>
				SC_VERSION_NOT_SUPPORTED = 505,
				/// <summary>
				/// 
				/// </summary>
				SC_MESSAGE_TOO_LARGE = 513,
				/// <summary>
				/// 
				/// </summary>
				SC_PRECONDITION_FAILURE = 580,
				/// <summary>
				/// 
				/// </summary>
				SC_BUSY_EVERYWHERE = 600,
				/// <summary>
				/// 
				/// </summary>
				SC_DECLINE = 603,
				/// <summary>
				/// 
				/// </summary>
				SC_DOES_NOT_EXIST_ANYWHERE = 604,
				/// <summary>
				/// 
				/// </summary>
				SC_NOT_ACCEPTABLE_ANYWHERE = 606,
				/// <summary>
				/// 
				/// </summary>
				SC__force_32bit = 0x7FFFFFFF
			};
		}
	}
}
#endif