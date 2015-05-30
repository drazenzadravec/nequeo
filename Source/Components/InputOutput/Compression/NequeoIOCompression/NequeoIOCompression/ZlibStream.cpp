/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ZlibStream.cpp
*  Purpose :       ZlibStream class.
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

#include "ZlibStream.h"

#include <fstream>
#include <iostream>
#include <boost/iostreams/filtering_streambuf.hpp>
#include <boost/iostreams/filtering_stream.hpp>
#include <boost/iostreams/copy.hpp>
#include <boost/iostreams/filter/zlib.hpp>
#include <boost/iostreams/filter/gzip.hpp>

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			ZlibStream::ZlibStream() : _disposed(false)
			{

			}

			/// <summary>
			/// This destructor.
			/// </summary>
			ZlibStream::~ZlibStream()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;
				}
			}

			/// <summary>
			/// Compress the array of data.
			/// </summary>
			/// <param name="decompressedData">The array of decompressed data to compress.</param>
			/// <returns>The compressed data.</returns>
			std::vector<char> ZlibStream::Compress(const std::vector<char> decompressedData)
			{
				// compressed data.
				std::vector<char> compressedData = std::vector<char>();

				// use the out filter stream.
				boost::iostreams::filtering_ostream os;

				// use the compressor.
				os.push(boost::iostreams::zlib_compressor());
				os.push(boost::iostreams::back_inserter(compressedData));

				// write the compressed data.
				boost::iostreams::write(os, &decompressedData[0], decompressedData.size());

				// compression data.
				return compressedData;
			}

			/// <summary>
			/// Decompress the array of data.
			/// </summary>
			/// <param name="compressedData">The array of compressed data to decompress.</param>
			/// <returns>The decompressed data.</returns>
			std::vector<char> ZlibStream::Decompress(const std::vector<char> compressedData)
			{
				// decompressed data.
				std::vector<char> decompressedData = std::vector<char>();

				// use the out filter stream.
				boost::iostreams::filtering_ostream os;

				// use the decompressor.
				os.push(boost::iostreams::zlib_decompressor());
				os.push(boost::iostreams::back_inserter(decompressedData));

				// write the decompressed data.
				boost::iostreams::write(os, &compressedData[0], compressedData.size());

				// decompression data.
				return decompressedData;
			}

			/// <summary>
			/// Compress the file.
			/// </summary>
			/// <param name="pathToDecompressedFile">The path and filename to the decompressed file which to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
			void ZlibStream::CompressFile(const char *pathToDecompressedFile, const char *pathToCompressedFile)
			{
				// create the input and out file streams.
				std::ifstream inStream(pathToDecompressedFile, std::ios_base::in | ios_base::binary);
				std::ofstream outStream(pathToCompressedFile, std::ios_base::out | ios_base::binary);

				// create the compressor buffer stream.
				boost::iostreams::filtering_streambuf< boost::iostreams::input> in;
				in.push(boost::iostreams::zlib_compressor());
				in.push(inStream);
				boost::iostreams::copy(in, outStream);
			}

			/// <summary>
			/// Decompress the file.
			/// </summary>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file which to decompress.</param>
			/// <param name="pathToDecompressedFile">The path and filename to the decompressed file.</param>
			void ZlibStream::DecompressFile(const char *pathToCompressedFile, const char *pathToDecompressedFile)
			{
				// create the input and out file streams.
				std::ifstream inStream(pathToCompressedFile, std::ios_base::in | ios_base::binary);
				std::ofstream outStream(pathToDecompressedFile, std::ios_base::out | ios_base::binary);

				// create the compressor buffer stream.
				boost::iostreams::filtering_streambuf< boost::iostreams::input> in;
				in.push(boost::iostreams::zlib_decompressor());
				in.push(inStream);
				boost::iostreams::copy(in, outStream);
			}
		}
	}
}