/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          RandomGenerator.h
*  Purpose :       RandomGenerator class.
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

#ifndef _RANDOMGENERATOR_H
#define _RANDOMGENERATOR_H

#include "GlobalCryptography.h"
#include "PasswordType.h"

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Secure pseudo random number generator.
		/// </summary>
		class RandomGenerator
		{
		public:
			RandomGenerator();
			~RandomGenerator();

			/// <summary>
			/// Create the random number.
			/// </summary>
			/// <param name="minimum">The minimum number to create.</param>
			/// <param name="maximum">The maximum number to create.</param>
			/// <returns>The random number created.</returns>
			unsigned int CreatePositiveNumber(unsigned int minimum, unsigned int maximum);

			/// <summary>
			/// Create the random number.
			/// </summary>
			/// <param name="minimum">The minimum number to create.</param>
			/// <param name="maximum">The maximum number to create.</param>
			/// <returns>The random number created.</returns>
			signed int CreateNegativeNumber(signed int minimum, signed int maximum);

			/// <summary>
			/// Create the random number.
			/// </summary>
			/// <param name="minimum">The minimum number to create.</param>
			/// <param name="maximum">The maximum number to create.</param>
			/// <returns>The random number created.</returns>
			int CreateNumber(int minimum, int maximum);

			/// <summary>
			/// Create the random string.
			/// </summary>
			/// <param name="minimum">The minimum number of characters to create.</param>
			/// <param name="maximum">The maximum number of characters to create.</param>
			/// <param name="passwordType">The type of random string to create.</param>
			/// <returns>The random string.</returns>
			string Create(unsigned int minimum, unsigned int maximum, PasswordType passwordType);

		private:
			bool _disposed;

			unsigned char _numbers[10];
			unsigned char _uppercaseLetters[26];
			unsigned char _lowercaseLetters[26];
			unsigned char _special[24];

			string Guid();
			string Number(unsigned int minimum, unsigned int maximum);
			string Special(unsigned int minimum, unsigned int maximum);
			string UpperCaseLetters(unsigned int minimum, unsigned int maximum);
			string LowerCaseLetters(unsigned int minimum, unsigned int maximum);
			string UpperCaseLettersNumber(unsigned int minimum, unsigned int maximum);
			string LowerCaseLettersNumber(unsigned int minimum, unsigned int maximum);
			string UpperCaseLettersLowerCaseLetters(unsigned int minimum, unsigned int maximum);
			string UpperCaseLettersLowerCaseLettersNumber(unsigned int minimum, unsigned int maximum);
			string UpperCaseLettersSpecial(unsigned int minimum, unsigned int maximum);
			string LowerCaseLettersSpecial(unsigned int minimum, unsigned int maximum);
			string UpperCaseLettersLowerCaseLettersSpecial(unsigned int minimum, unsigned int maximum);
			string UpperCaseLettersNumberSpecial(unsigned int minimum, unsigned int maximum);
			string LowerCaseLettersNumberSpecial(unsigned int minimum, unsigned int maximum);
			string UpperCaseLettersLowerCaseLettersNumberSpecial(unsigned int minimum, unsigned int maximum);

			string GuidTranslatorA(unsigned int minimum, unsigned int maximum);
			unsigned int SpecialTranslatorA(unsigned int number);
			unsigned int NumberTranslatorA(unsigned int number);
			unsigned int NumberTranslatorB(unsigned int number);
			unsigned int NumberTranslatorC(unsigned int number);
			unsigned int UpperCaseLettersTranslatorA(unsigned int number);
			unsigned int UpperCaseLettersTranslatorB(unsigned int number);
		};
	}
}
#endif