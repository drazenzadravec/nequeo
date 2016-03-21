// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#include <stdio.h>
#include <tchar.h>
#include <iostream>
#include <windows.h>

#include <mfapi.h>
#include <mfobjects.h>
#include <mfidl.h>
#include <mfplay.h>
#include <mferror.h>

#include <shlwapi.h>
#include <codecapi.h>

#include <memory.h>
#include <vector>

// Safe release of resources. Media foundation only.
template <class T> void SafeRelease(T **ppT)
{
	if (*ppT)
	{
		(*ppT)->Release();
		*ppT = NULL;
	}
}