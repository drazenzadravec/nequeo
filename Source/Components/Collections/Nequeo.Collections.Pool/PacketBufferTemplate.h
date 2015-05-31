/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          PacketBufferTemplate.h
 *  Purpose :       
 *					Template class PacketBufferTemplate
 *
 *					Provides simple network packed buffer.
 *					
 *					When two computers communicate over a network, they send information to each other divided up into
					discrete chunks called packets. The networking layer of the computer’s operating system must pick up the
					packets and store them as they arrive. However, the computer might not have enough bandwidth to process
					all of them at once. Thus, the networking layer usually buffers, or stores, the packets until the higher
					layers have a chance to attend to them. The packets should be processed in the order they arrive, so this
					problem is perfect for a queue structure. Following is a small PacketBuffer class that stores incoming
					packets in a queue until they are processed. It’s a template so that different layers of the networking layer
					can use it for different kinds of packets, such as IP packets or TCP packets. It allows the client to specify a
					max size because operating systems usually limit the number of packets that can be stored, so as not to
					use too much memory. When the buffer is full, subsequently arriving packets are ignored
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
			///	Provides simple network packed buffer.
			///	</summary>
			template <typename T>
			class PacketBufferTemplate
			{
				public:
					
					// If maxSize is nonpositive, the size is unlimited.
					// Otherwise only maxSize packets are allowed in
					// the buffer at any one time.
					PacketBufferTemplate(int maxSize = -1);
					virtual ~PacketBufferTemplate();

					// Stores the packet in the buffer.
					// Throws overflow_error is the buffer is full.
					void bufferPacket(const T& packet);

					// Returns the next packet. Throws out_of_range
					// if the buffer is empty.
					T getNextPacket() throw (std::out_of_range);
					
				protected:
					queue<T> mPackets;
					int mMaxSize;
					
				private:
					// Prevent assignment and pass-by-value.
					PacketBufferTemplate(const PacketBufferTemplate& src);
					PacketBufferTemplate& operator=(const PacketBufferTemplate& rhs);

			};
		}
	}
}