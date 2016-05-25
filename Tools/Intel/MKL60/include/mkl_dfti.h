/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright (c) 2003 Intel Corporation. All Rights Reserved.
//
// File    : mkl_dfti.h
// Purpose : Intel(R) Math Kernel Library (MKL) interface for DFTI routines
*/

#ifndef _MKL_DFTI_H_
#define _MKL_DFTI_H_

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/* return error values: */
#define DFTI_NO_ERROR                    0
#define DFTI_MEMORY_ERROR                1
#define DFTI_INVALID_CONFIGURATION       2
#define DFTI_INCONSISTENT_CONFIGURATION  3
#define DFTI_MULTITHREADED_ERROR         4
#define DFTI_BAD_DESCRIPTOR              5
#define DFTI_UNIMPLEMENTED               6

#define DFTI_MAX_MESSAGE_LENGTH  40  /*maximum LENGTH of the error string */
#define DFTI_MAX_NAME_LENGTH  10     /*maximum LENGTH of the desc user name */
#define DFTI_VERSION_LENGTH  100     /*maximum LENGTH of the MKL version */
#define DFTI_ERROR_CLASS  60         /*number of error class */

/* derived types: */
#define DFT_Intp  int

/* DFTI_Options_Name for DFTI_Change and DFTI_Inquire funtions */
enum DFTI_CONFIG_PARAM {
 DFTI_FORWARD_DOMAIN = 0,  /* Domain for forward transform, no default */
 DFTI_DIMENSION      = 1,  /* Dimension, no default */
 DFTI_LENGTHS        = 2,  /* length(s) of transform, no default */
 DFTI_PRECISION      = 3,  /* Precision of computation, no default */
 DFTI_FORWARD_SCALE  = 4,  /* Scale factor for forward transform, default = 1.0 */
 DFTI_BACKWARD_SCALE = 5,  /* Scale factor for backward transform, default = 1.0 */
 DFTI_FORWARD_SIGN   = 6,  /* Default for forward transform = DFTI_NEGATIVE  */
 DFTI_NUMBER_OF_TRANSFORMS = 7,   /* Number of data sets to be transformed, default = 1 */
 DFTI_COMPLEX_STORAGE = 8, /* Representation for complex domain, default = DFTI_COMPLEX_COMPLEX */
 DFTI_REAL_STORAGE           = 9, /* Rep. for real domain, default = DFTI_REAL_REAL */
 DFTI_CONJUGATE_EVEN_STORAGE = 10, /* Rep. for conjugate even domain, default = DFTI_COMPLEX_REAL */
 DFTI_PLACEMENT      = 11,    /* Placement of result, default = DFTI_INPLACE */
 DFTI_INPUT_STRIDES  = 12,    /* Stride information of input data, default = tigthly */
 DFTI_OUTPUT_STRIDES = 13,    /* Stride information of output data, default = tigthly */
 DFTI_INPUT_DISTANCE  = 14,   /* Stride information of input data, default = 0 */
 DFTI_OUTPUT_DISTANCE = 15,   /* Stride information of output data, default = 0 */
 DFTI_INITIALIZATION_EFFORT = 16,    /* Effort spent in initialization, default = DFTI_MEDIUM */
 DFTI_WORKSPACE   = 17,    /* Use of workspace during computation, default = DFTI_ALLOW */
 DFTI_ORDERING    = 18,    /* Possible out of order computation, default = DFTI_ORDERED */
 DFTI_TRANSPOSE   = 19,    /* Possible transposition of result, default = DFTI_NONE */
 DFTI_DESCRIPTOR_NAME = 20,  /* name of descriptor, default = string of zero length */
 DFTI_PACKED_FORMAT = 21,    /* name of descriptor, default = string of zero length */
/* below for get_value functions only */
 DFTI_COMMIT_STATUS     = 22,     /* Whether descriptor has been commited */
 DFTI_VERSION           = 23,     /* DFTI implementation version number */
 DFTI_FORWARD_ORDERING  = 24,     /* The ordering of forward transform */
 DFTI_BACKWARD_ORDERING = 25      /* The ordering of backward transform */
};

/*DFTI_Options_Val */
enum DFTI_CONFIG_VALUE {
 DFTI_COMMITTED     = 30,    /* status - commit */
 DFTI_UNCOMMITTED   = 31,    /* status - uncommit */
 DFTI_COMPLEX      = 32,    /* General domain */
 DFTI_REAL         = 33,    /* Real domain */
 DFTI_CONJUGATE_EVEN = 34,  /* Conjugate even domain */
 DFTI_SINGLE       = 35,    /* Single precision */
 DFTI_DOUBLE       = 36,    /* Double precision */
 DFTI_NEGATIVE     = 37,    /* -i, for setting definition of transform */
 DFTI_POSITIVE     = 38,    /* +i, for setting definition of transform */
 DFTI_COMPLEX_COMPLEX = 39, /* Representation method for domain */
 DFTI_COMPLEX_REAL   = 40, /* Representation method for domain */
 DFTI_REAL_COMPLEX   = 41, /* Representation method for domain */
 DFTI_REAL_REAL    = 42,   /* Representation method for domain */
 DFTI_INPLACE      = 43,   /* Result overwrites input */
 DFTI_NOT_INPLACE  = 44,   /* Result placed differently than input */
 DFTI_LOW          = 45,   /* A low setting */
 DFTI_MEDIUM       = 46,   /* A medium setting */
 DFTI_HIGH         = 47,   /* A high setting */
 DFTI_ORDERED      = 48,   /* Data on forward and backward domain ordered */
 DFTI_BACKWARD_SCRAMBLED  = 49,   /* Data on forward ordered and backward domain scrambled */
 DFTI_FORWARD_SCRAMBLED   = 50,   /* Data on forward scrambled and backward domain ordered */
 DFTI_ALLOW        = 51,   /* Allow certain request or usage */
 DFTI_AVOID        = 52,   /* Avoid certain request or usage */
 DFTI_NONE         = 53,   /* none certain request or usage */
 DFTI_CCS_FORMAT   = 54,   /* ccs format for real DFT */
 DFTI_PACK_FORMAT  = 55,   /* pack format for real DFT */
 DFTI_PERM_FORMAT  = 56    /* perm format for real DFT */
};

typedef struct DFTI_DFT_Desc_struct DFTI_Descriptor_struct;
#define DFTI_Descriptor DFTI_Descriptor_struct

#define DFTI_DESCRIPTOR DFTI_Descriptor
#define DFTI_DESCRIPTOR_HANDLE DFTI_DESCRIPTOR*

long DftiCreateDescriptor(DFTI_DESCRIPTOR_HANDLE*, enum DFTI_CONFIG_VALUE,
						  enum DFTI_CONFIG_VALUE, long, ...);
long DftiCommitDescriptor(DFTI_DESCRIPTOR_HANDLE);
long DftiComputeForward(DFTI_DESCRIPTOR_HANDLE, void*, ...);
long DftiComputeBackward(DFTI_DESCRIPTOR_HANDLE, void*, ...);
long DftiSetValue(DFTI_DESCRIPTOR_HANDLE, enum DFTI_CONFIG_PARAM, ...);
long DftiGetValue(DFTI_DESCRIPTOR_HANDLE, enum DFTI_CONFIG_PARAM, ...);
long DftiCopyDescriptor(DFTI_DESCRIPTOR_HANDLE, DFTI_DESCRIPTOR_HANDLE*);
long DftiFreeDescriptor(DFTI_DESCRIPTOR_HANDLE*);
char* DftiErrorMessage(long);
long DftiErrorClass(long, long);

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* _MKL_DFTI_H_ */
