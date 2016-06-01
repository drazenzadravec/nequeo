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
#include <random>

#ifdef NEQUEOMATHOPENBLAS_EXPORTS
#define EXPORT_NEQUEO_OPENBLAS_API __declspec(dllexport) 
#else
#define EXPORT_NEQUEO_OPENBLAS_API __declspec(dllimport) 
#endif

#define blas_int blasint
#define blas_complex_float openblas_complex_float
#define blas_complex_double openblas_complex_double
#define LAPACK_COMPLEX_CUSTOM
#define C_MSVC

#define lapack_complex_float std::complex<float>
#define lapack_complex_double std::complex<double>

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