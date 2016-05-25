/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright (C) 1996-2003 Intel Corporation. All Rights Reserved.
//
// File   : mkl_vml_defines.h
// Purpose: VML definitions (Windows version)
*/

#ifndef __VML_DEFINES_H__
#define __VML_DEFINES_H__

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */


/*******************************************************************************
  Errors definitions
*******************************************************************************/

/* Codes for vml?Error function call and errstatus */
#define VML_STATUS_OK                    0
#define VML_STATUS_BADSIZE              -1
#define VML_STATUS_BADMEM               -2
#define VML_STATUS_ERRDOM                1
#define VML_STATUS_SING                  2
#define VML_STATUS_OVERFLOW              3
#define VML_STATUS_UNDERFLOW             4

/* Mask for standard error handler modes separation*/
#define VML_ERRMODE_STDHANDLER_MASK       0x0f00
/* Mask for additional callback switch bit separation */
#define VML_ERRMODE_CALLBACK_MASK         0xf000


/*******************************************************************************
  Mode definitions
*******************************************************************************/

/* Defines for accuracy setting LA or HA */
#define VML_XX                          0x000
#define VML_LA                          0x001
#define VML_21                          0x001
#define VML_50                          0x001
#define VML_HA                          0x002
#define VML_11                          0x004

/* Defines for precision setting mode (to set or not to set :0)) */
#define VML_DEFAULT_PRECISION           0x000
#define VML_FLOAT_CONSISTENT            0x010
#define VML_DOUBLE_CONSISTENT           0x020
#define VML_RESTORE                     0x030

/* Defines for error modes  */
#define VML_ERRMODE_IGNORE              0x0100
#define VML_ERRMODE_ERRNO               0x0200
#define VML_ERRMODE_STDERR              0x0400
#define VML_ERRMODE_EXCEPT              0x0800
#define VML_ERRMODE_CALLBACK            0x1000
#define VML_ERRMODE_DEFAULT             VML_ERRMODE_ERRNO | VML_ERRMODE_CALLBACK | VML_ERRMODE_EXCEPT

/* Mask for accuracy setting separation */
#define VML_ACCURACY_MASK               0x000f
/* Mask for precision setting separation */
#define VML_FPUMODE_MASK                0x00f0
/* Mask for error mode separation */
#define VML_ERRMODE_MASK                0xff00


#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* __VML_DEFINES_H__ */
