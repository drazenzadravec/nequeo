/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Huffman.cpp
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

#include "stdafx.h"

#include "Huffman.h"

namespace Nequeo {
	namespace IO {
		namespace Compression
		{
			/// <summary>
			/// Provides methods and properties used to compress and decompress archives.
			/// </summary>
			Huffman::Huffman() : _disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			Huffman::~Huffman()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;
				}
			}

			/// <summary>
			/// Compress the source file.
			/// </summary>
			/// <param name="sourceFileName">The path and filename of the file to compress.</param>
			/// <param name="destinationFileName">The path and filename of the file where the compressed data will be stored.</param>
			/// <returns>True if compressed; else false.</returns>
			bool Huffman::CompressFile(LPCWSTR sourceFileName, LPCWSTR destinationFileName)
			{
				COMPRESSOR_HANDLE Compressor = NULL;
				PBYTE CompressedBuffer = NULL;
				PBYTE InputBuffer = NULL;
				HANDLE InputFile = INVALID_HANDLE_VALUE;
				HANDLE CompressedFile = INVALID_HANDLE_VALUE;
				BOOL DeleteTargetFile = TRUE;
				BOOL Success;
				SIZE_T CompressedDataSize, CompressedBufferSize;
				DWORD InputFileSize, ByteRead, ByteWritten;
				LARGE_INTEGER FileSize;
				ULONGLONG StartTime, EndTime;
				double TimeDuration;
				bool ret = false;

				// Open input file for reading, existing file only. The file to compress.
				InputFile = CreateFile(
					sourceFileName,           //  Input file name
					GENERIC_READ,             //  Open for reading
					FILE_SHARE_READ,          //  Share for read
					NULL,                     //  Default security
					OPEN_EXISTING,            //  Existing file only
					FILE_ATTRIBUTE_NORMAL,    //  Normal file
					NULL);                    //  No attr. template

				// If the file exists.
				if (InputFile == INVALID_HANDLE_VALUE)
				{
					// Cannot open sourceFileName.
					ret = false;
					goto done;
				}

				// Get input file size.
				Success = GetFileSizeEx(InputFile, &FileSize);
				if ((!Success) || (FileSize.QuadPart > 0xFFFFFFFF))
				{
					// Cannot get input file size or file is larger than 4GB.
					ret = false;
					goto done;
				}

				// Get the file size.
				InputFileSize = FileSize.LowPart;

				// Allocate memory for file content.
				InputBuffer = (PBYTE)malloc(InputFileSize);
				if (!InputBuffer)
				{
					// Cannot allocate memory for uncompressed buffer.
					ret = false;
					goto done;
				}

				// Read input file.
				Success = ReadFile(InputFile, InputBuffer, InputFileSize, &ByteRead, NULL);
				if ((!Success) || (ByteRead != InputFileSize))
				{
					// Cannot read from sourceFileName.
					ret = false;
					goto done;
				}

				// Open an empty file for writing, if exist, overwrite it.
				CompressedFile = CreateFile(
					destinationFileName,      //  Compressed file name
					GENERIC_WRITE | DELETE,   //  Open for writing; delete if cannot compress
					0,                        //  Do not share
					NULL,                     //  Default security
					CREATE_ALWAYS,            //  Create a new file; if exist, overwrite it
					FILE_ATTRIBUTE_NORMAL,    //  Normal file
					NULL);

				// Could not create the compressed file.
				if (CompressedFile == INVALID_HANDLE_VALUE)
				{
					// Cannot create file destinationFileName.
					ret = false;
					goto done;
				}

				// Create an XpressHuff compressor.
				Success = CreateCompressor(
					COMPRESS_ALGORITHM_XPRESS_HUFF, //  Compression Algorithm
					NULL,                           //  Optional allocation routine
					&Compressor);

				// If the compressor has been created.
				if (!Success)
				{
					// Cannot create a compressor %d.\n", GetLastError());
					ret = false;
					goto done;
				}

				// Query compressed buffer size.
				Success = Compress(
					Compressor,                  //  Compressor Handle
					InputBuffer,                 //  Input buffer, Uncompressed data
					InputFileSize,               //  Uncompressed data size
					NULL,                        //  Compressed Buffer
					0,                           //  Compressed Buffer size
					&CompressedBufferSize);      //  Compressed Data size

				// Allocate memory for compressed buffer.
				if (!Success)
				{
					DWORD ErrorCode = GetLastError();

					if (ErrorCode != ERROR_INSUFFICIENT_BUFFER)
					{
						// Cannot compress data: %d.\n", ErrorCode);
						ret = false;
						goto done;
					}

					CompressedBuffer = (PBYTE)malloc(CompressedBufferSize);
					if (!CompressedBuffer)
					{
						// Cannot allocate memory for compressed buffer.\n");
						ret = false;
						goto done;
					}
				}

				// Start the timer.
				StartTime = GetTickCount64();

				// Call Compress() again to do real compression and output the compressed
				// data to CompressedBuffer.
				Success = Compress(
					Compressor,             //  Compressor Handle
					InputBuffer,            //  Input buffer, Uncompressed data
					InputFileSize,          //  Uncompressed data size
					CompressedBuffer,       //  Compressed Buffer
					CompressedBufferSize,   //  Compressed Buffer size
					&CompressedDataSize);   //  Compressed Data size

				// If the compression was successful.
				if (!Success)
				{
					// Cannot compress data: %d\n", GetLastError());
					ret = false;
					goto done;
				}

				// End the timer.
				EndTime = GetTickCount64();

				//  Get compression time.
				TimeDuration = (EndTime - StartTime) / 1000.0;

				// Write compressed data to output file.
				Success = WriteFile(
					CompressedFile,     //  File handle
					CompressedBuffer,   //  Start of data to write
					CompressedDataSize, //  Number of byte to write
					&ByteWritten,       //  Number of byte written
					NULL);              //  No overlapping structure

				// If the compressed data can not be written to the destination file.
				if ((ByteWritten != CompressedDataSize) || (!Success))
				{
					// Cannot write compressed data to file: %d.\n", GetLastError());
					ret = false;
					goto done;
				}

				// Do not delete the destination file
				// because the compress was successful.
				DeleteTargetFile = FALSE;
				ret = true;

			done:
				if (Compressor != NULL)
				{
					CloseCompressor(Compressor);
				}

				if (CompressedBuffer)
				{
					free(CompressedBuffer);
				}

				if (InputBuffer)
				{
					free(InputBuffer);
				}

				if (InputFile != INVALID_HANDLE_VALUE)
				{
					CloseHandle(InputFile);
				}

				if (CompressedFile != INVALID_HANDLE_VALUE)
				{
					//  Compression fails, delete the compressed file.
					if (DeleteTargetFile)
					{
						FILE_DISPOSITION_INFO fdi;
						fdi.DeleteFile = TRUE;      //  Marking for deletion
						Success = SetFileInformationByHandle(
							CompressedFile,
							FileDispositionInfo,
							&fdi,
							sizeof(FILE_DISPOSITION_INFO));

						if (!Success) 
						{
							// Cannot delete corrupted compressed file.\n");
							ret = false;
						}
					}
					CloseHandle(CompressedFile);
				}

				// Return the result.
				return ret;
			}

