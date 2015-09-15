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

#include "stdafx.h"

#include "Random.h"

///	<summary>
///	Construct the random generator.
///	</summary>
/// <exception cref="System::Exception">Thrown when the random number generation fails.</exception>
Nequeo::Math::Random::Random() : m_disposed(true), m_numNumeric(4), m_numCharNumeric(10)
{
	// Total number of numerics in the 2D array.
	m_totalNumberNumerics = 0;

	m_disposed = false;

	// Initialise the numeric charactor array.
	InitialiseArrays();
}

///	<summary>
///	Deconstruct the random generator.
///	</summary>
Nequeo::Math::Random::~Random()
{
	// If not disposed.
    if (!m_disposed)
    {
		// If the objects has been created.
		if(m_charGroups != nullptr)
		{
			// Delete the memory allocation
			// for items in the array.
			for (int i = 0; i < m_numNumeric; i++)
			{
				if(m_charGroups[i] != nullptr)
					delete[] m_charGroups[i];
			}

			// Delete the memory for the top-level array.
			delete[] m_charGroups;
		}

        m_disposed = true;
    }
}

///	<summary>
///	Generator a random number between 1 and maxValue.
///	</summary>
/// <param name='maxValue'>The maximum random number to generate.</param>
/// <returns>The random number generated.</returns>
int Nequeo::Math::Random::Number(int maxValue)
{
	// Initialize random seed.
	srand((unsigned)time(NULL) );

	// Generate the number.
	int randomNumber = rand() % maxValue + 1;

	// Return the generated number.
	return randomNumber;
}

///	<summary>
///	Generator a random number.
///	</summary>
/// <param name='minLength'>The minimum number length</param>
/// <param name='maxLength'>The maximum number length.</param>
/// <returns>The random number generated.</returns>
/// <remarks>
/// The length of the generated number will be determined at
/// random and it will fall with the range determined by the
/// function parameters.
/// </remarks>
/// <exception cref="System::Exception">Thrown when the random number generation fails.</exception>
/// <exception cref="System::IndexOutOfRangeException">Thrown when the arguments are outside of the bounds.</exception>
String^ Nequeo::Math::Random::Number(int minLength, int maxLength)
{
	// Make sure that input parameters are valid.
    if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
		throw gcnew IndexOutOfRangeException("The arguments are outside of the bounds");

	// Use this code to deleted memory automatically
	// when not used any more, uses to stack.
	//auto_ptr<System::Random> charsLeftInGroup(new System::Random());

	const char* errMessage = nullptr;
	int* charsLeftInGroup;
	int* leftGroupsOrder;

	// Random number of bytes.
	array<Byte>^ randomBytes = nullptr;
	
	// This array will hold number characters.
	array<Char>^ number = nullptr;

	RNGCryptoServiceProvider^ rng = nullptr;
	System::Random^ random = nullptr;
	
	try
	{
		// Use this array to track the number of unused characters in each
        // character group.
        charsLeftInGroup = new int[m_numNumeric];

		// Initially, all characters in each group are not used.
        for (int i = 0; i < m_numNumeric; i++)
			charsLeftInGroup[i] = m_numCharNumeric;

		// Use this array to track (iterate through) unused character groups.
        leftGroupsOrder = new int[m_numNumeric];
		int sizeOfLeftGroupsOrder = m_numNumeric;

		// Initially, all character groups are not used.
		for (int i = 0; i < sizeOfLeftGroupsOrder; i++)
			leftGroupsOrder[i] = i;
		
		// Because we cannot use the default randomizer, which is based on the
        // current time (it will produce the same "random" number within a
        // second), we will use a random number generator to seed the
        // randomizer.

        // Use a 4-byte array to fill it with random bytes and convert it then
		// to an integer value.
        randomBytes = gcnew array<Byte>(4);
		
		// Generate 4 random bytes.
        rng = gcnew RNGCryptoServiceProvider();
        rng->GetBytes(randomBytes);

		// Convert 4 bytes into a 32-bit integer value.
        int seed = (randomBytes[0] & 0x7f) << 24 |
                    randomBytes[1] << 16 |
                    randomBytes[2] << 8 |
                    randomBytes[3];

		// Now, this is real randomization.
		random = gcnew System::Random(seed);

		// Allocate appropriate memory for the number.
		if (minLength < maxLength)
			number = gcnew array<Char>(random->Next(minLength, maxLength + 1));
        else
			number = gcnew array<Char>(minLength);

		// Index of the next character to be added to number.
        int nextCharIdx;

        // Index of the next character group to be processed.
        int nextGroupIdx;

        // Index which will be used to track not processed character groups.
        int nextLeftGroupsOrderIdx;

        // Index of the last non-processed character in a group.
        int lastCharIdx;

        // Index of the last non-processed group.
        int lastLeftGroupsOrderIdx = sizeOfLeftGroupsOrder - 1;
		int sizeOfNumber = number->Length;

		// Generate password characters one at a time.
        for (int i = 0; i < sizeOfNumber; i++)
        {
            // If only one character group remained unprocessed, process it;
            // otherwise, pick a random character group from the unprocessed
            // group list. To allow a special character to appear in the
            // first position, increment the second parameter of the Next
            // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
            if (lastLeftGroupsOrderIdx == 0)
                nextLeftGroupsOrderIdx = 0;
            else
                nextLeftGroupsOrderIdx = random->Next(0, lastLeftGroupsOrderIdx);

            // Get the actual index of the character group, from which we will
            // pick the next character.
            nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

            // Get the index of the last unprocessed characters in this group.
            lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

            // If only one unprocessed character is left, pick it; otherwise,
            // get a random character from the unused character list.
            if (lastCharIdx == 0)
                nextCharIdx = 0;
            else
                nextCharIdx = random->Next(0, lastCharIdx + 1);

            // Add this character to the number.
			char numberValue = m_charGroups[nextGroupIdx][nextCharIdx];
            number[i] = numberValue;

            // If we processed the last character in this group, start over.
            if (lastCharIdx == 0)
                charsLeftInGroup[nextGroupIdx] = m_numCharNumeric;
            // There are more unprocessed characters left.
            else
            {
                // Swap processed character with the last unprocessed character
                // so that we don't pick it until we process all characters in
                // this group.
                if (lastCharIdx != nextCharIdx)
                {
                    char temp = m_charGroups[nextGroupIdx][lastCharIdx];
                    m_charGroups[nextGroupIdx][lastCharIdx] = m_charGroups[nextGroupIdx][nextCharIdx];
                    m_charGroups[nextGroupIdx][nextCharIdx] = temp;
                }
                // Decrement the number of unprocessed characters in
                // this group.
                charsLeftInGroup[nextGroupIdx]--;
            }

            // If we processed the last group, start all over.
            if (lastLeftGroupsOrderIdx == 0)
                lastLeftGroupsOrderIdx = sizeOfLeftGroupsOrder - 1;
            // There are more unprocessed groups left.
            else
            {
                // Swap processed group with the last unprocessed group
                // so that we don't pick it until we process all groups.
                if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                {
                    int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                    leftGroupsOrder[lastLeftGroupsOrderIdx] = leftGroupsOrder[nextLeftGroupsOrderIdx];
                    leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                }
                // Decrement the number of unprocessed groups.
                lastLeftGroupsOrderIdx--;
            }
        }

		// Convert number characters into 
        // a string and return the result.
		return gcnew System::String(number, 0, sizeOfNumber);
	}
	catch(const std::exception& ex)
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Throw a general exception.
		throw gcnew System::Exception("Unable to generate the random number.", innerException);
	}
	finally 
	{
		// Clean up memory allocations.
		if(charsLeftInGroup != nullptr)
			delete[] charsLeftInGroup;

		if(leftGroupsOrder != nullptr)
			delete[] leftGroupsOrder;

		if(randomBytes != nullptr)
			delete[] randomBytes;

		if(number != nullptr)
			delete[] number;

		if(rng != nullptr)
			delete rng;

		if(random != nullptr)
			delete random;

		if(errMessage != nullptr)
			delete errMessage;
	}
}

