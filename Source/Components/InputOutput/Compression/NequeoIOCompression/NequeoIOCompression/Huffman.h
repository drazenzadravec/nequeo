/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Huffman.h
*  Purpose :       Huffman class.
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

#ifndef _HUFFMAN_H
#define _HUFFMAN_H

#include "GlobalIOCompression.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Provides methods and properties used to compress and decompress archives.
			/// </summary>
			class Huffman
			{
			public:
				/// <summary>
				/// Provides methods and properties used to compress and decompress archives.
				/// </summary>
				Huffman();
				~Huffman();

				/// <summary>
				/// Compress the source file.
				/// </summary>
				/// <param name="sourceFileName">The path and filename of the file to compress.</param>
				/// <param name="destinationFileName">The path and filename of the file where the compressed data will be stored.</param>
				/// <returns>True if compressed; else false.</returns>
				bool CompressFile(LPCWSTR sourceFileName, LPCWSTR destinationFileName);

				/// <summary>
				/// Decompress the source file.
				/// </summary>
				/// <param name="sourceFileName">The path and filename of the file containing the compressed data.</param>
				/// <param name="destinationFileName">The path and filename of the file where the decompressed data will be stored.</param>
				/// <returns>True if decompressed; else false.</returns>
				bool DecompressFile(LPCWSTR sourceFileName, LPCWSTR destinationFileName);

				/// <summary>
				/// Compress the source data.
				/// </summary>
				/// <param name="sourceData">The byte array of data to compress.</param>
				/// <param name="sourceDataSize">The size of the source data to compress.</param>
				/// <param name="destinationData">The byte array where compressed data will be stored.</param>
				/// <param name="destinationDataSize">The size of the destination compressed data.</param>
				/// <returns>True if compressed; else false.</returns>
				bool CompressData(PBYTE sourceData, SIZE_T sourceDataSize, PBYTE destinationData, PSIZE_T destinationDataSize);

				/// <summary>
				/// Decompress the source data.
				/// </summary>
				/// <param name="sourceData">The byte array of data to decompress.</param>
				/// <param name="sourceDataSize">The size of the source data to decompress.</param>
				/// <param name="destinationData">The byte array where decompressed data will be stored.</param>
				/// <param name="destinationDataSize">The size of the destination decompressed data.</param>
				/// <returns>True if decompressed; else false.</returns>
				bool DecompressData(PBYTE sourceData, SIZE_T sourceDataSize, PBYTE destinationData, PSIZE_T destinationDataSize);

			private:
				bool _disposed;

			};
		}
	}
}
#endif