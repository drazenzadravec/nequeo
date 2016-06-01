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

namespace Nequeo
{
	/// <summary>
	/// Provides methods and properties used to compress and decompress archives.
	/// </summary>
	LibraryLoader::LibraryLoader() : _disposed(false), _libraryLoaded(false), _library(NULL), _func(NULL)
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

			// Free the seven zip library.
			Free();
		}
	}

	/// <summary>
	/// Loads the library.
	/// </summary>
	/// <param name="libraryPath">The path and file name (if not in the same directory) of the library.</param>
	/// <returns>True if loaded: else false.</returns>
	bool LibraryLoader::Load(const Nequeo::TFormatString& libraryPath)
	{
		Free();

		// Assign the library path.
		_libraryPath = libraryPath;

		// Load the library.
		_library = LoadLibrary(libraryPath.c_str());
		_libraryLoaded = true;

		// If the library has not been created.
		if (_library == NULL)
		{
			_libraryLoaded = false;
			return false;
		}

		// Create the function handler.
		_func = reinterpret_cast<CreateObjectFunc>(GetProcAddress(_library, "CreateObject"));
		if (_func == NULL)
		{
			Free();
			_libraryLoaded = false;
			return false;
		}

		// Return the loaded state.
		return _libraryLoaded;
	}

	/// <summary>
	/// Free the library.
	/// </summary>
	/// <returns>True if freeded: else false.</returns>
	bool LibraryLoader::Free()
	{
		if (_library != NULL)
		{
			FreeLibrary(_library);
			_library = NULL;
			_func = NULL;
			_libraryLoaded = false;
		}

		// Return the loaded state.
		return _libraryLoaded;
	}

	/// <summary>
	/// Create the compression object within the library.
	/// </summary>
	/// <returns>The HR result.</returns>
	HRESULT LibraryLoader::CreateObject(const GUID& clsID, const GUID& interfaceID, void** outObject) const
	{
		if (_func != NULL)
		{
			// Execute the function handler.
			HRESULT hr = _func(&clsID, &interfaceID, outObject);
			return hr;
		}
		return -1;
	}
}
