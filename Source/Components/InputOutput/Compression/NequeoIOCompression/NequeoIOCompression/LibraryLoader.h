/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          LibraryLoader.h
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

#pragma once

#ifndef _LIBRARYLOADER_H
#define _LIBRARYLOADER_H

#include "GlobalIOCompression.h"
#include "Archive.h"
#include "Base\Global.h"

#include <sevenzip\7zpp.h>

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Provides methods to load libraries.
			/// </summary>
			class LibraryLoader
			{
			public:
				/// <summary>
				/// Provides methods and properties used to compress and decompress archives.
				/// </summary>
				LibraryLoader();
				~LibraryLoader();

				/// <summary>
				/// Load the seven zip library.
				/// </summary>
				/// <param name="sevenZipLibraryPath">The path and file name (if not in the same directory) of the seven zip library.</param>
				/// <returns>The seven zip library handler.</returns>
				SevenZip::SevenZipLibrary LoadSevenZipLibrary(const Nequeo::TFormatString& sevenZipLibraryPath = _T("7z.dll"));

				/// <summary>
				/// Get the seven zip compression format type.
				/// </summary>
				/// <param name="compressionFormat">The compression format type.</param>
				/// <returns>The seven zip compression format.</returns>
				SevenZip::CompressionFormat::_Enum GetSevenZipCompressionFormatType(CompressionFormatType compressionFormat);

				/// <summary>
				/// Get the seven zip compression level type.
				/// </summary>
				/// <param name="compressionLevel">The compression level type.</param>
				/// <returns>The seven zip compression level.</returns>
				SevenZip::CompressionLevel::_Enum GetSevenZipCompressionLevelType(CompressionLevelType compressionLevel);

			private:
				bool _disposed;
	
			};
		}
	}
}
#endif

