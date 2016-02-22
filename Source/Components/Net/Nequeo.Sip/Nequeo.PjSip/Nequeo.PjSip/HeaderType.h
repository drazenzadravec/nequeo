/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          HeaderType.h
*  Purpose :       SIP HeaderType class.
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

#ifndef _HEADERTYPE_H
#define _HEADERTYPE_H

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
			/// Header types, as defined by RFC3261.
			/// </summary>
			public enum class HeaderType
			{
				/*
				* These are the headers documented in RFC3261. Headers not documented
				* there must have type PJSIP_H_OTHER, and the header type itself is
				* recorded in the header name string.
				*/
				PJSIP_H_ACCEPT,
				PJSIP_H_ACCEPT_ENCODING_UNIMP,	/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_ACCEPT_LANGUAGE_UNIMP,	/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_ALERT_INFO_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_ALLOW,
				PJSIP_H_AUTHENTICATION_INFO_UNIMP,	/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_AUTHORIZATION,
				PJSIP_H_CALL_ID,
				PJSIP_H_CALL_INFO_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_CONTACT,
				PJSIP_H_CONTENT_DISPOSITION_UNIMP,	/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_CONTENT_ENCODING_UNIMP,	/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_CONTENT_LANGUAGE_UNIMP,	/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_CONTENT_LENGTH,
				PJSIP_H_CONTENT_TYPE,
				PJSIP_H_CSEQ,
				PJSIP_H_DATE_UNIMP,			/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_ERROR_INFO_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_EXPIRES,
				PJSIP_H_FROM,
				PJSIP_H_IN_REPLY_TO_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_MAX_FORWARDS,
				PJSIP_H_MIME_VERSION_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_MIN_EXPIRES,
				PJSIP_H_ORGANIZATION_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_PRIORITY_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_PROXY_AUTHENTICATE,
				PJSIP_H_PROXY_AUTHORIZATION,
				PJSIP_H_PROXY_REQUIRE_UNIMP,	/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_RECORD_ROUTE,
				PJSIP_H_REPLY_TO_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_REQUIRE,
				PJSIP_H_RETRY_AFTER,
				PJSIP_H_ROUTE,
				PJSIP_H_SERVER_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_SUBJECT_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_SUPPORTED,
				PJSIP_H_TIMESTAMP_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_TO,
				PJSIP_H_UNSUPPORTED,
				PJSIP_H_USER_AGENT_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_VIA,
				PJSIP_H_WARNING_UNIMP,		/* N/A, use pjsip_generic_string_hdr */
				PJSIP_H_WWW_AUTHENTICATE,
				PJSIP_H_OTHER
			};
		}
	}
}
#endif