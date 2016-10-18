/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          FileSystemUtils.h
*  Purpose :       FileSystemUtils class.
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

#include <fstream>

namespace Nequeo
{
	namespace FileSystem
	{
		class FStreamWithFileName : public Nequeo::FStream
		{
		public:
			FStreamWithFileName(const Nequeo::String& fileName, std::ios_base::openmode openFlags) :
				Nequeo::FStream(fileName.c_str(), openFlags), m_fileName(fileName) {}

			virtual ~FStreamWithFileName() = default;

			const Nequeo::String& GetFileName() const { return m_fileName; }
		protected:
			Nequeo::String m_fileName;
		};

		/**
		*  Provides a fstream around a temporary file. This file gets deleted upon an instance of this class's destructor being called.
		*/
		class TempFile : public Nequeo::FileSystem::FStreamWithFileName
		{
		public:
			/**
			*  Creates a temporary file with [prefix][temp name][suffix] e.g.
			*  prefix of "foo" and suffix of ".bar" will generate foo[some random string].bar
			*/
			TempFile(const char* prefix, const char* suffix, std::ios_base::openmode openFlags);
			/**
			*  Creates a temporary file with [prefix][temp name] e.g.
			*  prefix of "foo" will generate foo[some random string]
			*/
			TempFile(const char* prefix, std::ios_base::openmode openFlags);
			/**
			* Creates a temporary file with a randome string for the name.
			*/
			TempFile(std::ios_base::openmode openFlags);

			~TempFile();
		};
	}
}