/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallMethodData.h
*  Purpose :       SIP CallMethodData class.
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

#ifndef _CALLMETHODDATA_H
#define _CALLMETHODDATA_H

#include "stdafx.h"

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
			///	Structure containing current call data.
			///	</summary>
			struct call_data
			{
				pj_pool_t          *pool;
				pjmedia_port       *tonegen;
				pjsua_conf_port_id  toneslot;
			};

			///	<summary>
			///	Call method data.
			///	</summary>
			class CallMethodData
			{
			public:
				///	<summary>
				///	Call method data.
				///	</summary>
				CallMethodData();

				///	<summary>
				///	Call method data.
				///	</summary>
				~CallMethodData();

				/// <summary>
				/// Call initialise tone generator.
				/// </summary>
				/// <param name="call_id">The call id.</param>
				/// <returns>The stream info.</returns>
				call_data* call_init_tonegen(pjsua_call_id call_id);

				/// <summary>
				/// Call play digit tone.
				/// </summary>
				/// <param name="call_id">The call id.</param>
				/// <param name="digits">The digits.</param>
				void call_play_digit(pjsua_call_id call_id, const char *digits);

				/// <summary>
				/// Call de-initialise tone generator.
				/// </summary>
				/// <param name="call_id">The call id.</param>
				void call_deinit_tonegen(pjsua_call_id call_id);

			private:
				bool _disposed;

			};
		}
	}
}
#endif