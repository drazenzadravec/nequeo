/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ZipStream.cpp
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

#include "stdafx.h"

#include "ZipStream.h"
#include <Poco\Zip\ZipStream.h>
#include <Poco\Zip\Compress.h>
#include <Poco\Zip\Decompress.h>
#include <Poco\Zip\ZipManipulator.h>
#include <Poco\File.h>
#include <Poco\FileStream.h>
#include <Poco\StreamCopier.h>

#include <fstream>
#include <sstream>

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			ZipStream::ZipStream() : _disposed(false)
			{

			}

			/// <summary>
			/// This destructor.
			/// </summary>
			ZipStream::~ZipStream()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;
				}
			}

			/// <summary>
			/// Compress the file.
			/// </summary>
			/// <param name="pathToDecompressedFile">The path and filename to the decompressed file which to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
			void ZipStream::CompressFile(const char *pathToDecompressedFile, const char *pathToCompressedFile)
			{
				// open the output file stream.
				std::ofstream outputFile(pathToCompressedFile, std::ios::binary);

				// get the path to the input file.
				Poco::Path inputFile(pathToDecompressedFile);

				// compress the file the the output stream.
				Poco::Zip::Compress compress(outputFile, true);
				compress.addFile(inputFile, inputFile.getFileName());
				Poco::Zip::ZipArchive archive(compress.close());
			}

			/// <summary>
			/// Decompress a single file within the archive.
			/// </summary>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file which to decompress.</param>
			/// <param name="pathToDecompressedFile">The path and filename to the decompressed file.</param>
			/// <param name="compressedFileName">The name of the file within the archive to decompress.</param>
			void ZipStream::DecompressFile(const char *pathToCompressedFile, const char *pathToDecompressedFile, const char *compressedFileName)
			{
				// open the input file stream.
				std::ifstream inputFile(pathToCompressedFile, std::ios::binary);

				// get the archive and add the compressed file.
				Poco::Zip::ZipArchive archive(inputFile);

				// find the file within the archive.
				Poco::Zip::ZipArchive::FileHeaders::const_iterator it = archive.findHeader(std::string(compressedFileName));

				// decompress the file to the stream.
				Poco::Zip::ZipInputStream zipin(inputFile, it->second);
				std::ofstream outputFile(pathToDecompressedFile, std::ios::binary);
				Poco::StreamCopier::copyStream(zipin, outputFile);
			}

			/// <summary>
			/// Compress the directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
			void ZipStream::CompressDirectory(const char *pathToDirectory, const char *pathToCompressedFile)
			{
				// open the output file stream.
				std::ofstream outputFile(pathToCompressedFile, std::ios::binary);

				// create the archive path.
				Poco::Path files(pathToDirectory);

				// compress the directory.
				Poco::Zip::Compress compress(outputFile, true);
				compress.addRecursive(files, Poco::Zip::ZipCommon::CL_MAXIMUM, true, files);
				Poco::Zip::ZipArchive archive(compress.close());
			}

			/// <summary>
			/// Deompress to directory.
			/// </summary>
			/// <param name="pathToDirectory">The path to the directory to decompress to.</param>
			/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
			void ZipStream::DecompressFiles(const char *pathToDirectory, const char *pathToCompressedFile)
			{
				// open the input file stream.
				std::ifstream inputFile(pathToCompressedFile, std::ios::binary);

				// decompress the file to the directory.
				Poco::Zip::Decompress decompress(inputFile, Poco::Path(pathToDirectory));
				decompress.decompressAllFiles();
			}
		}
	}
}