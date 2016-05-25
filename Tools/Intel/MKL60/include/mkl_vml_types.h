/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright (C) 1999-2003 Intel Corporation. All Rights Reserved.
//
// File   : mkl_vml_types.h
// Purpose: VML types definition (Windows version)
*/

#ifndef __VML_TYPES_H__
#define __VML_TYPES_H__

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */


/*******************************************************************************
  Math types
*******************************************************************************/

/* Complex type (single precision). */
typedef
struct _vml_sComplex_t {
    float    re;
    float    im;
} vml_sComplex_t;


/* Complex type (double precision). */
typedef
struct _vml_dComplex_t {
    double    re;
    double    im;
} vml_dComplex_t;


/*******************************************************************************
  Errors types
*******************************************************************************/

/* Type for error context structure (parameter for additional callback) */
typedef struct _DefVmlErrorContext
{ 
    int iCode; 
    int iIndex;
    double dbA1;
    double dbA2;
    double dbR1;
    double dbR2;
    char cFuncName[64];
    int  iFuncNameLen;
} DefVmlErrorContext;

/* Additional callback error handler function type */
typedef int (*VMLErrorCallBack) (DefVmlErrorContext* pdefVmlErrorContext);


#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* __VML_TYPES_H__ */