			/// <summary>
			/// Decompress the source file.
			/// </summary>
			/// <param name="sourceFileName">The path and filename of the file containing the compressed data.</param>
			/// <param name="destinationFileName">The path and filename of the file where the decompressed data will be stored.</param>
			/// <returns>True if compressed; else false.</returns>
			bool Huffman::DecompressFile(LPCWSTR sourceFileName, LPCWSTR destinationFileName)
			{
				DECOMPRESSOR_HANDLE Decompressor = NULL;
				PBYTE CompressedBuffer = NULL;
				PBYTE DecompressedBuffer = NULL;
				HANDLE InputFile = INVALID_HANDLE_VALUE;
				HANDLE DecompressedFile = INVALID_HANDLE_VALUE;
				BOOL DeleteTargetFile = TRUE;
				BOOL Success;
				SIZE_T DecompressedBufferSize, DecompressedDataSize;
				DWORD InputFileSize, ByteRead, ByteWritten;
				ULONGLONG StartTime, EndTime;
				LARGE_INTEGER FileSize;
				double TimeDuration;
				bool ret = false;

				// Open input file for reading, existing file only.
				InputFile = CreateFile(
					sourceFileName,           //  Input file name, compressed file
					GENERIC_READ,             //  Open for reading
					FILE_SHARE_READ,          //  Share for read
					NULL,                     //  Default security
					OPEN_EXISTING,            //  Existing file only
					FILE_ATTRIBUTE_NORMAL,    //  Normal file
					NULL);                    //  No template

				// Can not open file.
				if (InputFile == INVALID_HANDLE_VALUE)
				{
					// Cannot open sourceFileName.
					ret = false;
					goto done;
				}

				// Get compressed file size.
				Success = GetFileSizeEx(InputFile, &FileSize);
				if ((!Success) || (FileSize.QuadPart > 0xFFFFFFFF))
				{
					// Cannot get input file size or file is larger than 4GB.
					ret = false;
					goto done;
				}

				// Get the file size.
				InputFileSize = FileSize.LowPart;

				// Allocation memory for compressed content.
				CompressedBuffer = (PBYTE)malloc(InputFileSize);
				if (!CompressedBuffer)
				{
					// Cannot allocate memory for compressed buffer.
					ret = false;
					goto done;
				}

				// Read compressed content into buffer.
				Success = ReadFile(InputFile, CompressedBuffer, InputFileSize, &ByteRead, NULL);
				if ((!Success) || (ByteRead != InputFileSize))
				{
					// Cannot read from sourceFileName.
					ret = false;
					goto done;
				}

				// Open an empty file for writing, if exist, destroy it.
				DecompressedFile = CreateFile(
					destinationFileName,      //  Decompressed file name
					GENERIC_WRITE | DELETE,   //  Open for writing
					0,                        //  Do not share
					NULL,                     //  Default security
					CREATE_ALWAYS,            //  Create a new file, if exists, overwrite it.
					FILE_ATTRIBUTE_NORMAL,    //  Normal file
					NULL);                    //  No template

				// If the decompression file can not be created.
				if (DecompressedFile == INVALID_HANDLE_VALUE)
				{
					// Cannot create file destinationFileName.
					ret = false;
					goto done;
				}

				// Create an XpressHuff decompressor.
				Success = CreateDecompressor(
					COMPRESS_ALGORITHM_XPRESS_HUFF, //  Compression Algorithm
					NULL,                           //  Optional allocation routine
					&Decompressor);                 //  Handle

				// If the decompressor was created.
				if (!Success)
				{
					// Cannot create a decompressor: %d.\n", GetLastError());
					ret = false;
					goto done;
				}

				// Query decompressed buffer size.
				Success = Decompress(
					Decompressor,                //  Compressor Handle
					CompressedBuffer,            //  Compressed data
					InputFileSize,               //  Compressed data size
					NULL,                        //  Buffer set to NULL
					0,                           //  Buffer size set to 0
					&DecompressedBufferSize);    //  Decompressed Data size

				// Allocate memory for decompressed buffer.
				if (!Success)
				{
					DWORD ErrorCode = GetLastError();

					// Note that the original size returned by the function is extracted 
					// from the buffer itself and should be treated as untrusted and tested
					// against reasonable limits.
					if (ErrorCode != ERROR_INSUFFICIENT_BUFFER)
					{
						// Cannot decompress data: %d.\n", ErrorCode);
						ret = false;
						goto done;
					}

					DecompressedBuffer = (PBYTE)malloc(DecompressedBufferSize);
					if (!DecompressedBuffer)
					{
						// Cannot allocate memory for decompressed buffer.\n");
						ret = false;
						goto done;
					}
				}

				// Start the timer.
				StartTime = GetTickCount64();

				// Decompress data and write data to DecompressedBuffer.
				Success = Decompress(
					Decompressor,               //  Decompressor handle
					CompressedBuffer,           //  Compressed data
					InputFileSize,              //  Compressed data size
					DecompressedBuffer,         //  Decompressed buffer
					DecompressedBufferSize,     //  Decompressed buffer size
					&DecompressedDataSize);     //  Decompressed data size

				// If the decompression was successful.
				if (!Success)
				{
					// Cannot decompress data: %d.\n", GetLastError());
					ret = false;
					goto done;
				}

				// End the timer.
				EndTime = GetTickCount64();

				//  Get decompression time.
				TimeDuration = (EndTime - StartTime) / 1000.0;

				// Write decompressed data to output file.
				Success = WriteFile(
					DecompressedFile,       //  File handle
					DecompressedBuffer,     //  Start of data to write
					DecompressedDataSize,   //  Number of byte to write
					&ByteWritten,           //  Number of byte written
					NULL);                  //  No overlapping structure

				// If the decompressed data was not written.
				if ((ByteWritten != DecompressedDataSize) || (!Success))
				{
					// Cannot write decompressed data to file.\n");
					ret = false;
					goto done;
				}

				// Do not delete the destination file
				// because the decompress was successful.
				DeleteTargetFile = FALSE;
				ret = true;

			done:
				if (Decompressor != NULL)
				{
					CloseDecompressor(Decompressor);
				}

				if (CompressedBuffer)
				{
					free(CompressedBuffer);
				}

				if (DecompressedBuffer)
				{
					free(DecompressedBuffer);
				}

				if (InputFile != INVALID_HANDLE_VALUE)
				{
					CloseHandle(InputFile);
				}

				if (DecompressedFile != INVALID_HANDLE_VALUE)
				{
					//  Compression fails, delete the compressed file.
					if (DeleteTargetFile)
					{
						FILE_DISPOSITION_INFO fdi;
						fdi.DeleteFile = TRUE;      //  Marking for deletion
						Success = SetFileInformationByHandle(
							DecompressedFile,
							FileDispositionInfo,
							&fdi,
							sizeof(FILE_DISPOSITION_INFO));

						if (!Success) 
						{
							// Cannot delete corrupted decompressed file.\n");
							ret = false;
						}
					}
					CloseHandle(DecompressedFile);
				}

				// Return the result.
				return ret;
			}

