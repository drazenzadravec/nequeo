/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright(C) 1999-2003 Intel Corporation. All Rights Reserved.
//
// File    : mkl_blas.h
// Purpose : Intel(R) Math Kernel Library (MKL) interface for BLAS routines
*/

#ifndef _MKL_BLAS_H_
#define _MKL_BLAS_H_

#include "mkl_types.h"

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/* Upper case declaration */

/* BLAS Level1 */

float SASUM(int *n,float *x,int *incx);
void  SAXPY(int *n,float *alpha,float *x,int *incx,float *y,int *incy);
void  SAXPYI(int *nz,float *a,float *x,int *indx,float *y);
float SCASUM(int *n,MKL_Complex8 *x,int *incx); 
float SCNRM2(int *n,MKL_Complex8 *x,int *incx); 
void  SCOPY(int *n,float *x,int *incx,float *y,int *incy);
float SDOT(int *n,float *x,int *incx,float *y,int *incy);
float SDOTI(int *nz,float *x,int *indx,float *y);
void  SGTHR(int *nz,float *y,float *x,int *indx);
void  SGTHRZ(int *nz,float *y,float *x,int *indx);
float SNRM2(int *n,float *x,int *incx);
void  SROT(int *n,float *x,int *incx,float *y,int *incy,float *c,float *s);
void  SROTG(float *a,float *b,float *c,float *s);
void  SROTI(int *nz,float *x,int *indx,float *y,float *c,float *s);
void  SROTM(int *n,float *x,int *incx,float *y,int *incy,float *param);
void  SROTMG(float *d1,float *d2,float *x1,float *y1,float *param);
void  SSCAL(int *n,float *a,float *x,int *incx);
void  SSCTR(int *nz,float *x,int *indx,float *y);
void  SSWAP(int *n,float *x,int *incx,float *y,int *incy);
int   ISAMAX(int *n,float *x,int *incx);
int   ISAMIN(int *n,float *x,int *incx);

