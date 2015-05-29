/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          Random.h
 *  Purpose :       Random number class.
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

#include "stdafx.h"

using namespace System;
using namespace System::Numerics;
using namespace System::Security;
using namespace System::Security::Cryptography;

namespace Nequeo 
{
	namespace Math 
	{
		///	<summary>
		///	Random number genrator.
		///	</summary>
		public ref class Random sealed
		{
			public:
				// Constructors
				Random();
				virtual ~Random();

				// Methods
				virtual int Number(int maxValue);
				virtual String^ Number(int minLength, int maxLength);

			private:
				// Fields
				bool m_disposed;
				int m_totalNumberNumerics;

				const int m_numNumeric;
				const int m_numCharNumeric;

				// Create a local array containing supported number characters
				// grouped by types. You can remove character groups from this
				// array, but doing so will weaken the number strength.
				char** m_charGroups;

				// Methods
				void InitialiseArrays();
		};
	}
}