///	<summary>
///	Initialise the 2D array.
///	</summary>
void Nequeo::Math::Random::InitialiseArrays()
{
	const char* errMessage = nullptr;

	try	
	{
		// Create a local array containing supported number characters
		// grouped by types. You can remove character groups from this
		// array, but doing so will weaken the number strength.
		m_charGroups = new char*[m_numNumeric];
		for (int i = 0; i < m_numNumeric; i++)
		{
			// Create the first-dimension.
			m_charGroups[i] = new char[m_numCharNumeric];

			// Assign the characters.
			m_charGroups[i][0] = '0';
			m_charGroups[i][1] = '1';
			m_charGroups[i][2] = '2';
			m_charGroups[i][3] = '3';
			m_charGroups[i][4] = '4';
			m_charGroups[i][5] = '5';
			m_charGroups[i][6] = '6';
			m_charGroups[i][7] = '7';
			m_charGroups[i][8] = '8';
			m_charGroups[i][9] = '9';

			// Increment the total element count;
			m_totalNumberNumerics += m_numCharNumeric;
		}
	}
	catch(const std::exception& ex)
	{
		// Get the error thrown
		errMessage = ex.what();
		System::String^ errorMessage = gcnew System::String(errMessage);
		System::Exception^ innerException = gcnew System::Exception(errorMessage);

		// Clean-up the memory.
		this->~Random();

		// Throw a general exception.
		throw gcnew System::Exception("Unable to allocate dynamic memory.", innerException);
	}
	finally
	{
		if(errMessage != nullptr)
			delete errMessage;
	}
}