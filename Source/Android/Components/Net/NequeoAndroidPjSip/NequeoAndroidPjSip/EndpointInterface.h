/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          EndpointInterface.h
*  Purpose :       SIP Endpoint Interface class.
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

#include <pjsua2\endpoint.hpp>

namespace Nequeo {
	namespace Net {
		namespace Android {
			namespace PjSip
			{
				typedef void(*OnNatDetectionComplete_Function)(const pj::OnNatDetectionCompleteParam&);
				typedef void(*OnNatCheckStunServersComplete_Function)(const pj::OnNatCheckStunServersCompleteParam&);
				typedef void(*OnTransportState_Function)(const pj::OnTransportStateParam&);
				typedef void(*OnTimer_Function)(const pj::OnTimerParam&);
				typedef void(*OnSelectAccount_Function)(pj::OnSelectAccountParam&);

				///	<summary>
				///	Endpoint callbacks.
				///	</summary>
				class EndpointInterface : public pj::Endpoint
				{
				public:
					///	<summary>
					///	Endpoint callbacks.
					///	</summary>
					EndpointInterface();

					///	<summary>
					///	Endpoint callbacks.
					///	</summary>
					virtual ~EndpointInterface();
				};
			}
		}
	}
}
