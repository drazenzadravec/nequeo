/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          GlobalCUDA.h
*  Purpose :       Global CUDA.
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

#ifndef _GLOBALCUDA_H
#define _GLOBALCUDA_H

#include "stdafx.h"

#include <cstring>

const int ALIGNMENT = 64;
const int INSUFFICIENT_MEMORY = -999999;

#define sgetrf cusolverDnSgetrf
#define dgetrf cusolverDnDgetrf
#define cgetrf cusolverDnCgetrf
#define zgetrf cusolverDnZgetrf
#define sgetrfbsize cusolverDnSgetrf_bufferSize
#define dgetrfbsize cusolverDnDgetrf_bufferSize
#define cgetrfbsize cusolverDnCgetrf_bufferSize
#define zgetrfbsize cusolverDnZgetrf_bufferSize

#define sgetrs cusolverDnSgetrs
#define dgetrs cusolverDnDgetrs
#define cgetrs cusolverDnCgetrs
#define zgetrs cusolverDnZgetrs

#define spotrf cusolverDnSpotrf
#define dpotrf cusolverDnDpotrf
#define cpotrf cusolverDnCpotrf
#define zpotrf cusolverDnZpotrf
#define spotrfbsize cusolverDnSpotrf_bufferSize
#define dpotrfbsize cusolverDnDpotrf_bufferSize
#define cpotrfbsize cusolverDnCpotrf_bufferSize
#define zpotrfbsize cusolverDnZpotrf_bufferSize

#define spotrs cusolverDnSpotrs
#define dpotrs cusolverDnDpotrs
#define cpotrs cusolverDnCpotrs
#define zpotrs cusolverDnZpotrs

#define sgeqrf cusolverDnSgeqrf
#define dgeqrf cusolverDnDgeqrf
#define cgeqrf cusolverDnCgeqrf
#define zgeqrf cusolverDnZgeqrf

#define sormqr cusolverDnSormqr
#define dormqr cusolverDnDormqr

#define sgesvd cusolverDnSgesvd
#define dgesvd cusolverDnDgesvd
#define cgesvd cusolverDnCgesvd
#define zgesvd cusolverDnZgesvd
#define sgesvdbsize cusolverDnSgesvd_bufferSize
#define dgesvdbsize cusolverDnDgesvd_bufferSize
#define cgesvdbsize cusolverDnCgesvd_bufferSize
#define zgesvdbsize cusolverDnZgesvd_bufferSize

#define sgetribatched cublasSgetriBatched
#define dgetribatched cublasDgetriBatched
#define cgetribatched cublasCgetriBatched
#define zgetribatched cublasZgetriBatched

#endif