			/// <summary>
			/// Compress the source data.
			/// </summary>
			/// <param name="sourceData">The byte array of data to compress.</param>
			/// <param name="sourceDataSize">The size of the source data to compress.</param>
			/// <param name="destinationData">The byte array where compressed data will be stored.</param>
			/// <param name="destinationDataSize">The size of the destination compressed data.</param>
			/// <returns>True if compressed; else false.</returns>
			bool Huffman::CompressData(PBYTE sourceData, SIZE_T sourceDataSize, PBYTE destinationData, PSIZE_T destinationDataSize)
			{
				COMPRESSOR_HANDLE Compressor = NULL;
				BOOL Success;
				SIZE_T CompressedBufferSize;
				ULONGLONG StartTime, EndTime;
				double TimeDuration;
				bool ret = false;

				// Create an XpressHuff compressor.
				Success = CreateCompressor(
					COMPRESS_ALGORITHM_XPRESS_HUFF, //  Compression Algorithm
					NULL,                           //  Optional allocation routine
					&Compressor);

				// If the compressor has been created.
				if (!Success)
				{
					// Cannot create a compressor %d.\n", GetLastError());
					ret = false;
					goto done;
				}

				// Query compressed buffer size.
				Success = Compress(
					Compressor,                  //  Compressor Handle
					sourceData,                  //  Input buffer, Uncompressed data
					sourceDataSize,              //  Uncompressed data size
					NULL,                        //  Compressed Buffer
					0,                           //  Compressed Buffer size
					&CompressedBufferSize);      //  Compressed Data size

				// Allocate memory for compressed buffer.
				if (!Success)
				{
					DWORD ErrorCode = GetLastError();

					if (ErrorCode != ERROR_INSUFFICIENT_BUFFER)
					{
						// Cannot compress data: %d.\n", ErrorCode);
						ret = false;
						goto done;
					}

					destinationData = (PBYTE)malloc(CompressedBufferSize);
					if (!destinationData)
					{
						// Cannot allocate memory for compressed buffer.\n");
						ret = false;
						goto done;
					}
				}

				// Start the timer.
				StartTime = GetTickCount64();

				// Call Compress() again to do real compression and output the compressed
				// data to CompressedBuffer.
				Success = Compress(
					Compressor,             //  Compressor Handle
					sourceData,             //  Input buffer, Uncompressed data
					sourceDataSize,         //  Uncompressed data size
					destinationData,        //  Compressed Buffer
					CompressedBufferSize,   //  Compressed Buffer size
					destinationDataSize);   //  Compressed Data size

				// If the compression was successful.
				if (!Success)
				{
					// Cannot compress data: %d\n", GetLastError());
					ret = false;
					goto done;
				}

				// End the timer.
				EndTime = GetTickCount64();

				//  Get compression time.
				TimeDuration = (EndTime - StartTime) / 1000.0;
				ret = true;

			done:
				if (Compressor != NULL)
				{
					CloseCompressor(Compressor);
				}

				// Return the result.
				return ret;
			}

