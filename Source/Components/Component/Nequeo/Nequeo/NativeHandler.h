/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          NativeHandler.h
*  Purpose :       NativeHandler definition header.
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

#ifndef _NATIVEHANDLER_H
#define _NATIVEHANDLER_H

#include "stdafx.h"

#include "Types.h"

using namespace Nequeo::System::Any;

#ifdef _UNICODE
#define CREATENETOBJECT "CreateNetObjectW"
#define CALLNETOBJECT "CallNetObjectW"
#define GETNETOBJECTPROPERTY "GetNetObjectPropertyW"
#define SETNETOBJECTPROPERTY "SetNetObjectPropertyW"
#define CALLNETCLASS "CallNetClassW"
#define GETNETCLASSPROPERTY "GetNetClassPropertyW"
#define SETNETCLASSPROPERTY "SetNetClassPropertyW"
#define RELEASENETOBJECT "ReleaseNetObjectW"
#else
#define CREATENETOBJECT "CreateNetObjectA"
#define CALLNETOBJECT "CallNetObjectA"
#define GETNETOBJECTPROPERTY "GetNetObjectPropertyA"
#define SETNETOBJECTPROPERTY "SetNetObjectPropertyA"
#define CALLNETCLASS "CallNetClassA"
#define GETNETCLASSPROPERTY "GetNetClassPropertyA"
#define SETNETCLASSPROPERTY "SetNetClassPropertyA"
#define RELEASENETOBJECT "ReleaseNetObjectA"
#endif 

extern "C"
{
	// create/release
	typedef void * (*CreateNetObject) (const stdtstring &dllFileName, const stdtstring &className, const AnyTypeArray &ctorParams, stdtstring &exceptionText);
	typedef bool(*ReleaseNetObject) (void *pointerToManagedObject, stdtstring &exceptionText);

	// members
	typedef bool(*CallNetObject) (void *pointerToManagedObject, const stdtstring &functionName, AnyTypeArray &inputParameters, AnyType &result, stdtstring &exceptionText);
	typedef bool(*GetNetObjectProperty) (void *pointerToManagedObject, const stdtstring &functionName, AnyType &result, stdtstring &exceptionText);
	typedef bool(*SetNetObjectProperty) (void *pointerToManagedObject, const stdtstring &functionName, const AnyType &value, stdtstring &exceptionText);

	// statics 
	typedef bool(*CallNetClass) (const stdtstring &dllFileName, const stdtstring &className, const stdtstring &functionName, AnyTypeArray &inputParameters, AnyType &result, stdtstring &exceptionText);
	typedef bool(*GetNetClassProperty) (const stdtstring &dllFileName, const stdtstring &className, const stdtstring &functionName, AnyType &result, stdtstring &exceptionText);
	typedef bool(*SetNetClassProperty) (const stdtstring &dllFileName, const stdtstring &className, const stdtstring &functionName, const AnyType &value, stdtstring &exceptionText);
}
#endif