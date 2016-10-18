/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Platform.cpp
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

#include "stdafx.h"

#include "Platform.h"
#include "StringUtils.h"

#include <stdio.h>
#include <utility>
#include <Userenv.h>
#include <time.h>

namespace Nequeo
{
	namespace Environment
	{
		/*
		using std::getenv generates a warning on windows so we use _dupenv_s instead.
		The character array returned by this function is our responsibility to clean up, so rather than returning raw strings
		that would need to be manually freed in all the client functions, just copy it into a Aws::String instead, freeing it here.
		*/
		Nequeo::String GetEnvironment(const char* name)
		{
			char* variableValue = nullptr;
			std::size_t valueSize = 0;
			auto queryResult = _dupenv_s(&variableValue, &valueSize, name);

			Nequeo::String result;
			if (queryResult == 0 && variableValue != nullptr && valueSize > 0)
			{
				result.assign(variableValue, valueSize - 1);  // don't copy the c-string terminator byte
				free(variableValue);
			}

			return result;
		}
	}

	namespace FileSystem
	{
		static const char* FILE_SYSTEM_UTILS_LOG_TAG = "FileSystem";

		Nequeo::String GetHomeDirectory()
		{
			static const char* HOME_DIR_ENV_VAR = "USERPROFILE";

			Nequeo::String homeDir = Nequeo::Environment::GetEnvironment(HOME_DIR_ENV_VAR);
			
			if (homeDir.empty())
			{
				HANDLE hToken;

				if (OpenProcessToken(GetCurrentProcess(), TOKEN_READ, &hToken))
				{
					DWORD len = MAX_PATH;
					CHAR path[MAX_PATH];
					if (GetUserProfileDirectoryA(hToken, path, &len))
					{
						homeDir = path;
					}
					CloseHandle(hToken);
				}
			}

			Nequeo::String retVal = (homeDir.size() > 0) ? Nequeo::StringUtils::Trim(homeDir.c_str()) : "";

			if (!retVal.empty())
			{
				if (retVal.at(retVal.length() - 1) != Nequeo::FileSystem::PATH_DELIM)
				{
					retVal += Nequeo::FileSystem::PATH_DELIM;
				}
			}

			return retVal;
		}

		bool CreateDirectoryIfNotExists(const char* path)
		{
			if (CreateDirectoryA(path, nullptr))
			{
				return true;
			}
			else
			{
				DWORD errorCode = GetLastError();
				return errorCode == ERROR_ALREADY_EXISTS;
			}
		}

		bool RemoveFileIfExists(const char* path)
		{
			if (DeleteFileA(path))
			{
				return true;
			}
			else
			{
				DWORD errorCode = GetLastError();
				return errorCode == ERROR_FILE_NOT_FOUND;
			}
		}

		bool RelocateFileOrDirectory(const char* from, const char* to)
		{
			if (MoveFileA(from, to))
			{
				return true;
			}
			else
			{
				int errorCode = GetLastError();
				return false;
			}
		}

		Nequeo::String CreateTempFilePath()
		{
#ifdef _MSC_VER
#pragma warning(disable: 4996) // _CRT_SECURE_NO_WARNINGS
#endif
			char s_tempName[L_tmpnam_s + 1];

			/*
			Prior to VS 2014, tmpnam/tmpnam_s generated root level files ("\filename") which were not appropriate for our usage, so for the windows version, we prepended a '.' to make it a
			tempfile in the current directory.  Starting with VS2014, the behavior of tmpnam/tmpnam_s was changed to be a full, valid filepath based on the
			current user ("C:\Users\username\AppData\Local\Temp\...").

			See the tmpnam section in http://blogs.msdn.com/b/vcblog/archive/2014/06/18/crt-features-fixes-and-breaking-changes-in-visual-studio-14-ctp1.aspx
			for more details.
			*/

#if _MSC_VER >= 1900
			tmpnam_s(s_tempName, L_tmpnam_s);
#else
			s_tempName[0] = '.';
			tmpnam_s(s_tempName + 1, L_tmpnam_s);
#endif // _MSC_VER


			return s_tempName;
		}
	}

#pragma warning(disable: 4996)
#include <windows.h>
	namespace OSVersionInfo
	{
		Nequeo::String ComputeOSVersionString()
		{
			OSVERSIONINFOA versionInfo;
			ZeroMemory(&versionInfo, sizeof(OSVERSIONINFOA));
			versionInfo.dwOSVersionInfoSize = sizeof(OSVERSIONINFOA);
			GetVersionExA(&versionInfo);
			Nequeo::StringStream ss;
			ss << "Windows/" << versionInfo.dwMajorVersion << "." << versionInfo.dwMinorVersion << "." << versionInfo.dwBuildNumber << "-" << versionInfo.szCSDVersion;

			SYSTEM_INFO sysInfo;
			ZeroMemory(&sysInfo, sizeof(SYSTEM_INFO));
			GetSystemInfo(&sysInfo);

			switch (sysInfo.wProcessorArchitecture)
			{
				//PROCESSOR_ARCHIECTURE_AMD64
			case 0x09:
				ss << " AMD64";
				break;
				//PROCESSOR_ARCHITECTURE_IA64
			case 0x06:
				ss << " IA64";
				break;
				//PROCESSOR_ARCHITECTURE_INTEL
			case 0x00:
				ss << " x86";
				break;
			default:
				ss << " Unknown Processor Architecture";
				break;
			}

			return ss.str();
		}
	}

	namespace Security
	{
		void SecureMemoryCleanup(unsigned char *data, size_t length)
		{
			SecureZeroMemory(data, length);
		}
	}

	namespace Time
	{
		time_t TimeGM(struct tm* const t)
		{
			return _mkgmtime(t);
		}

		void LocalTime(tm* t, std::time_t time)
		{
			localtime_s(t, &time);
		}

		void GMTime(tm* t, std::time_t time)
		{
			gmtime_s(t, &time);
		}
	}
}