/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ZipStream.h
*  Purpose :       ZipStream class.
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

#ifndef _ZIPSTREAM_H
#define _ZIPSTREAM_H

#include "GlobalIOCompression.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Provides methods and properties used to compress and decompress streams.
			/// </summary>
			class ZipStream
			{
			public:
				ZipStream();
				~ZipStream();

				/// <summary>
				/// Compress the file.
				/// </summary>
				/// <param name="pathToDecompressedFile">The path and filename to the decompressed file which to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
				void CompressFile(const char *pathToDecompressedFile, const char *pathToCompressedFile);

				/// <summary>
				/// Decompress a single file within the archive.
				/// </summary>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file which to decompress.</param>
				/// <param name ="pathToDecompressedFile">The path and filename to the decompressed file.</param>
				/// <param name="compressedFileName">The name of the file within the archive to decompress.</param>
				void DecompressFile(const char *pathToCompressedFile, const char *pathToDecompressedFile, const char *compressedFileName);

				/// <summary>
				/// Compress the directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
				void CompressDirectory(const char *pathToDirectory, const char *pathToCompressedFile);

				/// <summary>
				/// Deompress to directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to decompress to.</param>
				/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
				void DecompressFiles(const char *pathToDirectory, const char *pathToCompressedFile);

			private:
				bool _disposed;
			};
		}
	}
}
#endif