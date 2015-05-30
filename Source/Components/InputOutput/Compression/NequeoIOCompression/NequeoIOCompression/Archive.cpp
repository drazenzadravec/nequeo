/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Archive.cpp
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

#include "stdafx.h"

#include "Archive.h"
#include "LibraryLoader.h"

#include <fstream>
#include <iostream>
#include <sstream>

#include <sevenzip\7zpp.h>

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Provides methods and properties used to compress and decompress archives.
			/// </summary>
			/// <param name="sevenZipLibraryPath">The path and file name (if not in the same directory) of the seven zip library.</param>
			Archive::Archive(const Nequeo::TFormatString& sevenZipLibraryPath) : _disposed(false)
			{
				_sevenZipLibraryPath = sevenZipLibraryPath;
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			Archive::~Archive()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;
				}
			}

			/// <summary>
			/// Extract to directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to decompress to.</param>
			/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			void Archive::Extract(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile, CompressionFormatType compressionFormatType)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Extract the compressed archive to the directory.
				SevenZip::SevenZipExtractor extractor(library, pathToCompressedFile);
				extractor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				extractor.ExtractArchive(pathToDirectory);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Extract to directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to decompress to.</param>
			/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			/// <param name="password">The password used for decompression.</param>
			void Archive::Extract(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType, const wchar_t *password)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Extract the compressed archive to the directory.
				SevenZip::SevenZipExtractor extractor(library, pathToCompressedFile);
				extractor.SetPassword(password);
				extractor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				extractor.ExtractArchive(pathToDirectory);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Create from directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			void Archive::Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile, CompressionFormatType compressionFormatType)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressDirectory(pathToDirectory);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Create from directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			/// <param name="password">The password used for compression.</param>
			void Archive::Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType, const wchar_t *password)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetPassword(password);
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressDirectory(pathToDirectory);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Create from directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			/// <param name="compressionLevelType">The compression level type.</param>
			void Archive::Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetCompressionLevel(libraryLoader.GetSevenZipCompressionLevelType(compressionLevelType));
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressDirectory(pathToDirectory);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Create from directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			/// <param name="compressionLevelType">The compression level type.</param>
			/// <param name="password">The password used for compression.</param>
			void Archive::Create(const Nequeo::TFormatString &pathToDirectory, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType, const wchar_t *password)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetPassword(password);
				compressor.SetCompressionLevel(libraryLoader.GetSevenZipCompressionLevelType(compressionLevelType));
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressDirectory(pathToDirectory);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Compress a single file.
			/// </summary>
			/// <param name="pathToFile">The path and filename of the file to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			void Archive::CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressFile(pathToFile);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Compress a single file.
			/// </summary>
			/// <param name="pathToFile">The path and filename of the file to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			/// <param name="password">The password used for compression.</param>
			void Archive::CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType, const wchar_t *password)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetPassword(password);
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressFile(pathToFile);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Compress a single file.
			/// </summary>
			/// <param name="pathToFile">The path and filename of the file to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			/// <param name="compressionLevelType">The compression level type.</param>
			void Archive::CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetCompressionLevel(libraryLoader.GetSevenZipCompressionLevelType(compressionLevelType));
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressFile(pathToFile);

				// Upload the library.
				library.Free();
			}

			/// <summary>
			/// Compress a single file.
			/// </summary>
			/// <param name="pathToFile">The path and filename of the file to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file to create.</param>
			/// <param name="compressionFormatType">The compression format type.</param>
			/// <param name="compressionLevelType">The compression level type.</param>
			/// <param name="password">The password used for compression.</param>
			void Archive::CompressFile(const Nequeo::TFormatString &pathToFile, const Nequeo::TFormatString &pathToCompressedFile,
				CompressionFormatType compressionFormatType, CompressionLevelType compressionLevelType, const wchar_t *password)
			{
				// Load the seven zip library into memory.
				LibraryLoader libraryLoader;
				SevenZip::SevenZipLibrary library = libraryLoader.LoadSevenZipLibrary(_sevenZipLibraryPath);

				// Create a compressed archive from the directory.
				SevenZip::SevenZipCompressor compressor(library, pathToCompressedFile);
				compressor.SetPassword(password);
				compressor.SetCompressionLevel(libraryLoader.GetSevenZipCompressionLevelType(compressionLevelType));
				compressor.SetCompressionFormat(libraryLoader.GetSevenZipCompressionFormatType(compressionFormatType));
				compressor.CompressFile(pathToFile);

				// Upload the library.
				library.Free();
			}
		}
	}
}