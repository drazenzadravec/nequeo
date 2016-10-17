/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AccountInfo.h
*  Purpose :       SIP AccountInfo class.
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

#ifndef _ACCOUNTINFO_H
#define _ACCOUNTINFO_H

#include "stdafx.h"

#include "StatusCode.h"

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
			/// <summary>
			/// Account information.
			/// </summary>
			public ref class AccountInfo sealed
			{
			public:
				/// <summary>
				/// Account information.
				/// </summary>
				AccountInfo();

				/// <summary>
				/// Gets or sets the account id.
				/// </summary>
				property int Id
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets a flag to indicate whether this is the default account.
				/// </summary>
				property bool IsDefault
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the presence online status for this account.
				/// </summary>
				property bool OnlineStatus
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the presence online status text.
				/// </summary>
				property String^ OnlineStatusText
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets an up to date expiration interval for account registration session.
				/// </summary>
				property int RegExpiresSec
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets a flag to tell whether this account is currently registered (has active registration session).
				/// </summary>
				property bool RegIsActive
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets a flag to tell whether this account has registration setting (reg_uri is not empty).
				/// </summary>
				property bool RegIsConfigured
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the Last registration error code. When the status field contains a SIP
				/// status code that indicates a registration failure, last registration
				/// error code contains the error code that causes the failure.In any
				/// other case, its value is zero.
				/// </summary>
				property int RegLastErr
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Gets or sets the status code.
				/// </summary>
				property StatusCode RegStatus
				{
					StatusCode get();
					void set(StatusCode value);
				}

				/// <summary>
				/// Gets or sets a describing the registration status.
				/// </summary>
				property String^ RegStatusText
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the account URI.
				/// </summary>
				property String^ Uri
				{
					String^ get();
					void set(String^ value);
				}

			private:
				int _id;
				bool _isDefault;
				bool _onlineStatus;
				String^ _onlineStatusText;
				int _regExpiresSec;
				bool _regIsActive;
				bool _regIsConfigured;
				int _regLastErr;
				StatusCode _regStatus;
				String^ _regStatusText;
				String^ _uri;
			};
		}
	}
}
#endif