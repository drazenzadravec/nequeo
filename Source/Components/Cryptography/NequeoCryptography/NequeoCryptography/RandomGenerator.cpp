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

#include "stdafx.h"

#include "RandomGenerator.h"
#include "cryptlib.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"

#include <boost\random\linear_congruential.hpp>
#include <boost\random\uniform_int.hpp>
#include <boost\random\uniform_real.hpp>
#include <boost\random\variate_generator.hpp>
#include <boost\generator_iterator.hpp>
#include <boost\random.hpp>

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Secure pseudo random number generator.
		/// </summary>
		RandomGenerator::RandomGenerator() : _disposed(false)
		{
			// Assign numbers.
			_numbers[0] = 48;
			_numbers[1] = 49;
			_numbers[2] = 50;
			_numbers[3] = 51;
			_numbers[4] = 52;
			_numbers[5] = 53;
			_numbers[6] = 54;
			_numbers[7] = 55;
			_numbers[8] = 56;
			_numbers[9] = 57;

			// Assign special characters.
			_special[0] = 33;	// !  - 123
			_special[1] = 35;	// #  - 124
			_special[2] = 36;	// $  - 125
			_special[3] = 38;	// &  - 126
			_special[4] = 40;	// (  - 127
			_special[5] = 41;	// )  - 128
			_special[6] = 42;	// *  - 129
			_special[7] = 43;	// +  - 130
			_special[8] = 45;	// -  - 131
			_special[9] = 47;	// /  - 132
			_special[10] = 58;	// :  - 133
			_special[11] = 59;	// ;  - 134
			_special[12] = 60;	// <  - 135
			_special[13] = 61;	// =  - 136
			_special[14] = 62;	// >  - 137
			_special[15] = 63;	// ?  - 138
			_special[16] = 64;	// @  - 139
			_special[17] = 91;	// [  - 140
			_special[18] = 93;	// ]  - 141
			_special[19] = 94;	// ^  - 142
			_special[20] = 95;	// _  - 143
			_special[21] = 123;	// {  - 144
			_special[22] = 124;	// |  - 145
			_special[23] = 125;	// }  - 146

			// Assign upper case letters.
			_uppercaseLetters[0] = 65;
			_uppercaseLetters[1] = 66;
			_uppercaseLetters[2] = 67;
			_uppercaseLetters[3] = 68;
			_uppercaseLetters[4] = 69;
			_uppercaseLetters[5] = 70;
			_uppercaseLetters[6] = 71;
			_uppercaseLetters[7] = 72;
			_uppercaseLetters[8] = 73;
			_uppercaseLetters[9] = 74;
			_uppercaseLetters[10] = 75;
			_uppercaseLetters[11] = 76;
			_uppercaseLetters[12] = 77;
			_uppercaseLetters[13] = 78;
			_uppercaseLetters[14] = 79;
			_uppercaseLetters[15] = 80;
			_uppercaseLetters[16] = 81;
			_uppercaseLetters[17] = 82;
			_uppercaseLetters[18] = 83;
			_uppercaseLetters[19] = 84;
			_uppercaseLetters[20] = 85;
			_uppercaseLetters[21] = 86;
			_uppercaseLetters[22] = 87;
			_uppercaseLetters[23] = 88;
			_uppercaseLetters[24] = 89;
			_uppercaseLetters[25] = 90;

			// Assign lower case letters.
			_lowercaseLetters[0] = 97;
			_lowercaseLetters[1] = 98;
			_lowercaseLetters[2] = 99;
			_lowercaseLetters[3] = 100;
			_lowercaseLetters[4] = 101;
			_lowercaseLetters[5] = 102;
			_lowercaseLetters[6] = 103;
			_lowercaseLetters[7] = 104;
			_lowercaseLetters[8] = 105;
			_lowercaseLetters[9] = 106;
			_lowercaseLetters[10] = 107;
			_lowercaseLetters[11] = 108;
			_lowercaseLetters[12] = 109;
			_lowercaseLetters[13] = 110;
			_lowercaseLetters[14] = 111;
			_lowercaseLetters[15] = 112;
			_lowercaseLetters[16] = 113;
			_lowercaseLetters[17] = 114;
			_lowercaseLetters[18] = 115;
			_lowercaseLetters[19] = 116;
			_lowercaseLetters[20] = 117;
			_lowercaseLetters[21] = 118;
			_lowercaseLetters[22] = 119;
			_lowercaseLetters[23] = 120;
			_lowercaseLetters[24] = 121;
			_lowercaseLetters[25] = 122;
		}

		/// <summary>
		/// This destructor.
		/// </summary>
		RandomGenerator::~RandomGenerator()
		{
			// If not disposed.
			if (!_disposed)
			{
				// Indicate that dispose has been called.
				_disposed = true;
			}
		}

		/// <summary>
		/// Create the random number.
		/// </summary>
		/// <param name="minimum">The minimum number to create.</param>
		/// <param name="maximum">The maximum number to create.</param>
		/// <returns>The random number created.</returns>
		unsigned int RandomGenerator::CreatePositiveNumber(unsigned int minimum, unsigned int maximum)
		{
			if (minimum < 0 || maximum < 0 || minimum > maximum || maximum > 0xffffffffL || minimum > 0xffffffffL)
				throw Nequeo::Exceptions::RangeException("Invalid parameters.");

			// non-deterministic generator
			// to seed mersenne twister.
			// replace the call to rd() with a
			// constant value to get repeatable
			// results.
			random_device rd;   
			mt19937 gen(rd());  

			// This is a typedef for a random number generator.
			// Try boost::mt19937 or boost::ecuyer1988 instead of boost::minstd_rand
			typedef boost::mt19937 base_generator_type;

			// Define a random number generator and initialize it with a reproducible seed.
			base_generator_type generator(42);
			generator.seed(gen());

			// Create a dist between min and max.
			boost::uniform_int<> degen_dist(minimum, maximum);
			boost::variate_generator<base_generator_type&, boost::uniform_int<> > deg(generator, degen_dist);
			
			// Get the value.
			unsigned int *resultNumbers = new unsigned int[10];
			
			// Generate new numbers an take the last.
			for (unsigned int i = 0; i < 10; i++)
				resultNumbers[i] = deg();

			// Take the last.
			unsigned int result = resultNumbers[9];
			delete[] resultNumbers;
			return result;
		}

		/// <summary>
		/// Create the random number.
		/// </summary>
		/// <param name="minimum">The minimum number to create.</param>
		/// <param name="maximum">The maximum number to create.</param>
		/// <returns>The random number created.</returns>
		signed int RandomGenerator::CreateNegativeNumber(signed int minimum, signed int maximum)
		{
			// non-deterministic generator
			// to seed mersenne twister.
			// replace the call to rd() with a
			// constant value to get repeatable
			// results.
			random_device rd;
			mt19937 gen(rd());

			// This is a typedef for a random number generator.
			// Try boost::mt19937 or boost::ecuyer1988 instead of boost::minstd_rand
			typedef boost::mt19937 base_generator_type;

			// Define a random number generator and initialize it with a reproducible seed.
			base_generator_type generator(42);
			generator.seed(gen());

			// Create a dist between min and max.
			boost::uniform_int<> degen_dist(minimum, maximum);
			boost::variate_generator<base_generator_type&, boost::uniform_int<> > deg(generator, degen_dist);

			// Get the value.
			signed int *resultNumbers = new signed int[10];

			// Generate new numbers an take the last.
			for (unsigned int i = 0; i < 10; i++)
				resultNumbers[i] = deg();

			// Take the last.
			signed int result = resultNumbers[9];
			delete[] resultNumbers;
			return result;
		}

		/// <summary>
		/// Create the random number.
		/// </summary>
		/// <param name="minimum">The minimum number to create.</param>
		/// <param name="maximum">The maximum number to create.</param>
		/// <returns>The random number created.</returns>
		int RandomGenerator::CreateNumber(int minimum, int maximum)
		{
			// non-deterministic generator
			// to seed mersenne twister.
			// replace the call to rd() with a
			// constant value to get repeatable
			// results.
			random_device rd;
			mt19937 gen(rd());

			// This is a typedef for a random number generator.
			// Try boost::mt19937 or boost::ecuyer1988 instead of boost::minstd_rand
			typedef boost::mt19937 base_generator_type;

			// Define a random number generator and initialize it with a reproducible seed.
			base_generator_type generator(42);
			generator.seed(gen());

			// Create a dist between min and max.
			boost::uniform_int<> degen_dist(minimum, maximum);
			boost::variate_generator<base_generator_type&, boost::uniform_int<> > deg(generator, degen_dist);

			// Get the value.
			int *resultNumbers = new int[10];

			// Generate new numbers an take the last.
			for (unsigned int i = 0; i < 10; i++)
				resultNumbers[i] = deg();

			// Take the last.
			int result = resultNumbers[9];
			delete[] resultNumbers;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <param name="passwordType">The type of random string to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::Create(unsigned int minimum, unsigned int maximum, PasswordType passwordType)
		{
			if (minimum <= 0 || maximum <= 0 || minimum > maximum || maximum >= 0xffffffffL || minimum >= 0xffffffffL)
				throw Nequeo::Exceptions::RangeException("Invalid parameters.");

			string result;

			// Select the type of string to create.
			switch (passwordType)
			{
			case Nequeo::Cryptography::Number:
				result = Number(minimum, maximum);
				break;
			case Nequeo::Cryptography::Special:
				result = Special(minimum, maximum);
				break;
			case Nequeo::Cryptography::Token:
				result = UpperCaseLettersLowerCaseLetters(minimum, maximum);
				break;
			case Nequeo::Cryptography::Guid:
				result = Guid(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters:
				result = UpperCaseLetters(minimum, maximum);
				break;
			case Nequeo::Cryptography::LowerCaseLetters:
				result = LowerCaseLetters(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters_Number:
				result = UpperCaseLettersNumber(minimum, maximum);
				break;
			case Nequeo::Cryptography::LowerCaseLetters_Number:
				result = LowerCaseLettersNumber(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters_LowerCaseLetters:
				result = UpperCaseLettersLowerCaseLetters(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters_LowerCaseLetters_Number:
				result = UpperCaseLettersLowerCaseLettersNumber(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters_Special:
				result = UpperCaseLettersSpecial(minimum, maximum);
				break;
			case Nequeo::Cryptography::LowerCaseLetters_Special:
				result = LowerCaseLettersSpecial(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters_LowerCaseLetters_Special:
				result = UpperCaseLettersLowerCaseLettersSpecial(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters_Number_Special:
				result = UpperCaseLettersNumberSpecial(minimum, maximum);
				break;
			case Nequeo::Cryptography::LowerCaseLetters_Number_Special:
				result = LowerCaseLettersNumberSpecial(minimum, maximum);
				break;
			case Nequeo::Cryptography::UpperCaseLetters_LowerCaseLetters_Number_Special:
				result = UpperCaseLettersLowerCaseLettersNumberSpecial(minimum, maximum);
				break;
			default:
				result = Number(minimum, maximum);
				break;
			}

			// Return the result.
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::Guid(unsigned int minimum, unsigned int maximum)
		{
			string one = GuidTranslatorA(8, 8);
			string two = GuidTranslatorA(4, 4);
			string three = GuidTranslatorA(4, 4);
			string four = GuidTranslatorA(4, 4);
			string five = GuidTranslatorA(12, 12);
			return one + "-" + two + "-" + three + "-" + four + "-" + five;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::Number(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = CreatePositiveNumber(48, 57);
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::Special(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = SpecialTranslatorA(CreatePositiveNumber(123, 146));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLetters(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = CreatePositiveNumber(65, 90);
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::LowerCaseLetters(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = CreatePositiveNumber(97, 122);
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLettersNumber(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = NumberTranslatorA(CreatePositiveNumber(55, 90));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::LowerCaseLettersNumber(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = NumberTranslatorB(CreatePositiveNumber(87, 122));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLettersLowerCaseLetters(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = UpperCaseLettersTranslatorA(CreatePositiveNumber(71, 122));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLettersLowerCaseLettersNumber(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = UpperCaseLettersTranslatorA(NumberTranslatorC(CreatePositiveNumber(61, 122)));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLettersLowerCaseLettersNumberSpecial(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = SpecialTranslatorA(UpperCaseLettersTranslatorA(NumberTranslatorC(CreatePositiveNumber(61, 146))));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLettersSpecial(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = SpecialTranslatorA(UpperCaseLettersTranslatorB(CreatePositiveNumber(97, 146)));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::LowerCaseLettersSpecial(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = SpecialTranslatorA(CreatePositiveNumber(97, 146));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLettersLowerCaseLettersSpecial(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = SpecialTranslatorA(UpperCaseLettersTranslatorA(CreatePositiveNumber(71, 146)));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::UpperCaseLettersNumberSpecial(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = SpecialTranslatorA(UpperCaseLettersTranslatorB(NumberTranslatorB(CreatePositiveNumber(87, 146))));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::LowerCaseLettersNumberSpecial(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = SpecialTranslatorA(NumberTranslatorB(CreatePositiveNumber(87, 146)));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Create the random string.
		/// </summary>
		/// <param name="minimum">The minimum number of characters to create.</param>
		/// <param name="maximum">The maximum number of characters to create.</param>
		/// <returns>The random string.</returns>
		string RandomGenerator::GuidTranslatorA(unsigned int minimum, unsigned int maximum)
		{
			string result;
			unsigned int currentNumber;
			unsigned int totalNumber = CreatePositiveNumber(minimum, maximum);
			char *numberChar = new char[totalNumber];

			// For the number of characters to create.
			for (unsigned int i = 0; i < totalNumber; i++)
			{
				// Create the random number, and get the char value.
				currentNumber = NumberTranslatorA(CreatePositiveNumber(55, 70));
				numberChar[i] = currentNumber;
			}

			// Return the result.
			result = string(numberChar, totalNumber);
			delete[] numberChar;
			return result;
		}

		/// <summary>
		/// Get the cuurent translation.
		/// </summary>
		/// <param name="number">The number to translate.</param>
		/// <returns>The translated number.</returns>
		unsigned int RandomGenerator::UpperCaseLettersTranslatorB(unsigned int number)
		{
			unsigned int trans;

			// Select the current special number.
			switch (number)
			{
			case 97:
				trans = 65;	// A
				break;
			case 98:
				trans = 66;	// B
				break;
			case 99:
				trans = 67;	// C
				break;
			case 100:
				trans = 68;	// D
				break;
			case 101:
				trans = 69;	// E
				break;
			case 102:
				trans = 70;	// F
				break;
			case 103:
				trans = 71;	// G
				break;
			case 104:
				trans = 72;	// H
				break;
			case 105:
				trans = 73;	// I
				break;
			case 106:
				trans = 74;	// J
				break;
			case 107:
				trans = 75;	// K
				break;
			case 108:
				trans = 76;	// L
				break;
			case 109:
				trans = 77;	// M
				break;
			case 110:
				trans = 78;	// N
				break;
			case 111:
				trans = 79;	// O
				break;
			case 112:
				trans = 80;	// P
				break;
			case 113:
				trans = 81;	// Q
				break;
			case 114:
				trans = 82;	// R
				break;
			case 115:
				trans = 83;	// S
				break;
			case 116:
				trans = 84;	// T
				break;
			case 117:
				trans = 85;	// U
				break;
			case 118:
				trans = 86;	// V
				break;
			case 119:
				trans = 87;	// W
				break;
			case 120:
				trans = 88;	// X
				break;
			case 121:
				trans = 89;	// Y
				break;
			case 122:
				trans = 90;	// Z
				break;
			default:
				trans = number;
				break;
			}

			// Return the translation.
			return trans;
		}

		/// <summary>
		/// Get the cuurent translation.
		/// </summary>
		/// <param name="number">The number to translate.</param>
		/// <returns>The translated number.</returns>
		unsigned int RandomGenerator::NumberTranslatorC(unsigned int number)
		{
			unsigned int trans;

			// Select the current special number.
			switch (number)
			{
			case 61:
				trans = 48;	// 0
				break;
			case 62:
				trans = 49;	// 1
				break;
			case 63:
				trans = 50;	// 2
				break;
			case 64:
				trans = 51;	// 3
				break;
			case 65:
				trans = 52;	// 4
				break;
			case 66:
				trans = 53;	// 5
				break;
			case 67:
				trans = 54;	// 6
				break;
			case 68:
				trans = 55;	// 7
				break;
			case 69:
				trans = 56;	// 8
				break;
			case 70:
				trans = 57;	// 9
				break;
			default:
				trans = number;
				break;
			}

			// Return the translation.
			return trans;
		}

		/// <summary>
		/// Get the cuurent translation.
		/// </summary>
		/// <param name="number">The number to translate.</param>
		/// <returns>The translated number.</returns>
		unsigned int RandomGenerator::UpperCaseLettersTranslatorA(unsigned int number)
		{
			unsigned int trans;

			// Select the current special number.
			switch (number)
			{
			case 71:
				trans = 65;	// A
				break;
			case 72:
				trans = 66;	// B
				break;
			case 73:
				trans = 67;	// C
				break;
			case 74:
				trans = 68;	// D
				break;
			case 75:
				trans = 69;	// E
				break;
			case 76:
				trans = 70;	// F
				break;
			case 77:
				trans = 71;	// G
				break;
			case 78:
				trans = 72;	// H
				break;
			case 79:
				trans = 73;	// I
				break;
			case 80:
				trans = 74;	// J
				break;
			case 81:
				trans = 75;	// K
				break;
			case 82:
				trans = 76;	// L
				break;
			case 83:
				trans = 77;	// M
				break;
			case 84:
				trans = 78;	// N
				break;
			case 85:
				trans = 79;	// O
				break;
			case 86:
				trans = 80;	// P
				break;
			case 87:
				trans = 81;	// Q
				break;
			case 88:
				trans = 82;	// R
				break;
			case 89:
				trans = 83;	// S
				break;
			case 90:
				trans = 84;	// T
				break;
			case 91:
				trans = 85;	// U
				break;
			case 92:
				trans = 86;	// V
				break;
			case 93:
				trans = 87;	// W
				break;
			case 94:
				trans = 88;	// X
				break;
			case 95:
				trans = 89;	// Y
				break;
			case 96:
				trans = 90;	// Z
				break;
			default:
				trans = number;
				break;
			}

			// Return the translation.
			return trans;
		}

		/// <summary>
		/// Get the cuurent translation.
		/// </summary>
		/// <param name="number">The number to translate.</param>
		/// <returns>The translated number.</returns>
		unsigned int RandomGenerator::NumberTranslatorB(unsigned int number)
		{
			unsigned int trans;

			// Select the current special number.
			switch (number)
			{
			case 87:
				trans = 48;	// 0
				break;
			case 88:
				trans = 49;	// 1
				break;
			case 89:
				trans = 50;	// 2
				break;
			case 90:
				trans = 51;	// 3
				break;
			case 91:
				trans = 52;	// 4
				break;
			case 92:
				trans = 53;	// 5
				break;
			case 93:
				trans = 54;	// 6
				break;
			case 94:
				trans = 55;	// 7
				break;
			case 95:
				trans = 56;	// 8
				break;
			case 96:
				trans = 57;	// 9
				break;
			default:
				trans = number;
				break;
			}

			// Return the translation.
			return trans;
		}

		/// <summary>
		/// Get the cuurent translation.
		/// </summary>
		/// <param name="number">The number to translate.</param>
		/// <returns>The translated number.</returns>
		unsigned int RandomGenerator::NumberTranslatorA(unsigned int number)
		{
			unsigned int trans;

			// Select the current number char.
			switch (number)
			{
			case 55:
				trans = 48;	// 0
				break;
			case 56:
				trans = 49;	// 1
				break;
			case 57:
				trans = 50;	// 2
				break;
			case 58:
				trans = 51;	// 3
				break;
			case 59:
				trans = 52;	// 4
				break;
			case 60:
				trans = 53;	// 5
				break;
			case 61:
				trans = 54;	// 6
				break;
			case 62:
				trans = 55;	// 7
				break;
			case 63:
				trans = 56;	// 8
				break;
			case 64:
				trans = 57;	// 9
				break;
			default:
				trans = number;
				break;
			}

			// Return the translation.
			return trans;
		}

		/// <summary>
		/// Get the cuurent translation.
		/// </summary>
		/// <param name="number">The number to translate.</param>
		/// <returns>The translated number.</returns>
		unsigned int RandomGenerator::SpecialTranslatorA(unsigned int number)
		{
			unsigned int trans;

			// Select the current special char.
			switch (number)
			{
			case 123:
				trans = 33;	// !
				break;
			case 124:
				trans = 35;	// #
				break;
			case 125:
				trans = 36;	// $
				break;
			case 126:
				trans = 38;	// &
				break;
			case 127:
				trans = 40;	// (
				break;
			case 128:
				trans = 41;	// )
				break;
			case 129:
				trans = 42;	// *
				break;
			case 130:
				trans = 43;	// +
				break;
			case 131:
				trans = 45;	// -
				break;
			case 132:
				trans = 47;	// /
				break;
			case 133:
				trans = 58;	// :
				break;
			case 134:
				trans = 59;	// ;
				break;
			case 135:
				trans = 60;	// <
				break;
			case 136:
				trans = 61;	// =
				break;
			case 137:
				trans = 62;	// >
				break;
			case 138:
				trans = 63;	// ?
				break;
			case 139:
				trans = 64;	// @
				break;
			case 140:
				trans = 91;	// [
				break;
			case 141:
				trans = 93;	// ]
				break;
			case 142:
				trans = 94;	// ^
				break;
			case 143:
				trans = 95;	// _
				break;
			case 144:
				trans = 123;	// {
				break;
			case 145:
				trans = 124;	// |
				break;
			case 146:
				trans = 125;	// }
				break;
			default:
				trans = number;
				break;
			}

			// Return the translation.
			return trans;
		}
	}
}