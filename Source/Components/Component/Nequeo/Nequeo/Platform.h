/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Platform.h
*  Purpose :       Platform class.
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

#include "Global.h"
#include "Allocator.h"

#include <ctime>

namespace Nequeo
{
	namespace Environment
	{
		/*
		using std::getenv generates a warning on windows so we use _dupenv_s instead.
		The character array returned by this function is our responsibility to clean up, so rather than returning raw strings
		that would need to be manually freed in all the client functions, just copy it into a Aws::String instead, freeing it here.
		*/
		Nequeo::String GetEnvironment(const char* name);
	}

	namespace FileSystem
	{
#ifdef _WIN32
		static const char PATH_DELIM = '\\';
#else
		static const char PATH_DELIM = '/';
#endif

		/**
		* Returns the directory path for the home dir env variable
		*/
		Nequeo::String GetHomeDirectory();

		/**
		* Creates directory if it doesn't exist. Returns true if the directory was created
		* or already exists. False for failure.
		*/
		bool CreateDirectoryIfNotExists(const char* path);

		/**
		* Deletes file if it exists. Returns true if file doesn't exist or on success.
		*/
		bool RemoveFileIfExists(const char* fileName);

		/**
		* Moves the file. Returns true on success
		*/
		bool RelocateFileOrDirectory(const char* from, const char* to);

		/**
		* Computes a unique tmp file path
		*/
		Nequeo::String CreateTempFilePath();
	}

	namespace OSVersionInfo
	{
		/**
		* computing the version string for the current running operating system.
		*/
		Nequeo::String ComputeOSVersionString();
	}

	namespace Security
	{
		/*
		* Securely clears a block of memory
		*/
		void SecureMemoryCleanup(unsigned char *data, size_t length);
	}

	namespace Time
	{
		/*
		* A platform-agnostic implementation of the timegm function from gnu extensions
		*/
		time_t TimeGM(tm* const t);

		/*
		* Converts from a time value of std::time_t type to a C tm structure for easier date analysis
		*/
		void LocalTime(tm* t, std::time_t time);

		/*
		* Converts from a time value of std::time_t type to a C tm structure for easier date analysis
		*/
		void GMTime(tm* t, std::time_t time);
	}
}