void CAXPY(int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void CAXPYI(int *nz,MKL_Complex8 *a,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void CCOPY(int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void CDOTC(MKL_Complex8 *pres,int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void CDOTCI(MKL_Complex8 *pres,int *nz,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void CDOTU(MKL_Complex8 *pres,int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void CDOTUI(MKL_Complex8 *pres,int *nz,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void CGTHR(int *nz,MKL_Complex8 *y,MKL_Complex8 *x,int *indx); 
void CGTHRZ(int *nz,MKL_Complex8 *y,MKL_Complex8 *x,int *indx); 
void CROTG(MKL_Complex8 *a,MKL_Complex8 *b,float *c,MKL_Complex8 *s); 
void CSCAL(int *n,MKL_Complex8 *a,MKL_Complex8 *x,int *incx); 
void CSCTR(int *nz,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void CSROT(int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,float *c,float *s); 
void CSSCAL(int *n,float *a,MKL_Complex8 *x,int *incx); 
void CSWAP(int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
int  ICAMAX(int *n,MKL_Complex8 *x,int *incx); 
int  ICAMIN(int *n,MKL_Complex8 *x,int *incx); 

double DASUM(int *n,double *x,int *incx);
void   DAXPY(int *n,double *alpha,double *x,int *incx,double *y,int *incy);
void   DAXPYI(int *nz,double *a,double *x,int *indx,double *y);
void   DCOPY(int *n,double *x,int *incx,double *y,int *incy);
double DDOT(int *n,double *x,int *incx,double *y,int *incy);
double DDOTI(int *nz,double *x,int *indx,double *y);
void   DGTHR(int *nz,double *y,double *x,int *indx);
void   DGTHRZ(int *nz,double *y,double *x,int *indx);
double DNRM2(int *n,double *x,int *incx);
void   DROT(int *n,double *x,int *incx,double *y,int *incy,double *c,double *s);
void   DROTG(double *a,double *b,double *c,double *s);
void   DROTI(int *nz,double *x,int *indx,double *y,double *c,double *s);
void   DROTM(int *n,double *x,int *incx,double *y,int *incy,double *param);
void   DROTMG(double *d1,double *d2,double *x1,double *y1,double *param);
void   DSCAL(int *n,double *a,double *x,int *incx);
void   DSCTR(int *nz,double *x,int *indx,double *y);
void   DSWAP(int *n,double *x,int *incx,double *y,int *incy);
double DZASUM(int *n,MKL_Complex16 *x,int *incx); 
double DZNRM2(int *n,MKL_Complex16 *x,int *incx); 
int    IDAMAX(int *n,double *x,int *incx);
int    IDAMIN(int *n,double *x,int *incx);

void ZAXPY(int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void ZAXPYI(int *nz,MKL_Complex16 *a,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void ZCOPY(int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void ZDOTC(MKL_Complex16 *pres,int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void ZDOTCI(MKL_Complex16 *pres,int *nz,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void ZDOTU(MKL_Complex16 *pres,int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void ZDOTUI(MKL_Complex16 *pres,int *nz,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void ZDROT(int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,double *c,double *s); 
void ZDSCAL(int *n,double *a,MKL_Complex16 *x,int *incx); 
void ZGTHR(int *nz,MKL_Complex16 *y,MKL_Complex16 *x,int *indx); 
void ZGTHRZ(int *nz,MKL_Complex16 *y,MKL_Complex16 *x,int *indx); 
void ZROTG(MKL_Complex16 *a,MKL_Complex16 *b,double *c,MKL_Complex16 *s); 
void ZSCAL(int *n,MKL_Complex16 *a,MKL_Complex16 *x,int *incx); 
void ZSCTR(int *nz,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void ZSWAP(int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
int  IZAMAX(int *n,MKL_Complex16 *x,int *incx); 
int  IZAMIN(int *n,MKL_Complex16 *x,int *incx); 

/* BLAS Level2 */

void SGBMV(char *trans,int *m,int *n,int *kl,int *ku,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void SGEMV(char *trans,int *m,int *n,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void SGER(int *m,int *n,float *alpha,float *x,int *incx,float *y,int *incy,float *a,int *lda);
void SSBMV(char *uplo,int *n,int *k,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void SSPMV(char *uplo,int *n,float *alpha,float *ap,float *x,int *incx,float *beta,float *y,int *incy);
void SSPR(char *uplo,int *n,float *alpha,float *x,int *incx,float *ap);
void SSPR2(char *uplo,int *n,float *alpha,float *x,int *incx,float *y,int *incy,float *ap);
void SSYMV(char *uplo,int *n,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void SSYR(char *uplo,int *n,float *alpha,float *x,int *incx,float *a,int *lda);
void SSYR2(char *uplo,int *n,float *alpha,float *x,int *incx,float *y,int *incy,float *a,int *lda);
void STBMV(char *uplo,char *trans,char *diag,int *n,int *k,float *a,int *lda,float *x,int *incx);
void STBSV(char *uplo,char *trans,char *diag,int *n,int *k,float *a,int *lda,float *x,int *incx);
void STPMV(char *uplo,char *trans,char *diag,int *n,float *ap,float *x,int *incx);
void STPSV(char *uplo,char *trans,char *diag,int *n,float *ap,float *x,int *incx);
void STRMV(char *uplo,char *transa,char *diag,int *n,float *a,int *lda,float *b,int *incx);
void STRSV(char *uplo,char *trans,char *diag,int *n,float *a,int *lda,float *x,int *incx);

void CGBMV(char *trans,int *m,int *n,int *kl,int *ku,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void CGEMV(char *trans,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void CGERC(int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *a,int *lda); 
void CGERU(int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *a,int *lda); 
void CHBMV(char *uplo,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void CHEMV(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void CHER(char *uplo,int *n,float *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *a,int *lda); 
void CHER2(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *a,int *lda); 
void CHPMV(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *ap,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void CHPR(char *uplo,int *n,float *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *ap); 
void CHPR2(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *ap); 
void CTBMV(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx); 
void CTBSV(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx); 
void CTPMV(char *uplo,char *trans,char *diag,int *n,MKL_Complex8 *ap,MKL_Complex8 *x,int *incx); 
void CTPSV(char *uplo,char *trans,char *diag,int *n,MKL_Complex8 *ap,MKL_Complex8 *x,int *incx); 
void CTRMV(char *uplo,char *transa,char *diag,int *n,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *incx); 
void CTRSV(char *uplo,char *trans,char *diag,int *n,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx); 

void DGBMV(char *trans,int *m,int *n,int *kl,int *ku,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void DGEMV(char *trans,int *m,int *n,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void DGER(int *m,int *n,double *alpha,double *x,int *incx,double *y,int *incy,double *a,int *lda);
void DSBMV(char *uplo,int *n,int *k,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void DSPMV(char *uplo,int *n,double *alpha,double *ap,double *x,int *incx,double *beta,double *y,int *incy);
void DSPR(char *uplo,int *n,double *alpha,double *x,int *incx,double *ap);
void DSPR2(char *uplo,int *n,double *alpha,double *x,int *incx,double *y,int *incy,double *ap);
void DSYMV(char *uplo,int *n,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void DSYR(char *uplo,int *n,double *alpha,double *x,int *incx,double *a,int *lda);
void DSYR2(char *uplo,int *n,double *alpha,double *x,int *incx,double *y,int *incy,double *a,int *lda);
void DTBMV(char *uplo,char *trans,char *diag,int *n,int *k,double *a,int *lda,double *x,int *incx);
void DTBSV(char *uplo,char *trans,char *diag,int *n,int *k,double *a,int *lda,double *x,int *incx);
void DTPMV(char *uplo,char *trans,char *diag,int *n,double *ap,double *x,int *incx);
void DTPSV(char *uplo,char *trans,char *diag,int *n,double *ap,double *x,int *incx);
void DTRMV(char *uplo,char *transa,char *diag,int *n,double *a,int *lda,double *b,int *incx);
void DTRSV(char *uplo,char *trans,char *diag,int *n,double *a,int *lda,double *x,int *incx);

void ZGBMV(char *trans,int *m,int *n,int *kl,int *ku,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void ZGEMV(char *trans,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void ZGERC(int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *a,int *lda); 
void ZGERU(int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *a,int *lda); 
void ZHBMV(char *uplo,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void ZHEMV(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void ZHER(char *uplo,int *n,double *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *a,int *lda); 
void ZHER2(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *a,int *lda); 
void ZHPMV(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *ap,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void ZHPR(char *uplo,int *n,double *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *ap); 
void ZHPR2(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *ap); 
void ZTBMV(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx); 
void ZTBSV(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx); 
void ZTPMV(char *uplo,char *trans,char *diag,int *n,MKL_Complex16 *ap,MKL_Complex16 *x,int *incx); 
void ZTPSV(char *uplo,char *trans,char *diag,int *n,MKL_Complex16 *ap,MKL_Complex16 *x,int *incx); 
void ZTRMV(char *uplo,char *transa,char *diag,int *n,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *incx); 
void ZTRSV(char *uplo,char *trans,char *diag,int *n,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx); 

/* BLAS Level3 */

void SGEMM(char *transa,char *transb,int *m,int *n,int *k,float *alpha,float *a,int *lda,float *b,int *ldb,float *beta,float *c,int *ldc);
void SSYMM(char *side,char *uplo,int *m,int *n,float *alpha,float *a,int *lda,float *b,int *ldb,float *beta,float *c,int *ldc);
void SSYR2K(char *uplo,char *trans,int *n,int *k,float *alpha,float *a,int *lda,float *b,int *ldb,float *beta,float *c,int *ldc);
void SSYRK(char *uplo,char *trans,int *n,int *k,float *alpha,float *a,int *lda,float *beta,float *c,int *ldc);
void STRMM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,float *alpha,float *a,int *lda,float *b,int *ldb);
void STRSM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,float *alpha,float *a,int *lda,float *b,int *ldb);

void CGEMM(char *transa,char *transb,int *m,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void CHEMM(char *side,char *uplo,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void CHER2K(char *uplo,char *trans,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,float *beta,MKL_Complex8 *c,int *ldc); 
void CHERK(char *uplo,char *trans,int *n,int *k,float *alpha,MKL_Complex8 *a,int *lda,float *beta,MKL_Complex8 *c,int *ldc); 
void CSYMM(char *side,char *uplo,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void CSYR2K(char *uplo,char *trans,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void CSYRK(char *uplo,char *trans,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void CTRMM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb); 
void CTRSM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb); 

void DGEMM(char *transa,char *transb,int *m,int *n,int *k,double *alpha,double *a,int *lda,double *b,int *ldb,double *beta,double *c,int *ldc);
void DSYMM(char *side,char *uplo,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb,double *beta,double *c,int *ldc);
void DSYR2K(char *uplo,char *trans,int *n,int *k,double *alpha,double *a,int *lda,double *b,int *ldb,double *beta,double *c,int *ldc);
void DSYRK(char *uplo,char *trans,int *n,int *k,double *alpha,double *a,int *lda,double *beta,double *c,int *ldc);
void DTRMM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb);
void DTRSM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb);

void ZGEMM(char *transa,char *transb,int *m,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void ZGEMM(char *transa,char *transb,int *m,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void ZHEMM(char *side,char *uplo,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void ZHER2K(char *uplo,char *trans,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,double *beta,MKL_Complex16 *c,int *ldc); 
void ZHERK(char *uplo,char *trans,int *n,int *k,double *alpha,MKL_Complex16 *a,int *lda,double *beta,MKL_Complex16 *c,int *ldc); 
void ZSYMM(char *side,char *uplo,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void ZSYR2K(char *uplo,char *trans,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void ZSYRK(char *uplo,char *trans,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void ZTRMM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb); 
void ZTRSM(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb); 

/* Lower case declaration */

/* BLAS Level1 */

float sasum(int *n,float *x,int *incx);
void  saxpy(int *n,float *alpha,float *x,int *incx,float *y,int *incy);
void  saxpyi(int *nz,float *a,float *x,int *indx,float *y);
float scasum(int *n,MKL_Complex8 *x,int *incx); 
float scnrm2(int *n,MKL_Complex8 *x,int *incx); 
void  scopy(int *n,float *x,int *incx,float *y,int *incy);
float sdot(int *n,float *x,int *incx,float *y,int *incy);
float sdoti(int *nz,float *x,int *indx,float *y);
void  sgthr(int *nz,float *y,float *x,int *indx);
void  sgthrz(int *nz,float *y,float *x,int *indx);
float snrm2(int *n,float *x,int *incx);
void  srot(int *n,float *x,int *incx,float *y,int *incy,float *c,float *s);
void  srotg(float *a,float *b,float *c,float *s);
void  sroti(int *nz,float *x,int *indx,float *y,float *c,float *s);
void  srotm(int *n,float *x,int *incx,float *y,int *incy,float *param);
void  srotmg(float *d1,float *d2,float *x1,float *y1,float *param);
void  sscal(int *n,float *a,float *x,int *incx);
void  ssctr(int *nz,float *x,int *indx,float *y);
void  sswap(int *n,float *x,int *incx,float *y,int *incy);
int   isamax(int *n,float *x,int *incx);
int   isamin(int *n,float *x,int *incx);

void caxpy(int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void caxpyi(int *nz,MKL_Complex8 *a,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void ccopy(int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void cdotc(MKL_Complex8 *pres,int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void cdotci(MKL_Complex8 *pres,int *nz,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void cdotu(MKL_Complex8 *pres,int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
void cdotui(MKL_Complex8 *pres,int *nz,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void cgthr(int *nz,MKL_Complex8 *y,MKL_Complex8 *x,int *indx); 
void cgthrz(int *nz,MKL_Complex8 *y,MKL_Complex8 *x,int *indx); 
void crotg(MKL_Complex8 *a,MKL_Complex8 *b,float *c,MKL_Complex8 *s); 
void cscal(int *n,MKL_Complex8 *a,MKL_Complex8 *x,int *incx); 
void csctr(int *nz,MKL_Complex8 *x,int *indx,MKL_Complex8 *y); 
void csrot(int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,float *c,float *s); 
void csscal(int *n,float *a,MKL_Complex8 *x,int *incx); 
void cswap(int *n,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy); 
int  icamax(int *n,MKL_Complex8 *x,int *incx); 
int  icamin(int *n,MKL_Complex8 *x,int *incx); 

double dasum(int *n,double *x,int *incx);
void   daxpy(int *n,double *alpha,double *x,int *incx,double *y,int *incy);
void   daxpyi(int *nz,double *a,double *x,int *indx,double *y);
void   dcopy(int *n,double *x,int *incx,double *y,int *incy);
double ddot(int *n,double *x,int *incx,double *y,int *incy);
double ddoti(int *nz,double *x,int *indx,double *y);
void   dgthr(int *nz,double *y,double *x,int *indx);
void   dgthrz(int *nz,double *y,double *x,int *indx);
double dnrm2(int *n,double *x,int *incx);
void   drot(int *n,double *x,int *incx,double *y,int *incy,double *c,double *s);
void   drotg(double *a,double *b,double *c,double *s);
void   droti(int *nz,double *x,int *indx,double *y,double *c,double *s);
void   drotm(int *n,double *x,int *incx,double *y,int *incy,double *param);
void   drotmg(double *d1,double *d2,double *x1,double *y1,double *param);
void   dscal(int *n,double *a,double *x,int *incx);
void   dsctr(int *nz,double *x,int *indx,double *y);
void   dswap(int *n,double *x,int *incx,double *y,int *incy);
double dzasum(int *n,MKL_Complex16 *x,int *incx); 
double dznrm2(int *n,MKL_Complex16 *x,int *incx); 
int    idamax(int *n,double *x,int *incx);
int    idamin(int *n,double *x,int *incx);

void zaxpy(int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void zaxpyi(int *nz,MKL_Complex16 *a,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void zcopy(int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void zdotc(MKL_Complex16 *pres,int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void zdotci(MKL_Complex16 *pres,int *nz,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void zdotu(MKL_Complex16 *pres,int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
void zdotui(MKL_Complex16 *pres,int *nz,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void zdrot(int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,double *c,double *s); 
void zdscal(int *n,double *a,MKL_Complex16 *x,int *incx); 
void zgthr(int *nz,MKL_Complex16 *y,MKL_Complex16 *x,int *indx); 
void zgthrz(int *nz,MKL_Complex16 *y,MKL_Complex16 *x,int *indx); 
void zrotg(MKL_Complex16 *a,MKL_Complex16 *b,double *c,MKL_Complex16 *s); 
void zscal(int *n,MKL_Complex16 *a,MKL_Complex16 *x,int *incx); 
void zsctr(int *nz,MKL_Complex16 *x,int *indx,MKL_Complex16 *y); 
void zswap(int *n,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy); 
int  izamax(int *n,MKL_Complex16 *x,int *incx); 
int  izamin(int *n,MKL_Complex16 *x,int *incx); 

/* blas level2 */

void sgbmv(char *trans,int *m,int *n,int *kl,int *ku,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void sgemv(char *trans,int *m,int *n,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void sger(int *m,int *n,float *alpha,float *x,int *incx,float *y,int *incy,float *a,int *lda);
void ssbmv(char *uplo,int *n,int *k,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void sspmv(char *uplo,int *n,float *alpha,float *ap,float *x,int *incx,float *beta,float *y,int *incy);
void sspr(char *uplo,int *n,float *alpha,float *x,int *incx,float *ap);
void sspr2(char *uplo,int *n,float *alpha,float *x,int *incx,float *y,int *incy,float *ap);
void ssymv(char *uplo,int *n,float *alpha,float *a,int *lda,float *x,int *incx,float *beta,float *y,int *incy);
void ssyr(char *uplo,int *n,float *alpha,float *x,int *incx,float *a,int *lda);
void ssyr2(char *uplo,int *n,float *alpha,float *x,int *incx,float *y,int *incy,float *a,int *lda);
void stbmv(char *uplo,char *trans,char *diag,int *n,int *k,float *a,int *lda,float *x,int *incx);
void stbsv(char *uplo,char *trans,char *diag,int *n,int *k,float *a,int *lda,float *x,int *incx);
void stpmv(char *uplo,char *trans,char *diag,int *n,float *ap,float *x,int *incx);
void stpsv(char *uplo,char *trans,char *diag,int *n,float *ap,float *x,int *incx);
void strmv(char *uplo,char *transa,char *diag,int *n,float *a,int *lda,float *b,int *incx);
void strsv(char *uplo,char *trans,char *diag,int *n,float *a,int *lda,float *x,int *incx);

void cgbmv(char *trans,int *m,int *n,int *kl,int *ku,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void cgemv(char *trans,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void cgerc(int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *a,int *lda); 
void cgeru(int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *a,int *lda); 
void chbmv(char *uplo,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void chemv(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void cher(char *uplo,int *n,float *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *a,int *lda); 
void cher2(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *a,int *lda); 
void chpmv(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *ap,MKL_Complex8 *x,int *incx,MKL_Complex8 *beta,MKL_Complex8 *y,int *incy); 
void chpr(char *uplo,int *n,float *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *ap); 
void chpr2(char *uplo,int *n,MKL_Complex8 *alpha,MKL_Complex8 *x,int *incx,MKL_Complex8 *y,int *incy,MKL_Complex8 *ap); 
void ctbmv(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx); 
void ctbsv(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx); 
void ctpmv(char *uplo,char *trans,char *diag,int *n,MKL_Complex8 *ap,MKL_Complex8 *x,int *incx); 
void ctpsv(char *uplo,char *trans,char *diag,int *n,MKL_Complex8 *ap,MKL_Complex8 *x,int *incx); 
void ctrmv(char *uplo,char *transa,char *diag,int *n,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *incx); 
void ctrsv(char *uplo,char *trans,char *diag,int *n,MKL_Complex8 *a,int *lda,MKL_Complex8 *x,int *incx); 

void dgbmv(char *trans,int *m,int *n,int *kl,int *ku,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void dgemv(char *trans,int *m,int *n,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void dger(int *m,int *n,double *alpha,double *x,int *incx,double *y,int *incy,double *a,int *lda);
void dsbmv(char *uplo,int *n,int *k,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void dspmv(char *uplo,int *n,double *alpha,double *ap,double *x,int *incx,double *beta,double *y,int *incy);
void dspr(char *uplo,int *n,double *alpha,double *x,int *incx,double *ap);
void dspr2(char *uplo,int *n,double *alpha,double *x,int *incx,double *y,int *incy,double *ap);
void dsymv(char *uplo,int *n,double *alpha,double *a,int *lda,double *x,int *incx,double *beta,double *y,int *incy);
void dsyr(char *uplo,int *n,double *alpha,double *x,int *incx,double *a,int *lda);
void dsyr2(char *uplo,int *n,double *alpha,double *x,int *incx,double *y,int *incy,double *a,int *lda);
void dtbmv(char *uplo,char *trans,char *diag,int *n,int *k,double *a,int *lda,double *x,int *incx);
void dtbsv(char *uplo,char *trans,char *diag,int *n,int *k,double *a,int *lda,double *x,int *incx);
void dtpmv(char *uplo,char *trans,char *diag,int *n,double *ap,double *x,int *incx);
void dtpsv(char *uplo,char *trans,char *diag,int *n,double *ap,double *x,int *incx);
void dtrmv(char *uplo,char *transa,char *diag,int *n,double *a,int *lda,double *b,int *incx);
void dtrsv(char *uplo,char *trans,char *diag,int *n,double *a,int *lda,double *x,int *incx);

void zgbmv(char *trans,int *m,int *n,int *kl,int *ku,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void zgemv(char *trans,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void zgerc(int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *a,int *lda); 
void zgeru(int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *a,int *lda); 
void zhbmv(char *uplo,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void zhemv(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void zher(char *uplo,int *n,double *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *a,int *lda); 
void zher2(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *a,int *lda); 
void zhpmv(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *ap,MKL_Complex16 *x,int *incx,MKL_Complex16 *beta,MKL_Complex16 *y,int *incy); 
void zhpr(char *uplo,int *n,double *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *ap); 
void zhpr2(char *uplo,int *n,MKL_Complex16 *alpha,MKL_Complex16 *x,int *incx,MKL_Complex16 *y,int *incy,MKL_Complex16 *ap); 
void ztbmv(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx); 
void ztbsv(char *uplo,char *trans,char *diag,int *n,int *k,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx); 
void ztpmv(char *uplo,char *trans,char *diag,int *n,MKL_Complex16 *ap,MKL_Complex16 *x,int *incx); 
void ztpsv(char *uplo,char *trans,char *diag,int *n,MKL_Complex16 *ap,MKL_Complex16 *x,int *incx); 
void ztrmv(char *uplo,char *transa,char *diag,int *n,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *incx); 
void ztrsv(char *uplo,char *trans,char *diag,int *n,MKL_Complex16 *a,int *lda,MKL_Complex16 *x,int *incx); 

/* blas level3 */

void sgemm(char *transa,char *transb,int *m,int *n,int *k,float *alpha,float *a,int *lda,float *b,int *ldb,float *beta,float *c,int *ldc);
void ssymm(char *side,char *uplo,int *m,int *n,float *alpha,float *a,int *lda,float *b,int *ldb,float *beta,float *c,int *ldc);
void ssyr2k(char *uplo,char *trans,int *n,int *k,float *alpha,float *a,int *lda,float *b,int *ldb,float *beta,float *c,int *ldc);
void ssyrk(char *uplo,char *trans,int *n,int *k,float *alpha,float *a,int *lda,float *beta,float *c,int *ldc);
void strmm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,float *alpha,float *a,int *lda,float *b,int *ldb);
void strsm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,float *alpha,float *a,int *lda,float *b,int *ldb);

void cgemm(char *transa,char *transb,int *m,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void chemm(char *side,char *uplo,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void cher2k(char *uplo,char *trans,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,float *beta,MKL_Complex8 *c,int *ldc); 
void cherk(char *uplo,char *trans,int *n,int *k,float *alpha,MKL_Complex8 *a,int *lda,float *beta,MKL_Complex8 *c,int *ldc); 
void csymm(char *side,char *uplo,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void csyr2k(char *uplo,char *trans,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void csyrk(char *uplo,char *trans,int *n,int *k,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *beta,MKL_Complex8 *c,int *ldc); 
void ctrmm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb); 
void ctrsm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex8 *alpha,MKL_Complex8 *a,int *lda,MKL_Complex8 *b,int *ldb); 

void dgemm(char *transa,char *transb,int *m,int *n,int *k,double *alpha,double *a,int *lda,double *b,int *ldb,double *beta,double *c,int *ldc);
void dsymm(char *side,char *uplo,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb,double *beta,double *c,int *ldc);
void dsyr2k(char *uplo,char *trans,int *n,int *k,double *alpha,double *a,int *lda,double *b,int *ldb,double *beta,double *c,int *ldc);
void dsyrk(char *uplo,char *trans,int *n,int *k,double *alpha,double *a,int *lda,double *beta,double *c,int *ldc);
void dtrmm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb);
void dtrsm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb);

void zgemm(char *transa,char *transb,int *m,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void zgemm(char *transa,char *transb,int *m,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void zhemm(char *side,char *uplo,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void zher2k(char *uplo,char *trans,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,double *beta,MKL_Complex16 *c,int *ldc); 
void zherk(char *uplo,char *trans,int *n,int *k,double *alpha,MKL_Complex16 *a,int *lda,double *beta,MKL_Complex16 *c,int *ldc); 
void zsymm(char *side,char *uplo,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void zsyr2k(char *uplo,char *trans,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void zsyrk(char *uplo,char *trans,int *n,int *k,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *beta,MKL_Complex16 *c,int *ldc); 
void ztrmm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb); 
void ztrsm(char *side,char *uplo,char *transa,char *diag,int *m,int *n,MKL_Complex16 *alpha,MKL_Complex16 *a,int *lda,MKL_Complex16 *b,int *ldb); 

/* MKL support */

void MKLGetVersion(MKLVersion *ver);
void MKLGetVersionString(char * buffer, int len);

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* _MKL_BLAS_H_ */
