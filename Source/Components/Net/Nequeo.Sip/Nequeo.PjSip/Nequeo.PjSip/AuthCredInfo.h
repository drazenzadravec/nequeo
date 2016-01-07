/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AuthCredInfo.h
*  Purpose :       SIP AuthCredInfo class.
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

#ifndef _AUTHCREDINFO_H
#define _AUTHCREDINFO_H

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
			/// <summary>
			/// Credential information. Credential contains information to authenticate against a service.
			/// </summary>
			public ref class AuthCredInfo sealed
			{
			public:
				/// <summary>
				/// Credential information. Credential contains information to authenticate against a service.
				/// </summary>
				AuthCredInfo();

				/// <summary>
				/// Credential information. Credential contains information to authenticate against a service.
				/// </summary>
				/// <param name="username">The sip username.</param>
				/// <param name="password">The sip password.</param>
				AuthCredInfo(String^ username, String^ password);

				/// <summary>
				/// Credential information. Credential contains information to authenticate against a service.
				/// </summary>
				/// <param name="username">The sip username.</param>
				/// <param name="password">The sip password.</param>
				/// <param name="scheme">The authentication scheme (e.g. "digest").</param>
				/// <param name="realm">Realm on which this credential is to be used. Use "*" to make a credential that can be used to authenticate against any challenges.</param>
				/// <param name="dataType">Type of data that is contained in the "data" field. Use 0 if the data contains plain text password.</param>
				AuthCredInfo(String^ username, String^ password, String^ scheme, String^ realm, int dataType);

				/// <summary>
				/// Gets or sets the the data, which can be a plain text password or a hashed digest.
				/// </summary>
				property String^ Data
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the Realm on which this credential is to be used. Use "*" to make
				/// a credential that can be used to authenticate against any challenges.
				/// </summary>
				property String^ Realm
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the authentication scheme (e.g. "digest").
				/// </summary>
				property String^ Scheme
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the authentication user name.
				/// </summary>
				property String^ Username
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets the type of data that is contained in the "data" field. Use 0 if the data
				/// contains plain text password.
				/// </summary>
				property int DataType
				{
					int get();
					void set(int value);
				}

			private:
				String^ _username;
				String^ _data;
				String^ _scheme;
				String^ _realm;
				int _dataType;
			};
		}
	}
}
#endif