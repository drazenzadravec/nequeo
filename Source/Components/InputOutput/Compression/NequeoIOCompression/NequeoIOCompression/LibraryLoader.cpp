/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          LibraryLoader.cpp
*  Purpose :       LibraryLoader class.
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

#include "LibraryLoader.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Provides methods and properties used to compress and decompress archives.
			/// </summary>
			LibraryLoader::LibraryLoader() : _disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			LibraryLoader::~LibraryLoader()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;
				}
			}

			/// <summary>
			/// Load the seven zip library.
			/// </summary>
			/// <param name="sevenZipLibraryPath">The path and file name (if not in the same directory) of the seven zip library.</param>
			/// <returns>The seven zip library handler.</returns>
			SevenZip::SevenZipLibrary LibraryLoader::LoadSevenZipLibrary(const Nequeo::TFormatString& sevenZipLibraryPath)
			{
				// Load the seven zip library.
				SevenZip::SevenZipLibrary sevenZipLibrary;
				sevenZipLibrary.Load(sevenZipLibraryPath);

				// Return the library handler.
				return sevenZipLibrary;
			}

			/// <summary>
			/// Get the seven zip compression format type.
			/// </summary>
			/// <param name="compressionFormat">The compression format type..</param>
			/// <returns>The seven zip compression format.</returns>
			SevenZip::CompressionFormat::_Enum LibraryLoader::GetSevenZipCompressionFormatType(CompressionFormatType compressionFormat)
			{
				switch (compressionFormat)
				{
				case Nequeo::IO::Compression::Unknown:
					return SevenZip::CompressionFormat::Unknown;
				case Nequeo::IO::Compression::SevenZip:
					return SevenZip::CompressionFormat::SevenZip;
				case Nequeo::IO::Compression::Zip:
					return SevenZip::CompressionFormat::Zip;
				case Nequeo::IO::Compression::GZip:
					return SevenZip::CompressionFormat::GZip;
				case Nequeo::IO::Compression::BZip2:
					return SevenZip::CompressionFormat::BZip2;
				case Nequeo::IO::Compression::Rar:
					return SevenZip::CompressionFormat::Rar;
				case Nequeo::IO::Compression::Tar:
					return SevenZip::CompressionFormat::Tar;
				case Nequeo::IO::Compression::Iso:
					return SevenZip::CompressionFormat::Iso;
				case Nequeo::IO::Compression::Cab:
					return SevenZip::CompressionFormat::Cab;
				case Nequeo::IO::Compression::Lzma:
					return SevenZip::CompressionFormat::Lzma;
				case Nequeo::IO::Compression::Lzma86:
					return SevenZip::CompressionFormat::Lzma86;
				default:
					return SevenZip::CompressionFormat::SevenZip;
				}
			}

			/// <summary>
			/// Get the seven zip compression level type.
			/// </summary>
			/// <param name="compressionLevel">The compression level type.</param>
			/// <returns>The seven zip compression level.</returns>
			SevenZip::CompressionLevel::_Enum LibraryLoader::GetSevenZipCompressionLevelType(CompressionLevelType compressionLevel)
			{
				switch (compressionLevel)
				{
				case Nequeo::IO::Compression::None:
					return SevenZip::CompressionLevel::None;
				case Nequeo::IO::Compression::Fastest:
					return SevenZip::CompressionLevel::Fastest;
				case Nequeo::IO::Compression::Fast:
					return SevenZip::CompressionLevel::Fast;
				case Nequeo::IO::Compression::Normal:
					return SevenZip::CompressionLevel::Normal;
				case Nequeo::IO::Compression::Maximum:
					return SevenZip::CompressionLevel::Maximum;
				case Nequeo::IO::Compression::Ultra:
					return SevenZip::CompressionLevel::Ultra;
				default:
					return SevenZip::CompressionLevel::Normal;
				}
			}
		}
	}
}
