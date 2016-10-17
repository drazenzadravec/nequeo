/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          ContactMapper.h
*  Purpose :       SIP Contact Mapper class.
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

#ifndef _CONTACTMAPPER_H
#define _CONTACTMAPPER_H

#include "stdafx.h"

#include "IPv6_Use.h"
#include "SRTP_SecureSignaling.h"
#include "SRTP_Use.h" 
#include "StatusCode.h"
#include "RpidActivity.h"
#include "BuddyStatus.h"
#include "SubscriptionState.h"

#include "pjsua2\presence.hpp"
#include "pjsua2\account.hpp"
#include "pjsua2\endpoint.hpp"

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	Contact connection mapper.
			///	</summary>
			class ContactMapper
			{
			public:
				///	<summary>
				///	Contact connection mapper.
				///	</summary>
				ContactMapper();

				///	<summary>
				///	Contact connection mapper.
				///	</summary>
				~ContactMapper();

				///	<summary>
				///	Gets or sets specify whether presence subscription should start immediately.
				///	</summary>
				void SetSubscribe(bool value);
				bool GetSubscribe();

				///	<summary>
				///	Gets or sets the contact URL or name address (sip:[Name or IP Address]@[Provider Domain or IP Address]:[Optional port number]).
				///	</summary>
				void SetUri(std::string value);
				std::string GetUri();

			private:
				bool _disposed;

				std::string _uri;
				bool _subscribe;
			};
		}
	}
}
#endif