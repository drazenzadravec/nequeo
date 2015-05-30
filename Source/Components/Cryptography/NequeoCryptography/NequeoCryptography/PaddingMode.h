/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          PaddingMode.h
*  Purpose :       PaddingMode class.
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

#ifndef _PADDINGMODE_H
#define _PADDINGMODE_H

#include "GlobalCryptography.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Specifies the type of padding to apply when the message data block is shorter
		/// than the full number of bytes needed for a cryptographic operation.
		/// </summary>
		enum PaddingMode
		{
			/// <summary>
			/// No padding is done.
			/// </summary>
			No = 1,
			/// <summary>
			/// The PKCS #7 padding string consists of a sequence of bytes, each of which
			/// is equal to the total number of padding bytes added.
			/// </summary>
			PKCS7 = 2,
			/// <summary>
			/// The padding string consists of bytes set to zero.
			/// </summary>
			Zeros = 3,
			/// <summary>
			/// The ANSIX923 padding string consists of a sequence of bytes filled with zeros
			/// before the length.
			/// </summary>
			ANSIX923 = 4,
			/// <summary>
			/// The ISO10126 padding string consists of random data before the length.
			/// </summary>
			ISO10126 = 5,
		};
	}
}
#endif