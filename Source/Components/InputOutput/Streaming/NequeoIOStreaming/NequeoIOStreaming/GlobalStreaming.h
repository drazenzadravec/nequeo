/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          GlobalStreaming.h
*  Purpose :       GlobalStreaming class.
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

#ifndef _GLOBALSTREAMING_H
#define _GLOBALSTREAMING_H

#include "stdafx.h"

#include <streambuf>
#include <iosfwd>
#include <ios>
#include <istream>
#include <ostream>

#if !defined(NEQUEO_IOS_INIT_HACK)
// Microsoft Visual Studio with Dinkumware STL (but not STLport)
#	if defined(_MSC_VER) && (!defined(_STLP_MSVC) || defined(_STLP_NO_OWN_IOSTREAMS))
#		define NEQUEO_IOS_INIT_HACK 1
// QNX with Dinkumware but not GNU C++ Library
#	elif defined(__QNX__) && !defined(__GLIBCPP__)
#		define NEQUEO_IOS_INIT_HACK 1
#	endif
#endif

#if defined(NEQUEO_IOS_INIT_HACK)
#	define nequeo_ios_init(buf)
#else
#	define nequeo_ios_init(buf) init(buf)
#endif

using namespace std;

#endif