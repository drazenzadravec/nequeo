/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallFlag.h
*  Purpose :       SIP CallFlag class.
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

#ifndef _CALLFLAG_H
#define _CALLFLAG_H

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
			/// Flags to be given to various call APIs. More than one flags may be
			/// specified by bitmasking them.
			/// </summary>
			public enum class CallFlag : unsigned
			{
				///	<summary>
				/// When the call is being put on hold, specify this flag to unhold it.
				/// This flag is only valid for #pjsua_call_reinvite() and
				/// #pjsua_call_update(). Note: for compatibility reason, this flag must
				/// have value of 1 because previously the unhold option is specified as
				/// boolean value.
				///	</summary>
				PJSUA_CALL_UNHOLD = 1,

				///	<summary>
				/// Update the local invite session's contact with the contact URI from
				/// the account. This flag is only valid for #pjsua_call_set_hold2(),
				/// #pjsua_call_reinvite() and #pjsua_call_update(). This flag is useful
				/// in IP address change situation, after the local account's Contact has
				/// been updated (typically with re-registration) use this flag to update
				/// the invite session with the new Contact and to inform this new Contact
				/// to the remote peer with the outgoing re-INVITE or UPDATE.
				///	</summary>
				PJSUA_CALL_UPDATE_CONTACT = 2,

				///	<summary>
				/// Include SDP "m=" line with port set to zero for each disabled media
				/// (i.e when aud_cnt or vid_cnt is set to zero). This flag is only valid
				/// for #pjsua_call_make_call(), #pjsua_call_reinvite(), and
				/// #pjsua_call_update(). Note that even this flag is applicable in
				/// #pjsua_call_reinvite() and #pjsua_call_update(), it will only take
				/// effect when the re-INVITE/UPDATE operation regenerates SDP offer,
				/// such as changing audio or video count in the call setting.
				///	</summary>
				PJSUA_CALL_INCLUDE_DISABLED_MEDIA = 4,

				///	<summary>
				/// Do not send SDP when sending INVITE or UPDATE. This flag is only valid
				/// for #pjsua_call_make_call(), #pjsua_call_reinvite()/reinvite2(), or
				/// #pjsua_call_update()/update2(). For re-invite/update, specifying
				/// PJSUA_CALL_UNHOLD will take precedence over this flag.
				///	</summary>
				PJSUA_CALL_NO_SDP_OFFER = 8
			};
		}
	}
}
#endif