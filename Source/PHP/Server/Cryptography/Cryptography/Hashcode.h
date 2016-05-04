/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          Hashcode.h
 *  Purpose :       
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

#ifndef _HASHCODE_
#define _HASHCODE_

#include "stdafx.h"

#include <vcclr.h>
#include <msclr\auto_handle.h>
#include <msclr\marshal_cppstd.h>
#include <string.h>
#include <string>

#include "HashcodeType.h"

using namespace msclr::interop;

using namespace System;
using namespace System::IO;
using namespace System::Text;
using namespace System::Numerics;
using namespace System::Security;
using namespace System::Security::Cryptography;
using namespace System::Runtime::InteropServices;

namespace Nequeo 
{
	namespace Cryptography 
	{
		///	<summary>
		///	Hash algorithm generation class.
		///	</summary>
		public ref class Hashcode sealed
		{
			public:
				// Constructors
				Hashcode();
				virtual ~Hashcode();

				static String^ GetHashcode(String^ value, HashcodeType hashcodeType);
				static String^ GetHashcodeMD5(String^ value);
				static String^ GetHashcodeSHA1(String^ value);

			private:

				bool m_disposed;

		};
	}
}

#endif