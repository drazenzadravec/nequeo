/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          RingBufferDebugImplementation.h
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

#pragma once

#include "stdafx.h"

using namespace System;

namespace Nequeo 
{
	namespace Collections 
	{
		namespace Pool 
		{
			///	<summary>
			///	Provides a simple ring buffer file dump implementation.
			///	</summary>
			class RingBufferDebugImplementation
			{
				public:
					// Constructs a ring buffer with space for numEntries.
					// Entries are written to *ostr as they are queued.
					RingBufferDebugImplementation(int numEntries = kDefaultNumEntries, ostream* ostr = NULL);
					virtual ~RingBufferDebugImplementation();

					// Adds the string to the ring buffer, possibly overwriting the
					// oldest string in the buffer (if the buffer is full).
					void addEntry(const string& entry);

					// Streams the buffer entries, separated by newlines, to ostr.
					friend ostream& operator<<(ostream& ostr, const RingBufferDebugImplementation& rb);

					// Sets the output stream to which entries are streamed as they are added.
					// Returns the old output stream.
					ostream* setOutput(ostream* newOstr);

				protected:
					vector<string> mEntries;
					ostream* mOstr;
					int mNumEntries, mNext;
					bool mWrapped;
					static const int kDefaultNumEntries = 500;

				private:
					// Prevent assignment and pass-by-value.
					RingBufferDebugImplementation(const RingBufferDebugImplementation& src);
					RingBufferDebugImplementation& operator=(const RingBufferDebugImplementation& rhs);

			};
		}
	}
}