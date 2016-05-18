// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>
#include <string.h>
#include <exception>
#include <stdio.h>
#include <stdexcept>
#include <vector>
#include <algorithm>
#include <memory>

#ifdef NEQUEOMATHMKL_EXPORTS
#define EXPORT_NEQUEO_MKL_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_MKL_API __declspec(dllimport) 
#endif

// Complex double.
typedef struct _Complex_Double
{
	double real;
	double imag;
} ComplexDouble;

// Complex float.
typedef struct _Complex_Float
{
	float real;
	float imag;
} ComplexFloat;