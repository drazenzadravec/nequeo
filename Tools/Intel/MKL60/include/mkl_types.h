/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright(C) 1999-2003 Intel Corporation. All Rights Reserved.
//
// File    : mkl_types.h
// Purpose : Intel(R) Math Kernel Library (MKL) types definition
*/

#ifndef _MKL_TYPES_H_
#define _MKL_TYPES_H_

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/* Complex type (single precision). */
typedef
struct _MKL_Complex8 {
    float    real;
    float    imag;
} MKL_Complex8;

/* Complex type (double precision). */
typedef
struct _MKL_Complex16 {
    double    real;
    double    imag;
} MKL_Complex16;

typedef
struct {
    int       MajorVersion;
    int       MinorVersion;
    char*     ProductStatus;
    char *    BuildDate;
    char *    Processor;
    char *    Platform;
} MKLVersion;

#ifdef __cplusplus
} 
#endif /* __cplusplus */

#endif /* _MKL_TYPES_H_ */
