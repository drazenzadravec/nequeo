/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Global.h
*  Purpose :       Global.
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

#include "stdafx.h"

#ifdef _MSC_VER
// Disable warning C4251: <data member>: <type> needs to have dll-interface to be used.
#pragma warning(disable : 4251)
#endif

#ifdef NEQUEOAZURESTORAGE_EXPORTS
#define EXPORT_NEQUEO_AZURE_STORAGE_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_AZURE_STORAGE_API __declspec(dllimport) 
#endif

#undef max
#undef min

#include <cpprest\ws_client.h>
#include <cpprest\http_client.h>
#include <cpprest\http_msg.h>
#include <cpprest\json.h>
#include <cpprest\uri.h>
#include <cpprest\uri_builder.h>
#include <cpprest\streams.h>
#include <cpprest\containerstream.h>
#include <cpprest\astreambuf.h>
#include <cpprest\interopstream.h>
#include <cpprest\rawptrstream.h>
#include <cpprest\filestream.h>

#include <was\core.h>
#include <was\common.h>
#include <was\auth.h>
#include <was\storage_account.h>
#include <was\error_code_strings.h>
#include <was\retry_policies.h>
#include <was\service_client.h>

#include <wascore\executor.h>
#include <wascore\protocol.h>
#include <wascore\resources.h>
#include <wascore\streams.h>
#include <wascore\streambuf.h>
#include <wascore\util.h>