/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          StunNatType.h
*  Purpose :       SIP StunNatType class.
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

#ifndef _STUNNATTYPE_H
#define _STUNNATTYPE_H

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
			/// This enumeration describes the NAT types, as specified by RFC 3489
			/// Section 5, NAT Variations.
			/// </summary>
			public enum class StunNatType
			{
				/// <summary>
				/// NAT type is unknown because the detection has not been performed.
				/// </summary>
				PJ_STUN_NAT_TYPE_UNKNOWN,

				/// <summary>
				/// NAT type is unknown because there is failure in the detection
				/// process, possibly because server does not support RFC 3489.
				/// </summary>
				PJ_STUN_NAT_TYPE_ERR_UNKNOWN,

				/// <summary>
				/// This specifies that the client has open access to Internet (or
				/// at least, its behind a firewall that behaves like a full-cone NAT,
				/// but without the translation)
				/// </summary>
				PJ_STUN_NAT_TYPE_OPEN,

				/// <summary>
				/// This specifies that communication with server has failed, probably
				/// because UDP packets are blocked.
				/// </summary>
				PJ_STUN_NAT_TYPE_BLOCKED,

				/// <summary>
				/// Firewall that allows UDP out, and responses have to come back to
				/// the source of the request (like a symmetric NAT, but no
				/// translation.
				/// </summary>
				PJ_STUN_NAT_TYPE_SYMMETRIC_UDP,

				/// <summary>
				/// A full cone NAT is one where all requests from the same internal
				/// IP address and port are mapped to the same external IP address and
				/// port.  Furthermore, any external host can send a packet to the
				/// internal host, by sending a packet to the mapped external address.
				/// </summary>
				PJ_STUN_NAT_TYPE_FULL_CONE,

				/// <summary>
				/// A symmetric NAT is one where all requests from the same internal
				/// IP address and port, to a specific destination IP address and port,
				/// are mapped to the same external IP address and port.  If the same
				/// host sends a packet with the same source address and port, but to
				/// a different destination, a different mapping is used.  Furthermore,
				/// only the external host that receives a packet can send a UDP packet
				/// back to the internal host.
				/// </summary>
				PJ_STUN_NAT_TYPE_SYMMETRIC,

				/// <summary>
				/// A restricted cone NAT is one where all requests from the same
				/// internal IP address and port are mapped to the same external IP
				/// address and port.  Unlike a full cone NAT, an external host (with
				/// IP address X) can send a packet to the internal host only if the
				/// internal host had previously sent a packet to IP address X.
				/// </summary>
				PJ_STUN_NAT_TYPE_RESTRICTED,

				/// <summary>
				/// A port restricted cone NAT is like a restricted cone NAT, but the
				/// restriction includes port numbers. Specifically, an external host
				/// can send a packet, with source IP address X and source port P,
				/// to the internal host only if the internal host had previously sent
				/// a packet to IP address X and port P.
				/// </summary>
				PJ_STUN_NAT_TYPE_PORT_RESTRICTED
			};
		}
	}
}
#endif