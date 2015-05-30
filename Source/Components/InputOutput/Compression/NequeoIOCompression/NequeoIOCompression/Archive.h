/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Archive.h
*  Purpose :       Archive class.
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

#ifndef _ARCHIVE_H
#define _ARCHIVE_H

#include "GlobalIOCompression.h"
#include "Base\Global.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// The compression level type.
			/// </summary>
			enum CompressionLevelType
			{
				/// <summary>
				/// None.
				/// </summary>
				None,
				/// <summary>
				/// Fastest.
				/// </summary>
				Fastest,
				/// <summary>
				/// Fast.
				/// </summary>
				Fast,
				/// <summary>
				/// Normal.
				/// </summary>
				Normal,
				/// <summary>
				/// Maximum.
				/// </summary>
				Maximum,
				/// <summary>
				/// Ultra.
				/// </summary>
				Ultra,
			};

			/// <summary>
			/// The compression format type.
			/// </summary>
			enum CompressionFormatType
			{
				/// <summary>
				/// Unknown.
				/// </summary>
				Unknown,
				/// <summary>
				/// SevenZip.
				/// </summary>
				SevenZip,
				/// <summary>
				/// Zip.
				/// </summary>
				Zip,
				/// <summary>
				/// GZip.
				/// </summary>
				GZip,
				/// <summary>
				/// BZip2.
				/// </summary>
				BZip2,
				/// <summary>
				/// Rar.
				/// </summary>
				Rar,
				/// <summary>
				/// Tar.
				/// </summary>
				Tar,
				/// <summary>
				/// Iso.
				/// </summary>
				Iso,
				/// <summary>
				/// Cab.
				/// </summary>
				Cab,
				/// <summary>
				/// Lzma.
				/// </summary>
				Lzma,
				/// <summary>
				/// Lzma86.
				/// </summary>
				Lzma86,
			};

			/// <summary>
			/// Provides methods and properties used to compress and decompress archives.
			/// </summary>
			class Archive
			{
			public:
				/// <summary>
				/// Provides methods and properties used to compress and decompress archives.
				/// </summary>
				/// <param name="sevenZipLibraryPath">The path and file name (if not in the same directory) of the seven zip library.</param>
				Archive(const Nequeo::TFormatString& sevenZipLibraryPath = _T("7z.dll"));
				~Archive();

				/// <summary>
				/// Extract to directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to decompress to.</param>
				/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				void Extract(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile, 
					CompressionFormatType compressionFormatType);

				/// <summary>
				/// Extract to directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to decompress to.</param>
				/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				/// <param name="password">The password used for decompression.</param>
				void Extract(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile, 
					CompressionFormatType compressionFormatType, const wchar_t *password);

				/// <summary>
				/// Create from directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				void Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile,
					CompressionFormatType compressionFormatType);

				/// <summary>
				/// Create from directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				/// <param name="password">The password used for compression.</param>
				void Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile,
					CompressionFormatType compressionFormatType, const wchar_t *password);

				/// <summary>
				/// Create from directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				/// <param name="compressionLevelType">The compression level type.</param>
				void Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile, 
					CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType);

				/// <summary>
				/// Create from directory.
				/// </summary>
				/// <param name="pathToDirectory">The path to the directory to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				/// <param name="compressionLevelType">The compression level type.</param>
				/// <param name="password">The password used for compression.</param>
				void Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile, 
					CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType, const wchar_t *password);

				/// <summary>
				/// Compress a single file.
				/// </summary>
				/// <param name="pathToFile">The path and filename of the file to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				void CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
					CompressionFormatType compressionFormatType);

				/// <summary>
				/// Compress a single file.
				/// </summary>
				/// <param name="pathToFile">The path and filename of the file to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				/// <param name="password">The password used for compression.</param>
				void CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
					CompressionFormatType compressionFormatType, const wchar_t *password);

				/// <summary>
				/// Compress a single file.
				/// </summary>
				/// <param name="pathToFile">The path and filename of the file to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				/// <param name="compressionLevelType">The compression level type.</param>
				void CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
					CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType);

				/// <summary>
				/// Compress a single file.
				/// </summary>
				/// <param name="pathToFile">The path and filename of the file to compress.</param>
				/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
				/// <param name="compressionFormatType">The compression format type.</param>
				/// <param name="compressionLevelType">The compression level type.</param>
				/// <param name="password">The password used for compression.</param>
				void CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
					CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType, const wchar_t *password);

			private:
				bool _disposed;
				Nequeo::TFormatString _sevenZipLibraryPath;

			};
		}
	}
}
#endif