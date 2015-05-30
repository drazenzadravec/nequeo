/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          BZip2Stream.cpp
*  Purpose :       BZip2Stream class.
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

#include "BZip2Stream.h"

#include <fstream>
#include <iostream>
#include <sstream>

#include <bzlib.h>

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Constructor for the current class.
			/// </summary>
			BZip2Stream::BZip2Stream() : _disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			BZip2Stream::~BZip2Stream()
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
			std::vector<char> BZip2Stream::Compress(const std::vector<char> decompressedData)
			{
				int totalRead = 0;
				bool isCompressing = true;

				// compressed data.
				std::vector<char> compressedData = std::vector<char>();
				std::vector<char> decompressedDataInt = std::vector<char>(decompressedData);

				// Get the array of data.
				Nequeo::IO::VectorBuffer<char> vectorInBuf(decompressedDataInt);
				Nequeo::IO::VectorBuffer<char> vectorOutBuf(compressedData);

				// Create the input and out streams.
				std::istream inStream(&vectorInBuf);
				std::ostream outStream(&vectorOutBuf);

				bz_stream bzStream;
				int blockSize = 9;
				int workFactor = 30;

				char writeBuffer[1024];
				char readBuffer[1024];
				unsigned int writeBufferSize = sizeof(writeBuffer);
				unsigned int readBufferSize = sizeof(readBuffer);

				// Create the memory.
				ZeroMemory(&bzStream, sizeof(bzStream));

				// Initialise the compress stream.
				BZ2_bzCompressInit(&bzStream, blockSize, 0, workFactor);
				
				bzStream.next_out = writeBuffer;
				bzStream.avail_out = writeBufferSize;

				// Read then write.
				while (isCompressing)
				{
					// If data exists.
					if (bzStream.avail_in == 0)
					{
						// Read the next data.
						char* buffer = bzStream.next_in;

						// If the data size is the same or less
						// than the buffer size.
						if (decompressedData.size() <= 1024)
						{
							// Set the read buffer size.
							readBufferSize = decompressedData.size();
						}
						else
						{
							// Number of bytes left to read.
							int leftToRead = decompressedData.size() - totalRead;

							// If last set of data to read.
							if (leftToRead <= 1024)
							{
								// Set the read buffer size.
								readBufferSize = leftToRead;
							}
						}

						// Read the next set of data.
						inStream.read(buffer = readBuffer, readBufferSize);

						// Get the number of bytes read.
						unsigned int bytesRead = (unsigned int)inStream.gcount();
						totalRead += bytesRead;

						// If data.
						if ((bzStream.avail_in = bytesRead) == 0)
						{
							// No more data left to read
							int nError;
							while (isCompressing)
							{
								nError = BZ2_bzCompress(&bzStream, BZ_FINISH);
								switch (nError)
								{
								case BZ_FINISH_OK:
								case BZ_STREAM_END:
									outStream.write(writeBuffer, writeBufferSize - bzStream.avail_out);
									break;
								default:
									isCompressing = false;
									break;
								}

								// If not complete.
								if (isCompressing)
								{
									if (nError == BZ_STREAM_END)
										isCompressing = false;

									// If not complete.
									if (isCompressing)
									{
										bzStream.next_out = writeBuffer;
										bzStream.avail_out = writeBufferSize;
									}
								}
							}
						}
					}

					// If not complete.
					if (isCompressing)
					{
						if (BZ2_bzCompress(&bzStream, BZ_RUN) != BZ_RUN_OK)
							isCompressing = false;

						// If not complete.
						if (isCompressing)
						{
							if (bzStream.avail_out == 0)
							{
								// Flush data
								outStream.write(writeBuffer, writeBufferSize);
								bzStream.next_out = writeBuffer;
								bzStream.avail_out = writeBufferSize;
							}
						}
					}
				}

				// End the compress stream.
				BZ2_bzCompressEnd(&bzStream);

				// compression data.
				return compressedData;
			}

			/// <summary>
			/// Decompress the array of data.
			/// </summary>
			/// <param name="compressedData">The array of compressed data to decompress.</param>
			/// <returns>The decompressed data.</returns>
			std::vector<char> BZip2Stream::Decompress(const std::vector<char> compressedData)
			{
				int totalRead = 0;
				bool isCompressing = true;

				// decompressed data.
				std::vector<char> decompressedData = std::vector<char>();
				std::vector<char> compressedDataInt = std::vector<char>(compressedData);

				// Get the array of data.
				Nequeo::IO::VectorBuffer<char> vectorInBuf(compressedDataInt);
				Nequeo::IO::VectorBuffer<char> vectorOutBuf(decompressedData);

				// Create the input and out streams.
				std::istream inStream(&vectorInBuf);
				std::ostream outStream(&vectorOutBuf);

				bz_stream bzStream;
				int small = false;

				char writeBuffer[1024];
				char readBuffer[1024];
				unsigned int writeBufferSize = sizeof(writeBuffer);
				unsigned int readBufferSize = sizeof(readBuffer);

				// Create the memory.
				ZeroMemory(&bzStream, sizeof(bzStream));

				// Initialise the compress stream.
				BZ2_bzDecompressInit(&bzStream, 0, small);

				bzStream.next_out = writeBuffer;
				bzStream.avail_out = writeBufferSize;

				// Read then write.
				while (isCompressing)
				{
					// If data exists.
					if (bzStream.avail_in == 0)
					{
						// Read the next data.
						char* buffer = bzStream.next_in;

						// If the data size is the same or less
						// than the buffer size.
						if (compressedData.size() <= 1024)
						{
							// Set the read buffer size.
							readBufferSize = compressedData.size();
						}
						else
						{
							// Number of bytes left to read.
							int leftToRead = compressedData.size() - totalRead;

							// If last set of data to read.
							if (leftToRead <= 1024)
							{
								// Set the read buffer size.
								readBufferSize = leftToRead;
							}
						}

						// Read the next set of data.
						inStream.read(buffer = readBuffer, readBufferSize);

						// Get the number of bytes read.
						unsigned int bytesRead = (unsigned int)inStream.gcount();
						totalRead += bytesRead;

						// If data.
						if ((bzStream.avail_in = bytesRead) == 0)
						{
							// No more data left to read
							int nError;
							while (isCompressing)
							{
								nError = BZ2_bzDecompress(&bzStream);
								switch (nError)
								{
								case BZ_OK:
								case BZ_STREAM_END:
									outStream.write(writeBuffer, writeBufferSize - bzStream.avail_out);
									break;
								default:
									isCompressing = false;
									break;
								}

								// If not complete.
								if (isCompressing)
								{
									if (nError == BZ_STREAM_END)
										isCompressing = false;

									// If not complete.
									if (isCompressing)
									{
										bzStream.next_out = writeBuffer;
										bzStream.avail_out = writeBufferSize;
									}
								}
							}
						}
					}

					// If not complete.
					if (isCompressing)
					{
						switch (BZ2_bzDecompress(&bzStream))
						{
						case BZ_OK:
							outStream.write(writeBuffer, writeBufferSize);
							bzStream.next_out = writeBuffer;
							bzStream.avail_out = writeBufferSize;
							break;
						case BZ_STREAM_END:
							outStream.write(writeBuffer, writeBufferSize - bzStream.avail_out);
							isCompressing = false;
							break;
						default:
							isCompressing = false;
							break;
						}
					}
				}

				// End the compress stream.
				BZ2_bzDecompressEnd(&bzStream);

				// decompression data.
				return decompressedData;
			}

			/// <summary>
			/// Compress the file.
			/// </summary>
			/// <param name="pathToDecompressedFile">The path and filename to the decompressed file which to compress.</param>
			/// <param name="pathToCompressedFile">The path and filename to the compressed file.</param>
			void BZip2Stream::CompressFile(const char *pathToDecompressedFile, const char *pathToCompressedFile)
			{
				bool isCompressing = true;

				bz_stream bzStream;
				int blockSize = 9;
				int workFactor = 30;

				char writeBuffer[16384];
				char readBuffer[16384];
				unsigned int writeBufferSize = sizeof(writeBuffer);
				unsigned int readBufferSize = sizeof(readBuffer);

				// Create the memory.
				ZeroMemory(&bzStream, sizeof(bzStream));

				// Initialise the compress stream.
				BZ2_bzCompressInit(&bzStream, blockSize, 0, workFactor);

				// Compress the file.
				// Create the input and out file streams.
				std::ifstream inStream(pathToDecompressedFile, std::ios_base::in | ios_base::binary);
				std::ofstream outStream(pathToCompressedFile, std::ios_base::out | ios_base::binary);

				bzStream.next_out = writeBuffer;
				bzStream.avail_out = writeBufferSize;

				// Read then write.
				while (isCompressing)
				{
					// If data exists.
					if (bzStream.avail_in == 0)
					{
						// Read the next data.
						char* buffer = bzStream.next_in;
						inStream.read(buffer = readBuffer, readBufferSize);

						// Get the number of bytes read.
						unsigned int bytesRead = (unsigned int)inStream.gcount();

						// If data.
						if ((bzStream.avail_in = bytesRead) == 0)
						{	
							// No more data left to read
							int nError;
							while (isCompressing)
							{
								nError = BZ2_bzCompress(&bzStream, BZ_FINISH);
								switch (nError)
								{
								case BZ_FINISH_OK:
								case BZ_STREAM_END:
									outStream.write(writeBuffer, writeBufferSize - bzStream.avail_out);
									break;
								default:
									isCompressing = false;
									break;
								}

								// If not complete.
								if (isCompressing)
								{
									if (nError == BZ_STREAM_END)
										isCompressing = false;

									// If not complete.
									if (isCompressing)
									{
										bzStream.next_out = writeBuffer;
										bzStream.avail_out = writeBufferSize;
									}
								}
							}
						}
					}

					// If not complete.
					if (isCompressing)
					{
						if (BZ2_bzCompress(&bzStream, BZ_RUN) != BZ_RUN_OK)
							isCompressing = false;

						// If not complete.
						if (isCompressing)
						{
							if (bzStream.avail_out == 0)
							{
								// Flush data
								outStream.write(writeBuffer, writeBufferSize);
								bzStream.next_out = writeBuffer;
								bzStream.avail_out = writeBufferSize;
							}
						}
					}
				}

				inStream.close();
				outStream.close();

				// End the compress stream.
				BZ2_bzCompressEnd(&bzStream);
			}

			/// <summary>
			/// Decompress the file.
			/// </summary>
			/// <param name="pathToCompressedFile">The path and filename of the compressed file which to decompress.</param>
			/// <param name="pathToDecompressedFile">The path and filename to the decompressed file.</param>
			void BZip2Stream::DecompressFile(const char *pathToCompressedFile, const char *pathToDecompressedFile)
			{
				bool isCompressing = true;

				bz_stream bzStream;
				int small = false;

				char writeBuffer[16384];
				char readBuffer[16384];
				unsigned int writeBufferSize = sizeof(writeBuffer);
				unsigned int readBufferSize = sizeof(readBuffer);

				// Create the memory.
				ZeroMemory(&bzStream, sizeof(bzStream));

				// Initialise the compress stream.
				BZ2_bzDecompressInit(&bzStream, 0, small);

				// Decompress the file.
				// Create the input and out file streams.
				std::ifstream inStream(pathToCompressedFile, std::ios_base::in | ios_base::binary);
				std::ofstream outStream(pathToDecompressedFile, std::ios_base::out | ios_base::binary);

				bzStream.next_out = writeBuffer;
				bzStream.avail_out = writeBufferSize;

				// Read then write.
				while (isCompressing)
				{
					// If data exists.
					if (bzStream.avail_in == 0)
					{
						// Read the next data.
						char* buffer = bzStream.next_in;
						inStream.read(buffer = readBuffer, readBufferSize);

						// Get the number of bytes read.
						unsigned int bytesRead = (unsigned int)inStream.gcount();

						// If data.
						if ((bzStream.avail_in = bytesRead) == 0)
						{
							// No more data left to read
							int nError;
							while (isCompressing)
							{
								nError = BZ2_bzDecompress(&bzStream);
								switch (nError)
								{
								case BZ_OK:
								case BZ_STREAM_END:
									outStream.write(writeBuffer, writeBufferSize - bzStream.avail_out);
									break;
								default:
									isCompressing = false;
									break;
								}

								// If not complete.
								if (isCompressing)
								{
									if (nError == BZ_STREAM_END)
										isCompressing = false;

									// If not complete.
									if (isCompressing)
									{
										bzStream.next_out = writeBuffer;
										bzStream.avail_out = writeBufferSize;
									}
								}
							}
						}
					}

					// If not complete.
					if (isCompressing)
					{
						switch (BZ2_bzDecompress(&bzStream))
						{
						case BZ_OK:
							outStream.write(writeBuffer, writeBufferSize);
							bzStream.next_out = writeBuffer;
							bzStream.avail_out = writeBufferSize;
							break;
						case BZ_STREAM_END:
							outStream.write(writeBuffer, writeBufferSize - bzStream.avail_out);
							isCompressing = false;
							break;
						default:
							isCompressing = false;
							break;
						}
					}
				}

				inStream.close();
				outStream.close();

				// End the compress stream.
				BZ2_bzDecompressEnd(&bzStream);
			}
		}
	}
}