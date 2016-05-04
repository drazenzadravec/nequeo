/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          CHashcode.h
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

#ifndef _CHASHCODE_
#define _CHASHCODE_

#include "stdafx.h"

#include <string.h>
#include <string>
#include <hash_map>

#include "HashcodeType.h"

namespace Nequeo 
{
	namespace Cryptography 
	{
		///	<summary>
		///	Hash algorithm generation class.
		///	</summary>
		public class CHashcode
		{
			public:
				// Constructors
				CHashcode();
				virtual ~CHashcode();

				std::string GetHashcode(std::string value, HashcodeType hashcodeType);
				std::string GetHashcodeMD5(std::string value);
				std::string GetHashcodeSHA1(std::string value);
			
			private:
				
				bool m_disposed;
		};
	}
}

#endif