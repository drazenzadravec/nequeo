/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          RingBufferDebugImplementation.cpp
 *  Purpose :       
 *					Class RingBufferDebugImplementation
 *
 *					Provides a simple debug buffer. The client specifies the number
 *					of entries in the constructor and adds messages with the addEntry()
 *					method. Once the number of entries exceeds the number allowed, new
 *					entries overwrite the oldest entries in the buffer.
 *
 *					The buffer also provides the option to print entries as they
 *					are added to the buffer. The client can specify an output stream
 *					in the constructor, and can reset it with the setOutput() method.
 *
 *					Finally, the buffer supports streaming to an output stream.
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

#include "RingBufferDebugImplementation.h"

//
// Initialize the vector to hold exactly numEntries. The vector size
// does not need to change during the lifetime of the object.
//
// Initialize the other members.
//
Nequeo::Collections::Pool::RingBufferDebugImplementation::RingBufferDebugImplementation(int numEntries, ostream* ostr) : 
	mEntries(numEntries), mOstr(ostr), mNumEntries(numEntries), mNext(0), mWrapped(false)
{
}

Nequeo::Collections::Pool::RingBufferDebugImplementation::~RingBufferDebugImplementation()
{
}

//
// The algorithm is pretty simple: add the entry to the next
// free spot, then reset mNext to indicate the free spot after
// that. If mNext reaches the end of the vector, it starts over at 0.
//
// The buffer needs to know if the buffer has wrapped or not so
// that it knows whether to print the entries past mNext in operator<<
//
void Nequeo::Collections::Pool::RingBufferDebugImplementation::addEntry(const string& entry)
{
	// Add the entry to the next free spot and increment
	// mNext to point to the free spot after that.
	mEntries[mNext++] = entry;

	// Check if we’ve reached the end of the buffer. If so, we need to wrap.
	if (mNext >= mNumEntries) 
	{
		mNext = 0;
		mWrapped = true;
	}

	// If there is a valid ostream, write this entry to it.
	if (mOstr != NULL) 
	{
		*mOstr << entry << endl;
	}
}

ostream* Nequeo::Collections::Pool::RingBufferDebugImplementation::setOutput(ostream* newOstr)
{
	ostream* ret = mOstr;
	mOstr = newOstr;
	return (ret);
}

//
// This function uses an ostream_iterator to “copy” entries directly
// from the vector to the output stream.
//
// This function must print the entries in order. If the buffer has wrapped,
// the earliest entry is one past the most recent entry, which is the entry
// indicated by mNext. So first print from entry mNext to the end.
//
// Then (even if the buffer hasn’t wrapped) print from the beginning to mNext - 1.
//
ostream& Nequeo::Collections::Pool::operator<<(ostream& ostr, const Nequeo::Collections::Pool::RingBufferDebugImplementation& rb)
{
	if (rb.mWrapped) 
	{
		//
		// If the buffer has wrapped, print the elements from
		// the earliest entry to the end.
		//
		copy (rb.mEntries.begin() + rb.mNext, rb.mEntries.end(),
		ostream_iterator<string>(ostr, "\n"));
	}

	//
	// Now print up to the most recent entry.
	// Go up to begin() + mNext because the range is not inclusive on the
	// right side.
	//
	copy (rb.mEntries.begin(), rb.mEntries.begin() + rb.mNext,
	ostream_iterator<string>(ostr, "\n"));
	return (ostr);
}

///	<summary>
///	The default number of entries.
///	</summary>
const int Nequeo::Collections::Pool::RingBufferDebugImplementation::kDefaultNumEntries;;