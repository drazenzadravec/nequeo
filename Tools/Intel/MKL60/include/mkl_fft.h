/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright(C) 1999-2003 Intel Corporation. All Rights Reserved.
//
// File    : mkl_fft.h
// Purpose : Intel(R) Math Kernel Library (MKL) interface for FFT routines
*/

#ifndef __mkl_fft_h__
#define __mkl_fft_h__

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/* Upper case declaration */

void   CFFT1D(float* ar,  int* m, int* isign, float* ws);
void   ZFFT1D(double* ar, int* m, int* isign, double* ws);
void   CSFFT1D(float* r,  int* m, int* isign, float* ws);
void   SCFFT1D(float* r,  int* m, int* isign, float* ws);
void   ZDFFT1D(double* r, int* m, int* isign, double* ws);
void   DZFFT1D(double* r, int* m, int* isign, double* ws);
void   CFFT1DC(float* ar, float* ai, int m, int isign, float* ws);
void   ZFFT1DC(double* ar, double* ai, int m, int isign, double* ws);
void   CSFFT1DC(float* r,  int m, int isign, float* ws);
void   SCFFT1DC(float* r,  int m, int isign, float* ws);
void   ZDFFT1DC(double* r, int m, int isign, double* ws);
void   DZFFT1DC(double* r, int m, int isign, double* ws);
void   CFFT2D(float* ar,  int* m, int* n, int* isign);
void   ZFFT2D(double* ar, int* m, int* n, int* isign);
void   CSFFT2D(float* r,  int* m, int* n);
void   SCFFT2D(float* r,  int* m, int* n);
void   ZDFFT2D(double* r, int* m, int* n);
void   DZFFT2D(double* r, int* m, int* n);
void   CFFT2DC(float* ar, float* ai, int m, int n, int isign);
void   ZFFT2DC(double* ar, double* ai, int m, int n, int isign);
void   CSFFT2DC(float* r, int m, int n);
void   SCFFT2DC(float* r, int m, int n);
void   ZDFFT2DC(double* r, int m, int n);
void   DZFFT2DC(double* r, int m, int n);

/* Lower case declaration */

void   cfft1d(float* ar,  int* m, int* isign, float* ws);
void   zfft1d(double* ar, int* m, int* isign, double* ws);
void   csfft1d(float* r,  int* m, int* isign, float* ws);
void   scfft1d(float* r,  int* m, int* isign, float* ws);
void   zdfft1d(double* r, int* m, int* isign, double* ws);
void   dzfft1d(double* r, int* m, int* isign, double* ws);
void   cfft1dc(float* ar, float* ai, int m, int isign, float* ws);
void   zfft1dc(double* ar, double* ai, int m, int isign, double* ws);
void   csfft1dc(float* r,  int m, int isign, float* ws);
void   scfft1dc(float* r,  int m, int isign, float* ws);
void   zdfft1dc(double* r, int m, int isign, double* ws);
void   dzfft1dc(double* r, int m, int isign, double* ws);
void   cfft2d(float* ar,  int* m, int* n, int* isign);
void   zfft2d(double* ar, int* m, int* n, int* isign);
void   csfft2d(float* r,  int* m, int* n);
void   scfft2d(float* r,  int* m, int* n);
void   zdfft2d(double* r, int* m, int* n);
void   dzfft2d(double* r, int* m, int* n);
void   cfft2dc(float* ar, float* ai, int m, int n, int isign);
void   zfft2dc(double* ar, double* ai, int m, int n, int isign);
void   csfft2dc(float* r, int m, int n);
void   scfft2dc(float* r, int m, int n);
void   zdfft2dc(double* r, int m, int n);
void   dzfft2dc(double* r, int m, int n);

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* __mkl_fft_h__ */
