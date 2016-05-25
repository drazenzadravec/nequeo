/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright (C) 2001-2003 Intel Corporation. All Rights Reserved.
//
// File   : mkl_vsl_types.h
// Purpose: VSL types definition (Windows version)
*/

#ifndef __VSL_TYPES_H__
#define __VSL_TYPES_H__

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/*************************** Pointer to stream state **************************/
typedef void*	VSLStreamStatePtr;

/******************* Pointer to InitStream function for given BRNG ************/
typedef	int (*InitStreamPtr)( int method, VSLStreamStatePtr stream, int n, const unsigned int params[] );
typedef	void (*sBRngPtr)( VSLStreamStatePtr stream, int n, float r[], float a, float b );
typedef	void (*dBRngPtr)( VSLStreamStatePtr stream, int n, double r[], double a, double b );
typedef	void (*iBRngPtr)( VSLStreamStatePtr stream, int n, unsigned int r[] );

/****************************** BRNG properties *******************************/
typedef struct _VSLBRngProperties {
    int StreamStateSize;       /* Stream state size (in bytes) */
    int NSeeds;                /* Number of seeds */
    int IncludesZero;          /* Zero flag */
    int WordSize;              /* Size (in bytes) of base word */
    int NBits;                 /* Number of actually used bits */
    InitStreamPtr InitStream;  /* Pointer to InitStream func */
    sBRngPtr sBRng;            /* Pointer to S func */
    dBRngPtr dBRng;            /* Pointer to D func */
    iBRngPtr iBRng;            /* Pointer to I func */
} VSLBRngProperties;

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* __VSL_TYPES_H__ */
