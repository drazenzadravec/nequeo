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
#include <complex>

#ifdef NEQUEOMATHACML_EXPORTS
#define EXPORT_NEQUEO_ACML_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_ACML_API __declspec(dllimport) 
#endif

#define _ACML_COMPLEX
typedef struct
{
	float real, imag;
} complex;
typedef struct
{
	double real, imag;
} doublecomplex;

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