			/// <summary>
			/// Decompress the source data.
			/// </summary>
			bool Huffman::DecompressData(PBYTE sourceData, SIZE_T sourceDataSize, PBYTE destinationData, PSIZE_T destinationDataSize)
			{
				DECOMPRESSOR_HANDLE Decompressor = NULL;
				BOOL Success;
				SIZE_T DecompressedBufferSize;
				ULONGLONG StartTime, EndTime;
				double TimeDuration;
				bool ret = false;

				// Create an XpressHuff decompressor.
				Success = CreateDecompressor(
					COMPRESS_ALGORITHM_XPRESS_HUFF, //  Compression Algorithm
					NULL,                           //  Optional allocation routine
					&Decompressor);                 //  Handle

				// If the decompressor was created.
				if (!Success)
				{
					// Cannot create a decompressor: %d.\n", GetLastError());
					ret = false;
					goto done;
				}

				// Query decompressed buffer size.
				Success = Decompress(
					Decompressor,                //  Compressor Handle
					sourceData,					 //  Compressed data
					sourceDataSize,              //  Compressed data size
					NULL,                        //  Buffer set to NULL
					0,                           //  Buffer size set to 0
					&DecompressedBufferSize);    //  Decompressed Data size

				// Allocate memory for decompressed buffer.
				if (!Success)
				{
					DWORD ErrorCode = GetLastError();

					// Note that the original size returned by the function is extracted 
					// from the buffer itself and should be treated as untrusted and tested
					// against reasonable limits.
					if (ErrorCode != ERROR_INSUFFICIENT_BUFFER)
					{
						// Cannot decompress data: %d.\n", ErrorCode);
						ret = false;
						goto done;
					}

					destinationData = (PBYTE)malloc(DecompressedBufferSize);
					if (!destinationData)
					{
						// Cannot allocate memory for decompressed buffer.\n");
						ret = false;
						goto done;
					}
				}

				// Start the timer.
				StartTime = GetTickCount64();

				// Decompress data and write data to DecompressedBuffer.
				Success = Decompress(
					Decompressor,				//  Decompressor handle
					sourceData,					//  Compressed data
					sourceDataSize,             //  Compressed data size
					destinationData,			//  Decompressed buffer
					DecompressedBufferSize,     //  Decompressed buffer size
					destinationDataSize);		//  Decompressed data size

				// If the decompression was successful.
				if (!Success)
				{
					// Cannot decompress data: %d.\n", GetLastError());
					ret = false;
					goto done;
				}

				// End the timer.
				EndTime = GetTickCount64();

				//  Get decompression time.
				TimeDuration = (EndTime - StartTime) / 1000.0;
				ret = true;

			done:
				if (Decompressor != NULL)
				{
					CloseDecompressor(Decompressor);
				}

				// Return the result.
				return ret;
			}
		}
	}
}