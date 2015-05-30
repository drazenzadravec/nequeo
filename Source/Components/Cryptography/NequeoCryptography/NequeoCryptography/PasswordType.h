/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          PasswordType.h
*  Purpose :       PasswordType class.
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

#ifndef _PASSWORDTYPE_H
#define _PASSWORDTYPE_H

#include "GlobalCryptography.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Password type to generate.
		/// </summary>
		enum PasswordType
		{
			Number = 0,
			Special = 1,
			Token = 2,
			Guid = 3,
			UpperCaseLetters = 4,
			LowerCaseLetters = 5,
			UpperCaseLetters_Number = 6,
			LowerCaseLetters_Number = 7,
			UpperCaseLetters_LowerCaseLetters = 8,
			UpperCaseLetters_LowerCaseLetters_Number = 9,
			UpperCaseLetters_Special = 10,
			LowerCaseLetters_Special = 11,
			UpperCaseLetters_LowerCaseLetters_Special = 12,
			UpperCaseLetters_Number_Special = 13,
			LowerCaseLetters_Number_Special = 14,
			UpperCaseLetters_LowerCaseLetters_Number_Special = 15,
		};
	}
}
#endif