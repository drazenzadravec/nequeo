/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          GZipStream.h
*  Purpose :       GZipStream class.
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

#ifndef _GZIPSTREAM_H
#define _GZIPSTREAM_H

#include "GlobalIOCompression.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Provides methods and properties used to compress and decompress streams.
			/// </summary>
			class GZipStream
			{
			public:
				GZipStream();
				~GZipStream();

				/// <summary>
				/// Compress the array of data.
				/// </summary>
				/// <param name="decompressedData">The array of decompressed data to compress.</param>
				/// <returns>The compressed data.</returns>
				std::vector<char> Compress(const std::vector<char> decompressedData);

				/// <summary>
				/// Decompress the array of data.
				/// </summary>
				/// <param name="compressedData">The array of compressed data to decompress.</param>
				/// <returns>The decompressed data.</returns>
				std::vector<char> Decompress(const std::vector<char> compressedData);

				/// <summary>
				/// Compress the file.
				/// </summary>
				/// <param name="pathToDecompressedFile">The path and filename to the decompressed file which to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
				void CompressFile(const char *pathToDecompressedFile, const char *pathToCompressedFile);

				/// <summary>
				/// Decompress the file.
				/// </summary>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file which to decompress.</param>
				/// <param name="pathToDecompressedFile">The path and filename to the decompressed file.</param>
				void DecompressFile(const char *pathToCompressedFile, const char *pathToDecompressedFile);

			private:
				bool _disposed;
			};
		}
	}
}
#endif