/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CryptoGlobal.h
*  Purpose :       CryptoGlobal class.
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

#ifndef _CRYPTOGLOBAL_H
#define _CRYPTOGLOBAL_H

#include "stdafx.h"

#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <string>
#include <vector>
#include <array>

// SAFE_DELETE macro.
// Deletes a pointer allocated with new.
#ifndef SAFE_DELETE
#define SAFE_DELETE(x) if (x != NULL) { delete x; x = NULL; }
#endif

// SAFE_ARRAY_DELETE macro.
// Deletes an array allocated with new [].
#ifndef SAFE_ARRAY_DELETE
#define SAFE_ARRAY_DELETE(x) if (x != NULL) { delete [] x; x = NULL; }
#endif

#define NT_SUCCESS(Status)          (((NTSTATUS)(Status)) >= 0)
#define STATUS_UNSUCCESSFUL         ((NTSTATUS)0xC0000001L)

using namespace std;

#endif