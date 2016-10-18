/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Array.cpp
*  Purpose :       Array class.
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

#include "Array.h"

namespace Nequeo
{
	Array<CryptoBuffer> CryptoBuffer::Slice(size_t sizeOfSlice) const
	{
		assert(sizeOfSlice <= GetLength());

		size_t numberOfSlices = (GetLength() + sizeOfSlice - 1) / sizeOfSlice;
		size_t currentSliceIndex = 0;
		Array<CryptoBuffer> slices(numberOfSlices);

		for (size_t i = 0; i < numberOfSlices - 1; ++i)
		{
			CryptoBuffer newArray(sizeOfSlice);
			for (size_t cpyIdx = 0; cpyIdx < newArray.GetLength(); ++cpyIdx)
			{
				newArray[cpyIdx] = GetItem(cpyIdx + currentSliceIndex);
			}
			currentSliceIndex += sizeOfSlice;
			slices[i] = std::move(newArray);
		}

		CryptoBuffer lastArray(GetLength() % sizeOfSlice == 0 ? sizeOfSlice : GetLength() % sizeOfSlice);
		for (size_t cpyIdx = 0; cpyIdx < lastArray.GetLength(); ++cpyIdx)
		{
			lastArray[cpyIdx] = GetItem(cpyIdx + currentSliceIndex);
		}
		slices[slices.GetLength() - 1] = std::move(lastArray);

		return slices;
	}

	CryptoBuffer& CryptoBuffer::operator^(const CryptoBuffer& operand)
	{
		size_t smallestSize = std::min<size_t>(GetLength(), operand.GetLength());
		for (size_t i = 0; i < smallestSize; ++i)
		{
			(*this)[i] ^= operand[i];
		}

		return *this;
	}

	/**
	* Zero out the array securely.
	*/
	void CryptoBuffer::Zero()
	{
		if (GetUnderlyingData())
		{
			SecureZeroMemory(GetUnderlyingData(), GetLength());
		}
	}
}