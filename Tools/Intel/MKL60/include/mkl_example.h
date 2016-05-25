/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright(C) 1999-2003 Intel Corporation. All Rights Reserved.
//
// File    : mkl_examples.h
// Purpose : Intel(R) Math Kernel Library (MKL) examples interface
*/

#ifndef _MKL_EXAMPLE_H_
#define _MKL_EXAMPLE_H_

#include "mkl_types.h"
#include "mkl_cblas.h"
#include <stdio.h>

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/*
 * ===========================================
 * Prototypes for example program functions 
 * ===========================================
 */

void GetVectorS(FILE*, int, float*, int);
void GetVectorC(FILE*, int, MKL_Complex8*, int);
void GetVectorD(FILE*, int, double*, int);
void GetVectorZ(FILE*, int, MKL_Complex16*, int);
void GetArrayS(FILE*, CBLAS_ORDER*, int, int*, int*, float*, int*);
void GetArrayD(FILE*, CBLAS_ORDER*, int, int*, int*, double*, int*);
void GetArrayC(FILE*, CBLAS_ORDER*, int, int*, int*, MKL_Complex8*, int*);
void GetArrayZ(FILE*, CBLAS_ORDER*, int, int*, int*, MKL_Complex16*, int*);
void GetBandArrayS(FILE*, CBLAS_ORDER*, int, int, int, int, float*, int); 
void GetBandArrayD(FILE*, CBLAS_ORDER*, int, int, int, int, double*, int); 
void GetBandArrayC(FILE*, CBLAS_ORDER*, int, int, int, int, MKL_Complex8*, int); 
void GetBandArrayZ(FILE*, CBLAS_ORDER*, int, int, int, int, MKL_Complex16*, int); 
void PrintTrans(int, CBLAS_TRANSPOSE*, CBLAS_TRANSPOSE*);
void PrintOrder(CBLAS_ORDER*);
void PrintVectorS(int, int, float*, int, char*);
void PrintVectorC(int, int, MKL_Complex8*, int, char*);
void PrintVectorD(int, int, double*, int, char*);
void PrintArrayS(CBLAS_ORDER*, int, int, int*, int*, float*, int*, char*);
void PrintArrayD(CBLAS_ORDER*, int, int, int*, int*, double*, int*, char*);
void PrintArrayC(CBLAS_ORDER*, int, int, int*, int*, MKL_Complex8*, int*, char*);
void PrintArrayZ(CBLAS_ORDER*, int, int, int*, int*, MKL_Complex16*, int*, char*);
void PrintBandArrayS(CBLAS_ORDER*, int, int, int, int, int, float*, int, char*);
void PrintBandArrayD(CBLAS_ORDER*, int, int, int, int, int, double*, int, char*);
void PrintBandArrayC(CBLAS_ORDER*, int, int, int, int, int, MKL_Complex8*, int, char*);
void PrintBandArrayZ(CBLAS_ORDER*, int, int, int, int, int, MKL_Complex16*, int, char*);

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* _MKL_EXAMPLE_H_ */
